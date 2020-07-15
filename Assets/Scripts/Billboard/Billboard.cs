using UnityEngine;

/*
 * 自定义公告牌，跟随镜头旋转永远平行面向屏幕，跟随镜头缩放缩放大小不变
 */ 
public class Billboard : MonoBehaviour {
    
    [Header("跟随镜头旋转")]
    public bool isRotation = false;
    
    [Header("跟随镜头缩放")]
    public bool isZoom = false;
  
    [Header("大小比例，为0时，以当前对象的Scale为准")]
    public float size = 0F;
    
    Camera camera;//主镜头

    private float _distance;//初始距离，需要在程序运行时立即获取

    private bool first = true;

    void Start () {
        camera = Camera.main; 
                
        //初始设置距离
        _distance = Vector3.Distance(camera.transform.position, transform.position);

        if (size == 0)
        {
            size = transform.localScale.x;
        }
    }
	
    void Update ()
    {
        //跟随镜头旋转，直接把主镜头的旋转值赋值给公告牌即可
        if (isRotation)
        {
            transform.rotation = camera.transform.rotation;
        }

        //跟随镜头缩放（缩放镜头设置的镜头的位置），根据公告牌到主镜头的距离来做等距离缩放即可
        if (isZoom)
        {
            float distance = Vector3.Distance(camera.transform.position, transform.position);//不断变化的距离
        
            var scale = distance / _distance * size;
            transform.localScale = new Vector3(scale,scale,scale);
        }
    }
 
}
