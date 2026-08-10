using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueCrawler
{
    static class DungeonHelper
    {
        public static string GetQualityPrefix(float quality) => quality switch
        {
            <=  0   => DungeonConstants.QualityNameT0,      // Unusable
            <   1   => DungeonConstants.QualityNameT1,      // Less than base (0-1)
            <   3   => DungeonConstants.QualityNameT2,      // Base Damage (1-3) 
            <   7   => DungeonConstants.QualityNameT3,      // Double (3-7)
            <   15  => DungeonConstants.QualityNameT4,      // Triple (7-15)
            <   31  => DungeonConstants.QualityNameT5,      // Quadruple (15-31)
            <=  32  => DungeonConstants.QualityNameT6,      // Pentuple (31-32)
            _       => DungeonConstants.QualityNameError    // Not possible
        };
        public static string GetQualityPrefix(QualityLevel quality) => quality switch
        {
            QualityLevel.Low        => DungeonConstants.QualityNameT1,
            QualityLevel.Normal     => DungeonConstants.QualityNameT2,
            QualityLevel.Superior   => DungeonConstants.QualityNameT3,
            QualityLevel.Exalted    => DungeonConstants.QualityNameT4,
            QualityLevel.Legendary  => DungeonConstants.QualityNameT5,
            QualityLevel.Divine     => DungeonConstants.QualityNameT6,
            _                       => DungeonConstants.QualityNameError
        };

        public static string GetSkillMasteryTitle(int skillLevel) => (skillLevel * 100 / DungeonSettings.MaxSkillLevel) switch
        {
            <  0   => DungeonConstants.SkillMasteryTitleError,  // Negative skills not possible
            <  10  => DungeonConstants.SkillMasteryTitleT1,     // Novice (0-9)
            <  30  => DungeonConstants.SkillMasteryTitleT2,     // Apprentice (10-29)
            <  50  => DungeonConstants.SkillMasteryTitleT3,     // Adept (30-49)
            <  70  => DungeonConstants.SkillMasteryTitleT4,     // Expert (50-69)
            <  90  => DungeonConstants.SkillMasteryTitleT5,     // Master (70-89)
            <= 100 => DungeonConstants.SkillMasteryTitleT6,     // Grandmaster (90-100)
            _      => DungeonConstants.SkillMasteryTitleError   // Skills over the cap not possible
        };
        public static string GetSkillMasteryTitle(MasteryLevel mastery) => mastery switch
        {
            MasteryLevel.Novice      => DungeonConstants.SkillMasteryTitleT1,
            MasteryLevel.Apprentice  => DungeonConstants.SkillMasteryTitleT2,
            MasteryLevel.Adept       => DungeonConstants.SkillMasteryTitleT3,
            MasteryLevel.Expert      => DungeonConstants.SkillMasteryTitleT4,
            MasteryLevel.Master      => DungeonConstants.SkillMasteryTitleT5,
            MasteryLevel.Grandmaster => DungeonConstants.SkillMasteryTitleT6,
            _                        => DungeonConstants.SkillMasteryTitleError
        };

        public static float CalcStatPoints(Creature c, AttributeType major, AttributeType minor1, AttributeType minor2)
        {
            return
                c.GetAttribute(major) * DungeonSettings.StatPointsPerMajor +
                (c.GetAttribute(minor1) + c.GetAttribute(minor2)) * DungeonSettings.StatPointsPerMinor;
        }
    }
}
