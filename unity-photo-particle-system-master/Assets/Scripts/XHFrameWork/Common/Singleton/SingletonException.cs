//
// /**************************************************************************
//
// SingletonException.cs
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
namespace XHFrameWork
{
	public class SingletonException : System.Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SingletonException"/> class.
		/// </summary>
		/// <param name="msg">Message.</param>
		public SingletonException (string msg) : base(msg)
		{
		}
	}
}

