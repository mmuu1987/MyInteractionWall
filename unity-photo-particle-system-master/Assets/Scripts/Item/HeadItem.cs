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

   
    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        RectTransform = this.GetComponent<RectTransform>();

        _personName = this.transform.Find("HeadItem/Parent/PersonName").GetComponent<Text>();

        _personInfo = this.transform.Find("HeadItem/Parent/Describe").GetComponent<Text>();

        _head = this.transform.Find("head").GetComponent<Image>();


        this.transform.Find("HeadItem/Parent/Close").GetComponent<Button>().onClick.AddListener((() =>
        {
            Destroy(this.gameObject);
        }));

        this.transform.localScale = Vector3.one*0.1f;
    }
    private void Start()
    {
        
        DoAnimation();
    }

    public void DoAnimation()
    {
        AnimatonRt.DOSizeDelta(new Vector2(668, 1400f), 1f).SetEase(Ease.InCubic);

    }

    /// <summary>
    /// 设置数据
    /// </summary>
    public void SetData(PersonInfo info)
    {


        Debug.Log(info.PersonName + "    " + info.Describe);

        _personName.text = info.PersonName;

        _personInfo.text = info.Describe;

        _head.sprite = info.headTex;

        StartCoroutine(Common.WaitTime(40f, (() =>
        {
            Destroy(this.gameObject);
        })));
    }
}
