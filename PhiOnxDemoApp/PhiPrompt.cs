using Microsoft.ML.OnnxRuntimeGenAI;

namespace PhiOnxDemoApp;

public class PhiPrompt
{
    private readonly Model _model;
    private readonly Tokenizer _tokenizer;
    private readonly string _systemPrompt =
        @"As an AI assistant answer questions using a direct style.";

    public PhiPrompt(AppSettings settings)
    {
        _model = new Model(settings.ModelPath);
        _tokenizer = new Tokenizer(_model);
    }

    public void Run()
    {
        Console.WriteLine("Hello, how can I help you today?");

        while (true)
        {
            if (!TryGetUserQuestion(out var question))
            {
                Console.WriteLine("See you next time :-)");
                break;
            }

            ShowAnswer(question!);
        }
    }

    private static bool TryGetUserQuestion(out string? question)
    {
        Console.WriteLine();
        Console.Write(@"Question (press ENTER to finish): ");
        question = Console.ReadLine();
        return !string.IsNullOrWhiteSpace(question);
    }

    private void ShowAnswer(string question)
    {
        Console.WriteLine();
        Console.Write("Answer: ");
        Console.WriteLine();
        var fullPrompt = $"<|system|>{_systemPrompt}<|end|><|user|>{question}<|end|><|assistant|>";
        var tokens = _tokenizer.Encode(fullPrompt);

        var generatorParams = new GeneratorParams(_model);
        generatorParams.SetSearchOption("max_length", 2048);
        generatorParams.SetSearchOption("past_present_share_buffer", false);
        generatorParams.SetInputSequences(tokens);

        var generator = new Generator(_model, generatorParams);
        while (!generator.IsDone())
        {
            generator.ComputeLogits();
            generator.GenerateNextToken();
            var outputTokens = generator.GetSequence(0);
            var newToken = outputTokens.Slice(outputTokens.Length - 1, 1);
            var output = _tokenizer.Decode(newToken);
            Console.Write(output);
        }

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(new string('-', Console.WindowWidth));
    }
}