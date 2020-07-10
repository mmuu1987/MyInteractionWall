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

    

    /// <summary>
    /// 改变文件夹名字
    /// </summary>
    public string GetDirectName(Direct type)
    {
       
        DirectoryInfo dif = null;
        switch (type)
        {
            case Direct.None:
                break;
            case Direct.FirstDir:
                dif  =new DirectoryInfo( Setting.FirstDir);
                return dif.Name;
               
            case Direct.SecondDir:
                dif = new DirectoryInfo(Setting.SecondDir);
                return dif.Name;
            case Direct.ThirdDir:
                dif = new DirectoryInfo(Setting.ThirdDir);
                return dif.Name;
              
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }

       // ChangeDirectName(path, directName);


       // SaveDirectInfo();
        return null;
    }

   

}

public class Setting
{
    /// <summary>
    /// 大事件的第一层文件夹路径
    /// </summary>
    public string FirstDir;
    /// <summary>
    /// 大事件的第一层文件夹路径
    /// </summary>
    public string SecondDir;
    /// <summary>
    /// 大事件的第一层文件夹路径
    /// </summary>
    public string ThirdDir;

    public Setting()
    {
        FirstDir = Application.streamingAssetsPath + "/大事记/2001-2009";
        SecondDir = Application.streamingAssetsPath + "/大事记/2010-2019";
        ThirdDir = Application.streamingAssetsPath + "/大事记/2020";
    }


}

public enum Direct
{
    None,
    FirstDir,
    SecondDir,
    ThirdDir
}
