
namespace CodeArt.Optimizely.PackageExplorer.Core.Models
{
    public class CategoryDefinition
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public bool? Selectable { get; set; }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }
    }
}
