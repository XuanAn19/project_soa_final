using LibraryManagementAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly BookDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        public GenresController(BookDbContext dbContext, IWebHostEnvironment hostEnvironment)
        {
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
        }
        //get : api/Bookapi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenresModel>>> GetAllGenre()
        {
            return await _dbContext.Genres.ToListAsync();
        }
        // GET: api/userapi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GenresModel>> GetGenre(int id)
        {
            var user = await _dbContext.Genres.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<GenresModel>> CreateGenre(GenresModel genres)
        {
            try
            {
                _dbContext.Genres.Add(genres);
                _dbContext.SaveChanges();
                return Ok(genres);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateGenre(int id, GenresModel genre)
        {
            if (id != genre.GenreID)
            {
                return BadRequest("ID mismatch.");
            }

            // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingGenre = await _dbContext.Genres.FindAsync(id);

            if (existingGenre == null)
            {
                return NotFound($"Genre with ID {id} not found.");
            }

            try
            {
                // Chỉ cập nhật các thuộc tính không phải khóa
                existingGenre.GenreName = genre.GenreName;
                existingGenre.Description = genre.Description;

                // Lưu thay đổi
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
                {
                    return NotFound($"Genre with ID {id} no longer exists.");
                }
                else
                {
                    throw; // Ném lỗi nếu không xác định được nguyên nhân
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return NoContent(); // Trả về thành công mà không có nội dung
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var book = await _dbContext.Genres.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _dbContext.Genres.Remove(book);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private bool GenreExists(int id)
        {
            return _dbContext.Genres.Any(e => e.GenreID == id);
        }
        
    }
}
