using GenerateData.Data;
using GenerateData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GenerateData.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            var hobbies = _context.tblM_Hobi.ToList();
            return View(hobbies);
        }

        public IActionResult Generate()
        {
            var hobbies = _context.tblM_Hobi
                .Select(h => new Hobby { Id = h.Id, Nama = h.Nama })
                .ToList();
            var genders = _context.tblM_Gender
                .Select(g => new Gender { Id = g.Id, Nama = g.Nama })
                .ToList();

            var model = new GenerateDataViewModel
            {
                Hobbies = hobbies,
                Genders = genders
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult GenerateData(int numRecords)
        {
            List<PersonalDataModel> generatedData = GenerateRandomData(numRecords);
            return Json(generatedData);
        }

        /*
        [HttpPost]
        public IActionResult SubmitData([FromBody] List<PersonalDataModel> generatedData)
        {
            if (generatedData == null || !generatedData.Any())
            {
                return BadRequest("Data tidak boleh kosong.");
            }

            try
            {
                var jsonData = JsonSerializer.Serialize(generatedData);

                // Execute stored procedure with JSON data
                _context.Database.ExecuteSqlRaw("EXEC InsertPersonalDataBatch @p0", jsonData);

                return Ok("Data berhasil disimpan!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Terjadi kesalahan saat menyimpan data: " + ex.Message);
            }
        }
        */

        [HttpPost]
        public IActionResult SubmitData([FromBody] List<PersonalDataModel> generatedData)
        {
            if (generatedData == null || !generatedData.Any())
            {
                return BadRequest("Data tidak boleh kosong.");
            }

            try
            {
                // Konversi data ke DataTable
                var dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(int));
                dataTable.Columns.Add("Nama", typeof(string));
                dataTable.Columns.Add("IdGender", typeof(int));
                dataTable.Columns.Add("IdHobi", typeof(string));
                dataTable.Columns.Add("Umur", typeof(int));

                foreach (var item in generatedData)
                {
                    dataTable.Rows.Add(item.Id, item.Nama, item.IdGender, item.IdHobi, item.Umur);
                }

                // Buat parameter untuk User Defined Table Type
                var param = new SqlParameter("@PersonalData", SqlDbType.Structured)
                {
                    TypeName = "dbo.PersonalDataType",
                    Value = dataTable
                };

                // Panggil prosedur tersimpan
                _context.Database.ExecuteSqlRaw("EXEC InsertPersonalDataBatch_ @PersonalData", param);

                return Ok("Data berhasil disimpan!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Terjadi kesalahan saat menyimpan data: " + ex.Message);
            }
        }

        private List<PersonalDataModel> GenerateRandomData(int numRecords)
        {
            // Logika untuk menghasilkan data acak
            Random rand = new Random();
            List<PersonalDataModel> dataList = new List<PersonalDataModel>();

            var hobbies = _context.tblM_Hobi.ToList();
            var gender = _context.tblM_Gender.ToList();

            for (int i = 0; i < numRecords; i++)
            {
                PersonalDataModel data = new PersonalDataModel();
                data.Nama = GenerateRandomString(25);
                data.IdGender = gender[rand.Next(gender.Count())].Id;
                data.IdHobi = hobbies[rand.Next(hobbies.Count())].Id.ToString();
                data.Umur = rand.Next(18, 41);
                dataList.Add(data);
            }

            return dataList;
        }

        private string GenerateRandomString(int length)
        {
            // Logika untuk menghasilkan string acak
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] result = new char[length];
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }

            return new string(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
