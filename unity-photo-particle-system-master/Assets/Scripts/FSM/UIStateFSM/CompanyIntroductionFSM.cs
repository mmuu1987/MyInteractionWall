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

    private int _curIndex;
    public CompanyIntroductionFSM(Transform go,params  object [] args) : base(go)
    {

        Introduce = Parent.transform.Find("1集团介绍").GetComponent<Button>();
        Info = Parent.transform.Find("2基本信息").GetComponent<Button>();
        Shareholder = Parent.transform.Find("3股东概况").GetComponent<Button>();
        Honor = Parent.transform.Find("4荣誉奖项").GetComponent<Button>();
        Product = Parent.transform.Find("5产品体系").GetComponent<Button>();
        Service = Parent.transform.Find("6服务体系").GetComponent<Button>();

        IntroduceTexs = PictureHandle.Instance.AllTexList[0];
        InfoTexs = PictureHandle.Instance.AllTexList[1];
        ShareholderTexs = PictureHandle.Instance.AllTexList[2];
        HonorTexs = PictureHandle.Instance.AllTexList[3];
        ProductTexs = PictureHandle.Instance.AllTexList[4];
        ServiceTexs = PictureHandle.Instance.AllTexList[5];


        ShowImage = Parent.parent.Find("ShowImage").GetComponent<RawImage>();

        Introduce.onClick.AddListener((() =>
        {
            SetBtn(IntroduceTexs);

        }));

        Info.onClick.AddListener((() =>
        {
            SetBtn(InfoTexs);

        })); 

        Shareholder.onClick.AddListener((() =>
        {
            SetBtn(ShareholderTexs);

        })); 

        Honor.onClick.AddListener((() =>
        {
            SetBtn(HonorTexs);

        })); 

        Product.onClick.AddListener((() =>
        {
            SetBtn(ProductTexs);

        })); 

        Service.onClick.AddListener((() =>
        {
            SetBtn(ServiceTexs);

        })); 

        
    }

    private void SetBtn(List<Texture2D> texs )
    {
        _curTex = texs;
        _curIndex = 0;
        ShowImage.texture = _curTex[_curIndex];

    }
    public override void Enter()
    {
        base.Enter();

        Transform previous = Target.transform.Find("CompanyIntroduction/Previous");

        Transform next = Target.transform.Find("CompanyIntroduction/Next");

        EventTriggerListener.Get(previous.gameObject).SetEventHandle(EnumTouchEventType.OnClick, Previous);

        EventTriggerListener.Get(next.gameObject).SetEventHandle(EnumTouchEventType.OnClick, Next);


        _curTex = IntroduceTexs;
        _curIndex = -1;
        Next(null,null);
        Parent.gameObject.SetActive(true);
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
    }

    private void Previous(GameObject _listener, object _args, params object[] _params)
    {
        //Debug.Log("previous");
        _curIndex--;
        if (_curIndex < 0) _curIndex = 0;

        ShowImage.texture = _curTex[_curIndex];
    }

    public override void Exit()
    {
        base.Exit();
    }

   
}
