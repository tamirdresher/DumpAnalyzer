using System;

namespace DebugDiag.DumpAnalyzer
{
    internal class DumpFileInfo
    {
        public string FilePath { get; }
        public DumpPriority Priority { get; set; } = DumpPriority.Low;
        public DumpFileInfo(string filePath)
        {
            FilePath = filePath;
        }
    }

    internal enum DumpPriority
    {
        Low,
        High
    }
}