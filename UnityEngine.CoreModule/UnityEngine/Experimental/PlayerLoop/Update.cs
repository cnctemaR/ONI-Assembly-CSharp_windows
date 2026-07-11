using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop
{
	/// <summary>
	///   <para>Update phase in the native player loop.</para>
	/// </summary>
	[RequiredByNativeCode]
	public struct Update
	{
		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ScriptRunBehaviourUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct DirectorUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ScriptRunDelayedDynamicFrameRate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ScriptRunDelayedTasks
		{
		}
	}
}
