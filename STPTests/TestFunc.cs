using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Amib.Threading;
using NUnit.Framework;

namespace STPTests
{
    /// <summary>
    /// Summary description for TestCancel.
    /// </summary>
    [TestFixture]
    [Category("TestFuncT")]
    public class TestFuncT
    {
        [Test]
        public void FuncT()
        {
            SmartThreadPool stp = new SmartThreadPool();
            IWorkItemResult<int> wir = 
                stp.QueueWorkItem(new Func<int, CancellationToken, int>(f), 1);

            int y = wir.GetResult();

            Assert.AreEqual(y, 2);

            try
            {
                wir.GetResult();
            }
            finally
            {
                stp.Shutdown();
            }
        }

        private int f(int x, CancellationToken cancellationToken)
        {
            return x + 1;
        }
    }
}
