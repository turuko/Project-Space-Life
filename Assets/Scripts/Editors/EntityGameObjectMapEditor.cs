using System.Collections.Generic;
using Model.Databases;
using Model.Factions;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    /*[CustomEditor(typeof(EntityGameObjectMap))]
    public class EntityGameObjectMapEditor : UnityEditor.Editor
    {
        
        private EntityGameObjectMap entityGameObjectMap;
        
        private Dictionary<string, string> unitIdToName;
        private Dictionary<string, string> characterIdToName;

        private void OnEnable()
        {
            entityGameObjectMap = (EntityGameObjectMap)target;

            unitIdToName = new Dictionary<string, string>();
            var unitGUIDs = AssetDatabase.FindAssets("t:Unit");
            foreach (var unitGUID in unitGUIDs)
            {
                var unitPath = AssetDatabase.GUIDToAssetPath(unitGUID);
                var unit = AssetDatabase.LoadAssetAtPath<Unit>(unitPath);
                unitIdToName.Add(unit.Id, unit.Name);
            }
            
            characterIdToName = new Dictionary<string, string>();
            var characterGUIDs = AssetDatabase.FindAssets("t:Character");
            foreach (var characterGUID in characterGUIDs)
            {
                var characterPath = AssetDatabase.GUIDToAssetPath(characterGUID);
                var character = AssetDatabase.LoadAssetAtPath<Character>(characterPath);
                characterIdToName.Add(character.Id, character.Name);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var prevType = entityGameObjectMap.Type;
            entityGameObjectMap.Type = (EntityMapType)EditorGUILayout.EnumPopup("Entity type", entityGameObjectMap.Type);
            
            if(entityGameObjectMap.Type != prevType)
                EditorUtility.SetDirty(target);
            
            // Create a copy of the dictionary keys to avoid modifying the collection while iterating
            if (entityGameObjectMap.Map == null)
            {
                if(entityGameObjectMap.Type == EntityMapType.Unit)
                    entityGameObjectMap.PopulateUnitMap();
                else
                    entityGameObjectMap.PopulateCharacterMap();
            }
            List<string> entityKeys = new List<string>(entityGameObjectMap.Map.Keys);

            foreach (var entityKey in entityKeys)
            {
                EditorGUILayout.BeginVertical();
                string entityName = GetEntityName(entityKey);

                // Display the entity name
                LabelField("Entity Name:", entityName);

                EditorGUILayout.BeginHorizontal();
                string newKey = TextField("ID:", entityKey);
                GameObject value = entityGameObjectMap.Map[entityKey];
                GameObject newValue = (GameObject)EditorGUILayout.ObjectField("", value, typeof(GameObject), false);

                if (newKey != entityKey)
                {
                    // Remove the old key and add the new key if it changed
                    entityGameObjectMap.Map.Remove(entityKey);
                    entityGameObjectMap.Map.Add(newKey, newValue);
                }
                else if (newValue != value)
                {
                    // Update the value if it changed
                    entityGameObjectMap.Map[entityKey] = newValue;
                }

                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    // Remove the key-value pair
                    entityGameObjectMap.Map.Remove(entityKey);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Entry"))
            {
                // Add a new key-value pair with empty values
                entityGameObjectMap.Map.Add("", null);
                EditorUtility.SetDirty(target);
            }
            
            EditorUtility.SetDirty(target);
        }
        
        private static string TextField(string label, string text)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x;
            return EditorGUILayout.TextField(label, text);
        }
        
        private static void LabelField(string label, string text)
        {
            var labelDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = labelDimensions.x;
            EditorGUILayout.LabelField(label, text);
        }
        
        private string GetEntityName(string id)
        {
            if (unitIdToName.TryGetValue(id, out var unitName))
            {
                return unitName;
            }
            if (characterIdToName.TryGetValue(id, out var characterName))
            {
                return characterName;
            }

            return null;
        }
    }*/
}