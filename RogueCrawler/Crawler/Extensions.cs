using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueCrawler
{
    static class Extensions
    {
        public static string PlacesFormatString(this int places)
            => $"n{places}";
        public static string ToPlacesString(this float f, int places)
            => f.ToString($"n{places}");
    }
}
