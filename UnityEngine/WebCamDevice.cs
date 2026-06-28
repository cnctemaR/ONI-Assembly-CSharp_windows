using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public struct WebCamDevice
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public bool isFrontFacing
		{
			get
			{
				return (this.m_Flags & 1) == 1;
			}
		}

		internal string m_Name;

		internal int m_Flags;
	}
}
