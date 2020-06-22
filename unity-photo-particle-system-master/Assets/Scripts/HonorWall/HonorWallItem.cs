﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XHFrameWork;

public class HonorWallItem : MonoBehaviour
{

  

    public float speed = 10f;

    private List<Image> _images;

    public float MinPos = -612.666f;

    public float MaxPos =3306.6f;

    /// <summary>
    /// 可以点击的
    /// </summary>
    public List<Image>  ButtonImages= new List<Image>();

    private void Awake()
    {
        

        Image[] tems = this.transform.GetComponentsInChildren<Image>();

        _images = new List<Image>(tems);

   

        foreach (Image image in _images)
        {
            if (image.sprite == null)
            {
                EventTriggerListener.Get(image.gameObject).SetEventHandle(EnumTouchEventType.OnClick, OnClick);
                ButtonImages.Add(image);
            }
        }
    }

    private void OnClick(GameObject _listener, object _args, params object[] _params)
    {
        Sprite sprite = _listener.GetComponent<Image>().sprite;

        if (sprite != null && sprite.name!="logo")
        {
            int index = int.Parse(_listener.name);

            Debug.Log("index is " + index);
        }
        else Debug.Log(_listener.name);

      

    }

    private void Update()
    {
        foreach (Image image in _images)
        {

            Vector3 pos = image.rectTransform.anchoredPosition;

            if (speed>0)
            {
                pos.y += Time.deltaTime*speed;

                image.rectTransform.anchoredPosition = pos;

                if (image.rectTransform.anchoredPosition.y >= 3240f)
                {
                    image.rectTransform.anchoredPosition = new Vector3(pos.x, MinPos, pos.z);
                }
            }
            else
            {

                pos.y += Time.deltaTime * speed;

                image.rectTransform.anchoredPosition = pos;

                if (image.rectTransform.anchoredPosition.y <= MinPos)
                {
                    image.rectTransform.anchoredPosition = new Vector3(pos.x, MaxPos, pos.z);
                }
            }
        }
    }
    /// <summary>
    /// 加载荣誉数据
    /// </summary>
    public void LoadData()
    {
        
    }

	
}
