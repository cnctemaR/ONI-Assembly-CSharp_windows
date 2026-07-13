using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TextCore.Text;

namespace UnityEngine
{
	internal class IMGUITextHandle : TextHandle
	{
		internal static void EmptyCache()
		{
			GUIStyle.Internal_CleanupAllTextGenerator();
			IMGUITextHandle.textHandles.Clear();
			IMGUITextHandle.textHandlesTuple.Clear();
		}

		internal static void EmptyManagedCache()
		{
			IMGUITextHandle.textHandles.Clear();
			IMGUITextHandle.textHandlesTuple.Clear();
		}

		internal static IMGUITextHandle GetTextHandle(GUIStyle style, Rect position, string content, Color32 textColor)
		{
			bool flag = false;
			IMGUITextHandle.ConvertGUIStyleToGenerationSettings(TextHandle.settings, style, textColor, content, position);
			return IMGUITextHandle.GetTextHandle(TextHandle.settings, false, ref flag);
		}

		internal static IMGUITextHandle GetTextHandle(GUIStyle style, Rect position, string content, Color32 textColor, ref bool isCached)
		{
			IMGUITextHandle.ConvertGUIStyleToGenerationSettings(TextHandle.settings, style, textColor, content, position);
			return IMGUITextHandle.GetTextHandle(TextHandle.settings, true, ref isCached);
		}

		private static bool ShouldCleanup(float currentTime, float lastTime, float cleanupThreshold)
		{
			float num = currentTime - lastTime;
			return num > cleanupThreshold || num < 0f;
		}

		private static void ClearUnusedTextHandles()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			while (IMGUITextHandle.textHandlesTuple.Count > 0)
			{
				IMGUITextHandle.TextHandleTuple textHandleTuple = IMGUITextHandle.textHandlesTuple.First<IMGUITextHandle.TextHandleTuple>();
				bool flag = IMGUITextHandle.ShouldCleanup(realtimeSinceStartup, textHandleTuple.lastTimeUsed, 5f);
				if (!flag)
				{
					break;
				}
				GUIStyle.Internal_DestroyTextGenerator(textHandleTuple.hashCode);
				IMGUITextHandle imguitextHandle;
				bool flag2 = IMGUITextHandle.textHandles.TryGetValue(textHandleTuple.hashCode, out imguitextHandle);
				if (flag2)
				{
					imguitextHandle.RemoveFromPermanentCache();
				}
				IMGUITextHandle.textHandles.Remove(textHandleTuple.hashCode);
				IMGUITextHandle.textHandlesTuple.RemoveFirst();
			}
		}

		private static IMGUITextHandle GetTextHandle(TextGenerationSettings settings, bool isCalledFromNative, ref bool isCached)
		{
			isCached = false;
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			bool flag = IMGUITextHandle.ShouldCleanup(realtimeSinceStartup, IMGUITextHandle.lastCleanupTime, 30f) || IMGUITextHandle.newHandlesSinceCleanup > 500;
			if (flag)
			{
				IMGUITextHandle.ClearUnusedTextHandles();
				IMGUITextHandle.lastCleanupTime = realtimeSinceStartup;
				IMGUITextHandle.newHandlesSinceCleanup = 0;
			}
			int hashCode = settings.GetHashCode();
			IMGUITextHandle imguitextHandle;
			bool flag2 = IMGUITextHandle.textHandles.TryGetValue(hashCode, out imguitextHandle);
			IMGUITextHandle imguitextHandle2;
			if (flag2)
			{
				IMGUITextHandle.textHandlesTuple.Remove(imguitextHandle.tuple);
				IMGUITextHandle.textHandlesTuple.AddLast(imguitextHandle.tuple);
				isCached = !isCalledFromNative || imguitextHandle.isCachedOnNative;
				bool flag3 = !imguitextHandle.isCachedOnNative && isCalledFromNative;
				if (flag3)
				{
					imguitextHandle.UpdateWithHash(hashCode);
					imguitextHandle.UpdatePreferredSize();
					imguitextHandle.isCachedOnNative = true;
				}
				imguitextHandle2 = imguitextHandle;
			}
			else
			{
				IMGUITextHandle imguitextHandle3 = new IMGUITextHandle();
				IMGUITextHandle.TextHandleTuple textHandleTuple = new IMGUITextHandle.TextHandleTuple(realtimeSinceStartup, hashCode);
				LinkedListNode<IMGUITextHandle.TextHandleTuple> linkedListNode = new LinkedListNode<IMGUITextHandle.TextHandleTuple>(textHandleTuple);
				imguitextHandle3.tuple = linkedListNode;
				IMGUITextHandle.textHandles[hashCode] = imguitextHandle3;
				imguitextHandle3.UpdateWithHash(hashCode);
				imguitextHandle3.UpdatePreferredSize();
				IMGUITextHandle.textHandlesTuple.AddLast(linkedListNode);
				imguitextHandle3.isCachedOnNative = isCalledFromNative;
				IMGUITextHandle.newHandlesSinceCleanup++;
				imguitextHandle2 = imguitextHandle3;
			}
			return imguitextHandle2;
		}

		protected override float GetPixelsPerPoint()
		{
			return GUIUtility.pixelsPerPoint;
		}

		internal static float GetLineHeight(GUIStyle style)
		{
			IMGUITextHandle.ConvertGUIStyleToGenerationSettings(TextHandle.settings, style, Color.white, "", Rect.zero);
			return TextHandle.GetLineHeightDefault(TextHandle.settings) / GUIUtility.pixelsPerPoint;
		}

		internal int GetNumCharactersThatFitWithinWidth(float width)
		{
			this.AddToPermanentCacheAndGenerateMesh();
			int characterCount = base.textInfo.lineInfo[0].characterCount;
			float num = 0f;
			width = base.PointsToPixels(width);
			int i;
			for (i = 0; i < characterCount; i++)
			{
				num += base.textInfo.textElementInfo[i].xAdvance - base.textInfo.textElementInfo[i].origin;
				bool flag = num > width;
				if (flag)
				{
					break;
				}
			}
			return i;
		}

		public Rect[] GetHyperlinkRects(Rect content)
		{
			this.AddToPermanentCacheAndGenerateMesh();
			List<Rect> list = new List<Rect>();
			float num = 1f / this.GetPixelsPerPoint();
			for (int i = 0; i < base.textInfo.linkCount; i++)
			{
				Vector2 vector = base.GetCursorPositionFromStringIndexUsingLineHeight(base.textInfo.linkInfo[i].linkTextfirstCharacterIndex, false, true) + new Vector2(content.x, content.y);
				Vector2 vector2 = base.GetCursorPositionFromStringIndexUsingLineHeight(base.textInfo.linkInfo[i].linkTextLength + base.textInfo.linkInfo[i].linkTextfirstCharacterIndex, false, true) + new Vector2(content.x, content.y);
				float num2 = base.textInfo.lineInfo[0].lineHeight * num;
				bool flag = vector.y == vector2.y;
				if (flag)
				{
					list.Add(new Rect(vector.x, vector.y - num2, vector2.x - vector.x, num2));
				}
				else
				{
					list.Add(new Rect(vector.x, vector.y - num2, base.textInfo.lineInfo[0].width * num - vector.x, num2));
					list.Add(new Rect(content.x, vector.y, base.textInfo.lineInfo[0].width * num, vector2.y - vector.y - num2));
					bool flag2 = vector2.x != 0f;
					if (flag2)
					{
						list.Add(new Rect(content.x, vector2.y - num2, vector2.x, num2));
					}
				}
			}
			return list.ToArray();
		}

		private static void ConvertGUIStyleToGenerationSettings(TextGenerationSettings settings, GUIStyle style, Color textColor, string text, Rect rect)
		{
			settings.textSettings = RuntimeTextSettings.defaultTextSettings;
			bool flag = settings.textSettings == null;
			if (!flag)
			{
				Font font = style.font;
				bool flag2 = !font;
				if (flag2)
				{
					font = GUIStyle.GetDefaultFont();
				}
				float pixelsPerPoint = GUIUtility.pixelsPerPoint;
				bool flag3 = style.fontSize > 0;
				if (flag3)
				{
					settings.fontSize = Mathf.RoundToInt((float)style.fontSize * pixelsPerPoint);
				}
				else
				{
					bool flag4 = font;
					if (flag4)
					{
						settings.fontSize = Mathf.RoundToInt((float)font.fontSize * pixelsPerPoint);
					}
					else
					{
						settings.fontSize = Mathf.RoundToInt(13f * pixelsPerPoint);
					}
				}
				settings.fontStyle = TextGeneratorUtilities.LegacyStyleToNewStyle(style.fontStyle);
				settings.fontAsset = settings.textSettings.GetCachedFontAsset(font);
				bool flag5 = settings.fontAsset == null;
				if (!flag5)
				{
					bool flag6 = settings.fontAsset.IsBitmap();
					if (flag6)
					{
						settings.screenRect = new Rect(0f, 0f, Mathf.Max(0f, Mathf.Round(rect.width * pixelsPerPoint)), Mathf.Max(0f, Mathf.Round(rect.height * pixelsPerPoint)));
					}
					else
					{
						settings.screenRect = new Rect(0f, 0f, Mathf.Max(0f, rect.width * pixelsPerPoint), Mathf.Max(0f, rect.height * pixelsPerPoint));
						settings.fontAsset.material.SetFloat("_Sharpness", 0.5f);
					}
					settings.text = text;
					TextAnchor textAnchor = style.alignment;
					bool flag7 = style.imagePosition == ImagePosition.ImageAbove;
					if (flag7)
					{
						switch (style.alignment)
						{
						case TextAnchor.MiddleLeft:
						case TextAnchor.LowerLeft:
							textAnchor = TextAnchor.UpperLeft;
							break;
						case TextAnchor.MiddleCenter:
						case TextAnchor.LowerCenter:
							textAnchor = TextAnchor.UpperCenter;
							break;
						case TextAnchor.MiddleRight:
						case TextAnchor.LowerRight:
							textAnchor = TextAnchor.UpperRight;
							break;
						}
					}
					settings.textAlignment = TextGeneratorUtilities.LegacyAlignmentToNewAlignment(textAnchor);
					settings.overflowMode = IMGUITextHandle.LegacyClippingToNewOverflow(style.clipping);
					bool flag8 = rect.width > 0f && style.wordWrap;
					if (flag8)
					{
						settings.textWrappingMode = TextWrappingMode.PreserveWhitespace;
					}
					else
					{
						settings.textWrappingMode = TextWrappingMode.PreserveWhitespaceNoWrap;
					}
					settings.richText = style.richText;
					settings.parseControlCharacters = false;
					settings.isPlaceholder = false;
					settings.isRightToLeft = false;
					settings.characterSpacing = 0f;
					settings.wordSpacing = 0f;
					settings.paragraphSpacing = 0f;
					settings.color = textColor;
					settings.isIMGUI = true;
					settings.shouldConvertToLinearSpace = QualitySettings.activeColorSpace == ColorSpace.Linear;
					settings.emojiFallbackSupport = true;
					settings.extraPadding = 6f;
					settings.pixelsPerPoint = pixelsPerPoint;
				}
			}
		}

		private static TextOverflowMode LegacyClippingToNewOverflow(TextClipping clipping)
		{
			switch (clipping)
			{
			case TextClipping.Clip:
				return TextOverflowMode.Masking;
			case TextClipping.Ellipsis:
				return TextOverflowMode.Ellipsis;
			}
			return TextOverflowMode.Overflow;
		}

		internal override bool IsAdvancedTextEnabledForElement()
		{
			return false;
		}

		internal LinkedListNode<IMGUITextHandle.TextHandleTuple> tuple;

		private const float sFallbackFontSize = 13f;

		private const float sTimeToFlush = 5f;

		private const float sTimeBetweenCleanupRuns = 30f;

		private const int sNewHandlesBetweenCleanupRuns = 500;

		private static Dictionary<int, IMGUITextHandle> textHandles = new Dictionary<int, IMGUITextHandle>();

		private static LinkedList<IMGUITextHandle.TextHandleTuple> textHandlesTuple = new LinkedList<IMGUITextHandle.TextHandleTuple>();

		private static float lastCleanupTime;

		private static int newHandlesSinceCleanup = 0;

		internal bool isCachedOnNative = false;

		internal class TextHandleTuple
		{
			public TextHandleTuple(float lastTimeUsed, int hashCode)
			{
				this.hashCode = hashCode;
				this.lastTimeUsed = lastTimeUsed;
			}

			public float lastTimeUsed;

			public int hashCode;
		}
	}
}
