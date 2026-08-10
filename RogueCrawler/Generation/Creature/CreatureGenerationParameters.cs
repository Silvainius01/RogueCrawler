using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CommandEngine;

namespace RogueCrawler
{
    /// <summary>
    /// Required Qualities:
    /// <para>WeaponQuality, WeaponWeight, CreatureDifficulty, ArmorQuality</para>
    /// </summary>
    class CreatureGenerationParameters : BaseGenerationParameters
    {
        public Vector2Int LevelRange { get; set; } = new Vector2Int(-1, -1);
        public Vector2Int SkillRange { get; set; } = new Vector2Int(-1, -1);

        public int MaxArmorPieces { get; set; } = -1;
        public float WeaponChance { get; set; } = -1;
        public float ArmorChance { get; set; } = -1;

        public const int numQualities = 5;
        public QualityLevel QualityBias { get => Qualities[0]; }
        public QualityLevel WeaponQuality { get => Qualities[1]; }
        public QualityLevel ArmorQuality { get => Qualities[3]; }
        public QualityLevel SkillQuality { get => Qualities[4]; }
        public QualityLevel LevelQuality { get=> Qualities[5]; }

        public CreatureGenerationParameters(QualityLevel quality) : base(numQualities, quality) { }
        public CreatureGenerationParameters(Func<QualityLevel> DetermineQuality) : base(numQualities, DetermineQuality) { }
        public CreatureGenerationParameters(IEnumerable<QualityLevel> qualities) : base(qualities) { }
        public CreatureGenerationParameters(params QualityLevel[] qualities) : base(qualities) { }

        protected override bool ValidateInternal()
        {
            QualityLevel[] rLevel = new QualityLevel[] { QualityLevel.Low, QualityLevel.Normal };
            bool IsValidRange(Vector2Int v) => v.X >= 0 && v.Y >= 0;

            while (Qualities.Count < numQualities)
                Qualities.Add(rLevel.RandomItem());


            // I hope this shit gets meme'd on by the code review YouTubers
            // float dr = ((int)CreatureDifficulty + 1) / (float)EnumExt<QualityLevel>.Count;

            LevelRange = Mathc.Max(LevelRange.Sort(), 1);
            ArmorChance = Math.Clamp(ArmorChance, 0.0f, 1.0f);
            WeaponChance = Math.Clamp(WeaponChance, 0.0f, 1.0f);
            MaxArmorPieces = Math.Clamp(MaxArmorPieces, 0, CreatureArmorSlots.TotalSlots);

            if (!IsValidRange(SkillRange))
                SkillRange = CreatureGenerationPresets.GetBaseSkillRange(SkillQuality);
            if (!IsValidRange(LevelRange))
                LevelRange = Mathc.Clamp(LevelRange, 1, 100);

            return true;
        }
    }
}
