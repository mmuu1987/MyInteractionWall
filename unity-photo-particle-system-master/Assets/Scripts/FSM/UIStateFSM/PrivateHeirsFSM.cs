using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private List<Texture2D> _brandTex;

  

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

  

   
    private List<Image> _highlights;

    private Transform _previous;
    private Transform _next;

   
    public PrivateHeirsFSM(Transform go) : base(go)
    {
        _highlights = new List<Image>();

     

        BrandIntroductionBtn = Parent.transform.Find("1品牌介绍").GetComponent<Button>();

        ValueAddedServices = Parent.transform.Find("2尊享服务").GetComponent<Button>();

        DawanDistrictBtn = Parent.transform.Find("3大湾区高净值中心").GetComponent<Button>();

        ShowImage = Parent.parent.Find("ShowImage").GetComponent<RawImage>();

        _brandTex = PictureHandle.Instance.PrivateHeirsAllTexList[0].TexInfo;

        DawanTex = PictureHandle.Instance.PrivateHeirsAllTexList[1].TexInfo;

        ValueAddTex = PictureHandle.Instance.PrivateHeirsAllTexList[2].TexInfo;

    


      

        BrandIntroductionBtn.onClick.AddListener((() =>
        {
            SetBtn(_brandTex);
            SetHighlight(BrandIntroductionBtn.transform);
        }));
        BrandIntroductionBtn.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.PhOne);

        DawanDistrictBtn.onClick.AddListener((() =>
        {
            SetBtn(DawanTex);
            SetHighlight(DawanDistrictBtn.transform);
        }));
        DawanDistrictBtn.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.PhTwo);

        ValueAddedServices.onClick.AddListener((() =>
        {
            SetBtn(ValueAddTex);
            SetHighlight(ValueAddedServices.transform);
        }));
        ValueAddedServices.transform.Find("Text").GetComponent<Text>().text = SettingManager.Instance.GetDirectName(Direct.PhThree);
       

        _highlights.Add(BrandIntroductionBtn.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(ValueAddedServices.transform.Find("Image").GetComponent<Image>());
        _highlights.Add(DawanDistrictBtn.transform.Find("Image").GetComponent<Image>());


        SetHighlight(BrandIntroductionBtn.transform);


        AddVideoTex(_brandTex, PictureHandle.Instance.PrivateHeirsAllTexList[0].VideoInfo);
        AddVideoTex(DawanTex, PictureHandle.Instance.PrivateHeirsAllTexList[1].VideoInfo);
        AddVideoTex(ValueAddTex, PictureHandle.Instance.PrivateHeirsAllTexList[2].VideoInfo);
      

    }

  

    public override void Enter()
    {
        base.Enter();

        _previous = Target.transform.Find("CompanyIntroduction/Previous");

        _next = Target.transform.Find("CompanyIntroduction/Next");

        EventTriggerListener.Get(_previous.gameObject).SetEventHandle(EnumTouchEventType.OnClick, Previous);

        EventTriggerListener.Get(_next.gameObject).SetEventHandle(EnumTouchEventType.OnClick, Next);

        _curTex = _brandTex;
        BrandIntroductionBtn.onClick.Invoke();
        Parent.parent.gameObject.SetActive(true);//父级别也要显示
      


     
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
    private void Next(GameObject _listener, object _args, params object[] _params)
    {
        _curIndex++;
        if (_curIndex >= _curTex.Count)
        {
            _curIndex--;
        }

        ShowImage.texture = _curTex[_curIndex];
        CheckVideoTex(_curTex[_curIndex], ShowImage.gameObject);
        

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
        _videoPlayer.enabled = false;
        base.Exit();

        Parent.gameObject.SetActive(false);

    }
}
