using BookReviewApp.Models.ViewModels.Api;
using BookReviewApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReviewApp.Controllers.Api
{
    [Route("api/[controller]")]
    public class BooksController : BaseController
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
            : base(logger)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks(
            [FromQuery] string? genre = null,
            [FromQuery] int? year = null,
            [FromQuery] double? minRating = null)
        {
            try
            {
                var books = await _bookService.GetAllBooksAsync(genre, year, minRating);
                return Ok(books);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<BookDto>>(ex, "fetching books");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);

                if (book == null)
                {
                    return HandleNotFound<BookDto>("Book", id);
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                return HandleException<BookDto>(ex, $"fetching book {id}");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto createBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = await _bookService.CreateBookAsync(createBookDto);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                return HandleException<BookDto>(ex, "creating book");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<BookDto>> UpdateBook(int id, [FromBody] CreateBookDto updateBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = await _bookService.UpdateBookAsync(id, updateBookDto);

                if (book == null)
                {
                    return HandleNotFound<BookDto>("Book", id);
                }

                return Ok(book);
            }
            catch (Exception ex)
            {
                return HandleException<BookDto>(ex, $"updating book {id}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(id);

                if (!result)
                {
                    return HandleNotFound("Book", id);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex, $"deleting book {id}");
            }
        }
    }
}