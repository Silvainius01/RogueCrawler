using CommandEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RogueCrawler
{
    class ItemWeaponGenerator : BaseDungeonObjectGenerator<ItemWeapon, ItemWeaponGenerationParameters>
    {
        static Dictionary<string, WeaponTypeData> WeaponTypes => WeaponTypeManager.WeaponTypes;

        public override ItemWeapon Generate(ItemWeaponGenerationParameters wParams)
        {
            wParams.Validate();

            string weaponType = null;

            if (!wParams.PossibleWeaponTypes.Any())
                weaponType = WeaponTypeManager.RandomType;
            else weaponType = wParams.PossibleWeaponTypes.RandomItem();

            WeaponTypeData weaponTypeData = WeaponTypes[weaponType];

            ItemWeapon weapon = new ItemWeapon()
            {
                ID = NextId,
                Weight = CommandEngine.Random.NextInt(wParams.WeightRange, true) / 10.0f,
                BaseValue = weaponTypeData.BaseValue,
                Material = wParams.Material,
                Quality = GetQuality(wParams),

                WeaponType = weaponType,
                BaseDamage = weaponTypeData.BaseDamage,
                DamageType = DamageTypeManager.DamageTypes[weaponTypeData.DamageType],
                MajorAttribute = weaponTypeData.MajorAttribute,
                MinorAttribute = weaponTypeData.MinorAttribute,
                AttributeRequirements = new CrawlerAttributeSet(),
            };

            weapon.AttributeRequirements.SetAttribute(AttributeType.STR, (int)Math.Ceiling(weapon.Weight / DungeonSettings.WeaponWeightPerStr));
            weapon.ObjectName = GetWeaponName(weaponTypeData, weapon.IsLargeWeapon);
            weapon.ItemName = GetDisplayName(weapon);

            if (weaponTypeData.SubTypes.TryFirst(std => std.TypeName == weapon.ObjectName, out var subType))
            {
                weapon.MinorAttribute = subType.MinorAttributeOverride;
                weapon.Handedness = subType.WeaponHandedness;
            }

            if (weapon.IsLargeWeapon)
            {
                weapon.BaseDamage *= weaponTypeData.LargeWeaponDamageMult;
                weapon.Weight *= weaponTypeData.LargeWeaponWeightMult;
            }

            return weapon;
        }

        public ItemWeapon FromSerialized(SerializedWeapon serialized)
        {
            var weaponType = serialized.WeaponType;

            // Update for legacy weapons
            if (serialized.DamageType is null || serialized.DamageType == string.Empty)
                serialized.DamageType = WeaponTypeManager.WeaponTypes[weaponType].DamageType;

            ItemWeapon weapon = new ItemWeapon()
            {
                ID = NextId,
                ItemName = serialized.ItemName,
                ObjectName = serialized.ObjectName,
                Weight = serialized.Weight,
                Quality = serialized.Quality,
                BaseValue = serialized.BaseValue,
                Material = MaterialTypeManager.GetMaterialFromName(serialized.MaterialName),
                Condition = serialized.Condition,
                MaxCondition = serialized.MaxCondition,

                WeaponType = weaponType,
                BaseDamage = serialized.BaseDamage,
                DamageType = DamageTypeManager.DamageTypes[serialized.DamageType],
                MinorAttribute = WeaponTypes[weaponType].MinorAttribute,
                MajorAttribute = WeaponTypes[weaponType].MajorAttribute,
                Handedness = serialized.Handedness,
            };

            return weapon;
        }

        public ItemWeapon GenerateUnarmed(Creature c)
        {
            ItemWeapon unarmedWeapon = new ItemWeapon()
            {
                ID = -1,
                BaseDamage = c.GetAttribute(AttributeType.STR),
                DamageType = DamageTypeManager.PhysicalDamage,
                Weight = 1.0f,
                Quality = 1.0f,
                BaseValue = 0,
                ObjectName = "Unarmed",
                WeaponType = "Blunt",
                ItemName = "Bare Fists",
                Handedness = ItemWeaponHandedness.Both,
                MajorAttribute = AttributeType.STR,
                MinorAttribute = AttributeType.DEX,
                Material = MaterialTypeManager.DefaultMaterial
            };

            unarmedWeapon.Quality += CreatureSkillUtility.GetWeaponSkillBonus(unarmedWeapon, c.Proficiencies);

            return unarmedWeapon;
        }

        string GetWeaponName(WeaponTypeData typeData, bool isLarge)
        {
            return isLarge
                ? typeData.TwoHandedWeaponNames.RandomItem()
                : typeData.OneHandedWeaponNames.RandomItem();
        }
        string GetDisplayName(ItemWeapon weapon)
        {
            StringBuilder builder = new StringBuilder();

            //if (weapon.IsLargeWeapon)
            //    builder.Append("Large");

            builder.Append(DungeonHelper.GetQualityPrefix(weapon.Quality));
            builder.Append(weapon.Material.Name);
            builder.Append(weapon.ObjectName);
            return builder.ToString();
        }

        int GetStrengthReq(float weight)
            => (int)Math.Ceiling(weight / 5.0);

        bool IsLargeWeapon(WeaponTypeData weaponType, ItemWeaponGenerationParameters wParams)
        {
            return
                weaponType.Handedness != ItemWeaponHandedness.One && (
                weaponType.Handedness == ItemWeaponHandedness.Two ||
                CommandEngine.Random.NextInt(100) < wParams.LargeWeaponProbability);
        }
    }
}