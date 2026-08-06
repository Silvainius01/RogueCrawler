using CommandEngine;
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

        static bool Loaded = false;

        public static void LoadTypes()
        {
            if (Loaded)
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
                ++index;
            }

            Loaded = true;
        }

        public static List<CreatureSkillTypeData> GetDefaultTypes()
        {
            List<CreatureSkillTypeData> skillTypes = new List<CreatureSkillTypeData>()
            {
                new CreatureSkillTypeData("Evasion", 1.25f, ModifierMode.Multiplier, ValueInfluenceMode.Always)
                {
                    FatigueInfluence = new FatigueLink(0.25f, ModifierMode.Addend, ValueInfluenceMode.Always),

                    // Undefined armor applies coverage as-is (e.g. medium armor)
                    DefaultArmorCoverageInf = new LinkedArmorClass("DEFAULT_ARMOR_LINK", 1.0f, ModifierMode.Multiplier, ValueInfluenceMode.Never),
                    ArmorListMode = ListInfluenceMode.Additive,
                    LinkedArmors = new List<LinkedArmorClass>()
                    {
                        // Uncovered or lightly clothed slots do not affect evasion.
                        new LinkedArmorClass(DungeonConstants.ArmorClassUnarmored, 0.0f, ModifierMode.Multiplier, ValueInfluenceMode.Always),
                        new LinkedArmorClass(DungeonConstants.ArmorClassClothing, 0.0f, ModifierMode.Multiplier, ValueInfluenceMode.Always),
                        
                        // Light armor counts for half coverage
                        new LinkedArmorClass(DungeonConstants.ArmorClassLight, 0.5f, ModifierMode.Multiplier, ValueInfluenceMode.Always),

                        // Heavy armor coverage is doubled
                        new LinkedArmorClass(DungeonConstants.ArmorSkillHeavy, 2.0f, ModifierMode.Multiplier, ValueInfluenceMode.Always),
                    },

                    AttributeListMode = ListInfluenceMode.Additive,
                    LinkedAttributes = new List<LinkedAttribute>()
                    {
                        new LinkedAttribute(AttributeType.DEX, 1.0f, ModifierMode.Multiplier),
                        new LinkedAttribute(AttributeType.AGI, 0.5f, ModifierMode.Multiplier),
                    },
                }
                // ADD ARMOR SKILLS
            };

            // Generate weapon skills
            IEnumerable<WeaponTypeData> weaponTypes = WeaponTypeManager.TypesLoaded
                ? WeaponTypeManager.WeaponTypes.Values
                : WeaponTypeManager.GetDefaultTypes();
            foreach (WeaponTypeData wtd in weaponTypes)
            {
                foreach (string typeName in wtd.OneHandedWeaponNames.Concat(wtd.TwoHandedWeaponNames))
                {
                    var t = new CreatureSkillTypeData(typeName, DungeonSettings.WeaponSpecificSkillInfluence, ModifierMode.Multiplier, ValueInfluenceMode.Always)
                    {
                        SkillListMode = ListInfluenceMode.Additive,
                        LinkedSkills = new List<LinkedSkill>()
                        {
                            new LinkedSkill(wtd.WeaponType, DungeonSettings.WeaponGeneralSkillInfluence, ModifierMode.Multiplier, ValueInfluenceMode.Always),
                        },

                        AttributeListMode = ListInfluenceMode.Normalized,
                        LinkedAttributes = new List<LinkedAttribute>()
                        {
                            new LinkedAttribute(wtd.MajorAttribute, DungeonSettings.WeaponMajorAttributeInfluence, ModifierMode.Multiplier, ValueInfluenceMode.Always),
                        }
                    };

                    // Add the minor attribute of the subtype if applicable.
                    LinkedAttribute minorLink = new LinkedAttribute(wtd.MinorAttribute, DungeonSettings.WeaponMinorAttributeInfluence, ModifierMode.Multiplier);
                    if (wtd.SubTypes.TryFirst(std => std.TypeName == typeName, out var subType))
                        minorLink.Attribute = subType.MinorAttributeOverride;
                    t.LinkedAttributes.Add(minorLink);

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
