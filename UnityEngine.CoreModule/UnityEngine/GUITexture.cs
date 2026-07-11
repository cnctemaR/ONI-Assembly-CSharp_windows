using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[Obsolete("This component is part of the legacy UI system and will be removed in a future release.")]
	[NativeHeader("Runtime/Camera/RenderLayers/GUITexture.h")]
	public class GUITexture : GUIElement
	{
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

		public extern Texture texture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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
