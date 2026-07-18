using Nest;
using Search.API.Models;

namespace Search.API.Services;

public class ElasticsearchService
{
    private readonly IElasticClient _client;
    private const string IndexName = "products";

    public ElasticsearchService(IElasticClient client)
    {
        _client = client;
        CreateIndexIfNotExists();
    }

    private void CreateIndexIfNotExists()
    {
        var existsResponse = _client.Indices.Exists(IndexName);

        if (!existsResponse.Exists)
        {
            _client.Indices.Create(IndexName, c => c
                .Map<ProductDocument>(m => m
                    .AutoMap()));
        }
    }

    public async Task IndexProductAsync(ProductDocument product)
    {
        await _client.IndexDocumentAsync(product);
    }

    /*
        ✅ Elasticsearch Setup
        ✅ ProductCreatedEvent Consumer
        ✅ Fuzzy Search
        ✅ Wildcard Search
        ✅ Prefix Search
        ✅ Pagination 
        ✅ Boost
    */
    public async Task<IEnumerable<ProductDocument>> SearchAsync(string query, int size = 400)
    {
        var response = await _client.SearchAsync<ProductDocument>(s => s
            .Index(IndexName)
            .Size(size)
            .Query(q => q
                .Bool(b => b
                    .Should(
                        // Fuzzy - Anywhere in the word with typo tolerance
                        sh => sh.MultiMatch(m => m
                            .Fields(f => f
                                .Field(p => p.Name, boost: 3)
                                .Field(p => p.Description, boost: 1)
                                .Field(p => p.Category, boost: 2))
                            .Query(query)
                            .Fuzziness(Fuzziness.Auto)
                            .PrefixLength(1)),

                        // Wildcard - Anywhere in the word with *query*
                        sh => sh.Wildcard(w => w
                            .Field(p => p.Name)
                            .Value($"*{query.ToLower()}*")),

                        sh => sh.Wildcard(w => w
                            .Field(p => p.Category)
                            .Value($"*{query.ToLower()}*")),
                        // Prefix - Start of the word
                        sh => sh.Prefix(p => p
                            .Field(f => f.Name)
                            .Value(query.ToLower()))
                    )
                    .MinimumShouldMatch(1)
                )
            )
            .Sort(ss => ss
                .Descending(SortSpecialField.Score))
        );

        return response.Documents;
    }
}