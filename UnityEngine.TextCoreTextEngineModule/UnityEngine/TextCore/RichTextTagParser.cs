using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine.TextCore
{
	[global::System.Runtime.CompilerServices.Nullable(0)]
	[global::System.Runtime.CompilerServices.NullableContext(1)]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal static class RichTextTagParser
	{
		[global::System.Runtime.CompilerServices.NullableContext(0)]
		private unsafe static bool tagMatch(ReadOnlySpan<char> tagCandidate, [global::System.Runtime.CompilerServices.Nullable(1)] string tagName)
		{
			return tagCandidate.StartsWith<char>(tagName.AsSpan()) && (tagCandidate.Length == tagName.Length || (!char.IsLetter((char)(*tagCandidate[tagName.Length])) && *tagCandidate[tagName.Length] != 45));
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		private unsafe static bool SpanToEnum(ReadOnlySpan<char> tagCandidate, out RichTextTagParser.TagType tagType, [global::System.Runtime.CompilerServices.Nullable(2)] out string error, out ReadOnlySpan<char> attribute)
		{
			for (int i = 0; i < RichTextTagParser.TagsInfo.Length; i++)
			{
				string name = RichTextTagParser.TagsInfo[i].name;
				bool flag = RichTextTagParser.tagMatch(tagCandidate, name);
				if (flag)
				{
					tagType = RichTextTagParser.TagsInfo[i].TagType;
					error = null;
					attribute = tagCandidate.Slice(name.Length);
					return true;
				}
			}
			bool flag2 = tagCandidate.Length > 4 && *tagCandidate[0] == 35;
			if (flag2)
			{
				tagType = RichTextTagParser.TagType.Color;
				error = null;
				attribute = tagCandidate;
				return true;
			}
			error = "Unknown tag: " + tagCandidate.ToString();
			tagType = RichTextTagParser.TagType.Unknown;
			attribute = null;
			return false;
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		private static RichTextTagParser.TagValue ParseColorAttribute(ReadOnlySpan<char> attributeSection)
		{
			attributeSection = RichTextTagParser.GetAttributeSpan(attributeSection);
			Color color;
			bool flag = ColorUtility.TryParseHtmlString(attributeSection, out color);
			RichTextTagParser.TagValue tagValue;
			if (flag)
			{
				tagValue = new RichTextTagParser.TagValue(color, new RichTextTagParser.ValueID?(RichTextTagParser.ValueID.Color));
			}
			else
			{
				tagValue = null;
			}
			return tagValue;
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		private unsafe static RichTextTagParser.TagValue ParsePaddingAttribute(ReadOnlySpan<char> value)
		{
			Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)16], 4);
			Span<int> span2 = span;
			int num = 0;
			while (!value.IsEmpty && num < 4)
			{
				int num2 = value.IndexOf(',');
				bool flag = num2 >= 0;
				ReadOnlySpan<char> readOnlySpan;
				if (flag)
				{
					readOnlySpan = value.Slice(0, num2);
					value = value.Slice(num2 + 1);
				}
				else
				{
					readOnlySpan = value;
					value = ReadOnlySpan<char>.Empty;
				}
				bool flag2 = !int.TryParse(readOnlySpan, NumberStyles.Integer, CultureInfo.InvariantCulture, span2[num]);
				if (flag2)
				{
					return null;
				}
				num++;
			}
			bool flag3 = num != 4;
			if (flag3)
			{
				return null;
			}
			return new RichTextTagParser.TagValue(new Vector4((float)(*span2[0]), (float)(*span2[1]), (float)(*span2[2]), (float)(*span2[3])), new RichTextTagParser.ValueID?(RichTextTagParser.ValueID.Padding));
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		[return: global::System.Runtime.CompilerServices.Nullable(2)]
		private static RichTextTagParser.TagValue ParseHref(ReadOnlySpan<char> attributeSection)
		{
			string text;
			bool flag = RichTextTagParser.TryGetSimpleHref(attributeSection, out text);
			RichTextTagParser.TagValue tagValue;
			if (flag)
			{
				tagValue = new RichTextTagParser.TagValue(text, null);
			}
			else
			{
				tagValue = new RichTextTagParser.TagValue(attributeSection.TrimStart().ToString(), null);
			}
			return tagValue;
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		private unsafe static bool TryGetSimpleHref(ReadOnlySpan<char> attributeSection, [global::System.Runtime.CompilerServices.Nullable(1)] out string hrefValue)
		{
			hrefValue = "";
			attributeSection = attributeSection.Trim();
			bool flag = !attributeSection.StartsWith("href=".AsSpan(), StringComparison.OrdinalIgnoreCase);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ReadOnlySpan<char> readOnlySpan = attributeSection.Slice("href=".Length);
				char c = (char)((readOnlySpan.Length > 0) ? (*readOnlySpan[0]) : 0);
				bool flag3 = c == '"' || c == '\'';
				if (flag3)
				{
					ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(1);
					int num = readOnlySpan2.IndexOf(c);
					bool flag4 = num == -1;
					if (flag4)
					{
						return false;
					}
					bool flag5 = readOnlySpan2.Slice(num + 1).Trim().Length > 0;
					if (flag5)
					{
						return false;
					}
					hrefValue = readOnlySpan2.Slice(0, num).ToString();
				}
				else
				{
					bool flag6 = readOnlySpan.Contains(new ReadOnlySpan<char>(new char[] { ' ' }), StringComparison.OrdinalIgnoreCase);
					if (flag6)
					{
						return false;
					}
					hrefValue = readOnlySpan.ToString();
				}
				flag2 = true;
			}
			return flag2;
		}

		[global::System.Runtime.CompilerServices.NullableContext(2)]
		private unsafe static bool ParseSpriteAttributes([global::System.Runtime.CompilerServices.Nullable(0)] ReadOnlySpan<char> attributeSection, [global::System.Runtime.CompilerServices.Nullable(1)] TextSettings textSettings, out char unicode, out RichTextTagParser.TagValue spriteAssetValue, out RichTextTagParser.TagValue glyphMetricsValue, out RichTextTagParser.TagValue tintValue, out RichTextTagParser.TagValue scaleValue, out RichTextTagParser.TagValue colorValue, out string spriteAssetNameOut)
		{
			int num = -1;
			unicode = '\0';
			spriteAssetValue = null;
			glyphMetricsValue = null;
			tintValue = null;
			scaleValue = null;
			colorValue = null;
			spriteAssetNameOut = null;
			ReadOnlySpan<char> readOnlySpan = ReadOnlySpan<char>.Empty;
			ReadOnlySpan<char> readOnlySpan2 = ReadOnlySpan<char>.Empty;
			SpriteAsset spriteAsset = null;
			while (!attributeSection.IsEmpty)
			{
				attributeSection = attributeSection.TrimStart();
				bool isEmpty = attributeSection.IsEmpty;
				if (isEmpty)
				{
					break;
				}
				int num2 = attributeSection.IndexOf('=');
				bool flag = num2 == -1;
				if (flag)
				{
					break;
				}
				ReadOnlySpan<char> readOnlySpan3 = attributeSection.Slice(0, num2).Trim();
				ReadOnlySpan<char> readOnlySpan4 = attributeSection.Slice(num2 + 1).TrimStart();
				char c = (char)((readOnlySpan4.Length > 0) ? (*readOnlySpan4[0]) : 0);
				bool flag2 = c == '"' || c == '\'';
				ReadOnlySpan<char> readOnlySpan6;
				if (flag2)
				{
					ReadOnlySpan<char> readOnlySpan5 = readOnlySpan4.Slice(1);
					int num3 = readOnlySpan5.IndexOf(c);
					bool flag3 = num3 == -1;
					if (flag3)
					{
						break;
					}
					readOnlySpan6 = readOnlySpan5.Slice(0, num3);
					attributeSection = readOnlySpan5.Slice(num3 + 1);
				}
				else
				{
					int num4 = readOnlySpan4.IndexOf(' ');
					bool flag4 = num4 == -1;
					if (flag4)
					{
						readOnlySpan6 = readOnlySpan4;
						attributeSection = ReadOnlySpan<char>.Empty;
					}
					else
					{
						readOnlySpan6 = readOnlySpan4.Slice(0, num4);
						attributeSection = readOnlySpan4.Slice(num4);
					}
				}
				bool isEmpty2 = readOnlySpan3.IsEmpty;
				if (isEmpty2)
				{
					int num5;
					bool flag5 = int.TryParse(readOnlySpan6, out num5);
					if (flag5)
					{
						num = num5;
					}
					else
					{
						readOnlySpan = readOnlySpan6;
					}
				}
				else
				{
					bool flag6 = readOnlySpan3.SequenceEqual<char>("name");
					if (flag6)
					{
						readOnlySpan2 = readOnlySpan6;
					}
					else
					{
						bool flag7 = readOnlySpan3.SequenceEqual<char>("index");
						if (flag7)
						{
							int num6;
							bool flag8 = int.TryParse(readOnlySpan6, out num6);
							if (flag8)
							{
								num = num6;
							}
						}
						else
						{
							bool flag9 = readOnlySpan3.SequenceEqual<char>("tint");
							if (flag9)
							{
								int num7;
								bool flag10 = int.TryParse(readOnlySpan6, out num7) && num7 == 1;
								if (flag10)
								{
									tintValue = new RichTextTagParser.TagValue(true, new RichTextTagParser.ValueID?(RichTextTagParser.ValueID.Tint));
								}
							}
							else
							{
								bool flag11 = readOnlySpan3.SequenceEqual<char>("color");
								if (flag11)
								{
									readOnlySpan6 = RichTextTagParser.GetAttributeSpan(readOnlySpan6);
									Color color;
									bool flag12 = ColorUtility.TryParseHtmlString(readOnlySpan6, out color);
									if (flag12)
									{
										colorValue = new RichTextTagParser.TagValue(color, new RichTextTagParser.ValueID?(RichTextTagParser.ValueID.SpriteColor));
									}
								}
							}
						}
					}
				}
			}
			bool flag13 = !readOnlySpan.IsEmpty;
			if (flag13)
			{
				spriteAssetNameOut = readOnlySpan.ToString();
				WeakReference<SpriteAsset> weakReference;
				bool flag14 = !RichTextTagParser.s_SpriteAssetCache.TryGetValue(spriteAssetNameOut, out weakReference) || !weakReference.TryGetTarget(out spriteAsset);
				if (flag14)
				{
					return false;
				}
			}
			else
			{
				bool flag15 = textSettings.defaultSpriteAsset != null;
				if (flag15)
				{
					spriteAsset = textSettings.defaultSpriteAsset;
				}
				else
				{
					bool flag16 = TextSettings.s_GlobalSpriteAsset != null;
					if (flag16)
					{
						spriteAsset = TextSettings.s_GlobalSpriteAsset;
					}
				}
				bool flag17 = spriteAsset == null;
				if (flag17)
				{
					return false;
				}
			}
			bool flag18 = !readOnlySpan2.IsEmpty;
			if (flag18)
			{
				num = spriteAsset.GetSpriteIndexFromName(readOnlySpan2.ToString());
			}
			bool flag19 = num == -1;
			bool flag20;
			if (flag19)
			{
				flag20 = false;
			}
			else
			{
				bool flag21 = spriteAsset.spriteCharacterTable.Count <= num;
				if (flag21)
				{
					flag20 = false;
				}
				else
				{
					SpriteCharacter spriteCharacter = spriteAsset.spriteCharacterTable[num];
					spriteAssetValue = new RichTextTagParser.TagValue((float)spriteAsset.instanceID, RichTextTagParser.TagUnitType.Unknown, new RichTextTagParser.ValueID?(RichTextTagParser.ValueID.AssetID));
					glyphMetricsValue = new RichTextTagParser.TagValue(spriteCharacter.glyph.metrics, new RichTextTagParser.ValueID?(RichTextTagParser.ValueID.GlyphMetrics));
					scaleValue = new RichTextTagParser.TagValue(spriteCharacter.scale, RichTextTagParser.TagUnitType.Unknown, new RichTextTagParser.ValueID?(RichTextTagParser.ValueID.Scale));
					unicode = (char)((int)RichTextTagParser.k_PrivateArea + num);
					flag20 = true;
				}
			}
			return flag20;
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		public unsafe static int GetHashCode(ReadOnlySpan<char> span)
		{
			HashCode hashCode = default(HashCode);
			ReadOnlySpan<char> readOnlySpan = span;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = (char)(*readOnlySpan[i]);
				hashCode.Add<char>(c);
			}
			return hashCode.ToHashCode();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void PreloadFontAssetsFromTags(string text, TextSettings textSettings)
		{
			List<string> list;
			bool flag = !RichTextTagParser.HasFontTags(text, textSettings, out list);
			if (!flag)
			{
				foreach (string text2 in list)
				{
					bool flag2 = RichTextTagParser.s_FontAssetCache.ContainsKey(text2);
					if (!flag2)
					{
						FontAsset fontAsset = Resources.Load<FontAsset>(textSettings.defaultFontAssetPath + text2);
						bool flag3 = fontAsset == null;
						if (!flag3)
						{
							fontAsset.EnsureNativeFontAssetIsCreated();
							RichTextTagParser.s_FontAssetCache[text2] = fontAsset.nativeFontAsset;
						}
					}
				}
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void PreloadSpriteAssetsFromTags(string text, TextSettings textSettings)
		{
			List<string> list;
			bool flag = !RichTextTagParser.HasSpriteTags(text, textSettings, out list);
			if (!flag)
			{
				foreach (string text2 in list)
				{
					bool flag2 = RichTextTagParser.s_SpriteAssetCache.ContainsKey(text2);
					if (!flag2)
					{
						SpriteAsset spriteAsset = Resources.Load<SpriteAsset>(textSettings.defaultSpriteAssetPath + text2);
						bool flag3 = spriteAsset == null;
						if (!flag3)
						{
							spriteAsset.UpdateLookupTables();
							RichTextTagParser.s_SpriteAssetCache[text2] = new WeakReference<SpriteAsset>(spriteAsset);
						}
					}
				}
			}
		}

		internal unsafe static List<RichTextTagParser.Tag> FindTags(ref string inputStr, TextSettings textSettings, bool preprocessingOnly = false, [global::System.Runtime.CompilerServices.Nullable(new byte[] { 2, 1 })] List<RichTextTagParser.ParseError> errors = null)
		{
			char[] array = inputStr.ToCharArray();
			List<RichTextTagParser.Tag> list = new List<RichTextTagParser.Tag>();
			int num = 0;
			for (;;)
			{
				int num2 = Array.IndexOf<char>(array, '<', num);
				bool flag = num2 == -1;
				if (flag)
				{
					break;
				}
				int num3 = Array.IndexOf<char>(array, '>', num2);
				bool flag2 = num3 == -1;
				if (flag2)
				{
					break;
				}
				bool flag3 = array.Length > num2 + 1 && array[num2 + 1] == '/';
				bool flag4 = num3 == num2 + 1;
				if (flag4)
				{
					if (errors != null)
					{
						errors.Add(new RichTextTagParser.ParseError("Empty tag", num2));
					}
					num = num3 + 1;
				}
				else
				{
					num = num3 + 1;
					bool flag5 = !flag3;
					if (flag5)
					{
						Span<char> span = array.AsSpan<char>(num2 + 1, num3 - num2 - 1);
						RichTextTagParser.TagType tagType;
						string text;
						ReadOnlySpan<char> readOnlySpan;
						bool flag6 = RichTextTagParser.SpanToEnum(span, out tagType, out text, out readOnlySpan);
						if (flag6)
						{
							RichTextTagParser.TagValue tagValue = null;
							RichTextTagParser.TagValue tagValue2 = null;
							bool flag7 = tagType == RichTextTagParser.TagType.Color;
							if (flag7)
							{
								tagValue = RichTextTagParser.ParseColorAttribute(readOnlySpan);
								bool flag8 = tagValue == null;
								if (flag8)
								{
									if (errors != null)
									{
										errors.Add(new RichTextTagParser.ParseError("Invalid color value", num2));
									}
									num = num2 + 1;
									continue;
								}
							}
							bool flag9 = tagType == RichTextTagParser.TagType.Mark;
							if (flag9)
							{
								tagValue = RichTextTagParser.ParseColorAttribute(readOnlySpan);
								bool flag10 = tagValue == null;
								if (flag10)
								{
									while (!readOnlySpan.IsEmpty)
									{
										int num4 = readOnlySpan.IndexOf(' ');
										bool flag11 = num4 >= 0;
										ReadOnlySpan<char> readOnlySpan2;
										if (flag11)
										{
											readOnlySpan2 = readOnlySpan.Slice(0, num4);
											bool flag12 = num4 + 1 < readOnlySpan.Length;
											if (flag12)
											{
												readOnlySpan = readOnlySpan.Slice(num4 + 1);
											}
											else
											{
												readOnlySpan = ReadOnlySpan<char>.Empty;
											}
										}
										else
										{
											readOnlySpan2 = readOnlySpan;
											readOnlySpan = ReadOnlySpan<char>.Empty;
										}
										int num5 = readOnlySpan2.IndexOf('=');
										bool flag13 = num5 <= 0 || num5 >= readOnlySpan2.Length - 1;
										if (!flag13)
										{
											ReadOnlySpan<char> readOnlySpan3 = readOnlySpan2.Slice(0, num5);
											ReadOnlySpan<char> readOnlySpan4 = readOnlySpan2.Slice(num5 + 1);
											bool flag14 = readOnlySpan3.SequenceEqual<char>("color");
											if (flag14)
											{
												tagValue = RichTextTagParser.ParseColorAttribute(readOnlySpan4);
											}
											else
											{
												bool flag15 = readOnlySpan3.SequenceEqual<char>("padding");
												if (flag15)
												{
													tagValue2 = RichTextTagParser.ParsePaddingAttribute(readOnlySpan4);
												}
											}
										}
									}
								}
							}
							bool flag16 = tagType == RichTextTagParser.TagType.Hyperlink;
							if (flag16)
							{
								tagValue = RichTextTagParser.ParseHref(readOnlySpan);
							}
							bool flag17 = tagType == RichTextTagParser.TagType.Link;
							if (flag17)
							{
								readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
								string text2 = readOnlySpan.ToString();
								tagValue = new RichTextTagParser.TagValue(text2, null);
							}
							bool flag18 = tagType == RichTextTagParser.TagType.Sprite;
							if (flag18)
							{
								char c;
								RichTextTagParser.TagValue tagValue3;
								RichTextTagParser.TagValue tagValue4;
								RichTextTagParser.TagValue tagValue5;
								string text3;
								bool flag19 = RichTextTagParser.ParseSpriteAttributes(readOnlySpan, textSettings, out c, out tagValue, out tagValue2, out tagValue3, out tagValue4, out tagValue5, out text3);
								bool flag20 = !flag19;
								if (flag20)
								{
									bool flag21 = preprocessingOnly && text3 != null;
									if (flag21)
									{
										list.Add(new RichTextTagParser.Tag
										{
											tagType = tagType,
											start = num2,
											end = num3,
											isClosing = false,
											value = new RichTextTagParser.TagValue(text3, null)
										});
									}
								}
								else
								{
									list.Add(new RichTextTagParser.Tag
									{
										tagType = tagType,
										start = num2,
										end = num3,
										isClosing = false,
										value = tagValue,
										value2 = tagValue2,
										value3 = tagValue3,
										value4 = tagValue4,
										value5 = tagValue5
									});
									inputStr = inputStr.Insert(num3 + 1, c.ToString() + "/");
									array = inputStr.ToCharArray();
									list.Add(new RichTextTagParser.Tag
									{
										tagType = tagType,
										start = num3 + 2,
										end = num3 + 2,
										isClosing = true,
										value = tagValue,
										value2 = tagValue2,
										value3 = tagValue3,
										value4 = tagValue4,
										value5 = tagValue5
									});
									num = num3 + 2;
								}
							}
							else
							{
								bool flag22 = tagType == RichTextTagParser.TagType.Br;
								if (flag22)
								{
									bool flag23 = !readOnlySpan.IsEmpty;
									if (!flag23)
									{
										list.Add(new RichTextTagParser.Tag
										{
											tagType = tagType,
											start = num2,
											end = num3,
											isClosing = false,
											value = null
										});
										inputStr = inputStr.Insert(num3 + 1, "\n/");
										array = inputStr.ToCharArray();
										list.Add(new RichTextTagParser.Tag
										{
											tagType = tagType,
											start = num3 + 2,
											end = num3 + 2,
											isClosing = true,
											value = null
										});
										num = num3 + 2;
									}
								}
								else
								{
									bool flag24 = tagType == RichTextTagParser.TagType.Align;
									if (flag24)
									{
										readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
										string text4 = readOnlySpan.ToString();
										HorizontalAlignment horizontalAlignment;
										bool flag25 = Enum.TryParse<HorizontalAlignment>(text4, true, out horizontalAlignment);
										if (flag25)
										{
											tagValue = new RichTextTagParser.TagValue(text4, null);
										}
										bool flag26 = tagValue == null;
										if (flag26)
										{
											if (errors != null)
											{
												errors.Add(new RichTextTagParser.ParseError(string.Format("Invalid {0} value", tagType), num2));
											}
											num = num2 + 1;
											continue;
										}
									}
									bool flag27 = tagType == RichTextTagParser.TagType.Mspace || tagType == RichTextTagParser.TagType.CSpace;
									if (flag27)
									{
										RichTextTagParser.TagUnitType tagUnitType = RichTextTagParser.ParseTagUnitType(ref readOnlySpan);
										bool flag28 = tagUnitType == RichTextTagParser.TagUnitType.Percentage;
										if (flag28)
										{
											if (errors != null)
											{
												errors.Add(new RichTextTagParser.ParseError(string.Format("Invalid {0} value", tagUnitType), num2));
											}
											num = num2 + 1;
											continue;
										}
										bool flag29 = tagUnitType == RichTextTagParser.TagUnitType.Unknown;
										if (flag29)
										{
											tagUnitType = RichTextTagParser.TagUnitType.Pixels;
										}
										readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
										float num6;
										bool flag30 = !float.TryParse(readOnlySpan, NumberStyles.Float, CultureInfo.InvariantCulture, out num6);
										if (flag30)
										{
											if (errors != null)
											{
												errors.Add(new RichTextTagParser.ParseError("Invalid numerical value", num2));
											}
											num = num2 + 1;
											continue;
										}
										tagValue = new RichTextTagParser.TagValue(num6, tagUnitType, null);
									}
									bool flag31 = tagType == RichTextTagParser.TagType.Margin || tagType == RichTextTagParser.TagType.MarginLeft || tagType == RichTextTagParser.TagType.MarginRight;
									if (flag31)
									{
										RichTextTagParser.TagUnitType tagUnitType2 = RichTextTagParser.ParseTagUnitType(ref readOnlySpan);
										bool flag32 = tagUnitType2 == RichTextTagParser.TagUnitType.Unknown;
										if (flag32)
										{
											tagUnitType2 = RichTextTagParser.TagUnitType.Pixels;
										}
										readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
										float num7;
										bool flag33 = !float.TryParse(readOnlySpan, NumberStyles.Float, CultureInfo.InvariantCulture, out num7);
										if (flag33)
										{
											if (errors != null)
											{
												errors.Add(new RichTextTagParser.ParseError("Invalid numerical value", num2));
											}
											num = num2 + 1;
											continue;
										}
										tagValue = new RichTextTagParser.TagValue(num7, tagUnitType2, null);
									}
									bool flag34 = tagType == RichTextTagParser.TagType.Font;
									if (flag34)
									{
										readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
										string text5 = readOnlySpan.ToString();
										bool flag35 = string.IsNullOrEmpty(text5);
										if (flag35)
										{
											if (errors != null)
											{
												errors.Add(new RichTextTagParser.ParseError("Font name cannot be empty", num2));
											}
											num = num2 + 1;
											continue;
										}
										bool flag36 = RichTextTagParser.s_FontAssetCache.ContainsKey(text5);
										bool flag37 = !flag36;
										if (flag37)
										{
											if (preprocessingOnly)
											{
												list.Add(new RichTextTagParser.Tag
												{
													tagType = tagType,
													start = num2,
													end = num3,
													isClosing = false,
													value = new RichTextTagParser.TagValue(text5, null)
												});
											}
											num = num2 + 1;
											continue;
										}
										tagValue = new RichTextTagParser.TagValue(text5, null);
									}
									bool flag38 = tagType == RichTextTagParser.TagType.Size;
									if (flag38)
									{
										RichTextTagParser.TagUnitType tagUnitType3 = RichTextTagParser.ParseTagUnitType(ref readOnlySpan);
										bool flag39 = tagUnitType3 == RichTextTagParser.TagUnitType.Unknown;
										if (flag39)
										{
											tagUnitType3 = RichTextTagParser.TagUnitType.Pixels;
										}
										readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
										bool flag40 = false;
										bool flag41 = readOnlySpan.Length > 0 && (*readOnlySpan[0] == 43 || *readOnlySpan[0] == 45);
										if (flag41)
										{
											flag40 = true;
										}
										float num8;
										bool flag42 = !float.TryParse(readOnlySpan, NumberStyles.Float, CultureInfo.InvariantCulture, out num8);
										if (flag42)
										{
											if (errors != null)
											{
												errors.Add(new RichTextTagParser.ParseError("Invalid size value", num2));
											}
											num = num2 + 1;
											continue;
										}
										tagValue = new RichTextTagParser.TagValue(num8, tagUnitType3, null);
										tagValue2 = new RichTextTagParser.TagValue(flag40, null);
									}
									bool flag43 = tagType == RichTextTagParser.TagType.FontWeight;
									if (flag43)
									{
										readOnlySpan = RichTextTagParser.GetAttributeSpan(readOnlySpan);
										int num9;
										bool flag44 = int.TryParse(readOnlySpan, NumberStyles.Integer, CultureInfo.InvariantCulture, out num9);
										if (!flag44)
										{
											if (errors != null)
											{
												errors.Add(new RichTextTagParser.ParseError("Invalid font-weight value", num2));
											}
											num = num2 + 1;
											continue;
										}
										bool flag45 = Enum.IsDefined(typeof(TextFontWeight), num9);
										if (!flag45)
										{
											if (errors != null)
											{
												errors.Add(new RichTextTagParser.ParseError(string.Format("Invalid font-weight value: {0}", num9), num2));
											}
											num = num2 + 1;
											continue;
										}
										tagValue = new RichTextTagParser.TagValue((float)num9, RichTextTagParser.TagUnitType.Unknown, null);
									}
									list.Add(new RichTextTagParser.Tag
									{
										tagType = tagType,
										start = num2,
										end = num3,
										isClosing = flag3,
										value = tagValue,
										value2 = tagValue2
									});
									bool flag46 = tagType == RichTextTagParser.TagType.NoParse;
									if (flag46)
									{
										bool flag47 = (num2 = array.AsSpan<char>(num).IndexOf<char>("</noparse>")) == -1;
										if (flag47)
										{
											break;
										}
										num2 += num;
										num3 = num2 + "</noparse>".Length - 1;
										list.Add(new RichTextTagParser.Tag
										{
											tagType = RichTextTagParser.TagType.NoParse,
											start = num2,
											end = num3,
											isClosing = true
										});
										num = num3 + 1;
									}
								}
							}
						}
						else
						{
							bool flag48 = text != null;
							if (flag48)
							{
								if (errors != null)
								{
									errors.Add(new RichTextTagParser.ParseError(text, num2));
								}
							}
							num = num2 + 1;
						}
					}
					else
					{
						RichTextTagParser.TagType tagType2;
						string text6;
						ReadOnlySpan<char> readOnlySpan5;
						bool flag49 = RichTextTagParser.SpanToEnum(array.AsSpan<char>(num2 + 2, num3 - num2 - 2), out tagType2, out text6, out readOnlySpan5);
						if (flag49)
						{
							list.Add(new RichTextTagParser.Tag
							{
								tagType = tagType2,
								start = num2,
								end = num3,
								isClosing = flag3
							});
						}
						else
						{
							bool flag50 = text6 != null;
							if (flag50)
							{
								if (errors != null)
								{
									errors.Add(new RichTextTagParser.ParseError(text6, num2));
								}
							}
							num = num2 + 1;
						}
					}
				}
			}
			return list;
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		private unsafe static ReadOnlySpan<char> GetAttributeSpan(ReadOnlySpan<char> attributeSection)
		{
			bool flag = attributeSection.Length >= 1 && *attributeSection[0] == 61;
			if (flag)
			{
				attributeSection = attributeSection.Slice(1);
			}
			bool flag2 = attributeSection.Length >= 2 && ((*attributeSection[0] == 34 && *attributeSection[attributeSection.Length - 1] == 34) || (*attributeSection[0] == 39 && *attributeSection[attributeSection.Length - 1] == 39));
			ReadOnlySpan<char> readOnlySpan;
			if (flag2)
			{
				readOnlySpan = attributeSection.Slice(1, attributeSection.Length - 2);
			}
			else
			{
				readOnlySpan = attributeSection;
			}
			return readOnlySpan;
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		private static RichTextTagParser.TagUnitType ParseTagUnitType(ref ReadOnlySpan<char> attributeSection)
		{
			bool flag = attributeSection.EndsWith("em".AsSpan(), StringComparison.OrdinalIgnoreCase);
			RichTextTagParser.TagUnitType tagUnitType;
			if (flag)
			{
				attributeSection = attributeSection.Slice(0, attributeSection.Length - 2);
				tagUnitType = RichTextTagParser.TagUnitType.FontUnits;
			}
			else
			{
				bool flag2 = attributeSection.EndsWith("px".AsSpan(), StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					attributeSection = attributeSection.Slice(0, attributeSection.Length - 2);
					tagUnitType = RichTextTagParser.TagUnitType.Pixels;
				}
				else
				{
					bool flag3 = attributeSection.EndsWith("%".AsSpan(), StringComparison.OrdinalIgnoreCase);
					if (flag3)
					{
						attributeSection = attributeSection.Slice(0, attributeSection.Length - 1);
						tagUnitType = RichTextTagParser.TagUnitType.Percentage;
					}
					else
					{
						tagUnitType = RichTextTagParser.TagUnitType.Unknown;
					}
				}
			}
			return tagUnitType;
		}

		internal unsafe static List<RichTextTagParser.Tag> PickResultingTags(List<RichTextTagParser.Tag> allTags, string input, int atPosition, [global::System.Runtime.CompilerServices.Nullable(2)] List<RichTextTagParser.Tag> applicableTags = null)
		{
			bool flag = applicableTags == null;
			if (flag)
			{
				applicableTags = new List<RichTextTagParser.Tag>();
			}
			else
			{
				applicableTags.Clear();
			}
			int num = 0;
			Debug.Assert(string.IsNullOrEmpty(input) || (atPosition < input.Length && atPosition >= 0), "Invalid position");
			Debug.Assert(num <= atPosition && num >= 0, "Invalid starting position");
			int num2 = 0;
			foreach (RichTextTagParser.Tag tag in allTags)
			{
				Debug.Assert(tag.start >= num2, "Tags are not sorted");
				num2 = tag.end + 1;
			}
			foreach (RichTextTagParser.Tag tag2 in applicableTags)
			{
				Debug.Assert(tag2.end <= num, "Tag end pass the point where we should start parsing");
				Debug.Assert(allTags.Contains(tag2));
			}
			int count = allTags.Count;
			Span<int?> span2;
			Span<int?> span3;
			int num4;
			checked
			{
				Span<int?> span = new Span<int?>(stackalloc byte[unchecked((UIntPtr)count) * (UIntPtr)sizeof(int?)], count);
				span2 = span;
				int num3 = RichTextTagParser.TagsInfo.Length;
				span = new Span<int?>(stackalloc byte[unchecked((UIntPtr)num3) * (UIntPtr)sizeof(int?)], num3);
				span3 = span;
				num4 = -1;
			}
			foreach (RichTextTagParser.Tag tag3 in allTags)
			{
				num4++;
				bool flag2 = tag3.end < num;
				if (!flag2)
				{
					bool flag3 = tag3.tagType == RichTextTagParser.TagType.NoParse;
					if (!flag3)
					{
						bool flag4 = tag3.start > atPosition;
						if (flag4)
						{
							break;
						}
						bool isClosing = tag3.isClosing;
						if (isClosing)
						{
							bool flag5 = span3[(int)tag3.tagType] != null;
							if (flag5)
							{
								bool flag6 = span2[num4] != null;
								if (flag6)
								{
									*span3[(int)tag3.tagType] = *span2[num4];
								}
								else
								{
									*span3[(int)tag3.tagType] = null;
								}
							}
						}
						else
						{
							int? num5 = *span3[(int)tag3.tagType];
							bool flag7 = num5 != null;
							if (flag7)
							{
								*span2[num4] = num5;
							}
							*span3[(int)tag3.tagType] = new int?(num4);
						}
					}
				}
			}
			int num6 = 0;
			foreach (RichTextTagParser.Tag tag4 in allTags)
			{
				int? num7 = *span3[(int)tag4.tagType];
				bool flag8 = num7 != null && num6 == num7.Value;
				if (flag8)
				{
					applicableTags.Add(tag4);
				}
				num6++;
			}
			return applicableTags;
		}

		internal static RichTextTagParser.Segment[] GenerateSegments(string input, List<RichTextTagParser.Tag> tags)
		{
			List<RichTextTagParser.Segment> list = new List<RichTextTagParser.Segment>();
			int num = 0;
			for (int i = 0; i < tags.Count; i++)
			{
				Debug.Assert(tags[i].start >= num);
				bool flag = tags[i].start > num;
				if (flag)
				{
					list.Add(new RichTextTagParser.Segment
					{
						start = num,
						end = tags[i].start - 1
					});
				}
				num = tags[i].end + 1;
			}
			bool flag2 = num < input.Length;
			if (flag2)
			{
				list.Add(new RichTextTagParser.Segment
				{
					start = num,
					end = input.Length - 1
				});
			}
			return list.ToArray();
		}

		internal static void ApplyStateToSegment(string input, List<RichTextTagParser.Tag> tags, RichTextTagParser.Segment[] segments)
		{
			for (int i = 0; i < segments.Length; i++)
			{
				segments[i].tags = RichTextTagParser.PickResultingTags(tags, input, segments[i].start, null);
			}
		}

		private static int AddLink(RichTextTagParser.TagType type, string value, [global::System.Runtime.CompilerServices.Nullable(new byte[] { 1, 0, 1 })] List<ValueTuple<int, RichTextTagParser.TagType, string>> links)
		{
			foreach (ValueTuple<int, RichTextTagParser.TagType, string> valueTuple in links)
			{
				int item = valueTuple.Item1;
				RichTextTagParser.TagType item2 = valueTuple.Item2;
				string item3 = valueTuple.Item3;
				bool flag = type == item2 && value == item3;
				if (flag)
				{
					return item;
				}
			}
			int count = links.Count;
			links.Add(new ValueTuple<int, RichTextTagParser.TagType, string>(count, type, value));
			return count;
		}

		private static TextSpan CreateTextSpan(RichTextTagParser.Segment segment, ref NativeTextGenerationSettings tgs, [global::System.Runtime.CompilerServices.Nullable(new byte[] { 1, 0, 1 })] List<ValueTuple<int, RichTextTagParser.TagType, string>> links, Color hyperlinkColor, float pixelsPerPoint)
		{
			TextSpan textSpan = tgs.CreateTextSpan();
			bool flag = segment.tags == null;
			TextSpan textSpan2;
			if (flag)
			{
				textSpan2 = textSpan;
			}
			else
			{
				for (int i = 0; i < segment.tags.Count; i++)
				{
					switch (segment.tags[i].tagType)
					{
					case RichTextTagParser.TagType.Hyperlink:
					{
						RichTextTagParser.TagType tagType = RichTextTagParser.TagType.Hyperlink;
						RichTextTagParser.TagValue value = segment.tags[i].value;
						textSpan.linkID = RichTextTagParser.AddLink(tagType, ((value != null) ? value.StringValue : null) ?? "", links);
						textSpan.color = hyperlinkColor;
						textSpan.fontStyle |= FontStyles.Underline;
						break;
					}
					case RichTextTagParser.TagType.Align:
						Enum.TryParse<HorizontalAlignment>(segment.tags[i].value.StringValue, true, out textSpan.alignment);
						break;
					case RichTextTagParser.TagType.AllCaps:
					case RichTextTagParser.TagType.Uppercase:
						textSpan.fontStyle |= FontStyles.UpperCase;
						break;
					case RichTextTagParser.TagType.Bold:
						textSpan.fontWeight = TextFontWeight.Bold;
						break;
					case RichTextTagParser.TagType.Color:
						textSpan.color = segment.tags[i].value.ColorValue;
						break;
					case RichTextTagParser.TagType.CSpace:
					{
						float num = ((segment.tags[i].value.unit == RichTextTagParser.TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
						textSpan.cspace = (int)(segment.tags[i].value.NumericalValue * num);
						textSpan.cspaceUnitType = segment.tags[i].value.unit;
						break;
					}
					case RichTextTagParser.TagType.Font:
					{
						RichTextTagParser.TagValue value2 = segment.tags[i].value;
						string text = ((value2 != null) ? value2.StringValue : null) ?? "";
						bool flag2 = !string.IsNullOrEmpty(text);
						if (flag2)
						{
							IntPtr intPtr;
							bool flag3 = RichTextTagParser.s_FontAssetCache.TryGetValue(text, out intPtr);
							if (flag3)
							{
								textSpan.fontAsset = intPtr;
							}
						}
						break;
					}
					case RichTextTagParser.TagType.FontWeight:
					{
						RichTextTagParser.TagValue value3 = segment.tags[i].value;
						bool flag4 = value3 != null && value3.type == RichTextTagParser.TagValueType.NumericalValue;
						if (flag4)
						{
							textSpan.fontWeight = (TextFontWeight)segment.tags[i].value.NumericalValue;
						}
						break;
					}
					case RichTextTagParser.TagType.Italic:
						textSpan.fontStyle |= FontStyles.Italic;
						break;
					case RichTextTagParser.TagType.Link:
					{
						RichTextTagParser.TagType tagType2 = RichTextTagParser.TagType.Link;
						RichTextTagParser.TagValue value4 = segment.tags[i].value;
						textSpan.linkID = RichTextTagParser.AddLink(tagType2, ((value4 != null) ? value4.StringValue : null) ?? "", links);
						break;
					}
					case RichTextTagParser.TagType.Lowercase:
					case RichTextTagParser.TagType.SmallCaps:
						textSpan.fontStyle |= FontStyles.LowerCase;
						break;
					case RichTextTagParser.TagType.Margin:
					case RichTextTagParser.TagType.MarginLeft:
					case RichTextTagParser.TagType.MarginRight:
					{
						float num2 = ((segment.tags[i].value.unit == RichTextTagParser.TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
						textSpan.margin = (int)(segment.tags[i].value.NumericalValue * num2);
						textSpan.marginUnitType = segment.tags[i].value.unit;
						RichTextTagParser.TagType tagType3 = segment.tags[i].tagType;
						if (!true)
						{
						}
						MarginDirection marginDirection;
						switch (tagType3)
						{
						case RichTextTagParser.TagType.Margin:
							marginDirection = MarginDirection.Both;
							break;
						case RichTextTagParser.TagType.MarginLeft:
							marginDirection = MarginDirection.Left;
							break;
						case RichTextTagParser.TagType.MarginRight:
							marginDirection = MarginDirection.Right;
							break;
						default:
							marginDirection = MarginDirection.Both;
							break;
						}
						if (!true)
						{
						}
						textSpan.marginDirection = marginDirection;
						break;
					}
					case RichTextTagParser.TagType.Mark:
					{
						textSpan.fontStyle |= FontStyles.Highlight;
						RichTextTagParser.TagValue value5 = segment.tags[i].value;
						bool flag5;
						if (value5 == null)
						{
							flag5 = false;
						}
						else
						{
							RichTextTagParser.ValueID? valueID = value5.ID;
							RichTextTagParser.ValueID valueID2 = RichTextTagParser.ValueID.Color;
							flag5 = (valueID.GetValueOrDefault() == valueID2) & (valueID != null);
						}
						bool flag6 = flag5;
						if (flag6)
						{
							textSpan.highlightColor = segment.tags[i].value.ColorValue;
						}
						else
						{
							textSpan.highlightColor = RichTextTagParser.k_HighlightColor;
						}
						RichTextTagParser.TagValue value6 = segment.tags[i].value2;
						bool flag7;
						if (value6 == null)
						{
							flag7 = false;
						}
						else
						{
							RichTextTagParser.ValueID? valueID = value6.ID;
							RichTextTagParser.ValueID valueID2 = RichTextTagParser.ValueID.Padding;
							flag7 = (valueID.GetValueOrDefault() == valueID2) & (valueID != null);
						}
						bool flag8 = flag7;
						if (flag8)
						{
							textSpan.highlightPadding = segment.tags[i].value2.Vector4Value;
						}
						break;
					}
					case RichTextTagParser.TagType.Mspace:
					{
						float num3 = ((segment.tags[i].value.unit == RichTextTagParser.TagUnitType.Pixels) ? (pixelsPerPoint * 64f) : 64f);
						textSpan.mspace = (int)(segment.tags[i].value.NumericalValue * num3);
						textSpan.mspaceUnitType = segment.tags[i].value.unit;
						break;
					}
					case RichTextTagParser.TagType.NoParse:
					case RichTextTagParser.TagType.Unknown:
						throw new InvalidOperationException("Invalid tag type" + segment.tags[i].tagType.ToString());
					case RichTextTagParser.TagType.Strikethrough:
						textSpan.fontStyle |= FontStyles.Strikethrough;
						break;
					case RichTextTagParser.TagType.Size:
					{
						float numericalValue = segment.tags[i].value.NumericalValue;
						RichTextTagParser.TagUnitType unit = segment.tags[i].value.unit;
						RichTextTagParser.TagValue value7 = segment.tags[i].value2;
						bool flag9 = value7 != null && value7.BoolValue;
						bool flag10 = flag9;
						if (flag10)
						{
							float num4 = (float)tgs.fontSize / 64f;
							float num5 = numericalValue * pixelsPerPoint;
							float num6 = num4 + num5;
							textSpan.fontSize = (int)Math.Round((double)(num6 * 64f), MidpointRounding.AwayFromZero);
						}
						else
						{
							bool flag11 = numericalValue <= 0f;
							if (flag11)
							{
								textSpan.fontSize = 0;
							}
							else
							{
								switch (unit)
								{
								case RichTextTagParser.TagUnitType.Pixels:
									goto IL_06B0;
								case RichTextTagParser.TagUnitType.FontUnits:
								{
									float num7 = (float)tgs.fontSize / 64f;
									float num8 = numericalValue * num7;
									textSpan.fontSize = (int)Math.Round((double)(num8 * 64f), MidpointRounding.AwayFromZero);
									break;
								}
								case RichTextTagParser.TagUnitType.Percentage:
								{
									float num9 = (float)tgs.fontSize / 64f;
									float num10 = numericalValue / 100f * num9;
									textSpan.fontSize = (int)Math.Round((double)(num10 * 64f), MidpointRounding.AwayFromZero);
									break;
								}
								default:
									goto IL_06B0;
								}
								break;
								IL_06B0:
								textSpan.fontSize = (int)Math.Round((double)(numericalValue * pixelsPerPoint * 64f), MidpointRounding.AwayFromZero);
							}
						}
						break;
					}
					case RichTextTagParser.TagType.Sprite:
					{
						RichTextTagParser.TagValue value8 = segment.tags[i].value;
						bool flag12;
						if (value8 == null)
						{
							flag12 = false;
						}
						else
						{
							RichTextTagParser.ValueID? valueID = value8.ID;
							RichTextTagParser.ValueID valueID2 = RichTextTagParser.ValueID.AssetID;
							flag12 = (valueID.GetValueOrDefault() == valueID2) & (valueID != null);
						}
						bool flag13 = flag12;
						if (flag13)
						{
							textSpan.spriteID = (int)segment.tags[i].value.NumericalValue;
						}
						RichTextTagParser.TagValue value9 = segment.tags[i].value2;
						bool flag14;
						if (value9 == null)
						{
							flag14 = false;
						}
						else
						{
							RichTextTagParser.ValueID? valueID = value9.ID;
							RichTextTagParser.ValueID valueID2 = RichTextTagParser.ValueID.GlyphMetrics;
							flag14 = (valueID.GetValueOrDefault() == valueID2) & (valueID != null);
						}
						bool flag15 = flag14;
						if (flag15)
						{
							textSpan.spriteMetrics = segment.tags[i].value2.GlyphMetricsValue;
						}
						RichTextTagParser.TagValue value10 = segment.tags[i].value3;
						bool flag16;
						if (value10 == null)
						{
							flag16 = false;
						}
						else
						{
							RichTextTagParser.ValueID? valueID = value10.ID;
							RichTextTagParser.ValueID valueID2 = RichTextTagParser.ValueID.Tint;
							flag16 = (valueID.GetValueOrDefault() == valueID2) & (valueID != null);
						}
						bool flag17 = flag16;
						if (flag17)
						{
							textSpan.spriteTint = segment.tags[i].value3.BoolValue;
						}
						RichTextTagParser.TagValue value11 = segment.tags[i].value4;
						bool flag18;
						if (value11 == null)
						{
							flag18 = false;
						}
						else
						{
							RichTextTagParser.ValueID? valueID = value11.ID;
							RichTextTagParser.ValueID valueID2 = RichTextTagParser.ValueID.Scale;
							flag18 = (valueID.GetValueOrDefault() == valueID2) & (valueID != null);
						}
						bool flag19 = flag18;
						if (flag19)
						{
							textSpan.spriteScale = (int)segment.tags[i].value4.NumericalValue;
						}
						RichTextTagParser.TagValue value12 = segment.tags[i].value5;
						bool flag20;
						if (value12 == null)
						{
							flag20 = false;
						}
						else
						{
							RichTextTagParser.ValueID? valueID = value12.ID;
							RichTextTagParser.ValueID valueID2 = RichTextTagParser.ValueID.SpriteColor;
							flag20 = (valueID.GetValueOrDefault() == valueID2) & (valueID != null);
						}
						bool flag21 = flag20;
						if (flag21)
						{
							textSpan.spriteColor = segment.tags[i].value5.ColorValue;
						}
						else
						{
							textSpan.spriteColor = Color.white;
						}
						break;
					}
					case RichTextTagParser.TagType.Subscript:
						textSpan.fontStyle |= FontStyles.Subscript;
						break;
					case RichTextTagParser.TagType.Superscript:
						textSpan.fontStyle |= FontStyles.Superscript;
						break;
					case RichTextTagParser.TagType.Underline:
						textSpan.fontStyle |= FontStyles.Underline;
						break;
					}
				}
				textSpan2 = textSpan;
			}
			return textSpan2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static void CreateTextGenerationSettingsArray(ref NativeTextGenerationSettings tgs, [global::System.Runtime.CompilerServices.Nullable(new byte[] { 1, 0, 1 })] List<ValueTuple<int, RichTextTagParser.TagType, string>> links, Color hyperlinkColor, float pixelsPerPoint, TextSettings textSettings)
		{
			links.Clear();
			List<RichTextTagParser.Tag> list = RichTextTagParser.FindTags(ref tgs.text, textSettings, false, null);
			RichTextTagParser.Segment[] array = RichTextTagParser.GenerateSegments(tgs.text, list);
			RichTextTagParser.ApplyStateToSegment(tgs.text, list, array);
			StringBuilder stringBuilder = new StringBuilder(tgs.text.Length);
			tgs.textSpans = new TextSpan[array.Length];
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				RichTextTagParser.Segment segment = array[i];
				string text = tgs.text.Substring(segment.start, segment.end + 1 - segment.start);
				TextSpan textSpan = RichTextTagParser.CreateTextSpan(segment, ref tgs, links, hyperlinkColor, pixelsPerPoint);
				textSpan.startIndex = num;
				textSpan.length = text.Length;
				tgs.textSpans[i] = textSpan;
				stringBuilder.Append(text);
				num += text.Length;
			}
			tgs.text = stringBuilder.ToString();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static bool MayNeedParsing(string text)
		{
			bool flag = string.IsNullOrEmpty(text);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ReadOnlySpan<char> readOnlySpan = text.AsSpan();
				int num = readOnlySpan.IndexOf('<');
				bool flag3 = num < 0 || num >= readOnlySpan.Length - 1;
				flag2 = !flag3 && readOnlySpan.Slice(num + 1).IndexOf('>') >= 0;
			}
			return flag2;
		}

		private unsafe static bool ContainsFontTag(string text)
		{
			bool flag = string.IsNullOrEmpty(text);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ReadOnlySpan<char> readOnlySpan = text.AsSpan();
				ReadOnlySpan<char> readOnlySpan2 = "<font=".AsSpan();
				int num = readOnlySpan.IndexOf(readOnlySpan2, StringComparison.Ordinal);
				bool flag3 = num < 0;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					int num2 = num + readOnlySpan2.Length;
					for (int i = num2; i < readOnlySpan.Length; i++)
					{
						bool flag4 = *readOnlySpan[i] == 62;
						if (flag4)
						{
							return true;
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal unsafe static bool ContainsSpriteTag(string text)
		{
			bool flag = string.IsNullOrEmpty(text);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ReadOnlySpan<char> readOnlySpan = text.AsSpan();
				ReadOnlySpan<char> readOnlySpan2 = "<sprite".AsSpan();
				int num = readOnlySpan.IndexOf(readOnlySpan2, StringComparison.Ordinal);
				bool flag3 = num < 0;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					int num2 = num + readOnlySpan2.Length;
					for (int i = num2; i < readOnlySpan.Length; i++)
					{
						bool flag4 = *readOnlySpan[i] == 62;
						if (flag4)
						{
							return true;
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		internal unsafe static bool ContainsStyleTags(string text)
		{
			bool flag = string.IsNullOrEmpty(text);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ReadOnlySpan<char> readOnlySpan = text.AsSpan();
				ReadOnlySpan<char> readOnlySpan2 = "<style=\"".AsSpan();
				int num = readOnlySpan.IndexOf(readOnlySpan2, StringComparison.Ordinal);
				bool flag3 = num < 0;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					int num2 = num + readOnlySpan2.Length;
					for (int i = num2; i < readOnlySpan.Length; i++)
					{
						bool flag4 = *readOnlySpan[i] == 62;
						if (flag4)
						{
							return true;
						}
					}
					flag2 = false;
				}
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule" })]
		internal static bool HasFontTags(string text, TextSettings textSettings, out List<string> fontAssetNames)
		{
			fontAssetNames = new List<string>();
			bool flag = !RichTextTagParser.ContainsFontTag(text);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				List<RichTextTagParser.Tag> list = RichTextTagParser.FindTags(ref text, textSettings, true, null);
				foreach (RichTextTagParser.Tag tag in list)
				{
					bool flag3;
					if (tag.tagType == RichTextTagParser.TagType.Font && !tag.isClosing)
					{
						RichTextTagParser.TagValue value = tag.value;
						flag3 = ((value != null) ? value.StringValue : null) != null;
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						string stringValue = tag.value.StringValue;
						bool flag5 = !fontAssetNames.Contains(stringValue);
						if (flag5)
						{
							fontAssetNames.Add(stringValue);
						}
					}
				}
				flag2 = fontAssetNames.Count > 0;
			}
			return flag2;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule" })]
		internal static bool HasSpriteTags(string text, TextSettings textSettings, out List<string> spriteAssetNames)
		{
			spriteAssetNames = new List<string>();
			bool flag = !RichTextTagParser.ContainsSpriteTag(text);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				List<RichTextTagParser.Tag> list = RichTextTagParser.FindTags(ref text, textSettings, true, null);
				foreach (RichTextTagParser.Tag tag in list)
				{
					bool flag3 = tag.tagType == RichTextTagParser.TagType.Sprite && !tag.isClosing;
					if (flag3)
					{
						RichTextTagParser.TagValue value = tag.value;
						bool flag4 = value != null && value.type == RichTextTagParser.TagValueType.StringValue;
						if (flag4)
						{
							string stringValue = tag.value.StringValue;
							bool flag5 = !string.IsNullOrEmpty(stringValue) && !spriteAssetNames.Contains(stringValue);
							if (flag5)
							{
								spriteAssetNames.Add(stringValue);
							}
						}
					}
				}
				flag2 = spriteAssetNames.Count > 0;
			}
			return flag2;
		}

		internal static readonly Color32 k_HighlightColor = new Color32(byte.MaxValue, byte.MaxValue, 0, 64);

		internal static readonly char k_PrivateArea = '\ue000';

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal static readonly Dictionary<string, IntPtr> s_FontAssetCache = new Dictionary<string, IntPtr>();

		internal static readonly Dictionary<string, WeakReference<SpriteAsset>> s_SpriteAssetCache = new Dictionary<string, WeakReference<SpriteAsset>>();

		internal static readonly RichTextTagParser.TagTypeInfo[] TagsInfo = new RichTextTagParser.TagTypeInfo[]
		{
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Hyperlink, "a", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Align, "align", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.AllCaps, "allcaps", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Alpha, "alpha", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Bold, "b", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Br, "br", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Color, "color", RichTextTagParser.TagValueType.ColorValue, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.CSpace, "cspace", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Font, "font", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.FontWeight, "font-weight", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Italic, "i", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Indent, "indent", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.LineHeight, "line-height", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.LineIndent, "line-indent", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Link, "link", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Lowercase, "lowercase", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Margin, "margin", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.MarginLeft, "margin-left", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.MarginRight, "margin-right", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Mark, "mark", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Mspace, "mspace", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.NoBr, "nobr", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.NoParse, "noparse", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Strikethrough, "s", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Size, "size", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.SmallCaps, "smallcaps", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Space, "space", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Sprite, "sprite", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Style, "style", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Subscript, "sub", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Superscript, "sup", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Underline, "u", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown),
			new RichTextTagParser.TagTypeInfo(RichTextTagParser.TagType.Uppercase, "uppercase", RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType.Unknown)
		};

		private const string k_FontTag = "<font=";

		private const string k_SpriteTag = "<sprite";

		private const string k_StyleTag = "<style=\"";

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		public enum TagType
		{
			Hyperlink,
			Align,
			AllCaps,
			Alpha,
			Bold,
			Br,
			Color,
			CSpace,
			Font,
			FontWeight,
			Italic,
			Indent,
			LineHeight,
			LineIndent,
			Link,
			Lowercase,
			Margin,
			MarginLeft,
			MarginRight,
			Mark,
			Mspace,
			NoBr,
			NoParse,
			Strikethrough,
			Size,
			SmallCaps,
			Space,
			Sprite,
			Style,
			Subscript,
			Superscript,
			Underline,
			Uppercase,
			Unknown
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		public enum ValueID
		{
			Color,
			Padding,
			AssetID,
			GlyphMetrics,
			Scale,
			Tint,
			SpriteColor
		}

		[global::System.Runtime.CompilerServices.Nullable(0)]
		internal class TagTypeInfo : IEquatable<RichTextTagParser.TagTypeInfo>
		{
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(RichTextTagParser.TagTypeInfo);
				}
			}

			internal TagTypeInfo(RichTextTagParser.TagType tagType, string name, RichTextTagParser.TagValueType valueType = RichTextTagParser.TagValueType.None, RichTextTagParser.TagUnitType unitType = RichTextTagParser.TagUnitType.Unknown)
			{
				this.TagType = tagType;
				this.name = name;
				this.valueType = valueType;
				this.unitType = unitType;
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("TagTypeInfo");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("TagType = ");
				builder.Append(this.TagType.ToString());
				builder.Append(", name = ");
				builder.Append(this.name);
				builder.Append(", valueType = ");
				builder.Append(this.valueType.ToString());
				builder.Append(", unitType = ");
				builder.Append(this.unitType.ToString());
				return true;
			}

			[global::System.Runtime.CompilerServices.NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(RichTextTagParser.TagTypeInfo left, RichTextTagParser.TagTypeInfo right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			[global::System.Runtime.CompilerServices.NullableContext(2)]
			public static bool operator ==(RichTextTagParser.TagTypeInfo left, RichTextTagParser.TagTypeInfo right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<RichTextTagParser.TagType>.Default.GetHashCode(this.TagType)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.name)) * -1521134295 + EqualityComparer<RichTextTagParser.TagValueType>.Default.GetHashCode(this.valueType)) * -1521134295 + EqualityComparer<RichTextTagParser.TagUnitType>.Default.GetHashCode(this.unitType);
			}

			[global::System.Runtime.CompilerServices.NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as RichTextTagParser.TagTypeInfo);
			}

			[CompilerGenerated]
			[global::System.Runtime.CompilerServices.NullableContext(2)]
			public virtual bool Equals(RichTextTagParser.TagTypeInfo other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<RichTextTagParser.TagType>.Default.Equals(this.TagType, other.TagType) && EqualityComparer<string>.Default.Equals(this.name, other.name) && EqualityComparer<RichTextTagParser.TagValueType>.Default.Equals(this.valueType, other.valueType) && EqualityComparer<RichTextTagParser.TagUnitType>.Default.Equals(this.unitType, other.unitType));
			}

			[CompilerGenerated]
			protected TagTypeInfo(RichTextTagParser.TagTypeInfo original)
			{
				this.TagType = original.TagType;
				this.name = original.name;
				this.valueType = original.valueType;
				this.unitType = original.unitType;
			}

			public RichTextTagParser.TagType TagType;

			public string name;

			public RichTextTagParser.TagValueType valueType;

			public RichTextTagParser.TagUnitType unitType;
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		internal enum TagValueType
		{
			None,
			NumericalValue,
			StringValue,
			ColorValue,
			Vector4Value,
			GlyphMetricsValue,
			BoolValue
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		internal enum TagUnitType
		{
			Unknown,
			Pixels,
			FontUnits,
			Percentage
		}

		[global::System.Runtime.CompilerServices.Nullable(0)]
		[global::System.Runtime.CompilerServices.NullableContext(2)]
		internal class TagValue : IEquatable<RichTextTagParser.TagValue>
		{
			[global::System.Runtime.CompilerServices.Nullable(1)]
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[global::System.Runtime.CompilerServices.NullableContext(1)]
				[CompilerGenerated]
				get
				{
					return typeof(RichTextTagParser.TagValue);
				}
			}

			internal TagValue(float value, RichTextTagParser.TagUnitType tagUnitType = RichTextTagParser.TagUnitType.Unknown, RichTextTagParser.ValueID? id = null)
			{
				this.type = RichTextTagParser.TagValueType.NumericalValue;
				this.unit = tagUnitType;
				this.m_numericalValue = value;
				this.m_ID = id;
			}

			internal TagValue(Color value, RichTextTagParser.ValueID? id = null)
			{
				this.type = RichTextTagParser.TagValueType.ColorValue;
				this.m_colorValue = value;
				this.m_ID = id;
			}

			[global::System.Runtime.CompilerServices.NullableContext(1)]
			internal TagValue(string value, RichTextTagParser.ValueID? id = null)
			{
				this.type = RichTextTagParser.TagValueType.StringValue;
				this.m_stringValue = value;
				this.m_ID = id;
			}

			internal TagValue(Vector4 value, RichTextTagParser.ValueID? id = null)
			{
				this.type = RichTextTagParser.TagValueType.Vector4Value;
				this.m_vector4Value = value;
				this.m_ID = id;
			}

			internal TagValue(GlyphMetrics value, RichTextTagParser.ValueID? id = null)
			{
				this.type = RichTextTagParser.TagValueType.GlyphMetricsValue;
				this.m_glyphMetricsValue = value;
				this.m_ID = id;
			}

			internal TagValue(bool value, RichTextTagParser.ValueID? id = null)
			{
				this.type = RichTextTagParser.TagValueType.BoolValue;
				this.m_boolValue = value;
				this.m_ID = id;
			}

			internal string StringValue
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.StringValue;
					if (flag)
					{
						throw new InvalidOperationException("Not a string value");
					}
					return this.m_stringValue;
				}
			}

			internal float NumericalValue
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.NumericalValue;
					if (flag)
					{
						throw new InvalidOperationException("Not a numerical value");
					}
					return this.m_numericalValue;
				}
			}

			internal Color ColorValue
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.ColorValue;
					if (flag)
					{
						throw new InvalidOperationException("Not a color value");
					}
					return this.m_colorValue;
				}
			}

			internal Vector4 Vector4Value
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.Vector4Value;
					if (flag)
					{
						throw new InvalidOperationException("Not a vector4 value");
					}
					return this.m_vector4Value;
				}
			}

			internal GlyphMetrics GlyphMetricsValue
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.GlyphMetricsValue;
					if (flag)
					{
						throw new InvalidOperationException("Not a GlyphMetrics value");
					}
					return this.m_glyphMetricsValue;
				}
			}

			internal bool BoolValue
			{
				get
				{
					bool flag = this.type != RichTextTagParser.TagValueType.BoolValue;
					if (flag)
					{
						throw new InvalidOperationException("Not a Bool value");
					}
					return this.m_boolValue;
				}
			}

			internal RichTextTagParser.ValueID? ID
			{
				get
				{
					return this.m_ID;
				}
			}

			[CompilerGenerated]
			[global::System.Runtime.CompilerServices.NullableContext(1)]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("TagValue");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			[global::System.Runtime.CompilerServices.NullableContext(1)]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				return false;
			}

			[CompilerGenerated]
			public static bool operator !=(RichTextTagParser.TagValue left, RichTextTagParser.TagValue right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(RichTextTagParser.TagValue left, RichTextTagParser.TagValue right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return ((((((((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<RichTextTagParser.TagValueType>.Default.GetHashCode(this.type)) * -1521134295 + EqualityComparer<RichTextTagParser.TagUnitType>.Default.GetHashCode(this.unit)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.m_stringValue)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.m_numericalValue)) * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(this.m_colorValue)) * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(this.m_vector4Value)) * -1521134295 + EqualityComparer<GlyphMetrics>.Default.GetHashCode(this.m_glyphMetricsValue)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.m_boolValue)) * -1521134295 + EqualityComparer<RichTextTagParser.ValueID?>.Default.GetHashCode(this.m_ID);
			}

			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as RichTextTagParser.TagValue);
			}

			[CompilerGenerated]
			public virtual bool Equals(RichTextTagParser.TagValue other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<RichTextTagParser.TagValueType>.Default.Equals(this.type, other.type) && EqualityComparer<RichTextTagParser.TagUnitType>.Default.Equals(this.unit, other.unit) && EqualityComparer<string>.Default.Equals(this.m_stringValue, other.m_stringValue) && EqualityComparer<float>.Default.Equals(this.m_numericalValue, other.m_numericalValue) && EqualityComparer<Color>.Default.Equals(this.m_colorValue, other.m_colorValue) && EqualityComparer<Vector4>.Default.Equals(this.m_vector4Value, other.m_vector4Value) && EqualityComparer<GlyphMetrics>.Default.Equals(this.m_glyphMetricsValue, other.m_glyphMetricsValue) && EqualityComparer<bool>.Default.Equals(this.m_boolValue, other.m_boolValue) && EqualityComparer<RichTextTagParser.ValueID?>.Default.Equals(this.m_ID, other.m_ID));
			}

			[CompilerGenerated]
			protected TagValue([global::System.Runtime.CompilerServices.Nullable(1)] RichTextTagParser.TagValue original)
			{
				this.type = original.type;
				this.unit = original.unit;
				this.m_stringValue = original.m_stringValue;
				this.m_numericalValue = original.m_numericalValue;
				this.m_colorValue = original.m_colorValue;
				this.m_vector4Value = original.m_vector4Value;
				this.m_glyphMetricsValue = original.m_glyphMetricsValue;
				this.m_boolValue = original.m_boolValue;
				this.m_ID = original.m_ID;
			}

			internal RichTextTagParser.TagValueType type;

			internal RichTextTagParser.TagUnitType unit;

			private string m_stringValue;

			private float m_numericalValue;

			private Color m_colorValue;

			private Vector4 m_vector4Value;

			private GlyphMetrics m_glyphMetricsValue;

			private bool m_boolValue;

			private RichTextTagParser.ValueID? m_ID;
		}

		[global::System.Runtime.CompilerServices.NullableContext(2)]
		[global::System.Runtime.CompilerServices.Nullable(0)]
		internal struct Tag
		{
			public RichTextTagParser.TagType tagType;

			public bool isClosing;

			public int start;

			public int end;

			public RichTextTagParser.TagValue value;

			public RichTextTagParser.TagValue value2;

			public RichTextTagParser.TagValue value3;

			public RichTextTagParser.TagValue value4;

			public RichTextTagParser.TagValue value5;
		}

		[global::System.Runtime.CompilerServices.NullableContext(0)]
		public struct Segment
		{
			[global::System.Runtime.CompilerServices.Nullable(2)]
			public List<RichTextTagParser.Tag> tags;

			public int start;

			public int end;
		}

		[global::System.Runtime.CompilerServices.Nullable(0)]
		internal class ParseError : IEquatable<RichTextTagParser.ParseError>
		{
			[CompilerGenerated]
			protected virtual Type EqualityContract
			{
				[CompilerGenerated]
				get
				{
					return typeof(RichTextTagParser.ParseError);
				}
			}

			internal ParseError(string message, int position)
			{
				this.message = message;
				this.position = position;
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("ParseError");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			protected virtual bool PrintMembers(StringBuilder builder)
			{
				RuntimeHelpers.EnsureSufficientExecutionStack();
				builder.Append("position = ");
				builder.Append(this.position.ToString());
				builder.Append(", message = ");
				builder.Append(this.message);
				return true;
			}

			[global::System.Runtime.CompilerServices.NullableContext(2)]
			[CompilerGenerated]
			public static bool operator !=(RichTextTagParser.ParseError left, RichTextTagParser.ParseError right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			[global::System.Runtime.CompilerServices.NullableContext(2)]
			public static bool operator ==(RichTextTagParser.ParseError left, RichTextTagParser.ParseError right)
			{
				return left == right || (left != null && left.Equals(right));
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.position)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.message);
			}

			[global::System.Runtime.CompilerServices.NullableContext(2)]
			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				return this.Equals(obj as RichTextTagParser.ParseError);
			}

			[CompilerGenerated]
			[global::System.Runtime.CompilerServices.NullableContext(2)]
			public virtual bool Equals(RichTextTagParser.ParseError other)
			{
				return this == other || (other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(this.position, other.position) && EqualityComparer<string>.Default.Equals(this.message, other.message));
			}

			[CompilerGenerated]
			protected ParseError(RichTextTagParser.ParseError original)
			{
				this.position = original.position;
				this.message = original.message;
			}

			public readonly int position;

			public readonly string message;
		}
	}
}
