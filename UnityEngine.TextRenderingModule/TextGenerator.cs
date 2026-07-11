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
			if (this.m_Ptr != IntPtr.Zero)
			{
				TextGenerator.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public int characterCountVisible
		{
			[CompilerGenerated]
			get
			{
				return this.characterCount - 1;
			}
		}

		private TextGenerationSettings ValidatedSettings(TextGenerationSettings settings)
		{
			TextGenerationSettings textGenerationSettings;
			if (settings.font != null && settings.font.dynamic)
			{
				textGenerationSettings = settings;
			}
			else
			{
				if (settings.fontSize != 0 || settings.fontStyle != FontStyle.Normal)
				{
					if (settings.font != null)
					{
						Debug.LogWarningFormat(settings.font, "Font size and style overrides are only supported for dynamic fonts. Font '{0}' is not dynamic.", new object[] { settings.font.name });
					}
					settings.fontSize = 0;
					settings.fontStyle = FontStyle.Normal;
				}
				if (settings.resizeTextForBestFit)
				{
					if (settings.font != null)
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
			bool flag;
			if (textGenerationError == TextGenerationError.None)
			{
				flag = true;
			}
			else
			{
				if ((textGenerationError & TextGenerationError.CustomSizeOnNonDynamicFont) != TextGenerationError.None)
				{
					Debug.LogErrorFormat(context, "Font '{0}' is not dynamic, which is required to override its size", new object[] { settings.font });
				}
				if ((textGenerationError & TextGenerationError.CustomStyleOnNonDynamicFont) != TextGenerationError.None)
				{
					Debug.LogErrorFormat(context, "Font '{0}' is not dynamic, which is required to override its style", new object[] { settings.font });
				}
				flag = false;
			}
			return flag;
		}

		public bool Populate(string str, TextGenerationSettings settings)
		{
			TextGenerationError textGenerationError = this.PopulateWithError(str, settings);
			return textGenerationError == TextGenerationError.None;
		}

		private TextGenerationError PopulateWithError(string str, TextGenerationSettings settings)
		{
			TextGenerationError textGenerationError;
			if (this.m_HasGenerated && str == this.m_LastString && settings.Equals(this.m_LastSettings))
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
				if (!this.m_CachedVerts)
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
				if (!this.m_CachedCharacters)
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
				if (!this.m_CachedLines)
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
				Rect rect;
				this.get_rectExtents_Injected(out rect);
				return rect;
			}
		}

		public extern int vertexCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int characterCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int lineCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeProperty("FontSizeFoundForBestFit", false, TargetType.Function)]
		public extern int fontSizeUsedForBestFit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		internal bool Populate_Internal(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry, out uint error)
		{
			return this.Populate_Internal_Injected(str, font, ref color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, verticalOverFlow, horizontalOverflow, updateBounds, anchor, extentsX, extentsY, pivotX, pivotY, generateOutOfBounds, alignByGeometry, out error);
		}

		internal bool Populate_Internal(string str, Font font, Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, VerticalWrapMode verticalOverFlow, HorizontalWrapMode horizontalOverflow, bool updateBounds, TextAnchor anchor, Vector2 extents, Vector2 pivot, bool generateOutOfBounds, bool alignByGeometry, out TextGenerationError error)
		{
			bool flag;
			if (font == null)
			{
				error = TextGenerationError.NoFont;
				flag = false;
			}
			else
			{
				uint num = 0U;
				bool flag2 = this.Populate_Internal(str, font, color, fontSize, scaleFactor, lineSpacing, style, richText, resizeTextForBestFit, resizeTextMinSize, resizeTextMaxSize, (int)verticalOverFlow, (int)horizontalOverflow, updateBounds, anchor, extents.x, extents.y, pivot.x, pivot.y, generateOutOfBounds, alignByGeometry, out num);
				error = (TextGenerationError)num;
				flag = flag2;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern UIVertex[] GetVerticesArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern UICharInfo[] GetCharactersArray();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern UILineInfo[] GetLinesArray();

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetVerticesInternal(object vertices);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetCharactersInternal(object characters);

		[NativeThrows]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetLinesInternal(object lines);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_rectExtents_Injected(out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Populate_Internal_Injected(string str, Font font, ref Color color, int fontSize, float scaleFactor, float lineSpacing, FontStyle style, bool richText, bool resizeTextForBestFit, int resizeTextMinSize, int resizeTextMaxSize, int verticalOverFlow, int horizontalOverflow, bool updateBounds, TextAnchor anchor, float extentsX, float extentsY, float pivotX, float pivotY, bool generateOutOfBounds, bool alignByGeometry, out uint error);

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
	}
}
