using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.StyleSheets
{
	internal static class StylePropertyUtil
	{
		public static int GetEnumIntValue(StyleEnumType enumType, string value)
		{
			int num;
			switch (enumType)
			{
			case StyleEnumType.Align:
			{
				bool flag = string.Equals(value, "auto", StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					num = 0;
				}
				else
				{
					bool flag2 = string.Equals(value, "flex-start", StringComparison.OrdinalIgnoreCase);
					if (flag2)
					{
						num = 1;
					}
					else
					{
						bool flag3 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
						if (flag3)
						{
							num = 2;
						}
						else
						{
							bool flag4 = string.Equals(value, "flex-end", StringComparison.OrdinalIgnoreCase);
							if (flag4)
							{
								num = 3;
							}
							else
							{
								bool flag5 = string.Equals(value, "stretch", StringComparison.OrdinalIgnoreCase);
								if (flag5)
								{
									num = 4;
								}
								else
								{
									num = 0;
								}
							}
						}
					}
				}
				break;
			}
			case StyleEnumType.DisplayStyle:
			{
				bool flag6 = string.Equals(value, "flex", StringComparison.OrdinalIgnoreCase);
				if (flag6)
				{
					num = 0;
				}
				else
				{
					bool flag7 = string.Equals(value, "none", StringComparison.OrdinalIgnoreCase);
					if (flag7)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
				}
				break;
			}
			case StyleEnumType.FlexDirection:
			{
				bool flag8 = string.Equals(value, "column", StringComparison.OrdinalIgnoreCase);
				if (flag8)
				{
					num = 0;
				}
				else
				{
					bool flag9 = string.Equals(value, "column-reverse", StringComparison.OrdinalIgnoreCase);
					if (flag9)
					{
						num = 1;
					}
					else
					{
						bool flag10 = string.Equals(value, "row", StringComparison.OrdinalIgnoreCase);
						if (flag10)
						{
							num = 2;
						}
						else
						{
							bool flag11 = string.Equals(value, "row-reverse", StringComparison.OrdinalIgnoreCase);
							if (flag11)
							{
								num = 3;
							}
							else
							{
								num = 0;
							}
						}
					}
				}
				break;
			}
			case StyleEnumType.FontStyle:
			{
				bool flag12 = string.Equals(value, "normal", StringComparison.OrdinalIgnoreCase);
				if (flag12)
				{
					num = 0;
				}
				else
				{
					bool flag13 = string.Equals(value, "bold", StringComparison.OrdinalIgnoreCase);
					if (flag13)
					{
						num = 1;
					}
					else
					{
						bool flag14 = string.Equals(value, "italic", StringComparison.OrdinalIgnoreCase);
						if (flag14)
						{
							num = 2;
						}
						else
						{
							bool flag15 = string.Equals(value, "bold-and-italic", StringComparison.OrdinalIgnoreCase);
							if (flag15)
							{
								num = 3;
							}
							else
							{
								num = 0;
							}
						}
					}
				}
				break;
			}
			case StyleEnumType.Justify:
			{
				bool flag16 = string.Equals(value, "flex-start", StringComparison.OrdinalIgnoreCase);
				if (flag16)
				{
					num = 0;
				}
				else
				{
					bool flag17 = string.Equals(value, "center", StringComparison.OrdinalIgnoreCase);
					if (flag17)
					{
						num = 1;
					}
					else
					{
						bool flag18 = string.Equals(value, "flex-end", StringComparison.OrdinalIgnoreCase);
						if (flag18)
						{
							num = 2;
						}
						else
						{
							bool flag19 = string.Equals(value, "space-between", StringComparison.OrdinalIgnoreCase);
							if (flag19)
							{
								num = 3;
							}
							else
							{
								bool flag20 = string.Equals(value, "space-around", StringComparison.OrdinalIgnoreCase);
								if (flag20)
								{
									num = 4;
								}
								else
								{
									num = 0;
								}
							}
						}
					}
				}
				break;
			}
			case StyleEnumType.Overflow:
			{
				bool flag21 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag21)
				{
					num = 0;
				}
				else
				{
					bool flag22 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
					if (flag22)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
				}
				break;
			}
			case StyleEnumType.OverflowClipBox:
			{
				bool flag23 = string.Equals(value, "padding-box", StringComparison.OrdinalIgnoreCase);
				if (flag23)
				{
					num = 0;
				}
				else
				{
					bool flag24 = string.Equals(value, "content-box", StringComparison.OrdinalIgnoreCase);
					if (flag24)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
				}
				break;
			}
			case StyleEnumType.OverflowInternal:
			{
				bool flag25 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag25)
				{
					num = 0;
				}
				else
				{
					bool flag26 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
					if (flag26)
					{
						num = 1;
					}
					else
					{
						bool flag27 = string.Equals(value, "scroll", StringComparison.OrdinalIgnoreCase);
						if (flag27)
						{
							num = 2;
						}
						else
						{
							num = 0;
						}
					}
				}
				break;
			}
			case StyleEnumType.Position:
			{
				bool flag28 = string.Equals(value, "relative", StringComparison.OrdinalIgnoreCase);
				if (flag28)
				{
					num = 0;
				}
				else
				{
					bool flag29 = string.Equals(value, "absolute", StringComparison.OrdinalIgnoreCase);
					if (flag29)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
				}
				break;
			}
			case StyleEnumType.ScaleMode:
			{
				bool flag30 = string.Equals(value, "stretch-to-fill", StringComparison.OrdinalIgnoreCase);
				if (flag30)
				{
					num = 0;
				}
				else
				{
					bool flag31 = string.Equals(value, "scale-and-crop", StringComparison.OrdinalIgnoreCase);
					if (flag31)
					{
						num = 1;
					}
					else
					{
						bool flag32 = string.Equals(value, "scale-to-fit", StringComparison.OrdinalIgnoreCase);
						if (flag32)
						{
							num = 2;
						}
						else
						{
							num = 0;
						}
					}
				}
				break;
			}
			case StyleEnumType.TextAnchor:
			{
				bool flag33 = string.Equals(value, "upper-left", StringComparison.OrdinalIgnoreCase);
				if (flag33)
				{
					num = 0;
				}
				else
				{
					bool flag34 = string.Equals(value, "upper-center", StringComparison.OrdinalIgnoreCase);
					if (flag34)
					{
						num = 1;
					}
					else
					{
						bool flag35 = string.Equals(value, "upper-right", StringComparison.OrdinalIgnoreCase);
						if (flag35)
						{
							num = 2;
						}
						else
						{
							bool flag36 = string.Equals(value, "middle-left", StringComparison.OrdinalIgnoreCase);
							if (flag36)
							{
								num = 3;
							}
							else
							{
								bool flag37 = string.Equals(value, "middle-center", StringComparison.OrdinalIgnoreCase);
								if (flag37)
								{
									num = 4;
								}
								else
								{
									bool flag38 = string.Equals(value, "middle-right", StringComparison.OrdinalIgnoreCase);
									if (flag38)
									{
										num = 5;
									}
									else
									{
										bool flag39 = string.Equals(value, "lower-left", StringComparison.OrdinalIgnoreCase);
										if (flag39)
										{
											num = 6;
										}
										else
										{
											bool flag40 = string.Equals(value, "lower-center", StringComparison.OrdinalIgnoreCase);
											if (flag40)
											{
												num = 7;
											}
											else
											{
												bool flag41 = string.Equals(value, "lower-right", StringComparison.OrdinalIgnoreCase);
												if (flag41)
												{
													num = 8;
												}
												else
												{
													num = 0;
												}
											}
										}
									}
								}
							}
						}
					}
				}
				break;
			}
			case StyleEnumType.TextOverflow:
			{
				bool flag42 = string.Equals(value, "clip", StringComparison.OrdinalIgnoreCase);
				if (flag42)
				{
					num = 0;
				}
				else
				{
					bool flag43 = string.Equals(value, "ellipsis", StringComparison.OrdinalIgnoreCase);
					if (flag43)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
				}
				break;
			}
			case StyleEnumType.TextOverflowPosition:
			{
				bool flag44 = string.Equals(value, "start", StringComparison.OrdinalIgnoreCase);
				if (flag44)
				{
					num = 1;
				}
				else
				{
					bool flag45 = string.Equals(value, "middle", StringComparison.OrdinalIgnoreCase);
					if (flag45)
					{
						num = 2;
					}
					else
					{
						bool flag46 = string.Equals(value, "end", StringComparison.OrdinalIgnoreCase);
						if (flag46)
						{
							num = 0;
						}
						else
						{
							num = 0;
						}
					}
				}
				break;
			}
			case StyleEnumType.Visibility:
			{
				bool flag47 = string.Equals(value, "visible", StringComparison.OrdinalIgnoreCase);
				if (flag47)
				{
					num = 0;
				}
				else
				{
					bool flag48 = string.Equals(value, "hidden", StringComparison.OrdinalIgnoreCase);
					if (flag48)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
				}
				break;
			}
			case StyleEnumType.WhiteSpace:
			{
				bool flag49 = string.Equals(value, "normal", StringComparison.OrdinalIgnoreCase);
				if (flag49)
				{
					num = 0;
				}
				else
				{
					bool flag50 = string.Equals(value, "nowrap", StringComparison.OrdinalIgnoreCase);
					if (flag50)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
				}
				break;
			}
			case StyleEnumType.Wrap:
			{
				bool flag51 = string.Equals(value, "nowrap", StringComparison.OrdinalIgnoreCase);
				if (flag51)
				{
					num = 0;
				}
				else
				{
					bool flag52 = string.Equals(value, "wrap", StringComparison.OrdinalIgnoreCase);
					if (flag52)
					{
						num = 1;
					}
					else
					{
						bool flag53 = string.Equals(value, "wrap-reverse", StringComparison.OrdinalIgnoreCase);
						if (flag53)
						{
							num = 2;
						}
						else
						{
							num = 0;
						}
					}
				}
				break;
			}
			default:
				num = 0;
				break;
			}
			return num;
		}

		public const int k_GroupOffset = 16;

		internal static readonly Dictionary<string, StylePropertyId> s_NameToId = new Dictionary<string, StylePropertyId>
		{
			{
				"align-content",
				StylePropertyId.AlignContent
			},
			{
				"align-items",
				StylePropertyId.AlignItems
			},
			{
				"align-self",
				StylePropertyId.AlignSelf
			},
			{
				"background-color",
				StylePropertyId.BackgroundColor
			},
			{
				"background-image",
				StylePropertyId.BackgroundImage
			},
			{
				"border-bottom-color",
				StylePropertyId.BorderBottomColor
			},
			{
				"border-bottom-left-radius",
				StylePropertyId.BorderBottomLeftRadius
			},
			{
				"border-bottom-right-radius",
				StylePropertyId.BorderBottomRightRadius
			},
			{
				"border-bottom-width",
				StylePropertyId.BorderBottomWidth
			},
			{
				"border-color",
				StylePropertyId.BorderColor
			},
			{
				"border-left-color",
				StylePropertyId.BorderLeftColor
			},
			{
				"border-left-width",
				StylePropertyId.BorderLeftWidth
			},
			{
				"border-radius",
				StylePropertyId.BorderRadius
			},
			{
				"border-right-color",
				StylePropertyId.BorderRightColor
			},
			{
				"border-right-width",
				StylePropertyId.BorderRightWidth
			},
			{
				"border-top-color",
				StylePropertyId.BorderTopColor
			},
			{
				"border-top-left-radius",
				StylePropertyId.BorderTopLeftRadius
			},
			{
				"border-top-right-radius",
				StylePropertyId.BorderTopRightRadius
			},
			{
				"border-top-width",
				StylePropertyId.BorderTopWidth
			},
			{
				"border-width",
				StylePropertyId.BorderWidth
			},
			{
				"bottom",
				StylePropertyId.Bottom
			},
			{
				"color",
				StylePropertyId.Color
			},
			{
				"cursor",
				StylePropertyId.Cursor
			},
			{
				"display",
				StylePropertyId.Display
			},
			{
				"flex",
				StylePropertyId.Flex
			},
			{
				"flex-basis",
				StylePropertyId.FlexBasis
			},
			{
				"flex-direction",
				StylePropertyId.FlexDirection
			},
			{
				"flex-grow",
				StylePropertyId.FlexGrow
			},
			{
				"flex-shrink",
				StylePropertyId.FlexShrink
			},
			{
				"flex-wrap",
				StylePropertyId.FlexWrap
			},
			{
				"font-size",
				StylePropertyId.FontSize
			},
			{
				"height",
				StylePropertyId.Height
			},
			{
				"justify-content",
				StylePropertyId.JustifyContent
			},
			{
				"left",
				StylePropertyId.Left
			},
			{
				"margin",
				StylePropertyId.Margin
			},
			{
				"margin-bottom",
				StylePropertyId.MarginBottom
			},
			{
				"margin-left",
				StylePropertyId.MarginLeft
			},
			{
				"margin-right",
				StylePropertyId.MarginRight
			},
			{
				"margin-top",
				StylePropertyId.MarginTop
			},
			{
				"max-height",
				StylePropertyId.MaxHeight
			},
			{
				"max-width",
				StylePropertyId.MaxWidth
			},
			{
				"min-height",
				StylePropertyId.MinHeight
			},
			{
				"min-width",
				StylePropertyId.MinWidth
			},
			{
				"opacity",
				StylePropertyId.Opacity
			},
			{
				"overflow",
				StylePropertyId.Overflow
			},
			{
				"padding",
				StylePropertyId.Padding
			},
			{
				"padding-bottom",
				StylePropertyId.PaddingBottom
			},
			{
				"padding-left",
				StylePropertyId.PaddingLeft
			},
			{
				"padding-right",
				StylePropertyId.PaddingRight
			},
			{
				"padding-top",
				StylePropertyId.PaddingTop
			},
			{
				"position",
				StylePropertyId.Position
			},
			{
				"right",
				StylePropertyId.Right
			},
			{
				"text-overflow",
				StylePropertyId.TextOverflow
			},
			{
				"top",
				StylePropertyId.Top
			},
			{
				"-unity-background-image-tint-color",
				StylePropertyId.UnityBackgroundImageTintColor
			},
			{
				"-unity-background-scale-mode",
				StylePropertyId.UnityBackgroundScaleMode
			},
			{
				"-unity-font",
				StylePropertyId.UnityFont
			},
			{
				"-unity-font-style",
				StylePropertyId.UnityFontStyleAndWeight
			},
			{
				"-unity-overflow-clip-box",
				StylePropertyId.UnityOverflowClipBox
			},
			{
				"-unity-slice-bottom",
				StylePropertyId.UnitySliceBottom
			},
			{
				"-unity-slice-left",
				StylePropertyId.UnitySliceLeft
			},
			{
				"-unity-slice-right",
				StylePropertyId.UnitySliceRight
			},
			{
				"-unity-slice-top",
				StylePropertyId.UnitySliceTop
			},
			{
				"-unity-text-align",
				StylePropertyId.UnityTextAlign
			},
			{
				"-unity-text-overflow-position",
				StylePropertyId.UnityTextOverflowPosition
			},
			{
				"visibility",
				StylePropertyId.Visibility
			},
			{
				"white-space",
				StylePropertyId.WhiteSpace
			},
			{
				"width",
				StylePropertyId.Width
			}
		};
	}
}
