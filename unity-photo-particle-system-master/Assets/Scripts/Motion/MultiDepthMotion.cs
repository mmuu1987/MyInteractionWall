using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct DepthInfo
{
    /// <summary>
    /// 本来的深度
    /// </summary>
    public int originalDepth;
    /// <summary>
    /// 移动到的深度
    /// </summary>
    public float toDepth;
    /// <summary>
    /// 深度距离
    /// </summary>
    public float handleDepth;
    /// <summary>
    /// originalDepth深度下的缩放
    /// </summary>
    public float scale;

    public DepthInfo(int d,float o,float h,float s)
    {
        this.originalDepth = d;
        this.toDepth = o;
        this.handleDepth = h;
        scale = s;
    }

};


/// <summary>
/// 多层深度的面片运动
/// </summary>
public class MultiDepthMotion : MotionInputMoveBase
{

    private DepthInfo[] _depths;

    private ComputeBuffer depthBuffer;
    protected override void Init()
    {
        base.Init();

        int instanceCount = TextureInstanced.Instance.InstanceCount;

        PosAndDir[] datas = new PosAndDir[ComputeBuffer.count];

        ComputeBuffer.GetData(datas);

        //写死5个深度，每个深度一千个面片，这个类的运动，我们只要5000就可以了
        //每个深度距离相差10个单位  
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
         depthBuffer = new ComputeBuffer(5, stride);
         _depths = new DepthInfo[5];
        int k = 0;
        float z = 20;
      
        for (int j = 0; j < newData.Length; j++)
        {
           

            if (j % 1000 == 0)
            {
                k++;
                float tempZ = k*z;

                _screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, tempZ - Camera.main.transform.position.z));
                _screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, tempZ - Camera.main.transform.position.z));
                _screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, tempZ - Camera.main.transform.position.z));
                _screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, tempZ - Camera.main.transform.position.z));

                
               

                float s = 1f;//不同层次的给定不同的缩放  
                if (k == 0 || k == 1) s = 1f;
                else if (k == 2) s = 1.25f;
                else if (k == 3) s = 1.5f;
                else if (k == 4) s = 1.75f;
                _depths[k - 1] = new DepthInfo(k - 1, tempZ - 5, tempZ - 5, s);
                
            }


            int index = j ;



            float rangeX = Random.Range(_screenPosLeftDown.x , _screenPosRightDown.x );//随机x轴位置
            float rangeY = Random.Range(_screenPosLeftDown.y, _screenPosLeftUp.y);
            float rangeZ = Random.Range(-0.1f, 0.1f);//在同一层次，再随机不同的深度位置，不至于重叠一起，显得错落有致


            Vector4 posTemp = newData[index].position;

            float scale = 1f;//不同层次的给定不同的缩放  
            if (k == 1 || k == 2) scale = 1f;
            else if (k == 3) scale = 1.25f;
            else if (k == 4) scale = 2f;
            else if (k == 5) scale = 2.5f;
            else if (k == 6) scale = 4f;

            newData[index].position = new Vector4(posTemp.x, posTemp.y, posTemp.z, scale);


            newData[index].moveTarget = new Vector3(rangeX, rangeY, rangeZ);
            newData[index].uvOffset = new Vector4(1f, 1f, 0f, 0f);
            newData[index].uv2Offset = new Vector4(1f, 1f, 0f, 0f);
            newData[index].picIndex = index % TextureInstanced.Instance.textures.Count;
            newData[index].bigIndex = index % TextureInstanced.Instance.textures.Count;
            newData[index].velocity = new Vector4(k-1, 0, 0, 0);//x存储层次深度
        }

        TextureInstanced.Instance.InstanceMaterial.SetVector("_WHScale", new Vector4(1f, 1f, 1f, 1f));


        allDataList.AddRange(newData);

        ComputeBuffer.SetData(allDataList.ToArray());
        depthBuffer.SetData(_depths);

        ComputeShader.SetBuffer(dispatchID, "depthBuffer", depthBuffer);
        ComputeShader.SetBuffer(dispatchID, "positionBuffer", ComputeBuffer);
        ComputeShader.SetBuffer(InitID, "positionBuffer", ComputeBuffer);
        MoveSpeed = 50f;//更改更快的插值速度
        ComputeShader.SetFloat("MoveSpeed", MoveSpeed);

        InitDisPatch(InitID);


    }

    /// <summary>
    /// 调换层次深度
    /// </summary>
    private void SetDepth(int form,int to)
    {
        float z1 = _depths[form].handleDepth;

        float z2 = _depths[to].handleDepth;

        _depths[to].handleDepth = z1;

        _depths[form].handleDepth = z2;

        depthBuffer.SetData(_depths);

        ComputeShader.SetBuffer(dispatchID, "depthBuffer", depthBuffer);
    }
    protected override void Dispatch(ComputeBuffer system)
    {

        ComputeShader.SetFloat("deltaTime", Time.deltaTime);

        Dispatch(dispatchID, system);

    }

    public override void ExitMotion()
    {
        base.ExitMotion();
        depthBuffer.Release();
        depthBuffer = null;
    }



    public  void ChangeState()
    {
        SetDepth(3, 4);
    }
}
