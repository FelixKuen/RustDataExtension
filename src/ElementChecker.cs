using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

namespace RustDataHarmony
{
    public static class ElementChecker
    {
        public static HashSet<SoundEntityStruct> Entities = new HashSet<SoundEntityStruct>();
        public static HashSet<SoundEntityStruct> Sounds = new HashSet<SoundEntityStruct>();
        public static HashSet<SoundEntityStruct> Items = new HashSet<SoundEntityStruct>();

        public static void Start()
        {
            GetElements();
        }

        private static void GetElements()
        {
            GetItems();
            GetEntities();
            GetSounds();
        }

        private static void GetItems()
        {
            var itemNames = new HashSet<string>();

            foreach (var item in ItemManager.itemList)
            {
                var enumName = Utils.MakeStringEnumReady(item.shortname);
                var uniqueName = GetUniqueName(enumName, item.shortname, itemNames);

                itemNames.Add(uniqueName);
                Items.Add(new SoundEntityStruct
                {
                    EnumElementName = uniqueName,
                    Path = item.itemid.ToString()
                });
            }
        }

        private static void GetEntities()
        {
            var entities = new HashSet<SoundEntityStruct>();
            var entityNames = new HashSet<string>();

            foreach (var prefab in FileSystem.Backend.cache)
            {
                if (prefab.Value is GameObject go && go.HasComponent<BaseEntity>())
                {
                    var entity = go.GetComponent<BaseEntity>();
                    
                    var originalName = GetUniqueName(entity.name, prefab.Key, entityNames);
                    var uniqueName = Utils.MakeStringEnumReady(originalName);
                    
                    if (entityNames.Contains(uniqueName))
                        continue;
                    
                    entityNames.Add(uniqueName);
                    entities.Add(new SoundEntityStruct
                    {
                        EnumElementName = uniqueName,
                        Path = prefab.Key
                    });
                }
            }

            Entities = entities;
        }

        private static void GetSounds()
        {
            var effects = new HashSet<SoundEntityStruct>();
            var soundNames = new HashSet<string>();

            foreach (var numberKey in StringPool.toNumber.Keys)
            {
                if (numberKey.Contains("/effects/") || numberKey.Contains("/fx/"))
                {
                    var originalName = GetUniqueName(numberKey.Split("/").Last().Split(".")[0], numberKey, soundNames);
                    
                    var uniqueName = Utils.MakeStringEnumReady(originalName);
                    
                    if (soundNames.Contains(uniqueName))
                        continue;

                    soundNames.Add(uniqueName);
                    effects.Add(new SoundEntityStruct
                    {
                        EnumElementName = uniqueName,
                        Path = numberKey
                    });
                }
            }

            Sounds = effects;
        }

        /// <summary>
        /// Recursively generates a unique name by prepending folder names if needed.
        /// </summary>
        private static string GetUniqueName(string baseName, string path, HashSet<string> existingNames)
        {
            if (!existingNames.Contains(baseName))
                return baseName;

            var parts = path.Split('/');
            
            for (int i = parts.Length - 2; i >= 0; i--)
            {
                string newName = $"{parts[i]}_{baseName}";
                if (!existingNames.Contains(newName))
                    return newName;
            }

            // Falls immer noch doppelt, eine Zahl anhängen
            int count = 1;
            string finalName = baseName;
            while (existingNames.Contains(finalName))
            {
                finalName = $"{baseName}_{count}";
                count++;
            }

            return finalName;
        }
    }

    
    public struct SoundEntityStruct
    {
        public string EnumElementName;
        public string Path;
    }
  
}