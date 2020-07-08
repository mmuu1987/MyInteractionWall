using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class SetTexColor_1 : MonoBehaviour
{
    public Material mat;
    public ComputeShader shader;

    public int BorderWidth = 50;//边框像素单位宽度

    public int LableHeight = 50;//文字占有高度像素单位

  

    public Transform Image;

    public Texture2D HandleTex;
    /// <summary>
    /// 年份贴图
    /// </summary>
    public Texture2D YearTex;
    void Start()
    {
        RunShader(HandleTex, YearTex);
    }

    /// <summary>
    /// 为外置文件夹的图片添加边框
    /// </summary>
    private void AddOutLine()
    {
        
    }
    void RunShader(Texture2D sourceTex,Texture2D yearTex)
    {

        int texWidth = sourceTex.width + 2 * BorderWidth;
        int texHeight = sourceTex.height + BorderWidth + LableHeight;
        ////////////////////////////////////////
        //    RenderTexture
        ////////////////////////////////////////
        //1 新建RenderTexture
        RenderTexture rt = new RenderTexture(texWidth, texHeight, 24);
        //2 开启随机写入
        rt.enableRandomWrite = true;
        //3 创建RenderTexture
        rt.Create();
        if(mat!=null)
        //4 赋予材质
        mat.mainTexture = rt;
        ////////////////////////////////////////
        //    Compute Shader
        ////////////////////////////////////////
        //1 找到compute shader中所要使用的KernelID
        int k = shader.FindKernel("CSMain1");
        //2 设置贴图    参数1=kid  参数2=shader中对应的buffer名 参数3=对应的texture, 如果要写入贴图，贴图必须是RenderTexture并enableRandomWrite
        shader.SetTexture(k, "Result", rt);
        shader.SetTexture(k, "Source", sourceTex);
        shader.SetTexture(k, "YearTex", yearTex);

        shader.SetInt("BorderWidth", BorderWidth);
        shader.SetInt("LableHeight", LableHeight);
      

        Debug.Log("tex info width is " + texWidth +"  Height is " +texHeight);
        //3 运行shader  参数1=kid  参数2=线程组在x维度的数量 参数3=线程组在y维度的数量 参数4=线程组在z维度的数量
        shader.Dispatch(k, texWidth, texHeight, 1);

        float scale = texWidth*1f/texHeight;

        if(Image!=null)
        Image.localScale = new Vector3(scale,1,1);
       // SaveRenderTextureToJpg(rt, contents, pngName);
    }

     //将RenderTexture保存成一张png图片  
    public Texture2D SaveRenderTextureToJpg(RenderTexture rt, string contents, string pngName)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToJPG();

        File.WriteAllBytes(contents + "/AddOutLine_" + pngName + ".png", bytes);
        //同时，删掉旧贴图
        File.Delete(contents + "/" + pngName + ".png");
       
        RenderTexture.active = prev;

        rt.Release();
        return png;

    }  

}