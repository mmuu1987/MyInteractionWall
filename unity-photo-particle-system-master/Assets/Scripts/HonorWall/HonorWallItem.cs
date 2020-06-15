using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XHFrameWork;

public class HonorWallItem : MonoBehaviour
{

  

    public float speed = 10f;

    private float itemHeight;

    private List<Image> images;

    public float MinPos = -612.666f;

    public float MaxPos =3306.6f;

    private void Awake()
    {
        

        Image[] tems = this.transform.GetComponentsInChildren<Image>();

        images = new List<Image>(tems);

        itemHeight = images[0].rectTransform.sizeDelta.y;

        foreach (Image image in images)
        {
            EventTriggerListener.Get(image.gameObject).SetEventHandle(EnumTouchEventType.OnClick, OnClick);
        }
    }

    private void OnClick(GameObject _listener, object _args, params object[] _params)
    {
        Sprite sprite = _listener.GetComponent<Image>().sprite;

        if (sprite != null)
        {
            int index = int.Parse(sprite.name);

            Debug.Log("index is " + index);
        }
        else Debug.Log(_listener.name);

      

    }

    private void Update()
    {
        foreach (Image image in images)
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

                if (image.rectTransform.anchoredPosition.y <= -itemHeight)
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
