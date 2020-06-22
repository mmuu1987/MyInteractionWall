using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 大屏互动UI控制器
/// </summary>
public class UIControl : MonoBehaviour
{

    public Button HonorWallBtn;

    /// <summary>
    /// 荣誉墙
    /// </summary>
    public RectTransform HonorWall;
    
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
