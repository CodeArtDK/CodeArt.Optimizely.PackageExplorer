using System;
using System.Collections.Generic;

namespace CodeArt.Optimizely.PackageExplorer.Core.Models
{
    public class VisitorGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsSecurityRole { get; set; }
        public bool EnableStatistics { get; set; }
        public int PointsThreshold { get; set; }
        public string CriteriaOperator { get; set; } = string.Empty; // "All" or "Any"
        public List<VisitorGroupCriterion> Criteria { get; set; } = new();
        public string Notes { get; set; } = string.Empty;
    }
}
