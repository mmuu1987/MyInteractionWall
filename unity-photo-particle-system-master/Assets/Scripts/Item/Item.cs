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
    public void LoadData(YearsEvent yearsEvent,Texture2DArray texArry,Vector2 scale)
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

        Vector2 size = _image.rectTransform.sizeDelta;//max 800  600

        Vector2  newSize = new Vector2(size.x * scale.x,size.y *scale.y);//1500:2000

        float v1 = size.x / size.y;

        float v2 = scale.x/scale.y;


        float newWidth = newSize.x;

        float newHeight = newSize.y;

        if (newSize.x > newSize.y)
        {
            if (newSize.x > 800)
            {
                float a = newSize.x/800f;

                 newWidth = 800f;

                 newHeight = newSize.y/a;

                if (newHeight > 600)
                {
                    float a1 = newHeight/600f;

                    newWidth = newWidth/a1;

                    newHeight = 600;
                }
            }
        }
        else if (newSize.x <= newSize.y)
        {
            if (newSize.y > 600f)
            {
                float a = newSize.y / 600f;

                 newWidth = newSize.x/a;

                 newHeight = 600f;

                 if (newWidth > 800)
                 {
                     float a1 = newWidth / 800f;

                     newHeight = newHeight / a1;

                     newWidth = 800;
                 }
            }
        }



        _image.rectTransform.sizeDelta = new Vector2(newWidth,newHeight)  ;
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
