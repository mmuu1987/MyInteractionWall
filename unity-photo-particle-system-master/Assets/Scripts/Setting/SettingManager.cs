using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XHFrameWork;

/// <summary>
/// 设置管理，包含一些保存到本地的文件夹名字
/// </summary>
public class SettingManager : Singleton<SettingManager>
{

    public Setting Setting;

    public override void Init()
    {
        base.Init();

        Setting = new Setting();
        LoadDirectInfo();
    }



    /// <summary>
    /// 加载文件夹信息
    /// </summary>
    public void LoadDirectInfo()
    {
        string path = Application.streamingAssetsPath + "/Setting.data";

        if (!File.Exists(path))//不包含则创建，那就是游戏安装的时候第一次运行
        {
            string data = JsonUtility.ToJson(Setting);

            byte[] bytes = Encoding.UTF8.GetBytes(data);

            File.WriteAllBytes(path, bytes);
        }
        else
        {
            byte[] bytes = File.ReadAllBytes(path);

            string data = Encoding.UTF8.GetString(bytes);

            Setting = JsonUtility.FromJson<Setting>(data);
        }
    }

    public void SaveDirectInfo()
    {
        string path = Application.streamingAssetsPath + "/Setting.data";

        string data = JsonUtility.ToJson(Setting);

        byte[] bytes = Encoding.UTF8.GetBytes(data);

        File.WriteAllBytes(path, bytes);
    }

    /// <summary>
    /// 改变文件夹名字
    /// </summary>
    public void ChangeDirectName(Direct type, string directName)
    {
        string path = null;
        switch (type)
        {
            case Direct.None:
                break;
            case Direct.FirstDir:
                path = Application.streamingAssetsPath + "/大事记/" + Setting.FirstDir;
                Setting.FirstDir = directName;
                break;
            case Direct.SecondDir:
                path = Application.streamingAssetsPath + "/大事记/" + Setting.SecondDir;
                Setting.SecondDir = directName;
                break;
            case Direct.ThirdDir:
                path = Application.streamingAssetsPath + "/大事记/" + Setting.ThirdDir;
                Setting.ThirdDir = directName;
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }

        ChangeDirectName(path, directName);


        SaveDirectInfo();
    }

    private void ChangeDirectName(string path, string directName)
    {
        DirectoryInfo dif = new DirectoryInfo(path);

        string newPath = dif.Parent + "/" + directName;

        Directory.Move(dif.FullName, newPath);

        Debug.Log("改名成功");
    }

}

public class Setting
{
    /// <summary>
    /// 大事件的第一层文件夹名字
    /// </summary>
    public string FirstDir;
    /// <summary>
    /// 大事件的第一层文件夹名字
    /// </summary>
    public string SecondDir;
    /// <summary>
    /// 大事件的第一层文件夹名字
    /// </summary>
    public string ThirdDir;

    public Setting()
    {
        FirstDir = "2001-2009";
        SecondDir = "2010-2019";
        ThirdDir = "2020";
    }


}

public enum Direct
{
    None,
    FirstDir,
    SecondDir,
    ThirdDir
}
