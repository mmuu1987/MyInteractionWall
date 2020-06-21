using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


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



    public DepthInfo(int d, float o, float s, float a)
    {
        this.originalDepth = d;
        this.toDepth = o;
        this.handleDepth = o;
        scale = s;
        originalScal = s;
        alpha = a;
    }

};

public class ClickData
{
    public Vector3 Position;

    public float ClickTime;
}
/// <summary>
/// 多层深度的面片运动
/// </summary>
public class MultiDepthMotion : MotionInputMoveBase
{

    private DepthInfo[] _depths;

    private ComputeBuffer _depthBuffer;

    public Material CurMaterial;


    public Canvas Canvas;


    public int Depth = 2;
    private int _zeroIndexCount;


    /// <summary>
    /// 点击点 
    /// </summary>
    private Vector3 _clickPoint;

    /// <summary>
    /// 触摸互动吸引的点击buff
    /// </summary>
    private ComputeBuffer _clickBuff;

    /// <summary>
    /// 获取点击到图片的信息
    /// </summary>
    private ComputeBuffer _clickPointBuff;

    /// <summary>
    /// 触摸数据int为id,vector,4为屏幕位置，加 点击的 时间点
    /// </summary>
    private Dictionary<int, ClickData> _touchIds;
    protected override void Init()
    {
        base.Init();

        MotionType = MotionType.MultiDepth;
        // Camera.main.fieldOfView = 30f;

        _touchIds = new Dictionary<int, ClickData>();
        PosAndDir[] datas = new PosAndDir[ComputeBuffer.count];

        ComputeBuffer.GetData(datas);

        List<PosAndDir> temp = new List<PosAndDir>();
        List<PosAndDir> allDataList = new List<PosAndDir>();



        //这里注意，posAndDir属于结构体，值拷贝
        for (int i = 0; i < datas.Length; i++)
        {
            PosAndDir data = datas[i];
            if (i < Common.PictureCount*3)
            {
                temp.Add(data);//编号一样的单独拿出来
            }
            else
            {
                data.position = Vector4.zero;//其他编号都隐藏
                data.originalPos = Vector4.one;
                allDataList.Add(data);
            }

        }
        _zeroIndexCount = allDataList.Count;
        PosAndDir[] newData = temp.ToArray();
        int stride = Marshal.SizeOf(typeof(DepthInfo));
        _depthBuffer = new ComputeBuffer(3, stride);
        _depths = new DepthInfo[3];

        //点击缓存
        _clickPointBuff = new ComputeBuffer(1, Marshal.SizeOf(typeof(PosAndDir)));
        PosAndDir[] clickPoint = { new PosAndDir(-1) };
        _clickPointBuff.SetData(clickPoint);

        int k = 0;
        float z = 2;
        float scaleY = 1;//y轴位置屏幕有内容的比率
        float delay = 0f;
        List<Vector2> randomPos = new List<Vector2>();

        for (int j = 0; j < newData.Length; j++)
        {


            if (j % Common.PictureCount == 0)
            {
                k++;
                float tempZ = k * z - 6;

                _screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, tempZ - Camera.main.transform.position.z));
                _screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, tempZ - Camera.main.transform.position.z));
                _screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, tempZ - Camera.main.transform.position.z));
                _screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, tempZ - Camera.main.transform.position.z));




                float s = 1f;//不同层次的给定不同的缩放  
                float a = 1f;//不同层次的给定不同的透明度
                if (k == 1)
                {
                    s = 1f;
                    a = 1f;
                    scaleY = 0.8f;
                }
                else if (k == 2)
                {
                    s = 1.2f;
                    a = 0.35f;
                    scaleY = 0.6f;
                }
                else if (k == 3)
                {
                    s = 1.4f;
                    a = 0.15f;
                    scaleY = 0.5f;
                }
                else if (k == 4)
                {
                    s = 1.6f;
                    a = 0.45f;
                    scaleY = 0.7f;
                }
                else if (k == 5)
                {
                    s = 1.8f;
                    a = 0.25f;
                    scaleY = 0.6f;
                }
                randomPos = Common.Sample2D((_screenPosRightDown.x - _screenPosLeftDown.x) *4, (_screenPosLeftUp.y - _screenPosLeftDown.y) * scaleY, s+0.75f, 25);
                Debug.Log("randomPos count is  " + randomPos.Count + " 层级为=> " + (k - 1));
                _depths[k - 1] = new DepthInfo(k - 1, tempZ, s, a);
            }



            float rangeZ = Random.Range(-0.2f, 0.2f);//在同一层次，再随机不同的深度位置，不至于重叠一起，显得错落有致


            Vector4 posTemp = newData[j].position;
            newData[j].position = new Vector4(posTemp.x, posTemp.y, posTemp.z, 1);


            Vector2 randomPoint;

            if (randomPos.Count > 0)
            {
             int rangIndex = Random.Range(0, randomPos.Count);

             randomPoint = randomPos[rangIndex];

             randomPos.RemoveAt(rangIndex);
            }
            else//如果多出来，则放到看不到的地方
            {
                randomPoint = new Vector2(_screenPosLeftDown.x,Random.Range(1000f,2000f));
            }
           

           // Vector2 randomPoint = randomPos[j % randomPos.Count];

            //计算Y轴的位置(1 - scaleY)为空余的位置(1 - scaleY)/2f上下空余的位置，(1 - scaleY)/2f*(_screenPosLeftUp.y - _screenPosLeftDown.y)空余位置的距离
            float heightTmep = (1 - scaleY)/2f*(_screenPosLeftUp.y - _screenPosLeftDown.y);

            randomPoint = new Vector2(randomPoint.x + _screenPosLeftDown.x, randomPoint.y + _screenPosLeftDown.y + heightTmep);

            newData[j].moveTarget = new Vector3(randomPoint.x, randomPoint.y, rangeZ);
            newData[j].uvOffset = new Vector4(1f, 1f, 0f, 0f);
            newData[j].uv2Offset = new Vector4(1f, 1f, 0f, 0f);

            int picIndex = 0;
            Vector2 size = PictureHandle.Instance.GetLevelIndexSize(j, k - 1, out picIndex);//得到缩放尺寸

            float xScale = size.x/512f;
            float yScale = size.y/512f;
            float proportion = size.x/size.y;
            if (xScale >= 2 || yScale >= 2)
            {
                //如果超过2倍大小，则强制缩放到一倍大小以内，并以宽度为准，等比例减少
                int a = (int)xScale;
                xScale = xScale - (a) + 2f;

                yScale = xScale / proportion;
            }
            

            newData[j].initialVelocity = new Vector3(xScale, yScale, 0f);//填充真实宽高
            newData[j].picIndex = picIndex;
            newData[j].bigIndex = picIndex;
            //x存储层次的索引,y存储透明度,   z存储，x轴右边的边界值，为正数   
            newData[j].velocity = new Vector4(k - 1, 1f, _screenPosRightDown.x * 4f, 0);
            Vector4 otherData = new Vector4();
            newData[j].originalPos = otherData;
        }
        TextureInstanced.Instance.ChangeInstanceMat(CurMaterial);
        TextureInstanced.Instance.CurMaterial.SetVector("_WHScale", new Vector4(1f, 1f, 1f, 1f));




        allDataList.AddRange(newData);

        ComputeBuffer.SetData(allDataList.ToArray());
        _depthBuffer.SetData(_depths);

        ComputeShader.SetBuffer(dispatchID, "depthBuffer", _depthBuffer);
        ComputeShader.SetBuffer(dispatchID, "positionBuffer", ComputeBuffer);

        //ComputeShader.SetBuffer(InitID, "positionBuffer", ComputeBuffer);
        //ComputeShader.SetBuffer(InitID, "depthBuffer", _depthBuffer);

        TextureInstanced.Instance.CurMaterial.SetBuffer("positionBuffer", ComputeBuffer);
        TextureInstanced.Instance.CurMaterial.SetTexture("_TexArr", TextureInstanced.Instance.TexArr);


        MoveSpeed = 50f;//更改更快的插值速度
        ComputeShader.SetFloat("MoveSpeed", MoveSpeed);
        ComputeShader.SetFloat("dis", 2);

        ComputeShader.SetBuffer(dispatchID, "clickPointsBuff", _clickPointBuff);


        //触摸点最大为十个
        List<Vector3> clicks = new List<Vector3>();
        for (int i = 0; i < 10; i++)
        {
            clicks.Add(Vector3.one * 100000);
        }
        _clickBuff = new ComputeBuffer(10, 12);
        _clickBuff.SetData(clicks.ToArray());
        ComputeShader.SetBuffer(dispatchID, "clicks", _clickBuff);
        InitDisPatch(InitID);


    }

   
    /// <summary>
    /// 当前在最前面的深度，默认是0
    /// </summary>
    private int _curDepth = 0;

    /// <summary>
    ///  调换层次位置
    /// </summary>
    /// <param name="top">需要切换到最前的层次</param>
    private void SetDepth(int top)
    {
        if (_curDepth == top) return;
        if (top >= 3) return;

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
    /// <summary>
    /// 触摸长按后传输到GPU
    /// </summary>
    private void MouseButtonDownAction()
    {

        Vector3[] temp = new Vector3[_clickBuff.count];

        //_clickBuff.GetData(temp);
        //传输过去前，先重置数据
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = Vector3.one * 1000000;
        }

        int n = 0;
        foreach (KeyValuePair<int, ClickData> keyValuePair in _touchIds)
        {

            Vector3 pos = keyValuePair.Value.Position;
            pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 6));
            temp[n] = pos;
            n++;
        }
        LeftDownTip.position = temp[0];
        _clickBuff.SetData(temp);
        ComputeShader.SetBuffer(dispatchID, "clicks", _clickBuff);
    }

    protected override void Dispatch(ComputeBuffer system)
    {
        MouseButtonDownAction();

        //if (Input.GetMouseButtonDown(1))
        //{
        //    ComputeShader.SetVector("rangeRot", new Vector4(_zeroIndexCount + 115, _zeroIndexCount + 130, _zeroIndexCount + 50, _zeroIndexCount + 157));
        //}
        //if (Input.GetMouseButtonUp(1))
        //{
        //    ComputeShader.SetVector("rangeRot", new Vector4(-1, -1, -1, -1));
        //}

        if (_clickPoint.z < 0)
            _clickPoint = Camera.main.ScreenToWorldPoint(new Vector3(_clickPoint.x, _clickPoint.y, 6));
        else
            _clickPoint = Vector3.one * 1000000;

        ComputeShader.SetVector("clickPoint", _clickPoint);
        ComputeShader.SetFloat("deltaTime", Time.deltaTime);

        Dispatch(dispatchID, system);

        if (_clickPoint.z < 1000000)//相当于有点击事件才触发
        {
            PosAndDir[] datas = new PosAndDir[1];
            _clickPointBuff.GetData(datas);
            int index = datas[0].picIndex;
            //Debug.Log("click index is " + index);
            PictureHandle.Instance.GetYearInfo(datas[0], Canvas);
            _clickPoint = Vector3.one * 1000000;//重置数据
        }


    }


    public override void ExitMotion()
    {
        base.ExitMotion();
        if (_depthBuffer != null)
            _depthBuffer.Release();
        _depthBuffer = null;

        if (_clickBuff != null)
            _clickBuff.Release();
        _clickBuff = null;

        if (_clickPointBuff != null)
            _clickPointBuff.Release();
        _clickPointBuff = null;
    }




    public void ChangeState(int depth)
    {
        SetDepth(depth);
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        if (MotionType != TextureInstanced.Instance.Type) return;
        base.OnPointerUp(eventData);

        if (_touchIds.ContainsKey(eventData.pointerId))
        {
            float temp = _touchIds[eventData.pointerId].ClickTime;

            // Debug.Log(temp);
            if (temp <= 0.35f)
            {
                // Debug.Log("产生点击事件");
                _clickPoint = new Vector3(eventData.position.x, eventData.position.y, -1);//-1表示有点击事件产生
            }
            _touchIds.Remove(eventData.pointerId);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (MotionType != TextureInstanced.Instance.Type) return;
        base.OnPointerDown(eventData);
        //  Debug.Log("this is OnPointerDown  eventData.clickTime " + eventData.clickTime);

        if (!_touchIds.ContainsKey(eventData.pointerId))
        {
            Vector3 pos = eventData.position;
            ClickData data = new ClickData();
            data.Position = pos;
            data.ClickTime = 0;
            _touchIds.Add(eventData.pointerId, data);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (MotionType != TextureInstanced.Instance.Type) return;
        base.OnDrag(eventData);
        if (_touchIds.ContainsKey(eventData.pointerId))
        {
            Vector3 pos = eventData.position;
            _touchIds[eventData.pointerId].Position = pos;//保留初始点击时间
        }
    }

    protected override void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(Depth);
            Depth--;
            if (Depth < 0) Depth = 2;
            //ClassiFicationMotion.ChangeState(1);
        }
    }
    private void LateUpdate()
    {
        if (_touchIds != null)
            foreach (KeyValuePair<int, ClickData> data in _touchIds)
            {
                data.Value.ClickTime += Time.deltaTime;
            }
    }


}
