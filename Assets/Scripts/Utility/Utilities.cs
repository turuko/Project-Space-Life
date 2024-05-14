using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Utility
{
    public static class Utilities
    {
        public static int ConvertToInt(this string input)
        {
            int result = 0;
    
            foreach (char c in input)
            {
                if (Char.IsDigit(c))
                {
                    result = result * 10 + (c - '0');
                }
                else if (Char.IsLetter(c))
                {
                    char lowerCase = Char.ToLower(c);
                    result = result * 26 + (lowerCase - 'a');
                }
                else
                {
                    // Handle invalid characters or other cases if needed
                    throw new ArgumentException("Invalid character in input string.");
                }
            }
    
            return result;
        }

        public static string ConvertToRomanNumeral(int number)
        {
            if (number == 0)
                return "0";
            
            if (number < 1 || number > 3999)
                throw new ArgumentOutOfRangeException("number", "Value must be between 1 and 3999.");

            // Define Roman numeral symbols and their values
            string[] romanSymbols = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
            int[] romanValues = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

            StringBuilder romanNumeral = new StringBuilder();

            for (int i = 0; i < romanValues.Length; i++)
            {
                while (number >= romanValues[i])
                {
                    number -= romanValues[i];
                    romanNumeral.Append(romanSymbols[i]);
                }
            }

            return romanNumeral.ToString();
        }
        
        public static string TrimNonLetterChars(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int i = str.Length - 1;
            while (i >= 0 && !char.IsLetter(str[i]))
            {
                i--;
            }

            return str.Substring(0, i + 1);
        }
        
        public static bool IsUIObjectOutsideScreen(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            
            var screenRect = new Rect(0, 0, Screen.width, Screen.height);
            
            var allCornersInScreen = corners.All(screenRect.Contains);
            
            Debug.Log("On screen: " + allCornersInScreen);

            return !allCornersInScreen;
        }

        public static Vector2 GetOffsetToBringUIObjectInsideScreen(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            var screenRect = new Rect(0, 0, Screen.width, Screen.height);
            
            Vector2 offset = Vector2.zero;
            for (int i = 0; i < corners.Length; i++)
            {
                Vector3 screenPoint = corners[i];
                if (screenPoint.x < 0)
                {
                    offset.x += screenRect.width - screenPoint.x;
                }
                else if (screenPoint.x > screenRect.width)
                {
                    offset.x -= screenPoint.x - screenRect.width;
                }

                if (screenPoint.y < 0)
                {
                    offset.y += screenRect.height - screenPoint.y;
                }
                else if (screenPoint.y > screenRect.height)
                {
                    offset.y -= screenPoint.y - screenRect.height;
                }
            }

            Debug.LogFormat("Offset: " + offset);
            
            return offset;
        }
    }
}