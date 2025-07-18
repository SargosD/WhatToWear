using System.Text.Json;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace WhatToWear;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Weather-based Clothing Advisor (LLM)");
        Console.WriteLine("====================================");
        Console.Write("Enter city name: ");
        string? city = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(city))
        {
            Console.WriteLine("No city provided. Exiting.");
            return;
        }

        var weather = await GetWeatherAsync(city.Trim());
        if (weather == null)
        {
            Console.WriteLine("Could not retrieve weather data.");
            return;
        }

        string weatherSummary = $"Current weather in {weather.Location}: {weather.Condition}, {weather.TempC}°C, feels like {weather.FeelsLikeC}°C, humidity {weather.Humidity}%, wind {weather.WindKph} kph.";
        Console.WriteLine(weatherSummary);

        var suggestion = await SuggestClothesWithLLMAsync(weatherSummary);
        Console.WriteLine("\nLLM Suggestion:");
        Console.WriteLine(suggestion);
    }

    // Weather API response model
    public class WeatherResult
    {
        public string Location { get; set; } = "";
        public string Condition { get; set; } = "";
        public double TempC { get; set; }
        public double FeelsLikeC { get; set; }
        public int Humidity { get; set; }
        public double WindKph { get; set; }
    }

    static async Task<WeatherResult?> GetWeatherAsync(string city)
    {
        string apiKey = "apiKey"; // api key for weatherapi
        string url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={Uri.EscapeDataString(city)}";
        using var client = new HttpClient();
        try
        {
            var response = await client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;

            var location = root.GetProperty("location").GetProperty("name").GetString() ?? city;
            var current = root.GetProperty("current");
            var condition = current.GetProperty("condition").GetProperty("text").GetString() ?? "";
            var tempC = current.GetProperty("temp_c").GetDouble();
            var feelsLikeC = current.GetProperty("feelslike_c").GetDouble();
            var humidity = current.GetProperty("humidity").GetInt32();
            var windKph = current.GetProperty("wind_kph").GetDouble();

            return new WeatherResult
            {
                Location = location,
                Condition = condition,
                TempC = tempC,
                FeelsLikeC = feelsLikeC,
                Humidity = humidity,
                WindKph = windKph
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Weather API error: {ex.Message}");
            return null;
        }
    }

    static async Task<string> SuggestClothesWithLLMAsync(string weatherSummary)
    {
        string modelPath = @"models\llama-2-7b-chat.Q4_K_M.gguf"; // path to LLM
        if (!File.Exists(modelPath))
        {
            return $"Model file not found at {modelPath}. Please download a GGUF model and place it in the models directory.";
        }

        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 1024,
            GpuLayerCount = 0
        };

        using var weights = LLamaWeights.LoadFromFile(parameters);
        using var context = weights.CreateContext(parameters);
        var executor = new InteractiveExecutor(context);

        var chatHistory = new ChatHistory();
        chatHistory.AddMessage(AuthorRole.System, "You are a helpful assistant that suggests what to wear based on the weather. Be specific and practical.");

        var session = new ChatSession(executor, chatHistory);

        var inferenceParams = new InferenceParams
        {
            MaxTokens = 256,
            AntiPrompts = new List<string> { "User:" },
            SamplingPipeline = new DefaultSamplingPipeline(),
        };

        var suggestion = new System.Text.StringBuilder();
        await foreach (var text in session.ChatAsync(
            new ChatHistory.Message(AuthorRole.User, $"Here is the weather: {weatherSummary}\nWhat should I wear today?"),
            inferenceParams))
        {
            suggestion.Append(text);
        }
        return suggestion.ToString().Trim();
    }
}
