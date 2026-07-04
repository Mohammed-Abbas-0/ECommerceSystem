using AutoMapper;
using MediatR;
using Products.Application.DTOs;
using Products.Domain.Interfaces;

namespace Products.Application.Features.Products.Commands.UpdateProduct;

public class DeleteProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;
    public DeleteProductCommandHandler(IProductRepository repository, IMapper mapper, ICacheService cache)
    {
        _repository = repository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);

        if (product is null)
            throw new KeyNotFoundException($"Product with Id {request.Id} not found");

        product.Update(request.Name, request.Description, request.Price, request.Category);

        await _repository.UpdateAsync(product);

        await _cache.RemoveAsync($"product:{request.Id}");

        return _mapper.Map<ProductDto>(product);
    }
}