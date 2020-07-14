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
            Setting = new Setting();

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
    /// 获取文件夹名字
    /// </summary>
    public string GetDirectName(Direct type,bool isPath=false)
    {

        DirectoryInfo dif = null;
        string path = Application.streamingAssetsPath+"/";
        string temp = null;
        switch (type)
        {
            case Direct.None:
                break;
            case Direct.FirstDir:
                dif = new DirectoryInfo(path+Setting.FirstDir);
                break;
            case Direct.SecondDir:
                dif = new DirectoryInfo(path + Setting.SecondDir);
                break;
            case Direct.ThirdDir:
                dif = new DirectoryInfo(path + Setting.ThirdDir);
                break;
            case Direct.IcOne:
                dif = new DirectoryInfo(path + Setting.IcOne);
                break;
            case Direct.IcTwo:
                dif = new DirectoryInfo(path + Setting.IcTwo);
                break;
            case Direct.IcThree:
                dif = new DirectoryInfo(path + Setting.IcThree);
                break;
            case Direct.IcFour:
                dif = new DirectoryInfo(path + Setting.IcFour);
                break;
            case Direct.IcFive:
                dif = new DirectoryInfo(path + Setting.IcFive);
                break;
            case Direct.IcSix:
                dif = new DirectoryInfo(path + Setting.IcSix);
                break;
            case Direct.PhOne:
                dif = new DirectoryInfo(path + Setting.PhOne);
                break;
            case Direct.PhTwo:
                dif = new DirectoryInfo(path + Setting.PhTwo);
                break;
            case Direct.PhThree:
                dif = new DirectoryInfo(path + Setting.PhThree);
                break;
            case Direct.OsOne:
                dif = new DirectoryInfo(path + Setting.OsOne);
                break;
            case Direct.OsTwo:
                dif = new DirectoryInfo(path + Setting.OsTwo);
                break;
            case Direct.OsThree:
                dif = new DirectoryInfo(path + Setting.OsThree);
                break;


            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }

        if (isPath) return dif.FullName;
         return dif.Name;
       
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
    /// <summary>
    /// 公司介绍里面的的第1栏
    /// </summary>
    public string IcOne;
    /// <summary>
    /// 公司介绍里面的的第2栏
    /// </summary>
    public string IcTwo;
    /// <summary>
    /// 公司介绍里面的的第3栏
    /// </summary>
    public string IcThree;
    /// <summary>
    /// 公司介绍里面的的第4栏
    /// </summary>
    public string IcFour;
    /// <summary>
    /// 公司介绍里面的的第5栏
    /// </summary>
    public string IcFive;
    /// <summary>
    /// 公司介绍里面的的第6栏
    /// </summary>
    public string IcSix;

    /// <summary>
    /// 私享穿甲第1栏
    /// </summary>
    public string PhOne;

    /// <summary>
    /// 私享穿甲第2栏
    /// </summary>
    public string PhTwo;
    /// <summary>
    /// 私享穿甲第3栏
    /// </summary>
    public string PhThree;

    /// <summary>
    /// 卓越风采第1栏
    /// </summary>
    public string OsOne;
    /// <summary>
    /// 卓越风采第2栏
    /// </summary>
    public string OsTwo;
    /// <summary>
    /// 卓越风采第3栏
    /// </summary>
    public string OsThree;


    public Setting()
    {

      

        FirstDir = "/大事记/2001-2009";
        SecondDir =  "/大事记/2010-2019";
        ThirdDir =  "/大事记/2020";

        IcOne =  "/公司介绍/集团介绍";
        IcTwo = "/公司介绍/基本信息";
        IcThree =  "/公司介绍/股东概况";
        IcFour =  "/公司介绍/荣誉奖项";
        IcFive = "/公司介绍/产品体系";
        IcSix =  "/公司介绍/服务体系";

        PhOne =  "/私享传家/品牌介绍";
        PhTwo =  "/私享传家/尊享服务";
        PhThree =  "/私享传家/大湾区高净值中心";

        OsOne = "/卓越风采/MDRT荣誉榜";
        OsTwo ="/卓越风采/2020年MDRT达标榜";
        OsThree =  "/卓越风采/双百万储备力量";

    
    }


}

public enum Direct
{
    None,
    FirstDir,
    SecondDir,
    ThirdDir,
    IcOne,
    IcTwo,
    IcThree,
    IcFour,
    IcFive,
    IcSix,
    PhOne,
    PhTwo,
    PhThree,
    OsOne,
    OsTwo,
    OsThree
    
}
