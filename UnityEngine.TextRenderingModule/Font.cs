using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Script interface for.</para>
	/// </summary>
	[NativeHeader("Modules/TextRendering/Public/Font.h")]
	[NativeHeader("Modules/TextRendering/Public/FontImpl.h")]
	[StaticAccessor("TextRenderingPrivate", StaticAccessorType.DoubleColon)]
	[NativeClass("TextRendering::Font")]
	public sealed class Font : Object
	{
		/// <summary>
		///   <para>Create a new Font.</para>
		/// </summary>
		/// <param name="name">The name of the created Font object.</param>
		public Font()
		{
			Font.Internal_CreateFont(this, null);
		}

		/// <summary>
		///   <para>Create a new Font.</para>
		/// </summary>
		/// <param name="name">The name of the created Font object.</param>
		public Font(string name)
		{
			Font.Internal_CreateFont(this, name);
		}

		private Font(string[] names, int size)
		{
			Font.Internal_CreateDynamicFont(this, names, size);
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Font> textureRebuilt;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Font.FontTextureRebuildCallback m_FontTextureRebuildCallback;

		/// <summary>
		///   <para>The material used for the font display.</para>
		/// </summary>
		public extern Material material
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string[] fontNames
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Is the font a dynamic font.</para>
		/// </summary>
		public extern bool dynamic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The ascent of the font.</para>
		/// </summary>
		public extern int ascent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The default size of the font.</para>
		/// </summary>
		public extern int fontSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Access an array of all characters contained in the font texture.</para>
		/// </summary>
		public extern CharacterInfo[] characterInfo
		{
			[FreeFunction("TextRenderingPrivate::GetFontCharacterInfo", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("TextRenderingPrivate::SetFontCharacterInfo", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The line height of the font.</para>
		/// </summary>
		[NativeProperty("LineSpacing", false, TargetType.Function)]
		public extern int lineHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("Font.textureRebuildCallback has been deprecated. Use Font.textureRebuilt instead.")]
		public Font.FontTextureRebuildCallback textureRebuildCallback
		{
			get
			{
				return this.m_FontTextureRebuildCallback;
			}
			set
			{
				this.m_FontTextureRebuildCallback = value;
			}
		}

		/// <summary>
		///   <para>Creates a Font object which lets you render a font installed on the user machine.</para>
		/// </summary>
		/// <param name="fontname">The name of the OS font to use for this font object.</param>
		/// <param name="size">The default character size of the generated font.</param>
		/// <param name="fontnames">Am array of names of OS fonts to use for this font object. When rendering characters using this font object, the first font which is installed on the machine, which contains the requested character will be used.</param>
		/// <returns>
		///   <para>The generate Font object.</para>
		/// </returns>
		public static Font CreateDynamicFontFromOSFont(string fontname, int size)
		{
			return new Font(new string[] { fontname }, size);
		}

		/// <summary>
		///   <para>Creates a Font object which lets you render a font installed on the user machine.</para>
		/// </summary>
		/// <param name="fontname">The name of the OS font to use for this font object.</param>
		/// <param name="size">The default character size of the generated font.</param>
		/// <param name="fontnames">Am array of names of OS fonts to use for this font object. When rendering characters using this font object, the first font which is installed on the machine, which contains the requested character will be used.</param>
		/// <returns>
		///   <para>The generate Font object.</para>
		/// </returns>
		public static Font CreateDynamicFontFromOSFont(string[] fontnames, int size)
		{
			return new Font(fontnames, size);
		}

		[RequiredByNativeCode]
		internal static void InvokeTextureRebuilt_Internal(Font font)
		{
			if (Font.textureRebuilt != null)
			{
				Font.textureRebuilt(font);
			}
			Font.FontTextureRebuildCallback fontTextureRebuildCallback = font.m_FontTextureRebuildCallback;
			if (fontTextureRebuildCallback != null)
			{
				fontTextureRebuildCallback();
			}
		}

		/// <summary>
		///   <para>Returns the maximum number of verts that the text generator may return for a given string.</para>
		/// </summary>
		/// <param name="str">Input string.</param>
		public static int GetMaxVertsForString(string str)
		{
			return str.Length * 4 + 4;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Font GetDefault();

		/// <summary>
		///   <para>Does this font have a specific character?</para>
		/// </summary>
		/// <param name="c">The character to check for.</param>
		/// <returns>
		///   <para>Whether or not the font has the character specified.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool HasCharacter(char c);

		/// <summary>
		///   <para>Get names of fonts installed on the machine.</para>
		/// </summary>
		/// <returns>
		///   <para>An array of the names of all fonts installed on the machine.</para>
		/// </returns>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetOSInstalledFontNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateFont([Writable] Font self, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateDynamicFont([Writable] Font self, string[] _names, int size);

		[FreeFunction("TextRenderingPrivate::GetCharacterInfo", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern bool GetCharacterInfo(char ch, out CharacterInfo info, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style);

		[ExcludeFromDocs]
		public bool GetCharacterInfo(char ch, out CharacterInfo info, int size)
		{
			return this.GetCharacterInfo(ch, out info, size, FontStyle.Normal);
		}

		[ExcludeFromDocs]
		public bool GetCharacterInfo(char ch, out CharacterInfo info)
		{
			return this.GetCharacterInfo(ch, out info, 0, FontStyle.Normal);
		}

		/// <summary>
		///   <para>Request characters to be added to the font texture (dynamic fonts only).</para>
		/// </summary>
		/// <param name="characters">The characters which are needed to be in the font texture.</param>
		/// <param name="size">The size of the requested characters (the default value of zero will use the font's default size).</param>
		/// <param name="style">The style of the requested characters.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RequestCharactersInTexture(string characters, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style);

		[ExcludeFromDocs]
		public void RequestCharactersInTexture(string characters, int size)
		{
			this.RequestCharactersInTexture(characters, size, FontStyle.Normal);
		}

		[ExcludeFromDocs]
		public void RequestCharactersInTexture(string characters)
		{
			this.RequestCharactersInTexture(characters, 0, FontStyle.Normal);
		}

		public delegate void FontTextureRebuildCallback();
	}
}
