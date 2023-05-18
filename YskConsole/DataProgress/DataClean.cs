using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YskConsole.Database;
using YskConsole.Database.Repository;
using YskConsole.Models;

namespace YskConsole.DataProgress
{
    public class DataClean
    {
        public DataClean()
        {
            oyveItesiOylarRepository = new OyveItesiOylarRepository();
            oyveOtesiNeighborhoodsRepository = new OyveOtesiNeighborhoodsRepository();
            oyveOtesiDistrictsRepository = new OyveOtesiDistrictsRepository();
            OyveOtesiCities = new OyveOtesiCitiesRepository();
        }

        OyveItesiOylarRepository oyveItesiOylarRepository;
        OyveOtesiNeighborhoodsRepository oyveOtesiNeighborhoodsRepository;
        OyveOtesiDistrictsRepository oyveOtesiDistrictsRepository;
        OyveOtesiCitiesRepository OyveOtesiCities;

        public async Task OyveOtesiIlIlceNameUpdate()
        {
            var datas = await oyveItesiOylarRepository.GetAll();

            var oyveOtesiNeighborhoodsdata = await oyveOtesiNeighborhoodsRepository.GetAll();
            var oyveOtesiDistrictsdata = await oyveOtesiDistrictsRepository.GetAll();
            var oyveOtesiCitiesdata = await OyveOtesiCities.GetAll();   

            foreach (var item in datas)
            {
                var Neighborhood = oyveOtesiNeighborhoodsdata.AsParallel().SingleOrDefault(x => x.OyveOtesiId == item.NeighborhoodId);
                var District = oyveOtesiDistrictsdata.AsParallel().SingleOrDefault(x => x.OyveOtesiId == item.DistrictId);
                var city = oyveOtesiCitiesdata.AsParallel().SingleOrDefault(x => x.OyveOtesiId == item.CityId);

                item.CityName = city.name;
                item.DistrictName = District.name;
                item.NeighborhoodName = Neighborhood.name;

            }
            await oyveItesiOylarRepository.UpdateBulkCityDistrict(datas);
        }
        public async Task OyveOtesiIdUpdate()
        {
            var datas = await oyveItesiOylarRepository.GetAll();

            var groupdatasupdate = datas.GroupBy(x => new { x.CityId, x.DistrictId, x.NeighborhoodId }).Select(x => x.First()).ToList();

            var oyveOtesiNeighborhoodsdata = await oyveOtesiNeighborhoodsRepository.GetAll();
            var oyveOtesiDistrictsdata = await oyveOtesiDistrictsRepository.GetAll();

            foreach (var item in groupdatasupdate)
            {
                var Neighborhood = oyveOtesiNeighborhoodsdata.AsParallel().SingleOrDefault(x => x.OyveOtesiId == item.NeighborhoodId);
                Neighborhood.CityId = item.CityId;
                Neighborhood.DistrictId = item.DistrictId;

                var District = oyveOtesiDistrictsdata.AsParallel().SingleOrDefault(x => x.OyveOtesiId == item.DistrictId);
                District.CityId = item.CityId;

            }
            await oyveOtesiNeighborhoodsRepository.UpdateBulkCityDistrict(oyveOtesiNeighborhoodsdata);
            await oyveOtesiDistrictsRepository.UpdateBulkCityDistrict(oyveOtesiDistrictsdata);
        }

        public async Task DuplicateCleanerCityandNeighborhoods()
        {
            
            var oyveOtesiNeighborhoodsdata = await oyveOtesiNeighborhoodsRepository.GetAll();


            var groupNeighborhoods = oyveOtesiNeighborhoodsdata.GroupBy(x=>x.OyveOtesiId).Where(x=>x.Count()>1).ToList();

            foreach (var neighborhoods in groupNeighborhoods)
            {
                var neighborhoodsgroupdata = neighborhoods.ToList();
                for (int i = 1; i < neighborhoods.Count(); i++)
                {
                    await oyveOtesiNeighborhoodsRepository.Delete(neighborhoodsgroupdata[i].Id);
                }
            }

            var oyveOtesiDistrictsdata = await oyveOtesiDistrictsRepository.GetAll();


            var group = oyveOtesiDistrictsdata.GroupBy(x => x.OyveOtesiId).Where(x => x.Count() > 1).ToList();

            foreach (var Districts in group)
            {
                var Districtsgroupdata = Districts.ToList();
                for (int i = 1; i < Districts.Count(); i++)
                {
                    await oyveOtesiDistrictsRepository.Delete(Districtsgroupdata[i].Id);
                }
            }
        }

        public async Task DuplicateCleanerOylar()
        {
            var datas = await oyveItesiOylarRepository.GetAll();

            var groupdatas = datas.GroupBy(x => new { x.CityId, x.DistrictId, x.NeighborhoodId, x.school_name, x.ballot_box_number }).Where(x=>x.Count()>1).ToList();

            foreach (var data in groupdatas) { 
            
                var select = data.FirstOrDefault(x=>(x.mv_result != null && x.cm_result != null) || (x.mv_result != null || x.cm_result != null) || (x.mv_result == null || x.cm_result == null));

                if (select != null)
                {
                    foreach (var item in data)
                    {
                        if (select == item)
                            continue;
                        await oyveItesiOylarRepository.Delete(item.Id);
                    }
                }

            }

        }

    }
}
