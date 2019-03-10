using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using DebugDiag.DotNet.AnalysisRules;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;

namespace DebugDiag.DumpAnalyzer
{
    class Program
    {
        static List<string> dumpFiles = new List<string>();
        static List<string> assemblyNames = new List<string>();
        static List<AnalysisRuleInfo> analysisRuleInfos = new List<AnalysisRuleInfo>();

        private static Stopwatch stopwatch = new Stopwatch();
        private static ArgsValidator _av;
        private static AnalysisJob _analysis;

        [STAThread]
        static void Main(string[] args)
        {
            //Console.ReadKey();
            _av = new ArgsValidator(args);
            if (!_av.ValidArguments) return;
            _analysis = _av.GetAnalysisJob;
            var analyzer = new DumpAnalyzer();
            if (!string.IsNullOrEmpty(_analysis.MonitoredFolder))
            {
                var dumpsCollection = new BlockingCollection<DumpFileInfo>(new ConcurrentQueue<DumpFileInfo>());
                var consumer = new DumpFileConsumer(dumpsCollection, _analysis);
                var watcher = new DumpFileWatcher(_analysis.MonitoredFolder, dumpsCollection);

                var cts=new CancellationTokenSource();
                Console.CancelKeyPress += (sender, eventArgs) => cts.Cancel();

                consumer.StartAsync(cts.Token);
            }
            else
            {
                analyzer.RunAnalysis(_analysis);
            }
        }

    }
}
