using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text
{
	internal class TextGenerator
	{
		private static TextGenerator GetTextGenerator()
		{
			bool flag = TextGenerator.s_TextGenerator == null;
			if (flag)
			{
				TextGenerator.s_TextGenerator = new TextGenerator();
			}
			return TextGenerator.s_TextGenerator;
		}

		public static void GenerateText(TextGenerationSettings settings, TextInfo textInfo)
		{
			bool flag = settings.fontAsset == null || settings.fontAsset.characterLookupTable == null;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
			}
			else
			{
				bool flag2 = textInfo == null;
				if (flag2)
				{
					Debug.LogError("Null TextInfo provided to TextGenerator. Cannot update its content.");
				}
				else
				{
					TextGenerator textGenerator = TextGenerator.GetTextGenerator();
					textGenerator.Prepare(settings, textInfo);
					FontAsset.UpdateFontAssetsInUpdateQueue();
					textGenerator.GenerateTextMesh(settings, textInfo);
				}
			}
		}

		public static Vector2 GetCursorPosition(TextGenerationSettings settings, int index)
		{
			bool flag = settings.fontAsset == null || settings.fontAsset.characterLookupTable == null;
			Vector2 vector;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
				vector = Vector2.zero;
			}
			else
			{
				TextInfo textInfo = new TextInfo();
				TextGenerator.GenerateText(settings, textInfo);
				vector = TextGenerator.GetCursorPosition(textInfo, settings.screenRect, index, true);
			}
			return vector;
		}

		public static Vector2 GetCursorPosition(TextInfo textInfo, Rect screenRect, int index, bool inverseYAxis = true)
		{
			Vector2 vector = screenRect.position;
			bool flag = textInfo.characterCount == 0;
			Vector2 vector2;
			if (flag)
			{
				vector2 = vector;
			}
			else
			{
				TextElementInfo textElementInfo = textInfo.textElementInfo[textInfo.characterCount - 1];
				LineInfo lineInfo = textInfo.lineInfo[textElementInfo.lineNumber];
				float num = lineInfo.lineHeight - (lineInfo.ascender - lineInfo.descender);
				bool flag2 = index >= textInfo.characterCount;
				if (flag2)
				{
					vector += (inverseYAxis ? new Vector2(textElementInfo.xAdvance, screenRect.height - lineInfo.ascender - num) : new Vector2(textElementInfo.xAdvance, lineInfo.descender));
					vector2 = vector;
				}
				else
				{
					textElementInfo = textInfo.textElementInfo[index];
					lineInfo = textInfo.lineInfo[textElementInfo.lineNumber];
					num = lineInfo.lineHeight - (lineInfo.ascender - lineInfo.descender);
					vector += (inverseYAxis ? new Vector2(textElementInfo.origin, screenRect.height - lineInfo.ascender - num) : new Vector2(textElementInfo.origin, lineInfo.descender));
					vector2 = vector;
				}
			}
			return vector2;
		}

		public static float GetPreferredWidth(TextGenerationSettings settings, TextInfo textInfo)
		{
			bool flag = settings.fontAsset == null || settings.fontAsset.characterLookupTable == null;
			float num;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
				num = 0f;
			}
			else
			{
				TextGenerator textGenerator = TextGenerator.GetTextGenerator();
				textGenerator.Prepare(settings, textInfo);
				num = textGenerator.GetPreferredWidthInternal(settings, textInfo);
			}
			return num;
		}

		public static float GetPreferredHeight(TextGenerationSettings settings, TextInfo textInfo)
		{
			bool flag = settings.fontAsset == null || settings.fontAsset.characterLookupTable == null;
			float num;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
				num = 0f;
			}
			else
			{
				TextGenerator textGenerator = TextGenerator.GetTextGenerator();
				textGenerator.Prepare(settings, textInfo);
				num = textGenerator.GetPreferredHeightInternal(settings, textInfo);
			}
			return num;
		}

		public static Vector2 GetPreferredValues(TextGenerationSettings settings, TextInfo textInfo)
		{
			bool flag = settings.fontAsset == null || settings.fontAsset.characterLookupTable == null;
			Vector2 vector;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
				vector = Vector2.zero;
			}
			else
			{
				TextGenerator textGenerator = TextGenerator.GetTextGenerator();
				textGenerator.Prepare(settings, textInfo);
				vector = textGenerator.GetPreferredValuesInternal(settings, textInfo);
			}
			return vector;
		}

		private bool vertexBufferAutoSizeReduction
		{
			get
			{
				return this.m_VertexBufferAutoSizeReduction;
			}
			set
			{
				this.m_VertexBufferAutoSizeReduction = value;
			}
		}

		public static bool isTextTruncated
		{
			get
			{
				return TextGenerator.m_IsTextTruncated;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event TextGenerator.MissingCharacterEventCallback OnMissingCharacter;

		private void Prepare(TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			this.m_Padding = generationSettings.extraPadding;
			this.m_FontStyleInternal = generationSettings.fontStyle;
			this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
			this.GetSpecialCharacters(generationSettings);
			this.ComputeMarginSize(generationSettings.screenRect, generationSettings.margins);
			this.PopulateTextBackingArray(generationSettings.text);
			this.PopulateTextProcessingArray(generationSettings);
			this.SetArraySizes(this.m_TextProcessingArray, generationSettings, textInfo);
			bool autoSize = generationSettings.autoSize;
			if (autoSize)
			{
				this.m_FontSize = Mathf.Clamp(generationSettings.fontSize, generationSettings.fontSizeMin, generationSettings.fontSizeMax);
			}
			else
			{
				this.m_FontSize = generationSettings.fontSize;
			}
			this.m_MaxFontSize = generationSettings.fontSizeMax;
			this.m_MinFontSize = generationSettings.fontSizeMin;
			this.m_LineSpacingDelta = 0f;
			this.m_CharWidthAdjDelta = 0f;
		}

		private void GenerateTextMesh(TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			bool flag = generationSettings.fontAsset == null || generationSettings.fontAsset.characterLookupTable == null;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned.");
				this.m_IsAutoSizePointSizeSet = true;
			}
			else
			{
				bool flag2 = textInfo != null;
				if (flag2)
				{
					textInfo.Clear();
				}
				bool flag3 = this.m_TextProcessingArray == null || this.m_TextProcessingArray.Length == 0 || this.m_TextProcessingArray[0].unicode == 0U;
				if (flag3)
				{
					TextGenerator.ClearMesh(true, textInfo);
					this.m_PreferredWidth = 0f;
					this.m_PreferredHeight = 0f;
					this.m_IsAutoSizePointSizeSet = true;
				}
				else
				{
					this.m_CurrentFontAsset = generationSettings.fontAsset;
					this.m_CurrentMaterial = generationSettings.material;
					this.m_CurrentMaterialIndex = 0;
					this.m_MaterialReferenceStack.SetDefault(new MaterialReference(this.m_CurrentMaterialIndex, this.m_CurrentFontAsset, null, this.m_CurrentMaterial, this.m_Padding));
					this.m_CurrentSpriteAsset = generationSettings.spriteAsset;
					int totalCharacterCount = this.m_TotalCharacterCount;
					float num = this.m_FontSize / (float)generationSettings.fontAsset.m_FaceInfo.pointSize * generationSettings.fontAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
					float num2 = num;
					float num3 = this.m_FontSize * 0.01f * (generationSettings.isOrthographic ? 1f : 0.1f);
					this.m_FontScaleMultiplier = 1f;
					this.m_CurrentFontSize = this.m_FontSize;
					this.m_SizeStack.SetDefault(this.m_CurrentFontSize);
					uint num4 = 0U;
					this.m_FontStyleInternal = generationSettings.fontStyle;
					this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
					this.m_FontWeightStack.SetDefault(this.m_FontWeightInternal);
					this.m_FontStyleStack.Clear();
					this.m_LineJustification = generationSettings.textAlignment;
					this.m_LineJustificationStack.SetDefault(this.m_LineJustification);
					float num5 = 0f;
					this.m_BaselineOffset = 0f;
					this.m_BaselineOffsetStack.Clear();
					bool flag4 = false;
					Vector3 zero = Vector3.zero;
					Vector3 zero2 = Vector3.zero;
					bool flag5 = false;
					Vector3 zero3 = Vector3.zero;
					Vector3 zero4 = Vector3.zero;
					bool flag6 = false;
					Vector3 vector = Vector3.zero;
					Vector3 vector2 = Vector3.zero;
					this.m_FontColor32 = generationSettings.color;
					this.m_HtmlColor = this.m_FontColor32;
					this.m_UnderlineColor = this.m_HtmlColor;
					this.m_StrikethroughColor = this.m_HtmlColor;
					this.m_ColorStack.SetDefault(this.m_HtmlColor);
					this.m_UnderlineColorStack.SetDefault(this.m_HtmlColor);
					this.m_StrikethroughColorStack.SetDefault(this.m_HtmlColor);
					this.m_HighlightStateStack.SetDefault(new HighlightState(this.m_HtmlColor, Offset.zero));
					this.m_ColorGradientPreset = null;
					this.m_ColorGradientStack.SetDefault(null);
					this.m_ItalicAngle = (int)this.m_CurrentFontAsset.italicStyleSlant;
					this.m_ItalicAngleStack.SetDefault(this.m_ItalicAngle);
					this.m_ActionStack.Clear();
					this.m_FXScale = Vector3.one;
					this.m_FXRotation = Quaternion.identity;
					this.m_LineOffset = 0f;
					this.m_LineHeight = -32767f;
					float num6 = this.m_CurrentFontAsset.faceInfo.lineHeight - (this.m_CurrentFontAsset.m_FaceInfo.ascentLine - this.m_CurrentFontAsset.m_FaceInfo.descentLine);
					this.m_CSpacing = 0f;
					this.m_MonoSpacing = 0f;
					this.m_XAdvance = 0f;
					this.m_TagLineIndent = 0f;
					this.m_TagIndent = 0f;
					this.m_IndentStack.SetDefault(0f);
					this.m_TagNoParsing = false;
					this.m_CharacterCount = 0;
					this.m_FirstCharacterOfLine = 0;
					this.m_LastCharacterOfLine = 0;
					this.m_FirstVisibleCharacterOfLine = 0;
					this.m_LastVisibleCharacterOfLine = 0;
					this.m_MaxLineAscender = -32767f;
					this.m_MaxLineDescender = 32767f;
					this.m_LineNumber = 0;
					this.m_StartOfLineAscender = 0f;
					this.m_LineVisibleCharacterCount = 0;
					this.m_LineVisibleSpaceCount = 0;
					bool flag7 = true;
					this.m_IsDrivenLineSpacing = false;
					this.m_FirstOverflowCharacterIndex = -1;
					this.m_LastBaseGlyphIndex = int.MinValue;
					this.m_PageNumber = 0;
					int num7 = Mathf.Clamp(generationSettings.pageToDisplay - 1, 0, textInfo.pageInfo.Length - 1);
					textInfo.ClearPageInfo();
					Vector4 margins = generationSettings.margins;
					float num8 = ((this.m_MarginWidth > 0f) ? this.m_MarginWidth : 0f);
					float num9 = ((this.m_MarginHeight > 0f) ? this.m_MarginHeight : 0f);
					this.m_MarginLeft = 0f;
					this.m_MarginRight = 0f;
					this.m_Width = -1f;
					float num10 = num8 + 0.0001f - this.m_MarginLeft - this.m_MarginRight;
					this.m_MeshExtents.min = TextGeneratorUtilities.largePositiveVector2;
					this.m_MeshExtents.max = TextGeneratorUtilities.largeNegativeVector2;
					textInfo.ClearLineInfo();
					this.m_MaxCapHeight = 0f;
					this.m_MaxAscender = 0f;
					this.m_MaxDescender = 0f;
					this.m_PageAscender = 0f;
					float num11 = 0f;
					bool flag8 = false;
					this.m_IsNewPage = false;
					bool flag9 = true;
					this.m_IsNonBreakingSpace = false;
					bool flag10 = false;
					int num12 = 0;
					CharacterSubstitution characterSubstitution = new CharacterSubstitution(-1, 0U);
					bool flag11 = false;
					TextWrappingMode textWrappingMode = (generationSettings.wordWrap ? TextWrappingMode.Normal : TextWrappingMode.NoWrap);
					this.SaveWordWrappingState(ref this.m_SavedWordWrapState, -1, -1, textInfo);
					this.SaveWordWrappingState(ref this.m_SavedLineState, -1, -1, textInfo);
					this.SaveWordWrappingState(ref this.m_SavedEllipsisState, -1, -1, textInfo);
					this.SaveWordWrappingState(ref this.m_SavedLastValidState, -1, -1, textInfo);
					this.SaveWordWrappingState(ref this.m_SavedSoftLineBreakState, -1, -1, textInfo);
					this.m_EllipsisInsertionCandidateStack.Clear();
					TextGenerator.m_IsTextTruncated = false;
					TextSettings textSettings = generationSettings.textSettings;
					int num13 = 0;
					int num14 = 0;
					while (num14 < this.m_TextProcessingArray.Length && this.m_TextProcessingArray[num14].unicode > 0U)
					{
						num4 = this.m_TextProcessingArray[num14].unicode;
						bool flag12 = num13 > 5;
						if (flag12)
						{
							Debug.LogError("Line breaking recursion max threshold hit... Character [" + num4.ToString() + "] index: " + num14.ToString());
							characterSubstitution.index = this.m_CharacterCount;
							characterSubstitution.unicode = 3U;
						}
						bool flag13 = num4 == 26U;
						int num38;
						if (!flag13)
						{
							bool flag14 = generationSettings.richText && num4 == 60U;
							if (flag14)
							{
								this.m_isTextLayoutPhase = true;
								this.m_TextElementType = TextElementType.Character;
								int num15;
								bool flag15 = this.ValidateHtmlTag(this.m_TextProcessingArray, num14 + 1, out num15, generationSettings, textInfo);
								if (flag15)
								{
									num14 = num15;
									bool flag16 = this.m_TextElementType == TextElementType.Character;
									if (flag16)
									{
										goto IL_43D7;
									}
								}
							}
							else
							{
								this.m_TextElementType = textInfo.textElementInfo[this.m_CharacterCount].elementType;
								this.m_CurrentMaterialIndex = textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex;
								this.m_CurrentFontAsset = textInfo.textElementInfo[this.m_CharacterCount].fontAsset;
							}
							int currentMaterialIndex = this.m_CurrentMaterialIndex;
							bool isUsingAlternateTypeface = textInfo.textElementInfo[this.m_CharacterCount].isUsingAlternateTypeface;
							this.m_isTextLayoutPhase = false;
							bool flag17 = false;
							bool flag18 = characterSubstitution.index == this.m_CharacterCount;
							if (flag18)
							{
								num4 = characterSubstitution.unicode;
								this.m_TextElementType = TextElementType.Character;
								flag17 = true;
								uint num16 = num4;
								uint num17 = num16;
								if (num17 != 3U)
								{
									if (num17 != 45U)
									{
										if (num17 == 8230U)
										{
											textInfo.textElementInfo[this.m_CharacterCount].textElement = this.m_Ellipsis.character;
											textInfo.textElementInfo[this.m_CharacterCount].elementType = TextElementType.Character;
											textInfo.textElementInfo[this.m_CharacterCount].fontAsset = this.m_Ellipsis.fontAsset;
											textInfo.textElementInfo[this.m_CharacterCount].material = this.m_Ellipsis.material;
											textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex = this.m_Ellipsis.materialIndex;
											TextGenerator.m_IsTextTruncated = true;
											characterSubstitution.index = this.m_CharacterCount + 1;
											characterSubstitution.unicode = 3U;
										}
									}
								}
								else
								{
									textInfo.textElementInfo[this.m_CharacterCount].textElement = this.m_CurrentFontAsset.characterLookupTable[3U];
									TextGenerator.m_IsTextTruncated = true;
								}
							}
							bool flag19 = this.m_CharacterCount < generationSettings.firstVisibleCharacter && num4 != 3U;
							if (flag19)
							{
								textInfo.textElementInfo[this.m_CharacterCount].isVisible = false;
								textInfo.textElementInfo[this.m_CharacterCount].character = '\u200b';
								textInfo.textElementInfo[this.m_CharacterCount].lineNumber = 0;
								this.m_CharacterCount++;
							}
							else
							{
								float num18 = 1f;
								bool flag20 = this.m_TextElementType == TextElementType.Character;
								if (flag20)
								{
									bool flag21 = (this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase;
									if (flag21)
									{
										bool flag22 = char.IsLower((char)num4);
										if (flag22)
										{
											num4 = (uint)char.ToUpper((char)num4);
										}
									}
									else
									{
										bool flag23 = (this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase;
										if (flag23)
										{
											bool flag24 = char.IsUpper((char)num4);
											if (flag24)
											{
												num4 = (uint)char.ToLower((char)num4);
											}
										}
										else
										{
											bool flag25 = (this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps;
											if (flag25)
											{
												bool flag26 = char.IsLower((char)num4);
												if (flag26)
												{
													num18 = 0.8f;
													num4 = (uint)char.ToUpper((char)num4);
												}
											}
										}
									}
								}
								float num19 = 0f;
								float num20 = 0f;
								float num21 = 0f;
								bool flag27 = this.m_TextElementType == TextElementType.Sprite;
								if (flag27)
								{
									SpriteCharacter spriteCharacter = (SpriteCharacter)textInfo.textElementInfo[this.m_CharacterCount].textElement;
									this.m_CurrentSpriteAsset = spriteCharacter.textAsset as SpriteAsset;
									this.m_SpriteIndex = (int)spriteCharacter.glyphIndex;
									bool flag28 = spriteCharacter == null;
									if (flag28)
									{
										goto IL_43D7;
									}
									bool flag29 = num4 == 60U;
									if (flag29)
									{
										num4 = (uint)(57344 + this.m_SpriteIndex);
									}
									else
									{
										this.m_SpriteColor = Color.white;
									}
									float num22 = this.m_CurrentFontSize / (float)this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
									bool flag30 = this.m_CurrentSpriteAsset.m_FaceInfo.pointSize > 0;
									if (flag30)
									{
										float num23 = this.m_CurrentFontSize / (float)this.m_CurrentSpriteAsset.m_FaceInfo.pointSize * this.m_CurrentSpriteAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										num2 = spriteCharacter.m_Scale * spriteCharacter.m_Glyph.scale * num23;
										num20 = this.m_CurrentSpriteAsset.m_FaceInfo.ascentLine;
										num19 = this.m_CurrentSpriteAsset.m_FaceInfo.baseline * num22 * this.m_FontScaleMultiplier * this.m_CurrentSpriteAsset.m_FaceInfo.scale;
										num21 = this.m_CurrentSpriteAsset.m_FaceInfo.descentLine;
									}
									else
									{
										float num24 = this.m_CurrentFontSize / (float)this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										num2 = this.m_CurrentFontAsset.m_FaceInfo.ascentLine / spriteCharacter.m_Glyph.metrics.height * spriteCharacter.m_Scale * spriteCharacter.m_Glyph.scale * num24;
										float num25 = num24 / num2;
										num20 = this.m_CurrentFontAsset.m_FaceInfo.ascentLine * num25;
										num19 = this.m_CurrentFontAsset.m_FaceInfo.baseline * num22 * this.m_FontScaleMultiplier * this.m_CurrentFontAsset.m_FaceInfo.scale;
										num21 = this.m_CurrentFontAsset.m_FaceInfo.descentLine * num25;
									}
									this.m_CachedTextElement = spriteCharacter;
									textInfo.textElementInfo[this.m_CharacterCount].elementType = TextElementType.Sprite;
									textInfo.textElementInfo[this.m_CharacterCount].scale = num2;
									textInfo.textElementInfo[this.m_CharacterCount].spriteAsset = this.m_CurrentSpriteAsset;
									textInfo.textElementInfo[this.m_CharacterCount].fontAsset = this.m_CurrentFontAsset;
									textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex = this.m_CurrentMaterialIndex;
									this.m_CurrentMaterialIndex = currentMaterialIndex;
									num5 = 0f;
								}
								else
								{
									bool flag31 = this.m_TextElementType == TextElementType.Character;
									if (flag31)
									{
										this.m_CachedTextElement = textInfo.textElementInfo[this.m_CharacterCount].textElement;
										bool flag32 = this.m_CachedTextElement == null;
										if (flag32)
										{
											goto IL_43D7;
										}
										this.m_CurrentFontAsset = textInfo.textElementInfo[this.m_CharacterCount].fontAsset;
										this.m_CurrentMaterial = textInfo.textElementInfo[this.m_CharacterCount].material;
										this.m_CurrentMaterialIndex = textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex;
										bool flag33 = flag17 && this.m_TextProcessingArray[num14].unicode == 10U && this.m_CharacterCount != this.m_FirstCharacterOfLine;
										float num26;
										if (flag33)
										{
											num26 = textInfo.textElementInfo[this.m_CharacterCount - 1].pointSize * num18 / (float)this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										}
										else
										{
											num26 = this.m_CurrentFontSize * num18 / (float)this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										}
										bool flag34 = flag17 && num4 == 8230U;
										if (flag34)
										{
											num20 = 0f;
											num21 = 0f;
										}
										else
										{
											num20 = this.m_CurrentFontAsset.m_FaceInfo.ascentLine;
											num21 = this.m_CurrentFontAsset.m_FaceInfo.descentLine;
										}
										num2 = num26 * this.m_FontScaleMultiplier * this.m_CachedTextElement.m_Scale * this.m_CachedTextElement.m_Glyph.scale;
										num19 = this.m_CurrentFontAsset.m_FaceInfo.baseline * num26 * this.m_FontScaleMultiplier * this.m_CurrentFontAsset.m_FaceInfo.scale;
										textInfo.textElementInfo[this.m_CharacterCount].elementType = TextElementType.Character;
										textInfo.textElementInfo[this.m_CharacterCount].scale = num2;
										num5 = this.m_Padding;
									}
								}
								float num27 = num2;
								bool flag35 = num4 == 173U || num4 == 3U;
								if (flag35)
								{
									num2 = 0f;
								}
								textInfo.textElementInfo[this.m_CharacterCount].character = (char)num4;
								textInfo.textElementInfo[this.m_CharacterCount].pointSize = this.m_CurrentFontSize;
								textInfo.textElementInfo[this.m_CharacterCount].color = this.m_HtmlColor;
								textInfo.textElementInfo[this.m_CharacterCount].underlineColor = this.m_UnderlineColor;
								textInfo.textElementInfo[this.m_CharacterCount].strikethroughColor = this.m_StrikethroughColor;
								textInfo.textElementInfo[this.m_CharacterCount].highlightState = this.m_HighlightState;
								textInfo.textElementInfo[this.m_CharacterCount].style = this.m_FontStyleInternal;
								Glyph alternativeGlyph = textInfo.textElementInfo[this.m_CharacterCount].alternativeGlyph;
								GlyphMetrics glyphMetrics = ((alternativeGlyph == null) ? this.m_CachedTextElement.m_Glyph.metrics : alternativeGlyph.metrics);
								bool flag36 = num4 <= 65535U && char.IsWhiteSpace((char)num4);
								GlyphValueRecord glyphValueRecord = default(GlyphValueRecord);
								float num28 = generationSettings.characterSpacing;
								bool enableKerning = generationSettings.enableKerning;
								if (enableKerning)
								{
									uint glyphIndex = this.m_CachedTextElement.m_GlyphIndex;
									bool flag37 = this.m_CharacterCount < totalCharacterCount - 1;
									if (flag37)
									{
										uint glyphIndex2 = textInfo.textElementInfo[this.m_CharacterCount + 1].textElement.m_GlyphIndex;
										uint num29 = (glyphIndex2 << 16) | glyphIndex;
										GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
										bool flag38 = this.m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num29, out glyphPairAdjustmentRecord);
										if (flag38)
										{
											glyphValueRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphValueRecord;
											num28 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num28);
										}
									}
									bool flag39 = this.m_CharacterCount >= 1;
									if (flag39)
									{
										uint glyphIndex3 = textInfo.textElementInfo[this.m_CharacterCount - 1].textElement.m_GlyphIndex;
										uint num30 = (glyphIndex << 16) | glyphIndex3;
										GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
										bool flag40 = this.m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num30, out glyphPairAdjustmentRecord);
										if (flag40)
										{
											glyphValueRecord += glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphValueRecord;
											num28 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num28);
										}
									}
								}
								textInfo.textElementInfo[this.m_CharacterCount].adjustedHorizontalAdvance = glyphValueRecord.xAdvance;
								bool flag41 = TextGeneratorUtilities.IsBaseGlyph(num4);
								bool flag42 = flag41;
								if (flag42)
								{
									this.m_LastBaseGlyphIndex = this.m_CharacterCount;
								}
								bool flag43 = this.m_CharacterCount > 0 && !flag41;
								if (flag43)
								{
									bool flag44 = this.m_LastBaseGlyphIndex != int.MinValue && this.m_LastBaseGlyphIndex == this.m_CharacterCount - 1;
									if (flag44)
									{
										Glyph glyph = textInfo.textElementInfo[this.m_LastBaseGlyphIndex].textElement.glyph;
										uint index = glyph.index;
										uint glyphIndex4 = this.m_CachedTextElement.glyphIndex;
										uint num31 = (glyphIndex4 << 16) | index;
										MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord;
										bool flag45 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num31, out markToBaseAdjustmentRecord);
										if (flag45)
										{
											float num32 = (textInfo.textElementInfo[this.m_LastBaseGlyphIndex].origin - this.m_XAdvance) / num2;
											glyphValueRecord.xPlacement = num32 + markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment;
											glyphValueRecord.yPlacement = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment;
											num28 = 0f;
										}
									}
									else
									{
										bool flag46 = false;
										int num33 = this.m_CharacterCount - 1;
										while (num33 >= 0 && num33 != this.m_LastBaseGlyphIndex)
										{
											Glyph glyph2 = textInfo.textElementInfo[num33].textElement.glyph;
											uint index2 = glyph2.index;
											uint glyphIndex5 = this.m_CachedTextElement.glyphIndex;
											uint num34 = (glyphIndex5 << 16) | index2;
											MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord;
											bool flag47 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryGetValue(num34, out markToMarkAdjustmentRecord);
											if (flag47)
											{
												float num35 = (textInfo.textElementInfo[num33].origin - this.m_XAdvance) / num2;
												float num36 = num19 - this.m_LineOffset + this.m_BaselineOffset;
												float num37 = (textInfo.textElementInfo[num33].baseLine - num36) / num2;
												glyphValueRecord.xPlacement = num35 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment;
												glyphValueRecord.yPlacement = num37 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment;
												num28 = 0f;
												flag46 = true;
												break;
											}
											num38 = num33;
											num33 = num38 - 1;
										}
										bool flag48 = this.m_LastBaseGlyphIndex != int.MinValue && !flag46;
										if (flag48)
										{
											Glyph glyph3 = textInfo.textElementInfo[this.m_LastBaseGlyphIndex].textElement.glyph;
											uint index3 = glyph3.index;
											uint glyphIndex6 = this.m_CachedTextElement.glyphIndex;
											uint num39 = (glyphIndex6 << 16) | index3;
											MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2;
											bool flag49 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num39, out markToBaseAdjustmentRecord2);
											if (flag49)
											{
												float num40 = (textInfo.textElementInfo[this.m_LastBaseGlyphIndex].origin - this.m_XAdvance) / num2;
												glyphValueRecord.xPlacement = num40 + markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.xPositionAdjustment;
												glyphValueRecord.yPlacement = markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.yPositionAdjustment;
												num28 = 0f;
											}
										}
									}
								}
								num20 += glyphValueRecord.yPlacement;
								num21 += glyphValueRecord.yPlacement;
								bool isRightToLeft = generationSettings.isRightToLeft;
								if (isRightToLeft)
								{
									this.m_XAdvance -= glyphMetrics.horizontalAdvance * (1f - this.m_CharWidthAdjDelta) * num2;
									bool flag50 = flag36 || num4 == 8203U;
									if (flag50)
									{
										this.m_XAdvance -= generationSettings.wordSpacing * num3;
									}
								}
								float num41 = 0f;
								bool flag51 = this.m_MonoSpacing != 0f;
								if (flag51)
								{
									num41 = (this.m_MonoSpacing / 2f - (glyphMetrics.width / 2f + glyphMetrics.horizontalBearingX) * num2) * (1f - this.m_CharWidthAdjDelta);
									this.m_XAdvance += num41;
								}
								bool flag52 = this.m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold;
								float num42;
								float num43;
								if (flag52)
								{
									bool flag53 = this.m_CurrentMaterial != null && this.m_CurrentMaterial.HasProperty(TextShaderUtilities.ID_GradientScale);
									if (flag53)
									{
										float @float = this.m_CurrentMaterial.GetFloat(TextShaderUtilities.ID_GradientScale);
										num42 = this.m_CurrentFontAsset.boldStyleWeight / 4f * @float * this.m_CurrentMaterial.GetFloat(TextShaderUtilities.ID_ScaleRatio_A);
										bool flag54 = num42 + num5 > @float;
										if (flag54)
										{
											num5 = @float - num42;
										}
									}
									else
									{
										num42 = 0f;
									}
									num43 = this.m_CurrentFontAsset.boldStyleSpacing;
								}
								else
								{
									bool flag55 = this.m_CurrentMaterial != null && this.m_CurrentMaterial.HasProperty(TextShaderUtilities.ID_GradientScale) && this.m_CurrentMaterial.HasProperty(TextShaderUtilities.ID_ScaleRatio_A);
									if (flag55)
									{
										float float2 = this.m_CurrentMaterial.GetFloat(TextShaderUtilities.ID_GradientScale);
										num42 = this.m_CurrentFontAsset.m_RegularStyleWeight / 4f * float2 * this.m_CurrentMaterial.GetFloat(TextShaderUtilities.ID_ScaleRatio_A);
										bool flag56 = num42 + num5 > float2;
										if (flag56)
										{
											num5 = float2 - num42;
										}
									}
									else
									{
										num42 = 0f;
									}
									num43 = 0f;
								}
								Vector3 vector3;
								vector3.x = this.m_XAdvance + (glyphMetrics.horizontalBearingX * this.m_FXScale.x - num5 - num42 + glyphValueRecord.xPlacement) * num2 * (1f - this.m_CharWidthAdjDelta);
								vector3.y = num19 + (glyphMetrics.horizontalBearingY + num5 + glyphValueRecord.yPlacement) * num2 - this.m_LineOffset + this.m_BaselineOffset;
								vector3.z = 0f;
								Vector3 vector4;
								vector4.x = vector3.x;
								vector4.y = vector3.y - (glyphMetrics.height + num5 * 2f) * num2;
								vector4.z = 0f;
								Vector3 vector5;
								vector5.x = vector4.x + (glyphMetrics.width * this.m_FXScale.x + num5 * 2f + num42 * 2f) * num2 * (1f - this.m_CharWidthAdjDelta);
								vector5.y = vector3.y;
								vector5.z = 0f;
								Vector3 vector6;
								vector6.x = vector5.x;
								vector6.y = vector4.y;
								vector6.z = 0f;
								bool flag57 = this.m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (this.m_FontStyleInternal & FontStyles.Italic) == FontStyles.Italic;
								if (flag57)
								{
									float num44 = (float)this.m_ItalicAngle * 0.01f;
									float num45 = (this.m_CurrentFontAsset.m_FaceInfo.capLine - (this.m_CurrentFontAsset.m_FaceInfo.baseline + this.m_BaselineOffset)) / 2f * this.m_FontScaleMultiplier * this.m_CurrentFontAsset.m_FaceInfo.scale;
									Vector3 vector7 = new Vector3(num44 * ((glyphMetrics.horizontalBearingY + num5 + num42 - num45) * num2), 0f, 0f);
									Vector3 vector8 = new Vector3(num44 * ((glyphMetrics.horizontalBearingY - glyphMetrics.height - num5 - num42 - num45) * num2), 0f, 0f);
									vector3 += vector7;
									vector4 += vector8;
									vector5 += vector7;
									vector6 += vector8;
								}
								bool flag58 = this.m_FXRotation != Quaternion.identity;
								if (flag58)
								{
									Matrix4x4 matrix4x = Matrix4x4.Rotate(this.m_FXRotation);
									Vector3 vector9 = (vector5 + vector4) / 2f;
									vector3 = matrix4x.MultiplyPoint3x4(vector3 - vector9) + vector9;
									vector4 = matrix4x.MultiplyPoint3x4(vector4 - vector9) + vector9;
									vector5 = matrix4x.MultiplyPoint3x4(vector5 - vector9) + vector9;
									vector6 = matrix4x.MultiplyPoint3x4(vector6 - vector9) + vector9;
								}
								textInfo.textElementInfo[this.m_CharacterCount].bottomLeft = vector4;
								textInfo.textElementInfo[this.m_CharacterCount].topLeft = vector3;
								textInfo.textElementInfo[this.m_CharacterCount].topRight = vector5;
								textInfo.textElementInfo[this.m_CharacterCount].bottomRight = vector6;
								textInfo.textElementInfo[this.m_CharacterCount].origin = this.m_XAdvance + glyphValueRecord.xPlacement * num2;
								textInfo.textElementInfo[this.m_CharacterCount].baseLine = num19 - this.m_LineOffset + this.m_BaselineOffset + glyphValueRecord.yPlacement * num2;
								textInfo.textElementInfo[this.m_CharacterCount].aspectRatio = (vector5.x - vector4.x) / (vector3.y - vector4.y);
								float num46 = ((this.m_TextElementType == TextElementType.Character) ? (num20 * num2 / num18 + this.m_BaselineOffset) : (num20 * num2 + this.m_BaselineOffset));
								float num47 = ((this.m_TextElementType == TextElementType.Character) ? (num21 * num2 / num18 + this.m_BaselineOffset) : (num21 * num2 + this.m_BaselineOffset));
								float num48 = num46;
								float num49 = num47;
								bool flag59 = this.m_CharacterCount == this.m_FirstCharacterOfLine;
								bool flag60 = flag59 || !flag36;
								if (flag60)
								{
									bool flag61 = this.m_BaselineOffset != 0f;
									if (flag61)
									{
										num48 = Mathf.Max((num46 - this.m_BaselineOffset) / this.m_FontScaleMultiplier, num48);
										num49 = Mathf.Min((num47 - this.m_BaselineOffset) / this.m_FontScaleMultiplier, num49);
									}
									this.m_MaxLineAscender = Mathf.Max(num48, this.m_MaxLineAscender);
									this.m_MaxLineDescender = Mathf.Min(num49, this.m_MaxLineDescender);
								}
								bool flag62 = flag59 || !flag36;
								if (flag62)
								{
									textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender = num48;
									textInfo.textElementInfo[this.m_CharacterCount].adjustedDescender = num49;
									textInfo.textElementInfo[this.m_CharacterCount].ascender = num46 - this.m_LineOffset;
									this.m_MaxDescender = (textInfo.textElementInfo[this.m_CharacterCount].descender = num47 - this.m_LineOffset);
								}
								else
								{
									textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender = this.m_MaxLineAscender;
									textInfo.textElementInfo[this.m_CharacterCount].adjustedDescender = this.m_MaxLineDescender;
									textInfo.textElementInfo[this.m_CharacterCount].ascender = this.m_MaxLineAscender - this.m_LineOffset;
									this.m_MaxDescender = (textInfo.textElementInfo[this.m_CharacterCount].descender = this.m_MaxLineDescender - this.m_LineOffset);
								}
								bool flag63 = this.m_LineNumber == 0 || this.m_IsNewPage;
								if (flag63)
								{
									bool flag64 = flag59 || !flag36;
									if (flag64)
									{
										this.m_MaxAscender = this.m_MaxLineAscender;
										this.m_MaxCapHeight = Mathf.Max(this.m_MaxCapHeight, this.m_CurrentFontAsset.m_FaceInfo.capLine * num2 / num18);
									}
								}
								bool flag65 = this.m_LineOffset == 0f;
								if (flag65)
								{
									bool flag66 = flag59 || !flag36;
									if (flag66)
									{
										this.m_PageAscender = ((this.m_PageAscender > num46) ? this.m_PageAscender : num46);
									}
								}
								textInfo.textElementInfo[this.m_CharacterCount].isVisible = false;
								bool flag67 = (this.m_LineJustification & (TextAlignment)16) == (TextAlignment)16 || (this.m_LineJustification & (TextAlignment)8) == (TextAlignment)8;
								bool flag68 = num4 == 9U || ((textWrappingMode == TextWrappingMode.PreserveWhitespace || textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) && (flag36 || num4 == 8203U)) || (!flag36 && num4 != 8203U && num4 != 173U && num4 != 3U) || (num4 == 173U && !flag11) || this.m_TextElementType == TextElementType.Sprite;
								if (flag68)
								{
									textInfo.textElementInfo[this.m_CharacterCount].isVisible = true;
									float num50 = this.m_MarginLeft;
									float num51 = this.m_MarginRight;
									bool flag69 = flag17;
									if (flag69)
									{
										num50 = textInfo.lineInfo[this.m_LineNumber].marginLeft;
										num51 = textInfo.lineInfo[this.m_LineNumber].marginRight;
									}
									num10 = ((this.m_Width != -1f) ? Mathf.Min(num8 + 0.0001f - num50 - num51, this.m_Width) : (num8 + 0.0001f - num50 - num51));
									float num52 = Mathf.Abs(this.m_XAdvance) + ((!generationSettings.isRightToLeft) ? glyphMetrics.horizontalAdvance : 0f) * (1f - this.m_CharWidthAdjDelta) * ((num4 == 173U) ? num27 : num2);
									float num53 = this.m_MaxAscender - (this.m_MaxLineDescender - this.m_LineOffset) + ((this.m_LineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_MaxLineAscender - this.m_StartOfLineAscender) : 0f);
									int characterCount = this.m_CharacterCount;
									bool flag70 = num53 > num9 + 0.0001f;
									if (flag70)
									{
										bool flag71 = this.m_FirstOverflowCharacterIndex == -1;
										if (flag71)
										{
											this.m_FirstOverflowCharacterIndex = this.m_CharacterCount;
										}
										bool autoSize = generationSettings.autoSize;
										if (autoSize)
										{
											bool flag72 = this.m_LineSpacingDelta > generationSettings.lineSpacingMax && this.m_LineOffset > 0f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
											if (flag72)
											{
												float num54 = (num9 - num53) / (float)this.m_LineNumber;
												this.m_LineSpacingDelta = Mathf.Max(this.m_LineSpacingDelta + num54 / num, generationSettings.lineSpacingMax);
												return;
											}
											bool flag73 = this.m_FontSize > generationSettings.fontSizeMin && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
											if (flag73)
											{
												this.m_MaxFontSize = this.m_FontSize;
												float num55 = Mathf.Max((this.m_FontSize - this.m_MinFontSize) / 2f, 0.05f);
												this.m_FontSize -= num55;
												this.m_FontSize = Mathf.Max((float)((int)(this.m_FontSize * 20f + 0.5f)) / 20f, generationSettings.fontSizeMin);
												return;
											}
										}
										switch (generationSettings.overflowMode)
										{
										case TextOverflowMode.Ellipsis:
										{
											bool flag74 = this.m_LineNumber > 0;
											if (flag74)
											{
												bool flag75 = this.m_EllipsisInsertionCandidateStack.Count == 0;
												if (flag75)
												{
													num14 = -1;
													this.m_CharacterCount = 0;
													characterSubstitution.index = 0;
													characterSubstitution.unicode = 3U;
													this.m_FirstCharacterOfLine = 0;
													goto IL_43D7;
												}
												WordWrapState wordWrapState = this.m_EllipsisInsertionCandidateStack.Pop();
												num14 = this.RestoreWordWrappingState(ref wordWrapState, textInfo);
												num14--;
												this.m_CharacterCount--;
												characterSubstitution.index = this.m_CharacterCount;
												characterSubstitution.unicode = 8230U;
												num13++;
												goto IL_43D7;
											}
											break;
										}
										case TextOverflowMode.Truncate:
											num14 = this.RestoreWordWrappingState(ref this.m_SavedLastValidState, textInfo);
											characterSubstitution.index = characterCount;
											goto IL_43D7;
										case TextOverflowMode.Page:
										{
											bool flag76 = num14 < 0 || characterCount == 0;
											if (flag76)
											{
												num14 = -1;
												this.m_CharacterCount = 0;
												characterSubstitution.index = 0;
												characterSubstitution.unicode = 3U;
												goto IL_43D7;
											}
											bool flag77 = this.m_MaxLineAscender - this.m_MaxLineDescender > num9 + 0.0001f;
											if (flag77)
											{
												num14 = this.RestoreWordWrappingState(ref this.m_SavedLineState, textInfo);
												characterSubstitution.index = characterCount;
												characterSubstitution.unicode = 3U;
												goto IL_43D7;
											}
											num14 = this.RestoreWordWrappingState(ref this.m_SavedLineState, textInfo);
											this.m_IsNewPage = true;
											this.m_FirstCharacterOfLine = this.m_CharacterCount;
											this.m_MaxLineAscender = -32767f;
											this.m_MaxLineDescender = 32767f;
											this.m_StartOfLineAscender = 0f;
											this.m_XAdvance = 0f + this.m_TagIndent;
											this.m_LineOffset = 0f;
											this.m_MaxAscender = 0f;
											this.m_PageAscender = 0f;
											this.m_LineNumber++;
											this.m_PageNumber++;
											goto IL_43D7;
										}
										case TextOverflowMode.Linked:
											num14 = this.RestoreWordWrappingState(ref this.m_SavedLastValidState, textInfo);
											characterSubstitution.index = characterCount;
											characterSubstitution.unicode = 3U;
											goto IL_43D7;
										}
									}
									bool flag78 = flag41 && num52 > num10 * (flag67 ? 1.05f : 1f);
									if (flag78)
									{
										bool flag79 = textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap && this.m_CharacterCount != this.m_FirstCharacterOfLine;
										if (flag79)
										{
											num14 = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState, textInfo);
											bool flag80 = this.m_LineHeight == -32767f;
											float num56;
											if (flag80)
											{
												float adjustedAscender = textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender;
												num56 = ((this.m_LineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_MaxLineAscender - this.m_StartOfLineAscender) : 0f) - this.m_MaxLineDescender + adjustedAscender + (num6 + this.m_LineSpacingDelta) * num + generationSettings.lineSpacing * num3;
											}
											else
											{
												num56 = this.m_LineHeight + generationSettings.lineSpacing * num3;
												this.m_IsDrivenLineSpacing = true;
											}
											float num57 = this.m_MaxAscender + num56 + this.m_LineOffset - textInfo.textElementInfo[this.m_CharacterCount].adjustedDescender;
											bool flag81 = textInfo.textElementInfo[this.m_CharacterCount - 1].character == '\u00ad' && !flag11;
											if (flag81)
											{
												bool flag82 = generationSettings.overflowMode == TextOverflowMode.Overflow || num57 < num9 + 0.0001f;
												if (flag82)
												{
													characterSubstitution.index = this.m_CharacterCount - 1;
													characterSubstitution.unicode = 45U;
													num14--;
													this.m_CharacterCount--;
													goto IL_43D7;
												}
											}
											flag11 = false;
											bool flag83 = textInfo.textElementInfo[this.m_CharacterCount].character == '\u00ad';
											if (flag83)
											{
												flag11 = true;
												goto IL_43D7;
											}
											bool flag84 = generationSettings.autoSize && flag9;
											if (flag84)
											{
												bool flag85 = this.m_CharWidthAdjDelta < generationSettings.charWidthMaxAdj / 100f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag85)
												{
													float num58 = num52;
													bool flag86 = this.m_CharWidthAdjDelta > 0f;
													if (flag86)
													{
														num58 /= 1f - this.m_CharWidthAdjDelta;
													}
													float num59 = num52 - (num10 - 0.0001f) * (flag67 ? 1.05f : 1f);
													this.m_CharWidthAdjDelta += num59 / num58;
													this.m_CharWidthAdjDelta = Mathf.Min(this.m_CharWidthAdjDelta, generationSettings.charWidthMaxAdj / 100f);
													return;
												}
												bool flag87 = this.m_FontSize > generationSettings.fontSizeMin && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag87)
												{
													this.m_MaxFontSize = this.m_FontSize;
													float num60 = Mathf.Max((this.m_FontSize - this.m_MinFontSize) / 2f, 0.05f);
													this.m_FontSize -= num60;
													this.m_FontSize = Mathf.Max((float)((int)(this.m_FontSize * 20f + 0.5f)) / 20f, generationSettings.fontSizeMin);
													return;
												}
											}
											int previousWordBreak = this.m_SavedSoftLineBreakState.previousWordBreak;
											bool flag88 = flag9 && previousWordBreak != -1;
											if (flag88)
											{
												bool flag89 = previousWordBreak != num12;
												if (flag89)
												{
													num14 = this.RestoreWordWrappingState(ref this.m_SavedSoftLineBreakState, textInfo);
													num12 = previousWordBreak;
													bool flag90 = textInfo.textElementInfo[this.m_CharacterCount - 1].character == '\u00ad';
													if (flag90)
													{
														characterSubstitution.index = this.m_CharacterCount - 1;
														characterSubstitution.unicode = 45U;
														num14--;
														this.m_CharacterCount--;
														goto IL_43D7;
													}
												}
											}
											bool flag91 = num57 > num9 + 0.0001f;
											if (!flag91)
											{
												this.InsertNewLine(num14, num, num2, num3, num43, num28, num10, num6, ref flag8, ref num11, generationSettings, textInfo);
												flag7 = true;
												flag9 = true;
												goto IL_43D7;
											}
											bool flag92 = this.m_FirstOverflowCharacterIndex == -1;
											if (flag92)
											{
												this.m_FirstOverflowCharacterIndex = this.m_CharacterCount;
											}
											bool autoSize2 = generationSettings.autoSize;
											if (autoSize2)
											{
												bool flag93 = this.m_LineSpacingDelta > generationSettings.lineSpacingMax && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag93)
												{
													float num61 = (num9 - num57) / (float)(this.m_LineNumber + 1);
													this.m_LineSpacingDelta = Mathf.Max(this.m_LineSpacingDelta + num61 / num, generationSettings.lineSpacingMax);
													return;
												}
												bool flag94 = this.m_CharWidthAdjDelta < generationSettings.charWidthMaxAdj / 100f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag94)
												{
													float num62 = num52;
													bool flag95 = this.m_CharWidthAdjDelta > 0f;
													if (flag95)
													{
														num62 /= 1f - this.m_CharWidthAdjDelta;
													}
													float num63 = num52 - (num10 - 0.0001f) * (flag67 ? 1.05f : 1f);
													this.m_CharWidthAdjDelta += num63 / num62;
													this.m_CharWidthAdjDelta = Mathf.Min(this.m_CharWidthAdjDelta, generationSettings.charWidthMaxAdj / 100f);
													return;
												}
												bool flag96 = this.m_FontSize > generationSettings.fontSizeMin && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag96)
												{
													this.m_MaxFontSize = this.m_FontSize;
													float num64 = Mathf.Max((this.m_FontSize - this.m_MinFontSize) / 2f, 0.05f);
													this.m_FontSize -= num64;
													this.m_FontSize = Mathf.Max((float)((int)(this.m_FontSize * 20f + 0.5f)) / 20f, generationSettings.fontSizeMin);
													return;
												}
											}
											switch (generationSettings.overflowMode)
											{
											case TextOverflowMode.Overflow:
											case TextOverflowMode.Masking:
											case TextOverflowMode.ScrollRect:
												this.InsertNewLine(num14, num, num2, num3, num43, num28, num10, num6, ref flag8, ref num11, generationSettings, textInfo);
												flag7 = true;
												flag9 = true;
												goto IL_43D7;
											case TextOverflowMode.Ellipsis:
											{
												bool flag97 = this.m_EllipsisInsertionCandidateStack.Count == 0;
												if (flag97)
												{
													num14 = -1;
													this.m_CharacterCount = 0;
													characterSubstitution.index = 0;
													characterSubstitution.unicode = 3U;
													this.m_FirstCharacterOfLine = 0;
													goto IL_43D7;
												}
												WordWrapState wordWrapState2 = this.m_EllipsisInsertionCandidateStack.Pop();
												num14 = this.RestoreWordWrappingState(ref wordWrapState2, textInfo);
												num14--;
												this.m_CharacterCount--;
												characterSubstitution.index = this.m_CharacterCount;
												characterSubstitution.unicode = 8230U;
												num13++;
												goto IL_43D7;
											}
											case TextOverflowMode.Truncate:
												num14 = this.RestoreWordWrappingState(ref this.m_SavedLastValidState, textInfo);
												characterSubstitution.index = characterCount;
												characterSubstitution.unicode = 3U;
												goto IL_43D7;
											case TextOverflowMode.Page:
												this.m_IsNewPage = true;
												this.InsertNewLine(num14, num, num2, num3, num43, num28, num10, num6, ref flag8, ref num11, generationSettings, textInfo);
												this.m_StartOfLineAscender = 0f;
												this.m_LineOffset = 0f;
												this.m_MaxAscender = 0f;
												this.m_PageAscender = 0f;
												this.m_PageNumber++;
												flag7 = true;
												flag9 = true;
												goto IL_43D7;
											case TextOverflowMode.Linked:
												characterSubstitution.index = this.m_CharacterCount;
												characterSubstitution.unicode = 3U;
												goto IL_43D7;
											}
										}
										else
										{
											bool flag98 = generationSettings.autoSize && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
											if (flag98)
											{
												bool flag99 = this.m_CharWidthAdjDelta < generationSettings.charWidthMaxAdj / 100f;
												if (flag99)
												{
													float num65 = num52;
													bool flag100 = this.m_CharWidthAdjDelta > 0f;
													if (flag100)
													{
														num65 /= 1f - this.m_CharWidthAdjDelta;
													}
													float num66 = num52 - (num10 - 0.0001f) * (flag67 ? 1.05f : 1f);
													this.m_CharWidthAdjDelta += num66 / num65;
													this.m_CharWidthAdjDelta = Mathf.Min(this.m_CharWidthAdjDelta, generationSettings.charWidthMaxAdj / 100f);
													return;
												}
												bool flag101 = this.m_FontSize > generationSettings.fontSizeMin;
												if (flag101)
												{
													this.m_MaxFontSize = this.m_FontSize;
													float num67 = Mathf.Max((this.m_FontSize - this.m_MinFontSize) / 2f, 0.05f);
													this.m_FontSize -= num67;
													this.m_FontSize = Mathf.Max((float)((int)(this.m_FontSize * 20f + 0.5f)) / 20f, generationSettings.fontSizeMin);
													return;
												}
											}
											switch (generationSettings.overflowMode)
											{
											case TextOverflowMode.Ellipsis:
											{
												bool flag102 = this.m_EllipsisInsertionCandidateStack.Count == 0;
												if (flag102)
												{
													num14 = -1;
													this.m_CharacterCount = 0;
													characterSubstitution.index = 0;
													characterSubstitution.unicode = 3U;
													this.m_FirstCharacterOfLine = 0;
													goto IL_43D7;
												}
												WordWrapState wordWrapState3 = this.m_EllipsisInsertionCandidateStack.Pop();
												num14 = this.RestoreWordWrappingState(ref wordWrapState3, textInfo);
												num14--;
												this.m_CharacterCount--;
												characterSubstitution.index = this.m_CharacterCount;
												characterSubstitution.unicode = 8230U;
												num13++;
												goto IL_43D7;
											}
											case TextOverflowMode.Truncate:
												num14 = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState, textInfo);
												characterSubstitution.index = characterCount;
												characterSubstitution.unicode = 3U;
												goto IL_43D7;
											case TextOverflowMode.Linked:
												num14 = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState, textInfo);
												characterSubstitution.index = this.m_CharacterCount;
												characterSubstitution.unicode = 3U;
												goto IL_43D7;
											}
										}
									}
									bool flag103 = flag36;
									if (flag103)
									{
										textInfo.textElementInfo[this.m_CharacterCount].isVisible = false;
										this.m_LastVisibleCharacterOfLine = this.m_CharacterCount;
										ref int ptr = ref textInfo.lineInfo[this.m_LineNumber].spaceCount;
										this.m_LineVisibleSpaceCount = ++ptr;
										textInfo.lineInfo[this.m_LineNumber].marginLeft = num50;
										textInfo.lineInfo[this.m_LineNumber].marginRight = num51;
										textInfo.spaceCount++;
									}
									else
									{
										bool flag104 = num4 == 173U;
										if (flag104)
										{
											textInfo.textElementInfo[this.m_CharacterCount].isVisible = false;
										}
										else
										{
											bool overrideRichTextColors = generationSettings.overrideRichTextColors;
											Color32 color;
											if (overrideRichTextColors)
											{
												color = this.m_FontColor32;
											}
											else
											{
												color = this.m_HtmlColor;
											}
											bool flag105 = this.m_TextElementType == TextElementType.Character;
											if (flag105)
											{
												this.SaveGlyphVertexInfo(num5, num42, color, generationSettings, textInfo);
											}
											else
											{
												bool flag106 = this.m_TextElementType == TextElementType.Sprite;
												if (flag106)
												{
													this.SaveSpriteVertexInfo(color, generationSettings, textInfo);
												}
											}
											bool flag107 = flag7;
											if (flag107)
											{
												flag7 = false;
												this.m_FirstVisibleCharacterOfLine = this.m_CharacterCount;
											}
											this.m_LineVisibleCharacterCount++;
											this.m_LastVisibleCharacterOfLine = this.m_CharacterCount;
											textInfo.lineInfo[this.m_LineNumber].marginLeft = num50;
											textInfo.lineInfo[this.m_LineNumber].marginRight = num51;
										}
									}
								}
								else
								{
									bool flag108 = generationSettings.overflowMode == TextOverflowMode.Linked && (num4 == 10U || num4 == 11U);
									if (flag108)
									{
										float num68 = this.m_MaxAscender - (this.m_MaxLineDescender - this.m_LineOffset) + ((this.m_LineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_MaxLineAscender - this.m_StartOfLineAscender) : 0f);
										int characterCount2 = this.m_CharacterCount;
										bool flag109 = num68 > num9 + 0.0001f;
										if (flag109)
										{
											bool flag110 = this.m_FirstOverflowCharacterIndex == -1;
											if (flag110)
											{
												this.m_FirstOverflowCharacterIndex = this.m_CharacterCount;
											}
											num14 = this.RestoreWordWrappingState(ref this.m_SavedLastValidState, textInfo);
											characterSubstitution.index = characterCount2;
											characterSubstitution.unicode = 3U;
											goto IL_43D7;
										}
									}
									bool flag111 = (num4 == 10U || num4 == 11U || num4 == 160U || num4 == 8199U || num4 == 8232U || num4 == 8233U || char.IsSeparator((char)num4)) && num4 != 173U && num4 != 8203U && num4 != 8288U;
									if (flag111)
									{
										ref int ptr = ref textInfo.lineInfo[this.m_LineNumber].spaceCount;
										ptr++;
										textInfo.spaceCount++;
									}
									bool flag112 = num4 == 160U;
									if (flag112)
									{
										ref int ptr = ref textInfo.lineInfo[this.m_LineNumber].controlCharacterCount;
										ptr++;
									}
								}
								bool flag113 = generationSettings.overflowMode == TextOverflowMode.Ellipsis && (!flag17 || num4 == 45U);
								if (flag113)
								{
									float num69 = this.m_CurrentFontSize / (float)this.m_Ellipsis.fontAsset.m_FaceInfo.pointSize * this.m_Ellipsis.fontAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
									float num70 = num69 * this.m_FontScaleMultiplier * this.m_Ellipsis.character.m_Scale * this.m_Ellipsis.character.m_Glyph.scale;
									float num71 = this.m_MarginLeft;
									float num72 = this.m_MarginRight;
									bool flag114 = num4 == 10U && this.m_CharacterCount != this.m_FirstCharacterOfLine;
									if (flag114)
									{
										num69 = textInfo.textElementInfo[this.m_CharacterCount - 1].pointSize / (float)this.m_Ellipsis.fontAsset.m_FaceInfo.pointSize * this.m_Ellipsis.fontAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										num70 = num69 * this.m_FontScaleMultiplier * this.m_Ellipsis.character.m_Scale * this.m_Ellipsis.character.m_Glyph.scale;
										num71 = textInfo.lineInfo[this.m_LineNumber].marginLeft;
										num72 = textInfo.lineInfo[this.m_LineNumber].marginRight;
									}
									float num73 = this.m_MaxAscender - (this.m_MaxLineDescender - this.m_LineOffset) + ((this.m_LineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_MaxLineAscender - this.m_StartOfLineAscender) : 0f);
									float num74 = Mathf.Abs(this.m_XAdvance) + ((!generationSettings.isRightToLeft) ? this.m_Ellipsis.character.m_Glyph.metrics.horizontalAdvance : 0f) * (1f - this.m_CharWidthAdjDelta) * num70;
									float num75 = ((this.m_Width != -1f) ? Mathf.Min(num8 + 0.0001f - num71 - num72, this.m_Width) : (num8 + 0.0001f - num71 - num72));
									bool flag115 = num74 < num75 * (flag67 ? 1.05f : 1f);
									if (flag115)
									{
										this.SaveWordWrappingState(ref this.m_SavedEllipsisState, num14, this.m_CharacterCount, textInfo);
										this.m_EllipsisInsertionCandidateStack.Push(this.m_SavedEllipsisState);
									}
								}
								textInfo.textElementInfo[this.m_CharacterCount].lineNumber = this.m_LineNumber;
								textInfo.textElementInfo[this.m_CharacterCount].pageNumber = this.m_PageNumber;
								bool flag116 = (num4 != 10U && num4 != 11U && num4 != 13U && !flag17) || textInfo.lineInfo[this.m_LineNumber].characterCount == 1;
								if (flag116)
								{
									textInfo.lineInfo[this.m_LineNumber].alignment = this.m_LineJustification;
								}
								bool flag117 = num4 != 8203U;
								if (flag117)
								{
									bool flag118 = num4 == 9U;
									if (flag118)
									{
										float num76 = this.m_CurrentFontAsset.m_FaceInfo.tabWidth * (float)this.m_CurrentFontAsset.tabMultiple * num2;
										float num77 = Mathf.Ceil(this.m_XAdvance / num76) * num76;
										this.m_XAdvance = ((num77 > this.m_XAdvance) ? num77 : (this.m_XAdvance + num76));
									}
									else
									{
										bool flag119 = this.m_MonoSpacing != 0f;
										if (flag119)
										{
											this.m_XAdvance += (this.m_MonoSpacing - num41 + (this.m_CurrentFontAsset.regularStyleSpacing + num28) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
											bool flag120 = flag36 || num4 == 8203U;
											if (flag120)
											{
												this.m_XAdvance += generationSettings.wordSpacing * num3;
											}
										}
										else
										{
											bool isRightToLeft2 = generationSettings.isRightToLeft;
											if (isRightToLeft2)
											{
												this.m_XAdvance -= (glyphValueRecord.xAdvance * num2 + (this.m_CurrentFontAsset.regularStyleSpacing + num28 + num43) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
												bool flag121 = flag36 || num4 == 8203U;
												if (flag121)
												{
													this.m_XAdvance -= generationSettings.wordSpacing * num3;
												}
											}
											else
											{
												this.m_XAdvance += ((glyphMetrics.horizontalAdvance * this.m_FXScale.x + glyphValueRecord.xAdvance) * num2 + (this.m_CurrentFontAsset.regularStyleSpacing + num28 + num43) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
												bool flag122 = flag36 || num4 == 8203U;
												if (flag122)
												{
													this.m_XAdvance += generationSettings.wordSpacing * num3;
												}
											}
										}
									}
								}
								textInfo.textElementInfo[this.m_CharacterCount].xAdvance = this.m_XAdvance;
								bool flag123 = num4 == 13U;
								if (flag123)
								{
									this.m_XAdvance = 0f + this.m_TagIndent;
								}
								bool flag124 = generationSettings.overflowMode == TextOverflowMode.Page && num4 != 10U && num4 != 11U && num4 != 13U && num4 != 8232U && num4 != 8233U;
								if (flag124)
								{
									bool flag125 = this.m_PageNumber + 1 > textInfo.pageInfo.Length;
									if (flag125)
									{
										TextInfo.Resize<PageInfo>(ref textInfo.pageInfo, this.m_PageNumber + 1, true);
									}
									textInfo.pageInfo[this.m_PageNumber].ascender = this.m_PageAscender;
									textInfo.pageInfo[this.m_PageNumber].descender = ((this.m_MaxDescender < textInfo.pageInfo[this.m_PageNumber].descender) ? this.m_MaxDescender : textInfo.pageInfo[this.m_PageNumber].descender);
									bool isNewPage = this.m_IsNewPage;
									if (isNewPage)
									{
										this.m_IsNewPage = false;
										textInfo.pageInfo[this.m_PageNumber].firstCharacterIndex = this.m_CharacterCount;
									}
									textInfo.pageInfo[this.m_PageNumber].lastCharacterIndex = this.m_CharacterCount;
								}
								bool flag126 = num4 == 10U || num4 == 11U || num4 == 3U || num4 == 8232U || num4 == 8233U || (num4 == 45U && flag17) || this.m_CharacterCount == totalCharacterCount - 1;
								if (flag126)
								{
									float num78 = this.m_MaxLineAscender - this.m_StartOfLineAscender;
									bool flag127 = this.m_LineOffset > 0f && Math.Abs(num78) > 0.01f && !this.m_IsDrivenLineSpacing && !this.m_IsNewPage;
									if (flag127)
									{
										TextGeneratorUtilities.AdjustLineOffset(this.m_FirstCharacterOfLine, this.m_CharacterCount, num78, textInfo);
										this.m_MaxDescender -= num78;
										this.m_LineOffset += num78;
										bool flag128 = this.m_SavedEllipsisState.lineNumber == this.m_LineNumber;
										if (flag128)
										{
											this.m_SavedEllipsisState = this.m_EllipsisInsertionCandidateStack.Pop();
											ref float ptr2 = ref this.m_SavedEllipsisState.startOfLineAscender;
											ptr2 += num78;
											ptr2 = ref this.m_SavedEllipsisState.lineOffset;
											ptr2 += num78;
											this.m_EllipsisInsertionCandidateStack.Push(this.m_SavedEllipsisState);
										}
									}
									this.m_IsNewPage = false;
									float num79 = this.m_MaxLineAscender - this.m_LineOffset;
									float num80 = this.m_MaxLineDescender - this.m_LineOffset;
									this.m_MaxDescender = ((this.m_MaxDescender < num80) ? this.m_MaxDescender : num80);
									bool flag129 = !flag8;
									if (flag129)
									{
										num11 = this.m_MaxDescender;
									}
									bool flag130 = generationSettings.useMaxVisibleDescender && (this.m_CharacterCount >= generationSettings.maxVisibleCharacters || this.m_LineNumber >= generationSettings.maxVisibleLines);
									if (flag130)
									{
										flag8 = true;
									}
									textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex = this.m_FirstCharacterOfLine;
									textInfo.lineInfo[this.m_LineNumber].firstVisibleCharacterIndex = (this.m_FirstVisibleCharacterOfLine = ((this.m_FirstCharacterOfLine > this.m_FirstVisibleCharacterOfLine) ? this.m_FirstCharacterOfLine : this.m_FirstVisibleCharacterOfLine));
									textInfo.lineInfo[this.m_LineNumber].lastCharacterIndex = (this.m_LastCharacterOfLine = this.m_CharacterCount);
									textInfo.lineInfo[this.m_LineNumber].lastVisibleCharacterIndex = (this.m_LastVisibleCharacterOfLine = ((this.m_LastVisibleCharacterOfLine < this.m_FirstVisibleCharacterOfLine) ? this.m_FirstVisibleCharacterOfLine : this.m_LastVisibleCharacterOfLine));
									textInfo.lineInfo[this.m_LineNumber].characterCount = textInfo.lineInfo[this.m_LineNumber].lastCharacterIndex - textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex + 1;
									textInfo.lineInfo[this.m_LineNumber].visibleCharacterCount = this.m_LineVisibleCharacterCount;
									textInfo.lineInfo[this.m_LineNumber].visibleSpaceCount = this.m_LineVisibleSpaceCount;
									textInfo.lineInfo[this.m_LineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[this.m_FirstVisibleCharacterOfLine].bottomLeft.x, num80);
									textInfo.lineInfo[this.m_LineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].topRight.x, num79);
									textInfo.lineInfo[this.m_LineNumber].length = textInfo.lineInfo[this.m_LineNumber].lineExtents.max.x - num5 * num2;
									textInfo.lineInfo[this.m_LineNumber].width = num10;
									bool flag131 = textInfo.lineInfo[this.m_LineNumber].characterCount == 1;
									if (flag131)
									{
										textInfo.lineInfo[this.m_LineNumber].alignment = this.m_LineJustification;
									}
									float num81 = ((this.m_CurrentFontAsset.regularStyleSpacing + num28 + num43) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
									bool isVisible = textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].isVisible;
									if (isVisible)
									{
										textInfo.lineInfo[this.m_LineNumber].maxAdvance = textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num81 : (-num81));
									}
									else
									{
										textInfo.lineInfo[this.m_LineNumber].maxAdvance = textInfo.textElementInfo[this.m_LastCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num81 : (-num81));
									}
									textInfo.lineInfo[this.m_LineNumber].baseline = 0f - this.m_LineOffset;
									textInfo.lineInfo[this.m_LineNumber].ascender = num79;
									textInfo.lineInfo[this.m_LineNumber].descender = num80;
									textInfo.lineInfo[this.m_LineNumber].lineHeight = num79 - num80 + num6 * num;
									bool flag132 = num4 == 10U || num4 == 11U || num4 == 45U || num4 == 8232U || num4 == 8233U;
									if (flag132)
									{
										this.SaveWordWrappingState(ref this.m_SavedLineState, num14, this.m_CharacterCount, textInfo);
										this.m_LineNumber++;
										flag7 = true;
										flag10 = false;
										flag9 = true;
										this.m_FirstCharacterOfLine = this.m_CharacterCount + 1;
										this.m_LineVisibleCharacterCount = 0;
										this.m_LineVisibleSpaceCount = 0;
										bool flag133 = this.m_LineNumber >= textInfo.lineInfo.Length;
										if (flag133)
										{
											TextGeneratorUtilities.ResizeLineExtents(this.m_LineNumber, textInfo);
										}
										float adjustedAscender2 = textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender;
										bool flag134 = this.m_LineHeight == -32767f;
										if (flag134)
										{
											float num82 = 0f - this.m_MaxLineDescender + adjustedAscender2 + (num6 + this.m_LineSpacingDelta) * num + (generationSettings.lineSpacing + ((num4 == 10U || num4 == 8233U) ? generationSettings.paragraphSpacing : 0f)) * num3;
											this.m_LineOffset += num82;
											this.m_IsDrivenLineSpacing = false;
										}
										else
										{
											this.m_LineOffset += this.m_LineHeight + (generationSettings.lineSpacing + ((num4 == 10U || num4 == 8233U) ? generationSettings.paragraphSpacing : 0f)) * num3;
											this.m_IsDrivenLineSpacing = true;
										}
										this.m_MaxLineAscender = -32767f;
										this.m_MaxLineDescender = 32767f;
										this.m_StartOfLineAscender = adjustedAscender2;
										this.m_XAdvance = 0f + this.m_TagLineIndent + this.m_TagIndent;
										this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num14, this.m_CharacterCount, textInfo);
										this.SaveWordWrappingState(ref this.m_SavedLastValidState, num14, this.m_CharacterCount, textInfo);
										this.m_CharacterCount++;
										goto IL_43D7;
									}
									bool flag135 = num4 == 3U;
									if (flag135)
									{
										num14 = this.m_TextProcessingArray.Length;
									}
								}
								bool isVisible2 = textInfo.textElementInfo[this.m_CharacterCount].isVisible;
								if (isVisible2)
								{
									this.m_MeshExtents.min.x = Mathf.Min(this.m_MeshExtents.min.x, textInfo.textElementInfo[this.m_CharacterCount].bottomLeft.x);
									this.m_MeshExtents.min.y = Mathf.Min(this.m_MeshExtents.min.y, textInfo.textElementInfo[this.m_CharacterCount].bottomLeft.y);
									this.m_MeshExtents.max.x = Mathf.Max(this.m_MeshExtents.max.x, textInfo.textElementInfo[this.m_CharacterCount].topRight.x);
									this.m_MeshExtents.max.y = Mathf.Max(this.m_MeshExtents.max.y, textInfo.textElementInfo[this.m_CharacterCount].topRight.y);
								}
								bool flag136 = (textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap) || generationSettings.overflowMode == TextOverflowMode.Truncate || generationSettings.overflowMode == TextOverflowMode.Ellipsis || generationSettings.overflowMode == TextOverflowMode.Linked;
								if (flag136)
								{
									bool flag137 = (flag36 || num4 == 8203U || num4 == 45U || num4 == 173U) && (!this.m_IsNonBreakingSpace || flag10) && num4 != 160U && num4 != 8199U && num4 != 8209U && num4 != 8239U && num4 != 8288U;
									if (flag137)
									{
										this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num14, this.m_CharacterCount, textInfo);
										flag9 = false;
										this.m_SavedSoftLineBreakState.previousWordBreak = -1;
									}
									else
									{
										bool flag138 = !this.m_IsNonBreakingSpace && ((TextGeneratorUtilities.IsHangul(num4) && !textSettings.lineBreakingRules.useModernHangulLineBreakingRules) || TextGeneratorUtilities.IsCJK(num4));
										if (flag138)
										{
											bool flag139 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(num4);
											bool flag140 = this.m_CharacterCount < totalCharacterCount - 1 && textSettings.lineBreakingRules.followingCharactersLookup.Contains((uint)textInfo.textElementInfo[this.m_CharacterCount + 1].character);
											bool flag141 = !flag139;
											if (flag141)
											{
												bool flag142 = !flag140;
												if (flag142)
												{
													this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num14, this.m_CharacterCount, textInfo);
													flag9 = false;
												}
												bool flag143 = flag9;
												if (flag143)
												{
													bool flag144 = flag36;
													if (flag144)
													{
														this.SaveWordWrappingState(ref this.m_SavedSoftLineBreakState, num14, this.m_CharacterCount, textInfo);
													}
													this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num14, this.m_CharacterCount, textInfo);
												}
											}
											else
											{
												bool flag145 = flag9 && flag59;
												if (flag145)
												{
													bool flag146 = flag36;
													if (flag146)
													{
														this.SaveWordWrappingState(ref this.m_SavedSoftLineBreakState, num14, this.m_CharacterCount, textInfo);
													}
													this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num14, this.m_CharacterCount, textInfo);
												}
											}
										}
										else
										{
											bool flag147 = flag9;
											if (flag147)
											{
												bool flag148 = (flag36 && num4 != 160U) || (num4 == 173U && !flag11);
												if (flag148)
												{
													this.SaveWordWrappingState(ref this.m_SavedSoftLineBreakState, num14, this.m_CharacterCount, textInfo);
												}
												this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num14, this.m_CharacterCount, textInfo);
											}
										}
									}
								}
								this.SaveWordWrappingState(ref this.m_SavedLastValidState, num14, this.m_CharacterCount, textInfo);
								this.m_CharacterCount++;
							}
						}
						IL_43D7:
						num38 = num14;
						num14 = num38 + 1;
					}
					float num83 = this.m_MaxFontSize - this.m_MinFontSize;
					bool flag149 = generationSettings.autoSize && num83 > 0.051f && this.m_FontSize < generationSettings.fontSizeMax && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
					if (flag149)
					{
						bool flag150 = this.m_CharWidthAdjDelta < generationSettings.charWidthMaxAdj / 100f;
						if (flag150)
						{
							this.m_CharWidthAdjDelta = 0f;
						}
						this.m_MinFontSize = this.m_FontSize;
						float num84 = Mathf.Max((this.m_MaxFontSize - this.m_FontSize) / 2f, 0.05f);
						this.m_FontSize += num84;
						this.m_FontSize = Mathf.Min((float)((int)(this.m_FontSize * 20f + 0.5f)) / 20f, generationSettings.charWidthMaxAdj);
					}
					else
					{
						this.m_IsAutoSizePointSizeSet = true;
						bool flag151 = this.m_AutoSizeIterationCount >= this.m_AutoSizeMaxIterationCount;
						if (flag151)
						{
							Debug.Log("Auto Size Iteration Count: " + this.m_AutoSizeIterationCount.ToString() + ". Final Point Size: " + this.m_FontSize.ToString());
						}
						bool flag152 = this.m_CharacterCount == 0 || (this.m_CharacterCount == 1 && num4 == 3U);
						if (flag152)
						{
							TextGenerator.ClearMesh(true, textInfo);
						}
						else
						{
							textInfo.meshInfo[this.m_CurrentMaterialIndex].Clear(false);
							Vector3 vector10 = Vector3.zero;
							Vector3[] rectTransformCorners = this.m_RectTransformCorners;
							TextAlignment textAlignment = generationSettings.textAlignment;
							TextAlignment textAlignment2 = textAlignment;
							if (textAlignment2 <= TextAlignment.BottomGeoAligned)
							{
								if (textAlignment2 <= TextAlignment.MiddleRight)
								{
									if (textAlignment2 <= TextAlignment.TopJustified)
									{
										if (textAlignment2 - TextAlignment.TopLeft > 1 && textAlignment2 != TextAlignment.TopRight && textAlignment2 != TextAlignment.TopJustified)
										{
											goto IL_4C35;
										}
									}
									else if (textAlignment2 <= TextAlignment.TopGeoAligned)
									{
										if (textAlignment2 != TextAlignment.TopFlush && textAlignment2 != TextAlignment.TopGeoAligned)
										{
											goto IL_4C35;
										}
									}
									else
									{
										if (textAlignment2 - TextAlignment.MiddleLeft > 1 && textAlignment2 != TextAlignment.MiddleRight)
										{
											goto IL_4C35;
										}
										goto IL_4973;
									}
									bool flag153 = generationSettings.overflowMode != TextOverflowMode.Page;
									if (flag153)
									{
										vector10 = rectTransformCorners[1] + new Vector3(0f + margins.x, 0f - this.m_MaxAscender - margins.y, 0f);
									}
									else
									{
										vector10 = rectTransformCorners[1] + new Vector3(0f + margins.x, 0f - textInfo.pageInfo[num7].ascender - margins.y, 0f);
									}
									goto IL_4C35;
								}
								if (textAlignment2 <= TextAlignment.BottomCenter)
								{
									if (textAlignment2 <= TextAlignment.MiddleFlush)
									{
										if (textAlignment2 != TextAlignment.MiddleJustified && textAlignment2 != TextAlignment.MiddleFlush)
										{
											goto IL_4C35;
										}
										goto IL_4973;
									}
									else
									{
										if (textAlignment2 == TextAlignment.MiddleGeoAligned)
										{
											goto IL_4973;
										}
										if (textAlignment2 - TextAlignment.BottomLeft > 1)
										{
											goto IL_4C35;
										}
									}
								}
								else if (textAlignment2 <= TextAlignment.BottomJustified)
								{
									if (textAlignment2 != TextAlignment.BottomRight && textAlignment2 != TextAlignment.BottomJustified)
									{
										goto IL_4C35;
									}
								}
								else if (textAlignment2 != TextAlignment.BottomFlush && textAlignment2 != TextAlignment.BottomGeoAligned)
								{
									goto IL_4C35;
								}
								bool flag154 = generationSettings.overflowMode != TextOverflowMode.Page;
								if (flag154)
								{
									vector10 = rectTransformCorners[0] + new Vector3(0f + margins.x, 0f - num11 + margins.w, 0f);
								}
								else
								{
									vector10 = rectTransformCorners[0] + new Vector3(0f + margins.x, 0f - textInfo.pageInfo[num7].descender + margins.w, 0f);
								}
								goto IL_4C35;
								IL_4973:
								bool flag155 = generationSettings.overflowMode != TextOverflowMode.Page;
								if (flag155)
								{
									vector10 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margins.x, 0f - (this.m_MaxAscender + margins.y + num11 - margins.w) / 2f, 0f);
								}
								else
								{
									vector10 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margins.x, 0f - (textInfo.pageInfo[num7].ascender + margins.y + textInfo.pageInfo[num7].descender - margins.w) / 2f, 0f);
								}
							}
							else
							{
								if (textAlignment2 <= TextAlignment.MidlineRight)
								{
									if (textAlignment2 <= TextAlignment.BaselineJustified)
									{
										if (textAlignment2 - TextAlignment.BaselineLeft > 1 && textAlignment2 != TextAlignment.BaselineRight && textAlignment2 != TextAlignment.BaselineJustified)
										{
											goto IL_4C35;
										}
									}
									else if (textAlignment2 <= TextAlignment.BaselineGeoAligned)
									{
										if (textAlignment2 != TextAlignment.BaselineFlush && textAlignment2 != TextAlignment.BaselineGeoAligned)
										{
											goto IL_4C35;
										}
									}
									else
									{
										if (textAlignment2 - TextAlignment.MidlineLeft > 1 && textAlignment2 != TextAlignment.MidlineRight)
										{
											goto IL_4C35;
										}
										goto IL_4B58;
									}
									vector10 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margins.x, 0f, 0f);
									goto IL_4C35;
								}
								if (textAlignment2 <= TextAlignment.CaplineCenter)
								{
									if (textAlignment2 <= TextAlignment.MidlineFlush)
									{
										if (textAlignment2 != TextAlignment.MidlineJustified && textAlignment2 != TextAlignment.MidlineFlush)
										{
											goto IL_4C35;
										}
										goto IL_4B58;
									}
									else
									{
										if (textAlignment2 == TextAlignment.MidlineGeoAligned)
										{
											goto IL_4B58;
										}
										if (textAlignment2 - TextAlignment.CaplineLeft > 1)
										{
											goto IL_4C35;
										}
									}
								}
								else if (textAlignment2 <= TextAlignment.CaplineJustified)
								{
									if (textAlignment2 != TextAlignment.CaplineRight && textAlignment2 != TextAlignment.CaplineJustified)
									{
										goto IL_4C35;
									}
								}
								else if (textAlignment2 != TextAlignment.CaplineFlush && textAlignment2 != TextAlignment.CaplineGeoAligned)
								{
									goto IL_4C35;
								}
								vector10 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margins.x, 0f - (this.m_MaxCapHeight - margins.y - margins.w) / 2f, 0f);
								goto IL_4C35;
								IL_4B58:
								vector10 = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f + margins.x, 0f - (this.m_MeshExtents.max.y + margins.y + this.m_MeshExtents.min.y - margins.w) / 2f, 0f);
							}
							IL_4C35:
							Vector3 vector11 = Vector3.zero;
							Vector3 vector12 = Vector3.zero;
							int num85 = 0;
							int num86 = 0;
							int num87 = 0;
							bool flag156 = false;
							bool flag157 = false;
							int num88 = 0;
							Color32 color2 = Color.white;
							Color32 color3 = Color.white;
							HighlightState highlightState = new HighlightState(new Color32(byte.MaxValue, byte.MaxValue, 0, 64), Offset.zero);
							float num89 = 0f;
							float num90 = 0f;
							float num91 = 0f;
							float num92 = 0f;
							float num93 = 32767f;
							int num94 = 0;
							float num95 = 0f;
							float num96 = 0f;
							float num97 = 0f;
							TextElementInfo[] textElementInfo = textInfo.textElementInfo;
							int i = 0;
							int num38;
							while (i < this.m_CharacterCount)
							{
								FontAsset fontAsset = textElementInfo[i].fontAsset;
								char character = textElementInfo[i].character;
								bool flag158 = char.IsWhiteSpace(character);
								int lineNumber = textElementInfo[i].lineNumber;
								LineInfo lineInfo = textInfo.lineInfo[lineNumber];
								num86 = lineNumber + 1;
								TextAlignment alignment = lineInfo.alignment;
								TextAlignment textAlignment3 = alignment;
								TextAlignment textAlignment4 = textAlignment3;
								if (textAlignment4 <= TextAlignment.BottomGeoAligned)
								{
									if (textAlignment4 <= TextAlignment.MiddleJustified)
									{
										if (textAlignment4 <= TextAlignment.TopFlush)
										{
											switch (textAlignment4)
											{
											case TextAlignment.TopLeft:
												goto IL_501A;
											case TextAlignment.TopCenter:
												goto IL_507C;
											case (TextAlignment)259:
												break;
											case TextAlignment.TopRight:
												goto IL_5126;
											default:
												if (textAlignment4 == TextAlignment.TopJustified || textAlignment4 == TextAlignment.TopFlush)
												{
													goto IL_51A0;
												}
												break;
											}
										}
										else
										{
											if (textAlignment4 == TextAlignment.TopGeoAligned)
											{
												goto IL_50C1;
											}
											switch (textAlignment4)
											{
											case TextAlignment.MiddleLeft:
												goto IL_501A;
											case TextAlignment.MiddleCenter:
												goto IL_507C;
											case (TextAlignment)515:
												break;
											case TextAlignment.MiddleRight:
												goto IL_5126;
											default:
												if (textAlignment4 == TextAlignment.MiddleJustified)
												{
													goto IL_51A0;
												}
												break;
											}
										}
									}
									else if (textAlignment4 <= TextAlignment.BottomRight)
									{
										if (textAlignment4 == TextAlignment.MiddleFlush)
										{
											goto IL_51A0;
										}
										if (textAlignment4 == TextAlignment.MiddleGeoAligned)
										{
											goto IL_50C1;
										}
										switch (textAlignment4)
										{
										case TextAlignment.BottomLeft:
											goto IL_501A;
										case TextAlignment.BottomCenter:
											goto IL_507C;
										case TextAlignment.BottomRight:
											goto IL_5126;
										}
									}
									else
									{
										if (textAlignment4 == TextAlignment.BottomJustified || textAlignment4 == TextAlignment.BottomFlush)
										{
											goto IL_51A0;
										}
										if (textAlignment4 == TextAlignment.BottomGeoAligned)
										{
											goto IL_50C1;
										}
									}
								}
								else if (textAlignment4 <= TextAlignment.MidlineJustified)
								{
									if (textAlignment4 <= TextAlignment.BaselineFlush)
									{
										switch (textAlignment4)
										{
										case TextAlignment.BaselineLeft:
											goto IL_501A;
										case TextAlignment.BaselineCenter:
											goto IL_507C;
										case (TextAlignment)2051:
											break;
										case TextAlignment.BaselineRight:
											goto IL_5126;
										default:
											if (textAlignment4 == TextAlignment.BaselineJustified || textAlignment4 == TextAlignment.BaselineFlush)
											{
												goto IL_51A0;
											}
											break;
										}
									}
									else
									{
										if (textAlignment4 == TextAlignment.BaselineGeoAligned)
										{
											goto IL_50C1;
										}
										switch (textAlignment4)
										{
										case TextAlignment.MidlineLeft:
											goto IL_501A;
										case TextAlignment.MidlineCenter:
											goto IL_507C;
										case (TextAlignment)4099:
											break;
										case TextAlignment.MidlineRight:
											goto IL_5126;
										default:
											if (textAlignment4 == TextAlignment.MidlineJustified)
											{
												goto IL_51A0;
											}
											break;
										}
									}
								}
								else if (textAlignment4 <= TextAlignment.CaplineRight)
								{
									if (textAlignment4 == TextAlignment.MidlineFlush)
									{
										goto IL_51A0;
									}
									if (textAlignment4 == TextAlignment.MidlineGeoAligned)
									{
										goto IL_50C1;
									}
									switch (textAlignment4)
									{
									case TextAlignment.CaplineLeft:
										goto IL_501A;
									case TextAlignment.CaplineCenter:
										goto IL_507C;
									case TextAlignment.CaplineRight:
										goto IL_5126;
									}
								}
								else
								{
									if (textAlignment4 == TextAlignment.CaplineJustified || textAlignment4 == TextAlignment.CaplineFlush)
									{
										goto IL_51A0;
									}
									if (textAlignment4 == TextAlignment.CaplineGeoAligned)
									{
										goto IL_50C1;
									}
								}
								IL_55B9:
								vector12 = vector10 + vector11;
								bool isVisible3 = textElementInfo[i].isVisible;
								bool flag159 = isVisible3;
								ref Vector3 ptr3;
								if (flag159)
								{
									TextElementType elementType = textElementInfo[i].elementType;
									TextElementType textElementType = elementType;
									TextElementType textElementType2 = textElementType;
									if (textElementType2 != TextElementType.Character)
									{
										if (textElementType2 != TextElementType.Sprite)
										{
										}
									}
									else
									{
										Extents lineExtents = lineInfo.lineExtents;
										float num98 = generationSettings.uvLineOffset * (float)lineNumber % 1f;
										switch (generationSettings.horizontalMapping)
										{
										case TextureMapping.Character:
											textElementInfo[i].vertexBottomLeft.uv2.x = 0f;
											textElementInfo[i].vertexTopLeft.uv2.x = 0f;
											textElementInfo[i].vertexTopRight.uv2.x = 1f;
											textElementInfo[i].vertexBottomRight.uv2.x = 1f;
											break;
										case TextureMapping.Line:
										{
											bool flag160 = generationSettings.textAlignment != TextAlignment.MiddleJustified;
											if (flag160)
											{
												textElementInfo[i].vertexBottomLeft.uv2.x = (textElementInfo[i].vertexBottomLeft.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num98;
												textElementInfo[i].vertexTopLeft.uv2.x = (textElementInfo[i].vertexTopLeft.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num98;
												textElementInfo[i].vertexTopRight.uv2.x = (textElementInfo[i].vertexTopRight.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num98;
												textElementInfo[i].vertexBottomRight.uv2.x = (textElementInfo[i].vertexBottomRight.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + num98;
											}
											else
											{
												textElementInfo[i].vertexBottomLeft.uv2.x = (textElementInfo[i].vertexBottomLeft.position.x + vector11.x - this.m_MeshExtents.min.x) / (this.m_MeshExtents.max.x - this.m_MeshExtents.min.x) + num98;
												textElementInfo[i].vertexTopLeft.uv2.x = (textElementInfo[i].vertexTopLeft.position.x + vector11.x - this.m_MeshExtents.min.x) / (this.m_MeshExtents.max.x - this.m_MeshExtents.min.x) + num98;
												textElementInfo[i].vertexTopRight.uv2.x = (textElementInfo[i].vertexTopRight.position.x + vector11.x - this.m_MeshExtents.min.x) / (this.m_MeshExtents.max.x - this.m_MeshExtents.min.x) + num98;
												textElementInfo[i].vertexBottomRight.uv2.x = (textElementInfo[i].vertexBottomRight.position.x + vector11.x - this.m_MeshExtents.min.x) / (this.m_MeshExtents.max.x - this.m_MeshExtents.min.x) + num98;
											}
											break;
										}
										case TextureMapping.Paragraph:
											textElementInfo[i].vertexBottomLeft.uv2.x = (textElementInfo[i].vertexBottomLeft.position.x + vector11.x - this.m_MeshExtents.min.x) / (this.m_MeshExtents.max.x - this.m_MeshExtents.min.x) + num98;
											textElementInfo[i].vertexTopLeft.uv2.x = (textElementInfo[i].vertexTopLeft.position.x + vector11.x - this.m_MeshExtents.min.x) / (this.m_MeshExtents.max.x - this.m_MeshExtents.min.x) + num98;
											textElementInfo[i].vertexTopRight.uv2.x = (textElementInfo[i].vertexTopRight.position.x + vector11.x - this.m_MeshExtents.min.x) / (this.m_MeshExtents.max.x - this.m_MeshExtents.min.x) + num98;
											textElementInfo[i].vertexBottomRight.uv2.x = (textElementInfo[i].vertexBottomRight.position.x + vector11.x - this.m_MeshExtents.min.x) / (this.m_MeshExtents.max.x - this.m_MeshExtents.min.x) + num98;
											break;
										case TextureMapping.MatchAspect:
										{
											switch (generationSettings.verticalMapping)
											{
											case TextureMapping.Character:
												textElementInfo[i].vertexBottomLeft.uv2.y = 0f;
												textElementInfo[i].vertexTopLeft.uv2.y = 1f;
												textElementInfo[i].vertexTopRight.uv2.y = 0f;
												textElementInfo[i].vertexBottomRight.uv2.y = 1f;
												break;
											case TextureMapping.Line:
												textElementInfo[i].vertexBottomLeft.uv2.y = (textElementInfo[i].vertexBottomLeft.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num98;
												textElementInfo[i].vertexTopLeft.uv2.y = (textElementInfo[i].vertexTopLeft.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + num98;
												textElementInfo[i].vertexTopRight.uv2.y = textElementInfo[i].vertexBottomLeft.uv2.y;
												textElementInfo[i].vertexBottomRight.uv2.y = textElementInfo[i].vertexTopLeft.uv2.y;
												break;
											case TextureMapping.Paragraph:
												textElementInfo[i].vertexBottomLeft.uv2.y = (textElementInfo[i].vertexBottomLeft.position.y - this.m_MeshExtents.min.y) / (this.m_MeshExtents.max.y - this.m_MeshExtents.min.y) + num98;
												textElementInfo[i].vertexTopLeft.uv2.y = (textElementInfo[i].vertexTopLeft.position.y - this.m_MeshExtents.min.y) / (this.m_MeshExtents.max.y - this.m_MeshExtents.min.y) + num98;
												textElementInfo[i].vertexTopRight.uv2.y = textElementInfo[i].vertexBottomLeft.uv2.y;
												textElementInfo[i].vertexBottomRight.uv2.y = textElementInfo[i].vertexTopLeft.uv2.y;
												break;
											case TextureMapping.MatchAspect:
												Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
												break;
											}
											float num99 = (1f - (textElementInfo[i].vertexBottomLeft.uv2.y + textElementInfo[i].vertexTopLeft.uv2.y) * textElementInfo[i].aspectRatio) / 2f;
											textElementInfo[i].vertexBottomLeft.uv2.x = textElementInfo[i].vertexBottomLeft.uv2.y * textElementInfo[i].aspectRatio + num99 + num98;
											textElementInfo[i].vertexTopLeft.uv2.x = textElementInfo[i].vertexBottomLeft.uv2.x;
											textElementInfo[i].vertexTopRight.uv2.x = textElementInfo[i].vertexTopLeft.uv2.y * textElementInfo[i].aspectRatio + num99 + num98;
											textElementInfo[i].vertexBottomRight.uv2.x = textElementInfo[i].vertexTopRight.uv2.x;
											break;
										}
										}
										switch (generationSettings.verticalMapping)
										{
										case TextureMapping.Character:
											textElementInfo[i].vertexBottomLeft.uv2.y = 0f;
											textElementInfo[i].vertexTopLeft.uv2.y = 1f;
											textElementInfo[i].vertexTopRight.uv2.y = 1f;
											textElementInfo[i].vertexBottomRight.uv2.y = 0f;
											break;
										case TextureMapping.Line:
											textElementInfo[i].vertexBottomLeft.uv2.y = (textElementInfo[i].vertexBottomLeft.position.y - lineInfo.descender) / (lineInfo.ascender - lineInfo.descender);
											textElementInfo[i].vertexTopLeft.uv2.y = (textElementInfo[i].vertexTopLeft.position.y - lineInfo.descender) / (lineInfo.ascender - lineInfo.descender);
											textElementInfo[i].vertexTopRight.uv2.y = textElementInfo[i].vertexTopLeft.uv2.y;
											textElementInfo[i].vertexBottomRight.uv2.y = textElementInfo[i].vertexBottomLeft.uv2.y;
											break;
										case TextureMapping.Paragraph:
											textElementInfo[i].vertexBottomLeft.uv2.y = (textElementInfo[i].vertexBottomLeft.position.y - this.m_MeshExtents.min.y) / (this.m_MeshExtents.max.y - this.m_MeshExtents.min.y);
											textElementInfo[i].vertexTopLeft.uv2.y = (textElementInfo[i].vertexTopLeft.position.y - this.m_MeshExtents.min.y) / (this.m_MeshExtents.max.y - this.m_MeshExtents.min.y);
											textElementInfo[i].vertexTopRight.uv2.y = textElementInfo[i].vertexTopLeft.uv2.y;
											textElementInfo[i].vertexBottomRight.uv2.y = textElementInfo[i].vertexBottomLeft.uv2.y;
											break;
										case TextureMapping.MatchAspect:
										{
											float num100 = (1f - (textElementInfo[i].vertexBottomLeft.uv2.x + textElementInfo[i].vertexTopRight.uv2.x) / textElementInfo[i].aspectRatio) / 2f;
											textElementInfo[i].vertexBottomLeft.uv2.y = num100 + textElementInfo[i].vertexBottomLeft.uv2.x / textElementInfo[i].aspectRatio;
											textElementInfo[i].vertexTopLeft.uv2.y = num100 + textElementInfo[i].vertexTopRight.uv2.x / textElementInfo[i].aspectRatio;
											textElementInfo[i].vertexBottomRight.uv2.y = textElementInfo[i].vertexBottomLeft.uv2.y;
											textElementInfo[i].vertexTopRight.uv2.y = textElementInfo[i].vertexTopLeft.uv2.y;
											break;
										}
										}
										num89 = textElementInfo[i].scale * (1f - this.m_CharWidthAdjDelta) * 1f;
										bool flag161 = !textElementInfo[i].isUsingAlternateTypeface && (textElementInfo[i].style & FontStyles.Bold) == FontStyles.Bold;
										if (flag161)
										{
											num89 *= -1f;
										}
										textElementInfo[i].vertexBottomLeft.uv.w = num89;
										textElementInfo[i].vertexTopLeft.uv.w = num89;
										textElementInfo[i].vertexTopRight.uv.w = num89;
										textElementInfo[i].vertexBottomRight.uv.w = num89;
										textElementInfo[i].vertexBottomLeft.uv2.x = 1f;
										textElementInfo[i].vertexBottomLeft.uv2.y = num89;
										textElementInfo[i].vertexTopLeft.uv2.x = 1f;
										textElementInfo[i].vertexTopLeft.uv2.y = num89;
										textElementInfo[i].vertexTopRight.uv2.x = 1f;
										textElementInfo[i].vertexTopRight.uv2.y = num89;
										textElementInfo[i].vertexBottomRight.uv2.x = 1f;
										textElementInfo[i].vertexBottomRight.uv2.y = num89;
									}
									bool flag162 = i < generationSettings.maxVisibleCharacters && num85 < generationSettings.maxVisibleWords && lineNumber < generationSettings.maxVisibleLines && generationSettings.overflowMode != TextOverflowMode.Page;
									if (flag162)
									{
										ptr3 = ref textElementInfo[i].vertexBottomLeft.position;
										ptr3 += vector12;
										ptr3 = ref textElementInfo[i].vertexTopLeft.position;
										ptr3 += vector12;
										ptr3 = ref textElementInfo[i].vertexTopRight.position;
										ptr3 += vector12;
										ptr3 = ref textElementInfo[i].vertexBottomRight.position;
										ptr3 += vector12;
									}
									else
									{
										bool flag163 = i < generationSettings.maxVisibleCharacters && num85 < generationSettings.maxVisibleWords && lineNumber < generationSettings.maxVisibleLines && generationSettings.overflowMode == TextOverflowMode.Page && textElementInfo[i].pageNumber == num7;
										if (flag163)
										{
											ptr3 = ref textElementInfo[i].vertexBottomLeft.position;
											ptr3 += vector12;
											ptr3 = ref textElementInfo[i].vertexTopLeft.position;
											ptr3 += vector12;
											ptr3 = ref textElementInfo[i].vertexTopRight.position;
											ptr3 += vector12;
											ptr3 = ref textElementInfo[i].vertexBottomRight.position;
											ptr3 += vector12;
										}
										else
										{
											textElementInfo[i].vertexBottomLeft.position = Vector3.zero;
											textElementInfo[i].vertexTopLeft.position = Vector3.zero;
											textElementInfo[i].vertexTopRight.position = Vector3.zero;
											textElementInfo[i].vertexBottomRight.position = Vector3.zero;
											textElementInfo[i].isVisible = false;
										}
									}
									bool flag164 = QualitySettings.activeColorSpace == ColorSpace.Linear && generationSettings.shouldConvertToLinearSpace;
									bool flag165 = elementType == TextElementType.Character;
									if (flag165)
									{
										TextGeneratorUtilities.FillCharacterVertexBuffers(i, flag164, generationSettings, textInfo);
									}
									else
									{
										bool flag166 = elementType == TextElementType.Sprite;
										if (flag166)
										{
											TextGeneratorUtilities.FillSpriteVertexBuffers(i, flag164, generationSettings, textInfo);
										}
									}
								}
								ptr3 = ref textInfo.textElementInfo[i].bottomLeft;
								ptr3 += vector12;
								ptr3 = ref textInfo.textElementInfo[i].topLeft;
								ptr3 += vector12;
								ptr3 = ref textInfo.textElementInfo[i].topRight;
								ptr3 += vector12;
								ptr3 = ref textInfo.textElementInfo[i].bottomRight;
								ptr3 += vector12;
								ref float ptr2 = ref textInfo.textElementInfo[i].origin;
								ptr2 += vector12.x;
								ptr2 = ref textInfo.textElementInfo[i].xAdvance;
								ptr2 += vector12.x;
								ptr2 = ref textInfo.textElementInfo[i].ascender;
								ptr2 += vector12.y;
								ptr2 = ref textInfo.textElementInfo[i].descender;
								ptr2 += vector12.y;
								ptr2 = ref textInfo.textElementInfo[i].baseLine;
								ptr2 += vector12.y;
								bool flag167 = isVisible3;
								if (flag167)
								{
								}
								bool flag168 = lineNumber != num87 || i == this.m_CharacterCount - 1;
								if (flag168)
								{
									bool flag169 = lineNumber != num87;
									if (flag169)
									{
										ptr2 = ref textInfo.lineInfo[num87].baseline;
										ptr2 += vector12.y;
										ptr2 = ref textInfo.lineInfo[num87].ascender;
										ptr2 += vector12.y;
										ptr2 = ref textInfo.lineInfo[num87].descender;
										ptr2 += vector12.y;
										ptr2 = ref textInfo.lineInfo[num87].maxAdvance;
										ptr2 += vector12.x;
										textInfo.lineInfo[num87].lineExtents.min = new Vector2(textInfo.textElementInfo[textInfo.lineInfo[num87].firstCharacterIndex].bottomLeft.x, textInfo.lineInfo[num87].descender);
										textInfo.lineInfo[num87].lineExtents.max = new Vector2(textInfo.textElementInfo[textInfo.lineInfo[num87].lastVisibleCharacterIndex].topRight.x, textInfo.lineInfo[num87].ascender);
									}
									bool flag170 = i == this.m_CharacterCount - 1;
									if (flag170)
									{
										ptr2 = ref textInfo.lineInfo[lineNumber].baseline;
										ptr2 += vector12.y;
										ptr2 = ref textInfo.lineInfo[lineNumber].ascender;
										ptr2 += vector12.y;
										ptr2 = ref textInfo.lineInfo[lineNumber].descender;
										ptr2 += vector12.y;
										ptr2 = ref textInfo.lineInfo[lineNumber].maxAdvance;
										ptr2 += vector12.x;
										textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, textInfo.lineInfo[lineNumber].descender);
										textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex].topRight.x, textInfo.lineInfo[lineNumber].ascender);
									}
								}
								bool flag171 = char.IsLetterOrDigit(character) || character == '-' || character == '\u00ad' || character == '‐' || character == '‑';
								if (flag171)
								{
									bool flag172 = !flag157;
									if (flag172)
									{
										flag157 = true;
										num88 = i;
									}
									bool flag173 = flag157 && i == this.m_CharacterCount - 1;
									if (flag173)
									{
										int num101 = textInfo.wordInfo.Length;
										int wordCount = textInfo.wordCount;
										bool flag174 = textInfo.wordCount + 1 > num101;
										if (flag174)
										{
											TextInfo.Resize<WordInfo>(ref textInfo.wordInfo, num101 + 1);
										}
										int num102 = i;
										textInfo.wordInfo[wordCount].firstCharacterIndex = num88;
										textInfo.wordInfo[wordCount].lastCharacterIndex = num102;
										textInfo.wordInfo[wordCount].characterCount = num102 - num88 + 1;
										num85++;
										textInfo.wordCount++;
										ref int ptr = ref textInfo.lineInfo[lineNumber].wordCount;
										ptr++;
									}
								}
								else
								{
									bool flag175 = flag157 || (i == 0 && (!char.IsPunctuation(character) || flag158 || character == '\u200b' || i == this.m_CharacterCount - 1));
									if (flag175)
									{
										bool flag176 = i > 0 && i < textElementInfo.Length - 1 && i < this.m_CharacterCount && (character == '\'' || character == '’') && char.IsLetterOrDigit(textElementInfo[i - 1].character) && char.IsLetterOrDigit(textElementInfo[i + 1].character);
										if (!flag176)
										{
											int num102 = ((i == this.m_CharacterCount - 1 && char.IsLetterOrDigit(character)) ? i : (i - 1));
											flag157 = false;
											int num103 = textInfo.wordInfo.Length;
											int wordCount2 = textInfo.wordCount;
											bool flag177 = textInfo.wordCount + 1 > num103;
											if (flag177)
											{
												TextInfo.Resize<WordInfo>(ref textInfo.wordInfo, num103 + 1);
											}
											textInfo.wordInfo[wordCount2].firstCharacterIndex = num88;
											textInfo.wordInfo[wordCount2].lastCharacterIndex = num102;
											textInfo.wordInfo[wordCount2].characterCount = num102 - num88 + 1;
											num85++;
											textInfo.wordCount++;
											ref int ptr = ref textInfo.lineInfo[lineNumber].wordCount;
											ptr++;
										}
									}
								}
								bool flag178 = (textInfo.textElementInfo[i].style & FontStyles.Underline) == FontStyles.Underline;
								bool flag179 = flag178;
								if (flag179)
								{
									bool flag180 = true;
									int pageNumber = textInfo.textElementInfo[i].pageNumber;
									textInfo.textElementInfo[i].underlineVertexIndex = this.m_MaterialReferences[this.m_Underline.materialIndex].referenceCount * 4;
									bool flag181 = i > generationSettings.maxVisibleCharacters || lineNumber > generationSettings.maxVisibleLines || (generationSettings.overflowMode == TextOverflowMode.Page && pageNumber + 1 != generationSettings.pageToDisplay);
									if (flag181)
									{
										flag180 = false;
									}
									bool flag182 = !flag158 && character != '\u200b';
									if (flag182)
									{
										num92 = Mathf.Max(num92, textInfo.textElementInfo[i].scale);
										num90 = Mathf.Max(num90, Mathf.Abs(num89));
										num93 = Mathf.Min((pageNumber == num94) ? num93 : 32767f, textInfo.textElementInfo[i].baseLine + fontAsset.faceInfo.underlineOffset * num92);
										num94 = pageNumber;
									}
									bool flag183 = !flag4 && flag180 && i <= lineInfo.lastVisibleCharacterIndex && character != '\n' && character != '\v' && character != '\r';
									if (flag183)
									{
										bool flag184 = i == lineInfo.lastVisibleCharacterIndex && char.IsSeparator(character);
										if (!flag184)
										{
											flag4 = true;
											num91 = textInfo.textElementInfo[i].scale;
											bool flag185 = num92 == 0f;
											if (flag185)
											{
												num92 = num91;
												num90 = num89;
											}
											zero = new Vector3(textInfo.textElementInfo[i].bottomLeft.x, num93, 0f);
											color2 = textInfo.textElementInfo[i].underlineColor;
										}
									}
									bool flag186 = flag4 && this.m_CharacterCount == 1;
									if (flag186)
									{
										flag4 = false;
										zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num93, 0f);
										float num104 = textInfo.textElementInfo[i].scale;
										this.DrawUnderlineMesh(zero, zero2, num91, num104, num92, num90, color2, generationSettings, textInfo);
										num92 = 0f;
										num90 = 0f;
										num93 = 32767f;
									}
									else
									{
										bool flag187 = flag4 && (i == lineInfo.lastCharacterIndex || i >= lineInfo.lastVisibleCharacterIndex);
										if (flag187)
										{
											bool flag188 = flag158 || character == '\u200b';
											float num104;
											if (flag188)
											{
												int lastVisibleCharacterIndex = lineInfo.lastVisibleCharacterIndex;
												zero2 = new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex].topRight.x, num93, 0f);
												num104 = textInfo.textElementInfo[lastVisibleCharacterIndex].scale;
											}
											else
											{
												zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num93, 0f);
												num104 = textInfo.textElementInfo[i].scale;
											}
											flag4 = false;
											this.DrawUnderlineMesh(zero, zero2, num91, num104, num92, num90, color2, generationSettings, textInfo);
											num92 = 0f;
											num90 = 0f;
											num93 = 32767f;
										}
										else
										{
											bool flag189 = flag4 && !flag180;
											if (flag189)
											{
												flag4 = false;
												zero2 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, num93, 0f);
												float num104 = textInfo.textElementInfo[i - 1].scale;
												this.DrawUnderlineMesh(zero, zero2, num91, num104, num92, num90, color2, generationSettings, textInfo);
												num92 = 0f;
												num90 = 0f;
												num93 = 32767f;
											}
											else
											{
												bool flag190 = flag4 && i < this.m_CharacterCount - 1 && !ColorUtilities.CompareColors(color2, textInfo.textElementInfo[i + 1].underlineColor);
												if (flag190)
												{
													flag4 = false;
													zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num93, 0f);
													float num104 = textInfo.textElementInfo[i].scale;
													this.DrawUnderlineMesh(zero, zero2, num91, num104, num92, num90, color2, generationSettings, textInfo);
													num92 = 0f;
													num90 = 0f;
													num93 = 32767f;
												}
											}
										}
									}
								}
								else
								{
									bool flag191 = flag4;
									if (flag191)
									{
										flag4 = false;
										zero2 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, num93, 0f);
										float num104 = textInfo.textElementInfo[i - 1].scale;
										this.DrawUnderlineMesh(zero, zero2, num91, num104, num92, num90, color2, generationSettings, textInfo);
										num92 = 0f;
										num90 = 0f;
										num93 = 32767f;
									}
								}
								bool flag192 = (textInfo.textElementInfo[i].style & FontStyles.Strikethrough) == FontStyles.Strikethrough;
								float strikethroughOffset = fontAsset.faceInfo.strikethroughOffset;
								bool flag193 = flag192;
								if (flag193)
								{
									bool flag194 = true;
									textInfo.textElementInfo[i].strikethroughVertexIndex = this.m_MaterialReferences[this.m_Underline.materialIndex].referenceCount * 4;
									bool flag195 = i > generationSettings.maxVisibleCharacters || lineNumber > generationSettings.maxVisibleLines || (generationSettings.overflowMode == TextOverflowMode.Page && textInfo.textElementInfo[i].pageNumber + 1 != generationSettings.pageToDisplay);
									if (flag195)
									{
										flag194 = false;
									}
									bool flag196 = !flag5 && flag194 && i <= lineInfo.lastVisibleCharacterIndex && character != '\n' && character != '\v' && character != '\r';
									if (flag196)
									{
										bool flag197 = i == lineInfo.lastVisibleCharacterIndex && char.IsSeparator(character);
										if (!flag197)
										{
											flag5 = true;
											num95 = textInfo.textElementInfo[i].pointSize;
											num96 = textInfo.textElementInfo[i].scale;
											zero3 = new Vector3(textInfo.textElementInfo[i].bottomLeft.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num96, 0f);
											color3 = textInfo.textElementInfo[i].strikethroughColor;
											num97 = textInfo.textElementInfo[i].baseLine;
										}
									}
									bool flag198 = flag5 && this.m_CharacterCount == 1;
									if (flag198)
									{
										flag5 = false;
										zero4 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num96, 0f);
										this.DrawUnderlineMesh(zero3, zero4, num96, num96, num96, num89, color3, generationSettings, textInfo);
									}
									else
									{
										bool flag199 = flag5 && i == lineInfo.lastCharacterIndex;
										if (flag199)
										{
											bool flag200 = flag158 || character == '\u200b';
											if (flag200)
											{
												int lastVisibleCharacterIndex2 = lineInfo.lastVisibleCharacterIndex;
												zero4 = new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex2].topRight.x, textInfo.textElementInfo[lastVisibleCharacterIndex2].baseLine + strikethroughOffset * num96, 0f);
											}
											else
											{
												zero4 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num96, 0f);
											}
											flag5 = false;
											this.DrawUnderlineMesh(zero3, zero4, num96, num96, num96, num89, color3, generationSettings, textInfo);
										}
										else
										{
											bool flag201 = flag5 && i < this.m_CharacterCount && (textInfo.textElementInfo[i + 1].pointSize != num95 || !TextGeneratorUtilities.Approximately(textInfo.textElementInfo[i + 1].baseLine + vector12.y, num97));
											if (flag201)
											{
												flag5 = false;
												int lastVisibleCharacterIndex3 = lineInfo.lastVisibleCharacterIndex;
												bool flag202 = i > lastVisibleCharacterIndex3;
												if (flag202)
												{
													zero4 = new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex3].topRight.x, textInfo.textElementInfo[lastVisibleCharacterIndex3].baseLine + strikethroughOffset * num96, 0f);
												}
												else
												{
													zero4 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num96, 0f);
												}
												this.DrawUnderlineMesh(zero3, zero4, num96, num96, num96, num89, color3, generationSettings, textInfo);
											}
											else
											{
												bool flag203 = flag5 && i < this.m_CharacterCount && fontAsset.GetInstanceID() != textElementInfo[i + 1].fontAsset.GetInstanceID();
												if (flag203)
												{
													flag5 = false;
													zero4 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num96, 0f);
													this.DrawUnderlineMesh(zero3, zero4, num96, num96, num96, num89, color3, generationSettings, textInfo);
												}
												else
												{
													bool flag204 = flag5 && !flag194;
													if (flag204)
													{
														flag5 = false;
														zero4 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, textInfo.textElementInfo[i - 1].baseLine + strikethroughOffset * num96, 0f);
														this.DrawUnderlineMesh(zero3, zero4, num96, num96, num96, num89, color3, generationSettings, textInfo);
													}
												}
											}
										}
									}
								}
								else
								{
									bool flag205 = flag5;
									if (flag205)
									{
										flag5 = false;
										zero4 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, textInfo.textElementInfo[i - 1].baseLine + strikethroughOffset * num96, 0f);
										this.DrawUnderlineMesh(zero3, zero4, num96, num96, num96, num89, color3, generationSettings, textInfo);
									}
								}
								bool flag206 = (textInfo.textElementInfo[i].style & FontStyles.Highlight) == FontStyles.Highlight;
								bool flag207 = flag206;
								if (flag207)
								{
									bool flag208 = true;
									int pageNumber2 = textInfo.textElementInfo[i].pageNumber;
									bool flag209 = i > generationSettings.maxVisibleCharacters || lineNumber > generationSettings.maxVisibleLines || (generationSettings.overflowMode == TextOverflowMode.Page && pageNumber2 + 1 != generationSettings.pageToDisplay);
									if (flag209)
									{
										flag208 = false;
									}
									bool flag210 = !flag6 && flag208 && i <= lineInfo.lastVisibleCharacterIndex && character != '\n' && character != '\v' && character != '\r';
									if (flag210)
									{
										bool flag211 = i == lineInfo.lastVisibleCharacterIndex && char.IsSeparator(character);
										if (!flag211)
										{
											flag6 = true;
											vector = TextGeneratorUtilities.largePositiveVector2;
											vector2 = TextGeneratorUtilities.largeNegativeVector2;
											highlightState = textInfo.textElementInfo[i].highlightState;
										}
									}
									bool flag212 = flag6;
									if (flag212)
									{
										TextElementInfo textElementInfo2 = textInfo.textElementInfo[i];
										HighlightState highlightState2 = textElementInfo2.highlightState;
										bool flag213 = false;
										bool flag214 = highlightState != highlightState2;
										if (flag214)
										{
											bool flag215 = flag158;
											if (flag215)
											{
												vector2.x = (vector2.x - highlightState.padding.right + textElementInfo2.origin) / 2f;
											}
											else
											{
												vector2.x = (vector2.x - highlightState.padding.right + textElementInfo2.bottomLeft.x) / 2f;
											}
											vector.y = Mathf.Min(vector.y, textElementInfo2.descender);
											vector2.y = Mathf.Max(vector2.y, textElementInfo2.ascender);
											this.DrawTextHighlight(vector, vector2, highlightState.color, generationSettings, textInfo);
											flag6 = true;
											vector = new Vector2(vector2.x, textElementInfo2.descender - highlightState2.padding.bottom);
											bool flag216 = flag158;
											if (flag216)
											{
												vector2 = new Vector2(textElementInfo2.xAdvance + highlightState2.padding.right, textElementInfo2.ascender + highlightState2.padding.top);
											}
											else
											{
												vector2 = new Vector2(textElementInfo2.topRight.x + highlightState2.padding.right, textElementInfo2.ascender + highlightState2.padding.top);
											}
											highlightState = highlightState2;
											flag213 = true;
										}
										bool flag217 = !flag213;
										if (flag217)
										{
											bool flag218 = flag158;
											if (flag218)
											{
												vector.x = Mathf.Min(vector.x, textElementInfo2.origin - highlightState.padding.left);
												vector2.x = Mathf.Max(vector2.x, textElementInfo2.xAdvance + highlightState.padding.right);
											}
											else
											{
												vector.x = Mathf.Min(vector.x, textElementInfo2.bottomLeft.x - highlightState.padding.left);
												vector2.x = Mathf.Max(vector2.x, textElementInfo2.topRight.x + highlightState.padding.right);
											}
											vector.y = Mathf.Min(vector.y, textElementInfo2.descender - highlightState.padding.bottom);
											vector2.y = Mathf.Max(vector2.y, textElementInfo2.ascender + highlightState.padding.top);
										}
									}
									bool flag219 = flag6 && this.m_CharacterCount == 1;
									if (flag219)
									{
										flag6 = false;
										this.DrawTextHighlight(vector, vector2, highlightState.color, generationSettings, textInfo);
									}
									else
									{
										bool flag220 = flag6 && (i == lineInfo.lastCharacterIndex || i >= lineInfo.lastVisibleCharacterIndex);
										if (flag220)
										{
											flag6 = false;
											this.DrawTextHighlight(vector, vector2, highlightState.color, generationSettings, textInfo);
										}
										else
										{
											bool flag221 = flag6 && !flag208;
											if (flag221)
											{
												flag6 = false;
												this.DrawTextHighlight(vector, vector2, highlightState.color, generationSettings, textInfo);
											}
										}
									}
								}
								else
								{
									bool flag222 = flag6;
									if (flag222)
									{
										flag6 = false;
										this.DrawTextHighlight(vector, vector2, highlightState.color, generationSettings, textInfo);
									}
								}
								num87 = lineNumber;
								num38 = i;
								i = num38 + 1;
								continue;
								IL_501A:
								bool flag223 = !generationSettings.isRightToLeft;
								if (flag223)
								{
									vector11 = new Vector3(0f + lineInfo.marginLeft, 0f, 0f);
								}
								else
								{
									vector11 = new Vector3(0f - lineInfo.maxAdvance, 0f, 0f);
								}
								goto IL_55B9;
								IL_507C:
								vector11 = new Vector3(lineInfo.marginLeft + lineInfo.width / 2f - lineInfo.maxAdvance / 2f, 0f, 0f);
								goto IL_55B9;
								IL_50C1:
								vector11 = new Vector3(lineInfo.marginLeft + lineInfo.width / 2f - (lineInfo.lineExtents.min.x + lineInfo.lineExtents.max.x) / 2f, 0f, 0f);
								goto IL_55B9;
								IL_5126:
								bool flag224 = !generationSettings.isRightToLeft;
								if (flag224)
								{
									vector11 = new Vector3(lineInfo.marginLeft + lineInfo.width - lineInfo.maxAdvance, 0f, 0f);
								}
								else
								{
									vector11 = new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f);
								}
								goto IL_55B9;
								IL_51A0:
								bool flag225 = i > lineInfo.lastVisibleCharacterIndex || character == '\n' || character == '\u00ad' || character == '\u200b' || character == '\u2060' || character == '\u0003';
								if (flag225)
								{
									goto IL_55B9;
								}
								char character2 = textElementInfo[lineInfo.lastCharacterIndex].character;
								bool flag226 = (alignment & (TextAlignment)16) == (TextAlignment)16;
								bool flag227 = (!char.IsControl(character2) && lineNumber < this.m_LineNumber) || flag226 || lineInfo.maxAdvance > lineInfo.width;
								if (flag227)
								{
									bool flag228 = lineNumber != num87 || i == 0 || i == generationSettings.firstVisibleCharacter;
									if (flag228)
									{
										bool flag229 = !generationSettings.isRightToLeft;
										if (flag229)
										{
											vector11 = new Vector3(lineInfo.marginLeft, 0f, 0f);
										}
										else
										{
											vector11 = new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f);
										}
										bool flag230 = char.IsSeparator(character);
										flag156 = flag230;
									}
									else
									{
										float num105 = ((!generationSettings.isRightToLeft) ? (lineInfo.width - lineInfo.maxAdvance) : (lineInfo.width + lineInfo.maxAdvance));
										int num106 = lineInfo.visibleCharacterCount - 1 + lineInfo.controlCharacterCount;
										int num107 = lineInfo.spaceCount - lineInfo.controlCharacterCount;
										bool flag231 = flag156;
										if (flag231)
										{
											num107--;
											num106++;
										}
										float num108 = ((num107 > 0) ? generationSettings.wordWrappingRatio : 1f);
										bool flag232 = num107 < 1;
										if (flag232)
										{
											num107 = 1;
										}
										bool flag233 = character != '\u00a0' && (character == '\t' || char.IsSeparator(character));
										if (flag233)
										{
											bool flag234 = !generationSettings.isRightToLeft;
											if (flag234)
											{
												vector11 += new Vector3(num105 * (1f - num108) / (float)num107, 0f, 0f);
											}
											else
											{
												vector11 -= new Vector3(num105 * (1f - num108) / (float)num107, 0f, 0f);
											}
										}
										else
										{
											bool flag235 = !generationSettings.isRightToLeft;
											if (flag235)
											{
												vector11 += new Vector3(num105 * num108 / (float)num106, 0f, 0f);
											}
											else
											{
												vector11 -= new Vector3(num105 * num108 / (float)num106, 0f, 0f);
											}
										}
									}
								}
								else
								{
									bool flag236 = !generationSettings.isRightToLeft;
									if (flag236)
									{
										vector11 = new Vector3(lineInfo.marginLeft, 0f, 0f);
									}
									else
									{
										vector11 = new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f);
									}
								}
								goto IL_55B9;
							}
							textInfo.characterCount = this.m_CharacterCount;
							textInfo.spriteCount = this.m_SpriteCount;
							textInfo.lineCount = num86;
							textInfo.wordCount = ((num85 != 0 && this.m_CharacterCount > 0) ? num85 : 1);
							textInfo.pageCount = this.m_PageNumber + 1;
							for (int j = 1; j < textInfo.materialCount; j = num38 + 1)
							{
								textInfo.meshInfo[j].ClearUnusedVertices();
								bool flag237 = generationSettings.geometrySortingOrder > VertexSortingOrder.Normal;
								if (flag237)
								{
									textInfo.meshInfo[j].SortGeometry(VertexSortingOrder.Reverse);
								}
								num38 = j;
							}
						}
					}
				}
			}
		}

		private void SaveWordWrappingState(ref WordWrapState state, int index, int count, TextInfo textInfo)
		{
			state.currentFontAsset = this.m_CurrentFontAsset;
			state.currentSpriteAsset = this.m_CurrentSpriteAsset;
			state.currentMaterial = this.m_CurrentMaterial;
			state.currentMaterialIndex = this.m_CurrentMaterialIndex;
			state.previousWordBreak = index;
			state.totalCharacterCount = count;
			state.visibleCharacterCount = this.m_LineVisibleCharacterCount;
			state.visibleSpaceCount = this.m_LineVisibleSpaceCount;
			state.visibleLinkCount = textInfo.linkCount;
			state.firstCharacterIndex = this.m_FirstCharacterOfLine;
			state.firstVisibleCharacterIndex = this.m_FirstVisibleCharacterOfLine;
			state.lastVisibleCharIndex = this.m_LastVisibleCharacterOfLine;
			state.fontStyle = this.m_FontStyleInternal;
			state.italicAngle = this.m_ItalicAngle;
			state.fontScaleMultiplier = this.m_FontScaleMultiplier;
			state.currentFontSize = this.m_CurrentFontSize;
			state.xAdvance = this.m_XAdvance;
			state.maxCapHeight = this.m_MaxCapHeight;
			state.maxAscender = this.m_MaxAscender;
			state.maxDescender = this.m_MaxDescender;
			state.maxLineAscender = this.m_MaxLineAscender;
			state.maxLineDescender = this.m_MaxLineDescender;
			state.startOfLineAscender = this.m_StartOfLineAscender;
			state.preferredWidth = this.m_PreferredWidth;
			state.preferredHeight = this.m_PreferredHeight;
			state.meshExtents = this.m_MeshExtents;
			state.pageAscender = this.m_PageAscender;
			state.lineNumber = this.m_LineNumber;
			state.lineOffset = this.m_LineOffset;
			state.baselineOffset = this.m_BaselineOffset;
			state.isDrivenLineSpacing = this.m_IsDrivenLineSpacing;
			state.vertexColor = this.m_HtmlColor;
			state.underlineColor = this.m_UnderlineColor;
			state.strikethroughColor = this.m_StrikethroughColor;
			state.highlightColor = this.m_HighlightColor;
			state.highlightState = this.m_HighlightState;
			state.isNonBreakingSpace = this.m_IsNonBreakingSpace;
			state.tagNoParsing = this.m_TagNoParsing;
			state.fxScale = this.m_FXScale;
			state.fxRotation = this.m_FXRotation;
			state.basicStyleStack = this.m_FontStyleStack;
			state.italicAngleStack = this.m_ItalicAngleStack;
			state.colorStack = this.m_ColorStack;
			state.underlineColorStack = this.m_UnderlineColorStack;
			state.strikethroughColorStack = this.m_StrikethroughColorStack;
			state.highlightColorStack = this.m_HighlightColorStack;
			state.colorGradientStack = this.m_ColorGradientStack;
			state.highlightStateStack = this.m_HighlightStateStack;
			state.sizeStack = this.m_SizeStack;
			state.indentStack = this.m_IndentStack;
			state.fontWeightStack = this.m_FontWeightStack;
			state.styleStack = this.m_StyleStack;
			state.baselineStack = this.m_BaselineOffsetStack;
			state.actionStack = this.m_ActionStack;
			state.materialReferenceStack = this.m_MaterialReferenceStack;
			state.lineJustificationStack = this.m_LineJustificationStack;
			state.lastBaseGlyphIndex = this.m_LastBaseGlyphIndex;
			state.spriteAnimationId = this.m_SpriteAnimationId;
			bool flag = this.m_LineNumber < textInfo.lineInfo.Length;
			if (flag)
			{
				state.lineInfo = textInfo.lineInfo[this.m_LineNumber];
			}
		}

		protected int RestoreWordWrappingState(ref WordWrapState state, TextInfo textInfo)
		{
			int previousWordBreak = state.previousWordBreak;
			this.m_CurrentFontAsset = state.currentFontAsset;
			this.m_CurrentSpriteAsset = state.currentSpriteAsset;
			this.m_CurrentMaterial = state.currentMaterial;
			this.m_CurrentMaterialIndex = state.currentMaterialIndex;
			this.m_CharacterCount = state.totalCharacterCount + 1;
			this.m_LineVisibleCharacterCount = state.visibleCharacterCount;
			this.m_LineVisibleSpaceCount = state.visibleSpaceCount;
			textInfo.linkCount = state.visibleLinkCount;
			this.m_FirstCharacterOfLine = state.firstCharacterIndex;
			this.m_FirstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			this.m_LastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			this.m_FontStyleInternal = state.fontStyle;
			this.m_ItalicAngle = state.italicAngle;
			this.m_FontScaleMultiplier = state.fontScaleMultiplier;
			this.m_CurrentFontSize = state.currentFontSize;
			this.m_XAdvance = state.xAdvance;
			this.m_MaxCapHeight = state.maxCapHeight;
			this.m_MaxAscender = state.maxAscender;
			this.m_MaxDescender = state.maxDescender;
			this.m_MaxLineAscender = state.maxLineAscender;
			this.m_MaxLineDescender = state.maxLineDescender;
			this.m_StartOfLineAscender = state.startOfLineAscender;
			this.m_PreferredWidth = state.preferredWidth;
			this.m_PreferredHeight = state.preferredHeight;
			this.m_MeshExtents = state.meshExtents;
			this.m_PageAscender = state.pageAscender;
			this.m_LineNumber = state.lineNumber;
			this.m_LineOffset = state.lineOffset;
			this.m_BaselineOffset = state.baselineOffset;
			this.m_IsDrivenLineSpacing = state.isDrivenLineSpacing;
			this.m_HtmlColor = state.vertexColor;
			this.m_UnderlineColor = state.underlineColor;
			this.m_StrikethroughColor = state.strikethroughColor;
			this.m_HighlightColor = state.highlightColor;
			this.m_HighlightState = state.highlightState;
			this.m_IsNonBreakingSpace = state.isNonBreakingSpace;
			this.m_TagNoParsing = state.tagNoParsing;
			this.m_FXScale = state.fxScale;
			this.m_FXRotation = state.fxRotation;
			this.m_FontStyleStack = state.basicStyleStack;
			this.m_ItalicAngleStack = state.italicAngleStack;
			this.m_ColorStack = state.colorStack;
			this.m_UnderlineColorStack = state.underlineColorStack;
			this.m_StrikethroughColorStack = state.strikethroughColorStack;
			this.m_HighlightColorStack = state.highlightColorStack;
			this.m_ColorGradientStack = state.colorGradientStack;
			this.m_HighlightStateStack = state.highlightStateStack;
			this.m_SizeStack = state.sizeStack;
			this.m_IndentStack = state.indentStack;
			this.m_FontWeightStack = state.fontWeightStack;
			this.m_StyleStack = state.styleStack;
			this.m_BaselineOffsetStack = state.baselineStack;
			this.m_ActionStack = state.actionStack;
			this.m_MaterialReferenceStack = state.materialReferenceStack;
			this.m_LineJustificationStack = state.lineJustificationStack;
			this.m_LastBaseGlyphIndex = state.lastBaseGlyphIndex;
			this.m_SpriteAnimationId = state.spriteAnimationId;
			bool flag = this.m_LineNumber < textInfo.lineInfo.Length;
			if (flag)
			{
				textInfo.lineInfo[this.m_LineNumber] = state.lineInfo;
			}
			return previousWordBreak;
		}

		protected bool ValidateHtmlTag(TextProcessingElement[] chars, int startIndex, out int endIndex, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			TextSettings textSettings = generationSettings.textSettings;
			int num = 0;
			byte b = 0;
			int num2 = 0;
			this.ClearMarkupTagAttributes();
			TagValueType tagValueType = TagValueType.None;
			TagUnitType tagUnitType = TagUnitType.Pixels;
			endIndex = startIndex;
			bool flag = false;
			bool flag2 = false;
			int num3 = startIndex;
			while (num3 < chars.Length && chars[num3].unicode != 0U && num < this.m_HtmlTag.Length && chars[num3].unicode != 60U)
			{
				uint unicode = chars[num3].unicode;
				bool flag3 = unicode == 62U;
				if (flag3)
				{
					flag2 = true;
					endIndex = num3;
					this.m_HtmlTag[num] = '\0';
					break;
				}
				this.m_HtmlTag[num] = (char)unicode;
				num++;
				bool flag4 = b == 1;
				if (flag4)
				{
					bool flag5 = tagValueType == TagValueType.None;
					if (flag5)
					{
						bool flag6 = unicode == 43U || unicode == 45U || unicode == 46U || (unicode >= 48U && unicode <= 57U);
						if (flag6)
						{
							tagUnitType = TagUnitType.Pixels;
							tagValueType = (this.m_XmlAttribute[num2].valueType = TagValueType.NumericalValue);
							this.m_XmlAttribute[num2].valueStartIndex = num - 1;
							RichTextTagAttribute[] xmlAttribute = this.m_XmlAttribute;
							int num4 = num2;
							xmlAttribute[num4].valueLength = xmlAttribute[num4].valueLength + 1;
						}
						else
						{
							bool flag7 = unicode == 35U;
							if (flag7)
							{
								tagUnitType = TagUnitType.Pixels;
								tagValueType = (this.m_XmlAttribute[num2].valueType = TagValueType.ColorValue);
								this.m_XmlAttribute[num2].valueStartIndex = num - 1;
								RichTextTagAttribute[] xmlAttribute2 = this.m_XmlAttribute;
								int num5 = num2;
								xmlAttribute2[num5].valueLength = xmlAttribute2[num5].valueLength + 1;
							}
							else
							{
								bool flag8 = unicode == 34U;
								if (flag8)
								{
									tagUnitType = TagUnitType.Pixels;
									tagValueType = (this.m_XmlAttribute[num2].valueType = TagValueType.StringValue);
									this.m_XmlAttribute[num2].valueStartIndex = num;
								}
								else
								{
									tagUnitType = TagUnitType.Pixels;
									tagValueType = (this.m_XmlAttribute[num2].valueType = TagValueType.StringValue);
									this.m_XmlAttribute[num2].valueStartIndex = num - 1;
									this.m_XmlAttribute[num2].valueHashCode = ((this.m_XmlAttribute[num2].valueHashCode << 5) + this.m_XmlAttribute[num2].valueHashCode) ^ (int)TextGeneratorUtilities.ToUpperFast((char)unicode);
									RichTextTagAttribute[] xmlAttribute3 = this.m_XmlAttribute;
									int num6 = num2;
									xmlAttribute3[num6].valueLength = xmlAttribute3[num6].valueLength + 1;
								}
							}
						}
					}
					else
					{
						bool flag9 = tagValueType == TagValueType.NumericalValue;
						if (flag9)
						{
							bool flag10 = unicode == 112U || unicode == 101U || unicode == 37U || unicode == 32U;
							if (flag10)
							{
								b = 2;
								tagValueType = TagValueType.None;
								uint num7 = unicode;
								uint num8 = num7;
								if (num8 != 37U)
								{
									if (num8 != 101U)
									{
										tagUnitType = (this.m_XmlAttribute[num2].unitType = TagUnitType.Pixels);
									}
									else
									{
										tagUnitType = (this.m_XmlAttribute[num2].unitType = TagUnitType.FontUnits);
									}
								}
								else
								{
									tagUnitType = (this.m_XmlAttribute[num2].unitType = TagUnitType.Percentage);
								}
								num2++;
								this.m_XmlAttribute[num2].nameHashCode = 0;
								this.m_XmlAttribute[num2].valueHashCode = 0;
								this.m_XmlAttribute[num2].valueType = TagValueType.None;
								this.m_XmlAttribute[num2].unitType = TagUnitType.Pixels;
								this.m_XmlAttribute[num2].valueStartIndex = 0;
								this.m_XmlAttribute[num2].valueLength = 0;
							}
							else
							{
								RichTextTagAttribute[] xmlAttribute4 = this.m_XmlAttribute;
								int num9 = num2;
								xmlAttribute4[num9].valueLength = xmlAttribute4[num9].valueLength + 1;
							}
						}
						else
						{
							bool flag11 = tagValueType == TagValueType.ColorValue;
							if (flag11)
							{
								bool flag12 = unicode != 32U;
								if (flag12)
								{
									RichTextTagAttribute[] xmlAttribute5 = this.m_XmlAttribute;
									int num10 = num2;
									xmlAttribute5[num10].valueLength = xmlAttribute5[num10].valueLength + 1;
								}
								else
								{
									b = 2;
									tagValueType = TagValueType.None;
									tagUnitType = TagUnitType.Pixels;
									num2++;
									this.m_XmlAttribute[num2].nameHashCode = 0;
									this.m_XmlAttribute[num2].valueType = TagValueType.None;
									this.m_XmlAttribute[num2].unitType = TagUnitType.Pixels;
									this.m_XmlAttribute[num2].valueHashCode = 0;
									this.m_XmlAttribute[num2].valueStartIndex = 0;
									this.m_XmlAttribute[num2].valueLength = 0;
								}
							}
							else
							{
								bool flag13 = tagValueType == TagValueType.StringValue;
								if (flag13)
								{
									bool flag14 = unicode != 34U;
									if (flag14)
									{
										this.m_XmlAttribute[num2].valueHashCode = ((this.m_XmlAttribute[num2].valueHashCode << 5) + this.m_XmlAttribute[num2].valueHashCode) ^ (int)TextGeneratorUtilities.ToUpperFast((char)unicode);
										RichTextTagAttribute[] xmlAttribute6 = this.m_XmlAttribute;
										int num11 = num2;
										xmlAttribute6[num11].valueLength = xmlAttribute6[num11].valueLength + 1;
									}
									else
									{
										b = 2;
										tagValueType = TagValueType.None;
										tagUnitType = TagUnitType.Pixels;
										num2++;
										this.m_XmlAttribute[num2].nameHashCode = 0;
										this.m_XmlAttribute[num2].valueType = TagValueType.None;
										this.m_XmlAttribute[num2].unitType = TagUnitType.Pixels;
										this.m_XmlAttribute[num2].valueHashCode = 0;
										this.m_XmlAttribute[num2].valueStartIndex = 0;
										this.m_XmlAttribute[num2].valueLength = 0;
									}
								}
							}
						}
					}
				}
				bool flag15 = unicode == 61U;
				if (flag15)
				{
					b = 1;
				}
				bool flag16 = b == 0 && unicode == 32U;
				if (flag16)
				{
					bool flag17 = flag;
					if (flag17)
					{
						return false;
					}
					flag = true;
					b = 2;
					tagValueType = TagValueType.None;
					tagUnitType = TagUnitType.Pixels;
					num2++;
					this.m_XmlAttribute[num2].nameHashCode = 0;
					this.m_XmlAttribute[num2].valueType = TagValueType.None;
					this.m_XmlAttribute[num2].unitType = TagUnitType.Pixels;
					this.m_XmlAttribute[num2].valueHashCode = 0;
					this.m_XmlAttribute[num2].valueStartIndex = 0;
					this.m_XmlAttribute[num2].valueLength = 0;
				}
				bool flag18 = b == 0;
				if (flag18)
				{
					this.m_XmlAttribute[num2].nameHashCode = ((this.m_XmlAttribute[num2].nameHashCode << 5) + this.m_XmlAttribute[num2].nameHashCode) ^ (int)TextGeneratorUtilities.ToUpperFast((char)unicode);
				}
				bool flag19 = b == 2 && unicode == 32U;
				if (flag19)
				{
					b = 0;
				}
				num3++;
			}
			bool flag20 = !flag2;
			if (flag20)
			{
				return false;
			}
			bool flag21 = this.m_TagNoParsing && this.m_XmlAttribute[0].nameHashCode != -294095813;
			if (flag21)
			{
				return false;
			}
			bool flag22 = this.m_XmlAttribute[0].nameHashCode == -294095813;
			if (flag22)
			{
				this.m_TagNoParsing = false;
				return true;
			}
			bool flag23 = this.m_HtmlTag[0] == '#' && num == 4;
			if (flag23)
			{
				this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, num);
				this.m_ColorStack.Add(this.m_HtmlColor);
				return true;
			}
			bool flag24 = this.m_HtmlTag[0] == '#' && num == 5;
			if (flag24)
			{
				this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, num);
				this.m_ColorStack.Add(this.m_HtmlColor);
				return true;
			}
			bool flag25 = this.m_HtmlTag[0] == '#' && num == 7;
			if (flag25)
			{
				this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, num);
				this.m_ColorStack.Add(this.m_HtmlColor);
				return true;
			}
			bool flag26 = this.m_HtmlTag[0] == '#' && num == 9;
			if (flag26)
			{
				this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, num);
				this.m_ColorStack.Add(this.m_HtmlColor);
				return true;
			}
			MarkupTag nameHashCode = (MarkupTag)this.m_XmlAttribute[0].nameHashCode;
			MarkupTag markupTag = nameHashCode;
			if (markupTag <= MarkupTag.SLASH_STRIKETHROUGH)
			{
				if (markupTag <= MarkupTag.LINE_INDENT)
				{
					if (markupTag <= MarkupTag.SLASH_INDENT)
					{
						if (markupTag <= MarkupTag.SLASH_MARGIN)
						{
							if (markupTag <= MarkupTag.FONT_WEIGHT)
							{
								if (markupTag == MarkupTag.GRADIENT)
								{
									int valueHashCode = this.m_XmlAttribute[0].valueHashCode;
									TextColorGradient textColorGradient;
									bool flag27 = MaterialReferenceManager.TryGetColorGradientPreset(valueHashCode, out textColorGradient);
									if (flag27)
									{
										this.m_ColorGradientPreset = textColorGradient;
									}
									else
									{
										bool flag28 = textColorGradient == null;
										if (flag28)
										{
											textColorGradient = Resources.Load<TextColorGradient>(textSettings.defaultColorGradientPresetsPath + new string(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength));
										}
										bool flag29 = textColorGradient == null;
										if (flag29)
										{
											return false;
										}
										MaterialReferenceManager.AddColorGradientPreset(valueHashCode, textColorGradient);
										this.m_ColorGradientPreset = textColorGradient;
									}
									this.m_ColorGradientPresetIsTinted = false;
									int num12 = 1;
									while (num12 < this.m_XmlAttribute.Length && this.m_XmlAttribute[num12].nameHashCode != 0)
									{
										int nameHashCode2 = this.m_XmlAttribute[num12].nameHashCode;
										MarkupTag markupTag2 = (MarkupTag)nameHashCode2;
										MarkupTag markupTag3 = markupTag2;
										if (markupTag3 == MarkupTag.TINT)
										{
											this.m_ColorGradientPresetIsTinted = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[num12].valueStartIndex, this.m_XmlAttribute[num12].valueLength) != 0f;
										}
										num12++;
									}
									this.m_ColorGradientStack.Add(this.m_ColorGradientPreset);
									return true;
								}
								if (markupTag != MarkupTag.FONT_WEIGHT)
								{
									goto IL_40F4;
								}
								float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								bool flag30 = num13 == -32768f;
								if (flag30)
								{
									return false;
								}
								int num14 = (int)num13;
								int num15 = num14;
								if (num15 <= 400)
								{
									if (num15 <= 200)
									{
										if (num15 != 100)
										{
											if (num15 == 200)
											{
												this.m_FontWeightInternal = TextFontWeight.ExtraLight;
											}
										}
										else
										{
											this.m_FontWeightInternal = TextFontWeight.Thin;
										}
									}
									else if (num15 != 300)
									{
										if (num15 == 400)
										{
											this.m_FontWeightInternal = TextFontWeight.Regular;
										}
									}
									else
									{
										this.m_FontWeightInternal = TextFontWeight.Light;
									}
								}
								else if (num15 <= 600)
								{
									if (num15 != 500)
									{
										if (num15 == 600)
										{
											this.m_FontWeightInternal = TextFontWeight.SemiBold;
										}
									}
									else
									{
										this.m_FontWeightInternal = TextFontWeight.Medium;
									}
								}
								else if (num15 != 700)
								{
									if (num15 != 800)
									{
										if (num15 == 900)
										{
											this.m_FontWeightInternal = TextFontWeight.Black;
										}
									}
									else
									{
										this.m_FontWeightInternal = TextFontWeight.Heavy;
									}
								}
								else
								{
									this.m_FontWeightInternal = TextFontWeight.Bold;
								}
								this.m_FontWeightStack.Add(this.m_FontWeightInternal);
								return true;
							}
							else
							{
								if (markupTag == MarkupTag.SLASH_GRADIENT)
								{
									this.m_ColorGradientPreset = this.m_ColorGradientStack.Remove();
									return true;
								}
								if (markupTag == MarkupTag.ACTION)
								{
									int valueHashCode2 = this.m_XmlAttribute[0].valueHashCode;
									bool isTextLayoutPhase = this.m_isTextLayoutPhase;
									if (isTextLayoutPhase)
									{
										this.m_ActionStack.Add(valueHashCode2);
										Debug.Log("Action ID: [" + valueHashCode2.ToString() + "] First character index: " + this.m_CharacterCount.ToString());
									}
									return true;
								}
								if (markupTag != MarkupTag.SLASH_MARGIN)
								{
									goto IL_40F4;
								}
								this.m_MarginLeft = 0f;
								this.m_MarginRight = 0f;
								return true;
							}
						}
						else if (markupTag <= MarkupTag.CHARACTER_SPACE)
						{
							if (markupTag == MarkupTag.SLASH_MONOSPACE)
							{
								this.m_MonoSpacing = 0f;
								return true;
							}
							if (markupTag != MarkupTag.CHARACTER_SPACE)
							{
								goto IL_40F4;
							}
							float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag31 = num13 == -32768f;
							if (flag31)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_CSpacing = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
								break;
							case TagUnitType.FontUnits:
								this.m_CSpacing = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
								break;
							case TagUnitType.Percentage:
								return false;
							}
							return true;
						}
						else if (markupTag != MarkupTag.INDENT)
						{
							if (markupTag == MarkupTag.LOWERCASE)
							{
								this.m_FontStyleInternal |= FontStyles.LowerCase;
								this.m_FontStyleStack.Add(FontStyles.LowerCase);
								return true;
							}
							if (markupTag != MarkupTag.SLASH_INDENT)
							{
								goto IL_40F4;
							}
							this.m_TagIndent = this.m_IndentStack.Remove();
							return true;
						}
						else
						{
							float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag32 = num13 == -32768f;
							if (flag32)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_TagIndent = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
								break;
							case TagUnitType.FontUnits:
								this.m_TagIndent = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
								break;
							case TagUnitType.Percentage:
								this.m_TagIndent = this.m_MarginWidth * num13 / 100f;
								break;
							}
							this.m_IndentStack.Add(this.m_TagIndent);
							this.m_XAdvance = this.m_TagIndent;
							return true;
						}
					}
					else if (markupTag <= MarkupTag.SLASH_ACTION)
					{
						if (markupTag <= MarkupTag.SLASH_CHARACTER_SPACE)
						{
							if (markupTag == MarkupTag.SLASH_LOWERCASE)
							{
								bool flag33 = (generationSettings.fontStyle & FontStyles.LowerCase) != FontStyles.LowerCase;
								if (flag33)
								{
									bool flag34 = this.m_FontStyleStack.Remove(FontStyles.LowerCase) == 0;
									if (flag34)
									{
										this.m_FontStyleInternal &= ~FontStyles.LowerCase;
									}
								}
								return true;
							}
							if (markupTag != MarkupTag.SLASH_CHARACTER_SPACE)
							{
								goto IL_40F4;
							}
							bool flag35 = !this.m_isTextLayoutPhase;
							if (flag35)
							{
								return true;
							}
							bool flag36 = this.m_CharacterCount > 0;
							if (flag36)
							{
								this.m_XAdvance -= this.m_CSpacing;
								textInfo.textElementInfo[this.m_CharacterCount - 1].xAdvance = this.m_XAdvance;
							}
							this.m_CSpacing = 0f;
							return true;
						}
						else if (markupTag != MarkupTag.MARGIN)
						{
							if (markupTag != MarkupTag.MONOSPACE)
							{
								if (markupTag != MarkupTag.SLASH_ACTION)
								{
									goto IL_40F4;
								}
								bool isTextLayoutPhase2 = this.m_isTextLayoutPhase;
								if (isTextLayoutPhase2)
								{
									Debug.Log("Action ID: [" + this.m_ActionStack.CurrentItem().ToString() + "] Last character index: " + (this.m_CharacterCount - 1).ToString());
								}
								this.m_ActionStack.Remove();
								return true;
							}
							else
							{
								float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								bool flag37 = num13 == -32768f;
								if (flag37)
								{
									return false;
								}
								switch (tagUnitType)
								{
								case TagUnitType.Pixels:
									this.m_MonoSpacing = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
									break;
								case TagUnitType.FontUnits:
									this.m_MonoSpacing = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
									break;
								case TagUnitType.Percentage:
									return false;
								}
								return true;
							}
						}
						else
						{
							TagValueType valueType = this.m_XmlAttribute[0].valueType;
							TagValueType tagValueType2 = valueType;
							float num13;
							if (tagValueType2 == TagValueType.None)
							{
								int num16 = 1;
								while (num16 < this.m_XmlAttribute.Length && this.m_XmlAttribute[num16].nameHashCode != 0)
								{
									int nameHashCode3 = this.m_XmlAttribute[num16].nameHashCode;
									MarkupTag markupTag4 = (MarkupTag)nameHashCode3;
									MarkupTag markupTag5 = markupTag4;
									if (markupTag5 != MarkupTag.LEFT)
									{
										if (markupTag5 == MarkupTag.RIGHT)
										{
											num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[num16].valueStartIndex, this.m_XmlAttribute[num16].valueLength);
											bool flag38 = num13 == -32768f;
											if (flag38)
											{
												return false;
											}
											switch (this.m_XmlAttribute[num16].unitType)
											{
											case TagUnitType.Pixels:
												this.m_MarginRight = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
												break;
											case TagUnitType.FontUnits:
												this.m_MarginRight = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
												break;
											case TagUnitType.Percentage:
												this.m_MarginRight = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num13 / 100f;
												break;
											}
											this.m_MarginRight = ((this.m_MarginRight >= 0f) ? this.m_MarginRight : 0f);
										}
									}
									else
									{
										num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[num16].valueStartIndex, this.m_XmlAttribute[num16].valueLength);
										bool flag39 = num13 == -32768f;
										if (flag39)
										{
											return false;
										}
										switch (this.m_XmlAttribute[num16].unitType)
										{
										case TagUnitType.Pixels:
											this.m_MarginLeft = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
											break;
										case TagUnitType.FontUnits:
											this.m_MarginLeft = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
											break;
										case TagUnitType.Percentage:
											this.m_MarginLeft = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num13 / 100f;
											break;
										}
										this.m_MarginLeft = ((this.m_MarginLeft >= 0f) ? this.m_MarginLeft : 0f);
									}
									num16++;
								}
								return true;
							}
							if (tagValueType2 != TagValueType.NumericalValue)
							{
								return false;
							}
							num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag40 = num13 == -32768f;
							if (flag40)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_MarginLeft = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
								break;
							case TagUnitType.FontUnits:
								this.m_MarginLeft = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
								break;
							case TagUnitType.Percentage:
								this.m_MarginLeft = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num13 / 100f;
								break;
							}
							this.m_MarginLeft = ((this.m_MarginLeft >= 0f) ? this.m_MarginLeft : 0f);
							this.m_MarginRight = this.m_MarginLeft;
							return true;
						}
					}
					else if (markupTag <= MarkupTag.ROTATE)
					{
						if (markupTag == MarkupTag.SLASH_MATERIAL)
						{
							MaterialReference materialReference = this.m_MaterialReferenceStack.Remove();
							this.m_CurrentMaterial = materialReference.material;
							this.m_CurrentMaterialIndex = materialReference.index;
							return true;
						}
						if (markupTag != MarkupTag.ROTATE)
						{
							goto IL_40F4;
						}
						float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag41 = num13 == -32768f;
						if (flag41)
						{
							return false;
						}
						this.m_FXRotation = Quaternion.Euler(0f, 0f, num13);
						return true;
					}
					else if (markupTag != MarkupTag.SPRITE)
					{
						if (markupTag == MarkupTag.SLASH_TABLE)
						{
							return false;
						}
						if (markupTag != MarkupTag.LINE_INDENT)
						{
							goto IL_40F4;
						}
						float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag42 = num13 == -32768f;
						if (flag42)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
							this.m_TagLineIndent = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
							break;
						case TagUnitType.FontUnits:
							this.m_TagLineIndent = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
							break;
						case TagUnitType.Percentage:
							this.m_TagLineIndent = this.m_MarginWidth * num13 / 100f;
							break;
						}
						this.m_XAdvance += this.m_TagLineIndent;
						return true;
					}
					else
					{
						int valueHashCode3 = this.m_XmlAttribute[0].valueHashCode;
						this.m_SpriteIndex = -1;
						bool flag43 = this.m_XmlAttribute[0].valueType == TagValueType.None || this.m_XmlAttribute[0].valueType == TagValueType.NumericalValue;
						if (flag43)
						{
							bool flag44 = generationSettings.spriteAsset != null;
							if (flag44)
							{
								this.m_CurrentSpriteAsset = generationSettings.spriteAsset;
							}
							else
							{
								bool flag45 = textSettings.defaultSpriteAsset != null;
								if (flag45)
								{
									this.m_CurrentSpriteAsset = textSettings.defaultSpriteAsset;
								}
								else
								{
									bool flag46 = this.m_DefaultSpriteAsset != null;
									if (flag46)
									{
										this.m_CurrentSpriteAsset = this.m_DefaultSpriteAsset;
									}
									else
									{
										bool flag47 = this.m_DefaultSpriteAsset == null;
										if (flag47)
										{
											this.m_DefaultSpriteAsset = Resources.Load<SpriteAsset>("Sprite Assets/Default Sprite Asset");
											this.m_CurrentSpriteAsset = this.m_DefaultSpriteAsset;
										}
									}
								}
							}
							bool flag48 = this.m_CurrentSpriteAsset == null;
							if (flag48)
							{
								return false;
							}
						}
						else
						{
							SpriteAsset spriteAsset;
							bool flag49 = MaterialReferenceManager.TryGetSpriteAsset(valueHashCode3, out spriteAsset);
							if (flag49)
							{
								this.m_CurrentSpriteAsset = spriteAsset;
							}
							else
							{
								bool flag50 = spriteAsset == null;
								if (flag50)
								{
									bool flag51 = spriteAsset == null;
									if (flag51)
									{
										spriteAsset = Resources.Load<SpriteAsset>(textSettings.defaultSpriteAssetPath + new string(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength));
									}
								}
								bool flag52 = spriteAsset == null;
								if (flag52)
								{
									return false;
								}
								MaterialReferenceManager.AddSpriteAsset(valueHashCode3, spriteAsset);
								this.m_CurrentSpriteAsset = spriteAsset;
							}
						}
						bool flag53 = this.m_XmlAttribute[0].valueType == TagValueType.NumericalValue;
						if (flag53)
						{
							int num17 = (int)TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag54 = num17 == -32768;
							if (flag54)
							{
								return false;
							}
							bool flag55 = num17 > this.m_CurrentSpriteAsset.spriteCharacterTable.Count - 1;
							if (flag55)
							{
								return false;
							}
							this.m_SpriteIndex = num17;
						}
						this.m_SpriteColor = Color.white;
						this.m_TintSprite = false;
						int num18 = 0;
						while (num18 < this.m_XmlAttribute.Length && this.m_XmlAttribute[num18].nameHashCode != 0)
						{
							int nameHashCode4 = this.m_XmlAttribute[num18].nameHashCode;
							int num19 = 0;
							MarkupTag markupTag6 = (MarkupTag)nameHashCode4;
							MarkupTag markupTag7 = markupTag6;
							if (markupTag7 <= MarkupTag.NAME)
							{
								if (markupTag7 != MarkupTag.ANIM)
								{
									if (markupTag7 != MarkupTag.NAME)
									{
										goto IL_35FD;
									}
									this.m_CurrentSpriteAsset = SpriteAsset.SearchForSpriteByHashCode(this.m_CurrentSpriteAsset, this.m_XmlAttribute[num18].valueHashCode, true, out num19, null);
									bool flag56 = num19 == -1;
									if (flag56)
									{
										return false;
									}
									this.m_SpriteIndex = num19;
								}
								else
								{
									int attributeParameters = TextGeneratorUtilities.GetAttributeParameters(this.m_HtmlTag, this.m_XmlAttribute[num18].valueStartIndex, this.m_XmlAttribute[num18].valueLength, ref this.m_AttributeParameterValues);
									bool flag57 = attributeParameters != 3;
									if (flag57)
									{
										return false;
									}
									this.m_SpriteIndex = (int)this.m_AttributeParameterValues[0];
									bool isTextLayoutPhase3 = this.m_isTextLayoutPhase;
									if (isTextLayoutPhase3)
									{
									}
								}
							}
							else if (markupTag7 != MarkupTag.TINT)
							{
								if (markupTag7 != MarkupTag.COLOR)
								{
									if (markupTag7 != MarkupTag.INDEX)
									{
										goto IL_35FD;
									}
									num19 = (int)TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
									bool flag58 = num19 == -32768;
									if (flag58)
									{
										return false;
									}
									bool flag59 = num19 > this.m_CurrentSpriteAsset.spriteCharacterTable.Count - 1;
									if (flag59)
									{
										return false;
									}
									this.m_SpriteIndex = num19;
								}
								else
								{
									this.m_SpriteColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[num18].valueStartIndex, this.m_XmlAttribute[num18].valueLength);
								}
							}
							else
							{
								this.m_TintSprite = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[num18].valueStartIndex, this.m_XmlAttribute[num18].valueLength) != 0f;
							}
							IL_3619:
							num18++;
							continue;
							IL_35FD:
							bool flag60 = nameHashCode4 != -991527447;
							if (flag60)
							{
								return false;
							}
							goto IL_3619;
						}
						bool flag61 = this.m_SpriteIndex == -1;
						if (flag61)
						{
							return false;
						}
						this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentSpriteAsset.material, this.m_CurrentSpriteAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
						this.m_TextElementType = TextElementType.Sprite;
						return true;
					}
				}
				else
				{
					if (markupTag <= MarkupTag.MARGIN_LEFT)
					{
						if (markupTag <= MarkupTag.SLASH_FONT_WEIGHT)
						{
							if (markupTag <= MarkupTag.SLASH_ALLCAPS)
							{
								if (markupTag != MarkupTag.LINE_HEIGHT)
								{
									if (markupTag != MarkupTag.SLASH_ALLCAPS)
									{
										goto IL_40F4;
									}
								}
								else
								{
									float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
									bool flag62 = num13 == -32768f;
									if (flag62)
									{
										return false;
									}
									switch (tagUnitType)
									{
									case TagUnitType.Pixels:
										this.m_LineHeight = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
										break;
									case TagUnitType.FontUnits:
										this.m_LineHeight = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
										break;
									case TagUnitType.Percentage:
									{
										float num20 = this.m_CurrentFontSize / (float)this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										this.m_LineHeight = generationSettings.fontAsset.faceInfo.lineHeight * num13 / 100f * num20;
										break;
									}
									}
									return true;
								}
							}
							else
							{
								if (markupTag == MarkupTag.SMALLCAPS)
								{
									this.m_FontStyleInternal |= FontStyles.SmallCaps;
									this.m_FontStyleStack.Add(FontStyles.SmallCaps);
									return true;
								}
								if (markupTag == MarkupTag.SLASH_ROTATE)
								{
									this.m_FXRotation = Quaternion.identity;
									return true;
								}
								if (markupTag != MarkupTag.SLASH_FONT_WEIGHT)
								{
									goto IL_40F4;
								}
								this.m_FontWeightStack.Remove();
								bool flag63 = this.m_FontStyleInternal == FontStyles.Bold;
								if (flag63)
								{
									this.m_FontWeightInternal = TextFontWeight.Bold;
								}
								else
								{
									this.m_FontWeightInternal = this.m_FontWeightStack.Peek();
								}
								return true;
							}
						}
						else if (markupTag <= MarkupTag.MARGIN_RIGHT)
						{
							if (markupTag != MarkupTag.SLASH_UPPERCASE)
							{
								if (markupTag != MarkupTag.MARGIN_RIGHT)
								{
									goto IL_40F4;
								}
								float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								bool flag64 = num13 == -32768f;
								if (flag64)
								{
									return false;
								}
								switch (tagUnitType)
								{
								case TagUnitType.Pixels:
									this.m_MarginRight = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
									break;
								case TagUnitType.FontUnits:
									this.m_MarginRight = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
									break;
								case TagUnitType.Percentage:
									this.m_MarginRight = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num13 / 100f;
									break;
								}
								this.m_MarginRight = ((this.m_MarginRight >= 0f) ? this.m_MarginRight : 0f);
								return true;
							}
						}
						else
						{
							if (markupTag == MarkupTag.NO_PARSE)
							{
								this.m_TagNoParsing = true;
								return true;
							}
							if (markupTag == MarkupTag.UPPERCASE)
							{
								goto IL_3701;
							}
							if (markupTag != MarkupTag.MARGIN_LEFT)
							{
								goto IL_40F4;
							}
							float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag65 = num13 == -32768f;
							if (flag65)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_MarginLeft = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
								break;
							case TagUnitType.FontUnits:
								this.m_MarginLeft = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
								break;
							case TagUnitType.Percentage:
								this.m_MarginLeft = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num13 / 100f;
								break;
							}
							this.m_MarginLeft = ((this.m_MarginLeft >= 0f) ? this.m_MarginLeft : 0f);
							return true;
						}
						bool flag66 = (generationSettings.fontStyle & FontStyles.UpperCase) != FontStyles.UpperCase;
						if (flag66)
						{
							bool flag67 = this.m_FontStyleStack.Remove(FontStyles.UpperCase) == 0;
							if (flag67)
							{
								this.m_FontStyleInternal &= ~FontStyles.UpperCase;
							}
						}
						return true;
					}
					if (markupTag <= MarkupTag.STRIKETHROUGH)
					{
						if (markupTag <= MarkupTag.A)
						{
							if (markupTag == MarkupTag.SLASH_VERTICAL_OFFSET)
							{
								this.m_BaselineOffset = 0f;
								return true;
							}
							if (markupTag != MarkupTag.A)
							{
								goto IL_40F4;
							}
							bool flag68 = this.m_isTextLayoutPhase && !this.m_IsCalculatingPreferredValues;
							if (flag68)
							{
								bool flag69 = this.m_XmlAttribute[1].nameHashCode == 2535353;
								if (flag69)
								{
									int linkCount = textInfo.linkCount;
									bool flag70 = linkCount + 1 > textInfo.linkInfo.Length;
									if (flag70)
									{
										TextInfo.Resize<LinkInfo>(ref textInfo.linkInfo, linkCount + 1);
									}
									textInfo.linkInfo[linkCount].hashCode = 2535353;
									textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = this.m_CharacterCount;
									textInfo.linkInfo[linkCount].linkIdFirstCharacterIndex = startIndex + this.m_XmlAttribute[1].valueStartIndex;
									textInfo.linkInfo[linkCount].SetLinkId(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
								}
								textInfo.linkCount++;
							}
							return true;
						}
						else
						{
							if (markupTag == MarkupTag.BOLD)
							{
								this.m_FontStyleInternal |= FontStyles.Bold;
								this.m_FontStyleStack.Add(FontStyles.Bold);
								this.m_FontWeightInternal = TextFontWeight.Bold;
								return true;
							}
							if (markupTag == MarkupTag.ITALIC)
							{
								this.m_FontStyleInternal |= FontStyles.Italic;
								this.m_FontStyleStack.Add(FontStyles.Italic);
								bool flag71 = this.m_XmlAttribute[1].nameHashCode == 75347905;
								if (flag71)
								{
									this.m_ItalicAngle = (int)TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
									bool flag72 = this.m_ItalicAngle < -180 || this.m_ItalicAngle > 180;
									if (flag72)
									{
										return false;
									}
								}
								else
								{
									this.m_ItalicAngle = (int)this.m_CurrentFontAsset.italicStyleSlant;
								}
								this.m_ItalicAngleStack.Add(this.m_ItalicAngle);
								return true;
							}
							if (markupTag != MarkupTag.STRIKETHROUGH)
							{
								goto IL_40F4;
							}
							this.m_FontStyleInternal |= FontStyles.Strikethrough;
							this.m_FontStyleStack.Add(FontStyles.Strikethrough);
							bool flag73 = this.m_XmlAttribute[1].nameHashCode == 81999901;
							if (flag73)
							{
								this.m_StrikethroughColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
								this.m_StrikethroughColor.a = ((this.m_HtmlColor.a < this.m_StrikethroughColor.a) ? this.m_HtmlColor.a : this.m_StrikethroughColor.a);
								textInfo.hasMultipleColors = true;
							}
							else
							{
								this.m_StrikethroughColor = this.m_HtmlColor;
							}
							this.m_StrikethroughColorStack.Add(this.m_StrikethroughColor);
							return true;
						}
					}
					else if (markupTag <= MarkupTag.SLASH_BOLD)
					{
						if (markupTag == MarkupTag.UNDERLINE)
						{
							this.m_FontStyleInternal |= FontStyles.Underline;
							this.m_FontStyleStack.Add(FontStyles.Underline);
							bool flag74 = this.m_XmlAttribute[1].nameHashCode == 81999901;
							if (flag74)
							{
								this.m_UnderlineColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
								this.m_UnderlineColor.a = ((this.m_HtmlColor.a < this.m_UnderlineColor.a) ? this.m_HtmlColor.a : this.m_UnderlineColor.a);
								textInfo.hasMultipleColors = true;
							}
							else
							{
								this.m_UnderlineColor = this.m_HtmlColor;
							}
							this.m_UnderlineColorStack.Add(this.m_UnderlineColor);
							return true;
						}
						if (markupTag == MarkupTag.SLASH_ITALIC)
						{
							bool flag75 = (generationSettings.fontStyle & FontStyles.Italic) != FontStyles.Italic;
							if (flag75)
							{
								this.m_ItalicAngle = this.m_ItalicAngleStack.Remove();
								bool flag76 = this.m_FontStyleStack.Remove(FontStyles.Italic) == 0;
								if (flag76)
								{
									this.m_FontStyleInternal &= ~FontStyles.Italic;
								}
							}
							return true;
						}
						if (markupTag != MarkupTag.SLASH_BOLD)
						{
							goto IL_40F4;
						}
						bool flag77 = (generationSettings.fontStyle & FontStyles.Bold) != FontStyles.Bold;
						if (flag77)
						{
							bool flag78 = this.m_FontStyleStack.Remove(FontStyles.Bold) == 0;
							if (flag78)
							{
								this.m_FontStyleInternal &= ~FontStyles.Bold;
								this.m_FontWeightInternal = this.m_FontWeightStack.Peek();
							}
						}
						return true;
					}
					else
					{
						if (markupTag == MarkupTag.SLASH_A)
						{
							bool flag79 = this.m_isTextLayoutPhase && !this.m_IsCalculatingPreferredValues;
							if (flag79)
							{
								bool flag80 = textInfo.linkInfo.Length == 0 || textInfo.linkCount <= 0;
								if (flag80)
								{
									bool displayWarnings = generationSettings.textSettings.displayWarnings;
									if (displayWarnings)
									{
										Debug.LogWarning("There seems to be an issue with the formatting of the <a> tag. Possible issues include: missing or misplaced closing '>', missing or incorrect attribute, or unclosed quotes for attribute values. Please review the tag syntax.");
									}
								}
								else
								{
									int num21 = textInfo.linkCount - 1;
									textInfo.linkInfo[num21].linkTextLength = this.m_CharacterCount - textInfo.linkInfo[num21].linkTextfirstCharacterIndex;
								}
							}
							return true;
						}
						if (markupTag == MarkupTag.SLASH_UNDERLINE)
						{
							bool flag81 = (generationSettings.fontStyle & FontStyles.Underline) != FontStyles.Underline;
							if (flag81)
							{
								bool flag82 = this.m_FontStyleStack.Remove(FontStyles.Underline) == 0;
								if (flag82)
								{
									this.m_FontStyleInternal &= ~FontStyles.Underline;
								}
							}
							this.m_UnderlineColor = this.m_UnderlineColorStack.Remove();
							return true;
						}
						if (markupTag != MarkupTag.SLASH_STRIKETHROUGH)
						{
							goto IL_40F4;
						}
						bool flag83 = (generationSettings.fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough;
						if (flag83)
						{
							bool flag84 = this.m_FontStyleStack.Remove(FontStyles.Strikethrough) == 0;
							if (flag84)
							{
								this.m_FontStyleInternal &= ~FontStyles.Strikethrough;
							}
						}
						this.m_StrikethroughColor = this.m_StrikethroughColorStack.Remove();
						return true;
					}
				}
			}
			else if (markupTag <= MarkupTag.SLASH_SIZE)
			{
				if (markupTag <= MarkupTag.PAGE)
				{
					if (markupTag <= MarkupTag.SLASH_SUPERSCRIPT)
					{
						if (markupTag <= MarkupTag.SUBSCRIPT)
						{
							if (markupTag != MarkupTag.POSITION)
							{
								if (markupTag != MarkupTag.SUBSCRIPT)
								{
									goto IL_40F4;
								}
								this.m_FontScaleMultiplier *= ((this.m_CurrentFontAsset.faceInfo.subscriptSize > 0f) ? this.m_CurrentFontAsset.faceInfo.subscriptSize : 1f);
								this.m_BaselineOffsetStack.Push(this.m_BaselineOffset);
								float num20 = this.m_CurrentFontSize / (float)this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
								this.m_BaselineOffset += this.m_CurrentFontAsset.faceInfo.subscriptOffset * num20 * this.m_FontScaleMultiplier;
								this.m_FontStyleStack.Add(FontStyles.Subscript);
								this.m_FontStyleInternal |= FontStyles.Subscript;
								return true;
							}
							else
							{
								float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								bool flag85 = num13 == -32768f;
								if (flag85)
								{
									return false;
								}
								switch (tagUnitType)
								{
								case TagUnitType.Pixels:
									this.m_XAdvance = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
									return true;
								case TagUnitType.FontUnits:
									this.m_XAdvance = num13 * this.m_CurrentFontSize * (generationSettings.isOrthographic ? 1f : 0.1f);
									return true;
								case TagUnitType.Percentage:
									this.m_XAdvance = this.m_MarginWidth * num13 / 100f;
									return true;
								default:
									return false;
								}
							}
						}
						else
						{
							if (markupTag == MarkupTag.SUPERSCRIPT)
							{
								this.m_FontScaleMultiplier *= ((this.m_CurrentFontAsset.faceInfo.superscriptSize > 0f) ? this.m_CurrentFontAsset.faceInfo.superscriptSize : 1f);
								this.m_BaselineOffsetStack.Push(this.m_BaselineOffset);
								float num20 = this.m_CurrentFontSize / (float)this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
								this.m_BaselineOffset += this.m_CurrentFontAsset.faceInfo.superscriptOffset * num20 * this.m_FontScaleMultiplier;
								this.m_FontStyleStack.Add(FontStyles.Superscript);
								this.m_FontStyleInternal |= FontStyles.Superscript;
								return true;
							}
							if (markupTag == MarkupTag.SLASH_SUBSCRIPT)
							{
								bool flag86 = (this.m_FontStyleInternal & FontStyles.Subscript) == FontStyles.Subscript;
								if (flag86)
								{
									bool flag87 = this.m_FontScaleMultiplier < 1f;
									if (flag87)
									{
										this.m_BaselineOffset = this.m_BaselineOffsetStack.Pop();
										this.m_FontScaleMultiplier /= ((this.m_CurrentFontAsset.faceInfo.subscriptSize > 0f) ? this.m_CurrentFontAsset.faceInfo.subscriptSize : 1f);
									}
									bool flag88 = this.m_FontStyleStack.Remove(FontStyles.Subscript) == 0;
									if (flag88)
									{
										this.m_FontStyleInternal &= ~FontStyles.Subscript;
									}
								}
								return true;
							}
							if (markupTag != MarkupTag.SLASH_SUPERSCRIPT)
							{
								goto IL_40F4;
							}
							bool flag89 = (this.m_FontStyleInternal & FontStyles.Superscript) == FontStyles.Superscript;
							if (flag89)
							{
								bool flag90 = this.m_FontScaleMultiplier < 1f;
								if (flag90)
								{
									this.m_BaselineOffset = this.m_BaselineOffsetStack.Pop();
									this.m_FontScaleMultiplier /= ((this.m_CurrentFontAsset.faceInfo.superscriptSize > 0f) ? this.m_CurrentFontAsset.faceInfo.superscriptSize : 1f);
								}
								bool flag91 = this.m_FontStyleStack.Remove(FontStyles.Superscript) == 0;
								if (flag91)
								{
									this.m_FontStyleInternal &= ~FontStyles.Superscript;
								}
							}
							return true;
						}
					}
					else if (markupTag <= MarkupTag.FONT)
					{
						if (markupTag == MarkupTag.SLASH_POSITION)
						{
							this.m_IsIgnoringAlignment = false;
							return true;
						}
						if (markupTag != MarkupTag.FONT)
						{
							goto IL_40F4;
						}
						int valueHashCode4 = this.m_XmlAttribute[0].valueHashCode;
						int nameHashCode5 = this.m_XmlAttribute[1].nameHashCode;
						int num22 = this.m_XmlAttribute[1].valueHashCode;
						bool flag92 = valueHashCode4 == -620974005;
						if (flag92)
						{
							this.m_CurrentFontAsset = this.m_MaterialReferences[0].fontAsset;
							this.m_CurrentMaterial = this.m_MaterialReferences[0].material;
							this.m_CurrentMaterialIndex = 0;
							this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[0]);
							return true;
						}
						FontAsset fontAsset;
						MaterialReferenceManager.TryGetFontAsset(valueHashCode4, out fontAsset);
						bool flag93 = fontAsset == null;
						if (flag93)
						{
							bool flag94 = fontAsset == null;
							if (flag94)
							{
								fontAsset = Resources.Load<FontAsset>(textSettings.defaultFontAssetPath + new string(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength));
							}
							bool flag95 = fontAsset == null;
							if (flag95)
							{
								return false;
							}
							MaterialReferenceManager.AddFontAsset(fontAsset);
						}
						bool flag96 = nameHashCode5 == 0 && num22 == 0;
						if (flag96)
						{
							this.m_CurrentMaterial = fontAsset.material;
							this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, fontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
							this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
						}
						else
						{
							bool flag97 = nameHashCode5 == 825491659;
							if (!flag97)
							{
								return false;
							}
							Material material;
							bool flag98 = MaterialReferenceManager.TryGetMaterial(num22, out material);
							if (flag98)
							{
								this.m_CurrentMaterial = material;
								this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, fontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
								this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
							}
							else
							{
								material = Resources.Load<Material>(textSettings.defaultFontAssetPath + new string(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength));
								bool flag99 = material == null;
								if (flag99)
								{
									return false;
								}
								MaterialReferenceManager.AddFontMaterial(num22, material);
								this.m_CurrentMaterial = material;
								this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, fontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
								this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
							}
						}
						this.m_CurrentFontAsset = fontAsset;
						return true;
					}
					else
					{
						if (markupTag == MarkupTag.LINK)
						{
							bool flag100 = this.m_isTextLayoutPhase && !this.m_IsCalculatingPreferredValues;
							if (flag100)
							{
								int linkCount2 = textInfo.linkCount;
								bool flag101 = linkCount2 + 1 > textInfo.linkInfo.Length;
								if (flag101)
								{
									TextInfo.Resize<LinkInfo>(ref textInfo.linkInfo, linkCount2 + 1);
								}
								textInfo.linkInfo[linkCount2].hashCode = this.m_XmlAttribute[0].valueHashCode;
								textInfo.linkInfo[linkCount2].linkTextfirstCharacterIndex = this.m_CharacterCount;
								textInfo.linkInfo[linkCount2].linkIdFirstCharacterIndex = startIndex + this.m_XmlAttribute[0].valueStartIndex;
								textInfo.linkInfo[linkCount2].SetLinkId(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							}
							return true;
						}
						if (markupTag == MarkupTag.MARK)
						{
							this.m_FontStyleInternal |= FontStyles.Highlight;
							this.m_FontStyleStack.Add(FontStyles.Highlight);
							Color32 color = new Color32(byte.MaxValue, byte.MaxValue, 0, 64);
							Offset offset = Offset.zero;
							int num23 = 0;
							while (num23 < this.m_XmlAttribute.Length && this.m_XmlAttribute[num23].nameHashCode != 0)
							{
								MarkupTag nameHashCode6 = (MarkupTag)this.m_XmlAttribute[num23].nameHashCode;
								MarkupTag markupTag8 = nameHashCode6;
								if (markupTag8 != MarkupTag.PADDING)
								{
									if (markupTag8 != MarkupTag.MARK)
									{
										if (markupTag8 == MarkupTag.COLOR)
										{
											color = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[num23].valueStartIndex, this.m_XmlAttribute[num23].valueLength);
										}
									}
									else
									{
										bool flag102 = this.m_XmlAttribute[num23].valueType == TagValueType.ColorValue;
										if (flag102)
										{
											color = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
										}
									}
								}
								else
								{
									int attributeParameters2 = TextGeneratorUtilities.GetAttributeParameters(this.m_HtmlTag, this.m_XmlAttribute[num23].valueStartIndex, this.m_XmlAttribute[num23].valueLength, ref this.m_AttributeParameterValues);
									bool flag103 = attributeParameters2 != 4;
									if (flag103)
									{
										return false;
									}
									offset = new Offset(this.m_AttributeParameterValues[0], this.m_AttributeParameterValues[1], this.m_AttributeParameterValues[2], this.m_AttributeParameterValues[3]);
									offset *= this.m_FontSize * 0.01f * (generationSettings.isOrthographic ? 1f : 0.1f);
								}
								num23++;
							}
							color.a = ((this.m_HtmlColor.a < color.a) ? this.m_HtmlColor.a : color.a);
							this.m_HighlightState = new HighlightState(color, offset);
							this.m_HighlightStateStack.Push(this.m_HighlightState);
							textInfo.hasMultipleColors = true;
							return true;
						}
						if (markupTag != MarkupTag.PAGE)
						{
							goto IL_40F4;
						}
						bool flag104 = generationSettings.overflowMode == TextOverflowMode.Page;
						if (flag104)
						{
							this.m_XAdvance = 0f + this.m_TagLineIndent + this.m_TagIndent;
							this.m_LineOffset = 0f;
							this.m_PageNumber++;
							this.m_IsNewPage = true;
						}
						return true;
					}
				}
				else if (markupTag <= MarkupTag.TH)
				{
					if (markupTag <= MarkupTag.SIZE)
					{
						if (markupTag == MarkupTag.NO_BREAK)
						{
							this.m_IsNonBreakingSpace = true;
							return true;
						}
						if (markupTag != MarkupTag.SIZE)
						{
							goto IL_40F4;
						}
						float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag105 = num13 == -32768f;
						if (flag105)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
						{
							bool flag106 = this.m_HtmlTag[5] == '+';
							if (flag106)
							{
								this.m_CurrentFontSize = this.m_FontSize + num13;
								this.m_SizeStack.Add(this.m_CurrentFontSize);
								return true;
							}
							bool flag107 = this.m_HtmlTag[5] == '-';
							if (flag107)
							{
								this.m_CurrentFontSize = this.m_FontSize + num13;
								this.m_SizeStack.Add(this.m_CurrentFontSize);
								return true;
							}
							this.m_CurrentFontSize = num13;
							this.m_SizeStack.Add(this.m_CurrentFontSize);
							return true;
						}
						case TagUnitType.FontUnits:
							this.m_CurrentFontSize = this.m_FontSize * num13;
							this.m_SizeStack.Add(this.m_CurrentFontSize);
							return true;
						case TagUnitType.Percentage:
							this.m_CurrentFontSize = this.m_FontSize * num13 / 100f;
							this.m_SizeStack.Add(this.m_CurrentFontSize);
							return true;
						default:
							return false;
						}
					}
					else
					{
						if (markupTag == MarkupTag.TR)
						{
							return false;
						}
						if (markupTag == MarkupTag.TD)
						{
							return false;
						}
						if (markupTag != MarkupTag.TH)
						{
							goto IL_40F4;
						}
						return false;
					}
				}
				else if (markupTag <= MarkupTag.SLASH_MARK)
				{
					if (markupTag == MarkupTag.SLASH_NO_BREAK)
					{
						this.m_IsNonBreakingSpace = false;
						return true;
					}
					if (markupTag != MarkupTag.SLASH_MARK)
					{
						goto IL_40F4;
					}
					bool flag108 = (generationSettings.fontStyle & FontStyles.Highlight) != FontStyles.Highlight;
					if (flag108)
					{
						this.m_HighlightStateStack.Remove();
						this.m_HighlightState = this.m_HighlightStateStack.current;
						bool flag109 = this.m_FontStyleStack.Remove(FontStyles.Highlight) == 0;
						if (flag109)
						{
							this.m_FontStyleInternal &= ~FontStyles.Highlight;
						}
					}
					return true;
				}
				else
				{
					if (markupTag == MarkupTag.SLASH_LINK)
					{
						bool flag110 = this.m_isTextLayoutPhase && !this.m_IsCalculatingPreferredValues;
						if (flag110)
						{
							bool flag111 = textInfo.linkCount < textInfo.linkInfo.Length;
							if (flag111)
							{
								textInfo.linkInfo[textInfo.linkCount].linkTextLength = this.m_CharacterCount - textInfo.linkInfo[textInfo.linkCount].linkTextfirstCharacterIndex;
								textInfo.linkCount++;
							}
						}
						return true;
					}
					if (markupTag == MarkupTag.SLASH_FONT)
					{
						MaterialReference materialReference2 = this.m_MaterialReferenceStack.Remove();
						this.m_CurrentFontAsset = materialReference2.fontAsset;
						this.m_CurrentMaterial = materialReference2.material;
						this.m_CurrentMaterialIndex = materialReference2.index;
						return true;
					}
					if (markupTag != MarkupTag.SLASH_SIZE)
					{
						goto IL_40F4;
					}
					this.m_CurrentFontSize = this.m_SizeStack.Remove();
					return true;
				}
			}
			else if (markupTag <= MarkupTag.SLASH_TH)
			{
				if (markupTag <= MarkupTag.SLASH_LINE_INDENT)
				{
					if (markupTag <= MarkupTag.ALPHA)
					{
						if (markupTag == MarkupTag.ALIGN)
						{
							MarkupTag valueHashCode5 = (MarkupTag)this.m_XmlAttribute[0].valueHashCode;
							MarkupTag markupTag9 = valueHashCode5;
							if (markupTag9 <= MarkupTag.LEFT)
							{
								if (markupTag9 == MarkupTag.CENTER)
								{
									this.m_LineJustification = TextAlignment.MiddleCenter;
									this.m_LineJustificationStack.Add(this.m_LineJustification);
									return true;
								}
								if (markupTag9 == MarkupTag.LEFT)
								{
									this.m_LineJustification = TextAlignment.MiddleLeft;
									this.m_LineJustificationStack.Add(this.m_LineJustification);
									return true;
								}
							}
							else
							{
								if (markupTag9 == MarkupTag.FLUSH)
								{
									this.m_LineJustification = TextAlignment.MiddleFlush;
									this.m_LineJustificationStack.Add(this.m_LineJustification);
									return true;
								}
								if (markupTag9 == MarkupTag.RIGHT)
								{
									this.m_LineJustification = TextAlignment.MiddleRight;
									this.m_LineJustificationStack.Add(this.m_LineJustification);
									return true;
								}
								if (markupTag9 == MarkupTag.JUSTIFIED)
								{
									this.m_LineJustification = TextAlignment.MiddleJustified;
									this.m_LineJustificationStack.Add(this.m_LineJustification);
									return true;
								}
							}
							return false;
						}
						if (markupTag != MarkupTag.ALPHA)
						{
							goto IL_40F4;
						}
						bool flag112 = this.m_XmlAttribute[0].valueLength != 3;
						if (flag112)
						{
							return false;
						}
						this.m_HtmlColor.a = (byte)(TextGeneratorUtilities.HexToInt(this.m_HtmlTag[7]) * 16U + TextGeneratorUtilities.HexToInt(this.m_HtmlTag[8]));
						return true;
					}
					else if (markupTag != MarkupTag.COLOR)
					{
						if (markupTag == MarkupTag.CLASS)
						{
							return false;
						}
						if (markupTag != MarkupTag.SLASH_LINE_INDENT)
						{
							goto IL_40F4;
						}
						this.m_TagLineIndent = 0f;
						return true;
					}
					else
					{
						textInfo.hasMultipleColors = true;
						bool flag113 = this.m_HtmlTag[6] == '#' && num == 10;
						if (flag113)
						{
							this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, num);
							this.m_ColorStack.Add(this.m_HtmlColor);
							return true;
						}
						bool flag114 = this.m_HtmlTag[6] == '#' && num == 11;
						if (flag114)
						{
							this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, num);
							this.m_ColorStack.Add(this.m_HtmlColor);
							return true;
						}
						bool flag115 = this.m_HtmlTag[6] == '#' && num == 13;
						if (flag115)
						{
							this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, num);
							this.m_ColorStack.Add(this.m_HtmlColor);
							return true;
						}
						bool flag116 = this.m_HtmlTag[6] == '#' && num == 15;
						if (flag116)
						{
							this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, num);
							this.m_ColorStack.Add(this.m_HtmlColor);
							return true;
						}
						int valueHashCode6 = this.m_XmlAttribute[0].valueHashCode;
						int num24 = valueHashCode6;
						if (num24 <= 91635)
						{
							if (num24 <= -1108587920)
							{
								if (num24 == -1250222130)
								{
									this.m_HtmlColor = new Color32(160, 32, 240, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num24 == -1108587920)
								{
									this.m_HtmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
							}
							else
							{
								if (num24 == -992792864)
								{
									this.m_HtmlColor = new Color32(173, 216, 230, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num24 == -882444668)
								{
									this.m_HtmlColor = Color.yellow;
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num24 == 91635)
								{
									this.m_HtmlColor = Color.red;
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
							}
						}
						else if (num24 <= 3680713)
						{
							if (num24 == 2457214)
							{
								this.m_HtmlColor = Color.blue;
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
							if (num24 == 3680713)
							{
								this.m_HtmlColor = new Color32(128, 128, 128, byte.MaxValue);
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
						}
						else
						{
							if (num24 == 81074727)
							{
								this.m_HtmlColor = Color.black;
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
							if (num24 == 87065851)
							{
								this.m_HtmlColor = Color.green;
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
							if (num24 == 105680263)
							{
								this.m_HtmlColor = Color.white;
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
						}
						return false;
					}
				}
				else if (markupTag <= MarkupTag.SCALE)
				{
					if (markupTag != MarkupTag.SPACE)
					{
						if (markupTag != MarkupTag.SCALE)
						{
							goto IL_40F4;
						}
						float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag117 = num13 == -32768f;
						if (flag117)
						{
							return false;
						}
						this.m_FXScale = new Vector3(num13, 1f, 1f);
						return true;
					}
					else
					{
						float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag118 = num13 == -32768f;
						if (flag118)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
							this.m_XAdvance += num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
							return true;
						case TagUnitType.FontUnits:
							this.m_XAdvance += num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
							return true;
						case TagUnitType.Percentage:
							return false;
						default:
							return false;
						}
					}
				}
				else if (markupTag != MarkupTag.WIDTH)
				{
					if (markupTag == MarkupTag.SLASH_TR)
					{
						return false;
					}
					if (markupTag != MarkupTag.SLASH_TH)
					{
						goto IL_40F4;
					}
					return false;
				}
				else
				{
					float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
					bool flag119 = num13 == -32768f;
					if (flag119)
					{
						return false;
					}
					switch (tagUnitType)
					{
					case TagUnitType.Pixels:
						this.m_Width = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
						break;
					case TagUnitType.FontUnits:
						return false;
					case TagUnitType.Percentage:
						this.m_Width = this.m_MarginWidth * num13 / 100f;
						break;
					}
					return true;
				}
			}
			else if (markupTag <= MarkupTag.TABLE)
			{
				if (markupTag <= MarkupTag.SLASH_SMALLCAPS)
				{
					if (markupTag == MarkupTag.SLASH_TD)
					{
						return false;
					}
					if (markupTag != MarkupTag.SLASH_SMALLCAPS)
					{
						goto IL_40F4;
					}
					bool flag120 = (generationSettings.fontStyle & FontStyles.SmallCaps) != FontStyles.SmallCaps;
					if (flag120)
					{
						bool flag121 = this.m_FontStyleStack.Remove(FontStyles.SmallCaps) == 0;
						if (flag121)
						{
							this.m_FontStyleInternal &= ~FontStyles.SmallCaps;
						}
					}
					return true;
				}
				else
				{
					if (markupTag == MarkupTag.SLASH_LINE_HEIGHT)
					{
						this.m_LineHeight = -32767f;
						return true;
					}
					if (markupTag != MarkupTag.ALLCAPS)
					{
						if (markupTag != MarkupTag.TABLE)
						{
							goto IL_40F4;
						}
						return false;
					}
				}
			}
			else if (markupTag <= MarkupTag.SLASH_ALIGN)
			{
				if (markupTag != MarkupTag.MATERIAL)
				{
					if (markupTag == MarkupTag.SLASH_COLOR)
					{
						this.m_HtmlColor = this.m_ColorStack.Remove();
						return true;
					}
					if (markupTag != MarkupTag.SLASH_ALIGN)
					{
						goto IL_40F4;
					}
					this.m_LineJustification = this.m_LineJustificationStack.Remove();
					return true;
				}
				else
				{
					int num22 = this.m_XmlAttribute[0].valueHashCode;
					bool flag122 = num22 == -620974005;
					if (flag122)
					{
						this.m_CurrentMaterial = this.m_MaterialReferences[0].material;
						this.m_CurrentMaterialIndex = 0;
						this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[0]);
						return true;
					}
					Material material;
					bool flag123 = MaterialReferenceManager.TryGetMaterial(num22, out material);
					if (flag123)
					{
						this.m_CurrentMaterial = material;
						this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
						this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
					}
					else
					{
						material = Resources.Load<Material>(textSettings.defaultFontAssetPath + new string(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength));
						bool flag124 = material == null;
						if (flag124)
						{
							return false;
						}
						MaterialReferenceManager.AddFontMaterial(num22, material);
						this.m_CurrentMaterial = material;
						this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
						this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
					}
					return true;
				}
			}
			else
			{
				if (markupTag == MarkupTag.SLASH_WIDTH)
				{
					this.m_Width = -1f;
					return true;
				}
				if (markupTag == MarkupTag.SLASH_SCALE)
				{
					this.m_FXScale = Vector3.one;
					return true;
				}
				if (markupTag != MarkupTag.VERTICAL_OFFSET)
				{
					goto IL_40F4;
				}
				float num13 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
				bool flag125 = num13 == -32768f;
				if (flag125)
				{
					return false;
				}
				switch (tagUnitType)
				{
				case TagUnitType.Pixels:
					this.m_BaselineOffset = num13 * (generationSettings.isOrthographic ? 1f : 0.1f);
					return true;
				case TagUnitType.FontUnits:
					this.m_BaselineOffset = num13 * (generationSettings.isOrthographic ? 1f : 0.1f) * this.m_CurrentFontSize;
					return true;
				case TagUnitType.Percentage:
					return false;
				default:
					return false;
				}
			}
			IL_3701:
			this.m_FontStyleInternal |= FontStyles.UpperCase;
			this.m_FontStyleStack.Add(FontStyles.UpperCase);
			return true;
			IL_40F4:
			return false;
		}

		private void SaveGlyphVertexInfo(float padding, float stylePadding, Color32 vertexColor, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.position = textInfo.textElementInfo[this.m_CharacterCount].bottomLeft;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.position = textInfo.textElementInfo[this.m_CharacterCount].topLeft;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.position = textInfo.textElementInfo[this.m_CharacterCount].topRight;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.position = textInfo.textElementInfo[this.m_CharacterCount].bottomRight;
			vertexColor.a = ((this.m_FontColor32.a < vertexColor.a) ? this.m_FontColor32.a : vertexColor.a);
			bool flag = false;
			bool flag2 = generationSettings.fontColorGradient == null || flag;
			if (flag2)
			{
				vertexColor = (flag ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, vertexColor.a) : vertexColor);
				textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.color = vertexColor;
				textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.color = vertexColor;
				textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.color = vertexColor;
				textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.color = vertexColor;
			}
			else
			{
				bool flag3 = !generationSettings.overrideRichTextColors && this.m_ColorStack.index > 1;
				if (flag3)
				{
					textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.color = vertexColor;
					textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.color = vertexColor;
					textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.color = vertexColor;
					textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.color = vertexColor;
				}
				else
				{
					bool flag4 = generationSettings.fontColorGradientPreset != null;
					if (flag4)
					{
						textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.color = generationSettings.fontColorGradientPreset.bottomLeft * vertexColor;
						textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.color = generationSettings.fontColorGradientPreset.topLeft * vertexColor;
						textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.color = generationSettings.fontColorGradientPreset.topRight * vertexColor;
						textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.color = generationSettings.fontColorGradientPreset.bottomRight * vertexColor;
					}
					else
					{
						textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.color = generationSettings.fontColorGradient.bottomLeft * vertexColor;
						textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.color = generationSettings.fontColorGradient.topLeft * vertexColor;
						textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.color = generationSettings.fontColorGradient.topRight * vertexColor;
						textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.color = generationSettings.fontColorGradient.bottomRight * vertexColor;
					}
				}
			}
			bool flag5 = this.m_ColorGradientPreset != null && !flag;
			if (flag5)
			{
				bool colorGradientPresetIsTinted = this.m_ColorGradientPresetIsTinted;
				if (colorGradientPresetIsTinted)
				{
					TextElementInfo[] textElementInfo = textInfo.textElementInfo;
					int characterCount = this.m_CharacterCount;
					textElementInfo[characterCount].vertexBottomLeft.color = textElementInfo[characterCount].vertexBottomLeft.color * this.m_ColorGradientPreset.bottomLeft;
					TextElementInfo[] textElementInfo2 = textInfo.textElementInfo;
					int characterCount2 = this.m_CharacterCount;
					textElementInfo2[characterCount2].vertexTopLeft.color = textElementInfo2[characterCount2].vertexTopLeft.color * this.m_ColorGradientPreset.topLeft;
					TextElementInfo[] textElementInfo3 = textInfo.textElementInfo;
					int characterCount3 = this.m_CharacterCount;
					textElementInfo3[characterCount3].vertexTopRight.color = textElementInfo3[characterCount3].vertexTopRight.color * this.m_ColorGradientPreset.topRight;
					TextElementInfo[] textElementInfo4 = textInfo.textElementInfo;
					int characterCount4 = this.m_CharacterCount;
					textElementInfo4[characterCount4].vertexBottomRight.color = textElementInfo4[characterCount4].vertexBottomRight.color * this.m_ColorGradientPreset.bottomRight;
				}
				else
				{
					textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.color = this.m_ColorGradientPreset.bottomLeft.MinAlpha(vertexColor);
					textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.color = this.m_ColorGradientPreset.topLeft.MinAlpha(vertexColor);
					textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.color = this.m_ColorGradientPreset.topRight.MinAlpha(vertexColor);
					textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.color = this.m_ColorGradientPreset.bottomRight.MinAlpha(vertexColor);
				}
			}
			stylePadding = 0f;
			Glyph alternativeGlyph = textInfo.textElementInfo[this.m_CharacterCount].alternativeGlyph;
			GlyphRect glyphRect = ((alternativeGlyph == null) ? this.m_CachedTextElement.m_Glyph.glyphRect : alternativeGlyph.glyphRect);
			Vector2 vector;
			vector.x = ((float)glyphRect.x - padding - stylePadding) / (float)this.m_CurrentFontAsset.atlasWidth;
			vector.y = ((float)glyphRect.y - padding - stylePadding) / (float)this.m_CurrentFontAsset.atlasHeight;
			Vector2 vector2;
			vector2.x = vector.x;
			vector2.y = ((float)glyphRect.y + padding + stylePadding + (float)glyphRect.height) / (float)this.m_CurrentFontAsset.atlasHeight;
			Vector2 vector3;
			vector3.x = ((float)glyphRect.x + padding + stylePadding + (float)glyphRect.width) / (float)this.m_CurrentFontAsset.atlasWidth;
			vector3.y = vector2.y;
			Vector2 vector4;
			vector4.x = vector3.x;
			vector4.y = vector.y;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.uv = vector;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.uv = vector2;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.uv = vector3;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.uv = vector4;
		}

		private void SaveSpriteVertexInfo(Color32 vertexColor, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.position = textInfo.textElementInfo[this.m_CharacterCount].bottomLeft;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.position = textInfo.textElementInfo[this.m_CharacterCount].topLeft;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.position = textInfo.textElementInfo[this.m_CharacterCount].topRight;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.position = textInfo.textElementInfo[this.m_CharacterCount].bottomRight;
			bool tintSprites = generationSettings.tintSprites;
			if (tintSprites)
			{
				this.m_TintSprite = true;
			}
			Color32 color = (this.m_TintSprite ? ColorUtilities.MultiplyColors(this.m_SpriteColor, vertexColor) : this.m_SpriteColor);
			color.a = ((color.a < this.m_FontColor32.a) ? ((color.a < vertexColor.a) ? color.a : vertexColor.a) : this.m_FontColor32.a);
			Color32 color2 = color;
			Color32 color3 = color;
			Color32 color4 = color;
			Color32 color5 = color;
			bool flag = generationSettings.fontColorGradient != null;
			if (flag)
			{
				bool flag2 = generationSettings.fontColorGradientPreset != null;
				if (flag2)
				{
					color2 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color2, generationSettings.fontColorGradientPreset.bottomLeft) : color2);
					color3 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color3, generationSettings.fontColorGradientPreset.topLeft) : color3);
					color4 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color4, generationSettings.fontColorGradientPreset.topRight) : color4);
					color5 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color5, generationSettings.fontColorGradientPreset.bottomRight) : color5);
				}
				else
				{
					color2 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color2, generationSettings.fontColorGradient.bottomLeft) : color2);
					color3 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color3, generationSettings.fontColorGradient.topLeft) : color3);
					color4 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color4, generationSettings.fontColorGradient.topRight) : color4);
					color5 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color5, generationSettings.fontColorGradient.bottomRight) : color5);
				}
			}
			bool flag3 = this.m_ColorGradientPreset != null;
			if (flag3)
			{
				color2 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color2, this.m_ColorGradientPreset.bottomLeft) : color2);
				color3 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color3, this.m_ColorGradientPreset.topLeft) : color3);
				color4 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color4, this.m_ColorGradientPreset.topRight) : color4);
				color5 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color5, this.m_ColorGradientPreset.bottomRight) : color5);
			}
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.color = color2;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.color = color3;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.color = color4;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.color = color5;
			Vector2 vector = new Vector2((float)this.m_CachedTextElement.glyph.glyphRect.x / (float)this.m_CurrentSpriteAsset.spriteSheet.width, (float)this.m_CachedTextElement.glyph.glyphRect.y / (float)this.m_CurrentSpriteAsset.spriteSheet.height);
			Vector2 vector2 = new Vector2(vector.x, (float)(this.m_CachedTextElement.glyph.glyphRect.y + this.m_CachedTextElement.glyph.glyphRect.height) / (float)this.m_CurrentSpriteAsset.spriteSheet.height);
			Vector2 vector3 = new Vector2((float)(this.m_CachedTextElement.glyph.glyphRect.x + this.m_CachedTextElement.glyph.glyphRect.width) / (float)this.m_CurrentSpriteAsset.spriteSheet.width, vector2.y);
			Vector2 vector4 = new Vector2(vector3.x, vector.y);
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.uv = vector;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.uv = vector2;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.uv = vector3;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.uv = vector4;
		}

		private void DrawUnderlineMesh(Vector3 start, Vector3 end, float startScale, float endScale, float maxScale, float sdfScale, Color32 underlineColor, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			this.GetUnderlineSpecialCharacter(generationSettings);
			bool flag = this.m_Underline.character == null;
			if (flag)
			{
				bool displayWarnings = generationSettings.textSettings.displayWarnings;
				if (displayWarnings)
				{
					Debug.LogWarning("Unable to add underline or strikethrough since the character [0x5F] used by these features is not present in the Font Asset assigned to this text object.");
				}
			}
			else
			{
				int vertexCount = textInfo.meshInfo[this.m_CurrentMaterialIndex].vertexCount;
				int num = vertexCount + 12;
				bool flag2 = num > textInfo.meshInfo[this.m_CurrentMaterialIndex].vertices.Length;
				if (flag2)
				{
					textInfo.meshInfo[this.m_CurrentMaterialIndex].ResizeMeshInfo(num / 4);
				}
				start.y = Mathf.Min(start.y, end.y);
				end.y = Mathf.Min(start.y, end.y);
				GlyphMetrics metrics = this.m_Underline.character.glyph.metrics;
				GlyphRect glyphRect = this.m_Underline.character.glyph.glyphRect;
				start.x += (startScale - maxScale) * this.m_Padding;
				end.x += (maxScale - endScale) * this.m_Padding;
				float num2 = (metrics.width * 0.5f + this.m_Padding) * maxScale;
				float num3 = 1f;
				float num4 = 2f * num2;
				float num5 = end.x - start.x;
				bool flag3 = num5 < num4;
				if (flag3)
				{
					num3 = num5 / num4;
					num2 *= num3;
				}
				float underlineThickness = this.m_Underline.fontAsset.faceInfo.underlineThickness;
				float x = start.x;
				float num6 = start.x + num2;
				float num7 = end.x - num2;
				float x2 = end.x;
				float num8 = start.y - (underlineThickness + this.m_Padding) * maxScale;
				float num9 = start.y + this.m_Padding * maxScale;
				Vector3[] vertices = textInfo.meshInfo[this.m_CurrentMaterialIndex].vertices;
				vertices[vertexCount] = new Vector3(x, num8);
				vertices[vertexCount + 1] = new Vector3(x, num9);
				vertices[vertexCount + 2] = new Vector3(num6, num9);
				vertices[vertexCount + 3] = new Vector3(num6, num8);
				vertices[vertexCount + 4] = new Vector3(num6, num8);
				vertices[vertexCount + 5] = new Vector3(num6, num9);
				vertices[vertexCount + 6] = new Vector3(num7, num9);
				vertices[vertexCount + 7] = new Vector3(num7, num8);
				vertices[vertexCount + 8] = new Vector3(num7, num8);
				vertices[vertexCount + 9] = new Vector3(num7, num9);
				vertices[vertexCount + 10] = new Vector3(x2, num9);
				vertices[vertexCount + 11] = new Vector3(x2, num8);
				bool inverseYAxis = generationSettings.inverseYAxis;
				if (inverseYAxis)
				{
					Vector3 vector;
					vector.x = 0f;
					vector.y = generationSettings.screenRect.y + generationSettings.screenRect.height;
					vector.z = 0f;
					for (int i = 0; i < 12; i++)
					{
						vertices[vertexCount + i].y = vertices[vertexCount + i].y * -1f + vector.y;
					}
				}
				float num10 = 1f / (float)this.m_Underline.fontAsset.atlasWidth;
				float num11 = 1f / (float)this.m_Underline.fontAsset.atlasHeight;
				float num12 = ((float)glyphRect.width * 0.5f + this.m_Padding) * num3 * num10;
				float num13 = ((float)glyphRect.x - this.m_Padding) * num10;
				float num14 = num13 + num12;
				float num15 = ((float)glyphRect.x + (float)glyphRect.width * 0.5f) * num10;
				float num16 = ((float)(glyphRect.x + glyphRect.width) + this.m_Padding) * num10;
				float num17 = num16 - num12;
				float num18 = ((float)glyphRect.y - this.m_Padding) * num11;
				float num19 = ((float)(glyphRect.y + glyphRect.height) + this.m_Padding) * num11;
				float num20 = Mathf.Abs(sdfScale);
				Vector4[] uvs = textInfo.meshInfo[this.m_CurrentMaterialIndex].uvs0;
				uvs[vertexCount] = new Vector4(num13, num18, 0f, num20);
				uvs[1 + vertexCount] = new Vector4(num13, num19, 0f, num20);
				uvs[2 + vertexCount] = new Vector4(num14, num19, 0f, num20);
				uvs[3 + vertexCount] = new Vector4(num14, num18, 0f, num20);
				uvs[4 + vertexCount] = new Vector4(num15, num18, 0f, num20);
				uvs[5 + vertexCount] = new Vector4(num15, num19, 0f, num20);
				uvs[6 + vertexCount] = new Vector4(num15, num19, 0f, num20);
				uvs[7 + vertexCount] = new Vector4(num15, num18, 0f, num20);
				uvs[8 + vertexCount] = new Vector4(num17, num18, 0f, num20);
				uvs[9 + vertexCount] = new Vector4(num17, num19, 0f, num20);
				uvs[10 + vertexCount] = new Vector4(num16, num19, 0f, num20);
				uvs[11 + vertexCount] = new Vector4(num16, num18, 0f, num20);
				float num21 = 1f / num5;
				float num22 = (vertices[vertexCount + 2].x - start.x) * num21;
				Vector2[] uvs2 = textInfo.meshInfo[this.m_CurrentMaterialIndex].uvs2;
				uvs2[vertexCount] = TextGeneratorUtilities.PackUV(0f, 0f, num20);
				uvs2[1 + vertexCount] = TextGeneratorUtilities.PackUV(0f, 1f, num20);
				uvs2[2 + vertexCount] = TextGeneratorUtilities.PackUV(num22, 1f, num20);
				uvs2[3 + vertexCount] = TextGeneratorUtilities.PackUV(num22, 0f, num20);
				float num23 = (vertices[vertexCount + 4].x - start.x) * num21;
				num22 = (vertices[vertexCount + 6].x - start.x) * num21;
				uvs2[4 + vertexCount] = TextGeneratorUtilities.PackUV(num23, 0f, num20);
				uvs2[5 + vertexCount] = TextGeneratorUtilities.PackUV(num23, 1f, num20);
				uvs2[6 + vertexCount] = TextGeneratorUtilities.PackUV(num22, 1f, num20);
				uvs2[7 + vertexCount] = TextGeneratorUtilities.PackUV(num22, 0f, num20);
				num23 = (vertices[vertexCount + 8].x - start.x) * num21;
				uvs2[8 + vertexCount] = TextGeneratorUtilities.PackUV(num23, 0f, num20);
				uvs2[9 + vertexCount] = TextGeneratorUtilities.PackUV(num23, 1f, num20);
				uvs2[10 + vertexCount] = TextGeneratorUtilities.PackUV(1f, 1f, num20);
				uvs2[11 + vertexCount] = TextGeneratorUtilities.PackUV(1f, 0f, num20);
				underlineColor.a = ((this.m_FontColor32.a < underlineColor.a) ? this.m_FontColor32.a : underlineColor.a);
				Color32[] colors = textInfo.meshInfo[this.m_CurrentMaterialIndex].colors32;
				for (int j = 0; j < 12; j++)
				{
					colors[j + vertexCount] = underlineColor;
				}
				MeshInfo[] meshInfo = textInfo.meshInfo;
				int currentMaterialIndex = this.m_CurrentMaterialIndex;
				meshInfo[currentMaterialIndex].vertexCount = meshInfo[currentMaterialIndex].vertexCount + 12;
			}
		}

		private void DrawTextHighlight(Vector3 start, Vector3 end, Color32 highlightColor, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			this.GetUnderlineSpecialCharacter(generationSettings);
			bool flag = this.m_Underline.character == null;
			if (flag)
			{
				bool displayWarnings = generationSettings.textSettings.displayWarnings;
				if (displayWarnings)
				{
					Debug.LogWarning("Unable to add highlight since the primary Font Asset doesn't contain the underline character.");
				}
			}
			else
			{
				int vertexCount = textInfo.meshInfo[this.m_CurrentMaterialIndex].vertexCount;
				int num = vertexCount + 4;
				bool flag2 = num > textInfo.meshInfo[this.m_CurrentMaterialIndex].vertices.Length;
				if (flag2)
				{
					textInfo.meshInfo[this.m_CurrentMaterialIndex].ResizeMeshInfo(num / 4);
				}
				Vector3[] vertices = textInfo.meshInfo[this.m_CurrentMaterialIndex].vertices;
				vertices[vertexCount] = start;
				vertices[vertexCount + 1] = new Vector3(start.x, end.y, 0f);
				vertices[vertexCount + 2] = end;
				vertices[vertexCount + 3] = new Vector3(end.x, start.y, 0f);
				bool inverseYAxis = generationSettings.inverseYAxis;
				if (inverseYAxis)
				{
					Vector3 vector;
					vector.x = 0f;
					vector.y = generationSettings.screenRect.y + generationSettings.screenRect.height;
					vector.z = 0f;
					vertices[vertexCount].y = vertices[vertexCount].y * -1f + vector.y;
					vertices[vertexCount + 1].y = vertices[vertexCount + 1].y * -1f + vector.y;
					vertices[vertexCount + 2].y = vertices[vertexCount + 2].y * -1f + vector.y;
					vertices[vertexCount + 3].y = vertices[vertexCount + 3].y * -1f + vector.y;
				}
				Vector4[] uvs = textInfo.meshInfo[this.m_CurrentMaterialIndex].uvs0;
				int atlasWidth = this.m_Underline.fontAsset.atlasWidth;
				int atlasHeight = this.m_Underline.fontAsset.atlasHeight;
				GlyphRect glyphRect = this.m_Underline.character.glyph.glyphRect;
				Vector2 vector2 = new Vector2(((float)glyphRect.x + (float)glyphRect.width / 2f) / (float)atlasWidth, ((float)glyphRect.y + (float)glyphRect.height / 2f) / (float)atlasHeight);
				Vector2 vector3 = new Vector2(1f / (float)atlasWidth, 1f / (float)atlasHeight);
				uvs[vertexCount] = vector2 - vector3;
				uvs[1 + vertexCount] = vector2 + new Vector2(-vector3.x, vector3.y);
				uvs[2 + vertexCount] = vector2 + vector3;
				uvs[3 + vertexCount] = vector2 + new Vector2(vector3.x, -vector3.y);
				Vector2[] uvs2 = textInfo.meshInfo[this.m_CurrentMaterialIndex].uvs2;
				Vector2 vector4 = new Vector2(0f, 1f);
				uvs2[vertexCount] = vector4;
				uvs2[1 + vertexCount] = vector4;
				uvs2[2 + vertexCount] = vector4;
				uvs2[3 + vertexCount] = vector4;
				highlightColor.a = ((this.m_FontColor32.a < highlightColor.a) ? this.m_FontColor32.a : highlightColor.a);
				Color32[] colors = textInfo.meshInfo[this.m_CurrentMaterialIndex].colors32;
				colors[vertexCount] = highlightColor;
				colors[1 + vertexCount] = highlightColor;
				colors[2 + vertexCount] = highlightColor;
				colors[3 + vertexCount] = highlightColor;
				MeshInfo[] meshInfo = textInfo.meshInfo;
				int currentMaterialIndex = this.m_CurrentMaterialIndex;
				meshInfo[currentMaterialIndex].vertexCount = meshInfo[currentMaterialIndex].vertexCount + 4;
			}
		}

		private static void ClearMesh(bool updateMesh, TextInfo textInfo)
		{
			textInfo.ClearMeshInfo(updateMesh);
		}

		internal int SetArraySizes(TextProcessingElement[] textProcessingArray, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			TextSettings textSettings = generationSettings.textSettings;
			int num = 0;
			this.m_TotalCharacterCount = 0;
			this.m_isTextLayoutPhase = false;
			this.m_TagNoParsing = false;
			this.m_FontStyleInternal = generationSettings.fontStyle;
			this.m_FontStyleStack.Clear();
			this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
			this.m_FontWeightStack.SetDefault(this.m_FontWeightInternal);
			this.m_CurrentFontAsset = generationSettings.fontAsset;
			this.m_CurrentMaterial = generationSettings.material;
			this.m_CurrentMaterialIndex = 0;
			this.m_MaterialReferenceStack.SetDefault(new MaterialReference(this.m_CurrentMaterialIndex, this.m_CurrentFontAsset, null, this.m_CurrentMaterial, this.m_Padding));
			this.m_MaterialReferenceIndexLookup.Clear();
			MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
			bool flag = textInfo == null;
			if (flag)
			{
				textInfo = new TextInfo();
			}
			else
			{
				bool flag2 = textInfo.textElementInfo.Length < this.m_InternalTextProcessingArraySize;
				if (flag2)
				{
					TextInfo.Resize<TextElementInfo>(ref textInfo.textElementInfo, this.m_InternalTextProcessingArraySize, false);
				}
			}
			this.m_TextElementType = TextElementType.Character;
			bool flag3 = generationSettings.overflowMode == TextOverflowMode.Ellipsis;
			if (flag3)
			{
				this.GetEllipsisSpecialCharacter(generationSettings);
				bool flag4 = this.m_Ellipsis.character != null;
				if (flag4)
				{
					bool flag5 = this.m_Ellipsis.fontAsset.GetInstanceID() != this.m_CurrentFontAsset.GetInstanceID();
					if (flag5)
					{
						bool flag6 = textSettings.matchMaterialPreset && this.m_CurrentMaterial.GetInstanceID() != this.m_Ellipsis.fontAsset.material.GetInstanceID();
						if (flag6)
						{
							this.m_Ellipsis.material = MaterialManager.GetFallbackMaterial(this.m_CurrentMaterial, this.m_Ellipsis.fontAsset.material);
						}
						else
						{
							this.m_Ellipsis.material = this.m_Ellipsis.fontAsset.material;
						}
						this.m_Ellipsis.materialIndex = MaterialReference.AddMaterialReference(this.m_Ellipsis.material, this.m_Ellipsis.fontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
						this.m_MaterialReferences[this.m_Ellipsis.materialIndex].referenceCount = 0;
					}
				}
				else
				{
					generationSettings.overflowMode = TextOverflowMode.Truncate;
					bool displayWarnings = textSettings.displayWarnings;
					if (displayWarnings)
					{
						Debug.LogWarning("The character used for Ellipsis is not available in font asset [" + this.m_CurrentFontAsset.name + "] or any potential fallbacks. Switching Text Overflow mode to Truncate.");
					}
				}
			}
			int num2 = 0;
			while (num2 < textProcessingArray.Length && textProcessingArray[num2].unicode > 0U)
			{
				bool flag7 = textInfo.textElementInfo == null || this.m_TotalCharacterCount >= textInfo.textElementInfo.Length;
				if (flag7)
				{
					TextInfo.Resize<TextElementInfo>(ref textInfo.textElementInfo, this.m_TotalCharacterCount + 1, true);
				}
				uint num3 = textProcessingArray[num2].unicode;
				int num4 = this.m_CurrentMaterialIndex;
				bool flag8 = generationSettings.richText && num3 == 60U;
				if (!flag8)
				{
					goto IL_045A;
				}
				num4 = this.m_CurrentMaterialIndex;
				int num5;
				bool flag9 = this.ValidateHtmlTag(textProcessingArray, num2 + 1, out num5, generationSettings, textInfo);
				if (!flag9)
				{
					goto IL_045A;
				}
				int stringIndex = textProcessingArray[num2].stringIndex;
				num2 = num5;
				bool flag10 = this.m_TextElementType == TextElementType.Sprite;
				if (flag10)
				{
					MaterialReference[] materialReferences = this.m_MaterialReferences;
					int currentMaterialIndex = this.m_CurrentMaterialIndex;
					materialReferences[currentMaterialIndex].referenceCount = materialReferences[currentMaterialIndex].referenceCount + 1;
					textInfo.textElementInfo[this.m_TotalCharacterCount].character = (char)(57344 + this.m_SpriteIndex);
					textInfo.textElementInfo[this.m_TotalCharacterCount].fontAsset = this.m_CurrentFontAsset;
					textInfo.textElementInfo[this.m_TotalCharacterCount].materialReferenceIndex = this.m_CurrentMaterialIndex;
					textInfo.textElementInfo[this.m_TotalCharacterCount].textElement = this.m_CurrentSpriteAsset.spriteCharacterTable[this.m_SpriteIndex];
					textInfo.textElementInfo[this.m_TotalCharacterCount].elementType = this.m_TextElementType;
					textInfo.textElementInfo[this.m_TotalCharacterCount].index = stringIndex;
					textInfo.textElementInfo[this.m_TotalCharacterCount].stringLength = textProcessingArray[num2].stringIndex - stringIndex + 1;
					this.m_TextElementType = TextElementType.Character;
					this.m_CurrentMaterialIndex = num4;
					num++;
					this.m_TotalCharacterCount++;
				}
				IL_0C98:
				num2++;
				continue;
				IL_045A:
				bool flag11 = false;
				FontAsset currentFontAsset = this.m_CurrentFontAsset;
				Material currentMaterial = this.m_CurrentMaterial;
				num4 = this.m_CurrentMaterialIndex;
				bool flag12 = this.m_TextElementType == TextElementType.Character;
				if (flag12)
				{
					bool flag13 = (this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase;
					if (flag13)
					{
						bool flag14 = char.IsLower((char)num3);
						if (flag14)
						{
							num3 = (uint)char.ToUpper((char)num3);
						}
					}
					else
					{
						bool flag15 = (this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase;
						if (flag15)
						{
							bool flag16 = char.IsUpper((char)num3);
							if (flag16)
							{
								num3 = (uint)char.ToLower((char)num3);
							}
						}
						else
						{
							bool flag17 = (this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps;
							if (flag17)
							{
								bool flag18 = char.IsLower((char)num3);
								if (flag18)
								{
									num3 = (uint)char.ToUpper((char)num3);
								}
							}
						}
					}
				}
				bool flag19;
				TextElement textElement = this.GetTextElement(generationSettings, num3, this.m_CurrentFontAsset, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag19);
				bool flag20 = textElement == null;
				if (flag20)
				{
					this.DoMissingGlyphCallback(num3, textProcessingArray[num2].stringIndex, this.m_CurrentFontAsset, textInfo);
					uint num6 = num3;
					num3 = (textProcessingArray[num2].unicode = (uint)((textSettings.missingCharacterUnicode == 0) ? 9633 : textSettings.missingCharacterUnicode));
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag19);
					bool flag21 = textElement == null;
					if (flag21)
					{
						bool flag22 = textSettings.fallbackFontAssets != null && textSettings.fallbackFontAssets.Count > 0;
						if (flag22)
						{
							textElement = FontAssetUtilities.GetCharacterFromFontAssets(num3, this.m_CurrentFontAsset, textSettings.fallbackFontAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag19);
						}
					}
					bool flag23 = textElement == null;
					if (flag23)
					{
						bool flag24 = textSettings.defaultFontAsset != null;
						if (flag24)
						{
							textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, textSettings.defaultFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag19);
						}
					}
					bool flag25 = textElement == null;
					if (flag25)
					{
						num3 = (textProcessingArray[num2].unicode = 32U);
						textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag19);
					}
					bool flag26 = textElement == null;
					if (flag26)
					{
						num3 = (textProcessingArray[num2].unicode = 3U);
						textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag19);
					}
					bool displayWarnings2 = textSettings.displayWarnings;
					if (displayWarnings2)
					{
						string text = ((num6 > 65535U) ? string.Format("The character with Unicode value \\U{0:X8} was not found in the [{1}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{2:X4}.", num6, generationSettings.fontAsset.name, textElement.unicode) : string.Format("The character with Unicode value \\u{0:X4} was not found in the [{1}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{2:X4}.", num6, generationSettings.fontAsset.name, textElement.unicode));
						Debug.LogWarning(text);
					}
				}
				textInfo.textElementInfo[this.m_TotalCharacterCount].alternativeGlyph = null;
				bool flag27 = textElement.elementType == TextElementType.Character;
				if (flag27)
				{
					bool flag28 = textElement.textAsset.instanceID != this.m_CurrentFontAsset.instanceID;
					if (flag28)
					{
						flag11 = true;
						this.m_CurrentFontAsset = textElement.textAsset as FontAsset;
					}
					List<LigatureSubstitutionRecord> list;
					bool flag29 = this.m_CurrentFontAsset.fontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(textElement.glyphIndex, out list);
					if (flag29)
					{
						bool flag30 = list == null;
						if (flag30)
						{
							break;
						}
						for (int i = 0; i < list.Count; i++)
						{
							LigatureSubstitutionRecord ligatureSubstitutionRecord = list[i];
							int num7 = ligatureSubstitutionRecord.componentGlyphIDs.Length;
							uint num8 = ligatureSubstitutionRecord.ligatureGlyphID;
							for (int j = 1; j < num7; j++)
							{
								uint glyphIndex = this.m_CurrentFontAsset.GetGlyphIndex(textProcessingArray[num2 + j].unicode);
								bool flag31 = glyphIndex == ligatureSubstitutionRecord.componentGlyphIDs[j];
								if (!flag31)
								{
									num8 = 0U;
									break;
								}
							}
							bool flag32 = num8 > 0U;
							if (flag32)
							{
								Glyph glyph;
								bool flag33 = this.m_CurrentFontAsset.TryAddGlyphInternal(num8, out glyph);
								if (flag33)
								{
									textInfo.textElementInfo[this.m_TotalCharacterCount].alternativeGlyph = glyph;
									for (int k = 0; k < num7; k++)
									{
										bool flag34 = k == 0;
										if (flag34)
										{
											textProcessingArray[num2 + k].length = num7;
										}
										else
										{
											textProcessingArray[num2 + k].unicode = 26U;
										}
									}
									num2 += num7 - 1;
									break;
								}
							}
						}
					}
				}
				textInfo.textElementInfo[this.m_TotalCharacterCount].elementType = TextElementType.Character;
				textInfo.textElementInfo[this.m_TotalCharacterCount].textElement = textElement;
				textInfo.textElementInfo[this.m_TotalCharacterCount].isUsingAlternateTypeface = flag19;
				textInfo.textElementInfo[this.m_TotalCharacterCount].character = (char)num3;
				textInfo.textElementInfo[this.m_TotalCharacterCount].index = textProcessingArray[num2].stringIndex;
				textInfo.textElementInfo[this.m_TotalCharacterCount].stringLength = textProcessingArray[num2].length;
				textInfo.textElementInfo[this.m_TotalCharacterCount].fontAsset = this.m_CurrentFontAsset;
				bool flag35 = textElement.elementType == TextElementType.Sprite;
				if (flag35)
				{
					SpriteAsset spriteAsset = textElement.textAsset as SpriteAsset;
					this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(spriteAsset.material, spriteAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
					MaterialReference[] materialReferences2 = this.m_MaterialReferences;
					int currentMaterialIndex2 = this.m_CurrentMaterialIndex;
					materialReferences2[currentMaterialIndex2].referenceCount = materialReferences2[currentMaterialIndex2].referenceCount + 1;
					textInfo.textElementInfo[this.m_TotalCharacterCount].elementType = TextElementType.Sprite;
					textInfo.textElementInfo[this.m_TotalCharacterCount].materialReferenceIndex = this.m_CurrentMaterialIndex;
					this.m_TextElementType = TextElementType.Character;
					this.m_CurrentMaterialIndex = num4;
					num++;
					this.m_TotalCharacterCount++;
					goto IL_0C98;
				}
				bool flag36 = flag11 && this.m_CurrentFontAsset.instanceID != generationSettings.fontAsset.instanceID;
				if (flag36)
				{
					bool matchMaterialPreset = textSettings.matchMaterialPreset;
					if (matchMaterialPreset)
					{
						this.m_CurrentMaterial = MaterialManager.GetFallbackMaterial(this.m_CurrentMaterial, this.m_CurrentFontAsset.material);
					}
					else
					{
						this.m_CurrentMaterial = this.m_CurrentFontAsset.material;
					}
					this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
				}
				bool flag37 = textElement != null && textElement.glyph.atlasIndex > 0;
				if (flag37)
				{
					this.m_CurrentMaterial = MaterialManager.GetFallbackMaterial(this.m_CurrentFontAsset, this.m_CurrentMaterial, textElement.glyph.atlasIndex);
					this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
					flag11 = true;
				}
				bool flag38 = !char.IsWhiteSpace((char)num3) && num3 != 8203U;
				if (flag38)
				{
					bool flag39 = this.m_MaterialReferences[this.m_CurrentMaterialIndex].referenceCount < 16383;
					if (flag39)
					{
						MaterialReference[] materialReferences3 = this.m_MaterialReferences;
						int currentMaterialIndex3 = this.m_CurrentMaterialIndex;
						materialReferences3[currentMaterialIndex3].referenceCount = materialReferences3[currentMaterialIndex3].referenceCount + 1;
					}
					else
					{
						this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(new Material(this.m_CurrentMaterial), this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
						MaterialReference[] materialReferences4 = this.m_MaterialReferences;
						int currentMaterialIndex4 = this.m_CurrentMaterialIndex;
						materialReferences4[currentMaterialIndex4].referenceCount = materialReferences4[currentMaterialIndex4].referenceCount + 1;
					}
				}
				textInfo.textElementInfo[this.m_TotalCharacterCount].material = this.m_CurrentMaterial;
				textInfo.textElementInfo[this.m_TotalCharacterCount].materialReferenceIndex = this.m_CurrentMaterialIndex;
				this.m_MaterialReferences[this.m_CurrentMaterialIndex].isFallbackMaterial = flag11;
				bool flag40 = flag11;
				if (flag40)
				{
					this.m_MaterialReferences[this.m_CurrentMaterialIndex].fallbackMaterial = currentMaterial;
					this.m_CurrentFontAsset = currentFontAsset;
					this.m_CurrentMaterial = currentMaterial;
					this.m_CurrentMaterialIndex = num4;
				}
				this.m_TotalCharacterCount++;
				goto IL_0C98;
			}
			bool isCalculatingPreferredValues = this.m_IsCalculatingPreferredValues;
			int num9;
			if (isCalculatingPreferredValues)
			{
				this.m_IsCalculatingPreferredValues = false;
				num9 = this.m_TotalCharacterCount;
			}
			else
			{
				textInfo.spriteCount = num;
				int num10 = (textInfo.materialCount = this.m_MaterialReferenceIndexLookup.Count);
				bool flag41 = num10 > textInfo.meshInfo.Length;
				if (flag41)
				{
					TextInfo.Resize<MeshInfo>(ref textInfo.meshInfo, num10, false);
				}
				bool flag42 = this.m_VertexBufferAutoSizeReduction && textInfo.textElementInfo.Length - this.m_TotalCharacterCount > 256;
				if (flag42)
				{
					TextInfo.Resize<TextElementInfo>(ref textInfo.textElementInfo, Mathf.Max(this.m_TotalCharacterCount + 1, 256), true);
				}
				for (int l = 0; l < num10; l++)
				{
					int referenceCount = this.m_MaterialReferences[l].referenceCount;
					bool flag43 = textInfo.meshInfo[l].vertices == null || textInfo.meshInfo[l].vertices.Length < referenceCount * 4;
					if (flag43)
					{
						bool flag44 = textInfo.meshInfo[l].vertices == null;
						if (flag44)
						{
							textInfo.meshInfo[l] = new MeshInfo(referenceCount + 1);
						}
						else
						{
							textInfo.meshInfo[l].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.NextPowerOfTwo(referenceCount));
						}
					}
					else
					{
						bool flag45 = textInfo.meshInfo[l].vertices.Length - referenceCount * 4 > 1024;
						if (flag45)
						{
							textInfo.meshInfo[l].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.Max(Mathf.NextPowerOfTwo(referenceCount), 256));
						}
					}
					textInfo.meshInfo[l].material = this.m_MaterialReferences[l].material;
					textInfo.meshInfo[l].glyphRenderMode = this.m_MaterialReferences[l].fontAsset.atlasRenderMode;
				}
				num9 = this.m_TotalCharacterCount;
			}
			return num9;
		}

		internal TextElement GetTextElement(TextGenerationSettings generationSettings, uint unicode, FontAsset fontAsset, FontStyles fontStyle, TextFontWeight fontWeight, out bool isUsingAlternativeTypeface)
		{
			TextSettings textSettings = generationSettings.textSettings;
			Character character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, fontAsset, false, fontStyle, fontWeight, out isUsingAlternativeTypeface);
			bool flag = character != null;
			TextElement textElement;
			if (flag)
			{
				textElement = character;
			}
			else
			{
				bool flag2 = fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0;
				if (flag2)
				{
					character = FontAssetUtilities.GetCharacterFromFontAssets(unicode, fontAsset, fontAsset.m_FallbackFontAssetTable, true, fontStyle, fontWeight, out isUsingAlternativeTypeface);
				}
				bool flag3 = character != null;
				if (flag3)
				{
					fontAsset.AddCharacterToLookupCache(unicode, character);
					textElement = character;
				}
				else
				{
					bool flag4 = fontAsset.instanceID != generationSettings.fontAsset.instanceID;
					if (flag4)
					{
						character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, generationSettings.fontAsset, false, fontStyle, fontWeight, out isUsingAlternativeTypeface);
						bool flag5 = character != null;
						if (flag5)
						{
							this.m_CurrentMaterialIndex = 0;
							this.m_CurrentMaterial = this.m_MaterialReferences[0].material;
							fontAsset.AddCharacterToLookupCache(unicode, character);
							return character;
						}
						bool flag6 = generationSettings.fontAsset.m_FallbackFontAssetTable != null && generationSettings.fontAsset.m_FallbackFontAssetTable.Count > 0;
						if (flag6)
						{
							character = FontAssetUtilities.GetCharacterFromFontAssets(unicode, fontAsset, generationSettings.fontAsset.m_FallbackFontAssetTable, true, fontStyle, fontWeight, out isUsingAlternativeTypeface);
						}
						bool flag7 = character != null;
						if (flag7)
						{
							fontAsset.AddCharacterToLookupCache(unicode, character);
							return character;
						}
					}
					bool flag8 = generationSettings.spriteAsset != null;
					if (flag8)
					{
						SpriteCharacter spriteCharacterFromSpriteAsset = FontAssetUtilities.GetSpriteCharacterFromSpriteAsset(unicode, generationSettings.spriteAsset, true);
						bool flag9 = spriteCharacterFromSpriteAsset != null;
						if (flag9)
						{
							return spriteCharacterFromSpriteAsset;
						}
					}
					bool flag10 = textSettings.fallbackFontAssets != null && textSettings.fallbackFontAssets.Count > 0;
					if (flag10)
					{
						character = FontAssetUtilities.GetCharacterFromFontAssets(unicode, fontAsset, textSettings.fallbackFontAssets, true, fontStyle, fontWeight, out isUsingAlternativeTypeface);
					}
					bool flag11 = character != null;
					if (flag11)
					{
						fontAsset.AddCharacterToLookupCache(unicode, character);
						textElement = character;
					}
					else
					{
						bool flag12 = textSettings.defaultFontAsset != null;
						if (flag12)
						{
							character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, textSettings.defaultFontAsset, true, fontStyle, fontWeight, out isUsingAlternativeTypeface);
						}
						bool flag13 = character != null;
						if (flag13)
						{
							fontAsset.AddCharacterToLookupCache(unicode, character);
							textElement = character;
						}
						else
						{
							bool flag14 = textSettings.defaultSpriteAsset != null;
							if (flag14)
							{
								SpriteCharacter spriteCharacterFromSpriteAsset2 = FontAssetUtilities.GetSpriteCharacterFromSpriteAsset(unicode, textSettings.defaultSpriteAsset, true);
								bool flag15 = spriteCharacterFromSpriteAsset2 != null;
								if (flag15)
								{
									return spriteCharacterFromSpriteAsset2;
								}
							}
							textElement = null;
						}
					}
				}
			}
			return textElement;
		}

		private void ComputeMarginSize(Rect rect, Vector4 margins)
		{
			this.m_MarginWidth = rect.width - margins.x - margins.z;
			this.m_MarginHeight = rect.height - margins.y - margins.w;
			this.m_RectTransformCorners[0].x = 0f;
			this.m_RectTransformCorners[0].y = 0f;
			this.m_RectTransformCorners[1].x = 0f;
			this.m_RectTransformCorners[1].y = rect.height;
			this.m_RectTransformCorners[2].x = rect.width;
			this.m_RectTransformCorners[2].y = rect.height;
			this.m_RectTransformCorners[3].x = rect.width;
			this.m_RectTransformCorners[3].y = 0f;
		}

		protected void GetSpecialCharacters(TextGenerationSettings generationSettings)
		{
			this.GetEllipsisSpecialCharacter(generationSettings);
			this.GetUnderlineSpecialCharacter(generationSettings);
		}

		protected void GetEllipsisSpecialCharacter(TextGenerationSettings generationSettings)
		{
			FontAsset fontAsset = this.m_CurrentFontAsset ?? generationSettings.fontAsset;
			TextSettings textSettings = generationSettings.textSettings;
			bool flag;
			Character character = FontAssetUtilities.GetCharacterFromFontAsset(8230U, fontAsset, false, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
			bool flag2 = character == null;
			if (flag2)
			{
				bool flag3 = fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0;
				if (flag3)
				{
					character = FontAssetUtilities.GetCharacterFromFontAssets(8230U, fontAsset, fontAsset.m_FallbackFontAssetTable, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
				}
			}
			bool flag4 = character == null;
			if (flag4)
			{
				bool flag5 = textSettings.fallbackFontAssets != null && textSettings.fallbackFontAssets.Count > 0;
				if (flag5)
				{
					character = FontAssetUtilities.GetCharacterFromFontAssets(8230U, fontAsset, textSettings.fallbackFontAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
				}
			}
			bool flag6 = character == null;
			if (flag6)
			{
				bool flag7 = textSettings.defaultFontAsset != null;
				if (flag7)
				{
					character = FontAssetUtilities.GetCharacterFromFontAsset(8230U, textSettings.defaultFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
				}
			}
			bool flag8 = character != null;
			if (flag8)
			{
				this.m_Ellipsis = new TextGenerator.SpecialCharacter(character, 0);
			}
		}

		protected void GetUnderlineSpecialCharacter(TextGenerationSettings generationSettings)
		{
			FontAsset fontAsset = this.m_CurrentFontAsset ?? generationSettings.fontAsset;
			TextSettings textSettings = generationSettings.textSettings;
			bool flag;
			Character characterFromFontAsset = FontAssetUtilities.GetCharacterFromFontAsset(95U, fontAsset, false, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
			bool flag2 = characterFromFontAsset != null;
			if (flag2)
			{
				this.m_Underline = new TextGenerator.SpecialCharacter(characterFromFontAsset, this.m_CurrentMaterialIndex);
			}
		}

		private float GetPreferredWidthInternal(TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			bool flag = generationSettings.textSettings == null;
			float num;
			if (flag)
			{
				num = 0f;
			}
			else
			{
				float num2 = (generationSettings.autoSize ? generationSettings.fontSizeMax : this.m_FontSize);
				this.m_MinFontSize = generationSettings.fontSizeMin;
				this.m_MaxFontSize = generationSettings.fontSizeMax;
				this.m_CharWidthAdjDelta = 0f;
				Vector2 largePositiveVector = TextGeneratorUtilities.largePositiveVector2;
				TextWrappingMode textWrappingMode = (generationSettings.wordWrap ? TextWrappingMode.NoWrap : TextWrappingMode.PreserveWhitespaceNoWrap);
				this.m_AutoSizeIterationCount = 0;
				float x = this.CalculatePreferredValues(ref num2, largePositiveVector, true, textWrappingMode, generationSettings, textInfo).x;
				num = x;
			}
			return num;
		}

		private float GetPreferredHeightInternal(TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			bool flag = generationSettings.textSettings == null;
			float num;
			if (flag)
			{
				num = 0f;
			}
			else
			{
				float num2 = (generationSettings.autoSize ? generationSettings.fontSizeMax : this.m_FontSize);
				this.m_MinFontSize = generationSettings.fontSizeMin;
				this.m_MaxFontSize = generationSettings.fontSizeMax;
				this.m_CharWidthAdjDelta = 0f;
				Vector2 vector = new Vector2((this.m_MarginWidth != 0f) ? this.m_MarginWidth : 32767f, 32767f);
				this.m_IsAutoSizePointSizeSet = false;
				this.m_AutoSizeIterationCount = 0;
				float num3 = 0f;
				TextWrappingMode textWrappingMode = (generationSettings.wordWrap ? TextWrappingMode.Normal : TextWrappingMode.NoWrap);
				while (!this.m_IsAutoSizePointSizeSet)
				{
					num3 = this.CalculatePreferredValues(ref num2, vector, generationSettings.autoSize, textWrappingMode, generationSettings, textInfo).y;
					this.m_AutoSizeIterationCount++;
				}
				num = num3;
			}
			return num;
		}

		private Vector2 GetPreferredValuesInternal(TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			bool flag = generationSettings.textSettings == null;
			Vector2 vector;
			if (flag)
			{
				vector = Vector2.zero;
			}
			else
			{
				float num = (generationSettings.autoSize ? generationSettings.fontSizeMax : this.m_FontSize);
				this.m_MinFontSize = generationSettings.fontSizeMin;
				this.m_MaxFontSize = generationSettings.fontSizeMax;
				this.m_CharWidthAdjDelta = 0f;
				Vector2 vector2 = new Vector2((this.m_MarginWidth != 0f) ? this.m_MarginWidth : 32767f, (this.m_MarginHeight != 0f) ? this.m_MarginHeight : 32767f);
				TextWrappingMode textWrappingMode = (generationSettings.wordWrap ? TextWrappingMode.Normal : TextWrappingMode.NoWrap);
				this.m_AutoSizeIterationCount = 0;
				vector = this.CalculatePreferredValues(ref num, vector2, generationSettings.autoSize, textWrappingMode, generationSettings, textInfo);
			}
			return vector;
		}

		protected virtual Vector2 CalculatePreferredValues(ref float fontSize, Vector2 marginSize, bool isTextAutoSizingEnabled, TextWrappingMode textWrapMode, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			bool flag = generationSettings.fontAsset == null || generationSettings.fontAsset.characterLookupTable == null;
			Vector2 vector;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned.");
				this.m_IsAutoSizePointSizeSet = true;
				vector = Vector2.zero;
			}
			else
			{
				bool flag2 = this.m_TextProcessingArray == null || this.m_TextProcessingArray.Length == 0 || this.m_TextProcessingArray[0].unicode == 0U;
				if (flag2)
				{
					this.m_IsAutoSizePointSizeSet = true;
					vector = Vector2.zero;
				}
				else
				{
					this.m_CurrentFontAsset = generationSettings.fontAsset;
					this.m_CurrentMaterial = generationSettings.material;
					this.m_CurrentMaterialIndex = 0;
					this.m_MaterialReferenceStack.SetDefault(new MaterialReference(0, this.m_CurrentFontAsset, null, this.m_CurrentMaterial, this.m_Padding));
					int totalCharacterCount = this.m_TotalCharacterCount;
					bool flag3 = this.m_InternalTextElementInfo == null || totalCharacterCount > this.m_InternalTextElementInfo.Length;
					if (flag3)
					{
						this.m_InternalTextElementInfo = new TextElementInfo[(totalCharacterCount > 1024) ? (totalCharacterCount + 256) : Mathf.NextPowerOfTwo(totalCharacterCount)];
					}
					float num = fontSize / (float)generationSettings.fontAsset.faceInfo.pointSize * generationSettings.fontAsset.faceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
					float num2 = num;
					float num3 = fontSize * 0.01f * (generationSettings.isOrthographic ? 1f : 0.1f);
					this.m_FontScaleMultiplier = 1f;
					this.m_CurrentFontSize = fontSize;
					this.m_SizeStack.SetDefault(this.m_CurrentFontSize);
					this.m_FontStyleInternal = generationSettings.fontStyle;
					this.m_LineJustification = generationSettings.textAlignment;
					this.m_LineJustificationStack.SetDefault(this.m_LineJustification);
					this.m_BaselineOffset = 0f;
					this.m_BaselineOffsetStack.Clear();
					this.m_FXScale = Vector3.one;
					this.m_LineOffset = 0f;
					this.m_LineHeight = -32767f;
					float num4 = this.m_CurrentFontAsset.faceInfo.lineHeight - (this.m_CurrentFontAsset.faceInfo.ascentLine - this.m_CurrentFontAsset.faceInfo.descentLine);
					this.m_CSpacing = 0f;
					this.m_MonoSpacing = 0f;
					this.m_XAdvance = 0f;
					this.m_TagLineIndent = 0f;
					this.m_TagIndent = 0f;
					this.m_IndentStack.SetDefault(0f);
					this.m_TagNoParsing = false;
					this.m_CharacterCount = 0;
					this.m_FirstCharacterOfLine = 0;
					this.m_MaxLineAscender = -32767f;
					this.m_MaxLineDescender = 32767f;
					this.m_LineNumber = 0;
					this.m_StartOfLineAscender = 0f;
					this.m_IsDrivenLineSpacing = false;
					this.m_LastBaseGlyphIndex = int.MinValue;
					TextSettings textSettings = generationSettings.textSettings;
					float x = marginSize.x;
					float y = marginSize.y;
					this.m_MarginLeft = 0f;
					this.m_MarginRight = 0f;
					this.m_Width = -1f;
					float num5 = x + 0.0001f - this.m_MarginLeft - this.m_MarginRight;
					float num6 = 0f;
					float num7 = 0f;
					this.m_IsCalculatingPreferredValues = true;
					this.m_MaxCapHeight = 0f;
					this.m_MaxAscender = 0f;
					this.m_MaxDescender = 0f;
					bool flag4 = false;
					bool flag5 = true;
					this.m_IsNonBreakingSpace = false;
					bool flag6 = false;
					CharacterSubstitution characterSubstitution = new CharacterSubstitution(-1, 0U);
					bool flag7 = false;
					WordWrapState wordWrapState = default(WordWrapState);
					WordWrapState wordWrapState2 = default(WordWrapState);
					WordWrapState wordWrapState3 = default(WordWrapState);
					TextGenerator.m_IsTextTruncated = false;
					this.m_AutoSizeIterationCount++;
					int num8 = 0;
					while (num8 < this.m_TextProcessingArray.Length && this.m_TextProcessingArray[num8].unicode > 0U)
					{
						uint num9 = this.m_TextProcessingArray[num8].unicode;
						bool flag8 = num9 == 26U;
						if (!flag8)
						{
							bool flag9 = generationSettings.richText && num9 == 60U;
							if (flag9)
							{
								this.m_isTextLayoutPhase = true;
								this.m_TextElementType = TextElementType.Character;
								int num10;
								bool flag10 = this.ValidateHtmlTag(this.m_TextProcessingArray, num8 + 1, out num10, generationSettings, textInfo);
								if (flag10)
								{
									num8 = num10;
									bool flag11 = this.m_TextElementType == TextElementType.Character;
									if (flag11)
									{
										goto IL_20EB;
									}
								}
							}
							else
							{
								this.m_TextElementType = textInfo.textElementInfo[this.m_CharacterCount].elementType;
								this.m_CurrentMaterialIndex = textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex;
								this.m_CurrentFontAsset = textInfo.textElementInfo[this.m_CharacterCount].fontAsset;
							}
							int currentMaterialIndex = this.m_CurrentMaterialIndex;
							bool isUsingAlternateTypeface = textInfo.textElementInfo[this.m_CharacterCount].isUsingAlternateTypeface;
							this.m_isTextLayoutPhase = false;
							bool flag12 = false;
							bool flag13 = characterSubstitution.index == this.m_CharacterCount;
							if (flag13)
							{
								num9 = characterSubstitution.unicode;
								this.m_TextElementType = TextElementType.Character;
								flag12 = true;
								uint num11 = num9;
								uint num12 = num11;
								if (num12 != 3U)
								{
									if (num12 != 45U)
									{
										if (num12 == 8230U)
										{
											this.m_InternalTextElementInfo[this.m_CharacterCount].textElement = this.m_Ellipsis.character;
											this.m_InternalTextElementInfo[this.m_CharacterCount].elementType = TextElementType.Character;
											this.m_InternalTextElementInfo[this.m_CharacterCount].fontAsset = this.m_Ellipsis.fontAsset;
											this.m_InternalTextElementInfo[this.m_CharacterCount].material = this.m_Ellipsis.material;
											this.m_InternalTextElementInfo[this.m_CharacterCount].materialReferenceIndex = this.m_Ellipsis.materialIndex;
											TextGenerator.m_IsTextTruncated = true;
											characterSubstitution.index = this.m_CharacterCount + 1;
											characterSubstitution.unicode = 3U;
										}
									}
								}
								else
								{
									this.m_InternalTextElementInfo[this.m_CharacterCount].textElement = this.m_CurrentFontAsset.characterLookupTable[3U];
									TextGenerator.m_IsTextTruncated = true;
								}
							}
							bool flag14 = this.m_CharacterCount < generationSettings.firstVisibleCharacter && num9 != 3U;
							if (flag14)
							{
								this.m_InternalTextElementInfo[this.m_CharacterCount].isVisible = false;
								this.m_InternalTextElementInfo[this.m_CharacterCount].character = '\u200b';
								this.m_InternalTextElementInfo[this.m_CharacterCount].lineNumber = 0;
								this.m_CharacterCount++;
							}
							else
							{
								float num13 = 1f;
								bool flag15 = this.m_TextElementType == TextElementType.Character;
								if (flag15)
								{
									bool flag16 = (this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase;
									if (flag16)
									{
										bool flag17 = char.IsLower((char)num9);
										if (flag17)
										{
											num9 = (uint)char.ToUpper((char)num9);
										}
									}
									else
									{
										bool flag18 = (this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase;
										if (flag18)
										{
											bool flag19 = char.IsUpper((char)num9);
											if (flag19)
											{
												num9 = (uint)char.ToLower((char)num9);
											}
										}
										else
										{
											bool flag20 = (this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps;
											if (flag20)
											{
												bool flag21 = char.IsLower((char)num9);
												if (flag21)
												{
													num13 = 0.8f;
													num9 = (uint)char.ToUpper((char)num9);
												}
											}
										}
									}
								}
								float num14 = 0f;
								float num15 = 0f;
								float num16 = 0f;
								bool flag22 = this.m_TextElementType == TextElementType.Sprite;
								if (flag22)
								{
									SpriteCharacter spriteCharacter = (SpriteCharacter)textInfo.textElementInfo[this.m_CharacterCount].textElement;
									this.m_CurrentSpriteAsset = spriteCharacter.textAsset as SpriteAsset;
									this.m_SpriteIndex = (int)spriteCharacter.glyphIndex;
									bool flag23 = spriteCharacter == null;
									if (flag23)
									{
										goto IL_20EB;
									}
									bool flag24 = num9 == 60U;
									if (flag24)
									{
										num9 = (uint)(57344 + this.m_SpriteIndex);
									}
									bool flag25 = this.m_CurrentSpriteAsset.faceInfo.pointSize > 0;
									if (flag25)
									{
										float num17 = this.m_CurrentFontSize / (float)this.m_CurrentSpriteAsset.faceInfo.pointSize * this.m_CurrentSpriteAsset.faceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										num2 = spriteCharacter.scale * spriteCharacter.glyph.scale * num17;
										num15 = this.m_CurrentSpriteAsset.faceInfo.ascentLine;
										num16 = this.m_CurrentSpriteAsset.faceInfo.descentLine;
									}
									else
									{
										float num18 = this.m_CurrentFontSize / (float)this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										num2 = this.m_CurrentFontAsset.faceInfo.ascentLine / spriteCharacter.glyph.metrics.height * spriteCharacter.scale * spriteCharacter.glyph.scale * num18;
										float num19 = num18 / num2;
										num15 = this.m_CurrentFontAsset.faceInfo.ascentLine * num19;
										num16 = this.m_CurrentFontAsset.faceInfo.descentLine * num19;
									}
									this.m_CachedTextElement = spriteCharacter;
									this.m_InternalTextElementInfo[this.m_CharacterCount].elementType = TextElementType.Sprite;
									this.m_InternalTextElementInfo[this.m_CharacterCount].scale = num2;
									this.m_CurrentMaterialIndex = currentMaterialIndex;
								}
								else
								{
									bool flag26 = this.m_TextElementType == TextElementType.Character;
									if (flag26)
									{
										this.m_CachedTextElement = textInfo.textElementInfo[this.m_CharacterCount].textElement;
										bool flag27 = this.m_CachedTextElement == null;
										if (flag27)
										{
											goto IL_20EB;
										}
										this.m_CurrentFontAsset = textInfo.textElementInfo[this.m_CharacterCount].fontAsset;
										this.m_CurrentMaterial = textInfo.textElementInfo[this.m_CharacterCount].material;
										this.m_CurrentMaterialIndex = textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex;
										bool flag28 = flag12 && this.m_TextProcessingArray[num8].unicode == 10U && this.m_CharacterCount != this.m_FirstCharacterOfLine;
										float num20;
										if (flag28)
										{
											num20 = textInfo.textElementInfo[this.m_CharacterCount - 1].pointSize * num13 / (float)this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										}
										else
										{
											num20 = this.m_CurrentFontSize * num13 / (float)this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale * (generationSettings.isOrthographic ? 1f : 0.1f);
										}
										bool flag29 = flag12 && num9 == 8230U;
										if (flag29)
										{
											num15 = 0f;
											num16 = 0f;
										}
										else
										{
											num15 = this.m_CurrentFontAsset.m_FaceInfo.ascentLine;
											num16 = this.m_CurrentFontAsset.m_FaceInfo.descentLine;
										}
										num2 = num20 * this.m_FontScaleMultiplier * this.m_CachedTextElement.scale;
										this.m_InternalTextElementInfo[this.m_CharacterCount].elementType = TextElementType.Character;
									}
								}
								float num21 = num2;
								bool flag30 = num9 == 173U || num9 == 3U;
								if (flag30)
								{
									num2 = 0f;
								}
								this.m_InternalTextElementInfo[this.m_CharacterCount].character = (char)num9;
								Glyph alternativeGlyph = textInfo.textElementInfo[this.m_CharacterCount].alternativeGlyph;
								GlyphMetrics glyphMetrics = ((alternativeGlyph == null) ? this.m_CachedTextElement.m_Glyph.metrics : alternativeGlyph.metrics);
								bool flag31 = num9 <= 65535U && char.IsWhiteSpace((char)num9);
								GlyphValueRecord glyphValueRecord = default(GlyphValueRecord);
								float num22 = generationSettings.characterSpacing;
								bool enableKerning = generationSettings.enableKerning;
								if (enableKerning)
								{
									uint glyphIndex = this.m_CachedTextElement.m_GlyphIndex;
									bool flag32 = this.m_CharacterCount < totalCharacterCount - 1;
									if (flag32)
									{
										uint glyphIndex2 = textInfo.textElementInfo[this.m_CharacterCount + 1].textElement.m_GlyphIndex;
										uint num23 = (glyphIndex2 << 16) | glyphIndex;
										GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
										bool flag33 = this.m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num23, out glyphPairAdjustmentRecord);
										if (flag33)
										{
											glyphValueRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphValueRecord;
											num22 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num22);
										}
									}
									bool flag34 = this.m_CharacterCount >= 1;
									if (flag34)
									{
										uint glyphIndex3 = textInfo.textElementInfo[this.m_CharacterCount - 1].textElement.m_GlyphIndex;
										uint num24 = (glyphIndex << 16) | glyphIndex3;
										GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
										bool flag35 = this.m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num24, out glyphPairAdjustmentRecord);
										if (flag35)
										{
											glyphValueRecord += glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphValueRecord;
											num22 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num22);
										}
									}
									this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedHorizontalAdvance = glyphValueRecord.xAdvance;
								}
								bool flag36 = TextGeneratorUtilities.IsBaseGlyph(num9);
								bool flag37 = flag36;
								if (flag37)
								{
									this.m_LastBaseGlyphIndex = this.m_CharacterCount;
								}
								bool flag38 = this.m_CharacterCount > 0 && !flag36;
								if (flag38)
								{
									bool flag39 = this.m_LastBaseGlyphIndex != int.MinValue && this.m_LastBaseGlyphIndex == this.m_CharacterCount - 1;
									if (flag39)
									{
										Glyph glyph = textInfo.textElementInfo[this.m_LastBaseGlyphIndex].textElement.glyph;
										uint index = glyph.index;
										uint glyphIndex4 = this.m_CachedTextElement.glyphIndex;
										uint num25 = (glyphIndex4 << 16) | index;
										MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord;
										bool flag40 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num25, out markToBaseAdjustmentRecord);
										if (flag40)
										{
											float num26 = (this.m_InternalTextElementInfo[this.m_LastBaseGlyphIndex].origin - this.m_XAdvance) / num2;
											glyphValueRecord.xPlacement = num26 + markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment;
											glyphValueRecord.yPlacement = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment;
											num22 = 0f;
										}
									}
									else
									{
										bool flag41 = false;
										int num27 = this.m_CharacterCount - 1;
										while (num27 >= 0 && num27 != this.m_LastBaseGlyphIndex)
										{
											Glyph glyph2 = textInfo.textElementInfo[num27].textElement.glyph;
											uint index2 = glyph2.index;
											uint glyphIndex5 = this.m_CachedTextElement.glyphIndex;
											uint num28 = (glyphIndex5 << 16) | index2;
											MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord;
											bool flag42 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryGetValue(num28, out markToMarkAdjustmentRecord);
											if (flag42)
											{
												float num29 = (textInfo.textElementInfo[num27].origin - this.m_XAdvance) / num2;
												float num30 = num14 - this.m_LineOffset + this.m_BaselineOffset;
												float num31 = (this.m_InternalTextElementInfo[num27].baseLine - num30) / num2;
												glyphValueRecord.xPlacement = num29 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment;
												glyphValueRecord.yPlacement = num31 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment;
												num22 = 0f;
												flag41 = true;
												break;
											}
											num27--;
										}
										bool flag43 = this.m_LastBaseGlyphIndex != int.MinValue && !flag41;
										if (flag43)
										{
											Glyph glyph3 = textInfo.textElementInfo[this.m_LastBaseGlyphIndex].textElement.glyph;
											uint index3 = glyph3.index;
											uint glyphIndex6 = this.m_CachedTextElement.glyphIndex;
											uint num32 = (glyphIndex6 << 16) | index3;
											MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2;
											bool flag44 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num32, out markToBaseAdjustmentRecord2);
											if (flag44)
											{
												float num33 = (this.m_InternalTextElementInfo[this.m_LastBaseGlyphIndex].origin - this.m_XAdvance) / num2;
												glyphValueRecord.xPlacement = num33 + markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.xPositionAdjustment;
												glyphValueRecord.yPlacement = markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.yPositionAdjustment;
												num22 = 0f;
											}
										}
									}
								}
								num15 += glyphValueRecord.yPlacement;
								num16 += glyphValueRecord.yPlacement;
								float num34 = 0f;
								bool flag45 = this.m_MonoSpacing != 0f;
								if (flag45)
								{
									num34 = (this.m_MonoSpacing / 2f - (this.m_CachedTextElement.glyph.metrics.width / 2f + this.m_CachedTextElement.glyph.metrics.horizontalBearingX) * num2) * (1f - this.m_CharWidthAdjDelta);
									this.m_XAdvance += num34;
								}
								float num35 = 0f;
								bool flag46 = this.m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold;
								if (flag46)
								{
									num35 = this.m_CurrentFontAsset.boldStyleSpacing;
								}
								this.m_InternalTextElementInfo[this.m_CharacterCount].origin = this.m_XAdvance + glyphValueRecord.xPlacement * num2;
								this.m_InternalTextElementInfo[this.m_CharacterCount].baseLine = num14 - this.m_LineOffset + this.m_BaselineOffset + glyphValueRecord.yPlacement * num2;
								float num36 = ((this.m_TextElementType == TextElementType.Character) ? (num15 * num2 / num13 + this.m_BaselineOffset) : (num15 * num2 + this.m_BaselineOffset));
								float num37 = ((this.m_TextElementType == TextElementType.Character) ? (num16 * num2 / num13 + this.m_BaselineOffset) : (num16 * num2 + this.m_BaselineOffset));
								float num38 = num36;
								float num39 = num37;
								bool flag47 = this.m_CharacterCount == this.m_FirstCharacterOfLine;
								bool flag48 = flag47 || !flag31;
								if (flag48)
								{
									bool flag49 = this.m_BaselineOffset != 0f;
									if (flag49)
									{
										num38 = Mathf.Max((num36 - this.m_BaselineOffset) / this.m_FontScaleMultiplier, num38);
										num39 = Mathf.Min((num37 - this.m_BaselineOffset) / this.m_FontScaleMultiplier, num39);
									}
									this.m_MaxLineAscender = Mathf.Max(num38, this.m_MaxLineAscender);
									this.m_MaxLineDescender = Mathf.Min(num39, this.m_MaxLineDescender);
								}
								bool flag50 = flag47 || !flag31;
								if (flag50)
								{
									this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedAscender = num38;
									this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedDescender = num39;
									this.m_InternalTextElementInfo[this.m_CharacterCount].ascender = num36 - this.m_LineOffset;
									this.m_MaxDescender = (this.m_InternalTextElementInfo[this.m_CharacterCount].descender = num37 - this.m_LineOffset);
								}
								else
								{
									this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedAscender = this.m_MaxLineAscender;
									this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedDescender = this.m_MaxLineDescender;
									this.m_InternalTextElementInfo[this.m_CharacterCount].ascender = this.m_MaxLineAscender - this.m_LineOffset;
									this.m_MaxDescender = (this.m_InternalTextElementInfo[this.m_CharacterCount].descender = this.m_MaxLineDescender - this.m_LineOffset);
								}
								bool flag51 = this.m_LineNumber == 0 || this.m_IsNewPage;
								if (flag51)
								{
									bool flag52 = flag47 || !flag31;
									if (flag52)
									{
										this.m_MaxAscender = this.m_MaxLineAscender;
										this.m_MaxCapHeight = Mathf.Max(this.m_MaxCapHeight, this.m_CurrentFontAsset.m_FaceInfo.capLine * num2 / num13);
									}
								}
								bool flag53 = this.m_LineOffset == 0f;
								if (flag53)
								{
									bool flag54 = !flag31 || this.m_CharacterCount == this.m_FirstCharacterOfLine;
									if (flag54)
									{
										this.m_PageAscender = ((this.m_PageAscender > num36) ? this.m_PageAscender : num36);
									}
								}
								bool flag55 = (this.m_LineJustification & (TextAlignment)16) == (TextAlignment)16 || (this.m_LineJustification & (TextAlignment)8) == (TextAlignment)8;
								bool flag56 = num9 == 9U || num9 == 8203U || ((textWrapMode == TextWrappingMode.PreserveWhitespace || textWrapMode == TextWrappingMode.PreserveWhitespaceNoWrap) && (flag31 || num9 == 8203U)) || (!flag31 && num9 != 8203U && num9 != 173U && num9 != 3U) || (num9 == 173U && !flag7) || this.m_TextElementType == TextElementType.Sprite;
								if (flag56)
								{
									num5 = ((this.m_Width != -1f) ? Mathf.Min(x + 0.0001f - this.m_MarginLeft - this.m_MarginRight, this.m_Width) : (x + 0.0001f - this.m_MarginLeft - this.m_MarginRight));
									float num40 = Mathf.Abs(this.m_XAdvance) + glyphMetrics.horizontalAdvance * (1f - this.m_CharWidthAdjDelta) * ((num9 == 173U) ? num21 : num2);
									int characterCount = this.m_CharacterCount;
									bool flag57 = flag36 && num40 > num5 * (flag55 ? 1.05f : 1f);
									if (flag57)
									{
										bool flag58 = textWrapMode != TextWrappingMode.NoWrap && textWrapMode != TextWrappingMode.PreserveWhitespaceNoWrap && this.m_CharacterCount != this.m_FirstCharacterOfLine;
										if (flag58)
										{
											num8 = this.RestoreWordWrappingState(ref wordWrapState, textInfo);
											bool flag59 = this.m_InternalTextElementInfo[this.m_CharacterCount - 1].character == '\u00ad' && !flag7 && generationSettings.overflowMode == TextOverflowMode.Overflow;
											if (flag59)
											{
												characterSubstitution.index = this.m_CharacterCount - 1;
												characterSubstitution.unicode = 45U;
												num8--;
												this.m_CharacterCount--;
												goto IL_20EB;
											}
											flag7 = false;
											bool flag60 = this.m_InternalTextElementInfo[this.m_CharacterCount].character == '\u00ad';
											if (flag60)
											{
												flag7 = true;
												goto IL_20EB;
											}
											bool flag61 = isTextAutoSizingEnabled && flag5;
											if (flag61)
											{
												bool flag62 = this.m_CharWidthAdjDelta < generationSettings.charWidthMaxAdj / 100f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag62)
												{
													float num41 = num40;
													bool flag63 = this.m_CharWidthAdjDelta > 0f;
													if (flag63)
													{
														num41 /= 1f - this.m_CharWidthAdjDelta;
													}
													float num42 = num40 - (num5 - 0.0001f) * (flag55 ? 1.05f : 1f);
													this.m_CharWidthAdjDelta += num42 / num41;
													this.m_CharWidthAdjDelta = Mathf.Min(this.m_CharWidthAdjDelta, generationSettings.charWidthMaxAdj / 100f);
													return Vector2.zero;
												}
												bool flag64 = fontSize > generationSettings.fontSizeMin && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag64)
												{
													this.m_MaxFontSize = fontSize;
													float num43 = Mathf.Max((fontSize - this.m_MinFontSize) / 2f, 0.05f);
													fontSize -= num43;
													fontSize = Mathf.Max((float)((int)(fontSize * 20f + 0.5f)) / 20f, generationSettings.fontSizeMin);
												}
											}
											float num44 = this.m_MaxLineAscender - this.m_StartOfLineAscender;
											bool flag65 = this.m_LineOffset > 0f && Math.Abs(num44) > 0.01f && !this.m_IsDrivenLineSpacing && !this.m_IsNewPage;
											if (flag65)
											{
												this.m_MaxDescender -= num44;
												this.m_LineOffset += num44;
											}
											float num45 = this.m_MaxLineAscender - this.m_LineOffset;
											float num46 = this.m_MaxLineDescender - this.m_LineOffset;
											this.m_MaxDescender = ((this.m_MaxDescender < num46) ? this.m_MaxDescender : num46);
											bool flag66 = !flag4;
											if (flag66)
											{
												float maxDescender = this.m_MaxDescender;
											}
											bool flag67 = generationSettings.useMaxVisibleDescender && (this.m_CharacterCount >= generationSettings.maxVisibleCharacters || this.m_LineNumber >= generationSettings.maxVisibleLines);
											if (flag67)
											{
												flag4 = true;
											}
											this.m_FirstCharacterOfLine = this.m_CharacterCount;
											this.m_LineVisibleCharacterCount = 0;
											this.SaveWordWrappingState(ref wordWrapState2, num8, this.m_CharacterCount - 1, textInfo);
											this.m_LineNumber++;
											float adjustedAscender = this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedAscender;
											bool flag68 = this.m_LineHeight == -32767f;
											if (flag68)
											{
												this.m_LineOffset += 0f - this.m_MaxLineDescender + adjustedAscender + (num4 + this.m_LineSpacingDelta) * num + generationSettings.lineSpacing * num3;
												this.m_IsDrivenLineSpacing = false;
											}
											else
											{
												this.m_LineOffset += this.m_LineHeight + generationSettings.lineSpacing * num3;
												this.m_IsDrivenLineSpacing = true;
											}
											this.m_MaxLineAscender = -32767f;
											this.m_MaxLineDescender = 32767f;
											this.m_StartOfLineAscender = adjustedAscender;
											this.m_XAdvance = 0f + this.m_TagIndent;
											flag5 = true;
											goto IL_20EB;
										}
									}
									num6 = Mathf.Max(num6, num40 + this.m_MarginLeft + this.m_MarginRight);
									num7 = Mathf.Max(num7, this.m_MaxAscender - this.m_MaxDescender);
								}
								bool flag69 = this.m_LineOffset > 0f && !TextGeneratorUtilities.Approximately(this.m_MaxLineAscender, this.m_StartOfLineAscender) && !this.m_IsDrivenLineSpacing && !this.m_IsNewPage;
								if (flag69)
								{
									float num47 = this.m_MaxLineAscender - this.m_StartOfLineAscender;
									this.m_MaxDescender -= num47;
									this.m_LineOffset += num47;
									this.m_StartOfLineAscender += num47;
									wordWrapState.lineOffset = this.m_LineOffset;
									wordWrapState.startOfLineAscender = this.m_StartOfLineAscender;
								}
								bool flag70 = num9 != 8203U;
								if (flag70)
								{
									bool flag71 = num9 == 9U;
									if (flag71)
									{
										float num48 = this.m_CurrentFontAsset.faceInfo.tabWidth * (float)this.m_CurrentFontAsset.tabMultiple * num2;
										float num49 = Mathf.Ceil(this.m_XAdvance / num48) * num48;
										this.m_XAdvance = ((num49 > this.m_XAdvance) ? num49 : (this.m_XAdvance + num48));
									}
									else
									{
										bool flag72 = this.m_MonoSpacing != 0f;
										if (flag72)
										{
											this.m_XAdvance += (this.m_MonoSpacing - num34 + (this.m_CurrentFontAsset.regularStyleSpacing + num22) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
											bool flag73 = flag31 || num9 == 8203U;
											if (flag73)
											{
												this.m_XAdvance += generationSettings.wordSpacing * num3;
											}
										}
										else
										{
											this.m_XAdvance += ((glyphMetrics.horizontalAdvance * this.m_FXScale.x + glyphValueRecord.xAdvance) * num2 + (this.m_CurrentFontAsset.regularStyleSpacing + num22 + num35) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
											bool flag74 = flag31 || num9 == 8203U;
											if (flag74)
											{
												this.m_XAdvance += generationSettings.wordSpacing * num3;
											}
										}
									}
								}
								bool flag75 = num9 == 13U;
								if (flag75)
								{
									this.m_XAdvance = 0f + this.m_TagIndent;
								}
								bool flag76 = num9 == 10U || num9 == 11U || num9 == 3U || num9 == 8232U || num9 == 8233U || this.m_CharacterCount == totalCharacterCount - 1;
								if (flag76)
								{
									float num50 = this.m_MaxLineAscender - this.m_StartOfLineAscender;
									bool flag77 = this.m_LineOffset > 0f && Math.Abs(num50) > 0.01f && !this.m_IsDrivenLineSpacing && !this.m_IsNewPage;
									if (flag77)
									{
										this.m_MaxDescender -= num50;
										this.m_LineOffset += num50;
									}
									this.m_IsNewPage = false;
									float num51 = this.m_MaxLineDescender - this.m_LineOffset;
									this.m_MaxDescender = ((this.m_MaxDescender < num51) ? this.m_MaxDescender : num51);
									bool flag78 = num9 == 10U || num9 == 11U || num9 == 45U || num9 == 8232U || num9 == 8233U;
									if (flag78)
									{
										this.SaveWordWrappingState(ref wordWrapState2, num8, this.m_CharacterCount, textInfo);
										this.SaveWordWrappingState(ref wordWrapState, num8, this.m_CharacterCount, textInfo);
										this.m_LineNumber++;
										this.m_FirstCharacterOfLine = this.m_CharacterCount + 1;
										float adjustedAscender2 = this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedAscender;
										bool flag79 = this.m_LineHeight == -32767f;
										if (flag79)
										{
											float num52 = 0f - this.m_MaxLineDescender + adjustedAscender2 + (num4 + this.m_LineSpacingDelta) * num + (generationSettings.lineSpacing + ((num9 == 10U || num9 == 8233U) ? generationSettings.paragraphSpacing : 0f)) * num3;
											this.m_LineOffset += num52;
											this.m_IsDrivenLineSpacing = false;
										}
										else
										{
											this.m_LineOffset += this.m_LineHeight + (generationSettings.lineSpacing + ((num9 == 10U || num9 == 8233U) ? generationSettings.paragraphSpacing : 0f)) * num3;
											this.m_IsDrivenLineSpacing = true;
										}
										this.m_MaxLineAscender = -32767f;
										this.m_MaxLineDescender = 32767f;
										this.m_StartOfLineAscender = adjustedAscender2;
										this.m_XAdvance = 0f + this.m_TagLineIndent + this.m_TagIndent;
										this.m_CharacterCount++;
										goto IL_20EB;
									}
									bool flag80 = num9 == 3U;
									if (flag80)
									{
										num8 = this.m_TextProcessingArray.Length;
									}
								}
								bool flag81 = (textWrapMode != TextWrappingMode.NoWrap && textWrapMode != TextWrappingMode.PreserveWhitespaceNoWrap) || generationSettings.overflowMode == TextOverflowMode.Truncate || generationSettings.overflowMode == TextOverflowMode.Ellipsis;
								if (flag81)
								{
									bool flag82 = false;
									bool flag83 = false;
									bool flag84 = (flag31 || num9 == 8203U || num9 == 45U || num9 == 173U) && (!this.m_IsNonBreakingSpace || flag6) && num9 != 160U && num9 != 8199U && num9 != 8209U && num9 != 8239U && num9 != 8288U;
									if (flag84)
									{
										bool flag85 = num9 != 45U || this.m_CharacterCount <= 0 || !char.IsWhiteSpace(textInfo.textElementInfo[this.m_CharacterCount - 1].character);
										if (flag85)
										{
											flag5 = false;
											flag82 = true;
											wordWrapState3.previousWordBreak = -1;
										}
									}
									else
									{
										bool flag86 = !this.m_IsNonBreakingSpace && ((TextGeneratorUtilities.IsHangul(num9) && !textSettings.useModernHangulLineBreakingRules) || TextGeneratorUtilities.IsCJK(num9));
										if (flag86)
										{
											bool flag87 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(num9);
											bool flag88 = this.m_CharacterCount < totalCharacterCount - 1 && textSettings.lineBreakingRules.leadingCharactersLookup.Contains((uint)this.m_InternalTextElementInfo[this.m_CharacterCount + 1].character);
											bool flag89 = !flag87;
											if (flag89)
											{
												bool flag90 = !flag88;
												if (flag90)
												{
													flag5 = false;
													flag82 = true;
												}
												bool flag91 = flag5;
												if (flag91)
												{
													bool flag92 = flag31;
													if (flag92)
													{
														flag83 = true;
													}
													flag82 = true;
												}
											}
											else
											{
												bool flag93 = flag5 && flag47;
												if (flag93)
												{
													bool flag94 = flag31;
													if (flag94)
													{
														flag83 = true;
													}
													flag82 = true;
												}
											}
										}
										else
										{
											bool flag95 = flag5;
											if (flag95)
											{
												bool flag96 = (flag31 && num9 != 160U) || (num9 == 173U && !flag7);
												if (flag96)
												{
													flag83 = true;
												}
												flag82 = true;
											}
										}
									}
									bool flag97 = flag82;
									if (flag97)
									{
										this.SaveWordWrappingState(ref wordWrapState, num8, this.m_CharacterCount, textInfo);
									}
									bool flag98 = flag83;
									if (flag98)
									{
										this.SaveWordWrappingState(ref wordWrapState3, num8, this.m_CharacterCount, textInfo);
									}
								}
								this.m_CharacterCount++;
							}
						}
						IL_20EB:
						num8++;
					}
					float num53 = this.m_MaxFontSize - this.m_MinFontSize;
					bool flag99 = isTextAutoSizingEnabled && num53 > 0.051f && fontSize < generationSettings.fontSizeMax && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
					if (flag99)
					{
						bool flag100 = this.m_CharWidthAdjDelta < generationSettings.charWidthMaxAdj / 100f;
						if (flag100)
						{
							this.m_CharWidthAdjDelta = 0f;
						}
						this.m_MinFontSize = fontSize;
						float num54 = Mathf.Max((this.m_MaxFontSize - fontSize) / 2f, 0.05f);
						fontSize += num54;
						fontSize = Mathf.Min((float)((int)(fontSize * 20f + 0.5f)) / 20f, generationSettings.fontSizeMax);
						vector = Vector2.zero;
					}
					else
					{
						this.m_IsAutoSizePointSizeSet = true;
						this.m_IsCalculatingPreferredValues = false;
						num6 += ((generationSettings.margins.x > 0f) ? generationSettings.margins.x : 0f);
						num6 += ((generationSettings.margins.z > 0f) ? generationSettings.margins.z : 0f);
						num7 += ((generationSettings.margins.y > 0f) ? generationSettings.margins.y : 0f);
						num7 += ((generationSettings.margins.w > 0f) ? generationSettings.margins.w : 0f);
						num6 = (float)((int)(num6 * 100f + 1f)) / 100f;
						num7 = (float)((int)(num7 * 100f + 1f)) / 100f;
						vector = new Vector2(num6, num7);
					}
				}
			}
			return vector;
		}

		private void PopulateTextBackingArray(string sourceText)
		{
			int num = ((sourceText == null) ? 0 : sourceText.Length);
			this.PopulateTextBackingArray(sourceText, 0, num);
		}

		private void PopulateTextBackingArray(string sourceText, int start, int length)
		{
			int num = 0;
			bool flag = sourceText == null;
			int i;
			if (flag)
			{
				i = 0;
				length = 0;
			}
			else
			{
				i = Mathf.Clamp(start, 0, sourceText.Length);
				length = Mathf.Clamp(length, 0, (start + length < sourceText.Length) ? length : (sourceText.Length - start));
			}
			bool flag2 = length >= this.m_TextBackingArray.Capacity;
			if (flag2)
			{
				this.m_TextBackingArray.Resize(length);
			}
			int num2 = i + length;
			while (i < num2)
			{
				this.m_TextBackingArray[num] = (uint)sourceText[i];
				num++;
				i++;
			}
			this.m_TextBackingArray[num] = 0U;
			this.m_TextBackingArray.Count = num;
		}

		private void PopulateTextBackingArray(StringBuilder sourceText, int start, int length)
		{
			int num = 0;
			bool flag = sourceText == null;
			int i;
			if (flag)
			{
				i = 0;
				length = 0;
			}
			else
			{
				i = Mathf.Clamp(start, 0, sourceText.Length);
				length = Mathf.Clamp(length, 0, (start + length < sourceText.Length) ? length : (sourceText.Length - start));
			}
			bool flag2 = length >= this.m_TextBackingArray.Capacity;
			if (flag2)
			{
				this.m_TextBackingArray.Resize(length);
			}
			int num2 = i + length;
			while (i < num2)
			{
				this.m_TextBackingArray[num] = (uint)sourceText[i];
				num++;
				i++;
			}
			this.m_TextBackingArray[num] = 0U;
			this.m_TextBackingArray.Count = num;
		}

		private void PopulateTextBackingArray(char[] sourceText, int start, int length)
		{
			int num = 0;
			bool flag = sourceText == null;
			int i;
			if (flag)
			{
				i = 0;
				length = 0;
			}
			else
			{
				i = Mathf.Clamp(start, 0, sourceText.Length);
				length = Mathf.Clamp(length, 0, (start + length < sourceText.Length) ? length : (sourceText.Length - start));
			}
			bool flag2 = length >= this.m_TextBackingArray.Capacity;
			if (flag2)
			{
				this.m_TextBackingArray.Resize(length);
			}
			int num2 = i + length;
			while (i < num2)
			{
				this.m_TextBackingArray[num] = (uint)sourceText[i];
				num++;
				i++;
			}
			this.m_TextBackingArray[num] = 0U;
			this.m_TextBackingArray.Count = num;
		}

		private void PopulateTextProcessingArray(TextGenerationSettings generationSettings)
		{
			int count = this.m_TextBackingArray.Count;
			bool flag = this.m_TextProcessingArray.Length < count;
			if (flag)
			{
				TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray, count);
			}
			TextProcessingStack<int>.SetDefault(this.m_TextStyleStacks, 0);
			this.m_TextStyleStackDepth = 0;
			int num = 0;
			int num2 = this.m_TextStyleStacks[0].Pop();
			TextStyle style = TextGeneratorUtilities.GetStyle(generationSettings, num2);
			bool flag2 = style != null && style.hashCode != -1183493901;
			if (flag2)
			{
				TextGeneratorUtilities.InsertOpeningStyleTag(style, ref this.m_TextProcessingArray, ref num, ref this.m_TextStyleStackDepth, ref this.m_TextStyleStacks, ref generationSettings);
			}
			bool flag3 = generationSettings.tagNoParsing;
			int i = 0;
			while (i < count)
			{
				uint num3 = this.m_TextBackingArray[i];
				bool flag4 = num3 == 0U;
				if (flag4)
				{
					break;
				}
				bool flag5 = num3 == 92U && i < count - 1;
				if (flag5)
				{
					uint num4 = this.m_TextBackingArray[i + 1];
					uint num5 = num4;
					if (num5 != 85U)
					{
						if (num5 != 92U)
						{
							switch (num5)
							{
							case 110U:
							{
								bool flag6 = !generationSettings.parseControlCharacters;
								if (!flag6)
								{
									this.m_TextProcessingArray[num] = new TextProcessingElement
									{
										elementType = TextProcessingElementType.TextCharacterElement,
										stringIndex = i,
										length = 1,
										unicode = 10U
									};
									i++;
									num++;
									goto IL_0A01;
								}
								break;
							}
							case 114U:
							{
								bool flag7 = !generationSettings.parseControlCharacters;
								if (!flag7)
								{
									this.m_TextProcessingArray[num] = new TextProcessingElement
									{
										elementType = TextProcessingElementType.TextCharacterElement,
										stringIndex = i,
										length = 1,
										unicode = 13U
									};
									i++;
									num++;
									goto IL_0A01;
								}
								break;
							}
							case 116U:
							{
								bool flag8 = !generationSettings.parseControlCharacters;
								if (!flag8)
								{
									this.m_TextProcessingArray[num] = new TextProcessingElement
									{
										elementType = TextProcessingElementType.TextCharacterElement,
										stringIndex = i,
										length = 1,
										unicode = 9U
									};
									i++;
									num++;
									goto IL_0A01;
								}
								break;
							}
							case 117U:
							{
								bool flag9 = count > i + 5 && TextGeneratorUtilities.IsValidUTF16(this.m_TextBackingArray, i + 2);
								if (flag9)
								{
									this.m_TextProcessingArray[num] = new TextProcessingElement
									{
										elementType = TextProcessingElementType.TextCharacterElement,
										stringIndex = i,
										length = 6,
										unicode = TextGeneratorUtilities.GetUTF16(this.m_TextBackingArray, i + 2)
									};
									i += 5;
									num++;
									goto IL_0A01;
								}
								break;
							}
							case 118U:
							{
								bool flag10 = !generationSettings.parseControlCharacters;
								if (!flag10)
								{
									this.m_TextProcessingArray[num] = new TextProcessingElement
									{
										elementType = TextProcessingElementType.TextCharacterElement,
										stringIndex = i,
										length = 1,
										unicode = 11U
									};
									i++;
									num++;
									goto IL_0A01;
								}
								break;
							}
							}
						}
						else
						{
							bool flag11 = !generationSettings.parseControlCharacters;
							if (!flag11)
							{
								i++;
							}
						}
					}
					else
					{
						bool flag12 = count > i + 9 && TextGeneratorUtilities.IsValidUTF32(this.m_TextBackingArray, i + 2);
						if (flag12)
						{
							this.m_TextProcessingArray[num] = new TextProcessingElement
							{
								elementType = TextProcessingElementType.TextCharacterElement,
								stringIndex = i,
								length = 10,
								unicode = TextGeneratorUtilities.GetUTF32(this.m_TextBackingArray, i + 2)
							};
							i += 9;
							num++;
							goto IL_0A01;
						}
					}
					goto IL_03B4;
				}
				goto IL_03B4;
				IL_0A01:
				i++;
				continue;
				IL_03B4:
				bool flag13 = num3 >= 55296U && num3 <= 56319U && count > i + 1 && this.m_TextBackingArray[i + 1] >= 56320U && this.m_TextBackingArray[i + 1] <= 57343U;
				if (flag13)
				{
					this.m_TextProcessingArray[num] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 2,
						unicode = TextGeneratorUtilities.ConvertToUTF32(num3, this.m_TextBackingArray[i + 1])
					};
					i++;
					num++;
					goto IL_0A01;
				}
				bool flag14 = num3 == 60U && generationSettings.richText;
				if (flag14)
				{
					int markupTagHashCode = TextGeneratorUtilities.GetMarkupTagHashCode(this.m_TextBackingArray, i + 1);
					MarkupTag markupTag = (MarkupTag)markupTagHashCode;
					MarkupTag markupTag2 = markupTag;
					if (markupTag2 <= MarkupTag.CR)
					{
						if (markupTag2 <= MarkupTag.A)
						{
							if (markupTag2 != MarkupTag.NO_PARSE)
							{
								if (markupTag2 != MarkupTag.SLASH_NO_PARSE)
								{
									if (markupTag2 == MarkupTag.A)
									{
										bool flag15 = this.m_TextBackingArray.Count > i + 4 && this.m_TextBackingArray[i + 3] == 104U && this.m_TextBackingArray[i + 4] == 114U;
										if (flag15)
										{
											TextGeneratorUtilities.InsertOpeningTextStyle(TextGeneratorUtilities.GetStyle(generationSettings, 65), ref this.m_TextProcessingArray, ref num, ref this.m_TextStyleStackDepth, ref this.m_TextStyleStacks, ref generationSettings);
										}
									}
								}
								else
								{
									flag3 = false;
								}
							}
							else
							{
								flag3 = true;
							}
						}
						else if (markupTag2 != MarkupTag.SLASH_A)
						{
							if (markupTag2 != MarkupTag.BR)
							{
								if (markupTag2 == MarkupTag.CR)
								{
									bool flag16 = flag3;
									if (!flag16)
									{
										bool flag17 = num == this.m_TextProcessingArray.Length;
										if (flag17)
										{
											TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
										}
										this.m_TextProcessingArray[num] = new TextProcessingElement
										{
											elementType = TextProcessingElementType.TextCharacterElement,
											stringIndex = i,
											length = 4,
											unicode = 13U
										};
										num++;
										i += 3;
										goto IL_0A01;
									}
								}
							}
							else
							{
								bool flag18 = flag3;
								if (!flag18)
								{
									bool flag19 = num == this.m_TextProcessingArray.Length;
									if (flag19)
									{
										TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
									}
									this.m_TextProcessingArray[num] = new TextProcessingElement
									{
										elementType = TextProcessingElementType.TextCharacterElement,
										stringIndex = i,
										length = 4,
										unicode = 10U
									};
									num++;
									i += 3;
									goto IL_0A01;
								}
							}
						}
						else
						{
							TextGeneratorUtilities.InsertClosingTextStyle(TextGeneratorUtilities.GetStyle(generationSettings, 65), ref this.m_TextProcessingArray, ref num, ref this.m_TextStyleStackDepth, ref this.m_TextStyleStacks, ref generationSettings);
						}
					}
					else if (markupTag2 <= MarkupTag.NBSP)
					{
						if (markupTag2 != MarkupTag.SHY)
						{
							if (markupTag2 != MarkupTag.ZWJ)
							{
								if (markupTag2 == MarkupTag.NBSP)
								{
									bool flag20 = flag3;
									if (!flag20)
									{
										bool flag21 = num == this.m_TextProcessingArray.Length;
										if (flag21)
										{
											TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
										}
										this.m_TextProcessingArray[num] = new TextProcessingElement
										{
											elementType = TextProcessingElementType.TextCharacterElement,
											stringIndex = i,
											length = 6,
											unicode = 160U
										};
										num++;
										i += 5;
										goto IL_0A01;
									}
								}
							}
							else
							{
								bool flag22 = flag3;
								if (!flag22)
								{
									bool flag23 = num == this.m_TextProcessingArray.Length;
									if (flag23)
									{
										TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
									}
									this.m_TextProcessingArray[num] = new TextProcessingElement
									{
										elementType = TextProcessingElementType.TextCharacterElement,
										stringIndex = i,
										length = 5,
										unicode = 8205U
									};
									num++;
									i += 4;
									goto IL_0A01;
								}
							}
						}
						else
						{
							bool flag24 = flag3;
							if (!flag24)
							{
								bool flag25 = num == this.m_TextProcessingArray.Length;
								if (flag25)
								{
									TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
								}
								this.m_TextProcessingArray[num] = new TextProcessingElement
								{
									elementType = TextProcessingElementType.TextCharacterElement,
									stringIndex = i,
									length = 5,
									unicode = 173U
								};
								num++;
								i += 4;
								goto IL_0A01;
							}
						}
					}
					else if (markupTag2 != MarkupTag.ZWSP)
					{
						if (markupTag2 != MarkupTag.STYLE)
						{
							if (markupTag2 == MarkupTag.SLASH_STYLE)
							{
								bool flag26 = flag3;
								if (!flag26)
								{
									int j = num;
									TextGeneratorUtilities.ReplaceClosingStyleTag(ref this.m_TextProcessingArray, ref num, ref this.m_TextStyleStackDepth, ref this.m_TextStyleStacks, ref generationSettings);
									while (j < num)
									{
										this.m_TextProcessingArray[j].stringIndex = i;
										this.m_TextProcessingArray[j].length = 8;
										j++;
									}
									i += 7;
									goto IL_0A01;
								}
							}
						}
						else
						{
							bool flag27 = flag3;
							if (!flag27)
							{
								int k = num;
								int num6;
								bool flag28 = TextGeneratorUtilities.ReplaceOpeningStyleTag(ref this.m_TextBackingArray, i, out num6, ref this.m_TextProcessingArray, ref num, ref this.m_TextStyleStackDepth, ref this.m_TextStyleStacks, ref generationSettings);
								if (flag28)
								{
									while (k < num)
									{
										this.m_TextProcessingArray[k].stringIndex = i;
										this.m_TextProcessingArray[k].length = num6 - i + 1;
										k++;
									}
									i = num6;
									goto IL_0A01;
								}
							}
						}
					}
					else
					{
						bool flag29 = flag3;
						if (!flag29)
						{
							bool flag30 = num == this.m_TextProcessingArray.Length;
							if (flag30)
							{
								TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
							}
							this.m_TextProcessingArray[num] = new TextProcessingElement
							{
								elementType = TextProcessingElementType.TextCharacterElement,
								stringIndex = i,
								length = 6,
								unicode = 8203U
							};
							num++;
							i += 5;
							goto IL_0A01;
						}
					}
				}
				bool flag31 = num == this.m_TextProcessingArray.Length;
				if (flag31)
				{
					TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
				}
				this.m_TextProcessingArray[num] = new TextProcessingElement
				{
					elementType = TextProcessingElementType.TextCharacterElement,
					stringIndex = i,
					length = 1,
					unicode = num3
				};
				num++;
				goto IL_0A01;
			}
			this.m_TextStyleStackDepth = 0;
			bool flag32 = style != null && style.hashCode != -1183493901;
			if (flag32)
			{
				TextGeneratorUtilities.InsertClosingStyleTag(ref this.m_TextProcessingArray, ref num, ref this.m_TextStyleStackDepth, ref this.m_TextStyleStacks, ref generationSettings);
			}
			bool flag33 = num == this.m_TextProcessingArray.Length;
			if (flag33)
			{
				TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
			}
			this.m_TextProcessingArray[num].unicode = 0U;
			this.m_InternalTextProcessingArraySize = num;
		}

		private void InsertNewLine(int i, float baseScale, float currentElementScale, float currentEmScale, float boldSpacingAdjustment, float characterSpacingAdjustment, float width, float lineGap, ref bool isMaxVisibleDescenderSet, ref float maxVisibleDescender, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			float num = this.m_MaxLineAscender - this.m_StartOfLineAscender;
			bool flag = this.m_LineOffset > 0f && Math.Abs(num) > 0.01f && !this.m_IsDrivenLineSpacing && !this.m_IsNewPage;
			if (flag)
			{
				TextGeneratorUtilities.AdjustLineOffset(this.m_FirstCharacterOfLine, this.m_CharacterCount, num, textInfo);
				this.m_MaxDescender -= num;
				this.m_LineOffset += num;
			}
			float num2 = this.m_MaxLineAscender - this.m_LineOffset;
			float num3 = this.m_MaxLineDescender - this.m_LineOffset;
			this.m_MaxDescender = ((this.m_MaxDescender < num3) ? this.m_MaxDescender : num3);
			bool flag2 = !isMaxVisibleDescenderSet;
			if (flag2)
			{
				maxVisibleDescender = this.m_MaxDescender;
			}
			bool flag3 = generationSettings.useMaxVisibleDescender && (this.m_CharacterCount >= generationSettings.maxVisibleCharacters || this.m_LineNumber >= generationSettings.maxVisibleLines);
			if (flag3)
			{
				isMaxVisibleDescenderSet = true;
			}
			textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex = this.m_FirstCharacterOfLine;
			textInfo.lineInfo[this.m_LineNumber].firstVisibleCharacterIndex = (this.m_FirstVisibleCharacterOfLine = ((this.m_FirstCharacterOfLine > this.m_FirstVisibleCharacterOfLine) ? this.m_FirstCharacterOfLine : this.m_FirstVisibleCharacterOfLine));
			textInfo.lineInfo[this.m_LineNumber].lastCharacterIndex = (this.m_LastCharacterOfLine = ((this.m_CharacterCount - 1 > 0) ? (this.m_CharacterCount - 1) : 0));
			textInfo.lineInfo[this.m_LineNumber].lastVisibleCharacterIndex = (this.m_LastVisibleCharacterOfLine = ((this.m_LastVisibleCharacterOfLine < this.m_FirstVisibleCharacterOfLine) ? this.m_FirstVisibleCharacterOfLine : this.m_LastVisibleCharacterOfLine));
			textInfo.lineInfo[this.m_LineNumber].characterCount = textInfo.lineInfo[this.m_LineNumber].lastCharacterIndex - textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex + 1;
			textInfo.lineInfo[this.m_LineNumber].visibleCharacterCount = this.m_LineVisibleCharacterCount;
			textInfo.lineInfo[this.m_LineNumber].visibleSpaceCount = this.m_LineVisibleSpaceCount;
			textInfo.lineInfo[this.m_LineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[this.m_FirstVisibleCharacterOfLine].bottomLeft.x, num3);
			textInfo.lineInfo[this.m_LineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].topRight.x, num2);
			textInfo.lineInfo[this.m_LineNumber].length = textInfo.lineInfo[this.m_LineNumber].lineExtents.max.x;
			textInfo.lineInfo[this.m_LineNumber].width = width;
			float adjustedHorizontalAdvance = textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].adjustedHorizontalAdvance;
			float num4 = (adjustedHorizontalAdvance * currentElementScale + (this.m_CurrentFontAsset.regularStyleSpacing + characterSpacingAdjustment + boldSpacingAdjustment) * currentEmScale + this.m_CSpacing) * (1f - generationSettings.charWidthMaxAdj);
			float num5 = (textInfo.lineInfo[this.m_LineNumber].maxAdvance = textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num4 : (-num4)));
			textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].xAdvance = num5;
			textInfo.lineInfo[this.m_LineNumber].baseline = 0f - this.m_LineOffset;
			textInfo.lineInfo[this.m_LineNumber].ascender = num2;
			textInfo.lineInfo[this.m_LineNumber].descender = num3;
			textInfo.lineInfo[this.m_LineNumber].lineHeight = num2 - num3 + lineGap * baseScale;
			this.m_FirstCharacterOfLine = this.m_CharacterCount;
			this.m_LineVisibleCharacterCount = 0;
			this.m_LineVisibleSpaceCount = 0;
			this.SaveWordWrappingState(ref this.m_SavedLineState, i, this.m_CharacterCount - 1, textInfo);
			this.m_LineNumber++;
			bool flag4 = this.m_LineNumber >= textInfo.lineInfo.Length;
			if (flag4)
			{
				TextGeneratorUtilities.ResizeLineExtents(this.m_LineNumber, textInfo);
			}
			bool flag5 = this.m_LineHeight == -32767f;
			if (flag5)
			{
				float adjustedAscender = textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender;
				float num6 = 0f - this.m_MaxLineDescender + adjustedAscender + (lineGap + this.m_LineSpacingDelta) * baseScale + generationSettings.lineSpacing * currentEmScale;
				this.m_LineOffset += num6;
				this.m_StartOfLineAscender = adjustedAscender;
			}
			else
			{
				this.m_LineOffset += this.m_LineHeight + generationSettings.lineSpacing * currentEmScale;
			}
			this.m_MaxLineAscender = -32767f;
			this.m_MaxLineDescender = 32767f;
			this.m_XAdvance = 0f + this.m_TagIndent;
		}

		protected void DoMissingGlyphCallback(uint unicode, int stringIndex, FontAsset fontAsset, TextInfo textInfo)
		{
			TextGenerator.MissingCharacterEventCallback onMissingCharacter = TextGenerator.OnMissingCharacter;
			if (onMissingCharacter != null)
			{
				onMissingCharacter(unicode, stringIndex, textInfo, fontAsset);
			}
		}

		private void ClearMarkupTagAttributes()
		{
			int num = this.m_XmlAttribute.Length;
			for (int i = 0; i < num; i++)
			{
				this.m_XmlAttribute[i] = default(RichTextTagAttribute);
			}
		}

		private const int k_Tab = 9;

		private const int k_LineFeed = 10;

		private const int k_CarriageReturn = 13;

		private const int k_Space = 32;

		private const int k_DoubleQuotes = 34;

		private const int k_NumberSign = 35;

		private const int k_PercentSign = 37;

		private const int k_SingleQuote = 39;

		private const int k_Plus = 43;

		private const int k_Minus = 45;

		private const int k_Period = 46;

		private const int k_LesserThan = 60;

		private const int k_Equal = 61;

		private const int k_GreaterThan = 62;

		private const int k_Underline = 95;

		private const int k_NoBreakSpace = 160;

		private const int k_SoftHyphen = 173;

		private const int k_HyphenMinus = 45;

		private const int k_FigureSpace = 8199;

		private const int k_Hyphen = 8208;

		private const int k_NonBreakingHyphen = 8209;

		private const int k_ZeroWidthSpace = 8203;

		private const int k_NarrowNoBreakSpace = 8239;

		private const int k_WordJoiner = 8288;

		private const int k_HorizontalEllipsis = 8230;

		private const int k_RightSingleQuote = 8217;

		private const int k_Square = 9633;

		private const int k_HangulJamoStart = 4352;

		private const int k_HangulJamoEnd = 4607;

		private const int k_CjkStart = 11904;

		private const int k_CjkEnd = 40959;

		private const int k_HangulJameExtendedStart = 43360;

		private const int k_HangulJameExtendedEnd = 43391;

		private const int k_HangulSyllablesStart = 44032;

		private const int k_HangulSyllablesEnd = 55295;

		private const int k_CjkIdeographsStart = 63744;

		private const int k_CjkIdeographsEnd = 64255;

		private const int k_CjkFormsStart = 65072;

		private const int k_CjkFormsEnd = 65103;

		private const int k_CjkHalfwidthStart = 65280;

		private const int k_CjkHalfwidthEnd = 65519;

		private const int k_EndOfText = 3;

		private const float k_FloatUnset = -32767f;

		private const int k_MaxCharacters = 8;

		private static TextGenerator s_TextGenerator;

		private TextBackingContainer m_TextBackingArray = new TextBackingContainer(4);

		internal TextProcessingElement[] m_TextProcessingArray = new TextProcessingElement[8];

		internal int m_InternalTextProcessingArraySize;

		[SerializeField]
		protected bool m_VertexBufferAutoSizeReduction = false;

		private char[] m_HtmlTag = new char[128];

		internal HighlightState m_HighlightState = new HighlightState(Color.white, Offset.zero);

		protected bool m_IsIgnoringAlignment;

		protected static bool m_IsTextTruncated;

		private Vector3[] m_RectTransformCorners = new Vector3[4];

		private float m_MarginWidth;

		private float m_MarginHeight;

		private float m_PreferredWidth;

		private float m_PreferredHeight;

		private FontAsset m_CurrentFontAsset;

		private Material m_CurrentMaterial;

		private int m_CurrentMaterialIndex;

		private TextProcessingStack<MaterialReference> m_MaterialReferenceStack = new TextProcessingStack<MaterialReference>(new MaterialReference[16]);

		private float m_Padding;

		private SpriteAsset m_CurrentSpriteAsset;

		private int m_TotalCharacterCount;

		private float m_FontSize;

		private float m_FontScaleMultiplier;

		private float m_CurrentFontSize;

		private TextProcessingStack<float> m_SizeStack = new TextProcessingStack<float>(16);

		protected TextProcessingStack<int>[] m_TextStyleStacks = new TextProcessingStack<int>[8];

		protected int m_TextStyleStackDepth = 0;

		private FontStyles m_FontStyleInternal = FontStyles.Normal;

		private FontStyleStack m_FontStyleStack;

		private TextFontWeight m_FontWeightInternal = TextFontWeight.Regular;

		private TextProcessingStack<TextFontWeight> m_FontWeightStack = new TextProcessingStack<TextFontWeight>(8);

		private TextAlignment m_LineJustification;

		private TextProcessingStack<TextAlignment> m_LineJustificationStack = new TextProcessingStack<TextAlignment>(16);

		private float m_BaselineOffset;

		private TextProcessingStack<float> m_BaselineOffsetStack = new TextProcessingStack<float>(new float[16]);

		private Color32 m_FontColor32;

		private Color32 m_HtmlColor;

		private Color32 m_UnderlineColor;

		private Color32 m_StrikethroughColor;

		private TextProcessingStack<Color32> m_ColorStack = new TextProcessingStack<Color32>(new Color32[16]);

		private TextProcessingStack<Color32> m_UnderlineColorStack = new TextProcessingStack<Color32>(new Color32[16]);

		private TextProcessingStack<Color32> m_StrikethroughColorStack = new TextProcessingStack<Color32>(new Color32[16]);

		private TextProcessingStack<Color32> m_HighlightColorStack = new TextProcessingStack<Color32>(new Color32[16]);

		private TextProcessingStack<HighlightState> m_HighlightStateStack = new TextProcessingStack<HighlightState>(new HighlightState[16]);

		private TextProcessingStack<int> m_ItalicAngleStack = new TextProcessingStack<int>(new int[16]);

		private TextColorGradient m_ColorGradientPreset;

		private TextProcessingStack<TextColorGradient> m_ColorGradientStack = new TextProcessingStack<TextColorGradient>(new TextColorGradient[16]);

		private bool m_ColorGradientPresetIsTinted;

		private TextProcessingStack<int> m_ActionStack = new TextProcessingStack<int>(new int[16]);

		private float m_LineOffset;

		private float m_LineHeight;

		private bool m_IsDrivenLineSpacing;

		private float m_CSpacing;

		private float m_MonoSpacing;

		private float m_XAdvance;

		private float m_TagLineIndent;

		private float m_TagIndent;

		private TextProcessingStack<float> m_IndentStack = new TextProcessingStack<float>(new float[16]);

		private bool m_TagNoParsing;

		private int m_CharacterCount;

		private int m_FirstCharacterOfLine;

		private int m_LastCharacterOfLine;

		private int m_FirstVisibleCharacterOfLine;

		private int m_LastVisibleCharacterOfLine;

		private float m_MaxLineAscender;

		private float m_MaxLineDescender;

		private int m_LineNumber;

		private int m_LineVisibleCharacterCount;

		private int m_LineVisibleSpaceCount;

		private int m_FirstOverflowCharacterIndex;

		private int m_PageNumber;

		private float m_MarginLeft;

		private float m_MarginRight;

		private float m_Width;

		private Extents m_MeshExtents;

		private float m_MaxCapHeight;

		private float m_MaxAscender;

		private float m_MaxDescender;

		private bool m_IsNewPage;

		private bool m_IsNonBreakingSpace;

		private WordWrapState m_SavedWordWrapState;

		private WordWrapState m_SavedLineState;

		private WordWrapState m_SavedEllipsisState = default(WordWrapState);

		private WordWrapState m_SavedLastValidState = default(WordWrapState);

		private WordWrapState m_SavedSoftLineBreakState = default(WordWrapState);

		private TextElementType m_TextElementType;

		private bool m_isTextLayoutPhase;

		private int m_SpriteIndex;

		private Color32 m_SpriteColor;

		private TextElement m_CachedTextElement;

		private Color32 m_HighlightColor;

		private float m_CharWidthAdjDelta;

		private float m_MaxFontSize;

		private float m_MinFontSize;

		private int m_AutoSizeIterationCount;

		private int m_AutoSizeMaxIterationCount = 100;

		private bool m_IsAutoSizePointSizeSet;

		private float m_StartOfLineAscender;

		private float m_LineSpacingDelta;

		private MaterialReference[] m_MaterialReferences = new MaterialReference[8];

		private int m_SpriteCount = 0;

		private TextProcessingStack<int> m_StyleStack = new TextProcessingStack<int>(new int[16]);

		private TextProcessingStack<WordWrapState> m_EllipsisInsertionCandidateStack = new TextProcessingStack<WordWrapState>(8, 8);

		private int m_SpriteAnimationId;

		private int m_ItalicAngle;

		private Vector3 m_FXScale;

		private Quaternion m_FXRotation;

		private int m_LastBaseGlyphIndex;

		private float m_PageAscender;

		private RichTextTagAttribute[] m_XmlAttribute = new RichTextTagAttribute[8];

		private float[] m_AttributeParameterValues = new float[16];

		private Dictionary<int, int> m_MaterialReferenceIndexLookup = new Dictionary<int, int>();

		private bool m_IsCalculatingPreferredValues;

		private SpriteAsset m_DefaultSpriteAsset;

		private bool m_TintSprite;

		protected TextGenerator.SpecialCharacter m_Ellipsis;

		protected TextGenerator.SpecialCharacter m_Underline;

		private TextElementInfo[] m_InternalTextElementInfo;

		public delegate void MissingCharacterEventCallback(uint unicode, int stringIndex, TextInfo text, FontAsset fontAsset);

		protected struct SpecialCharacter
		{
			public SpecialCharacter(Character character, int materialIndex)
			{
				this.character = character;
				this.fontAsset = character.textAsset as FontAsset;
				this.material = ((this.fontAsset != null) ? this.fontAsset.material : null);
				this.materialIndex = materialIndex;
			}

			public Character character;

			public FontAsset fontAsset;

			public Material material;

			public int materialIndex;
		}
	}
}
