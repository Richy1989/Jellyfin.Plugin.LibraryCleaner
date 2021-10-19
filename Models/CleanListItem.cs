using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.Template.Models
{
    public class CleanListItem
    {
        public String Path { get; set; }
        public String Name { get; set; }
        public String Extension { get; set; }
        public String IsDeletionApproved { get; set; }
    }
}
