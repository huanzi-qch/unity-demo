using System;
using System.Collections.Generic;
using System.Reflection;
using Common;
using DefaultNamespace.Common.Dialog;
using DefaultNamespace.Common.Line;
using DG.Tweening;
using Script.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace DefaultNamespace
{
    /// <summary>
    /// 主脚本
    /// </summary>
    public class Main : MonoBehaviour
    {
        [Header("视频播放器预制体")]
        public GameObject videoPrefab;
        private GameObject videoGameObject;
        
        [Header("线路预制体")]
        public GameObject linePrefab;
        
        //当前UI根节点
        private Transform UIRoot;

        //立方块
        private Transform findCube;

        void Start()
        {
            UIRoot = GameObject.Find("UIRoot").transform;
            
            findCube = transform.Find("Cube");
            
            Config.InitConfig((configVo) =>
            {
                UiAnimation(configVo);
            
                EvenInit();

                InitLine();
            });
        }

        void Update()
        {
            
        }

        /// <summary>
        /// UI进场动画
        /// </summary>
        private void UiAnimation(ConfigVo configVo)
        {
            //标题
            Transform title = UIRoot.Find("Title");
            title.Find("Text").GetComponent<Text>().text = configVo.name;
            RectTransform titleRectTransform = title.GetComponent<RectTransform>();
            titleRectTransform.DOAnchorPosY(-25F, 0.5F).SetEase(Ease.Linear);
            
            //信息面板 
            Transform leftInfo = UIRoot.Find("LeftInfo");
            Transform panel = leftInfo.Find("Panel");
            foreach (PropertyInfo p in configVo.GetType().GetProperties())
            {
                Transform find = panel.Find(p.Name);
                if (find == null)
                {
                    continue;
                }
                find.Find("Text").GetComponent<Text>().text = p.GetValue(configVo, null) + "";
            }
            leftInfo.GetComponent<RectTransform>().DOAnchorPosX(180F, 0.5F).SetEase(Ease.Linear);
            UIRoot.Find("RightInfo").GetComponent<RectTransform>().DOAnchorPosX(-180F, 0.5F).SetEase(Ease.Linear);
            
            //按钮组
            RectTransform buttonGroup = UIRoot.Find("ButtonGroup").GetComponent<RectTransform>();
            buttonGroup.DOAnchorPosY(25F, 0.5F).SetEase(Ease.Linear);
        }

        /// <summary>
        /// 事件初始化
        /// </summary>
        private void EvenInit()
        {
            Click findCubeClick = findCube.gameObject.AddComponent<Click>();
            //立方块点击事件
            findCubeClick.OnClickListener.AddListener(() => { UIRoot.GetComponent<Dialog>().msg("你单击了立方块..."); });

            //立方块双击事件
            findCubeClick.OnDblclickListener.AddListener(() => { UIRoot.GetComponent<Dialog>().msg("你双击了立方块！！！"); });

            Transform buttonGroup = UIRoot.Find("ButtonGroup");
            //按钮0，展开、收起详情
            Text info = buttonGroup.Find("Info").Find("Text").GetComponent<Text>();
            RectTransform leftInfoRectTransform = UIRoot.Find("LeftInfo").GetComponent<RectTransform>();
            RectTransform rightInfoRectTransform = UIRoot.Find("RightInfo").GetComponent<RectTransform>();
            buttonGroup.Find("Info").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (leftInfoRectTransform.anchoredPosition3D.x == 180)
                {
                    info.text = "展开面板";
                    leftInfoRectTransform.DOAnchorPosX(-180F, 0.5F).SetEase(Ease.Linear);
                    rightInfoRectTransform.DOAnchorPosX(180F, 0.5F).SetEase(Ease.Linear);
                }
                else
                {
                    info.text = "收起面板";
                    leftInfoRectTransform.DOAnchorPosX(180F, 0.5F).SetEase(Ease.Linear);
                    rightInfoRectTransform.DOAnchorPosX(-180F, 0.5F).SetEase(Ease.Linear);
                }

            });
            //按钮1，开始、暂停立方体动画
            Text buttonText = buttonGroup.Find("Button").Find("Text").GetComponent<Text>();
            buttonGroup.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                bool isAnimation = findCube.GetComponent<CubeAnimation>().isAnimation;
                if (isAnimation)
                {
                    buttonText.text = "开始";
                }
                else
                {
                    buttonText.text = "暂停";
                }

                findCube.GetComponent<CubeAnimation>().isAnimation = !isAnimation;
            });

            //按钮2，警告框
            buttonGroup.Find("Button1").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIRoot.GetComponent<Dialog>().alert("警告框",
                    "alert弹窗，可以用来做必看的提示信息框，必须要用户手动关闭");
            });

            //按钮3，提示框
            buttonGroup.Find("Button2").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIRoot.GetComponent<Dialog>().msg("简单提示框，2秒后自动消失");
            });

            //按钮4，确认框
            buttonGroup.Find("Button3").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIRoot.GetComponent<Dialog>().affirm("确认框",
                    "确认框，与alert框不同，它带有按钮，可自定义按钮动作，目前仅扩展了确认按钮动作",
                    () => { UIRoot.GetComponent<Dialog>().msg("你点击了确认按钮"); });
            });

            //按钮5，滚动框
            buttonGroup.Find("Button4").GetComponent<Button>().onClick.AddListener(() =>
            {
                Transform content = UIRoot.GetComponent<Dialog>().scrollBox("滚动框");
                Transform item = content.Find("Text");
                item.GetComponent<Text>().text = "滚动框，可动态设置滚动区域高度，这里设置350";
                
                //克隆对象
                Transform cloneItem = GameObject.Instantiate(item, content);
                cloneItem.GetComponent<Text>().text = "已经设置自动布局，分好一行一行了";
                Transform cloneItem1 = GameObject.Instantiate(item, content);
                cloneItem1.GetComponent<Text>().text = "但是每一行的宽度要控制好，不能超出父类";

                for (int i = 0; i < 10; i++)
                {
                    Transform cloneItem2 = GameObject.Instantiate(item, content);
                    cloneItem2.GetComponent<Text>().text = "无用测试数据";
                }
                //设置高度
                content.GetComponent<RectTransform>().sizeDelta = new Vector2(0,350); 

            });

            //按钮5，定点巡航
            buttonGroup.Find("Button5").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIRoot.GetComponent<Dialog>().msg("开始定点巡航...");

                //设置镜头中心点，调用镜头巡航
                CameraMove cameraMove = Camera.main.GetComponent<CameraMove>();
                Vector3 target = new Vector3(findCube.position.x, 0, findCube.position.z);

                //旋转轴心点
                cameraMove.RotaAxis = target;

                //把摄像朝向目标点
                Camera.main.transform.DOLookAt(target, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    cameraMove.CameraCruiseByTarget(target, () =>
                    {
                        UIRoot.GetComponent<Dialog>().msg("定点巡航结束...");
                    });
                });
            });
            
            //按钮6，定线巡航
            buttonGroup.Find("Button6").GetComponent<Button>().onClick.AddListener(() =>
            {
                UIRoot.GetComponent<Dialog>().msg("开始定线巡航...");

                //设置镜头中心点，调用镜头巡航
                CameraMove cameraMove = Camera.main.GetComponent<CameraMove>();
                List<Vector3> poVector3s = new List<Vector3>();
                poVector3s.Add(new Vector3(1.3F, Camera.main.transform.position.y, -8F));
                poVector3s.Add(new Vector3(1.3F, Camera.main.transform.position.y, 8F));
                poVector3s.Add(new Vector3(-1.3F, Camera.main.transform.position.y, 8F));
                poVector3s.Add(new Vector3(-1.3F, Camera.main.transform.position.y, -8F));

                cameraMove.CameraCruiseByPoVector3s(poVector3s, () =>
                {
                    UIRoot.GetComponent<Dialog>().msg("定线巡航结束...");
                    Vector3 target = new Vector3(findCube.position.x, 0, findCube.position.z);

                    //旋转轴心点
                    cameraMove.RotaAxis = target;

                    //把摄像朝向目标点
                    Camera.main.transform.DOLookAt(target, 0.5f).SetEase(Ease.Linear);
                });
            });
                        
            //按钮7，视频监控，注：unity自带的视频播放组件存在一些问题，有时候会导致程序直接崩掉
            buttonGroup.Find("Button7").GetComponent<Button>().onClick.AddListener(() =>
            {
                Transform content = UIRoot.GetComponent<Dialog>().alert("模拟视频监控","", () =>
                {
                    GameObject.Destroy(videoGameObject);
                });
                
                //加载播放器对象
                videoGameObject = GameObject.Instantiate(videoPrefab, content, true);
                VideoPlayer videoPlayer = videoGameObject.transform.Find("Video Player").gameObject.GetComponent<VideoPlayer>();

                //设置宽高
                RectTransform rectTransform = videoGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition3D = Vector3.zero;
                videoGameObject.transform.Find("RawImage").GetComponent<RectTransform>().sizeDelta =
                    content.GetComponent<RectTransform>().sizeDelta;
                
                try
                {
                    //设置视频地址
                    string url = Application.streamingAssetsPath +"/test.mp4";
                    Debug.Log("视频播放地址："+url);
                    videoPlayer.url = url;
                    
                    videoPlayer.prepareCompleted += (VideoPlayer source) =>
                    {
                        //预加载完之后调用我
                        videoPlayer.Play();
                    };
                    videoPlayer.errorReceived += (VideoPlayer vp, string desc) =>
                    {
                        //当视频播放出错时的回调
                        Debug.Log("播放出错" + desc);
                    };
                    videoPlayer.Prepare();
                }catch(Exception e) {
                    //当视频播放出错时的回调
                    Debug.Log("播放异常" + e);   
                }
            });
        }

        /// <summary>
        /// 初始化警告公告牌下方的流动线路、以及模拟机柜、机柜节点线路
        /// </summary>
        private void InitLine()
        {
            //警告公告牌下方的流动线路
            Transform triangleLabel = transform.Find("TriangleLabel");
            Line line = triangleLabel.Find("Line").GetComponent<Line>();
            
            //确定坐标，并重新初始化Line
            Vector3 triangleLabelPosition = triangleLabel.position;
            triangleLabelPosition.y = 0.1F;
            List<Vector3> linePoints = new List<Vector3>();
            linePoints.Add(new Vector3(triangleLabelPosition.x + 1,triangleLabelPosition.y,triangleLabelPosition.z + 1));
            linePoints.Add(new Vector3(triangleLabelPosition.x + 1,triangleLabelPosition.y,triangleLabelPosition.z - 1));
            linePoints.Add(new Vector3(triangleLabelPosition.x - 1,triangleLabelPosition.y,triangleLabelPosition.z - 1));
            linePoints.Add(new Vector3(triangleLabelPosition.x - 1,triangleLabelPosition.y,triangleLabelPosition.z + 1));
            linePoints.Add(new Vector3(triangleLabelPosition.x + 1,triangleLabelPosition.y,triangleLabelPosition.z + 1));

            line.points = linePoints.ToArray();
            line.InitLine();

            
            //模拟机柜、机柜节点线路
            Vector3 rackPosition = transform.Find("Rack").position;
            
            //第一条
            Vector3[] rackNode1Pos = BezierUtils.GetBeizerList(transform.Find("RackNode1").position, rackPosition,20,2);
            GameObject lineGameObject1 = GameObject.Instantiate(linePrefab, transform, true);
            Line line1 = lineGameObject1.GetComponent<Line>();
            line1.points = rackNode1Pos;
            line1.InitLine();
                        
            //第二条
            Vector3[] rackNode2Pos = BezierUtils.GetBeizerList(transform.Find("RackNode2").position, rackPosition, 20,2);
            GameObject lineGameObject2 = GameObject.Instantiate(linePrefab, transform, true);
            Line line2 = lineGameObject2.GetComponent<Line>();
            line2.points = rackNode2Pos;
            line2.InitLine();
        }
        
        
    }
        
}