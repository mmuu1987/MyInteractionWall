//
// /**************************************************************************
//
// DDOLSingleton.cs
//
// Author: xiaohong  <704872627@qq.com>
//
// Unity课程讨论群:  152767675
//
// Date: 15-8-6
//
// Description:Provide  functions  to connect Oracle
//
// Copyright (c) 2015 xiaohong
//
// **************************************************************************/
using UnityEngine;
using System.Collections;

/// <summary>
/// DDOL singleton.
/// </summary>
public abstract class DDOLSingleton<T> : MonoBehaviour where T : DDOLSingleton<T>
{
	protected static T _Instance = null;
	
	public static T Instance
	{
		get{
			if (null == _Instance)
			{
				GameObject go = GameObject.Find("DDOLGameObject");
				if (null == go)
				{
					go = new GameObject("DDOLGameObject");
					DontDestroyOnLoad(go);
				}
				_Instance = go.AddComponent<T>();

			}
			return _Instance;
		}
	}

	/// <summary>
	/// Raises the application quit event.
	/// </summary>
	private void OnApplicationQuit ()
	{
		_Instance = null;
	}
}

