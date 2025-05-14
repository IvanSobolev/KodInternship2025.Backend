using Demo.DAL.Dto;
using Demo.DAL.Models;
using Demo.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Demo.DAL.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly DemoDbContext _context;

    public ProductRepository(DemoDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        return await _context.Products
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price })
            .ToListAsync();
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return null;

        return new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price };
    }

    public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
    {
        var product = new Product { Name = dto.Name, Price = dto.Price };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price };
    }

    public async Task<bool> UpdateAsync(int id, ProductCreateDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        product.Name = dto.Name;
        product.Price = dto.Price;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}