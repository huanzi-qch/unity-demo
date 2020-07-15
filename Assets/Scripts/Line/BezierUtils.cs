using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Common.Line
{
    /// <summary>
    /// 贝塞尔曲线工具类
    /// 参考博客：https://www.cnblogs.com/msxh/p/6270468.html
    /// </summary>
    public class BezierUtils : MonoBehaviour
    {
        /// <summary>
        /// 获取存储贝塞尔曲线点的数组
        /// </summary>
        /// <param name="startPoint"></param>起始点
        /// <param name="endPoint"></param>目标点
        /// <param name="segmentNum"></param>采样点的数量，采样点越多弧度越圆滑
        /// <param name="offset">偏移量，偏移量越大，弧度越大</param>
        /// <returns></returns>存储贝塞尔曲线点的数组
        public static Vector3[] GetBeizerList(Vector3 startPoint, Vector3 endPoint, int segmentNum ,int offset)
        {
            Vector3 controlPoint = CalcControlPos(startPoint,endPoint,offset);
            Vector3[] path = new Vector3[segmentNum + 1];
            path[0] = startPoint;
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float) segmentNum;
                Vector3 pixel = CalculateCubicBezierPoint(t, startPoint,
                    controlPoint, endPoint);
                path[i] = pixel;
            }

            return path;
        }

        /// <summary>
        /// 根据T值，计算贝塞尔曲线上面相对应的点
        /// </summary>
        /// <param name="t"></param>T值
        /// <param name="p0"></param>起始点
        /// <param name="p1"></param>控制点
        /// <param name="p2"></param>目标点
        /// <returns></returns>根据T值计算出来的贝赛尔曲线点
        private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }


        /// <summary>
        /// 获取控制点.
        /// </summary>
        /// <param name="startPos">起点.</param>
        /// <param name="endPos">终点.</param>
        /// <param name="offset">偏移量.</param>
        private static Vector3 CalcControlPos(Vector3 startPos, Vector3 endPos, float offset)
        {
            //方向(由起始点指向终点)
            Vector3 dir = endPos - startPos;
            //取另外一个方向. 这里取向上.
            Vector3 otherDir = Vector3.up;

            //求平面法线.  注意otherDir与dir不能调换位置,平面的法线是有方向的,(调换位置会导致法线方向相反)
            //ps: 左手坐标系使用左手定则 右手坐标系使用右手定则 (具体什么是左右手坐标系这里不细说请Google)
            //unity中世界坐标使用的是左手坐标系,所以法线的方向应该用左手定则判断.
            Vector3 planeNormal = Vector3.Cross(otherDir, dir);

            //再求startPos与endPos的垂线. 其实就是再求一次叉乘.
            Vector3 vertical = Vector3.Cross(dir, planeNormal).normalized;
            //中点.
            Vector3 centerPos = (startPos + endPos) / 2f;
            //控制点.
            Vector3 controlPos = centerPos + vertical * offset;

            return controlPos;
        }
    }
}
