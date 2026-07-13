using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[StaticAccessor("TextRenderingPrivate", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/TextRendering/Public/FontImpl.h")]
	[NativeClass("TextRendering::Font")]
	[NativeHeader("Modules/TextRendering/Public/Font.h")]
	public sealed class Font : Object
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<Font> textureRebuilt;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Font.FontTextureRebuildCallback m_FontTextureRebuildCallback;

		public Material material
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Material>(Font.get_material_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Font.set_material_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(value));
			}
		}

		public string[] fontNames
		{
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Font.get_fontNames_Injected(intPtr);
			}
			[param: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Font.set_fontNames_Injected(intPtr, value);
			}
		}

		public bool dynamic
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Font.get_dynamic_Injected(intPtr);
			}
		}

		public int ascent
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Font.get_ascent_Injected(intPtr);
			}
		}

		public int fontSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Font.get_fontSize_Injected(intPtr);
			}
		}

		public unsafe CharacterInfo[] characterInfo
		{
			[FreeFunction("TextRenderingPrivate::GetFontCharacterInfo", HasExplicitThis = true)]
			get
			{
				CharacterInfo[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					Font.get_characterInfo_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					CharacterInfo[] array;
					blittableArrayWrapper.Unmarshal<CharacterInfo>(ref array);
					array2 = array;
				}
				return array2;
			}
			[FreeFunction("TextRenderingPrivate::SetFontCharacterInfo", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<CharacterInfo> span = new Span<CharacterInfo>(value);
				fixed (CharacterInfo* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					Font.set_characterInfo_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		[NativeProperty("LineSpacing", false, TargetType.Function)]
		public int lineHeight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Font.get_lineHeight_Injected(intPtr);
			}
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

		public Font()
		{
			Font.Internal_CreateFont(this, null);
		}

		public Font(string name)
		{
			bool flag = Path.GetDirectoryName(name) == string.Empty;
			bool flag2 = flag;
			if (flag2)
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
			Action<Font> action = Font.textureRebuilt;
			if (action != null)
			{
				action(font);
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

		[VisibleToOtherModules(new string[] { "UnityEditor.TextRenderingModule" })]
		internal static Font GetDefault()
		{
			return Unmarshal.UnmarshalUnityObject<Font>(Font.GetDefault_Injected());
		}

		public bool HasCharacter(char c)
		{
			return this.HasCharacter((int)c);
		}

		private bool HasCharacter(int c)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Font.HasCharacter_Injected(intPtr, c);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern string[] GetOSInstalledFontNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern string[] GetPathsToOSFonts();

		[VisibleToOtherModules(new string[] { "UnityEngine.TextCoreTextEngineModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal static extern string[] GetOSFallbacks();

		[ThreadSafe]
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.CoreModule" })]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsFontSmoothingEnabled();

		private unsafe static void Internal_CreateFont([Writable] Font self, string name)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Font.Internal_CreateFont_Injected(self, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		private unsafe static void Internal_CreateFontFromPath([Writable] Font self, string fontPath)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fontPath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fontPath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Font.Internal_CreateFontFromPath_Injected(self, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateDynamicFont([Writable] Font self, [UnityMarshalAs(NativeType.ScriptingObjectPtr)] string[] _names, int size);

		[FreeFunction("TextRenderingPrivate::GetCharacterInfo", HasExplicitThis = true)]
		public bool GetCharacterInfo(char ch, out CharacterInfo info, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Font.GetCharacterInfo_Injected(intPtr, ch, out info, size, style);
		}

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

		public unsafe void RequestCharactersInTexture(string characters, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(characters, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = characters.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Font.RequestCharactersInTexture_Injected(intPtr, ref managedSpanWrapper, size, style);
			}
			finally
			{
				char* ptr = null;
			}
		}

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_material_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_material_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] get_fontNames_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_fontNames_Injected(IntPtr _unity_self, string[] value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_dynamic_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_ascent_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_fontSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_characterInfo_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_characterInfo_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_lineHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDefault_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasCharacter_Injected(IntPtr _unity_self, int c);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateFont_Injected([Writable] Font self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateFontFromPath_Injected([Writable] Font self, ref ManagedSpanWrapper fontPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetCharacterInfo_Injected(IntPtr _unity_self, char ch, out CharacterInfo info, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RequestCharactersInTexture_Injected(IntPtr _unity_self, ref ManagedSpanWrapper characters, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style);

		public delegate void FontTextureRebuildCallback();
	}
}
