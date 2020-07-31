using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 重写原生按钮脚本，扩展鼠标悬浮等事件
/// </summary>
public class HuanziButton : Button
{  
  //鼠标悬浮提示文字
  private string HoveText; 
  //鼠标悬浮提示文字的字体
  private Font HoveTextFont;

  //四种鼠标动作
  private enum Selection
  {
      Normal,//无
      Highlighted,//鼠标悬浮
      Pressed,//鼠标按下
      Disabled//
  }
  
  //当前鼠标动作
  private Selection selection;

  void Start()
  {
      //获取鼠标悬浮提示文字、字体
      HoveText = transform.Find("Text").GetComponent<Text>().text;
      HoveTextFont = transform.Find("Text").GetComponent<Text>().font;
  }

  protected override void DoStateTransition(SelectionState state, bool instant)
  {
      base.DoStateTransition(state, instant);
      switch (state)
      {
          case SelectionState.Normal:
              selection = Selection.Normal;
              break;
          case SelectionState.Highlighted:
              selection = Selection.Highlighted;
              break;
          case SelectionState.Pressed:
              selection = Selection.Pressed;
              break;
          case SelectionState.Disabled:
              selection = Selection.Disabled;
              break;
          default:
              break;
      }
  }

  private void OnGUI()
  {
      
      switch (selection)
      {
          //鼠标悬浮
          case Selection.Highlighted:
              if (IsOverGUI(Input.mousePosition,transform.gameObject))
              {
                  GUI.skin.box.font = HoveTextFont;//设置字体
                  GUI.contentColor = Color.white;//设置字体颜色
                  GUI.skin.box.fontSize = 14;//设置字体大小
             
                  GUI.Box(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 100, 30), HoveText);
              }
              break;
           default:
              break;
      }
  }
  
  /// <summary>
  /// 判断鼠标是否在某一ui上
  /// </summary>
  private bool IsOverGUI(Vector2 pos,GameObject target = null)
  {
      EventSystem es = EventSystem.current;
      PointerEventData ped = new PointerEventData(es);
      ped.position = pos;
      List<RaycastResult> rr = new List<RaycastResult>();
      rr.Clear();
      es.RaycastAll(ped, rr);
      for (int i = 0; i < rr.Count; i++)
      {
          if (target != null && rr[i].gameObject == target)
          {
              return true;//表示该位置pos在ui物体target上
          }
      }
      return false;
  }
}
