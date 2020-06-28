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
    /// 荣誉墙
    /// </summary>
    public RectTransform HonorWall;

    public UIStateMachine _Machine;

    public Dictionary<UIState, UIStateFSM> DicUI;
    private void Awake()
    {
        DicUI = new Dictionary<UIState, UIStateFSM>();

        _Machine = new UIStateMachine(this);

        DicUI.Add(UIState.CompanyIntroduction, new CompanyIntroductionFSM(this.transform.Find("CompanyIntroduction/CompanyIntroduction")));
        DicUI.Add(UIState.PrivateHeirs, new PrivateHeirsFSM(null));
        DicUI.Add(UIState.OutstandingStyle, new OutstandingStyleFSM(null) );
        DicUI.Add(UIState.Close, new CloseFSM(null));

        _Machine.SetCurrentState(DicUI[UIState.Close]);

        CompanyIntroductionBtn.onClick.AddListener((() =>
        {
            _Machine.ChangeState(DicUI[UIState.CompanyIntroduction]);
        }));

        CloseButton.onClick.AddListener((() =>
        {
            _Machine.ChangeState(DicUI[UIState.Close]);
        }));
    }
    
	// Use this for initialization
	void Start () 
    {
        HonorWallBtn.onClick.AddListener((() =>
        {

            RectTransform btRt = HonorWallBtn.GetComponent<RectTransform>();

           
		    if (HonorWall.position.x < 0)
		    {
		        HonorWall.DOLocalMoveX(0f, 0.5f).SetEase(Ease.InOutQuad);
                btRt.DOAnchorPosX(245.5f, 0.5f).SetEase(Ease.InOutQuad);
		    }
		    else
		    {
		        HonorWall.DOLocalMoveX(-7680, 0.5f).SetEase(Ease.InOutQuad);
                btRt.DOAnchorPosX(0f, 0.5f).SetEase(Ease.InOutQuad);
		    }

          
		}));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
