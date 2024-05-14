using System;
using System.Collections.Generic;
using Model.Factions;
using Model.Items;
using Model.Stat_System;
using Model.Universe;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    /*public class CharacterEditorWindow : EditorWindow
    {
        private Character character;

        private List<Equipment> allEquipments;
        private List<Species> allSpecies;
        private List<Faction> allFactions;
        private int selectedEquipmentIndex = 0;
        private int selectedSpeciesIndex = 0;
        private int selectedFactionIndex = 0;

        private Vector2 scrollPosition = Vector2.zero;

        [MenuItem("Project Space Life/Character Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<CharacterEditorWindow>();
            window.titleContent = new GUIContent("Character Editor");
            window.Show();
        }

        private void OnEnable()
        {
            UpdateExistingAssets();
        }
    
        private void OnFocus()
        {
            UpdateExistingAssets();
        }

        private void UpdateExistingAssets()
        {
            allEquipments = new List<Equipment>();
            var guidsEquipments = AssetDatabase.FindAssets("t:Equipment");


            foreach (var guid in guidsEquipments)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Equipment equipment = AssetDatabase.LoadAssetAtPath<Equipment>(path);

                if (equipment != null)
                {
                    allEquipments.Add(equipment);
                }
            }
            
            allSpecies = new List<Species>();
            var guidsSpecies = AssetDatabase.FindAssets("t:Species");


            foreach (var guid in guidsSpecies)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Species species = AssetDatabase.LoadAssetAtPath<Species>(path);

                if (species != null)
                {
                    allSpecies.Add(species);
                }
            }
            
            allFactions = new List<Faction>();
            var guidsFactions = AssetDatabase.FindAssets("t:Faction");


            foreach (var guid in guidsFactions)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Faction faction = AssetDatabase.LoadAssetAtPath<Faction>(path);

                if (faction != null)
                {
                    allFactions.Add(faction);
                }
            }
        }

        private void OnGUI()
        {
        
            EditorGUILayout.LabelField("Character Editor", EditorStyles.boldLabel);

            if (character == null)
            {
                if (GUILayout.Button("Create New Character"))
                {
                    character = CreateInstance<Character>();
                    character.CombatStats = CreateInstance<StatCollection>();
                    character.Attributes = CreateInstance<StatCollection>();
                    character.Id = Guid.Empty.ToString();
                }
                else
                {
                    character = (Character)EditorGUILayout.ObjectField("Select existing character", character,
                        typeof(Character), false);
                }
            }
            else
            {
            
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                // Basic properties
                character.Name = EditorGUILayout.TextField("Name", character.Name);

                if (GUILayout.Button("Generate ID"))
                {
                    character.Id = Guid.NewGuid().ToString();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("ID: ");
                EditorGUILayout.LabelField(character.Id);
                EditorGUILayout.EndHorizontal();
                
                
                selectedSpeciesIndex = EditorGUILayout.Popup("Species", selectedSpeciesIndex, GetSpeciesNames());
                
                character.SpeciesId = allSpecies[selectedSpeciesIndex].Id;

                character.FoodCost = EditorGUILayout.FloatField("Food Cost", character.FoodCost);
                character.GoldCost = EditorGUILayout.FloatField("Gold Cost", character.GoldCost);
                character.CurrentHealth =
                    character.MaxHealth = EditorGUILayout.FloatField("Max Health", character.MaxHealth);
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Current Health");
                EditorGUILayout.LabelField(character.CurrentHealth.ToString());
                EditorGUILayout.EndHorizontal();
            
                character.Level = EditorGUILayout.IntField("Level", character.Level);
                character.AttributePoints = EditorGUILayout.IntField("Attribute points", character.AttributePoints);
                character.CombatSkillPoints =
                    EditorGUILayout.IntField("Combat skill points", character.CombatSkillPoints);
                
                selectedFactionIndex = EditorGUILayout.Popup("Faction", selectedFactionIndex, GetFactionNames());
                
                character.FactionId = allFactions[selectedFactionIndex].Id;

                // Combat stats
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Combat Stats", EditorStyles.boldLabel);

                if (character.CombatStats.Stats == null)
                {
                    character.CombatStats.Type =
                        (StatType)EditorGUILayout.EnumPopup("Stat Type", character.CombatStats.Type);
                    if (GUILayout.Button("Initialize Collection"))
                    {
                        character.CombatStats.Init(character.CombatStats.Type == StatType.Attribute
                            ? StatNames.AttributeNames
                            : StatNames.CombatNames);
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;

                    foreach (var kvp in character.CombatStats.Stats)
                    {
                        var stat = kvp.Value;
                        stat.BaseValue = EditorGUILayout.FloatField(kvp.Key, stat.BaseValue);
                    }

                    EditorGUI.indentLevel--;
                }

                // Attribute stats
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Attribute Stats", EditorStyles.boldLabel);

                if (character.Attributes.Stats == null)
                {
                    character.Attributes.Type =
                        (StatType)EditorGUILayout.EnumPopup("Stat Type", character.Attributes.Type);
                    if (GUILayout.Button("Initialize Collection"))
                    {
                        character.Attributes.Init(character.Attributes.Type == StatType.Attribute
                            ? StatNames.AttributeNames
                            : StatNames.CombatNames);
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;

                    foreach (var kvp in character.Attributes.Stats)
                    {
                        var stat = kvp.Value;
                        stat.BaseValue = EditorGUILayout.FloatField(kvp.Key, stat.BaseValue);
                    }

                    EditorGUI.indentLevel--;
                }

                //Equipment
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Equipment", EditorStyles.boldLabel);

                if (character.Equipment == null)
                {
                    var tmp = GetInventoryNames();
                    var equipmentOptions = new string[tmp.Length + 1];
                    Array.Copy(tmp, 0, equipmentOptions, 1, tmp.Length);
                    equipmentOptions[0] = "New";

                    selectedEquipmentIndex =
                        EditorGUILayout.Popup("Existing equipment", selectedEquipmentIndex, equipmentOptions);

                    if (GUILayout.Button("Initialize Equipment"))
                    {
                        if (selectedEquipmentIndex == 0)
                        {
                            character.Equipment = CreateInstance<Equipment>();
                            character.Equipment.Init();
                        }
                        else if (selectedEquipmentIndex >= 1 && selectedEquipmentIndex <= allEquipments.Count)
                        {
                            character.Equipment = CreateInstance<Equipment>();
                            if (!allEquipments[selectedEquipmentIndex - 1].AnyItems())
                            {
                                character.Equipment.Init();
                            }
                            else
                            {
                                character.Equipment.Init(allEquipments[selectedEquipmentIndex - 1]);
                            }
                        }
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;

                    foreach (var kvp in character.Equipment.EquipmentSlots)
                    {
                        var slots = kvp.Value;
                        for (int i = 0; i < slots.Length; i++)
                        {
                            var item = (Item)EditorGUILayout.ObjectField(kvp.Key + " " + (i + 1), slots[i],
                                typeof(Item), false);
                            if (item != null && item.ItemSlot != kvp.Key)
                            {
                                EditorUtility.DisplayDialog("Wrong Item Type",
                                    item.Name + " does not have the same item type as the slot, " + kvp.Key, "OK");
                                item = null;
                            }

                            slots[i] = item;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            
            
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

                // Save changes to Character
                if (GUILayout.Button("Save"))
                {
                    if (AssetDatabase.IsValidFolder("Assets/SOs/Units/" + character.Name))
                    {
                        EditorUtility.SetDirty(character.CombatStats);
                        EditorUtility.SetDirty(character.Attributes);
                        EditorUtility.SetDirty(character.Equipment);
                        EditorUtility.SetDirty(character);
                        AssetDatabase.SaveAssets();
                        return;
                    }

                    // Create a new Character asset
                    var guid = AssetDatabase.CreateFolder("Assets/SOs/Characters", character.Name);
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.CreateAsset(character.CombatStats,
                        path + "/" + character.Name + " CombatStats.asset");
                    AssetDatabase.CreateAsset(character.Attributes,
                        path + "/" + character.Name + " Attributes.asset");
                    AssetDatabase.CreateAsset(character.Equipment, path + "/" + character.Name + " Equipment.asset");
                    AssetDatabase.CreateAsset(character, path + "/" + character.Name + ".asset");

                    // Save changes to Character asset
                    EditorUtility.SetDirty(character.CombatStats);
                    EditorUtility.SetDirty(character.Attributes);
                    EditorUtility.SetDirty(character.Equipment);
                    EditorUtility.SetDirty(character);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Reset"))
                {
                    character = null;
                    UpdateExistingAssets();
                    selectedEquipmentIndex = 0;
                    scrollPosition = Vector2.zero;
                }
            
                EditorGUILayout.EndScrollView();
            }
        }

        private string[] GetInventoryNames()
        {
            string[] names = new string[allEquipments.Count];

            for (int i = 0; i < allEquipments.Count; i++)
            {
                names[i] = allEquipments[i].name;
            }

            return names;
        }
        
        private string[] GetSpeciesNames()
        {
            string[] names = new string[allSpecies.Count];

            for (int i = 0; i < allSpecies.Count; i++)
            {
                names[i] = allSpecies[i].name;
            }

            return names;
        }
        
        private string[] GetFactionNames()
        {
            string[] names = new string[allFactions.Count];

            for (int i = 0; i < allFactions.Count; i++)
            {
                names[i] = allFactions[i].name;
            }

            return names;
        }
    }*/
}