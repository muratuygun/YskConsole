using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YskConsole.Database;
using YskConsole.Database.Repository;
using YskConsole.Models;

namespace YskConsole.Manager
{
    public class ManagerOyveOtesi
    {
        public ManagerOyveOtesi()
        {
            oyveItesiOylarRepository = new OyveItesiOylarRepository();
            oyveOtesiNeighborhoodsRepository = new OyveOtesiNeighborhoodsRepository();
            oyveOtesiDistrictsRepository = new OyveOtesiDistrictsRepository();
            OyveOtesiCities = new OyveOtesiCitiesRepository();
            oyveOtesiCrawler = new OyveOtesiCrawler();
        }

        OyveItesiOylarRepository oyveItesiOylarRepository;
        OyveOtesiNeighborhoodsRepository oyveOtesiNeighborhoodsRepository;
        OyveOtesiDistrictsRepository oyveOtesiDistrictsRepository;
        OyveOtesiCitiesRepository OyveOtesiCities;
        OyveOtesiCrawler oyveOtesiCrawler;


        public async Task GetAll()
        {
            var cities = await OyveOtesiCities.GetAll();


            foreach (var city in cities)
            {
                var Districts = await oyveOtesiDistrictsRepository.GetAll(city.OyveOtesiId);
                if (!Districts.Any())
                {
                    Districts = await oyveOtesiCrawler.GetDistricts(city.OyveOtesiId.ToString());
                    await oyveOtesiDistrictsRepository.Add(Districts);
                }

                Console.WriteLine(city.name);
                foreach (var District in Districts)
                {
                    Console.WriteLine($"{cities.Count}/{cities.IndexOf(city)} {city.name} / {District.name}");
                    var Neighborhoods = await oyveOtesiNeighborhoodsRepository.GetAll(city.OyveOtesiId, District.OyveOtesiId);
                    if (!Neighborhoods.Any())
                    {
                        Neighborhoods = await oyveOtesiCrawler.GetNeighborhoods(city.OyveOtesiId.ToString(), District.OyveOtesiId.ToString());
                        await oyveOtesiNeighborhoodsRepository.Add(Neighborhoods);
                    }

                    var mevcutOylar = await oyveItesiOylarRepository.GetAll(Neighborhoods.Select(x => x.OyveOtesiId).ToList());

                    foreach (var item in mevcutOylar)
                    {
                        var removeitem = Neighborhoods.SingleOrDefault(x => x.OyveOtesiId == item.NeighborhoodId);
                        if (removeitem != null)
                            Neighborhoods.Remove(removeitem);
                    }

                    var sw = Stopwatch.StartNew();
                    var oylarrlist = new ConcurrentBag<List<OyveItesiOylar>>();
                    await Parallel.ForEachAsync(Neighborhoods, new ParallelOptions() { MaxDegreeOfParallelism = 100 }, async (Neighborhood, y) =>
                    {

                        var oylarr = await oyveOtesiCrawler.GetOylarrr(Neighborhood.OyveOtesiId.ToString());

                        if (oylarr == null)
                            return;
                        foreach (var item in oylarr)
                        {
                            item.NeighborhoodId = Neighborhood.OyveOtesiId;
                            item.CityId = city.OyveOtesiId;
                            item.DistrictId = District.OyveOtesiId;

                        }

                        oylarrlist.Add(oylarr);
                    });
                    sw.Stop();
                    Console.WriteLine($"{sw.Elapsed}");
                    await oyveItesiOylarRepository.Add(oylarrlist.Where(x => x != null).SelectMany(x => x).ToList());
                }

            }

        }

    }
}
