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
        public string SkillName => SelfInfluence.SkillName;

        public LinkedSkill SelfInfluence { get; set; }
        public FatigueLink FatigueInfluence { get; set; } = new FatigueLink(1.0f, ModifierMode.Multiplier, ValueInfluenceMode.Always);
        public LinkedArmorClass DefaultArmorCoverageInf { get; set; } = new LinkedArmorClass("ARMOR_CLASS_HERE", 1.0f, ModifierMode.Multiplier, ValueInfluenceMode.Never);

        public ListInfluenceMode InfluenceMergeMode = ListInfluenceMode.Additive;
        public ListInfluenceMode SkillListMode;
        public ListInfluenceMode AttributeListMode;
        public ListInfluenceMode ArmorListMode;

        public List<LinkedSkill> LinkedSkills { get; set; } = new List<LinkedSkill>();
        public List<LinkedAttribute> LinkedAttributes { get; set; } = new List<LinkedAttribute>();
        public List<LinkedArmorClass> LinkedArmors { get; set; } = new List<LinkedArmorClass>() { };

        bool initialized = false;

        public CreatureSkillTypeData(string name, float influence, ModifierMode modifierMode, ValueInfluenceMode influenceMode) 
        {
            SelfInfluence = new LinkedSkill(name, influence, modifierMode, influenceMode);
        }
        public CreatureSkillTypeData(LinkedSkill skill)
        {
            SelfInfluence = skill;
        }

        void Initlialize()
        {
            if(initialized) 
                return;

            var armorClasses = ArmorTypeManager.ArmorByClass.Keys;
            foreach(var ac in armorClasses)
                if(!LinkedArmors.Contains((link) => link.ArmorClass == ac))
                {
                    LinkedArmorClass newLink = new LinkedArmorClass(ac, DefaultArmorCoverageInf);
                    LinkedArmors.Add(newLink);
                }

            // Add ourself to the skill list for appropriate modes.
            switch(SkillListMode)
            {
                case ListInfluenceMode.Normalized:
                case ListInfluenceMode.Additive:
                case ListInfluenceMode.Multiplicative:
                    LinkedSkills.Add(SelfInfluence);
                    break;
            }
        }

        public float GetSkillInfluence(CreatureProficiencies c)
        {
            if (SkillListMode == ListInfluenceMode.None || !LinkedSkills.Any())
                return SelfInfluence.Calculate(c);

            Comparison<LinkedSkill> comparison = (s1, s2) =>
                c.GetSkillLevel(s1.SkillName).CompareTo(c.GetSkillLevel(s2.SkillName));
            float listInf = GetListInfluence(LinkedSkills, SkillListMode, c, comparison);

            switch (SkillListMode)
            {
                case ListInfluenceMode.Least:
                case ListInfluenceMode.Greatest:
                    return SelfInfluence.Calculate(c) + listInf;
                case ListInfluenceMode.Normalized:
                case ListInfluenceMode.Additive:
                case ListInfluenceMode.Multiplicative:
                    return listInf;
                default:
                    ConsoleExt.WriteErrorLine($"{SkillName} has an invalid influence mode for linked skills");
                    return 0.0f;
            }
        }

        public float GetAttributeInfluence(Creature c)
        {
            if (!LinkedAttributes.Any())
                return 1.0f;

            Comparison<LinkedAttribute> comparison = (a1, a2) =>
                c.GetAttributePercent(a1.Attribute).CompareTo(c.GetAttributePercent(a2.Attribute));
            return GetListInfluence(LinkedAttributes, AttributeListMode, c, comparison);
        }

        public float GetFatigueInfluence(Creature c)
        {
            return FatigueInfluence.Calculate(c);
        }

        public float GetCoverageInfluence(Creature c)
        {
            if (ArmorListMode == ListInfluenceMode.None || !LinkedArmors.Any())
                return 1.0f;
            Comparison<LinkedArmorClass> comparison = (a1, a2) =>
                c.Armor.GetArmorCoverageOfClass(a1.ArmorClass).CompareTo(c.Armor.GetArmorCoverageOfClass(a2.ArmorClass));
            return GetListInfluence(LinkedArmors, ArmorListMode, c, comparison);
        }

        float GetListInfluence<TLink, TArg>(List<TLink> links, ListInfluenceMode listMode, TArg linkArg, Comparison<TLink> comparison) where TLink : InfluenceLink<TArg>
        {
            float retval = 0.0f;
            switch (listMode)
            {
                case ListInfluenceMode.None:
                    return 1.0f;
                case ListInfluenceMode.Least:
                    TLink least = links.Least(comparison);
                    retval = least.Calculate(linkArg);
                    break;
                case ListInfluenceMode.Greatest:
                    TLink greatest = links.Greatest(comparison);
                    retval = greatest.Calculate(linkArg);
                    break;
                case ListInfluenceMode.Normalized:
                    float influence = 0.0f;
                    float factor = 0.0f;
                    foreach (var link in links)
                    {
                        influence += link.Influence;
                        factor += link.Calculate(linkArg);
                    }
                    retval = (influence / factor);
                    break;
                case ListInfluenceMode.Additive:
                    foreach (var link in links)
                        retval += link.Calculate(linkArg);
                    break;
                case ListInfluenceMode.Multiplicative:
                    retval = 1.0f;
                    foreach (var link in links)
                        retval *= link.Calculate(linkArg);
                    break;
                default:
                    ConsoleExt.WriteErrorLine($"{SkillName} has an invalid influence mode for linked attributes");
                    return 1.0f;
            }

            return retval;
        }
    }
}
