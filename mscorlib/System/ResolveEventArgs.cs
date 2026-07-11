using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public class ResolveEventArgs : EventArgs
	{
		public ResolveEventArgs(string name)
		{
			this.m_Name = name;
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		private string m_Name;
	}
}
