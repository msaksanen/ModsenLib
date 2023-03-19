using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModsenLibCQS.Books.Queries;
using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Security.Claims;
using Azure;
using MediatR;
using System.Text.RegularExpressions;
using ModsenLibAbstractions.DataTransferObjects;
using ModsenLibCQS.Books.Commands;
using ModsenLibAPI.Models;
using System.Net;

namespace ModsenLibAPI.Controllers
{   /// <summary>
    /// Book Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Book Controller Ctor
        /// </summary>
        public BookController(IMapper mapper, IMediator mediator, IConfiguration configuration)
        {
            _mapper = mapper;
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Get book list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBookList()
        {
            try
            {
                var result = await _mediator.Send(new GetBookListQuery() { });
                return Ok(result);
            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        /// <summary>
        /// Get book by Id
        /// </summary>
        /// <param name="id string? [FromQuery]"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBookById([FromQuery] string? id)
        {
            var res = Guid.TryParse(id, out var bookId);
            if (!res)
            {
                return BadRequest(new { Message = "Book Id is invalid" });
            }
            var headers = HttpContext.Request.Headers;
            var resUserId = headers.TryGetValue("UserId", out var UserId);
            var resUserEmail = headers.TryGetValue("UserEmail", out var UserEmail);
            if (!resUserId && !resUserEmail)
                return BadRequest(new { Message = "Unable read user Id & Email" });

            var resUId = Guid.TryParse(UserId, out var UId);
            if (!resUId)
                return BadRequest(new { Message = "User Id is invalid" });

            int BookPeriod = 7;
            if (int.TryParse(_configuration["Lib:BookPeriod"], out int resb))
                BookPeriod = resb;
            try
            {
                var bookResult = await _mediator.Send(new GetBookByIdQuery() { BookId = bookId });
                if (bookResult == null)
                    return NotFound(new { Message = "Book is not found or has been taken" });

                var takeRes = await _mediator.Send(new ChangeBookUserDataCommand()
                { BookId = bookResult.Id, BookPeriod = BookPeriod, UserEmail = UserEmail, UserId = UId });
                if (takeRes > 0)
                    return Ok(new
                    {
                        bookResult,
                        Message = "User data has been saved in the library",
                        ReturnDate = $"{DateTime.Today.AddDays(BookPeriod)}"
                    });
                else
                    return new ObjectResult(new { bookResult, Message = "User data has not been saved in the library" });
            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }
        /// <summary>
        /// Get book By ISBN
        /// </summary>
        /// <param name="isbn string? [FromQuery]"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBookByISBN([FromQuery] string? isbn)
        {
            int checkISBN = CheckISBN(isbn);

            if (checkISBN == 0)
                return BadRequest(new { Message = "Book ISBN is null or empty" });

            if (checkISBN == 1)
                return BadRequest(new { Message = "Book ISBN  format is invalid" });

            var headers = HttpContext.Request.Headers;
            var resUserId = headers.TryGetValue("UserId", out var UserId);
            var resUserEmail = headers.TryGetValue("UserEmail", out var UserEmail);
            if (!resUserId && !resUserEmail)
                return BadRequest(new { Message = "Unable read user Id & Email" });

            var resUId = Guid.TryParse(UserId, out var UId);
            if (!resUId)
                return BadRequest(new { Message = "User Id is invalid" });

            int BookPeriod = 7;
            if (int.TryParse(_configuration["Lib:BookPeriod"], out int res))
                BookPeriod = res;

            //var ISBN10 = _configuration["ISBN:ISBN10"];
            //var ISBN13 = _configuration["ISBN:ISBN13"];
            //if (ISBN10 == null && ISBN13==null)
            //    return BadRequest(new { Message = "No ISBN pattern is provided" });

            //if (!(Regex.IsMatch(isbn, ISBN10!, RegexOptions.IgnoreCase) || (Regex.IsMatch(isbn, ISBN13!, RegexOptions.IgnoreCase))))
            //    return BadRequest(new { Message = "Book ISBN is invalid" });

            try
            {
                var bookResult = await _mediator.Send(new GetBookByISBNQuery() { BookISBN = isbn });
                if (bookResult == null)
                    return NotFound(new { Message = "Book is not found or has been taken" });

                var takeRes = await _mediator.Send(new ChangeBookUserDataCommand() 
                                    {BookId = bookResult.Id, BookPeriod = BookPeriod, UserEmail = UserEmail, UserId = UId});
                if (takeRes>0)
                    return Ok(new { bookResult, Message = "User data has been saved in the library", 
                                    ReturnDate = $"{DateTime.Today.AddDays(BookPeriod)}" });
                else
                    return new ObjectResult(new { bookResult, Message= "User data has not been saved in the library" });
            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        /// <summary>
        /// Add new book to the library
        /// </summary>
        /// <param name="model AddBookRequestModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddNewBook([FromBody] AddBookRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Book data is invalid" });

            var headers = HttpContext.Request.Headers;
            var resRoles = headers.TryGetValue("UserRole", out var Roles);
            if (!resRoles)
                return BadRequest(new { Message = "Unable read user roles" });

            var isAdmin = Roles.ToString().Contains("Admin", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
                return BadRequest(new { Message = "Book data addition is enabled for administrators only" });

            var newbook = _mapper.Map<BookDto>(model);

            try
            {
                var result = await _mediator.Send(new CreateNewBookCommand() { BookDto = newbook });

                if (result > 0)
                    return Ok(new { Message = "Book has been added to the library" });
                else
                    return BadRequest(new { Message = "Book has not been added to the library" });

            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }


        }

        /// <summary>
        /// Remove a book by Id from the library
        /// </summary>
        /// <param name="model RemoveBookModel"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> RemoveBookById([FromBody] RemoveBookModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Book Id is invalid" });

            var headers = HttpContext.Request.Headers;
            var resRoles = headers.TryGetValue("UserRole", out var Roles);
            if (!resRoles)
                return BadRequest(new { Message = "Unable read user roles" });

            var isAdmin = Roles.ToString().Contains("Admin", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
                return BadRequest(new { Message = "Book data removal is enabled for administrators only" });

            try
            {
                var result = await _mediator.Send(new RemoveBookCommand() { BookId = model.Id });

                if (result > 0)
                    return Ok(new { Message = "Book has been removed from the library" });
                else
                    return BadRequest(new { Message = "Book has not been removed from the library" });

            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }


        }

        /// <summary>
        /// Modifiy book's data
        /// </summary>
        /// <param name="model EditBookRequestModel [FromBody] "></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> BookEdit([FromBody] EditBookRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Book data is invalid" });

            var headers = HttpContext.Request.Headers;
            var resRoles = headers.TryGetValue("UserRole", out var Roles);
            if (!resRoles)
                return BadRequest(new { Message = "Unable read user roles" });

            var isAdmin = Roles.ToString().Contains("Admin", StringComparison.OrdinalIgnoreCase);
            if (!isAdmin)
                return BadRequest(new { Message = "Book data editing is enabled for administrators only" });

            var modBook = _mapper.Map<BookDto>(model);

            try
            {
                var result = await _mediator.Send(new BookEditCommand() { BookDto = modBook });

                if (result > 0)
                    return Ok(new { Message = "Book data has been modified" });
                else
                    return BadRequest(new { Message = "Book data has not been modified" });

            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }


        }

        private int CheckISBN (string? isbn)
        {
            Regex regexISBN10 = new Regex(@"\d{1,5}-\d{1,8}-\d{1,6}-[0-9]");          // 2-266-11156-6 
            Regex regexISBN13 = new Regex(@"\d{1,3}-\d{1,5}-\d{1,8}-\d{1,6}-[0-9]");  // 978-2-266-11156-6 

            if (string.IsNullOrEmpty(isbn) || string.IsNullOrWhiteSpace(isbn))
                return 0;

            if(regexISBN10.IsMatch(isbn) || regexISBN13.IsMatch(isbn))
                return 2;
            else
                return 1; //failed check
        }

    }
}
