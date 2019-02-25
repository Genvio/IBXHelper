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
            Console.WriteLine("Welcome InfoBlox Helper Test Console");

            Console.WriteLine(Helper.GetVersion());
            var nets = RetrieveNetworks().Result;
            Console.WriteLine(JsonConvert.SerializeObject(nets));
            Console.ReadKey();

            //RetrieveIP().Wait();
            var _ip = RetrieveIP().Result;
            Console.WriteLine(_ip);
            Console.ReadKey();

            Console.Write("Please enter a name for the host to be created: -> ");
            Console.WriteLine();
            string _hostname = $"{Console.ReadLine()}.url.goes.here";

            //Write a Record.
            HostRecord _newRecord = AddNewRecord(_hostname).Result;

            Console.WriteLine(JsonConvert.SerializeObject(_newRecord));
            Console.ReadKey();

            //Read a host record.
            HostRecord _retrieveRecord = GetHostRecord(_hostname).Result;

            Console.WriteLine(JsonConvert.SerializeObject(_retrieveRecord));
            Console.ReadKey();

            //Search a host record by IP address.
            string _ipv4ToSearch = _retrieveRecord.Ipv4Addresses[0].Value;

            HostRecord _retrieveIpRecord = GetHostByIP(_ipv4ToSearch).Result;

            Console.WriteLine(JsonConvert.SerializeObject(_retrieveRecord));
            Console.ReadKey();

            //Update the host record with a new IP address.
            string _ipv4ToUpdate = ibxHelper.GetIPAsync(1).Result.IPAddresses[0];

            HostRecordPost _recordToChange = new HostRecordPost()
            {
                Name = _retrieveIpRecord.Name,
                Ipv4Addresses = new Ipv4AddressPost[] { new Ipv4AddressPost() { Value = _ipv4ToUpdate } }
            };

            HostRecord _updatedRecord = ibxHelper.UpdateHostRecordAsync(_retrieveIpRecord.Name, _recordToChange).Result;

            Console.WriteLine(JsonConvert.SerializeObject(_updatedRecord));
            Console.ReadKey();

            //Delete the host record
            bool _isRecordDeleted = ibxHelper.DeleteHostRecordAsync(_recordToChange).Result;

            Console.WriteLine(JsonConvert.SerializeObject(_isRecordDeleted));
            Console.ReadKey();

            //Delete a record that has already been removed (deleted).
            _isRecordDeleted = ibxHelper.DeleteHostRecordAsync(_recordToChange).Result;

            Console.WriteLine(JsonConvert.SerializeObject(_isRecordDeleted));
            Console.ReadKey();

            Console.WriteLine("InfoBlox Helper Test Completed. Press any key to end.");
            Console.ReadKey();

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


        public static async Task<HostRecord> AddNewRecord(string hostname)
        {
            var _IpHostRecord = await ibxHelper.CreateHostRecordAsync(hostname);
            return (_IpHostRecord);
        }

        public static async Task<HostRecord> GetHostRecord(string hostname)
        {
            var _IpHostRecord = await ibxHelper.GetHostRecordAsync(hostname);
            return (_IpHostRecord);
        }

        public static async Task<HostRecord> GetHostByIP(string Ipv4Address)
        {
            var _IpHostRecord = await ibxHelper.GetHostRecordByIPAddressAsync(Ipv4Address);
            return (_IpHostRecord);
        }
    }
}
