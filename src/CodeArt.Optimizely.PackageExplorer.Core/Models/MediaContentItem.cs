using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.PackageExplorer.Core.Models
{
    public class MediaContentItem
    {
        public ContentItem Content { get; set; }

        public MediaContentItem(ContentItem contentItem) {
            Content = contentItem;
        }
    }
}
