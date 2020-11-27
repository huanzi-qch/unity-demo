namespace DefaultNamespace
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;
    
    public class IsPointerOverUI
    {
        //射线上所有UI对象
        public List<RaycastResult> results;

        /// <summary>
        /// 判断鼠标是否放在UI上，作用等同于：EventSystem.current.IsPointerOverGameObject()
        /// </summary>
        /// <returns></returns>
        public bool IsPointerOverUIObject() {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            
            return results.Count > 0;
        }
    }
    
}