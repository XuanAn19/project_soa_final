using LibraryManagementAPI.DTOs;
using LibraryManagementAPI.Models;
using LibraryManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BookApiController : ControllerBase
    {

        private readonly BookDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ICloudinaryService _cloudinaryService;
        public BookApiController(BookDbContext dbContext, IWebHostEnvironment hostEnvironment, ICloudinaryService cloudinaryService)
        {
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            _cloudinaryService = cloudinaryService;

        }
        //get : api/Bookapi
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

        [HttpPut("borrow/{id}")]
        [Authorize]
        public async Task<IActionResult> BorrowBook(int id)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
                return NotFound("Book not found");


            book.AvailableCopies -= 1;

            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Book borrowed successfully",
                RemainingCopies = book.AvailableCopies,
                IsAvailable = book.IsAvailable
            });
        }

        [HttpPut("return/{bookId}")]
        [Authorize(Roles = "admin")] 
        public async Task<IActionResult> ReturnBookAsync(int bookId)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound(new { message = "Sách không tồn tại" });
            }

            // Tăng số lượng sách khi trả sách
            book.AvailableCopies += 1;

            _dbContext.Books.Update(book);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Số lượng sách đã được cập nhật", RemainingCopies = book.AvailableCopies });
        }


        private async Task<string> UploadImagesToCloudinary(IFormFile files)
        {


            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(files.FileName.Split('.')[1]);
            var uploadResult = await _cloudinaryService.UploadImageAsync(files, uniqueFileName);
            if (uploadResult != null)
            {
                return uploadResult;
            }

            return null;
        }

        private string GetPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);//Chuyển chuỗi URL- đối tượng URI 
            var segments = uri.Segments;//segements. lấy các phần của URI thành chuỗi "/", "image.jpg"
            var publicIdWithExtension = segments[segments.Length - 1];//lấy phần cuối "image.jpg"
            var publicId = publicIdWithExtension.Split('.')[0];//tách tên file ra
            return publicId;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<BookModel>> CreateBook([FromForm] BookDTO bookDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid input data.",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // Kiểm tra GenreID
            var genre = await _dbContext.Genres.FindAsync(bookDto.GenreID);
            if (genre == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Genre with ID {bookDto.GenreID} does not exist."
                });
            }

            // Tải ảnh lên Cloudinary (nếu có)
            string? uploadedImageUrl = null;
            if (bookDto.ImageFile != null)
            {
                try
                {
                    uploadedImageUrl = await UploadImagesToCloudinary(bookDto.ImageFile);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Failed to upload image.",
                        error = ex.Message
                    });
                }
            }

            // Tạo đối tượng BookModel từ BookDTO
            var newBook = new BookModel
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                PublishedDate = bookDto.PublishedDate,
                IsAvailable = true,
                Price = bookDto.Price,
                AvailableCopies = bookDto.AvailableCopies,
                GenreID = bookDto.GenreID,
                Genre = genre,
                Image = uploadedImageUrl,
            };

            // Lưu vào database
            try
            {
                _dbContext.Books.Add(newBook);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Failed to save book to database.",
                    error = ex.Message
                });
            }

            // Trả về kết quả thành công
            return CreatedAtAction(nameof(GetBookAsync), new { id = newBook.Id }, new
            {
                success = true,
                message = "Book created successfully.",
                data = newBook
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<BookModel>> UpdateBook(int id, [FromForm] BookDTO bookDto)
        {
            // Kiểm tra sách có tồn tại không
            var existingBook = await _dbContext.Books.Include(b => b.Genre).FirstOrDefaultAsync(b => b.Id == id);
            if (existingBook == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            // Kiểm tra xem GenreID có tồn tại không
            var genre = await _dbContext.Genres.FindAsync(bookDto.GenreID);
            if (genre == null)
            {
                return BadRequest($"Genre with ID {bookDto.GenreID} does not exist.");
            }

            // Cập nhật thông tin sách
            existingBook.Title = bookDto.Title;
            existingBook.Author = bookDto.Author;
            existingBook.PublishedDate = bookDto.PublishedDate;
            existingBook.Price = bookDto.Price;
            existingBook.AvailableCopies = bookDto.AvailableCopies;
            existingBook.GenreID = bookDto.GenreID;
            existingBook.Genre = genre;

            // Xóa ảnh cũ và cập nhật ảnh mới nếu có file mới
            if (bookDto.ImageFile != null)
            {
                // Xóa ảnh cũ khỏi Cloudinary nếu đã tồn tại
                if (!string.IsNullOrEmpty(existingBook.Image))
                {
                    var publicId = GetPublicIdFromUrl(existingBook.Image);
                    await _cloudinaryService.DeleteImageAsync(publicId);
                }

                // Upload ảnh mới lên Cloudinary
                string? uploadedImageUrl = null;
                if (bookDto.ImageFile != null)
                {
                    var uniqueFileName = await UploadImagesToCloudinary(bookDto.ImageFile);
                    if (uniqueFileName != null)
                    {
                        uploadedImageUrl = uniqueFileName;
                    }

                }
                existingBook.Image = uploadedImageUrl;
            }

            // Lưu thay đổi
            await _dbContext.SaveChangesAsync();

            // Trả về đối tượng đã cập nhật
            return Ok(existingBook);
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            // Tìm sách trong cơ sở dữ liệu
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            // Nếu sách có ảnh, thực hiện xóa ảnh khỏi Cloudinary
            if (!string.IsNullOrEmpty(book.Image))
            {
                try
                {
                    var publicId = GetPublicIdFromUrl(book.Image);

                    // Gọi dịch vụ Cloudinary để xóa ảnh
                await _cloudinaryService.DeleteImageAsync(publicId);
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi và tiếp tục thực hiện xóa sách trong cơ sở dữ liệu
                    Console.WriteLine($"Error deleting image: {ex.Message}");
                }
            }

            // Xóa sách khỏi cơ sở dữ liệu
            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }



    }
}
