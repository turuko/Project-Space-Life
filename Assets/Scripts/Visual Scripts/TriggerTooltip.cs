using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace Visual_Scripts
{
    public class TriggerTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string TooltipText;

        public GameObject ToolTip;

        private Vector3 offset;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer enter " + eventData.pointerEnter.name);
            ToolTip.SetActive(true);
            offset = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y, 0);
            ToolTip.transform.position = eventData.pointerEnter.transform.position + offset;
            if (Utilities.IsUIObjectOutsideScreen(ToolTip.GetComponent<RectTransform>()))
            {
                Debug.Log("Tooltip outside screen");
                var offset = Utilities.GetOffsetToBringUIObjectInsideScreen(ToolTip.GetComponent<RectTransform>());
                ToolTip.transform.position += (Vector3)offset;
            }
            
            ToolTip.GetComponentInChildren<ResizeText>().Open(TooltipText);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Pointer exit " + eventData.pointerEnter.name);
            ToolTip.SetActive(false);
        }
    }
}
