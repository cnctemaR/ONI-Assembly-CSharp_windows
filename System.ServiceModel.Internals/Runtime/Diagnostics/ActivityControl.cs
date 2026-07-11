using System;

namespace System.Runtime.Diagnostics
{
	internal enum ActivityControl : uint
	{
		EVENT_ACTIVITY_CTRL_GET_ID = 1U,
		EVENT_ACTIVITY_CTRL_SET_ID,
		EVENT_ACTIVITY_CTRL_CREATE_ID,
		EVENT_ACTIVITY_CTRL_GET_SET_ID,
		EVENT_ACTIVITY_CTRL_CREATE_SET_ID
	}
}
