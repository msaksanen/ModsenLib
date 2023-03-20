﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ModsenLibAbstractions.DataTransferObjects;
using ModsenLibDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsenLibCQS.Books.Queries
{
    public class GetBookListQueryHandler : IRequestHandler<GetBookListQuery, List<BookDto>?>
    {
        private readonly ModsenLibAPIContext _context;
        private readonly IMapper _mapper;

        public GetBookListQueryHandler(ModsenLibAPIContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<BookDto>?> Handle(GetBookListQuery query, CancellationToken cts)
        {
            if (_context.Books != null && _context.Books.Any() && _context.BookPassports!=null)
            {
                var list = await _context.Books
                                 .AsNoTracking()
                                 .Include(b=> b.BookPassport)
                                 .Select(b => _mapper.Map<BookDto>(b))
                                 .ToListAsync(cts);
                return list;
            }
            return null;
        }
    }
}
