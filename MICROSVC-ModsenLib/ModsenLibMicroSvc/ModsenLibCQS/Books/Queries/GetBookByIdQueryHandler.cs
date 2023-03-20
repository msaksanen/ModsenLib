using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ModsenLibAbstractions.DataTransferObjects;
using ModsenLibDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Queries
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
    {
        private readonly ModsenLibAPIContext _context;
        private readonly IMapper _mapper;

        public GetBookByIdQueryHandler(ModsenLibAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BookDto?> Handle(GetBookByIdQuery request,
        CancellationToken cts)

        {
            if (_context.Books != null && _context.Books.Any() && _context.BookPassports != null)
            {
                var entity = await _context.Books
                             .AsNoTracking()
                             .Include(b => b.BookPassport)
                             .FirstOrDefaultAsync(entity => entity.Id.Equals(request.BookId), cts);

                if (entity == null) return null;
                if (entity.BookPassport != null && entity.BookPassport.IsTaken == true) return null;

                var dto = _mapper.Map<BookDto>(entity);
                return dto;
            }
            return null;
        }
    }
}
