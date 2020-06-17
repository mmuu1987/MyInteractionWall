using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 渐变特效  ，目前是透明度渐变
/// </summary>
public class GradientEffect : MonoBehaviour
{

    /// <summary>
    /// 初始的时候是否全部透明
    /// </summary>
    public bool IsAlpha = true;
    private List<MaskableGraphic> _mgs;

    private bool _isEnableGradient = false;

    private Action _completeCallBack;

    private bool _isCompleted = false;

    private void Awake()
    {

     
        MaskableGraphic[] temp = transform.GetComponentsInChildren<MaskableGraphic>(true);

       
        if (null != temp && temp.Length != 0)
        {
            _isEnableGradient = true;

            _mgs = new List<MaskableGraphic>(temp);
        }

        if (IsAlpha)
        {
            foreach (MaskableGraphic graphic in temp)
            {
                graphic.color = new Color(graphic.color.r, graphic.color.g,graphic.color.b,0f);
            }
        }
    }

    public void DoFade(float target,float speed,Ease easeType,Action completeCallBack)
    {
        if (!_isEnableGradient) return;

        _isCompleted = false;
        _completeCallBack = completeCallBack;

        foreach (MaskableGraphic maskableGraphic in _mgs)
        {
            Color color = maskableGraphic.color;
            maskableGraphic.DOKill();
            maskableGraphic.DOColor(new Color(color.r, color.g, color.b, target), speed).SetEase(easeType).OnComplete((
                () => { _isCompleted = true; }));
        }
        
    }

    public void SetValue(float value)
    {
        if (!_isEnableGradient) return;

        foreach (MaskableGraphic maskableGraphic in _mgs)
        {
            Color color = maskableGraphic.color;
            maskableGraphic.color = new Color(color.r,color.g,color.b,value);
        }
    }

    private void Update()
    {
        if (_isCompleted && _completeCallBack != null)
        {
            _completeCallBack();
            _isCompleted = false;
        }
    }

   
}
