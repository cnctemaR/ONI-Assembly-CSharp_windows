using System;
using System.Reflection;
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

		public ResolveEventArgs(string name, Assembly requestingAssembly)
		{
			this.m_Name = name;
			this.m_Requesting = requestingAssembly;
		}

		public string Name
		{
			get
			{
				return this.m_Name;
			}
		}

		public Assembly RequestingAssembly
		{
			get
			{
				return this.m_Requesting;
			}
		}

		private string m_Name;

		private Assembly m_Requesting;
	}
}
