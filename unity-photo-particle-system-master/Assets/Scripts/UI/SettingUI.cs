using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置界面的UI
/// </summary>
public class SettingUI : MonoBehaviour
{

    public Button OpenDic;

    public Button OpenDoc;

    public Button EnterGame;


    /// <summary>
    /// 是否有按钮操作
    /// </summary>
    private bool isClick = false;

    private Coroutine _coroutine;
	// Use this for initialization
	void Start () {
		
        OpenDic.onClick.AddListener((() =>
        {
          string output = Application.streamingAssetsPath;
       
        output = output.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", output);
          isClick = true;

        }));

        OpenDoc.onClick.AddListener((() =>
        {
            string path = Application.streamingAssetsPath;
            Application.OpenURL("file:///" + path + "/" + "大屏互动外置文件规范.docx");
            isClick = true;
        }));


        EnterGame.onClick.AddListener((() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("zxdp");
        }));

	    AutoEnterGame();
      

      

	}

   
    /// <summary>
    /// 自动进入游戏
    /// </summary>
    private void AutoEnterGame()
    {



      StartCoroutine(Common.WaitTime(1f, (() =>
     {
         if (isClick)
         {
             //isClick = false;
             //AutoEnterGame();
             return;
         }
         
         EnterGame.onClick.Invoke();
        })));
    }
	// Update is called once per frame
	void Update () {
		
	}
}
