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
    class CreatureSkillTypeData
    {
        [JsonIgnore]
        public string SkillName => SelfInfluence.SkillName;

        public LinkedSkill SelfInfluence { get; set; }
        public LinkedArmorClass DefaultArmorCoverageInf { get; set; } = new LinkedArmorClass("ARMOR_CLASS_HERE", 1.0f, ModifierMode.Multiplier, ConditionMode.Never);

        public InfluenceMergeMode MergeMode = InfluenceMergeMode.Additive;
        public List<LinkedModifier> PostProcessing { get; set; } = new List<LinkedModifier>();

        public LinkedSkillsList LinkedSkills { get; set; } = new LinkedSkillsList(InfluenceMergeMode.None);
        public LinkedAttributesList LinkedAttributes { get; set; } = new LinkedAttributesList(InfluenceMergeMode.None);
        public LinkedArmorClassList LinkedArmors { get; set; } = new LinkedArmorClassList(InfluenceMergeMode.None);
        public LinkedCreatureStats LinkedStats { get; set; } = new LinkedCreatureStats(InfluenceMergeMode.None);


        bool initialized = false;
        List<float> influences = new List<float>(4);

        [JsonConstructor]
        public CreatureSkillTypeData() { }
        public CreatureSkillTypeData(string name, float influence, ModifierMode modifierMode, ConditionMode influenceMode)
        {
            SelfInfluence = new LinkedSkill(name, influence, modifierMode, influenceMode);
        }
        public CreatureSkillTypeData(LinkedSkill skill)
        {
            SelfInfluence = skill;
        }

        public void Initlialize()
        {
            if (initialized)
                return;

            var armorClasses = ArmorTypeManager.ArmorByClass.Keys;
            foreach (var ac in armorClasses)
                if (!LinkedArmors.Links.Contains((link) => link.ArmorClass == ac))
                {
                    LinkedArmorClass newLink = new LinkedArmorClass(ac, DefaultArmorCoverageInf);
                    LinkedArmors.Links.Add(newLink);
                }

            // Add ourself to the skill list for appropriate modes.
            switch (LinkedSkills.MergeMode)
            {
                case InfluenceMergeMode.Normalized:
                case InfluenceMergeMode.Additive:
                case InfluenceMergeMode.Multiplicative:
                    SelfInfluence.ConditionMode = ConditionMode.Never;
                    LinkedSkills.Links.Add(SelfInfluence);
                    break;
            }

            initialized = true;
        }

        public float CalculateInfluence(Creature c)
        {
            influences.Clear();

            if (SelfInfluence.TryCalculate(c.Proficiencies, out float selfInf))
                influences.Add(selfInf);
            if (LinkedSkills.TryCalculate(c.Proficiencies, out float skillInf))
                influences.Add(skillInf);
            if (LinkedAttributes.TryCalculate(c, out float attributesInf))
                influences.Add(attributesInf);
            if (LinkedArmors.TryCalculate(c, out float armorInf))
                influences.Add(armorInf);
            if (LinkedStats.TryCalculate(c, out float statInf))
                influences.Add(statInf);

            if (!influences.Any())
            {
                ConsoleExt.WriteErrorLine($"Failed to calculate influence for skill {SkillName}");
                return 0.0f;
            }

            float influence = 0.0f;
            switch (MergeMode)
            {
                case InfluenceMergeMode.Additive:
                    influence = influences.Sum(); break;
                case InfluenceMergeMode.Multiplicative:
                    influence = influences.Aggregate((x, y) => x * y); break;
                case InfluenceMergeMode.Normalized:
                    float g = influences.Greatest();
                    g = Math.Abs(g) > 0 ? g : 1.0f;
                    for (int i = 0; i < influences.Count; ++i)
                        influences[i] /= g;
                    influence = influences.Sum();
                    break;
                case InfluenceMergeMode.Greatest:
                    influence = influences.Greatest(); break;
                case InfluenceMergeMode.Least:
                    influence = influences.Least(); break;
                default:
                    ConsoleExt.WriteWarningLine($"Unhandled influence merge type {MergeMode}. Returning first valid influence metric.");
                    influence = influences[0];
                    break;
            }

            foreach(var link in PostProcessing)
                if(link.TryCalculate(influence, out float next))
                    influence = next;

            return influence;
        }

        public float CalculateSkillInfluence(CreatureProficiencies c)
        {
            switch (LinkedSkills.MergeMode)
            {
                case InfluenceMergeMode.None:
                    return SelfInfluence.Calculate(c);
                case InfluenceMergeMode.Least:
                case InfluenceMergeMode.Greatest:
                    return SelfInfluence.Calculate(c) + LinkedSkills.Calculate(c);
                default:
                    return LinkedSkills.Calculate(c);
            }
        }
    }
}
