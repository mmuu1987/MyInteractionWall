using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Image = System.Drawing.Image;

public class ScaleImage : MonoBehaviour
{
    public RawImage SrcRawImage;

    public RawImage DstRawImage;
    /// <summary>
    /// 处理前的图像
    /// </summary>
    public Texture2D SourceTexture2D;

    public ComputeShader ScaleImageComputeShader;

    /// <summary>
    /// 源图缩放至目标图的宽度倍数
    /// </summary>
    public float WidthScale = 0.5f;
    /// <summary>
    /// 源图缩放至目标图的高度倍数
    /// </summary>
    public float HeightScale = 0.5f;
    // Use this for initialization
    void Start()
    {

        Standardization();
    }

    private void Standardization()
    {
        bool isStand = !(WidthScale <= 0f || HeightScale <= 0f);

        if (ScaleImageComputeShader == null || SourceTexture2D == null || DstRawImage == null || SrcRawImage == null)
            isStand = false;

        if (!isStand)
        {
            throw new UnityException("变量数据不符合规范");
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void ScaleImageUserRt()
    {
        RenderTexture rtDes = new RenderTexture((int)(SourceTexture2D.width * WidthScale), (int)(SourceTexture2D.height * HeightScale), 24);
        rtDes.enableRandomWrite = true;
        rtDes.Create();
        ////////////////////////////////////////
        //    Compute Shader
        ////////////////////////////////////////
        //1 找到compute shader中所要使用的KernelID
        int k = ScaleImageComputeShader.FindKernel("CSMain");

        ScaleImageComputeShader.SetTexture(k, "Source", SourceTexture2D);
        ScaleImageComputeShader.SetTexture(k, "Dst", rtDes);
        ScaleImageComputeShader.SetFloat("widthScale", WidthScale);
        ScaleImageComputeShader.SetFloat("heightScale", HeightScale);

        //3 运行shader  参数1=kid  参数2=线程组在x维度的数量 参数3=线程组在y维度的数量 参数4=线程组在z维度的数量
        ScaleImageComputeShader.Dispatch(k, (int)(SourceTexture2D.width * WidthScale), (int)(SourceTexture2D.height * HeightScale), 1);

        //cumputeShader gpu那边已经计算完毕。rtDes是gpu计算后的结果
        SrcRawImage.texture = SourceTexture2D;
        DstRawImage.texture = rtDes;
        DstRawImage.SetNativeSize();

        //后续操作，把reDes转为Texture2D  
        //删掉rtDes,SourceTexture2D，我们就得到了所要的目标，并且不产生内存垃圾
    }

    private void LoadTexture()
    {
        string path = Application.streamingAssetsPath + "/test.jpg";

        System.Drawing.Image bitmap = Image.FromFile(path);

        byte[] tempBytes;
        MemoryStream ms = new MemoryStream();



        ImageCodecInfo dc = GetEncoderInfo("image/bmp");
        EncoderParameter myEncoderParameter;
        EncoderParameters myEncoderParameters;
        Encoder myEncoder;
        myEncoder = Encoder.Quality;
        // EncoderParameter object in the array.
        myEncoderParameters = new EncoderParameters(1);
        myEncoderParameter = new EncoderParameter(myEncoder, 100L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        bitmap.Save(ms, dc, myEncoderParameters);


       // bitmap.Save(ms,ImageFormat.Jpeg);


        tempBytes = ms.GetBuffer();//786486
        Debug.Log(tempBytes.Length);

        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGB24, false);

        //tex.LoadImage(tempBytes);
        tex.LoadRawTextureData(tempBytes);

        tex.Apply();





        //byte[] bytes = File.ReadAllBytes(path);//39830

        //Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        //tex.LoadImage(bytes);
        //tex.Apply();
        //Debug.Log(tex.format);
        //byte[] temps = tex.GetRawTextureData();//786,432


        DstRawImage.texture = tex;
    }

    private  ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        int j;
        ImageCodecInfo[] encoders;
        encoders = ImageCodecInfo.GetImageEncoders();

        foreach (ImageCodecInfo encoder in encoders)
        {
            //Debug.Log(encoder.MimeType);
        }
        for (j = 0; j < encoders.Length; ++j)
        {
            if (encoders[j].MimeType == mimeType)
                return encoders[j];
        }
        return null;
    }

    public  void Main1()
    {
        Bitmap myBitmap;
        ImageCodecInfo myImageCodecInfo;
        Encoder myEncoder;
        EncoderParameter myEncoderParameter;
        EncoderParameters myEncoderParameters;

        // Create a Bitmap object based on a BMP file.
        myBitmap = new Bitmap("Shapes.bmp");

        // Get an ImageCodecInfo object that represents the JPEG codec.
        myImageCodecInfo = GetEncoderInfo("image/jpeg");

        // Create an Encoder object based on the GUID

        // for the Quality parameter category.
        myEncoder = Encoder.Quality;

        // Create an EncoderParameters object.

        // An EncoderParameters object has an array of EncoderParameter

        // objects. In this case, there is only one

        // EncoderParameter object in the array.
        myEncoderParameters = new EncoderParameters(1);

        // Save the bitmap as a JPEG file with quality level 25.
        myEncoderParameter = new EncoderParameter(myEncoder, 25L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        myBitmap.Save("Shapes025.jpg", myImageCodecInfo, myEncoderParameters);

        // Save the bitmap as a JPEG file with quality level 50.
        myEncoderParameter = new EncoderParameter(myEncoder, 50L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        myBitmap.Save("Shapes050.jpg", myImageCodecInfo, myEncoderParameters);

        // Save the bitmap as a JPEG file with quality level 75.
        myEncoderParameter = new EncoderParameter(myEncoder, 75L);
        myEncoderParameters.Param[0] = myEncoderParameter;
        myBitmap.Save("Shapes075.jpg", myImageCodecInfo, myEncoderParameters);
    }



    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
        {
            //ScaleImageUserRt();
            LoadTexture();
        }
    }
}
