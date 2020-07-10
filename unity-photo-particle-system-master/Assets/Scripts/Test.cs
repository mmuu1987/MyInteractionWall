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
    void Start()
    {
        // HandleTextureArry(Texture2DOne, "_TexArrOne");
        // HandleTextureArry(Texture2DTwo, "_TexArrTwo");
    }
    private void HandleTextureArry(List<Texture2D> textures, string texsName)
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
    private Vector3[] QuadTest(Rect rect, int count, float z)
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
        Quab.transform.localScale = new Vector3(wwidth, wheight, 1f);


        Vector3[] vector3S = new Vector3[row * column];
        //把小矩形填充整个大矩形
        int rowsTemp = 0;//横列个数    y轴概念
        //row*column 为实际上用到的quad个数
        for (int i = 0; i < row * column; i++)
        {
            // Vector3 pos = Vector3.zero;
            if (i != 0 && i % row == 0)
                rowsTemp++;
            int columnTemp = i - rowsTemp * row;//竖列个数  x轴概念

            Vector3 pos = new Vector3(origin.x, origin.y, 0) +
                //这里的xy,存放的是倍数
                new Vector3(wwidthBig * _rect.position.x, wheightBig * _rect.position.y, 0) +
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



            //不好理解可以先吧_timeTemp=1的时候理解
            //角度插值系数
            float lerpValue = Mathf.Lerp(0f, _angle, _timeTemp);
            Debug.Log(lerpValue);
            //高度插值系数
            float tempHeight = Mathf.Lerp(0f, _height, _timeTemp);
            //距离插值系数
            float dis = Mathf.Lerp(Distance, 3, _timeTemp);


            //相机到头部的向量  +  头部到相机的高度的向量  当_timeTemp=1时，dir的y轴必为0，这样就可以水平注视头部
            Vector3 dir = _camToheadDir + new Vector3(0f, tempHeight, 0f);
            dir = dir.normalized;

            //r为旋转量，当_timeTemp=1时，就是相机应该旋转多少度才能跟头部相对的 量 
            //-lerpValue可能为正，也可能为负数，这就是-号存在的意义 正数顺时针，负数逆时针 
            Quaternion r = Quaternion.Euler(new Vector3(0f, -lerpValue, 0f));

            Vector3 newDir = r * -dir;//用四元数旋转向量，很牛逼，必须记住//-号是因为我们需要的是反向的向量

            //新位置= 位置 +向量 这个公式不用说了吧  
            // newDir * dis单位向量乘以长度得到长度为dis的向量
            cam.position = Head.position + newDir * dis;//


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

        float top = near * Mathf.Tan(Mathf.Deg2Rad * (fov / 2));

        float bottom = -top;

        float right = top * aspect;

        float left = -right;

        _projection = new Matrix4x4();

        _projection.m00 = 2 * near / (right - left);

        _projection.m11 = 2 * near / (top - bottom);

        _projection.m02 = (right + left) / (right - left);

        _projection.m12 = (right - left) / (right + left);

        _projection.m22 = -(far + near) / (far - near);

        _projection.m32 = -1;

        _projection.m23 = (-2 * far * near) / (far - near);

        _projection.m12 = 0f;

        Debug.Log(_projection);

        Debug.Log(Camera.main.projectionMatrix);

    }



    private void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 100f, 100f), "Test"))
        {

            // MyScreenToWorldPoint(new Vector3(300, 400, 200));
            // MoveToHead();
          StartCoroutine(  PrintPoints());
        }
    }

    #region 相机原理代码
    /// <summary>
    /// 屏幕坐标转世界坐标，不支持相机旋转设置，必须固定相机旋转为vector3(0,0,0)
    /// </summary>
    public void MyScreenToWorldPoint(Vector3 screenPos)
    {

        Debug.Log("初始屏幕坐标为 " + screenPos);

        //屏幕坐标转为投影坐标矩阵
        Matrix4x4 p = Camera.main.projectionMatrix;
        //世界坐标到相机坐标矩阵
        Matrix4x4 v = Camera.main.worldToCameraMatrix;


        Vector3 apiWorldPos = Camera.main.ScreenToWorldPoint(screenPos);

        Vector3 cam = Camera.main.transform.position;

        Debug.Log("apiPos 得到的世界坐标为 " + apiWorldPos);

        Debug.Log("相机矩阵为 \r\n" + v);

        Vector3 v1 = v.MultiplyPoint(apiWorldPos);//4阶矩阵必须跟四维向量相乘，这里四维向量w自动补全为1

        Debug.Log("根据unity的api 得到的相机坐标为 " + v1);

        float cx1 = apiWorldPos.x + v.m03;//v.mo3  为相机 -cam.x 的值

        float cy1 = apiWorldPos.y + v.m13;//v.m13 为相机  -cam.y 的值

        float cz1 = -apiWorldPos.z + v.m23;//v.m13 为相机  cam.z 的值

        Debug.Log("自己算法得到的世界坐标为 " + new Vector3(cx1, cy1, cz1));

        float cx2 = apiWorldPos.x - cam.x;

        float cy2 = apiWorldPos.y - cam.y;

        float cz2 = -apiWorldPos.z + cam.z;

        Debug.Log("根据相机位置算出的世界坐标为 " + new Vector3(cx2, cy2, cz2));


        float px = screenPos.x / Screen.width;

        px = (px - 0.5f) / 0.5f;

        float py = screenPos.y / Screen.height;

        py = (py - 0.5f) / 0.5f;

        Vector3 ppos = new Vector3(px, py, screenPos.z);//得到了齐次坐标

        Debug.Log("齐次坐标 " + ppos);
        ppos = new Vector3(ppos.x * ppos.z, ppos.y * ppos.z, ppos.z);//反透视除法

        float z = ppos.z / p.m32;

        float x = ppos.x / p.m00;

        float y = ppos.y / p.m11;



        Debug.Log("得到相机坐标" + new Vector3(x, y, z));

        Vector3 camPos = Camera.main.transform.position;

        x = x + camPos.x;

        y = y + camPos.y;

        z = camPos.z - z;

        Debug.Log("得到的世界坐标为 " + new Vector3(x, y, z));

        MyWorldToScreenPos(new Vector3(x, y, z));

    }

    /// <summary>
    /// 世界坐标转屏幕坐标，不支持相机旋转设置，必须固定相机旋转为vector3(0,0,0)
    /// </summary>
    /// <param name="worldPos"></param>
    public void MyWorldToScreenPos(Vector3 worldPos)
    {
        Vector3 camPos = Camera.main.transform.position;

        Matrix4x4 p = Camera.main.projectionMatrix;

        float z = camPos.z - worldPos.z;

        float x = worldPos.x - camPos.x;

        float y = worldPos.y - camPos.y;

        Vector3 temp1 = new Vector3(x, y, z);//得到相机坐标

        Debug.Log("======================>>>>>得到的相机坐标为 " + temp1);

        float z1 = temp1.z * p.m32;

        float x1 = temp1.x * p.m00;

        float y1 = temp1.y * p.m11;

        Vector3 ppos = new Vector3(x1 / z1, y1 / z1, z1);//透视除法

        Debug.Log("======================>>>>>其次坐标为 " + ppos);

        float x2 = ppos.x * 0.5f + 0.5f;

        float y2 = ppos.y * 0.5f + 0.5f;

        x2 = x2 * Screen.width;

        y2 = y2 * Screen.height;


        Debug.Log("======================>>>>>得到的屏幕坐标为 " + new Vector3(x2, y2, 0));


    }
    #endregion

    private IEnumerator PrintPoints()
    {

        int n = 0;

        while (true)
        {
            Debug.Log("run");
            var v1 = GetPoints(8.211203f, -8.211203f, 4.46410f, -2.464101f, 0.55f, 1);
            // Debug.Log("randomPos count is  " + v1.Count + " 层级为=> " + 1);

           // var v2 = GetPoints(16.42241f, -16.42241f, 7.928202f, -5.928202f, 0.25f, 1.2f);
            //  Debug.Log("randomPos count is  " + v2.Count + " 层级为=> " + 2);

           // var v3 = GetPoints(24.63361f, -24.63361f, 11.3923f, -9.392303f, 0.18f, 1.4f);
            // Debug.Log("randomPos count is  " + v3.Count + " 层级为=> " + 3);

            if (v1.Count < 50) Debug.LogError("计算错误v1< 30:    >>>>>>>>>>>>>" + v1.Count);

            //if (v1.Count > v2.Count)
            //{
            //    Debug.LogError("计算错误v1>v2:    " + v1.Count + "    " + v2.Count);
            //}
            //else if (v2.Count > v3.Count)
            //{
            //    Debug.LogError("计算错误 v2>v3    " + v2.Count + "    " + v3.Count);
            //}
            //else if (v1.Count > v3.Count)
            //{
            //    Debug.LogError("计算错误 v1>v3    " + v1.Count + "    " + v3.Count);
            //}

            yield return null;

            n++;
            if (n >= 1000) break;
        }

      
    }
    public List<Vector2> GetPoints(float x, float x1, float y, float y1, float scaleY, float s)
    {
        return Common.Sample2D((x - x1) * 4, (y - y1) * scaleY, s + 0.75f, 25);

    }
}
