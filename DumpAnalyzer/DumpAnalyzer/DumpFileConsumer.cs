using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DebugDiag.DumpAnalyzer
{
    internal class DumpFileConsumer
    {
        private readonly BlockingCollection<DumpFileInfo> _dumpsCollection;
        private AnalysisJob _defaultAnalysis;
        private DumpAnalyzer _dumpAnalyzer;

        public DumpFileConsumer(BlockingCollection<DumpFileInfo> dumpsCollection, AnalysisJob defaultAnalysis)
        {
            _dumpsCollection = dumpsCollection;
            _defaultAnalysis = defaultAnalysis;
            _dumpAnalyzer = new DumpAnalyzer();

        }

        public void StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var dumpFileInfo = _dumpsCollection.Take(cancellationToken);
                _defaultAnalysis.DumpFiles.Clear();
                _defaultAnalysis.AddDumpFile(dumpFileInfo.FilePath);
                _defaultAnalysis.ReportPath = dumpFileInfo.FilePath + ".mht";
                _defaultAnalysis.Symbols = DumpAnalyzerConfig.PublicSymbols;
                if (!string.IsNullOrEmpty(DumpAnalyzerConfig.RelativeSymbolsDirectory))
                {
                    _defaultAnalysis.Symbols += $";{GetRelativeSymbolsPath(dumpFileInfo)}";
                }

                _dumpAnalyzer.RunAnalysis(_defaultAnalysis);
            }
        }

        private static string GetRelativeSymbolsPath(DumpFileInfo dumpFileInfo)
        {
            try
            {
                var version = "";
                if (!string.IsNullOrEmpty(DumpAnalyzerConfig.VersionFile))
                {
                    version = File.ReadAllLines(DumpAnalyzerConfig.VersionFile).FirstOrDefault()?.Trim() ?? "";
                }

                var relativeSymbolsDirectory = DumpAnalyzerConfig.RelativeSymbolsDirectory.ToLower().Replace("[version]", version);
                Logger.PrintTrace("relativeSymbol path is "+relativeSymbolsDirectory);
                return Path.Combine(Path.GetDirectoryName(dumpFileInfo.FilePath), relativeSymbolsDirectory);
            }
            catch (System.Exception ex)
            {

                Logger.PrintError(ex);

            }

            return "";
        }
    }
}