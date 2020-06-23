using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 显示角色的详细内容和一些UI动画
/// </summary>
public class HeadItem : MonoBehaviour
{


    public RectTransform AnimatonRt;

    public RectTransform RectTransform;

    private Text _personName;

    private Text _personInfo;

    private Image _head;

    private Coroutine _coroutine;

    private void Awake()
    {
        RectTransform = this.GetComponent<RectTransform>();

        _personName = this.transform.Find("HeadItem/Parent/PersonName").GetComponent<Text>();

        _personInfo = this.transform.Find("HeadItem/Parent/Text").GetComponent<Text>();

        _head = this.transform.Find("head").GetComponent<Image>(); 
    }
    private void Start()
    {
        
        DoAnimation();
    }

    public void DoAnimation()
    {
        AnimatonRt.DOSizeDelta(new Vector2(668, 1120f), 0.75f).SetEase(Ease.InCubic).SetDelay(0.85f);
        
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    public void SetData(PersonInfo info)
    {
        _personName.text = info.PersonName;

        _personInfo.text = info.Describe;

        _head.sprite = info.headTex;

        StartCoroutine(Common.WaitTime(10f, (() =>
        {
            Destroy(this.gameObject);
        })));
    }
}
