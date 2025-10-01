using DotNetEnv;
using OllamaSharp;

Env.TraversePath().Load();

var uri = new Uri(Env.GetString("OLLAMA_URI"));

// Set to empty to pick from list at runtime
var defaultModel = ""; 
var ollama = new OllamaApiClient(uri: uri, defaultModel: defaultModel);

if (string.IsNullOrEmpty(defaultModel))
{
    var models = (await ollama.ListLocalModelsAsync()).ToList();
    Console.WriteLine("========== MODELS ==========");
    foreach (var model in models)
    {
        Console.WriteLine(model.Name);
    }
    Console.WriteLine("========== END MODELS ==========");
    Console.WriteLine();
    
    while (true)
    {
        Console.Write("Enter a model from list: ");
        var userInput = Console.ReadLine();
        if (string.IsNullOrEmpty(userInput)) continue;

        var model = models.Find(m => m.Name.StartsWith(userInput));
        if (model == null) continue;

        Console.WriteLine($"Selected Model: {model.Name}");
        Console.WriteLine();
        
        ollama.SelectedModel = model.Name;
        break;
    }
}

var chat = new Chat(ollama);

while (true)
{
    Console.Write("Chat > ");
    var message = Console.ReadLine();
    
    if (string.IsNullOrEmpty(message)) continue;
    if (message == "end") break;
    
    await foreach (var answerToken in chat.SendAsync(message))
        Console.Write(answerToken);
    Console.WriteLine();
}
