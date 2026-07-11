using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Behaviours are Components that can be enabled or disabled.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
	public class Behaviour : Component
	{
		/// <summary>
		///   <para>Enabled Behaviours are Updated, disabled Behaviours are not.</para>
		/// </summary>
		[RequiredByNativeCode]
		[NativeProperty]
		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Has the Behaviour had enabled called.</para>
		/// </summary>
		[NativeProperty]
		public extern bool isActiveAndEnabled
		{
			[NativeMethod("IsAddedToManager")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
