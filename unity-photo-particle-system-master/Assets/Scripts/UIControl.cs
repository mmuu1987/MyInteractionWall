using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;


public enum UIState
{
    
    None,
    /// <summary>
    /// 关闭界面状态
    /// </summary>
    Close,
    /// <summary>
    /// 公司介绍
    /// </summary>
    CompanyIntroduction,
    /// <summary>
    /// 私享传家
    /// </summary>
    PrivateHeirs,
    /// <summary>
    /// 卓越风采
    /// </summary>
    OutstandingStyle
}
/// <summary>
/// 大屏互动UI控制器
/// </summary>
public class UIControl : MonoBehaviour
{
    /// <summary>
    /// 荣誉墙按钮
    /// </summary>
    public Button HonorWallBtn;

    /// <summary>
    /// 公司介绍按钮
    /// </summary>
    public Button CompanyIntroductionBtn;

    /// <summary>
    /// 关闭公司介绍，私享传家  所共用的界面
    /// </summary>
    public Button CloseButton;
    /// <summary>
    /// 私享传家按钮
    /// </summary>
    public Button PrivateHeirsBtn;

    public Button OutstandingStyleBtn;

    public Button Btn2000_2009;

    public Button Btn2010_2019;

    public Button Btn2020;


    public MultiDepthMotion MultiDepthMotion;

    /// <summary>
    /// 荣誉墙
    /// </summary>
    public RectTransform HonorWall;

    public UIStateMachine _Machine;

    public Dictionary<UIState, UIStateFSM> DicUI;

    public Sprite HonorWallBtnLeft;
    public Sprite HonorWallBtnRight;

    private void Awake()
    {



        Screen.SetResolution(7680, 3240, true, 60);

        DicUI = new Dictionary<UIState, UIStateFSM>();

        _Machine = new UIStateMachine(this);

        DicUI.Add(UIState.CompanyIntroduction, new CompanyIntroductionFSM(this.transform.Find("CompanyIntroduction/CompanyIntroduction")));
        DicUI.Add(UIState.PrivateHeirs, new PrivateHeirsFSM(this.transform.Find("CompanyIntroduction/PrivateHeirs")));
        DicUI.Add(UIState.OutstandingStyle, new OutstandingStyleFSM(this.transform.Find("CompanyIntroduction/OutstandingStyle")));
        DicUI.Add(UIState.Close, new CloseFSM(null));

        _Machine.SetCurrentState(DicUI[UIState.Close]);

        CompanyIntroductionBtn.onClick.AddListener((() =>
        {
            _Machine.ChangeState(DicUI[UIState.CompanyIntroduction]);
        }));

        PrivateHeirsBtn.onClick.AddListener((() =>
        {
            _Machine.ChangeState(DicUI[UIState.PrivateHeirs]);
        }));

        OutstandingStyleBtn.onClick.AddListener((() =>
        {
            _Machine.ChangeState(DicUI[UIState.OutstandingStyle]);
        }));

        CloseButton.onClick.AddListener((() =>
        {
            _Machine.ChangeState(DicUI[UIState.Close]);
        }));
        Btn2000_2009.transform.Find("Tip").gameObject.SetActive(true);
        Btn2000_2009.onClick.AddListener((() =>
        {
            MultiDepthMotion.ChangeState(0);
            Btn2000_2009.transform.Find("Tip").gameObject.SetActive(true);
            Btn2010_2019.transform.Find("Tip").gameObject.SetActive(false);
            Btn2020.transform.Find("Tip").gameObject.SetActive(false);
        }));

        Btn2010_2019.onClick.AddListener((() =>
        {
            MultiDepthMotion.ChangeState(1);
            Btn2000_2009.transform.Find("Tip").gameObject.SetActive(false);
            Btn2010_2019.transform.Find("Tip").gameObject.SetActive(true);
            Btn2020.transform.Find("Tip").gameObject.SetActive(false);
        }));

        Btn2020.onClick.AddListener((() =>
        {
            MultiDepthMotion.ChangeState(2);
            Btn2000_2009.transform.Find("Tip").gameObject.SetActive(false);
            Btn2010_2019.transform.Find("Tip").gameObject.SetActive(false);
            Btn2020.transform.Find("Tip").gameObject.SetActive(true);
        }));
    }
    
	// Use this for initialization
	void Start () 
    {
        HonorWallBtn.onClick.AddListener((() =>
        {

            RectTransform btRt = HonorWallBtn.GetComponent<RectTransform>();


           
		    if (HonorWall.position.x < 0)//打开荣誉墙
		    {
		        HonorWall.DOLocalMoveX(0f, 0.5f).SetEase(Ease.InOutQuad);
                btRt.DOAnchorPosX(245.5f, 0.5f).SetEase(Ease.InOutQuad);
                HonorWallBtn.transform.Find("Image").GetComponent<Image>().sprite = HonorWallBtnLeft;

		        Item[] items = this.transform.GetComponentsInChildren<Item>();

                foreach (Item item in items)
		        {
		            Destroy(item.gameObject);
		        }
		    }
		    else//关闭荣誉墙
		    {
		        HonorWall.DOLocalMoveX(-7680, 0.5f).SetEase(Ease.InOutQuad);
                btRt.DOAnchorPosX(0f, 0.5f).SetEase(Ease.InOutQuad);
                HonorWallBtn.transform.Find("Image").GetComponent<Image>().sprite = HonorWallBtnRight;

                HeadItem[] items = this.transform.GetComponentsInChildren<HeadItem>();

                foreach (HeadItem item in items)
                {
                    Destroy(item.gameObject);
                }
		    }

          
		}));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
