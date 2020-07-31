using UnityEngine;

/// <summary>
/// 3D物体转2D屏幕坐标
/// </summary>
public class Position3DTo2D : MonoBehaviour {

    [Header("3D对象")]
    public Transform object3D; 
    
    [Header("2DUI")]
    public Transform ui2D;   
    
    [Header("UI偏移量")]
    public Vector2 uiOffset;
    
    private Vector3 originOff;  // 当前UI系统(0,0)点 相对于屏幕左下角(0, 0)点的 原点偏移量

    private void Start () {
        originOff = new Vector3(-Screen.width / 2, -Screen.height / 2);
        
        Reposition();
    }

    private void Update () {
        // 需要性能优化 仅在物体移动或相机移动后调用即可
        Reposition();
    }

    // 根据目标物体 重定位UI
    private void Reposition() {
        Vector3 position = Camera.main.WorldToScreenPoint(object3D.position) + originOff;
        position.z = 0;
        position.x += uiOffset.x;
        position.y += uiOffset.y;
        ui2D.localPosition = position;
    }
}
