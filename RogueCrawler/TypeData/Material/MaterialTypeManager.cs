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
    class MaterialTypeManager
    {
        public static string DataPath = $"{DungeonCrawlerManager.TextPath}\\Data\\ItemMaterials.json";

        public static bool Loaded = false;
        public static MaterialTypeData DefaultMaterial => Materials[DungeonConstants.MaterialIron];
        public static Dictionary<string, MaterialTypeData> Materials = new Dictionary<string, MaterialTypeData>();

        public static MappedCommandModule<MaterialTypeData> MaterialNameCommandModule;

        public static void LoadMaterials()
        {
            StreamReader reader = new StreamReader(DataPath);
            string json = reader.ReadToEnd();
            reader.Close();

            int index = 0;
            var serializer = JsonSerializer.CreateDefault();
            var jArray = JsonConvert.DeserializeObject<JArray>(json);

            foreach (var obj in jArray)
            {
                if (IsValidMaterial(obj, index))
                {
                    MaterialTypeData data = (MaterialTypeData)serializer.Deserialize(new JTokenReader(obj), typeof(MaterialTypeData));
                    Materials.Add(data.Name, data);
                }
                ++index;
            }
            Loaded = true;
            MaterialNameCommandModule = new MappedCommandModule<MaterialTypeData>("What is the default material name prompt??", Materials);
        }

        public static MaterialTypeData GetMaterialFromName(string name)
        {
            if (name is null || !Materials.ContainsKey(name))
            {
                ConsoleExt.WriteWarning($"Material '{name}' doesnt exist. Defaulting to {DefaultMaterial.Name}.");
                return DefaultMaterial;
            }
            return Materials[name];
        }

        static bool IsValidMaterial(JToken obj, int index)
        {
            int sbLength = 0;
            string starterString = $"Invalid material detected at position [{index}]";
            // SmartStringBuilder sb = new SmartStringBuilder();

            using (ManagedStringBuilder msb = new ManagedStringBuilder("InvalidMaterial", starterString))
            {
                var sb = msb.Builder;
                sb.Append(starterString);
                sb.NewlineAppend(1, "Missing Required Fields:");
                sbLength = sb.Length;

                if (obj["Name"] is null)
                    sb.NewlineAppend(2, "Name -> string");
                if (obj["IsMetallic"] is null)
                    sb.NewlineAppend(2, "IsMetallic -> bool");
                if (obj["IsWeaponMaterial"] is null)
                    sb.NewlineAppend(2, "IsWeaponMaterial -> bool");
                if (obj["IsArmorMaterial"] is null)
                    sb.NewlineAppend(2, "IsArmorMaterial -> bool");

                if (sb.Length > sbLength)
                {
                    ConsoleExt.WriteWarningLine(sb.ToString());
                    return false;
                }
            }
            return true;
        }

        public static void GenerateDefaultTypes()
        {
            List<MaterialTypeData> materials = new List<MaterialTypeData>(16)
            {
                new MaterialTypeData(DungeonConstants.MaterialLeather)
                {
                    ValueModifier = 0.75f,
                    WeightModifier = 0.75f,
                    DurabilityModifier = 0.6f,
                    IsArmorMaterial = true,
                },
                new MaterialTypeData(DungeonConstants.MaterialWood)
                {
                    ValueModifier = 0.75f,
                    WeightModifier = 0.75f,
                    DurabilityModifier = 0.6f,
                    IsWeaponMaterial = true,
                },
                new MaterialTypeData(DungeonConstants.MaterialCopper)
                {
                    ValueModifier = 0.75f,
                    WeightModifier = 0.75f,
                    DurabilityModifier = 0.6f,
                    IsMetallic = true,
                    IsWeaponMaterial = true,
                    IsArmorMaterial = true,
                },
                new MaterialTypeData(DungeonConstants.MaterialBronze)
                {
                    ValueModifier = 0.75f,
                    WeightModifier = 0.75f,
                    DurabilityModifier = 0.6f,
                    IsMetallic = true,
                    IsWeaponMaterial = true,
                    IsArmorMaterial = true,
                },
                new MaterialTypeData(DungeonConstants.MaterialIron)
                {
                    IsMetallic = true,
                    IsWeaponMaterial = true,
                    IsArmorMaterial = true,
                },
                new MaterialTypeData(DungeonConstants.MaterialSteel)
                {
                    ValueModifier = 1.5f,
                    DamageModifier = 1.25f,
                    DurabilityModifier = 1.25f,
                    IsMetallic = true,
                    IsWeaponMaterial = true,
                    IsArmorMaterial = true,
                },
                new MaterialTypeData(DungeonConstants.MaterialMythryl)
                {
                    ValueModifier = 2.0f,
                    WeightModifier = 0.75f,
                    DamageModifier = 1.75f,
                    DurabilityModifier = 1.5f,
                    IsMetallic = true,
                    IsWeaponMaterial = true,
                    IsArmorMaterial = true,
                },
                new MaterialTypeData(DungeonConstants.MaterialManasteel)
                {
                    ValueModifier = 1.7f,
                    WeightModifier = 0.75f,
                    DamageModifier = 1.75f,
                    DurabilityModifier = 1.5f,
                    IsMetallic = true,
                    IsWeaponMaterial = true,
                    IsArmorMaterial = true,
                },
                new MaterialTypeData(DungeonConstants.MaterialAdamantite)
                {
                    ValueModifier = 3.0f,
                    WeightModifier = 1.5f,
                    DamageModifier = 2.0f,
                    IsMetallic = true,
                    IsWeaponMaterial = true,
                    IsArmorMaterial = true,
                },
            };

            using StreamWriter writer = new StreamWriter(DataPath);
            writer.Write(JsonConvert.SerializeObject(materials));
            writer.Close();
        }
    }
}
