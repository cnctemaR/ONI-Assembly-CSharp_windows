using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule", "UnityEditor.QuickSearchModule" })]
	[DebuggerDisplay("{settings.text}")]
	internal class TextHandle
	{
		~TextHandle()
		{
			this.RemoveFromTemporaryCache();
			this.RemoveFromPermanentCache();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal static void InitThreadArrays()
		{
			bool flag = TextHandle.s_Settings != null && TextHandle.s_Generators != null && TextHandle.s_TextInfosCommon != null;
			if (!flag)
			{
				TextHandle.InitArray<TextGenerationSettings>(ref TextHandle.s_Settings, () => new TextGenerationSettings());
				TextHandle.InitArray<TextGenerator>(ref TextHandle.s_Generators, () => new TextGenerator());
				TextHandle.InitArray<TextInfo>(ref TextHandle.s_TextInfosCommon, () => new TextInfo());
			}
		}

		internal static TextGenerationSettings[] settingsArray
		{
			get
			{
				bool flag = TextHandle.s_Settings == null;
				if (flag)
				{
					TextHandle.InitArray<TextGenerationSettings>(ref TextHandle.s_Settings, () => new TextGenerationSettings());
				}
				return TextHandle.s_Settings;
			}
		}

		internal static TextGenerator[] generators
		{
			get
			{
				bool flag = TextHandle.s_Generators == null;
				if (flag)
				{
					TextHandle.InitArray<TextGenerator>(ref TextHandle.s_Generators, () => new TextGenerator());
				}
				return TextHandle.s_Generators;
			}
		}

		internal static TextInfo[] textInfosCommon
		{
			get
			{
				bool flag = TextHandle.s_TextInfosCommon == null;
				if (flag)
				{
					TextHandle.InitArray<TextInfo>(ref TextHandle.s_TextInfosCommon, () => new TextInfo());
				}
				return TextHandle.s_TextInfosCommon;
			}
		}

		private static void InitArray<T>(ref T[] array, Func<T> createInstance)
		{
			bool flag = array != null;
			if (!flag)
			{
				array = new T[JobsUtility.ThreadIndexCount];
				for (int i = 0; i < JobsUtility.ThreadIndexCount; i++)
				{
					array[i] = createInstance();
				}
			}
		}

		internal static TextInfo textInfoCommon
		{
			get
			{
				return TextHandle.textInfosCommon[JobsUtility.ThreadIndex];
			}
		}

		private static TextGenerator generator
		{
			get
			{
				return TextHandle.generators[JobsUtility.ThreadIndex];
			}
		}

		internal static TextGenerationSettings settings
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
			get
			{
				return TextHandle.settingsArray[JobsUtility.ThreadIndex];
			}
		}

		internal Vector2 preferredSize
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
			get
			{
				return this.PixelsToPoints(this.pixelPreferedSize);
			}
		}

		protected float PointsToPixels(float point)
		{
			return point * this.GetPixelsPerPoint();
		}

		protected float PixelsToPoints(float pixel)
		{
			return pixel / this.GetPixelsPerPoint();
		}

		protected Vector2 PointsToPixels(Vector2 point)
		{
			return point * this.GetPixelsPerPoint();
		}

		protected Vector2 PixelsToPoints(Vector2 pixel)
		{
			return pixel / this.GetPixelsPerPoint();
		}

		protected virtual float GetPixelsPerPoint()
		{
			return 1f;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal IntPtr textGenerationInfo
		{
			get
			{
				bool isCachedPermanentATG = this.IsCachedPermanentATG;
				if (isCachedPermanentATG)
				{
					Debug.Assert(this.m_TextGenerationInfo != IntPtr.Zero, "Internal Text Error: element is marked in permanent cache but the cache doesn't exist");
				}
				bool flag = !this.IsCachedPermanentATG && this.m_CreateGenerationIteration != TextGenerationInfo.CurrentGenerationIteration;
				if (flag)
				{
					this.m_TextGenerationInfo = IntPtr.Zero;
				}
				return this.m_TextGenerationInfo;
			}
			set
			{
				Debug.Assert(value == IntPtr.Zero || this.m_TextGenerationInfo == IntPtr.Zero, "Internal Text Error: Transitioning from one cache structure to another directly. This might cause a memory leak");
				this.m_TextGenerationInfo = value;
				bool flag = this.m_TextHandleFlags.HasFlag(TextHandle.TextHandleFlags.IsCachedPermanentATG);
				this.m_CreateGenerationIteration = TextGenerationInfo.CurrentGenerationIteration;
			}
		}

		internal LinkedListNode<TextCacheEntry> TextInfoNode { get; set; }

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		protected internal bool IsCachedPermanent
		{
			get
			{
				return (this.m_TextHandleFlags & (TextHandle.TextHandleFlags.IsCachedPermanentTextCore | TextHandle.TextHandleFlags.IsCachedPermanentATG)) > (TextHandle.TextHandleFlags)0;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool IsCachedPermanentATG
		{
			get
			{
				bool flag = this.m_TextHandleFlags.HasFlag(TextHandle.TextHandleFlags.IsCachedPermanentATG);
				bool flag2 = flag;
				if (flag2)
				{
					Debug.Assert(this.m_TextGenerationInfo != IntPtr.Zero, "Internal Text Error : The element is marked as being in the permanent cache without having the cache assigned");
				}
				return flag;
			}
			set
			{
				if (value)
				{
					this.m_TextHandleFlags |= TextHandle.TextHandleFlags.IsCachedPermanentATG;
				}
				else
				{
					this.m_TextHandleFlags ^= TextHandle.TextHandleFlags.IsCachedPermanentATG;
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool IsCachedPermanentTextCore
		{
			get
			{
				bool flag = this.m_TextHandleFlags.HasFlag(TextHandle.TextHandleFlags.IsCachedPermanentTextCore);
				bool flag2 = !this.IsCachedTemporary;
				if (flag2)
				{
					Debug.AssertFormat(flag == (this.TextInfoNode != null), "TextHandle : TextCore Permananent cache mismatch. isCache {0} but {1}", new object[]
					{
						flag,
						(this.TextInfoNode == null) ? " has no node" : "has a node"
					});
				}
				return flag;
			}
			set
			{
				if (value)
				{
					this.m_TextHandleFlags |= TextHandle.TextHandleFlags.IsCachedPermanentTextCore;
				}
				else
				{
					this.m_TextHandleFlags ^= TextHandle.TextHandleFlags.IsCachedPermanentTextCore;
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool IsCachedTemporary { get; set; }

		internal bool useAdvancedText
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
			get
			{
				return this.IsAdvancedTextEnabledForElement();
			}
		}

		internal int characterCount
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
			get
			{
				return this.useAdvancedText ? TextLib.GetCharacterCount(this.textGenerationInfo) : this.textInfo.characterCount;
			}
		}

		public virtual void AddToPermanentCacheAndGenerateMesh()
		{
			bool useAdvancedText = this.useAdvancedText;
			if (useAdvancedText)
			{
				throw new InvalidOperationException("Method is virtual and should be overriden in ATGTextHanle, the only valid handle for ATG");
			}
			TextHandle.s_PermanentCache.AddToCache(this);
		}

		public void AddTextInfoToTemporaryCache(int hashCode)
		{
			bool useAdvancedText = this.useAdvancedText;
			if (!useAdvancedText)
			{
				TextHandle.s_TemporaryCache.AddTextInfoToCache(this, hashCode);
			}
		}

		public void RemoveFromTemporaryCache()
		{
			TextHandle.s_TemporaryCache.RemoveFromCache(this);
		}

		public void RemoveFromPermanentCache()
		{
			this.RemoveFromPermanentCacheATG();
			this.RemoveFromPermanentCacheTextCore();
		}

		public void RemoveFromPermanentCacheTextCore()
		{
			TextHandle.s_PermanentCache.RemoveFromCache(this);
		}

		public void RemoveFromPermanentCacheATG()
		{
			bool isCachedPermanentATG = this.IsCachedPermanentATG;
			if (isCachedPermanentATG)
			{
				TextGenerationInfo.Destroy(this.textGenerationInfo);
				this.textGenerationInfo = IntPtr.Zero;
				this.IsCachedPermanentATG = false;
			}
		}

		public static void UpdateCurrentFrame()
		{
			TextHandle.s_TemporaryCache.UpdateCurrentFrame();
		}

		internal TextInfo textInfo
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
			get
			{
				bool useAdvancedText = this.useAdvancedText;
				if (useAdvancedText)
				{
					Debug.LogError("TextHandle.textInfo should not be used with Advanced Text, use textGenerationInfo instead.");
				}
				bool flag = this.TextInfoNode == null;
				TextInfo textInfo;
				if (flag)
				{
					textInfo = TextHandle.textInfoCommon;
				}
				else
				{
					textInfo = this.TextInfoNode.Value.textInfo;
				}
				return textInfo;
			}
		}

		internal bool IsTextInfoAllocated()
		{
			return this.textInfo != null;
		}

		public virtual void SetDirty()
		{
			this.isDirty = true;
		}

		public bool IsDirty(int hashCode)
		{
			bool flag = this.m_PreviousGenerationSettingsHash == hashCode && !this.isDirty && (this.IsCachedTemporary || this.IsCachedPermanent);
			return !flag;
		}

		public float ComputeTextWidth(TextGenerationSettings tgs)
		{
			this.UpdatePreferredValues(tgs);
			return this.preferredSize.x;
		}

		public float ComputeTextHeight(TextGenerationSettings tgs)
		{
			this.UpdatePreferredValues(tgs);
			return this.preferredSize.y;
		}

		public virtual bool IsPlaceholder
		{
			get
			{
				return this.m_IsPlaceholder;
			}
		}

		protected void UpdatePreferredValues(TextGenerationSettings tgs)
		{
			this.pixelPreferedSize = TextHandle.generator.GetPreferredValues(tgs, TextHandle.textInfoCommon);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal TextInfo Update()
		{
			return this.UpdateWithHash(TextHandle.settings.GetHashCode());
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal TextInfo UpdateWithHash(int hashCode)
		{
			this.m_ScreenRect = TextHandle.settings.screenRect;
			this.m_LineHeightDefault = TextHandle.GetLineHeightDefault(TextHandle.settings);
			this.m_IsPlaceholder = TextHandle.settings.isPlaceholder;
			bool flag = !this.IsDirty(hashCode);
			TextInfo textInfo;
			if (flag)
			{
				textInfo = this.textInfo;
			}
			else
			{
				bool flag2 = TextHandle.settings.fontAsset == null;
				if (flag2)
				{
					Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
					textInfo = this.textInfo;
				}
				else
				{
					TextHandle.generator.GenerateText(TextHandle.settings, this.textInfo);
					this.m_PreviousGenerationSettingsHash = hashCode;
					this.isDirty = false;
					this.m_IsElided = TextHandle.generator.isTextTruncated;
					textInfo = this.textInfo;
				}
			}
			return textInfo;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal bool PrepareFontAsset()
		{
			bool flag = TextHandle.settings.fontAsset == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = !this.IsDirty(TextHandle.settings.GetHashCode());
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = TextHandle.generator.PrepareFontAsset(TextHandle.settings);
					flag2 = flag4;
				}
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
		internal void UpdatePreferredSize()
		{
			bool flag = this.textInfo.characterCount <= 0;
			if (!flag)
			{
				float num = float.MinValue;
				float num2 = this.textInfo.textElementInfo[this.textInfo.characterCount - 1].descender;
				float num3 = 0f;
				for (int i = 0; i < this.textInfo.lineCount; i++)
				{
					LineInfo lineInfo = this.textInfo.lineInfo[i];
					num = Mathf.Max(num, this.textInfo.textElementInfo[lineInfo.firstVisibleCharacterIndex].ascender);
					num2 = Mathf.Min(num2, this.textInfo.textElementInfo[lineInfo.firstVisibleCharacterIndex].descender);
					num3 = (TextHandle.settings.isIMGUI ? Mathf.Max(num3, lineInfo.length) : Mathf.Max(num3, lineInfo.lineExtents.max.x - lineInfo.lineExtents.min.x));
				}
				float num4 = num - num2;
				num3 = (float)((int)(num3 * 100f + 1f)) / 100f;
				num4 = (float)((int)(num4 * 100f + 1f)) / 100f;
				this.pixelPreferedSize = new Vector2(num3, num4);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static float ConvertPixelUnitsToTextCoreRelativeUnits(float fontSize, FontAsset fontAsset)
		{
			float num = 1f / (float)fontAsset.atlasPadding;
			float num2 = fontAsset.faceInfo.pointSize / fontSize;
			return num * num2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
		internal static float GetLineHeightDefault(TextGenerationSettings settings)
		{
			bool flag = settings != null && settings.fontAsset != null;
			float num;
			if (flag)
			{
				num = settings.fontAsset.faceInfo.lineHeight / settings.fontAsset.faceInfo.pointSize * (float)settings.fontSize;
			}
			else
			{
				num = 0f;
			}
			return num;
		}

		public virtual Vector2 GetCursorPositionFromStringIndexUsingCharacterHeight(int index, bool inverseYAxis = true)
		{
			this.AddToPermanentCacheAndGenerateMesh();
			Vector2 vector = (this.useAdvancedText ? TextSelectionService.GetCursorPositionFromLogicalIndex(this.textGenerationInfo, index) : this.textInfo.GetCursorPositionFromStringIndexUsingCharacterHeight(index, this.m_ScreenRect, this.m_LineHeightDefault, inverseYAxis));
			return this.PixelsToPoints(vector);
		}

		public Vector2 GetCursorPositionFromStringIndexUsingLineHeight(int index, bool useXAdvance = false, bool inverseYAxis = true)
		{
			this.AddToPermanentCacheAndGenerateMesh();
			Vector2 vector = (this.useAdvancedText ? TextSelectionService.GetCursorPositionFromLogicalIndex(this.textGenerationInfo, index) : this.textInfo.GetCursorPositionFromStringIndexUsingLineHeight(index, this.m_ScreenRect, this.m_LineHeightDefault, useXAdvance, inverseYAxis));
			return this.PixelsToPoints(vector);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
		internal Rect[] GetHighlightRectangles(int cursorIndex, int selectIndex)
		{
			bool flag = !this.useAdvancedText;
			Rect[] array;
			if (flag)
			{
				Debug.LogError("Cannot use GetHighlightRectangles while using Standard Text");
				array = new Rect[0];
			}
			else
			{
				Rect[] highlightRectangles = TextSelectionService.GetHighlightRectangles(this.textGenerationInfo, cursorIndex, selectIndex);
				float num = 1f / this.GetPixelsPerPoint();
				for (int i = 0; i < highlightRectangles.Length; i++)
				{
					Rect[] array2 = highlightRectangles;
					int num2 = i;
					array2[num2].x = array2[num2].x * num;
					Rect[] array3 = highlightRectangles;
					int num3 = i;
					array3[num3].y = array3[num3].y * num;
					Rect[] array4 = highlightRectangles;
					int num4 = i;
					array4[num4].width = array4[num4].width * num;
					Rect[] array5 = highlightRectangles;
					int num5 = i;
					array5[num5].height = array5[num5].height * num;
				}
				array = highlightRectangles;
			}
			return array;
		}

		public int GetCursorIndexFromPosition(Vector2 position, bool inverseYAxis = true)
		{
			position = this.PointsToPixels(position);
			return this.useAdvancedText ? TextSelectionService.GetCursorLogicalIndexFromPosition(this.textGenerationInfo, position) : this.textInfo.GetCursorIndexFromPosition(position, this.m_ScreenRect, inverseYAxis);
		}

		public int LineDownCharacterPosition(int originalLogicalPos)
		{
			return this.useAdvancedText ? TextSelectionService.LineDownCharacterPosition(this.textGenerationInfo, originalLogicalPos) : this.textInfo.LineDownCharacterPosition(originalLogicalPos);
		}

		public int LineUpCharacterPosition(int originalLogicalPos)
		{
			return this.useAdvancedText ? TextSelectionService.LineUpCharacterPosition(this.textGenerationInfo, originalLogicalPos) : this.textInfo.LineUpCharacterPosition(originalLogicalPos);
		}

		public int FindWordIndex(int cursorIndex)
		{
			bool useAdvancedText = this.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				Debug.LogError("Cannot use FindWordIndex while using Advanced Text");
				num = 0;
			}
			else
			{
				num = this.textInfo.FindWordIndex(cursorIndex);
			}
			return num;
		}

		public int FindNearestLine(Vector2 position)
		{
			position = this.PointsToPixels(position);
			bool useAdvancedText = this.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				Debug.LogError("Cannot use FindNearestLine while using Advanced Text");
				num = 0;
			}
			else
			{
				num = this.textInfo.FindNearestLine(position);
			}
			return num;
		}

		public int FindNearestCharacterOnLine(Vector2 position, int line, bool visibleOnly)
		{
			bool useAdvancedText = this.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				Debug.LogError("Cannot use FindNearestCharacterOnLine while using Advanced Text");
				num = 0;
			}
			else
			{
				position = this.PointsToPixels(position);
				num = this.textInfo.FindNearestCharacterOnLine(position, line, visibleOnly);
			}
			return num;
		}

		public int FindIntersectingLink(Vector3 position, bool inverseYAxis = true)
		{
			bool useAdvancedText = this.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				Debug.LogError("Cannot use FindIntersectingLink while using Advanced Text");
				num = 0;
			}
			else
			{
				position = this.PointsToPixels(position);
				num = this.textInfo.FindIntersectingLink(position, this.m_ScreenRect, inverseYAxis);
			}
			return num;
		}

		public int GetCorrespondingStringIndex(int index)
		{
			return this.useAdvancedText ? index : this.textInfo.GetCorrespondingStringIndex(index);
		}

		public int GetCorrespondingCodePointIndex(int stringIndex)
		{
			return this.useAdvancedText ? stringIndex : this.textInfo.GetCorrespondingCodePointIndex(stringIndex);
		}

		public LineInfo GetLineInfoFromCharacterIndex(int index)
		{
			bool useAdvancedText = this.useAdvancedText;
			LineInfo lineInfo;
			if (useAdvancedText)
			{
				Debug.LogError("Cannot use GetLineInfoFromCharacterIndex while using Advanced Text");
				lineInfo = default(LineInfo);
			}
			else
			{
				lineInfo = this.textInfo.GetLineInfoFromCharacterIndex(index);
			}
			return lineInfo;
		}

		public int GetLineNumber(int index)
		{
			return this.useAdvancedText ? TextSelectionService.GetLineNumber(this.textGenerationInfo, index) : this.textInfo.GetLineNumber(index);
		}

		public float GetLineHeight(int lineNumber)
		{
			return this.PixelsToPoints(this.useAdvancedText ? TextSelectionService.GetLineHeight(this.textGenerationInfo, lineNumber) : this.textInfo.GetLineHeight(lineNumber));
		}

		public float GetLineHeightFromCharacterIndex(int index)
		{
			return this.PixelsToPoints(this.useAdvancedText ? TextSelectionService.GetCharacterHeightFromIndex(this.textGenerationInfo, index) : this.textInfo.GetLineHeightFromCharacterIndex(index));
		}

		public float GetCharacterHeightFromIndex(int index)
		{
			return this.PixelsToPoints(this.useAdvancedText ? TextSelectionService.GetCharacterHeightFromIndex(this.textGenerationInfo, index) : this.textInfo.GetCharacterHeightFromIndex(index));
		}

		public string Substring(int startIndex, int length)
		{
			return this.useAdvancedText ? TextSelectionService.Substring(this.textGenerationInfo, startIndex, startIndex + length) : this.textInfo.Substring(startIndex, length);
		}

		public int PreviousCodePointIndex(int currentIndex)
		{
			bool flag = !this.useAdvancedText;
			int num;
			if (flag)
			{
				Debug.LogError("Cannot use PreviousCodePointIndex while using Standard Text");
				num = 0;
			}
			else
			{
				num = TextSelectionService.PreviousCodePointIndex(this.textGenerationInfo, currentIndex);
			}
			return num;
		}

		public int NextCodePointIndex(int currentIndex)
		{
			bool flag = !this.useAdvancedText;
			int num;
			if (flag)
			{
				Debug.LogError("Cannot use NextCodePointIndex while using Standard Text");
				num = 0;
			}
			else
			{
				num = TextSelectionService.NextCodePointIndex(this.textGenerationInfo, currentIndex);
			}
			return num;
		}

		public int GetStartOfNextWord(int currentIndex)
		{
			bool flag = !this.useAdvancedText;
			int num;
			if (flag)
			{
				Debug.LogError("Cannot use GetStartOfNextWord while using Standard Text");
				num = 0;
			}
			else
			{
				num = TextSelectionService.GetStartOfNextWord(this.textGenerationInfo, currentIndex);
			}
			return num;
		}

		public int GetEndOfPreviousWord(int currentIndex)
		{
			bool flag = !this.useAdvancedText;
			int num;
			if (flag)
			{
				Debug.LogError("Cannot use GetEndOfPreviousWord while using Standard Text");
				num = 0;
			}
			else
			{
				num = TextSelectionService.GetEndOfPreviousWord(this.textGenerationInfo, currentIndex);
			}
			return num;
		}

		public int GetFirstCharacterIndexOnLine(int currentIndex)
		{
			bool flag = !this.useAdvancedText;
			int num;
			if (flag)
			{
				LineInfo lineInfoFromCharacterIndex = this.GetLineInfoFromCharacterIndex(currentIndex);
				num = lineInfoFromCharacterIndex.firstCharacterIndex;
			}
			else
			{
				num = TextSelectionService.GetFirstCharacterIndexOnLine(this.textGenerationInfo, currentIndex);
			}
			return num;
		}

		public int GetLastCharacterIndexOnLine(int currentIndex)
		{
			bool flag = !this.useAdvancedText;
			int num;
			if (flag)
			{
				LineInfo lineInfoFromCharacterIndex = this.GetLineInfoFromCharacterIndex(currentIndex);
				num = lineInfoFromCharacterIndex.lastCharacterIndex;
			}
			else
			{
				num = TextSelectionService.GetLastCharacterIndexOnLine(this.textGenerationInfo, currentIndex) + 1;
			}
			return num;
		}

		public int IndexOf(char value, int startIndex)
		{
			bool useAdvancedText = this.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				Debug.LogError("Cannot use IndexOf while using Advanced Text");
				num = 0;
			}
			else
			{
				num = this.textInfo.IndexOf(value, startIndex);
			}
			return num;
		}

		public int LastIndexOf(char value, int startIndex)
		{
			bool useAdvancedText = this.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				Debug.LogError("Cannot use LastIndexOf while using Advanced Text");
				num = 0;
			}
			else
			{
				num = this.textInfo.LastIndexOf(value, startIndex);
			}
			return num;
		}

		public void SelectCurrentWord(int index, ref int cursorIndex, ref int selectIndex)
		{
			bool flag = !this.useAdvancedText;
			if (flag)
			{
				Debug.LogError("Cannot use SelectCurrentWord while using Standard Text");
			}
			else
			{
				TextSelectionService.SelectCurrentWord(this.textGenerationInfo, index, ref cursorIndex, ref selectIndex);
			}
		}

		public void SelectCurrentParagraph(ref int cursorIndex, ref int selectIndex)
		{
			bool flag = !this.useAdvancedText;
			if (flag)
			{
				Debug.LogError("Cannot use SelectCurrentParagraph while using Standard Text");
			}
			else
			{
				TextSelectionService.SelectCurrentParagraph(this.textGenerationInfo, ref cursorIndex, ref selectIndex);
			}
		}

		public void SelectToPreviousParagraph(ref int cursorIndex)
		{
			bool flag = !this.useAdvancedText;
			if (flag)
			{
				Debug.LogError("Cannot use SelectToPreviousParagraph while using Standard Text");
			}
			else
			{
				TextSelectionService.SelectToPreviousParagraph(this.textGenerationInfo, ref cursorIndex);
			}
		}

		public void SelectToNextParagraph(ref int cursorIndex)
		{
			bool flag = !this.useAdvancedText;
			if (flag)
			{
				Debug.LogError("Cannot use SelectToNextParagraph while using Standard Text");
			}
			else
			{
				TextSelectionService.SelectToNextParagraph(this.textGenerationInfo, ref cursorIndex);
			}
		}

		public void SelectToStartOfParagraph(ref int cursorIndex)
		{
			bool flag = !this.useAdvancedText;
			if (flag)
			{
				Debug.LogError("Cannot use SelectToStartOfParagraph while using Standard Text");
			}
			else
			{
				TextSelectionService.SelectToStartOfParagraph(this.textGenerationInfo, ref cursorIndex);
			}
		}

		public void SelectToEndOfParagraph(ref int cursorIndex)
		{
			bool flag = !this.useAdvancedText;
			if (flag)
			{
				Debug.LogError("Cannot use SelectToEndOfParagraph while using Standard Text");
			}
			else
			{
				TextSelectionService.SelectToEndOfParagraph(this.textGenerationInfo, ref cursorIndex);
			}
		}

		internal virtual bool IsAdvancedTextEnabledForElement()
		{
			return false;
		}

		internal int GetTextElementCount()
		{
			bool useAdvancedText = this.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				Debug.LogError("Cannot use GetTextElementCount while using Advanced Text");
				num = 0;
			}
			else
			{
				num = this.textInfo.textElementInfo.Length;
			}
			return num;
		}

		internal TextHandle.GlyphMetricsForOverlay GetScaledCharacterMetrics(int i)
		{
			bool useAdvancedText = this.useAdvancedText;
			if (useAdvancedText)
			{
				throw new InvalidOperationException("Cannot use GetScaledCharacterMetrics while using Advanced Text");
			}
			return new TextHandle.GlyphMetricsForOverlay(ref this.textInfo.textElementInfo[i], this.GetPixelsPerPoint());
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static TextHandleTemporaryCache s_TemporaryCache = new TextHandleTemporaryCache();

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static TextHandlePermanentCache s_PermanentCache = new TextHandlePermanentCache();

		private static TextGenerationSettings[] s_Settings;

		private static TextGenerator[] s_Generators;

		private static TextInfo[] s_TextInfosCommon;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal NativeTextGenerationSettings nativeSettings = NativeTextGenerationSettings.Default;

		protected Vector2 pixelPreferedSize;

		private Rect m_ScreenRect;

		private float m_LineHeightDefault;

		private bool m_IsPlaceholder;

		protected bool m_IsElided;

		private int m_CreateGenerationIteration;

		private IntPtr m_TextGenerationInfo;

		private protected TextHandle.TextHandleFlags m_TextHandleFlags;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal int m_PreviousGenerationSettingsHash;

		protected bool isDirty;

		[Flags]
		internal enum TextHandleFlags
		{
			IsCachedPermanentTextCore = 2,
			IsCachedPermanentATG = 4
		}

		internal readonly struct GlyphMetricsForOverlay : IEquatable<TextHandle.GlyphMetricsForOverlay>
		{
			public GlyphMetricsForOverlay(ref TextElementInfo textElementInfo, float pixelPerPoint)
			{
				float num = 1f / pixelPerPoint;
				this.isVisible = textElementInfo.isVisible;
				this.origin = textElementInfo.origin * num;
				this.xAdvance = textElementInfo.xAdvance * num;
				this.ascentline = textElementInfo.ascender * num;
				this.baseline = textElementInfo.baseLine * num;
				this.descentline = textElementInfo.descender * num;
				this.topLeft = textElementInfo.topLeft * num;
				this.bottomLeft = textElementInfo.bottomLeft * num;
				this.topRight = textElementInfo.topRight * num;
				this.bottomRight = textElementInfo.bottomRight * num;
				this.scale = textElementInfo.scale;
				this.lineNumber = textElementInfo.lineNumber;
				this.fontCapLine = textElementInfo.fontAsset.faceInfo.capLine * num;
				this.fontMeanLine = textElementInfo.fontAsset.faceInfo.meanLine * num;
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("GlyphMetricsForOverlay");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append("isVisible = ");
				builder.Append(this.isVisible.ToString());
				builder.Append(", origin = ");
				builder.Append(this.origin.ToString());
				builder.Append(", xAdvance = ");
				builder.Append(this.xAdvance.ToString());
				builder.Append(", ascentline = ");
				builder.Append(this.ascentline.ToString());
				builder.Append(", baseline = ");
				builder.Append(this.baseline.ToString());
				builder.Append(", descentline = ");
				builder.Append(this.descentline.ToString());
				builder.Append(", topLeft = ");
				builder.Append(this.topLeft.ToString());
				builder.Append(", bottomLeft = ");
				builder.Append(this.bottomLeft.ToString());
				builder.Append(", topRight = ");
				builder.Append(this.topRight.ToString());
				builder.Append(", bottomRight = ");
				builder.Append(this.bottomRight.ToString());
				builder.Append(", scale = ");
				builder.Append(this.scale.ToString());
				builder.Append(", lineNumber = ");
				builder.Append(this.lineNumber.ToString());
				builder.Append(", fontCapLine = ");
				builder.Append(this.fontCapLine.ToString());
				builder.Append(", fontMeanLine = ");
				builder.Append(this.fontMeanLine.ToString());
				return true;
			}

			[CompilerGenerated]
			public static bool operator !=(TextHandle.GlyphMetricsForOverlay left, TextHandle.GlyphMetricsForOverlay right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(TextHandle.GlyphMetricsForOverlay left, TextHandle.GlyphMetricsForOverlay right)
			{
				return left.Equals(right);
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return ((((((((((((EqualityComparer<bool>.Default.GetHashCode(this.isVisible) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.origin)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.xAdvance)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.ascentline)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.baseline)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.descentline)) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(this.topLeft)) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(this.bottomLeft)) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(this.topRight)) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(this.bottomRight)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.scale)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.lineNumber)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.fontCapLine)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.fontMeanLine);
			}

			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return obj is TextHandle.GlyphMetricsForOverlay && this.Equals((TextHandle.GlyphMetricsForOverlay)obj);
			}

			[CompilerGenerated]
			public bool Equals(TextHandle.GlyphMetricsForOverlay other)
			{
				return EqualityComparer<bool>.Default.Equals(this.isVisible, other.isVisible) && EqualityComparer<float>.Default.Equals(this.origin, other.origin) && EqualityComparer<float>.Default.Equals(this.xAdvance, other.xAdvance) && EqualityComparer<float>.Default.Equals(this.ascentline, other.ascentline) && EqualityComparer<float>.Default.Equals(this.baseline, other.baseline) && EqualityComparer<float>.Default.Equals(this.descentline, other.descentline) && EqualityComparer<Vector3>.Default.Equals(this.topLeft, other.topLeft) && EqualityComparer<Vector3>.Default.Equals(this.bottomLeft, other.bottomLeft) && EqualityComparer<Vector3>.Default.Equals(this.topRight, other.topRight) && EqualityComparer<Vector3>.Default.Equals(this.bottomRight, other.bottomRight) && EqualityComparer<float>.Default.Equals(this.scale, other.scale) && EqualityComparer<int>.Default.Equals(this.lineNumber, other.lineNumber) && EqualityComparer<float>.Default.Equals(this.fontCapLine, other.fontCapLine) && EqualityComparer<float>.Default.Equals(this.fontMeanLine, other.fontMeanLine);
			}

			public readonly bool isVisible;

			public readonly float origin;

			public readonly float xAdvance;

			public readonly float ascentline;

			public readonly float baseline;

			public readonly float descentline;

			public readonly Vector3 topLeft;

			public readonly Vector3 bottomLeft;

			public readonly Vector3 topRight;

			public readonly Vector3 bottomRight;

			public readonly float scale;

			public readonly int lineNumber;

			public readonly float fontCapLine;

			public readonly float fontMeanLine;
		}
	}
}
