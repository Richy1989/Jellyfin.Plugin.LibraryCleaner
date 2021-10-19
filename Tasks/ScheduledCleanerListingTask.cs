using Jellyfin.Data.Events;
using Jellyfin.Plugin.Template.Configuration;
using Jellyfin.Plugin.Template.Models;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Template.Tasks
{
    internal class ScheduledCleanerListingTask : IScheduledTask
    {
        private ILibraryManager libraryManager;
        private List<CleanListItem> cleanItemList;
        private PluginConfiguration pluginConfiguration;
        private IProgress<double> progress;
        private double countProg = 0;

        public string Name => "Cleaner Listing Task";

        public string Key => Guid.NewGuid().ToString();

        public string Description => "Cleaner Listing Task";

        public string Category => "Maintenance";

        public ScheduledCleanerListingTask(ILibraryManager libraryManager, PluginConfiguration pluginConfiguration)
        {
            this.pluginConfiguration = pluginConfiguration;
            this.cleanItemList = CleanerPluginManager.CleanListItemList;
            this.libraryManager = libraryManager;
        }

        public async Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            this.progress = progress;
            await Task.Run(() =>
            {
                progress?.Report(0.0);
                List<FileInfo> foundFiles = new List<FileInfo>();

                cleanItemList.Clear();

                try
                {
                    foreach (var child in libraryManager.RootFolder.Children)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        DirectoryInfo directoryInfo = new DirectoryInfo(child.Path);
                        foundFiles.AddRange(WalkDirectoryTree(directoryInfo));
                    }
                    progress?.Report(50.0);
                }
                catch (Exception ex)
                {
                    //ToDo: Remove this and add logging
                    cleanItemList.Add(new CleanListItem { Path = ex.Message, Name = ex.Message });
                }

                if (foundFiles != null)
                {
                    foreach (var file in foundFiles)
                    {
                        cleanItemList.Add(new CleanListItem { Path = file.FullName, Name = file.Name, Extension = file.Extension });
                    }
                }
            }, cancellationToken);
        }

        private List<FileInfo> WalkDirectoryTree(DirectoryInfo root)
        {
            List<FileInfo> returnFiles = new List<FileInfo>();
            FileInfo[] files = root.GetFiles();

            //Filter files for Contains text and File Extensions
            foreach (var fi in files)
            {

                var extensionList = pluginConfiguration.ExtensionFilterList.Split(",");

                foreach (var extensionFilter in extensionList)
                {
                    if (fi.Extension == extensionFilter)
                        returnFiles.Add(fi);
                }

                var containsList = pluginConfiguration.ContainsFilterList.Split(",");

                foreach (var containsFilter in containsList)
                {
                    if (fi.Name.Contains(containsFilter))
                        returnFiles.Add(fi);
                }
                countProg = countProg + 10;
                progress.Report(countProg);
            }

            // Now find all the subdirectories under this directory.
            DirectoryInfo[] subDirs = root.GetDirectories();

            if (subDirs.Length == 0)
            {
                //ToDo: Add empty directories
                //returnFiles.AddRange(WalkDirectoryTree(dirInfo));
            }
            else
            {
                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    returnFiles.AddRange(WalkDirectoryTree(dirInfo));
                }
            }
            return returnFiles;
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            TaskTriggerInfo tinfo = new TaskTriggerInfo();
            tinfo.Type = "IntervalTrigger";
            tinfo.IntervalTicks = TimeSpan.FromHours(24).Ticks;
            var li = new List<TaskTriggerInfo>
            {
                tinfo
            };

            return li;
        }
    }
}
