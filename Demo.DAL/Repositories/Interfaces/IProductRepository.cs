using Demo.DAL.Dto;

namespace Demo.DAL.Repositories.Interfaces;

public interface IProductRepository
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(ProductCreateDto dto);
    Task<bool> UpdateAsync(int id, ProductCreateDto dto);
    Task<bool> DeleteAsync(int id);
}