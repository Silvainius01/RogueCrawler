using CommandEngine.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueCrawler
{
    internal class DamageTypeManager : ITypeManager<DamageTypeData>
    {
        static string DataPath = $"{DungeonCrawlerManager.TextPath}\\Data\\DamageTypes.json";
        static string ITypeManager<DamageTypeData>.DataPath
        {
            get => DataPath;
            set => throw new InvalidOperationException("Cannot change DataPath after initialization");
        }

        static bool IsLoaded = false;
        static bool ITypeManager<DamageTypeData>.IsLoaded
        {
            get => IsLoaded;
            set => throw new InvalidOperationException("Cannot set IsLoaded externally.");
        }

        public static Dictionary<string, DamageTypeData> DamageTypes = new Dictionary<string, DamageTypeData>();

        public static DamageTypeData TrueDamage => DamageTypes[DungeonConstants.DamageTypeTrue];
        public static DamageTypeData PhysicalDamage => DamageTypes[DungeonConstants.DamageTypeBlunt];
        public static DamageTypeData MagicalDamage => DamageTypes[DungeonConstants.DamageTypeArcane];
        public static DamageTypeData DivineDamage => DamageTypes[DungeonConstants.DamageTypeDivine];

        public static void LoadTypes()
        {
            if(IsLoaded) 
                return;

            StreamReader reader = new StreamReader(DataPath);
            string json = reader.ReadToEnd();
            reader.Close();

            int index = 0;
            var serializer = JsonSerializer.CreateDefault();
            var jArray = JsonConvert.DeserializeObject<JArray>(json);

            foreach (var obj in jArray)
            {
                DamageTypeData data = (DamageTypeData)serializer.Deserialize(new JTokenReader(obj), typeof(DamageTypeData));
                DamageTypes.Add(data.Name, data);
                ++index;
            }

            IsLoaded = true;
            // MaterialNameCommandModule = new MappedCommandModule<ItemMaterial>("What is the default material name prompt??", Materials);
        }

        public static List<DamageTypeData> GetDefaultTypes()
        {
            List<DamageTypeData> damageTypes = new List<DamageTypeData>(16)
            {
                new DamageTypeData(DungeonConstants.DamageTypeTrue)
                {
                    Category = DamageCategory.True,
                    Flags = DamageFlags.True
                },

                // Physical
                new DamageTypeData(DungeonConstants.DamageTypePierce)
                {
                    Category = DamageCategory.Physical,
                    Flags = DamageFlags.IsBlockable
                },
                new DamageTypeData(DungeonConstants.DamageTypeSlash)
                {
                    Category = DamageCategory.Physical,
                    Flags = DamageFlags.IsBlockable
                },
                new DamageTypeData(DungeonConstants.DamageTypeBlunt)
                {
                    Category = DamageCategory.Physical, 
                    Flags = DamageFlags.IsBlockable | DamageFlags.IsResistable 
                },

                // Magical
                new DamageTypeData(DungeonConstants.DamageTypeArcane)
                { 
                    Category = DamageCategory.Magical, 
                    Flags = DamageFlags.IsBlockable | DamageFlags.IsResistable
                },
                new DamageTypeData(DungeonConstants.DamageTypeAstral)
                { 
                    Category = DamageCategory.Magical, 
                    Flags = DamageFlags.IsResistable 
                },

                // Elemental
                new DamageTypeData(DungeonConstants.DamageTypeIce)
                {  
                    Category = DamageCategory.Elemental, 
                    Flags = DamageFlags.IsResistable 
                },
                new DamageTypeData(DungeonConstants.DamageTypeFire)
                { 
                    Category = DamageCategory.Elemental, 
                    Flags = DamageFlags.IsResistable 
                },
                new DamageTypeData(DungeonConstants.DamageTypeLightning)
                {   
                    Category = DamageCategory.Elemental, 
                    Flags = DamageFlags.IsResistable 
                },

                //Divine
                new DamageTypeData(DungeonConstants.DamageTypeDivine)
                {   
                    Category = DamageCategory.Divine, 
                    Flags = DamageFlags.IsResistable 
                },
            };

            return damageTypes;
        }

        public static void SaveDefaultTypes()
        {
            using StreamWriter writer = new StreamWriter(DataPath);
            writer.Write(JsonConvert.SerializeObject(GetDefaultTypes()));
            writer.Close();
        }
    }
}
