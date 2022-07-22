using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCore_DBLibrary;
using InventoryModels.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryDatabaseLayer
{
    public class CategoriesRepo : ICategoriesRepo
    {
        private readonly IMapper _mapper;
        private readonly InventoryDbContext _context;

        public CategoriesRepo(InventoryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public List<CategoryDto> ListCategoriesAndDetails()
        {
            return _context.Categories.Include(x => x.CategoryDetail)
                .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
                .ToList();
        }
    }
}
