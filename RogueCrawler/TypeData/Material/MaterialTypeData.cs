using CommandEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueCrawler
{
    class MaterialTypeData
    {
        public string Name { get; set; } = "Anomolous";
        public float ValueModifier { get; set; } = 1.0f;
        public float WeightModifier { get; set; } = 1.0f;
        public float QualityModifier { get; set; } = 1.0f;
        public float DamageModifier { get; set; } = 1.0f;
        public float DurabilityModifier { get; set; } = 1.0f;
        public float ArmorRatingModifier { get; set; } = 1.0f;
        public float ArmorCoverageModifier { get; set; } = 1.0f;

        public bool IsMetallic { get; set; } = false;
        public bool IsWeaponMaterial { get; set; } = false;
        public bool IsArmorMaterial { get; set; } = false;

        public MaterialTypeData() { }
        public MaterialTypeData(string name)
        {
            Name = name;
        }
    }
}
