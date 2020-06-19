using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;
using Graphics = UnityEngine.Graphics;

/// <summary>
/// 处理图片，整理，分类
/// </summary>
public class PictureHandle : MonoBehaviour
{

    public static PictureHandle Instance;


    public RawImage TestImage;

    public Canvas Canvas;

    public Texture2DArray TexArr { get; set; }

    List<YearsInfo> _yesrsInfos = new List<YearsInfo>();

    public List<Texture2D> Texs = new List<Texture2D>();

    private List<int> _index20012009;
    private List<int> _index20102019;
    private List<int> _index2020Max;

    private void Awake()
    {
        if (Instance != null) throw new UnityException("单例错误");

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        LoadPicture();
        LoadTextureAssets();

        _index20012009 = GetYearIndex(1);
        _index20102019 = GetYearIndex(2);
        _index2020Max = GetYearIndex(3);


        HandleTextureArry(Texs);

        //LoadYearInfo();
        // var temp = Common.Sample2D(1920, 1080, 1,10);

        // Debug.Log(temp.Count);

        // UnityEngine.SceneManagement.SceneManager.LoadScene("test1");


    }

    private void LoadYearInfo()
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/Info");

        GameObject temp = Instantiate(go, Canvas.transform);

        Item item = temp.GetComponent<Item>();
      
        item.LoadData(_yesrsInfos[1].yearsEvents[1], TexArr);
    }
    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 获取该层次的图片索引
    /// </summary>
    /// <returns></returns>
    public int GetLevelIndex(int count, int level)
    {

        List<int> indexs;
        switch (level)
        {
            case 0://2001-2009

                indexs = _index20012009;
                break;
            case 1://2010-2019
                indexs = _index20102019;
                break;
            case 2://2020-至今
                indexs = _index2020Max;
                break;
            default:
                Debug.Log("levle is " + level);
                throw new UnityException("年代参数错误");
        }
        //根据个数分配索引
        int temp = count % indexs.Count;

        return indexs[temp];
    }

    /// <summary>
    /// 获取该层次图片索引的宽高,并返回该索引
    /// </summary>
    /// <param name="count">图片所在的个数</param>
    /// <param name="level">层级</param>
    /// <param name="index">返回图片的索引</param>
    /// <returns></returns>
    public Vector2 GetLevelIndexSize(int count, int level,out int index)
    {

        index =GetLevelIndex(count, level);

        foreach (YearsInfo yesrsInfo in _yesrsInfos)
        {
            foreach (YearsEvent @event in yesrsInfo.yearsEvents)
            {
                if (@event.PictureInfos.ContainsKey(index))
                {
                    return @event.PictureInfos[index];
                }
               
            }
        }

        return Vector2.zero;

    }

    /// <summary>
    /// 根据图片索引拿到年代事件信息
    /// </summary>
    public void GetYearInfo(PosAndDir pad, Canvas canvas)
    {
        if (pad.picIndex < 0) return;

        YearsEvent ye = null;
        foreach (YearsInfo yearsInfo in _yesrsInfos)
        {
            foreach (var yearsEvent in yearsInfo.yearsEvents)
            {
                foreach (int inde in yearsEvent.PictureIndes)
                {
                    if (pad.picIndex == inde)
                    {
                        ye = yearsEvent;
                        break;
                    }

                }
            }

        }

        if(ye==null)throw new UnityException("没有找到相应的年代事件");

        GameObject go = Resources.Load<GameObject>("Prefabs/Info");

        GameObject temp = Instantiate(go, canvas.transform);

        Item item = temp.GetComponent<Item>();

        item.LoadData(ye, TexArr);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(pad.position);

        RectTransform rectTransform = item.GetComponent<RectTransform>();

        rectTransform.DOScale(1f, 0.75f);
        //rectTransform.DOLocalRotate(new Vector3(0f, 360, 0f), 1f, RotateMode.LocalAxisAdd).OnComplete((() =>
        //{
        //    item.RotEnd();
        //}));

       
        rectTransform.anchoredPosition = screenPos;

        //Debug.Log(ye.ToString());
    }
    /// <summary>
    /// 根据年代层次获得该层次所有的图片索引
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public List<int> GetYearIndex(int level)
    {
        List<int> indes;

        switch (level)
        {
            case 1://2001-2009

                indes = GetYearsIndef(2001, 2009);
                break;
            case 2://2010-2019
                indes = GetYearsIndef(2010, 2019);
                break;
            case 3://2020-至今
                indes = GetYearsIndef(2020, 2029);
                break;
            default:
                throw new UnityException("年代参数错误");
        }
        return indes;
    }

    private List<int> GetYearsIndef(int minYears, int maxYears)
    {
        List<int> indes = new List<int>();
        foreach (YearsInfo yearsInfo in _yesrsInfos)
        {
            int year = int.Parse(yearsInfo.Years);
            if (year <= maxYears && year >= minYears)
            {
                foreach (var yearsEvent in yearsInfo.yearsEvents)
                {
                    indes.AddRange(yearsEvent.PictureIndes);
                }
            }
        }
        return indes;
    }
    /// <summary>
    /// 加载外部图片文件
    /// </summary>
    public void LoadPicture()
    {
        string path = Application.streamingAssetsPath + "/Picture";

        DirectoryInfo directoryInfo = new DirectoryInfo(path);

        DirectoryInfo[] infos = directoryInfo.GetDirectories();//获取年份目录
        foreach (DirectoryInfo info in infos)
        {
            DirectoryInfo[] temps = info.GetDirectories();//获取年份下事件

            YearsInfo tempYearsInfo = new YearsInfo();

            tempYearsInfo.Years = info.Name;

            tempYearsInfo.EventCount = temps.Length;


            int index = 0;
            foreach (DirectoryInfo temp in temps)
            {
                index++;
                YearsEvent yearsEvent = new YearsEvent();

                yearsEvent.Years = info.Name;
                yearsEvent.IndexPos = index;


                FileInfo[] fileInfos = temp.GetFiles();

                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (fileInfo.Extension == ".txt")
                    {

                        yearsEvent.DescribePath = fileInfo.FullName;
                    }
                    else if (fileInfo.Extension == ".jpg")
                    {
                        yearsEvent.PicturesPath.Add(fileInfo.FullName);
                    }
                    else if (fileInfo.Extension == ".mp4")
                    {
                        yearsEvent.YearEventVideo = fileInfo.FullName;
                    }
                }
                tempYearsInfo.yearsEvents.Add(yearsEvent);
            }

            _yesrsInfos.Add(tempYearsInfo);
        }


    }

    /// <summary>
    /// 加载图片资源
    /// </summary>
    public void LoadTextureAssets()
    {
        //先默认为512*512的图片,原始图片的长宽我们在用另外的vector2保存
        //生成需要表现的图片

        int pictureIndex = 0;//产生的图片索引
        foreach (YearsInfo yesrsInfo in _yesrsInfos)
        {
            foreach (YearsEvent yearsEvent in yesrsInfo.yearsEvents)
            {
                foreach (string s in yearsEvent.PicturesPath)
                {
                    if (File.Exists(s))
                    {

                        Vector2 vector2;

                        byte[] bytes = Common.MakeThumNail(s, 512, 512, "HW", out vector2);

                        Texture2D tex = new Texture2D(512, 512, TextureFormat.DXT1, false);

                        tex.LoadImage(bytes);

                        tex.Compress(true);

                        tex.Apply();

                        Texs.Add(tex);

                        yearsEvent.PictureIndes.Add(pictureIndex);

                        yearsEvent.AddPictureInfo(pictureIndex, vector2);

                        pictureIndex++;
                    }

                }
            }
        }

        // Debug.Log(Texs.Count);
    }

    public void DestroyTexture()
    {
        foreach (Texture2D texture2D in Texs)
        {
            Destroy(texture2D);
        }
        Texs.Clear();
        Texs = null;
        Resources.UnloadUnusedAssets();
    }


    private void HandleTextureArry(List<Texture2D> texs)
    {

        if (texs == null || texs.Count == 0)
        {
            enabled = false;
            return;
        }

        if (SystemInfo.copyTextureSupport == CopyTextureSupport.None ||
            !SystemInfo.supports2DArrayTextures)
        {
            enabled = false;
            return;
        }
        TexArr = new Texture2DArray(texs[0].width, texs[0].width, texs.Count, TextureFormat.DXT5, false, false);

        for (int i = 0; i < texs.Count; i++)
        {

            Graphics.CopyTexture(texs[i], 0, 0, TexArr, i, 0);

        }

        TexArr.wrapMode = TextureWrapMode.Clamp;
        TexArr.filterMode = FilterMode.Bilinear;
    }

    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0f, 0f, 300f, 300f), "Test"))
    //    {
    //        RestSizePicture();
    //    }
    //}

    private void RestSizePicture()
    {
        string path = "D:\\介绍图片_001.jpg";

        System.Diagnostics.Stopwatch watch = new Stopwatch();
        Vector2 size = Vector2.zero;
        watch.Start();
        byte[] bytes = Common.MakeThumNail(path, 512, 512, "HW",out size);
        //init();计算耗时的方法
        watch.Stop();
        var mSeconds = watch.ElapsedMilliseconds;
        UnityEngine.Debug.Log("耗时：" + mSeconds + "  原始尺寸是 " + size);

        Texture2D tex = new Texture2D(512, 512, TextureFormat.DXT5, false,true);

        tex.LoadImage(bytes);

        

        tex.Apply();

        TestImage.texture = tex;

        TestImage.SetNativeSize();
    }



}



/// <summary>
/// 年代信息类
/// </summary>

public class YearsInfo
{
    public string Years;
    /// <summary>
    /// 该年代的事件个数
    /// </summary>
    public int EventCount;

    public List<YearsEvent> yearsEvents;


    public YearsInfo()
    {
        yearsEvents = new List<YearsEvent>();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("\r\n");
        sb.Append("\r\n");
        sb.Append("Years is  " + Years + "  \r\n");
        sb.Append("EventCount is  " + EventCount + " \r\n");
        foreach (YearsEvent yearsEvent in yearsEvents)
        {
            sb.Append(yearsEvent.ToString());
        }
        sb.Append("\r\n");
        sb.Append("\r\n");
        return sb.ToString();
    }
}

/// <summary>
/// 年代事件类
/// </summary>
public class YearsEvent
{
    /// <summary>
    /// 所属年代
    /// </summary>
    public string Years;

    /// <summary>
    /// 事件顺序位置索引，比如该年的第一个事件，或者第二个事件
    /// </summary>
    public int IndexPos;

    /// <summary>
    /// 图片索引集合
    /// </summary>
    public List<int> PictureIndes;

    /// <summary>
    /// 该年代的事件描述
    /// </summary>
    public string Describe;

    /// <summary>
    /// 该年代的事件描述路径
    /// </summary>
    public string DescribePath;


    /// <summary>
    /// 该事件下的图片描述集合，存的是路径
    /// </summary>
    public List<string> PicturesPath;

    /// <summary>
    /// 描述该事件的视频
    /// </summary>
    public string YearEventVideo;
    /// <summary>
    /// 索引图片的长和宽
    /// </summary>
    public Dictionary<int, Vector2> PictureInfos;

    public YearsEvent()
    {
        PicturesPath = new List<string>();
        PictureIndes = new List<int>();
        PictureInfos = new Dictionary<int, Vector2>();
    }

    public void AddPictureInfo(int index, Vector2 size)
    {
        PictureIndes.Add(index);
        PictureInfos.Add(index, size);
    }
    public override string ToString()
    {


        StringBuilder sb = new StringBuilder();

        sb.Append("\r\n");
        sb.Append("\r\n");
        sb.Append("Years is  " + Years + " \r\n");
        sb.Append("IndexPos is  " + IndexPos + " \r\n");
        sb.Append("DescribePath is  " + DescribePath + " \r\n");
        foreach (string s in PicturesPath)
        {
            sb.Append("PicturesPath is " + s + "\r\n");
        }
        sb.Append("YearEventVideo is  " + YearEventVideo + "\r\n");
        sb.Append("\r\n");
        sb.Append("\r\n");

        return sb.ToString();
    }
}

/// <summary>
/// 任务信息
/// </summary>
public class PersonInfo
{
    public string PersonName;

    public string PicturePath;

    public int PictureIndex;

    public string DescribeFilePath;

    public string Describe;

    /// <summary>
    /// 介绍角色的视频  
    /// </summary>
    public string YearEventVideo;
}
