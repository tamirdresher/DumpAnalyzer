using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using DebugDiag.DotNet;
using DebugDiag.DotNet.AnalysisRules;

namespace DebugDiag.DumpAnalyzer
{
    class DumpAnalyzer
    {
        Stopwatch _stopwatch = new Stopwatch();
        public void RunAnalysis(AnalysisJob analysis)
        {
            using (NetAnalyzer analyzer = new NetAnalyzer())
            {
                List<AnalysisRuleInfo> ari = analysis.AnalisysRuleInfos;

                if (ari.Count < 1 && analysis.Modules.Count < 1)
                {
                    Logger.ReportError("Could not find any rule that matches the parameters.\r\n");
                    Logger.ShowCommandLineOptions();
                    return;
                }

                Logger.PrintProgress(("\r\nThe Dump Files being analyzed are: \n\r"));

                foreach (string dumpName in analysis.DumpFiles)
                {
                    Logger.PrintTrace(dumpName);
                }
                Logger.PrintTrace();

                foreach (Assembly module in analysis.Modules)
                    analyzer.AddAnalysisRulesToRunList(module);

                //analyzer.AnalysisRuleInfos = analysisRuleInfos;
                analyzer.AnalysisRuleInfos.AddRange(ari);

                Logger.PrintProgress("The rules being executed are: \n\r");

                foreach (AnalysisRuleInfo ruleInfo in analyzer.AnalysisRuleInfos)
                {
                    Logger.PrintTrace(ruleInfo.DisplayName);
                }
                Logger.PrintTrace("");

                Logger.PrintProgress("The symbols sources are: \n\r");

                Logger.PrintTrace(analysis.Symbols);
                Logger.PrintTrace("");

                //Add Dump list to Analizer object so we can analyze them with the debugger
                analyzer.AddDumpFiles(analysis.DumpFiles, analysis.Symbols);

                NetProgress np = new NetProgress();
                np.OnSetOverallStatusChanged += new EventHandler<SetOverallStatusEventArgs>(np_OnSetOverallStatusChanged);
                //np.OnSetCurrentPositionChanged += new EventHandler<SetCurrentPositionEventArgs>(np_OnSetCurrentPositionChanged);
                np.OnSetCurrentStatusChanged += new EventHandler<SetCurrentStatusEventArgs>(np_OnSetCurrentStatusChanged);
                np.OnEnd += new EventHandler(np_OnEnd);

                Logger.PrintTrace($"{DateTime.Now.ToLongTimeString()} - Start Analysis");
                _stopwatch.Start();

                try
                {
                    analyzer.RunAnalysisRules(np, analysis.Symbols, "", analysis.ReportPath);
                }
                catch (Exception ex)
                {
                    Logger.PrintError(ex);
                }
                finally
                {
                    _stopwatch.Stop();
                }
                np.End();

                if (analysis.ShowResults)
                    analyzer.ShowReportFiles();
            }
        }

        void np_OnEnd(object sender, EventArgs e)
        {
            Logger.PrintTrace($"{DateTime.Now.ToLongTimeString()} - Finished");
            Logger.PrintProgress($"\n\rTotal time of analysis: {_stopwatch.Elapsed.ToString()}");
        }

        void np_OnSetCurrentStatusChanged(object sender, SetCurrentStatusEventArgs e)
        {
            if (e.NewStatus != string.Empty)
                Logger.PrintTrace($"{DateTime.Now.ToLongTimeString()} - {e.NewStatus}");
        }

        static void np_OnSetOverallStatusChanged(object sender, SetOverallStatusEventArgs e)
        {
            if (e.NewStatus != string.Empty)
                Logger.PrintTrace($"{DateTime.Now.ToLongTimeString()} - {e.NewStatus}");
        }

    }
}