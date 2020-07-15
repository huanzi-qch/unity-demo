using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Common.Dialog
{
    /// <summary>
    /// 自定义弹窗
    /// </summary>
    public class Dialog : MonoBehaviour
    {
        [Header("滚动框预制体")]
        public GameObject scrollBoxPrefab;
        private GameObject scrollBoxGameObject;
        
        [Header("确认框预制体")]
        public GameObject affirmPrefab;
        private GameObject affirmGameObject;
        
        [Header("警告框预制体")]
        public GameObject alertPrefab;
        private GameObject alertGameObject;
        
        [Header("提示框预制体")]
        public GameObject msgPrefab;
        private GameObject msgGameObject;
        
        /// <summary>
        /// 滚动框
        /// </summary>
        public Transform scrollBox(String title,Action closeCallBack = null)
        {
            scrollBoxGameObject = GameObject.Instantiate(scrollBoxPrefab, transform, true);
            RectTransform rectTransform = scrollBoxGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.offsetMax = new Vector2(0, 0);
            rectTransform.offsetMin  = new Vector2(0, 0);
            
            Transform scrollBox = scrollBoxGameObject.transform.Find("ScrollBox");
            scrollBox.Find("Title").GetComponent<Text>().text = title;
            scrollBox.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Destroy(scrollBoxGameObject);
                if (closeCallBack != null)
                {
                    closeCallBack.Invoke();
                }

            });
            return scrollBox.Find("Scroll View").Find("Viewport").Find("Content");
        }
        
        /// <summary>
        /// 确认框
        /// </summary>
        public Transform affirm(String title,String text,Action affirmCallBack,Action closeCallBack = null)
        {
            affirmGameObject = GameObject.Instantiate(affirmPrefab, transform, true);
            RectTransform rectTransform = affirmGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.offsetMax = new Vector2(0, 0);
            rectTransform.offsetMin  = new Vector2(0, 0);
            
            Transform affirm = affirmGameObject.transform.Find("Affirm");
            affirm.Find("Title").GetComponent<Text>().text = title;
            affirm.Find("Text").GetComponent<Text>().text = text;
            Button closeButton = affirm.Find("Close").GetComponent<Button>();
            closeButton.onClick.AddListener(() =>
            {
                GameObject.Destroy(affirmGameObject);
                if (closeCallBack != null)
                {
                    closeCallBack.Invoke();
                }
            });
            affirm.Find("Cancel").GetComponent<Button>().onClick.AddListener(() =>
            {
                closeButton.onClick.Invoke();
            });
            affirm.Find("Affirm").GetComponent<Button>().onClick.AddListener(() =>
            {
                closeButton.onClick.Invoke();
                affirmCallBack.Invoke();
            });
            return affirm.Find("Content");
        }
        
        /// <summary>
        /// 警告框
        /// </summary>
        public Transform alert(String title,String text,Action closeCallBack = null)
        {
            alertGameObject = GameObject.Instantiate(alertPrefab, transform, true);
            RectTransform rectTransform = alertGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.offsetMax = new Vector2(0, 0);
            rectTransform.offsetMin  = new Vector2(0, 0);
            
            Transform alert = alertGameObject.transform.Find("Alert");
            alert.Find("Title").GetComponent<Text>().text = title;
            alert.Find("Text").GetComponent<Text>().text = text;
            alert.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Destroy(alertGameObject);
                if (closeCallBack != null)
                {
                    closeCallBack.Invoke();
                }
            });
            return alert.Find("Content");
        }
        
        /// <summary>
        /// 提示框，2秒后自动删除
        /// </summary>
        public void msg(String text)
        {
            if (msgGameObject != null)
            {
                GameObject.Destroy(msgGameObject);
            }
            msgGameObject = GameObject.Instantiate(msgPrefab, transform, true);
            msgGameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            msgGameObject.transform.Find("Text").GetComponent<Text>().text = text;
            
            Invoke("Timer", 2f);
        }

        //提示框定时器任务
        private void Timer()
        {
            GameObject.Destroy(msgGameObject);
        }
    }
}