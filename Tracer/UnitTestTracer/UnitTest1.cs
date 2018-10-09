using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;
using Tracer;

namespace TracerUnitTest1
{
    [TestClass]
    public class TracerUnitTest
    {
        private static Tracer.Tracer tracer;
        private static int waitTime = 100;
        private static int threadsCount = 3;
        private static string timeTestFailMessage = "Failed time test: most probably actual < expected";
        private static string classNameTestFailMessage = "Fail class name test: expected class name != actual class name";
        private static string methodNameTestFailMessage = "Fail method name test: expected method name != actual method name";
        private static string countTestFailMessage = "Fail count test: expected count != actual count";

        private bool TimeTest(long actual, long expected)
        {
            return actual >= expected;
        }

        private void SimpleTestMethod()
        {
            tracer.StartTrace();
            Thread.Sleep(waitTime);
            tracer.StopTrace();
        }

        private void MultiThreadedMethod()
        {
            var threads = new List<Thread>();
            Thread newThread;
            for (int i = 0; i < threadsCount; i++)
            {
                newThread = new Thread(SimpleTestMethod);
                threads.Add(newThread);
            }
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
            tracer.StartTrace();
            Thread.Sleep(waitTime);
            tracer.StopTrace();
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        [TestMethod]
        public void SingleThreadTest()
        {
            tracer = new Tracer.Tracer();
            tracer.StartTrace();
            Thread.Sleep(waitTime);
            tracer.StopTrace();
            long actual = tracer.GetTraceResult().ThreadsList[0].Time;
            Assert.AreEqual(TimeTest(actual, waitTime), true, timeTestFailMessage);
        }

        [TestMethod]
        public void SimpleMultiThreadTest()
        {
            tracer = new Tracer.Tracer();
            var threads = new List<Thread>();
            long expected = 0;
            Thread newThread;
            for (int i = 0; i < threadsCount; i++)
            {
                newThread = new Thread(SimpleTestMethod);
                threads.Add(newThread);
                newThread.Start();
                expected += waitTime;
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            long actual = 0;
            foreach (ThreadTracer threadResult in tracer.GetTraceResult().ThreadsList)
            {
                actual += threadResult.Time;
            }
            Assert.AreEqual(TimeTest(actual, expected), true, timeTestFailMessage);
        }

        [TestMethod]
        public void MethodInMethodTest()
        {
            tracer = new Tracer.Tracer();
            tracer.StartTrace();
            Thread.Sleep(waitTime);
            SimpleTestMethod();
            tracer.StopTrace();
            TraceResult traceResult = tracer.GetTraceResult();

            Assert.AreEqual(1, traceResult.ThreadsList.Count, countTestFailMessage);
            Assert.AreEqual(TimeTest(traceResult.ThreadsList[0].Time, waitTime * 2), true, timeTestFailMessage);
            Assert.AreEqual(1, traceResult.ThreadsList[0].InnerMethods.Count, countTestFailMessage);
            MethodTracer methodResult = traceResult.ThreadsList[0].InnerMethods[0];
            Assert.AreEqual(nameof(TracerUnitTest), methodResult.ClassName, classNameTestFailMessage);
            Assert.AreEqual(nameof(MethodInMethodTest), methodResult.Name, methodNameTestFailMessage);
            Assert.AreEqual(TimeTest(methodResult.Time, waitTime), true, timeTestFailMessage);
            Assert.AreEqual(1, methodResult.InnerMethods.Count, countTestFailMessage);
            MethodTracer innerMethodResult = methodResult.InnerMethods[0];
            Assert.AreEqual(nameof(TracerUnitTest), innerMethodResult.ClassName, classNameTestFailMessage);
            Assert.AreEqual(nameof(SimpleTestMethod), innerMethodResult.Name, methodNameTestFailMessage);
            Assert.AreEqual(TimeTest(innerMethodResult.Time, waitTime), true, timeTestFailMessage);
        }
    }
}
