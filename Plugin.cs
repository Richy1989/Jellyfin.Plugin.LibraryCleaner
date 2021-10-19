using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Plugin.Template.Configuration;
using Jellyfin.Plugin.Template.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Tasks;

namespace Jellyfin.Plugin.Template
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        private ILibraryManager libraryManager;
        private CleanerPluginManager cleanerCollector;
        public override string Name => "Library Cleaner";

        public override Guid Id => Guid.Parse("04A8270D-D30B-4C36-8D52-A510C433C1C2");

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILibraryManager libraryManager, ITaskManager taskManager) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            this.libraryManager = libraryManager;
            cleanerCollector = new CleanerPluginManager(libraryManager);

            var task = new ScheduledCleanerListingTask(libraryManager, CleanerPluginManager.CleanListItemList);

            var taskList = new List<IScheduledTask>
            {
                task
            };

            taskManager.AddTasks(taskList);
        }

        public static Plugin Instance { get; private set; }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.Configuration.configPage.html", GetType().Namespace)
                }
            };
        }
    }
}
