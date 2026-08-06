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
    enum ListInfluenceMode
    {
        /// <summary> Exters no Influence. </summary>
        None,
        /// <summary>
        /// Only the smallest link in the list will be used, e.g.
        /// <para> The skill with the lowest level </para>
        /// <para> The attribute with the smallest score </para>
        /// <para> The armor class with least coverage </para>
        /// <b>Does nothing on singular links.</b>
        /// </summary>
        Least,
        /// <summary>
        /// When using a list of links, only the greatest will be used, e.g.
        /// <para> The skill with the highest level </para>
        /// <para> The attribute with the greatest score </para>
        /// <para> The armor class with the most coverage </para>
        /// <b>Does nothing on singular links.</b>
        /// </summary>
        Greatest,
        /// <summary>
        /// When using a list of links, all infleunce links are calculated and added together, then divided by the sum of the raw influence values.
        /// <para><b>Does nothing on singular links.</b></para>
        /// </summary>
        Normalized,
        /// <summary>
        /// All influence links are summed together. 
        /// <para><b>Does nothing on singular links.</b></para>
        /// </summary>
        Additive,
        /// <summary>
        /// All influence links are multiplied together.
        /// <para><b>Does nothing on singular links.</b></para>
        /// </summary>
        Multiplicative,
    };
    enum ValueInfluenceMode
    {
        /// <summary>
        /// This value is always ignored.
        /// </summary>
        Never,
        /// <summary>
        /// This value is always calculated.
        /// </summary>
        Always,
        /// <summary>
        /// Only calculated if the linked value is below a threshold.
        /// <para><b>Does nothing on singular links.</b></para>
        /// </summary>
        ThresholdBelow,
        /// <summary>
        /// Only calculated if the linked value is above a threshold.
        /// <para><b>Does nothing on singular links.</b></para>
        /// </summary>
        ThresholdAbove,
    }
    enum ModifierMode
    {
        /// <summary> Value * Influence </summary>
        Multiplier,
        /// <summary> Value / Influence </summary>
        Divisor,
        /// <summary> Influence / Value </summary>
        Dividend,
        /// <summary> Value + Influence </summary>
        Addend,
        /// <summary> Value ^ Influence </summary>
        Exponent,
        /// <summary> Influence ^ Value </summary>
        ExponentBase,
        /// <summary> Log base Influence of (Value) </summary>
        Logarithm,
        /// <summary>  Nth Root of Value </summary>
        Root
    };

    [Flags]
    internal enum DamageFlags
    {
        True = 0,
        IsBlockable = 1, // If a damage type is blockable, it can be mitigated by armor.
        IsResistable = 2
    }
}
