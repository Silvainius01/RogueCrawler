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

                if (ArmorByClass.ContainsKey(data.ArmorClass))
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
                    ArmorClass = DungeonConstants.ArmorClassLight,
                    ArmorSlot = ArmorSlotType.Head,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ DungeonConstants.MaterialLeather },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Coif",
                    ArmorClass = DungeonConstants.ArmorClassMedium,
                    ArmorSlot = ArmorSlotType.Head,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Helmet",
                    ArmorClass = DungeonConstants.ArmorClassHeavy,
                    ArmorSlot = ArmorSlotType.Head,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Jacket",
                    ArmorClass = DungeonConstants.ArmorClassLight,
                    ArmorSlot = ArmorSlotType.Chest,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ DungeonConstants.MaterialLeather },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Brigandine",
                    ArmorClass = DungeonConstants.ArmorClassMedium,
                    ArmorSlot = ArmorSlotType.Chest,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ DungeonConstants.MaterialLeather },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Cuirass",
                    ArmorClass = DungeonConstants.ArmorClassHeavy,
                    ArmorSlot = ArmorSlotType.Chest,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Sleeves",
                    ArmorClass = DungeonConstants.ArmorClassLight,
                    ArmorSlot = ArmorSlotType.Arm,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ DungeonConstants.MaterialLeather },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Vambrace",
                    ArmorClass = DungeonConstants.ArmorClassMedium,
                    ArmorSlot = ArmorSlotType.Arm,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Pauldrons",
                    ArmorClass = DungeonConstants.ArmorClassHeavy,
                    ArmorSlot = ArmorSlotType.Arm,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Gloves",
                    ArmorClass = DungeonConstants.ArmorClassLight,
                    ArmorSlot = ArmorSlotType.Hand,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ DungeonConstants.MaterialLeather },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Bracers",
                    ArmorClass = DungeonConstants.ArmorClassMedium,
                    ArmorSlot = ArmorSlotType.Hand,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Gauntlets",
                    ArmorClass = DungeonConstants.ArmorClassHeavy,
                    ArmorSlot = ArmorSlotType.Hand,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Pants",
                    ArmorClass = DungeonConstants.ArmorClassLight,
                    ArmorSlot = ArmorSlotType.Waist,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ DungeonConstants.MaterialLeather },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Cuisses",
                    ArmorClass = DungeonConstants.ArmorClassMedium,
                    ArmorSlot = ArmorSlotType.Waist,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Greaves",
                    ArmorClass = DungeonConstants.ArmorClassHeavy,
                    ArmorSlot = ArmorSlotType.Waist,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                },
                new ArmorTypeData()
                {
                    ArmorType = "Shoes",
                    ArmorClass = DungeonConstants.ArmorClassLight,
                    ArmorSlot = ArmorSlotType.Foot,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ DungeonConstants.MaterialLeather },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Boots",
                    ArmorClass = DungeonConstants.ArmorClassMedium,
                    ArmorSlot = ArmorSlotType.Foot,
                    BaseValue = 1,
                    BaseArmorRating = 1,
                    AllowAnyMetal = true,
                    AllowedMaterials = new []{ DungeonConstants.MaterialLeather },
                },
                new ArmorTypeData()
                {
                    ArmorType = "Sabatons",
                    ArmorClass = DungeonConstants.ArmorClassHeavy,
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
