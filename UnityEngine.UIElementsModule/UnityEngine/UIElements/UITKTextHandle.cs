using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
	internal class UITKTextHandle : TextHandle
	{
		private List<ValueTuple<int, RichTextTagParser.TagType, string>> Links
		{
			get
			{
				List<ValueTuple<int, RichTextTagParser.TagType, string>> list;
				if ((list = this.m_Links) == null)
				{
					list = (this.m_Links = new List<ValueTuple<int, RichTextTagParser.TagType, string>>());
				}
				return list;
			}
		}

		private void ComputeNativeTextSize(in string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
		{
			bool flag = !this.ConvertUssToNativeTextGenerationSettings(textToMeasure, fontsize);
			if (!flag)
			{
				bool flag2 = string.IsNullOrEmpty(this.nativeSettings.text) && this.m_TextElement.isInputField;
				if (flag2)
				{
					this.nativeSettings.text = "\u200b";
				}
				bool flag3 = widthMode == VisualElement.MeasureMode.Undefined || float.IsNaN(width) || float.IsNegative(width);
				if (flag3)
				{
					this.nativeSettings.screenWidth = -1;
				}
				else
				{
					this.nativeSettings.screenWidth = (int)(width * 64f);
				}
				bool flag4 = heightMode == VisualElement.MeasureMode.Undefined || float.IsNaN(height) || float.IsNegative(height);
				if (flag4)
				{
					this.nativeSettings.screenHeight = -1;
				}
				else
				{
					this.nativeSettings.screenHeight = (int)(height * 64f);
				}
				bool flag5 = base.textGenerationInfo == IntPtr.Zero;
				if (flag5)
				{
					base.textGenerationInfo = TextGenerationInfo.Create(base.IsCachedPermanent);
				}
				this.pixelPreferedSize = this.textLib.MeasureText(this.nativeSettings, base.textGenerationInfo);
			}
		}

		public ValueTuple<NativeTextInfo, bool> UpdateNative(bool generateNativeSettings = true)
		{
			bool flag = generateNativeSettings && !this.ConvertUssToNativeTextGenerationSettings(null, null);
			ValueTuple<NativeTextInfo, bool> valueTuple;
			if (flag)
			{
				valueTuple = new ValueTuple<NativeTextInfo, bool>(default(NativeTextInfo), false);
			}
			else
			{
				bool hasLink = this.nativeSettings.hasLink;
				if (hasLink)
				{
					this.m_TextElement.uitkTextHandle.CacheTextGenerationInfo();
					if (this.m_ATGTextEventHandler == null)
					{
						this.m_ATGTextEventHandler = new ATGTextEventHandler(this.m_TextElement);
					}
				}
				bool flag2 = base.textGenerationInfo == IntPtr.Zero;
				if (flag2)
				{
					base.textGenerationInfo = TextGenerationInfo.Create(base.IsCachedPermanent);
				}
				bool flag3 = false;
				NativeTextInfo nativeTextInfo = this.textLib.GenerateText(this.nativeSettings, base.textGenerationInfo, ref flag3);
				bool flag4 = !flag3;
				if (flag4)
				{
					this.uvsAreGenerated = false;
				}
				this.m_IsElided = nativeTextInfo.isElided;
				valueTuple = new ValueTuple<NativeTextInfo, bool>(nativeTextInfo, true);
			}
			return valueTuple;
		}

		public void CacheTextGenerationInfo()
		{
			bool flag = !base.useAdvancedText;
			if (flag)
			{
				Debug.LogError("CacheTextGenerationInfo should only be called for ATG.");
			}
			else
			{
				bool flag2 = this.m_TextHandleFlags.HasFlag(TextHandle.TextHandleFlags.IsCachedPermanentATG);
				bool flag3 = flag2;
				if (!flag3)
				{
					bool flag4 = base.textGenerationInfo != IntPtr.Zero;
					if (flag4)
					{
						TextGenerationInfo.Destroy(base.textGenerationInfo);
						base.textGenerationInfo = IntPtr.Zero;
					}
					base.IsCachedPermanentATG = true;
					base.textGenerationInfo = TextGenerationInfo.Create(base.IsCachedPermanent);
				}
			}
		}

		public void ShapeText()
		{
			bool flag = !this.ConvertUssToNativeTextGenerationSettings(null, null);
			if (!flag)
			{
				bool flag2 = base.textGenerationInfo == IntPtr.Zero;
				if (flag2)
				{
					base.textGenerationInfo = TextGenerationInfo.Create(base.IsCachedPermanent);
				}
				this.textLib.ShapeText(this.nativeSettings, base.textGenerationInfo);
			}
		}

		public void ProcessMeshInfos(NativeTextInfo textInfo, ref List<List<List<int>>> textElementIndicesByMesh, ref List<bool> hasMultipleColorsByMesh)
		{
			this.textLib.ProcessMeshInfos(textInfo, this.nativeSettings, ref textElementIndicesByMesh, ref hasMultipleColorsByMesh, this.uvsAreGenerated);
			this.uvsAreGenerated = true;
		}

		public bool HasMissingGlyphs(NativeTextInfo textInfo, ref Dictionary<int, HashSet<uint>> missingGlyphsPerFontAsset)
		{
			return this.textLib.HasMissingGlyphs(textInfo, ref missingGlyphsPerFontAsset);
		}

		private ValueTuple<bool, bool> hasLinkAndHyperlink()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = this.m_Links != null;
			if (flag3)
			{
				foreach (ValueTuple<int, RichTextTagParser.TagType, string> valueTuple in this.Links)
				{
					RichTextTagParser.TagType item = valueTuple.Item2;
					flag = flag || item == RichTextTagParser.TagType.Link;
					flag2 = flag2 || item == RichTextTagParser.TagType.Hyperlink;
					bool flag4 = flag && flag2;
					if (flag4)
					{
						break;
					}
				}
			}
			return new ValueTuple<bool, bool>(flag, flag2);
		}

		internal ValueTuple<RichTextTagParser.TagType, string> ATGFindIntersectingLink(Vector2 point)
		{
			Debug.Assert(base.useAdvancedText);
			bool flag = base.textGenerationInfo == IntPtr.Zero;
			ValueTuple<RichTextTagParser.TagType, string> valueTuple;
			if (flag)
			{
				Debug.LogError("TextGenerationInfo pointer is null.");
				valueTuple = new ValueTuple<RichTextTagParser.TagType, string>(RichTextTagParser.TagType.Unknown, null);
			}
			else
			{
				int num = TextLib.FindIntersectingLink(point * this.GetPixelsPerPoint(), base.textGenerationInfo);
				bool flag2 = num == -1;
				if (flag2)
				{
					valueTuple = new ValueTuple<RichTextTagParser.TagType, string>(RichTextTagParser.TagType.Unknown, null);
				}
				else
				{
					valueTuple = new ValueTuple<RichTextTagParser.TagType, string>(this.m_Links[num].Item2, this.m_Links[num].Item3);
				}
			}
			return valueTuple;
		}

		internal void UpdateATGTextEventHandler()
		{
			bool flag = this.m_ATGTextEventHandler == null;
			if (!flag)
			{
				ValueTuple<bool, bool> valueTuple = this.hasLinkAndHyperlink();
				bool item = valueTuple.Item1;
				bool item2 = valueTuple.Item2;
				bool flag2 = item;
				if (flag2)
				{
					this.m_ATGTextEventHandler.RegisterLinkTagCallbacks();
				}
				else
				{
					this.m_ATGTextEventHandler.UnRegisterLinkTagCallbacks();
				}
				bool flag3 = item2;
				if (flag3)
				{
					this.m_ATGTextEventHandler.RegisterHyperlinkCallbacks();
				}
				else
				{
					this.m_ATGTextEventHandler.UnRegisterHyperlinkCallbacks();
				}
			}
		}

		internal void EnsureIsReadyForJobs()
		{
			this.InitTextLib();
			FontAsset fontAsset = TextUtilities.GetFontAsset(this.m_TextElement);
			bool flag = fontAsset == null;
			if (!flag)
			{
				TextUtilities.GetTextSettingsFrom(this.m_TextElement).UpdateNativeTextSettings();
				fontAsset.EnsureNativeFontAssetIsCreated();
			}
		}

		[global::System.Runtime.CompilerServices.NullableContext(2)]
		internal unsafe bool ConvertUssToNativeTextGenerationSettings(string textToMeasure = null, float? fontsize = null)
		{
			float pixelsPerPoint = this.GetPixelsPerPoint();
			ComputedStyle computedStyle = *this.m_TextElement.computedStyle;
			this.nativeSettings.preProcessFlags = PreProcessFlags.None;
			this.nativeSettings.text = ((this.m_TextElement.isElided && !this.TextLibraryCanElide()) ? this.m_TextElement.elidedText : this.m_TextElement.renderedTextString);
			bool flag = textToMeasure != null;
			if (flag)
			{
				this.nativeSettings.text = textToMeasure;
			}
			bool flag2 = this.nativeSettings.text == null;
			if (flag2)
			{
				this.nativeSettings.text = "";
			}
			float num = (fontsize ?? computedStyle.fontSize.value) * pixelsPerPoint;
			this.nativeSettings.fontSize = (int)Math.Round((double)(num * 64f), MidpointRounding.AwayFromZero);
			this.nativeSettings.bestFit = computedStyle.unityTextAutoSize.mode == TextAutoSizeMode.BestFit;
			this.nativeSettings.maxFontSize = (int)(computedStyle.unityTextAutoSize.maxSize.value * 64f * pixelsPerPoint);
			this.nativeSettings.minFontSize = (int)(computedStyle.unityTextAutoSize.minSize.value * 64f * pixelsPerPoint);
			this.nativeSettings.wordWrapEnabled = computedStyle.whiteSpace == WhiteSpace.Normal || computedStyle.whiteSpace == WhiteSpace.PreWrap;
			bool flag3 = !this.m_TextElement.isInputField && (computedStyle.whiteSpace == WhiteSpace.NoWrap || computedStyle.whiteSpace == WhiteSpace.Normal);
			if (flag3)
			{
				this.nativeSettings.preProcessFlags = this.nativeSettings.preProcessFlags | PreProcessFlags.CollapseWhiteSpaces;
			}
			bool parseEscapeSequences = this.m_TextElement.parseEscapeSequences;
			if (parseEscapeSequences)
			{
				this.nativeSettings.preProcessFlags = this.nativeSettings.preProcessFlags | PreProcessFlags.ParseEscapeSequences;
			}
			this.nativeSettings.overflow = computedStyle.textOverflow.toTextCore(computedStyle.overflow, computedStyle.unityTextOverflowPosition);
			this.nativeSettings.horizontalAlignment = TextGeneratorUtilities.GetHorizontalAlignment(computedStyle.unityTextAlign);
			this.nativeSettings.verticalAlignment = TextGeneratorUtilities.GetVerticalAlignment(computedStyle.unityTextAlign);
			this.nativeSettings.characterSpacing = (int)(computedStyle.letterSpacing.value * 64f);
			this.nativeSettings.wordSpacing = (int)(computedStyle.wordSpacing.value * 64f);
			this.nativeSettings.paragraphSpacing = (int)(computedStyle.unityParagraphSpacing.value * 64f);
			this.nativeSettings.color = computedStyle.color;
			this.nativeSettings.color = this.nativeSettings.color * this.m_TextElement.playModeTintColor;
			this.nativeSettings.languageDirection = this.m_TextElement.localLanguageDirection.toTextCore();
			FontStyles fontStyles = TextGeneratorUtilities.LegacyStyleToNewStyle(computedStyle.unityFontStyleAndWeight);
			this.nativeSettings.fontStyle = fontStyles & ~FontStyles.Bold;
			this.nativeSettings.fontWeight = (((fontStyles & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : TextFontWeight.Regular);
			Vector2 size = this.m_TextElement.contentRect.size;
			bool flag4 = this.ATGMeasuredWidth != null && Mathf.Abs(size.x - this.ATGRoundedWidth) < 0.01f && this.LastPixelPerPoint == pixelsPerPoint;
			if (flag4)
			{
				size.x = this.ATGMeasuredWidth.Value;
			}
			else
			{
				this.ATGRoundedWidth = size.x;
				this.ATGMeasuredWidth = null;
			}
			this.nativeSettings.screenWidth = Mathf.RoundToInt(size.x * 64f * pixelsPerPoint);
			this.nativeSettings.screenHeight = Mathf.RoundToInt(size.y * 64f * pixelsPerPoint);
			FontAsset fontAsset = TextUtilities.GetFontAsset(this.m_TextElement);
			bool flag5 = fontAsset == null;
			bool flag6;
			if (flag5)
			{
				flag6 = false;
			}
			else
			{
				bool flag7 = fontAsset.atlasPopulationMode == AtlasPopulationMode.Static;
				if (flag7)
				{
					Debug.LogError("Advanced text system cannot render using static font asset " + fontAsset.faceInfo.familyName);
					flag6 = false;
				}
				else
				{
					this.nativeSettings.vertexPadding = (int)(this.GetVertexPadding(fontAsset) * 64f);
					this.nativeSettings.fontAsset = fontAsset.nativeFontAsset;
					bool flag8 = fontAsset.nativeFontAsset == IntPtr.Zero;
					if (flag8)
					{
						flag6 = false;
					}
					else
					{
						this.nativeSettings.textSettings = TextUtilities.GetTextSettingsFrom(this.m_TextElement).nativeTextSettings;
						bool flag9 = this.m_TextElement.enableRichText && RichTextTagParser.MayNeedParsing(this.nativeSettings.text);
						if (flag9)
						{
							TextPreprocessor.PreProcessString(ref this.nativeSettings.text, this.nativeSettings.preProcessFlags, TextUtilities.GetTextSettingsFrom(this.m_TextElement));
							this.nativeSettings.preProcessFlags = PreProcessFlags.None;
							RichTextTagParser.CreateTextGenerationSettingsArray(ref this.nativeSettings, this.Links, this.atgHyperlinkColor, this.GetPixelsPerPoint(), TextUtilities.GetTextSettingsFrom(this.m_TextElement));
						}
						else
						{
							this.nativeSettings.textSpans = null;
						}
						flag6 = true;
					}
				}
			}
			return flag6;
		}

		internal void EnsureFontAssetsAreCreatedOnTheMainThread()
		{
			FontAsset fontAsset = TextUtilities.GetFontAsset(this.m_TextElement);
			fontAsset.EnsureNativeFontAssetIsCreated();
		}

		private TextAsset GetICUAsset()
		{
			bool flag = this.m_TextElement.panel == null;
			if (flag)
			{
				throw new InvalidOperationException("Text cannot be processed on elements not in a panel");
			}
			TextAsset textAsset = ((PanelSettings)((RuntimePanel)this.m_TextElement.panel).ownerObject).m_ICUDataAsset;
			bool flag2 = textAsset != null;
			TextAsset textAsset2;
			if (flag2)
			{
				textAsset2 = textAsset;
			}
			else
			{
				textAsset = UITKTextHandle.GetICUAssetStaticFalback();
				bool flag3 = textAsset != null;
				if (flag3)
				{
					textAsset2 = textAsset;
				}
				else
				{
					Debug.LogError("ICU Data not available. The data should be automatically assigned to the PanelSettings in the editor if the advanced text option is enable in the project settings. It will not be present on PanelSettings created at runtime, so make sure the build contains at least one PanelSettings asset");
					textAsset2 = null;
				}
			}
			return textAsset2;
		}

		internal static TextAsset GetICUAssetStaticFalback()
		{
			foreach (TextAsset textAsset in Resources.FindObjectsOfTypeAll<TextAsset>())
			{
				bool flag = textAsset.name == "icudt73l";
				if (flag)
				{
					return textAsset;
				}
			}
			return null;
		}

		protected internal TextLib textLib
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				this.InitTextLib();
				return UITKTextHandle.s_TextLib;
			}
		}

		protected internal void InitTextLib()
		{
			if (UITKTextHandle.s_TextLib == null)
			{
				UITKTextHandle.s_TextLib = new TextLib(this.GetICUAsset().bytes);
			}
		}

		public UITKTextHandle(TextElement te)
		{
			this.m_TextElement = te;
			this.m_TextEventHandler = new TextEventHandler(te);
		}

		protected override float GetPixelsPerPoint()
		{
			TextElement textElement = this.m_TextElement;
			return (textElement != null) ? textElement.scaledPixelsPerPoint : 1f;
		}

		internal float LastPixelPerPoint { get; set; }

		public override void SetDirty()
		{
			this.MeasuredWidth = null;
			this.ATGMeasuredWidth = null;
			base.SetDirty();
		}

		internal float? MeasuredWidth { get; set; }

		internal float RoundedWidth { get; set; }

		internal float? ATGMeasuredWidth { get; set; }

		internal float ATGRoundedWidth { get; set; }

		public Vector2 ComputeTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
		{
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this.m_TextElement);
			Vector2 vector;
			if (flag)
			{
				RenderedText renderedText = new RenderedText(textToMeasure);
				vector = this.ComputeTextSize(in renderedText, width, height, fontsize);
			}
			else
			{
				float pixelsPerPoint = this.GetPixelsPerPoint();
				width = Mathf.Floor(width * pixelsPerPoint);
				height = Mathf.Floor(height * pixelsPerPoint);
				this.ComputeNativeTextSize(in textToMeasure, width, widthMode, height, heightMode, fontsize);
				vector = base.preferredSize;
			}
			return vector;
		}

		public Vector2 ComputeTextSize(in RenderedText textToMeasure, float width, float height, float? fontsize = null)
		{
			bool flag = TextUtilities.IsAdvancedTextEnabledForElement(this.m_TextElement);
			Vector2 vector;
			if (flag)
			{
				vector = Vector2.zero;
			}
			else
			{
				float pixelsPerPoint = this.GetPixelsPerPoint();
				width = Mathf.Floor(width * pixelsPerPoint);
				height = Mathf.Floor(height * pixelsPerPoint);
				this.ConvertUssToTextGenerationSettings(false, fontsize);
				TextHandle.settings.renderedText = textToMeasure;
				TextHandle.settings.screenRect = new Rect(0f, 0f, width, height);
				base.UpdatePreferredValues(TextHandle.settings);
				vector = base.preferredSize;
			}
			return vector;
		}

		public void ComputeSettingsAndUpdate()
		{
			bool useAdvancedText = base.useAdvancedText;
			if (useAdvancedText)
			{
				this.UpdateNative(true);
				this.UpdateATGTextEventHandler();
			}
			else
			{
				this.UpdateMesh();
				this.HandleATag();
				this.HandleLinkTag();
				this.HandleLinkAndATagCallbacks();
			}
		}

		public void HandleATag()
		{
			TextEventHandler textEventHandler = this.m_TextEventHandler;
			if (textEventHandler != null)
			{
				textEventHandler.HandleATag();
			}
		}

		public void HandleLinkTag()
		{
			TextEventHandler textEventHandler = this.m_TextEventHandler;
			if (textEventHandler != null)
			{
				textEventHandler.HandleLinkTag();
			}
		}

		public void HandleLinkAndATagCallbacks()
		{
			TextEventHandler textEventHandler = this.m_TextEventHandler;
			if (textEventHandler != null)
			{
				textEventHandler.HandleLinkAndATagCallbacks();
			}
		}

		public void UpdateMesh()
		{
			this.ConvertUssToTextGenerationSettings(true, null);
			int hashCode = TextHandle.settings.GetHashCode();
			bool flag = this.m_PreviousGenerationSettingsHash == hashCode && !this.isDirty;
			if (flag)
			{
				base.AddTextInfoToTemporaryCache(hashCode);
			}
			else
			{
				base.RemoveFromTemporaryCache();
				base.UpdateWithHash(hashCode);
			}
		}

		public override void AddToPermanentCacheAndGenerateMesh()
		{
			bool useAdvancedText = base.useAdvancedText;
			if (useAdvancedText)
			{
				this.CacheTextGenerationInfo();
				this.UpdateNative(true);
				this.UpdateATGTextEventHandler();
			}
			else
			{
				bool flag = this.ConvertUssToTextGenerationSettings(true, null);
				if (flag)
				{
					base.AddToPermanentCacheAndGenerateMesh();
				}
			}
			this.ReleaseResourcesIfPossible();
		}

		private unsafe TextOverflowMode GetTextOverflowMode()
		{
			ComputedStyle computedStyle = *this.m_TextElement.computedStyle;
			bool flag = computedStyle.textOverflow == TextOverflow.Clip;
			TextOverflowMode textOverflowMode;
			if (flag)
			{
				textOverflowMode = TextOverflowMode.Masking;
			}
			else
			{
				bool flag2 = computedStyle.textOverflow != TextOverflow.Ellipsis;
				if (flag2)
				{
					textOverflowMode = TextOverflowMode.Overflow;
				}
				else
				{
					bool flag3 = !this.TextLibraryCanElide();
					if (flag3)
					{
						textOverflowMode = TextOverflowMode.Masking;
					}
					else
					{
						bool flag4 = computedStyle.overflow == OverflowInternal.Hidden;
						if (flag4)
						{
							textOverflowMode = TextOverflowMode.Ellipsis;
						}
						else
						{
							textOverflowMode = TextOverflowMode.Overflow;
						}
					}
				}
			}
			return textOverflowMode;
		}

		internal unsafe virtual bool ConvertUssToTextGenerationSettings(bool populateScreenRect, float? fontsize = null)
		{
			ComputedStyle computedStyle = *this.m_TextElement.computedStyle;
			TextGenerationSettings settings = TextHandle.settings;
			bool flag = computedStyle.unityTextAutoSize != TextAutoSize.None();
			if (flag)
			{
				Debug.LogWarning("TextAutoSize is not supported with the Standard TextGenerator. Please use Advanced Text Generation instead.");
			}
			settings.text = string.Empty;
			settings.isIMGUI = false;
			settings.textSettings = TextUtilities.GetTextSettingsFrom(this.m_TextElement);
			bool flag2 = settings.textSettings == null;
			bool flag3;
			if (flag2)
			{
				flag3 = false;
			}
			else
			{
				settings.fontAsset = TextUtilities.GetFontAsset(this.m_TextElement);
				bool flag4 = settings.fontAsset == null;
				if (flag4)
				{
					flag3 = false;
				}
				else
				{
					settings.extraPadding = this.GetVertexPadding(settings.fontAsset);
					settings.renderedText = ((this.m_TextElement.isElided && !this.TextLibraryCanElide()) ? new RenderedText(this.m_TextElement.elidedText) : this.m_TextElement.renderedText);
					settings.isPlaceholder = this.m_TextElement.showPlaceholderText;
					float pixelsPerPoint = this.GetPixelsPerPoint();
					float num = fontsize ?? computedStyle.fontSize.value;
					settings.fontSize = (int)Math.Round((double)(num * pixelsPerPoint), MidpointRounding.AwayFromZero);
					settings.fontStyle = TextGeneratorUtilities.LegacyStyleToNewStyle(computedStyle.unityFontStyleAndWeight);
					settings.textAlignment = TextGeneratorUtilities.LegacyAlignmentToNewAlignment(computedStyle.unityTextAlign);
					settings.textWrappingMode = computedStyle.whiteSpace.toTextWrappingMode(this.m_TextElement.isInputField && !this.m_TextElement.edition.multiline);
					settings.richText = this.m_TextElement.enableRichText;
					settings.overflowMode = this.GetTextOverflowMode();
					settings.characterSpacing = computedStyle.letterSpacing.value;
					settings.wordSpacing = computedStyle.wordSpacing.value;
					settings.paragraphSpacing = computedStyle.unityParagraphSpacing.value;
					settings.color = computedStyle.color;
					settings.color *= this.m_TextElement.playModeTintColor;
					settings.shouldConvertToLinearSpace = false;
					settings.parseControlCharacters = this.m_TextElement.parseEscapeSequences;
					settings.isRightToLeft = this.m_TextElement.localLanguageDirection == LanguageDirection.RTL;
					settings.emojiFallbackSupport = this.m_TextElement.emojiFallbackSupport;
					TextHandle.settings.pixelsPerPoint = pixelsPerPoint;
					if (populateScreenRect)
					{
						Vector2 size = this.m_TextElement.contentRect.size;
						bool flag5 = this.MeasuredWidth != null && Mathf.Abs(size.x - this.RoundedWidth) < 0.01f && this.LastPixelPerPoint == pixelsPerPoint;
						if (flag5)
						{
							size.x = this.MeasuredWidth.Value;
						}
						else
						{
							this.RoundedWidth = size.x;
							this.MeasuredWidth = null;
							this.LastPixelPerPoint = pixelsPerPoint;
						}
						size.x *= pixelsPerPoint;
						size.y *= pixelsPerPoint;
						bool flag6 = settings.fontAsset.IsBitmap();
						if (flag6)
						{
							size.x = Mathf.Round(size.x);
							size.y = Mathf.Round(size.y);
						}
						settings.screenRect = new Rect(Vector2.zero, size);
					}
					flag3 = true;
				}
			}
			return flag3;
		}

		internal bool TextLibraryCanElide()
		{
			return this.m_TextElement.computedStyle.unityTextOverflowPosition == TextOverflowPosition.End;
		}

		internal unsafe float GetVertexPadding(FontAsset fontAsset)
		{
			ComputedStyle computedStyle = *this.m_TextElement.computedStyle;
			float num = computedStyle.unityTextOutlineWidth / 2f;
			float num2 = Mathf.Abs(computedStyle.textShadow.offset.x);
			float num3 = Mathf.Abs(computedStyle.textShadow.offset.y);
			float num4 = Mathf.Abs(computedStyle.textShadow.blurRadius);
			bool flag = num <= 0f && num2 <= 0f && num3 <= 0f && num4 <= 0f;
			float num5;
			if (flag)
			{
				num5 = UITKTextHandle.k_MinPadding;
			}
			else
			{
				float num6 = Mathf.Max(num2 + num4, num);
				float num7 = Mathf.Max(num3 + num4, num);
				float num8 = Mathf.Max(num6, num7) + UITKTextHandle.k_MinPadding;
				float num9 = TextHandle.ConvertPixelUnitsToTextCoreRelativeUnits(computedStyle.fontSize.value, fontAsset);
				int num10 = fontAsset.atlasPadding + 1;
				num5 = Mathf.Min(num8 * num9 * (float)num10, (float)num10);
			}
			return num5;
		}

		internal override bool IsAdvancedTextEnabledForElement()
		{
			return TextUtilities.IsAdvancedTextEnabledForElement(this.m_TextElement);
		}

		internal void ReleaseResourcesIfPossible()
		{
			bool flag = TextUtilities.IsAdvancedTextEnabledForElement(this.m_TextElement);
			bool flag2 = !flag;
			if (flag2)
			{
				base.RemoveFromPermanentCacheATG();
				bool flag3 = this.m_ATGTextEventHandler != null;
				if (flag3)
				{
					ATGTextEventHandler atgtextEventHandler = this.m_ATGTextEventHandler;
					if (atgtextEventHandler != null)
					{
						atgtextEventHandler.OnDestroy();
					}
					this.m_ATGTextEventHandler = null;
				}
				bool flag4 = this.m_TextEventHandler == null;
				if (flag4)
				{
					this.m_TextEventHandler = new TextEventHandler(this.m_TextElement);
				}
			}
			else
			{
				bool isCachedPermanentTextCore = base.IsCachedPermanentTextCore;
				if (isCachedPermanentTextCore)
				{
					base.RemoveFromPermanentCacheTextCore();
				}
				bool isCachedTemporary = base.IsCachedTemporary;
				if (isCachedTemporary)
				{
					base.RemoveFromTemporaryCache();
				}
				bool flag5 = this.m_TextEventHandler != null;
				if (flag5)
				{
					TextEventHandler textEventHandler = this.m_TextEventHandler;
					if (textEventHandler != null)
					{
						textEventHandler.OnDestroy();
					}
					this.m_TextEventHandler = null;
				}
			}
		}

		public override bool IsPlaceholder
		{
			get
			{
				return base.useAdvancedText ? this.m_TextElement.showPlaceholderText : base.IsPlaceholder;
			}
		}

		public bool IsElided()
		{
			bool flag = string.IsNullOrEmpty(this.m_TextElement.text);
			return flag || this.m_IsElided;
		}

		internal ATGTextEventHandler m_ATGTextEventHandler;

		private List<ValueTuple<int, RichTextTagParser.TagType, string>> m_Links;

		internal Color atgHyperlinkColor = Color.blue;

		private bool uvsAreGenerated = false;

		private static TextLib s_TextLib;

		internal TextEventHandler m_TextEventHandler;

		protected TextElement m_TextElement;

		internal static readonly float k_MinPadding = 6f;
	}
}
