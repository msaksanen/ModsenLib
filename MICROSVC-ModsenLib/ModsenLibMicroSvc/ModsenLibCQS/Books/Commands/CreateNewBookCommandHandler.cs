using AutoMapper;
using MediatR;
using ModsenLibDb;
using ModsenLibDb.Enitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Commands
{
    public class CreateNewBookCommandHandler : IRequestHandler<CreateNewBookCommand, int?>
    {
        private readonly ModsenLibAPIContext _context;
        private readonly IMapper _mapper;

        public CreateNewBookCommandHandler(ModsenLibAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(CreateNewBookCommand cmd, CancellationToken cts)
        {
            if (_context.Books != null && _context.BookPassports!=null)
            {
                var entity = _mapper.Map<Book>(cmd.BookDto);

                if (entity != null)
                {
                    await _context.Books.AddAsync(entity, cts);
                    var bookPass = new BookPassport()
                    {
                        Id = Guid.NewGuid(),
                        BookId = entity.Id,
                        Book = entity,
                        IsTaken = false
                    };
                    await _context.BookPassports.AddAsync(bookPass, cts);
                }

            }
            var res = await _context.SaveChangesAsync(cts);
            return res;
        }
    }
}
