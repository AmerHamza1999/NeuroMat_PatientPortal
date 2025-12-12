using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeuroMat.Data;

namespace NeuroMat.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDBContext _db;

        public HomeController(AppDBContext db)
        {
            _db = db;
        }

        // =========================
        // DASHBOARD
        // =========================
        public async Task<IActionResult> Index()
        {
            var patients = await _db.Patients
                .OrderBy(p => p.DisplayName)
                .ToListAsync();

            return View(patients);
        }

        // =========================
        // GRAPH DATA API
        // =========================
        [HttpGet]
        public async Task<IActionResult> PressureData(Guid patientId, int hours)
        {
            var since = DateTimeOffset.UtcNow.AddHours(-hours);

            var frames = await _db.PressureFrames
                .Where(f => f.PatientId == patientId && f.Timestamp >= since)
                .OrderBy(f => f.Timestamp)
                .Select(f => new
                {
                    time = f.Timestamp,
                    peak = f.PeakPressureIndex,
                    contact = f.ContactAreaPercent
                })
                .ToListAsync();

            return Json(frames);
        }
    }
}
