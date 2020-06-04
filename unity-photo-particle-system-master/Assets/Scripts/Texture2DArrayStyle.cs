using UnityEngine;
using UnityEngine.Rendering;

public class Texture2DArrayStyle : MonoBehaviour
{
    public MeshRenderer render;
    public Texture2D[] textures;
    public ECopyTexMethpd copyTexMethod;                // 把Texrure2D信息拷贝到Texture2DArray对象中使用的方式 //

    public enum ECopyTexMethpd
    {
        CopyTexture = 0,                                 // 使用 Graphics.CopyTexture 方法 //
        SetPexels = 1,                                      // 使用 Texture2DArray.SetPixels 方法 //
    }

    private Material m_mat;

    public Texture2DArray texArr;

    void Start()
    {
        if (textures == null || textures.Length == 0)
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

         texArr = new Texture2DArray(textures[0].width, textures[0].width, textures.Length, textures[0].format, false, false);

        // 结论 //
        // Graphics.CopyTexture耗时(单位:Tick): 5914, 8092, 6807, 5706, 5993, 5865, 6104, 5780 //
        // Texture2DArray.SetPixels耗时(单位:Tick): 253608, 255041, 225135, 256947, 260036, 295523, 250641, 266044 //
        // Graphics.CopyTexture 明显快于 Texture2DArray.SetPixels 方法 //
        // Texture2DArray.SetPixels 方法的耗时大约是 Graphics.CopyTexture 的50倍左右 //
        // Texture2DArray.SetPixels 耗时的原因是需要把像素数据从cpu传到gpu, 原文: Call Apply to actually upload the changed pixels to the graphics card //
        // 而Graphics.CopyTexture只在gpu端进行操作, 原文: operates on GPU-side data exclusively //

        // using (Timer timer = new Timer(Timer.ETimerLogType.Tick))
        //{
        if (copyTexMethod == ECopyTexMethpd.CopyTexture)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                // 以下两行都可以 //
                //Graphics.CopyTexture(textures[i], 0, texArr, i);
                Graphics.CopyTexture(textures[i], 0, 0, texArr, i, 0);
            }
        }
        else if (copyTexMethod == ECopyTexMethpd.SetPexels)
        {
            for (int i = 0; i < textures.Length; i++)
            {
                // 以下两行都可以 //
                //texArr.SetPixels(textures[i].GetPixels(), i);
                texArr.SetPixels(textures[i].GetPixels(), i, 0);
            }

            texArr.Apply();
        }
        //}

        texArr.wrapMode = TextureWrapMode.Clamp;
        texArr.filterMode = FilterMode.Bilinear;

        m_mat = render.material;

        m_mat.SetTexture("_TexArr", texArr);
        m_mat.SetFloat("_Index", Random.Range(0, textures.Length));

        //AssetDatabase.CreateAsset(texArr, "Assets/RogueX/Prefab/texArray.asset");
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "Change Texture"))
        {
            m_mat.SetFloat("_Index", Random.Range(0, textures.Length));
        }
    }
}