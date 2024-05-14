using System;
using System.Collections.Generic;
using System.Linq;
using Model.Stat_System;
using Model.Universe;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Editors
{
    /*public class SpeciesEditorWindow : EditorWindow
    {
        private Species species;

        private List<Species> allSpecies;

        private List<string> statNames;
        [MenuItem("Project Space Life/Species Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<SpeciesEditorWindow>();
            window.titleContent = new GUIContent("Species Editor");
            window.Show();
        }
        
        private void OnEnable()
        {
            UpdateExistingAssets();
            statNames = new List<string>();
            statNames = StatNames.AttributeNames.Concat(StatNames.CombatNames).ToList();
        }
    
        private void OnFocus()
        {
            UpdateExistingAssets();
            statNames = new List<string>();
            statNames = StatNames.AttributeNames.Concat(StatNames.CombatNames).ToList();
        }

        private void UpdateExistingAssets()
        {
            allSpecies = new List<Species>();
            var guidsUnits = AssetDatabase.FindAssets("t:Species");

            foreach (var guid in guidsUnits)
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
            EditorGUILayout.LabelField("Species Editor", EditorStyles.boldLabel);

            if (species == null)
            {
                if (GUILayout.Button("Create New Species"))
                {
                    species = CreateInstance<Species>();
                    species.Name = "";
                }
                else
                {
                    species = (Species)EditorGUILayout.ObjectField("Select existing species", species, typeof(Species),
                        false);
                }
            }
            else
            {
                species.Name = EditorGUILayout.TextField("Name", species.Name);
                
                if (GUILayout.Button("Generate ID"))
                {
                    species.Id = Guid.NewGuid().ToString();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("ID: ");
                EditorGUILayout.LabelField(species.Id);
                EditorGUILayout.EndHorizontal();
                
                //Size
                if (species.Size == null)
                {
                    species.Size = new ConstrainedFloat(0, 0, 0);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    species.Size.Value = EditorGUILayout.FloatField("Initial Size", species.Size.Value);
                    species.Size.Min = EditorGUILayout.FloatField("Minimum Size", species.Size.Min);
                    species.Size.Max = EditorGUILayout.FloatField("Maximum Size", species.Size.Max);
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                //Species relations
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Species Relations", EditorStyles.boldLabel);
                
                if (species.SpeciesRelation == null)
                {
                    species.InitRelations(allSpecies);
                }
                else
                {
                    if (species.SpeciesRelation.Count > allSpecies.Count)
                    {
                        foreach (var speciesRelationKey in species.SpeciesRelation.Keys)
                        {
                            if (allSpecies.All(s => s.Id != speciesRelationKey))
                            {
                                species.SpeciesRelation.Remove(speciesRelationKey);
                            }
                        }
                    }
                    else if (species.SpeciesRelation.Count < allSpecies.Count)
                    {
                        foreach (var spec in allSpecies)
                        {
                            if (!species.SpeciesRelation.ContainsKey(spec.Id))
                            {
                                species.SpeciesRelation.Add(spec.Id, spec.Name == species.Name ? 100f : 0f);
                            }
                        }
                    }
                    else
                    {
                        EditorGUI.indentLevel++;
                        var keysToModify = species.SpeciesRelation.Keys.ToList(); // Create a copy of keys

                        foreach (var key in keysToModify)
                        {
                            var value = species.SpeciesRelation[key];

                            EditorGUI.BeginDisabledGroup(allSpecies.First(s => s.Id == key).Name == species.Name);

                            // Use EditorGUI to modify the value
                            value = EditorGUILayout.FloatField(allSpecies.First(s => s.Id == key).Name, value);

                            // Update the dictionary with the modified value
                            species.SpeciesRelation[key] = value;

                            EditorGUI.EndDisabledGroup();
                        }
                        EditorGUI.indentLevel--;
                    }
                }
                
                //Stat bonuses
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Species Stat Bonuses", EditorStyles.boldLabel);

                if (species.SpeciesStatBonuses == null)
                {
                    species.InitBonuses(statNames);
                }
                else
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < species.SpeciesStatBonuses.Count; i++)
                    {
                        var key = species.SpeciesStatBonuses.Keys.ElementAt(i);
                        EditorGUILayout.BeginHorizontal();
                        
                        EditorGUILayout.LabelField(key);

                        if (species.SpeciesStatBonuses[key] == null || species.SpeciesStatBonuses[key].Type == StatModType.None)
                        {
                            if (GUILayout.Button("Create Species Bonus"))
                            {
                                species.SpeciesStatBonuses[key] = new StatModifier(0f, StatModType.Flat, species);
                            }
                            else
                            {
                                species.SpeciesStatBonuses[key] = null;
                            }
                        }
                        else
                        {
                            species.SpeciesStatBonuses[key].Value = EditorGUILayout.FloatField("Bonus Value", species.SpeciesStatBonuses[key].Value);
                            species.SpeciesStatBonuses[key].Type = (StatModType)EditorGUILayout.EnumPopup("Bonus type", species.SpeciesStatBonuses[key].Type);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            
                // Save changes to Unit
                if (GUILayout.Button("Save"))
                {
                    
                    AssetDatabase.CreateAsset(species, "Assets/SOs/Species/"+ species.Name + ".asset");
                    // Save changes to Unit asset
                    EditorUtility.SetDirty(species);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Reset"))
                {
                    species = null;
                    UpdateExistingAssets();
                }
            }
        }
    }*/
}