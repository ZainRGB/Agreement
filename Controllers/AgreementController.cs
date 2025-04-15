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

            // Process hpcsafile
            if (record.hpcsa != null)
            {
                record.hpcsafile = record.hpcsa.FileName;
                record.hpcsafileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.hpcsa.FileName)}";
                await SaveFile(record.hpcsa, Path.Combine(uploadPath, record.hpcsafileStoredName));
            }


            // Process bohffile
            if (record.boh != null)
            {
                record.bohffile = record.boh.FileName;
                record.bohffileStoredname = $"{Guid.NewGuid()}{Path.GetExtension(record.boh.FileName)}";
                await SaveFile(record.boh, Path.Combine(uploadPath, record.bohffileStoredname));
            }

            // Process ppiifile
            if (record.ppi != null)
            {
                record.ppiifile = record.ppi.FileName;
                record.ppiifileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.ppi.FileName)}";
                await SaveFile(record.ppi, Path.Combine(uploadPath, record.ppiifileStoredName));
            }

            // Process idfile
            if (record.idf != null)
            {
                record.idfile = record.idf.FileName;
                record.idfileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.idf.FileName)}";
                await SaveFile(record.idf, Path.Combine(uploadPath, record.idfileStoredName));
            }

            // Process qsfile
            if (record.qsf != null)
            {
                record.qsfile = record.qsf.FileName;
                record.qsfileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.qsf.FileName)}";
                await SaveFile(record.qsf, Path.Combine(uploadPath, record.qsfileStoredName));
            }

            // Process emerfile
            if (record.emer != null)
            {
                record.emerfile = record.emer.FileName;
                record.emerfileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.emer.FileName)}";
                await SaveFile(record.emer, Path.Combine(uploadPath, record.emerfileStoredName));
            }


            //_context.Agreements.Add(record);
            //await _context.SaveChangesAsync();
            //return RedirectToAction("Index");
            try
            {
                _context.Agreements.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "An error occurred while saving the data.");
                return View("Index", record);
            }

        }

        private async Task SaveFile(IFormFile file, string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
        }
    }
}