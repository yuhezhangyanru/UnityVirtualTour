using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine;
/// <summary>
/// function：用于记录一些坐标系计算的常用函数
/// date：2018-3-21 17:59:05
/// </summary>
public class VectorTool
{
    private static Canvas canvas
    {
        get
        {
            return GameObject.Find("Canvas").GetComponent<Canvas>();
        }
    }

    /// <summary>
    /// 模型的世界坐标，转为 画布的 画布坐标
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public static Vector3 GetWorldAnchorPos(Vector3 worldPos)//Transform needRect)
    {
        Vector3 result = Vector3.zero;
        Vector2 pos;

        Vector2 screen = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPos);// needRect.transform.position);
        Vector3 screenPos = new Vector3(screen.x, screen.y, 0);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out pos))
        {
            result = new Vector3(pos.x, pos.y, 0);

        }
        //result = screen;//

        return result;
    }
}