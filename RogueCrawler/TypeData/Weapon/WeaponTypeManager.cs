using CommandEngine;
using CommandEngine.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RogueCrawler
{
    class WeaponTypeManager : TypeManager<WeaponTypeData, WeaponTypeManager>
    {
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

        static string DataPath = $"{DungeonCrawlerManager.TextPath}\\Data\\WeaponTypes.json";

        public static MappedCommandModule<WeaponTypeData> WeaponTypeCommandModule;

        protected override string GetDataPath()
        {
            return DataPath;
        }

        protected override void AddTypeEntry(WeaponTypeData data)
        {
            WeaponTypes.Add(data.WeaponType, data);
        }

        protected override List<WeaponTypeData> GetDefaultTypesInternal()
        {
            List<WeaponTypeData> weaponTypes = new List<WeaponTypeData>()
            {
                new WeaponTypeData(DungeonConstants.WeaponTypeAxes)
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
                new WeaponTypeData(DungeonConstants.WeaponTypeBlades)
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
                new WeaponTypeData(DungeonConstants.WeaponTypeBlunt)
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
                new WeaponTypeData(DungeonConstants.WeaponTypeRanged)
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
                new WeaponTypeData(DungeonConstants.WeaponTypeSpears)
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
    }
}
