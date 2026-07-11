using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Suspends the coroutine execution for the given amount of seconds using scaled time.</para>
	/// </summary>
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class WaitForSeconds : YieldInstruction
	{
		/// <summary>
		///   <para>Creates a yield instruction to wait for a given number of seconds using scaled time.</para>
		/// </summary>
		/// <param name="seconds"></param>
		public WaitForSeconds(float seconds)
		{
			this.m_Seconds = seconds;
		}

		internal float m_Seconds;
	}
}
