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
    public class DataPrepare
    {
        public DataPrepare()
        {
            OyveItesiOylarRepository = new OyveItesiOylarRepository();
            CumgurbaskanligiSecimBilgileriRepository = new CumgurbaskanligiSecimBilgileriRepository();
            milletVekiliSecimBilgileriRepository = new MilletVekiliSecimBilgileriRepository();

            resultCompareModelRepository = new ResultCompareModelRepository();
        }

        private ResultCompareModelRepository resultCompareModelRepository;
        private MilletVekiliSecimBilgileriRepository milletVekiliSecimBilgileriRepository;
        private OyveItesiOylarRepository OyveItesiOylarRepository { get; set; }
        private CumgurbaskanligiSecimBilgileriRepository CumgurbaskanligiSecimBilgileriRepository { get; set; }

        public async Task Prepare()
        {
            var oyveotesidatas = await OyveItesiOylarRepository.GetAll();


            var datas = new List<ResultCompareModel>();

            foreach (var item in oyveotesidatas)
            {
                var datasysk = await CumgurbaskanligiSecimBilgileriRepository.GetAllIlIlceSandik(item.CityName, item.DistrictName, item.ballot_box_number.ToString());
                var dataMvsysk = await milletVekiliSecimBilgileriRepository.GetAllIlIlceSandik(item.CityName, item.DistrictName, item.ballot_box_number.ToString());

                var cmveridegismis =datasysk.AsParallel().GroupBy(x => new { x.txtCB1, x.txtCB2, x.txtCB3, x.txtCB4 }).Count();
                var mvveridegismis = dataMvsysk.AsParallel().GroupBy(x => new { x.txtAkp, x.txtCHP, x.txtMhp, x.txtIyi, x.txtYesilSol }).Count();

                var model = new ResultCompareModel();
                if (cmveridegismis > 1 || mvveridegismis > 1)
                {
                    Console.WriteLine($"{oyveotesidatas.Count} {oyveotesidatas.IndexOf(item)} {datas.Where(x => x.Hatali).Count()} {model.Il} {model.Ilce} {model.Mahalle} {model.SandikNo} Veriler Değişmiş");
                    model.CmVerilerGunIcındeDegismis = (cmveridegismis > 1);
                    model.MvVerilerGunIcındeDegismis = (mvveridegismis> 1);
                    
                    var cc11 = dataMvsysk.GroupBy(x => new { x.txtAkp, x.txtCHP, x.txtMhp, x.txtIyi, x.txtYesilSol }).ToList();
                    var cc12 = datasysk.GroupBy(x => new { x.txtCB1, x.txtCB2, x.txtCB3, x.txtCB4,  }).ToList();

                    if (cmveridegismis > 2 || mvveridegismis > 2)
                    {

                    }
                }


                model.Il = item.CityName;
                model.Ilce = item.DistrictName;
                model.Mahalle = item.NeighborhoodName;
                model.Okul = item.school_name;
                model.SandikNo = item.ballot_box_number;

                model.CmOyveOtesiVerisiYok = (item.cm_result == null);
                if (item.cm_result != null)
                {
                    model.OyveOtesiCmVoteTayyip = item.cm_result.votes._1;
                    model.OyveOtesiCmVoteMuharrem = item.cm_result.votes._2;
                    model.OyveOtesiCmVoteKemal = item.cm_result.votes._3;
                    model.OyveOtesiCmVoteSinan = item.cm_result.votes._4;
                }
                model.MvOyveOtesiVerisiYok = (item.mv_result == null);
                if (item.mv_result != null)
                {
                    model.OyveOtesiMvAkp = item.mv_result.votes._9;
                    model.OyveOtesiMvChp = item.mv_result.votes._21;
                    model.OyveOtesiMvIyi = item.mv_result.votes._22;
                    model.OyveOtesiMvYsp = item.mv_result.votes._12;
                    model.OyveOtesiMvMhp = item.mv_result.votes._11;

                    //var sum =item.mv_result.votes._1 + item.mv_result.votes._2 + item.mv_result.votes._3 + item.mv_result.votes._4+ item.mv_result.votes._5+ //item.mv_result.votes._6+
                    //         item.mv_result.votes._7 + item.mv_result.votes._8 + item.mv_result.votes._10 + item.mv_result.votes._11 + item.mv_result.votes._12 + item.mv_result.votes._13
                    //         + item.mv_result.votes._14 + item.mv_result.votes._15 + item.mv_result.votes._16 + item.mv_result.votes._17 + item.mv_result.votes._18 + item.mv_result.votes._19 + item.mv_result.votes._20
                    //         + item.mv_result.votes._21 + item.mv_result.votes._22 + item.mv_result.votes._23 + item.mv_result.votes._24;

                    //model.OyveOtesiMvOther = item.mv_result.votes._4;
                }

                var data = datasysk.FirstOrDefault();
                model.CmYskVerisiYok = (data == null);


                if (data != null)
                {
                    model.YskCmVoteTayyip = int.Parse(data.txtCB1);
                    model.YskCmVoteMuharrem = int.Parse(data.txtCB2);
                    model.YskCmVoteKemal = int.Parse(data.txtCB3);
                    model.YskCmVoteSinan = int.Parse(data.txtCB4);

                }

                var datamv = dataMvsysk.FirstOrDefault();
                model.MvYskVerisiYok = (datamv == null);



                if (datamv != null )
                {

                    model.YskMvAkp = int.Parse(datamv.txtAkp);
                    model.YskMvChp = int.Parse(datamv.txtCHP);
                    model.YskMvIyi = int.Parse(datamv.txtIyi);
                    model.YskMvYsp = int.Parse(datamv.txtYesilSol);
                    model.YskMvMhp = int.Parse(datamv.txtMhp);
                }



                if (item.cm_result != null && data != null && (model.OyveOtesiCmVoteTayyip != model.YskCmVoteTayyip ||
                model.OyveOtesiCmVoteKemal != model.YskCmVoteKemal ||
                model.OyveOtesiCmVoteSinan != model.YskCmVoteSinan ||
                model.OyveOtesiCmVoteMuharrem != model.YskCmVoteMuharrem))
                {
                    model.Hatali = true;

                    model.FarkCmVoteTayyip = model.OyveOtesiCmVoteTayyip - model.YskCmVoteTayyip;
                    model.FarkCmVoteKemal = model.OyveOtesiCmVoteKemal - model.YskCmVoteKemal;
                    model.FarkCmVoteSinan = model.OyveOtesiCmVoteSinan - model.YskCmVoteSinan;
                    model.FarkCmVoteMuharrem = model.OyveOtesiCmVoteMuharrem - model.YskCmVoteMuharrem;

                    Console.WriteLine($"{oyveotesidatas.Count} {oyveotesidatas.IndexOf(item)} {datas.Where(x => x.Hatali).Count()} {model.Il} {model.Ilce} {model.Mahalle} {model.SandikNo} Hatalı Tayyip :{model.OyveOtesiCmVoteTayyip - model.YskCmVoteTayyip} Kemal {model.OyveOtesiCmVoteKemal - model.YskCmVoteKemal} Sinan {model.OyveOtesiCmVoteSinan - model.YskCmVoteSinan} Muharrem {model.OyveOtesiCmVoteMuharrem - model.YskCmVoteMuharrem}");
                }

                if(item.mv_result != null && datamv != null &&  (model.YskMvAkp   !=   model.OyveOtesiMvAkp ||
                model.YskMvChp   !=   model.OyveOtesiMvChp ||
                model.YskMvIyi   !=   model.OyveOtesiMvIyi ||
                model.YskMvYsp   !=   model.OyveOtesiMvYsp ||
                model.YskMvMhp   !=   model.OyveOtesiMvMhp))
                {
                    model.Hatali = true;


                    model.FarkMvChp = model.YskMvChp - model.OyveOtesiMvChp;
                    model.FarkMvAkp = model.YskMvAkp - model.OyveOtesiMvAkp;
                    model.FarkMvIyi = model.YskMvIyi - model.OyveOtesiMvIyi;
                    model.FarkMvYsp = model.YskMvYsp - model.OyveOtesiMvYsp;
                    model.FarkMvMhp = model.YskMvMhp - model.OyveOtesiMvMhp;

                    Console.WriteLine($"{oyveotesidatas.Count} {oyveotesidatas.IndexOf(item)} {datas.Where(x=>x.Hatali).Count()} {model.Il} {model.Ilce} {model.Mahalle} {model.SandikNo} Hatalı MilletVekili | Akp {model.YskMvAkp - model.OyveOtesiMvAkp} Chp {model.YskMvChp - model.OyveOtesiMvChp} iyi {model.YskMvIyi - model.OyveOtesiMvIyi} Ysp {model.YskMvYsp - model.OyveOtesiMvYsp} Mhp {model.YskMvMhp - model.OyveOtesiMvMhp}");
                }



                datas.Add(model);
            }

            await resultCompareModelRepository.Add(datas);
        }

    }
}
