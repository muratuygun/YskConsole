using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YskConsole.Database;
using YskConsole.Database.Repository;

namespace YskConsole.Manager
{
    public class Manager
    {
        LocationUrlCrawlerRepository locationUrlCrawlerRepository;
        MilletVekiliSecimBilgileriRepository milletVekiliSecimBilgileriRepository;
        CumgurbaskanligiSecimBilgileriRepository cumgurbaskanligiSecimBilgileriRepository;
        StsChpOrgCrawler stsChpOrgCrawler;
        public Manager()
        {
            locationUrlCrawlerRepository = new LocationUrlCrawlerRepository();
            milletVekiliSecimBilgileriRepository = new MilletVekiliSecimBilgileriRepository();
            cumgurbaskanligiSecimBilgileriRepository = new CumgurbaskanligiSecimBilgileriRepository();
            stsChpOrgCrawler = new StsChpOrgCrawler();

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
