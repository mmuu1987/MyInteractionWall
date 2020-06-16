using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class LoopMotion : MotionInputMoveBase
{

    /// <summary>
    /// 获取点击到图片的信息
    /// </summary>
    private ComputeBuffer _clickPointBuff;

    protected override void Init()
    {
        base.Init();


        MotionType = MotionType.Loop;

        _screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Z - Camera.main.transform.position.z));
        _screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, Z - Camera.main.transform.position.z));
        _screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, Z - Camera.main.transform.position.z));
        _screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, Z - Camera.main.transform.position.z));


        int HorizontalColumn = TextureInstanced.Instance.HorizontalColumn;

        int InstanceCount = TextureInstanced.Instance.InstanceCount;

        float SizeWidth = TextureInstanced.Instance.SizeWidth;

        float SizeHeight = TextureInstanced.Instance.SizeHeight;

      



        //拿到实际上得到的最大横数，因为 Mathf.ClosestPowerOfTwo(InstanceCount);已经约束为2的N次方
        int maxRow = InstanceCount / HorizontalColumn;

        float rectangleWidth = (HorizontalColumn) * 0.1f + (HorizontalColumn) * SizeWidth;

        float rectangleHeight = maxRow * 0.1f + maxRow * SizeHeight;


        PosAndDir[] datas = new PosAndDir[ComputeBuffer.count];

        ComputeBuffer.GetData(datas);


        int rows = 0;//横列个数    y轴改变
        //使面片排列成阵列
        for (int i = 0; i < InstanceCount; i++)
        {

            if (i != 0 && i % HorizontalColumn == 0)
                rows++;
            int column = i - rows * HorizontalColumn;//竖列个数  x轴概念
            //0.1f，为两面片间的间隙参数
            Vector3 pos = new Vector3(0.1f * column, 0.1f * rows) + new Vector3(SizeWidth * column + SizeWidth / 2, SizeHeight * rows + SizeHeight / 2f);
            //+左下角基准点
            Vector4 temp = new Vector4(pos.x, pos.y, pos.z, 1f) + new Vector4(_screenPosLeftDown.x, _screenPosLeftDown.y, _screenPosLeftDown.z, 0);
            // datas[i].position = temp;
            datas[i].originalPos = datas[i].position;//存储变换前的位置
            datas[i].moveTarget = temp;
            datas[i].moveDir = Vector3.zero;
            // if (i == 0) colors[i] = Color.white;//Color.red;
            //else colors[i] = Random.ColorHSV();

            datas[i].indexRC = new Vector2(rows, column);
            datas[i].uvOffset = new Vector4(1f, 1f, 0f, 0f);
            datas[i].uv2Offset = new Vector4(1f, 1f, 0f, 0f);
            datas[i].picIndex = i % TextureInstanced.Instance.TexArr.depth;
            datas[i].bigIndex = i % TextureInstanced.Instance.TexArr.depth;
            Vector4 posTemp = datas[i].position;
            datas[i].position =new Vector4(posTemp.x,posTemp.y,posTemp.z,1f);
            if (i == 0) _originalPosLeftDown = temp;//保存面片坐标点原点位置

            if (i == HorizontalColumn - 1)//右下角的索引
            {
                //colors[i] = Color.white;//颜色标记
                _originalPosRightDown = temp;
            }
            if (rows + 1 == maxRow && i % HorizontalColumn == 0)//左上角倒数第二排第一个索引
            {
                //colors[i] = Color.white;//颜色标记
                _originalPosLeftUp = temp;
            }
            if (rows + 1 == maxRow && i % HorizontalColumn == HorizontalColumn - 1)//右上角倒数第二排最后一个索引
            {
                // colors[i] = Color.white;//颜色标记
                _originalPosRightUp = temp;
            }
        }

        ComputeBuffer.SetData(datas);

        int stride = Marshal.SizeOf(typeof(PosAndDir));

        //点击缓存
        _clickPointBuff = new ComputeBuffer(1, stride);
        PosAndDir[] clickPoint = { new PosAndDir(-1) };
        _clickPointBuff.SetData(clickPoint);

        ComputeShader.SetBuffer(dispatchID, "positionBuffer", ComputeBuffer);

        ComputeShader.SetBuffer(dispatchID, "clickPointsBuff", _clickPointBuff);

        ComputeShader.SetBuffer(InitID, "positionBuffer", ComputeBuffer);

        ComputeShader.SetFloat("rectangleWidth", rectangleWidth);
        ComputeShader.SetFloat("rectangleHeight", rectangleHeight);
        ComputeShader.SetFloat("MoveSpeed", MoveSpeed);
        TextureInstanced.Instance.ChangeInstanceMat(null);
        TextureInstanced.Instance.CurMaterial.SetVector("_WHScale", new Vector4(1, 1, 1, 1));

    }

    protected override void InitDisPatch(int id)
    {
        float t = TimeTmep / StopTime;
        // Debug.Log(t);
        ComputeShader.SetFloat("deltaTime", t);//初始化运动的时候的插值数值

        base.InitDisPatch(id);

       
    }

    /// <summary>
    /// 点击点
    /// </summary>
    private Vector3 _clickPoint;
    private void MouseButtonDownAction()
    {

        _clickPoint = Vector3.one * 1000000;//不让其再次触发

        if (InputManager.Instance.GetMouseButtonDown(1))
        {
            //Debug.Log("鼠标右键按下 " + Input.mousePosition);

            _clickPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Z - Camera.main.transform.position.z));
        }

        ComputeShader.SetVector("clickPoint", _clickPoint);

      

    }

    protected override void Dispatch(ComputeBuffer system)
    {
        MouseButtonDownAction();
        base.Dispatch(system);



        if (_clickPoint.z < 1000000)//相当于有点击事件才触发
        {
            PosAndDir[] datas = new PosAndDir[1];
            _clickPointBuff.GetData(datas);
            Debug.Log(datas[0].picIndex);
        }
       
    }
}
