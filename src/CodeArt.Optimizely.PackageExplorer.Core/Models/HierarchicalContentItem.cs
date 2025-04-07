using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.PackageExplorer.Core.Models
{
    public class HierarchicalContentItem
    {
        public ContentItem Content { get; set; }

        public HierarchicalContentItem(ContentItem content)
        {
            Content = content;
        }
        //Parent and child

        public List<HierarchicalContentItem> Children { get; set; } = new List<HierarchicalContentItem>();

        public List<HierarchicalContentItem> ContentAssets { get; set; } = new List<HierarchicalContentItem>();

        public override string ToString()
        {
            return Content.ToString();
        }
    }
}
