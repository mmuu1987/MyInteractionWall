//
// /**************************************************************************
//
// MethodExtension.cs
//
// Author: xiaohong  <704872627@qq.com>
//
// Unity课程讨论群:  152767675
//
// Date: 15-8-25
//
// Description:Provide  functions  to connect Oracle
//
// Copyright (c) 2015 xiaohong
//
// **************************************************************************/

using System;
using UnityEngine;


namespace XHFrameWork
{
	static public class MethodExtension
	{
		/// <summary>
		/// Gets the or add component.
		/// </summary>
		/// <returns>The or add component.</returns>
		/// <param name="go">Go.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static public T GetOrAddComponent<T>(this GameObject go) where T : Component
		{
			T ret = go.GetComponent<T>();
			if (null == ret)
				ret = go.AddComponent<T>();
			return ret;
		}

	}
}

