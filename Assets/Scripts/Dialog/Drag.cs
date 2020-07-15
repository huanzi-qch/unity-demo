namespace DefaultNamespace.Common.Dialog
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    
    /// <summary>
    /// 按住标题拖动弹窗 
    /// </summary>
    public class Drag: MonoBehaviour, IDragHandler, IPointerDownHandler {

        private Vector2 offsetPos;  //临时记录点击点与UI的相对位置

        public void OnDrag(PointerEventData eventData)
        {
            //计算新坐标
            Vector2 eventDataPosition = eventData.position - offsetPos;
            Vector2 vector2 = new Vector2(eventDataPosition.x - transform.position.x, eventDataPosition.y - transform.position.y);

            //父类弹窗移动
            transform.parent.position = new Vector2(transform.parent.position.x + vector2.x,transform.parent.position.y + vector2.y);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            offsetPos = eventData.position - (Vector2)transform.position;
        }
    }
}