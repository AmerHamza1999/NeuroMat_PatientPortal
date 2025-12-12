using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeuroMat.Data;
using NeuroMat.Models;
using NeuroMat.Services;

namespace NeuroMat.Controllers
{
    public class PatientController : Controller
    {
        private readonly AppDBContext _db;
        private readonly IPressureAnalysisService _analysis;

        public PatientController(AppDBContext db, IPressureAnalysisService analysis)
        {
            _db = db;
            _analysis = analysis;
        }

        // =========================
        // GET: /Patient/Create
        // =========================
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // POST: /Patient/Create
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            Console.WriteLine("POST CREATE HIT");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("MODEL INVALID");
                return View(patient);
            }

            patient.Id = Guid.NewGuid();
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            Console.WriteLine("PATIENT SAVED");

            return RedirectToAction("Index", "Home");
        }


        // =========================
        // GET: /Patient/Details/{id}
        // =========================
        public async Task<IActionResult> Details(Guid id)
        {
            var patient = await _db.Patients
                .Include(p => p.PressureFrames)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            // If no pressure data → ask for CSV
            if (!patient.PressureFrames.Any())
                return View("UploadCsv", patient);

            // If data exists → show graphs
            return View("PressureDetails", patient);
        }

        // =========================
        // POST: /Patient/UploadCsv
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCsv(Guid patientId, IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
                return RedirectToAction("Details", new { id = patientId });

            var patient = await _db.Patients.FindAsync(patientId);
            if (patient == null)
                return NotFound();

            using var reader = new StreamReader(csvFile.OpenReadStream());
            string? line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                var parts = line.Split(',');

                // timestamp,value1,value2,...value1024
                if (parts.Length != 1025)
                    continue;

                var timestamp = DateTimeOffset.Parse(parts[0]);
                var values = parts.Skip(1).Select(byte.Parse).ToArray();

                var metrics = _analysis.Analyze(values);

                _db.PressureFrames.Add(new PressureFrame
                {
                    Id = Guid.NewGuid(),
                    PatientId = patient.Id,
                    Timestamp = timestamp,
                    Values = values,
                    PeakPressureIndex = metrics.PeakPressureIndex,
                    ContactAreaPercent = metrics.ContactAreaPercent
                });
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = patientId });
        }
    }
}
