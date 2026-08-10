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
                ++index;
            }

            IsLoaded = true;
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
                    SkillMode = InfluenceMode.Added,

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

            // Generate weapon skills. We only care about the subtype overrides and general skills.
            IEnumerable<WeaponTypeData> weaponTypes = WeaponTypeManager.IsLoaded
                ? WeaponTypeManager.WeaponTypes.Values
                : WeaponTypeManager.GetDefaultTypes();
            foreach (WeaponTypeData wtd in weaponTypes)
            {
                foreach (var subType in wtd.SubTypes)
                {
                    var t = new CreatureSkillTypeData(subType.TypeName)
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
                            new LinkedAttribute(subType.MinorAttributeOverride, DungeonSettings.WeaponMinorAttributeInfluence),
                        }
                    };

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
