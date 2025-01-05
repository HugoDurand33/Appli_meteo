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
        string response = await MakeApiRequestAsync(apiKey,"Bordeaux");

        // Display the response
        var city = this.FindControl<TextBlock>("City");
        var lat = this.FindControl<TextBlock>("Lat_and_long");
        var temp = this.FindControl<TextBlock>("temp");
        var desc = this.FindControl<TextBlock>("description");
        var humid = this.FindControl<TextBlock>("humidity");

        var json = JObject.Parse(response);

#pragma warning disable CS8602 // Dereference of a possibly null reference.  

        city.Text = json["name"].ToString();
        lat.Text = json["coord"]["lon"].ToString() + " " + json["coord"]["lat"].ToString();
        temp.Text = json["main"]["temp"].ToString();
        desc.Text = json["weather"][0]["description"].ToString();
        humid.Text = json["main"]["humidity"].ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.  
        
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



    public static async Task<string> MakeApiRequestAsync(string apiKey, string city)
    {
        using (var client = new HttpClient())
        {
            // Set the base address for the HttpClient
            client.BaseAddress = new Uri("https://api.openweathermap.org");


            // Send the GET request
            var response = await client.GetAsync($"/data/2.5/weather?q={city}&appid={apiKey}&units=metric");

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response content
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
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