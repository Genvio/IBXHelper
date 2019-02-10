using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using KPMG.KTech.Automation.InfoBlox;
using KPMG.KTech.Automation.InfoBlox.Model;

namespace KPMG.KTech
{
    public class Program

    {

        private static Helper infoBloxHelper = Helper.Instance;
        public static async Task Main(string[] args)
        {
            TestRetrieveNetworks().Wait();
            TestRetrieveIpMethod().Wait();
        }

        private async static Task<bool> TestRetrieveNetworks()
        {
            try
            {

                List<InfobloxNetwork> _lstNetworks = await infoBloxHelper.RetrieveNetworkLists();

                Console.Write(_lstNetworks.ToJson());
            }
            catch (System.Exception)
            {

                //throw;
                return false;
            }

            return true;
        }

        private async static Task<bool> TestRetrieveIpMethod()
        {
            try
            {

                IpResult _ipResult = await infoBloxHelper.RetrieveIP(10);

                Console.Write(_ipResult.ToJson());
            }
            catch (System.Exception)
            {

                //throw;
                return false;

            }

            return true;
        }
    }


}

