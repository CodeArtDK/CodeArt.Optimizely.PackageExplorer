using System;

namespace CodeArt.Optimizely.PackageExplorer.Core.Models
{
    public class VisitorGroupCriterion
    {
        public Guid Id { get; set; }
        public bool Enabled { get; set; }
        public string Notes { get; set; } = string.Empty;
        public bool Required { get; set; }
        public int Points { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string NamingContainer { get; set; } = string.Empty;
        public string ModelData { get; set; } = string.Empty; // Simplified - stores serialized model info
    }
}
