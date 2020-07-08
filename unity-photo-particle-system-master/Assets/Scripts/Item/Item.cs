using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;


public class Item : MonoBehaviour, IDragHandler, IPointerClickHandler,IPointerDownHandler,IPointerUpHandler
{


    private RectTransform _rectTransform;

    private YearsEvent _yearsEvent;

    private Text _yearText;

    private Image _image;

    private RawImage _videoImage;

    private Text _describe;

    private List<int> textureIndex;

    private Material _mat;

    private int _curIndex;

    private Coroutine _coroutine;

    private WaitForSeconds _waitForSeconds;

    private VideoPlayer _videoPlayer;

    private string _mp4Url = "";

    
	// Use this for initialization

    private void Awake()
    {
        _touchIds = new Dictionary<int, ClickData>();

       // _touchIds.Add(10,new ClickData());
        _yearText = this.transform.Find("YearText").GetComponent<Text>();
        _image = this.transform.Find("Image").GetComponent<Image>();
        _describe = this.transform.Find("Scroll View/Viewport/Content/Describe").GetComponent<Text>();
        _videoPlayer = this.transform.Find("VideoPlay").GetComponent<VideoPlayer>();
        _videoImage = this.transform.Find("VideoPlay").GetComponent<RawImage>();

        RenderTexture rt = new RenderTexture(650,500,0);

        _videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        _videoPlayer.targetTexture = rt;

        _videoImage.texture = rt;

        _videoImage.gameObject.SetActive(false);

        _waitForSeconds = new WaitForSeconds(45f);


        this.transform.Find("previous").GetComponent<Button>().onClick.AddListener((() =>
         {
             //Debug.Log("previous");
             _curIndex--;
             if (_curIndex < 0) _curIndex=0;

             _mat.SetInt("_Index", textureIndex[_curIndex]);
             _videoPlayer.Pause();
             _videoImage.gameObject.SetActive(false);
             _image.gameObject.SetActive(true);

             _image.SetNativeSize();
            

         }));
         this.transform.Find("next").GetComponent<Button>().onClick.AddListener((() =>
         {
            // Debug.Log("next");
             _curIndex ++;
             if (_curIndex >= textureIndex.Count)
             {
                 if (_curIndex == textureIndex.Count)
                 {
                     if (!string.IsNullOrEmpty(_mp4Url))
                     {
                         _videoImage.gameObject.SetActive(true);
                         _image.gameObject.SetActive(false);
                         _videoPlayer.Play();
                     }
                     else
                     {
                         _mat.SetInt("_Index", textureIndex[textureIndex.Count-1]);
                         _videoImage.gameObject.SetActive(false);
                         _image.gameObject.SetActive(true);
                         _curIndex--;
                     }
                 }
                 else
                 {
                     _curIndex--;
                 }
             }
             else
             {
                 _mat.SetInt("_Index", textureIndex[_curIndex]);
                 _videoImage.gameObject.SetActive(false);
                 _image.gameObject.SetActive(true);
                
             }

             _image.SetNativeSize();
         }));

         this.transform.Find("destroy").GetComponent<Button>().onClick.AddListener((() =>
         {
             // Debug.Log("next");
           if(_coroutine!=null)StopCoroutine(_coroutine);
             _coroutine = null;
             Destroy(this.gameObject);
             
         }));

        

  

        
    }
	void Start ()
	{
	    _rectTransform = this.GetComponent<RectTransform>();

	    _coroutine = StartCoroutine(WaitTimeDestroy());

       
	}
	
	// Update is called once per frame
	void Update ()
	{

	    Scale();
	}

  

    /// <summary>
    /// 等待一段时间，如果没有人接触，则自动毁掉
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitTimeDestroy()
    {
        yield return _waitForSeconds;

        Destroy(this.gameObject);
    }
   

    public void OnPointerClick(PointerEventData eventData)
    {
       // Debug.Log("OnPointerClick");

        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(WaitTimeDestroy());
    }

    /// <summary>
    /// 导入数据
    /// </summary>
    public void LoadData(YearsEvent yearsEvent,Texture2DArray texArry)
    {
        _yearsEvent = yearsEvent;

        _yearText.text = _yearsEvent.Years;

        _describe.text = _yearsEvent.Describe;

        _describe.transform.GetComponent<ContentSizeFitter>().SetLayoutVertical(); 

        textureIndex = _yearsEvent.PictureIndes;

        _mat = Resources.Load<Material>("ItemShader");

         _mat = Instantiate(_mat);//拷贝一份

         _image.material = _mat;

        if (yearsEvent.PictureIndes.Count == 2)
        {
             this.transform.Find("previous").gameObject.SetActive(false);
             this.transform.Find("next").gameObject.SetActive(false);
        }


        _mp4Url = "";

        if (!string.IsNullOrEmpty(_mp4Url))
        {
            _videoPlayer.url = _mp4Url;
        }

        _mat.SetTexture("_TexArrOne", texArry);

        _mat.SetInt("_Index", textureIndex[0]);

        _image.Rebuild(CanvasUpdate.LatePreRender);

        _curIndex = 0;

        Vector2 temp = yearsEvent.PictureInfos[textureIndex[0]];

        temp.y -= PictureHandle.Instance.LableHeight;
        //图片的容器的宽高
        Vector2 size = _image.rectTransform.sizeDelta;//max 800  600

        float scaleY = (temp.y -PictureHandle.Instance.LableHeight+15)/temp.y;

        float v2 = temp.x / temp.y;//图片的比率
       

        if (temp.x > temp.y)//如果图片宽大于高
        {
            if (temp.x > size.x)//如果图片宽大于容器的宽
            {
                temp.x = size.x;//以容器宽为准

                temp.y = size.x / v2;//把图片高按比例缩小

                if (temp.y > size.y)//如果图片的高还是大于容器的高
                 {
                     temp.y = size.y;//则以容器的高为标准

                     temp.x = size.y * v2;//容器的高再度计算赋值

                    //一下逻辑同理
                 }
            }
            else //如果图片宽小于容器的宽
            {
                
                if (temp.y > size.y)//如果图片的高还是大于容器的高
                {
                    temp.y = size.y;//则以容器的高为标准

                    temp.x = size.y * v2;//容器的高再度计算赋值

                   
                }
            }
        }
        else if (temp.x <= temp.y)//如果图片的高大于宽 
        {
            if (temp.y > size.y)//如果图片高大于容器的高
            {
                temp.y = size.y;//以容器的高为准

                temp.x = size.y * v2;//重新计算图片的宽

                if (temp.x > size.x)//如果图片的宽还是大于容器的高
                 {

                     temp.x = size.x;//则再次以容器的宽为标准

                     temp.y = size.x / v2;//再以容器的宽计算得到容器的高
                 }
            }
            else //如果图片的高小于容器的高
            {
                //但是图片的宽大于容器的宽
                if (temp.x > size.x)
                {
                    temp.x = size.x;//以容器的宽为准
                    temp.y = size.x / v2;//再以容器的宽计算得到容器的高
                }
               
            }
        }


     //   Vector2 realSize =_yearsEvent.PictureIndes

        _image.rectTransform.sizeDelta = new Vector2(temp.x,temp.y);

        _mat.SetFloat("_Yscale", scaleY);
        _image.SetNativeSize();

    }

    /// <summary>
    /// 调整图片尺寸
    /// </summary>
    private void ReztPictureSize(Vector2 realScale)
    {
        //图片的容器的宽高
        Vector2 size = _image.rectTransform.sizeDelta;//max 800  600

        Vector2 newSize = new Vector2(size.x * realScale.x, size.y * realScale.y);//1500:2000

        float v2 = realScale.x / realScale.y;//图片的比率


        float newWidth = newSize.x;

        float newHeight = newSize.y;

        if (newWidth > newHeight)//如果图片宽大于高
        {
            if (newWidth > size.x)//如果图片宽大于容器的宽
            {

                newWidth = size.x;//以容器宽为准

                newHeight = size.x / v2;//把图片高按比例缩小

                if (newHeight > size.y)//如果图片的高还是大于容器的高
                {


                    newHeight = size.y;//则以容器的高为标准

                    newWidth = size.y * v2;//容器的高再度计算赋值

                    //一下逻辑同理
                }
            }
        }
        else if (newWidth <= newHeight)
        {
            if (newHeight > size.y)
            {
                newHeight = size.x;

                newWidth = size.y * v2;

                if (newWidth > size.x)
                {
                    newHeight = size.x / v2;

                    newWidth = size.x;
                }
            }
        }


        //   Vector2 realSize =_yearsEvent.PictureIndes

        _image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        _image.SetNativeSize();
    }
    private void OnDestroy()
    {
        Destroy(_mat);
        _mat = null;
    }
    /// <summary>
    /// 触摸数据int为id,vector,4为屏幕位置，加 点击的 时间点
    /// </summary>
    private Dictionary<int, ClickData> _touchIds;

    /// <summary>
    /// 两个触摸点的距离
    /// </summary>
    private float _distanceScale = -1f;
    /// <summary>
    /// 缩放缓存
    /// </summary>
    private float _scaleTemp = 1f;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_touchIds.ContainsKey(eventData.pointerId))
        {
            Vector3 pos = eventData.position;
            ClickData data = new ClickData();
            data.Position = pos;
            data.ClickTime = 0;
            _touchIds.Add(eventData.pointerId, data);
        }

        if (_touchIds.Count >= 2)
        {
            //算出两个触摸点的距离
            int n = 0;
            Vector3 pos1 = Vector3.zero;
            Vector3 pos2 = Vector3.zero;
            foreach (KeyValuePair<int, ClickData> id in _touchIds)
            {
                if (n == 0) pos1 = id.Value.Position;
                if (n == 1)
                {
                    pos2 = id.Value.Position;
                    break;
                }
                n++;
            }
            _distanceScale = Vector3.Distance(pos1, pos2);
        }
    }

    private void Scale()
    {
        if (_touchIds.Count >= 2 && _distanceScale>0)
        {
            float dis = 0;

            //算出两个触摸点的距离
            int n = 0;
            Vector3 pos1 = Vector3.zero;
            Vector3 pos2 = Vector3.zero;
            foreach (KeyValuePair<int, ClickData> id in _touchIds)
            {
                if (n == 0) pos1 = id.Value.Position;
                if (n == 1)
                {
                    pos2 = id.Value.Position;
                    break;
                }
                n++;
            }
            dis = Vector3.Distance(pos1, pos2);

            float scale = dis/_distanceScale;

            float value = scale*_scaleTemp;

            if (value >= 2) value = 2f;
            if (value <= 0.35f) value = 0.35f;
            this.transform.localScale = new Vector3(value, value, value);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (_touchIds.ContainsKey(eventData.pointerId))
        {
            _touchIds.Remove(eventData.pointerId);
        }
        if (_touchIds.Count <= 1)
        {
            _distanceScale = -1;
            _scaleTemp = this.transform.localScale.x;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 clickPos = eventData.delta;

        if(_touchIds.Count<=1)//一个触摸点的才可以移动，两个就不行
        _rectTransform.position += clickPos;

        if (_touchIds.ContainsKey(eventData.pointerId))
        {
            Vector3 pos = eventData.position;
            _touchIds[eventData.pointerId].Position = pos;//保留初始点击时间
        }


    }
}
