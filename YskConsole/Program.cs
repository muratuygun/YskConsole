
using YskConsole;
using YskConsole.DataProgress;
using YskConsole.Manager;

Console.WriteLine("Başladı");

var req3 = new Manager();
await req3.GetAllFindBallotBox();
return;
var req1 = new Manager();
await req1.GetAllCurrentDatas();
return;
var dataPrepare = new DataPrepare();

await dataPrepare.Prepare();
return;

var dclean2 = new DataClean();
await dclean2.OyveOtesiTcUpdate();
return;
var dclean = new DataClean();
await dclean.OyveOtesiIlIlceNameUpdate();


return;
var req2 = new ManagerOyveOtesi();
await req2.GetAll();

//await new StsChpOrgCrawler().Get("40381891958");
return;


Console.WriteLine("Bitti");