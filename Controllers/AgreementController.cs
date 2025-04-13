using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Agreement.Models;
using Agreement.Data;

namespace Agreement.Controllers
{
    public class AgreementController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public AgreementController(AppDbContext context, IConfiguration config, IWebHostEnvironment env)
        {
            _context = context;
            _config = config;
            _env = env;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Create(AgreementRecord record)
        {
            //if (!ModelState.IsValid) return View("Index", record);

            var uploadPath = _config["FileUploadPath"] ?? Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadPath);

            // Process MOU
            if (record.Mou != null)
            {
                record.MouFileName = record.Mou.FileName;
                record.MouStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.Mou.FileName)}";
                await SaveFile(record.Mou, Path.Combine(uploadPath, record.MouStoredName));
            }

            // Process NDA
            if (record.Nda != null)
            {
                record.NdaFileName = record.Nda.FileName;
                record.NdaStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.Nda.FileName)}";
                await SaveFile(record.Nda, Path.Combine(uploadPath, record.NdaStoredName));
            }

            _context.Agreements.Add(record);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private async Task SaveFile(IFormFile file, string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }
}