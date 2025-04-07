using CodeArt.Optimizely.PackageExplorer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services
{
    public class ContentItemEnricher
    {

        public static List<HierarchicalContentItem> EnrichContentItems(List<ContentItem> contentItems)
        {
            Dictionary<string, HierarchicalContentItem> parentContentMap = new Dictionary<string, HierarchicalContentItem>();
            Dictionary<string, HierarchicalContentItem> childContentMap = new Dictionary<string, HierarchicalContentItem>();
            Dictionary<string, HierarchicalContentItem> assetsContentMap = new Dictionary<string, HierarchicalContentItem>();
            var hierarchy = new List<HierarchicalContentItem>();
            foreach (var item in contentItems)
            {
                var hitem = new HierarchicalContentItem(item);
                if (parentContentMap.ContainsKey(item.ParentLink))
                {
                    //We have a parent, add as a child
                    var parent = parentContentMap[item.ParentLink];
                    parent.Children.Add(hitem);
                }
                else if (childContentMap.ContainsKey(item.ContentLink))
                {
                    var child = childContentMap[item.ContentLink];
                    hierarchy.Remove(child);
                    hitem.Children.Add(child);
                    hierarchy.Add(hitem);
                }
                else if (assetsContentMap.ContainsKey(item.ParentLink))
                {
                    //We have an asset
                    var assetowner = assetsContentMap[item.ParentLink];
                    assetowner.ContentAssets.Add(hitem);
                }
                else
                {
                    hierarchy.Add(hitem);
                }
                parentContentMap.TryAdd(item.ContentLink, hitem);
                childContentMap.TryAdd(item.ParentLink, hitem);
                if (item.ContentAssetsID != null)
                {
                    assetsContentMap.TryAdd(item.ContentAssetsID, hitem);
                }
            }
            return hierarchy;
        }

    }
}
