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
    class CreatureProficiencies : IInspectable, ISerializable<SerializedProfeciencies, CreatureProficiencies>
    {
        Dictionary<string, CreatureSkill> Skills = new Dictionary<string, CreatureSkill>();

        public CreatureSkill this[string key]
        {
            get => GetSkill(key);
        }

        public string BriefString()
        {
            return ToString();
        }
        public string DebugString(string prefix, int tabCount)
        {
            SmartStringBuilder builder = new SmartStringBuilder(DungeonSettings.TabString);

            if (prefix == string.Empty)
                prefix = "Skills:";
            builder.Append(tabCount, prefix);

            tabCount++;
            foreach (var skill in Skills.Values)
                builder.NewlineAppend(tabCount, skill.ToString());
            tabCount--;
            return builder.ToString();
        }
        public string InspectString(string prefix, int tabCount)
        {
            SmartStringBuilder builder = new SmartStringBuilder(DungeonSettings.TabString);

            if (prefix == string.Empty)
                prefix = "Skills:";
            builder.Append(tabCount, prefix);

            tabCount++;
            foreach (var skill in Skills.Values)
                if (skill.Level > 0 || skill.Experience > 0)
                    builder.NewlineAppend(tabCount, skill.ToString());
            tabCount--;
            return builder.ToString();
        }
        public override string ToString()
        {
            return Skills.ToString((kvp) => kvp.Value.ToString(), " ");
        }

        public CreatureSkill GetSkill(string skillName)
        {
            if (!Skills.ContainsKey(skillName))
            {
                Skills.Add(skillName, new CreatureSkill()
                {
                    Level = 0,
                    Experience = 0,
                    Name = skillName
                });
            }
            return Skills[skillName];
        }
        public int GetSkillLevel(string skillName) => GetSkill(skillName).Level;
        public float GetSkillProgress(string skillName) => GetSkill(skillName).Experience;

        public void SetSkill(string skillName, int level, int experience)
        {
            var skill = GetSkill(skillName);
            if (level >= 0)
                skill.Level = Math.Min(level, DungeonSettings.MaxSkillLevel);
            if (experience >= 0)
                skill.Experience = experience;
        }
        public void SetSkillLevel(string skillName, int level) => SetSkill(skillName, level, -1);
        public void SetSkillProgress(string skillName, int experience) => SetSkill(skillName, -1, experience);

        public void AddSkill(string skillName, int level, int experience)
        {
            var skill = GetSkill(skillName);

            if (experience > 0 && skill.Level < DungeonSettings.MaxSkillLevel)
            {
                int expNeeded = skill.ExperienceNeeded(skill.Level + level);
                skill.Experience += experience;
                while (skill.Experience >= expNeeded)
                {
                    ++level;
                    skill.Experience -= expNeeded;
                    expNeeded = skill.ExperienceNeeded(skill.Level + level);
                }
            }
            else skill.Experience = 0;

            skill.Level = Math.Min(skill.Level + level, DungeonSettings.MaxSkillLevel);
        }
        public void AddSkillLevel(string skillName, int level) => AddSkill(skillName, level, 0);
        public void AddSkillExperience(string skillName, int experience) => AddSkill(skillName, 0, experience);

        public SerializedProfeciencies GetSerializable()
        {
            Dictionary<string, CreatureSkill> savedSkills = new Dictionary<string, CreatureSkill>();

            // Dont bother saving skills with no levels or progress.
            // Also, we are copying skills here to avoid creating additional references.
            foreach (var skill in this.Skills.Values)
                if (skill.Level > 0 || skill.Experience > 0)
                    savedSkills.Add(skill.Name, new CreatureSkill()
                    {
                        Level = skill.Level,
                        Experience = skill.Experience,
                        Name = skill.Name
                    });

            return new SerializedProfeciencies()
            {
                Skills = savedSkills
            };
        }
    }

    class SerializedProfeciencies : ISerialized<CreatureProficiencies>
    {
        public Dictionary<string, CreatureSkill> Skills { get; set; }

        public CreatureProficiencies GetDeserialized()
        {
            CreatureProficiencies skills = new CreatureProficiencies();

            foreach (var skill in Skills.Values)
                skills.SetSkill(skill.Name, skill.Level, skill.Experience);

            return skills;
        }
    }
}
