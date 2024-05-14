using System;
using System.Collections.Generic;
using System.Linq;
using Model.Factions;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Model.Databases
{
    public static class CharacterDatabase
    {
        private static List<Character> allCharacters;
        private static Dictionary<string, Character> characterMap;

        private static EntityGameObjectMap characterGameObjectMap;

        public static void Init()
        {
            allCharacters = new List<Character>();
            var guids = AssetDatabase.FindAssets("t:Character");
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var characterAsset = AssetDatabase.LoadAssetAtPath<Character>(path);

                var character = characterAsset.Clone();
                allCharacters.Add(character);
            }

            characterMap = allCharacters.ToDictionary(c => c.Id);
            
            characterGameObjectMap = AssetDatabase.FindAssets("t:EntityGameObjectMap").Select(x =>
                    AssetDatabase.LoadAssetAtPath<EntityGameObjectMap>(AssetDatabase.GUIDToAssetPath(x)))
                .First(x => x.Type == EntityMapType.Character);
        }

        public static bool AddCharacter(Character character)
        {
            if (allCharacters.Contains(character))
                return false;
            
            allCharacters.Add(character);
            characterMap.Add(character.Id, character);
            return true;
        }
        
        public static void UpdateCharacter(Character character)
        {
            if (!characterMap.TryGetValue(character.Id, out var existingCharacter))
            {
                return;
            }
            
            var index = allCharacters.IndexOf(existingCharacter);
            if (index == -1) return;
            allCharacters[index] = character;
            characterMap[existingCharacter.Id] = character;
        }

        public static Character GetCharacter(string id)
        {
            return characterMap[id];
        }

        public static List<Character> GetCharacters(params string[] ids)
        {
            List<Character> characters = new List<Character>();

            foreach (string id in ids)
            {
                if (characterMap.TryGetValue(id, out var character))
                {
                    characters.Add(character);
                }
            }

            return characters;
        }

        public static List<Character> Query(Func<Character, bool> predicate)
        {
            return allCharacters.Where(predicate).ToList();
        }
        
        public static GameObject GetCharacterGameObject(Character character)
        {
            return characterGameObjectMap[character];
        }
    }
}