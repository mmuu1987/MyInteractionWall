using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 传递给GPU的结构体，在不同的运动类型，变量的意义有些不一样
/// </summary>
public struct PosAndDir
{
    public Vector4 position;
    /// <summary>
    /// 一般指速度，在不同的运动类有不同的意义
    /// </summary>
    public Vector4 velocity;
    /// <summary>
    /// 物体初始速度
    /// </summary>
    public Vector3 initialVelocity;
    /// <summary>
    /// 初始状态的位置
    /// </summary>
    public Vector4 originalPos;

    /// <summary>
    /// 移动到的目标点
    /// </summary>
    public Vector3 moveTarget;

    /// <summary>
    /// 粒子靠这个向量来自动移动
    /// </summary>
    public Vector3 moveDir;

    /// <summary>
    /// 所在的行和列的位置
    /// </summary>
    public Vector2 indexRC;

    /// <summary>
    /// 索要表现的贴图
    /// </summary>
    public int picIndex;

    /// <summary>
    /// 显示图片局部的index
    /// </summary>
    public int bigIndex;
    /// <summary>
    /// 第一套 UV加UV偏移
    /// </summary>
    public Vector4 uvOffset;
    /// <summary>
    /// 第二套UV加UV偏移
    /// </summary>
    public Vector4 uv2Offset;

    public PosAndDir(int id)
    {
        position = new Vector4();


        velocity = new Vector3();
        initialVelocity = new Vector3();
        originalPos = new Vector4();
        moveTarget = new Vector3();
        moveDir = new Vector3();
        indexRC = new Vector2();

        picIndex = id;
        bigIndex = 1;
        uvOffset = new Vector4();

        uv2Offset = new Vector4();
    }
}



/// <summary>
/// 运动类型
/// </summary>
public enum MotionType
{
    None,
    Wall,
    /// <summary>
    /// 立方体
    /// </summary>
    Cube,
    /// <summary>
    /// 水平循环左或右运动
    /// </summary>
    Loop,
    /// <summary>
    /// 分类运动
    /// </summary>
    ClassiFicationMotion,
    /// <summary>
    /// z轴不同的运动
    /// </summary>
    MultiDepth
}
/// <summary>
/// This demo shows the use of Compute Shaders to update the object's
/// positions. The buffer is stored and updated directly in GPU.
/// </summary>
public class TextureInstanced : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public int InstanceCount = 100000;

    public Mesh InstanceMesh;
    public Material InstanceMaterial;
    public MotionType _type;
    /// <summary>
    /// 每一列的元素个数
    /// </summary>
    private int _column;

    public ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer colorBuffer;
    private ComputeBuffer boundaryBuffer;



    /// <summary>
    /// 图片范围的长
    /// </summary>
    public int Width;
    /// <summary>
    /// 图片范围的高
    /// </summary>
    public int Height;

    /// <summary>
    /// 横列，一横有多少个数
    /// </summary>
    public int HorizontalColumn = 10;
    /// <summary>
    /// 竖列，一竖有多少个数
    /// </summary>
    public int VerticalColumn = 10;

    /// <summary>
    /// 面片的长
    /// </summary>
    public float SizeWidth = 1;
    /// <summary>
    /// 面片的高
    /// </summary>
    public float SizeHeight = 1f;


    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    /// <summary>
    /// 图片距离相机的距离
    /// </summary>
    public float Z = 10;

    public Transform tipLD;
    public Transform tipLU;
    public Transform tipRD;
    public Transform tipRU;


    public Texture2DArray texArr;

    // public Texture2D[] textures;
    public List<Texture> textures;
    public Texture2DArrayStyle.ECopyTexMethpd copyTexMethod;

    public CubeMotion CubeMotion;

    public LoopMotion LoopMotion;

    public MultiDepthMotion MultiDepthMotion;

    public ClassiFicationMotion ClassiFicationMotion;

    public static TextureInstanced Instance;


    public RawImage MoveTexture;

    /// <summary>
    /// 当前实例渲染的材质
    /// </summary>
    public Material CurMaterial { get; private set; }

    private void Awake()
    {
        if (Instance != null) throw new UnityException("已经有单例了，不能重复赋值");

        Instance = this;

    }

    void Start()
    {

        InstanceCount = HorizontalColumn * VerticalColumn;
        CurMaterial = InstanceMaterial;
        HandleTextureArry();

        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);

        CreateBuffers();


       

        // StartCoroutine(LoadVideo(path));
    }


    void Update()
    {
        InputManager.Instance.HandleInput();
        // UpdateBuffers();
        UpdateBuffers(_type);
        // Render
        Graphics.DrawMeshInstancedIndirect(InstanceMesh, 0, CurMaterial, InstanceMesh.bounds, argsBuffer, 0, null, ShadowCastingMode.Off, false);
    }

    /// <summary>
    /// 用不同的材质渲染实例对象
    /// </summary>
    public void ChangeInstanceMat(Material mat)
    {
        if (mat != null)
        {
            CurMaterial = mat;
        }
        else //如果为null，默认自带的材质
        {
            CurMaterial = InstanceMaterial;
        }
    }

    void UpdateCubeBuffers()
    {
        LoopMotion.ExitMotion();
        ClassiFicationMotion.ExitMotion();
       
        MultiDepthMotion.ExitMotion();
        CubeMotion.StartMotion(this);

    }

    void UpdateLoop()
    {
        CubeMotion.ExitMotion();
        ClassiFicationMotion.ExitMotion();
        MultiDepthMotion.ExitMotion();
        LoopMotion.StartMotion(this);
    }

    void UpdateClassiFicationMotion()
    {
        CubeMotion.ExitMotion();
        LoopMotion.ExitMotion();
        MultiDepthMotion.ExitMotion();
        ClassiFicationMotion.StartMotion(this);
    }
    void UpdateMultiDepthMotion()
    {
        CubeMotion.ExitMotion();
        LoopMotion.ExitMotion();
        ClassiFicationMotion.ExitMotion();
        MultiDepthMotion.StartMotion(this);
        
    }

    public void CubeType()
    {
        _type = MotionType.Cube;
    }

    public void LoopType()
    {
        _type = MotionType.Loop;
    }
    void UpdateBuffers(MotionType type)
    {

        switch (type)
        {
            case MotionType.None:
                break;
            case MotionType.Wall:
                //UpdateWallBuffers();
                break;
            case MotionType.Cube:
                UpdateCubeBuffers();
                break;
            case MotionType.Loop:
                UpdateLoop();
                break;
            case MotionType.ClassiFicationMotion:
                UpdateClassiFicationMotion();
                break;
            case MotionType.MultiDepth:
                UpdateMultiDepthMotion();
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }


    }

    void CreateBuffers()
    {
        if (InstanceCount < 1) InstanceCount = 1;

        if (_column < 100) _column = 100;

        InstanceCount = Mathf.ClosestPowerOfTwo(InstanceCount);


        InstanceMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        // Positions & Colors
        if (positionBuffer != null) positionBuffer.Release();
        if (colorBuffer != null) colorBuffer.Release();
        if (boundaryBuffer != null) boundaryBuffer.Release();


        int stride = Marshal.SizeOf(typeof(PosAndDir));
        //Debug.Log("stride byte size is " + stride);
        positionBuffer = new ComputeBuffer(InstanceCount, stride);//16

        colorBuffer = new ComputeBuffer(InstanceCount, 16);
        int boundbuff = Marshal.SizeOf(typeof(Vector4));
        boundaryBuffer = new ComputeBuffer(4, boundbuff);

        Vector4[] colors = new Vector4[InstanceCount];
        PosAndDir[] posDirs = new PosAndDir[InstanceCount];


        for (int i = 0; i < InstanceCount; i++)
        {
            posDirs[i].position = Vector4.one;
            posDirs[i].picIndex = i % textures.Count;
        }

        colorBuffer.SetData(colors);
        positionBuffer.SetData(posDirs);



        CurMaterial.SetBuffer("positionBuffer", positionBuffer);
      //  CurMaterial.SetBuffer("colorBuffer", colorBuffer);


        // indirect args
        uint numIndices = (InstanceMesh != null) ? InstanceMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)InstanceCount;
        argsBuffer.SetData(args);
    }

    private void HandleTextureArry()
    {

        if (textures == null || textures.Count == 0)
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
        // Texture tx = new Texture();
        texArr = new Texture2DArray(textures[0].width, textures[0].width, textures.Count, TextureFormat.DXT1, false, false);

        // 结论 //
        // Graphics.CopyTexture耗时(单位:Tick): 5914, 8092, 6807, 5706, 5993, 5865, 6104, 5780 //
        // Texture2DArray.SetPixels耗时(单位:Tick): 253608, 255041, 225135, 256947, 260036, 295523, 250641, 266044 //
        // Graphics.CopyTexture 明显快于 Texture2DArray.SetPixels 方法 //
        // Texture2DArray.SetPixels 方法的耗时大约是 Graphics.CopyTexture 的50倍左右 //
        // Texture2DArray.SetPixels 耗时的原因是需要把像素数据从cpu传到gpu, 原文: Call Apply to actually upload the changed pixels to the graphics card //
        // 而Graphics.CopyTexture只在gpu端进行操作, 原文: operates on GPU-side data exclusively //

        // using (Timer timer = new Timer(Timer.ETimerLogType.Tick))
        //{
        if (copyTexMethod == Texture2DArrayStyle.ECopyTexMethpd.CopyTexture)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                // 以下两行都可以 //
                //Graphics.CopyTexture(textures[i], 0, texArr, i);
                Graphics.CopyTexture(textures[i], 0, 0, texArr, i, 0);

            }
        }
        else if (copyTexMethod == Texture2DArrayStyle.ECopyTexMethpd.SetPexels)
        {
            //for (int i = 0; i < textures.Count; i++)
            //{
            //    // 以下两行都可以 //
            //    //texArr.SetPixels(textures[i].GetPixels(), i);
            //    texArr.SetPixels(textures[i].GetPixels(), i, 0);
            //}

            //texArr.Apply();
        }
        //}

        texArr.wrapMode = TextureWrapMode.Clamp;
        texArr.filterMode = FilterMode.Bilinear;



        CurMaterial.SetTexture("_TexArr", texArr);
        //m_mat.SetFloat("_Index", Random.Range(0, textures.Length));

        //AssetDatabase.CreateAsset(texArr, "Assets/RogueX/Prefab/texArray.asset");
    }
    void OnDisable()
    {
        if (positionBuffer != null) positionBuffer.Release();
        positionBuffer = null;

        if (colorBuffer != null) colorBuffer.Release();
        colorBuffer = null;

        if (argsBuffer != null) argsBuffer.Release();
        argsBuffer = null;

        if (boundaryBuffer != null) boundaryBuffer.Release();
        boundaryBuffer = null;


    }
    private Vector2 _delta;
    public void OnDrag(PointerEventData eventData)
    {
        _delta = eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _delta = Vector2.zero;
    }


    private IEnumerator LoadVideo(string url)
    {
        WWW www = new WWW("file://" + url);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            MovieTexture movie = www.GetMovieTexture();

            MoveTexture.texture = movie;

            movie.Stop();

            movie.Play();
            // textures.Clear();
            // textures.Add(movie);
            //HandleTextureArry();
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "test"))
        {

           ClassiFicationMotion. ChangeState(1);
        }
        if (GUI.Button(new Rect(100f, 0f, 100f, 100f), "test2"))
        {

            MultiDepthMotion.ChangeState();
        }
    }

}
