using System;
using System.IO;
using Avalonia.Controls;
using Newtonsoft.Json.Linq;

namespace Meteo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        string apiKey = getAPikey(); 
        var apiKeyTextBlock = this.FindControl<TextBlock>("ApiKeyTextBlock");
        apiKeyTextBlock.Text = "API Key: " + "oui baguette";
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