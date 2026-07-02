using AutoMapper;
using MediatR;
using Products.Application.DTOs;
using Products.Domain.Entities;
using Products.Domain.Interfaces;

namespace Products.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // بنستخدم الـ Factory Method من الـ Domain
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.Category
        );

        // بنحفظ عن طريق الـ Interface مش الـ Implementation
        await _repository.AddAsync(product);

        // بنحول من Entity لـ DTO
        return _mapper.Map<ProductDto>(product);
    }
}