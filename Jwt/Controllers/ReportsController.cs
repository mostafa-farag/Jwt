using Jwt.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Report.Models;

namespace Report.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly AppDBContext _dbcontext;
        public ReportsController(AppDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromForm] Reportdto dto)
        {
            var datastream=new MemoryStream();
            await dto.image.CopyToAsync(datastream);
            var report = new Reports
            {
                name = dto.name,
                location = dto.location,
                image =  datastream.ToArray(),
                DateTime = DateTime.Now
            };
            await _dbcontext.AddAsync(report);
            _dbcontext.SaveChanges();
            return Ok(report);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReports() 
        {
            var reports =await _dbcontext.Reports.ToListAsync();
            return Ok(reports);
        }       
        [HttpGet("Location")]
        public async Task<IActionResult> GetReportsbyLocation(string Location) 
        {
             var reports = _dbcontext.Reports.Where(s => s.location == Location).ToList();
            return reports == null ? NotFound() : Ok(reports);
        }
    }
}