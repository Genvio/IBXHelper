namespace IBX.Console
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

        private static Helper ibxHelper = Helper.Instance;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello InfoBlox Helper Console!");

            Console.WriteLine(Helper.GetVersion());
            var nets = RetrieveNetworks().Result;
            Console.WriteLine(JsonConvert.SerializeObject(nets));
            Console.ReadLine();

            //RetrieveIP().Wait();
            var _ip = RetrieveIP().Result;
            Console.WriteLine(_ip);
            Console.ReadLine();

            //Write a Record.
            HostRecord newRecord = AddNewRecord().Result;


            Console.WriteLine(JsonConvert.SerializeObject(newRecord));
            Console.ReadLine();

            Console.WriteLine(JsonConvert.SerializeObject(GetHostRecord(), new JsonSerializerSettings()
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            }));
            Console.ReadLine();

            //RetrieveIP(5).Wait();
        }
        public static async Task<List<InfobloxNetwork>> RetrieveNetworks()
        {
            List<InfobloxNetwork> _lstNetworks = await ibxHelper.GetNetworkListsAsync();

            return (_lstNetworks);
        }

        public static async Task<string> RetrieveIP()
        {
            var _ipResult = await ibxHelper.GetIPAsync(5);
            return (_ipResult.ToJson());
        }


        public static async Task<HostRecord> AddNewRecord()
        {
            var _IpHostRecord = await ibxHelper.CreateHostRecordAsync("newdemo1.kpmg.azure.cloud");
            return (_IpHostRecord);
        }

        public static async Task<HostRecord> GetHostRecord()
        {
            var _IpHostRecord = await ibxHelper.GetHostRecordAsync("newdemo1.kpmg.azure.cloud");
            return (_IpHostRecord);
        }
    }
}
