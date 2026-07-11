using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A script interface for the.</para>
	/// </summary>
	[RequireComponent(typeof(Transform), typeof(MeshRenderer))]
	[NativeClass("TextRenderingPrivate::TextMesh")]
	[NativeHeader("Modules/TextRendering/Public/TextMesh.h")]
	public sealed class TextMesh : Component
	{
		/// <summary>
		///   <para>The text that is displayed.</para>
		/// </summary>
		public extern string text
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The Font used.</para>
		/// </summary>
		public extern Font font
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
		///   <para>How far should the text be offset from the transform.position.z when drawing.</para>
		/// </summary>
		public extern float offsetZ
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How lines of text are aligned (Left, Right, Center).</para>
		/// </summary>
		public extern TextAlignment alignment
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Which point of the text shares the position of the Transform.</para>
		/// </summary>
		public extern TextAnchor anchor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The size of each character (This scales the whole text).</para>
		/// </summary>
		public extern float characterSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much space will be in-between lines of text.</para>
		/// </summary>
		public extern float lineSpacing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much space will be inserted for a tab '\t' character. This is a multiplum of the 'spacebar' character offset.</para>
		/// </summary>
		public extern float tabSize
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_color_Injected(out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_color_Injected(ref Color value);
	}
}
