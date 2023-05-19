using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YskConsole.Database;
using YskConsole.Database.Repository;
using YskConsole.Models;
using YskConsole.Utility;

namespace YskConsole.Manager
{
    public class Manager
    {
        LocationUrlCrawlerRepository locationUrlCrawlerRepository;
        MilletVekiliSecimBilgileriRepository milletVekiliSecimBilgileriRepository;
        CumgurbaskanligiSecimBilgileriRepository cumgurbaskanligiSecimBilgileriRepository;
        StsChpOrgCrawler stsChpOrgCrawler;
        OyveItesiOylarRepository oyveItesiOylarRepository;
        sqlRepository sqlRepository;
        public Manager()
        {
            locationUrlCrawlerRepository = new LocationUrlCrawlerRepository();
            milletVekiliSecimBilgileriRepository = new MilletVekiliSecimBilgileriRepository();
            cumgurbaskanligiSecimBilgileriRepository = new CumgurbaskanligiSecimBilgileriRepository();
            stsChpOrgCrawler = new StsChpOrgCrawler();
            oyveItesiOylarRepository = new OyveItesiOylarRepository();

            sqlRepository = new sqlRepository();
        }

        public async Task GetAllCurrentDatas()
        {

            var oyveotesidatas = await oyveItesiOylarRepository.GetAll();


            var datas = new List<ResultCompareModel>();

            foreach (var items in oyveotesidatas.Partition(100))
            {
                var lstResults = new ConcurrentBag<StsChpOrgGetModel>();
                foreach (var item in items)
                {
                    var datasysk = await cumgurbaskanligiSecimBilgileriRepository.GetFirstIlIlceSandik(item.CityName, item.DistrictName, item.ballot_box_number.ToString());
                    var dataMvsysk = await milletVekiliSecimBilgileriRepository.GetFirstIlIlceSandik(item.CityName, item.DistrictName, item.ballot_box_number.ToString());

                    if (datasysk == null)
                    {
                        continue;
                    }
                    var mastersw = Stopwatch.StartNew();

                    var result = await stsChpOrgCrawler.Get(datasysk.TcKimlikNo);
                    lstResults.Add(result);

                    if(result.milletVekiliSecimBilgileri.txtMhp != dataMvsysk.txtMhp ||
                        result.milletVekiliSecimBilgileri.txtCHP != dataMvsysk.txtCHP ||
                        result.milletVekiliSecimBilgileri.txtIyi != dataMvsysk.txtIyi ||
                        result.milletVekiliSecimBilgileri.txtAkp != dataMvsysk.txtAkp ||
                        result.milletVekiliSecimBilgileri.txtYesilSol != dataMvsysk.txtYesilSol )
                    {
                        Console.WriteLine("değişmiş 1 ");
                    }


                    if (result.cumgurbaskanligiSecimBilgileri.txtCB1 != datasysk.txtCB1 ||
                        result.cumgurbaskanligiSecimBilgileri.txtCB2 != datasysk.txtCB2 ||
                        result.cumgurbaskanligiSecimBilgileri.txtCB3 != datasysk.txtCB3 ||
                        result.cumgurbaskanligiSecimBilgileri.txtCB4 != datasysk.txtCB4)
                    {
                        Console.WriteLine("değişmiş  2");
                    }

                    mastersw.Stop();

                }

                await SaveDatas(lstResults);


            }

        }

        public string TurkishCharacterToEnglish(string text)
        {
            char[] turkishChars = { 'ı', 'ğ', 'İ', 'Ğ', 'ç', 'Ç', 'ş', 'Ş', 'ö', 'Ö', 'ü', 'Ü' };
            char[] englishChars = { 'i', 'g', 'I', 'G', 'c', 'C', 's', 'S', 'o', 'O', 'u', 'U' };

            // Match chars
            for (int i = 0; i < turkishChars.Length; i++)
                text = text.Replace(turkishChars[i], englishChars[i]);

            return text;
        }
        public async Task GetAllFindBallotBox()
        {
            var oyveotesidatas = await oyveItesiOylarRepository.GetAll();

            oyveotesidatas = oyveotesidatas.Where(x => x.TcKimlikNo == null).OrderBy(x=> x.CityName).ThenBy(x=>x.DistrictName).ToList();

            await Parallel.ForEachAsync(oyveotesidatas, new ParallelOptions() { MaxDegreeOfParallelism = 50 }, async (item, c) =>
            {

                Console.WriteLine($"{oyveotesidatas.Count} {oyveotesidatas.IndexOf(item)}  {item.CityName} {item.DistrictName} {item.NeighborhoodName} {item.ballot_box_number}");
                var datas = await sqlRepository.Get(TurkishCharacterToEnglish(item.CityName), TurkishCharacterToEnglish(item.DistrictName), TurkishCharacterToEnglish(item.NeighborhoodName), TurkishCharacterToEnglish(item.NeighborhoodName).Replace("MAH.", "KOYU"));
                var tcList = datas.Select(x => x.Field_1.ToString());
                if (tcList.Count() > 20)
                {
                    tcList = tcList.Take(20);
                }

                var lstResults = new ConcurrentBag<StsChpOrgGetModel>();
                await Parallel.ForEachAsync(tcList, new ParallelOptions() { MaxDegreeOfParallelism = 20 }, async (item, c) =>
                {

                    var result = await stsChpOrgCrawler.Get(item);
                    lstResults.Add(result);

                });
                await SaveDatas(lstResults);

                foreach (var res in lstResults)
                {
                    if (res.milletVekiliSecimBilgileri != null && item.ballot_box_number.ToString() == res.milletVekiliSecimBilgileri.SandikNo)
                    {

                        Console.WriteLine($"{oyveotesidatas.Count} {oyveotesidatas.IndexOf(item)}  {item.CityName} {item.DistrictName} {item.NeighborhoodName} {item.ballot_box_number} bulundu");  
                        item.TcKimlikNo = res.milletVekiliSecimBilgileri.TcKimlikNo;
                        await oyveItesiOylarRepository.UpdateBulkTc(new List<OyveItesiOylar>() { item });
                        break;
                    }
                }
            });



        }
        public async Task GetAll()
        {
            var openfile = File.OpenText("D:\\data\\data_dump-002.sql");

            var linecount = 49611709;
            var partitionCount = 5000;
            Console.WriteLine($"{linecount / partitionCount}");

            var skippages = int.Parse(Console.ReadLine());

            for (int i = 0; i < partitionCount * skippages; i++)
            {
                var line = await openfile.ReadLineAsync();

            }

            for (int i = 0; i < (int)Math.Ceiling((decimal)linecount / partitionCount); i++)
            {
                var tcList = await TcListPartition(openfile, partitionCount);


                var lstResults = new ConcurrentBag<StsChpOrgGetModel>();
                var mastersw = Stopwatch.StartNew();
                await Parallel.ForEachAsync(tcList, new ParallelOptions() { MaxDegreeOfParallelism = 100 }, async (item, c) =>
                {

                    var result = await stsChpOrgCrawler.Get(item);
                    lstResults.Add(result);

                });
                mastersw.Stop();
                Console.WriteLine($"{linecount} {i} {mastersw.Elapsed}");

                await SaveDatas(lstResults);
            }

        }

        private async Task SaveDatas(ConcurrentBag<StsChpOrgGetModel> lstResults)
        {
            Console.WriteLine($"Hatalı : {lstResults.Count(x => x == null) + lstResults.Where(x => x != null).Select(x => x.locationUrlCrawler).Count(x => x.Hatali)} Normal : {lstResults.Where(x => x != null).Select(x => x.locationUrlCrawler).Count(x => !x.Hatali)}");
            await locationUrlCrawlerRepository.Add(lstResults.Where(x => x.locationUrlCrawler != null).Select(x => x.locationUrlCrawler));
            await milletVekiliSecimBilgileriRepository.Add(lstResults.Where(x => x.milletVekiliSecimBilgileri != null).Select(x => x.milletVekiliSecimBilgileri));
            await cumgurbaskanligiSecimBilgileriRepository.Add(lstResults.Where(x => x.cumgurbaskanligiSecimBilgileri != null).Select(x => x.cumgurbaskanligiSecimBilgileri));
        }

        private async Task<List<string>> CheckTcList(List<string> tcs)
        {
            var savedTcList = await locationUrlCrawlerRepository.GetTcs(tcs);

            foreach (var item in savedTcList)
            {
                tcs.Remove(item.TcKimlikNo);
            }
            return tcs;
        }

        private async Task<List<string>> TcListPartition(StreamReader reader, int partitionCount)
        {
            var tcList = new List<string>();
            for (int l = 0; l < partitionCount; l++)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line))
                    continue;

                var linesplit = line.Split('	');

                if (linesplit.Length > 1)
                    tcList.Add(linesplit[1]);

            }
            return await CheckTcList(tcList);

        }
    }
}
