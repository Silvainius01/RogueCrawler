using System;
using System.Collections.Generic;
using System.Text;
using CommandEngine;
using static RogueCrawler.DungeonSettings;

namespace RogueCrawler
{
    class CreatureGenerationPresets
    {
        public static readonly Vector2Int LowSkillRange = new Vector2Int(1, 25);
        public static readonly Vector2Int MidSkillRange = new Vector2Int(25, 40);
        public static readonly Vector2Int HighSkillRange = new Vector2Int(40, 60);
        public static readonly Vector2Int RenownedSkillRange = new Vector2Int(60, 90);
        public static readonly Vector2Int LegendarySkillRange = new Vector2Int(85, 100);
        public static readonly Vector2Int AnySkillRange = new Vector2Int(1, 100);
        public static Vector2Int GetBaseSkillRange(QualityLevel level)
        {
            switch (level)
            {
                case QualityLevel.Low: return LowSkillRange;
                case QualityLevel.Normal: return MidSkillRange;
                case QualityLevel.Superior: return HighSkillRange;
                case QualityLevel.Exalted: return RenownedSkillRange;
                case QualityLevel.Legendary: return LegendarySkillRange;
            }
            return AnySkillRange;
        }

        public static CreatureGenerationParameters RandomCreature
        {
            get
            {
                // Each weapon type is equally likely, but unarmed is counted as one.
                // Therefore, we remove it to account for not having a weapon.
                int numWeapons = WeaponTypeManager.WeaponTypes.Keys.Count;
                float wr = (numWeapons - 1) / numWeapons;
                return new CreatureGenerationParameters(() => EnumExt<QualityLevel>.RandomValue)
                {
                    LevelRange = new Vector2Int(MinCreatureLevel, MaxCreatureLevel),
                    WeaponChance = wr,
                    ArmorChance = CommandEngine.Random.NextFloat(),

                };
            }
        }
        public static CreatureGenerationParameters GetLeveledCreature(int level)
        {
            return new CreatureGenerationParameters()
            {

            };
        }

        public static ItemArmorGenerationParameters DefaultCreatureArmorParams(CreatureGenerationParameters cParams)
        {
            var aParams = new ItemArmorGenerationParameters(cParams.ArmorQuality, cParams.Qualities[1]);
           
            return aParams;
        }
    }
}
