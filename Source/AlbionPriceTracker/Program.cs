using System;
using System.Threading.Tasks;
using AlbionPriceTracker.Enums;
using AlbionPriceTracker.Modèle;
using AlbionPriceTracker.Process;

namespace AlbionSpaceTracker
{
    class Program
    {
        static async Task Main(string[] args)
        {

            bool flag_stop = false;

            while (!flag_stop)
            {
                Console.Clear();
                Console.WriteLine("=== Sélection du Tier (1 à 8) ===");
                if (!int.TryParse(Console.ReadLine(), out int tierInput) || tierInput < 1 || tierInput > 8)
                {
                    Console.WriteLine("Tier invalide. Réessaie.");
                    await Task.Delay(1000);
                    continue;
                }
                Tier selectedTier = (Tier)Enum.Parse(typeof(Tier), $"T{tierInput}_");

                List<Ressources> availableResources = Enum.GetValues(typeof(Ressources))
                  .Cast<Ressources>()
                  .Where(r => tierInput >= 4 || (r != Ressources.RUNE && r != Ressources.SOUL && r != Ressources.RELIC))
                  .ToList();

                Console.WriteLine("\n=== Sélection des ressources (sépare par des virgules) ===");
                Console.WriteLine("Dispo : " + string.Join(", ", availableResources));
                Console.WriteLine("Exemple: WOOD,ORE");

                string input = Console.ReadLine()?.ToUpper() ?? "";

                var selectedResources = input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(r => r.Trim())
                             .Where(r => availableResources.Any(ar => ar.ToString() == r))
                             .ToList();

                if (!selectedResources.Any())
                {
                    Console.WriteLine("Aucune ressource valide sélectionnée. Réessaie.");
                    await Task.Delay(1000);
                    continue;
                }

                var itemsList = selectedResources.Select(r => $"{selectedTier}{r}");
                string items = string.Join(",", itemsList);


                Location[] locations = (Location[])Enum.GetValues(typeof(Location));
                JSONParser parser = new JSONParser(locations, items);

                try
                {
                    List<Item> returnedItems = await parser.FetchPricesAsync();
                    if (returnedItems == null || !returnedItems.Any())
                    {
                        Console.WriteLine("Aucun résultat trouvé.");
                    }
                    else
                    {
                        Console.WriteLine(new string('-', 80));
                        Console.WriteLine(
                            $"{"ItemId",-12} | {"City",-15} | {"Quality",7} | {"Sell Min",8} | {"Sell Max",8} | {"Buy Min",7} | {"Buy Max",7} | {"Last Updated",20}");
                        Console.WriteLine(new string('-', 80));

                        foreach (var item in returnedItems)
                        {
                            Console.WriteLine(
                                $"{item.ItemId,-12} | {item.City,-15} | {item.Quality,7} | {item.SellPriceMin,8} | {item.SellPriceMax,8} | {item.BuyPriceMin,7} | {item.BuyPriceMax,7} | {item.LastUpdated:yyyy-MM-dd HH:mm:ss}");
                        }
                        Console.WriteLine(new string('-', 80));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur : {ex.Message}");
                }

                Console.WriteLine("\nTape 'q' pour quitter ou une autre touche pour recommencer.");
                if (Console.ReadLine()?.ToLower() == "q")
                {
                    flag_stop = true;
                }

            }

        }
    }
}
