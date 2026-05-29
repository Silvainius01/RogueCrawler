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
    static class CreatureSkillUtility
    {
        static readonly float[] BaseSkillQualityBonus = CacheBaseSkillBonuses();
        static readonly Dictionary<string, CreatureSkillTypeData> SkillMetaDatas = new Dictionary<string, CreatureSkillTypeData>();

        // Calculate the skill bonuses for weapons. 
        // Assumption is that any bonus follows: floor(specSkill*0.75 + genSkill/4), or 0-100
        static float[] CacheBaseSkillBonuses()
        {
            float[] bonuses = new float[256];

            for (int i = 0; i < bonuses.Length; ++i)
                bonuses[i] = GetBaseSkillBonus(i);

            return bonuses;
        }
        static float GetBaseSkillBonus(int level)
        {
            // 1.014 / (1 + e^-0.1*x+5) - 0.007
            float sigmoid(int x) =>
                1.014f / (1 + MathF.Pow(MathF.E, -0.1f * x + 5)) - 0.007f;

            return Mathc.Clamp(sigmoid(level), 0, 1);
        }

        public static float GetDefaultSkillBonus(int skillLevel)
        {
            return skillLevel < BaseSkillQualityBonus.Length
               ? BaseSkillQualityBonus[skillLevel]
               : GetBaseSkillBonus(skillLevel);
        }
        public static float GetDefaultSkillBonus(string skill, CreatureProficiencies p)
        {
            return GetDefaultSkillBonus(p.GetSkillLevel(skill));
        }
        public static float GetWeaponSkillBonus(ItemWeapon weapon, CreatureProficiencies p)
        {
            int skillLevel = (int)
                ((p.GetSkillLevel(weapon.ObjectName) * 0.75f) +
                (p.GetSkillLevel(weapon.WeaponType) * 0.25f));
            return GetDefaultSkillBonus(skillLevel);
        }
        public static float GetWeaponSkillBonus(CreatureSkillTypeData weaponSkill, CreatureProficiencies p)
        {
            int skillLevel = (int)weaponSkill.GetSkillInfluence(p, false);
            return GetDefaultSkillBonus(skillLevel);
        }
        public static float GetArmorSkillBonus(ItemArmor armor, CreatureProficiencies p)
        {
            return GetDefaultSkillBonus(p.GetSkillLevel(armor.ArmorClass)) + 0.25f;
        }
    }
}
