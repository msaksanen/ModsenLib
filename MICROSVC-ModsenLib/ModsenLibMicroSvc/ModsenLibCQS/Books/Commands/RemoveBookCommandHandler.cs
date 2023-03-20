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
    public class RemoveBookCommandHandler : IRequestHandler<RemoveBookCommand, int?>
    {
        private readonly ModsenLibAPIContext _context;
        private readonly IMapper _mapper;

        public RemoveBookCommandHandler(ModsenLibAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Handle(RemoveBookCommand cmd, CancellationToken cts)
        {
            if (_context.Books != null && _context.Books.Any())
            {
                var entity = await _context.Books
                            .FirstOrDefaultAsync(entity => entity.Id.Equals(cmd.BookId), cts);

                if (entity == null) return null;
                
               _context.Books.Remove(entity);  

            }
            var res = await _context.SaveChangesAsync(cts);
            return res;
        }
    }
}