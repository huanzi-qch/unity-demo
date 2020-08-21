using DG.Tweening;
using UnityEngine;

public class Curtain : MonoBehaviour
{
    [Header("左边")]
    public RectTransform left;  
    
    [Header("右边")]
    public RectTransform right;

    [Header("开、闭幕耗时")]
    public float time = 0.8F;

    //闭幕
    public void CloseCurtain(TweenCallback callBack)
    {
        //重置左边、右边的宽度、位置
        float width = Screen.width / 2;
        left.sizeDelta = new Vector2(width, 0);
        left.anchoredPosition3D = new Vector3(-width,0,0);
        right.sizeDelta = new Vector2(width, 0);
        
        //两边往中间靠拢
        left.DOAnchorPosX(0, time);
        right.DOAnchorPosX(-width, time).OnComplete(callBack);
    }
    //开幕
    public void OpenCurtain(TweenCallback callBack)
    {
        //重置左边、右边的宽度、位置
        float width = Screen.width;
        right.sizeDelta = new Vector2(width, 0);
        right.anchoredPosition3D = new Vector3(-width,0,0);
        left.sizeDelta = new Vector2(width, 0);
        
        //两边往中间靠拢
        left.DOAnchorPosX(-width, time);
        right.DOAnchorPosX(0, time).OnComplete(callBack);
    }
    
}
