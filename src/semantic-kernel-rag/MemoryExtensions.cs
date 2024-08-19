using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

internal static class MemoryExtensions
{
    public static async Task AddFactsAsync(
        this ISemanticTextMemory memory,
        string collectionName,
        IEnumerable<Fact> facts,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(collectionName, nameof(collectionName));
        ArgumentNullException.ThrowIfNull(facts, nameof(facts));

        var tasks = facts.Select(f => memory.SaveInformationAsync(collection: collectionName,
            id: f.Id,
            text: f.Text,
            description: f.Description,
            additionalMetadata: f.Metadata,
            kernel: kernel,
            cancellationToken: cancellationToken));
        await Task.WhenAll(tasks)
                  .ConfigureAwait(false);
    }
}