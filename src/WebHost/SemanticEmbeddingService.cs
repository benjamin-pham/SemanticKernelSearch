using Microsoft.Extensions.AI;

namespace WebHost;

public class SemanticEmbeddingService : IEmbeddingGenerator<string, Embedding<float>>
{
    private readonly HttpClient _http;
    public SemanticEmbeddingService(IHttpClientFactory httpFactory)
    {
        _http = httpFactory.CreateClient("EmbeddingApi");
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public async Task<GeneratedEmbeddings<Embedding<float>>> GenerateAsync(IEnumerable<string> values, EmbeddingGenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var payload = new { texts = values };
        var response = await _http.PostAsJsonAsync("/embed", payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken: cancellationToken);

        var embeddings = json!.Embeddings
            .Select(vec => new Embedding<float>(vec))
            .ToList();

        return [.. embeddings];
    }

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        throw new NotImplementedException();
    }

    private class EmbeddingResponse
    {
        public float[][] Embeddings { get; set; } = [];
    }
}
