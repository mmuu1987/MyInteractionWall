﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class Item : MonoBehaviour, IDragHandler, IPointerClickHandler
{


    private RectTransform _rectTransform;

    private YearsEvent _yearsEvent;

    private Text _yearText;

    private RawImage _image;

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
        _yearText = this.transform.Find("YearText").GetComponent<Text>();
        _image = this.transform.Find("Image").GetComponent<RawImage>();
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
         }));

         this.transform.Find("destroy").GetComponent<Button>().onClick.AddListener((() =>
         {
             // Debug.Log("next");
           if(_coroutine!=null)StopCoroutine(_coroutine);
             _coroutine = null;
             Destroy(this.gameObject);
             
         }));

         _mat = Resources.Load<Material>("ItemShader");

        _image.material = _mat;

        
    }
	void Start ()
	{
	    _rectTransform = this.GetComponent<RectTransform>();

	    _coroutine = StartCoroutine(WaitTimeDestroy());

       
	}
	
	// Update is called once per frame
	void Update () {
		
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
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 clickPos = eventData.delta;

        _rectTransform.position += clickPos;
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

        _describe.text = "2012-052012-052012-052012-052012-052012-052012-052012-052012-05052012-052012-052012-" +
                         "052012-052012-052012-052012-052012-052012-05052012-052012-052012-052012-052012-05201" +
                         "2-052012-052012-052012-05052012-052012-052012-052012-052012-052012-052012-052012-052012" +
                         "-05052012-052012-052012-052012-052012-052012-052012-052012-052012-05052012-052012-052012-" +
                         "052012-052012-052012-052012-052012-052012-05052012-052012-052012-052012-052012-052012-0520" +
                         "12-052012-052012-05052012-052012-052012-052012-052012-052012-052012-052012-052012-05052012-" +
                         "052012-052012-052012-052012-052012-052012-052012-052012-05052012-052012-052012-052012-052012" +
                         "-052012-052012-052012-052012-0505";

        _describe.transform.GetComponent<ContentSizeFitter>().SetLayoutVertical(); 

        textureIndex = _yearsEvent.PictureIndes;

        _image.material = _mat;


        _mp4Url = "file://C:/Users/Administrator/Desktop/ed16da766ff705c4ff02412d18740105.mp4";

        if (!string.IsNullOrEmpty(_mp4Url))
        {
            _videoPlayer.url = _mp4Url;
        }

        _mat.SetTexture("_TexArrOne", texArry);

        _mat.SetInt("_Index", textureIndex[0]);

        _image.Rebuild(CanvasUpdate.LatePreRender);

        _curIndex = 0;
    }

    private void OnDestroy()
    {
        //Destroy(_mat,0.5f);
        //_mat = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}