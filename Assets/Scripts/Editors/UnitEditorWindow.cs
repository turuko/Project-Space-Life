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
    /*public class UnitEditorWindow : EditorWindow
    {
        private Unit unit;

        private List<Unit> allUnits;
        private List<Equipment> allEquipments;
        private List<Species> allSpecies;
        private List<int> indexes;
        private int selectedUnitFromIndex = 0;
        private int selectedEquipmentIndex = 0;
        private int selectedSpeciesIndex = 0;

        [MenuItem("Project Space Life/Unit Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<UnitEditorWindow>("Unit Editor");
        
            window.titleContent = new GUIContent("Unit Editor");
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
            allUnits = new List<Unit>();
            allEquipments = new List<Equipment>();
            var guidsUnits = AssetDatabase.FindAssets("t:Unit");
            var guidsEquipments = AssetDatabase.FindAssets("t:Equipment");

            foreach (var guid in guidsUnits)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Unit unit = AssetDatabase.LoadAssetAtPath<Unit>(path);

                if (unit != null)
                {
                    allUnits.Add(unit);
                }
            }

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
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Unit Editor", EditorStyles.boldLabel);

            if (unit == null)
            {
                if (GUILayout.Button("Create New Unit"))
                {
                    unit = CreateInstance<Unit>();
                    unit.CombatStats = CreateInstance<StatCollection>();
                }
                else
                {
                    unit = (Unit)EditorGUILayout.ObjectField("Select existing unit", unit, typeof(Unit), false);
                }
            }
            else
            {
                // Basic properties
                unit.Name = EditorGUILayout.TextField("Name",unit.Name);
                
                if (GUILayout.Button("Generate ID"))
                {
                    unit.Id = Guid.NewGuid().ToString();
                }
                if(!string.IsNullOrEmpty(unit.Id))
                {    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("ID: ");
                    EditorGUILayout.LabelField(unit.Id.ToString());
                    EditorGUILayout.EndHorizontal();
                }
                
                selectedSpeciesIndex = EditorGUILayout.Popup("Species", selectedSpeciesIndex, GetSpeciesNames());
                
                unit.SpeciesId = allSpecies[selectedSpeciesIndex].Id;

                unit.FoodCost = EditorGUILayout.FloatField("Food Cost", unit.FoodCost);
                unit.GoldCost = EditorGUILayout.FloatField("Gold Cost", unit.GoldCost);
                unit.RecruitmentCost = EditorGUILayout.FloatField("Recruitment Cost", unit.RecruitmentCost);

                // Combat stats
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Combat Stats", EditorStyles.boldLabel);

                if (unit.CombatStats.Stats == null || unit.CombatStats.Stats.Count == 0)
                {
                    unit.CombatStats.Type = (StatType)EditorGUILayout.EnumPopup("Stat Type", unit.CombatStats.Type);
                    if (GUILayout.Button("Initialize Collection"))
                    {
                        unit.CombatStats.Init(unit.CombatStats.Type == StatType.Attribute ? StatNames.AttributeNames : StatNames.CombatNames);
                    }
                }
                else
                {
                    EditorGUI.indentLevel++;
            
                    foreach (var kvp in unit.CombatStats.Stats)
                    {
                        var stat = kvp.Value;
                        stat.BaseValue = EditorGUILayout.FloatField(kvp.Key, stat.BaseValue);
                    }

                    EditorGUI.indentLevel--;
                }

                // Upgrade tree
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Upgrade Tree", EditorStyles.boldLabel);

                var tmpOptions = GetUnitNames();
                var options = new string[tmpOptions.Length + 1];
                Array.Copy(tmpOptions, 0, options, 1, tmpOptions.Length);
                options[0] = "Null";
            
            
                selectedUnitFromIndex = EditorGUILayout.Popup("Upgrades From", selectedUnitFromIndex, options);

                if (selectedUnitFromIndex == 0)
                {
                    unit.UpgradesFrom = Guid.Empty.ToString();
                }
                else if (selectedUnitFromIndex >= 1 && selectedUnitFromIndex <= allUnits.Count)
                {
                    unit.UpgradesFrom = allUnits[selectedUnitFromIndex-1].Id;
                }
            

                if (unit.UpgradesTo == null)
                {
                    unit.UpgradesTo = new List<string>();
                    indexes = new List<int>();
                }
                else if (indexes == null || indexes.Count != unit.UpgradesTo.Count)
                {
                    indexes = new List<int>();
                    for (int i = 0; i < unit.UpgradesTo.Count; i++)
                    {
                        indexes.Add(-1);
                    }
                }

                if (GUILayout.Button("Add Upgrades To"))
                {
                    unit.UpgradesTo.Add(Guid.Empty.ToString());
                    indexes.Add(-1);
                }

                if(unit.UpgradesTo.Count > 0)
                {
                    for (int i = 0; i < unit.UpgradesTo.Count; i++)
                    {
                    
                        EditorGUILayout.BeginHorizontal();
                        indexes[i] = EditorGUILayout.Popup($"Upgrades To {i+1}", indexes[i], GetUnitNames());

                        if (indexes[i] >= 0 && indexes[i] < allUnits.Count)
                        {
                            unit.UpgradesTo[i] = allUnits[indexes[i]].Id;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            
                //Equipment
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Equipment", EditorStyles.boldLabel);

                if (unit.Equipment == null)
                {
                    var tmp = GetInventoryNames();
                    var equipmentOptions = new string[tmp.Length + 1];
                    Array.Copy(tmp, 0, equipmentOptions, 1, tmp.Length);
                    equipmentOptions[0] = "New";
                    
                    selectedEquipmentIndex = EditorGUILayout.Popup("Existing equipment", selectedEquipmentIndex, equipmentOptions);
                
                    if(GUILayout.Button("Initialize Equipment"))
                    {
                        if (selectedEquipmentIndex == 0)
                        {
                            unit.Equipment = CreateInstance<Equipment>();
                            unit.Equipment.Init();
                        }
                        else if (selectedEquipmentIndex >= 1 && selectedEquipmentIndex <= allEquipments.Count)
                        {
                            unit.Equipment = CreateInstance<Equipment>();
                            if (!allEquipments[selectedEquipmentIndex - 1].AnyItems())
                            {
                                unit.Equipment.Init();
                            }
                            else
                            {
                                unit.Equipment.Init(allEquipments[selectedEquipmentIndex-1]);
                            }
                        }
                    }
                }
                else
                {
                    if (!unit.Equipment.AnyItems())
                    {
                        unit.Equipment.Init();
                    }
                    EditorGUI.indentLevel++;
                
                    foreach (var kvp in unit.Equipment.EquipmentSlots)
                    {
                        var slots = kvp.Value;
                        for (int i = 0; i < slots.Length; i++)
                        {
                            var item = (Item)EditorGUILayout.ObjectField(kvp.Key + " " +(i+1), slots[i], typeof(Item), false);
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
            
                // Save changes to Unit
                if (GUILayout.Button("Save"))
                {
                    if (AssetDatabase.IsValidFolder("Assets/SOs/Units/" + unit.Name))
                    {
                        EditorUtility.SetDirty(unit.CombatStats);
                        EditorUtility.SetDirty(unit.Equipment);
                        EditorUtility.SetDirty(unit);
                        AssetDatabase.SaveAssets();
                        return;
                    }
                    // Create a new Unit asset
                    var guid = AssetDatabase.CreateFolder("Assets/SOs/Units", unit.Name);
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.CreateAsset(unit.CombatStats, path +"/"+ unit.Name + " CombatStats.asset");
                    AssetDatabase.CreateAsset(unit.Equipment, path +"/"+ unit.Name + " Equipment.asset");
                    AssetDatabase.CreateAsset(unit, path +"/"+ unit.Name + ".asset");

                    // Save changes to Unit asset
                    EditorUtility.SetDirty(unit.CombatStats);
                    EditorUtility.SetDirty(unit.Equipment);
                    EditorUtility.SetDirty(unit);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Reset"))
                {
                    unit = null;
                    UpdateExistingAssets();
                    selectedUnitFromIndex = 0;
                    selectedEquipmentIndex = 0;
                }
            }
        }

        private string[] GetUnitNames()
        {
            string[] names = new string[allUnits.Count];

            for (int i = 0; i < allUnits.Count; i++)
            {
                names[i] = allUnits[i].name;
            }

            return names;
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
    }*/
}
