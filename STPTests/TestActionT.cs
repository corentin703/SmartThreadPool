using Amib.Threading;
using NUnit.Framework;
using System.Threading;

namespace SmartThreadPoolTests
{
    /// <summary>
    /// Summary description for TestCancel.
    /// </summary>
    [TestFixture]
    [Category("TestActionT")]
    public class TestActionT
    {
        private SmartThreadPool _stp;
        private object _result;

        [SetUp]
        public void Init()
        {
            _stp = new SmartThreadPool();
        }

        [TearDown]
        public void Fini()
        {
            _stp.Shutdown();
        }

            [Test]
            public void ActionT0()
            {
                _result = null;

                IWorkItemResult wir = _stp.QueueWorkItem(MaxInt);

                wir.GetResult();

                Assert.AreEqual(_result, int.MaxValue);
            }

            [Test]
            public void ActionT1()
            {
                _result = null;

                IWorkItemResult wir = _stp.QueueWorkItem(Not, true);

                wir.GetResult();

                Assert.AreEqual(_result, false);
            }

            [Test]
            public void ActionT2()
            {
                _result = null;

                IWorkItemResult wir = _stp.QueueWorkItem(Concat, "ABC", "xyz");

                wir.GetResult();

                Assert.AreEqual(_result, "ABCxyz");
            }

            [Test]
            public void ActionT3()
            {
                _result = null;

                IWorkItemResult wir = _stp.QueueWorkItem(Substring, "ABCDEF", 1, 2);

                wir.GetResult();

                Assert.AreEqual(_result, "BC");
            }

            [Test]
            public void ActionT4()
            {
                _result = null;

                int[] numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                IWorkItemResult wir = _stp.QueueWorkItem(Subarray, numbers, 1, 2, 3);

                wir.GetResult();

                Assert.AreEqual(_result, new int[] { 2, 3, 2, 3, 2, 3, });
            }

            private void MaxInt(CancellationToken cancellationToken)
            {
                _result = int.MaxValue;
            }

            private void Not(bool flag, CancellationToken cancellationToken)
            {
                _result = !flag;
            }

            private void Concat(string s1, string s2, CancellationToken cancellationToken)
            {
                _result = s1 + s2;
            }

            private void Substring(string s, int startIndex, int length, CancellationToken cancellationToken)
            {
                _result = s.Substring(startIndex, length);
            }

            private void Subarray(int[] numbers, int startIndex, int length, int repeat, CancellationToken cancellationToken)
            {
                int[] result = new int[length * repeat];
                for (int i = 0; i < repeat; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        result[i * length + j] = numbers[startIndex + j];
                    }
                }

                _result = result;
            }
    }
}
