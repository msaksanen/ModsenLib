using AutoMapper;
using MediatR;
using ModsenLibDb.Enitities;
using ModsenLibDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ModsenLibCQS.Books.Commands
{
    public class ChangeBookUserDataCommandHandler : IRequestHandler<ChangeBookUserDataCommand, int?>
    {
        private readonly ModsenLibAPIContext _context;
        private readonly IMapper _mapper;

        public ChangeBookUserDataCommandHandler(ModsenLibAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(ChangeBookUserDataCommand cmd, CancellationToken cts)
        {
            if (_context.BookPassports!=null)
            {
                var entity = await _context.BookPassports.FirstOrDefaultAsync(b => b.BookId.Equals(cmd.BookId),cts);

                if (entity == null)
                {
                    var newEnt = new BookPassport()
                    {
                        Id = Guid.NewGuid(),
                        BookId = cmd.BookId,
                        UserId = cmd.UserId,
                        UserEmail = cmd.UserEmail,
                        ReturnDate = cmd.ReturnDate,
                        TakenDate = DateTime.Today,
                        IsTaken = true
                    };
                    _context.BookPassports.Add(newEnt);
                }
                else                   
                {
                    entity.BookId = cmd.BookId;
                    entity.UserId = cmd.UserId;
                    entity.UserEmail = cmd.UserEmail;
                    entity.ReturnDate = cmd.ReturnDate;
                    entity.TakenDate = DateTime.Today;
                    entity.IsTaken = true;
                    _context.BookPassports.Update(entity);
                }

            }
            var res = await _context.SaveChangesAsync(cts);
            return res;
        }
    }
}
