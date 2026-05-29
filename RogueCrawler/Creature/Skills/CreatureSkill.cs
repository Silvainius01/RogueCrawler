using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;
using CommandEngine;
using Newtonsoft.Json;

namespace RogueCrawler
{
    class CreatureSkill
    {
        public int Level { get; set; }
        public int Experience { get; set; }
        public string Name { get; set; }
        

        public int ExpNeeded => ExperienceNeeded(Level);
        public float Progress => (float)Experience / ExpNeeded;

        public override string ToString()
        {
            if (Level >= DungeonSettings.MaxSkillLevel)
                return $"{Name}: [{Level}]"; 
            return $"{Name}: {Level} [{Experience}/{ExpNeeded}] -> {(Progress * 100).ToString("n1")}%";
        }

        public int ExperienceNeeded(int level)
        {
            return 50 + 25 * (int)Math.Ceiling(Math.Pow(Level, 1.1521));
        }
    }

    class CreatureSkillMetaData
    {
        public class LinkedSkill
        {
            public string SkillName { get; set; }
            public float Influence { get; set; }
        }

        public string SkillName { get; set; }
        public float FatigueInfluence { get; set; } = 1.0f; // No influence by default

        public AttributeType MajorAttribute { get; set; }
        public AttributeType MinorAttribute { get; set; }
        public List<LinkedSkill> LinkedSkills { get; set; }

        public CreatureSkillMetaData(string name) { SkillName = name; }
    }
}
