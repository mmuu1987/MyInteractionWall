using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多层深度的面片运动
/// </summary>
public class MultiDepthMotion : MotionInputMoveBase
{

    protected override void Init()
    {
        base.Init();

        //_screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Z - Camera.main.transform.position.z));
        //_screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, Z - Camera.main.transform.position.z));
        //_screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, Z - Camera.main.transform.position.z));
        //_screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, Z - Camera.main.transform.position.z));

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

        int k = 1;
        float z = -5;
        for (int j = 1; j <= newData.Length; j++)
        {


            _screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z - Camera.main.transform.position.z));
            _screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, z - Camera.main.transform.position.z));
            _screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, z - Camera.main.transform.position.z));
            _screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, z - Camera.main.transform.position.z));

            if (j%1000 == 0)
            {
                k++;
                z = k * 0.5f+z;//每个层级的深度
            }


            int index = j - 1;



            float rangeX = Random.Range(_screenPosLeftDown.x*50, _screenPosRightDown.x*50);//随机x轴位置
            float rangeY = Random.Range(_screenPosLeftDown.y, _screenPosLeftUp.y);



            Vector4 posTemp = newData[index].position;
            newData[index].position = new Vector4(posTemp.x, posTemp.y, posTemp.z, 1f);
            newData[index].moveTarget = new Vector3(rangeX, rangeY,  z );
            newData[index].uvOffset = new Vector4(1f, 1f, 0f, 0f);
            newData[index].uv2Offset = new Vector4(1f, 1f, 0f, 0f);
            newData[index].picIndex = index % TextureInstanced.Instance.textures.Count;
            newData[index].bigIndex = index % TextureInstanced.Instance.textures.Count;
        }

        TextureInstanced.Instance.InstanceMaterial.SetVector("_WHScale", new Vector4(1f, 1f, 1f, 1f));


        allDataList.AddRange(newData);

        ComputeBuffer.SetData(allDataList.ToArray());
        ComputeShader.SetBuffer(dispatchID, "positionBuffer", ComputeBuffer);
        ComputeShader.SetBuffer(InitID, "positionBuffer", ComputeBuffer);
        MoveSpeed = 50f;//更改更快的插值速度
        ComputeShader.SetFloat("MoveSpeed", MoveSpeed);

        InitDisPatch(InitID);


    }

    protected override void Dispatch(ComputeBuffer system)
    {

        Vector3 movePos = new Vector3(_delta.x, _delta.y, 0) * 0.01f;

        ComputeShader.SetVector("movePos", movePos);
        ComputeShader.SetFloat("Z", Z);
        ComputeShader.SetInt("actionState", _actionState);
        ComputeShader.SetFloat("deltaTime", Time.deltaTime);
        ComputeShader.SetVector("moveDir", _moveDir);
        ComputeShader.SetVector("originPos", _originalPosLeftDown);

        Dispatch(dispatchID, system);

    }

}
