using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A Canvas placable element that can be used to modify children Alpha, Raycasting, Enabled state.</para>
	/// </summary>
	[NativeClass("UI::CanvasGroup")]
	[NativeHeader("Runtime/UI/CanvasGroup.h")]
	public sealed class CanvasGroup : Component, ICanvasRaycastFilter
	{
		/// <summary>
		///   <para>Set the alpha of the group.</para>
		/// </summary>
		[NativeProperty("Alpha", false, TargetType.Function)]
		public extern float alpha
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is the group interactable (are the elements beneath the group enabled).</para>
		/// </summary>
		[NativeProperty("Interactable", false, TargetType.Function)]
		public extern bool interactable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Does this group block raycasting (allow collision).</para>
		/// </summary>
		[NativeProperty("BlocksRaycasts", false, TargetType.Function)]
		public extern bool blocksRaycasts
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Should the group ignore parent groups?</para>
		/// </summary>
		[NativeProperty("IgnoreParentGroups", false, TargetType.Function)]
		public extern bool ignoreParentGroups
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns true if the Group allows raycasts.</para>
		/// </summary>
		/// <param name="sp"></param>
		/// <param name="eventCamera"></param>
		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return this.blocksRaycasts;
		}
	}
}
