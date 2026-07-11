using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A texture image used in a 2D GUI.</para>
	/// </summary>
	[NativeHeader("Runtime/Camera/RenderLayers/GUITexture.h")]
	[Obsolete("This component is part of the legacy UI system and will be removed in a future release.")]
	public class GUITexture : GUIElement
	{
		/// <summary>
		///   <para>The color of the GUI texture.</para>
		/// </summary>
		public Color color
		{
			get
			{
				Color color;
				this.get_color_Injected(out color);
				return color;
			}
			set
			{
				this.set_color_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The texture used for drawing.</para>
		/// </summary>
		public extern Texture texture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Pixel inset used for pixel adjustments for size and position.</para>
		/// </summary>
		public Rect pixelInset
		{
			get
			{
				Rect rect;
				this.get_pixelInset_Injected(out rect);
				return rect;
			}
			set
			{
				this.set_pixelInset_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>The border defines the number of pixels from the edge that are not affected by scale.</para>
		/// </summary>
		public extern RectOffset border
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_color_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_color_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_pixelInset_Injected(out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_pixelInset_Injected(ref Rect value);
	}
}
