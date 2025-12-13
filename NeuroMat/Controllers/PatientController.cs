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

            if (!patient.PressureFrames.Any())
                return View("UploadCsv", patient);

            return View("PressureDetails", patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPressure(Guid patientId)
        {
            var frames = _db.PressureFrames
                .Where(f => f.PatientId == patientId);

            _db.PressureFrames.RemoveRange(frames);
            await _db.SaveChangesAsync();

            return RedirectToAction("Details", new { id = patientId });
        }

        // =========================
        // POST: /Patient/Delete
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var patient = await _db.Patients
                .Include(p => p.PressureFrames)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return RedirectToAction("Index", "Home");

            // delete pressure data first
            _db.PressureFrames.RemoveRange(patient.PressureFrames);
            _db.Patients.Remove(patient);

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

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

            var buffer = new List<byte>();   
            string? line;
            int framesSaved = 0;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                // Support comma OR semicolon CSV
                var parts = line.Split(',', ';');

                // Each CSV row must have exactly 32 values
                if (parts.Length != 32)
                    continue;

                foreach (var p in parts)
                {
                    if (byte.TryParse(p.Trim(), out var value))
                        buffer.Add(value);
                }

                // When we collect a full 32x32 frame
                if (buffer.Count == 1024)
                {
                    var values = buffer.ToArray();
                    buffer.Clear();

                    var metrics = _analysis.Analyze(values);

                    _db.PressureFrames.Add(new PressureFrame
                    {
                        Id = Guid.NewGuid(),
                        PatientId = patient.Id,
                        Timestamp = DateTimeOffset.UtcNow,
                        Values = values,
                        PeakPressureIndex = metrics.PeakPressureIndex,
                        ContactAreaPercent = metrics.ContactAreaPercent
                    });

                    framesSaved++;
                }
            }

            await _db.SaveChangesAsync();

            Console.WriteLine($"FRAMES SAVED: {framesSaved}");
            return RedirectToAction("Details", new { id = patientId });
        }

    }
}
