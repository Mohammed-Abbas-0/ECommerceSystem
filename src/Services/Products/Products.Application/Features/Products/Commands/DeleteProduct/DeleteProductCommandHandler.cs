using MediatR;
using Products.Domain.Interfaces;

namespace Products.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;

    public DeleteProductCommandHandler(IProductRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);

        if (product is null)
            throw new KeyNotFoundException($"Product with Id {request.Id} not found");

        await _repository.DeleteAsync(request.Id);

        await _cache.RemoveAsync($"product:{request.Id}");

        return true;
    }
}