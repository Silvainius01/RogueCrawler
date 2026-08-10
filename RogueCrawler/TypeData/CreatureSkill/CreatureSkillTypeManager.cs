using CommandEngine.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RogueCrawler
{
    class CreatureSkillTypeManager : ITypeManager<CreatureSkillTypeData>
    {
        public static string DataPath = $"{DungeonCrawlerManager.TextPath}\\Data\\SkillData.json";
        static string ITypeManager<CreatureSkillTypeData>.DataPath
        {
            get => DataPath;
            set => throw new InvalidOperationException("Cannot change DataPath after initialization");
        }

        public static Dictionary<string, CreatureSkillTypeData> SkillData = new Dictionary<string, CreatureSkillTypeData>();

        static bool IsLoaded = false;
        static bool ITypeManager<CreatureSkillTypeData>.IsLoaded
        {
            get => IsLoaded;
            set => throw new InvalidOperationException("Cannot set IsLoaded externally.");
        }

        public static void LoadTypes()
        {
            if (IsLoaded)
                return;

            StreamReader reader = new StreamReader(DataPath);
            string json = reader.ReadToEnd();
            reader.Close();

            int index = 0;
            var serializer = JsonSerializer.CreateDefault();
            var jArray = JsonConvert.DeserializeObject<JArray>(json);

            foreach (var obj in jArray)
            {
                var data = (CreatureSkillTypeData)serializer.Deserialize(new JTokenReader(obj), typeof(CreatureSkillTypeData));
                SkillData.Add(data.SkillName, data);
                data.Initlialize();
                ++index;
            }

            IsLoaded = true;
        }

        public static List<CreatureSkillTypeData> GetDefaultTypes()
        {
            List<CreatureSkillTypeData> skillTypes = new List<CreatureSkillTypeData>()
            {
                new CreatureSkillTypeData("Evasion", 1.25f, ModifierMode.Multiplier, ConditionMode.Always)
                {
                    // Undefined armor applies coverage as-is (e.g. medium armor)
                    DefaultArmorCoverageInf = new LinkedArmorClass("DEFAULT_ARMOR_LINK", 1.0f, ModifierMode.Multiplier, ConditionMode.Never),
                    LinkedArmors = new LinkedArmorClassList(InfluenceMergeMode.Additive)
                    {
                        Links = new List<LinkedArmorClass>()
                        {
                            // Uncovered or lightly clothed slots do not affect evasion.
                            new LinkedArmorClass(DungeonConstants.ArmorClassUnarmored, 0.0f, ModifierMode.Multiplier, ConditionMode.Always),
                            new LinkedArmorClass(DungeonConstants.ArmorClassClothing, 0.0f, ModifierMode.Multiplier, ConditionMode.Always),
                        
                            // Light armor counts for half coverage
                            new LinkedArmorClass(DungeonConstants.ArmorClassLight, 0.5f, ModifierMode.Multiplier, ConditionMode.Always),

                            // Heavy armor coverage is doubled
                            new LinkedArmorClass(DungeonConstants.ArmorSkillHeavy, 2.0f, ModifierMode.Multiplier, ConditionMode.Always),
                        },
                        PostProcessing = new List<LinkedModifier>()
                        {
                            // This results in 1 / (1 + aci)
                            new LinkedModifier(1, ModifierMode.Addend, ConditionMode.Always),
                            new LinkedModifier(1, ModifierMode.Dividend, ConditionMode.Always),
                        },
                    },
                    LinkedAttributes = new LinkedAttributesList(InfluenceMergeMode.Additive)
                    {
                        Links = new List<LinkedAttribute>()
                        {
                            new LinkedAttribute(AttributeType.DEX, 1.0f, ModifierMode.Multiplier),
                            new LinkedAttribute(AttributeType.AGI, 0.5f, ModifierMode.Multiplier),
                        },
                    },
                    LinkedStats = new LinkedCreatureStats(InfluenceMergeMode.Additive)
                    {
                        Links = new List<LinkedCreatureStat>()
                        {
                            new LinkedCreatureStat(DungeonConstants.CreatureFatigueIndex, DungeonSettings.EvasionFatigueInfluence, ModifierMode.Addend, ConditionMode.Always)
                        }
                    },
                    PostProcessing = new List<LinkedModifier>()
                    {
                        new LinkedModifier(0.01f, ModifierMode.Multiplier, ConditionMode.Always)
                    }
                }
                // ADD ARMOR SKILLS
            };

            // Generate weapon skills. We only care about the subtype overrides and general skills.
            IEnumerable<WeaponTypeData> weaponTypes = WeaponTypeManager.IsLoaded
                ? WeaponTypeManager.WeaponTypes.Values
                : WeaponTypeManager.GetDefaultTypes();
            foreach (WeaponTypeData wtd in weaponTypes)
            {
                var typeNames = wtd.OneHandedWeaponNames
                    .Concat(wtd.TwoHandedWeaponNames)
                    .Concat(wtd.SubTypes.Select(st => st.TypeName))
                    .Distinct();
                foreach (string typeName in typeNames)
                {
                    var t = new CreatureSkillTypeData(typeName, DungeonSettings.WeaponSpecificSkillInfluence, ModifierMode.Multiplier, ConditionMode.Always)
                    {
                        LinkedSkills = new LinkedSkillsList(InfluenceMergeMode.Additive)
                        {
                            Links = new List<LinkedSkill>()
                            {
                                new LinkedSkill(wtd.WeaponType, DungeonSettings.WeaponGeneralSkillInfluence, ModifierMode.Multiplier, ConditionMode.Always),
                            },
                        },

                        LinkedAttributes = new LinkedAttributesList(InfluenceMergeMode.Normalized)
                        {
                            Links = new List<LinkedAttribute>()
                            {
                                new LinkedAttribute(wtd.MajorAttribute, DungeonSettings.WeaponMajorAttributeInfluence, ModifierMode.Multiplier, ConditionMode.Always),
                            }
                        }
                    };

                    // Add the minor attribute of the subtype if applicable.
                    LinkedAttribute minorLink = new LinkedAttribute(wtd.MinorAttribute, DungeonSettings.WeaponMinorAttributeInfluence, ModifierMode.Multiplier);
                    if (wtd.SubTypes.TryFirst(std => std.TypeName == typeName, out var subType))
                        minorLink.Attribute = subType.MinorAttributeOverride;
                    t.LinkedAttributes.Links.Add(minorLink);

                    skillTypes.Add(t);
                }
            }

            // Sort by name
            // skillTypes.Sort((s1, s2) => s1.SkillName.CompareTo(s2.SkillName));
            return skillTypes;
        }

        public static void SaveDefaultTypes()
        {
            using StreamWriter writer = new StreamWriter(DataPath);
            writer.Write(JsonConvert.SerializeObject(GetDefaultTypes()));
            writer.Close();
        }
    }
}
