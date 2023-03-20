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
using System.Diagnostics;

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
                if (result == null || !result.Any())
                    return NotFound();
                result.ForEach(b => b.BookPassportDto = null); //open method, passport contains confidential data
                return Ok(result);
            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        /// <summary>
        /// Return book by Id
        /// </summary>
        /// <param name="id string? [FromQuery]"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ReturnBookById([FromQuery] string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { Message = "Book Id is null, empty or whitespace" });

            var headers = HttpContext.Request.Headers;
            var resUserId = headers.TryGetValue("UserId", out var UserId);
            var resUserEmail = headers.TryGetValue("UserEmail", out var UserEmail);
            if (!resUserId && !resUserEmail)
                return BadRequest(new { Message = "Unable read user Id & Email" });

            var resUId = Guid.TryParse(UserId, out var UId);
            if (!resUId)
                return BadRequest(new { Message = "User Id is invalid" });

            Guid bookId = Guid.Empty;
            var resCmd = new Result();

            try
            {
                    var res = Guid.TryParse(id, out bookId);
                    if (!res)
                    {
                        return BadRequest(new { Message = "Book Id is invalid" });
                    }
                    else
                    {
                        resCmd = await _mediator.Send(new ReturnBookByIdCommand() { BookId = bookId, UserId = UId});
                    }

                if (resCmd == null)
                    return NotFound(new { Message = "Book or Dbase is not found" });

                if (resCmd.IntResult==0)
                    return new ObjectResult(new {Message = "Nobody has taken the book" });

                if (resCmd.IntResult == 1)
                    return new ObjectResult(new { Message = "The user hasn't taken the book" });

                if (resCmd.IntResult == 2)
                    return new ObjectResult(new { Message = "The user has returned the book already" });

                if (resCmd.IntResult == 3 && resCmd.SaveChangesResult>0)
                    return new ObjectResult(new 
                    { 
                        Message = "The user has returned the book in time",
                        SysInfo = "Data has been modified successfully"
                    });

                if (resCmd.IntResult == 3 && resCmd.SaveChangesResult < 0)
                    return new ObjectResult(new
                    {
                        Message = "The user has returned the book in time",
                        SysInfo = "Data has not been modified successfully"
                    });

                if (resCmd.IntResult == 4 && resCmd.SaveChangesResult > 0)
                    return new ObjectResult(new
                    {
                        Message = "The user has returned the book late",
                        SysInfo = "Data has been modified successfully"
                    });

                if (resCmd.IntResult == 4 && resCmd.SaveChangesResult < 0)
                    return new ObjectResult(new
                    {
                        Message = "The user has returned the book late",
                        SysInfo = "Data has not been modified successfully"
                    });
                return BadRequest(new { Message = "Something went wrong" });
            }

            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }

        }

        /// <summary>
        /// Get book by Id or ISBN
        /// </summary>
        /// <param name="id string? [FromQuery]"></param>
        /// <param name="isbn string? [FromQuery]"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBookByIdISBN([FromQuery] string? id, string? isbn)
        {
            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(isbn))
                return BadRequest(new { Message = "Book Id/ISBN is null, empty or whitespace" });
            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(isbn))
                return BadRequest(new { Message = "Unable to proceed. Input either Id or ISBN"});

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

            Guid bookId = Guid.Empty;
            var bookResult = new BookDto();

            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var res = Guid.TryParse(id, out bookId);
                    if (!res)
                    {
                        return BadRequest(new { Message = "Book Id is invalid" });
                    }
                    else
                    {
                        bookResult = await _mediator.Send(new GetBookByIdQuery() { BookId = bookId });
                    }
                }
               if (!string.IsNullOrWhiteSpace(isbn))
               {
                    int checkISBN = CheckISBN(isbn);
                    if (checkISBN == 1)
                    {
                        return BadRequest(new { Message = "Book ISBN  format is invalid" });
                    }
                    else
                    {
                        bookResult = await _mediator.Send(new GetBookByISBNQuery() { BookISBN = isbn });
                    }
               }

               if (bookResult == null)
                    return NotFound(new { Message = "Book is not found or has been taken" });

                var returnDate = DateTime.Today.AddDays(BookPeriod);
                var takeRes = await _mediator.Send(new ChangeBookUserDataCommand()
                { BookId = bookResult.Id, UserEmail = UserEmail, UserId = UId , ReturnDate = returnDate });
                if (takeRes > 0)
                    return Ok(new
                    {
                        bookResult,
                        Message = "User data has been saved in the library",
                        ReturnDate = $"{returnDate}"
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

            if (string.IsNullOrWhiteSpace(isbn))
                return 0;

            if(regexISBN10.IsMatch(isbn) || regexISBN13.IsMatch(isbn))
                return 2;
            else
                return 1; //failed check
        }

    }
}
