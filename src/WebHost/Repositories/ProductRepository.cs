using Dapper;
using Npgsql;

namespace WebHost.Repositories;

public class ProductRepository(string connString)
{
    public async Task<int> InsertAsync(CreateProductModel model)
    {
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        const string sql = """
            INSERT INTO products (name, description, tags, embedding_name, embedding_description, embedding_tags)
            VALUES (@name, @description, @tags, @embedding_name, @embedding_description, @embedding_tags)
            RETURNING id;
        """;

        var id = await conn.ExecuteScalarAsync<int>(sql, new
        {
            name = model.Name,
            description = model.Description,
            tags = model.Tags,
            embedding_name = model.EmbeddingName,
            embedding_description = model.EmbeddingDescription,
            embedding_tags = model.EmbeddingTags
        });

        return id;
    }
    public async Task<List<SearchProductModel>> SearchSimilarAsync(float[] queryEmbedding, int limit = 10)
    {
        var vectorLiteral = $"[{string.Join(",", queryEmbedding)}]";
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        const string sql = """
            WITH tbl AS (
                SELECT id, name,         
                    (0.5 * (1 - (embedding_name <=> @vec::vector)) +
                    0.3 * (1 - (embedding_description <=> @vec::vector)) +
                    0.2 * (1 - (embedding_tags <=> @vec::vector))) AS similarity
                FROM products
            )
            SELECT id, name, similarity
            FROM tbl
            ORDER BY similarity DESC
            LIMIT @limit;
        """;

        //const string sql = """
        //    WITH tbl AS (
        //        SELECT id, name,         
        //            embedding_name <=> @vec::vector as dist_name,
        //            embedding_description <=> @vec::vector AS dist_desc,
        //            embedding_tags <=> @vec::vector AS dist_tags
        //        FROM products
        //    )
        //    SELECT id, name
        //    FROM tbl
        //    ORDER BY LEAST(dist_name, dist_desc, dist_tags)
        //    LIMIT @limit;
        //""";

        var results = (await conn.QueryAsync<SearchProductModel>(sql, new
        {
            vec = vectorLiteral,
            limit
        })).ToList();

        return results;
    }
    public async Task<GetProductDetailModel?> GetProductDetail(int id)
    {
        await using var conn = new NpgsqlConnection(connString);
        await conn.OpenAsync();

        const string sql = @"
            SELECT id, name, description, tags
            FROM products
            WHERE id = @id;
        ";

        var product = await conn.QuerySingleOrDefaultAsync<GetProductDetailModel>(sql, new { id });
        return product;
    }
    public record CreateProductModel(
        string Name,
        string Description,
        string[] Tags,
        float[] EmbeddingName,
        float[] EmbeddingDescription,
        float[] EmbeddingTags);
    public record SearchProductModel(int Id, string Name, double Similarity);
    public record GetProductDetailModel(int Id, string Name, string Description, string[] Tags);
}
