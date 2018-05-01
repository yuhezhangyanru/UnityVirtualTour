using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// <summary>
/// function：运动的指令类型
/// </summary>
public enum MoveCmdType
{
    None,
    AdjustToMove, //由于距离太远，需要先调整够距离，再进入到移动状态
    MoveForward,
    MoveBack,

    //不同操作引起的旋转
    MouseRotateLeft,
    MouseRotateRight,
    RotateLeft,
    RotateRight,
}

/// <summary>
/// 相机镜头的移动方式
/// </summary>
public enum CameraMoveType
{
    None,
    MoveNear,
    MoveFar,
}