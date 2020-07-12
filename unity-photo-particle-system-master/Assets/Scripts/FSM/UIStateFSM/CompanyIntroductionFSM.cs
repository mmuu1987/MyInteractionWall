using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XHFrameWork;

public class CompanyIntroductionFSM : UIStateFSM
{


    /// <summary>
    /// 公司介绍
    /// </summary>
    public Button Introduce;
    /// <summary>
    /// 基本信息
    /// </summary>
    public Button Info;
    /// <summary>
    /// 股东概况
    /// </summary>
    public Button Shareholder;
    /// <summary>
    /// 荣誉奖项
    /// </summary>
    public Button Honor;
    /// <summary>
    /// 产品体系
    /// </summary>
    public Button Product;
    /// <summary>
    /// 服务体系
    /// </summary>
    public Button Service;

    public List<Texture2D> IntroduceTexs;

    public List<Texture2D> InfoTexs;

    public List<Texture2D> ShareholderTexs;

    public List<Texture2D> HonorTexs;

    public List<Texture2D> ProductTexs;

    public List<Texture2D> ServiceTexs;

    /// <summary>
    /// 当前选中的贴图集合  
    /// </summary>
    private List<Texture2D> _curTex;

    /// <summary>
    /// 显示内容的贴图  
    /// </summary>
    private RawImage ShowImage;

    private List<Image> _highlights;

    private int _curIndex;
    private Transform _previous;
    private Transform _next;

    public CompanyIntroductionFSM(Transform go, params  object[] args)
        : base(go)
    {


        _highlights = new List<Image>();

        Introduce = Parent.transform.Find("1集团介绍").GetComponent<Button>();
        Info = Parent.transform.Find("2基本信息").GetComponent<Button>();
        Shareholder = Parent.transform.Find("3股东概况").GetComponent<Button>();
        Honor = Parent.transform.Find("4荣誉奖项").GetComponent<Button>();
        Product = Parent.transform.Find("5产品体系").GetComponent<Button>();
        Service = Parent.transform.Find("6服务体系").GetComponent<Button>();

        IntroduceTexs = PictureHandle.Instance.CompanyAllTexList[0].TexInfo;
        InfoTexs = PictureHandle.Instance.CompanyAllTexList[1].TexInfo;
        ShareholderTexs = PictureHandle.Instance.CompanyAllTexList[2].TexInfo;
        HonorTexs = PictureHandle.Instance.CompanyAllTexList[3].TexInfo;
        ProductTexs = PictureHandle.Instance.CompanyAllTexList[4].TexInfo;
        ServiceTexs = PictureHandle.Instance.CompanyAllTexList[5].TexInfo;


        ShowImage = Parent.parent.Find("ShowImage").GetComponent<RawImage>();

        Introduce.onClick.AddListener((() =>
        {
            SetBtn(IntroduceTexs);
            SetHighlight(Introduce.transform);
        }));
        Introduce.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.IcOne);

        Info.onClick.AddListener((() =>
        {
            SetBtn(InfoTexs);
            SetHighlight(Info.transform);

        }));
        Info.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.IcTwo);

        Shareholder.onClick.AddListener((() =>
        {
            SetBtn(ShareholderTexs);
            SetHighlight(Shareholder.transform);

        }));
        Shareholder.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.IcThree);


        Honor.onClick.AddListener((() =>
        {
            SetBtn(HonorTexs);
            SetHighlight(Honor.transform);

        }));
        Honor.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.IcFour);

        Product.onClick.AddListener((() =>
        {
            SetBtn(ProductTexs);
            SetHighlight(Product.transform);
        }));
        Product.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.IcFive);

        Service.onClick.AddListener((() =>
        {
            SetBtn(ServiceTexs);
            SetHighlight(Service.transform);
        }));
        Service.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.IcSix);

        _highlights.Add(Introduce.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(Info.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(Shareholder.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(Honor.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(Product.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(Service.transform.Find("Image").GetComponent<Image>());

        SetHighlight(Introduce.transform);

        AddVideoTex(IntroduceTexs, PictureHandle.Instance.CompanyAllTexList[0].VideoInfo);
        AddVideoTex(InfoTexs, PictureHandle.Instance.CompanyAllTexList[1].VideoInfo);
        AddVideoTex(ShareholderTexs, PictureHandle.Instance.CompanyAllTexList[2].VideoInfo);
        AddVideoTex(HonorTexs, PictureHandle.Instance.CompanyAllTexList[3].VideoInfo);
        AddVideoTex(ProductTexs, PictureHandle.Instance.CompanyAllTexList[4].VideoInfo);
        AddVideoTex(ServiceTexs, PictureHandle.Instance.CompanyAllTexList[5].VideoInfo);
    }

    private void SetBtn(List<Texture2D> texs)
    {
        _curTex = texs;
        _curIndex = 0;
        ShowImage.texture = _curTex[_curIndex];
        CheckVideoTex(null, ShowImage.gameObject);

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
    public override void Enter()
    {
        base.Enter();

        _previous = Target.transform.Find("CompanyIntroduction/Previous");

        _next = Target.transform.Find("CompanyIntroduction/Next");

        EventTriggerListener.Get(_previous.gameObject).SetEventHandle(EnumTouchEventType.OnClick, Previous);

        EventTriggerListener.Get(_next.gameObject).SetEventHandle(EnumTouchEventType.OnClick, Next);


        _curTex = IntroduceTexs;
        Introduce.onClick.Invoke();
        Parent.parent.gameObject.SetActive(true);//父级别也要显示

     
    }

    private void Next(GameObject _listener, object _args, params object[] _params)
    {
        _curIndex++;
        if (_curIndex >= _curTex.Count)
        {
            _curIndex--;
        }
      
        ShowImage.texture = _curTex[_curIndex];
        CheckVideoTex(_curTex[_curIndex], ShowImage.gameObject);
        if (_curIndex == _curTex.Count-1)
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

    private void Previous(GameObject _listener, object _args, params object[] _params)
    {
        //Debug.Log("previous");
        _curIndex--;
        if (_curIndex < 0)
        {
            _curIndex = 0;
        }

        ShowImage.texture = _curTex[_curIndex];
        CheckVideoTex(_curTex[_curIndex], ShowImage.gameObject);
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
        base.Exit();

    }


}
