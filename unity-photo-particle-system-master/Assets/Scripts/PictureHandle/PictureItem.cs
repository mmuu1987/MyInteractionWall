using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureItem : MonoBehaviour
{

   

    public List<Texture2D> yearTexs; 

    private PosAndDir _data;

    private DepthInfo[] _depthBuffer;//_depthBuffer



    //点击屏幕，触发吸引力的范围,必须在CS赋值初始化
    float dis = 3;


    private Transform _cacheTransform;


    PosAndDir _clickPointsBuff = new PosAndDir(-1);

    private Material _mat;
    private Transform _infoParent;

    public float Alpha;
    // Use this for initialization
    void Start()
    {

        _cacheTransform = this.transform;


    }

    // Update is called once per frame  
    void Update()
    {

    }

    public void SetData(PosAndDir data, DepthInfo[] infos, Transform infoParent, Material mat)
    {
        _data = data;
        _depthBuffer = infos;
        _infoParent = infoParent;

        this.GetComponent<Renderer>().material = mat;


        YearsEvent ye = PictureHandle.Instance.GetYearInfo(data);
        _mat = mat;

        _mat.SetTexture("_TexArrOne", PictureHandle.Instance.TexArr);

        _mat.SetInt("_Index", _data.picIndex);
        _mat.renderQueue = 3000;

        Texture2D tex = null;

        foreach (Texture2D texture2D in yearTexs)
        {
            if (texture2D.name == ye.Years)
            {
                tex = texture2D;
                break;
            }
        }
        _mat.SetTexture("_Year", tex);

        // this.transform.rotation = Quaternion.Euler(10f,5f,0f);
    }

    public void UpdateData(Vector3 clickPoint, Vector3[] clicks)
    {


        Vector4 pos = _data.position;

        if (Math.Abs(pos.w) < 0.0000001f) return;//初始数据w为0的不搞事情  

        Vector4 velocity = _data.velocity;

        Vector3 moveTarget = _data.moveTarget;

        float z = _depthBuffer[(int)velocity.x].handleDepth;

        Alpha = _depthBuffer[(int)velocity.x].alpha;

        float scale = _depthBuffer[(int)velocity.x].scale;

        Vector3 depthDir = new Vector3(0, 0, z);

        Vector3 moveDir = new Vector3(-0.004f, 0, 0);

        moveTarget += moveDir;

        Vector3 allDir = moveTarget + depthDir;


        //计算多点碰撞聚集运动的算法
        //-------------------------
        int count = clicks.Length;

        int n = -1;//让下面的循环else 体里只执行一次，否则执行十次，不是我们想要的

        Vector2 v1 = new Vector2(pos.x, pos.y);

        for (int i = 0; i < count; i++)
        {



            Vector2 v2 = new Vector2(clicks[i].x, clicks[i].y);

            float length = Vector2.Distance(v2, v1);

            //如果有鼠标点击事件//CS代码那边，如果没有点击，点击点会移到 Vector3.one * 1000000位置
            if (clicks[i].z < 1000 && length <= dis)
            {
                n = i;//如果有在这个范围的，则跳出循环
                break;
            }
        }

        if (n >= 0)//如果在某个触摸点半径范围，则吸引
        {

            //pos.xy = lerp(pos.xy,clicks[n].xy,deltaTime*0.5);
            Vector2 v2 = new Vector2(clicks[n].x, clicks[n].y);
            v1 = Vector2.Lerp(v1, v2, Time.deltaTime * 0.5f);
            pos.x = v1.x;
            pos.y = v1.y;
        }
        else
        {
            Vector4 v4 = new Vector4(allDir.x, allDir.y, allDir.z, scale);
            pos = Vector4.Lerp(pos, v4, Time.deltaTime);

            velocity.y = Mathf.Lerp(velocity.y, Alpha, Time.deltaTime);
        }
        //计算多点碰撞聚集运动的算法
        //-------------------------




        //边界检测
        float rightBorder = velocity.z;
        float leftBorder = -rightBorder;
        //if(pos.x<LeftBorder)pos.x += 2*RightBorder;
        if (pos.x < leftBorder)
        {
            pos.x -= 2 * leftBorder;
            moveTarget.x -= 2 * leftBorder;
        }
        //边界检测


        _data.position = pos;

        _data.moveTarget = moveTarget;

        _data.velocity = new Vector4(velocity.x, velocity.y, velocity.z, 0);//保存新的透明值


        Vector4 scaleVector4 = _data.initialVelocity;

        _cacheTransform.localScale = new Vector3(scaleVector4.x * pos.w, scaleVector4.y * pos.w);

        _cacheTransform.position = pos;

        _mat.SetFloat("_Alpha", velocity.y);

        CheckClickPoint(clickPoint);
        //  LerpTex(id);

    }

    //检测点击点在哪个面片里面
    void CheckClickPoint(Vector3 clickPoint)
    {
        _clickPointsBuff.picIndex = -1;//重置索引

        if (clickPoint.z >= 100000) return;

        Vector4 pos = _data.position;

        Vector4 velocity = _data.velocity;

        //  if(velocity.x>0)return;//值允许第一层有点击行为

        float alpha = _depthBuffer[(int)velocity.x].alpha;

        if (alpha < 1) return;// 在第一排的深度才可以点击，用透明度判断是否在第一排

        //float2 leftDownP2, float2 leftUpP1, float2 rightDownP3, float2 rightUpP4, float2 p
        //默认图片大小为长宽为1，一半就是0.5
        Vector2 leftDown = new Vector2(pos.x - 0.5f, pos.y - 0.5f);//+ float2(-0.5, -0.5));

        Vector2 leftUp = new Vector2(pos.x - 0.5f, pos.y + 0.5f);

        Vector2 rightDown = new Vector2(pos.x + 0.5f, pos.y - 0.5f);

        Vector2 rightUp = new Vector2(pos.x + 0.5f, pos.y + 0.5f);

        bool isContains = Common.ContainsQuadrangle(leftDown, leftUp, rightDown, rightUp, clickPoint);

        if (isContains)
        {
            // positionBuffer[id.x].position.w=2;
            _clickPointsBuff = _data;
            PictureHandle.Instance.GetYearInfo(_data, _infoParent);
            Debug.Log("Click index is " + _data.picIndex);
        }
    }
}
