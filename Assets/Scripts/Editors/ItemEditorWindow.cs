using System;
using System.Collections.Generic;
using System.Linq;
using Model.Items;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    /*public class ItemEditorWindow : EditorWindow
    {
        private Item item;
        
        [MenuItem("Project Space Life/Item Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<ItemEditorWindow>();
            window.titleContent = new GUIContent("Item Editor");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Item Editor", EditorStyles.boldLabel);

            if (item == null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Create New Melee Weapon"))
                {
                    item = CreateInstance<MeleeWeapon>();
                }
                else if (GUILayout.Button("Create New Ranged Weapon"))
                {
                    item = CreateInstance<RangedWeapon>();
                }
                else if (GUILayout.Button("Create New Armor"))
                {
                    item = CreateInstance<Armor>();
                }
                GUILayout.EndHorizontal();
                
                if(item == null)
                {
                    item = (Item)EditorGUILayout.ObjectField("Select existing item", item, typeof(Item), false);
                }
            }
            else
            {
                // Basic properties
                item.Name = EditorGUILayout.TextField("Name", item.Name);

                item.Weight = EditorGUILayout.FloatField("Weight", item.Weight);
                
                if (item.GetType() == typeof(RangedWeapon))
                {
                    
                    item.ItemSlot = ItemSlot.Weapon;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Item Type");
                    EditorGUILayout.LabelField(item.ItemSlot.ToString());
                    EditorGUILayout.EndHorizontal();

                    ((RangedWeapon)item).Type = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", ((RangedWeapon)item).Type);
                    ((RangedWeapon)item).Damage = EditorGUILayout.FloatField("Damage", ((RangedWeapon)item).Damage);
                    ((RangedWeapon)item).AttackSpeed = EditorGUILayout.FloatField("Attack Speed", ((RangedWeapon)item).AttackSpeed);
                    ((RangedWeapon)item).Range = EditorGUILayout.FloatField("Range", ((RangedWeapon)item).Range);
                    ((RangedWeapon)item).ReloadTime = EditorGUILayout.FloatField("Reload Time", ((RangedWeapon)item).ReloadTime);
                    ((RangedWeapon)item).MagSize = EditorGUILayout.IntField("Mag Size", ((RangedWeapon)item).MagSize);
                    ((RangedWeapon)item).ProjectilePrefab = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", ((RangedWeapon)item).ProjectilePrefab, typeof(GameObject), false);
                }
                else if (item.GetType() == typeof(MeleeWeapon))
                {
                    item.ItemSlot = ItemSlot.Weapon;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Item Type");
                    EditorGUILayout.LabelField(item.ItemSlot.ToString());
                    EditorGUILayout.EndHorizontal();

                    ((MeleeWeapon)item).Type = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", ((MeleeWeapon)item).Type);
                    ((MeleeWeapon)item).Damage = EditorGUILayout.FloatField("Damage", ((MeleeWeapon)item).Damage);
                    ((MeleeWeapon)item).AttackSpeed = EditorGUILayout.FloatField("Attack Speed", ((MeleeWeapon)item).AttackSpeed);
                }
                else if (item.GetType() == typeof(Armor))
                {
                    var enumValues = Enum.GetValues(typeof(ItemSlot)).Cast<ItemSlot>().Where(e => e != ItemSlot.Weapon).ToList();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Select an Item Type:");
                    item.ItemSlot = EnumDropDown(item.ItemSlot, enumValues);
                    EditorGUILayout.EndHorizontal();
                    
                    ((Armor)item).LaserArmorValue = EditorGUILayout.FloatField("Armor", ((Armor)item).LaserArmorValue);
                }

                if (GUILayout.Button("Save"))
                {
                    AssetDatabase.CreateAsset(item, "Assets/SOs/Items/" + item.Name + ".asset");
                    EditorUtility.SetDirty(item);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Reset"))
                {
                    item = null;
                }
            }
        }
        
        private ItemSlot EnumDropDown(ItemSlot selectedValue, List<ItemSlot> allowedValues)
        {
            // Display a dropdown button to open the enum value selection menu
            if (GUILayout.Button(selectedValue.ToString(), EditorStyles.popup))
            {
                // Create a menu for the allowed values
                GenericMenu menu = new GenericMenu();
                foreach (var value in allowedValues)
                {
                    // Add a menu item for each allowed value
                    menu.AddItem(new GUIContent(value.ToString()), selectedValue.Equals(value), OnEnumSelected, value);
                }
                // Show the menu
                menu.ShowAsContext();
            }
            return selectedValue;
        }
        
        private void OnEnumSelected(object selectedValue)
        {
            // Update the selected value when an enum value is selected
            item.ItemSlot = (ItemSlot)selectedValue;
        }
    }*/
}