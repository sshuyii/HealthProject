using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


    public static class UIExpansion
    {
        public static void Hide(CanvasGroup UIGroup) 
        {
            // Debug.Log("hide cg " + UIGroup.transform.name);
            UIGroup.alpha = 0f; //this makes everything transparent
            UIGroup.blocksRaycasts = false; //this prevents the UI element to receive input events
            UIGroup.interactable = false;
        }
    
        public static void Show(CanvasGroup UIGroup) 
        {
            // Debug.Log("show cg " + UIGroup.transform.name);
            UIGroup.alpha = 1f;
            UIGroup.blocksRaycasts = true;
            UIGroup.interactable = true;
    
        }

        public static void ScrollToTop(ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition= new Vector2(0, 1);
            Debug.Log("scroll" + scrollRect.normalizedPosition);
        }

        public static void LogDic(Dictionary<string, int> dic)
        {
            foreach(var pair in dic)
            {
                Debug.Log("Dictionary key = " + pair.Key + ", value = " + pair.Value.ToString());
            }
        }

    }

