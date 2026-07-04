using AutoMapper;
using MediatR;
using Products.Application.DTOs;
using Products.Domain.Interfaces;

namespace Products.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IProductRepository repository, ICacheService cacheService, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"product:{request.Id}";

        var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);
        if (cachedProduct is not null)
            return cachedProduct;

        var product = await _repository.GetByIdAsync(request.Id);

        if (product is null)
            throw new KeyNotFoundException($"Product with Id {request.Id} not found");

        var productDto = _mapper.Map<ProductDto>(product);
        await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(10));
        return productDto;
    }
}