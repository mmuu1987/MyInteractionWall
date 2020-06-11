//
// /**************************************************************************
//
// Singleton.cs
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

using System;
using UnityEngine;

namespace XHFrameWork
{
	#region Singleton<> first version
	//public abstract class Singleton<T> where T : class, new() {
	//
	//    public static readonly T Instance = new T();
	//
	//    public virtual void Init(){}
	// 
	//}
	#endregion
	
	#region Singleton<> second version
	/// <summary>
	/// Generic C# singleton.
	/// </summary>
	public abstract class Singleton<T> where T : class, new() {

		/// <summary>
		/// The m_ instance.
		/// </summary>
		protected static T _Instance = null;

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static T Instance
		{
			get
			{ 
				if (null == _Instance)
				{
					_Instance = new T();
				}
				return _Instance; 
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="XHEngine.Singleton`1"/> class.
		/// </summary>
		protected Singleton()
		{
			if(null != _Instance)
				throw new SingletonException("This " + (typeof(T)).ToString() + " Singleton Instance is not null !!!");
			Init ();
		}


		/// <summary></summary>
		/// Init this Singleton.
		/// </summary>
		public virtual void Init() {}
	}
	#endregion
}
