using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
/// <summary>
/// 分类运动模式
/// </summary>
public class ClassiFicationMotion : MotionInputMoveBase
{



   public uint CategoryCount = 8;
    protected override void Init()
    {
        base.Init();

        MotionType = MotionType.ClassiFicationMotion;

        _screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Z - Camera.main.transform.position.z));
        _screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, Z - Camera.main.transform.position.z));
        _screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, Z - Camera.main.transform.position.z));
        _screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, Z - Camera.main.transform.position.z));

        int instanceCount = TextureInstanced.Instance.InstanceCount;

        PosAndDir[] datas = new PosAndDir[ComputeBuffer.count];

        ComputeBuffer.GetData(datas);


        //根据种类个数生成盒子，先手动完成，盒子分配，有时间在完善=>开发出自动分配均匀大小的盒子填满窗口
        //这里以生成8个盒子还展开
        

        List<Rect> rects = new List<Rect>();
        if (CategoryCount == 8)
        {
            //生成八个盒子，横列4个，竖列2个
            float width = Width/4f; //480
            float height = Height/2f;//540

            //x,y 为倍数
            Rect rect0 = new Rect(0f, 0f, width, height);
            Rect rect1 = new Rect(1f, 0f, width, height);
            Rect rect2 = new Rect(2f, 0f, width, height);
            Rect rect3 = new Rect(3f, 0f, width, height);
            Rect rect4 = new Rect(0f, 1f, width, height);
            Rect rect5 = new Rect(1f, 1f, width, height);
            Rect rect6 = new Rect(2f, 1f, width, height);
            Rect rect7 = new Rect(3f, 1f, width, height);
            rects.Add(rect0);
            rects.Add(rect1);
            rects.Add(rect2);
            rects.Add(rect3);
            rects.Add(rect4);
            rects.Add(rect5);
            rects.Add(rect6);
            rects.Add(rect7);

        }



        //每个盒子分配得到的面片个数 
        int count = instanceCount / (int)CategoryCount;//8192
        Vector2 scale = Vector2.zero;
        for (int i = 0; i < rects.Count; i++)
        {

            Rect rect = rects[i];
            float width = rect.width; 
            float height = rect.height;

            float fitedge = (Mathf.Sqrt((width * height * 1f) / count));//适合的边长，

            int row = (int)(width / fitedge);//求得大概行横列的个数，即一横有多少个

            int column = (int)(height / fitedge);//求得竖列大概的个数，即一竖有多少个

            float smallWidth = width * 1f / row;//求得小矩形屏幕分辨率的宽

            float smallHeight = height * 1f / column;//求得小矩形屏幕分辨率的高，

            //求得小矩形的宽和高后，填满大矩形的个数跟每个盒子分配的面片个数不一致，
            //我们以求得的宽高后的盒子为准，多余的面片我们舍去
            //row*column 为实际上用到的quad个数

            //小矩形的宽高转化为世界宽度和高度，注意Z轴
            Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Z - Camera.main.transform.position.z));
            //世界位置小矩形的宽
            Vector3 wpos = Camera.main.ScreenToWorldPoint(new Vector3(smallWidth, 0f, Z - Camera.main.transform.position.z));
            //世界位置小矩形的高
            Vector3 hpos = Camera.main.ScreenToWorldPoint(new Vector3(0f, smallHeight, Z - Camera.main.transform.position.z));

            //得到小矩形在世界坐标中应得到的宽度
            float wwidth = wpos.x - origin.x;
            //得到小矩形哎世界坐标中得到的高度
            float wheight = hpos.y - origin.y;

            //世界位置大矩形的宽的位置
            Vector3 widthBigpos = Camera.main.ScreenToWorldPoint(new Vector3(rect.width, 0f, Z - Camera.main.transform.position.z));
            //世界位置大矩形的高的位置
            Vector3 heightBigpos = Camera.main.ScreenToWorldPoint(new Vector3(0f, rect.height, Z - Camera.main.transform.position.z));

            //得到大矩形在世界坐标中应得到的宽度
            float wwidthBig = widthBigpos.x - origin.x;
            //得到大矩形哎世界坐标中得到的高度
            float wheightBig = heightBigpos.y - origin.y;

            //因为quad边长为1，对quab进行缩放加工，quad的位置的Z轴位置一定要正确
            // Quab.transform.localScale = new Vector3(wwidth, wheight, 1f);

            scale = new Vector2(wwidth, wheight);//把面片缩放倍数弹出去

           
          
            int actual = row * column;//实际有作用的粒子个数

            //进一步加工uv  y=>rows  x=>horizontalColumn
            //得到一个新的uvwz,
            float u = 1f / row;

            float v = 1f / column;

            float delay = Random.Range(3, 8);

          

            //把小矩形填充整个大矩形
            int columnTempCount = 0;//横列个数    y轴概念
            for (int j = 0; j < actual; j++)
            {
                int index = i*count + j;//buff数组中的索引位置

                // Vector3 pos = Vector3.zero;
                if (j != 0 && j % row == 0)
                    columnTempCount++;
                int rowTempCount = j - columnTempCount * row;//竖列个数  x轴概念

                Vector3 pos = new Vector3(origin.x, origin.y, 0) +
                    //这里的xy,存放的是倍数
                    new Vector3(wwidthBig * rect.position.x, wheightBig * rect.position.y, 0) +
                    new Vector3(wwidth * rowTempCount + wwidth / 2, wheight * columnTempCount + wheight / 2f, Z);
                datas[index].moveTarget = pos;
                datas[index].originalPos = Vector4.one;
               // datas[index].position = Vector4.one;
                datas[index].indexRC = new Vector2(rowTempCount,columnTempCount);
                datas[index].picIndex = i; 
                datas[index].bigIndex = i;//分类编号
               
                Vector4 otherData = new Vector4();//切换图片索要缓存的数据
                otherData.x =  delay;//延迟播放的时间
                otherData.y = 1f;//Random.Range(0.1f,1f);//切换图片的时间
                otherData.z = 0f;//时间缓存
                otherData.w = 0f;//插值值
                datas[index].velocity = otherData;
                datas[index].initialVelocity = Vector3.zero;//缓存器
                float w = u * rowTempCount;//rowsCount

                float z = v * columnTempCount;//columnCount

                datas[index].uvOffset = new Vector4(u, v, w, z);

                #region 设置第二套UV，写死规定每个小矩形在切分成九个小矩形，这里只设置UV切分

              

                float row2 = row/3f;//小小矩形的横轴个数
                float column2 = column/3f;//小小矩形竖轴个数

                float u2 = 1f / row2;
                float v2 = 1f / column2;

                int indexrow = (int)( rowTempCount *1f /row2)+1;//小矩形里小小矩形的x轴个数
                int indexColumn = (int)(columnTempCount * 1f / column2) + 1;//小矩形里小小矩形的y轴的个数


                float w2 = ((rowTempCount % row2)*1f / row2) ;

                float z2 = ((columnTempCount % column2) * 1f / column2) ;

                datas[index].uv2Offset = new Vector4(u2, v2, w2, z2);
                datas[index].bigIndex = i + (indexColumn - 1)*3 + indexrow;//分类编号*3是因为一横有三个
                #endregion

            }
            int useless = count - actual;//剩下的没有用到的面片，

            //全部缩小至0，位置也归零
            for (int j = 0; j < useless; j++)
            {
                int index = actual+j;

                datas[index].position = Vector4.zero;
                //datas[index].moveTarget = Vector3.zero;
                //datas[index].originalPos = Vector4.zero;
            }

        }
        TextureInstanced.Instance.ChangeInstanceMat(null);
        TextureInstanced.Instance.CurMaterial.SetVector("_WHScale", new Vector4(scale.x, scale.y, 1, 1));

        ComputeBuffer.SetData(datas);
        ComputeShader.SetBuffer(dispatchID, "positionBuffer", ComputeBuffer);
        ComputeShader.SetBuffer(InitID, "positionBuffer", ComputeBuffer);
        MoveSpeed = 50f;//更改更快的插值速度
        ComputeShader.SetFloat("MoveSpeed", MoveSpeed);

    }

    /// <summary>
    /// 改变状态，直接把大矩形填满整个屏幕，其他大矩形则透明掉
    /// </summary>
    public void ChangeState(int classNumber)
    {
        if (MotionType != MotionType.ClassiFicationMotion) return;

        PosAndDir[] datas = new PosAndDir[ComputeBuffer.count];

        ComputeBuffer.GetData(datas);
        List<PosAndDir> temp = new List<PosAndDir>();
        List<PosAndDir> allDataList = new List<PosAndDir>();

        //这里注意，posAndDir属于结构体，值拷贝
        for (int i = datas.Length - 1; i >= 0; i--)
        {
            PosAndDir data = datas[i];
            if (datas[i].bigIndex == classNumber)
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

        //根据分辨率展开图片 写死每个大矩形的宽高
        int widthCount = Width / 96;//每个图片宽99像素
        int heightCount = Height/108;//每个图片高108素


        List<Rect> rects = new List<Rect>();

        int tempHeithtCount = 0;//y轴上的个数
        for (int i = 0; i < widthCount*heightCount; i++)
        {
            if (i != 0 && i%widthCount == 0)
                tempHeithtCount++;
            int tempWidthCount = i - tempHeithtCount * widthCount;//轴上的个数

            Rect rect = new Rect(tempWidthCount, tempHeithtCount, 96f, 108f);
            rects.Add(rect);
        }

        //每个盒子分配得到的面片个数 会有概率盒子分配过剩，记得把过剩的处理
        int count = newData.Length / rects.Count;
        Vector2 scale = Vector2.zero;
        for (int i = 0; i < rects.Count; i++)
        {
            #region 处理小矩形
            Rect rect = rects[i];
            float width = rect.width;
            float height = rect.height;

            float fitedge = (Mathf.Sqrt((width * height * 1f) / count));//适合的边长，

            int row = (int)(width / fitedge);//求得大概行横列的个数，即一横有多少个

            int column = (int)(height / fitedge);//求得竖列大概的个数，即一竖有多少个

            float smallWidth = width * 1f / row;//求得小矩形屏幕分辨率的宽

            float smallHeight = height * 1f / column;//求得小矩形屏幕分辨率的高，

            //求得小矩形的宽和高后，填满大矩形的个数跟每个盒子分配的面片个数不一致，
            //我们以求得的宽高后的盒子为准，多余的面片我们舍去
            //row*column 为实际上用到的quad个数

            //小矩形的宽高转化为世界宽度和高度，注意Z轴
            Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Z - Camera.main.transform.position.z));
            //世界位置小矩形的宽
            Vector3 wpos = Camera.main.ScreenToWorldPoint(new Vector3(smallWidth, 0f, Z - Camera.main.transform.position.z));
            //世界位置小矩形的高
            Vector3 hpos = Camera.main.ScreenToWorldPoint(new Vector3(0f, smallHeight, Z - Camera.main.transform.position.z));

            //得到小矩形在世界坐标中应得到的宽度
            float wwidth = wpos.x - origin.x;
            //得到小矩形哎世界坐标中得到的高度
            float wheight = hpos.y - origin.y;

            //世界位置大矩形的宽的位置
            Vector3 widthBigpos = Camera.main.ScreenToWorldPoint(new Vector3(rect.width, 0f, Z - Camera.main.transform.position.z));
            //世界位置大矩形的高的位置
            Vector3 heightBigpos = Camera.main.ScreenToWorldPoint(new Vector3(0f, rect.height, Z - Camera.main.transform.position.z));

            //得到大矩形在世界坐标中应得到的宽度
            float wwidthBig = widthBigpos.x - origin.x;
            //得到大矩形哎世界坐标中得到的高度
            float wheightBig = heightBigpos.y - origin.y;

            //因为quad边长为1，对quab进行缩放加工，quad的位置的Z轴位置一定要正确
            // Quab.transform.localScale = new Vector3(wwidth, wheight, 1f);

            scale = new Vector2(wwidth, wheight);//把面片缩放倍数弹出去



            int actual = row * column;//实际有作用的粒子个数

            //进一步加工uv  y=>rows  x=>horizontalColumn
            //得到一个新的uvwz,
            float u = 1f / row;

            float v = 1f / column;

            float delay = Random.Range(3, 8);


           

            //把小矩形填充整个大矩形
            int rowsCount = 0;//横列个数    y轴概念
            for (int j = 0; j < count; j++)
            {
                int index = i * count + j;//buff数组中的索引位置

               
                if (j >= actual)//多余的面片不应该显示
                {
                    newData[index].position = Vector4.zero;
                    //newData[index].moveTarget = Vector3.zero;
                    //newData[index].originalPos = Vector4.zero;

                    continue;
                }

                // Vector3 pos = Vector3.zero;
                if (j != 0 && j % row == 0)
                    rowsCount++;
                int columnCount = j - rowsCount * row;//竖列个数  x轴概念

                Vector3 pos = new Vector3(origin.x, origin.y, 0) +
                    //这里的xy,存放的是倍数
                    new Vector3(wwidthBig * rect.position.x, wheightBig * rect.position.y, 0) +
                    new Vector3(wwidth * columnCount + wwidth / 2, wheight * rowsCount + wheight / 2f, Z);

               
                newData[index].moveTarget = pos;
                newData[index].originalPos = Vector4.one;
                newData[index].indexRC = new Vector2(columnCount, rowsCount);
                newData[index].picIndex = i % TextureInstanced.Instance.TexArr.depth;
                newData[index].bigIndex = i % TextureInstanced.Instance.TexArr.depth;//分类编号

                Vector4 otherData = new Vector4();//切换图片索要缓存的数据
                otherData.x = delay;//延迟播放的时间
                otherData.y = 1f;//Random.Range(0.1f,1f);//切换图片的时间
                otherData.z = 0f;//时间缓存
                otherData.w = 0f;//插值值
                newData[index].velocity = otherData;


                float w = u * columnCount;

                float z = v * rowsCount;//rowsCount

                newData[index].uvOffset = new Vector4(u, v, w, z);
                newData[index].uv2Offset = new Vector4(u, v, w, z);

            }
            #endregion
        }

        //计算剩余的不用的面片隐藏掉
        int surplusCount = newData.Length - count*rects.Count;

        if (surplusCount > 0)
        {
            for (int i = newData.Length-1; i >=newData.Length-surplusCount; i--)
            {
               newData[i].position = Vector4.zero;
               
            }
        }

        TextureInstanced.Instance.CurMaterial.SetVector("_WHScale", new Vector4(scale.x, scale.y, 1, 1));


        allDataList.AddRange(newData);

        ComputeBuffer.SetData(allDataList.ToArray());
        ComputeShader.SetBuffer(dispatchID, "positionBuffer", ComputeBuffer);
        ComputeShader.SetBuffer(InitID, "positionBuffer", ComputeBuffer);
        MoveSpeed = 50f;//更改更快的插值速度
        ComputeShader.SetFloat("MoveSpeed", MoveSpeed);

        InitDisPatch(InitID);

    }

    private int rot = 0;

    private Vector3[] QuadTest(Rect rect, int count, float z,out Vector2 scale)
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
        Vector3 widthBigpos = Camera.main.ScreenToWorldPoint(new Vector3(rect.width, 0f, z - Camera.main.transform.position.z));
        //世界位置大矩形的高的位置
        Vector3 heightBigpos = Camera.main.ScreenToWorldPoint(new Vector3(0f, rect.height, z - Camera.main.transform.position.z));

        //得到大矩形在世界坐标中应得到的宽度
        float wwidthBig = widthBigpos.x - origin.x;
        //得到大矩形哎世界坐标中得到的高度
        float wheightBig = heightBigpos.y - origin.y;

        //因为quad边长为1，对quab进行缩放加工，quad的位置的Z轴位置一定要正确
       // Quab.transform.localScale = new Vector3(wwidth, wheight, 1f);

         scale = new Vector2(wwidth, wheightBig);//把面片缩放倍数弹出去

        
        //计算好后把数据返回出去
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
                new Vector3(wwidthBig * rect.position.x, wheightBig * rect.position.y, 0) +
                new Vector3(wwidth * columnTemp + wwidth / 2, wheight * rowsTemp + wheight / 2f, z);
            vector3S[i] = pos;
        }

        return vector3S;

    }
    protected override void InitDisPatch(int id)
    {
        float t = TimeTmep / StopTime;
        // Debug.Log(t);
        ComputeShader.SetFloat("deltaTime", Time.deltaTime);//初始化运动的时候的插值数值

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

    
    
    /// <summary>
    /// 拖拽的初始向量
    /// </summary>
    private Vector3 _drag;
    protected override void Dispatch(ComputeBuffer system)
    {

        Vector3 movePos = new Vector3(_delta.x, _delta.y, 0) * 0.01f;

        #region  另一版代码
        //MouseButtonDownAction();

        //_actionState = InputManager.Instance.InputState;


        //if (_actionState == 3) Z++;
        //else if (_actionState == 4) Z--;




        //_screenPosLeftDown = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Z - Camera.main.transform.position.z));
        //_screenPosLeftUp = Camera.main.ScreenToWorldPoint(new Vector3(0, Height, Z - Camera.main.transform.position.z));
        //_screenPosRightDown = Camera.main.ScreenToWorldPoint(new Vector3(Width, 0, Z - Camera.main.transform.position.z));
        //_screenPosRightUp = Camera.main.ScreenToWorldPoint(new Vector3(Width, Height, Z - Camera.main.transform.position.z));


     
        //CheckBound();
        //if (_actionState == 2)
        //{
        //    _drag = Vector3.zero;
        //    _originalPosLeftUp += movePos;
        //    _originalPosLeftDown += movePos;
        //    _originalPosRightUp += movePos;
        //    _originalPosRightDown += movePos;

        //    //累计拖拽力
        //    if (_lerpQueue.Count > 10)
        //    {
        //        _lerpQueue.Dequeue();
        //    }
        //    else
        //    {
        //        _lerpQueue.Enqueue(movePos);
        //    }

        //}
        //else if (_actionState == 0)
        //{

        //    _drag = Vector3.Lerp(_drag, Vector3.zero, Time.deltaTime*2f);//拖拽力随着时间慢慢减少


        //    _originalPosLeftDown = Vector3.Lerp(_originalPosLeftDown, _originalPosLeftDown + (_moveDir + _drag), Time.deltaTime * MoveSpeed);
        //    _originalPosLeftUp = Vector3.Lerp(_originalPosLeftUp, _originalPosLeftUp + (_moveDir + _drag), Time.deltaTime * MoveSpeed);
        //    _originalPosRightDown = Vector3.Lerp(_originalPosRightDown, _originalPosRightDown + (_moveDir + _drag), Time.deltaTime * MoveSpeed);
        //    _originalPosRightUp = Vector3.Lerp(_originalPosRightUp, _originalPosRightUp + (_moveDir + _drag), Time.deltaTime * MoveSpeed);



        //}


        //_delta = Vector2.zero;




        //if (_drag == Vector3.zero && _moveDir == Vector3.zero)//_moveDir等于0说明不是在边缘位置
        //{
        //    Vector3 temp = Vector3.zero;
        //    int count = _lerpQueue.Count;
        //    Vector3 dir = Vector3.zero;//拿到队列的最后一个作为方向
        //    //计算拖拽效果
        //    for (int i = 0; i < count; i++)
        //    {
        //        var data = _lerpQueue.Dequeue();
        //        if (i + 1 == count) dir = data;
        //        temp += data;
        //    }

        //    float value = temp.magnitude;

        //    _drag = dir.normalized * value;
        //}

        //LeftDownTip.position = _originalPosLeftDown + new Vector3(0, 0, -0.01f);
        //LeftUpTip.position = _originalPosLeftUp + new Vector3(0, 0, -0.01f);
        //RightDownTip.position = _originalPosRightDown + new Vector3(0, 0, -0.01f);
        //RightUpTip.position = _originalPosRightUp + new Vector3(0, 0, -0.01f);
#endregion
        


        ComputeShader.SetVector("movePos", movePos);
        ComputeShader.SetFloat("Z", Z);
        ComputeShader.SetInt("actionState", _actionState);
        ComputeShader.SetFloat("deltaTime", Time.deltaTime);
        ComputeShader.SetVector("moveDir", _moveDir);
        ComputeShader.SetVector("originPos", _originalPosLeftDown);
        ComputeShader.SetVector("drag", _drag);





        Dispatch(dispatchID,system);

    }

  

   
}
