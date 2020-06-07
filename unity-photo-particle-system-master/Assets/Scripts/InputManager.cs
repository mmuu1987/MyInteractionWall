using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入管理器
/// </summary>
public class InputManager : MonoBehaviour {


    public static InputManager Instance;

    /// <summary>
    /// 右键点击事件
    /// </summary>
    public Action MouseButtonDownAction;
    /// <summary>
    /// 鼠标动作状态 0为弹起后无操作状态,1为按下，2为按下中,3为滚轮状态滚上，4为滚轮状态滚下,
    /// </summary>
    private int _inputState = 0;

    /// <summary>
    /// 鼠标动作状态 0为弹起后无操作状态,1为按下，2为按下中,3，4为滚轮状态 5 弹起状态,
    /// </summary>
    public int InputState
    {
        get { return _inputState; }
    }

    private void Awake()
    {
        if (Instance != null) throw new UnityException("不允许重复制造单例");
        Instance = this;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private Coroutine _coroutine;
    public void HandleInput()
    {
        _inputState = 0;
        if (Input.GetMouseButton(0)) _inputState = 2;
        if (Input.GetMouseButtonDown(0)) _inputState = 1;
        if (Input.GetMouseButtonUp(0)) _inputState = 0;

       
           

        GetGetAxis("Mouse ScrollWheel");


    }

    public bool GetMouseButtonDown(int id)
    {
        if (Input.GetMouseButtonDown(id)) return true;
        return false;
    }

    public bool GetMouseButton(int id)
    {
        if (Input.GetMouseButton(id)) return true;
        return false;
    }
    /// <summary>
    /// 滚轮没有运动后0.5f回复 _inputState = 0;状态
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaiTime()
    {
        yield return  new WaitForSeconds(0.5f);
        //_inputState = 0;
    }
    public int GetGetAxis(string input)
    {
        if (Input.GetAxis(input) > 0)
        {
            _inputState = 3;
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(WaiTime());
            return 1;
        }
        if (Input.GetAxis(input) < 0)
        {
            _inputState = 4;
            if (_coroutine != null) StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(WaiTime());
            return -1;
        }
        return 0;
    }
}
