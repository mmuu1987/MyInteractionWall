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
    public string GetDirectName(Direct type)
    {

        DirectoryInfo dif = null;
        switch (type)
        {
            case Direct.None:
                break;
            case Direct.FirstDir:
                dif = new DirectoryInfo(Setting.FirstDir);
                return dif.Name;
            case Direct.SecondDir:
                dif = new DirectoryInfo(Setting.SecondDir);
                return dif.Name;
            case Direct.ThirdDir:
                dif = new DirectoryInfo(Setting.ThirdDir);
                return dif.Name;
            case Direct.IcOne:
                dif = new DirectoryInfo(Setting.IcOne);
                return dif.Name;
            case Direct.IcTwo:
                dif = new DirectoryInfo(Setting.IcTwo);
                return dif.Name;
            case Direct.IcThree:
                dif = new DirectoryInfo(Setting.IcThree);
                return dif.Name;
            case Direct.IcFour:
                dif = new DirectoryInfo(Setting.IcFour);
                return dif.Name;
            case Direct.IcFive:
                dif = new DirectoryInfo(Setting.IcFive);
                return dif.Name;
            case Direct.IcSix:
                dif = new DirectoryInfo(Setting.IcSix);
                return dif.Name;
            case Direct.PhOne:
                dif = new DirectoryInfo(Setting.PhOne);
                return dif.Name;
            case Direct.PhTwo:
                dif = new DirectoryInfo(Setting.PhTwo);
                return dif.Name;
            case Direct.PhThree:
                dif = new DirectoryInfo(Setting.PhThree);
                return dif.Name;
            case Direct.OsOne:
                dif = new DirectoryInfo(Setting.OsOne);
                return dif.Name;
            case Direct.OsTwo:
                dif = new DirectoryInfo(Setting.OsTwo);
                return dif.Name;
            case Direct.OsThree:
                dif = new DirectoryInfo(Setting.OsThree);
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

        string path = Application.streamingAssetsPath;//

        DirectoryInfo temp = new DirectoryInfo(path);

        FirstDir = temp + "/大事记/2001-2009";
        SecondDir = temp + "/大事记/2010-2019";
        ThirdDir = temp + "/大事记/2020";

        IcOne = temp + "/公司介绍/集团介绍";
        IcTwo = temp + "/公司介绍/基本信息";
        IcThree = temp + "/公司介绍/股东概况";
        IcFour = temp + "/公司介绍/荣誉奖项";
        IcFive = temp + "/公司介绍/产品体系";
        IcSix = temp + "/公司介绍/服务体系";

        PhOne = temp + "/私享传家/品牌介绍";
        PhTwo = temp + "/私享传家/尊享服务";
        PhThree = temp + "/私享传家/大湾区高净值中心";

        OsOne = temp + "/卓越风采/MDRT荣誉榜";
        OsTwo = temp + "/卓越风采/2020年MDRT达标榜";
        OsThree = temp + "/卓越风采/双百万储备力量";

        FirstDir = Application.streamingAssetsPath + "/大事记/2001-2009";
        SecondDir = Application.streamingAssetsPath + "/大事记/2010-2019";
        ThirdDir = Application.streamingAssetsPath + "/大事记/2020";

        IcOne = Application.streamingAssetsPath + "/公司介绍/集团介绍";
        IcTwo = Application.streamingAssetsPath + "/公司介绍/基本信息";
        IcThree = Application.streamingAssetsPath + "/公司介绍/股东概况";
        IcFour = Application.streamingAssetsPath + "/公司介绍/荣誉奖项";
        IcFive = Application.streamingAssetsPath + "/公司介绍/产品体系";
        IcSix = Application.streamingAssetsPath + "/公司介绍/服务体系";

        PhOne = Application.streamingAssetsPath + "/私享传家/品牌介绍";
        PhTwo = Application.streamingAssetsPath + "/私享传家/尊享服务";
        PhThree = Application.streamingAssetsPath + "/私享传家/大湾区高净值中心";

        OsOne = Application.streamingAssetsPath + "/卓越风采/MDRT荣誉榜";
        OsTwo = Application.streamingAssetsPath + "/卓越风采/2020年MDRT达标榜";
        OsThree = Application.streamingAssetsPath + "/卓越风采/双百万储备力量";
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
