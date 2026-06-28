using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class WaitForSeconds : YieldInstruction
	{
		public WaitForSeconds(float seconds)
		{
			this.m_Seconds = seconds;
		}

		internal float m_Seconds;
	}
}
