using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于处理显示UI相关
/// </summary>
public class CanvasCtrl : MonoBehaviour {

    public static CanvasCtrl Instance = null;
    public RectTransform rect_info;
    public Text txt_tip;
    public Button btn_tip;

    private bool _isShowMenu = true;

    // Use this for initialization
    void Awake()
    {
        Instance = this;

        btn_tip.onClick.AddListener(OnClickButton);
        UpdateMenuState();
        UpdateLog("");
    }

    private void UpdateMenuState()
    {
        btn_tip.transform.Find("text").GetComponent<Text>().text = _isShowMenu ? "隐藏菜单提示" : "显示菜单提示";
        rect_info.gameObject.SetActive(_isShowMenu);
    }

    private void OnClickButton()
    {
        _isShowMenu = !_isShowMenu;
        Logger.Log("点击了按钮  _isShowMenu="+ _isShowMenu);
        UpdateMenuState();
    }
	 
    public void UpdateLog(string log)
    {
        txt_tip.text = log;
    }
}
