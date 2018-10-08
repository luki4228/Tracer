﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Tracer;

namespace Example
{
    class Program
    {

        private static ITracer tracer;

        internal static void AnotherSImpleMethod()
        {
            tracer.StartTrace();
            Thread.Sleep(100);
            tracer.StopTrace();
        }

        internal static void SimpleMethod()
        {
            tracer.StartTrace();
            AnotherSImpleMethod();
            Thread.Sleep(100);
            tracer.StopTrace();
        }

        static void Main(string[] args)
        {
            tracer = new Tracer.Tracer();
            SimpleMethod();
            TraceResult traceResult = tracer.GetTraceResult();
            XMLSerializer xml = new XMLSerializer();
            ConsoleWriter cw = new ConsoleWriter();
            cw.Write(traceResult, xml);
            FileWriter fw = new FileWriter("./xml.xml");
            fw.Write(traceResult, xml);
            JSONSerializer json = new JSONSerializer();
            cw.Write(traceResult, json);
            fw.FileName = "./json.json";
            fw.Write(traceResult, json);
            Console.ReadKey();
        }
    }
}
