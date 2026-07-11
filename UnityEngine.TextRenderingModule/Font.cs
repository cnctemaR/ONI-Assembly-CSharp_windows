using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/TextRendering/Public/FontImpl.h")]
	[NativeHeader("Modules/TextRendering/Public/Font.h")]
	[StaticAccessor("TextRenderingPrivate", StaticAccessorType.DoubleColon)]
	[NativeClass("TextRendering::Font")]
	public sealed class Font : Object
	{
		public Font()
		{
			Font.Internal_CreateFont(this, null);
		}

		public Font(string name)
		{
			bool flag = Path.GetDirectoryName(name) == string.Empty;
			if (flag)
			{
				Font.Internal_CreateFont(this, name);
			}
			else
			{
				Font.Internal_CreateFontFromPath(this, name);
			}
		}

		private Font(string[] names, int size)
		{
			Font.Internal_CreateDynamicFont(this, names, size);
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Font> textureRebuilt;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Font.FontTextureRebuildCallback m_FontTextureRebuildCallback;

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

		public extern bool dynamic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int ascent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int fontSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern CharacterInfo[] characterInfo
		{
			[FreeFunction("TextRenderingPrivate::GetFontCharacterInfo", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction("TextRenderingPrivate::SetFontCharacterInfo", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public static Font CreateDynamicFontFromOSFont(string fontname, int size)
		{
			return new Font(new string[] { fontname }, size);
		}

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

		public static int GetMaxVertsForString(string str)
		{
			return str.Length * 4 + 4;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Font GetDefault();

		public bool HasCharacter(char c)
		{
			return this.HasCharacter((int)c);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool HasCharacter(int c);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetOSInstalledFontNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetPathsToOSFonts();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateFont([Writable] Font self, string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateFontFromPath([Writable] Font self, string fontPath);

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
