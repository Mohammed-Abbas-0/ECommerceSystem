using MediatR;
using Products.Application.DTOs;

namespace Products.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string Category
) : IRequest<ProductDto>;