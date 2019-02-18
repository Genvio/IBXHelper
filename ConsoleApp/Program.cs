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
            var _ip = Task<string>.Run(action: () => { RetrieveIP(); });
            Console.WriteLine(_ip);
            Console.ReadLine();

            //RetrieveIP(5).Wait();
        }
        public static async Task<List<InfobloxNetwork>> RetrieveNetworks()
        {
            List<InfobloxNetwork> _lstNetworks = await infoBloxHelper.RetrieveNetworkListsAsync();

            return (_lstNetworks);
        }


        public static async Task<string> RetrieveIP()
        {
            var _ipResult = await infoBloxHelper.RetrieveIPAsync(5);
            return (_ipResult.ToJson());
        }
    }
}
