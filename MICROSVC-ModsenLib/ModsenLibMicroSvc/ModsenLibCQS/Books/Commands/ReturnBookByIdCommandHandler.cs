using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ModsenLibAbstractions.DataTransferObjects;
using ModsenLibDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Commands
{
    public class ReturnBookByIdCommandHandler : IRequestHandler<ReturnBookByIdCommand, Result?>
    {
        private readonly ModsenLibAPIContext _context;
        private readonly IMapper _mapper;

        public ReturnBookByIdCommandHandler(ModsenLibAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result?> Handle(ReturnBookByIdCommand cmd, CancellationToken cts)
        {
            if (_context.Books != null && _context.Books.Any() && _context.BookPassports!=null)
            {
                var entity = await _context.Books
                            .Include(b=>b.BookPassport)
                            .FirstOrDefaultAsync(entity => entity.Id.Equals(cmd.BookId), cts);

                if (entity == null) return null;
                if (entity.BookPassport == null)
                    return new Result() {IntResult=0}; // nobody has taken the book
                if (entity.BookPassport != null && !entity.BookPassport.UserId.Equals(cmd.UserId))
                    return new Result() { IntResult = 1 }; //the user hasn't taken the book
                if (entity.BookPassport != null && entity.BookPassport.UserId.Equals(cmd.UserId) && entity.BookPassport.IsTaken == null)
                    return new Result() { IntResult = 1 };  //the user hasn't taken the book
                if (entity.BookPassport != null && entity.BookPassport.UserId.Equals(cmd.UserId) && entity.BookPassport.IsTaken==false)
                    return new Result() { IntResult = 2 }; //the user has returned the book already
                entity.BookPassport!.IsTaken = false;
                entity.BookPassport.UserId = null;
                entity.BookPassport.UserEmail = String.Empty;
                if (entity.BookPassport.ReturnDate >= DateTime.Today)
                {
                    entity.BookPassport.ReturnDate = null;
                    var res = await _context.SaveChangesAsync();
                    return new Result() { IntResult = 3, SaveChangesResult = res}; //the user has returned the book in time
                }
                else
                {
                    entity.BookPassport.ReturnDate = null;
                    var res = await _context.SaveChangesAsync();
                    return new Result() { IntResult = 4, SaveChangesResult = res }; //the user has returned the book later
                }

            }
            return null;
        }
    }
}