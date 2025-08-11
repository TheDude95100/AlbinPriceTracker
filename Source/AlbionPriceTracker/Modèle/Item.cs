using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionPriceTracker.Modèle
{
    public class Item
    {
        public string ItemId { get; set; }
        public string City { get; set; }
        public int Quality { get; set; }
        public int SellPriceMin { get; set; }
        public int SellPriceMax { get; set; }
        public int BuyPriceMin { get; set; }
        public int BuyPriceMax { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
