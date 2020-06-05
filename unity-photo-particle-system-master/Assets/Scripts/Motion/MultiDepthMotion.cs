using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct DepthInfo
{
    /// <summary>
    /// 每层的索引
    /// </summary>
    public int originalDepth;
    /// <summary>
    /// 原始的深度距离，原则是不能随便改写
    /// </summary>
    public float toDepth;
    /// <summary>
    /// 原始的缩放系数，原则是不能随便改写
    /// </summary>
    public float originalScal;
    /// <summary>
    /// 可以调整的深度距离，可随意改写
    /// </summary>
    public float handleDepth;
    /// <summary>
    /// 深度下的缩放，根据handleDepth的变动而变动
    /// </summary>
    public float scale;
    /// <summary>
    /// 该层次下的透明度
    /// </summary>
    public float alpha;

    public DepthInfo(int d, float o, float s,float a)
    {
        this.originalDepth = d;
        this.toDepth = o;
        this.handleDepth = o;
        scale = s;
        originalScal = s;
        alpha = a;
    }

};


/// <summary>
/// 多层深度的面片运动
/// </summary>
public class MultiDepthMotion : MotionInputMoveBase
{

    private DepthInfo[] _depths;

    private ComputeBuffer _depthBuffer;

    public Material CurMaterial;
    protected override void Init()
    {
        base.Init();

        PosAndDir[] datas = new PosAndDir[ComputeBuffer.count];

        ComputeBuffer.GetData(datas);

        //写死5个深度，每个深度一千个面片，这个类的运动，我们只要5000就可以了
        
        List<PosAndDir> temp = new List<PosAndDir>();
        List<PosAndDir> allDataList = new List<PosAndDir>();

        //这里注意，posAndDir属于结构体，值拷贝
        for (int i = 0; i < datas.Length; i++)
        {
            PosAndDir data = datas[i];
            if (i < 5000)
            {
                temp.Add(data);//编号一样的单独拿出来
            }
            else
            {
                data.position = Vector4.zero;//其他编号都隐藏
                allDataList.Add(data);
            }

        }

        PosAndDir[] newData = temp.ToArray();
        int stride = Marshal.SizeOf(typeof(DepthInfo));
        _depthBuffer = new ComputeBuffer(5, stride);
        _depths = new DepthInfo[5];
        int k = 0;
        float z = 2;

        for (int j = 0; j < newData.Length; j++)
        {


            if (j % 1000 == 0)
            {
                k++;
                float tempZ = k * z;

                _screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, tempZ - Camera.main.transform.position.z));
                _screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, tempZ - Camera.main.transform.position.z));
                _screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, tempZ - Camera.main.transform.position.z));
                _screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, tempZ - Camera.main.transform.position.z));



                var tempZZZ = tempZ - 8;
                float s = 1f;//不同层次的给定不同的缩放  
                float a = 1f;//不同层次的给定不同的透明度
                if (k == 1)
                {
                    s = 1f;
                    a = 1f;
                }
                else if (k == 2)
                {
                    s = 1.25f;
                    a = 0.75f;
                }
                else if (k == 3)
                {
                    s = 1.5f;
                    a = 0.5f;
                }
                else if (k == 4)
                {
                    s = 1.75f;
                    a = 0.25f;
                }
                else if (k == 5)
                {
                    s = 2f;
                    a = 0.1f;
                }
                _depths[k - 1] = new DepthInfo(k - 1, tempZZZ, s,a);



            }


            int index = j;



            float rangeX = Random.Range(_screenPosLeftDown.x * 20f, _screenPosRightDown.x * 20f);//随机x轴位置
            float rangeY = Random.Range(_screenPosLeftDown.y * 0.75f, _screenPosLeftUp.y * 0.75f);
            float rangeZ = Random.Range(-0.3f, 0.3f);//在同一层次，再随机不同的深度位置，不至于重叠一起，显得错落有致


            Vector4 posTemp = newData[index].position;
            newData[index].position = new Vector4(posTemp.x, posTemp.y, posTemp.z, 1);


            newData[index].moveTarget = new Vector3(rangeX, rangeY, rangeZ);
            newData[index].uvOffset = new Vector4(1f, 1f, 0f, 0f);
            newData[index].uv2Offset = new Vector4(1f, 1f, 0f, 0f);
            newData[index].picIndex = index % TextureInstanced.Instance.textures.Count;
            newData[index].bigIndex = index % TextureInstanced.Instance.textures.Count;
            newData[index].velocity = new Vector4(k - 1, 1f, 0, 0);//x存储层次的索引
        }
        TextureInstanced.Instance.ChangeInstanceMat(CurMaterial);
        TextureInstanced.Instance.CurMaterial.SetVector("_WHScale", new Vector4(1f, 1f, 1f, 1f));


        TextureInstanced.Instance.CurMaterial.SetBuffer("positionBuffer", ComputeBuffer);
        TextureInstanced.Instance.CurMaterial.SetTexture("_TexArr", TextureInstanced.Instance.texArr);

        allDataList.AddRange(newData);

        ComputeBuffer.SetData(allDataList.ToArray());
        _depthBuffer.SetData(_depths);

        ComputeShader.SetBuffer(dispatchID, "depthBuffer", _depthBuffer);
        ComputeShader.SetBuffer(dispatchID, "positionBuffer", ComputeBuffer);
        ComputeShader.SetBuffer(InitID, "positionBuffer", ComputeBuffer);
        MoveSpeed = 50f;//更改更快的插值速度
        ComputeShader.SetFloat("MoveSpeed", MoveSpeed);

        InitDisPatch(InitID);


    }

    /// <summary>
    /// 当前在最前面的深度，默认是0
    /// </summary>
    private int _curDepth=0;

    /// <summary>
    ///  调换层次位置
    /// </summary>
    /// <param name="top">需要切换到最前的层次</param>
    private void SetDepth(int top)
    {
        if (_curDepth == top) return;
        if (top > 5) return;

        float z1 = _depths[0].toDepth;
        float s1 = _depths[0].originalScal;
        float a1 = 1f;//在最前的透明度默认为1


        float z2 = _depths[top].handleDepth;
        float s2 = _depths[top].scale;
        float a2 = _depths[top].alpha;

        

        _depths[top].handleDepth = z1;
        _depths[top].scale = s1;
        _depths[top].alpha = a1;

        _depths[_curDepth].handleDepth = z2;
        _depths[_curDepth].scale = s2;
        _depths[_curDepth].alpha = a2;

        _curDepth = top;//重新得到当前在最前面的深度

        _depthBuffer.SetData(_depths);

        ComputeShader.SetBuffer(dispatchID, "depthBuffer", _depthBuffer);
    }
    protected override void Dispatch(ComputeBuffer system)
    {

        ComputeShader.SetFloat("deltaTime", Time.deltaTime);

        Dispatch(dispatchID, system);

    }

    public override void ExitMotion()
    {
        base.ExitMotion();
        if (_depthBuffer!=null)
        _depthBuffer.Release();
        _depthBuffer = null;

    }


    public int Depth=2;
    public void ChangeState()
    {
        SetDepth(Depth);
    }
}
