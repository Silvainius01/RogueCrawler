using System.Collections.Generic;
using System.Text;
using System.Linq;
using CommandEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RogueCrawler
{
    class WeaponTypeManager
    {
        public static string DataPath = $"{DungeonCrawlerManager.TextPath}\\Data\\WeaponTypes.json";

        public static bool TypesLoaded = false;
        public static Dictionary<string, WeaponTypeData> WeaponTypes = new Dictionary<string, WeaponTypeData>();

        public static string RandomType
        {
            get
            {
                return WeaponTypes.Keys.RandomItem();
            }
        }
        public static WeaponTypeData RandomWeaponData
        {
            get
            {
                return WeaponTypes.Values.RandomItem();
            }
        }

        public static MappedCommandModule<WeaponTypeData> WeaponTypeCommandModule;

        public static void LoadWeaponTypes()
        {
            StreamReader reader = new StreamReader(DataPath);
            string json = reader.ReadToEnd();
            reader.Close();

            var serializer = JsonSerializer.CreateDefault();
            var jArray = JsonConvert.DeserializeObject<JArray>(json);

            foreach (var obj in jArray)
            {
                WeaponTypeData data = (WeaponTypeData)serializer.Deserialize(new JTokenReader(obj), typeof(WeaponTypeData));
                WeaponTypes.Add(data.WeaponType, data);
            }
            TypesLoaded = true;
            WeaponTypeCommandModule = new MappedCommandModule<WeaponTypeData>("What is the default weapon type prompt??", WeaponTypes);
        }

        public static void GenerateDefaultTypes()
        {
            List<WeaponTypeData> weaponTypes = new List<WeaponTypeData>()
            {
                new WeaponTypeData("Axe")
                {
                    DamageType = "Slash",
                    MajorAttribute = AttributeType.STR,
                    MinorAttribute = AttributeType.DEX,
                    Handedness = ItemWeaponHandedness.Both,
                    BaseDamage = 1,
                    BaseValue = 5,
                    LargeWeaponDamageMult = 2.5f,
                    LargeWeaponWeightMult = 5.0f,
                    OneHandedWeaponNames = new[] { "WarAxe","Tomahawk","Ono" },
                    TwoHandedWeaponNames = new[] { "BattleAxe","Panabas","Tabar"},
                },
                new WeaponTypeData("Blade")
                {
                    DamageType = "Slash",
                    MajorAttribute = AttributeType.AGI,
                    MinorAttribute = AttributeType.STR,
                    Handedness = ItemWeaponHandedness.Both,
                    BaseDamage = 1,
                    BaseValue = 5,
                    LargeWeaponDamageMult = 2f,
                    LargeWeaponWeightMult = 3f,
                    OneHandedWeaponNames = new[] { "Longsword","Shortsword","Dagger" },
                    TwoHandedWeaponNames = new[] { "Claymore","Katana","Zweihander"},
                    SubTypes = new[]
                    {
                        new WeaponSubTypeData("Dagger")
                        {
                            MinorAttributeOverride = AttributeType.DEX,
                            WeaponHandedness = ItemWeaponHandedness.One,
                        },
                        new WeaponSubTypeData("Shortsword")
                        {
                            MinorAttributeOverride = AttributeType.DEX,
                            WeaponHandedness = ItemWeaponHandedness.One,
                        }
                    }
                },
                new WeaponTypeData("Blunt")
                {
                    DamageType = "Blunt",
                    MajorAttribute = AttributeType.STR,
                    MinorAttribute = AttributeType.CON,
                    Handedness = ItemWeaponHandedness.Both,
                    BaseDamage = 1,
                    BaseValue = 5,
                    LargeWeaponDamageMult = 3f,
                    LargeWeaponWeightMult = 4f,
                    OneHandedWeaponNames = new[] { "Club","Mace","Maul" },
                    TwoHandedWeaponNames = new[] { "WarHammer"},
                    SubTypes = new[]
                    {
                        new WeaponSubTypeData("Unarmed")
                        {
                            MinorAttributeOverride = AttributeType.DEX,
                            WeaponHandedness = ItemWeaponHandedness.Both,
                        }
                    }
                },
                new WeaponTypeData("Ranged")
                {
                    DamageType = "Pierce",
                    MajorAttribute = AttributeType.AGI,
                    MinorAttribute = AttributeType.DEX,
                    Handedness = ItemWeaponHandedness.Two,
                    BaseDamage = 1,
                    BaseValue = 5,
                    LargeWeaponDamageMult = 3f,
                    LargeWeaponWeightMult = 4f,
                    OneHandedWeaponNames = new[] { "Sling","Kunai" },
                    TwoHandedWeaponNames = new[] { "Shortbow","Longbow","Crossbow"},
                },
                new WeaponTypeData("Spear")
                {
                    DamageType = "Pierce",
                    MajorAttribute = AttributeType.DEX,
                    MinorAttribute = AttributeType.CON,
                    Handedness = ItemWeaponHandedness.Two,
                    BaseDamage = 1,
                    BaseValue = 5,
                    LargeWeaponDamageMult = 3f,
                    LargeWeaponWeightMult = 4f,
                    OneHandedWeaponNames = new[] { "Sling","Kunai" },
                    TwoHandedWeaponNames = new[] { "Shortbow","Longbow","Crossbow"},
                },
            };

            using StreamWriter writer = new StreamWriter(DataPath);
            writer.Write(JsonConvert.SerializeObject(weaponTypes));
            writer.Close();
        }
    }
}
