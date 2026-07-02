using MediatR;
using Products.Application.DTOs;

namespace Products.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category
) : IRequest<ProductDto>;