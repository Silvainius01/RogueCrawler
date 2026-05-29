using CommandEngine;
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
    internal class DamageTypeManager
    {
        public static string DataPath = $"{DungeonCrawlerManager.TextPath}\\Data\\DamageTypes.json";

        public static bool Loaded = false;
        public static Dictionary<string, DamageTypeData> DamageTypes = new Dictionary<string, DamageTypeData>();

        public static DamageTypeData TrueDamage => DamageTypes["True"];
        public static DamageTypeData PhysicalDamage => DamageTypes["Blunt"];
        public static DamageTypeData MagicalDamage => DamageTypes["Arcane"];
        public static DamageTypeData DivineDamage => DamageTypes["Divine"];

        public static void LoadDamageTypes()
        {
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

            Loaded = true;
            // MaterialNameCommandModule = new MappedCommandModule<ItemMaterial>("What is the default material name prompt??", Materials);
        }

        public static void GenerateDefaultTypes()
        {
            List<DamageTypeData> types = new List<DamageTypeData>(16)
            {
                new DamageTypeData("True")
                {
                    Category = DamageCategory.True,
                    Flags = DamageFlags.True
                },

                // Physical
                new DamageTypeData("Pierce")
                {
                    Category = DamageCategory.Physical,
                    Flags = DamageFlags.IsBlockable
                },
                new DamageTypeData("Slash")
                {
                    Category = DamageCategory.Physical,
                    Flags = DamageFlags.IsBlockable
                },
                new DamageTypeData("Blunt")
                {
                    Category = DamageCategory.Physical, 
                    Flags = DamageFlags.IsBlockable | DamageFlags.IsResistable 
                },

                // Magical
                new DamageTypeData("Arcane")
                { 
                    Category = DamageCategory.Magical, 
                    Flags = DamageFlags.IsBlockable | DamageFlags.IsResistable
                },
                new DamageTypeData("Astral")
                { 
                    Category = DamageCategory.Magical, 
                    Flags = DamageFlags.IsResistable 
                },

                // Elemental
                new DamageTypeData("Ice")
                {  
                    Category = DamageCategory.Elemental, 
                    Flags = DamageFlags.IsResistable 
                },
                new DamageTypeData("Fire")
                { 
                    Category = DamageCategory.Elemental, 
                    Flags = DamageFlags.IsResistable 
                },
                new DamageTypeData("Lightning")
                {   
                    Category = DamageCategory.Elemental, 
                    Flags = DamageFlags.IsResistable 
                },

                //Divine
                new DamageTypeData("Divine")
                {   
                    Category = DamageCategory.Divine, 
                    Flags = DamageFlags.IsResistable 
                },
            };

            using StreamWriter writer = new StreamWriter(DataPath);
            writer.Write(JsonConvert.SerializeObject(types));
            writer.Close();
        }
    }
}
