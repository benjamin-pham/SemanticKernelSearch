using Microsoft.Extensions.AI;
using WebHost.Repositories;

namespace WebHost.Endpoints;

public static class SearchSimilarProductEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/products/search-similar", async (SearchSimilarProductRequest request,
            ProductRepository productRepository,
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) =>
        {
            var embedding = await embeddingGenerator.GenerateAsync(request.Text);
            var normalizeVector = NormalizeVector.Handle(embedding.Vector.ToArray());
            var dataResult = await productRepository.SearchSimilarAsync(normalizeVector);
            var apiResult = dataResult.Select((product, index) =>
                new SearchSimilarProductResponse(
                    index,
                    product.Id,
                    product.Name,
                    product.Similarity))
            .ToList();
            return apiResult;
        }).WithTags("Product");
    }
    public record SearchSimilarProductRequest(string Text);
    public record SearchSimilarProductResponse(int Index, int Id, string Name, double Similarity);
}
