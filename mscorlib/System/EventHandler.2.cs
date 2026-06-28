using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public delegate void EventHandler(object sender, EventArgs e);
}
