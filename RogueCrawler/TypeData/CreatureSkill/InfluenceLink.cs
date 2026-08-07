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
    class InfluenceLink<T>
    {
        public float Influence;
        public float Threshold;
        public ModifierMode ModifierMode = ModifierMode.Multiplier;
        public ConditionMode ConditionMode = ConditionMode.Always;

        [JsonIgnore]
        protected Func<T, float> GetLinkedValue { get; set; } = (v) => 1.0f;

        [JsonConstructor] 
        protected InfluenceLink() { }
        public InfluenceLink(float m, ModifierMode modifierMode, ConditionMode conditionMode)
        {
            Influence = m;
            ModifierMode = modifierMode;
            ConditionMode = conditionMode;
        }
        public InfluenceLink(float m, ModifierMode modifierMode, ConditionMode conditionMode, Func<T, float> getLinkedValue)
        {
            Influence = m;
            ModifierMode = modifierMode;
            ConditionMode = conditionMode;
            GetLinkedValue = getLinkedValue;
        }
        public InfluenceLink(float m, ModifierMode modifierMode)
        {
            Influence = m;
            ModifierMode = modifierMode;
        }
        public InfluenceLink(float m, ModifierMode modifierMode, Func<T, float> getLinkedValue)
        {
            Influence = m;
            ModifierMode = modifierMode;
            GetLinkedValue = getLinkedValue;
        }
        public InfluenceLink(InfluenceLink<T> otherLink)
        {
            Influence = otherLink.Influence;
            ModifierMode = otherLink.ModifierMode;
            ConditionMode = otherLink.ConditionMode;
        }

        public bool CanCalculate(T argument)
        {
            float lv = GetLinkedValue(argument);
            return ConditionMode != ConditionMode.Never
               && (ConditionMode == ConditionMode.ThresholdAbove && lv > Influence)
               && (ConditionMode == ConditionMode.ThresholdBelow && lv < Influence);
        }

        bool CanCalculate(float lv)
        {
            switch (ConditionMode)
            {
                case ConditionMode.Always:
                    return true;
                case ConditionMode.ThresholdAbove:
                    return lv > Threshold;
                case ConditionMode.ThresholdBelow:
                    return lv < Threshold;
                default:
                    return false;
            }
        }

        public bool TryCalculate(T argument, out float result)
        {
            float lv = GetLinkedValue(argument);
            if (CanCalculate(lv))
            {
                result = Calculate(lv);
                return true;
            }
            result = 0.0f;
            return false;
        }

        public float Calculate(T argument)
        {
            float value = GetLinkedValue(argument);
            return Calculate(value);
        }
        float Calculate(float linkedValue)
        {
            switch (ModifierMode)
            {
                case ModifierMode.Multiplier:
                    return linkedValue * Influence;
                case ModifierMode.Divisor:
                    return linkedValue / Influence;
                case ModifierMode.Dividend:
                    return Influence / linkedValue;
                case ModifierMode.Addend:
                    return linkedValue + Influence;
                case ModifierMode.Exponent:
                    return MathF.Pow(linkedValue, Influence);
                case ModifierMode.ExponentBase:
                    return MathF.Pow(Influence, linkedValue);
                case ModifierMode.Logarithm:
                    return MathF.Log(linkedValue, Influence);
                case ModifierMode.Root:
                    return MathF.Pow(linkedValue, 1 / Influence);
                default:
                    return linkedValue;
            }
        }
    }

    class LinkedSkill : InfluenceLink<CreatureProficiencies>
    {
        public string SkillName { get; set; }

        [JsonConstructor] LinkedSkill() { GetLinkedValue = (c) => c.GetSkillLevel(SkillName); }
        public LinkedSkill(string skill, float m, ModifierMode modifierMode, ConditionMode conditionMode) : base(m, modifierMode, conditionMode)
        {
            SkillName = skill;
            GetLinkedValue = (c) => c.GetSkillLevel(SkillName);
        }
    }
    class LinkedAttribute : InfluenceLink<Creature>
    {
        public AttributeType Attribute { get; set; }

        [JsonConstructor] LinkedAttribute() { GetLinkedValue = (c) => c.GetAttributePercent(Attribute); }
        public LinkedAttribute(AttributeType attr, float m, ModifierMode modifierMode) : base(m, modifierMode, ConditionMode.Always)
        {
            Attribute = attr;
            GetLinkedValue = (c) => c.GetAttributePercent(Attribute);
        }
        public LinkedAttribute(AttributeType attr, float m, ModifierMode modifierMode, ConditionMode conditionMode) : base(m, modifierMode, conditionMode)
        {
            Attribute = attr;
            GetLinkedValue = (c) => c.GetAttributePercent(Attribute);
        }
    }
    class LinkedArmorClass : InfluenceLink<Creature>
    {
        public string ArmorClass { get; set; }

        [JsonConstructor] LinkedArmorClass() { GetLinkedValue = (c) => c.Armor.GetArmorCoverageOfClass(ArmorClass); }
        public LinkedArmorClass(string armorClass, float m, ModifierMode modifierMode, ConditionMode conditionMode) : base(m, modifierMode, conditionMode)
        {
            ArmorClass = armorClass;
            GetLinkedValue = (c) => c.Armor.GetArmorCoverageOfClass(ArmorClass);
        }
        public LinkedArmorClass(string armorClass, LinkedArmorClass otherLink) : base(otherLink)
        {
            ArmorClass = armorClass;
            GetLinkedValue = (c) => c.Armor.GetArmorCoverageOfClass(ArmorClass);
        }
    }
    class LinkedCreatureStat : InfluenceLink<Creature>
    {
        public int StatIndex { get; set; }

        [JsonConstructor] LinkedCreatureStat() { GetLinkedValue = (c) => c.Stats[StatIndex].Percent; }
        public LinkedCreatureStat(int statIndex, float m, ModifierMode modifierMode, ConditionMode conditionMode) : base(m, modifierMode, conditionMode)
        {
            StatIndex = statIndex;
            GetLinkedValue = (c) => c.Stats[StatIndex].Percent;
        }
    }
    class LinkedModifier : InfluenceLink<float>
    {
        [JsonConstructor] LinkedModifier() { GetLinkedValue = (f) => f; }
        public LinkedModifier(float m, ModifierMode modifierMode, ConditionMode conditionMode) : base(m, modifierMode)
        {
            GetLinkedValue = (f) => f;
        }
    }

    abstract class InfluenceList<TLink, TArg> where TLink : InfluenceLink<TArg>
    {
        public InfluenceMergeMode MergeMode { get; set; }
        public List<TLink> Links = new List<TLink>();
        public List<LinkedModifier> PostProcessing = new List<LinkedModifier>();

        [JsonConstructor]
        protected InfluenceList() { }
        public InfluenceList(InfluenceMergeMode mergeMode)
        {
            MergeMode = mergeMode;
        }

        protected abstract Comparison<TLink> GetComparison(TArg argument);

        /// <summary>
        /// Attmept to calculate the total influence.
        /// </summary>
        /// <param name="linkArg">The argument passed to the links for evaluation</param>
        /// <returns>True if conditions for calculating influence are met. False otherwise.</returns>
        public bool TryCalculate(TArg linkArg, out float result)
        {
            result = 0.0f;
            if (MergeMode == InfluenceMergeMode.None || !Links.Any())
                return false;

            result = CalculateListInfluence(linkArg);
            foreach (var link in PostProcessing)
                if (link.TryCalculate(result, out float next))
                    result = next;

            return true;
        }
        public float Calculate(TArg linkArg)
        {
            TryCalculate(linkArg, out float result);
            return result;
        }

        float CalculateListInfluence(TArg linkArg)
        {
            float retval = 0.0f;
            Comparison<TLink> comparison = GetComparison(linkArg);

            switch (MergeMode)
            {
                case InfluenceMergeMode.None:
                    return 1.0f;
                case InfluenceMergeMode.Least:
                    TLink least = Links.Least(comparison);
                    retval = least.Calculate(linkArg);
                    break;
                case InfluenceMergeMode.Greatest:
                    TLink greatest = Links.Greatest(comparison);
                    retval = greatest.Calculate(linkArg);
                    break;
                case InfluenceMergeMode.Normalized:
                    float influence = 0.0f;
                    float factor = 0.0f;
                    foreach (var link in Links)
                    {
                        factor += link.Influence;
                        influence += link.Calculate(linkArg);
                    }
                    retval = (influence / factor);
                    break;
                case InfluenceMergeMode.Additive:
                    foreach (var link in Links)
                        retval += link.Calculate(linkArg);
                    break;
                case InfluenceMergeMode.Multiplicative:
                    retval = 1.0f;
                    foreach (var link in Links)
                        retval *= link.Calculate(linkArg);
                    break;
                default:
                    ConsoleExt.WriteErrorLine($"Invalid merge mode for influence list");
                    return 1.0f;
            }

            return retval;
        }
    }
    class LinkedSkillsList : InfluenceList<LinkedSkill, CreatureProficiencies>
    {
        [JsonConstructor] LinkedSkillsList() { }
        public LinkedSkillsList(InfluenceMergeMode mergeMode) : base(mergeMode) { }

        protected override Comparison<LinkedSkill> GetComparison(CreatureProficiencies cSkills)
        {
            return (s1, s2) => cSkills.GetSkillLevel(s1.SkillName).CompareTo(cSkills.GetSkillLevel(s2.SkillName));
        }
    }
    class LinkedAttributesList : InfluenceList<LinkedAttribute, Creature>
    {
        [JsonConstructor] LinkedAttributesList() { }
        public LinkedAttributesList(InfluenceMergeMode mergeMode) : base(mergeMode) { }

        protected override Comparison<LinkedAttribute> GetComparison(Creature c)
        {
            return (a1, a2) => c.GetAttributePercent(a1.Attribute).CompareTo(c.GetAttributePercent(a2.Attribute));
        }
    }
    class LinkedArmorClassList : InfluenceList<LinkedArmorClass, Creature>
    {
        [JsonConstructor] LinkedArmorClassList() { }
        public LinkedArmorClassList(InfluenceMergeMode mergeMode) : base(mergeMode) { }

        protected override Comparison<LinkedArmorClass> GetComparison(Creature c)
        {
            return (a1, a2) => c.Armor.GetArmorCoverageOfClass(a1.ArmorClass).CompareTo(c.Armor.GetArmorCoverageOfClass(a2.ArmorClass));
        }
    }
    class LinkedCreatureStats : InfluenceList<LinkedCreatureStat, Creature>
    {
        [JsonConstructor] LinkedCreatureStats() { }
        public LinkedCreatureStats(InfluenceMergeMode mergeMode) : base(mergeMode)
        {
        }

        protected override Comparison<LinkedCreatureStat> GetComparison(Creature c)
        {
            return (a1, a2) => c.Stats[a1.StatIndex].Percent.CompareTo(c.Stats[a2.StatIndex].Percent);
        }
    }
}
