using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal class TextGenerator
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static bool IsExecutingJob { get; set; }

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static TextGenerator GetTextGenerator()
		{
			bool flag = TextGenerator.s_TextGenerator == null;
			if (flag)
			{
				TextGenerator.s_TextGenerator = new TextGenerator();
			}
			return TextGenerator.s_TextGenerator;
		}

		public void GenerateText(TextGenerationSettings settings, TextInfo textInfo)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			bool flag2 = settings.fontAsset == null;
			if (flag2)
			{
				Debug.LogWarning("Can't Generate Mesh, No Font Asset has been assigned.");
			}
			else
			{
				bool flag3 = textInfo == null;
				if (flag3)
				{
					Debug.LogError("Null TextInfo provided to TextGenerator. Cannot update its content.");
				}
				else
				{
					this.Prepare(settings, textInfo);
					bool flag4 = flag;
					if (flag4)
					{
						FontAsset.UpdateFontAssetsInUpdateQueue();
					}
					this.GenerateTextMesh(settings, textInfo);
				}
			}
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

		public bool isTextTruncated
		{
			get
			{
				return this.m_IsTextTruncated;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event TextGenerator.MissingCharacterEventCallback OnMissingCharacter;

		private float m_BaselineOffset
		{
			get
			{
				return this._m_BaselineOffset;
			}
			set
			{
				this._m_BaselineOffset = this.Round(value);
			}
		}

		private float m_LineOffset
		{
			get
			{
				return this._m_LineOffset;
			}
			set
			{
				this._m_LineOffset = this.Round(value);
			}
		}

		private float m_LineHeight
		{
			get
			{
				return this._m_LineHeight;
			}
			set
			{
				this._m_LineHeight = this.Round(value);
			}
		}

		private float m_XAdvance
		{
			get
			{
				return this._m_XAdvance;
			}
			set
			{
				float num = this.Round(value);
				this._m_XAdvance = num;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal void GenerateTextMesh(TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			bool flag = generationSettings.fontAsset == null;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned.");
			}
			else
			{
				bool flag2 = textInfo != null;
				if (flag2)
				{
					textInfo.Clear();
				}
				bool flag3 = generationSettings.fontSize <= 0 || this.m_TextProcessingArray == null || this.m_TextProcessingArray.Length == 0 || this.m_TextProcessingArray[0].unicode == 0U;
				if (flag3)
				{
					TextGenerator.ClearMesh(true, textInfo);
					this.m_PreferredWidth = 0f;
					this.m_PreferredHeight = 0f;
				}
				else
				{
					uint num;
					float num2;
					this.ParsingPhase(textInfo, generationSettings, out num, out num2);
					float num3 = this.m_MaxFontSize - this.m_MinFontSize;
					bool flag4 = this.m_AutoSizeIterationCount >= this.m_AutoSizeMaxIterationCount;
					if (flag4)
					{
						Debug.Log("Auto Size Iteration Count: " + this.m_AutoSizeIterationCount.ToString() + ". Final Point Size: " + this.m_FontSize.ToString());
					}
					bool flag5 = this.m_CharacterCount == 0 || (this.m_CharacterCount == 1 && num == 3U);
					if (flag5)
					{
						TextGenerator.ClearMesh(true, textInfo);
					}
					else
					{
						bool flag6 = this.NeedToRound && TextGenerator.EnableTextAlignmentAssertions;
						if (flag6)
						{
							Debug.AssertFormat((double)Mathf.Abs(generationSettings.screenRect.x - this.Round(generationSettings.screenRect.x)) < 0.01, "Bitmap Rendering specified and screenRect.x is not rounded:{0}", new object[] { generationSettings.screenRect.x });
							Debug.AssertFormat((double)Mathf.Abs(generationSettings.screenRect.y - this.Round(generationSettings.screenRect.y)) < 0.01, "Bitmap Rendering specified and screenRect.y is not rounded:{0}", new object[] { generationSettings.screenRect.y });
							Debug.AssertFormat((double)Mathf.Abs(generationSettings.screenRect.width - this.Round(generationSettings.screenRect.width)) < 0.01, "Bitmap Rendering specified and screenRect.width is not rounded:{0}", new object[] { generationSettings.screenRect.width });
							Debug.AssertFormat((double)Mathf.Abs(generationSettings.screenRect.height - this.Round(generationSettings.screenRect.height)) < 0.01, "Bitmap Rendering specified and screenRect.height is not rounded:{0}", new object[] { generationSettings.screenRect.height });
						}
						this.LayoutPhase(textInfo, generationSettings, num2);
						for (int i = 1; i < textInfo.materialCount; i++)
						{
							textInfo.meshInfo[i].ClearUnusedVertices();
						}
					}
				}
			}
		}

		private bool ValidateHtmlTag(TextProcessingElement[] chars, int startIndex, out int endIndex, TextGenerationSettings generationSettings, TextInfo textInfo, out bool isThreadSuccess)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			isThreadSuccess = true;
			TextSettings textSettings = generationSettings.textSettings;
			int num = 0;
			byte b = 0;
			int num2 = 0;
			this.ClearMarkupTagAttributes();
			TagValueType tagValueType = TagValueType.None;
			TagUnitType tagUnitType = TagUnitType.Pixels;
			endIndex = startIndex;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			int num3 = startIndex;
			while (num3 < chars.Length && chars[num3].unicode != 0U && num < this.m_HtmlTag.Length && chars[num3].unicode != 60U)
			{
				uint unicode = chars[num3].unicode;
				bool flag6 = unicode == 62U;
				if (flag6)
				{
					flag3 = true;
					endIndex = num3;
					this.m_HtmlTag[num] = '\0';
					break;
				}
				this.m_HtmlTag[num] = (char)unicode;
				num++;
				bool flag7 = b == 1;
				if (flag7)
				{
					bool flag8 = tagValueType == TagValueType.None;
					if (flag8)
					{
						bool flag9 = unicode == 43U || unicode == 45U || unicode == 46U || (unicode >= 48U && unicode <= 57U);
						if (flag9)
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
							bool flag10 = unicode == 35U;
							if (flag10)
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
								bool flag11 = unicode == 39U;
								if (flag11)
								{
									tagUnitType = TagUnitType.Pixels;
									tagValueType = (this.m_XmlAttribute[num2].valueType = TagValueType.StringValue);
									this.m_XmlAttribute[num2].valueStartIndex = num;
									flag4 = true;
								}
								else
								{
									bool flag12 = unicode == 34U;
									if (flag12)
									{
										tagUnitType = TagUnitType.Pixels;
										tagValueType = (this.m_XmlAttribute[num2].valueType = TagValueType.StringValue);
										this.m_XmlAttribute[num2].valueStartIndex = num;
										flag5 = true;
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
					}
					else
					{
						bool flag13 = tagValueType == TagValueType.NumericalValue;
						if (flag13)
						{
							bool flag14 = unicode == 112U || unicode == 101U || unicode == 37U || unicode == 32U;
							if (flag14)
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
							bool flag15 = tagValueType == TagValueType.ColorValue;
							if (flag15)
							{
								bool flag16 = unicode != 32U;
								if (flag16)
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
								bool flag17 = tagValueType == TagValueType.StringValue;
								if (flag17)
								{
									bool flag18 = (!flag5 || unicode != 34U) && (!flag4 || unicode != 39U);
									if (flag18)
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
										bool flag19 = this.m_XmlAttribute.Length <= num2;
										if (flag19)
										{
											int num12 = Mathf.NextPowerOfTwo(num2 + 1);
											Array.Resize<RichTextTagAttribute>(ref this.m_XmlAttribute, num12);
										}
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
				bool flag20 = unicode == 61U;
				if (flag20)
				{
					b = 1;
				}
				bool flag21 = b == 0 && unicode == 32U;
				if (flag21)
				{
					bool flag22 = flag2;
					if (flag22)
					{
						return false;
					}
					flag2 = true;
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
				bool flag23 = b == 0;
				if (flag23)
				{
					this.m_XmlAttribute[num2].nameHashCode = ((this.m_XmlAttribute[num2].nameHashCode << 5) + this.m_XmlAttribute[num2].nameHashCode) ^ (int)TextGeneratorUtilities.ToUpperFast((char)unicode);
				}
				bool flag24 = b == 2 && unicode == 32U;
				if (flag24)
				{
					b = 0;
				}
				num3++;
			}
			bool flag25 = !flag3;
			if (flag25)
			{
				return false;
			}
			bool flag26 = this.m_TagNoParsing && this.m_XmlAttribute[0].nameHashCode != -294095813;
			if (flag26)
			{
				return false;
			}
			bool flag27 = this.m_XmlAttribute[0].nameHashCode == -294095813;
			if (flag27)
			{
				this.m_TagNoParsing = false;
				return true;
			}
			bool flag28 = this.m_HtmlTag[0] == '#' && num == 4;
			if (flag28)
			{
				this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, 0, num);
				this.m_ColorStack.Add(this.m_HtmlColor);
				return true;
			}
			bool flag29 = this.m_HtmlTag[0] == '#' && num == 5;
			if (flag29)
			{
				this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, 0, num);
				this.m_ColorStack.Add(this.m_HtmlColor);
				return true;
			}
			bool flag30 = this.m_HtmlTag[0] == '#' && num == 7;
			if (flag30)
			{
				this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, 0, num);
				this.m_ColorStack.Add(this.m_HtmlColor);
				return true;
			}
			bool flag31 = this.m_HtmlTag[0] == '#' && num == 9;
			if (flag31)
			{
				this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, 0, num);
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
									bool flag32 = MaterialReferenceManager.TryGetColorGradientPreset(valueHashCode, out textColorGradient);
									if (flag32)
									{
										this.m_ColorGradientPreset = textColorGradient;
									}
									else
									{
										bool flag33 = textColorGradient == null;
										if (flag33)
										{
											bool flag34 = !flag;
											if (flag34)
											{
												isThreadSuccess = false;
												return false;
											}
											textColorGradient = Resources.Load<TextColorGradient>(textSettings.defaultColorGradientPresetsPath + new string(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength));
										}
										bool flag35 = textColorGradient == null;
										if (flag35)
										{
											return false;
										}
										MaterialReferenceManager.AddColorGradientPreset(valueHashCode, textColorGradient);
										this.m_ColorGradientPreset = textColorGradient;
									}
									this.m_ColorGradientPresetIsTinted = false;
									int num13 = 1;
									while (num13 < this.m_XmlAttribute.Length && this.m_XmlAttribute[num13].nameHashCode != 0)
									{
										int nameHashCode2 = this.m_XmlAttribute[num13].nameHashCode;
										MarkupTag markupTag2 = (MarkupTag)nameHashCode2;
										MarkupTag markupTag3 = markupTag2;
										if (markupTag3 == MarkupTag.TINT)
										{
											this.m_ColorGradientPresetIsTinted = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[num13].valueStartIndex, this.m_XmlAttribute[num13].valueLength) != 0f;
										}
										num13++;
									}
									this.m_ColorGradientStack.Add(this.m_ColorGradientPreset);
									return true;
								}
								if (markupTag != MarkupTag.FONT_WEIGHT)
								{
									goto IL_4569;
								}
								float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								bool flag36 = num14 == -32767f;
								if (flag36)
								{
									return false;
								}
								int num15 = (int)num14;
								int num16 = num15;
								if (num16 <= 400)
								{
									if (num16 <= 200)
									{
										if (num16 != 100)
										{
											if (num16 == 200)
											{
												this.m_FontWeightInternal = TextFontWeight.ExtraLight;
											}
										}
										else
										{
											this.m_FontWeightInternal = TextFontWeight.Thin;
										}
									}
									else if (num16 != 300)
									{
										if (num16 == 400)
										{
											this.m_FontWeightInternal = TextFontWeight.Regular;
										}
									}
									else
									{
										this.m_FontWeightInternal = TextFontWeight.Light;
									}
								}
								else if (num16 <= 600)
								{
									if (num16 != 500)
									{
										if (num16 == 600)
										{
											this.m_FontWeightInternal = TextFontWeight.SemiBold;
										}
									}
									else
									{
										this.m_FontWeightInternal = TextFontWeight.Medium;
									}
								}
								else if (num16 != 700)
								{
									if (num16 != 800)
									{
										if (num16 == 900)
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
									goto IL_4569;
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
								this.m_DuoSpace = false;
								return true;
							}
							if (markupTag != MarkupTag.CHARACTER_SPACE)
							{
								goto IL_4569;
							}
							float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag37 = num14 == -32767f;
							if (flag37)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_CSpacing = num14 * generationSettings.pixelsPerPoint;
								break;
							case TagUnitType.FontUnits:
								this.m_CSpacing = num14 * this.m_CurrentFontSize;
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
								goto IL_4569;
							}
							this.m_TagIndent = this.m_IndentStack.Remove();
							return true;
						}
						else
						{
							float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag38 = num14 == -32767f;
							if (flag38)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_TagIndent = num14 * generationSettings.pixelsPerPoint;
								break;
							case TagUnitType.FontUnits:
								this.m_TagIndent = num14 * this.m_CurrentFontSize;
								break;
							case TagUnitType.Percentage:
								this.m_TagIndent = this.m_MarginWidth * num14 / 100f;
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
								bool flag39 = (generationSettings.fontStyle & FontStyles.LowerCase) != FontStyles.LowerCase;
								if (flag39)
								{
									bool flag40 = this.m_FontStyleStack.Remove(FontStyles.LowerCase) == 0;
									if (flag40)
									{
										this.m_FontStyleInternal &= ~FontStyles.LowerCase;
									}
								}
								return true;
							}
							if (markupTag != MarkupTag.SLASH_CHARACTER_SPACE)
							{
								goto IL_4569;
							}
							bool flag41 = !this.m_isTextLayoutPhase || textInfo == null;
							if (flag41)
							{
								return true;
							}
							bool flag42 = this.m_CharacterCount > 0;
							if (flag42)
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
									goto IL_4569;
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
								float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								bool flag43 = num14 == -32767f;
								if (flag43)
								{
									return false;
								}
								switch (this.m_XmlAttribute[0].unitType)
								{
								case TagUnitType.Pixels:
									this.m_MonoSpacing = num14 * generationSettings.pixelsPerPoint;
									break;
								case TagUnitType.FontUnits:
									this.m_MonoSpacing = num14 * this.m_CurrentFontSize;
									break;
								case TagUnitType.Percentage:
									return false;
								}
								bool flag44 = this.m_XmlAttribute[1].nameHashCode == 582810522;
								if (flag44)
								{
									this.m_DuoSpace = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength) != 0f;
								}
								return true;
							}
						}
						else
						{
							TagValueType valueType = this.m_XmlAttribute[0].valueType;
							TagValueType tagValueType2 = valueType;
							float num14;
							if (tagValueType2 == TagValueType.None)
							{
								int num17 = 1;
								while (num17 < this.m_XmlAttribute.Length && this.m_XmlAttribute[num17].nameHashCode != 0)
								{
									int nameHashCode3 = this.m_XmlAttribute[num17].nameHashCode;
									MarkupTag markupTag4 = (MarkupTag)nameHashCode3;
									MarkupTag markupTag5 = markupTag4;
									if (markupTag5 != MarkupTag.LEFT)
									{
										if (markupTag5 == MarkupTag.RIGHT)
										{
											num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[num17].valueStartIndex, this.m_XmlAttribute[num17].valueLength);
											bool flag45 = num14 == -32767f;
											if (flag45)
											{
												return false;
											}
											switch (this.m_XmlAttribute[num17].unitType)
											{
											case TagUnitType.Pixels:
												this.m_MarginRight = num14 * generationSettings.pixelsPerPoint;
												break;
											case TagUnitType.FontUnits:
												this.m_MarginRight = num14 * this.m_CurrentFontSize;
												break;
											case TagUnitType.Percentage:
												this.m_MarginRight = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num14 / 100f;
												break;
											}
											this.m_MarginRight = ((this.m_MarginRight >= 0f) ? this.m_MarginRight : 0f);
										}
									}
									else
									{
										num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[num17].valueStartIndex, this.m_XmlAttribute[num17].valueLength);
										bool flag46 = num14 == -32767f;
										if (flag46)
										{
											return false;
										}
										switch (this.m_XmlAttribute[num17].unitType)
										{
										case TagUnitType.Pixels:
											this.m_MarginLeft = num14 * generationSettings.pixelsPerPoint;
											break;
										case TagUnitType.FontUnits:
											this.m_MarginLeft = num14 * this.m_CurrentFontSize;
											break;
										case TagUnitType.Percentage:
											this.m_MarginLeft = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num14 / 100f;
											break;
										}
										this.m_MarginLeft = ((this.m_MarginLeft >= 0f) ? this.m_MarginLeft : 0f);
									}
									num17++;
								}
								return true;
							}
							if (tagValueType2 != TagValueType.NumericalValue)
							{
								return false;
							}
							num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag47 = num14 == -32767f;
							if (flag47)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_MarginLeft = num14 * generationSettings.pixelsPerPoint;
								break;
							case TagUnitType.FontUnits:
								this.m_MarginLeft = num14 * this.m_CurrentFontSize;
								break;
							case TagUnitType.Percentage:
								this.m_MarginLeft = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num14 / 100f;
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
							goto IL_4569;
						}
						float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag48 = num14 == -32767f;
						if (flag48)
						{
							return false;
						}
						this.m_FXRotation = Quaternion.Euler(0f, 0f, num14);
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
							goto IL_4569;
						}
						float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag49 = num14 == -32767f;
						if (flag49)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
							this.m_TagLineIndent = num14 * generationSettings.pixelsPerPoint;
							break;
						case TagUnitType.FontUnits:
							this.m_TagLineIndent = num14 * this.m_CurrentFontSize;
							break;
						case TagUnitType.Percentage:
							this.m_TagLineIndent = this.m_MarginWidth * num14 / 100f;
							break;
						}
						this.m_XAdvance += this.m_TagLineIndent;
						return true;
					}
					else
					{
						int valueHashCode3 = this.m_XmlAttribute[0].valueHashCode;
						this.m_SpriteIndex = -1;
						bool flag50 = this.m_XmlAttribute[0].valueType == TagValueType.None || this.m_XmlAttribute[0].valueType == TagValueType.NumericalValue;
						if (flag50)
						{
							bool flag51 = textSettings.defaultSpriteAsset != null;
							if (flag51)
							{
								this.m_CurrentSpriteAsset = textSettings.defaultSpriteAsset;
							}
							else
							{
								bool flag52 = TextSettings.s_GlobalSpriteAsset != null;
								if (flag52)
								{
									this.m_CurrentSpriteAsset = TextSettings.s_GlobalSpriteAsset;
								}
							}
							bool flag53 = this.m_CurrentSpriteAsset == null;
							if (flag53)
							{
								return false;
							}
						}
						else
						{
							SpriteAsset spriteAsset;
							bool flag54 = MaterialReferenceManager.TryGetSpriteAsset(valueHashCode3, out spriteAsset);
							if (flag54)
							{
								this.m_CurrentSpriteAsset = spriteAsset;
							}
							else
							{
								bool flag55 = spriteAsset == null;
								if (flag55)
								{
									bool flag56 = spriteAsset == null;
									if (flag56)
									{
										bool flag57 = !flag;
										if (flag57)
										{
											isThreadSuccess = false;
											return false;
										}
										spriteAsset = Resources.Load<SpriteAsset>(textSettings.defaultSpriteAssetPath + new string(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength));
									}
								}
								bool flag58 = spriteAsset == null;
								if (flag58)
								{
									return false;
								}
								MaterialReferenceManager.AddSpriteAsset(valueHashCode3, spriteAsset);
								this.m_CurrentSpriteAsset = spriteAsset;
							}
						}
						bool flag59 = !flag && this.m_CurrentSpriteAsset.m_GlyphIndexLookup == null;
						if (flag59)
						{
							isThreadSuccess = false;
							return false;
						}
						bool flag60 = this.m_XmlAttribute[0].valueType == TagValueType.NumericalValue;
						if (flag60)
						{
							int num18 = (int)TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag61 = (float)num18 == -32767f;
							if (flag61)
							{
								return false;
							}
							bool flag62 = num18 > this.m_CurrentSpriteAsset.spriteCharacterTable.Count - 1;
							if (flag62)
							{
								return false;
							}
							this.m_SpriteIndex = num18;
						}
						this.m_SpriteColor = Color.white;
						this.m_TintSprite = false;
						int num19 = 0;
						while (num19 < this.m_XmlAttribute.Length && this.m_XmlAttribute[num19].nameHashCode != 0)
						{
							int nameHashCode4 = this.m_XmlAttribute[num19].nameHashCode;
							int num20 = 0;
							MarkupTag markupTag6 = (MarkupTag)nameHashCode4;
							MarkupTag markupTag7 = markupTag6;
							if (markupTag7 <= MarkupTag.NAME)
							{
								if (markupTag7 != MarkupTag.ANIM)
								{
									if (markupTag7 != MarkupTag.NAME)
									{
										goto IL_3B60;
									}
									this.m_CurrentSpriteAsset = SpriteAsset.SearchForSpriteByHashCode(this.m_CurrentSpriteAsset, this.m_XmlAttribute[num19].valueHashCode, true, out num20, null);
									bool flag63 = num20 == -1;
									if (flag63)
									{
										return false;
									}
									this.m_SpriteIndex = num20;
								}
								else
								{
									int attributeParameters = TextGeneratorUtilities.GetAttributeParameters(this.m_HtmlTag, this.m_XmlAttribute[num19].valueStartIndex, this.m_XmlAttribute[num19].valueLength, ref this.m_AttributeParameterValues);
									bool flag64 = attributeParameters != 3;
									if (flag64)
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
										goto IL_3B60;
									}
									num20 = (int)TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
									bool flag65 = (float)num20 == -32767f;
									if (flag65)
									{
										return false;
									}
									bool flag66 = num20 > this.m_CurrentSpriteAsset.spriteCharacterTable.Count - 1;
									if (flag66)
									{
										return false;
									}
									this.m_SpriteIndex = num20;
								}
								else
								{
									this.m_SpriteColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[num19].valueStartIndex, this.m_XmlAttribute[num19].valueLength);
								}
							}
							else
							{
								this.m_TintSprite = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[num19].valueStartIndex, this.m_XmlAttribute[num19].valueLength) != 0f;
							}
							IL_3B7C:
							num19++;
							continue;
							IL_3B60:
							bool flag67 = nameHashCode4 != -991527447;
							if (flag67)
							{
								return false;
							}
							goto IL_3B7C;
						}
						bool flag68 = this.m_SpriteIndex == -1;
						if (flag68)
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
										goto IL_4569;
									}
								}
								else
								{
									float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
									bool flag69 = num14 == -32767f;
									if (flag69)
									{
										return false;
									}
									switch (tagUnitType)
									{
									case TagUnitType.Pixels:
										this.m_LineHeight = num14 * generationSettings.pixelsPerPoint;
										break;
									case TagUnitType.FontUnits:
										this.m_LineHeight = num14 * this.m_CurrentFontSize;
										break;
									case TagUnitType.Percentage:
									{
										float num21 = this.m_CurrentFontSize / this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale;
										this.m_LineHeight = generationSettings.fontAsset.faceInfo.lineHeight * num14 / 100f * num21;
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
									goto IL_4569;
								}
								this.m_FontWeightStack.Remove();
								bool flag70 = this.m_FontStyleInternal == FontStyles.Bold;
								if (flag70)
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
									goto IL_4569;
								}
								float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								bool flag71 = num14 == -32767f;
								if (flag71)
								{
									return false;
								}
								switch (tagUnitType)
								{
								case TagUnitType.Pixels:
									this.m_MarginRight = num14 * generationSettings.pixelsPerPoint;
									break;
								case TagUnitType.FontUnits:
									this.m_MarginRight = num14 * this.m_CurrentFontSize;
									break;
								case TagUnitType.Percentage:
									this.m_MarginRight = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num14 / 100f;
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
								goto IL_3C64;
							}
							if (markupTag != MarkupTag.MARGIN_LEFT)
							{
								goto IL_4569;
							}
							float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
							bool flag72 = num14 == -32767f;
							if (flag72)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_MarginLeft = num14 * generationSettings.pixelsPerPoint;
								break;
							case TagUnitType.FontUnits:
								this.m_MarginLeft = num14 * this.m_CurrentFontSize;
								break;
							case TagUnitType.Percentage:
								this.m_MarginLeft = (this.m_MarginWidth - ((this.m_Width != -1f) ? this.m_Width : 0f)) * num14 / 100f;
								break;
							}
							this.m_MarginLeft = ((this.m_MarginLeft >= 0f) ? this.m_MarginLeft : 0f);
							return true;
						}
						bool flag73 = (generationSettings.fontStyle & FontStyles.UpperCase) != FontStyles.UpperCase;
						if (flag73)
						{
							bool flag74 = this.m_FontStyleStack.Remove(FontStyles.UpperCase) == 0;
							if (flag74)
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
								goto IL_4569;
							}
							bool flag75 = this.m_isTextLayoutPhase && !this.m_IsCalculatingPreferredValues;
							if (flag75)
							{
								bool flag76 = generationSettings.isIMGUI && textInfo != null;
								if (flag76)
								{
									this.CloseLastLinkTag(textInfo);
									int linkCount = textInfo.linkCount;
									bool flag77 = linkCount + 1 > textInfo.linkInfo.Length;
									if (flag77)
									{
										TextInfo.Resize<LinkInfo>(ref textInfo.linkInfo, linkCount + 1);
									}
									textInfo.linkInfo[linkCount].hashCode = 2535353;
									textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = this.m_CharacterCount;
									textInfo.linkInfo[linkCount].linkIdFirstCharacterIndex = 3;
									int num22 = this.m_XmlAttribute[1].valueLength;
									for (int i = num2; i >= 1; i--)
									{
										bool flag78 = this.m_XmlAttribute[i].valueLength <= 0;
										if (!flag78)
										{
											num22 = this.m_XmlAttribute[i].valueLength + this.m_XmlAttribute[i].valueStartIndex;
											break;
										}
									}
									bool flag79 = this.m_XmlAttribute[1].valueLength > 0;
									if (flag79)
									{
										textInfo.linkInfo[linkCount].SetLinkId(this.m_HtmlTag, 2, num22 - 1);
									}
									textInfo.linkCount++;
								}
								else
								{
									bool flag80 = this.m_XmlAttribute[1].nameHashCode == 2535353 && textInfo != null;
									if (flag80)
									{
										this.CloseLastLinkTag(textInfo);
										int linkCount2 = textInfo.linkCount;
										bool flag81 = linkCount2 + 1 > textInfo.linkInfo.Length;
										if (flag81)
										{
											TextInfo.Resize<LinkInfo>(ref textInfo.linkInfo, linkCount2 + 1);
										}
										textInfo.linkInfo[linkCount2].hashCode = 2535353;
										textInfo.linkInfo[linkCount2].linkTextfirstCharacterIndex = this.m_CharacterCount;
										textInfo.linkInfo[linkCount2].linkIdFirstCharacterIndex = startIndex + this.m_XmlAttribute[1].valueStartIndex;
										textInfo.linkInfo[linkCount2].SetLinkId(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
										textInfo.linkInfo[linkCount2].linkTextLength = -1;
										textInfo.linkCount++;
									}
								}
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
								bool flag82 = this.m_XmlAttribute[1].nameHashCode == 75347905;
								if (flag82)
								{
									this.m_ItalicAngle = (int)TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
									bool flag83 = this.m_ItalicAngle < -180 || this.m_ItalicAngle > 180;
									if (flag83)
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
								goto IL_4569;
							}
							this.m_FontStyleInternal |= FontStyles.Strikethrough;
							this.m_FontStyleStack.Add(FontStyles.Strikethrough);
							bool flag84 = this.m_XmlAttribute[1].nameHashCode == 81999901;
							if (flag84)
							{
								this.m_StrikethroughColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
								this.m_StrikethroughColor.a = ((this.m_HtmlColor.a < this.m_StrikethroughColor.a) ? this.m_HtmlColor.a : this.m_StrikethroughColor.a);
								bool flag85 = textInfo != null;
								if (flag85)
								{
									textInfo.hasMultipleColors = true;
								}
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
							bool flag86 = this.m_XmlAttribute[1].nameHashCode == 81999901;
							if (flag86)
							{
								this.m_UnderlineColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength);
								this.m_UnderlineColor.a = ((this.m_HtmlColor.a < this.m_UnderlineColor.a) ? this.m_HtmlColor.a : this.m_UnderlineColor.a);
								bool flag87 = textInfo != null;
								if (flag87)
								{
									textInfo.hasMultipleColors = true;
								}
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
							bool flag88 = (generationSettings.fontStyle & FontStyles.Italic) != FontStyles.Italic;
							if (flag88)
							{
								this.m_ItalicAngle = this.m_ItalicAngleStack.Remove();
								bool flag89 = this.m_FontStyleStack.Remove(FontStyles.Italic) == 0;
								if (flag89)
								{
									this.m_FontStyleInternal &= ~FontStyles.Italic;
								}
							}
							return true;
						}
						if (markupTag != MarkupTag.SLASH_BOLD)
						{
							goto IL_4569;
						}
						bool flag90 = (generationSettings.fontStyle & FontStyles.Bold) != FontStyles.Bold;
						if (flag90)
						{
							bool flag91 = this.m_FontStyleStack.Remove(FontStyles.Bold) == 0;
							if (flag91)
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
							bool flag92 = this.m_isTextLayoutPhase && !this.m_IsCalculatingPreferredValues && textInfo != null;
							if (flag92)
							{
								bool flag93 = textInfo.linkInfo.Length == 0 || textInfo.linkCount <= 0;
								if (flag93)
								{
									bool displayWarnings = generationSettings.textSettings.displayWarnings;
									if (displayWarnings)
									{
										Debug.LogWarning("There seems to be an issue with the formatting of the <a> tag. Possible issues include: missing or misplaced closing '>', missing or incorrect attribute, or unclosed quotes for attribute values. Please review the tag syntax.");
									}
								}
								else
								{
									int num23 = textInfo.linkCount - 1;
									textInfo.linkInfo[num23].linkTextLength = this.m_CharacterCount - textInfo.linkInfo[num23].linkTextfirstCharacterIndex;
								}
							}
							return true;
						}
						if (markupTag == MarkupTag.SLASH_UNDERLINE)
						{
							bool flag94 = (generationSettings.fontStyle & FontStyles.Underline) != FontStyles.Underline;
							if (flag94)
							{
								bool flag95 = this.m_FontStyleStack.Remove(FontStyles.Underline) == 0;
								if (flag95)
								{
									this.m_FontStyleInternal &= ~FontStyles.Underline;
								}
							}
							this.m_UnderlineColor = this.m_UnderlineColorStack.Remove();
							return true;
						}
						if (markupTag != MarkupTag.SLASH_STRIKETHROUGH)
						{
							goto IL_4569;
						}
						bool flag96 = (generationSettings.fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough;
						if (flag96)
						{
							bool flag97 = this.m_FontStyleStack.Remove(FontStyles.Strikethrough) == 0;
							if (flag97)
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
									goto IL_4569;
								}
								this.m_FontScaleMultiplier *= ((this.m_CurrentFontAsset.faceInfo.subscriptSize > 0f) ? this.m_CurrentFontAsset.faceInfo.subscriptSize : 1f);
								this.m_BaselineOffsetStack.Push(this.m_BaselineOffset);
								this.m_MaterialReferenceStack.Push(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
								float num21 = this.m_CurrentFontSize / this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale;
								this.m_BaselineOffset += this.m_CurrentFontAsset.faceInfo.subscriptOffset * num21 * this.m_FontScaleMultiplier;
								this.m_FontStyleStack.Add(FontStyles.Subscript);
								this.m_FontStyleInternal |= FontStyles.Subscript;
								return true;
							}
							else
							{
								float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								bool flag98 = num14 == -32767f;
								if (flag98)
								{
									return false;
								}
								switch (tagUnitType)
								{
								case TagUnitType.Pixels:
									this.m_XAdvance = num14;
									return true;
								case TagUnitType.FontUnits:
									this.m_XAdvance = num14 * this.m_CurrentFontSize;
									return true;
								case TagUnitType.Percentage:
									this.m_XAdvance = this.m_MarginWidth * num14 / 100f;
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
								this.m_MaterialReferenceStack.Push(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
								float num21 = this.m_CurrentFontSize / this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale;
								this.m_BaselineOffset += this.m_CurrentFontAsset.faceInfo.superscriptOffset * num21 * this.m_FontScaleMultiplier;
								this.m_FontStyleStack.Add(FontStyles.Superscript);
								this.m_FontStyleInternal |= FontStyles.Superscript;
								return true;
							}
							if (markupTag == MarkupTag.SLASH_SUBSCRIPT)
							{
								bool flag99 = (this.m_FontStyleInternal & FontStyles.Subscript) == FontStyles.Subscript;
								if (flag99)
								{
									FontAsset fontAsset = this.m_MaterialReferenceStack.Pop().fontAsset;
									bool flag100 = this.m_FontScaleMultiplier < 1f;
									if (flag100)
									{
										this.m_BaselineOffset = this.m_BaselineOffsetStack.Pop();
										this.m_FontScaleMultiplier /= ((fontAsset.faceInfo.subscriptSize > 0f) ? fontAsset.faceInfo.subscriptSize : 1f);
									}
									bool flag101 = this.m_FontStyleStack.Remove(FontStyles.Subscript) == 0;
									if (flag101)
									{
										this.m_FontStyleInternal &= ~FontStyles.Subscript;
									}
								}
								return true;
							}
							if (markupTag != MarkupTag.SLASH_SUPERSCRIPT)
							{
								goto IL_4569;
							}
							bool flag102 = (this.m_FontStyleInternal & FontStyles.Superscript) == FontStyles.Superscript;
							if (flag102)
							{
								FontAsset fontAsset2 = this.m_MaterialReferenceStack.Pop().fontAsset;
								bool flag103 = this.m_FontScaleMultiplier < 1f;
								if (flag103)
								{
									this.m_BaselineOffset = this.m_BaselineOffsetStack.Pop();
									this.m_FontScaleMultiplier /= ((fontAsset2.faceInfo.superscriptSize > 0f) ? fontAsset2.faceInfo.superscriptSize : 1f);
								}
								bool flag104 = this.m_FontStyleStack.Remove(FontStyles.Superscript) == 0;
								if (flag104)
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
							goto IL_4569;
						}
						int valueHashCode4 = this.m_XmlAttribute[0].valueHashCode;
						int nameHashCode5 = this.m_XmlAttribute[1].nameHashCode;
						int num24 = this.m_XmlAttribute[1].valueHashCode;
						bool flag105 = valueHashCode4 == -620974005;
						if (flag105)
						{
							this.m_CurrentFontAsset = this.m_MaterialReferences[0].fontAsset;
							this.m_CurrentMaterial = this.m_MaterialReferences[0].material;
							this.m_CurrentMaterialIndex = 0;
							this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[0]);
							return true;
						}
						FontAsset fontAsset3;
						MaterialReferenceManager.TryGetFontAsset(valueHashCode4, out fontAsset3);
						bool flag106 = fontAsset3 == null;
						if (flag106)
						{
							bool flag107 = fontAsset3 == null;
							if (flag107)
							{
								bool flag108 = !flag;
								if (flag108)
								{
									isThreadSuccess = false;
									return false;
								}
								fontAsset3 = Resources.Load<FontAsset>(textSettings.defaultFontAssetPath + new string(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength));
							}
							bool flag109 = fontAsset3 == null;
							if (flag109)
							{
								return false;
							}
							MaterialReferenceManager.AddFontAsset(fontAsset3);
						}
						bool flag110 = nameHashCode5 == 0 && num24 == 0;
						if (flag110)
						{
							this.m_CurrentMaterial = fontAsset3.material;
							this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, fontAsset3, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
							this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
						}
						else
						{
							bool flag111 = nameHashCode5 == 825491659;
							if (!flag111)
							{
								return false;
							}
							Material material;
							bool flag112 = MaterialReferenceManager.TryGetMaterial(num24, out material);
							if (flag112)
							{
								this.m_CurrentMaterial = material;
								this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, fontAsset3, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
								this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
							}
							else
							{
								bool flag113 = !flag;
								if (flag113)
								{
									isThreadSuccess = false;
									return false;
								}
								material = Resources.Load<Material>(textSettings.defaultFontAssetPath + new string(this.m_HtmlTag, this.m_XmlAttribute[1].valueStartIndex, this.m_XmlAttribute[1].valueLength));
								bool flag114 = material == null;
								if (flag114)
								{
									return false;
								}
								MaterialReferenceManager.AddFontMaterial(num24, material);
								this.m_CurrentMaterial = material;
								this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, fontAsset3, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
								this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
							}
						}
						this.m_CurrentFontAsset = fontAsset3;
						return true;
					}
					else
					{
						if (markupTag == MarkupTag.LINK)
						{
							bool flag115 = this.m_isTextLayoutPhase && !this.m_IsCalculatingPreferredValues && textInfo != null;
							if (flag115)
							{
								this.CloseLastLinkTag(textInfo);
								int linkCount3 = textInfo.linkCount;
								bool flag116 = linkCount3 + 1 > textInfo.linkInfo.Length;
								if (flag116)
								{
									TextInfo.Resize<LinkInfo>(ref textInfo.linkInfo, linkCount3 + 1);
								}
								textInfo.linkInfo[linkCount3].hashCode = this.m_XmlAttribute[0].valueHashCode;
								textInfo.linkInfo[linkCount3].linkTextfirstCharacterIndex = this.m_CharacterCount;
								textInfo.linkInfo[linkCount3].linkIdFirstCharacterIndex = startIndex + this.m_XmlAttribute[0].valueStartIndex;
								textInfo.linkInfo[linkCount3].SetLinkId(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
								textInfo.linkInfo[linkCount3].linkTextLength = -1;
								textInfo.linkCount++;
							}
							return true;
						}
						if (markupTag == MarkupTag.MARK)
						{
							this.m_FontStyleInternal |= FontStyles.Highlight;
							this.m_FontStyleStack.Add(FontStyles.Highlight);
							Color32 color = new Color32(byte.MaxValue, byte.MaxValue, 0, 64);
							Offset offset = Offset.zero;
							int num25 = 0;
							while (num25 < this.m_XmlAttribute.Length && this.m_XmlAttribute[num25].nameHashCode != 0)
							{
								MarkupTag nameHashCode6 = (MarkupTag)this.m_XmlAttribute[num25].nameHashCode;
								MarkupTag markupTag8 = nameHashCode6;
								if (markupTag8 != MarkupTag.PADDING)
								{
									if (markupTag8 != MarkupTag.MARK)
									{
										if (markupTag8 == MarkupTag.COLOR)
										{
											color = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[num25].valueStartIndex, this.m_XmlAttribute[num25].valueLength);
										}
									}
									else
									{
										bool flag117 = this.m_XmlAttribute[num25].valueType == TagValueType.ColorValue;
										if (flag117)
										{
											color = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
										}
									}
								}
								else
								{
									int attributeParameters2 = TextGeneratorUtilities.GetAttributeParameters(this.m_HtmlTag, this.m_XmlAttribute[num25].valueStartIndex, this.m_XmlAttribute[num25].valueLength, ref this.m_AttributeParameterValues);
									bool flag118 = attributeParameters2 != 4;
									if (flag118)
									{
										return false;
									}
									offset = new Offset(this.m_AttributeParameterValues[0], this.m_AttributeParameterValues[1], this.m_AttributeParameterValues[2], this.m_AttributeParameterValues[3]);
									offset *= this.m_FontSize * 0.01f;
								}
								num25++;
							}
							color.a = ((this.m_HtmlColor.a < color.a) ? this.m_HtmlColor.a : color.a);
							this.m_HighlightState = new HighlightState(color, offset);
							this.m_HighlightStateStack.Push(this.m_HighlightState);
							bool flag119 = textInfo != null;
							if (flag119)
							{
								textInfo.hasMultipleColors = true;
							}
							return true;
						}
						if (markupTag != MarkupTag.PAGE)
						{
							goto IL_4569;
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
							goto IL_4569;
						}
						float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag120 = num14 == -32767f;
						if (flag120)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
						{
							bool flag121 = this.m_HtmlTag[5] == '+';
							if (flag121)
							{
								this.m_CurrentFontSize = this.m_FontSize + num14 * generationSettings.pixelsPerPoint;
								this.m_SizeStack.Add(this.m_CurrentFontSize);
								return true;
							}
							bool flag122 = this.m_HtmlTag[5] == '-';
							if (flag122)
							{
								this.m_CurrentFontSize = this.m_FontSize + num14 * generationSettings.pixelsPerPoint;
								this.m_SizeStack.Add(this.m_CurrentFontSize);
								return true;
							}
							this.m_CurrentFontSize = num14 * generationSettings.pixelsPerPoint;
							this.m_SizeStack.Add(this.m_CurrentFontSize);
							return true;
						}
						case TagUnitType.FontUnits:
							this.m_CurrentFontSize = this.m_FontSize * num14;
							this.m_SizeStack.Add(this.m_CurrentFontSize);
							return true;
						case TagUnitType.Percentage:
							this.m_CurrentFontSize = this.m_FontSize * num14 / 100f;
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
							goto IL_4569;
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
						goto IL_4569;
					}
					bool flag123 = (generationSettings.fontStyle & FontStyles.Highlight) != FontStyles.Highlight;
					if (flag123)
					{
						this.m_HighlightStateStack.Remove();
						this.m_HighlightState = this.m_HighlightStateStack.current;
						bool flag124 = this.m_FontStyleStack.Remove(FontStyles.Highlight) == 0;
						if (flag124)
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
						bool flag125 = this.m_isTextLayoutPhase && !this.m_IsCalculatingPreferredValues && textInfo != null;
						if (flag125)
						{
							bool flag126 = textInfo.linkInfo.Length == 0 || textInfo.linkCount <= 0;
							if (flag126)
							{
								bool displayWarnings2 = generationSettings.textSettings.displayWarnings;
								if (displayWarnings2)
								{
									Debug.LogWarning("There seems to be an issue with the formatting of the <link> tag. Possible issues include: missing or misplaced closing '>', missing or incorrect attribute, or unclosed quotes for attribute values. Please review the tag syntax.");
								}
							}
							else
							{
								textInfo.linkInfo[textInfo.linkCount - 1].linkTextLength = this.m_CharacterCount - textInfo.linkInfo[textInfo.linkCount - 1].linkTextfirstCharacterIndex;
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
						goto IL_4569;
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
							goto IL_4569;
						}
						bool flag127 = this.m_XmlAttribute[0].valueLength != 3;
						if (flag127)
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
							goto IL_4569;
						}
						this.m_TagLineIndent = 0f;
						return true;
					}
					else
					{
						bool flag128 = textInfo != null;
						if (flag128)
						{
							textInfo.hasMultipleColors = true;
						}
						bool flag129 = this.m_HtmlTag[6] == '#' || this.m_HtmlTag[7] == '#';
						if (flag129)
						{
							int num26 = num;
							bool flag130 = this.m_HtmlTag[6] == '#';
							if (flag130)
							{
								startIndex = 6;
							}
							else
							{
								startIndex = 7;
								num26--;
							}
							this.m_HtmlColor = TextGeneratorUtilities.HexCharsToColor(this.m_HtmlTag, startIndex, num26 - startIndex);
							this.m_ColorStack.Add(this.m_HtmlColor);
							return true;
						}
						int valueHashCode6 = this.m_XmlAttribute[0].valueHashCode;
						int num27 = valueHashCode6;
						if (num27 <= 2284356)
						{
							if (num27 <= -1108587920)
							{
								if (num27 <= -1812576107)
								{
									if (num27 == -1960309918)
									{
										this.m_HtmlColor = new Color32(0, 0, 160, byte.MaxValue);
										this.m_ColorStack.Add(this.m_HtmlColor);
										return true;
									}
									if (num27 == -1812576107)
									{
										this.m_HtmlColor = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
										this.m_ColorStack.Add(this.m_HtmlColor);
										return true;
									}
								}
								else
								{
									if (num27 == -1355621936)
									{
										this.m_HtmlColor = new Color32(128, 0, 0, byte.MaxValue);
										this.m_ColorStack.Add(this.m_HtmlColor);
										return true;
									}
									if (num27 == -1250222130)
									{
										this.m_HtmlColor = new Color32(160, 32, 240, byte.MaxValue);
										this.m_ColorStack.Add(this.m_HtmlColor);
										return true;
									}
									if (num27 == -1108587920)
									{
										this.m_HtmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
										this.m_ColorStack.Add(this.m_HtmlColor);
										return true;
									}
								}
							}
							else if (num27 <= -960329321)
							{
								if (num27 == -1014785338)
								{
									this.m_HtmlColor = new Color32(0, 0, 0, 0);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num27 == -1002715645)
								{
									this.m_HtmlColor = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num27 == -960329321)
								{
									this.m_HtmlColor = new Color32(192, 192, 192, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
							}
							else
							{
								if (num27 == -882444668)
								{
									this.m_HtmlColor = Color.yellow;
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num27 == 91635)
								{
									this.m_HtmlColor = Color.red;
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num27 == 2284356)
								{
									this.m_HtmlColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
							}
						}
						else if (num27 <= 2947772)
						{
							if (num27 <= 2638345)
							{
								if (num27 == 2457214)
								{
									this.m_HtmlColor = Color.blue;
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num27 == 2504597)
								{
									this.m_HtmlColor = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num27 == 2638345)
								{
									this.m_HtmlColor = new Color32(128, 128, 128, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
							}
							else
							{
								if (num27 == 2656045)
								{
									this.m_HtmlColor = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num27 == 2876352)
								{
									this.m_HtmlColor = new Color32(0, 0, 128, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
								if (num27 == 2947772)
								{
									this.m_HtmlColor = new Color32(0, 128, 128, byte.MaxValue);
									this.m_ColorStack.Add(this.m_HtmlColor);
									return true;
								}
							}
						}
						else if (num27 <= 87065851)
						{
							if (num27 == 81017702)
							{
								this.m_HtmlColor = new Color32(165, 42, 42, byte.MaxValue);
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
							if (num27 == 81074727)
							{
								this.m_HtmlColor = Color.black;
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
							if (num27 == 87065851)
							{
								this.m_HtmlColor = Color.green;
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
						}
						else
						{
							if (num27 == 95492953)
							{
								this.m_HtmlColor = new Color32(128, 128, 0, byte.MaxValue);
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
							if (num27 == 105680263)
							{
								this.m_HtmlColor = Color.white;
								this.m_ColorStack.Add(this.m_HtmlColor);
								return true;
							}
							if (num27 == 341063360)
							{
								this.m_HtmlColor = new Color32(173, 216, 230, byte.MaxValue);
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
							goto IL_4569;
						}
						float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag131 = num14 == -32767f;
						if (flag131)
						{
							return false;
						}
						this.m_FXScale = new Vector3(num14, 1f, 1f);
						return true;
					}
					else
					{
						float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
						bool flag132 = num14 == -32767f;
						if (flag132)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
							this.m_XAdvance += num14 * generationSettings.pixelsPerPoint;
							return true;
						case TagUnitType.FontUnits:
							this.m_XAdvance += num14 * this.m_CurrentFontSize;
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
						goto IL_4569;
					}
					return false;
				}
				else
				{
					float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
					bool flag133 = num14 == -32767f;
					if (flag133)
					{
						return false;
					}
					switch (tagUnitType)
					{
					case TagUnitType.Pixels:
						this.m_Width = num14 * generationSettings.pixelsPerPoint;
						break;
					case TagUnitType.FontUnits:
						return false;
					case TagUnitType.Percentage:
						this.m_Width = this.m_MarginWidth * num14 / 100f;
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
						goto IL_4569;
					}
					bool flag134 = (generationSettings.fontStyle & FontStyles.SmallCaps) != FontStyles.SmallCaps;
					if (flag134)
					{
						bool flag135 = this.m_FontStyleStack.Remove(FontStyles.SmallCaps) == 0;
						if (flag135)
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
							goto IL_4569;
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
						goto IL_4569;
					}
					this.m_LineJustification = this.m_LineJustificationStack.Remove();
					return true;
				}
				else
				{
					int num24 = this.m_XmlAttribute[0].valueHashCode;
					bool flag136 = num24 == -620974005;
					if (flag136)
					{
						this.m_CurrentMaterial = this.m_MaterialReferences[0].material;
						this.m_CurrentMaterialIndex = 0;
						this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[0]);
						return true;
					}
					Material material;
					bool flag137 = MaterialReferenceManager.TryGetMaterial(num24, out material);
					if (flag137)
					{
						this.m_CurrentMaterial = material;
						this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
						this.m_MaterialReferenceStack.Add(this.m_MaterialReferences[this.m_CurrentMaterialIndex]);
					}
					else
					{
						bool flag138 = !flag;
						if (flag138)
						{
							isThreadSuccess = false;
							return false;
						}
						material = Resources.Load<Material>(textSettings.defaultFontAssetPath + new string(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength));
						bool flag139 = material == null;
						if (flag139)
						{
							return false;
						}
						MaterialReferenceManager.AddFontMaterial(num24, material);
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
					goto IL_4569;
				}
				float num14 = TextGeneratorUtilities.ConvertToFloat(this.m_HtmlTag, this.m_XmlAttribute[0].valueStartIndex, this.m_XmlAttribute[0].valueLength);
				bool flag140 = num14 == -32767f;
				if (flag140)
				{
					return false;
				}
				switch (tagUnitType)
				{
				case TagUnitType.Pixels:
					this.m_BaselineOffset = num14 * generationSettings.pixelsPerPoint;
					return true;
				case TagUnitType.FontUnits:
					this.m_BaselineOffset = num14 * this.m_CurrentFontSize;
					return true;
				case TagUnitType.Percentage:
					return false;
				default:
					return false;
				}
			}
			IL_3C64:
			this.m_FontStyleInternal |= FontStyles.UpperCase;
			this.m_FontStyleStack.Add(FontStyles.UpperCase);
			return true;
			IL_4569:
			return false;
		}

		internal void CloseLastLinkTag(TextInfo textInfo)
		{
			bool flag = textInfo.linkInfo.Length != 0 && textInfo.linkCount > 0;
			if (flag)
			{
				this.CloseLinkTag(textInfo, textInfo.linkCount - 1);
			}
		}

		internal void CloseAllLinkTags(TextInfo textInfo)
		{
			for (int i = textInfo.linkCount - 1; i >= 0; i--)
			{
				this.CloseLinkTag(textInfo, i);
			}
		}

		private void CloseLinkTag(TextInfo textInfo, int index)
		{
			bool flag = textInfo.linkInfo[index].linkTextLength == -1;
			if (flag)
			{
				textInfo.linkInfo[index].linkTextLength = this.m_CharacterCount - textInfo.linkInfo[index].linkTextfirstCharacterIndex;
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

		private int RestoreWordWrappingState(ref WordWrapState state, TextInfo textInfo)
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

		private void SaveGlyphVertexInfo(float padding, float stylePadding, Color32 vertexColor, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.position = textInfo.textElementInfo[this.m_CharacterCount].bottomLeft;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.position = textInfo.textElementInfo[this.m_CharacterCount].topLeft;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.position = textInfo.textElementInfo[this.m_CharacterCount].topRight;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.position = textInfo.textElementInfo[this.m_CharacterCount].bottomRight;
			vertexColor.a = ((this.m_FontColor32.a < vertexColor.a) ? this.m_FontColor32.a : vertexColor.a);
			bool flag = (this.m_CurrentFontAsset.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536;
			vertexColor = (flag ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, vertexColor.a) : vertexColor);
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.color = vertexColor;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.color = vertexColor;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.color = vertexColor;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.color = vertexColor;
			bool flag2 = this.m_ColorGradientPreset != null && !flag;
			if (flag2)
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
			Color32 color = (this.m_TintSprite ? ColorUtilities.MultiplyColors(this.m_SpriteColor, vertexColor) : this.m_SpriteColor);
			color.a = ((color.a < this.m_FontColor32.a) ? ((color.a < vertexColor.a) ? color.a : vertexColor.a) : this.m_FontColor32.a);
			Color32 color2 = color;
			Color32 color3 = color;
			Color32 color4 = color;
			Color32 color5 = color;
			bool flag = this.m_ColorGradientPreset != null;
			if (flag)
			{
				color2 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color2, this.m_ColorGradientPreset.bottomLeft) : color2);
				color3 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color3, this.m_ColorGradientPreset.topLeft) : color3);
				color4 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color4, this.m_ColorGradientPreset.topRight) : color4);
				color5 = (this.m_TintSprite ? ColorUtilities.MultiplyColors(color5, this.m_ColorGradientPreset.bottomRight) : color5);
			}
			this.m_TintSprite = false;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomLeft.color = color2;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopLeft.color = color3;
			textInfo.textElementInfo[this.m_CharacterCount].vertexTopRight.color = color4;
			textInfo.textElementInfo[this.m_CharacterCount].vertexBottomRight.color = color5;
			Vector2 vector = new Vector2((float)this.m_CachedTextElement.glyph.glyphRect.x / this.m_CurrentSpriteAsset.width, (float)this.m_CachedTextElement.glyph.glyphRect.y / this.m_CurrentSpriteAsset.height);
			Vector2 vector2 = new Vector2(vector.x, (float)(this.m_CachedTextElement.glyph.glyphRect.y + this.m_CachedTextElement.glyph.glyphRect.height) / this.m_CurrentSpriteAsset.height);
			Vector2 vector3 = new Vector2((float)(this.m_CachedTextElement.glyph.glyphRect.x + this.m_CachedTextElement.glyph.glyphRect.width) / this.m_CurrentSpriteAsset.width, vector2.y);
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
				int materialIndex = this.m_Underline.materialIndex;
				int vertexCount = textInfo.meshInfo[materialIndex].vertexCount;
				int num = vertexCount + 12;
				bool flag2 = num > textInfo.meshInfo[materialIndex].vertexBufferSize;
				if (flag2)
				{
					textInfo.meshInfo[materialIndex].ResizeMeshInfo(num / 4, generationSettings.isIMGUI);
				}
				start.y = Mathf.Min(start.y, end.y);
				end.y = Mathf.Min(start.y, end.y);
				GlyphMetrics metrics = this.m_Underline.character.glyph.metrics;
				GlyphRect glyphRect = this.m_Underline.character.glyph.glyphRect;
				float underlineThickness = this.m_Underline.fontAsset.faceInfo.underlineThickness;
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
				TextCoreVertex[] vertexData = textInfo.meshInfo[materialIndex].vertexData;
				float x = start.x;
				float num6 = start.x + num2;
				float num7 = end.x - num2;
				float x2 = end.x;
				float num8 = start.y - (underlineThickness + this.m_Padding) * maxScale;
				float num9 = start.y + this.m_Padding * maxScale;
				vertexData[vertexCount].position = new Vector3(x, num8);
				vertexData[vertexCount + 1].position = new Vector3(x, num9);
				vertexData[vertexCount + 2].position = new Vector3(num6, num9);
				vertexData[vertexCount + 3].position = new Vector3(num6, num8);
				vertexData[vertexCount + 4].position = new Vector3(num6, num8);
				vertexData[vertexCount + 5].position = new Vector3(num6, num9);
				vertexData[vertexCount + 6].position = new Vector3(num7, num9);
				vertexData[vertexCount + 7].position = new Vector3(num7, num8);
				vertexData[vertexCount + 8].position = new Vector3(num7, num8);
				vertexData[vertexCount + 9].position = new Vector3(num7, num9);
				vertexData[vertexCount + 10].position = new Vector3(x2, num9);
				vertexData[vertexCount + 11].position = new Vector3(x2, num8);
				Vector3 vector;
				vector.x = 0f;
				vector.y = generationSettings.screenRect.height;
				vector.z = 0f;
				for (int i = 0; i < 12; i++)
				{
					textInfo.meshInfo[materialIndex].vertexData[vertexCount + i].position.y = textInfo.meshInfo[materialIndex].vertexData[vertexCount + i].position.y * -1f + vector.y;
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
				vertexData[vertexCount].uv0 = new Vector4(num13, num18);
				vertexData[1 + vertexCount].uv0 = new Vector4(num13, num19);
				vertexData[2 + vertexCount].uv0 = new Vector4(num14, num19);
				vertexData[3 + vertexCount].uv0 = new Vector4(num14, num18);
				vertexData[4 + vertexCount].uv0 = new Vector4(num15, num18);
				vertexData[5 + vertexCount].uv0 = new Vector4(num15, num19);
				vertexData[6 + vertexCount].uv0 = new Vector4(num15, num19);
				vertexData[7 + vertexCount].uv0 = new Vector4(num15, num18);
				vertexData[8 + vertexCount].uv0 = new Vector4(num17, num18);
				vertexData[9 + vertexCount].uv0 = new Vector4(num17, num19);
				vertexData[10 + vertexCount].uv0 = new Vector4(num16, num19);
				vertexData[11 + vertexCount].uv0 = new Vector4(num16, num18);
				float num20 = 1f / num5;
				float num21 = (vertexData[vertexCount + 2].position.x - start.x) * num20;
				vertexData[vertexCount].uv2 = new Vector2(0f, 0f);
				vertexData[1 + vertexCount].uv2 = new Vector2(0f, 1f);
				vertexData[2 + vertexCount].uv2 = new Vector2(num21, 1f);
				vertexData[3 + vertexCount].uv2 = new Vector2(num21, 0f);
				float num22 = (vertexData[vertexCount + 4].position.x - start.x) * num20;
				num21 = (vertexData[vertexCount + 6].position.x - start.x) * num20;
				vertexData[4 + vertexCount].uv2 = new Vector2(num22, 0f);
				vertexData[5 + vertexCount].uv2 = new Vector2(num22, 1f);
				vertexData[6 + vertexCount].uv2 = new Vector2(num21, 1f);
				vertexData[7 + vertexCount].uv2 = new Vector2(num21, 0f);
				num22 = (vertexData[vertexCount + 8].position.x - start.x) * num20;
				vertexData[8 + vertexCount].uv2 = new Vector2(num22, 0f);
				vertexData[9 + vertexCount].uv2 = new Vector2(num22, 1f);
				vertexData[10 + vertexCount].uv2 = new Vector2(1f, 1f);
				vertexData[11 + vertexCount].uv2 = new Vector2(1f, 0f);
				underlineColor.a = ((this.m_FontColor32.a < underlineColor.a) ? this.m_FontColor32.a : underlineColor.a);
				for (int j = 0; j < 12; j++)
				{
					vertexData[j + vertexCount].color = underlineColor;
				}
				MeshInfo[] meshInfo = textInfo.meshInfo;
				int num23 = materialIndex;
				meshInfo[num23].vertexCount = meshInfo[num23].vertexCount + 12;
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
				bool flag2 = num > textInfo.meshInfo[this.m_CurrentMaterialIndex].vertexBufferSize;
				if (flag2)
				{
					textInfo.meshInfo[this.m_CurrentMaterialIndex].ResizeMeshInfo(num / 4, generationSettings.isIMGUI);
				}
				TextCoreVertex[] vertexData = textInfo.meshInfo[this.m_CurrentMaterialIndex].vertexData;
				vertexData[vertexCount].position = start;
				vertexData[vertexCount + 1].position = new Vector3(start.x, end.y, 0f);
				vertexData[vertexCount + 2].position = end;
				vertexData[vertexCount + 3].position = new Vector3(end.x, start.y, 0f);
				Vector3 vector;
				vector.x = 0f;
				vector.y = generationSettings.screenRect.height;
				vector.z = 0f;
				for (int i = 0; i < 4; i++)
				{
					vertexData[vertexCount + i].position.y = vertexData[vertexCount + i].position.y * -1f + vector.y;
				}
				int atlasWidth = this.m_Underline.fontAsset.atlasWidth;
				int atlasHeight = this.m_Underline.fontAsset.atlasHeight;
				GlyphRect glyphRect = this.m_Underline.character.glyph.glyphRect;
				Vector2 vector2 = new Vector2(((float)glyphRect.x + (float)glyphRect.width / 2f) / (float)atlasWidth, ((float)glyphRect.y + (float)glyphRect.height / 2f) / (float)atlasHeight);
				Vector2 vector3 = new Vector2(1f / (float)atlasWidth, 1f / (float)atlasHeight);
				vertexData[vertexCount].uv0 = vector2 - vector3;
				vertexData[1 + vertexCount].uv0 = vector2 + new Vector2(-vector3.x, vector3.y);
				vertexData[2 + vertexCount].uv0 = vector2 + vector3;
				vertexData[3 + vertexCount].uv0 = vector2 + new Vector2(vector3.x, -vector3.y);
				Vector2 vector4 = new Vector2(0f, 1f);
				vertexData[vertexCount].uv2 = vector4;
				vertexData[1 + vertexCount].uv2 = vector4;
				vertexData[2 + vertexCount].uv2 = vector4;
				vertexData[3 + vertexCount].uv2 = vector4;
				highlightColor.a = ((this.m_FontColor32.a < highlightColor.a) ? this.m_FontColor32.a : highlightColor.a);
				vertexData[vertexCount].color = highlightColor;
				vertexData[1 + vertexCount].color = highlightColor;
				vertexData[2 + vertexCount].color = highlightColor;
				vertexData[3 + vertexCount].color = highlightColor;
				MeshInfo[] meshInfo = textInfo.meshInfo;
				int currentMaterialIndex = this.m_CurrentMaterialIndex;
				meshInfo[currentMaterialIndex].vertexCount = meshInfo[currentMaterialIndex].vertexCount + 4;
			}
		}

		private static void ClearMesh(bool updateMesh, TextInfo textInfo)
		{
			textInfo.ClearMeshInfo(updateMesh);
		}

		public void LayoutPhase(TextInfo textInfo, TextGenerationSettings generationSettings, float maxVisibleDescender)
		{
			int num = this.m_MaterialReferences[this.m_Underline.materialIndex].referenceCount * 4;
			textInfo.meshInfo[this.m_CurrentMaterialIndex].Clear(false);
			Vector3 vector = Vector3.zero;
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
							goto IL_042B;
						}
					}
					else if (textAlignment2 <= TextAlignment.TopGeoAligned)
					{
						if (textAlignment2 != TextAlignment.TopFlush && textAlignment2 != TextAlignment.TopGeoAligned)
						{
							goto IL_042B;
						}
					}
					else
					{
						if (textAlignment2 - TextAlignment.MiddleLeft > 1 && textAlignment2 != TextAlignment.MiddleRight)
						{
							goto IL_042B;
						}
						goto IL_02D5;
					}
					vector = rectTransformCorners[1] + new Vector3(0f, 0f - this.m_MaxAscender, 0f);
					goto IL_042B;
				}
				if (textAlignment2 <= TextAlignment.BottomCenter)
				{
					if (textAlignment2 <= TextAlignment.MiddleFlush)
					{
						if (textAlignment2 != TextAlignment.MiddleJustified && textAlignment2 != TextAlignment.MiddleFlush)
						{
							goto IL_042B;
						}
						goto IL_02D5;
					}
					else
					{
						if (textAlignment2 == TextAlignment.MiddleGeoAligned)
						{
							goto IL_02D5;
						}
						if (textAlignment2 - TextAlignment.BottomLeft > 1)
						{
							goto IL_042B;
						}
					}
				}
				else if (textAlignment2 <= TextAlignment.BottomJustified)
				{
					if (textAlignment2 != TextAlignment.BottomRight && textAlignment2 != TextAlignment.BottomJustified)
					{
						goto IL_042B;
					}
				}
				else if (textAlignment2 != TextAlignment.BottomFlush && textAlignment2 != TextAlignment.BottomGeoAligned)
				{
					goto IL_042B;
				}
				vector = rectTransformCorners[0] + new Vector3(0f, 0f - maxVisibleDescender, 0f);
				goto IL_042B;
				IL_02D5:
				vector = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f, 0f - (this.m_MaxAscender + maxVisibleDescender) / 2f, 0f);
			}
			else
			{
				if (textAlignment2 <= TextAlignment.MidlineRight)
				{
					if (textAlignment2 <= TextAlignment.BaselineJustified)
					{
						if (textAlignment2 - TextAlignment.BaselineLeft > 1 && textAlignment2 != TextAlignment.BaselineRight && textAlignment2 != TextAlignment.BaselineJustified)
						{
							goto IL_042B;
						}
					}
					else if (textAlignment2 <= TextAlignment.BaselineGeoAligned)
					{
						if (textAlignment2 != TextAlignment.BaselineFlush && textAlignment2 != TextAlignment.BaselineGeoAligned)
						{
							goto IL_042B;
						}
					}
					else
					{
						if (textAlignment2 - TextAlignment.MidlineLeft > 1 && textAlignment2 != TextAlignment.MidlineRight)
						{
							goto IL_042B;
						}
						goto IL_0384;
					}
					vector = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f, 0f, 0f);
					goto IL_042B;
				}
				if (textAlignment2 <= TextAlignment.CaplineCenter)
				{
					if (textAlignment2 <= TextAlignment.MidlineFlush)
					{
						if (textAlignment2 != TextAlignment.MidlineJustified && textAlignment2 != TextAlignment.MidlineFlush)
						{
							goto IL_042B;
						}
						goto IL_0384;
					}
					else
					{
						if (textAlignment2 == TextAlignment.MidlineGeoAligned)
						{
							goto IL_0384;
						}
						if (textAlignment2 - TextAlignment.CaplineLeft > 1)
						{
							goto IL_042B;
						}
					}
				}
				else if (textAlignment2 <= TextAlignment.CaplineJustified)
				{
					if (textAlignment2 != TextAlignment.CaplineRight && textAlignment2 != TextAlignment.CaplineJustified)
					{
						goto IL_042B;
					}
				}
				else if (textAlignment2 != TextAlignment.CaplineFlush && textAlignment2 != TextAlignment.CaplineGeoAligned)
				{
					goto IL_042B;
				}
				vector = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f, 0f - this.m_MaxCapHeight / 2f, 0f);
				goto IL_042B;
				IL_0384:
				vector = (rectTransformCorners[0] + rectTransformCorners[1]) / 2f + new Vector3(0f, 0f - (this.m_MeshExtents.max.y + this.m_MeshExtents.min.y) / 2f, 0f);
			}
			IL_042B:
			Vector3 vector2 = Vector3.zero;
			Vector3 vector3 = Vector3.zero;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			bool flag = false;
			bool flag2 = false;
			int num5 = 0;
			Color32 color = Color.white;
			Color32 color2 = Color.white;
			HighlightState highlightState = new HighlightState(new Color32(byte.MaxValue, byte.MaxValue, 0, 64), Offset.zero);
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 32767f;
			float num11 = 0f;
			float num12 = 0f;
			float num13 = 0f;
			bool flag3 = false;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			bool flag4 = false;
			Vector3 zero3 = Vector3.zero;
			Vector3 zero4 = Vector3.zero;
			bool flag5 = false;
			Vector3 vector4 = Vector3.zero;
			Vector3 vector5 = Vector3.zero;
			TextElementInfo[] textElementInfo = textInfo.textElementInfo;
			int i = 0;
			while (i < this.m_CharacterCount)
			{
				FontAsset fontAsset = textElementInfo[i].fontAsset;
				char c = (char)textElementInfo[i].character;
				bool flag6 = char.IsWhiteSpace(c);
				int lineNumber = textElementInfo[i].lineNumber;
				LineInfo lineInfo = textInfo.lineInfo[lineNumber];
				num3 = lineNumber + 1;
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
								goto IL_077C;
							case TextAlignment.TopCenter:
								goto IL_07CE;
							case (TextAlignment)259:
								break;
							case TextAlignment.TopRight:
								goto IL_085C;
							default:
								if (textAlignment4 == TextAlignment.TopJustified || textAlignment4 == TextAlignment.TopFlush)
								{
									goto IL_08BA;
								}
								break;
							}
						}
						else
						{
							if (textAlignment4 == TextAlignment.TopGeoAligned)
							{
								goto IL_0807;
							}
							switch (textAlignment4)
							{
							case TextAlignment.MiddleLeft:
								goto IL_077C;
							case TextAlignment.MiddleCenter:
								goto IL_07CE;
							case (TextAlignment)515:
								break;
							case TextAlignment.MiddleRight:
								goto IL_085C;
							default:
								if (textAlignment4 == TextAlignment.MiddleJustified)
								{
									goto IL_08BA;
								}
								break;
							}
						}
					}
					else if (textAlignment4 <= TextAlignment.BottomRight)
					{
						if (textAlignment4 == TextAlignment.MiddleFlush)
						{
							goto IL_08BA;
						}
						if (textAlignment4 == TextAlignment.MiddleGeoAligned)
						{
							goto IL_0807;
						}
						switch (textAlignment4)
						{
						case TextAlignment.BottomLeft:
							goto IL_077C;
						case TextAlignment.BottomCenter:
							goto IL_07CE;
						case TextAlignment.BottomRight:
							goto IL_085C;
						}
					}
					else
					{
						if (textAlignment4 == TextAlignment.BottomJustified || textAlignment4 == TextAlignment.BottomFlush)
						{
							goto IL_08BA;
						}
						if (textAlignment4 == TextAlignment.BottomGeoAligned)
						{
							goto IL_0807;
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
							goto IL_077C;
						case TextAlignment.BaselineCenter:
							goto IL_07CE;
						case (TextAlignment)2051:
							break;
						case TextAlignment.BaselineRight:
							goto IL_085C;
						default:
							if (textAlignment4 == TextAlignment.BaselineJustified || textAlignment4 == TextAlignment.BaselineFlush)
							{
								goto IL_08BA;
							}
							break;
						}
					}
					else
					{
						if (textAlignment4 == TextAlignment.BaselineGeoAligned)
						{
							goto IL_0807;
						}
						switch (textAlignment4)
						{
						case TextAlignment.MidlineLeft:
							goto IL_077C;
						case TextAlignment.MidlineCenter:
							goto IL_07CE;
						case (TextAlignment)4099:
							break;
						case TextAlignment.MidlineRight:
							goto IL_085C;
						default:
							if (textAlignment4 == TextAlignment.MidlineJustified)
							{
								goto IL_08BA;
							}
							break;
						}
					}
				}
				else if (textAlignment4 <= TextAlignment.CaplineRight)
				{
					if (textAlignment4 == TextAlignment.MidlineFlush)
					{
						goto IL_08BA;
					}
					if (textAlignment4 == TextAlignment.MidlineGeoAligned)
					{
						goto IL_0807;
					}
					switch (textAlignment4)
					{
					case TextAlignment.CaplineLeft:
						goto IL_077C;
					case TextAlignment.CaplineCenter:
						goto IL_07CE;
					case TextAlignment.CaplineRight:
						goto IL_085C;
					}
				}
				else
				{
					if (textAlignment4 == TextAlignment.CaplineJustified || textAlignment4 == TextAlignment.CaplineFlush)
					{
						goto IL_08BA;
					}
					if (textAlignment4 == TextAlignment.CaplineGeoAligned)
					{
						goto IL_0807;
					}
				}
				IL_0B78:
				vector3 = vector + vector2;
				vector3 = new Vector3(this.Round(vector3.x), this.Round(vector3.y));
				bool isVisible = textElementInfo[i].isVisible;
				bool flag7 = isVisible;
				if (flag7)
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
						textElementInfo[i].vertexBottomLeft.uv2.x = 0f;
						textElementInfo[i].vertexTopLeft.uv2.x = 0f;
						textElementInfo[i].vertexTopRight.uv2.x = 1f;
						textElementInfo[i].vertexBottomRight.uv2.x = 1f;
						textElementInfo[i].vertexBottomLeft.uv2.y = 0f;
						textElementInfo[i].vertexTopLeft.uv2.y = 1f;
						textElementInfo[i].vertexTopRight.uv2.y = 1f;
						textElementInfo[i].vertexBottomRight.uv2.y = 0f;
						num6 = textElementInfo[i].scale * (1f - this.m_CharWidthAdjDelta) * 1f;
						bool flag8 = !textElementInfo[i].isUsingAlternateTypeface && (textElementInfo[i].style & FontStyles.Bold) == FontStyles.Bold;
						if (flag8)
						{
							num6 *= -1f;
						}
						textElementInfo[i].vertexBottomLeft.uv.w = num6;
						textElementInfo[i].vertexTopLeft.uv.w = num6;
						textElementInfo[i].vertexTopRight.uv.w = num6;
						textElementInfo[i].vertexBottomRight.uv.w = num6;
						textElementInfo[i].vertexBottomLeft.uv2.x = 1f;
						textElementInfo[i].vertexBottomLeft.uv2.y = num6;
						textElementInfo[i].vertexTopLeft.uv2.x = 1f;
						textElementInfo[i].vertexTopLeft.uv2.y = num6;
						textElementInfo[i].vertexTopRight.uv2.x = 1f;
						textElementInfo[i].vertexTopRight.uv2.y = num6;
						textElementInfo[i].vertexBottomRight.uv2.x = 1f;
						textElementInfo[i].vertexBottomRight.uv2.y = num6;
					}
					bool flag9 = i < 99999 && num2 < 99999 && lineNumber < 99999;
					if (flag9)
					{
						TextElementInfo[] array = textElementInfo;
						int num14 = i;
						array[num14].vertexBottomLeft.position = array[num14].vertexBottomLeft.position + vector3;
						TextElementInfo[] array2 = textElementInfo;
						int num15 = i;
						array2[num15].vertexTopLeft.position = array2[num15].vertexTopLeft.position + vector3;
						TextElementInfo[] array3 = textElementInfo;
						int num16 = i;
						array3[num16].vertexTopRight.position = array3[num16].vertexTopRight.position + vector3;
						TextElementInfo[] array4 = textElementInfo;
						int num17 = i;
						array4[num17].vertexBottomRight.position = array4[num17].vertexBottomRight.position + vector3;
					}
					else
					{
						textElementInfo[i].vertexBottomLeft.position = Vector3.zero;
						textElementInfo[i].vertexTopLeft.position = Vector3.zero;
						textElementInfo[i].vertexTopRight.position = Vector3.zero;
						textElementInfo[i].vertexBottomRight.position = Vector3.zero;
						textElementInfo[i].isVisible = false;
					}
					bool flag10 = elementType == TextElementType.Character;
					if (flag10)
					{
						TextGeneratorUtilities.FillCharacterVertexBuffers(i, generationSettings.shouldConvertToLinearSpace, generationSettings, textInfo, this.NeedToRound);
					}
					else
					{
						bool flag11 = elementType == TextElementType.Sprite;
						if (flag11)
						{
							TextGeneratorUtilities.FillSpriteVertexBuffers(i, generationSettings.shouldConvertToLinearSpace, generationSettings, textInfo);
						}
					}
				}
				TextElementInfo[] textElementInfo2 = textInfo.textElementInfo;
				int num18 = i;
				textElementInfo2[num18].bottomLeft = textElementInfo2[num18].bottomLeft + vector3;
				TextElementInfo[] textElementInfo3 = textInfo.textElementInfo;
				int num19 = i;
				textElementInfo3[num19].topLeft = textElementInfo3[num19].topLeft + vector3;
				TextElementInfo[] textElementInfo4 = textInfo.textElementInfo;
				int num20 = i;
				textElementInfo4[num20].topRight = textElementInfo4[num20].topRight + vector3;
				TextElementInfo[] textElementInfo5 = textInfo.textElementInfo;
				int num21 = i;
				textElementInfo5[num21].bottomRight = textElementInfo5[num21].bottomRight + vector3;
				TextElementInfo[] textElementInfo6 = textInfo.textElementInfo;
				int num22 = i;
				textElementInfo6[num22].origin = textElementInfo6[num22].origin + vector3.x;
				TextElementInfo[] textElementInfo7 = textInfo.textElementInfo;
				int num23 = i;
				textElementInfo7[num23].xAdvance = textElementInfo7[num23].xAdvance + vector3.x;
				TextElementInfo[] textElementInfo8 = textInfo.textElementInfo;
				int num24 = i;
				textElementInfo8[num24].ascender = textElementInfo8[num24].ascender + vector3.y;
				TextElementInfo[] textElementInfo9 = textInfo.textElementInfo;
				int num25 = i;
				textElementInfo9[num25].descender = textElementInfo9[num25].descender + vector3.y;
				TextElementInfo[] textElementInfo10 = textInfo.textElementInfo;
				int num26 = i;
				textElementInfo10[num26].baseLine = textElementInfo10[num26].baseLine + vector3.y;
				bool flag12 = isVisible;
				if (flag12)
				{
				}
				bool flag13 = lineNumber != num4 || i == this.m_CharacterCount - 1;
				if (flag13)
				{
					bool flag14 = lineNumber != num4;
					if (flag14)
					{
						int num27 = ((generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespace || generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) ? textInfo.lineInfo[num4].lastCharacterIndex : textInfo.lineInfo[num4].lastVisibleCharacterIndex);
						LineInfo[] lineInfo2 = textInfo.lineInfo;
						int num28 = num4;
						lineInfo2[num28].baseline = lineInfo2[num28].baseline + vector3.y;
						LineInfo[] lineInfo3 = textInfo.lineInfo;
						int num29 = num4;
						lineInfo3[num29].ascender = lineInfo3[num29].ascender + vector3.y;
						LineInfo[] lineInfo4 = textInfo.lineInfo;
						int num30 = num4;
						lineInfo4[num30].descender = lineInfo4[num30].descender + vector3.y;
						LineInfo[] lineInfo5 = textInfo.lineInfo;
						int num31 = num4;
						lineInfo5[num31].maxAdvance = lineInfo5[num31].maxAdvance + vector3.x;
						textInfo.lineInfo[num4].lineExtents.min = new Vector2(textInfo.textElementInfo[textInfo.lineInfo[num4].firstCharacterIndex].bottomLeft.x, textInfo.lineInfo[num4].descender);
						textInfo.lineInfo[num4].lineExtents.max = new Vector2(textInfo.textElementInfo[num27].topRight.x, textInfo.lineInfo[num4].ascender);
					}
					bool flag15 = i == this.m_CharacterCount - 1;
					if (flag15)
					{
						int num32 = ((generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespace || generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) ? textInfo.lineInfo[lineNumber].lastCharacterIndex : textInfo.lineInfo[lineNumber].lastVisibleCharacterIndex);
						LineInfo[] lineInfo6 = textInfo.lineInfo;
						int num33 = lineNumber;
						lineInfo6[num33].baseline = lineInfo6[num33].baseline + vector3.y;
						LineInfo[] lineInfo7 = textInfo.lineInfo;
						int num34 = lineNumber;
						lineInfo7[num34].ascender = lineInfo7[num34].ascender + vector3.y;
						LineInfo[] lineInfo8 = textInfo.lineInfo;
						int num35 = lineNumber;
						lineInfo8[num35].descender = lineInfo8[num35].descender + vector3.y;
						LineInfo[] lineInfo9 = textInfo.lineInfo;
						int num36 = lineNumber;
						lineInfo9[num36].maxAdvance = lineInfo9[num36].maxAdvance + vector3.x;
						textInfo.lineInfo[lineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[textInfo.lineInfo[lineNumber].firstCharacterIndex].bottomLeft.x, textInfo.lineInfo[lineNumber].descender);
						textInfo.lineInfo[lineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[num32].topRight.x, textInfo.lineInfo[lineNumber].ascender);
					}
				}
				bool flag16 = char.IsLetterOrDigit(c) || c == '-' || c == '\u00ad' || c == '‐' || c == '‑';
				if (flag16)
				{
					bool flag17 = !flag2;
					if (flag17)
					{
						flag2 = true;
						num5 = i;
					}
					bool flag18 = flag2 && i == this.m_CharacterCount - 1;
					if (flag18)
					{
						int num37 = textInfo.wordInfo.Length;
						int wordCount = textInfo.wordCount;
						bool flag19 = textInfo.wordCount + 1 > num37;
						if (flag19)
						{
							TextInfo.Resize<WordInfo>(ref textInfo.wordInfo, num37 + 1);
						}
						int num38 = i;
						textInfo.wordInfo[wordCount].firstCharacterIndex = num5;
						textInfo.wordInfo[wordCount].lastCharacterIndex = num38;
						textInfo.wordInfo[wordCount].characterCount = num38 - num5 + 1;
						num2++;
						textInfo.wordCount++;
						LineInfo[] lineInfo10 = textInfo.lineInfo;
						int num39 = lineNumber;
						lineInfo10[num39].wordCount = lineInfo10[num39].wordCount + 1;
					}
				}
				else
				{
					bool flag20 = flag2 || (i == 0 && (!char.IsPunctuation(c) || flag6 || c == '\u200b' || i == this.m_CharacterCount - 1));
					if (flag20)
					{
						bool flag21 = i > 0 && i < textElementInfo.Length - 1 && i < this.m_CharacterCount && (c == '\'' || c == '’') && char.IsLetterOrDigit((char)textElementInfo[i - 1].character) && char.IsLetterOrDigit((char)textElementInfo[i + 1].character);
						if (!flag21)
						{
							int num38 = ((i == this.m_CharacterCount - 1 && char.IsLetterOrDigit(c)) ? i : (i - 1));
							flag2 = false;
							int num40 = textInfo.wordInfo.Length;
							int wordCount2 = textInfo.wordCount;
							bool flag22 = textInfo.wordCount + 1 > num40;
							if (flag22)
							{
								TextInfo.Resize<WordInfo>(ref textInfo.wordInfo, num40 + 1);
							}
							textInfo.wordInfo[wordCount2].firstCharacterIndex = num5;
							textInfo.wordInfo[wordCount2].lastCharacterIndex = num38;
							textInfo.wordInfo[wordCount2].characterCount = num38 - num5 + 1;
							num2++;
							textInfo.wordCount++;
							LineInfo[] lineInfo11 = textInfo.lineInfo;
							int num41 = lineNumber;
							lineInfo11[num41].wordCount = lineInfo11[num41].wordCount + 1;
						}
					}
				}
				bool flag23 = (textInfo.textElementInfo[i].style & FontStyles.Underline) == FontStyles.Underline;
				bool flag24 = flag23;
				if (flag24)
				{
					bool flag25 = true;
					textInfo.textElementInfo[i].underlineVertexIndex = num;
					bool flag26 = i > 99999 || lineNumber > 99999;
					if (flag26)
					{
						flag25 = false;
					}
					bool flag27 = !flag6 && c != '\u200b';
					if (flag27)
					{
						num9 = Mathf.Max(num9, textInfo.textElementInfo[i].scale);
						num7 = Mathf.Max(num7, Mathf.Abs(num6));
						num10 = Mathf.Min(num10, textInfo.textElementInfo[i].baseLine + fontAsset.faceInfo.underlineOffset * num9);
					}
					bool flag28 = !flag3 && flag25 && i <= lineInfo.lastVisibleCharacterIndex && c != '\n' && c != '\v' && c != '\r';
					if (flag28)
					{
						bool flag29 = i == lineInfo.lastVisibleCharacterIndex && char.IsSeparator(c);
						if (!flag29)
						{
							flag3 = true;
							num8 = textInfo.textElementInfo[i].scale;
							bool flag30 = num9 == 0f;
							if (flag30)
							{
								num9 = num8;
								num7 = num6;
							}
							zero = new Vector3(textInfo.textElementInfo[i].bottomLeft.x, num10, 0f);
							color = textInfo.textElementInfo[i].underlineColor;
						}
					}
					bool flag31 = flag3 && this.m_CharacterCount == 1;
					if (flag31)
					{
						flag3 = false;
						zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num10, 0f);
						float num42 = textInfo.textElementInfo[i].scale;
						this.DrawUnderlineMesh(zero, zero2, num8, num42, num9, num7, color, generationSettings, textInfo);
						num9 = 0f;
						num7 = 0f;
						num10 = 32767f;
					}
					else
					{
						bool flag32 = flag3 && (i == lineInfo.lastCharacterIndex || i >= lineInfo.lastVisibleCharacterIndex);
						if (flag32)
						{
							bool flag33 = flag6 || c == '\u200b';
							float num42;
							if (flag33)
							{
								int lastVisibleCharacterIndex = lineInfo.lastVisibleCharacterIndex;
								zero2 = new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex].topRight.x, num10, 0f);
								num42 = textInfo.textElementInfo[lastVisibleCharacterIndex].scale;
							}
							else
							{
								zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num10, 0f);
								num42 = textInfo.textElementInfo[i].scale;
							}
							flag3 = false;
							this.DrawUnderlineMesh(zero, zero2, num8, num42, num9, num7, color, generationSettings, textInfo);
							num9 = 0f;
							num7 = 0f;
							num10 = 32767f;
						}
						else
						{
							bool flag34 = flag3 && !flag25;
							if (flag34)
							{
								flag3 = false;
								zero2 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, num10, 0f);
								float num42 = textInfo.textElementInfo[i - 1].scale;
								this.DrawUnderlineMesh(zero, zero2, num8, num42, num9, num7, color, generationSettings, textInfo);
								num9 = 0f;
								num7 = 0f;
								num10 = 32767f;
							}
							else
							{
								bool flag35 = flag3 && i < this.m_CharacterCount - 1 && !ColorUtilities.CompareColors(color, textInfo.textElementInfo[i + 1].underlineColor);
								if (flag35)
								{
									flag3 = false;
									zero2 = new Vector3(textInfo.textElementInfo[i].topRight.x, num10, 0f);
									float num42 = textInfo.textElementInfo[i].scale;
									this.DrawUnderlineMesh(zero, zero2, num8, num42, num9, num7, color, generationSettings, textInfo);
									num9 = 0f;
									num7 = 0f;
									num10 = 32767f;
								}
							}
						}
					}
				}
				else
				{
					bool flag36 = flag3;
					if (flag36)
					{
						flag3 = false;
						zero2 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, num10, 0f);
						float num42 = textInfo.textElementInfo[i - 1].scale;
						this.DrawUnderlineMesh(zero, zero2, num8, num42, num9, num7, color, generationSettings, textInfo);
						num9 = 0f;
						num7 = 0f;
						num10 = 32767f;
					}
				}
				bool flag37 = (textInfo.textElementInfo[i].style & FontStyles.Strikethrough) == FontStyles.Strikethrough;
				float strikethroughOffset = fontAsset.faceInfo.strikethroughOffset;
				bool flag38 = flag37;
				if (flag38)
				{
					bool flag39 = true;
					textInfo.textElementInfo[i].strikethroughVertexIndex = this.m_MaterialReferences[this.m_Underline.materialIndex].referenceCount * 4;
					bool flag40 = i > 99999 || lineNumber > 99999;
					if (flag40)
					{
						flag39 = false;
					}
					bool flag41 = !flag4 && flag39 && i <= lineInfo.lastVisibleCharacterIndex && c != '\n' && c != '\v' && c != '\r';
					if (flag41)
					{
						bool flag42 = i == lineInfo.lastVisibleCharacterIndex && char.IsSeparator(c);
						if (!flag42)
						{
							flag4 = true;
							num11 = textInfo.textElementInfo[i].pointSize;
							num12 = textInfo.textElementInfo[i].scale;
							zero3 = new Vector3(textInfo.textElementInfo[i].bottomLeft.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
							color2 = textInfo.textElementInfo[i].strikethroughColor;
							num13 = textInfo.textElementInfo[i].baseLine;
						}
					}
					bool flag43 = flag4 && this.m_CharacterCount == 1;
					if (flag43)
					{
						flag4 = false;
						zero4 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
						this.DrawUnderlineMesh(zero3, zero4, num12, num12, num12, num6, color2, generationSettings, textInfo);
					}
					else
					{
						bool flag44 = flag4 && i == lineInfo.lastCharacterIndex;
						if (flag44)
						{
							bool flag45 = flag6 || c == '\u200b';
							if (flag45)
							{
								int lastVisibleCharacterIndex2 = lineInfo.lastVisibleCharacterIndex;
								zero4 = new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex2].topRight.x, textInfo.textElementInfo[lastVisibleCharacterIndex2].baseLine + strikethroughOffset * num12, 0f);
							}
							else
							{
								zero4 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
							}
							flag4 = false;
							this.DrawUnderlineMesh(zero3, zero4, num12, num12, num12, num6, color2, generationSettings, textInfo);
						}
						else
						{
							bool flag46 = flag4 && i < this.m_CharacterCount && (textInfo.textElementInfo[i + 1].pointSize != num11 || !TextGeneratorUtilities.Approximately(textInfo.textElementInfo[i + 1].baseLine + vector3.y, num13));
							if (flag46)
							{
								flag4 = false;
								int lastVisibleCharacterIndex3 = lineInfo.lastVisibleCharacterIndex;
								bool flag47 = i > lastVisibleCharacterIndex3;
								if (flag47)
								{
									zero4 = new Vector3(textInfo.textElementInfo[lastVisibleCharacterIndex3].topRight.x, textInfo.textElementInfo[lastVisibleCharacterIndex3].baseLine + strikethroughOffset * num12, 0f);
								}
								else
								{
									zero4 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
								}
								this.DrawUnderlineMesh(zero3, zero4, num12, num12, num12, num6, color2, generationSettings, textInfo);
							}
							else
							{
								bool flag48 = flag4 && i < this.m_CharacterCount && fontAsset.GetHashCode() != textElementInfo[i + 1].fontAsset.GetHashCode();
								if (flag48)
								{
									flag4 = false;
									zero4 = new Vector3(textInfo.textElementInfo[i].topRight.x, textInfo.textElementInfo[i].baseLine + strikethroughOffset * num12, 0f);
									this.DrawUnderlineMesh(zero3, zero4, num12, num12, num12, num6, color2, generationSettings, textInfo);
								}
								else
								{
									bool flag49 = flag4 && !flag39;
									if (flag49)
									{
										flag4 = false;
										zero4 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, textInfo.textElementInfo[i - 1].baseLine + strikethroughOffset * num12, 0f);
										this.DrawUnderlineMesh(zero3, zero4, num12, num12, num12, num6, color2, generationSettings, textInfo);
									}
								}
							}
						}
					}
				}
				else
				{
					bool flag50 = flag4;
					if (flag50)
					{
						flag4 = false;
						zero4 = new Vector3(textInfo.textElementInfo[i - 1].topRight.x, textInfo.textElementInfo[i - 1].baseLine + strikethroughOffset * num12, 0f);
						this.DrawUnderlineMesh(zero3, zero4, num12, num12, num12, num6, color2, generationSettings, textInfo);
					}
				}
				bool flag51 = (textInfo.textElementInfo[i].style & FontStyles.Highlight) == FontStyles.Highlight;
				bool flag52 = flag51;
				if (flag52)
				{
					bool flag53 = true;
					bool flag54 = i > 99999 || lineNumber > 99999;
					if (flag54)
					{
						flag53 = false;
					}
					bool flag55 = !flag5 && flag53 && i <= lineInfo.lastVisibleCharacterIndex && c != '\n' && c != '\v' && c != '\r';
					if (flag55)
					{
						bool flag56 = i == lineInfo.lastVisibleCharacterIndex && char.IsSeparator(c);
						if (!flag56)
						{
							flag5 = true;
							vector4 = TextGeneratorUtilities.largePositiveVector2;
							vector5 = TextGeneratorUtilities.largeNegativeVector2;
							highlightState = textInfo.textElementInfo[i].highlightState;
						}
					}
					bool flag57 = flag5;
					if (flag57)
					{
						TextElementInfo textElementInfo11 = textInfo.textElementInfo[i];
						HighlightState highlightState2 = textElementInfo11.highlightState;
						bool flag58 = false;
						bool flag59 = highlightState != highlightState2;
						if (flag59)
						{
							bool flag60 = flag6;
							if (flag60)
							{
								vector5.x = (vector5.x - highlightState.padding.right + textElementInfo11.origin) / 2f;
							}
							else
							{
								vector5.x = (vector5.x - highlightState.padding.right + textElementInfo11.bottomLeft.x) / 2f;
							}
							vector4.y = Mathf.Min(vector4.y, textElementInfo11.descender);
							vector5.y = Mathf.Max(vector5.y, textElementInfo11.ascender);
							this.DrawTextHighlight(vector4, vector5, highlightState.color, generationSettings, textInfo);
							flag5 = true;
							vector4 = new Vector2(vector5.x, textElementInfo11.descender - highlightState2.padding.bottom);
							bool flag61 = flag6;
							if (flag61)
							{
								vector5 = new Vector2(textElementInfo11.xAdvance + highlightState2.padding.right, textElementInfo11.ascender + highlightState2.padding.top);
							}
							else
							{
								vector5 = new Vector2(textElementInfo11.topRight.x + highlightState2.padding.right, textElementInfo11.ascender + highlightState2.padding.top);
							}
							highlightState = highlightState2;
							flag58 = true;
						}
						bool flag62 = !flag58;
						if (flag62)
						{
							bool flag63 = flag6;
							if (flag63)
							{
								vector4.x = Mathf.Min(vector4.x, textElementInfo11.origin - highlightState.padding.left);
								vector5.x = Mathf.Max(vector5.x, textElementInfo11.xAdvance + highlightState.padding.right);
							}
							else
							{
								vector4.x = Mathf.Min(vector4.x, textElementInfo11.bottomLeft.x - highlightState.padding.left);
								vector5.x = Mathf.Max(vector5.x, textElementInfo11.topRight.x + highlightState.padding.right);
							}
							vector4.y = Mathf.Min(vector4.y, textElementInfo11.descender - highlightState.padding.bottom);
							vector5.y = Mathf.Max(vector5.y, textElementInfo11.ascender + highlightState.padding.top);
						}
					}
					bool flag64 = flag5 && this.m_CharacterCount == 1;
					if (flag64)
					{
						flag5 = false;
						this.DrawTextHighlight(vector4, vector5, highlightState.color, generationSettings, textInfo);
					}
					else
					{
						bool flag65 = flag5 && (i == lineInfo.lastCharacterIndex || i >= lineInfo.lastVisibleCharacterIndex);
						if (flag65)
						{
							flag5 = false;
							this.DrawTextHighlight(vector4, vector5, highlightState.color, generationSettings, textInfo);
						}
						else
						{
							bool flag66 = flag5 && !flag53;
							if (flag66)
							{
								flag5 = false;
								this.DrawTextHighlight(vector4, vector5, highlightState.color, generationSettings, textInfo);
							}
						}
					}
				}
				else
				{
					bool flag67 = flag5;
					if (flag67)
					{
						flag5 = false;
						this.DrawTextHighlight(vector4, vector5, highlightState.color, generationSettings, textInfo);
					}
				}
				num4 = lineNumber;
				i++;
				continue;
				IL_077C:
				bool flag68 = !generationSettings.isRightToLeft;
				if (flag68)
				{
					vector2 = new Vector3(0f + lineInfo.marginLeft, 0f, 0f);
				}
				else
				{
					vector2 = new Vector3(0f - lineInfo.maxAdvance, 0f, 0f);
				}
				goto IL_0B78;
				IL_07CE:
				vector2 = new Vector3(lineInfo.marginLeft + lineInfo.width / 2f - lineInfo.maxAdvance / 2f, 0f, 0f);
				goto IL_0B78;
				IL_0807:
				vector2 = new Vector3(lineInfo.marginLeft + lineInfo.width / 2f - (lineInfo.lineExtents.min.x + lineInfo.lineExtents.max.x) / 2f, 0f, 0f);
				goto IL_0B78;
				IL_085C:
				bool flag69 = !generationSettings.isRightToLeft;
				if (flag69)
				{
					vector2 = new Vector3(lineInfo.marginLeft + lineInfo.width - lineInfo.maxAdvance, 0f, 0f);
				}
				else
				{
					vector2 = new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f);
				}
				goto IL_0B78;
				IL_08BA:
				bool flag70 = i > lineInfo.lastVisibleCharacterIndex || c == '\n' || c == '\u00ad' || c == '\u200b' || c == '\u2060' || c == '\u0003';
				if (flag70)
				{
					goto IL_0B78;
				}
				char c2 = (char)textElementInfo[lineInfo.lastCharacterIndex].character;
				bool flag71 = (alignment & (TextAlignment)16) == (TextAlignment)16;
				bool flag72 = (!char.IsControl(c2) && lineNumber < this.m_LineNumber) || flag71 || lineInfo.maxAdvance > lineInfo.width;
				if (flag72)
				{
					bool flag73 = lineNumber != num4 || i == 0 || i == 0;
					if (flag73)
					{
						bool flag74 = !generationSettings.isRightToLeft;
						if (flag74)
						{
							vector2 = new Vector3(lineInfo.marginLeft, 0f, 0f);
						}
						else
						{
							vector2 = new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f);
						}
						bool flag75 = char.IsSeparator(c);
						flag = flag75;
					}
					else
					{
						float num43 = (generationSettings.isRightToLeft ? (lineInfo.width + lineInfo.maxAdvance) : (lineInfo.width - lineInfo.maxAdvance));
						int num44 = lineInfo.visibleCharacterCount - 1 + lineInfo.controlCharacterCount;
						int num45 = lineInfo.visibleSpaceCount - lineInfo.controlCharacterCount;
						bool flag76 = flag;
						if (flag76)
						{
							num45--;
							num44++;
						}
						float num46 = ((num45 > 0) ? 0.4f : 1f);
						bool flag77 = num45 < 1;
						if (flag77)
						{
							num45 = 1;
						}
						bool flag78 = c != '\u00a0' && (c == '\t' || char.IsSeparator(c));
						if (flag78)
						{
							bool flag79 = !generationSettings.isRightToLeft;
							if (flag79)
							{
								vector2 += new Vector3(num43 * (1f - num46) / (float)num45, 0f, 0f);
							}
							else
							{
								vector2 -= new Vector3(num43 * (1f - num46) / (float)num45, 0f, 0f);
							}
						}
						else
						{
							bool flag80 = !generationSettings.isRightToLeft;
							if (flag80)
							{
								vector2 += new Vector3(num43 * num46 / (float)num44, 0f, 0f);
							}
							else
							{
								vector2 -= new Vector3(num43 * num46 / (float)num44, 0f, 0f);
							}
						}
					}
				}
				else
				{
					bool flag81 = !generationSettings.isRightToLeft;
					if (flag81)
					{
						vector2 = new Vector3(lineInfo.marginLeft, 0f, 0f);
					}
					else
					{
						vector2 = new Vector3(lineInfo.marginLeft + lineInfo.width, 0f, 0f);
					}
				}
				goto IL_0B78;
			}
			textInfo.characterCount = this.m_CharacterCount;
			textInfo.spriteCount = this.m_SpriteCount;
			textInfo.lineCount = num3;
			textInfo.wordCount = ((num2 != 0 && this.m_CharacterCount > 0) ? num2 : 1);
		}

		private bool NeedToRound
		{
			get
			{
				return this.m_ShouldRenderBitmap;
			}
		}

		private float Round(float v)
		{
			bool flag = !this.NeedToRound;
			float num;
			if (flag)
			{
				num = v;
			}
			else
			{
				num = Mathf.Floor(v + 0.48f);
			}
			return num;
		}

		public void ParsingPhase(TextInfo textInfo, TextGenerationSettings generationSettings, out uint charCode, out float maxVisibleDescender)
		{
			TextSettings textSettings = generationSettings.textSettings;
			this.m_CurrentMaterial = generationSettings.fontAsset.material;
			this.m_CurrentMaterialIndex = 0;
			this.m_MaterialReferenceStack.SetDefault(new MaterialReference(this.m_CurrentMaterialIndex, this.m_CurrentFontAsset, null, this.m_CurrentMaterial, this.m_Padding));
			this.m_CurrentSpriteAsset = null;
			int totalCharacterCount = this.m_TotalCharacterCount;
			float num = this.m_FontSize / generationSettings.fontAsset.m_FaceInfo.pointSize * generationSettings.fontAsset.m_FaceInfo.scale;
			float num2 = num;
			float num3 = this.m_FontSize * 0.01f;
			this.m_FontScaleMultiplier = 1f;
			this.m_ShouldRenderBitmap = generationSettings.fontAsset.IsBitmap();
			this.m_CurrentFontSize = this.m_FontSize;
			this.m_SizeStack.SetDefault(this.m_CurrentFontSize);
			charCode = 0U;
			this.m_FontStyleInternal = generationSettings.fontStyle;
			this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
			this.m_FontWeightStack.SetDefault(this.m_FontWeightInternal);
			this.m_FontStyleStack.Clear();
			this.m_LineJustification = generationSettings.textAlignment;
			this.m_LineJustificationStack.SetDefault(this.m_LineJustification);
			float num4 = 0f;
			this.m_BaselineOffset = 0f;
			this.m_BaselineOffsetStack.Clear();
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
			float num5 = this.Round(this.m_CurrentFontAsset.faceInfo.lineHeight - (this.m_CurrentFontAsset.m_FaceInfo.ascentLine - this.m_CurrentFontAsset.m_FaceInfo.descentLine));
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
			bool flag = true;
			this.m_IsDrivenLineSpacing = false;
			this.m_FirstOverflowCharacterIndex = -1;
			this.m_LastBaseGlyphIndex = int.MinValue;
			bool flag2 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.kern);
			bool flag3 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.mark);
			bool flag4 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.mkmk);
			float num6 = ((this.m_MarginWidth > 0f) ? this.m_MarginWidth : 0f);
			float num7 = ((this.m_MarginHeight > 0f) ? this.m_MarginHeight : 0f);
			this.m_MarginLeft = 0f;
			this.m_MarginRight = 0f;
			this.m_Width = -1f;
			float num8 = num6 + 0.0001f - this.m_MarginLeft - this.m_MarginRight;
			this.m_MeshExtents.min = TextGeneratorUtilities.largePositiveVector2;
			this.m_MeshExtents.max = TextGeneratorUtilities.largeNegativeVector2;
			textInfo.ClearLineInfo();
			this.m_MaxCapHeight = 0f;
			this.m_MaxAscender = 0f;
			this.m_MaxDescender = 0f;
			this.m_PageAscender = 0f;
			maxVisibleDescender = 0f;
			bool flag5 = false;
			bool flag6 = true;
			this.m_IsNonBreakingSpace = false;
			bool flag7 = false;
			int num9 = 0;
			CharacterSubstitution characterSubstitution = new CharacterSubstitution(-1, 0U);
			bool flag8 = false;
			TextWrappingMode textWrappingMode = generationSettings.textWrappingMode;
			this.SaveWordWrappingState(ref this.m_SavedWordWrapState, -1, -1, textInfo);
			this.SaveWordWrappingState(ref this.m_SavedLineState, -1, -1, textInfo);
			this.SaveWordWrappingState(ref this.m_SavedEllipsisState, -1, -1, textInfo);
			this.SaveWordWrappingState(ref this.m_SavedLastValidState, -1, -1, textInfo);
			this.SaveWordWrappingState(ref this.m_SavedSoftLineBreakState, -1, -1, textInfo);
			this.m_EllipsisInsertionCandidateStack.Clear();
			this.m_IsTextTruncated = false;
			int num10 = 0;
			int num11 = 0;
			while (num11 < this.m_TextProcessingArray.Length && this.m_TextProcessingArray[num11].unicode > 0U)
			{
				charCode = this.m_TextProcessingArray[num11].unicode;
				bool flag9 = num10 > 5;
				if (flag9)
				{
					Debug.LogError("Line breaking recursion max threshold hit... Character [" + charCode.ToString() + "] index: " + num11.ToString());
					characterSubstitution.index = this.m_CharacterCount;
					characterSubstitution.unicode = 3U;
				}
				bool flag10 = charCode == 26U;
				if (!flag10)
				{
					bool flag11 = generationSettings.richText && charCode == 60U;
					if (flag11)
					{
						this.m_isTextLayoutPhase = true;
						this.m_TextElementType = TextElementType.Character;
						int num12;
						bool flag13;
						bool flag12 = this.ValidateHtmlTag(this.m_TextProcessingArray, num11 + 1, out num12, generationSettings, textInfo, out flag13);
						if (flag12)
						{
							num11 = num12;
							bool flag14 = this.m_TextElementType == TextElementType.Character;
							if (flag14)
							{
								goto IL_3B85;
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
					bool flag15 = false;
					bool flag16 = characterSubstitution.index == this.m_CharacterCount;
					if (flag16)
					{
						charCode = characterSubstitution.unicode;
						this.m_TextElementType = TextElementType.Character;
						flag15 = true;
						uint num13 = charCode;
						uint num14 = num13;
						if (num14 != 3U)
						{
							if (num14 != 45U)
							{
								if (num14 == 8230U)
								{
									textInfo.textElementInfo[this.m_CharacterCount].textElement = this.m_Ellipsis.character;
									textInfo.textElementInfo[this.m_CharacterCount].elementType = TextElementType.Character;
									textInfo.textElementInfo[this.m_CharacterCount].fontAsset = this.m_Ellipsis.fontAsset;
									textInfo.textElementInfo[this.m_CharacterCount].material = this.m_Ellipsis.material;
									textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex = this.m_Ellipsis.materialIndex;
									MaterialReference[] materialReferences = this.m_MaterialReferences;
									int materialIndex = this.m_Underline.materialIndex;
									materialReferences[materialIndex].referenceCount = materialReferences[materialIndex].referenceCount + 1;
									this.m_IsTextTruncated = true;
									characterSubstitution.index = this.m_CharacterCount + 1;
									characterSubstitution.unicode = 3U;
								}
							}
						}
						else
						{
							textInfo.textElementInfo[this.m_CharacterCount].textElement = this.m_CurrentFontAsset.characterLookupTable[3U];
							this.m_IsTextTruncated = true;
						}
					}
					bool flag17 = this.m_CharacterCount < 0 && charCode != 3U;
					if (flag17)
					{
						textInfo.textElementInfo[this.m_CharacterCount].isVisible = false;
						textInfo.textElementInfo[this.m_CharacterCount].character = 8203U;
						textInfo.textElementInfo[this.m_CharacterCount].lineNumber = 0;
						this.m_CharacterCount++;
					}
					else
					{
						float num15 = 1f;
						bool flag18 = this.m_TextElementType == TextElementType.Character;
						if (flag18)
						{
							bool flag19 = (this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase;
							if (flag19)
							{
								bool flag20 = char.IsLower((char)charCode);
								if (flag20)
								{
									charCode = (uint)char.ToUpper((char)charCode);
								}
							}
							else
							{
								bool flag21 = (this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase;
								if (flag21)
								{
									bool flag22 = char.IsUpper((char)charCode);
									if (flag22)
									{
										charCode = (uint)char.ToLower((char)charCode);
									}
								}
								else
								{
									bool flag23 = (this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps;
									if (flag23)
									{
										bool flag24 = char.IsLower((char)charCode);
										if (flag24)
										{
											num15 = 0.8f;
											charCode = (uint)char.ToUpper((char)charCode);
										}
									}
								}
							}
						}
						float num16 = 0f;
						float num17 = 0f;
						float num18 = 0f;
						bool flag25 = this.m_TextElementType == TextElementType.Sprite;
						if (flag25)
						{
							SpriteCharacter spriteCharacter = (SpriteCharacter)textInfo.textElementInfo[this.m_CharacterCount].textElement;
							this.m_CurrentSpriteAsset = spriteCharacter.textAsset as SpriteAsset;
							this.m_SpriteIndex = (int)spriteCharacter.glyphIndex;
							bool flag26 = charCode == 60U;
							if (flag26)
							{
								charCode = (uint)(57344 + this.m_SpriteIndex);
							}
							else
							{
								this.m_SpriteColor = Color.white;
							}
							float num19 = this.m_CurrentFontSize / this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale;
							bool flag27 = this.m_CurrentSpriteAsset.m_FaceInfo.pointSize > 0f;
							if (flag27)
							{
								float num20 = this.m_CurrentFontSize / this.m_CurrentSpriteAsset.m_FaceInfo.pointSize * this.m_CurrentSpriteAsset.m_FaceInfo.scale;
								num2 = spriteCharacter.m_Scale * spriteCharacter.m_Glyph.scale * num20;
								num17 = this.m_CurrentSpriteAsset.m_FaceInfo.ascentLine;
								num16 = this.m_CurrentSpriteAsset.m_FaceInfo.baseline * num19 * this.m_FontScaleMultiplier * this.m_CurrentSpriteAsset.m_FaceInfo.scale;
								num18 = this.m_CurrentSpriteAsset.m_FaceInfo.descentLine;
							}
							else
							{
								float num21 = this.m_CurrentFontSize / this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale;
								num2 = this.m_CurrentFontAsset.m_FaceInfo.ascentLine / spriteCharacter.m_Glyph.metrics.height * spriteCharacter.m_Scale * spriteCharacter.m_Glyph.scale * num21;
								float num22 = num21 / num2;
								num17 = this.m_CurrentFontAsset.m_FaceInfo.ascentLine * num22;
								num16 = this.m_CurrentFontAsset.m_FaceInfo.baseline * num19 * this.m_FontScaleMultiplier * this.m_CurrentFontAsset.m_FaceInfo.scale;
								num18 = this.m_CurrentFontAsset.m_FaceInfo.descentLine * num22;
							}
							this.m_CachedTextElement = spriteCharacter;
							textInfo.textElementInfo[this.m_CharacterCount].elementType = TextElementType.Sprite;
							textInfo.textElementInfo[this.m_CharacterCount].scale = num2;
							textInfo.textElementInfo[this.m_CharacterCount].spriteAsset = this.m_CurrentSpriteAsset;
							textInfo.textElementInfo[this.m_CharacterCount].fontAsset = this.m_CurrentFontAsset;
							textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex = this.m_CurrentMaterialIndex;
							this.m_CurrentMaterialIndex = currentMaterialIndex;
							num4 = 0f;
						}
						else
						{
							bool flag28 = this.m_TextElementType == TextElementType.Character;
							if (flag28)
							{
								this.m_CachedTextElement = textInfo.textElementInfo[this.m_CharacterCount].textElement;
								bool flag29 = this.m_CachedTextElement == null;
								if (flag29)
								{
									goto IL_3B85;
								}
								this.m_CurrentFontAsset = textInfo.textElementInfo[this.m_CharacterCount].fontAsset;
								this.m_CurrentMaterial = textInfo.textElementInfo[this.m_CharacterCount].material;
								this.m_CurrentMaterialIndex = textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex;
								bool flag30 = flag15 && this.m_TextProcessingArray[num11].unicode == 10U && this.m_CharacterCount != this.m_FirstCharacterOfLine;
								float num23;
								if (flag30)
								{
									num23 = textInfo.textElementInfo[this.m_CharacterCount - 1].pointSize * num15 / this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale;
								}
								else
								{
									num23 = this.m_CurrentFontSize * num15 / this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale;
								}
								bool flag31 = flag15 && charCode == 8230U;
								if (flag31)
								{
									num17 = 0f;
									num18 = 0f;
								}
								else
								{
									num17 = this.m_CurrentFontAsset.m_FaceInfo.ascentLine;
									num18 = this.m_CurrentFontAsset.m_FaceInfo.descentLine;
								}
								num2 = num23 * this.m_FontScaleMultiplier * this.m_CachedTextElement.m_Scale * this.m_CachedTextElement.m_Glyph.scale;
								num16 = this.Round(this.m_CurrentFontAsset.m_FaceInfo.baseline * num23 * this.m_FontScaleMultiplier * this.m_CurrentFontAsset.m_FaceInfo.scale);
								textInfo.textElementInfo[this.m_CharacterCount].elementType = TextElementType.Character;
								textInfo.textElementInfo[this.m_CharacterCount].scale = num2;
								num4 = this.m_Padding;
							}
						}
						float num24 = num2;
						bool flag32 = charCode == 173U || charCode == 3U;
						if (flag32)
						{
							num2 = 0f;
						}
						textInfo.textElementInfo[this.m_CharacterCount].character = charCode;
						textInfo.textElementInfo[this.m_CharacterCount].pointSize = this.m_CurrentFontSize;
						textInfo.textElementInfo[this.m_CharacterCount].color = this.m_HtmlColor;
						textInfo.textElementInfo[this.m_CharacterCount].underlineColor = this.m_UnderlineColor;
						textInfo.textElementInfo[this.m_CharacterCount].strikethroughColor = this.m_StrikethroughColor;
						textInfo.textElementInfo[this.m_CharacterCount].highlightState = this.m_HighlightState;
						textInfo.textElementInfo[this.m_CharacterCount].style = this.m_FontStyleInternal;
						bool flag33 = this.m_FontWeightInternal == TextFontWeight.Bold;
						if (flag33)
						{
							TextElementInfo[] textElementInfo = textInfo.textElementInfo;
							int characterCount = this.m_CharacterCount;
							textElementInfo[characterCount].style = textElementInfo[characterCount].style | FontStyles.Bold;
						}
						Glyph alternativeGlyph = textInfo.textElementInfo[this.m_CharacterCount].alternativeGlyph;
						GlyphMetrics glyphMetrics = ((alternativeGlyph == null) ? this.m_CachedTextElement.m_Glyph.metrics : alternativeGlyph.metrics);
						bool flag34 = charCode <= 65535U && char.IsWhiteSpace((char)charCode);
						GlyphValueRecord glyphValueRecord = default(GlyphValueRecord);
						float num25 = generationSettings.characterSpacing;
						bool flag35 = flag2 && this.m_TextElementType == TextElementType.Character;
						if (flag35)
						{
							uint glyphIndex = this.m_CachedTextElement.m_GlyphIndex;
							bool flag36 = this.m_CharacterCount < totalCharacterCount - 1 && textInfo.textElementInfo[this.m_CharacterCount + 1].elementType == TextElementType.Character;
							GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
							if (flag36)
							{
								uint glyphIndex2 = textInfo.textElementInfo[this.m_CharacterCount + 1].textElement.m_GlyphIndex;
								uint num26 = (glyphIndex2 << 16) | glyphIndex;
								bool flag37 = this.m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num26, out glyphPairAdjustmentRecord);
								if (flag37)
								{
									glyphValueRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphValueRecord;
									num25 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num25);
								}
							}
							bool flag38 = this.m_CharacterCount >= 1;
							if (flag38)
							{
								uint glyphIndex3 = textInfo.textElementInfo[this.m_CharacterCount - 1].textElement.m_GlyphIndex;
								uint num27 = (glyphIndex << 16) | glyphIndex3;
								bool flag39 = textInfo.textElementInfo[this.m_CharacterCount - 1].elementType == TextElementType.Character && this.m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num27, out glyphPairAdjustmentRecord);
								if (flag39)
								{
									glyphValueRecord += glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphValueRecord;
									num25 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num25);
								}
							}
							textInfo.textElementInfo[this.m_CharacterCount].adjustedHorizontalAdvance = glyphValueRecord.xAdvance;
						}
						bool flag40 = TextGeneratorUtilities.IsBaseGlyph(charCode);
						bool flag41 = flag40;
						if (flag41)
						{
							this.m_LastBaseGlyphIndex = this.m_CharacterCount;
						}
						bool flag42 = this.m_CharacterCount > 0 && !flag40;
						if (flag42)
						{
							bool flag43 = flag3 && this.m_LastBaseGlyphIndex != int.MinValue && this.m_LastBaseGlyphIndex == this.m_CharacterCount - 1;
							if (flag43)
							{
								Glyph glyph = textInfo.textElementInfo[this.m_LastBaseGlyphIndex].textElement.glyph;
								uint index = glyph.index;
								uint glyphIndex4 = this.m_CachedTextElement.glyphIndex;
								uint num28 = (glyphIndex4 << 16) | index;
								MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord;
								bool flag44 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num28, out markToBaseAdjustmentRecord);
								if (flag44)
								{
									float num29 = (textInfo.textElementInfo[this.m_LastBaseGlyphIndex].origin - this.m_XAdvance) / num2;
									glyphValueRecord.xPlacement = num29 + markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment;
									glyphValueRecord.yPlacement = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment;
									num25 = 0f;
								}
							}
							else
							{
								bool flag45 = false;
								bool flag46 = flag4;
								if (flag46)
								{
									int num30 = this.m_CharacterCount - 1;
									while (num30 >= 0 && num30 != this.m_LastBaseGlyphIndex)
									{
										Glyph glyph2 = textInfo.textElementInfo[num30].textElement.glyph;
										uint index2 = glyph2.index;
										uint glyphIndex5 = this.m_CachedTextElement.glyphIndex;
										uint num31 = (glyphIndex5 << 16) | index2;
										MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord;
										bool flag47 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryGetValue(num31, out markToMarkAdjustmentRecord);
										if (flag47)
										{
											float num32 = (textInfo.textElementInfo[num30].origin - this.m_XAdvance) / num2;
											float num33 = num16 - this.m_LineOffset + this.m_BaselineOffset;
											float num34 = (textInfo.textElementInfo[num30].baseLine - num33) / num2;
											glyphValueRecord.xPlacement = num32 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment;
											glyphValueRecord.yPlacement = num34 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment;
											num25 = 0f;
											flag45 = true;
											break;
										}
										num30--;
									}
								}
								bool flag48 = flag3 && this.m_LastBaseGlyphIndex != int.MinValue && !flag45;
								if (flag48)
								{
									Glyph glyph3 = textInfo.textElementInfo[this.m_LastBaseGlyphIndex].textElement.glyph;
									uint index3 = glyph3.index;
									uint glyphIndex6 = this.m_CachedTextElement.glyphIndex;
									uint num35 = (glyphIndex6 << 16) | index3;
									MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2;
									bool flag49 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num35, out markToBaseAdjustmentRecord2);
									if (flag49)
									{
										float num36 = (textInfo.textElementInfo[this.m_LastBaseGlyphIndex].origin - this.m_XAdvance) / num2;
										glyphValueRecord.xPlacement = num36 + markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.xPositionAdjustment;
										glyphValueRecord.yPlacement = markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.yPositionAdjustment;
										num25 = 0f;
									}
								}
							}
						}
						num17 += glyphValueRecord.yPlacement;
						num18 += glyphValueRecord.yPlacement;
						bool isRightToLeft = generationSettings.isRightToLeft;
						if (isRightToLeft)
						{
							this.m_XAdvance -= glyphMetrics.horizontalAdvance * (1f - this.m_CharWidthAdjDelta) * num2;
							bool flag50 = flag34 || charCode == 8203U;
							if (flag50)
							{
								this.m_XAdvance -= generationSettings.wordSpacing * num3;
							}
						}
						float num37 = 0f;
						bool flag51 = this.m_MonoSpacing != 0f && charCode != 8203U;
						if (flag51)
						{
							bool flag52 = this.m_DuoSpace && (charCode == 46U || charCode == 58U || charCode == 44U);
							if (flag52)
							{
								num37 = (this.m_MonoSpacing / 4f - (glyphMetrics.width / 2f + glyphMetrics.horizontalBearingX) * num2) * (1f - this.m_CharWidthAdjDelta);
							}
							else
							{
								num37 = (this.m_MonoSpacing / 2f - (glyphMetrics.width / 2f + glyphMetrics.horizontalBearingX) * num2) * (1f - this.m_CharWidthAdjDelta);
							}
							this.m_XAdvance += num37;
						}
						bool flag53 = this.m_CurrentFontAsset.atlasRenderMode != GlyphRenderMode.SMOOTH && this.m_CurrentFontAsset.atlasRenderMode != GlyphRenderMode.COLOR;
						bool flag54 = this.m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (textInfo.textElementInfo[this.m_CharacterCount].style & FontStyles.Bold) == FontStyles.Bold;
						float num39;
						float num40;
						if (flag54)
						{
							bool flag55 = flag53;
							if (flag55)
							{
								float num38 = ((generationSettings.isIMGUI && this.m_CurrentMaterial.HasFloat(TextShaderUtilities.ID_GradientScale)) ? this.m_CurrentMaterial.GetFloat(TextShaderUtilities.ID_GradientScale) : ((float)(this.m_CurrentFontAsset.atlasPadding + 1)));
								num39 = this.m_CurrentFontAsset.boldStyleWeight / 4f * num38;
								bool flag56 = num39 + num4 > num38;
								if (flag56)
								{
									num4 = num38 - num39;
								}
							}
							else
							{
								num39 = 0f;
							}
							num40 = this.m_CurrentFontAsset.boldStyleSpacing;
						}
						else
						{
							bool flag57 = flag53;
							if (flag57)
							{
								float num41 = ((generationSettings.isIMGUI && this.m_CurrentMaterial.HasFloat(TextShaderUtilities.ID_GradientScale)) ? this.m_CurrentMaterial.GetFloat(TextShaderUtilities.ID_GradientScale) : ((float)(this.m_CurrentFontAsset.atlasPadding + 1)));
								num39 = this.m_CurrentFontAsset.m_RegularStyleWeight / 4f * num41;
								bool flag58 = num39 + num4 > num41;
								if (flag58)
								{
									num4 = num41 - num39;
								}
							}
							else
							{
								num39 = 0f;
							}
							num40 = 0f;
						}
						Vector3 vector;
						vector.x = this.m_XAdvance + (glyphMetrics.horizontalBearingX * this.m_FXScale.x - num4 - num39 + glyphValueRecord.xPlacement) * num2 * (1f - this.m_CharWidthAdjDelta);
						vector.y = num16 + this.Round((glyphMetrics.horizontalBearingY + num4 + glyphValueRecord.yPlacement) * num2) - this.m_LineOffset + this.m_BaselineOffset;
						vector.z = 0f;
						Vector3 vector2;
						vector2.x = vector.x;
						vector2.y = vector.y - (glyphMetrics.height + num4 * 2f) * num2;
						vector2.z = 0f;
						Vector3 vector3;
						vector3.x = vector2.x + (glyphMetrics.width * this.m_FXScale.x + num4 * 2f + num39 * 2f) * num2 * (1f - this.m_CharWidthAdjDelta);
						vector3.y = vector.y;
						vector3.z = 0f;
						Vector3 vector4;
						vector4.x = vector3.x;
						vector4.y = vector2.y;
						vector4.z = 0f;
						bool flag59 = charCode == 8203U;
						if (flag59)
						{
							vector = Vector3.zero;
							vector2 = Vector3.zero;
							vector3 = Vector3.zero;
							vector4 = Vector3.zero;
						}
						bool flag60 = this.m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (this.m_FontStyleInternal & FontStyles.Italic) == FontStyles.Italic;
						if (flag60)
						{
							float num42 = (float)this.m_ItalicAngle * 0.01f;
							float num43 = (this.m_CurrentFontAsset.m_FaceInfo.capLine - (this.m_CurrentFontAsset.m_FaceInfo.baseline + this.m_BaselineOffset)) / 2f * this.m_FontScaleMultiplier * this.m_CurrentFontAsset.m_FaceInfo.scale;
							Vector3 vector5 = new Vector3(num42 * ((glyphMetrics.horizontalBearingY + num4 + num39 - num43) * num2), 0f, 0f);
							Vector3 vector6 = new Vector3(num42 * ((glyphMetrics.horizontalBearingY - glyphMetrics.height - num4 - num39 - num43) * num2), 0f, 0f);
							vector += vector5;
							vector2 += vector6;
							vector3 += vector5;
							vector4 += vector6;
						}
						bool flag61 = this.m_FXRotation != Quaternion.identity;
						if (flag61)
						{
							Matrix4x4 matrix4x = Matrix4x4.Rotate(this.m_FXRotation);
							Vector3 vector7 = (vector3 + vector2) / 2f;
							vector = matrix4x.MultiplyPoint3x4(vector - vector7) + vector7;
							vector2 = matrix4x.MultiplyPoint3x4(vector2 - vector7) + vector7;
							vector3 = matrix4x.MultiplyPoint3x4(vector3 - vector7) + vector7;
							vector4 = matrix4x.MultiplyPoint3x4(vector4 - vector7) + vector7;
						}
						textInfo.textElementInfo[this.m_CharacterCount].bottomLeft = vector2;
						textInfo.textElementInfo[this.m_CharacterCount].topLeft = vector;
						textInfo.textElementInfo[this.m_CharacterCount].topRight = vector3;
						textInfo.textElementInfo[this.m_CharacterCount].bottomRight = vector4;
						textInfo.textElementInfo[this.m_CharacterCount].origin = this.Round(this.m_XAdvance + glyphValueRecord.xPlacement * num2);
						textInfo.textElementInfo[this.m_CharacterCount].baseLine = this.Round(num16 - this.m_LineOffset + this.m_BaselineOffset + glyphValueRecord.yPlacement * num2);
						textInfo.textElementInfo[this.m_CharacterCount].aspectRatio = (vector3.x - vector2.x) / (vector.y - vector2.y);
						float num44 = ((this.m_TextElementType == TextElementType.Character) ? (num17 * num2 / num15 + this.m_BaselineOffset) : (num17 * num2 + this.m_BaselineOffset));
						float num45 = ((this.m_TextElementType == TextElementType.Character) ? (num18 * num2 / num15 + this.m_BaselineOffset) : (num18 * num2 + this.m_BaselineOffset));
						float num46 = num44;
						float num47 = num45;
						bool flag62 = this.m_CharacterCount == this.m_FirstCharacterOfLine;
						bool flag63 = flag62 || !flag34;
						if (flag63)
						{
							bool flag64 = this.m_BaselineOffset != 0f;
							if (flag64)
							{
								num46 = Mathf.Max((num44 - this.m_BaselineOffset) / this.m_FontScaleMultiplier, num46);
								num47 = Mathf.Min((num45 - this.m_BaselineOffset) / this.m_FontScaleMultiplier, num47);
							}
							this.m_MaxLineAscender = Mathf.Max(num46, this.m_MaxLineAscender);
							this.m_MaxLineDescender = Mathf.Min(num47, this.m_MaxLineDescender);
						}
						bool flag65 = flag62 || !flag34;
						if (flag65)
						{
							textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender = num46;
							textInfo.textElementInfo[this.m_CharacterCount].adjustedDescender = num47;
							textInfo.textElementInfo[this.m_CharacterCount].ascender = num44 - this.m_LineOffset;
							this.m_MaxDescender = (textInfo.textElementInfo[this.m_CharacterCount].descender = num45 - this.m_LineOffset);
						}
						else
						{
							textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender = this.m_MaxLineAscender;
							textInfo.textElementInfo[this.m_CharacterCount].adjustedDescender = this.m_MaxLineDescender;
							textInfo.textElementInfo[this.m_CharacterCount].ascender = this.m_MaxLineAscender - this.m_LineOffset;
							this.m_MaxDescender = (textInfo.textElementInfo[this.m_CharacterCount].descender = this.m_MaxLineDescender - this.m_LineOffset);
						}
						bool flag66 = this.m_LineNumber == 0;
						if (flag66)
						{
							bool flag67 = flag62 || !flag34;
							if (flag67)
							{
								this.m_MaxAscender = this.m_MaxLineAscender;
								this.m_MaxCapHeight = Mathf.Max(this.m_MaxCapHeight, this.m_CurrentFontAsset.m_FaceInfo.capLine * num2 / num15);
							}
						}
						bool flag68 = this.m_LineOffset == 0f;
						if (flag68)
						{
							bool flag69 = flag62 || !flag34;
							if (flag69)
							{
								this.m_PageAscender = ((this.m_PageAscender > num44) ? this.m_PageAscender : num44);
							}
						}
						textInfo.textElementInfo[this.m_CharacterCount].isVisible = false;
						bool flag70 = charCode == 9U || ((textWrappingMode == TextWrappingMode.PreserveWhitespace || textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) && (flag34 || charCode == 8203U)) || (!flag34 && charCode != 8203U && charCode != 173U && charCode != 3U) || (charCode == 173U && !flag8) || this.m_TextElementType == TextElementType.Sprite;
						if (flag70)
						{
							textInfo.textElementInfo[this.m_CharacterCount].isVisible = true;
							float num48 = this.m_MarginLeft;
							float num49 = this.m_MarginRight;
							bool flag71 = flag15;
							if (flag71)
							{
								num48 = textInfo.lineInfo[this.m_LineNumber].marginLeft;
								num49 = textInfo.lineInfo[this.m_LineNumber].marginRight;
							}
							num8 = ((this.m_Width != -1f) ? Mathf.Min(num6 + 0.0001f - num48 - num49, this.m_Width) : (num6 + 0.0001f - num48 - num49));
							float num50 = Mathf.Abs(this.m_XAdvance) + ((!generationSettings.isRightToLeft) ? glyphMetrics.horizontalAdvance : 0f) * (1f - this.m_CharWidthAdjDelta) * ((charCode == 173U) ? num24 : num2);
							float num51 = this.m_MaxAscender - (this.m_MaxLineDescender - this.m_LineOffset) + ((this.m_LineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_MaxLineAscender - this.m_StartOfLineAscender) : 0f);
							int characterCount2 = this.m_CharacterCount;
							bool flag72 = num51 > num7 + 0.0001f;
							if (flag72)
							{
								bool flag73 = this.m_FirstOverflowCharacterIndex == -1;
								if (flag73)
								{
									this.m_FirstOverflowCharacterIndex = this.m_CharacterCount;
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
											num11 = -1;
											this.m_CharacterCount = 0;
											characterSubstitution.index = 0;
											characterSubstitution.unicode = 3U;
											this.m_FirstCharacterOfLine = 0;
											goto IL_3B85;
										}
										WordWrapState wordWrapState = this.m_EllipsisInsertionCandidateStack.Pop();
										num11 = this.RestoreWordWrappingState(ref wordWrapState, textInfo);
										num11--;
										this.m_CharacterCount--;
										characterSubstitution.index = this.m_CharacterCount;
										characterSubstitution.unicode = 8230U;
										num10++;
										goto IL_3B85;
									}
									break;
								}
								case TextOverflowMode.Truncate:
									num11 = this.RestoreWordWrappingState(ref this.m_SavedLastValidState, textInfo);
									characterSubstitution.index = characterCount2;
									characterSubstitution.unicode = 3U;
									goto IL_3B85;
								case TextOverflowMode.Linked:
									num11 = this.RestoreWordWrappingState(ref this.m_SavedLastValidState, textInfo);
									characterSubstitution.index = characterCount2;
									characterSubstitution.unicode = 3U;
									goto IL_3B85;
								}
							}
							bool flag76 = flag40 && num50 > num8;
							if (flag76)
							{
								bool flag77 = textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap && this.m_CharacterCount != this.m_FirstCharacterOfLine;
								if (flag77)
								{
									num11 = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState, textInfo);
									bool flag78 = this.m_LineHeight == -32767f;
									float num52;
									if (flag78)
									{
										float adjustedAscender = textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender;
										num52 = ((this.m_LineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_MaxLineAscender - this.m_StartOfLineAscender) : 0f) - this.m_MaxLineDescender + adjustedAscender + (num5 + this.m_LineSpacingDelta) * num + 0f * num3;
									}
									else
									{
										num52 = this.m_LineHeight + 0f * num3;
										this.m_IsDrivenLineSpacing = true;
									}
									float num53 = this.m_MaxAscender + num52 + this.m_LineOffset - textInfo.textElementInfo[this.m_CharacterCount].adjustedDescender;
									bool flag79 = textInfo.textElementInfo[this.m_CharacterCount - 1].character == 173U && !flag8;
									if (flag79)
									{
										bool flag80 = generationSettings.overflowMode == TextOverflowMode.Overflow || num53 < num7 + 0.0001f;
										if (flag80)
										{
											characterSubstitution.index = this.m_CharacterCount - 1;
											characterSubstitution.unicode = 45U;
											num11--;
											this.m_CharacterCount--;
											goto IL_3B85;
										}
									}
									flag8 = false;
									bool flag81 = textInfo.textElementInfo[this.m_CharacterCount].character == 173U;
									if (flag81)
									{
										flag8 = true;
										goto IL_3B85;
									}
									int previousWordBreak = this.m_SavedSoftLineBreakState.previousWordBreak;
									bool flag82 = flag6 && previousWordBreak != -1;
									if (flag82)
									{
										bool flag83 = previousWordBreak != num9;
										if (flag83)
										{
											num11 = this.RestoreWordWrappingState(ref this.m_SavedSoftLineBreakState, textInfo);
											num9 = previousWordBreak;
											bool flag84 = textInfo.textElementInfo[this.m_CharacterCount - 1].character == 173U;
											if (flag84)
											{
												characterSubstitution.index = this.m_CharacterCount - 1;
												characterSubstitution.unicode = 45U;
												num11--;
												this.m_CharacterCount--;
												goto IL_3B85;
											}
										}
									}
									bool flag85 = num53 > num7 + 0.0001f;
									if (!flag85)
									{
										this.InsertNewLine(num11, num, num2, num3, num40, num25, num8, num5, ref flag5, ref maxVisibleDescender, generationSettings, textInfo);
										flag = true;
										flag6 = true;
										goto IL_3B85;
									}
									bool flag86 = this.m_FirstOverflowCharacterIndex == -1;
									if (flag86)
									{
										this.m_FirstOverflowCharacterIndex = this.m_CharacterCount;
									}
									switch (generationSettings.overflowMode)
									{
									case TextOverflowMode.Overflow:
									case TextOverflowMode.Masking:
									case TextOverflowMode.ScrollRect:
										this.InsertNewLine(num11, num, num2, num3, num40, num25, num8, num5, ref flag5, ref maxVisibleDescender, generationSettings, textInfo);
										flag = true;
										flag6 = true;
										goto IL_3B85;
									case TextOverflowMode.Ellipsis:
									{
										bool flag87 = this.m_EllipsisInsertionCandidateStack.Count == 0;
										if (flag87)
										{
											num11 = -1;
											this.m_CharacterCount = 0;
											characterSubstitution.index = 0;
											characterSubstitution.unicode = 3U;
											this.m_FirstCharacterOfLine = 0;
											goto IL_3B85;
										}
										WordWrapState wordWrapState2 = this.m_EllipsisInsertionCandidateStack.Pop();
										num11 = this.RestoreWordWrappingState(ref wordWrapState2, textInfo);
										num11--;
										this.m_CharacterCount--;
										characterSubstitution.index = this.m_CharacterCount;
										characterSubstitution.unicode = 8230U;
										num10++;
										goto IL_3B85;
									}
									case TextOverflowMode.Truncate:
										num11 = this.RestoreWordWrappingState(ref this.m_SavedLastValidState, textInfo);
										characterSubstitution.index = characterCount2;
										characterSubstitution.unicode = 3U;
										goto IL_3B85;
									case TextOverflowMode.Linked:
										characterSubstitution.index = this.m_CharacterCount;
										characterSubstitution.unicode = 3U;
										goto IL_3B85;
									}
								}
								else
								{
									switch (generationSettings.overflowMode)
									{
									case TextOverflowMode.Ellipsis:
									{
										bool flag88 = this.m_EllipsisInsertionCandidateStack.Count == 0;
										if (flag88)
										{
											num11 = -1;
											this.m_CharacterCount = 0;
											characterSubstitution.index = 0;
											characterSubstitution.unicode = 3U;
											this.m_FirstCharacterOfLine = 0;
											goto IL_3B85;
										}
										WordWrapState wordWrapState3 = this.m_EllipsisInsertionCandidateStack.Pop();
										num11 = this.RestoreWordWrappingState(ref wordWrapState3, textInfo);
										num11--;
										this.m_CharacterCount--;
										characterSubstitution.index = this.m_CharacterCount;
										characterSubstitution.unicode = 8230U;
										num10++;
										goto IL_3B85;
									}
									case TextOverflowMode.Truncate:
										num11 = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState, textInfo);
										characterSubstitution.index = characterCount2;
										characterSubstitution.unicode = 3U;
										goto IL_3B85;
									case TextOverflowMode.Linked:
										num11 = this.RestoreWordWrappingState(ref this.m_SavedWordWrapState, textInfo);
										characterSubstitution.index = this.m_CharacterCount;
										characterSubstitution.unicode = 3U;
										goto IL_3B85;
									}
								}
							}
							bool flag89 = flag34;
							if (flag89)
							{
								textInfo.textElementInfo[this.m_CharacterCount].isVisible = false;
								LineInfo[] lineInfo = textInfo.lineInfo;
								int lineNumber = this.m_LineNumber;
								this.m_LineVisibleSpaceCount = (lineInfo[lineNumber].spaceCount = lineInfo[lineNumber].spaceCount + 1);
								textInfo.lineInfo[this.m_LineNumber].marginLeft = num48;
								textInfo.lineInfo[this.m_LineNumber].marginRight = num49;
								textInfo.spaceCount++;
								bool flag90 = charCode == 160U;
								if (flag90)
								{
									LineInfo[] lineInfo2 = textInfo.lineInfo;
									int lineNumber2 = this.m_LineNumber;
									lineInfo2[lineNumber2].controlCharacterCount = lineInfo2[lineNumber2].controlCharacterCount + 1;
								}
							}
							else
							{
								bool flag91 = charCode == 173U;
								if (flag91)
								{
									textInfo.textElementInfo[this.m_CharacterCount].isVisible = false;
								}
								else
								{
									Color32 htmlColor = this.m_HtmlColor;
									bool flag92 = this.m_TextElementType == TextElementType.Character;
									if (flag92)
									{
										this.SaveGlyphVertexInfo(num4, num39, htmlColor, generationSettings, textInfo);
									}
									else
									{
										bool flag93 = this.m_TextElementType == TextElementType.Sprite;
										if (flag93)
										{
											this.SaveSpriteVertexInfo(htmlColor, generationSettings, textInfo);
										}
									}
									bool flag94 = flag;
									if (flag94)
									{
										flag = false;
										this.m_FirstVisibleCharacterOfLine = this.m_CharacterCount;
									}
									this.m_LineVisibleCharacterCount++;
									this.m_LastVisibleCharacterOfLine = this.m_CharacterCount;
									textInfo.lineInfo[this.m_LineNumber].marginLeft = num48;
									textInfo.lineInfo[this.m_LineNumber].marginRight = num49;
								}
							}
						}
						else
						{
							bool flag95 = generationSettings.overflowMode == TextOverflowMode.Linked && (charCode == 10U || charCode == 11U);
							if (flag95)
							{
								float num54 = this.m_MaxAscender - (this.m_MaxLineDescender - this.m_LineOffset) + ((this.m_LineOffset > 0f && !this.m_IsDrivenLineSpacing) ? (this.m_MaxLineAscender - this.m_StartOfLineAscender) : 0f);
								int characterCount3 = this.m_CharacterCount;
								bool flag96 = num54 > num7 + 0.0001f;
								if (flag96)
								{
									bool flag97 = this.m_FirstOverflowCharacterIndex == -1;
									if (flag97)
									{
										this.m_FirstOverflowCharacterIndex = this.m_CharacterCount;
									}
									num11 = this.RestoreWordWrappingState(ref this.m_SavedLastValidState, textInfo);
									characterSubstitution.index = characterCount3;
									characterSubstitution.unicode = 3U;
									goto IL_3B85;
								}
							}
							bool flag98 = (charCode == 10U || charCode == 11U || charCode == 160U || charCode == 8199U || charCode == 8232U || charCode == 8233U || char.IsSeparator((char)charCode)) && charCode != 173U && charCode != 8203U && charCode != 8288U;
							if (flag98)
							{
								LineInfo[] lineInfo3 = textInfo.lineInfo;
								int lineNumber3 = this.m_LineNumber;
								lineInfo3[lineNumber3].spaceCount = lineInfo3[lineNumber3].spaceCount + 1;
								textInfo.spaceCount++;
							}
							bool flag99 = charCode == 160U;
							if (flag99)
							{
								LineInfo[] lineInfo4 = textInfo.lineInfo;
								int lineNumber4 = this.m_LineNumber;
								lineInfo4[lineNumber4].controlCharacterCount = lineInfo4[lineNumber4].controlCharacterCount + 1;
							}
						}
						bool flag100 = generationSettings.overflowMode == TextOverflowMode.Ellipsis && (!flag15 || charCode == 45U);
						if (flag100)
						{
							float num55 = this.m_CurrentFontSize / this.m_Ellipsis.fontAsset.m_FaceInfo.pointSize * this.m_Ellipsis.fontAsset.m_FaceInfo.scale;
							float num56 = num55 * this.m_FontScaleMultiplier * this.m_Ellipsis.character.m_Scale * this.m_Ellipsis.character.m_Glyph.scale;
							float num57 = this.m_MarginLeft;
							float num58 = this.m_MarginRight;
							bool flag101 = charCode == 10U && this.m_CharacterCount != this.m_FirstCharacterOfLine;
							if (flag101)
							{
								num55 = textInfo.textElementInfo[this.m_CharacterCount - 1].pointSize / this.m_Ellipsis.fontAsset.m_FaceInfo.pointSize * this.m_Ellipsis.fontAsset.m_FaceInfo.scale;
								num56 = num55 * this.m_FontScaleMultiplier * this.m_Ellipsis.character.m_Scale * this.m_Ellipsis.character.m_Glyph.scale;
								num57 = textInfo.lineInfo[this.m_LineNumber].marginLeft;
								num58 = textInfo.lineInfo[this.m_LineNumber].marginRight;
							}
							float num59 = Mathf.Abs(this.m_XAdvance) + ((!generationSettings.isRightToLeft) ? this.m_Ellipsis.character.m_Glyph.metrics.horizontalAdvance : 0f) * (1f - this.m_CharWidthAdjDelta) * num56;
							float num60 = ((this.m_Width != -1f) ? Mathf.Min(num6 + 0.0001f - num57 - num58, this.m_Width) : (num6 + 0.0001f - num57 - num58));
							bool flag102 = num59 < num60;
							if (flag102)
							{
								this.SaveWordWrappingState(ref this.m_SavedEllipsisState, num11, this.m_CharacterCount, textInfo);
								this.m_EllipsisInsertionCandidateStack.Push(this.m_SavedEllipsisState);
							}
						}
						textInfo.textElementInfo[this.m_CharacterCount].lineNumber = this.m_LineNumber;
						bool flag103 = (charCode != 10U && charCode != 11U && charCode != 13U && !flag15) || textInfo.lineInfo[this.m_LineNumber].characterCount == 1;
						if (flag103)
						{
							textInfo.lineInfo[this.m_LineNumber].alignment = this.m_LineJustification;
						}
						bool flag104 = charCode != 8203U;
						if (flag104)
						{
							bool flag105 = charCode == 9U;
							if (flag105)
							{
								float num61 = this.m_CurrentFontAsset.m_FaceInfo.tabWidth * (float)this.m_CurrentFontAsset.tabMultiple * num2;
								float num62 = Mathf.Ceil(this.m_XAdvance / num61) * num61;
								this.m_XAdvance = ((num62 > this.m_XAdvance) ? num62 : (this.m_XAdvance + num61));
							}
							else
							{
								bool flag106 = this.m_MonoSpacing != 0f;
								if (flag106)
								{
									bool flag107 = this.m_DuoSpace && (charCode == 46U || charCode == 58U || charCode == 44U);
									float num63;
									if (flag107)
									{
										num63 = this.m_MonoSpacing / 2f - num37;
									}
									else
									{
										num63 = this.m_MonoSpacing - num37;
									}
									this.m_XAdvance += (num63 + (this.m_CurrentFontAsset.regularStyleSpacing + num25) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
									bool flag108 = flag34 || charCode == 8203U;
									if (flag108)
									{
										this.m_XAdvance += generationSettings.wordSpacing * num3;
									}
								}
								else
								{
									bool isRightToLeft2 = generationSettings.isRightToLeft;
									if (isRightToLeft2)
									{
										this.m_XAdvance -= (glyphValueRecord.xAdvance * num2 + (this.m_CurrentFontAsset.regularStyleSpacing + num25 + num40) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
										bool flag109 = flag34 || charCode == 8203U;
										if (flag109)
										{
											this.m_XAdvance -= generationSettings.wordSpacing * num3;
										}
									}
									else
									{
										this.m_XAdvance += ((glyphMetrics.horizontalAdvance * this.m_FXScale.x + glyphValueRecord.xAdvance) * num2 + (this.m_CurrentFontAsset.regularStyleSpacing + num25 + num40) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
										bool flag110 = flag34 || charCode == 8203U;
										if (flag110)
										{
											this.m_XAdvance += generationSettings.wordSpacing * num3;
										}
									}
								}
							}
						}
						textInfo.textElementInfo[this.m_CharacterCount].xAdvance = this.m_XAdvance;
						bool flag111 = charCode == 13U;
						if (flag111)
						{
							this.m_XAdvance = 0f + this.m_TagIndent;
						}
						bool flag112 = charCode == 10U || charCode == 11U || charCode == 3U || charCode == 8232U || charCode == 8232U || (charCode == 45U && flag15) || this.m_CharacterCount == totalCharacterCount - 1;
						if (flag112)
						{
							float num64 = this.m_MaxLineAscender - this.m_StartOfLineAscender;
							bool flag113 = this.m_LineOffset > 0f && Math.Abs(num64) > 0.01f && !this.m_IsDrivenLineSpacing;
							if (flag113)
							{
								TextGeneratorUtilities.AdjustLineOffset(this.m_FirstCharacterOfLine, this.m_CharacterCount, this.Round(num64), textInfo);
								this.m_MaxDescender -= num64;
								this.m_LineOffset += num64;
								bool flag114 = this.m_SavedEllipsisState.lineNumber == this.m_LineNumber;
								if (flag114)
								{
									this.m_SavedEllipsisState = this.m_EllipsisInsertionCandidateStack.Pop();
									this.m_SavedEllipsisState.startOfLineAscender = this.m_SavedEllipsisState.startOfLineAscender + num64;
									this.m_SavedEllipsisState.lineOffset = this.m_SavedEllipsisState.lineOffset + num64;
									this.m_EllipsisInsertionCandidateStack.Push(this.m_SavedEllipsisState);
								}
							}
							float num65 = this.m_MaxLineAscender - this.m_LineOffset;
							float num66 = this.m_MaxLineDescender - this.m_LineOffset;
							this.m_MaxDescender = ((this.m_MaxDescender < num66) ? this.m_MaxDescender : num66);
							bool flag115 = !flag5;
							if (flag115)
							{
								maxVisibleDescender = this.m_MaxDescender;
							}
							textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex = this.m_FirstCharacterOfLine;
							textInfo.lineInfo[this.m_LineNumber].firstVisibleCharacterIndex = (this.m_FirstVisibleCharacterOfLine = ((this.m_FirstCharacterOfLine > this.m_FirstVisibleCharacterOfLine) ? this.m_FirstCharacterOfLine : this.m_FirstVisibleCharacterOfLine));
							textInfo.lineInfo[this.m_LineNumber].lastCharacterIndex = (this.m_LastCharacterOfLine = this.m_CharacterCount);
							textInfo.lineInfo[this.m_LineNumber].lastVisibleCharacterIndex = (this.m_LastVisibleCharacterOfLine = ((this.m_LastVisibleCharacterOfLine < this.m_FirstVisibleCharacterOfLine) ? this.m_FirstVisibleCharacterOfLine : this.m_LastVisibleCharacterOfLine));
							int num67 = this.m_FirstVisibleCharacterOfLine;
							int num68 = this.m_LastVisibleCharacterOfLine;
							bool flag116 = generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespace || generationSettings.textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap;
							if (flag116)
							{
								bool flag117 = textInfo.textElementInfo[this.m_LastCharacterOfLine].xAdvance != 0f;
								if (flag117)
								{
									num67 = this.m_FirstCharacterOfLine;
									num68 = this.m_LastCharacterOfLine;
								}
							}
							textInfo.lineInfo[this.m_LineNumber].characterCount = textInfo.lineInfo[this.m_LineNumber].lastCharacterIndex - textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex + 1;
							textInfo.lineInfo[this.m_LineNumber].visibleCharacterCount = this.m_LineVisibleCharacterCount;
							textInfo.lineInfo[this.m_LineNumber].visibleSpaceCount = textInfo.lineInfo[this.m_LineNumber].lastVisibleCharacterIndex + 1 - textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex - this.m_LineVisibleCharacterCount;
							textInfo.lineInfo[this.m_LineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[num67].bottomLeft.x, num66);
							textInfo.lineInfo[this.m_LineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[num68].topRight.x, num65);
							textInfo.lineInfo[this.m_LineNumber].length = (generationSettings.isIMGUI ? textInfo.textElementInfo[num68].xAdvance : (textInfo.lineInfo[this.m_LineNumber].lineExtents.max.x - num4 * num2));
							textInfo.lineInfo[this.m_LineNumber].width = num8;
							bool flag118 = textInfo.lineInfo[this.m_LineNumber].characterCount == 1;
							if (flag118)
							{
								textInfo.lineInfo[this.m_LineNumber].alignment = this.m_LineJustification;
							}
							float num69 = ((this.m_CurrentFontAsset.regularStyleSpacing + num25 + num40) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
							bool isVisible = textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].isVisible;
							if (isVisible)
							{
								textInfo.lineInfo[this.m_LineNumber].maxAdvance = textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num69 : (-num69));
							}
							else
							{
								textInfo.lineInfo[this.m_LineNumber].maxAdvance = textInfo.textElementInfo[this.m_LastCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num69 : (-num69));
							}
							textInfo.lineInfo[this.m_LineNumber].baseline = 0f - this.m_LineOffset;
							textInfo.lineInfo[this.m_LineNumber].ascender = num65;
							textInfo.lineInfo[this.m_LineNumber].descender = num66;
							textInfo.lineInfo[this.m_LineNumber].lineHeight = num65 - num66 + num5 * num;
							bool flag119 = charCode == 10U || charCode == 11U || charCode == 45U || charCode == 8232U || charCode == 8233U;
							if (flag119)
							{
								this.SaveWordWrappingState(ref this.m_SavedLineState, num11, this.m_CharacterCount, textInfo);
								this.m_LineNumber++;
								flag = true;
								flag7 = false;
								flag6 = true;
								this.m_FirstCharacterOfLine = this.m_CharacterCount + 1;
								this.m_LineVisibleCharacterCount = 0;
								this.m_LineVisibleSpaceCount = 0;
								bool flag120 = this.m_LineNumber >= textInfo.lineInfo.Length;
								if (flag120)
								{
									TextGeneratorUtilities.ResizeLineExtents(this.m_LineNumber, textInfo);
								}
								float adjustedAscender2 = textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender;
								bool flag121 = this.m_LineHeight == -32767f;
								if (flag121)
								{
									float num70 = 0f - this.m_MaxLineDescender + adjustedAscender2 + (num5 + this.m_LineSpacingDelta) * num + (0f + ((charCode == 10U || charCode == 8233U) ? generationSettings.paragraphSpacing : 0f)) * num3;
									this.m_LineOffset += num70;
									this.m_IsDrivenLineSpacing = false;
								}
								else
								{
									this.m_LineOffset += this.m_LineHeight + (0f + ((charCode == 10U || charCode == 8233U) ? generationSettings.paragraphSpacing : 0f)) * num3;
									this.m_IsDrivenLineSpacing = true;
								}
								this.m_MaxLineAscender = -32767f;
								this.m_MaxLineDescender = 32767f;
								this.m_StartOfLineAscender = adjustedAscender2;
								this.m_XAdvance = 0f + this.m_TagLineIndent + this.m_TagIndent;
								this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num11, this.m_CharacterCount, textInfo);
								this.SaveWordWrappingState(ref this.m_SavedLastValidState, num11, this.m_CharacterCount, textInfo);
								this.m_CharacterCount++;
								goto IL_3B85;
							}
							bool flag122 = charCode == 3U;
							if (flag122)
							{
								num11 = this.m_TextProcessingArray.Length;
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
						bool flag123 = (textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap) || generationSettings.overflowMode == TextOverflowMode.Truncate || generationSettings.overflowMode == TextOverflowMode.Ellipsis || generationSettings.overflowMode == TextOverflowMode.Linked;
						if (flag123)
						{
							bool flag124 = false;
							bool flag125 = false;
							bool flag126 = (flag34 || charCode == 8203U || (charCode == 45U && (this.m_CharacterCount <= 0 || !char.IsWhiteSpace((char)textInfo.textElementInfo[this.m_CharacterCount - 1].character))) || charCode == 173U) && (!this.m_IsNonBreakingSpace || flag7) && charCode != 160U && charCode != 8199U && charCode != 8209U && charCode != 8239U && charCode != 8288U;
							if (flag126)
							{
								bool flag127 = charCode != 45U || this.m_CharacterCount <= 0 || !char.IsWhiteSpace((char)textInfo.textElementInfo[this.m_CharacterCount - 1].character);
								if (flag127)
								{
									flag6 = false;
									flag124 = true;
									this.m_SavedSoftLineBreakState.previousWordBreak = -1;
								}
							}
							else
							{
								bool flag128 = !this.m_IsNonBreakingSpace && ((TextGeneratorUtilities.IsHangul(charCode) && !textSettings.lineBreakingRules.useModernHangulLineBreakingRules) || TextGeneratorUtilities.IsCJK(charCode));
								if (flag128)
								{
									bool flag129 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(charCode);
									bool flag130 = this.m_CharacterCount < totalCharacterCount - 1 && textSettings.lineBreakingRules.followingCharactersLookup.Contains(textInfo.textElementInfo[this.m_CharacterCount + 1].character);
									bool flag131 = !flag129;
									if (flag131)
									{
										bool flag132 = !flag130;
										if (flag132)
										{
											flag6 = false;
											flag124 = true;
										}
										bool flag133 = flag6;
										if (flag133)
										{
											bool flag134 = flag34;
											if (flag134)
											{
												flag125 = true;
											}
											flag124 = true;
										}
									}
									else
									{
										bool flag135 = flag6 && flag62;
										if (flag135)
										{
											bool flag136 = flag34;
											if (flag136)
											{
												flag125 = true;
											}
											flag124 = true;
										}
									}
								}
								else
								{
									bool flag137 = !this.m_IsNonBreakingSpace && this.m_CharacterCount + 1 < totalCharacterCount && TextGeneratorUtilities.IsCJK(textInfo.textElementInfo[this.m_CharacterCount + 1].character);
									if (flag137)
									{
										uint character = textInfo.textElementInfo[this.m_CharacterCount + 1].character;
										bool flag138 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(charCode);
										bool flag139 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(character);
										bool flag140 = !flag138 && !flag139;
										if (flag140)
										{
											flag124 = true;
										}
									}
									else
									{
										bool flag141 = flag6;
										if (flag141)
										{
											bool flag142 = (flag34 && charCode != 160U) || (charCode == 173U && !flag8);
											if (flag142)
											{
												flag125 = true;
											}
											flag124 = true;
										}
									}
								}
							}
							bool flag143 = flag124;
							if (flag143)
							{
								this.SaveWordWrappingState(ref this.m_SavedWordWrapState, num11, this.m_CharacterCount, textInfo);
							}
							bool flag144 = flag125;
							if (flag144)
							{
								this.SaveWordWrappingState(ref this.m_SavedSoftLineBreakState, num11, this.m_CharacterCount, textInfo);
							}
						}
						this.SaveWordWrappingState(ref this.m_SavedLastValidState, num11, this.m_CharacterCount, textInfo);
						this.m_CharacterCount++;
					}
				}
				IL_3B85:
				num11++;
			}
			this.CloseAllLinkTags(textInfo);
		}

		private void InsertNewLine(int i, float baseScale, float currentElementScale, float currentEmScale, float boldSpacingAdjustment, float characterSpacingAdjustment, float width, float lineGap, ref bool isMaxVisibleDescenderSet, ref float maxVisibleDescender, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			float num = this.m_MaxLineAscender - this.m_StartOfLineAscender;
			bool flag = this.m_LineOffset > 0f && Math.Abs(num) > 0.01f && !this.m_IsDrivenLineSpacing;
			if (flag)
			{
				TextGeneratorUtilities.AdjustLineOffset(this.m_FirstCharacterOfLine, this.m_CharacterCount, this.Round(num), textInfo);
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
			textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex = this.m_FirstCharacterOfLine;
			textInfo.lineInfo[this.m_LineNumber].firstVisibleCharacterIndex = (this.m_FirstVisibleCharacterOfLine = ((this.m_FirstCharacterOfLine > this.m_FirstVisibleCharacterOfLine) ? this.m_FirstCharacterOfLine : this.m_FirstVisibleCharacterOfLine));
			textInfo.lineInfo[this.m_LineNumber].lastCharacterIndex = (this.m_LastCharacterOfLine = ((this.m_CharacterCount - 1 > 0) ? (this.m_CharacterCount - 1) : 0));
			textInfo.lineInfo[this.m_LineNumber].lastVisibleCharacterIndex = (this.m_LastVisibleCharacterOfLine = ((this.m_LastVisibleCharacterOfLine < this.m_FirstVisibleCharacterOfLine) ? this.m_FirstVisibleCharacterOfLine : this.m_LastVisibleCharacterOfLine));
			textInfo.lineInfo[this.m_LineNumber].characterCount = textInfo.lineInfo[this.m_LineNumber].lastCharacterIndex - textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex + 1;
			textInfo.lineInfo[this.m_LineNumber].visibleCharacterCount = this.m_LineVisibleCharacterCount;
			textInfo.lineInfo[this.m_LineNumber].visibleSpaceCount = textInfo.lineInfo[this.m_LineNumber].lastVisibleCharacterIndex + 1 - textInfo.lineInfo[this.m_LineNumber].firstCharacterIndex - this.m_LineVisibleCharacterCount;
			textInfo.lineInfo[this.m_LineNumber].lineExtents.min = new Vector2(textInfo.textElementInfo[this.m_FirstVisibleCharacterOfLine].bottomLeft.x, num3);
			textInfo.lineInfo[this.m_LineNumber].lineExtents.max = new Vector2(textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].topRight.x, num2);
			textInfo.lineInfo[this.m_LineNumber].length = (generationSettings.isIMGUI ? textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].xAdvance : textInfo.lineInfo[this.m_LineNumber].lineExtents.max.x);
			textInfo.lineInfo[this.m_LineNumber].width = width;
			float adjustedHorizontalAdvance = textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].adjustedHorizontalAdvance;
			float num4 = (adjustedHorizontalAdvance * currentElementScale + (this.m_CurrentFontAsset.regularStyleSpacing + characterSpacingAdjustment + boldSpacingAdjustment) * currentEmScale + this.m_CSpacing) * 1f;
			float num5 = (textInfo.lineInfo[this.m_LineNumber].maxAdvance = textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].xAdvance + (generationSettings.isRightToLeft ? num4 : (-num4)));
			textInfo.textElementInfo[this.m_LastVisibleCharacterOfLine].xAdvance = this.Round(num5);
			textInfo.lineInfo[this.m_LineNumber].baseline = 0f - this.m_LineOffset;
			textInfo.lineInfo[this.m_LineNumber].ascender = num2;
			textInfo.lineInfo[this.m_LineNumber].descender = num3;
			textInfo.lineInfo[this.m_LineNumber].lineHeight = num2 - num3 + lineGap * baseScale;
			this.m_FirstCharacterOfLine = this.m_CharacterCount;
			this.m_LineVisibleCharacterCount = 0;
			this.m_LineVisibleSpaceCount = 0;
			this.SaveWordWrappingState(ref this.m_SavedLineState, i, this.m_CharacterCount - 1, textInfo);
			this.m_LineNumber++;
			bool flag3 = this.m_LineNumber >= textInfo.lineInfo.Length;
			if (flag3)
			{
				TextGeneratorUtilities.ResizeLineExtents(this.m_LineNumber, textInfo);
			}
			bool flag4 = this.m_LineHeight == -32767f;
			if (flag4)
			{
				float adjustedAscender = textInfo.textElementInfo[this.m_CharacterCount].adjustedAscender;
				float num6 = 0f - this.m_MaxLineDescender + adjustedAscender + (lineGap + this.m_LineSpacingDelta) * baseScale + 0f * currentEmScale;
				this.m_LineOffset += num6;
				this.m_StartOfLineAscender = adjustedAscender;
			}
			else
			{
				this.m_LineOffset += this.m_LineHeight + 0f * currentEmScale;
			}
			this.m_MaxLineAscender = -32767f;
			this.m_MaxLineDescender = 32767f;
			this.m_XAdvance = 0f + this.m_TagIndent;
		}

		public Vector2 GetPreferredValues(TextGenerationSettings settings, TextInfo textInfo)
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
				bool flag2 = settings.fontSize <= 0;
				if (flag2)
				{
					vector = Vector2.zero;
				}
				else
				{
					this.Prepare(settings, textInfo);
					vector = this.GetPreferredValuesInternal(settings, textInfo);
				}
			}
			return vector;
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
				float fontSize = this.m_FontSize;
				this.m_MinFontSize = 0f;
				this.m_MaxFontSize = 0f;
				this.m_CharWidthAdjDelta = 0f;
				Vector2 vector2 = new Vector2((this.m_MarginWidth != 0f) ? this.m_MarginWidth : 32767f, (this.m_MarginHeight != 0f) ? this.m_MarginHeight : 32767f);
				this.m_AutoSizeIterationCount = 0;
				vector = this.CalculatePreferredValues(ref fontSize, vector2, false, generationSettings, textInfo);
			}
			return vector;
		}

		protected virtual Vector2 CalculatePreferredValues(ref float fontSize, Vector2 marginSize, bool isTextAutoSizingEnabled, TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			bool flag = generationSettings.fontAsset == null || generationSettings.fontAsset.characterLookupTable == null;
			Vector2 vector;
			if (flag)
			{
				Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned.");
				vector = Vector2.zero;
			}
			else
			{
				bool flag2 = this.m_TextProcessingArray == null || this.m_TextProcessingArray.Length == 0 || this.m_TextProcessingArray[0].unicode == 0U;
				if (flag2)
				{
					vector = Vector2.zero;
				}
				else
				{
					this.m_CurrentFontAsset = generationSettings.fontAsset;
					this.m_CurrentMaterial = generationSettings.fontAsset.material;
					this.m_CurrentMaterialIndex = 0;
					this.m_MaterialReferenceStack.SetDefault(new MaterialReference(0, this.m_CurrentFontAsset, null, this.m_CurrentMaterial, this.m_Padding));
					int totalCharacterCount = this.m_TotalCharacterCount;
					bool flag3 = this.m_InternalTextElementInfo == null || totalCharacterCount > this.m_InternalTextElementInfo.Length;
					if (flag3)
					{
						this.m_InternalTextElementInfo = new TextElementInfo[(totalCharacterCount > 1024) ? (totalCharacterCount + 256) : Mathf.NextPowerOfTwo(totalCharacterCount)];
					}
					float num = fontSize / generationSettings.fontAsset.faceInfo.pointSize * generationSettings.fontAsset.faceInfo.scale;
					float num2 = num;
					float num3 = fontSize * 0.01f;
					this.m_FontScaleMultiplier = 1f;
					this.m_ShouldRenderBitmap = generationSettings.fontAsset.IsBitmap();
					this.m_CurrentFontSize = fontSize;
					this.m_SizeStack.SetDefault(this.m_CurrentFontSize);
					this.m_FontStyleInternal = generationSettings.fontStyle;
					this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
					this.m_FontWeightStack.SetDefault(this.m_FontWeightInternal);
					this.m_FontStyleStack.Clear();
					this.m_LineJustification = generationSettings.textAlignment;
					this.m_LineJustificationStack.SetDefault(this.m_LineJustification);
					this.m_BaselineOffset = 0f;
					this.m_BaselineOffsetStack.Clear();
					this.m_FXScale = Vector3.one;
					this.m_LineOffset = 0f;
					this.m_LineHeight = -32767f;
					float num4 = this.Round(this.m_CurrentFontAsset.faceInfo.lineHeight - (this.m_CurrentFontAsset.faceInfo.ascentLine - this.m_CurrentFontAsset.faceInfo.descentLine));
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
					bool flag4 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.kern);
					bool flag5 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.mark);
					bool flag6 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.mkmk);
					TextSettings textSettings = generationSettings.textSettings;
					float x = marginSize.x;
					float y = marginSize.y;
					this.m_MarginLeft = 0f;
					this.m_MarginRight = 0f;
					this.m_Width = -1f;
					float num5 = x + 0.0001f - this.m_MarginLeft - this.m_MarginRight;
					TextWrappingMode textWrappingMode = generationSettings.textWrappingMode;
					float num6 = 0f;
					float num7 = 0f;
					this.m_IsCalculatingPreferredValues = true;
					this.m_MaxCapHeight = 0f;
					this.m_MaxAscender = 0f;
					this.m_MaxDescender = 0f;
					bool flag7 = false;
					bool flag8 = true;
					this.m_IsNonBreakingSpace = false;
					bool flag9 = false;
					CharacterSubstitution characterSubstitution = new CharacterSubstitution(-1, 0U);
					bool flag10 = false;
					WordWrapState wordWrapState = default(WordWrapState);
					WordWrapState wordWrapState2 = default(WordWrapState);
					WordWrapState wordWrapState3 = default(WordWrapState);
					this.m_IsTextTruncated = false;
					this.m_AutoSizeIterationCount++;
					int num8 = 0;
					while (num8 < this.m_TextProcessingArray.Length && this.m_TextProcessingArray[num8].unicode > 0U)
					{
						uint num9 = this.m_TextProcessingArray[num8].unicode;
						bool flag11 = num9 == 26U;
						if (!flag11)
						{
							bool flag12 = generationSettings.richText && num9 == 60U;
							if (flag12)
							{
								this.m_isTextLayoutPhase = true;
								this.m_TextElementType = TextElementType.Character;
								int num10;
								bool flag14;
								bool flag13 = this.ValidateHtmlTag(this.m_TextProcessingArray, num8 + 1, out num10, generationSettings, textInfo, out flag14);
								if (flag13)
								{
									num8 = num10;
									bool flag15 = this.m_TextElementType == TextElementType.Character;
									if (flag15)
									{
										goto IL_21DE;
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
							bool flag16 = false;
							bool flag17 = characterSubstitution.index == this.m_CharacterCount;
							if (flag17)
							{
								num9 = characterSubstitution.unicode;
								this.m_TextElementType = TextElementType.Character;
								flag16 = true;
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
											this.m_IsTextTruncated = true;
											characterSubstitution.index = this.m_CharacterCount + 1;
											characterSubstitution.unicode = 3U;
										}
									}
								}
								else
								{
									this.m_InternalTextElementInfo[this.m_CharacterCount].textElement = this.m_CurrentFontAsset.characterLookupTable[3U];
									this.m_IsTextTruncated = true;
								}
							}
							bool flag18 = this.m_CharacterCount < 0 && num9 != 3U;
							if (flag18)
							{
								this.m_InternalTextElementInfo[this.m_CharacterCount].isVisible = false;
								this.m_InternalTextElementInfo[this.m_CharacterCount].character = 8203U;
								this.m_InternalTextElementInfo[this.m_CharacterCount].lineNumber = 0;
								this.m_CharacterCount++;
							}
							else
							{
								float num13 = 1f;
								bool flag19 = this.m_TextElementType == TextElementType.Character;
								if (flag19)
								{
									bool flag20 = (this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase;
									if (flag20)
									{
										bool flag21 = char.IsLower((char)num9);
										if (flag21)
										{
											num9 = (uint)char.ToUpper((char)num9);
										}
									}
									else
									{
										bool flag22 = (this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase;
										if (flag22)
										{
											bool flag23 = char.IsUpper((char)num9);
											if (flag23)
											{
												num9 = (uint)char.ToLower((char)num9);
											}
										}
										else
										{
											bool flag24 = (this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps;
											if (flag24)
											{
												bool flag25 = char.IsLower((char)num9);
												if (flag25)
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
								bool flag26 = this.m_TextElementType == TextElementType.Sprite;
								if (flag26)
								{
									SpriteCharacter spriteCharacter = (SpriteCharacter)textInfo.textElementInfo[this.m_CharacterCount].textElement;
									this.m_CurrentSpriteAsset = spriteCharacter.textAsset as SpriteAsset;
									this.m_SpriteIndex = (int)spriteCharacter.glyphIndex;
									bool flag27 = spriteCharacter == null;
									if (flag27)
									{
										goto IL_21DE;
									}
									bool flag28 = num9 == 60U;
									if (flag28)
									{
										num9 = (uint)(57344 + this.m_SpriteIndex);
									}
									bool flag29 = this.m_CurrentSpriteAsset.faceInfo.pointSize > 0f;
									if (flag29)
									{
										float num17 = this.m_CurrentFontSize / this.m_CurrentSpriteAsset.faceInfo.pointSize * this.m_CurrentSpriteAsset.faceInfo.scale;
										num2 = spriteCharacter.scale * spriteCharacter.glyph.scale * num17;
										num15 = this.m_CurrentSpriteAsset.faceInfo.ascentLine;
										num16 = this.m_CurrentSpriteAsset.faceInfo.descentLine;
									}
									else
									{
										float num18 = this.m_CurrentFontSize / this.m_CurrentFontAsset.faceInfo.pointSize * this.m_CurrentFontAsset.faceInfo.scale;
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
									bool flag30 = this.m_TextElementType == TextElementType.Character;
									if (flag30)
									{
										this.m_CachedTextElement = textInfo.textElementInfo[this.m_CharacterCount].textElement;
										bool flag31 = this.m_CachedTextElement == null;
										if (flag31)
										{
											goto IL_21DE;
										}
										this.m_CurrentFontAsset = textInfo.textElementInfo[this.m_CharacterCount].fontAsset;
										this.m_CurrentMaterial = textInfo.textElementInfo[this.m_CharacterCount].material;
										this.m_CurrentMaterialIndex = textInfo.textElementInfo[this.m_CharacterCount].materialReferenceIndex;
										bool flag32 = flag16 && this.m_TextProcessingArray[num8].unicode == 10U && this.m_CharacterCount != this.m_FirstCharacterOfLine;
										float num20;
										if (flag32)
										{
											num20 = textInfo.textElementInfo[this.m_CharacterCount - 1].pointSize * num13 / this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale;
										}
										else
										{
											num20 = this.m_CurrentFontSize * num13 / this.m_CurrentFontAsset.m_FaceInfo.pointSize * this.m_CurrentFontAsset.m_FaceInfo.scale;
										}
										bool flag33 = flag16 && num9 == 8230U;
										if (flag33)
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
								bool flag34 = num9 == 173U || num9 == 3U;
								if (flag34)
								{
									num2 = 0f;
								}
								this.m_InternalTextElementInfo[this.m_CharacterCount].character = (uint)((ushort)num9);
								this.m_InternalTextElementInfo[this.m_CharacterCount].style = this.m_FontStyleInternal;
								bool flag35 = this.m_FontWeightInternal == TextFontWeight.Bold;
								if (flag35)
								{
									TextElementInfo[] internalTextElementInfo = this.m_InternalTextElementInfo;
									int characterCount = this.m_CharacterCount;
									internalTextElementInfo[characterCount].style = internalTextElementInfo[characterCount].style | FontStyles.Bold;
								}
								Glyph alternativeGlyph = textInfo.textElementInfo[this.m_CharacterCount].alternativeGlyph;
								GlyphMetrics glyphMetrics = ((alternativeGlyph == null) ? this.m_CachedTextElement.m_Glyph.metrics : alternativeGlyph.metrics);
								bool flag36 = num9 <= 65535U && char.IsWhiteSpace((char)num9);
								GlyphValueRecord glyphValueRecord = default(GlyphValueRecord);
								float num22 = generationSettings.characterSpacing;
								bool flag37 = flag4 && this.m_TextElementType == TextElementType.Character;
								if (flag37)
								{
									uint glyphIndex = this.m_CachedTextElement.m_GlyphIndex;
									bool flag38 = this.m_CharacterCount < totalCharacterCount - 1 && textInfo.textElementInfo[this.m_CharacterCount + 1].elementType == TextElementType.Character;
									GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
									if (flag38)
									{
										uint glyphIndex2 = textInfo.textElementInfo[this.m_CharacterCount + 1].textElement.m_GlyphIndex;
										uint num23 = (glyphIndex2 << 16) | glyphIndex;
										bool flag39 = this.m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num23, out glyphPairAdjustmentRecord);
										if (flag39)
										{
											glyphValueRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphValueRecord;
											num22 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num22);
										}
									}
									bool flag40 = this.m_CharacterCount >= 1;
									if (flag40)
									{
										uint glyphIndex3 = textInfo.textElementInfo[this.m_CharacterCount - 1].textElement.m_GlyphIndex;
										uint num24 = (glyphIndex << 16) | glyphIndex3;
										bool flag41 = textInfo.textElementInfo[this.m_CharacterCount - 1].elementType == TextElementType.Character && this.m_CurrentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num24, out glyphPairAdjustmentRecord);
										if (flag41)
										{
											glyphValueRecord += glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphValueRecord;
											num22 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num22);
										}
									}
									this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedHorizontalAdvance = glyphValueRecord.xAdvance;
								}
								bool flag42 = TextGeneratorUtilities.IsBaseGlyph(num9);
								bool flag43 = flag42;
								if (flag43)
								{
									this.m_LastBaseGlyphIndex = this.m_CharacterCount;
								}
								bool flag44 = this.m_CharacterCount > 0 && !flag42;
								if (flag44)
								{
									bool flag45 = this.m_LastBaseGlyphIndex != int.MinValue && this.m_LastBaseGlyphIndex == this.m_CharacterCount - 1;
									if (flag45)
									{
										Glyph glyph = textInfo.textElementInfo[this.m_LastBaseGlyphIndex].textElement.glyph;
										uint index = glyph.index;
										uint glyphIndex4 = this.m_CachedTextElement.glyphIndex;
										uint num25 = (glyphIndex4 << 16) | index;
										MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord;
										bool flag46 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num25, out markToBaseAdjustmentRecord);
										if (flag46)
										{
											float num26 = (this.m_InternalTextElementInfo[this.m_LastBaseGlyphIndex].origin - this.m_XAdvance) / num2;
											glyphValueRecord.xPlacement = num26 + markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment;
											glyphValueRecord.yPlacement = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment;
											num22 = 0f;
										}
									}
									else
									{
										bool flag47 = false;
										int num27 = this.m_CharacterCount - 1;
										while (num27 >= 0 && num27 != this.m_LastBaseGlyphIndex)
										{
											Glyph glyph2 = textInfo.textElementInfo[num27].textElement.glyph;
											uint index2 = glyph2.index;
											uint glyphIndex5 = this.m_CachedTextElement.glyphIndex;
											uint num28 = (glyphIndex5 << 16) | index2;
											MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord;
											bool flag48 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryGetValue(num28, out markToMarkAdjustmentRecord);
											if (flag48)
											{
												float num29 = (textInfo.textElementInfo[num27].origin - this.m_XAdvance) / num2;
												float num30 = num14 - this.m_LineOffset + this.m_BaselineOffset;
												float num31 = (this.m_InternalTextElementInfo[num27].baseLine - num30) / num2;
												glyphValueRecord.xPlacement = num29 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment;
												glyphValueRecord.yPlacement = num31 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment;
												num22 = 0f;
												flag47 = true;
												break;
											}
											num27--;
										}
										bool flag49 = this.m_LastBaseGlyphIndex != int.MinValue && !flag47;
										if (flag49)
										{
											Glyph glyph3 = textInfo.textElementInfo[this.m_LastBaseGlyphIndex].textElement.glyph;
											uint index3 = glyph3.index;
											uint glyphIndex6 = this.m_CachedTextElement.glyphIndex;
											uint num32 = (glyphIndex6 << 16) | index3;
											MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2;
											bool flag50 = this.m_CurrentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num32, out markToBaseAdjustmentRecord2);
											if (flag50)
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
								bool flag51 = this.m_MonoSpacing != 0f && num9 != 8203U;
								if (flag51)
								{
									num34 = (this.m_MonoSpacing / 2f - (this.m_CachedTextElement.glyph.metrics.width / 2f + this.m_CachedTextElement.glyph.metrics.horizontalBearingX) * num2) * (1f - this.m_CharWidthAdjDelta);
									this.m_XAdvance += num34;
								}
								float num35 = 0f;
								bool flag52 = this.m_TextElementType == TextElementType.Character && !isUsingAlternateTypeface && (this.m_InternalTextElementInfo[this.m_CharacterCount].style & FontStyles.Bold) == FontStyles.Bold;
								if (flag52)
								{
									num35 = this.m_CurrentFontAsset.boldStyleSpacing;
								}
								this.m_InternalTextElementInfo[this.m_CharacterCount].origin = this.Round(this.m_XAdvance + glyphValueRecord.xPlacement * num2);
								this.m_InternalTextElementInfo[this.m_CharacterCount].baseLine = this.Round(num14 - this.m_LineOffset + this.m_BaselineOffset + glyphValueRecord.yPlacement * num2);
								float num36 = ((this.m_TextElementType == TextElementType.Character) ? (num15 * num2 / num13 + this.m_BaselineOffset) : (num15 * num2 + this.m_BaselineOffset));
								float num37 = ((this.m_TextElementType == TextElementType.Character) ? (num16 * num2 / num13 + this.m_BaselineOffset) : (num16 * num2 + this.m_BaselineOffset));
								float num38 = num36;
								float num39 = num37;
								bool flag53 = this.m_CharacterCount == this.m_FirstCharacterOfLine;
								bool flag54 = flag53 || !flag36;
								if (flag54)
								{
									bool flag55 = this.m_BaselineOffset != 0f;
									if (flag55)
									{
										num38 = Mathf.Max((num36 - this.m_BaselineOffset) / this.m_FontScaleMultiplier, num38);
										num39 = Mathf.Min((num37 - this.m_BaselineOffset) / this.m_FontScaleMultiplier, num39);
									}
									this.m_MaxLineAscender = Mathf.Max(num38, this.m_MaxLineAscender);
									this.m_MaxLineDescender = Mathf.Min(num39, this.m_MaxLineDescender);
								}
								bool flag56 = flag53 || !flag36;
								if (flag56)
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
								bool flag57 = this.m_LineNumber == 0;
								if (flag57)
								{
									bool flag58 = flag53 || !flag36;
									if (flag58)
									{
										this.m_MaxAscender = this.m_MaxLineAscender;
										this.m_MaxCapHeight = Mathf.Max(this.m_MaxCapHeight, this.m_CurrentFontAsset.m_FaceInfo.capLine * num2 / num13);
									}
								}
								bool flag59 = this.m_LineOffset == 0f;
								if (flag59)
								{
									bool flag60 = !flag36 || this.m_CharacterCount == this.m_FirstCharacterOfLine;
									if (flag60)
									{
										this.m_PageAscender = ((this.m_PageAscender > num36) ? this.m_PageAscender : num36);
									}
								}
								bool flag61 = num9 == 9U || num9 == 8203U || ((textWrappingMode == TextWrappingMode.PreserveWhitespace || textWrappingMode == TextWrappingMode.PreserveWhitespaceNoWrap) && (flag36 || num9 == 8203U)) || (!flag36 && num9 != 8203U && num9 != 173U && num9 != 3U) || (num9 == 173U && !flag10) || this.m_TextElementType == TextElementType.Sprite;
								if (flag61)
								{
									num5 = ((this.m_Width != -1f) ? Mathf.Min(x + 0.0001f - this.m_MarginLeft - this.m_MarginRight, this.m_Width) : (x + 0.0001f - this.m_MarginLeft - this.m_MarginRight));
									float num40 = this.Round(Mathf.Abs(this.m_XAdvance) + glyphMetrics.horizontalAdvance * (1f - this.m_CharWidthAdjDelta) * ((num9 == 173U) ? num21 : num2));
									bool flag62 = flag42 && num40 > num5;
									if (flag62)
									{
										bool flag63 = textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap && this.m_CharacterCount != this.m_FirstCharacterOfLine;
										if (flag63)
										{
											num8 = this.RestoreWordWrappingState(ref wordWrapState, textInfo);
											bool flag64 = this.m_InternalTextElementInfo[this.m_CharacterCount - 1].character == 173U && !flag10 && generationSettings.overflowMode == TextOverflowMode.Overflow;
											if (flag64)
											{
												characterSubstitution.index = this.m_CharacterCount - 1;
												characterSubstitution.unicode = 45U;
												num8--;
												this.m_CharacterCount--;
												goto IL_21DE;
											}
											flag10 = false;
											bool flag65 = this.m_InternalTextElementInfo[this.m_CharacterCount].character == 173U;
											if (flag65)
											{
												flag10 = true;
												goto IL_21DE;
											}
											bool flag66 = isTextAutoSizingEnabled && flag8;
											if (flag66)
											{
												bool flag67 = this.m_CharWidthAdjDelta < 0f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag67)
												{
													float num41 = num40;
													bool flag68 = this.m_CharWidthAdjDelta > 0f;
													if (flag68)
													{
														num41 /= 1f - this.m_CharWidthAdjDelta;
													}
													float num42 = num40 - (num5 - 0.0001f);
													this.m_CharWidthAdjDelta += num42 / num41;
													this.m_CharWidthAdjDelta = Mathf.Min(this.m_CharWidthAdjDelta, 0f);
													return Vector2.zero;
												}
												bool flag69 = fontSize > 0f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
												if (flag69)
												{
													this.m_MaxFontSize = fontSize;
													float num43 = Mathf.Max((fontSize - this.m_MinFontSize) / 2f, 0.05f);
													fontSize -= num43;
													fontSize = Mathf.Max((float)((int)(fontSize * 20f + 0.5f)) / 20f, 0f);
												}
											}
											float num44 = this.m_MaxLineAscender - this.m_StartOfLineAscender;
											bool flag70 = this.m_LineOffset > 0f && Math.Abs(num44) > 0.01f && !this.m_IsDrivenLineSpacing;
											if (flag70)
											{
												this.m_MaxDescender -= num44;
												this.m_LineOffset += num44;
											}
											float num45 = this.m_MaxLineAscender - this.m_LineOffset;
											float num46 = this.m_MaxLineDescender - this.m_LineOffset;
											this.m_MaxDescender = ((this.m_MaxDescender < num46) ? this.m_MaxDescender : num46);
											bool flag71 = !flag7;
											if (flag71)
											{
												float maxDescender = this.m_MaxDescender;
											}
											this.m_FirstCharacterOfLine = this.m_CharacterCount;
											this.m_LineVisibleCharacterCount = 0;
											this.SaveWordWrappingState(ref wordWrapState2, num8, this.m_CharacterCount - 1, textInfo);
											this.m_LineNumber++;
											float adjustedAscender = this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedAscender;
											bool flag72 = this.m_LineHeight == -32767f;
											if (flag72)
											{
												this.m_LineOffset += 0f - this.m_MaxLineDescender + adjustedAscender + (num4 + this.m_LineSpacingDelta) * num + 0f * num3;
												this.m_IsDrivenLineSpacing = false;
											}
											else
											{
												this.m_LineOffset += this.m_LineHeight + 0f * num3;
												this.m_IsDrivenLineSpacing = true;
											}
											this.m_MaxLineAscender = -32767f;
											this.m_MaxLineDescender = 32767f;
											this.m_StartOfLineAscender = adjustedAscender;
											this.m_XAdvance = 0f + this.m_TagIndent;
											flag8 = true;
											goto IL_21DE;
										}
									}
									num6 = Mathf.Max(num6, num40 + this.m_MarginLeft + this.m_MarginRight);
									num7 = Mathf.Max(num7, this.m_MaxAscender - this.m_MaxDescender);
								}
								bool flag73 = this.m_LineOffset > 0f && !TextGeneratorUtilities.Approximately(this.m_MaxLineAscender, this.m_StartOfLineAscender) && !this.m_IsDrivenLineSpacing;
								if (flag73)
								{
									float num47 = this.m_MaxLineAscender - this.m_StartOfLineAscender;
									this.m_MaxDescender -= num47;
									this.m_LineOffset += num47;
									this.m_StartOfLineAscender += num47;
									wordWrapState.lineOffset = this.m_LineOffset;
									wordWrapState.startOfLineAscender = this.m_StartOfLineAscender;
								}
								bool flag74 = num9 != 8203U;
								if (flag74)
								{
									bool flag75 = num9 == 9U;
									if (flag75)
									{
										float num48 = this.m_CurrentFontAsset.faceInfo.tabWidth * (float)this.m_CurrentFontAsset.tabMultiple * num2;
										float num49 = Mathf.Ceil(this.m_XAdvance / num48) * num48;
										this.m_XAdvance = ((num49 > this.m_XAdvance) ? num49 : (this.m_XAdvance + num48));
									}
									else
									{
										bool flag76 = this.m_MonoSpacing != 0f;
										if (flag76)
										{
											bool flag77 = this.m_DuoSpace && (num9 == 46U || num9 == 58U || num9 == 44U);
											float num50;
											if (flag77)
											{
												num50 = this.m_MonoSpacing / 2f - num34;
											}
											else
											{
												num50 = this.m_MonoSpacing - num34;
											}
											this.m_XAdvance += (num50 + (this.m_CurrentFontAsset.regularStyleSpacing + num22) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
											bool flag78 = flag36 || num9 == 8203U;
											if (flag78)
											{
												this.m_XAdvance += generationSettings.wordSpacing * num3;
											}
										}
										else
										{
											this.m_XAdvance += ((glyphMetrics.horizontalAdvance * this.m_FXScale.x + glyphValueRecord.xAdvance) * num2 + (this.m_CurrentFontAsset.regularStyleSpacing + num22 + num35) * num3 + this.m_CSpacing) * (1f - this.m_CharWidthAdjDelta);
											bool flag79 = flag36 || num9 == 8203U;
											if (flag79)
											{
												this.m_XAdvance += generationSettings.wordSpacing * num3;
											}
										}
									}
								}
								bool flag80 = num9 == 13U;
								if (flag80)
								{
									this.m_XAdvance = 0f + this.m_TagIndent;
								}
								bool flag81 = num9 == 10U || num9 == 11U || num9 == 3U || num9 == 8232U || num9 == 8233U || this.m_CharacterCount == totalCharacterCount - 1;
								if (flag81)
								{
									float num51 = this.m_MaxLineAscender - this.m_StartOfLineAscender;
									bool flag82 = this.m_LineOffset > 0f && Math.Abs(num51) > 0.01f && !this.m_IsDrivenLineSpacing;
									if (flag82)
									{
										this.m_MaxDescender -= num51;
										this.m_LineOffset += num51;
									}
									float num52 = this.m_MaxLineDescender - this.m_LineOffset;
									this.m_MaxDescender = ((this.m_MaxDescender < num52) ? this.m_MaxDescender : num52);
									bool flag83 = num9 == 10U || num9 == 11U || num9 == 45U || num9 == 8232U || num9 == 8233U;
									if (flag83)
									{
										this.SaveWordWrappingState(ref wordWrapState2, num8, this.m_CharacterCount, textInfo);
										this.SaveWordWrappingState(ref wordWrapState, num8, this.m_CharacterCount, textInfo);
										this.m_LineNumber++;
										this.m_FirstCharacterOfLine = this.m_CharacterCount + 1;
										float adjustedAscender2 = this.m_InternalTextElementInfo[this.m_CharacterCount].adjustedAscender;
										bool flag84 = this.m_LineHeight == -32767f;
										if (flag84)
										{
											float num53 = 0f - this.m_MaxLineDescender + adjustedAscender2 + (num4 + this.m_LineSpacingDelta) * num + (0f + ((num9 == 10U || num9 == 8233U) ? generationSettings.paragraphSpacing : 0f)) * num3;
											this.m_LineOffset += num53;
											this.m_IsDrivenLineSpacing = false;
										}
										else
										{
											this.m_LineOffset += this.m_LineHeight + (0f + ((num9 == 10U || num9 == 8233U) ? generationSettings.paragraphSpacing : 0f)) * num3;
											this.m_IsDrivenLineSpacing = true;
										}
										this.m_MaxLineAscender = -32767f;
										this.m_MaxLineDescender = 32767f;
										this.m_StartOfLineAscender = adjustedAscender2;
										this.m_XAdvance = 0f + this.m_TagLineIndent + this.m_TagIndent;
										this.m_CharacterCount++;
										goto IL_21DE;
									}
									bool flag85 = num9 == 3U;
									if (flag85)
									{
										num8 = this.m_TextProcessingArray.Length;
									}
								}
								bool flag86 = (textWrappingMode != TextWrappingMode.NoWrap && textWrappingMode != TextWrappingMode.PreserveWhitespaceNoWrap) || generationSettings.overflowMode == TextOverflowMode.Truncate || generationSettings.overflowMode == TextOverflowMode.Ellipsis;
								if (flag86)
								{
									bool flag87 = false;
									bool flag88 = false;
									bool flag89 = (flag36 || num9 == 8203U || num9 == 45U || num9 == 173U) && (!this.m_IsNonBreakingSpace || flag9) && num9 != 160U && num9 != 8199U && num9 != 8209U && num9 != 8239U && num9 != 8288U;
									if (flag89)
									{
										bool flag90 = num9 != 45U || this.m_CharacterCount <= 0 || !char.IsWhiteSpace((char)textInfo.textElementInfo[this.m_CharacterCount - 1].character);
										if (flag90)
										{
											flag8 = false;
											flag87 = true;
											wordWrapState3.previousWordBreak = -1;
										}
									}
									else
									{
										bool flag91 = !this.m_IsNonBreakingSpace && ((TextGeneratorUtilities.IsHangul(num9) && !textSettings.lineBreakingRules.useModernHangulLineBreakingRules) || TextGeneratorUtilities.IsCJK(num9));
										if (flag91)
										{
											bool flag92 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(num9);
											bool flag93 = this.m_CharacterCount < totalCharacterCount - 1 && textSettings.lineBreakingRules.leadingCharactersLookup.Contains(this.m_InternalTextElementInfo[this.m_CharacterCount + 1].character);
											bool flag94 = !flag92;
											if (flag94)
											{
												bool flag95 = !flag93;
												if (flag95)
												{
													flag8 = false;
													flag87 = true;
												}
												bool flag96 = flag8;
												if (flag96)
												{
													bool flag97 = flag36;
													if (flag97)
													{
														flag88 = true;
													}
													flag87 = true;
												}
											}
											else
											{
												bool flag98 = flag8 && flag53;
												if (flag98)
												{
													bool flag99 = flag36;
													if (flag99)
													{
														flag88 = true;
													}
													flag87 = true;
												}
											}
										}
										else
										{
											bool flag100 = !this.m_IsNonBreakingSpace && this.m_CharacterCount + 1 < totalCharacterCount && TextGeneratorUtilities.IsCJK(textInfo.textElementInfo[this.m_CharacterCount + 1].character);
											if (flag100)
											{
												uint character = textInfo.textElementInfo[this.m_CharacterCount + 1].character;
												bool flag101 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(num9);
												bool flag102 = textSettings.lineBreakingRules.leadingCharactersLookup.Contains(character);
												bool flag103 = !flag101 && !flag102;
												if (flag103)
												{
													flag87 = true;
												}
											}
											else
											{
												bool flag104 = flag8;
												if (flag104)
												{
													bool flag105 = (flag36 && num9 != 160U) || (num9 == 173U && !flag10);
													if (flag105)
													{
														flag88 = true;
													}
													flag87 = true;
												}
											}
										}
									}
									bool flag106 = flag87;
									if (flag106)
									{
										this.SaveWordWrappingState(ref wordWrapState, num8, this.m_CharacterCount, textInfo);
									}
									bool flag107 = flag88;
									if (flag107)
									{
										this.SaveWordWrappingState(ref wordWrapState3, num8, this.m_CharacterCount, textInfo);
									}
								}
								this.m_CharacterCount++;
							}
						}
						IL_21DE:
						num8++;
					}
					float num54 = this.m_MaxFontSize - this.m_MinFontSize;
					bool flag108 = isTextAutoSizingEnabled && num54 > 0.051f && fontSize < 0f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount;
					if (flag108)
					{
						bool flag109 = this.m_CharWidthAdjDelta < 0f;
						if (flag109)
						{
							this.m_CharWidthAdjDelta = 0f;
						}
						this.m_MinFontSize = fontSize;
						float num55 = Mathf.Max((this.m_MaxFontSize - fontSize) / 2f, 0.05f);
						fontSize += num55;
						fontSize = Mathf.Min((float)((int)(fontSize * 20f + 0.5f)) / 20f, 0f);
						vector = Vector2.zero;
					}
					else
					{
						this.m_IsCalculatingPreferredValues = false;
						bool needToRound = this.NeedToRound;
						if (needToRound)
						{
							Debug.AssertFormat(num6 == Mathf.Round(num6), "renderedWidth was not rounded: {0}", new object[] { num6 });
						}
						else
						{
							bool flag110 = num6 != 0f;
							if (flag110)
							{
								num6 = (float)((int)(num6 * 100f + 1f)) / 100f;
							}
							bool flag111 = num7 != 0f;
							if (flag111)
							{
								num7 = (float)((int)(num7 * 100f + 1f)) / 100f;
							}
						}
						vector = new Vector2(num6, num7);
					}
				}
			}
			return vector;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal void Prepare(TextGenerationSettings generationSettings, TextInfo textInfo)
		{
			this.m_Padding = generationSettings.extraPadding;
			this.m_CurrentFontAsset = generationSettings.fontAsset;
			this.m_ShouldRenderBitmap = generationSettings.fontAsset.IsBitmap();
			this.m_FontStyleInternal = generationSettings.fontStyle;
			this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
			this.GetSpecialCharacters(generationSettings);
			this.ComputeMarginSize(generationSettings.screenRect, Vector4.zero);
			RenderedText renderedText = generationSettings.renderedText;
			this.PopulateTextBackingArray(in renderedText);
			this.PopulateTextProcessingArray(generationSettings);
			this.SetArraySizes(this.m_TextProcessingArray, generationSettings, textInfo);
			this.m_FontSize = (float)generationSettings.fontSize;
			this.m_MaxFontSize = 0f;
			this.m_MinFontSize = 0f;
			this.m_LineSpacingDelta = 0f;
			this.m_CharWidthAdjDelta = 0f;
		}

		internal bool PrepareFontAsset(TextGenerationSettings generationSettings)
		{
			this.m_CurrentFontAsset = generationSettings.fontAsset;
			this.m_FontStyleInternal = generationSettings.fontStyle;
			this.m_FontWeightInternal = (((this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold) ? TextFontWeight.Bold : generationSettings.fontWeight);
			bool flag = !this.GetSpecialCharacters(generationSettings);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				RenderedText renderedText = generationSettings.renderedText;
				this.PopulateTextBackingArray(in renderedText);
				this.PopulateTextProcessingArray(generationSettings);
				flag2 = this.PopulateFontAsset(generationSettings, this.m_TextProcessingArray);
			}
			return flag2;
		}

		private int SetArraySizes(TextProcessingElement[] textProcessingArray, TextGenerationSettings generationSettings, TextInfo textInfo)
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
			this.m_CurrentMaterial = generationSettings.fontAsset.material;
			this.m_CurrentMaterialIndex = 0;
			this.m_MaterialReferenceStack.SetDefault(new MaterialReference(this.m_CurrentMaterialIndex, this.m_CurrentFontAsset, null, this.m_CurrentMaterial, this.m_Padding));
			this.m_MaterialReferenceIndexLookup.Clear();
			MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
			this.m_CurrentSpriteAsset = null;
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
					bool flag5 = this.m_Ellipsis.fontAsset.GetHashCode() != this.m_CurrentFontAsset.GetHashCode();
					if (flag5)
					{
						bool flag6 = textSettings.matchMaterialPreset && this.m_CurrentMaterial.GetHashCode() != this.m_Ellipsis.fontAsset.material.GetHashCode();
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
			bool flag7 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.liga);
			int num2 = 0;
			while (num2 < textProcessingArray.Length && textProcessingArray[num2].unicode > 0U)
			{
				bool flag8 = textInfo.textElementInfo == null || this.m_TotalCharacterCount >= textInfo.textElementInfo.Length;
				if (flag8)
				{
					TextInfo.Resize<TextElementInfo>(ref textInfo.textElementInfo, this.m_TotalCharacterCount + 1, true);
				}
				uint num3 = textProcessingArray[num2].unicode;
				int num4 = this.m_CurrentMaterialIndex;
				bool flag9 = generationSettings.richText && num3 == 60U;
				if (!flag9)
				{
					goto IL_047A;
				}
				num4 = this.m_CurrentMaterialIndex;
				int num5;
				bool flag11;
				bool flag10 = this.ValidateHtmlTag(textProcessingArray, num2 + 1, out num5, generationSettings, textInfo, out flag11);
				if (!flag10)
				{
					goto IL_047A;
				}
				int stringIndex = textProcessingArray[num2].stringIndex;
				num2 = num5;
				bool flag12 = this.m_TextElementType == TextElementType.Sprite;
				if (flag12)
				{
					MaterialReference[] materialReferences = this.m_MaterialReferences;
					int currentMaterialIndex = this.m_CurrentMaterialIndex;
					materialReferences[currentMaterialIndex].referenceCount = materialReferences[currentMaterialIndex].referenceCount + 1;
					textInfo.textElementInfo[this.m_TotalCharacterCount].character = (uint)((ushort)(57344 + this.m_SpriteIndex));
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
				IL_0E7C:
				num2++;
				continue;
				IL_047A:
				bool flag13 = false;
				bool flag14 = false;
				FontAsset currentFontAsset = this.m_CurrentFontAsset;
				Material currentMaterial = this.m_CurrentMaterial;
				num4 = this.m_CurrentMaterialIndex;
				bool flag15 = this.m_TextElementType == TextElementType.Character;
				if (flag15)
				{
					bool flag16 = (this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase;
					if (flag16)
					{
						bool flag17 = char.IsLower((char)num3);
						if (flag17)
						{
							num3 = (uint)char.ToUpper((char)num3);
						}
					}
					else
					{
						bool flag18 = (this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase;
						if (flag18)
						{
							bool flag19 = char.IsUpper((char)num3);
							if (flag19)
							{
								num3 = (uint)char.ToLower((char)num3);
							}
						}
						else
						{
							bool flag20 = (this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps;
							if (flag20)
							{
								bool flag21 = char.IsLower((char)num3);
								if (flag21)
								{
									num3 = (uint)char.ToUpper((char)num3);
								}
							}
						}
					}
				}
				TextElement textElement = null;
				uint num6 = ((num2 + 1 < textProcessingArray.Length) ? textProcessingArray[num2 + 1].unicode : 0U);
				bool flag22 = generationSettings.emojiFallbackSupport && ((TextGeneratorUtilities.IsEmojiPresentationForm(num3) && num6 != 65038U) || (TextGeneratorUtilities.IsEmoji(num3) && num6 == 65039U));
				if (flag22)
				{
					bool flag23 = textSettings.emojiFallbackTextAssets != null && textSettings.emojiFallbackTextAssets.Count > 0;
					if (flag23)
					{
						textElement = FontAssetUtilities.GetTextElementFromTextAssets(num3, this.m_CurrentFontAsset, textSettings.emojiFallbackTextAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag13, flag7);
						bool flag24 = textElement != null;
						if (flag24)
						{
						}
					}
				}
				bool flag25 = textElement == null;
				if (flag25)
				{
					textElement = this.GetTextElement(generationSettings, num3, this.m_CurrentFontAsset, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag13, flag7);
				}
				bool flag26 = textElement == null;
				if (flag26)
				{
					this.DoMissingGlyphCallback(num3, textProcessingArray[num2].stringIndex, this.m_CurrentFontAsset, textInfo);
					uint num7 = num3;
					num3 = (textProcessingArray[num2].unicode = (uint)((textSettings.missingCharacterUnicode == 0) ? 9633 : textSettings.missingCharacterUnicode));
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag13, flag7);
					bool flag27 = textElement == null;
					if (flag27)
					{
						textElement = FontAssetUtilities.GetCharacterFromFontAssetsInternal(num3, this.m_CurrentFontAsset, textSettings.GetFallbackFontAssets(this.m_CurrentFontAsset.IsRaster(), this.m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag13, flag7);
					}
					bool flag28 = textElement == null;
					if (flag28)
					{
						bool flag29 = textSettings.defaultFontAsset != null;
						if (flag29)
						{
							textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, textSettings.defaultFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag13, flag7);
						}
					}
					bool flag30 = textElement == null;
					if (flag30)
					{
						num3 = (textProcessingArray[num2].unicode = 32U);
						textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag13, flag7);
					}
					bool flag31 = textElement == null;
					if (flag31)
					{
						num3 = (textProcessingArray[num2].unicode = 3U);
						textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag13, flag7);
					}
					bool displayWarnings2 = textSettings.displayWarnings;
					if (displayWarnings2)
					{
						bool flag32 = !JobsUtility.IsExecutingJob;
						string text = ((num7 > 65535U) ? string.Format("The character with Unicode value \\U{0:X8} was not found in the [{1}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{2:X4}.", num7, flag32 ? generationSettings.fontAsset.name : generationSettings.fontAsset.GetHashCode(), textElement.unicode) : string.Format("The character with Unicode value \\u{0:X4} was not found in the [{1}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{2:X4}.", num7, flag32 ? generationSettings.fontAsset.name : generationSettings.fontAsset.GetHashCode(), textElement.unicode));
						Debug.LogWarning(text);
					}
				}
				textInfo.textElementInfo[this.m_TotalCharacterCount].alternativeGlyph = null;
				bool flag33 = textElement.elementType == TextElementType.Character;
				if (flag33)
				{
					bool flag34 = textElement.textAsset.instanceID != this.m_CurrentFontAsset.instanceID;
					if (flag34)
					{
						flag14 = true;
						this.m_CurrentFontAsset = textElement.textAsset as FontAsset;
					}
					bool flag35 = (num6 >= 65024U && num6 <= 65039U) || (num6 >= 917760U && num6 <= 917999U);
					if (flag35)
					{
						uint glyphVariantIndex;
						bool flag36 = !this.m_CurrentFontAsset.TryGetGlyphVariantIndexInternal(num3, num6, out glyphVariantIndex);
						if (flag36)
						{
							glyphVariantIndex = this.m_CurrentFontAsset.GetGlyphVariantIndex(num3, num6);
							this.m_CurrentFontAsset.TryAddGlyphVariantIndexInternal(num3, num6, glyphVariantIndex);
						}
						bool flag37 = glyphVariantIndex > 0U;
						if (flag37)
						{
							Glyph glyph;
							bool flag38 = this.m_CurrentFontAsset.TryAddGlyphInternal(glyphVariantIndex, out glyph, true);
							if (flag38)
							{
								textInfo.textElementInfo[this.m_TotalCharacterCount].alternativeGlyph = glyph;
							}
						}
						textProcessingArray[num2 + 1].unicode = 26U;
						num2++;
					}
					List<LigatureSubstitutionRecord> list;
					bool flag39 = flag7 && this.m_CurrentFontAsset.fontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(textElement.glyphIndex, out list);
					if (flag39)
					{
						bool flag40 = list == null;
						if (flag40)
						{
							break;
						}
						for (int i = 0; i < list.Count; i++)
						{
							LigatureSubstitutionRecord ligatureSubstitutionRecord = list[i];
							int num8 = ligatureSubstitutionRecord.componentGlyphIDs.Length;
							uint num9 = ligatureSubstitutionRecord.ligatureGlyphID;
							for (int j = 1; j < num8; j++)
							{
								uint unicode = textProcessingArray[num2 + j].unicode;
								bool flag41;
								uint glyphIndex = this.m_CurrentFontAsset.GetGlyphIndex(unicode, out flag41);
								bool flag42 = glyphIndex == ligatureSubstitutionRecord.componentGlyphIDs[j];
								if (!flag42)
								{
									num9 = 0U;
									break;
								}
							}
							bool flag43 = num9 > 0U;
							if (flag43)
							{
								Glyph glyph2;
								bool flag44 = this.m_CurrentFontAsset.TryAddGlyphInternal(num9, out glyph2, true);
								if (flag44)
								{
									textInfo.textElementInfo[this.m_TotalCharacterCount].alternativeGlyph = glyph2;
									for (int k = 0; k < num8; k++)
									{
										bool flag45 = k == 0;
										if (flag45)
										{
											textProcessingArray[num2 + k].length = num8;
										}
										else
										{
											textProcessingArray[num2 + k].unicode = 26U;
										}
									}
									num2 += num8 - 1;
									break;
								}
							}
						}
					}
				}
				textInfo.textElementInfo[this.m_TotalCharacterCount].elementType = TextElementType.Character;
				textInfo.textElementInfo[this.m_TotalCharacterCount].textElement = textElement;
				textInfo.textElementInfo[this.m_TotalCharacterCount].isUsingAlternateTypeface = flag13;
				textInfo.textElementInfo[this.m_TotalCharacterCount].character = (uint)((ushort)num3);
				textInfo.textElementInfo[this.m_TotalCharacterCount].index = textProcessingArray[num2].stringIndex;
				textInfo.textElementInfo[this.m_TotalCharacterCount].stringLength = textProcessingArray[num2].length;
				textInfo.textElementInfo[this.m_TotalCharacterCount].fontAsset = this.m_CurrentFontAsset;
				bool flag46 = textElement.elementType == TextElementType.Sprite;
				if (flag46)
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
					goto IL_0E7C;
				}
				bool flag47 = flag14 && this.m_CurrentFontAsset.instanceID != generationSettings.fontAsset.instanceID;
				if (flag47)
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
				bool flag48 = textElement != null && textElement.glyph.atlasIndex > 0;
				if (flag48)
				{
					this.m_CurrentMaterial = MaterialManager.GetFallbackMaterial(this.m_CurrentFontAsset, this.m_CurrentMaterial, textElement.glyph.atlasIndex);
					this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
					flag14 = true;
				}
				bool flag49 = !char.IsWhiteSpace((char)num3) && num3 != 8203U;
				if (flag49)
				{
					bool flag50 = generationSettings.isIMGUI && this.m_MaterialReferences[this.m_CurrentMaterialIndex].referenceCount >= 16383;
					if (flag50)
					{
						this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(new Material(this.m_CurrentMaterial), this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
					}
					MaterialReference[] materialReferences3 = this.m_MaterialReferences;
					int currentMaterialIndex3 = this.m_CurrentMaterialIndex;
					materialReferences3[currentMaterialIndex3].referenceCount = materialReferences3[currentMaterialIndex3].referenceCount + 1;
				}
				textInfo.textElementInfo[this.m_TotalCharacterCount].material = this.m_CurrentMaterial;
				textInfo.textElementInfo[this.m_TotalCharacterCount].materialReferenceIndex = this.m_CurrentMaterialIndex;
				this.m_MaterialReferences[this.m_CurrentMaterialIndex].isFallbackMaterial = flag14;
				bool flag51 = flag14;
				if (flag51)
				{
					this.m_MaterialReferences[this.m_CurrentMaterialIndex].fallbackMaterial = currentMaterial;
					this.m_CurrentFontAsset = currentFontAsset;
					this.m_CurrentMaterial = currentMaterial;
					this.m_CurrentMaterialIndex = num4;
				}
				this.m_TotalCharacterCount++;
				goto IL_0E7C;
			}
			bool isCalculatingPreferredValues = this.m_IsCalculatingPreferredValues;
			int num10;
			if (isCalculatingPreferredValues)
			{
				this.m_IsCalculatingPreferredValues = false;
				num10 = this.m_TotalCharacterCount;
			}
			else
			{
				textInfo.spriteCount = num;
				int num11 = (textInfo.materialCount = this.m_MaterialReferenceIndexLookup.Count);
				bool flag52 = num11 > textInfo.meshInfo.Length;
				if (flag52)
				{
					TextInfo.Resize<MeshInfo>(ref textInfo.meshInfo, num11, false);
				}
				bool flag53 = this.m_VertexBufferAutoSizeReduction && textInfo.textElementInfo.Length - this.m_TotalCharacterCount > 256;
				if (flag53)
				{
					TextInfo.Resize<TextElementInfo>(ref textInfo.textElementInfo, Mathf.Max(this.m_TotalCharacterCount + 1, 256), true);
				}
				for (int l = 0; l < num11; l++)
				{
					int referenceCount = this.m_MaterialReferences[l].referenceCount;
					bool flag54 = textInfo.meshInfo[l].vertexData == null || textInfo.meshInfo[l].vertexBufferSize < referenceCount * 4;
					if (flag54)
					{
						bool flag55 = textInfo.meshInfo[l].vertexData == null;
						if (flag55)
						{
							textInfo.meshInfo[l] = new MeshInfo(referenceCount + 1, generationSettings.isIMGUI);
						}
						else
						{
							textInfo.meshInfo[l].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.NextPowerOfTwo(referenceCount), generationSettings.isIMGUI);
						}
					}
					else
					{
						bool flag56 = textInfo.meshInfo[l].vertexBufferSize - referenceCount * 4 > 1024;
						if (flag56)
						{
							textInfo.meshInfo[l].ResizeMeshInfo((referenceCount > 1024) ? (referenceCount + 256) : Mathf.Max(Mathf.NextPowerOfTwo(referenceCount), 256), generationSettings.isIMGUI);
						}
					}
					textInfo.meshInfo[l].material = this.m_MaterialReferences[l].material;
					textInfo.meshInfo[l].glyphRenderMode = this.m_MaterialReferences[l].fontAsset.atlasRenderMode;
				}
				num10 = this.m_TotalCharacterCount;
			}
			return num10;
		}

		private TextElement GetTextElement(TextGenerationSettings generationSettings, uint unicode, FontAsset fontAsset, FontStyles fontStyle, TextFontWeight fontWeight, out bool isUsingAlternativeTypeface, bool populateLigatures)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			TextSettings textSettings = generationSettings.textSettings;
			Character character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, fontAsset, false, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
			bool flag2 = character != null;
			TextElement textElement;
			if (flag2)
			{
				textElement = character;
			}
			else
			{
				bool flag3 = !flag && (fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic || fontAsset.atlasPopulationMode == AtlasPopulationMode.DynamicOS);
				if (flag3)
				{
					textElement = null;
				}
				else
				{
					bool flag4 = fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0;
					if (flag4)
					{
						character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(unicode, fontAsset, fontAsset.m_FallbackFontAssetTable, null, true, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
					}
					bool flag5 = character != null;
					if (flag5)
					{
						bool flag6 = isUsingAlternativeTypeface;
						if (flag6)
						{
							fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
						}
						else
						{
							fontAsset.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
						}
						textElement = character;
					}
					else
					{
						bool flag7 = (flag ? (fontAsset.instanceID == generationSettings.fontAsset.instanceID) : (fontAsset == generationSettings.fontAsset));
						bool flag8 = !flag7;
						if (flag8)
						{
							character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, generationSettings.fontAsset, false, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
							bool flag9 = character != null;
							if (flag9)
							{
								this.m_CurrentMaterialIndex = 0;
								this.m_CurrentMaterial = this.m_MaterialReferences[0].material;
								fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
								return character;
							}
							bool flag10 = generationSettings.fontAsset.m_FallbackFontAssetTable != null && generationSettings.fontAsset.m_FallbackFontAssetTable.Count > 0;
							if (flag10)
							{
								character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(unicode, fontAsset, generationSettings.fontAsset.m_FallbackFontAssetTable, null, true, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
							}
							bool flag11 = character != null;
							if (flag11)
							{
								bool flag12 = isUsingAlternativeTypeface;
								if (flag12)
								{
									fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
								}
								else
								{
									fontAsset.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
								}
								return character;
							}
						}
						bool flag13 = textSettings.GetStaticFallbackOSFontAsset() == null && !flag;
						if (flag13)
						{
							textElement = null;
						}
						else
						{
							character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(unicode, fontAsset, textSettings.GetFallbackFontAssets(fontAsset.IsRaster(), this.m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, true, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
							bool flag14 = character != null;
							if (flag14)
							{
								bool flag15 = isUsingAlternativeTypeface;
								if (flag15)
								{
									fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
								}
								else
								{
									fontAsset.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
								}
								textElement = character;
							}
							else
							{
								bool flag16 = textSettings.defaultFontAsset != null;
								if (flag16)
								{
									character = FontAssetUtilities.GetCharacterFromFontAsset(unicode, textSettings.defaultFontAsset, true, fontStyle, fontWeight, out isUsingAlternativeTypeface, populateLigatures);
								}
								bool flag17 = character != null;
								if (flag17)
								{
									bool flag18 = isUsingAlternativeTypeface;
									if (flag18)
									{
										fontAsset.AddCharacterToLookupCache(unicode, character, fontStyle, fontWeight);
									}
									else
									{
										fontAsset.AddCharacterToLookupCache(unicode, character, FontStyles.Normal, TextFontWeight.Regular);
									}
									textElement = character;
								}
								else
								{
									bool flag19 = textSettings.defaultSpriteAsset != null;
									if (flag19)
									{
										bool flag20 = !flag && textSettings.defaultSpriteAsset.m_SpriteCharacterLookup == null;
										if (flag20)
										{
											return null;
										}
										SpriteCharacter spriteCharacterFromSpriteAsset = FontAssetUtilities.GetSpriteCharacterFromSpriteAsset(unicode, textSettings.defaultSpriteAsset, true);
										bool flag21 = spriteCharacterFromSpriteAsset != null;
										if (flag21)
										{
											return spriteCharacterFromSpriteAsset;
										}
									}
									textElement = null;
								}
							}
						}
					}
				}
			}
			return textElement;
		}

		private void PopulateTextBackingArray(in RenderedText sourceText)
		{
			int num = 0;
			int characterCount = sourceText.CharacterCount;
			bool flag = characterCount >= this.m_TextBackingArray.Capacity;
			if (flag)
			{
				this.m_TextBackingArray.Resize(characterCount);
			}
			foreach (char c in sourceText)
			{
				this.m_TextBackingArray[num] = (uint)c;
				num++;
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
			bool flag3 = false;
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
									goto IL_0A95;
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
									goto IL_0A95;
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
									goto IL_0A95;
								}
								break;
							}
							case 117U:
							{
								bool flag9 = !generationSettings.parseControlCharacters;
								if (!flag9)
								{
									bool flag10 = count > i + 5 && TextGeneratorUtilities.IsValidUTF16(this.m_TextBackingArray, i + 2);
									if (flag10)
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
										goto IL_0A95;
									}
								}
								break;
							}
							case 118U:
							{
								bool flag11 = !generationSettings.parseControlCharacters;
								if (!flag11)
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
									goto IL_0A95;
								}
								break;
							}
							}
						}
						else
						{
							bool flag12 = !generationSettings.parseControlCharacters;
							if (!flag12)
							{
								i++;
							}
						}
					}
					else
					{
						bool flag13 = !generationSettings.parseControlCharacters;
						if (!flag13)
						{
							bool flag14 = count > i + 9 && TextGeneratorUtilities.IsValidUTF32(this.m_TextBackingArray, i + 2);
							if (flag14)
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
								goto IL_0A95;
							}
						}
					}
					goto IL_03D7;
				}
				goto IL_03D7;
				IL_0A95:
				i++;
				continue;
				IL_03D7:
				bool flag15 = num3 >= 55296U && num3 <= 56319U && count > i + 1 && this.m_TextBackingArray[i + 1] >= 56320U && this.m_TextBackingArray[i + 1] <= 57343U;
				if (flag15)
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
					goto IL_0A95;
				}
				bool flag16 = num3 == 13U && i + 1 < count && this.m_TextBackingArray[i + 1] == 10U;
				if (flag16)
				{
					this.m_TextProcessingArray[num] = new TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 2,
						unicode = 10U
					};
					i++;
					num++;
					goto IL_0A95;
				}
				bool flag17 = num3 == 60U && generationSettings.richText;
				if (flag17)
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
										bool flag18 = this.m_TextBackingArray.Count > i + 4 && this.m_TextBackingArray[i + 3] == 104U && this.m_TextBackingArray[i + 4] == 114U;
										if (flag18)
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
									bool flag19 = flag3;
									if (!flag19)
									{
										bool flag20 = num == this.m_TextProcessingArray.Length;
										if (flag20)
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
										goto IL_0A95;
									}
								}
							}
							else
							{
								bool flag21 = flag3;
								if (!flag21)
								{
									bool flag22 = num == this.m_TextProcessingArray.Length;
									if (flag22)
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
									goto IL_0A95;
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
									bool flag23 = flag3;
									if (!flag23)
									{
										bool flag24 = num == this.m_TextProcessingArray.Length;
										if (flag24)
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
										goto IL_0A95;
									}
								}
							}
							else
							{
								bool flag25 = flag3;
								if (!flag25)
								{
									bool flag26 = num == this.m_TextProcessingArray.Length;
									if (flag26)
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
									goto IL_0A95;
								}
							}
						}
						else
						{
							bool flag27 = flag3;
							if (!flag27)
							{
								bool flag28 = num == this.m_TextProcessingArray.Length;
								if (flag28)
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
								goto IL_0A95;
							}
						}
					}
					else if (markupTag2 != MarkupTag.ZWSP)
					{
						if (markupTag2 != MarkupTag.STYLE)
						{
							if (markupTag2 == MarkupTag.SLASH_STYLE)
							{
								bool flag29 = flag3;
								if (!flag29)
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
									goto IL_0A95;
								}
							}
						}
						else
						{
							bool flag30 = flag3;
							if (!flag30)
							{
								int k = num;
								int num6;
								bool flag31 = TextGeneratorUtilities.ReplaceOpeningStyleTag(ref this.m_TextBackingArray, i, out num6, ref this.m_TextProcessingArray, ref num, ref this.m_TextStyleStackDepth, ref this.m_TextStyleStacks, ref generationSettings);
								if (flag31)
								{
									while (k < num)
									{
										this.m_TextProcessingArray[k].stringIndex = i;
										this.m_TextProcessingArray[k].length = num6 - i + 1;
										k++;
									}
									i = num6;
									goto IL_0A95;
								}
							}
						}
					}
					else
					{
						bool flag32 = flag3;
						if (!flag32)
						{
							bool flag33 = num == this.m_TextProcessingArray.Length;
							if (flag33)
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
							goto IL_0A95;
						}
					}
				}
				bool flag34 = num == this.m_TextProcessingArray.Length;
				if (flag34)
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
				goto IL_0A95;
			}
			this.m_TextStyleStackDepth = 0;
			bool flag35 = style != null && style.hashCode != -1183493901;
			if (flag35)
			{
				TextGeneratorUtilities.InsertClosingStyleTag(ref this.m_TextProcessingArray, ref num, ref this.m_TextStyleStackDepth, ref this.m_TextStyleStacks, ref generationSettings);
			}
			bool flag36 = num == this.m_TextProcessingArray.Length;
			if (flag36)
			{
				TextGeneratorUtilities.ResizeInternalArray<TextProcessingElement>(ref this.m_TextProcessingArray);
			}
			this.m_TextProcessingArray[num].unicode = 0U;
			this.m_InternalTextProcessingArraySize = num;
		}

		private bool PopulateFontAsset(TextGenerationSettings generationSettings, TextProcessingElement[] textProcessingArray)
		{
			bool flag = !TextGenerator.IsExecutingJob;
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
			this.m_CurrentMaterial = generationSettings.fontAsset.material;
			this.m_CurrentMaterialIndex = 0;
			this.m_MaterialReferenceStack.SetDefault(new MaterialReference(this.m_CurrentMaterialIndex, this.m_CurrentFontAsset, null, this.m_CurrentMaterial, this.m_Padding));
			this.m_MaterialReferenceIndexLookup.Clear();
			MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
			this.m_TextElementType = TextElementType.Character;
			bool flag2 = generationSettings.overflowMode == TextOverflowMode.Ellipsis;
			if (flag2)
			{
				this.GetEllipsisSpecialCharacter(generationSettings);
				bool flag3 = this.m_Ellipsis.character != null;
				if (flag3)
				{
					bool flag4 = this.m_Ellipsis.fontAsset.GetHashCode() != this.m_CurrentFontAsset.GetHashCode();
					if (flag4)
					{
						bool flag5 = textSettings.matchMaterialPreset && this.m_CurrentMaterial.GetHashCode() != this.m_Ellipsis.fontAsset.material.GetHashCode();
						if (flag5)
						{
							bool flag6 = !flag;
							if (flag6)
							{
								return false;
							}
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
			}
			bool flag7 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.liga);
			int num2 = 0;
			while (num2 < textProcessingArray.Length && textProcessingArray[num2].unicode > 0U)
			{
				uint num3 = textProcessingArray[num2].unicode;
				int num4 = this.m_CurrentMaterialIndex;
				bool flag8 = generationSettings.richText && num3 == 60U;
				if (!flag8)
				{
					goto IL_02F0;
				}
				num4 = this.m_CurrentMaterialIndex;
				int num5;
				bool flag10;
				bool flag9 = this.ValidateHtmlTag(textProcessingArray, num2 + 1, out num5, generationSettings, null, out flag10);
				if (flag9)
				{
					int stringIndex = textProcessingArray[num2].stringIndex;
					num2 = num5;
					bool flag11 = this.m_TextElementType == TextElementType.Sprite;
					if (flag11)
					{
						this.m_TextElementType = TextElementType.Character;
						this.m_CurrentMaterialIndex = num4;
						num++;
						this.m_TotalCharacterCount++;
					}
				}
				else
				{
					bool flag12 = !flag10;
					if (flag12)
					{
						return false;
					}
					goto IL_02F0;
				}
				IL_0C08:
				num2++;
				continue;
				IL_02F0:
				bool flag13 = false;
				FontAsset currentFontAsset = this.m_CurrentFontAsset;
				Material currentMaterial = this.m_CurrentMaterial;
				num4 = this.m_CurrentMaterialIndex;
				bool flag14 = this.m_TextElementType == TextElementType.Character;
				if (flag14)
				{
					bool flag15 = (this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase;
					if (flag15)
					{
						bool flag16 = char.IsLower((char)num3);
						if (flag16)
						{
							num3 = (uint)char.ToUpper((char)num3);
						}
					}
					else
					{
						bool flag17 = (this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase;
						if (flag17)
						{
							bool flag18 = char.IsUpper((char)num3);
							if (flag18)
							{
								num3 = (uint)char.ToLower((char)num3);
							}
						}
						else
						{
							bool flag19 = (this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps;
							if (flag19)
							{
								bool flag20 = char.IsLower((char)num3);
								if (flag20)
								{
									num3 = (uint)char.ToUpper((char)num3);
								}
							}
						}
					}
				}
				bool flag21 = !flag && this.m_CurrentFontAsset.m_CharacterLookupDictionary == null;
				if (flag21)
				{
					return false;
				}
				TextElement textElement = null;
				uint num6 = ((num2 + 1 < textProcessingArray.Length) ? textProcessingArray[num2 + 1].unicode : 0U);
				bool flag22 = generationSettings.emojiFallbackSupport && ((TextGeneratorUtilities.IsEmojiPresentationForm(num3) && num6 != 65038U) || (TextGeneratorUtilities.IsEmoji(num3) && num6 == 65039U));
				if (flag22)
				{
					bool flag23 = textSettings.emojiFallbackTextAssets != null && textSettings.emojiFallbackTextAssets.Count > 0;
					if (flag23)
					{
						bool flag24;
						textElement = FontAssetUtilities.GetTextElementFromTextAssets(num3, this.m_CurrentFontAsset, textSettings.emojiFallbackTextAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag24, flag7);
						bool flag25 = textElement != null;
						if (flag25)
						{
						}
					}
				}
				bool flag26 = textElement == null;
				if (flag26)
				{
					bool flag24;
					textElement = this.GetTextElement(generationSettings, num3, this.m_CurrentFontAsset, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag24, flag7);
				}
				bool flag27 = textElement == null;
				if (flag27)
				{
					bool flag28 = !flag;
					if (flag28)
					{
						return false;
					}
					uint num7 = num3;
					num3 = (textProcessingArray[num2].unicode = (uint)((textSettings.missingCharacterUnicode == 0) ? 9633 : textSettings.missingCharacterUnicode));
					bool flag24;
					textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag24, flag7);
					bool flag29 = textElement == null;
					if (flag29)
					{
						bool flag30 = textSettings.GetFallbackFontAssets(this.m_CurrentFontAsset.IsRaster(), this.m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)) == null && !flag;
						if (flag30)
						{
							return false;
						}
						textElement = FontAssetUtilities.GetCharacterFromFontAssetsInternal(num3, this.m_CurrentFontAsset, textSettings.GetFallbackFontAssets(this.m_CurrentFontAsset.IsRaster(), this.m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag24, flag7);
					}
					bool flag31 = textElement == null;
					if (flag31)
					{
						bool flag32 = textSettings.defaultFontAsset != null;
						if (flag32)
						{
							textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, textSettings.defaultFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag24, flag7);
						}
					}
					bool flag33 = textElement == null;
					if (flag33)
					{
						num3 = (textProcessingArray[num2].unicode = 32U);
						textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag24, flag7);
					}
					bool flag34 = textElement == null;
					if (flag34)
					{
						num3 = (textProcessingArray[num2].unicode = 3U);
						textElement = FontAssetUtilities.GetCharacterFromFontAsset(num3, this.m_CurrentFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag24, flag7);
					}
					bool displayWarnings = textSettings.displayWarnings;
					if (displayWarnings)
					{
						string text = ((num7 > 65535U) ? string.Format("The character with Unicode value \\U{0:X8} was not found in the [{1}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{2:X4}.", num7, generationSettings.fontAsset.name, textElement.unicode) : string.Format("The character with Unicode value \\u{0:X4} was not found in the [{1}] font asset or any potential fallbacks. It was replaced by Unicode character \\u{2:X4}.", num7, generationSettings.fontAsset.name, textElement.unicode));
						Debug.LogWarning(text);
					}
				}
				bool flag35 = textElement.elementType == TextElementType.Character;
				if (flag35)
				{
					bool flag36 = (flag ? (textElement.textAsset.instanceID == this.m_CurrentFontAsset.instanceID) : (textElement.textAsset == this.m_CurrentFontAsset));
					bool flag37 = !flag36;
					if (flag37)
					{
						flag13 = true;
						this.m_CurrentFontAsset = textElement.textAsset as FontAsset;
					}
					bool flag38 = (num6 >= 65024U && num6 <= 65039U) || (num6 >= 917760U && num6 <= 917999U);
					if (flag38)
					{
						uint glyphVariantIndex;
						bool flag39 = !this.m_CurrentFontAsset.TryGetGlyphVariantIndexInternal(num3, num6, out glyphVariantIndex);
						if (flag39)
						{
							bool flag40 = !flag;
							if (flag40)
							{
								return false;
							}
							glyphVariantIndex = this.m_CurrentFontAsset.GetGlyphVariantIndex(num3, num6);
							this.m_CurrentFontAsset.TryAddGlyphVariantIndexInternal(num3, num6, glyphVariantIndex);
						}
						bool flag41 = glyphVariantIndex > 0U;
						if (flag41)
						{
							Glyph glyph;
							this.m_CurrentFontAsset.TryAddGlyphInternal(glyphVariantIndex, out glyph, true);
						}
						textProcessingArray[num2 + 1].unicode = 26U;
						num2++;
					}
					List<LigatureSubstitutionRecord> list;
					bool flag42 = flag7 && this.m_CurrentFontAsset.fontFeatureTable.m_LigatureSubstitutionRecordLookup.TryGetValue(textElement.glyphIndex, out list);
					if (flag42)
					{
						bool flag43 = list == null;
						if (flag43)
						{
							break;
						}
						for (int i = 0; i < list.Count; i++)
						{
							LigatureSubstitutionRecord ligatureSubstitutionRecord = list[i];
							int num8 = ligatureSubstitutionRecord.componentGlyphIDs.Length;
							uint num9 = ligatureSubstitutionRecord.ligatureGlyphID;
							for (int j = 1; j < num8; j++)
							{
								uint unicode = textProcessingArray[num2 + j].unicode;
								bool flag44;
								uint glyphIndex = this.m_CurrentFontAsset.GetGlyphIndex(unicode, out flag44);
								bool flag45 = !flag44;
								if (flag45)
								{
									return false;
								}
								bool flag46 = glyphIndex == ligatureSubstitutionRecord.componentGlyphIDs[j];
								if (!flag46)
								{
									num9 = 0U;
									break;
								}
							}
							bool flag47 = num9 > 0U;
							if (flag47)
							{
								bool flag48 = !flag;
								if (flag48)
								{
									return false;
								}
								Glyph glyph2;
								bool flag49 = this.m_CurrentFontAsset.TryAddGlyphInternal(num9, out glyph2, true);
								if (flag49)
								{
									for (int k = 0; k < num8; k++)
									{
										bool flag50 = k == 0;
										if (flag50)
										{
											textProcessingArray[num2 + k].length = num8;
										}
										else
										{
											textProcessingArray[num2 + k].unicode = 26U;
										}
									}
									num2 += num8 - 1;
									break;
								}
							}
						}
					}
				}
				bool flag51 = textElement.elementType == TextElementType.Sprite;
				if (flag51)
				{
					SpriteAsset spriteAsset = textElement.textAsset as SpriteAsset;
					this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(spriteAsset.material, spriteAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
					this.m_TextElementType = TextElementType.Character;
					this.m_CurrentMaterialIndex = num4;
					num++;
					this.m_TotalCharacterCount++;
					goto IL_0C08;
				}
				bool flag52 = flag13 && this.m_CurrentFontAsset.instanceID != generationSettings.fontAsset.instanceID;
				if (flag52)
				{
					bool flag53 = flag;
					if (flag53)
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
					}
					else
					{
						bool matchMaterialPreset2 = textSettings.matchMaterialPreset;
						if (matchMaterialPreset2)
						{
							return false;
						}
						this.m_CurrentMaterial = this.m_CurrentFontAsset.material;
					}
					this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
				}
				bool flag54 = textElement != null && textElement.glyph.atlasIndex > 0;
				if (flag54)
				{
					bool flag55 = flag;
					if (!flag55)
					{
						return false;
					}
					this.m_CurrentMaterial = MaterialManager.GetFallbackMaterial(this.m_CurrentFontAsset, this.m_CurrentMaterial, textElement.glyph.atlasIndex);
					this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(this.m_CurrentMaterial, this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
					flag13 = true;
				}
				bool flag56 = !char.IsWhiteSpace((char)num3) && num3 != 8203U;
				if (flag56)
				{
					bool flag57 = generationSettings.isIMGUI && this.m_MaterialReferences[this.m_CurrentMaterialIndex].referenceCount >= 16383;
					if (flag57)
					{
						this.m_CurrentMaterialIndex = MaterialReference.AddMaterialReference(new Material(this.m_CurrentMaterial), this.m_CurrentFontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
					}
					MaterialReference[] materialReferences = this.m_MaterialReferences;
					int currentMaterialIndex = this.m_CurrentMaterialIndex;
					materialReferences[currentMaterialIndex].referenceCount = materialReferences[currentMaterialIndex].referenceCount + 1;
				}
				this.m_MaterialReferences[this.m_CurrentMaterialIndex].isFallbackMaterial = flag13;
				bool flag58 = flag13;
				if (flag58)
				{
					this.m_MaterialReferences[this.m_CurrentMaterialIndex].fallbackMaterial = currentMaterial;
					this.m_CurrentFontAsset = currentFontAsset;
					this.m_CurrentMaterial = currentMaterial;
					this.m_CurrentMaterialIndex = num4;
				}
				this.m_TotalCharacterCount++;
				goto IL_0C08;
			}
			return true;
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

		protected bool GetSpecialCharacters(TextGenerationSettings generationSettings)
		{
			bool flag = !this.GetEllipsisSpecialCharacter(generationSettings);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = !this.GetUnderlineSpecialCharacter(generationSettings) || this.m_Underline.character == null;
				flag2 = !flag3;
			}
			return flag2;
		}

		protected bool GetEllipsisSpecialCharacter(TextGenerationSettings generationSettings)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			FontAsset fontAsset = this.m_CurrentFontAsset ?? generationSettings.fontAsset;
			TextSettings textSettings = generationSettings.textSettings;
			bool flag2 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.liga);
			bool flag3;
			Character character = FontAssetUtilities.GetCharacterFromFontAsset(8230U, fontAsset, false, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag3, flag2);
			bool flag4 = character == null;
			if (flag4)
			{
				bool flag5 = fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0;
				if (flag5)
				{
					character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(8230U, fontAsset, fontAsset.m_FallbackFontAssetTable, null, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag3, flag2);
				}
			}
			bool flag6 = character == null;
			if (flag6)
			{
				bool flag7 = textSettings.GetStaticFallbackOSFontAsset() == null && !flag;
				if (flag7)
				{
					return false;
				}
				character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(8230U, fontAsset, textSettings.GetFallbackFontAssets(fontAsset.IsRaster(), this.m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag3, flag2);
			}
			bool flag8 = character == null;
			if (flag8)
			{
				bool flag9 = textSettings.defaultFontAsset != null;
				if (flag9)
				{
					character = FontAssetUtilities.GetCharacterFromFontAsset(8230U, textSettings.defaultFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag3, flag2);
				}
			}
			bool flag10 = character != null;
			if (flag10)
			{
				this.m_Ellipsis = new TextGenerator.SpecialCharacter(character, 0);
			}
			return true;
		}

		protected bool GetUnderlineSpecialCharacter(TextGenerationSettings generationSettings)
		{
			bool flag = !TextGenerator.IsExecutingJob;
			FontAsset fontAsset = this.m_CurrentFontAsset ?? generationSettings.fontAsset;
			TextSettings textSettings = generationSettings.textSettings;
			bool flag2 = TextGenerationSettings.fontFeatures.Contains(OTL_FeatureTag.liga);
			bool flag3;
			Character character = FontAssetUtilities.GetCharacterFromFontAsset(95U, fontAsset, false, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag3, flag2);
			bool flag4 = character == null;
			if (flag4)
			{
				bool flag5 = fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0;
				if (flag5)
				{
					character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(95U, fontAsset, fontAsset.m_FallbackFontAssetTable, null, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag3, flag2);
				}
			}
			bool flag6 = character == null;
			if (flag6)
			{
				bool flag7 = textSettings.GetStaticFallbackOSFontAsset() == null && !flag;
				if (flag7)
				{
					return false;
				}
				character = FontAssetUtilities.GetCharacterFromFontAssetsInternal(95U, fontAsset, textSettings.GetFallbackFontAssets(fontAsset.IsRaster(), this.m_ShouldRenderBitmap ? generationSettings.fontSize : (-1)), textSettings.fallbackOSFontAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag3, flag2);
			}
			bool flag8 = character == null;
			if (flag8)
			{
				bool flag9 = textSettings.defaultFontAsset != null;
				if (flag9)
				{
					character = FontAssetUtilities.GetCharacterFromFontAsset(95U, textSettings.defaultFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag3, flag2);
				}
			}
			bool flag10 = character != null;
			if (flag10)
			{
				this.m_Underline = new TextGenerator.SpecialCharacter(character, this.m_CurrentMaterialIndex);
				bool flag11 = this.m_Underline.fontAsset.GetHashCode() != this.m_CurrentFontAsset.GetHashCode();
				if (flag11)
				{
					bool flag12 = generationSettings.textSettings.matchMaterialPreset && this.m_CurrentMaterial != null && this.m_CurrentMaterial.GetHashCode() != this.m_Underline.fontAsset.material.GetHashCode();
					if (flag12)
					{
						this.m_Underline.material = MaterialManager.GetFallbackMaterial(this.m_CurrentMaterial, this.m_Underline.fontAsset.material);
						bool flag13 = this.m_Underline.material == null;
						if (flag13)
						{
							return false;
						}
					}
					else
					{
						this.m_Underline.material = this.m_Underline.fontAsset.material;
					}
					this.m_Underline.materialIndex = MaterialReference.AddMaterialReference(this.m_Underline.material, this.m_Underline.fontAsset, ref this.m_MaterialReferences, this.m_MaterialReferenceIndexLookup);
					this.m_MaterialReferences[this.m_Underline.materialIndex].referenceCount = 0;
				}
			}
			return true;
		}

		protected void DoMissingGlyphCallback(uint unicode, int stringIndex, FontAsset fontAsset, TextInfo textInfo)
		{
			TextGenerator.MissingCharacterEventCallback onMissingCharacter = TextGenerator.OnMissingCharacter;
			if (onMissingCharacter != null)
			{
				onMissingCharacter(unicode, stringIndex, textInfo, fontAsset);
			}
		}

		private const int k_Tab = 9;

		private const int k_LineFeed = 10;

		private const int k_VerticalTab = 11;

		private const int k_CarriageReturn = 13;

		private const int k_Space = 32;

		private const int k_DoubleQuotes = 34;

		private const int k_NumberSign = 35;

		private const int k_PercentSign = 37;

		private const int k_SingleQuote = 39;

		private const int k_Plus = 43;

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

		private const int k_LineSeparator = 8232;

		private const int k_ParagraphSeparator = 8233;

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

		private char[] m_HtmlTag = new char[256];

		internal HighlightState m_HighlightState = new HighlightState(Color.white, Offset.zero);

		protected bool m_IsIgnoringAlignment;

		protected bool m_IsTextTruncated;

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

		private bool m_ShouldRenderBitmap;

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

		private float _m_BaselineOffset;

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

		private float _m_LineOffset;

		private float _m_LineHeight;

		private bool m_IsDrivenLineSpacing;

		private float m_CSpacing;

		private float m_MonoSpacing;

		private bool m_DuoSpace;

		private float _m_XAdvance;

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

		private float m_MarginLeft;

		private float m_MarginRight;

		private float m_Width;

		private Extents m_MeshExtents;

		private float m_MaxCapHeight;

		private float m_MaxAscender;

		private float m_MaxDescender;

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

		private float m_StartOfLineAscender;

		private float m_LineSpacingDelta;

		internal MaterialReference[] m_MaterialReferences = new MaterialReference[8];

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

		private bool m_TintSprite;

		protected TextGenerator.SpecialCharacter m_Ellipsis;

		protected TextGenerator.SpecialCharacter m_Underline;

		private TextElementInfo[] m_InternalTextElementInfo;

		internal static readonly bool EnableTextAlignmentAssertions;

		internal static readonly bool EnableCheckerboardPattern;

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
