using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeHeader("Modules/TextRendering/TextGenerator.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TextGenerator : IDisposable
	{
		public TextGenerator()
			: this(50)
		{
		}

		public TextGenerator(int initialCapacity)
		{
			this.m_Ptr = TextGenerator.Internal_Create();
			this.m_Verts = new List<UIVertex>((initialCapacity + 1) * 4);
			this.m_Characters = new List<UICharInfo>(initialCapacity + 1);
			this.m_Lines = new List<UILineInfo>(20);
		}

		~TextGenerator()
		{
			((IDisposable)this).Dispose();
		}

		void IDisposable.Dispose()
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				TextGenerator.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public int characterCountVisible
		{
			get
			{
				return this.characterCount - 1;
			}
		}

		private TextGenerationSettings ValidatedSettings(TextGenerationSettings settings)
		{
			bool flag = settings.font != null && settings.font.dynamic;
			TextGenerationSettings textGenerationSettings;
			if (flag)
			{
				textGenerationSettings = settings;
			}
			else
			{
				bool flag2 = settings.fontSize != 0 || settings.fontStyle > FontStyle.Normal;
				if (flag2)
				{
					bool flag3 = settings.font != null;
					if (flag3)
					{
						Debug.LogWarningFormat(settings.font, "Font size and style overrides are only supported for dynamic fonts. Font '{0}' is not dynamic.", new object[] { settings.font.name });
					}
					settings.fontSize = 0;
					settings.fontStyle = FontStyle.Normal;
				}
				bool resizeTextForBestFit = settings.resizeTextForBestFit;
				if (resizeTextForBestFit)
				{
					bool flag4 = settings.font != null;
					if (flag4)
					{
						Debug.LogWarningFormat(settings.font, "BestFit is only supported for dynamic fonts. Font '{0}' is not dynamic.", new object[] { settings.font.name });
					}
					settings.resizeTextForBestFit = false;
				}
				textGenerationSettings = settings;
			}
			return textGenerationSettings;
		}

		public void Invalidate()
		{
			this.m_HasGenerated = false;
		}

		public void GetCharacters(List<UICharInfo> characters)
		{
			this.GetCharactersInternal(characters);
		}

		public void GetLines(List<UILineInfo> lines)
		{
			this.GetLinesInternal(lines);
		}

		public void GetVertices(List<UIVertex> vertices)
		{
			this.GetVerticesInternal(vertices);
		}

		public float GetPreferredWidth(string str, TextGenerationSettings settings)
		{
			settings.horizontalOverflow = HorizontalWrapMode.Overflow;
			settings.verticalOverflow = VerticalWrapMode.Overflow;
			settings.updateBounds = true;
			this.Populate(str, settings);
			return this.rectExtents.width;
		}

		public float GetPreferredHeight(string str, TextGenerationSettings settings)
		{
			settings.verticalOverflow = VerticalWrapMode.Overflow;
			settings.updateBounds = true;
			this.Populate(str, settings);
			return this.rectExtents.height;
		}

		public bool PopulateWithErrors(string str, TextGenerationSettings settings, GameObject context)
		{
			TextGenerationError textGenerationError = this.PopulateWithError(str, settings);
			bool flag = textGenerationError == TextGenerationError.None;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = (textGenerationError & TextGenerationError.CustomSizeOnNonDynamicFont) > TextGenerationError.None;
				if (flag3)
				{
					Debug.LogErrorFormat(context, "Font '{0}' is not dynamic, which is required to override its size", new object[] { settings.font });
				}
				bool flag4 = (textGenerationError & TextGenerationError.CustomStyleOnNonDynamicFont) > TextGenerationError.None;
				if (flag4)
				{
					Debug.LogErrorFormat(context, "Font '{0}' is not dynamic, which is required to override its style", new object[] { settings.font });
				}
				flag2 = false;
			}
			return flag2;
		}

		public bool Populate(string str, TextGenerationSettings settings)
		{
			TextGenerationError textGenerationError = this.PopulateWithError(str, settings);
			return textGenerationError == TextGenerationError.None;
		}

		private TextGenerationError PopulateWithError(string str, TextGenerationSettings settings)
		{
			bool flag = this.m_HasGenerated && str == this.m_LastString && settings.Equals(this.m_LastSettings);
			TextGenerationError textGenerationError;
			if (flag)
			{
				textGenerationError = this.m_LastValid;
			}
			else
			{
				this.m_LastValid = this.PopulateAlways(str, settings);
				textGenerationError = this.m_LastValid;
			}
			return textGenerationError;
		}

		private TextGenerationError PopulateAlways(string str, TextGenerationSettings settings)
		{
			this.m_LastString = str;
			this.m_HasGenerated = true;
			this.m_CachedVerts = false;
			this.m_CachedCharacters = false;
			this.m_CachedLines = false;
			this.m_LastSettings = settings;
			TextGenerationSettings textGenerationSettings = this.ValidatedSettings(settings);
			TextGenerationError textGenerationError;
			this.Populate_Internal(str, textGenerationSettings.font, textGenerationSettings.color, textGenerationSettings.fontSize, textGenerationSettings.scaleFactor, textGenerationSettings.lineSpacing, textGenerationSettings.fontStyle, textGenerationSettings.richText, textGenerationSettings.resizeTextForBestFit, textGenerationSettings.resizeTextMinSize, textGenerationSettings.resizeTextMaxSize, textGenerationSettings.verticalOverflow, textGenerationSettings.horizontalOverflow, textGenerationSettings.updateBounds, textGenerationSettings.textAnchor, textGenerationSettings.generationExtents, textGenerationSettings.pivot, textGenerationSettings.generateOutOfBounds, textGenerationSettings.alignByGeometry, out textGenerationError);
			this.m_LastValid = textGenerationError;
			return textGenerationError;
		}

		public IList<UIVertex> verts
		{
			get
			{
				bool flag = !this.m_CachedVerts;
				if (flag)
				{
					this.GetVertices(this.m_Verts);
					this.m_CachedVerts = true;
				}
				return this.m_Verts;
			}
		}

		public IList<UICharInfo> characters
		{
			get
			{
				bool flag = !this.m_CachedCharacters;
				if (flag)
				{
					this.GetCharacters(this.m_Characters);
					this.m_CachedCharacters = true;
				}
				return this.m_Characters;
			}
		}

		public IList<UILineInfo> lines
		{
			get
			{
				bool flag = !this.m_CachedLines;
				if (flag)
				{
					this.GetLines(this.m_Lines);
					this.m_CachedLines = true;
				}
				return this.m_Lines;
			}
		}

		public Rect rectExtents
		{
			get
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rect rect;
				TextGenerator.get_rectExtents_Injected(intPtr, out rect);
				return rect;
			}
		}

		public int vertexCount
		{
			get
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TextGenerator.get_vertexCount_Injected(intPtr);
			}
		}

		public int characterCount
		{
			get
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TextGenerator.get_characterCount_Injected(intPtr);
			}
		}

		public int lineCount
		{
			get
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TextGenerator.get_lineCount_Injected(intPtr);
			}
		}

		[NativeProperty("FontSizeFoundForBestFit", false, TargetType.Function)]
		public int fontSizeUsedForBestFit
		{
			get
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return TextGenerator.get_fontSizeUsedForBestFit_Injected(intPtr);
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		internal unsafe bool Populate_Internal(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry, out uint error)
		{
			bool flag;
			try
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(str, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = str.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = TextGenerator.Populate_Internal_Injected(intPtr, ref managedSpanWrapper, Object.MarshalledUnityObject.Marshal<Font>(font), ref color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, verticalOverFlow, horizontalOverflow, updateBounds, anchor, extentsX, extentsY, pivotX, pivotY, generateOutOfBounds, alignByGeometry, out error);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		internal bool Populate_Internal(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, VerticalWrapMode verticalOverFlow, HorizontalWrapMode horizontalOverflow, bool updateBounds, TextAnchor anchor, Vector2 extents, Vector2 pivot, bool generateOutOfBounds, bool alignByGeometry, out TextGenerationError error)
		{
			bool flag = font == null;
			bool flag2;
			if (flag)
			{
				error = TextGenerationError.NoFont;
				flag2 = false;
			}
			else
			{
				uint num = 0U;
				bool flag3 = this.Populate_Internal(str, font, color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, (int)verticalOverFlow, (int)horizontalOverflow, updateBounds, anchor, extents.x, extents.y, pivot.x, pivot.y, generateOutOfBounds, alignByGeometry, out num);
				error = (TextGenerationError)num;
				flag2 = flag3;
			}
			return flag2;
		}

		public UIVertex[] GetVerticesArray()
		{
			UIVertex[] array2;
			try
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				TextGenerator.GetVerticesArray_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				UIVertex[] array;
				blittableArrayWrapper.Unmarshal<UIVertex>(ref array);
				array2 = array;
			}
			return array2;
		}

		public UICharInfo[] GetCharactersArray()
		{
			UICharInfo[] array2;
			try
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				TextGenerator.GetCharactersArray_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				UICharInfo[] array;
				blittableArrayWrapper.Unmarshal<UICharInfo>(ref array);
				array2 = array;
			}
			return array2;
		}

		public UILineInfo[] GetLinesArray()
		{
			UILineInfo[] array2;
			try
			{
				IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				TextGenerator.GetLinesArray_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				UILineInfo[] array;
				blittableArrayWrapper.Unmarshal<UILineInfo>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeThrows]
		private void GetVerticesInternal(object vertices)
		{
			IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TextGenerator.GetVerticesInternal_Injected(intPtr, vertices);
		}

		[NativeThrows]
		private void GetCharactersInternal(object characters)
		{
			IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TextGenerator.GetCharactersInternal_Injected(intPtr, characters);
		}

		[NativeThrows]
		private void GetLinesInternal(object lines)
		{
			IntPtr intPtr = TextGenerator.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			TextGenerator.GetLinesInternal_Injected(intPtr, lines);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rectExtents_Injected(IntPtr _unity_self, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_vertexCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_characterCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_lineCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_fontSizeUsedForBestFit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Populate_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper str, IntPtr font, [In] ref Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry, out uint error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVerticesArray_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCharactersArray_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLinesArray_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVerticesInternal_Injected(IntPtr _unity_self, object vertices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetCharactersInternal_Injected(IntPtr _unity_self, object characters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLinesInternal_Injected(IntPtr _unity_self, object lines);

		internal IntPtr m_Ptr;

		private string m_LastString;

		private TextGenerationSettings m_LastSettings;

		private bool m_HasGenerated;

		private TextGenerationError m_LastValid;

		private readonly List<UIVertex> m_Verts;

		private readonly List<UICharInfo> m_Characters;

		private readonly List<UILineInfo> m_Lines;

		private bool m_CachedVerts;

		private bool m_CachedCharacters;

		private bool m_CachedLines;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(TextGenerator textGenerator)
			{
				return textGenerator.m_Ptr;
			}
		}
	}
}
