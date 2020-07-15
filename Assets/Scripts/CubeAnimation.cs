using DG.Tweening;
using UnityEngine;

/// <summary>
/// 方块动画脚本
/// </summary>
[System.Serializable]
public class CubeAnimation : MonoBehaviour
{
    [Header("是否进行动画")]
    public bool isAnimation = true;

    private float value = 0.05F;

    void Start()
    {
        
    }

    void Update()
    {
        //是否进行动画
        if (!isAnimation)
        {
            return;
        }

        //角度
        transform.Rotate(Vector3.up);

        //位置
        if (transform.position.y > 3)
        {
            value = -value;
        }
        
        if (transform.position.y < 0)
        {
            value = -value;
        }

        transform.position = new Vector3(0,transform.position.y + value,0);
    }
}
