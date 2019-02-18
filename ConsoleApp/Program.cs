namespace ConsoleApp
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using InfoBlox.Automation;
    using InfoBlox.Automation.Model;
    using Newtonsoft.Json;
    class Program
    {

        private static Helper infoBloxHelper = Helper.Instance;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var nets = RetrieveNetworks().Result;
            Console.WriteLine(JsonConvert.SerializeObject(nets));
            Console.ReadLine();

            //RetrieveIP().Wait();
            var _ip = Task<string>.Run(action: () => { RetrieveIP().Wait(); });
            Console.WriteLine(_ip);
            Console.ReadLine();

            AddNewRecord();
            Console.ReadLine();

            //RetrieveIP(5).Wait();
        }
        public static async Task<List<InfobloxNetwork>> RetrieveNetworks()
        {
            List<InfobloxNetwork> _lstNetworks = await infoBloxHelper.GetNetworkListsAsync();

            return (_lstNetworks);
        }


        public static async Task<string> RetrieveIP()
        {
            var _ipResult = await infoBloxHelper.GetIPAsync(5);
            return (_ipResult.ToJson());
        }


        public static async Task<string> AddNewRecord()
        {
            var _IpHostRecord = await infoBloxHelper.CreateHostRecordAsync("newdemo-antonio.kpmg.msft.cloud");
            return (_IpHostRecord);
        }
    }
}
