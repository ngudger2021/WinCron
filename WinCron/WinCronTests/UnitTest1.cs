using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace WinCronTests
{
    [TestClass]
    public class UnitTest1
    {
        WinCron.WinCronService winCron = new WinCron.WinCronService();

        [TestMethod]
        public void SystemCheckTest()
        {
            winCron.SystemCheck();
        }

        [TestMethod]
        public void RunCronTest()
        {
            winCron.RunCron();
        }
    }
}
