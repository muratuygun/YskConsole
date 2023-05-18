using YskConsole.Database;

namespace YskConsole.Models
{
    public class AspNetBaseCrawler: MongoDbIdModel
    {
        public string eventTarget { get; set; }
        public string eventArgument { get; set; }
        public string lastFocus { get; set; }

        public string viewState { get; set; }
        public string viewStateGenerator { get; set; }
        public string eventValidation { get; set; }
    }
}
