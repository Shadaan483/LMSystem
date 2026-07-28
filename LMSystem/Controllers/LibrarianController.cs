using LMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LMSystem.Controllers
{
    // Managing librarian records is an admin-level task - a Librarian shouldn't
    // be able to add/remove other librarians.
    [Authorize(Roles = "Administrator")]
    public class LibrarianController : Controller
    {
        private readonly IConfiguration _config;

        public LibrarianController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index(string? searchTerm, int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 5; // Change this number to control items per page
            int offset = (page - 1) * pageSize;

            var librarians = new List<LibrarianModel>();
            int totalRecords = 0;

            using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            con.Open();

            // 1. Get Total Count for Pagination Links
            string countQuery = "SELECT COUNT(*) FROM Librarians WHERE (@SearchTerm IS NULL OR Name LIKE '%' + @SearchTerm + '%')";
            using (var countCmd = new SqlCommand(countQuery, con))
            {
                countCmd.Parameters.AddWithValue("@SearchTerm", (object?)searchTerm ?? DBNull.Value);
                totalRecords = (int)countCmd.ExecuteScalar();
            }

            // 2. Fetch Filtered and Paginated Records (ORDER BY is required for OFFSET)
            string dataQuery = @"SELECT * FROM Librarians
                                  WHERE (@SearchTerm IS NULL OR Name LIKE '%' + @SearchTerm + '%')
                                  ORDER BY LibrarianId
                                  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            using (var cmd = new SqlCommand(dataQuery, con))
            {
                cmd.Parameters.AddWithValue("@SearchTerm", (object?)searchTerm ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    librarians.Add(new LibrarianModel
                    {
                        LibrarianId = (int)reader["LibrarianId"],
                        Name = reader["Name"].ToString(),
                        Age = (int)reader["Age"],
                        Phone = reader["Phone"].ToString()
                    });
                }
            }

            // 3. Populate and return View Model
            var viewModel = new LibrarianIndexViewModel
            {
                Librarians = librarians,
                SearchTerm = searchTerm,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(LibrarianModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var cmd = new SqlCommand("INSERT INTO Librarians (Name, Age, Phone) VALUES (@Name, @Age, @Phone)", con);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Age", model.Age);
            cmd.Parameters.AddWithValue("@Phone", model.Phone);
            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            LibrarianModel librarian = new();
            using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var cmd = new SqlCommand("SELECT * FROM Librarians WHERE LibrarianId=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                librarian.LibrarianId = (int)reader["LibrarianId"];
                librarian.Name = reader["Name"].ToString();
                librarian.Age = (int)reader["Age"];
                librarian.Phone = reader["Phone"].ToString();
            }
            return View(librarian);
        }

        [HttpPost]
        public IActionResult Edit(LibrarianModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var cmd = new SqlCommand("UPDATE Librarians SET Name=@Name, Age=@Age, Phone=@Phone WHERE LibrarianId=@id", con);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Age", model.Age);
            cmd.Parameters.AddWithValue("@Phone", model.Phone);
            cmd.Parameters.AddWithValue("@id", model.LibrarianId);
            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var cmd = new SqlCommand("DELETE FROM Librarians WHERE LibrarianId=@id", con);
            cmd.Parameters.AddWithValue("@id", id);
            con.Open();
            cmd.ExecuteNonQuery();
            return RedirectToAction("Index");
        }
    }
}
