//
// /**************************************************************************
//
// IDynamicProperty.cs
//
// Author: xiaohong  <704872627@qq.com>
//
// Unity课程讨论群:  152767675
//
// Date: 15-9-6
//
// Description:Provide  functions  to connect Oracle
//
// Copyright (c) 2015 xiaohong
//
// **************************************************************************/

using System;

namespace XHFrameWork
{
	public interface IDynamicProperty
	{
		void DoChangeProperty(int id, object oldValue, object newValue);
		PropertyItem GetProperty(int id);
	}
}

