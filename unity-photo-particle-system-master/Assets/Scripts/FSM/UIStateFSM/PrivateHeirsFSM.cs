using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using XHFrameWork;

public class PrivateHeirsFSM : UIStateFSM
{
    /// <summary>
    /// 品牌介绍
    /// </summary>
    public Button BrandIntroductionBtn;
    /// <summary>
    /// 大湾区高净值中心按钮
    /// </summary>
    public Button DawanDistrictBtn;
    /// <summary>
    /// 增值服务
    /// </summary>
    public Button ValueAddedServices;

    public List<Texture2D> brandTex;

    public List<Texture2D> DawanTex;

    public List<Texture2D> ValueAddTex;

    /// <summary>
    /// 显示内容的贴图  
    /// </summary>
    private RawImage ShowImage;

    /// <summary>
    /// 当前选中的贴图集合  
    /// </summary>
    private List<Texture2D> _curTex;

    private int _curIndex;

    private RawImage _videoImage;
  

    private VideoPlayer _videoPlayer;

    private string _mp4Url;
    private List<Image> _highlights;

    private Transform _previous;
    private Transform _next;

    private Dictionary<string, Texture2D> _videoDic; 
    public PrivateHeirsFSM(Transform go) : base(go)
    {
        _highlights = new List<Image>();

        _videoDic = new Dictionary<string, Texture2D>();

        _mp4Url = "file://" + Application.streamingAssetsPath + "/私享传家/品牌介绍/传家新视频.mp4";

        BrandIntroductionBtn = Parent.transform.Find("1品牌介绍").GetComponent<Button>();

        ValueAddedServices = Parent.transform.Find("2尊享服务").GetComponent<Button>();

        DawanDistrictBtn = Parent.transform.Find("3大湾区高净值中心").GetComponent<Button>();

        ShowImage = Parent.parent.Find("ShowImage").GetComponent<RawImage>();

        brandTex = PictureHandle.Instance.PrivateHeirsAllTexList[0];

        ValueAddTex = PictureHandle.Instance.PrivateHeirsAllTexList[1];

        DawanTex = PictureHandle.Instance.PrivateHeirsAllTexList[2];


      

        BrandIntroductionBtn.onClick.AddListener((() =>
        {
            SetBtn(brandTex);
            SetHighlight(BrandIntroductionBtn.transform);
        }));

        DawanDistrictBtn.onClick.AddListener((() =>
        {
            SetBtn(DawanTex);
            SetHighlight(DawanDistrictBtn.transform);
        }));

        ValueAddedServices.onClick.AddListener((() =>
        {
            SetBtn(ValueAddTex);
            SetHighlight(ValueAddedServices.transform);
        }));

        _videoImage = Parent.transform.Find("VideoPlay").GetComponent<RawImage>();

        _videoPlayer = _videoImage.GetComponent<VideoPlayer>();

        RenderTexture rt = new RenderTexture(1280, 720, 0);

        _videoPlayer.renderMode = VideoRenderMode.RenderTexture;

        _videoPlayer.targetTexture = rt;

        _videoImage.texture = rt;

        _highlights.Add(BrandIntroductionBtn.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(ValueAddedServices.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(DawanDistrictBtn.transform.Find("Image").GetComponent<Image>());


        SetHighlight(BrandIntroductionBtn.transform);

        if (!string.IsNullOrEmpty(_mp4Url))
        {
            //加载视频贴图
            Texture2D tex = Resources.Load<Texture2D>("暂停");
            tex.name = "video";
            brandTex.Add(tex);
            _videoDic.Add(_mp4Url,tex);
        }
       

    }

    public override void Enter()
    {
        base.Enter();

        _previous = Target.transform.Find("CompanyIntroduction/Previous");

        _next = Target.transform.Find("CompanyIntroduction/Next");

        EventTriggerListener.Get(_previous.gameObject).SetEventHandle(EnumTouchEventType.OnClick, Previous);

        EventTriggerListener.Get(_next.gameObject).SetEventHandle(EnumTouchEventType.OnClick, Next);

        _curTex = brandTex;
        BrandIntroductionBtn.onClick.Invoke();
        Parent.parent.gameObject.SetActive(true);//父级别也要显示
        _videoImage.rectTransform.localScale = Vector3.zero;


     
    }
    private void SetBtn(List<Texture2D> texs)
    {
        _curTex = texs;
        _curIndex = 0;
        ShowImage.texture = _curTex[_curIndex];
        CheckVideoTex(null);

        if (_curTex.Count == 1)
        {
            _previous.gameObject.SetActive(false);
            _next.gameObject.SetActive(false);
        }
        else
        {

            _previous.gameObject.SetActive(false);
            _next.gameObject.SetActive(true);

        }

    }

    /// <summary>
    /// 设置高亮
    /// </summary>
    public void SetHighlight(Transform parent)
    {
        foreach (Image image in _highlights)
        {
            if (image.transform.parent == parent)
            {
                image.gameObject.SetActive(true);
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }
    }
    private void Next(GameObject _listener, object _args, params object[] _params)
    {
        _curIndex++;
        if (_curIndex >= _curTex.Count)
        {
            _curIndex--;
        }

        ShowImage.texture = _curTex[_curIndex];
        CheckVideoTex(_curTex[_curIndex]);
        

        if (_curIndex == _curTex.Count - 1)
        {
            _previous.gameObject.SetActive(true);
            _next.gameObject.SetActive(false);
        }
        else
        {
            _previous.gameObject.SetActive(true);
            _next.gameObject.SetActive(true);
        }
      
        
    }
    /// <summary>
    /// 检测是否是视频贴图，如果是，则播放视频
    /// </summary>
    private void CheckVideoTex(Texture2D tex)
    {


        if (tex!=null && tex.name == "video")
        {
            ShowImage.gameObject.SetActive(false);
            _videoPlayer.transform.localScale = Vector3.one*0.35f;
            _videoPlayer.Play();
        }
        else
        {
            _videoPlayer.transform.localScale = Vector3.zero;
            ShowImage.gameObject.SetActive(true);
           
        }
    }
    private void Previous(GameObject _listener, object _args, params object[] _params)
    {
        //Debug.Log("previous");
        _curIndex--;
        if (_curIndex < 0)
        {
            _curIndex = 0;
        }

        ShowImage.texture = _curTex[_curIndex];
        CheckVideoTex(_curTex[_curIndex]);
        if (_curIndex == 0)
        {
            _previous.gameObject.SetActive(false);
            _next.gameObject.SetActive(true);
        }
        else
        {
            _previous.gameObject.SetActive(true);
            _next.gameObject.SetActive(true);
        }

    }


    public override void Exit()
    {
        _videoPlayer.enabled = false;
        base.Exit();

        Parent.gameObject.SetActive(false);

    }
}
