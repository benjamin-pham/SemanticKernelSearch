using Microsoft.Extensions.AI;
using WebHost.Repositories;

namespace WebHost.Endpoints;

public static class GetProductDetailEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/products/get-detail", async (GetProductDetailRequest request,
            ProductRepository productRepository) =>
        {
            var dataResult = await productRepository.GetProductDetail(request.Id);
            if (dataResult == null)
                throw new Exception("not found");
            return new GetProductDetailResponse(
                dataResult.Id,
                dataResult.Name,
                dataResult.Description,
                dataResult.Tags);
        }).WithTags("Product");
    }
    public record GetProductDetailRequest(int Id);
    public record GetProductDetailResponse(int Id, string Name, string Description, string[] Tags);
}
