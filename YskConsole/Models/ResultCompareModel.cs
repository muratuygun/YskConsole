using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YskConsole.Database;

namespace YskConsole.Models
{

    public class ResultCompareModel : MongoDbIdModel
    {
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string Mahalle { get; set; }
        public string Okul { get; set; }
        public int SandikNo { get; set; }

        public bool Hatali { get; set; } = false;
        public bool MvVerilerGunIcındeDegismis { get; set; } = false;
        public bool CmVerilerGunIcındeDegismis { get; set; } = false;

        public bool CmYskVerisiYok { get; set; } = false;
        public bool MvYskVerisiYok { get; set; } = false;
        public bool CmOyveOtesiVerisiYok { get; set; } = false;
        public bool MvOyveOtesiVerisiYok { get; set; } = false;




        public int? FarkCmVoteTayyip { get; set; }
        public int? FarkCmVoteKemal { get; set; }
        public int? FarkCmVoteSinan { get; set; }
        public int? FarkCmVoteMuharrem { get; set; }


        public int? FarkMvChp { get; set; }
        public int? FarkMvAkp { get; set; }
        public int? FarkMvIyi { get; set; }
        public int? FarkMvYsp { get; set; }
        public int? FarkMvMhp { get; set; }


        public int? OyveOtesiCmVoteTayyip { get; set; }
        public int? OyveOtesiCmVoteKemal { get; set; }
        public int? OyveOtesiCmVoteSinan { get; set; }
        public int? OyveOtesiCmVoteMuharrem { get; set; }
                   
                   
        public int? OyveOtesiMvChp { get; set; }
        public int? OyveOtesiMvAkp { get; set; }
        public int? OyveOtesiMvIyi { get; set; }
        public int? OyveOtesiMvYsp { get; set; }
        public int? OyveOtesiMvMhp { get; set; }
        public int? OyveOtesiMvOther { get; set; }
        public int? OyveOtesiMvCumhurIttifaki { get; set; }
        public int? OyveOtesiMvMilletIttifaki { get; set; }



        public int? YskCmVoteTayyip { get; set; }
        public int? YskCmVoteKemal { get; set; }
        public int? YskCmVoteSinan { get; set; }
        public int? YskCmVoteMuharrem { get; set; }
                   
                   
        public int? YskMvChp { get; set; }
        public int? YskMvAkp { get; set; }
        public int? YskMvIyi { get; set; }
        public int? YskMvYsp { get; set; }
        public int? YskMvMhp { get; set; }
        public int? YskMvOther { get; set; }
        public int? YskMvCumhurIttifaki { get; set; }
        public int? YskMvMilletIttifaki { get; set; }

    }
}
