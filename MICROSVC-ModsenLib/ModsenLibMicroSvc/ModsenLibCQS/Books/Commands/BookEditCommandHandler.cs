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
    public class BookEditCommandHandler : IRequestHandler<BookEditCommand, int?>
    {
        private readonly ModsenLibAPIContext _context;
        private readonly IMapper _mapper;

        public BookEditCommandHandler(ModsenLibAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(BookEditCommand cmd, CancellationToken cts)
        {
            if (_context.Books != null && _context.Books.Any())
            {
                var entity = _mapper.Map<Book>(cmd.BookDto);

                if (entity != null)
                    _context.Books.Update(entity);
            }
            var res = await _context.SaveChangesAsync(cts);
            return res;
        }
    }
}
