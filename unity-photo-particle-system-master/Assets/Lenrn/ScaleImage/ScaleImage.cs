using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleImage : MonoBehaviour
{


    public RawImage SrcRawImage;

    public RawImage DstRawImage;

    public Texture2D TargeTexture2D;

    public ComputeShader ScaleImageComputeShader;

    /// <summary>
    /// 缩放的宽度倍数
    /// </summary>
    public float WidthScale = 0.5f;
    /// <summary>
    /// 缩放的高度倍数
    /// </summary>
    public float HeightScale = 0.5f;
	// Use this for initialization
	void Start ()
	{

	    Standardization();
	}

    private void Standardization()
    {
        bool isStand = !(WidthScale <= 0f || HeightScale <= 0f);

        if (ScaleImageComputeShader == null || TargeTexture2D == null || DstRawImage == null || SrcRawImage == null)
            isStand = false;

        if (!isStand)
        {
            throw new UnityException("变量数据不符合规范");
        }

    }
	// Update is called once per frame
	void Update () {
		
	}

    public void ScaleImageUserRt()
    {

       

        RenderTexture rtDes = new RenderTexture((int)(TargeTexture2D.width * WidthScale), (int)(TargeTexture2D.height * HeightScale), 24);
        rtDes.enableRandomWrite = true;
        rtDes.Create();


        ////////////////////////////////////////
        //    Compute Shader
        ////////////////////////////////////////
        //1 找到compute shader中所要使用的KernelID
        int k = ScaleImageComputeShader.FindKernel("CSMain");

        ScaleImageComputeShader.SetTexture(k, "Source", TargeTexture2D);
        ScaleImageComputeShader.SetTexture(k, "Dst", rtDes);
        ScaleImageComputeShader.SetFloat( "widthScale", WidthScale);
        ScaleImageComputeShader.SetFloat( "heightScale", HeightScale);
       



        //Debug.Log("tex info width is " + texWidth + "  Height is " + texHeight);
        //3 运行shader  参数1=kid  参数2=线程组在x维度的数量 参数3=线程组在y维度的数量 参数4=线程组在z维度的数量
        ScaleImageComputeShader.Dispatch(k, (int)(TargeTexture2D.width * WidthScale), (int)(TargeTexture2D.height * HeightScale), 1);

        //RenderTexture.active = rtDes;

        //Texture2D jpg = new Texture2D(rtDes.width, rtDes.height, TextureFormat.ARGB32, false);
        //jpg.ReadPixels(new Rect(0, 0, rtDes.width, rtDes.height), 0, 0);
        //RenderTexture.active = null;
        //byte[] bytesEnd = jpg.EncodeToJPG();

        SrcRawImage.texture = TargeTexture2D;
        DstRawImage.texture = rtDes;
        DstRawImage.SetNativeSize();


    }


    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
        {
            ScaleImageUserRt();
        }
    }
}
