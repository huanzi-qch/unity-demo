using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Common {

	/// <summary>
	/// 实现鼠标操作场景旋转和移动效果
	/// </summary>
	public class CameraMove : MonoBehaviour {
		[Header("旋转速度")]
		public uint rotateSpeed = 10;
		[Header("缩放速度")]
		public uint zoomSpeed = 10;
		[Header("最大俯视角度")]
		public int yMinLimit = 0; 
		[Header("最大仰视角度")]
		public uint yMaxLimit = 90;
		[Header("最小缩放范围值")]
		public uint zoomMin = 50; 
		[Header("最大缩放范围值")]
		public uint zoomMax = 100; 
		[Header("用于设置zoomMin和zoomMax的参考值")]
		public float distance; //
		[Header("是否可以移动")]
		public bool canMove = true;
		[Header("是否可以旋转")]
		public bool canRotation = true;
		[Header("是否可以缩放")]
		public bool canZoom = true;

		
		private int currentZoomLimitValue;//当前缩放范围的值，用于限制缩放距离
		private float currentAngleForX;//当前X轴旋转的角度
		private Vector3 rotaAxis;//旋转的轴心
		private float time1;//用于记录点击时间，实现双击效果 
		private float time2;//用于记录点击时间，实现双击效果 
		private Vector3 moveTargetPoint;//双击时摄像头移动的目标坐标
		private float cameraCruiseByTargetAngle = 0;//定点环绕360度巡航，镜头当前巡航角度
		private UnityEvent cameraCruiseByTargetCallBackListener; //定点环绕360度巡航结束时回调
		private UnityEvent cameraCruiseByPoVector3sCallBackListener; //定线巡航结束时回调
		private int cameraCruiseByPoVector3sCount = 0;//定线巡航定时任务当前调用次数
		private List<Vector3> cameraCruiseByPoVector3sPoVector3s;//定线巡航路线集合
		
		void Start()
		{
			InitCamera();
		}
		
		void LateUpdate() {
			Move();

			Rotate();

			Zoom();
		}

		/// <summary>
		/// 初始化镜头
		/// </summary>
		private void InitCamera()
		{
			currentAngleForX = transform.eulerAngles.x;
			
			//初始化缩放最小终点
			Ray ray = new Ray(transform.position, transform.forward * 50); 
			RaycastHit hit;
			bool isHit = Physics.Raycast(ray, out hit);//射线检测碰撞体
			if (isHit)
			{
				rotaAxis = new Vector3(hit.point.x, 0, hit.point.z);
				distance = Vector3.Distance(transform.position, rotaAxis);
			} 
		}

		/// <summary>
		///  获取两点之间一定距离的点
		/// </summary>
		/// <param name="start"> 开始坐标点 </param>
		/// <param name="end"> 终点坐标点</param>
		/// <param name="distance"> 要截取的长度</param>
		/// <returns> 两点之间一定距离的点 </returns>
		private Vector3 GetBetweenPointByDist(Vector3 start, Vector3 end, float distance)
		{
			Vector3 normal = (end - start).normalized;
			return normal * distance + start;
		}
		
		/// <summary>
		/// 获取两点之间距离一定百分比的一个点
		/// </summary>
		/// <param name="start">开始坐标点</param>
		/// <param name="end">终点坐标点</param>
		/// <param name="percent">要截取的百分比</param>
		/// <returns>两点之间距离一定百分比的一个点</returns>
		private Vector3 GetBetweenPointByPercent(Vector3 start, Vector3 end, float percent)
		{
			Vector3 normal = (end - start).normalized;
			float distance = Vector3.Distance(start, end);
			return normal * (distance * percent) + start;
		}
		
		/// <summary>
		/// 鼠标左键双击移动摄像头到事件坐标
		/// </summary>
		private void Move()
		{
			//鼠标是否放在UI上
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			
			if(Input.GetMouseButtonUp(0) && canMove){
				time2 = Time.realtimeSinceStartup; 
				
				//双击时执行
				if(time2 - time1 < 0.3){ 
					//从当前摄像头到鼠标的位置创建射线
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
					RaycastHit hit;
					bool isHit = Physics.Raycast(ray, out hit);//射线检测碰撞体
					if (isHit)
					{
						canZoom = canRotation = false;
						
						//获取摄像头移动的目标坐标
						var dist = Vector3.Distance(transform.position, hit.point) - zoomMin;
						moveTargetPoint = GetBetweenPointByDist(transform.position, hit.point, dist);
						
						//把摄像朝向目标点
						transform.DOLookAt(hit.point, 1f).OnComplete(() =>
						{
							currentAngleForX = transform.eulerAngles.x;
							canZoom = canRotation = true;
						});
						
						//重设旋转轴心点
						rotaAxis = new Vector3(hit.point.x, 0, hit.point.z);
					}
					else
					{
						moveTargetPoint = Vector3.zero;
					}
				}
				time1 = time2; 
			}
			

			if (moveTargetPoint != Vector3.zero)
			{
				canZoom = canRotation = false;
				float dist = Vector3.Distance(transform.position, moveTargetPoint);
				Vector3 move = Vector3.Lerp(transform.position, moveTargetPoint, Time.deltaTime * 5);
				transform.position = dist > 0.01f ? move : moveTargetPoint;
				
				if (Vector3.Distance(transform.position, moveTargetPoint) == 0)
				{
					moveTargetPoint = Vector3.zero;
					canZoom = canRotation = true;
				}
			}
		}

		/// <summary>
		/// 鼠标滚轮缩放场景效果
		/// </summary>
		private void Zoom() {
			//鼠标是否放在UI上
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			
			float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");

			if (mouseScrollWheel == 0 || canZoom==false)
				return;
			
			float zoom = mouseScrollWheel * zoomSpeed * Time.deltaTime * 30;

			//缩放设置的是坐标、旋转角度
			distance = Vector3.Distance(transform.position, rotaAxis);
			if (distance - zoom < zoomMin)
			{
				float offsetValue = distance - zoomMin;
				zoom = zoom < 0 ? -1 * offsetValue : offsetValue;
			}
			
			if (distance - zoom > zoomMax)
			{
				float offsetValue = zoomMax - distance;
				zoom = zoom < 0 ? -1 * offsetValue : offsetValue;
			}
			
			transform.Translate(new Vector3(0,0, zoom),Space.Self);
		}		

		/// <summary>
		/// 鼠标左键旋转场景效果
		/// </summary>
		private void Rotate() {
			//鼠标是否放在UI上
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			
			if(Input.GetMouseButton(0)  && canRotation) {
				//沿着X轴上下旋转
				float yMove = Input.GetAxis("Mouse Y") * 10 * rotateSpeed * Time.deltaTime;
				float rotatedAngle = currentAngleForX + yMove * -1;
				
				//限制沿X轴上下旋转旋转的角度
				if (rotatedAngle <= yMinLimit) {
					yMove = currentAngleForX - yMinLimit;
				} else if (rotatedAngle >= yMaxLimit) {
					yMove = -1 * (yMaxLimit - currentAngleForX);
				}

				transform.RotateAround(rotaAxis, -transform.right, yMove);
				currentAngleForX = ClampAngle(rotatedAngle, yMinLimit, yMaxLimit);
				
				//沿着Y轴左右旋转
				float xMove = Input.GetAxis("Mouse X") * 10 * rotateSpeed * Time.deltaTime;
				transform.RotateAround(rotaAxis, Vector3.up, xMove);

				time1 = time2;
			}
		}

		/// <summary>
		///  角度限制在 min与max之间,且>=-360和<=360
		/// </summary>
		/// <param name="angle">当前角度</param>
		/// <param name="min">最小角度</param>
		/// <param name="max">最大角度</param>
		/// <returns>限制在 min与max之间,且>=-360和<=360的角度</returns>
		private float ClampAngle(float angle, float min, float max) {
			if (angle < -360F)
				angle += 360F;
			if (angle > 360F)
				angle -= 360F;
			return Mathf.Clamp(angle, min, max);
		}

		
		public Vector3 RotaAxis
		{
			get => rotaAxis;
			set => rotaAxis = value;
		}

		/// <summary>
		/// 定点环绕360度巡航，镜头围绕给定的点旋转360度
		/// </summary>
		public void CameraCruiseByTarget(Vector3 target,UnityAction callback)
		{
			//禁用镜头操作			
			canMove = false;
			canRotation = false;
			canZoom = false;

			//绑定回调事件
			cameraCruiseByTargetCallBackListener = new UnityEvent();
			cameraCruiseByTargetCallBackListener.AddListener(callback);
			
			//定时器调用
			InvokeRepeating("CameraCruiseByTargetTimer", 0, 0.001F);
			
		}
		        
		//定点环绕360度巡航任务
		private void CameraCruiseByTargetTimer()
		{
			var angle = 10f * Time.deltaTime;
			transform.RotateAround(rotaAxis, Vector3.up, angle);
			cameraCruiseByTargetAngle += angle;
			
			//镜头当前巡航角度大于等于360度
			if (cameraCruiseByTargetAngle >= 360)
			{
				//停止调用
				CancelInvoke("CameraCruiseByTargetTimer");
				
				//恢复
				cameraCruiseByTargetAngle = 0;
				canMove = true;
				canRotation = true;
				canZoom = true;
				
				//回调
				cameraCruiseByTargetCallBackListener.Invoke();
			}
		}
		
		/// <summary>
		/// 定线巡航，给定路线集合，镜头按指定路线进行巡航
		/// </summary>
		public void CameraCruiseByPoVector3s(List<Vector3> poVector3s,UnityAction callback)
		{
			//禁用镜头操作	
			canMove = false;
			canRotation = false;
			canZoom = false;

			//绑定回调事件
			cameraCruiseByPoVector3sCallBackListener = new UnityEvent();
			cameraCruiseByPoVector3sCallBackListener.AddListener(callback);

			cameraCruiseByPoVector3sCount = 0;
			cameraCruiseByPoVector3sPoVector3s = poVector3s;
			for (var i = 0; i < cameraCruiseByPoVector3sPoVector3s.Count; i++)
			{
				float time = 1.5f * i;
				Invoke("CameraCruiseByPoVector3sTimer", time);
			}
		}
		
		//定线巡航任务
		private void CameraCruiseByPoVector3sTimer()
		{
			Vector3 poVector3 = cameraCruiseByPoVector3sPoVector3s[cameraCruiseByPoVector3sCount];
			cameraCruiseByPoVector3sCount++;
			
			//把摄像朝向目标点
			transform.DOLookAt(new Vector3(poVector3.x,0,poVector3.z), 0.5f).SetEase(Ease.Linear).OnComplete(() =>
			{
				//镜头移动至目标点
				transform.DOMove(poVector3, 1F).SetEase(Ease.Linear).OnComplete(() =>
				{
					//最后一次
					if (poVector3 == cameraCruiseByPoVector3sPoVector3s[cameraCruiseByPoVector3sPoVector3s.Count-1])
					{
						//停止调用
						CancelInvoke("CameraCruiseByPoVector3sTimer");
						
						//恢复
						cameraCruiseByPoVector3sCount = 0;
						canMove = true;
						canRotation = true;
						canZoom = true;
						
						//回调
						cameraCruiseByPoVector3sCallBackListener.Invoke();
					}
				});
			});
		}
	}
}