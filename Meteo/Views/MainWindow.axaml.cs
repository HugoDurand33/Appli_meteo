using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Newtonsoft.Json.Linq;

namespace Meteo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        _ = InitializeApiKeyAndMakeRequest();
    }

    public async Task InitializeApiKeyAndMakeRequest()
    {
    try
    {
        // Retrieve the API key
        string apiKey = getAPikey();

        // Make the API request
        string response = await MakeApiRequestAsync(apiKey);

        // Display the response
        var apiKeyTextBlock = this.FindControl<TextBlock>("ApiKeyTextBlock");
        if (apiKeyTextBlock != null)
        {
            apiKeyTextBlock.Text = response;
            Console.WriteLine("TextBlock updated.");
        }
        else
        {
            Console.WriteLine("TextBlock not found.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error during API request: " + ex.Message);
        var apiKeyTextBlock = this.FindControl<TextBlock>("ApiKeyTextBlock");
        if (apiKeyTextBlock != null)
        {
            apiKeyTextBlock.Text = "Error: " + ex.Message;
            Console.WriteLine("TextBlock updated with error message.");
        }
    }
    }



    public static async Task<string> MakeApiRequestAsync(string apiKey)
    {
        using (var client = new HttpClient())
        {
            // Set the base address for the HttpClient
            client.BaseAddress = new Uri("https://api.openweathermap.org");


            // Send the GET request
            var response = await client.GetAsync($"/data/2.5/weather?q=Bordeaux&appid={apiKey}");

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response content
            string responseBody = await response.Content.ReadAsStringAsync();
            
            var json = JObject.Parse(responseBody); 

#pragma warning disable CS8602 // Dereference of a possibly null reference.  
            var temperature = json["main"]["temp"].ToString();

            var weatherDescription = json["weather"][0]["description"].ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return $"Temperature: {temperature}, Weather: {weatherDescription}";
        }
    }


    public static string getAPikey() {
        // Path to the configuration file 
        var configFilePath = "config.json"; 
        // Read the configuration file
        var json = File.ReadAllText(configFilePath); 
        // Parse the JSON 
        var jObject = JObject.Parse(json); 
        // Get the API key 
        var apiKey = jObject["APIKey"]?.ToString(); 

        if (apiKey == null) {
            return "Changer_text_là"; // mettre API_key si fichier conf n'existe pas
        }
        return apiKey; // peut pas être null, normalement
    }
}