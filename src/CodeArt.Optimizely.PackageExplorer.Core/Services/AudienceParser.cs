using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CodeArt.Optimizely.PackageExplorer.Core.Models;

namespace CodeArt.Optimizely.PackageExplorer.Core.Services
{
    public static class AudienceParser
    {
        public static List<VisitorGroup> ParseAudiences(XDocument handlerMapXml, ZipPackage zipPackage)
        {
            var audiences = new List<VisitorGroup>();
            
            // Parse handlermap.xml to find data files
            var handlers = handlerMapXml.Root?.Elements("handler");
            if (handlers == null) return audiences;
            
            foreach (var handler in handlers)
            {
                var path = handler.Attribute("path")?.Value;
                if (string.IsNullOrEmpty(path)) continue;
                
                try
                {
                    // Read the data file
                    var dataXml = zipPackage.ReadXmlFile(path.TrimStart('/'));
                    var visitorGroups = ParseVisitorGroups(dataXml);
                    audiences.AddRange(visitorGroups);
                }
                catch
                {
                    // Skip files that can't be parsed
                    continue;
                }
            }
            
            return audiences;
        }
        
        private static List<VisitorGroup> ParseVisitorGroups(XDocument xml)
        {
            var visitorGroups = new List<VisitorGroup>();
            
            var objects = xml.Root?.Elements("object");
            if (objects == null) return visitorGroups;
            
            // Find all VisitorGroup objects
            foreach (var obj in objects)
            {
                var storename = obj.Attribute("storename")?.Value;
                if (storename != "VisitorGroup") continue;
                
                var visitorGroup = ParseVisitorGroup(obj, xml);
                if (visitorGroup != null)
                {
                    visitorGroups.Add(visitorGroup);
                }
            }
            
            return visitorGroups;
        }
        
        private static VisitorGroup? ParseVisitorGroup(XElement obj, XDocument xml)
        {
            try
            {
                var id = Guid.Parse(obj.Attribute("id")?.Value ?? Guid.Empty.ToString());
                
                var visitorGroup = new VisitorGroup
                {
                    Id = id,
                    Name = GetPropertyValue(obj, "Name") ?? "Unnamed",
                    IsSecurityRole = GetPropertyValue(obj, "IsSecurityRole") == "True",
                    EnableStatistics = GetPropertyValue(obj, "EnableStatistics") == "True",
                    PointsThreshold = int.TryParse(GetPropertyValue(obj, "PointsThreshold"), out var threshold) ? threshold : 0,
                    CriteriaOperator = GetPropertyValue(obj, "CriteriaOperator") ?? "All",
                    Notes = GetPropertyValue(obj, "Notes") ?? string.Empty
                };
                
                // Parse criteria
                var criteriaProperty = obj.Elements("property")
                    .FirstOrDefault(p => p.Attribute("name")?.Value == "Criteria");
                
                if (criteriaProperty != null)
                {
                    var collection = criteriaProperty.Element("collection");
                    if (collection != null)
                    {
                        var criteriaRefs = collection.Elements("value")
                            .Select(v => v.Attribute("refId")?.Value)
                            .Where(r => !string.IsNullOrEmpty(r))
                            .ToList();
                        
                        foreach (var refId in criteriaRefs)
                        {
                            var criterion = ParseCriterion(refId!, xml);
                            if (criterion != null)
                            {
                                visitorGroup.Criteria.Add(criterion);
                            }
                        }
                    }
                }
                
                return visitorGroup;
            }
            catch
            {
                return null;
            }
        }
        
        private static VisitorGroupCriterion? ParseCriterion(string refId, XDocument xml)
        {
            try
            {
                var criterionObj = xml.Root?.Elements("object")
                    .FirstOrDefault(o => o.Attribute("id")?.Value == refId && 
                                       o.Attribute("storename")?.Value == "VisitorGroupCriterion");
                
                if (criterionObj == null) return null;
                
                var criterion = new VisitorGroupCriterion
                {
                    Id = Guid.Parse(refId),
                    Enabled = GetPropertyValue(criterionObj, "Enabled") == "True",
                    Notes = GetPropertyValue(criterionObj, "Notes") ?? string.Empty,
                    Required = GetPropertyValue(criterionObj, "Required") == "True",
                    Points = int.TryParse(GetPropertyValue(criterionObj, "Points"), out var points) ? points : 0,
                    TypeName = GetPropertyValue(criterionObj, "TypeName") ?? string.Empty,
                    NamingContainer = GetPropertyValue(criterionObj, "NamingContainer") ?? string.Empty
                };
                
                // Get simplified model data
                var modelProperty = criterionObj.Elements("property")
                    .FirstOrDefault(p => p.Attribute("name")?.Value == "Model");
                
                if (modelProperty != null)
                {
                    var modelRefId = modelProperty.Attribute("refId")?.Value;
                    if (!string.IsNullOrEmpty(modelRefId))
                    {
                        var modelObj = xml.Root?.Elements("object")
                            .FirstOrDefault(o => o.Attribute("id")?.Value == modelRefId);
                        
                        if (modelObj != null)
                        {
                            // Extract readable model properties
                            var modelProps = modelObj.Elements("property")
                                .Where(p => p.Attribute("name") != null && p.Attribute("name")?.Value != "Id")
                                .Select(p => $"{p.Attribute("name")?.Value}: {p.Value}")
                                .ToList();
                            
                            criterion.ModelData = string.Join(", ", modelProps);
                        }
                    }
                }
                
                return criterion;
            }
            catch
            {
                return null;
            }
        }
        
        private static string? GetPropertyValue(XElement obj, string propertyName)
        {
            return obj.Elements("property")
                .FirstOrDefault(p => p.Attribute("name")?.Value == propertyName)
                ?.Value;
        }
    }
}
