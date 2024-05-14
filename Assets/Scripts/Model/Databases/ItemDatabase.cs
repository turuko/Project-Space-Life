using System;
using System.Collections.Generic;
using Model.Items;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Model.Databases
{
    public static class ItemDatabase
    {
        private static Dictionary<string, Item> itemMap;

        public static void Init()
        {
            itemMap = new Dictionary<string, Item>();

            var guids = AssetDatabase.FindAssets("t:Item");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var itemAsset = AssetDatabase.LoadAssetAtPath<Item>(path);

                var item = itemAsset.Clone();
                
                itemMap.Add(item.Id, item);
            }
        }

        public static Item GetItem(string id)
        {
            if (itemMap.TryGetValue(id, out Item item))
            {
                return item switch
                {
                    Weapon weapon => weapon switch
                    {
                        RangedWeapon rangedWeapon => rangedWeapon,
                        MeleeWeapon meleeWeapon => meleeWeapon,
                        _ => weapon
                    },
                    Armor armor => armor,
                    _ => item
                };
            }
            else
            {
                // Handle the case when the item with the given ID is not found.
                // You might want to return null or throw an exception, depending on your requirements.
                return null;
            }
        }

        public static GameObject GetSpaceshipGameObject(Item spaceship)
        {
            throw new NotImplementedException();
        }
    }
}