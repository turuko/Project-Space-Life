using System;
using TMPro;
using UnityEngine;

namespace Visual_Scripts
{
    public class ResizeText : MonoBehaviour
    {
        // Start is called before the first frame update
        public void Open(string text)
        {
            var tmpComponent = GetComponent<TextMeshProUGUI>();
            var size = tmpComponent.GetPreferredValues(text);
            tmpComponent.text = text;

            GetComponent<RectTransform>().sizeDelta = size;
            var parentTransform = transform.parent.GetComponent<RectTransform>();
            parentTransform.sizeDelta = size + new Vector2(15,15);
            Debug.Log("Parent transform: " + parentTransform.name);
        }
    }
}
