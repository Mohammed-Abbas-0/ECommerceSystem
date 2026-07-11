using AutoMapper;
using MassTransit;
using MediatR;
using Products.Application.DTOs;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using Shared.Events.Events;

namespace Products.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.Category
        );

        await _repository.AddAsync(product);

        //await _publishEndpoint.Publish(new ProductStockDecreasedEvent(
        //    product.Id,
        //    product.Name,
        //    product.Stock
        //));

        // Use It For save on ElasticSearch and other services, like notification service, etc.
        await _publishEndpoint.Publish(new ProductCreatedEvent(
         product.Id,
         product.Name,
         product.Description,
         product.Price,
         product.Stock,
         product.Category
     ));

        return _mapper.Map<ProductDto>(product);
    }
}