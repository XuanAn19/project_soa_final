using BorrowBooks.DTOs;
using BorrowBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;

namespace BorrowBooks.Controllers
{
    [Route("api/borrow")]
    [ApiController]
    public class BorrowControllers : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly BorrowDbContext _dataContext;
        public BorrowControllers(IHttpClientFactory httpClient, BorrowDbContext borrowService)
        {
            _httpClient = httpClient.CreateClient("BookService");
            _dataContext = borrowService;
        }
        [HttpGet]
        public async Task<List<BookDTO>> GetProductsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/BookApi");
            //request.Headers.Add("Authorization", "Bearer " + token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<BookDTO>>(result);
            return products;

        }
      
        private async Task<BookDTO?> GetBookByIdAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/BookApi/{id}");
            //request.Headers.Add("Authorization", "Bearer " + token);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<BookDTO>(result);
            return book;
        }
        private async Task<UserDTO?> GetUserByIdAsync(string id)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://localhost:7000/api/v1/user/getUserById/{id}");
            request.Headers.Add("Authorization",token);
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Không thể lấy thông tin người dùng.");
            }

            var result = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(result);

            // Lấy dữ liệu người dùng từ ApiResponse
            var user = apiResponse?.Data;

            return user;
        }

      

        [HttpGet("getAll")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<BorrowModel>>> GetAllBorrows()
        {
            // Lấy tất cả các bản ghi mượn từ cơ sở dữ liệu
            var borrows = await _dataContext.Borrows.ToListAsync();

            if (borrows == null || borrows.Count == 0)
            {
                return NotFound(new { message = "Không có bản ghi mượn sách nào." });
            }

            // Tạo danh sách BorrowModel để lưu trữ thông tin mượn
            var borrowDetails = new List<BorrowModel>();

            foreach (var borrow in borrows)
            {
                var user =await GetUserByIdAsync(borrow.UserId);
                var fullname = user?.FullName;
                // Thêm thông tin vào BorrowModel
                borrowDetails.Add(new BorrowModel
                {
                    Id = borrow.Id,
                    BookId = borrow.BookId,
                    BookName = borrow.BookName,
                    UserId = borrow.UserId,
                    FullName = fullname,
                    IsTrue = borrow.IsTrue,
                    BorrowDate = borrow.BorrowDate,
                    ReturnDate = borrow.ReturnDate
                });
            }

            return Ok(borrowDetails);
        }

        public class BorrowBookRequest
        {
            public int IdBook { get; set; }
            public int BorrowCount { get; set; }
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> BorrowBookAsync([FromBody] BorrowBookRequest bb)
        {
            // Lấy thông tin người dùng từ HttpContext (đã được gán từ middleware)
            var userId = HttpContext.Items["UserId"]?.ToString();
            var fullName = HttpContext.Items["FullName"]?.ToString();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
            {
                return Unauthorized(new { message = "Thông tin người dùng không hợp lệ" });
            }

            // Lấy thông tin sách từ API sách bên ngoài
            var book = await GetBookByIdAsync(bb.IdBook); // Hàm GetBookByIdAsync cần được viết trước đó để lấy thông tin sách

            if (book == null)
            {
                return NotFound(new { message = "Sách không tồn tại" });
            }

            // Kiểm tra xem số lượng sách có đủ để mượn không
            if (book.AvailableCopies < bb.BorrowCount)
            {
                return BadRequest(new { message = "Không đủ sách để mượn" });
            }

            // Cập nhật số lượng sách trong hệ thống bên ngoài (API sách)
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/BookApi/borrow/{bb.IdBook}");
            request.Content = new StringContent(JsonConvert.SerializeObject(bb.BorrowCount), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = "Cập nhật số lượng sách lỗi", error = errorContent });
            }

            // Lưu thông tin mượn sách vào cơ sở dữ liệu của bạn
            var borrow = new BorrowModel
            {
                BookId = bb.IdBook,
                BookName = book.Title,
                UserId = userId,
                FullName = fullName,
                IsTrue = false, // Trạng thái mượn chưa trả
                BorrowDate = DateTime.Now
            };

            _dataContext.Borrows.Add(borrow);
            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi lưu dữ liệu", error = ex.InnerException?.Message ?? ex.Message });
            }


            return Ok(new
            {
                Message = "Mượn sách thành công",
                BookName = book.Title,
                BorrowCount = bb.BorrowCount,
                RemainingCopies = book.AvailableCopies - bb.BorrowCount
            });
        }

        [HttpPut("return/{borrowId}")]
        [Authorize(Roles = "admin")] // Chỉ admin có thể trả sách
        public async Task<IActionResult> ReturnBookAsync(int borrowId)
        {
            var borrow = await _dataContext.Borrows.FirstOrDefaultAsync(b => b.Id == borrowId);

            if (borrow == null)
            {
                return NotFound(new { message = "Bản ghi mượn sách không tồn tại" });
            }

            // Cập nhật trạng thái trả sách
            borrow.IsTrue = true;
            borrow.ReturnDate = DateTime.Now;

            // Lấy thông tin sách từ API bên ngoài
            var book = await GetBookByIdAsync(borrow.BookId);

            if (book == null)
            {
                return NotFound(new { message = "Sách không tồn tại trong hệ thống" });
            }

            // Cập nhật số lượng sách có sẵn trong API sách
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/BookApi/return/{borrow.BookId}");
            request.Content = new StringContent(JsonConvert.SerializeObject(1), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = "Cập nhật số lượng sách lỗi", error = errorContent });
            }

            _dataContext.Borrows.Update(borrow);
            await _dataContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Trả sách thành công",
                BookName = book.Title,
                RemainingCopies = book.AvailableCopies + 1 // Tăng số lượng sách có sẵn
            });
        }


        [HttpDelete("borrow/{borrowId}")]
        [Authorize(Roles = "admin")] // Chỉ admin có thể xóa bản ghi mượn
        public async Task<IActionResult> DeleteBorrowRecordAsync(int borrowId)
        {
            var borrow = await _dataContext.Borrows.FirstOrDefaultAsync(b => b.Id == borrowId);

            if (borrow == null)
            {
                return NotFound(new { message = "Bản ghi mượn sách không tồn tại" });
            }

            // Lấy thông tin sách từ API bên ngoài
            var book = await GetBookByIdAsync(borrow.BookId);

            if (book == null)
            {
                return NotFound(new { message = "Sách không tồn tại trong hệ thống" });
            }

            // Cập nhật số lượng sách có sẵn trong API sách (tăng lại số lượng sách)
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/BookApi/return/{borrow.BookId}");
            request.Content = new StringContent(JsonConvert.SerializeObject(1), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = "Cập nhật số lượng sách lỗi", error = errorContent });
            }

            // Xóa bản ghi mượn sách
            _dataContext.Borrows.Remove(borrow);
            await _dataContext.SaveChangesAsync();

            return Ok(new { message = "Bản ghi mượn sách đã được xóa thành công" });
        }


        [HttpGet("getBorrowByIdUser")]
        public async Task<ActionResult<List<BorrowModel>>> GetBorrowsByUser()
        {

            var userId = HttpContext.Items["UserId"]?.ToString();

            var borrows = await _dataContext.Borrows
                                             .Where(b => b.UserId == userId)
                                             .ToListAsync();

            // Tạo danh sách BorrowModel để lưu trữ thông tin mượn
            var borrowDetails = new List<BorrowModel>();

            foreach (var borrow in borrows)
            {
                var user = await GetUserByIdAsync(borrow.UserId);
                var fullname = user?.FullName;

                // Thêm thông tin vào BorrowModel
                borrowDetails.Add(new BorrowModel
                {
                    Id = borrow.Id,
                    BookId = borrow.BookId,
                    BookName = borrow.BookName,
                    UserId = borrow.UserId,
                    FullName = fullname,
                    IsTrue = borrow.IsTrue,
                    BorrowDate = borrow.BorrowDate,
                    ReturnDate = borrow.ReturnDate
                });
            }

            return Ok(borrowDetails);
        }

    }
}
