
namespace Library.Test
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using KPMG.KTech.Automation.InfoBlox;
    using KPMG.KTech.Automation.InfoBlox.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InfoBloxTest
    {

        private static Helper infoBloxHelper = Helper.Instance;

        [TestMethod]
        public async Task RetrieveNetworks()
        {
            List<InfobloxNetwork> _lstNetworks = await infoBloxHelper.RetrieveNetworkLists();
            Assert.IsTrue(_lstNetworks.Count == 3);
        }

        [TestMethod]
        public async Task RetrieveIP()
        {
            var _ipResult = await infoBloxHelper.RetrieveIP(5);
            CollectionAssert.AllItemsAreNotNull(_ipResult.IPAddresses);
        }

    }
}