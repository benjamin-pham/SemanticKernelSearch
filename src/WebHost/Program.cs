using Dapper;
using Microsoft.Extensions.AI;
using Scalar.AspNetCore;
using WebHost;
using WebHost.Endpoints;
using WebHost.Repositories;
var builder = WebApplication.CreateBuilder(args);
SqlMapper.AddTypeHandler(new StringArrayHandler());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
string connectionString = builder.Configuration.GetConnectionString("Database")!;
builder.Services.AddScoped(sp =>
{
    return new ProductRepository(connectionString);
});
builder.Services.AddHttpClient("EmbeddingApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["EmbeddingApi:BaseUrl"]!);
});
builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>, SemanticEmbeddingService>();
builder.Services.AddProblemDetails();
var app = builder.Build();

app.UseSwagger(options =>
{
    options.RouteTemplate = "/openapi/{documentName}.json";
});

app.MapScalarApiReference(options =>
{
    options.WithTitle("Semantic Kernel Search")
        .WithTheme(ScalarTheme.BluePlanet)
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios)
        .WithLayout(ScalarLayout.Modern);
});

app.UseHttpsRedirection();

InsertProductEndpoint.MapEndpoint(app);
SearchSimilarProductEndpoint.MapEndpoint(app);
GetProductDetailEndpoint.MapEndpoint(app);

app.Run();