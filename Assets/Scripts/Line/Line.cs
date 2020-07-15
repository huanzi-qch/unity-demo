using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace DefaultNamespace.Common.Line
{
    /// <summary>
    /// 自定义流动线路
    /// </summary>
    public class Line : MonoBehaviour
    {
        [Header("连成线的每个点世界坐标")] public Vector3[] points;

        [Header("流动速度")] public float speed = 0.5f;

        [Header("线的宽度")] public float width = 0.07f;

        [Header("流动方向正反")] public bool direction = true;

        public Object startObject; //线开始端连接的对象

        public Object endObject; //线末端连接的对象

        private Material material;

        private float value = 0;

        void Awake()
        {
            InitLine();
        }

        void Update()
        {
            value += speed * 0.01F;
            material.mainTextureOffset = new Vector2(value,0);
        }

        public void InitLine()
        {
            if (direction)
            {
                speed = -speed;
            }
            
            //获取LineRenderer，设置参数
            LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
            lineRenderer.widthMultiplier = width;
            material = lineRenderer.material;
        }
    }
}
