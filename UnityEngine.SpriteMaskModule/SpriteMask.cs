using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A component for masking Sprites and Particles.</para>
	/// </summary>
	[NativeType(Header = "Modules/SpriteMask/Public/SpriteMask.h")]
	[RejectDragAndDropMaterial]
	public sealed class SpriteMask : Renderer
	{
		/// <summary>
		///   <para>Unique ID of the sorting layer defining the start of the custom range.</para>
		/// </summary>
		public extern int frontSortingLayerID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Order within the front sorting layer defining the start of the custom range.</para>
		/// </summary>
		public extern int frontSortingOrder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Unique ID of the sorting layer defining the end of the custom range.</para>
		/// </summary>
		public extern int backSortingLayerID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Order within the back sorting layer defining the end of the custom range.</para>
		/// </summary>
		public extern int backSortingOrder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The minimum alpha value used by the mask to select the area of influence defined over the mask's sprite.</para>
		/// </summary>
		public extern float alphaCutoff
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The Sprite used to define the mask.</para>
		/// </summary>
		public extern Sprite sprite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Mask sprites from front to back sorting values only.</para>
		/// </summary>
		public extern bool isCustomRangeActive
		{
			[NativeMethod("IsCustomRangeActive")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetCustomRangeActive")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Determines the position of the Sprite used for sorting the SpriteMask.</para>
		/// </summary>
		public extern SpriteSortPoint spriteSortPoint
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal Bounds GetSpriteBounds()
		{
			Bounds bounds;
			this.GetSpriteBounds_Injected(out bounds);
			return bounds;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetSpriteBounds_Injected(out Bounds ret);
	}
}
