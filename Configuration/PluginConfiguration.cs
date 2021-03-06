using Jellyfin.Plugin.Template.Models;
using MediaBrowser.Model.Plugins;
using System.Linq;
using System.Collections.Generic;

namespace Jellyfin.Plugin.Template.Configuration
{
    public enum SomeOptions
    {
        OneOption,
        AnotherOption
    }

    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>List of feeds</summary>
        /// <value>urls of xml podcast feeds</value>
        public List<CleanListItem> CleanList => CleanerPluginManager.CleanListItemList;

        // store configurable settings your plugin might need
        public bool TrueFalseSetting { get; set; }
        public int AnInteger { get; set; }
        public string ExtensionFilterList { get; set; }
        public string ContainsFilterList { get; set; }
        public SomeOptions Options { get; set; }

        public PluginConfiguration()
        {
            // set default options here
            Options = SomeOptions.AnotherOption;
            TrueFalseSetting = true;
            AnInteger = 2;
            ExtensionFilterList = ".html";
            ContainsFilterList = "sample";
        }
    }
}
