using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using DG.Tweening;
using UnityEngine;
using mattatz;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Test : MonoBehaviour, IDragHandler, IEndDragHandler
{

    /// <summary>
    /// API实现的坐标点
    /// </summary>
    public Transform targetApi;
    /// <summary>
    /// 自己实现的坐标点
    /// </summary>
    public Transform targetSelf;


    private Rect _rect;

    public GameObject Quab;

    

    public List<Texture2D> Texture2DOne;

    public List<Texture2D> Texture2DTwo;

    public Material CurMaterial;

	// Use this for initialization
	void Start ()
	{
       // HandleTextureArry(Texture2DOne, "_TexArrOne");
       // HandleTextureArry(Texture2DTwo, "_TexArrTwo");
	}
    private void HandleTextureArry(List<Texture2D> textures,string texsName)
    {
        Texture2DArray texs;
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
        texs = new Texture2DArray(textures[0].width, textures[0].width, textures.Count, TextureFormat.DXT1, false, false);

        // 结论 //
        // Graphics.CopyTexture耗时(单位:Tick): 5914, 8092, 6807, 5706, 5993, 5865, 6104, 5780 //
        // Texture2DArray.SetPixels耗时(单位:Tick): 253608, 255041, 225135, 256947, 260036, 295523, 250641, 266044 //
        // Graphics.CopyTexture 明显快于 Texture2DArray.SetPixels 方法 //
        // Texture2DArray.SetPixels 方法的耗时大约是 Graphics.CopyTexture 的50倍左右 //
        // Texture2DArray.SetPixels 耗时的原因是需要把像素数据从cpu传到gpu, 原文: Call Apply to actually upload the changed pixels to the graphics card //
        // 而Graphics.CopyTexture只在gpu端进行操作, 原文: operates on GPU-side data exclusively //

        // using (Timer timer = new Timer(Timer.ETimerLogType.Tick))
        //{
      
            for (int i = 0; i < textures.Count; i++)
            {
                // 以下两行都可以 //
                //Graphics.CopyTexture(textures[i], 0, texArr, i);
                Graphics.CopyTexture(textures[i], 0, 0, texs, i, 0);

            }
        
        
        //}

            texs.wrapMode = TextureWrapMode.Clamp;
            texs.filterMode = FilterMode.Bilinear;



            CurMaterial.SetTexture(texsName, texs);
        //m_mat.SetFloat("_Index", Random.Range(0, textures.Length));

        //AssetDatabase.CreateAsset(texArr, "Assets/RogueX/Prefab/texArray.asset");
    }
    private Vector3 [] QuadTest(Rect rect,int count,float z)
    {
      
        //生成八个盒子，横列4个，竖列2个
        float width = rect.width; //480
        float height = rect.height;//540

        float fitedge = (Mathf.Sqrt((width * height * 1f) / count));//适合的边长，

        int row = (int)(width / fitedge);//求得大概行横列的个数，即一横有多少个

        int column = (int)(height / fitedge);//求得竖列大概的个数，即一竖有多少个

        float smallWidth = width * 1f / row;//求得小矩形屏幕分辨率的宽

        float smallHeight = height * 1f / column;//求得小矩形屏幕分辨率的高，

        //求得小矩形的宽和高后，填满大矩形的个数跟每个盒子分配的面片个数不一致，
        //我们以求得的宽高后的盒子为准，多余的面片我们舍去
        //row*column 为实际上用到的quad个数

        //小矩形的宽高转化为世界宽度和高度，注意Z轴
        Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, z - Camera.main.transform.position.z));
        //世界位置小矩形的宽
        Vector3 wpos = Camera.main.ScreenToWorldPoint(new Vector3(smallWidth, 0f, z - Camera.main.transform.position.z));
        //世界位置小矩形的高
        Vector3 hpos = Camera.main.ScreenToWorldPoint(new Vector3(0f, smallHeight, z - Camera.main.transform.position.z));

        //得到小矩形在世界坐标中应得到的宽度
        float wwidth = wpos.x - origin.x;
        //得到小矩形哎世界坐标中得到的高度
        float wheight = hpos.y - origin.y;

        //世界位置大矩形的宽的位置
        Vector3 widthBigpos = Camera.main.ScreenToWorldPoint(new Vector3(_rect.width, 0f, z - Camera.main.transform.position.z));
        //世界位置大矩形的高的位置
        Vector3 heightBigpos = Camera.main.ScreenToWorldPoint(new Vector3(0f, _rect.height, z - Camera.main.transform.position.z));

        //得到大矩形在世界坐标中应得到的宽度
        float wwidthBig = widthBigpos.x - origin.x;
        //得到大矩形哎世界坐标中得到的高度
        float wheightBig = heightBigpos.y - origin.y;

        //因为quad边长为1，对quab进行缩放加工，quad的位置的Z轴位置一定要正确
        Quab.transform.localScale = new Vector3(wwidth,wheight,1f);


        Vector3[] vector3S = new Vector3[row * column];
        //把小矩形填充整个大矩形
        int rowsTemp = 0;//横列个数    y轴概念
        //row*column 为实际上用到的quad个数
        for (int i = 0; i < row*column; i++)
        {
           // Vector3 pos = Vector3.zero;
            if (i != 0 && i % row == 0)
                rowsTemp++;
            int columnTemp = i - rowsTemp * row;//竖列个数  x轴概念

            Vector3 pos =new Vector3(origin.x,origin.y,0) +
                //这里的xy,存放的是倍数
                new Vector3(wwidthBig *_rect.position.x, wheightBig * _rect.position.y, 0) + 
                new Vector3(wwidth * columnTemp + wwidth / 2, wheight * rowsTemp + wheight / 2f, Quab.transform.position.z);
            vector3S[i] = pos;
        }

        return vector3S;

    }
    public Transform Head;

    /// <summary>
    /// 需要旋转的角度
    /// </summary>
    private float _angle;

    /// <summary>
    /// 插值系数
    /// </summary>
    private float _timeTemp = 2f;
    /// <summary>
    /// 插值速度
    /// </summary>
    public float speed = 0.01f;

    public float Distance = 2f;

    /// <summary>
    /// 相机到头部的向量
    /// </summary>
    private Vector3 _camToheadDir;
    /// <summary>
    /// 相机到头部的相对高度
    /// </summary>
    private float _height;


    private void LoadPPTPicture()
    {
        string path = "D:\\test.pptx";
    }

    private void MoveToHead()
    {
        Transform cam = Camera.main.transform;

        _camToheadDir = Head.position - cam.position;

        _camToheadDir = new Vector3(_camToheadDir.x, 0f, _camToheadDir.z);

        Vector3 headDir = -new Vector3(Head.forward.x, 0f, Head.forward.z);

        _angle = Vector3.Angle(_camToheadDir, headDir);

        Vector3 dir = Vector3.Cross(_camToheadDir, headDir);//叉乘主要判断在脸部的左边还是右边

        if (dir.y > 0)
        {
            _angle *= -1;
        }
        Debug.Log(_angle);
         
        _timeTemp = 0f;

       

        //算出角度后，再重新获取完整的向量
        _camToheadDir = Head.position - cam.position;

        Distance = _camToheadDir.magnitude;

        _height = cam.position.y - Head.position.y;

          GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = cam.position;
    }


    // Update is called once per frame
    void Update()
    {



        if (_timeTemp <= 1)
        {
            _timeTemp += Time.deltaTime * speed;

            Transform cam = Camera.main.transform;
            
            
            //角度插值系数
            float lerpValue = Mathf.Lerp(0f, _angle, _timeTemp);
            Debug.Log(lerpValue);
            //高度插值系数
            float tempHeight = Mathf.Lerp(0f, _height, _timeTemp);
            //距离插值系数
            float dis = Mathf.Lerp(Distance, 3, _timeTemp);


            Vector3 dir = _camToheadDir + new Vector3(0f, tempHeight, 0f);

            dir = dir.normalized;

            Quaternion r = Quaternion.Euler(new Vector3(0f, -lerpValue, 0f));//正数顺时针，负数逆时针

            Vector3 newDir = r * -dir;//旋转向量

            cam.position = Head.position + newDir * dis;


            //旋转相对容易，难得是位置
            cam.transform.forward = Vector3.Lerp(cam.transform.forward, (-newDir).normalized, _timeTemp);//使相机慢慢朝向脸 timeTemp可另外使用一个变量参数

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = cam.position;

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
    }

    private Matrix4x4 _projection;
    /// <summary>
    /// 构造投影矩阵
    /// </summary>
    private void InitProjectionMatrix()
    {
        float fov = Camera.main.fieldOfView;
        float near = Camera.main.nearClipPlane;
        float far = Camera.main.farClipPlane;
        float aspect = Camera.main.aspect;

        float top = near * Mathf.Tan(Mathf.Deg2Rad *(fov / 2));

        float bottom = -top;

        float right = top*aspect;

        float left = -right;

         _projection = new Matrix4x4();

        _projection.m00 = 2*near/(right - left);

        _projection.m11 = 2*near/(top - bottom);

        _projection.m02 = (right + left)/(right - left);

        _projection.m12 = (right - left) / (right + left);

        _projection.m22 = -(far + near)/(far - near);

        _projection.m32 = -1;

        _projection.m23 = (-2*far*near)/(far - near);

        _projection.m12 = 0f;

        Debug.Log(_projection);

        Debug.Log(Camera.main.projectionMatrix);

    }



    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "Test"))
        {

          //  MyScreenToWorldPoint(new Vector3(300, 400, 200));
            MoveToHead();

        }
    }

    #region 相机原理代码
    /// <summary>
    /// 屏幕坐标转世界坐标
    /// </summary>
    public void MyScreenToWorldPoint(Vector3 screenPos)
    {
        //屏幕坐标转为投影坐标,

        Matrix4x4 p = Camera.main.projectionMatrix;

        Matrix4x4 v = Camera.main.worldToCameraMatrix;

        Debug.Log("初始屏幕坐标为 " + screenPos);

        Vector3 apiPos = Camera.main.ScreenToWorldPoint(screenPos);

        Debug.Log("apiPos 得到的世界坐标为 " + apiPos);

        Vector3 v1 = v.MultiplyPoint(apiPos);

        Debug.Log("api 世界到相机的坐标为 " + v1);


        float px = screenPos.x / Screen.width;

        px = (px - 0.5f) / 0.5f;

        float py = screenPos.y / Screen.height;

        py = (py - 0.5f) / 0.5f;

        Vector3 ppos = new Vector3(px, py, screenPos.z);//得到了齐次坐标

        ppos = new Vector3(ppos.x * ppos.z, ppos.y * ppos.z, ppos.z);//反透视除法

        float z = ppos.z / p.m32;

        float x = ppos.x / p.m00;

        float y = ppos.y / p.m11;

        Debug.Log("得到相机坐标" + new Vector3(x, y, z));

    }

    private void Stwts()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(300, 400, 200f));

        Debug.Log("得到的世界的坐标为 " + worldPos);

        Matrix4x4 p = Camera.main.projectionMatrix;

        Matrix4x4 v = Camera.main.worldToCameraMatrix;

        Vector3 v1 = v.MultiplyPoint(worldPos);

        Debug.Log("世界到相机的坐标为 " + v.MultiplyPoint(worldPos));

        Vector3 v2 = p.MultiplyPoint(v1);

        Vector3 v3 = _projection.MultiplyPoint(v1);

        Debug.Log("v2 is " + v2 + "  v3 is" + v3);

        Matrix4x4 pv = p * v;

        Vector3 pos = pv.MultiplyPoint(worldPos);

        Debug.Log("1  得到投影坐标为 " + pos + " z is " + pos.z);
        //转为屏幕坐标，这时候z值就可以舍弃了，得到了屏幕二维坐标，xy
        float x = pos.x * 0.5f + 0.5f;

        float y = pos.y * 0.5f + 0.5f;

        Vector3 newPos = new Vector3(x * 1920, y * 1080, pos.z);

        Debug.Log("2   得到屏幕坐标为 " + newPos);

        //屏幕坐标转为投影坐标,

        float px = newPos.x / 1920f;

        px = (px - 0.5f) / 0.5f;

        float py = newPos.y / 1080f;

        py = (py - 0.5f) / 0.5f;

        Vector3 ppos = new Vector3(px, py, pos.z);

        Debug.Log("3  得到投影坐标为 " + ppos);
        //屏幕坐标转世界坐标 ，ppos的z要怎么计算才能得到正确的z
        //就当ppos是一个新的屏幕坐标，我要怎么获取z的值呢,求大佬指教
        Matrix4x4 vp = p * v;

        Vector3 wpos = vp.inverse.MultiplyPoint(ppos);

        Debug.Log("得到世界坐标为 " + wpos);

        // UnprojectScreenToWorld(p,v,ref screenPos);

        //再从相机坐标转世界坐标
    }

    #endregion

}
