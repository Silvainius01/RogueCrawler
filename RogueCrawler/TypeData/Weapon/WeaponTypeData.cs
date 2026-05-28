using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace RogueCrawler
{
    class WeaponTypeData
    {
        public string WeaponType { get; set; }
        public string DamageType { get; set; }
        public AttributeType MajorAttribute { get; set; }
        public AttributeType MinorAttribute { get; set; }
        public ItemWeaponHandedness Handedness { get; set; }

        public int BaseDamage { get; set; }
        public int BaseValue { get; set; }
        public float LargeWeaponDamageMult { get; set; } = 2;
        public float LargeWeaponWeightMult { get; set; } = 3;
        public string[] OneHandedWeaponNames { get; set; }
        public string[] TwoHandedWeaponNames { get; set; }
        public WeaponSubTypeData[] SubTypes { get; set; }

        public WeaponTypeData() { }
        public WeaponTypeData(string name)
        {
            WeaponType = name;
        }
    }

    class WeaponSubTypeData
    {
        // Required
        public string TypeName { get; set; }
        public AttributeType MinorAttributeOverride { get; set; }
        public ItemWeaponHandedness WeaponHandedness { get; set; }

        public WeaponSubTypeData() { }
        public WeaponSubTypeData(string name)
        {
            TypeName = name;
        }
    }
}
