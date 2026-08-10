using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueCrawler
{
    class DungeonConstants
    {
        public const string ArmorClassUnarmored = "Unarmored";
        public const string ArmorClassClothing = "Clothing";
        public const string ArmorClassLight = "Light";
        public const string ArmorClassMedium = "Medium";
        public const string ArmorClassHeavy = "Heavy";

        public const string ArmorSkillUnarmored = ArmorClassUnarmored;
        public const string ArmorSkillClothing = ArmorClassClothing;
        public const string ArmorSkillLight = ArmorClassLight;
        public const string ArmorSkillMedium = ArmorClassMedium;
        public const string ArmorSkillHeavy = ArmorClassHeavy;

        public const string WeaponTypeBlades = "Blades";
        public const string WeaponTypeSpears = "Spears";
        public const string WeaponTypeAxes = "Axes";
        public const string WeaponTypeBlunt = "Blunt";
        public const string WeaponTypeRanged = "Ranged";

        public const string WeaponSkillBlades = WeaponTypeBlades;
        public const string WeaponSkillSpears = WeaponTypeSpears;
        public const string WeaponSkillAxes = WeaponTypeAxes;
        public const string WeaponSkillBlunt = WeaponTypeBlunt;
        public const string WeaponSkillRanged = WeaponTypeRanged;
        public const string WeaponSkillDaggers = "Dagger";
        public const string WeaponSkillShortsword = "Shortsword";
        public const string WeaponSkillUnarmed = "Unarmed";

        public const string CreatureSkillEvasion = "Evasion";

        public const string QualityNameT0 = "Broken";
        public const string QualityNameT1 = "Degraded";
        public const string QualityNameT2 = "";
        public const string QualityNameT3 = "Superior";
        public const string QualityNameT4 = "Exalted";
        public const string QualityNameT5 = "Legendary";
        public const string QualityNameT6 = "Divine";
        public const string QualityNameError = "Anomalous";

        public const string SkillMasteryTitleT1 = "Novice";
        public const string SkillMasteryTitleT2 = "Apprentice";
        public const string SkillMasteryTitleT3 = "Adept";
        public const string SkillMasteryTitleT4 = "Expert";
        public const string SkillMasteryTitleT5 = "Master";
        public const string SkillMasteryTitleT6 = "Grandmaster";
        public const string SkillMasteryTitleError = "Outcast";

        public const string DamageTypeTrue = "True";
        public const string DamageTypeBlunt = "Blunt";
        public const string DamageTypeSlash = "Slash";
        public const string DamageTypePierce = "Pierce";
        public const string DamageTypeArcane = "Arcane";
        public const string DamageTypeAstral = "Astral";
        public const string DamageTypeIce = "Ice";
        public const string DamageTypeFire = "Fire";
        public const string DamageTypeLightning = "Lightning";
        public const string DamageTypeDivine = "Divine";

        public const string MaterialLeather = "Leather";
        public const string MaterialWood = "Wood";
        public const string MaterialCopper = "Copper";
        public const string MaterialBronze = "Bronze";
        public const string MaterialIron = "Iron";
        public const string MaterialSteel = "Steel";
        public const string MaterialMythryl = "Mythryl";
        public const string MaterialManasteel = "Manasteel";
        public const string MaterialAdamantite = "Adamantite";

        public const string DefaultFloatPlaces = "n1";

        public const int CreatureHealthIndex = 0;
        public const int CreatureFatigueIndex = 1;
        public const int CreatureManaIndex = 2;
    }
}