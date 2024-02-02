namespace firstApi.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; } = null!;
        public string? TenancyName { get => this.Name.Replace(" ", "").Replace("-", "").Replace("_", "").ToLower(); private set { } }
        public string? ConnectionString { get; set; }
    }
}
