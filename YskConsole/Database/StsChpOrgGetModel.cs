using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YskConsole.Models;

namespace YskConsole.Database
{
    public class StsChpOrgGetModel
    {
        public LocationUrlCrawler locationUrlCrawler { get; set; }
        public CumgurbaskanligiSecimBilgileri cumgurbaskanligiSecimBilgileri { get; set; }
        public MilletVekiliSecimBilgileri milletVekiliSecimBilgileri { get; set; }
    }
}
