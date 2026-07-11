using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A text string displayed in a GUI.</para>
	/// </summary>
	[Obsolete("This component is part of the legacy UI system and will be removed in a future release.", false)]
	[NativeClass("TextRenderingPrivate::GUIText")]
	[NativeHeader("Runtime/Shaders/Material.h")]
	[NativeHeader("Modules/TextRendering/Public/GUIText.h")]
	public sealed class GUIText : GUIElement
	{
		/// <summary>
		///   <para>The text to display.</para>
		/// </summary>
		public extern string text
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The Material to use for rendering.</para>
		/// </summary>
		public extern Material material
		{
			[FreeFunction("TextRenderingPrivate::GetGUITextMaterialWithFallback", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The font used for the text.</para>
		/// </summary>
		public extern Font font
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The alignment of the text.</para>
		/// </summary>
		public extern TextAlignment alignment
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The anchor of the text.</para>
		/// </summary>
		public extern TextAnchor anchor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The line spacing multiplier.</para>
		/// </summary>
		public extern float lineSpacing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The tab width multiplier.</para>
		/// </summary>
		public extern float tabSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The font size to use (for dynamic fonts).</para>
		/// </summary>
		public extern int fontSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The font style to use (for dynamic fonts).</para>
		/// </summary>
		public extern FontStyle fontStyle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enable HTML-style tags for Text Formatting Markup.</para>
		/// </summary>
		public extern bool richText
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The color used to render the text.</para>
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
		///   <para>The pixel offset of the text.</para>
		/// </summary>
		public Vector2 pixelOffset
		{
			get
			{
				Vector2 vector;
				this.get_pixelOffset_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_pixelOffset_Injected(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_color_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_color_Injected(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_pixelOffset_Injected(out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_pixelOffset_Injected(ref Vector2 value);
	}
}
