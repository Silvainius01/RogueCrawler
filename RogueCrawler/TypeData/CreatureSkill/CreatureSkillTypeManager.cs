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
                new CreatureSkillTypeData("Evasion")
                {
                    FatigueInfluence = 0.25f,
                    FatigueMode = InfluenceMode.Linear,

                    SelfInfluence = 1.25f,

                    ArmorCoverageInfluence = 0.0f,
                    ArmorCoverageMode = InfluenceMode.Linear,
                    ExemptArmorClasses = new List<string>()
                    {
                        DungeonConstants.ArmorClassUnarmored,
                        DungeonConstants.ArmorClassClothing,
                        DungeonConstants.ArmorClassLight,
                    },

                    AttributeMode = InfluenceMode.Added,
                    LinkedAttributes = new List<LinkedAttribute>()
                    {
                        new LinkedAttribute(AttributeType.DEX, 1.0f),
                        new LinkedAttribute(AttributeType.AGI, 0.5f),
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
                    var t = new CreatureSkillTypeData(typeName)
                    {
                        SkillMode = InfluenceMode.Normalized,
                        SelfInfluence = DungeonSettings.WeaponSpecificSkillInfluence,
                        LinkedSkills = new List<LinkedSkill>()
                        {
                            new LinkedSkill(wtd.WeaponType, DungeonSettings.WeaponGeneralSkillInfluence),
                        },

                        AttributeMode = InfluenceMode.Normalized,
                        LinkedAttributes = new List<LinkedAttribute>()
                        {
                            new LinkedAttribute(wtd.MajorAttribute, DungeonSettings.WeaponMajorAttributeInfluence),
                        }
                    };

                    // Add the minor attribute of the subtype if applicable.
                    if (wtd.SubTypes.TryFirst(std => std.TypeName == typeName, out var subType))
                        t.LinkedAttributes.Add(new LinkedAttribute(subType.MinorAttributeOverride, DungeonSettings.WeaponMinorAttributeInfluence));
                    else t.LinkedAttributes.Add(new LinkedAttribute(wtd.MinorAttribute, DungeonSettings.WeaponMinorAttributeInfluence));
                }
            }

            // Sort by name
            skillTypes.Sort((s1, s2) => s1.SkillName.CompareTo(s2.SkillName));
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
