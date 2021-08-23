using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using WeatherAPI.Standard;
using WeatherAPI.Standard.Controllers;
using WeatherAPI.Standard.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WeatherClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string key = "8a547fd9fb634ae2b5684726210408";
            WeatherAPIClient client = new WeatherAPIClient(key);
            APIsController aPIs = client.APIs;
            string q = "HU12 9SU";

            while (true)
            {
                CurrentJsonResponse result = aPIs.GetRealtimeWeather(q);

                var mongoClient = new MongoClient(
                    "mongodb+srv://dbJosh:dbJoshArduino123@clusterlss.bn2ep.mongodb.net/LSSData?retryWrites=true&w=majority"
                    );

                IMongoDatabase database = mongoClient.GetDatabase("LSSData");
                var weather = database.GetCollection<BsonDocument>("Weather");

                var weatherData = new BsonDocument
                {
                    {"Time", DateTime.Now},
                    {"Post Code", q},
                    {"Temperature", result.Current.TempC + " Degrees C"},
                    {"Precipitation", result.Current.PrecipMm + "mm"},
                    {"Wind Direction", result.Current.WindDir},
                    {"Windspeed", result.Current.GustMph + "MPH"},
                    {"Humidity", result.Current.Humidity + "%" },
                };

                weather.InsertOne(weatherData);
                Thread.Sleep(30000);
            }
        }
    }
}
