using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.IO;

public class EditorResourceCheck
{
    [MenuItem("ResourceCheck/CheckLargeImages")]
    public static void CheckLargeImages()
    {
        float percent = 0f;
        EditorUtility.DisplayProgressBar("检查大图资源", "检查资源进度:"+ percent+"%", percent);
		string[] allPath = Directory.GetFiles(Application.dataPath + "/", "*", SearchOption.AllDirectories);
        string logFile = Application.dataPath + "/log.txt";
        Debug.Log("time=" + Time.unscaledTime + "开始检查资源logFile="+ logFile);

        float single = 1f / allPath.Length;
        string log = "";
        //循环遍历每一个路径，单独加载
        foreach (string itemPath in allPath)
        {
            //替换路径中的反斜杠为正斜杠       
            string strTempPath = itemPath.Replace(@"\", "/");
            //截取我们需要的路径
            strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
            //根据路径加载资源
            bool isImage = strTempPath.EndsWith(".png") || itemPath.EndsWith(".jpg") | strTempPath.EndsWith(".tga") || strTempPath.EndsWith(".psd");
            if (isImage)
            {
                var bytes = File.ReadAllBytes(strTempPath);
                float mb = bytes.Length / 1024f / 1024f / 2f;
                int minMb = 6;
                if (mb > minMb)
                {
                    log += "\n异常：" + strTempPath + ",size=" + mb.ToString() + " mb 大于"+ minMb;
                }
            }
            EditorUtility.DisplayProgressBar("检查大图资源", "检查资源进度:" +Mathf.RoundToInt(percent * 100) + "%", percent);
            percent += single;
        }

        File.WriteAllText(logFile, log);
        Debug.Log("time=" + Time.unscaledTime + "检查资源");
        EditorUtility.ClearProgressBar();
	}

	[MenuItem("ResourceCheck/CheckPrefabRef")]
	public static void CheckPrefabRef()
	{

		float percent = 0f;
		EditorUtility.DisplayProgressBar("check prefab reference", "process:"+ percent+"%", percent);
		string[] allPath = Directory.GetFiles(Application.dataPath + "/", "*", SearchOption.AllDirectories);
		float single = 1f / allPath.Length;
		string logFile = Application.dataPath + "/log.txt";
		string log = "";

		var childs = GameObject.Find ("all_plants").GetComponentsInChildren<Transform> (true);
		Dictionary<string,bool> hasRef = new Dictionary<string, bool> ();
		List<string> objNameList = new List<string> ();
		for (int index = 0; index < childs.Length; index++) {
			string smallName = childs [index].name.ToLower ();
			if (objNameList.Contains (smallName)) {
				objNameList.Add (smallName);
			}
		}
		 
		Debug.Log("time=" + Time.unscaledTime + "开始检查资源logFile="+ logFile);
		foreach (string itemPath in allPath) {
			if (itemPath.EndsWith (".prefab")) {
				int index = itemPath.LastIndexOf (@"\");
				string itemName = itemPath.Substring (index+1,itemPath.Length-index-1);
				itemName = itemName.Replace (".prefab","");

				string itemNameSmall = itemName.ToLower ();
				hasRef [itemNameSmall] = false;
			}
			EditorUtility.DisplayProgressBar("check prefab reference", "process:" +Mathf.RoundToInt(percent * 100) + "%", percent);
			percent += single;
		}

		foreach (KeyValuePair<string,bool>pair in hasRef) {
			if (!objNameList.Contains (pair.Key)) {
				log += "\n item="+pair.Key;
			}
		}

		File.WriteAllText(logFile, log);
		Debug.Log("time=" + Time.unscaledTime + "检查资源 未被使用的prefab="+log);
		EditorUtility.ClearProgressBar();
	}
}