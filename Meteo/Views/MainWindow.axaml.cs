using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Newtonsoft.Json.Linq;

namespace Meteo.Views;

public partial class MainWindow : Window
{

    string apiKey = getAPikey();
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        _ = InitializeApiKeyAndMakeRequest();
        _ = MakeApiRequestAsyncForecast("Bordeaux");
    }

    public async Task InitializeApiKeyAndMakeRequest(string city_api = "Bordeaux")
    {
    try
    {

        // Make the API request
        string response = await MakeApiRequestAsync(city_api);
        // Display the response
        var city = this.FindControl<TextBlock>("City");
        var lat = this.FindControl<TextBlock>("Lat_and_long");
        var temp = this.FindControl<TextBlock>("temp");
        var desc = this.FindControl<TextBlock>("description");
        var humid = this.FindControl<TextBlock>("humidity");

        var json = JObject.Parse(response);

#pragma warning disable CS8602 // Dereference of a possibly null reference.  

        city.Text = "City name : " + json["name"].ToString();
        lat.Text = "Latitude and longitude : " + json["coord"]["lon"].ToString() + " " + json["coord"]["lat"].ToString();
        temp.Text = "Temperature : " + json["main"]["temp"].ToString() + "°C";
        desc.Text = "Description : " + json["weather"][0]["description"].ToString();
        humid.Text = "Humidity : " + json["main"]["humidity"].ToString() + "%";
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
    private void OnSubmitClicked(object sender, RoutedEventArgs? e)
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string inputText = inputTextBox.Text;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        if (inputText == null) {
            inputText = "Bordeaux";
        }
        _ = InitializeApiKeyAndMakeRequest(inputText);
        _ = MakeApiRequestAsyncForecast(inputText);
    }

    private void OnKeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            OnSubmitClicked(sender, null);
        }
    }

    public async Task<string> MakeApiRequestAsync(string city)
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

    public async Task MakeApiRequestAsyncForecast(string city,int i = 0)
    {
        using (var client = new HttpClient())
        {
            // Set the base address for the HttpClient
            client.BaseAddress = new Uri("https://api.openweathermap.org");


            // Send the GET request
            var response = await client.GetAsync($"/data/2.5/forecast?q={city}&appid={apiKey}&units=metric");

            // Check if the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response content
            string responseBody = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(responseBody);

#pragma warning disable CS8602 // Dereference of a possibly null reference. 

            var city_forecast = this.FindControl<TextBlock>("City_forecast");
            var lat_forecast = this.FindControl<TextBlock>("Lat_and_long_forecast");
            var temp_forecast = this.FindControl<TextBlock>("Temp_forecast");
            var desc_forecast = this.FindControl<TextBlock>("description_forecast");
            var humid_forecast = this.FindControl<TextBlock>("humidity_forecast");

            city_forecast.Text = "City name : " + json["city"]["name"].ToString();
            lat_forecast.Text = "Latitude and longitude : " + json["city"]["coord"]["lon"].ToString() + " " + json["city"]["coord"]["lat"].ToString();
            int cpt = 0;
            foreach (var item in json["list"])
            {
                string dateTime = item["dt_txt"].ToString();
                DateTime date = DateTime.Parse(dateTime);
                ComboBoxItem text_combobox = forecastComboBox.Items[cpt] as ComboBoxItem;

                if (date.Hour == 12) {
                    if(cpt == i) {
                        temp_forecast.Text = "Temperature : " + item["main"]["temp"].ToString() + "°C";
                        desc_forecast.Text = "Description : " + item["weather"][0]["description"].ToString();
                        humid_forecast.Text = "Humidity : " + item["main"]["humidity"].ToString() + "%";
                    }
                    text_combobox.Content = item["dt"];
                    string formatted_date = date.ToString("dd MMMM yyyy , hh", CultureInfo.CreateSpecificCulture("fr-FR"));
                    text_combobox.Content = formatted_date + ":00";
                    cpt += 1;
                }
            }
#pragma warning restore CS8602 // Converting null literal or possible null value to non-nullable type.
        }
    }

    private void OnForecastSelectionChanged(object sender, RoutedEventArgs  args)
    {
        int selectedIndex = forecastComboBox.SelectedIndex;
#pragma warning disable CS8600 ,CS8604 // Converting null literal or possible null value to non-nullable type.
        string inputText = inputTextBox.Text;


        _ = MakeApiRequestAsyncForecast(inputText,selectedIndex);
#pragma warning restore CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
    }

    



    public static string getAPikey() {
        var apikey = "";
        // Path to the configuration file
        if (File.Exists("config.json")) {
            var configFilePath = "config.json"; 
            // Read the configuration file
            var json = File.ReadAllText(configFilePath); 
            // Parse the JSON 
            var jObject = JObject.Parse(json); 
            // Get the API key 
            apikey = jObject["APIKey"]?.ToString(); 
        }
        else {
            apikey = "changer ce text"; // changer ce text pour tester le code avec votre cle api
        }
        return apikey; // peut pas être null, normalement
    }
}