using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemTest : MonoBehaviour,IDragHandler,IPointerClickHandler
{


    private RectTransform _rectTransform;

    private YearsEvent _yearsEvent;

    private Text _yearText;

    private Image _image;

    private Text _describe;

    private List<int> textureIndex;

    
	// Use this for initialization

    private void Awake()
    {
        _yearText = this.transform.Find("YearText").GetComponent<Text>();
        _image = this.transform.Find("Image").GetComponent<Image>();
        _describe = this.transform.Find("Describe").GetComponent<Text>();


        this.transform.Find("previous").GetComponent<Button>().onClick.AddListener((() =>
         {
             Debug.Log("previous");
         }));
         this.transform.Find("next").GetComponent<Button>().onClick.AddListener((() =>
         {
             Debug.Log("next");
         }));
    }
	void Start ()
	{
	    _rectTransform = this.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 clickPos = eventData.delta;

        _rectTransform.position += clickPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       // Debug.Log("OnPointerClick");
    }

    /// <summary>
    /// 导入数据
    /// </summary>
    public void LoadData(YearsEvent yearsEvent)
    {
        _yearsEvent = yearsEvent;

        _yearText.text = _yearsEvent.Years;

        _describe.text = _yearsEvent.Describe;
    }
}
