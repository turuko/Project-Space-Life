using System;
using System.Collections.Generic;
using System.Linq;
using Model.Factions;
using Model.Universe;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Editors
{
    /*public class FactionEditorWindow : EditorWindow
    {
        private Faction faction;

        private Settlement capital;
        
        private List<Character> allCharacters;
        private int selectedLeaderIndex = -1;
        private int selectedCommanderIndex = -1;
        private int selectedCharacterIndex = -1;
        private Character commanderToAdd;
        private Character characterToAdd;
        
        private List<Banner> allBanners;
        private int selectedBannerIndex = -1;
        
        private List<Species> allSpecies;
        private Species speciesToAdd;
        private int selectedSpeciesIndex = -1;

        private List<Settlement> allSettlements;
        
        [MenuItem("Project Space Life/Faction Editor")]
        private static void ShowWindow()
        {
            var window = GetWindow<FactionEditorWindow>();
            window.titleContent = new GUIContent("Faction Editor");
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
            allCharacters = new List<Character>();
            allSpecies = new List<Species>();
            allSettlements = new List<Settlement>();
            allBanners = new List<Banner>();
            var guids = AssetDatabase.FindAssets("t:Character");
            var guidsSpecies = AssetDatabase.FindAssets("t:Species");
            var guidsSettlements = AssetDatabase.FindAssets("t:Settlement");
            var guidsBanners = AssetDatabase.FindAssets("t:Banner");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Character character = AssetDatabase.LoadAssetAtPath<Character>(path);

                if (character != null)
                {
                    allCharacters.Add(character);
                }
            }
            
            foreach (var guid in guidsSpecies)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Species species = AssetDatabase.LoadAssetAtPath<Species>(path);

                if (species != null)
                {
                    allSpecies.Add(species);
                }
            }
            
            foreach (var guid in guidsSettlements)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Settlement settlement = AssetDatabase.LoadAssetAtPath<Settlement>(path);

                if (settlement != null)
                {
                    allSettlements.Add(settlement);
                }
            }
            
            foreach (var guid in guidsBanners)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Banner banner = AssetDatabase.LoadAssetAtPath<Banner>(path);

                if (banner != null)
                {
                    allBanners.Add(banner);
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Faction Editor", EditorStyles.boldLabel);
            if (faction == null)
            {
                if (GUILayout.Button("Create New Faction"))
                {
                    faction = CreateInstance<Faction>();
                    faction.CommanderIds = new SerializableSet<string>();
                    faction.AllCharacterIds = new SerializableSet<string>();
                    faction.CoreSpeciesIds = new SerializableSet<string>();
                    faction.SettlementIds = new SerializableSet<string>();
                    faction.Id = Guid.NewGuid().ToString();
                    capital = CreateInstance<Settlement>();
                    faction.AddSettlement(capital);
                }
                else
                {
                    faction = (Faction)EditorGUILayout.ObjectField("Select existing faction", faction,
                        typeof(Faction), false);

                    if (faction != null)
                    {
                        capital = allSettlements.FirstOrDefault(s=> s.Id == faction.CapitalId);
                    }
                }
            }
            else
            {
                faction.Name = EditorGUILayout.TextField("Name", faction.Name);
                
                if (GUILayout.Button("Generate ID"))
                {
                    faction.Id = Guid.NewGuid().ToString();
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("ID: ");
                EditorGUILayout.LabelField(faction.Id);
                EditorGUILayout.EndHorizontal();

                selectedLeaderIndex = EditorGUILayout.Popup("Leader", selectedLeaderIndex, GetCharacterNames());

                if (selectedLeaderIndex >= 0 && selectedLeaderIndex < allCharacters.Count)
                {
                    faction.LeaderId = allCharacters[selectedLeaderIndex].Id;
                    faction.AddCommander(allCharacters[selectedLeaderIndex]);
                }
                
                selectedBannerIndex = EditorGUILayout.Popup("Banner", selectedBannerIndex, GetBannerNames());
                if (selectedBannerIndex >= 0 && selectedBannerIndex < allBanners.Count)
                {
                    faction.BannerId = allBanners[selectedBannerIndex].Id;
                }

                //Capital
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Capital", EditorStyles.boldLabel);

                if (capital == null)
                    capital = (Settlement)EditorGUILayout.ObjectField("Capital", capital, typeof(Settlement), false);
                else
                {
                    if (string.IsNullOrEmpty(capital.Id) || capital.Id == Guid.Empty.ToString())
                    {
                        capital.Id = Guid.NewGuid().ToString();
                    }
                    capital.Name = EditorGUILayout.TextField("Capital name", capital.Name);
                    capital.SettlementType = (SettlementType)EditorGUILayout.EnumPopup("Settlement Type", capital.SettlementType);
                    capital.RequiredSettlementSlots = (int)capital.SettlementType;
            
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Required Settlement Slots");
                    EditorGUILayout.LabelField(capital.RequiredSettlementSlots.ToString());
                    EditorGUILayout.EndHorizontal();
                }

                //Core Species
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Core Species", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal();

                selectedSpeciesIndex = EditorGUILayout.Popup("Species", selectedSpeciesIndex, GetSpeciesNames());

                if (selectedSpeciesIndex >= 0 && selectedSpeciesIndex < allSpecies.Count)
                {
                    speciesToAdd = allSpecies[selectedSpeciesIndex];
                }

                if (GUILayout.Button("Add Core Species"))
                {
                    faction.CoreSpeciesIds.Add(speciesToAdd.Id);
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.LabelField("Core Species");
                if(faction.CoreSpeciesIds.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < faction.CoreSpeciesIds.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.PrefixLabel($"Core Species {i + 1}:");
                        EditorGUILayout.LabelField(allSpecies.First(c => c.Id == faction.CoreSpeciesIds[i]).Name);

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }                
                //Commanders
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("Commanders", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                selectedCommanderIndex = EditorGUILayout.Popup("Commander", selectedCommanderIndex, GetCharacterNames());

                if (selectedCommanderIndex >= 0 && selectedCommanderIndex < allCharacters.Count)
                {
                    commanderToAdd = allCharacters[selectedCommanderIndex];
                }

                if (GUILayout.Button("Add Commander"))
                {
                    faction.AddCommander(commanderToAdd);
                }
                EditorGUILayout.EndHorizontal();
                
                
                EditorGUILayout.LabelField("All Commanders");
                if (faction.CommanderIds.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < faction.CommanderIds.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.PrefixLabel($"Commander {i + 1}:");
                        EditorGUILayout.LabelField(allCharacters.First(c => c.Id == faction.CommanderIds[i]).Name);

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }

                //All characters
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                EditorGUILayout.LabelField("All Characters", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();

                selectedCharacterIndex = EditorGUILayout.Popup("Characters", selectedCharacterIndex, GetCharacterNames());

                if (selectedCharacterIndex >= 0 && selectedCharacterIndex < allCharacters.Count)
                {
                    characterToAdd = allCharacters[selectedCharacterIndex];
                }

                if (GUILayout.Button("Add Character"))
                {
                    faction.AddCharacter(characterToAdd);
                }
                EditorGUILayout.EndHorizontal();
                
                
                EditorGUILayout.LabelField("All Character");
                if (faction.AllCharacterIds.Count > 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < faction.AllCharacterIds.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.PrefixLabel($"Character {i + 1}:");
                        EditorGUILayout.LabelField(allCharacters.First(c => c.Id == faction.AllCharacterIds[i]).Name);

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUI.indentLevel--;
                    EditorGUI.indentLevel--;
                }

                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                
                // Save changes to Faction
                if (GUILayout.Button("Save"))
                {
                    if (AssetDatabase.IsValidFolder("Assets/SOs/Factions/" + faction.Name))
                    {
                        EditorUtility.SetDirty(capital);
                        EditorUtility.SetDirty(faction);
                        AssetDatabase.SaveAssets();
                        return;
                    }

                    // Create a new Faction asset
                    AssetDatabase.CreateAsset(faction, "Assets/SOs/Factions/" + faction.Name + ".asset");
                    AssetDatabase.CreateAsset(capital, "Assets/SOs/Settlements/" + capital.Name + ".asset");

                    // Save changes to Faction asset
                    EditorUtility.SetDirty(faction);
                    EditorUtility.SetDirty(capital);
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Reset"))
                {
                    faction = null;
                    UpdateExistingAssets();
                    selectedCharacterIndex = -1;
                    selectedLeaderIndex = -1;
                    selectedCommanderIndex = -1;
                    selectedSpeciesIndex = -1;
                    selectedBannerIndex = -1;
                }
                
            }
        }

        private string[] GetCharacterNames()
        {
            string[] names = new string[allCharacters.Count];

            for (int i = 0; i < allCharacters.Count; i++)
            {
                names[i] = allCharacters[i].Name;
            }

            return names;
        }
        
        private string[] GetBannerNames()
        {
            string[] names = new string[allBanners.Count];

            for (int i = 0; i < allBanners.Count; i++)
            {
                names[i] = allBanners[i].name;
            }

            return names;
        }
        
        private string[] GetSpeciesNames()
        {
            string[] names = new string[allSpecies.Count];

            for (int i = 0; i < allSpecies.Count; i++)
            {
                names[i] = allSpecies[i].Name;
            }

            return names;
        }
    }*/
}