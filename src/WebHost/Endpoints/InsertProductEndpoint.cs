using Microsoft.Extensions.AI;
using System.Linq;
using WebHost.Repositories;
using static WebHost.Repositories.ProductRepository;

namespace WebHost.Endpoints;

public static class InsertProductEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost("/api/products/insert", async (InsertProductRequest request,
            ProductRepository productRepository,
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator) =>
        {
            var result = await embeddingGenerator.GenerateAsync([request.Name, request.Description, string.Join(", ", request.Tags)]);
            var embeddings = result.ToList();
            var nameEmbedding = embeddings[0].Vector.ToArray();
            var descriptionEmbedding = embeddings[1].Vector.ToArray();
            var tagsEmbedding = embeddings[2].Vector.ToArray();
            var productModel = new CreateProductModel(
                request.Name,
                request.Description,
                request.Tags,
                NormalizeVector.Handle(nameEmbedding),
                NormalizeVector.Handle(descriptionEmbedding),
                NormalizeVector.Handle(tagsEmbedding));
            await productRepository.InsertAsync(productModel);
        }).WithTags("Product");
    }
    public record InsertProductRequest(string Name, string Description, string[] Tags);
}
