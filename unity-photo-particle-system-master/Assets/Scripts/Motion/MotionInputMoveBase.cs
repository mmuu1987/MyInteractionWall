using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MotionInputMoveBase : MotionBase, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler,IBeginDragHandler
    
{
    /// <summary>
    /// 面片坐标系的原点========>所有面片组成的矩形的0,0点，组成的矩形相当于在x,y正向展开面片  
    /// </summary>
    protected Vector3 _originalPosLeftDown;
    /// <summary>
    /// 面片坐标系的原点========>所有面片组成的矩形的0,1点，组成的矩形相当于在x,y正向展开面片
    /// </summary>
    protected Vector3 _originalPosLeftUp;
    /// <summary>
    /// 面片坐标系的原点========>所有面片组成的矩形的1,0点，组成的矩形相当于在x,y正向展开面片
    /// </summary>
    protected Vector3 _originalPosRightDown;
    /// <summary>
    /// 面片坐标系的原点========>所有面片组成的矩形的1,1点，组成的矩形相当于在x,y正向展开面片
    /// </summary>
    protected Vector3 _originalPosRightUp;


    /// <summary>
    /// 面片中心到左下角的向量
    /// </summary>
    protected Vector3 _leftDownDir;
    /// <summary>
    /// 面片中心到左上角的向量
    /// </summary>
    protected Vector3 _leftUpDir;
    /// <summary>
    /// 面片中心到右下角的向量
    /// </summary>
    protected Vector3 _rightDownDir;
    /// <summary>
    /// 面片中心到右上角的向量
    /// </summary>
    protected Vector3 _rightUpDir;

    

    /// <summary>
    /// 超过边界，返回边界的向量
    /// </summary>
    protected Vector3 _moveDir;
    /// <summary>
    /// 相机左下角的屏幕位置转化的世界位置,我们以左下角屏幕位置为最初始点
    /// </summary>
    protected Vector3 _screenPosLeftDown;
    /// <summary>
    /// 相机做上角的屏幕位置
    /// </summary>
    protected Vector3 _screenPosLeftUp;
    /// <summary>
    /// 相机右下角的屏幕位置
    /// </summary>
    protected Vector3 _screenPosRightDown;
    /// <summary>
    /// 相机右上角的屏幕位置
    /// </summary>
    protected Vector3 _screenPosRightUp;

    /// <summary>
    /// 鼠标动作状态 0为弹起后无操作状态,1为按下，2为按下中,3,4为滚轮状态
    /// </summary>
    protected int _actionState = 0;


    protected Vector2 _delta;


    /// <summary>
    /// ,初始化的时候图片距离相机距离，cube运动中，为第一排图片距离相机的距离
    /// </summary>
    public float Z;
    /// <summary>
    /// 图片相对于相机前进的最大深度
    /// </summary>
    public float MaxDepth = 3;

    /// <summary>
    /// 插值速度运动速度
    /// </summary>
    protected float MoveSpeed = 5f;

    protected MotionType MotionType;
    protected override void Start()
    {
        dispatchID = ComputeShader.FindKernel(computeShaderName);
        if (!string.IsNullOrEmpty(InitName))
        InitID = ComputeShader.FindKernel(InitName);
        base.Start();
    }


    protected override void Init()
    {
        base.Init();


        float distanceSize = Mathf.Sqrt(TextureInstanced.Instance.SizeWidth / 2 * TextureInstanced.Instance.SizeWidth / 2 + TextureInstanced.Instance.SizeHeight / 2 * TextureInstanced.Instance.SizeHeight / 2);

        _leftDownDir = new Vector2(-1, -1).normalized * distanceSize;

        _leftUpDir = new Vector2(-1, 1).normalized * distanceSize;

        _rightDownDir = new Vector2(1, -1).normalized * distanceSize;

        _rightUpDir = new Vector2(1, 1).normalized * distanceSize;

        Camera.main.fieldOfView = 60f;

    }

   

    
    /// <summary>
    /// 判断Z轴缩小到限定边界,
    /// </summary>
    protected virtual void CheckBound()
    {
        Vector3 ld = _originalPosLeftDown - _rightUpDir;
        Vector3 lu = _originalPosLeftUp - _rightDownDir;
        Vector3 rd = _originalPosRightDown - _leftUpDir;
        Vector3 ru = _originalPosRightUp - _leftDownDir;

        //拓宽一下屏幕四个角顶点位置，让其它在顶点角外边,X轴拓宽，目前还不开发Y轴
        Vector3 sld = _screenPosLeftDown + new Vector3(-SizeWidth, 0f, 0f);

        Vector3 slu = _screenPosLeftUp + new Vector3(-SizeWidth, 0f, 0f);

        Vector3 srd = _screenPosRightDown + new Vector3(SizeWidth, 0f, 0f);

        Vector3 sru = _screenPosRightUp + new Vector3(SizeWidth, 0f, 0f);

        //<<<<<<<<<<<<<<<<<<<<<<=====================要特别注意的是，这个值是在进行GPGPU计算完成的时候才得到的值，所以我们
        //<<<<<<<<<<<<<<<<<<<<<<=====================在GPGPU用这个值来做处理的时候要想到该值在什么时候是zero
        _moveDir = Vector2.zero;

        if (_actionState == 0)
        {
            //判断面片组成的长方形上下左右是否进入屏幕

            if (ld.x >= sld.x) _moveDir = new Vector2(sld.x - ld.x, 0);

            if (rd.x <= srd.x) _moveDir = new Vector2(srd.x - rd.x, 0);

            if (lu.y <= slu.y) _moveDir = new Vector2(0, slu.y - lu.y);

            if (ld.y >= sld.y) _moveDir = new Vector2(0, sld.y - ld.y);


            //判断四个角 是否在屏幕范围内  positionBuffer[ID_leftDown] 等的值都是要在计算完成后才能拿，因为是并行计算

            bool bld = Common.ContainsQuadrangle(sld, slu, srd, sru, ld);

            bool blu = Common.ContainsQuadrangle(sld, slu, srd, sru, lu);

            bool brd = Common.ContainsQuadrangle(sld, slu, srd, sru, rd);

            bool bru = Common.ContainsQuadrangle(sld, slu, srd, sru, ru);

            if (bru) _moveDir = sru - ru;// + _rightUpDir;
            if (brd) _moveDir = srd - rd;//+_rightDownDir;
            if (blu) _moveDir = slu - lu;// + _leftUpDir;
            if (bld) _moveDir = sld - ld;//+ _leftDownDir;

            // Debug.Log(_moveDir);
        }

        //限定距离
        if (Z <= Camera.main.transform.position.z + MaxDepth) Z = Camera.main.transform.position.z + MaxDepth;

        //跟获取到的屏幕四个角的点转化到世界坐标后做比较，面片组成的矩形如果长或者宽小于屏幕转化的点，则限制z值
        float width = Mathf.Abs(srd.x - sld.x);
        float height = Mathf.Abs(slu.y - sld.y);

        //获取边界长宽
        float widthBound = Mathf.Abs(rd.x - ld.x);
        float heightBound = Mathf.Abs(lu.y - ld.y);

        if (width > widthBound) Z--;
        else if (height > heightBound) Z--;
     //  CheckZ();
       
    }

    protected virtual void  CheckZ()
    {
        

       
    }
    protected override void Dispatch(ComputeBuffer system)
    {
       // Debug.Log("run");
        _actionState = InputManager.Instance.InputState;


        if (_actionState == 3) Z++;
        else  if (_actionState == 4) Z--;


       

        _screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Z - Camera.main.transform.position.z));
        _screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, Z - Camera.main.transform.position.z));
        _screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, Z - Camera.main.transform.position.z));
        _screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, Z - Camera.main.transform.position.z));
        //Camera.main.matr
        
        Vector3 movePos = new Vector3(_delta.x, _delta.y, 0) * 0.01f;

        if (_actionState == 2)
        {
            _originalPosLeftUp += movePos;
            _originalPosLeftDown += movePos;
            _originalPosRightUp += movePos;
            _originalPosRightDown += movePos;


        }
        else if (_actionState == 0)
        {
            _originalPosLeftDown = Vector3.Lerp(_originalPosLeftDown, _originalPosLeftDown + _moveDir, Time.deltaTime * MoveSpeed);
            _originalPosLeftUp = Vector3.Lerp(_originalPosLeftUp, _originalPosLeftUp + _moveDir, Time.deltaTime * MoveSpeed);
            _originalPosRightDown = Vector3.Lerp(_originalPosRightDown, _originalPosRightDown + _moveDir, Time.deltaTime * MoveSpeed);
            _originalPosRightUp = Vector3.Lerp(_originalPosRightUp, _originalPosRightUp + _moveDir, Time.deltaTime * MoveSpeed);

          

        }


        _delta = Vector2.zero;

        CheckBound();

        LeftDownTip.position = _originalPosLeftDown + new Vector3(0, 0, -0.01f);
        LeftUpTip.position = _originalPosLeftUp + new Vector3(0, 0, -0.01f);
        RightDownTip.position = _originalPosRightDown + new Vector3(0, 0, -0.01f);
        RightUpTip.position = _originalPosRightUp + new Vector3(0, 0, -0.01f);


        ComputeShader.SetVector("movePos", movePos);
        ComputeShader.SetFloat("Z", Z);
        ComputeShader.SetInt("actionState", _actionState);
        ComputeShader.SetFloat("deltaTime", Time.deltaTime);
        ComputeShader.SetVector("moveDir", _moveDir);
        ComputeShader.SetVector("originPos", _originalPosLeftDown);



        Dispatch(dispatchID, system);

    }

    public override void ExitMotion()
    {
        base.ExitMotion();
      
        _actionState = 0;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
       // Debug.Log("OnPointerUp");
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
       // Debug.Log("OnPointerClick");
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
      //  Debug.Log("OnPointerDown");
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        _delta = eventData.delta;
      //  Debug.Log("OnDrag");
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        _delta = Vector2.zero;
      //  Debug.Log("OnEndDrag");
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
       // Debug.Log("OnBeginDrag");
    }
}
