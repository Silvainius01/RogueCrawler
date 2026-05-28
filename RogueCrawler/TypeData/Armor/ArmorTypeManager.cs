using CommandEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace RogueCrawler
{
    class ArmorTypeManager
    {
        public static string DataPath = $"{DungeonCrawlerManager.TextPath}\\Data\\ArmorTypes.json";

        public static bool TypesLoaded = false;
        public static Dictionary<string, ArmorTypeData> ArmorTypes = new Dictionary<string, ArmorTypeData>();
        public static Dictionary<string, List<ArmorTypeData>> ArmorByClass = new Dictionary<string, List<ArmorTypeData>>();
        
        public static MappedCommandModule<ArmorTypeData> ArmorTypeCommandModule;

        public static void LoadArmorTypes()
        {
            StreamReader reader = new StreamReader(DataPath);
            string json = reader.ReadToEnd();
            reader.Close();

            var serializer = JsonSerializer.CreateDefault();
            var jArray = JsonConvert.DeserializeObject<JArray>(json);

            foreach (var obj in jArray)
            {
                ArmorTypeData data = (ArmorTypeData)serializer.Deserialize(new JTokenReader(obj), typeof(ArmorTypeData));
                ArmorTypes.Add(data.ArmorType, data);

                if(ArmorByClass.ContainsKey(data.ArmorClass))
                    ArmorByClass[data.ArmorClass].Add(data);
                else ArmorByClass.Add(data.ArmorClass, new List<ArmorTypeData> { data });
            }
            TypesLoaded = true;
            ArmorTypeCommandModule = new MappedCommandModule<ArmorTypeData>("What is the default armor type prompt??", ArmorTypes);
        }

        public static void GenerateDefaultTypes()
        {
            List<ArmorTypeData> armorTypes = new List<ArmorTypeData>()
            {
                new ArmorTypeData()
                {
                    ArmorType = "Cap",
                    ArmorClass = "Light",
                    ArmorSlot = ArmorSlotType.Head,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ "Leather" },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Coif",
                    ArmorClass = "Medium",
                    ArmorSlot = ArmorSlotType.Head,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Helmet",
                    ArmorClass = "Heavy",
                    ArmorSlot = ArmorSlotType.Head,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Jacket",
                    ArmorClass = "Light",
                    ArmorSlot = ArmorSlotType.Chest,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ "Leather" },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Brigandine",
                    ArmorClass = "Medium",
                    ArmorSlot = ArmorSlotType.Chest,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ "Leather" },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Cuirass",
                    ArmorClass = "Heavy",
                    ArmorSlot = ArmorSlotType.Chest,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Sleeves",
                    ArmorClass = "Light",
                    ArmorSlot = ArmorSlotType.Arm,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ "Leather" },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Vambrace",
                    ArmorClass = "Medium",
                    ArmorSlot = ArmorSlotType.Arm,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Pauldrons",
                    ArmorClass = "Heavy",
                    ArmorSlot = ArmorSlotType.Arm,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Gloves",
                    ArmorClass = "Light",
                    ArmorSlot = ArmorSlotType.Hand,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ "Leather" },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Bracers",
                    ArmorClass = "Medium",
                    ArmorSlot = ArmorSlotType.Hand,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Gauntlets",
                    ArmorClass = "Heavy",
                    ArmorSlot = ArmorSlotType.Hand,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Pants",
                    ArmorClass = "Light",
                    ArmorSlot = ArmorSlotType.Waist,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ "Leather" },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Cuisses",
                    ArmorClass = "Medium",
                    ArmorSlot = ArmorSlotType.Waist,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Greaves",
                    ArmorClass = "Heavy",
                    ArmorSlot = ArmorSlotType.Waist,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Shoes",
                    ArmorClass = "Light",
                    ArmorSlot = ArmorSlotType.Foot,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ "Leather" },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Boots",
                    ArmorClass = "Medium",
                    ArmorSlot = ArmorSlotType.Foot,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ "Leather" },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Sabatons",
                    ArmorClass = "Heavy",
                    ArmorSlot = ArmorSlotType.Foot,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
            };

            using StreamWriter writer = new StreamWriter(DataPath);
            writer.Write(JsonConvert.SerializeObject(armorTypes));
            writer.Close();
        }
    }
}
