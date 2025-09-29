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
            var dataResult = await productRepository.SearchSimilarAsync(embedding.Vector.ToArray());
            var apiResult = dataResult.Select((product, index) =>
                new SearchSimilarProductResponse(
                    Index: index,
                    Id: product.Id,
                    Name: product.Name,
                    Similarity: product.Similarity))
            .ToList();
            return apiResult;
        }).WithTags("Product");
    }
    public record SearchSimilarProductRequest(string Text);
    public record SearchSimilarProductResponse(int Index, int Id, string Name, double Similarity);
}
