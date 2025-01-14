using LibraryManagementAPI.Models;
using LibraryManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LibraryManagementAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        private readonly BookDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ICloudinaryService _cloudinaryService;
        public HomeController(BookDbContext dbContext, IWebHostEnvironment hostEnvironment, ICloudinaryService cloudinaryService)
        {
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _cloudinaryService = cloudinaryService;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetAllBook()
        {
            return await _dbContext.Books.ToListAsync();
        }
        [HttpGet("{id}")]
        public async Task<BookModel?> GetBookAsync(int id)
        {
            var book = await _dbContext.Books.Include(g => g.Genre).FirstOrDefaultAsync(x => x.Id == id);

            if (book == null) return null;

            return new BookModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                PublishedDate = book.PublishedDate,
                IsAvailable = book.IsAvailable,
                Price = (int)book.Price,
                AvailableCopies = book.AvailableCopies,
                Genre = book.Genre != null ? new GenresModel
                {
                    GenreID = book.Genre.GenreID,
                    GenreName = book.Genre.GenreName,
                    Description = book.Genre.Description,
                } : null,
                Image = book.Image
            };
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookModel>>> SearchBooksByName([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest("Keyword is required for searching.");
            }

            var books = await _dbContext.Books
                .Include(b => b.Genre)
                .Where(b => EF.Functions.Like(b.Title, $"%{keyword}%"))
                .Select(book => new BookModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    PublishedDate = book.PublishedDate,
                    IsAvailable = book.IsAvailable,
                    Price = (int)book.Price,
                    AvailableCopies = book.AvailableCopies,
                    Genre = book.Genre != null ? new GenresModel
                    {
                        GenreID = book.Genre.GenreID,
                        GenreName = book.Genre.GenreName,
                        Description = book.Genre.Description,
                    } : null,
                    Image = book.Image
                })
                .ToListAsync();

            if (books.Count == 0)
            {
                return NotFound("No books found matching the search keyword.");
            }

            return Ok(books);
        }

        [HttpGet("genre/{genreId}")]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetBooksByGenreId(int genreId)
        {
            // Kiểm tra xem GenreID có tồn tại không
            var genreExists = await _dbContext.Genres.AnyAsync(g => g.GenreID == genreId);
            if (!genreExists)
            {
                return NotFound($"Genre with ID {genreId} not found.");
            }

            // Lấy danh sách sách theo GenreID
            var books = await _dbContext.Books
                .Include(b => b.Genre)
                .Where(b => b.GenreID == genreId)
                .Select(book => new BookModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    PublishedDate = book.PublishedDate,
                    IsAvailable = book.IsAvailable,
                    Price = (int)book.Price,
                    AvailableCopies = book.AvailableCopies,
                    Genre = book.Genre != null ? new GenresModel
                    {
                        GenreID = book.Genre.GenreID,
                        GenreName = book.Genre.GenreName,
                        Description = book.Genre.Description,
                    } : null,
                    Image = book.Image
                })
                .ToListAsync();

            if (books.Count == 0)
            {
                return NotFound($"No books found for Genre ID {genreId}.");
            }

            return Ok(books);
        }


    }
}
