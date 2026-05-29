using System.Collections.Generic;
using System.Text;
using System.Linq;
using CommandEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace RogueCrawler
{
    class WeaponTypeManager : ITypeManager<WeaponTypeData>
    {
        static string DataPath = $"{DungeonCrawlerManager.TextPath}\\Data\\WeaponTypes.json";
        static string ITypeManager<WeaponTypeData>.DataPath
        { 
            get => DataPath; 
            set => throw new InvalidOperationException("Cannot change DataPath after initialization");
        }

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

        public static void LoadTypes()
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

        public static List<WeaponTypeData> GetDefaultTypes()
        {
            List<WeaponTypeData> weaponTypes = new List<WeaponTypeData>()
            {
                new WeaponTypeData(DungeonConstants.WeaponSkillAxes)
                {
                    DamageType = DungeonConstants.DamageTypeSlash,
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
                new WeaponTypeData(DungeonConstants.WeaponSkillBlades)
                {
                    DamageType = DungeonConstants.DamageTypeSlash,
                    MajorAttribute = AttributeType.AGI,
                    MinorAttribute = AttributeType.STR,
                    Handedness = ItemWeaponHandedness.Both,
                    BaseDamage = 1,
                    BaseValue = 5,
                    LargeWeaponDamageMult = 2f,
                    LargeWeaponWeightMult = 3f,
                    OneHandedWeaponNames = new[] { "Longsword", DungeonConstants.WeaponSkillShortsword, DungeonConstants.WeaponSkillDaggers },
                    TwoHandedWeaponNames = new[] { "Claymore","Katana","Zweihander"},
                    SubTypes = new[]
                    {
                        new WeaponSubTypeData(DungeonConstants.WeaponSkillDaggers)
                        {
                            MinorAttributeOverride = AttributeType.DEX,
                            WeaponHandedness = ItemWeaponHandedness.One,
                        },
                        new WeaponSubTypeData(DungeonConstants.WeaponSkillShortsword)
                        {
                            MinorAttributeOverride = AttributeType.DEX,
                            WeaponHandedness = ItemWeaponHandedness.One,
                        }
                    }
                },
                new WeaponTypeData(DungeonConstants.WeaponSkillBlunt)
                {
                    DamageType = DungeonConstants.DamageTypeBlunt,
                    MajorAttribute = AttributeType.STR,
                    MinorAttribute = AttributeType.CON,
                    Handedness = ItemWeaponHandedness.Both,
                    BaseDamage = 1,
                    BaseValue = 5,
                    LargeWeaponDamageMult = 3f,
                    LargeWeaponWeightMult = 4f,
                    OneHandedWeaponNames = new[] { "Club","Mace", "Maul" },
                    TwoHandedWeaponNames = new[] { "WarHammer"},
                    SubTypes = new[]
                    {
                        new WeaponSubTypeData(DungeonConstants.WeaponSkillUnarmed)
                        {
                            MinorAttributeOverride = AttributeType.DEX,
                            WeaponHandedness = ItemWeaponHandedness.Both,
                        }
                    }
                },
                new WeaponTypeData(DungeonConstants.WeaponSkillRanged)
                {
                    DamageType = DungeonConstants.DamageTypePierce,
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
                new WeaponTypeData(DungeonConstants.WeaponSkillSpears)
                {
                    DamageType = DungeonConstants.DamageTypePierce,
                    MajorAttribute = AttributeType.DEX,
                    MinorAttribute = AttributeType.CON,
                    Handedness = ItemWeaponHandedness.Two,
                    BaseDamage = 1,
                    BaseValue = 5,
                    LargeWeaponDamageMult = 3f,
                    LargeWeaponWeightMult = 4f,
                    OneHandedWeaponNames = new[] { "Spear" },
                    TwoHandedWeaponNames = new[] { "Halberd","Polearm","Pike"},
                },
            };

            return weaponTypes;
        }

        public static void SaveDefaultTypes()
        {
            using StreamWriter writer = new StreamWriter(DataPath);
            writer.Write(JsonConvert.SerializeObject(GetDefaultTypes()));
            writer.Close();
        }
    }
}
