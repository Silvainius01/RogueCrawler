using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace RogueCrawler
{
    interface ITypeManager<T>
    {
        public static abstract string DataPath { get; set; }

        public static abstract void LoadTypes();
        public static abstract void SaveDefaultTypes();

        public static abstract List<T> GetDefaultTypes();
    }
}
