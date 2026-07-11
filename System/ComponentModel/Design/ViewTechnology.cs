using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public enum ViewTechnology
	{
		[Obsolete("Use ViewTechnology.Default.")]
		Passthrough,
		[Obsolete("Use ViewTechnology.Default.")]
		WindowsForms,
		Default
	}
}
