using System;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.Common.Dialog
{
    /// <summary>
    /// 自定义弹窗
    /// </summary>
    public class Dialog : MonoBehaviour
    {
        //[Header("滚动框预制体")]
        private GameObject scrollBoxPrefab;
        private GameObject scrollBoxGameObject;
        
        //[Header("确认框预制体")]
        private GameObject affirmPrefab;
        private GameObject affirmGameObject;
        
        //[Header("警告框预制体")]
        private GameObject alertPrefab;
        private GameObject alertGameObject;
        
        //[Header("提示框预制体")]
        private GameObject msgPrefab;
        private GameObject msgGameObject;
        
        public void Start()
        {
            loadPrefabs();
        }

        /// <summary>
        /// 动态加载预制体
        /// </summary>
        private void loadPrefabs()
        {
            scrollBoxPrefab = (GameObject)Resources.Load("Dialog/ScrollBox");
            affirmPrefab = (GameObject)Resources.Load("Dialog/Affirm");
            alertPrefab = (GameObject)Resources.Load("Dialog/Alert");
            msgPrefab = (GameObject)Resources.Load("Dialog/Msg");
        }
        
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
            
            CameraMove cameraMove = Camera.main.GetComponent<CameraMove>();
            cameraMove.enabled = false;//禁用摄像头操作脚本
            
            scrollBox.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Destroy(scrollBoxGameObject);
                cameraMove.enabled = true;//开放摄像头操作脚本
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
                        
            CameraMove cameraMove = Camera.main.GetComponent<CameraMove>();
            cameraMove.enabled = false;//禁用摄像头操作脚本

            Button closeButton = affirm.Find("Close").GetComponent<Button>();
            closeButton.onClick.AddListener(() =>
            {
                GameObject.Destroy(affirmGameObject);
                cameraMove.enabled = true;//开放摄像头操作脚本
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
                        
            CameraMove cameraMove = Camera.main.GetComponent<CameraMove>();
            cameraMove.enabled = false;//禁用摄像头操作脚本

            alert.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Destroy(alertGameObject);
                cameraMove.enabled = true;//开放摄像头操作脚本
                if (closeCallBack != null)
                {
                    closeCallBack.Invoke();
                }
            });
            return alert.Find("Content");
        }
        
        /// <summary>
        /// 提示框，time 秒后自动删除
        /// </summary>
        public void msg(String text ,float time = 2f)
        {
            if (msgGameObject != null)
            {
                GameObject.Destroy(msgGameObject);
            }
            msgGameObject = GameObject.Instantiate(msgPrefab, transform, true);
            msgGameObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            msgGameObject.transform.Find("Text").GetComponent<Text>().text = text;
            
            Invoke("Timer", time);
        }

        //提示框定时器任务
        private void Timer()
        {
            GameObject.Destroy(msgGameObject);
        }
    }
}