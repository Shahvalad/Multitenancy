using firstApi.Cryptography;
using firstApi.Models;
using Microsoft.EntityFrameworkCore;

namespace firstApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options ) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
    }

    public class DataContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ICryptographyService _cryptographyService;
        private readonly DataContext _dataContext;

        public DataContextFactory(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ICryptographyService cryptographyService, DataContext dataContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _cryptographyService = cryptographyService;
            _dataContext = dataContext;
        }

        public DataContext CreateContext()
        {
            var tenantId = _httpContextAccessor.HttpContext!.Request.Headers["TenantId"].FirstOrDefault();


            string connectionString = string.Empty;

            if (string.IsNullOrEmpty(tenantId))
            {
                connectionString = _configuration.GetConnectionString("DefaultConnection");
            }
            else
            {
                var tenant = _dataContext.Tenants.FirstOrDefault(x => x.TenantId == int.Parse(tenantId));
                connectionString = _cryptographyService.Decrypt(tenant.ConnectionString);
            }

            var optionBuilder = new DbContextOptionsBuilder<DataContext>();
            optionBuilder.UseSqlServer(connectionString);

            return new DataContext(optionBuilder.Options);
        }
    }
}
