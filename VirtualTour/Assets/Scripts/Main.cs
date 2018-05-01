using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// function:控制玩家运动的主脚本，以及菜单显示
/// date:2018-4-20 23:13:20
/// </summary>
public class Main : MonoBehaviour
{
	public Transform tranAllBoxColliders;//所有带碰撞的根节点
    public BoxCollider boxcollider_back_area;//背后的碰撞返回
    public BoxCollider colliderRoomArea;
    public Texture2D textureMenuBg;
    /// <summary>
    /// 相机跟随Box的组件
    /// </summary>
    public FollowPlayer followCom;
    public Transform tranCube;//用来做测试的人偶
    public Rigidbody cubeRigibody;
    /// <summary>
    /// 行走速度
    /// </summary>
    private float moveSpeed = 2f;//1.5f;//1f;//0.1f;//0.05f;//3f;//0.5f;
    /// <summary>
    /// 行走的加速度
    /// </summary>
    private float moveSpeedAddBase = 1f;

    /// <summary>
    /// 左右的转向速度
    /// </summary>
    private float rotateSpeed = 1f;
    private float mouseRotateRate = 0.5f;//鼠标滑动旋转是 键盘旋转的50%幅度

    private static Vector2 VecMax = new Vector2(float.MaxValue, float.MaxValue);
    //判定鼠标滑动屏幕相关的
    private Vector2 first = VecMax;
    private Vector2 second = VecMax;

    /// <summary>
    /// 当前运动方式,前进后退
    /// </summary>
    private MoveCmdType _cmdMove = MoveCmdType.None;
    /// <summary>
    /// 还是左右转
    /// </summary>
    private MoveCmdType _cmdRotate = MoveCmdType.None;
    /// <summary>
    /// 镜头距离远近的运动
    /// </summary>
    private CameraMoveType _cmdCameraDis = CameraMoveType.None;

    /// <summary>
    /// 当前是否正常可行走状态，否的话，先走到最近距离 再允许移动
    /// </summary>
    private bool _cmdMoveToNormal = true;

    private GUIStyle _guiStyle;
    private string strGUITip = "";
    private string paramStateTip = "";

    // Use this for initialization
    void Awake()
    {
        //_guiStyle = new GUIStyle();
        //_guiStyle.normal.textColor = new Color(1, 0.55f, 0f, 1f);
        //_guiStyle.normal.background = textureMenuBg;
        strGUITip = "(1).左右滑动鼠标旋转";
        strGUITip += "\n(2).上下滑动鼠标调整视角俯仰";
        strGUITip += "\n(3).键盘映射长按：W:前进，S：后退。A：向左转。B：向右转。";
        strGUITip += "\n(4).镜头远近 N：拉近视角。M：拉远视角。";
        strGUITip += "\n(5).键盘事件会覆盖鼠标事件";
        strGUITip += "\n(6).需退出建筑范围才可拉远事件";

		//避免碰撞区被看到
		var childs = tranAllBoxColliders.GetComponentsInChildren<MeshRenderer>(true);
		for (int index = 0; index < childs.Length; index++) {
			childs [index].enabled = false;
		}
    }

    ////给一些屏幕操作提示
    //private void OnGUI()
    //{ 
    //    GUILayout.Label(paramStateTip+strGUITip, _guiStyle);
    //}

    /// <summary>
    /// 鼠标滑屏引起的旋转只会相应当前帧
    /// </summary>
    /// <returns></returns>
    private IEnumerator nextFrameStopRotate()
    {
        yield return new WaitForFixedUpdate();
        if(_cmdRotate == MoveCmdType.MouseRotateLeft || _cmdRotate== MoveCmdType.MouseRotateRight)
        {
            _cmdRotate = MoveCmdType.None;
        }
    }

    private bool isInArea = true;

    // Update is called once per frame
    void Update()
    {
        ////测试代码 .如果新的状态在范围内，需要直接定位到近距离
        //if(isInArea != PositionTool.isInBoxArea(tranCube, colliderRoomArea))
        //{
        //    Debug.LogError("范围状态发生变化！！isInBoxArea？" + PositionTool.isInBoxArea(tranCube, colliderRoomArea) + ",old=" + isInArea);
        //    isInArea = PositionTool.isInBoxArea(tranCube, colliderRoomArea);
        //}

        float rotateSpeeedUse = rotateSpeed;

        if (Input.GetKeyDown(KeyCode.W))
        {
            Logger.Log("向前走");
             _cmdMove = MoveCmdType.MoveForward;
            //StartCoroutine(waitSetNormalMoveState(MoveCmdType.MoveForward));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Logger.Log("往后退");
            _cmdMove = MoveCmdType.MoveBack;
            //StartCoroutine(waitSetNormalMoveState(MoveCmdType.MoveBack));
        }
        
        //判断鼠标是否滑动屏幕了
        if (_cmdRotate == MoveCmdType.None)
        {
            if (first == VecMax)
            {
                first = Input.mousePosition;
            }
            else
            {
                //记录鼠标拖动的位置  
                second = Input.mousePosition;// Event.current.mousePosition;
                //讨论左右滑动
                if (second.x < first.x)
                {
                    //拖动的位置的x坐标比按下的位置的x坐标小时,响应向左事件  
                    Logger.Log("鼠标左滑");
                    _cmdRotate = MoveCmdType.MouseRotateLeft;
                    StopCoroutine(nextFrameStopRotate());
                    StartCoroutine(nextFrameStopRotate());
                    rotateSpeeedUse *= mouseRotateRate;
                }
                else if (second.x > first.x)
                {
                    //拖动的位置的x坐标比按下的位置的x坐标大时,响应向右事件  
                    Logger.Log("鼠标右滑");
                    _cmdRotate = MoveCmdType.MouseRotateRight;
                    StopCoroutine(nextFrameStopRotate());
                    StartCoroutine(nextFrameStopRotate());
                    rotateSpeeedUse *= mouseRotateRate;
                }
                else if (second.y < first.y) // //讨论鼠标上下移动 鼠标下滑动,距离越远，改动俯仰的程度应该越大，不然镜头扭不动了
                {
                    followCom.height -= FollowPlayer.HEIGHT_MOVE_SUB_BASE * getActionSpeed();
                    followCom.height = Mathf.Max(followCom.height, 0);//视角不能钻到地下！
                }
                else if (second.y > first.y) //鼠标上滑动
                {
                    followCom.height += FollowPlayer.HEIGHT_MOVE_SUB_BASE * getActionSpeed();
                }
                first = second;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Logger.Log("向左转");
            _cmdRotate = MoveCmdType.RotateLeft;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Logger.Log("向右转");
            _cmdRotate = MoveCmdType.RotateRight;
        }

        //判定是否终止标记
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            Logger.Log("松开前后操作键，终止位移");
            _cmdMove = MoveCmdType.None;
            moveSpeedAddBase = 1f;
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            Logger.Log("松开左右操作键，终止旋转");
            _cmdRotate = MoveCmdType.None;
            first = VecMax;
            second = VecMax;
        }

        //最后根据当前的指令决定怎么运动！
        DoMove(moveSpeed * getActionSpeed());//23点47分刚刚加入，远距离就改为较快的移动速度
        DORotate(rotateSpeeedUse);

        UpdateCemeraDistanceState();

        //刷新当前参数：
        paramStateTip = "当前视距:" + followCom.distance + ",当前视野高度:" + followCom.height + "\n";
        CanvasCtrl.Instance.UpdateLog(paramStateTip + strGUITip);
    }

    /// <summary>
    /// 根据我当前距离的远近决定操控的速度
    /// </summary>
    /// <returns></returns>
    private float getActionSpeed()
    {
        return followCom.distance / FollowPlayer.DISTANCE_MIN;
    }

    /// <summary>
    /// 最后关于 镜头拉远还是拉近的处理
    /// </summary>
    private void UpdateCemeraDistanceState()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            _cmdCameraDis = CameraMoveType.MoveNear;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            _cmdCameraDis = CameraMoveType.MoveFar;
        }
        if (Input.GetKeyUp(KeyCode.N) || Input.GetKeyUp(KeyCode.M))
        {
            _cmdCameraDis = CameraMoveType.None;
        }
        switch (_cmdCameraDis)
        {
            case CameraMoveType.MoveNear:
                {
                    setCamereMoveFar(-FollowPlayer.MOVE_DISTANCE_SUB); 
                }
                break;
            case CameraMoveType.MoveFar:
                {
                    setCamereMoveFar(+FollowPlayer.MOVE_DISTANCE_SUB);
                }
                break;
        }
    }

    /// <summary>
    /// 修改相机的距离
    /// </summary>
    /// <param name="subValue"></param>
    private void setCamereMoveFar(float subValue)
    {
        followCom.distance += subValue;
        followCom.distance = Mathf.Min(followCom.distance, FollowPlayer.DISTANCE_MAX);
        followCom.distance = Mathf.Max(followCom.distance, FollowPlayer.DISTANCE_MIN);
    }
     
    /// <summary>
    /// 检查玩家执行运动
    /// </summary>
    /// <param name="moveSpeedValue"></param>
    private void DoMove(float moveSpeedValue)
    {
        if(PositionTool.isInBoxArea(tranCube, colliderRoomArea))
        {
            bool fromRemote = (followCom.distance > FollowPlayer.DISTANCE_MIN) && (followCom.height > FollowPlayer.HEIGHT_MIN);
            if (fromRemote)
            {
                Logger.Log("此时玩家的位置=" + tranCube.position + "进入到了box区域 从天上掉下来，从背后进门？" + (PositionTool.isInBoxArea(tranCube, boxcollider_back_area)));
                if (PositionTool.isInBoxArea(tranCube, boxcollider_back_area))
                {
                    tranCube.position = new Vector3(-39.6f, 7.7f, -24f);
                    tranCube.eulerAngles = new Vector3(0, 95, 0);
                }
                else
                {
                    tranCube.position = new Vector3(37.7f, 7.7f, -9.5f);
                    tranCube.eulerAngles = new Vector3(0, -90, 0);
                }
            }
            if (followCom.distance> FollowPlayer.DISTANCE_MIN)
            {
                followCom.distance = FollowPlayer.DISTANCE_MIN;
            }
            if(followCom.height> FollowPlayer.HEIGHT_MIN)
            {
                followCom.height = FollowPlayer.HEIGHT_MIN;
            }
        }
         
        //向前或后位移
        switch (_cmdMove)
        {
            case MoveCmdType.MoveForward:
                {
                    //Logger.Log("执行运动");
                    //cubeRigibody.AddForce(Vector3.forward*10000, ForceMode.Acceleration);
                    tranCube.Translate(0, 0, moveSpeedValue * Time.deltaTime * moveSpeedAddBase);
                }
                break;
            case MoveCmdType.MoveBack:
                {
                    tranCube.Translate(0, 0, -moveSpeedValue * Time.deltaTime * moveSpeedAddBase);
                }
                break;
            default:
                return;
        }
        //tranCube.Translate(nextMove);// 0, 0, moveSpeedValue * Time.deltaTime * moveSpeedAddBase);

        moveSpeedAddBase *= 1.01f;
        moveSpeedAddBase = Mathf.Min(moveSpeedAddBase, 1.8f);
    }

    private void DORotate(float rotateSpeedValue)
    {
        //向左或向右旋转
        switch (_cmdRotate)
        {
            case MoveCmdType.MouseRotateLeft:
            case MoveCmdType.RotateLeft:
                {
                    tranCube.Rotate(new Vector3(0, -rotateSpeedValue, 0));
                }
                break;
            case MoveCmdType.MouseRotateRight:
            case MoveCmdType.RotateRight:
                {
                    tranCube.Rotate(new Vector3(0, rotateSpeedValue, 0));
                }
                break;
        }
    }
}
