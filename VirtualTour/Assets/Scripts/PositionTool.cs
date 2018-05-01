using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;

public class PositionTool
{
    /// <summary>
    /// 获得一个BoxCollider立方体的8个顶点坐标
    /// </summary>
    /// <param name="boxcollider"></param>
    /// <returns></returns>
    public static  Vector3[] GetBoxColliderVertexPositions(BoxCollider boxcollider)
    {
        var vertices = new Vector3[8];
        //下面4个点
        vertices[0] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, -boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[1] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, -boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[2] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, -boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        vertices[3] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, -boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        //上面4个点
        vertices[4] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[5] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[6] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        vertices[7] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);

        return vertices;
    }
     
    /// <summary>
    /// 判断一个对象是否在一个boxcollider的区域内 ,即x,z是否在这个范围
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="colliderArea"></param>
    /// <returns></returns>

    public static bool isInBoxArea(Transform tran, BoxCollider colliderArea)
    {
        var rect = GetColliderHorizontalRect(colliderArea);
        return (rect.Contains(new Vector2(tran.position.x, tran.position.z)));
    }

    /// <summary>
    /// 获取一个boxCollider的verrtex
    /// </summary>
    /// <param name="boxCollder"></param>
    public static  Rect GetColliderHorizontalRect(BoxCollider boxCollder)
    {
        Vector3[] veces = GetBoxColliderVertexPositions(boxCollder);

        float xMax = float.MinValue;
        float xMin = float.MaxValue;
        float zMax = float.MinValue;
        float zMin = float.MaxValue;

        for (int index = 0; index < veces.Length; index++)
        {
            Vector3 curPos = veces[index];

            if (curPos.x > xMax)
            {
                xMax = curPos.x;
            }
            if (curPos.x < xMin)
            {
                xMin = curPos.x;
            }
            if (curPos.z > zMax)
            {
                zMax = curPos.z;
            }
            if (curPos.z < zMin)
            {
                zMin = curPos.z;
            }
        }

        Rect rect = new Rect();
        rect.xMin = xMin;
        rect.xMax = xMax;
        rect.yMin = zMin;
        rect.yMax = zMax;
        return rect;
    }
}