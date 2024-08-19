using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;

Console.WriteLine("Hello! Ask any question you want. Type 'exit' to close the program.");
Console.WriteLine("*************************************************\n\n");

var modelPath = @"C:\sources\aimodels\Phi-3-mini-4k-instruct-onnx\cpu_and_mobile\cpu-int4-rtn-block-32";
var modelId = "Phi-3-mini-4k-instruct-onnx";

var embeddingOnnxModelPath = @"C:\sources\aimodels\TaylorAI-bge-micro-v2\onnx\model.onnx";
var embeddingVocabPath = @"C:\sources\aimodels\TaylorAI-bge-micro-v2\vocab.txt";

var builder = Kernel.CreateBuilder();
builder.AddOnnxRuntimeGenAIChatCompletion(modelId: modelId, modelPath: modelPath);
builder.AddBertOnnxTextEmbeddingGeneration(onnxModelPath: embeddingOnnxModelPath, vocabPath: embeddingVocabPath);
    //.AddLocalTextEmbeddingGeneration(); // this seems to have an issue, see https://github.com/dotnet-smartcomponents/smartcomponents/issues/75
var kernel = builder.Build();

var embeddingGenerator = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
var memoryBuilder = new MemoryBuilder();
memoryBuilder.WithMemoryStore(new VolatileMemoryStore())
             .WithTextEmbeddingGeneration(embeddingGenerator);
var memory = memoryBuilder.Build();

string collectionName = "DavidGuidaFacts";
await memory.AddFactsAsync(collectionName, Facts.GetFacts(), kernel);

kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

while (true)
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write($"You > ");
    var question = Console.ReadLine()!.Trim();
    if (question.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    var prompt = @"
        Question: {{$input}}
        Answer the question using the memory content: {{Recall}}";

    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        Temperature = 0.75,
        MaxTokens = 200
    };

    var response = kernel.InvokePromptStreamingAsync(
        promptTemplate: prompt,
        arguments: new KernelArguments(openAIPromptExecutionSettings)
        {            
            { "collection", collectionName },
            { "input", question },
        });

    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("\nAI > ");

    string combinedResponse = string.Empty;
    await foreach (var message in response)
    {        
        Console.Write(message);
        combinedResponse += message;
    }

    Console.WriteLine();
}
