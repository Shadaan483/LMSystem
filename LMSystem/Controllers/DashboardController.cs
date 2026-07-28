using LMSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LMSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _config;

        public DashboardController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            var model = new DashboardModel();

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                // Count Students
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Students", connection))
                {
                    model.TotalStudents = (int)cmd.ExecuteScalar();
                }

                // Count Books
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Books13", connection))
                {
                    model.TotalBooks = (int)cmd.ExecuteScalar();
                }

                // Count Librarians
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Librarians", connection))
                {
                    model.TotalLibrarians = (int)cmd.ExecuteScalar();
                }

                // Count Borrowings
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM BorrowRecords13", connection))
                {
                    model.TotalBorrowings = (int)cmd.ExecuteScalar();
                }

                // Count Publications
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Publications", connection))
                {
                    model.TotalPublications = (int)cmd.ExecuteScalar();
                }
            }

            return View(model);
        }
    }
}
