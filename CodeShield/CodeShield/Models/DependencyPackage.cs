namespace CodeShield.Models
{
    public class DependencyPackage
    {
        public string PackageName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public Ecosystem Ecosystem { get; set; }
    }
}
