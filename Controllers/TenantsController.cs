using firstApi.Cryptography;
using firstApi.Data;
using firstApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
namespace firstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<TenantsController> _logger;
        private readonly ICryptographyService _cryptographyService;

        public TenantsController(DataContext context, ILogger<TenantsController> logger, ICryptographyService cryptographyService)
        {
            _context = context;
            _logger = logger;
            _cryptographyService = cryptographyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tenant>>> GetAll()
        {
            return Ok(await _context.Tenants.ToListAsync());
        }
        [HttpPost("Create")]
        public async Task<ActionResult<Tenant>> Create(Tenant tenant)
        {
            if(!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state!");
                return BadRequest(ModelState);
            }
            _logger.LogInformation("Creating Tenant...");
            try
            {
                if(tenant == null)
                {
                    _logger.LogError("Tenant is null!");
                    throw new ArgumentNullException("Tenant is null!");
                }
                if(_context.Tenants.FirstOrDefault(t=>t.TenancyName ==  tenant.TenancyName) != null)
                {
                    _logger.LogError("Tenant already exists!");
                    throw new Exception("Tenant with such name already exists!");
                }

                tenant.ConnectionString = _cryptographyService.Encrypt(tenant.ConnectionString);
                await _context.Tenants.AddAsync(tenant);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tenant created successfully!");
                return Ok(tenant);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating tenant: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

    }
}
