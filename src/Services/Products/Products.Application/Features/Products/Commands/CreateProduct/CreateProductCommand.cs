using MediatR;
using Products.Application.DTOs;

namespace Products.Application.Features.Products.Commands.CreateProduct;

// الـ Command بيحمل البيانات المطلوبة
// وبيرجع ProductDto
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Category
) : IRequest<ProductDto>;