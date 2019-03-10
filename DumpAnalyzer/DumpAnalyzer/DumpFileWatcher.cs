using System.Collections.Concurrent;
using System.IO;

namespace DebugDiag.DumpAnalyzer
{
    internal class DumpFileWatcher
    {
        private readonly BlockingCollection<DumpFileInfo> _dumpFilesQueue;
        private const string DumpFileExtenions = "*.mdmp";
        private readonly FileSystemWatcher _fileSystemWatcher;

        public DumpFileWatcher(string folderPath, BlockingCollection<DumpFileInfo> dumpFilesQueue)
        {
            _dumpFilesQueue = dumpFilesQueue;
            _fileSystemWatcher = new FileSystemWatcher(folderPath, DumpFileExtenions)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName
            };
            _fileSystemWatcher.Created += OnNewDump;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnNewDump(object sender, FileSystemEventArgs e)
        {
            Logger.PrintTrace($"New dump file {e.FullPath}");
            _dumpFilesQueue.Add(new DumpFileInfo(e.FullPath));
        }
    }
}