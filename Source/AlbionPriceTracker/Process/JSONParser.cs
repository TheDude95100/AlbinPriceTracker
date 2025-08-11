using AlbionPriceTracker.Enums;
using AlbionPriceTracker.Modèle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlbionPriceTracker.Process
{

    public class RawItem
    {
        public string item_id { get; set; }
        public string city { get; set; }
        public int quality { get; set; }
        public int sell_price_min { get; set; }
        public DateTime sell_price_min_date { get; set; }
        public int sell_price_max { get; set; }
        public DateTime sell_price_max_date { get; set; }
        public int buy_price_min { get; set; }
        public DateTime buy_price_min_date { get; set; }
        public int buy_price_max { get; set; }
        public DateTime buy_price_max_date { get; set; }
    }
    public class JSONParser
    {
        const string URL_API = "https://west.albion-online-data.com/api/v2/stats/prices";
        const string BASIC_RESSOURCES_OPTIONS = "qualities=1";

        public Location[] requestedLocations;
        public string requestedItems;

        public JSONParser(Location[] requestedLocation, string items) {
            this.requestedLocations = requestedLocation;
            this.requestedItems = items;
        }

        public async Task<List<Item>> FetchPricesAsync()
        {
            if (string.IsNullOrWhiteSpace(requestedItems)) throw new InvalidOperationException("Requested Item undefined");

            string locationsParam = string.Join(",", requestedLocations.Select(l => l.ToString().ToLower()));
            
            string url = $"{URL_API}/{requestedItems}?locations={locationsParam}&{BASIC_RESSOURCES_OPTIONS}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Erreur API Albion : {response.StatusCode}");
                }
                string json = await response.Content.ReadAsStringAsync();

                return ToListMarketPrice(JsonSerializer.Deserialize<List<RawItem>>(json));
            }
        }
        public static Item ToMarketPrice(RawItem raw)
        {
            var lastUpdated = new[] {
                raw.sell_price_min_date,
                raw.sell_price_max_date,
                raw.buy_price_min_date,
                raw.buy_price_max_date
            }.Where(d => d != DateTime.MinValue).DefaultIfEmpty(DateTime.MinValue).Max();

            return new Item
            {
                ItemId = raw.item_id,
                City = raw.city,
                Quality = raw.quality,
                SellPriceMin = raw.sell_price_min,
                SellPriceMax = raw.sell_price_max,
                BuyPriceMin = raw.buy_price_min,
                BuyPriceMax = raw.buy_price_max,
                LastUpdated = lastUpdated
            };
        }

        public static List<Item> ToListMarketPrice (List<RawItem> rawList)
        {
            return rawList.Select(raw => ToMarketPrice(raw)).ToList();
        }

    }
}
