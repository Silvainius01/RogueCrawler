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
        public ModifierMode ModifierMode = ModifierMode.Multiplier;
        public ValueInfluenceMode InfluenceMode = ValueInfluenceMode.Always;

        [JsonIgnore]
        protected Func<T, float> GetLinkedValue { get; set; } = (v) => 1.0f;

        public InfluenceLink(float m, ModifierMode modifierMode, ValueInfluenceMode influenceMode)
        {
            Influence = m;
            ModifierMode = modifierMode;
            InfluenceMode = influenceMode;
        }
        public InfluenceLink(float m, ModifierMode modifierMode, ValueInfluenceMode influenceMode, Func<T, float> getLinkedValue)
        {
            Influence = m;
            ModifierMode = modifierMode;
            InfluenceMode = influenceMode;
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
            InfluenceMode = otherLink.InfluenceMode;
        }

        public float Calculate(T argument)
        {
            float value = GetLinkedValue(argument);
            switch (ModifierMode)
            {
                case ModifierMode.Multiplier:
                    return value * Influence;
                case ModifierMode.Divisor:
                    return value / Influence;
                case ModifierMode.Addend:
                    return value + Influence;
                case ModifierMode.Exponent:
                    return MathF.Pow(value, Influence);
                case ModifierMode.Logarithm:
                    return MathF.Log(value, Influence);
                case ModifierMode.Root:
                    return MathF.Pow(value, 1 / Influence);
                default:
                    return value;
            }
        }

        public float GetNullModifer(float value, float influence)
        {
            switch (ModifierMode)
            {
                case ModifierMode.Multiplier:
                case ModifierMode.Divisor:
                    return 1.0f;
                case ModifierMode.Addend:
                    return 0.0f;
                case ModifierMode.Exponent:
                    return MathF.Log(value, influence);
                case ModifierMode.Logarithm:
                    return MathF.Pow(value, influence);
                default:
                    return 1.0f;
            }
        }
    }
    class InfluenceLink : InfluenceLink<string>
    {
        public InfluenceLink(float m, ModifierMode modifierMode, Func<float> getLinkedValue) : base(m, modifierMode, (str) => getLinkedValue()) { }
        public InfluenceLink(float m, ModifierMode modifierMode, ValueInfluenceMode influenceMode, Func<float> getLinkedValue) : base(m, modifierMode, influenceMode, (str) => getLinkedValue()) { }
    }

    class LinkedSkill : InfluenceLink<CreatureProficiencies>
    {
        public string SkillName { get; set; }

        public LinkedSkill(string skill, float m, ModifierMode modifierMode, ValueInfluenceMode influenceMode) : base(m, modifierMode, influenceMode)
        {
            SkillName = skill;
            GetLinkedValue = (c) => c.GetSkillLevel(SkillName);
        }
    }
    class LinkedAttribute : InfluenceLink<Creature>
    {
        public AttributeType Attribute { get; set; }

        public LinkedAttribute(AttributeType attr, float m, ModifierMode modifierMode) : base(m, modifierMode, ValueInfluenceMode.Always)
        {
            Attribute = attr;
            GetLinkedValue = (c) => c.GetAttributePercent(Attribute);
        }
        public LinkedAttribute(AttributeType attr, float m, ModifierMode modifierMode, ValueInfluenceMode influenceMode) : base(m, modifierMode, influenceMode)
        {
            Attribute = attr;
            GetLinkedValue = (c) => c.GetAttributePercent(Attribute);
        }
    }
    class LinkedArmorClass : InfluenceLink<Creature>
    {
        public string ArmorClass { get; set; }

        public LinkedArmorClass(string armorClass, float m, ModifierMode modifierMode, ValueInfluenceMode influenceMode) : base(m, modifierMode, influenceMode)
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
    class FatigueLink : InfluenceLink<Creature>
    {
        public FatigueLink(float m, ModifierMode modifierMode, ValueInfluenceMode influenceMode) : base(m, modifierMode, influenceMode)
        {
            GetLinkedValue = (c) => c.Fatigue.Percent;
        }
    }
}
