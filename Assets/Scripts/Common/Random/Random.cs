using System;

namespace Script.Common
{
    /// <summary>
    /// 生成随机数
    /// </summary>
    public class Random
    {
        //生成n个指定范围的随机数，例如：GenerateUniqueRandom(1, 10, 5);// 结果 3, 7, 6, 1, 9
        public static int[] GenerateRandom(int minValue, int maxValue, int randNum)
        {
            System.Random ran = new System.Random((int)DateTime.Now.Ticks);
            int[] arr = new int[randNum];

            for (int i = 0; i < randNum; i++)
            {
                arr[i] = ran.Next(minValue, maxValue);
            }
            return arr;
        }
        
        // 生成n个绝对不重复的指定范围的随机数，例如：GenerateUniqueRandom(1, 10, 5);// 结果 3, 7, 6, 1, 9
        public static int[] GenerateUniqueRandom(int minValue, int maxValue, int n)
        {
            //如果生成随机数个数大于指定范围的数字总数，则最多只生成该范围内数字总数个随机数
            if (n > maxValue - minValue + 1)
                n = maxValue - minValue + 1;

            int maxIndex = maxValue - minValue + 2;// 索引数组上限
            int[] indexArr = new int[maxIndex];
            for (int i = 0; i < maxIndex; i++)
            {
                indexArr[i] = minValue - 1;
                minValue++;
            }

            System.Random ran = new System.Random();
            int[] randNum = new int[n];
            int index;
            for (int j = 0; j < n; j++)
            {
                index = ran.Next(1, maxIndex - 1);// 生成一个随机数作为索引

                //根据索引从索引数组中取一个数保存到随机数数组
                randNum[j] = indexArr[index];

                // 用索引数组中最后一个数取代已被选作随机数的数
                indexArr[index] = indexArr[maxIndex - 1];
                maxIndex--; //索引上限减 1
            }
            return randNum;
        }
        
        //生成指定范围的小数随机数，保留2位小数
        public static double NextDouble(double minValue, double maxValue)
        {
            return Convert.ToDouble(new System.Random().NextDouble() * (maxValue - minValue) + minValue.ToString("f2"));
        }
    }
}