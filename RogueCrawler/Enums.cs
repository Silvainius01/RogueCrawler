using System;
using System.Collections.Generic;
using System.Text;
using CommandEngine;

namespace RogueCrawler
{
    enum DungeonSize { Small, Medium, Large, Huge }
    enum AttributeType { STR, CON, AGI, DEX, INT, WIL, CHA, LCK }
    enum Direction { North, East, South, West }
    enum DungeonChestType { Weapon, Armor }
    enum QualityLevel { Low, Normal, Superior, Exalted, Legendary, Divine }
    enum ItemWeaponLargeRate { None, Low, Mid, High, All }
    enum ArmorSlotType { Head, Chest, Arm, Hand, Waist, Foot }
    enum ItemWeaponHandedness { Both, One, Two }
    enum DamageCategory { True, Physical, Magical, Elemental, Divine }

    // Skill Stuff
    enum InfluenceMode { None, Least, Greatest, Normalized, Added, Linear, Threshold };

    [Flags]
    internal enum DamageFlags
    {
        True = 0,
        IsBlockable = 1, // If a damage type is blockable, it can be mitigated by armor.
        IsResistable = 2
    }
}
