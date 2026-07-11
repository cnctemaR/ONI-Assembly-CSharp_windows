using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Tree Component for the tree creator.</para>
	/// </summary>
	[NativeHeader("Modules/Terrain/Public/Tree.h")]
	public sealed class Tree : Component
	{
		/// <summary>
		///   <para>Data asociated to the Tree.</para>
		/// </summary>
		[NativeProperty("TreeData")]
		public extern ScriptableObject data
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Tells if there is wind data exported from SpeedTree are saved on this component.</para>
		/// </summary>
		public extern bool hasSpeedTreeWind
		{
			[NativeMethod("HasSpeedTreeWind")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}
	}
}
