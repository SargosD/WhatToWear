# WhatToWear: AI-Powered Weather Clothing Advisor

This .NET console application suggests what to wear today based on real-time weather in your city, using a local LLM (Llama 2) for natural language advice.

## Features

- **Interactive CLI**: Prompts the user for a city name at runtime.
- **WeatherAPI Integration**: Fetches current weather data for any city using [WeatherAPI.com](https://www.weatherapi.com/docs/).
- **Local LLM Summarization**: Uses a local Llama 2 model (via LLamaSharp) to generate practical, context-aware clothing suggestions.
- **No Cloud AI**: All language model inference runs locally for privacy and speed.

## Setup

### 1. Prerequisites
- .NET 7.0 or later
- [WeatherAPI.com](https://www.weatherapi.com/) API key (already included in code for demo)
- Download a GGUF Llama 2 model (e.g., `llama-2-7b-chat.Q4_K_M.gguf`) and place it in the `models` directory:
  - Example: `models/llama-2-7b-chat.Q4_K_M.gguf`
- Install required NuGet packages:
  - `LLamaSharp`

### 2. Build
```sh
dotnet build
```

### 3. Run
```sh
dotnet run
```

## Usage
1. When prompted, enter a city name (e.g., `London`).
2. The app fetches the current weather for that city.
3. The weather summary is sent to the local LLM, which suggests what to wear.
4. The clothing suggestion is displayed in the console.

## Example Output
```
Weather-based Clothing Advisor (LLM)
====================================
Enter city name: London
Current weather in London: Partly cloudy, 18°C, feels like 17°C, humidity 60%, wind 12 kph.

LLM Suggestion:
You should wear a light jacket or sweater, comfortable pants, and consider an umbrella in case of rain. ...
```

## Project Structure
- `Program.cs` — Main application logic: prompts user, fetches weather, runs LLM, prints results.
- `models/llama-2-7b-chat.Q4_K_M.gguf` — Your local Llama 2 model file (not included; download separately).

## Developer Notes
- **Suppressing LLM Logs**: The app attempts to suppress LLamaSharp logs, but some native logs may still appear due to llama.cpp internals.
- **Model Path**: Update the `modelPath` variable in `Program.cs` if you use a different model file.
- **API Key**: The WeatherAPI key is hardcoded for demo purposes. For production, use environment variables or configuration files.
- **Extending**: You can easily adapt the LLM prompt for other advice (e.g., travel, sports, etc.) by changing the system message.

## References
- [WeatherAPI.com Docs](https://www.weatherapi.com/docs/)
- [LLamaSharp GitHub](https://github.com/SciSharp/LLamaSharp?tab=readme-ov-file)

## License
MIT 