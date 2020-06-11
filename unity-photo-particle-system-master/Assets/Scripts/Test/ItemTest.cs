using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemTest : MonoBehaviour,IDragHandler,IPointerClickHandler {

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnDrag(PointerEventData eventData)
    {
       // Debug.Log(eventData.position);
        this.GetComponent<RectTransform>().position = eventData.position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       // Debug.Log("OnPointerClick");
    }
}
