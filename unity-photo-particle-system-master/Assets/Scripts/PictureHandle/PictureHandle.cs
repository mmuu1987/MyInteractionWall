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
using Microsoft.Win32.SafeHandles;
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

    public List<Texture2D> YearTexs = new List<Texture2D>();

    private List<YearsInfo> _yesrsInfos = new List<YearsInfo>();

    public List<Texture2D> Texs = new List<Texture2D>();


    private List<int> _index20012009;
    private List<int> _index20102019;
    private List<int> _index2020Max;

    /// <summary>
    /// 卓越风采
    /// </summary>
    public List<List<PersonInfo>> PersonInfos = new List<List<PersonInfo>>();

    /// <summary>
    /// 荣誉墙
    /// </summary>
    public List<PersonInfo> HonorWall =new List<PersonInfo>();

    private GameObject _info;

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

        LoadCompanyIntroductionPic();
        LoadPrivateHeirsPic();
        LoadPersonInfo();

        _index20012009 = GetYearIndex(1);
        _index20102019 = GetYearIndex(2);
        _index2020Max = GetYearIndex(3);


        HandleTextureArry(Texs);


        _info = Resources.Load<GameObject>("Prefabs/Info");
        //预制体缩放，后面用来做缩放动画
        _info.transform.localScale = Vector3.one * 0.35f;
        //LoadYearInfo();
        // var temp = Common.Sample2D(1920, 1080, 1,10);

        // Debug.Log(temp.Count);

        //UnityEngine.SceneManagement.SceneManager.LoadScene("demo1");

        //
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
    public Vector2 GetLevelIndexSize(int count, int level, out int index)
    {

        index = GetLevelIndex(count, level);

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
    public void GetYearInfo(PosAndDir pad, Transform canvas)
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

        if (ye == null) throw new UnityException("没有找到相应的年代事件");




        GameObject temp = Instantiate(_info, canvas.transform);

        Item item = temp.GetComponent<Item>();

        item.LoadData(ye, TexArr);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(pad.position);

        RectTransform rectTransform = item.GetComponent<RectTransform>();
        // rectTransform.SetSiblingIndex(2);
        rectTransform.DOScale(1f, 0.75f);
        //rectTransform.DOLocalRotate(new Vector3(0f, 360, 0f), 1f, RotateMode.LocalAxisAdd).OnComplete((() =>
        //{
        //    item.RotEnd();
        //}));
        rectTransform.anchoredPosition = screenPos;

        if (screenPos.y >= 2300f) screenPos.y = 2300f;
        if (screenPos.y <= 1100f) screenPos.y = 1100f;
        if (screenPos.x >= 6500f) screenPos.x = 6500f;
        if (screenPos.x <= 1000f) screenPos.x = 1000f;


        rectTransform.DOAnchorPos(screenPos,0.35f);
       

        //Debug.Log(ye.ToString());
    }

    public YearsEvent GetYearInfo(PosAndDir pad )
    {
        if (pad.picIndex < 0) return null;

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

        if (ye == null) throw new UnityException("没有找到相应的年代事件");




        return ye;


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
        string path = Application.streamingAssetsPath + "/大事记";

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

                        byte[] bytes = File.ReadAllBytes(fileInfo.FullName);

                        string str = Encoding.UTF8.GetString(bytes);

                        yearsEvent.Describe = str;
                    }
                    else if (fileInfo.Extension == ".jpg" || fileInfo.Extension == ".JPG" || fileInfo.Extension == ".jpeg")
                    {

                        yearsEvent.PicturesPath.Add(fileInfo.FullName);
                    }
                    else if (fileInfo.Extension == ".mp4")
                    {
                        yearsEvent.YearEventVideo = fileInfo.FullName;
                    }
                    else if (fileInfo.Extension == ".png" || fileInfo.Extension == ".PNG")
                    {
                        yearsEvent.PicturesPath.Add(fileInfo.FullName);
                        // Debug.Log(fileInfo.FullName);
                    }
                }
                tempYearsInfo.yearsEvents.Add(yearsEvent);
            }

            _yesrsInfos.Add(tempYearsInfo);
        }


    }

    public void LoadCompanyIntroductionPic()
    {
        CompanyAllTexList = new List<CompanyInfo>();

        string path1 = Application.streamingAssetsPath + "/公司介绍/集团介绍";

        CompanyAllTexList.Add(LoadCompanyIntroductionPic(path1));

        string path2 = Application.streamingAssetsPath + "/公司介绍/基本信息";
        CompanyAllTexList.Add(LoadCompanyIntroductionPic(path2));

        string path3 = Application.streamingAssetsPath + "/公司介绍/股东概况";
        CompanyAllTexList.Add(LoadCompanyIntroductionPic(path3));

        string path4 = Application.streamingAssetsPath + "/公司介绍/荣誉奖项";
        CompanyAllTexList.Add(LoadCompanyIntroductionPic(path4));

        string path5 = Application.streamingAssetsPath + "/公司介绍/产品体系";
        CompanyAllTexList.Add(LoadCompanyIntroductionPic(path5));

        string path6 = Application.streamingAssetsPath + "/公司介绍/服务体系";
        CompanyAllTexList.Add(LoadCompanyIntroductionPic(path6));
    }

    public void LoadPrivateHeirsPic()
    {
        PrivateHeirsAllTexList = new List<CompanyInfo>();

        string path1 = Application.streamingAssetsPath + "/私享传家/品牌介绍";

        PrivateHeirsAllTexList.Add(LoadCompanyIntroductionPic(path1));

        string path2 = Application.streamingAssetsPath + "/私享传家/尊享服务";
        PrivateHeirsAllTexList.Add(LoadCompanyIntroductionPic(path2));

        string path3 = Application.streamingAssetsPath + "/私享传家/大湾区高净值中心";
        PrivateHeirsAllTexList.Add(LoadCompanyIntroductionPic(path3));

    }

    public List<CompanyInfo> CompanyAllTexList { get; set; }

    public List<CompanyInfo> PrivateHeirsAllTexList;

    /// <summary>
    /// 导入公司介绍图片
    /// </summary>
    public CompanyInfo LoadCompanyIntroductionPic(string path)
    {
        //string str1 = Application.streamingAssetsPath + "/公司介绍/企业介绍";

        CompanyInfo companyInfo = new CompanyInfo();

        List<Texture2D> texs = new List<Texture2D>();

        List<string> videoPaths = new List<string>();

        DirectoryInfo info = new DirectoryInfo(path);

        FileInfo[] files = info.GetFiles();

        foreach (FileInfo file in files)
        {
            if (file.FullName.Contains("meta")) continue;

            if (file.Extension == ".jpg" || file.Extension == ".JPG" || file.Extension == ".png" ||
                file.Extension == ".PNG" ||file.Extension == ".jpeg")
            {
                if (File.Exists(file.FullName))
                {
                    byte[] bytes = File.ReadAllBytes(file.FullName);

                    Texture2D tex = new Texture2D(512, 512, TextureFormat.DXT1, false);

                    tex.LoadImage(bytes);

                    tex.Compress(true);

                    tex.Apply();

                    texs.Add(tex);
                }
            }
            if (file.Extension == ".mp4")
            {
               
                videoPaths.Add(  file.FullName);
            }

          
        }
        companyInfo.TexInfo = texs;
        companyInfo.VideoInfo = videoPaths;
        return companyInfo;
    }

    public void LoadPersonInfo()
    {
        string path1 = Application.streamingAssetsPath + "/卓越风采/MDRT荣誉榜";
       PersonInfos.Add(LoadPersonInfo(path1));
       string path2 = Application.streamingAssetsPath + "/卓越风采/2020年MDRT达标榜";
       PersonInfos.Add(LoadPersonInfo(path2));
       string path3 = Application.streamingAssetsPath + "/卓越风采/双百万储备力量";
       PersonInfos.Add(LoadPersonInfo(path3));


        //荣誉墙
        string path4 = Application.streamingAssetsPath + "/荣誉墙";
        HonorWall = LoadPersonInfo(path4);
    }

    /// <summary>
    /// 加载人物信息 
    /// </summary>
    public List<PersonInfo> LoadPersonInfo(string path)
    {

        List<PersonInfo> personInfos = new List<PersonInfo>();
      

        DirectoryInfo directoryInfo = new DirectoryInfo(path);

        DirectoryInfo[] infos = directoryInfo.GetDirectories();//获取角色目录数
        foreach (DirectoryInfo info in infos)
        {
            string personName = null;
            if (info.Name.Contains("_"))//卓越风采的文件夹命名风格
            {
                personName = info.Name.Substring(4, info.Name.Length - 4);
            }
            else personName = info.Name;//荣誉墙的命名风格

            PersonInfo personInfo = new PersonInfo { PersonName = personName };


            FileInfo[] fileInfos = info.GetFiles();

            foreach (FileInfo fileInfo in fileInfos)
            {
                if (fileInfo.Extension == ".txt")
                {
                    personInfo.DescribeFilePath = fileInfo.FullName;
                }
                else if (fileInfo.Extension == ".jpg" || fileInfo.Extension == ".JPG" )
                {
                    personInfo.PicturePath = fileInfo.FullName;
                }
                else if (fileInfo.Extension == ".mp4")
                {
                    personInfo.YearEventVideo = fileInfo.FullName;
                }
                else if (fileInfo.Extension == ".png" || fileInfo.Extension == ".PNG")
                {
                    personInfo.PicturePath = fileInfo.FullName;
                }
            }
            personInfos.Add(personInfo);
        }
        //加载图片
        //加载文本
        int index = 0;
        foreach (PersonInfo personInfo in personInfos)
        {
            if (!string.IsNullOrEmpty(personInfo.DescribeFilePath))
            {
                byte[] bytes = File.ReadAllBytes(personInfo.DescribeFilePath);

                string str = Encoding.UTF8.GetString(bytes);
                

                personInfo.Describe = str;

            }

            if (!string.IsNullOrEmpty(personInfo.PicturePath))
            {
                string s = personInfo.PicturePath;

                if (File.Exists(s))
                {
                    Vector2 vector2;

                    byte[] bytes = Common.MakeThumNail(s, 512, 512, "HW", out vector2);

                    Texture2D tex = new Texture2D(512, 512, TextureFormat.DXT1, false);

                    tex.LoadImage(bytes);

                    tex.Compress(true);

                    tex.Apply();



                    personInfo.PictureIndex = index;

                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    sprite.name = index.ToString();
                    personInfo.headTex = sprite;

                    index++;

                }
                else Debug.LogError("没有找到头像文件");


            }
        }

        return personInfos;
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
                if (yearsEvent.PicturesPath.Count <= 0)//如果没有图片，我们生成一个logo的先填充
                {
                    string s = Application.streamingAssetsPath + "/logo.png";

                    Vector2 vector2;

                    byte[] bytes = Common.MakeThumNail(s, Common.PictureWidth, Common.PictureHeight, "HW", out vector2);

                    Texture2D tex = new Texture2D(Common.PictureWidth, Common.PictureHeight, TextureFormat.DXT1, false);

                    tex.LoadImage(bytes);

                    tex.Compress(true);

                    tex.Apply();

                    Texs.Add(tex);

                    yearsEvent.PictureIndes.Add(pictureIndex);

                    yearsEvent.AddPictureInfo(pictureIndex, vector2);

                    pictureIndex++;
                }
                else
                    foreach (string s in yearsEvent.PicturesPath)
                    {

                        if (File.Exists(s))
                        {

                            Vector2 vector2;

                            FileInfo fileInfo = new FileInfo(s);

                            byte[] bytes = HandlePicture(yearsEvent.Years, fileInfo.DirectoryName, fileInfo.Name, out vector2);


                            Texture2D tex = new Texture2D(Common.PictureWidth, Common.PictureHeight, TextureFormat.DXT1, false);

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
   
    public ComputeShader shader;

    public int width = 10;//边框像素单位宽度

    public int LableHeight = 10;//文字占有高度像素单位



   

   /// <summary>
   /// 用computeshader来设置贴图边框
   /// </summary>
   /// <param name="sourceTex"></param>
   /// <param name="contents"></param>
   /// <param name="fileName"></param>
   /// <returns></returns>
    string RunShader(string year, string contents, string fileName)
    {
        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(contents+"/"+fileName);

        byte[] bytes;
        MemoryStream ms = new MemoryStream();

       
        originalImage.Save(ms, ImageFormat.Jpeg);

        bytes = ms.GetBuffer();
        ms.Dispose();

        //获取年代图片
       Texture2D yearTex = null;

       foreach (Texture2D texture2D in YearTexs)
       {
           if (texture2D.name == year)
           {
               yearTex = texture2D;
               break;
           }
       }


        
        Texture2D sourceTex = new Texture2D(originalImage.Width,originalImage.Height);



        sourceTex.LoadImage(bytes);

        sourceTex.Apply();

        originalImage.Dispose();
        
        

        int texWidth = sourceTex.width + 2 * width;
        int texHeight = sourceTex.height + 2 * width + LableHeight;
        ////////////////////////////////////////
        //    RenderTexture
        ////////////////////////////////////////
        //1 新建RenderTexture
        RenderTexture rt = new RenderTexture(texWidth, texHeight, 24);
        //2 开启随机写入
        rt.enableRandomWrite = true;
        //3 创建RenderTexture
        rt.Create();
      
        ////////////////////////////////////////
        //    Compute Shader
        ////////////////////////////////////////
        //1 找到compute shader中所要使用的KernelID
        int k = shader.FindKernel("CSMain1");
        //2 设置贴图    参数1=kid  参数2=shader中对应的buffer名 参数3=对应的texture, 如果要写入贴图，贴图必须是RenderTexture并enableRandomWrite
        shader.SetTexture(k, "Result", rt);
        shader.SetTexture(k, "Source", sourceTex);
        shader.SetTexture(k, "YearTex", yearTex);
        shader.SetInt("BorderWidth", width);
        shader.SetInt("LableHeight", LableHeight);
       


        Debug.Log("tex info width is " + texWidth + "  Height is " + texHeight);
        //3 运行shader  参数1=kid  参数2=线程组在x维度的数量 参数3=线程组在y维度的数量 参数4=线程组在z维度的数量
        shader.Dispatch(k, texWidth, texHeight, 1);
     
        return   SaveRenderTextureToJpg(rt, contents, fileName);
    }
    /// <summary>
    /// 给图片加边框和标题，标题写的是年份，并且缩放图片规格，返回字节数据
    /// </summary>
    /// <param name="year"></param>
    /// <param name="contents"></param>
    /// <param name="fileName"></param>
    /// <param name="size">返回图片原始尺寸</param>
    /// <returns></returns>
    byte [] HandlePicture(string year, string contents, string fileName,out Vector2 size)
    {
        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(contents + "/" + fileName);

        size.x = originalImage.Width;
        size.y = originalImage.Height;

        byte[] bytes;

        MemoryStream ms = new MemoryStream();

        originalImage.Save(ms, ImageFormat.Jpeg);

        bytes = ms.GetBuffer();
        ms.Dispose();

        //获取年代图片
        Texture2D yearTex = null;

        foreach (Texture2D texture2D in YearTexs)
        {
            if (texture2D.name == year)
            {
                yearTex = texture2D;
                break;
            }
        }



        Texture2D sourceTex = new Texture2D(originalImage.Width, originalImage.Height);



        sourceTex.LoadImage(bytes);

        sourceTex.Apply();

        originalImage.Dispose();



        int texWidth = sourceTex.width + 2 * width;
        int texHeight = sourceTex.height + 2 * width + LableHeight;
        ////////////////////////////////////////
        //    RenderTexture
        ////////////////////////////////////////
        //1 新建RenderTexture
        RenderTexture rt = new RenderTexture(texWidth, texHeight, 24);
        //2 开启随机写入
        rt.enableRandomWrite = true;
        //3 创建RenderTexture
        rt.Create();

        ////////////////////////////////////////
        //    Compute Shader
        ////////////////////////////////////////
        //1 找到compute shader中所要使用的KernelID
        int k = shader.FindKernel("CSMain1");
        //2 设置贴图    参数1=kid  参数2=shader中对应的buffer名 参数3=对应的texture, 如果要写入贴图，贴图必须是RenderTexture并enableRandomWrite
        shader.SetTexture(k, "Result", rt);
        shader.SetTexture(k, "Source", sourceTex);
        shader.SetTexture(k, "YearTex", yearTex);
        shader.SetInt("BorderWidth", width);
        shader.SetInt("LableHeight", LableHeight);



        Debug.Log("tex info width is " + texWidth + "  Height is " + texHeight);
        //3 运行shader  参数1=kid  参数2=线程组在x维度的数量 参数3=线程组在y维度的数量 参数4=线程组在z维度的数量
        shader.Dispatch(k, texWidth, texHeight, 1);

       
        RenderTexture.active = rt;
        Texture2D jpg = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        jpg.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        byte[] bytesEnd = jpg.EncodeToJPG();



        return Common.MakeThumNail(bytesEnd, Common.PictureWidth, Common.PictureHeight, "HW"); 

    }

    //将RenderTexture保存成一张png图片  
    public string SaveRenderTextureToJpg(RenderTexture rt, string contents, string fileName)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToJPG();

        string filePath = contents + "/AddOutLine_" + fileName ;

        File.WriteAllBytes(filePath, bytes);
        //同时，删掉旧贴图
        File.Delete(contents + "/" + fileName );

        RenderTexture.active = prev;

        rt.Release();

        return filePath;

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
/// 人物信息
/// </summary>
public class PersonInfo
{
    /// <summary>
    /// 人物名字
    /// </summary>
    public string PersonName;
    /// <summary>
    /// 人物图片路径
    /// </summary>
    public string PicturePath;
    /// <summary>
    /// 人物图片所在数组中的索引
    /// </summary>
    public int PictureIndex;
    /// <summary>
    /// 人物描述文本的路径
    /// </summary>
    public string DescribeFilePath;
    /// <summary>
    /// 人物的描述
    /// </summary>
    public string Describe;

    /// <summary>
    /// 介绍角色的视频  
    /// </summary>
    public string YearEventVideo;

    /// <summary>
    /// 头像
    /// </summary>
    public Sprite headTex;


}

/// <summary>
/// 公司介绍信息
/// </summary>
public class CompanyInfo
{
    public List<Texture2D> TexInfo;

    public List<string> VideoInfo;
}
