using Jellyfin.Plugin.Template.Models;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Resolvers;
using MediaBrowser.Controller.Sorting;
using MediaBrowser.Model.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Template
{
    internal class CleanerPluginManager
    {
        private ILibraryManager libraryManager;

        private static List<CleanListItem> cleanItemList = new List<CleanListItem>();

        public static List<CleanListItem> CleanListItemList { get => cleanItemList; }

        internal CleanerPluginManager(ILibraryManager libraryManager)
        {
            this.libraryManager = libraryManager;
        }
    }
}
