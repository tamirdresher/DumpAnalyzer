using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugDiag.DumpAnalyzer
{
    public static class DumpAnalyzerConfig
    {
        public const string PublicSymbols = "srv*c:\\symsrv*http://msdl.microsoft.com/download/symbols";
        public  static string RelativeSymbolsDirectory => ConfigurationManager.AppSettings["relativeSymbolsDir"];
        public  static string VersionFile => ConfigurationManager.AppSettings["versionFile"];
    }
}
