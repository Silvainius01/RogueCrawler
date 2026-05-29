using CommandEngine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace RogueCrawler
{
    class LinkedSkill
    {
        public string SkillName { get; set; }
        public float Influence { get; set; }

        public LinkedSkill(string skill, float m) { SkillName = skill; Influence = m; }
    }
    class LinkedAttribute
    {
        public AttributeType Attribute { get; set; }
        public float Influence { get; set; }

        public LinkedAttribute(AttributeType attr, float m) { Attribute = attr; Influence = m; }
    }

    class CreatureSkillTypeData
    {
        public string SkillName { get; set; }

        public float SelfInfluence { get; set; } = 1.0f; // Contributes 100% of self by default
        public float FatigueInfluence { get; set; } = 1.0f; // No influence by default
        public float ArmorCoverageInfluence { get; set; } = 1.0f; // No influence by default

        public InfluenceMode SkillMode;
        public InfluenceMode AttributeMode;
        public InfluenceMode FatigueMode;
        public InfluenceMode ArmorCoverageMode;

        public List<string> ExemptArmorClasses { get; set; } = new List<string>();
        public List<LinkedSkill> LinkedSkills { get; set; } = new List<LinkedSkill>();
        public List<LinkedAttribute> LinkedAttributes { get; set; } = new List<LinkedAttribute>();
        
        public CreatureSkillTypeData(string name) { SkillName = name; }


        public float GetSkillInfluence(CreatureProficiencies c)
        {
            float retval = 0.0f;
            Comparison<LinkedSkill> comparison = (s1, s2) => 
                c.GetSkillLevel(s1.SkillName).CompareTo(c.GetSkillLevel(s2.SkillName));

            switch(SkillMode)
            {
                case InfluenceMode.None:
                    return 1.0f;
                case InfluenceMode.Least:
                    LinkedSkill least = LinkedSkills.Least(comparison);
                    retval =
                        c.GetSkillLevel(SkillName) * SelfInfluence +
                        c.GetSkillLevel(least.SkillName) * least.Influence;
                    break;
                case InfluenceMode.Greatest:
                    LinkedSkill greatest = LinkedSkills.Greatest(comparison);
                    retval =
                        c.GetSkillLevel(SkillName) * SelfInfluence +
                        c.GetSkillLevel(greatest.SkillName) * greatest.Influence;
                    break;
                case InfluenceMode.Normalized:
                    float influence = SelfInfluence;
                    float factor = c.GetSkillLevel(SkillName) * SelfInfluence;
                    foreach (var skill in LinkedSkills)
                    {
                        influence += skill.Influence;
                        factor += c.GetSkillLevel(skill.SkillName) * skill.Influence;
                    }
                    retval = (influence / factor);
                    break;
                case InfluenceMode.Added:
                    retval = c.GetSkillLevel(SkillName) * SelfInfluence;
                    foreach (var skill in LinkedSkills)
                        retval += c.GetSkillLevel(skill.SkillName) * skill.Influence;
                    break;
                default:
                    ConsoleExt.WriteErrorLine($"{SkillName} has an invalid influence mode for linked skills");
                    return 1.0f;
            }

            return retval / DungeonSettings.MaxSkillLevel;
        }

        public float GetAttributeInfluence(Creature c)
        {
            float retval = 0.0f;
            Comparison<LinkedAttribute> comparison = (a1, a2) =>
                c.GetAttributePercent(a1.Attribute).CompareTo(c.GetAttributePercent(a2.Attribute));

            switch (SkillMode)
            {
                case InfluenceMode.None:
                    return 1.0f;
                case InfluenceMode.Least:
                    LinkedAttribute least = LinkedAttributes.Least(comparison);
                    retval = c.GetAttributePercent(least.Attribute) * least.Influence;
                    break;
                case InfluenceMode.Greatest:
                    LinkedAttribute greatest = LinkedAttributes.Greatest(comparison);
                    retval = c.GetAttributePercent(greatest.Attribute) * greatest.Influence;
                    break;
                case InfluenceMode.Normalized:
                    float influence = 0.0f;
                    float factor = 0.0f;
                    foreach (var attr in LinkedAttributes)
                    {
                        influence += attr.Influence;
                        factor += c.GetAttributePercent(attr.Attribute) * attr.Influence;
                    }
                    retval = (influence / factor);
                    break;
                case InfluenceMode.Added:
                    foreach (var attr in LinkedAttributes)
                        retval += c.GetAttributePercent(attr.Attribute) * attr.Influence;
                    break;
                default:
                    ConsoleExt.WriteErrorLine($"{SkillName} has an invalid influence mode for linked attributes");
                    return 1.0f;
            }

            return retval;
        }

        public float GetFatigueInfluence(Creature c)
        {
            float influence = FatigueInfluence + c.Fatigue.Percent;
            switch (FatigueMode)
            {
                case InfluenceMode.None:
                    return 1.0f;
                case InfluenceMode.Linear:
                    return influence;
                case InfluenceMode.Threshold:
                    return influence < 1.0f ? influence : 1.0f;
                default:
                    ConsoleExt.WriteErrorLine($"{SkillName} has invalid influence mode for fatigue");
                    return 1.0f;
            }
        }

        public float GetCoverageInfluence(CreatureArmorSlots armor)
        {
            float influence = (1 - armor.ArmorCoverage) + ArmorCoverageInfluence;

            // Discount coverage added by exempt classes
            foreach (string ac in ExemptArmorClasses)
                influence += armor.GetArmorCoverageOfClass(ac);

            switch (FatigueMode)
            {
                case InfluenceMode.None:
                    return 1.0f;
                case InfluenceMode.Linear:
                    return influence;
                case InfluenceMode.Threshold:
                    return influence < 1.0f ? influence : 1.0f;
                default:
                    ConsoleExt.WriteErrorLine($"{SkillName} has invalid influence mode for armor coverage");
                    return 1.0f;
            }
        }
    }
}
