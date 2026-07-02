using MediatR;
using Products.Application.DTOs;

namespace Products.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;