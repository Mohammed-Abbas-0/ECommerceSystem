using MediatR;
using Products.Application.DTOs;

namespace Products.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;