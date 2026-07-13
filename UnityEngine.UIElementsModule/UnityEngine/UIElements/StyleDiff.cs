using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal sealed class StyleDiff : INotifyBindablePropertyChanged, IDataSourceViewHashProvider, IDisposable
	{
		[CreateProperty]
		public StylePropertyData<StyleEnum<Align>, Align> alignContent
		{
			get
			{
				return this.m_AlignContent;
			}
			private set
			{
				this.m_AlignContent.target = value.target;
				bool flag = this.m_AlignContent == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_AlignContent.Dispose();
					this.m_AlignContent = value;
					this.Notify("alignContent");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<Align>, Align> alignItems
		{
			get
			{
				return this.m_AlignItems;
			}
			private set
			{
				this.m_AlignItems.target = value.target;
				bool flag = this.m_AlignItems == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_AlignItems.Dispose();
					this.m_AlignItems = value;
					this.Notify("alignItems");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<Align>, Align> alignSelf
		{
			get
			{
				return this.m_AlignSelf;
			}
			private set
			{
				this.m_AlignSelf.target = value.target;
				bool flag = this.m_AlignSelf == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_AlignSelf.Dispose();
					this.m_AlignSelf = value;
					this.Notify("alignSelf");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleRatio, Ratio> aspectRatio
		{
			get
			{
				return this.m_AspectRatio;
			}
			private set
			{
				this.m_AspectRatio.target = value.target;
				bool flag = this.m_AspectRatio == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_AspectRatio.Dispose();
					this.m_AspectRatio = value;
					this.Notify("aspectRatio");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleColor, Color> backgroundColor
		{
			get
			{
				return this.m_BackgroundColor;
			}
			private set
			{
				this.m_BackgroundColor.target = value.target;
				bool flag = this.m_BackgroundColor == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BackgroundColor.Dispose();
					this.m_BackgroundColor = value;
					this.Notify("backgroundColor");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleBackground, Background> backgroundImage
		{
			get
			{
				return this.m_BackgroundImage;
			}
			private set
			{
				this.m_BackgroundImage.target = value.target;
				bool flag = this.m_BackgroundImage == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BackgroundImage.Dispose();
					this.m_BackgroundImage = value;
					this.Notify("backgroundImage");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleBackgroundPosition, BackgroundPosition> backgroundPositionX
		{
			get
			{
				return this.m_BackgroundPositionX;
			}
			private set
			{
				this.m_BackgroundPositionX.target = value.target;
				bool flag = this.m_BackgroundPositionX == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BackgroundPositionX.Dispose();
					this.m_BackgroundPositionX = value;
					this.Notify("backgroundPositionX");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleBackgroundPosition, BackgroundPosition> backgroundPositionY
		{
			get
			{
				return this.m_BackgroundPositionY;
			}
			private set
			{
				this.m_BackgroundPositionY.target = value.target;
				bool flag = this.m_BackgroundPositionY == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BackgroundPositionY.Dispose();
					this.m_BackgroundPositionY = value;
					this.Notify("backgroundPositionY");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleBackgroundRepeat, BackgroundRepeat> backgroundRepeat
		{
			get
			{
				return this.m_BackgroundRepeat;
			}
			private set
			{
				this.m_BackgroundRepeat.target = value.target;
				bool flag = this.m_BackgroundRepeat == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BackgroundRepeat.Dispose();
					this.m_BackgroundRepeat = value;
					this.Notify("backgroundRepeat");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleBackgroundSize, BackgroundSize> backgroundSize
		{
			get
			{
				return this.m_BackgroundSize;
			}
			private set
			{
				this.m_BackgroundSize.target = value.target;
				bool flag = this.m_BackgroundSize == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BackgroundSize.Dispose();
					this.m_BackgroundSize = value;
					this.Notify("backgroundSize");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleColor, Color> borderBottomColor
		{
			get
			{
				return this.m_BorderBottomColor;
			}
			private set
			{
				this.m_BorderBottomColor.target = value.target;
				bool flag = this.m_BorderBottomColor == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderBottomColor.Dispose();
					this.m_BorderBottomColor = value;
					this.Notify("borderBottomColor");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> borderBottomLeftRadius
		{
			get
			{
				return this.m_BorderBottomLeftRadius;
			}
			private set
			{
				this.m_BorderBottomLeftRadius.target = value.target;
				bool flag = this.m_BorderBottomLeftRadius == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderBottomLeftRadius.Dispose();
					this.m_BorderBottomLeftRadius = value;
					this.Notify("borderBottomLeftRadius");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> borderBottomRightRadius
		{
			get
			{
				return this.m_BorderBottomRightRadius;
			}
			private set
			{
				this.m_BorderBottomRightRadius.target = value.target;
				bool flag = this.m_BorderBottomRightRadius == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderBottomRightRadius.Dispose();
					this.m_BorderBottomRightRadius = value;
					this.Notify("borderBottomRightRadius");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> borderBottomWidth
		{
			get
			{
				return this.m_BorderBottomWidth;
			}
			private set
			{
				this.m_BorderBottomWidth.target = value.target;
				bool flag = this.m_BorderBottomWidth == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderBottomWidth.Dispose();
					this.m_BorderBottomWidth = value;
					this.Notify("borderBottomWidth");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleColor, Color> borderLeftColor
		{
			get
			{
				return this.m_BorderLeftColor;
			}
			private set
			{
				this.m_BorderLeftColor.target = value.target;
				bool flag = this.m_BorderLeftColor == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderLeftColor.Dispose();
					this.m_BorderLeftColor = value;
					this.Notify("borderLeftColor");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> borderLeftWidth
		{
			get
			{
				return this.m_BorderLeftWidth;
			}
			private set
			{
				this.m_BorderLeftWidth.target = value.target;
				bool flag = this.m_BorderLeftWidth == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderLeftWidth.Dispose();
					this.m_BorderLeftWidth = value;
					this.Notify("borderLeftWidth");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleColor, Color> borderRightColor
		{
			get
			{
				return this.m_BorderRightColor;
			}
			private set
			{
				this.m_BorderRightColor.target = value.target;
				bool flag = this.m_BorderRightColor == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderRightColor.Dispose();
					this.m_BorderRightColor = value;
					this.Notify("borderRightColor");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> borderRightWidth
		{
			get
			{
				return this.m_BorderRightWidth;
			}
			private set
			{
				this.m_BorderRightWidth.target = value.target;
				bool flag = this.m_BorderRightWidth == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderRightWidth.Dispose();
					this.m_BorderRightWidth = value;
					this.Notify("borderRightWidth");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleColor, Color> borderTopColor
		{
			get
			{
				return this.m_BorderTopColor;
			}
			private set
			{
				this.m_BorderTopColor.target = value.target;
				bool flag = this.m_BorderTopColor == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderTopColor.Dispose();
					this.m_BorderTopColor = value;
					this.Notify("borderTopColor");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> borderTopLeftRadius
		{
			get
			{
				return this.m_BorderTopLeftRadius;
			}
			private set
			{
				this.m_BorderTopLeftRadius.target = value.target;
				bool flag = this.m_BorderTopLeftRadius == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderTopLeftRadius.Dispose();
					this.m_BorderTopLeftRadius = value;
					this.Notify("borderTopLeftRadius");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> borderTopRightRadius
		{
			get
			{
				return this.m_BorderTopRightRadius;
			}
			private set
			{
				this.m_BorderTopRightRadius.target = value.target;
				bool flag = this.m_BorderTopRightRadius == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderTopRightRadius.Dispose();
					this.m_BorderTopRightRadius = value;
					this.Notify("borderTopRightRadius");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> borderTopWidth
		{
			get
			{
				return this.m_BorderTopWidth;
			}
			private set
			{
				this.m_BorderTopWidth.target = value.target;
				bool flag = this.m_BorderTopWidth == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_BorderTopWidth.Dispose();
					this.m_BorderTopWidth = value;
					this.Notify("borderTopWidth");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> bottom
		{
			get
			{
				return this.m_Bottom;
			}
			private set
			{
				this.m_Bottom.target = value.target;
				bool flag = this.m_Bottom == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Bottom.Dispose();
					this.m_Bottom = value;
					this.Notify("bottom");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleColor, Color> color
		{
			get
			{
				return this.m_Color;
			}
			private set
			{
				this.m_Color.target = value.target;
				bool flag = this.m_Color == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Color.Dispose();
					this.m_Color = value;
					this.Notify("color");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleCursor, Cursor> cursor
		{
			get
			{
				return this.m_Cursor;
			}
			private set
			{
				this.m_Cursor.target = value.target;
				bool flag = this.m_Cursor == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Cursor.Dispose();
					this.m_Cursor = value;
					this.Notify("cursor");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<DisplayStyle>, DisplayStyle> display
		{
			get
			{
				return this.m_Display;
			}
			private set
			{
				this.m_Display.target = value.target;
				bool flag = this.m_Display == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Display.Dispose();
					this.m_Display = value;
					this.Notify("display");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleList<FilterFunction>, List<FilterFunction>> filter
		{
			get
			{
				return this.m_Filter;
			}
			private set
			{
				this.m_Filter.target = value.target;
				bool flag = this.m_Filter == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Filter.Dispose();
					this.m_Filter = value;
					this.Notify("filter");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> flexBasis
		{
			get
			{
				return this.m_FlexBasis;
			}
			private set
			{
				this.m_FlexBasis.target = value.target;
				bool flag = this.m_FlexBasis == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_FlexBasis.Dispose();
					this.m_FlexBasis = value;
					this.Notify("flexBasis");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<FlexDirection>, FlexDirection> flexDirection
		{
			get
			{
				return this.m_FlexDirection;
			}
			private set
			{
				this.m_FlexDirection.target = value.target;
				bool flag = this.m_FlexDirection == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_FlexDirection.Dispose();
					this.m_FlexDirection = value;
					this.Notify("flexDirection");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> flexGrow
		{
			get
			{
				return this.m_FlexGrow;
			}
			private set
			{
				this.m_FlexGrow.target = value.target;
				bool flag = this.m_FlexGrow == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_FlexGrow.Dispose();
					this.m_FlexGrow = value;
					this.Notify("flexGrow");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> flexShrink
		{
			get
			{
				return this.m_FlexShrink;
			}
			private set
			{
				this.m_FlexShrink.target = value.target;
				bool flag = this.m_FlexShrink == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_FlexShrink.Dispose();
					this.m_FlexShrink = value;
					this.Notify("flexShrink");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<Wrap>, Wrap> flexWrap
		{
			get
			{
				return this.m_FlexWrap;
			}
			private set
			{
				this.m_FlexWrap.target = value.target;
				bool flag = this.m_FlexWrap == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_FlexWrap.Dispose();
					this.m_FlexWrap = value;
					this.Notify("flexWrap");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> fontSize
		{
			get
			{
				return this.m_FontSize;
			}
			private set
			{
				this.m_FontSize.target = value.target;
				bool flag = this.m_FontSize == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_FontSize.Dispose();
					this.m_FontSize = value;
					this.Notify("fontSize");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> height
		{
			get
			{
				return this.m_Height;
			}
			private set
			{
				this.m_Height.target = value.target;
				bool flag = this.m_Height == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Height.Dispose();
					this.m_Height = value;
					this.Notify("height");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<Justify>, Justify> justifyContent
		{
			get
			{
				return this.m_JustifyContent;
			}
			private set
			{
				this.m_JustifyContent.target = value.target;
				bool flag = this.m_JustifyContent == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_JustifyContent.Dispose();
					this.m_JustifyContent = value;
					this.Notify("justifyContent");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> left
		{
			get
			{
				return this.m_Left;
			}
			private set
			{
				this.m_Left.target = value.target;
				bool flag = this.m_Left == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Left.Dispose();
					this.m_Left = value;
					this.Notify("left");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> letterSpacing
		{
			get
			{
				return this.m_LetterSpacing;
			}
			private set
			{
				this.m_LetterSpacing.target = value.target;
				bool flag = this.m_LetterSpacing == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_LetterSpacing.Dispose();
					this.m_LetterSpacing = value;
					this.Notify("letterSpacing");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> marginBottom
		{
			get
			{
				return this.m_MarginBottom;
			}
			private set
			{
				this.m_MarginBottom.target = value.target;
				bool flag = this.m_MarginBottom == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_MarginBottom.Dispose();
					this.m_MarginBottom = value;
					this.Notify("marginBottom");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> marginLeft
		{
			get
			{
				return this.m_MarginLeft;
			}
			private set
			{
				this.m_MarginLeft.target = value.target;
				bool flag = this.m_MarginLeft == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_MarginLeft.Dispose();
					this.m_MarginLeft = value;
					this.Notify("marginLeft");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> marginRight
		{
			get
			{
				return this.m_MarginRight;
			}
			private set
			{
				this.m_MarginRight.target = value.target;
				bool flag = this.m_MarginRight == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_MarginRight.Dispose();
					this.m_MarginRight = value;
					this.Notify("marginRight");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> marginTop
		{
			get
			{
				return this.m_MarginTop;
			}
			private set
			{
				this.m_MarginTop.target = value.target;
				bool flag = this.m_MarginTop == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_MarginTop.Dispose();
					this.m_MarginTop = value;
					this.Notify("marginTop");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> maxHeight
		{
			get
			{
				return this.m_MaxHeight;
			}
			private set
			{
				this.m_MaxHeight.target = value.target;
				bool flag = this.m_MaxHeight == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_MaxHeight.Dispose();
					this.m_MaxHeight = value;
					this.Notify("maxHeight");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> maxWidth
		{
			get
			{
				return this.m_MaxWidth;
			}
			private set
			{
				this.m_MaxWidth.target = value.target;
				bool flag = this.m_MaxWidth == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_MaxWidth.Dispose();
					this.m_MaxWidth = value;
					this.Notify("maxWidth");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> minHeight
		{
			get
			{
				return this.m_MinHeight;
			}
			private set
			{
				this.m_MinHeight.target = value.target;
				bool flag = this.m_MinHeight == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_MinHeight.Dispose();
					this.m_MinHeight = value;
					this.Notify("minHeight");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> minWidth
		{
			get
			{
				return this.m_MinWidth;
			}
			private set
			{
				this.m_MinWidth.target = value.target;
				bool flag = this.m_MinWidth == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_MinWidth.Dispose();
					this.m_MinWidth = value;
					this.Notify("minWidth");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> opacity
		{
			get
			{
				return this.m_Opacity;
			}
			private set
			{
				this.m_Opacity.target = value.target;
				bool flag = this.m_Opacity == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Opacity.Dispose();
					this.m_Opacity = value;
					this.Notify("opacity");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<Overflow>, OverflowInternal> overflow
		{
			get
			{
				return this.m_Overflow;
			}
			private set
			{
				this.m_Overflow.target = value.target;
				bool flag = this.m_Overflow == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Overflow.Dispose();
					this.m_Overflow = value;
					this.Notify("overflow");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> paddingBottom
		{
			get
			{
				return this.m_PaddingBottom;
			}
			private set
			{
				this.m_PaddingBottom.target = value.target;
				bool flag = this.m_PaddingBottom == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_PaddingBottom.Dispose();
					this.m_PaddingBottom = value;
					this.Notify("paddingBottom");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> paddingLeft
		{
			get
			{
				return this.m_PaddingLeft;
			}
			private set
			{
				this.m_PaddingLeft.target = value.target;
				bool flag = this.m_PaddingLeft == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_PaddingLeft.Dispose();
					this.m_PaddingLeft = value;
					this.Notify("paddingLeft");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> paddingRight
		{
			get
			{
				return this.m_PaddingRight;
			}
			private set
			{
				this.m_PaddingRight.target = value.target;
				bool flag = this.m_PaddingRight == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_PaddingRight.Dispose();
					this.m_PaddingRight = value;
					this.Notify("paddingRight");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> paddingTop
		{
			get
			{
				return this.m_PaddingTop;
			}
			private set
			{
				this.m_PaddingTop.target = value.target;
				bool flag = this.m_PaddingTop == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_PaddingTop.Dispose();
					this.m_PaddingTop = value;
					this.Notify("paddingTop");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<Position>, Position> position
		{
			get
			{
				return this.m_Position;
			}
			private set
			{
				this.m_Position.target = value.target;
				bool flag = this.m_Position == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Position.Dispose();
					this.m_Position = value;
					this.Notify("position");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> right
		{
			get
			{
				return this.m_Right;
			}
			private set
			{
				this.m_Right.target = value.target;
				bool flag = this.m_Right == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Right.Dispose();
					this.m_Right = value;
					this.Notify("right");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleRotate, Rotate> rotate
		{
			get
			{
				return this.m_Rotate;
			}
			private set
			{
				this.m_Rotate.target = value.target;
				bool flag = this.m_Rotate == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Rotate.Dispose();
					this.m_Rotate = value;
					this.Notify("rotate");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleScale, Scale> scale
		{
			get
			{
				return this.m_Scale;
			}
			private set
			{
				this.m_Scale.target = value.target;
				bool flag = this.m_Scale == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Scale.Dispose();
					this.m_Scale = value;
					this.Notify("scale");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<TextOverflow>, TextOverflow> textOverflow
		{
			get
			{
				return this.m_TextOverflow;
			}
			private set
			{
				this.m_TextOverflow.target = value.target;
				bool flag = this.m_TextOverflow == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_TextOverflow.Dispose();
					this.m_TextOverflow = value;
					this.Notify("textOverflow");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleTextShadow, TextShadow> textShadow
		{
			get
			{
				return this.m_TextShadow;
			}
			private set
			{
				this.m_TextShadow.target = value.target;
				bool flag = this.m_TextShadow == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_TextShadow.Dispose();
					this.m_TextShadow = value;
					this.Notify("textShadow");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> top
		{
			get
			{
				return this.m_Top;
			}
			private set
			{
				this.m_Top.target = value.target;
				bool flag = this.m_Top == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Top.Dispose();
					this.m_Top = value;
					this.Notify("top");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleTransformOrigin, TransformOrigin> transformOrigin
		{
			get
			{
				return this.m_TransformOrigin;
			}
			private set
			{
				this.m_TransformOrigin.target = value.target;
				bool flag = this.m_TransformOrigin == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_TransformOrigin.Dispose();
					this.m_TransformOrigin = value;
					this.Notify("transformOrigin");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleList<TimeValue>, List<TimeValue>> transitionDelay
		{
			get
			{
				return this.m_TransitionDelay;
			}
			private set
			{
				this.m_TransitionDelay.target = value.target;
				bool flag = this.m_TransitionDelay == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_TransitionDelay.Dispose();
					this.m_TransitionDelay = value;
					this.Notify("transitionDelay");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleList<TimeValue>, List<TimeValue>> transitionDuration
		{
			get
			{
				return this.m_TransitionDuration;
			}
			private set
			{
				this.m_TransitionDuration.target = value.target;
				bool flag = this.m_TransitionDuration == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_TransitionDuration.Dispose();
					this.m_TransitionDuration = value;
					this.Notify("transitionDuration");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleList<StylePropertyName>, List<StylePropertyName>> transitionProperty
		{
			get
			{
				return this.m_TransitionProperty;
			}
			private set
			{
				this.m_TransitionProperty.target = value.target;
				bool flag = this.m_TransitionProperty == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_TransitionProperty.Dispose();
					this.m_TransitionProperty = value;
					this.Notify("transitionProperty");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleList<EasingFunction>, List<EasingFunction>> transitionTimingFunction
		{
			get
			{
				return this.m_TransitionTimingFunction;
			}
			private set
			{
				this.m_TransitionTimingFunction.target = value.target;
				bool flag = this.m_TransitionTimingFunction == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_TransitionTimingFunction.Dispose();
					this.m_TransitionTimingFunction = value;
					this.Notify("transitionTimingFunction");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleTranslate, Translate> translate
		{
			get
			{
				return this.m_Translate;
			}
			private set
			{
				this.m_Translate.target = value.target;
				bool flag = this.m_Translate == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Translate.Dispose();
					this.m_Translate = value;
					this.Notify("translate");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleColor, Color> unityBackgroundImageTintColor
		{
			get
			{
				return this.m_UnityBackgroundImageTintColor;
			}
			private set
			{
				this.m_UnityBackgroundImageTintColor.target = value.target;
				bool flag = this.m_UnityBackgroundImageTintColor == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityBackgroundImageTintColor.Dispose();
					this.m_UnityBackgroundImageTintColor = value;
					this.Notify("unityBackgroundImageTintColor");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<EditorTextRenderingMode>, EditorTextRenderingMode> unityEditorTextRenderingMode
		{
			get
			{
				return this.m_UnityEditorTextRenderingMode;
			}
			private set
			{
				this.m_UnityEditorTextRenderingMode.target = value.target;
				bool flag = this.m_UnityEditorTextRenderingMode == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityEditorTextRenderingMode.Dispose();
					this.m_UnityEditorTextRenderingMode = value;
					this.Notify("unityEditorTextRenderingMode");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFont, Font> unityFont
		{
			get
			{
				return this.m_UnityFont;
			}
			private set
			{
				this.m_UnityFont.target = value.target;
				bool flag = this.m_UnityFont == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityFont.Dispose();
					this.m_UnityFont = value;
					this.Notify("unityFont");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFontDefinition, FontDefinition> unityFontDefinition
		{
			get
			{
				return this.m_UnityFontDefinition;
			}
			private set
			{
				this.m_UnityFontDefinition.target = value.target;
				bool flag = this.m_UnityFontDefinition == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityFontDefinition.Dispose();
					this.m_UnityFontDefinition = value;
					this.Notify("unityFontDefinition");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<FontStyle>, FontStyle> unityFontStyleAndWeight
		{
			get
			{
				return this.m_UnityFontStyleAndWeight;
			}
			private set
			{
				this.m_UnityFontStyleAndWeight.target = value.target;
				bool flag = this.m_UnityFontStyleAndWeight == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityFontStyleAndWeight.Dispose();
					this.m_UnityFontStyleAndWeight = value;
					this.Notify("unityFontStyleAndWeight");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleMaterialDefinition, MaterialDefinition> unityMaterial
		{
			get
			{
				return this.m_UnityMaterial;
			}
			private set
			{
				this.m_UnityMaterial.target = value.target;
				bool flag = this.m_UnityMaterial == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityMaterial.Dispose();
					this.m_UnityMaterial = value;
					this.Notify("unityMaterial");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<OverflowClipBox>, OverflowClipBox> unityOverflowClipBox
		{
			get
			{
				return this.m_UnityOverflowClipBox;
			}
			private set
			{
				this.m_UnityOverflowClipBox.target = value.target;
				bool flag = this.m_UnityOverflowClipBox == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityOverflowClipBox.Dispose();
					this.m_UnityOverflowClipBox = value;
					this.Notify("unityOverflowClipBox");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> unityParagraphSpacing
		{
			get
			{
				return this.m_UnityParagraphSpacing;
			}
			private set
			{
				this.m_UnityParagraphSpacing.target = value.target;
				bool flag = this.m_UnityParagraphSpacing == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityParagraphSpacing.Dispose();
					this.m_UnityParagraphSpacing = value;
					this.Notify("unityParagraphSpacing");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleInt, int> unitySliceBottom
		{
			get
			{
				return this.m_UnitySliceBottom;
			}
			private set
			{
				this.m_UnitySliceBottom.target = value.target;
				bool flag = this.m_UnitySliceBottom == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnitySliceBottom.Dispose();
					this.m_UnitySliceBottom = value;
					this.Notify("unitySliceBottom");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleInt, int> unitySliceLeft
		{
			get
			{
				return this.m_UnitySliceLeft;
			}
			private set
			{
				this.m_UnitySliceLeft.target = value.target;
				bool flag = this.m_UnitySliceLeft == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnitySliceLeft.Dispose();
					this.m_UnitySliceLeft = value;
					this.Notify("unitySliceLeft");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleInt, int> unitySliceRight
		{
			get
			{
				return this.m_UnitySliceRight;
			}
			private set
			{
				this.m_UnitySliceRight.target = value.target;
				bool flag = this.m_UnitySliceRight == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnitySliceRight.Dispose();
					this.m_UnitySliceRight = value;
					this.Notify("unitySliceRight");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> unitySliceScale
		{
			get
			{
				return this.m_UnitySliceScale;
			}
			private set
			{
				this.m_UnitySliceScale.target = value.target;
				bool flag = this.m_UnitySliceScale == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnitySliceScale.Dispose();
					this.m_UnitySliceScale = value;
					this.Notify("unitySliceScale");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleInt, int> unitySliceTop
		{
			get
			{
				return this.m_UnitySliceTop;
			}
			private set
			{
				this.m_UnitySliceTop.target = value.target;
				bool flag = this.m_UnitySliceTop == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnitySliceTop.Dispose();
					this.m_UnitySliceTop = value;
					this.Notify("unitySliceTop");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<SliceType>, SliceType> unitySliceType
		{
			get
			{
				return this.m_UnitySliceType;
			}
			private set
			{
				this.m_UnitySliceType.target = value.target;
				bool flag = this.m_UnitySliceType == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnitySliceType.Dispose();
					this.m_UnitySliceType = value;
					this.Notify("unitySliceType");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<TextAnchor>, TextAnchor> unityTextAlign
		{
			get
			{
				return this.m_UnityTextAlign;
			}
			private set
			{
				this.m_UnityTextAlign.target = value.target;
				bool flag = this.m_UnityTextAlign == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityTextAlign.Dispose();
					this.m_UnityTextAlign = value;
					this.Notify("unityTextAlign");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleTextAutoSize, TextAutoSize> unityTextAutoSize
		{
			get
			{
				return this.m_UnityTextAutoSize;
			}
			private set
			{
				this.m_UnityTextAutoSize.target = value.target;
				bool flag = this.m_UnityTextAutoSize == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityTextAutoSize.Dispose();
					this.m_UnityTextAutoSize = value;
					this.Notify("unityTextAutoSize");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<TextGeneratorType>, TextGeneratorType> unityTextGenerator
		{
			get
			{
				return this.m_UnityTextGenerator;
			}
			private set
			{
				this.m_UnityTextGenerator.target = value.target;
				bool flag = this.m_UnityTextGenerator == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityTextGenerator.Dispose();
					this.m_UnityTextGenerator = value;
					this.Notify("unityTextGenerator");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleColor, Color> unityTextOutlineColor
		{
			get
			{
				return this.m_UnityTextOutlineColor;
			}
			private set
			{
				this.m_UnityTextOutlineColor.target = value.target;
				bool flag = this.m_UnityTextOutlineColor == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityTextOutlineColor.Dispose();
					this.m_UnityTextOutlineColor = value;
					this.Notify("unityTextOutlineColor");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleFloat, float> unityTextOutlineWidth
		{
			get
			{
				return this.m_UnityTextOutlineWidth;
			}
			private set
			{
				this.m_UnityTextOutlineWidth.target = value.target;
				bool flag = this.m_UnityTextOutlineWidth == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityTextOutlineWidth.Dispose();
					this.m_UnityTextOutlineWidth = value;
					this.Notify("unityTextOutlineWidth");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<TextOverflowPosition>, TextOverflowPosition> unityTextOverflowPosition
		{
			get
			{
				return this.m_UnityTextOverflowPosition;
			}
			private set
			{
				this.m_UnityTextOverflowPosition.target = value.target;
				bool flag = this.m_UnityTextOverflowPosition == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_UnityTextOverflowPosition.Dispose();
					this.m_UnityTextOverflowPosition = value;
					this.Notify("unityTextOverflowPosition");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<Visibility>, Visibility> visibility
		{
			get
			{
				return this.m_Visibility;
			}
			private set
			{
				this.m_Visibility.target = value.target;
				bool flag = this.m_Visibility == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Visibility.Dispose();
					this.m_Visibility = value;
					this.Notify("visibility");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleEnum<WhiteSpace>, WhiteSpace> whiteSpace
		{
			get
			{
				return this.m_WhiteSpace;
			}
			private set
			{
				this.m_WhiteSpace.target = value.target;
				bool flag = this.m_WhiteSpace == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_WhiteSpace.Dispose();
					this.m_WhiteSpace = value;
					this.Notify("whiteSpace");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> width
		{
			get
			{
				return this.m_Width;
			}
			private set
			{
				this.m_Width.target = value.target;
				bool flag = this.m_Width == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_Width.Dispose();
					this.m_Width = value;
					this.Notify("width");
				}
			}
		}

		[CreateProperty]
		public StylePropertyData<StyleLength, Length> wordSpacing
		{
			get
			{
				return this.m_WordSpacing;
			}
			private set
			{
				this.m_WordSpacing.target = value.target;
				bool flag = this.m_WordSpacing == value;
				if (flag)
				{
					value.Dispose();
				}
				else
				{
					this.m_WordSpacing.Dispose();
					this.m_WordSpacing = value;
					this.Notify("wordSpacing");
				}
			}
		}

		private void Refresh(VisualElement element, in StyleDiff.ResolutionContext context)
		{
			string text = "alignContent";
			StyleEnum<Align> styleEnum = element.style.alignContent;
			Align align = element.computedStyle.alignContent;
			this.alignContent = StyleDiff.ComputeStyleProperty<StyleEnum<Align>, Align>(element, text, in styleEnum, in align, in context);
			string text2 = "alignItems";
			styleEnum = element.style.alignItems;
			align = element.computedStyle.alignItems;
			this.alignItems = StyleDiff.ComputeStyleProperty<StyleEnum<Align>, Align>(element, text2, in styleEnum, in align, in context);
			string text3 = "alignSelf";
			styleEnum = element.style.alignSelf;
			align = element.computedStyle.alignSelf;
			this.alignSelf = StyleDiff.ComputeStyleProperty<StyleEnum<Align>, Align>(element, text3, in styleEnum, in align, in context);
			string text4 = "aspectRatio";
			StyleRatio aspectRatio = element.style.aspectRatio;
			Ratio aspectRatio2 = element.computedStyle.aspectRatio;
			this.aspectRatio = StyleDiff.ComputeStyleProperty<StyleRatio, Ratio>(element, text4, in aspectRatio, in aspectRatio2, in context);
			string text5 = "backgroundColor";
			StyleColor styleColor = element.style.backgroundColor;
			Color color = element.computedStyle.backgroundColor;
			this.backgroundColor = StyleDiff.ComputeStyleProperty<StyleColor, Color>(element, text5, in styleColor, in color, in context);
			string text6 = "backgroundImage";
			StyleBackground backgroundImage = element.style.backgroundImage;
			Background backgroundImage2 = element.computedStyle.backgroundImage;
			this.backgroundImage = StyleDiff.ComputeStyleProperty<StyleBackground, Background>(element, text6, in backgroundImage, in backgroundImage2, in context);
			string text7 = "backgroundPositionX";
			StyleBackgroundPosition styleBackgroundPosition = element.style.backgroundPositionX;
			BackgroundPosition backgroundPosition = element.computedStyle.backgroundPositionX;
			this.backgroundPositionX = StyleDiff.ComputeStyleProperty<StyleBackgroundPosition, BackgroundPosition>(element, text7, in styleBackgroundPosition, in backgroundPosition, in context);
			string text8 = "backgroundPositionY";
			styleBackgroundPosition = element.style.backgroundPositionY;
			backgroundPosition = element.computedStyle.backgroundPositionY;
			this.backgroundPositionY = StyleDiff.ComputeStyleProperty<StyleBackgroundPosition, BackgroundPosition>(element, text8, in styleBackgroundPosition, in backgroundPosition, in context);
			string text9 = "backgroundRepeat";
			StyleBackgroundRepeat backgroundRepeat = element.style.backgroundRepeat;
			BackgroundRepeat backgroundRepeat2 = element.computedStyle.backgroundRepeat;
			this.backgroundRepeat = StyleDiff.ComputeStyleProperty<StyleBackgroundRepeat, BackgroundRepeat>(element, text9, in backgroundRepeat, in backgroundRepeat2, in context);
			string text10 = "backgroundSize";
			StyleBackgroundSize backgroundSize = element.style.backgroundSize;
			BackgroundSize backgroundSize2 = element.computedStyle.backgroundSize;
			this.backgroundSize = StyleDiff.ComputeStyleProperty<StyleBackgroundSize, BackgroundSize>(element, text10, in backgroundSize, in backgroundSize2, in context);
			string text11 = "borderBottomColor";
			styleColor = element.style.borderBottomColor;
			color = element.computedStyle.borderBottomColor;
			this.borderBottomColor = StyleDiff.ComputeStyleProperty<StyleColor, Color>(element, text11, in styleColor, in color, in context);
			string text12 = "borderBottomLeftRadius";
			StyleLength styleLength = element.style.borderBottomLeftRadius;
			Length length = element.computedStyle.borderBottomLeftRadius;
			this.borderBottomLeftRadius = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text12, in styleLength, in length, in context);
			string text13 = "borderBottomRightRadius";
			styleLength = element.style.borderBottomRightRadius;
			length = element.computedStyle.borderBottomRightRadius;
			this.borderBottomRightRadius = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text13, in styleLength, in length, in context);
			string text14 = "borderBottomWidth";
			StyleFloat styleFloat = element.style.borderBottomWidth;
			float num = element.computedStyle.borderBottomWidth;
			this.borderBottomWidth = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text14, in styleFloat, in num, in context);
			string text15 = "borderLeftColor";
			styleColor = element.style.borderLeftColor;
			color = element.computedStyle.borderLeftColor;
			this.borderLeftColor = StyleDiff.ComputeStyleProperty<StyleColor, Color>(element, text15, in styleColor, in color, in context);
			string text16 = "borderLeftWidth";
			styleFloat = element.style.borderLeftWidth;
			num = element.computedStyle.borderLeftWidth;
			this.borderLeftWidth = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text16, in styleFloat, in num, in context);
			string text17 = "borderRightColor";
			styleColor = element.style.borderRightColor;
			color = element.computedStyle.borderRightColor;
			this.borderRightColor = StyleDiff.ComputeStyleProperty<StyleColor, Color>(element, text17, in styleColor, in color, in context);
			string text18 = "borderRightWidth";
			styleFloat = element.style.borderRightWidth;
			num = element.computedStyle.borderRightWidth;
			this.borderRightWidth = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text18, in styleFloat, in num, in context);
			string text19 = "borderTopColor";
			styleColor = element.style.borderTopColor;
			color = element.computedStyle.borderTopColor;
			this.borderTopColor = StyleDiff.ComputeStyleProperty<StyleColor, Color>(element, text19, in styleColor, in color, in context);
			string text20 = "borderTopLeftRadius";
			styleLength = element.style.borderTopLeftRadius;
			length = element.computedStyle.borderTopLeftRadius;
			this.borderTopLeftRadius = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text20, in styleLength, in length, in context);
			string text21 = "borderTopRightRadius";
			styleLength = element.style.borderTopRightRadius;
			length = element.computedStyle.borderTopRightRadius;
			this.borderTopRightRadius = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text21, in styleLength, in length, in context);
			string text22 = "borderTopWidth";
			styleFloat = element.style.borderTopWidth;
			num = element.computedStyle.borderTopWidth;
			this.borderTopWidth = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text22, in styleFloat, in num, in context);
			string text23 = "bottom";
			styleLength = element.style.bottom;
			length = element.computedStyle.bottom;
			this.bottom = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text23, in styleLength, in length, in context);
			string text24 = "color";
			styleColor = element.style.color;
			color = element.computedStyle.color;
			this.color = StyleDiff.ComputeStyleProperty<StyleColor, Color>(element, text24, in styleColor, in color, in context);
			string text25 = "cursor";
			StyleCursor cursor = element.style.cursor;
			Cursor cursor2 = element.computedStyle.cursor;
			this.cursor = StyleDiff.ComputeStyleProperty<StyleCursor, Cursor>(element, text25, in cursor, in cursor2, in context);
			string text26 = "display";
			StyleEnum<DisplayStyle> display = element.style.display;
			DisplayStyle display2 = element.computedStyle.display;
			this.display = StyleDiff.ComputeStyleProperty<StyleEnum<DisplayStyle>, DisplayStyle>(element, text26, in display, in display2, in context);
			string text27 = "filter";
			StyleList<FilterFunction> filter = element.style.filter;
			List<FilterFunction> filter2 = element.computedStyle.filter;
			this.filter = StyleDiff.ComputeStyleProperty<StyleList<FilterFunction>, List<FilterFunction>>(element, text27, in filter, in filter2, in context);
			string text28 = "flexBasis";
			styleLength = element.style.flexBasis;
			length = element.computedStyle.flexBasis;
			this.flexBasis = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text28, in styleLength, in length, in context);
			string text29 = "flexDirection";
			StyleEnum<FlexDirection> flexDirection = element.style.flexDirection;
			FlexDirection flexDirection2 = element.computedStyle.flexDirection;
			this.flexDirection = StyleDiff.ComputeStyleProperty<StyleEnum<FlexDirection>, FlexDirection>(element, text29, in flexDirection, in flexDirection2, in context);
			string text30 = "flexGrow";
			styleFloat = element.style.flexGrow;
			num = element.computedStyle.flexGrow;
			this.flexGrow = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text30, in styleFloat, in num, in context);
			string text31 = "flexShrink";
			styleFloat = element.style.flexShrink;
			num = element.computedStyle.flexShrink;
			this.flexShrink = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text31, in styleFloat, in num, in context);
			string text32 = "flexWrap";
			StyleEnum<Wrap> flexWrap = element.style.flexWrap;
			Wrap flexWrap2 = element.computedStyle.flexWrap;
			this.flexWrap = StyleDiff.ComputeStyleProperty<StyleEnum<Wrap>, Wrap>(element, text32, in flexWrap, in flexWrap2, in context);
			string text33 = "fontSize";
			styleLength = element.style.fontSize;
			length = element.computedStyle.fontSize;
			this.fontSize = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text33, in styleLength, in length, in context);
			string text34 = "height";
			styleLength = element.style.height;
			length = element.computedStyle.height;
			this.height = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text34, in styleLength, in length, in context);
			string text35 = "justifyContent";
			StyleEnum<Justify> justifyContent = element.style.justifyContent;
			Justify justifyContent2 = element.computedStyle.justifyContent;
			this.justifyContent = StyleDiff.ComputeStyleProperty<StyleEnum<Justify>, Justify>(element, text35, in justifyContent, in justifyContent2, in context);
			string text36 = "left";
			styleLength = element.style.left;
			length = element.computedStyle.left;
			this.left = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text36, in styleLength, in length, in context);
			string text37 = "letterSpacing";
			styleLength = element.style.letterSpacing;
			length = element.computedStyle.letterSpacing;
			this.letterSpacing = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text37, in styleLength, in length, in context);
			string text38 = "marginBottom";
			styleLength = element.style.marginBottom;
			length = element.computedStyle.marginBottom;
			this.marginBottom = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text38, in styleLength, in length, in context);
			string text39 = "marginLeft";
			styleLength = element.style.marginLeft;
			length = element.computedStyle.marginLeft;
			this.marginLeft = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text39, in styleLength, in length, in context);
			string text40 = "marginRight";
			styleLength = element.style.marginRight;
			length = element.computedStyle.marginRight;
			this.marginRight = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text40, in styleLength, in length, in context);
			string text41 = "marginTop";
			styleLength = element.style.marginTop;
			length = element.computedStyle.marginTop;
			this.marginTop = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text41, in styleLength, in length, in context);
			string text42 = "maxHeight";
			styleLength = element.style.maxHeight;
			length = element.computedStyle.maxHeight;
			this.maxHeight = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text42, in styleLength, in length, in context);
			string text43 = "maxWidth";
			styleLength = element.style.maxWidth;
			length = element.computedStyle.maxWidth;
			this.maxWidth = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text43, in styleLength, in length, in context);
			string text44 = "minHeight";
			styleLength = element.style.minHeight;
			length = element.computedStyle.minHeight;
			this.minHeight = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text44, in styleLength, in length, in context);
			string text45 = "minWidth";
			styleLength = element.style.minWidth;
			length = element.computedStyle.minWidth;
			this.minWidth = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text45, in styleLength, in length, in context);
			string text46 = "opacity";
			styleFloat = element.style.opacity;
			num = element.computedStyle.opacity;
			this.opacity = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text46, in styleFloat, in num, in context);
			string text47 = "overflow";
			StyleEnum<Overflow> overflow = element.style.overflow;
			OverflowInternal overflow2 = element.computedStyle.overflow;
			this.overflow = StyleDiff.ComputeStyleProperty<StyleEnum<Overflow>, OverflowInternal>(element, text47, in overflow, in overflow2, in context);
			string text48 = "paddingBottom";
			styleLength = element.style.paddingBottom;
			length = element.computedStyle.paddingBottom;
			this.paddingBottom = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text48, in styleLength, in length, in context);
			string text49 = "paddingLeft";
			styleLength = element.style.paddingLeft;
			length = element.computedStyle.paddingLeft;
			this.paddingLeft = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text49, in styleLength, in length, in context);
			string text50 = "paddingRight";
			styleLength = element.style.paddingRight;
			length = element.computedStyle.paddingRight;
			this.paddingRight = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text50, in styleLength, in length, in context);
			string text51 = "paddingTop";
			styleLength = element.style.paddingTop;
			length = element.computedStyle.paddingTop;
			this.paddingTop = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text51, in styleLength, in length, in context);
			string text52 = "position";
			StyleEnum<Position> position = element.style.position;
			Position position2 = element.computedStyle.position;
			this.position = StyleDiff.ComputeStyleProperty<StyleEnum<Position>, Position>(element, text52, in position, in position2, in context);
			string text53 = "right";
			styleLength = element.style.right;
			length = element.computedStyle.right;
			this.right = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text53, in styleLength, in length, in context);
			string text54 = "rotate";
			StyleRotate rotate = element.style.rotate;
			Rotate rotate2 = element.computedStyle.rotate;
			this.rotate = StyleDiff.ComputeStyleProperty<StyleRotate, Rotate>(element, text54, in rotate, in rotate2, in context);
			string text55 = "scale";
			StyleScale scale = element.style.scale;
			Scale scale2 = element.computedStyle.scale;
			this.scale = StyleDiff.ComputeStyleProperty<StyleScale, Scale>(element, text55, in scale, in scale2, in context);
			string text56 = "textOverflow";
			StyleEnum<TextOverflow> textOverflow = element.style.textOverflow;
			TextOverflow textOverflow2 = element.computedStyle.textOverflow;
			this.textOverflow = StyleDiff.ComputeStyleProperty<StyleEnum<TextOverflow>, TextOverflow>(element, text56, in textOverflow, in textOverflow2, in context);
			string text57 = "textShadow";
			StyleTextShadow textShadow = element.style.textShadow;
			TextShadow textShadow2 = element.computedStyle.textShadow;
			this.textShadow = StyleDiff.ComputeStyleProperty<StyleTextShadow, TextShadow>(element, text57, in textShadow, in textShadow2, in context);
			string text58 = "top";
			styleLength = element.style.top;
			length = element.computedStyle.top;
			this.top = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text58, in styleLength, in length, in context);
			string text59 = "transformOrigin";
			StyleTransformOrigin transformOrigin = element.style.transformOrigin;
			TransformOrigin transformOrigin2 = element.computedStyle.transformOrigin;
			this.transformOrigin = StyleDiff.ComputeStyleProperty<StyleTransformOrigin, TransformOrigin>(element, text59, in transformOrigin, in transformOrigin2, in context);
			string text60 = "transitionDelay";
			StyleList<TimeValue> styleList = element.style.transitionDelay;
			List<TimeValue> list = element.computedStyle.transitionDelay;
			this.transitionDelay = StyleDiff.ComputeStyleProperty<StyleList<TimeValue>, List<TimeValue>>(element, text60, in styleList, in list, in context);
			string text61 = "transitionDuration";
			styleList = element.style.transitionDuration;
			list = element.computedStyle.transitionDuration;
			this.transitionDuration = StyleDiff.ComputeStyleProperty<StyleList<TimeValue>, List<TimeValue>>(element, text61, in styleList, in list, in context);
			string text62 = "transitionProperty";
			StyleList<StylePropertyName> transitionProperty = element.style.transitionProperty;
			List<StylePropertyName> transitionProperty2 = element.computedStyle.transitionProperty;
			this.transitionProperty = StyleDiff.ComputeStyleProperty<StyleList<StylePropertyName>, List<StylePropertyName>>(element, text62, in transitionProperty, in transitionProperty2, in context);
			string text63 = "transitionTimingFunction";
			StyleList<EasingFunction> transitionTimingFunction = element.style.transitionTimingFunction;
			List<EasingFunction> transitionTimingFunction2 = element.computedStyle.transitionTimingFunction;
			this.transitionTimingFunction = StyleDiff.ComputeStyleProperty<StyleList<EasingFunction>, List<EasingFunction>>(element, text63, in transitionTimingFunction, in transitionTimingFunction2, in context);
			string text64 = "translate";
			StyleTranslate translate = element.style.translate;
			Translate translate2 = element.computedStyle.translate;
			this.translate = StyleDiff.ComputeStyleProperty<StyleTranslate, Translate>(element, text64, in translate, in translate2, in context);
			string text65 = "unityBackgroundImageTintColor";
			styleColor = element.style.unityBackgroundImageTintColor;
			color = element.computedStyle.unityBackgroundImageTintColor;
			this.unityBackgroundImageTintColor = StyleDiff.ComputeStyleProperty<StyleColor, Color>(element, text65, in styleColor, in color, in context);
			string text66 = "unityEditorTextRenderingMode";
			StyleEnum<EditorTextRenderingMode> unityEditorTextRenderingMode = element.style.unityEditorTextRenderingMode;
			EditorTextRenderingMode unityEditorTextRenderingMode2 = element.computedStyle.unityEditorTextRenderingMode;
			this.unityEditorTextRenderingMode = StyleDiff.ComputeStyleProperty<StyleEnum<EditorTextRenderingMode>, EditorTextRenderingMode>(element, text66, in unityEditorTextRenderingMode, in unityEditorTextRenderingMode2, in context);
			string text67 = "unityFont";
			StyleFont unityFont = element.style.unityFont;
			Font unityFont2 = element.computedStyle.unityFont;
			this.unityFont = StyleDiff.ComputeStyleProperty<StyleFont, Font>(element, text67, in unityFont, in unityFont2, in context);
			string text68 = "unityFontDefinition";
			StyleFontDefinition unityFontDefinition = element.style.unityFontDefinition;
			FontDefinition unityFontDefinition2 = element.computedStyle.unityFontDefinition;
			this.unityFontDefinition = StyleDiff.ComputeStyleProperty<StyleFontDefinition, FontDefinition>(element, text68, in unityFontDefinition, in unityFontDefinition2, in context);
			string text69 = "unityFontStyleAndWeight";
			StyleEnum<FontStyle> unityFontStyleAndWeight = element.style.unityFontStyleAndWeight;
			FontStyle unityFontStyleAndWeight2 = element.computedStyle.unityFontStyleAndWeight;
			this.unityFontStyleAndWeight = StyleDiff.ComputeStyleProperty<StyleEnum<FontStyle>, FontStyle>(element, text69, in unityFontStyleAndWeight, in unityFontStyleAndWeight2, in context);
			string text70 = "unityMaterial";
			StyleMaterialDefinition unityMaterial = element.style.unityMaterial;
			MaterialDefinition unityMaterial2 = element.computedStyle.unityMaterial;
			this.unityMaterial = StyleDiff.ComputeStyleProperty<StyleMaterialDefinition, MaterialDefinition>(element, text70, in unityMaterial, in unityMaterial2, in context);
			string text71 = "unityOverflowClipBox";
			StyleEnum<OverflowClipBox> unityOverflowClipBox = element.style.unityOverflowClipBox;
			OverflowClipBox unityOverflowClipBox2 = element.computedStyle.unityOverflowClipBox;
			this.unityOverflowClipBox = StyleDiff.ComputeStyleProperty<StyleEnum<OverflowClipBox>, OverflowClipBox>(element, text71, in unityOverflowClipBox, in unityOverflowClipBox2, in context);
			string text72 = "unityParagraphSpacing";
			styleLength = element.style.unityParagraphSpacing;
			length = element.computedStyle.unityParagraphSpacing;
			this.unityParagraphSpacing = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text72, in styleLength, in length, in context);
			string text73 = "unitySliceBottom";
			StyleInt styleInt = element.style.unitySliceBottom;
			int num2 = element.computedStyle.unitySliceBottom;
			this.unitySliceBottom = StyleDiff.ComputeStyleProperty<StyleInt, int>(element, text73, in styleInt, in num2, in context);
			string text74 = "unitySliceLeft";
			styleInt = element.style.unitySliceLeft;
			num2 = element.computedStyle.unitySliceLeft;
			this.unitySliceLeft = StyleDiff.ComputeStyleProperty<StyleInt, int>(element, text74, in styleInt, in num2, in context);
			string text75 = "unitySliceRight";
			styleInt = element.style.unitySliceRight;
			num2 = element.computedStyle.unitySliceRight;
			this.unitySliceRight = StyleDiff.ComputeStyleProperty<StyleInt, int>(element, text75, in styleInt, in num2, in context);
			string text76 = "unitySliceScale";
			styleFloat = element.style.unitySliceScale;
			num = element.computedStyle.unitySliceScale;
			this.unitySliceScale = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text76, in styleFloat, in num, in context);
			string text77 = "unitySliceTop";
			styleInt = element.style.unitySliceTop;
			num2 = element.computedStyle.unitySliceTop;
			this.unitySliceTop = StyleDiff.ComputeStyleProperty<StyleInt, int>(element, text77, in styleInt, in num2, in context);
			string text78 = "unitySliceType";
			StyleEnum<SliceType> unitySliceType = element.style.unitySliceType;
			SliceType unitySliceType2 = element.computedStyle.unitySliceType;
			this.unitySliceType = StyleDiff.ComputeStyleProperty<StyleEnum<SliceType>, SliceType>(element, text78, in unitySliceType, in unitySliceType2, in context);
			string text79 = "unityTextAlign";
			StyleEnum<TextAnchor> unityTextAlign = element.style.unityTextAlign;
			TextAnchor unityTextAlign2 = element.computedStyle.unityTextAlign;
			this.unityTextAlign = StyleDiff.ComputeStyleProperty<StyleEnum<TextAnchor>, TextAnchor>(element, text79, in unityTextAlign, in unityTextAlign2, in context);
			string text80 = "unityTextAutoSize";
			StyleTextAutoSize unityTextAutoSize = element.style.unityTextAutoSize;
			TextAutoSize unityTextAutoSize2 = element.computedStyle.unityTextAutoSize;
			this.unityTextAutoSize = StyleDiff.ComputeStyleProperty<StyleTextAutoSize, TextAutoSize>(element, text80, in unityTextAutoSize, in unityTextAutoSize2, in context);
			string text81 = "unityTextGenerator";
			StyleEnum<TextGeneratorType> unityTextGenerator = element.style.unityTextGenerator;
			TextGeneratorType unityTextGenerator2 = element.computedStyle.unityTextGenerator;
			this.unityTextGenerator = StyleDiff.ComputeStyleProperty<StyleEnum<TextGeneratorType>, TextGeneratorType>(element, text81, in unityTextGenerator, in unityTextGenerator2, in context);
			string text82 = "unityTextOutlineColor";
			styleColor = element.style.unityTextOutlineColor;
			color = element.computedStyle.unityTextOutlineColor;
			this.unityTextOutlineColor = StyleDiff.ComputeStyleProperty<StyleColor, Color>(element, text82, in styleColor, in color, in context);
			string text83 = "unityTextOutlineWidth";
			styleFloat = element.style.unityTextOutlineWidth;
			num = element.computedStyle.unityTextOutlineWidth;
			this.unityTextOutlineWidth = StyleDiff.ComputeStyleProperty<StyleFloat, float>(element, text83, in styleFloat, in num, in context);
			string text84 = "unityTextOverflowPosition";
			StyleEnum<TextOverflowPosition> unityTextOverflowPosition = element.style.unityTextOverflowPosition;
			TextOverflowPosition unityTextOverflowPosition2 = element.computedStyle.unityTextOverflowPosition;
			this.unityTextOverflowPosition = StyleDiff.ComputeStyleProperty<StyleEnum<TextOverflowPosition>, TextOverflowPosition>(element, text84, in unityTextOverflowPosition, in unityTextOverflowPosition2, in context);
			string text85 = "visibility";
			StyleEnum<Visibility> visibility = element.style.visibility;
			Visibility visibility2 = element.computedStyle.visibility;
			this.visibility = StyleDiff.ComputeStyleProperty<StyleEnum<Visibility>, Visibility>(element, text85, in visibility, in visibility2, in context);
			string text86 = "whiteSpace";
			StyleEnum<WhiteSpace> whiteSpace = element.style.whiteSpace;
			WhiteSpace whiteSpace2 = element.computedStyle.whiteSpace;
			this.whiteSpace = StyleDiff.ComputeStyleProperty<StyleEnum<WhiteSpace>, WhiteSpace>(element, text86, in whiteSpace, in whiteSpace2, in context);
			string text87 = "width";
			styleLength = element.style.width;
			length = element.computedStyle.width;
			this.width = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text87, in styleLength, in length, in context);
			string text88 = "wordSpacing";
			styleLength = element.style.wordSpacing;
			length = element.computedStyle.wordSpacing;
			this.wordSpacing = StyleDiff.ComputeStyleProperty<StyleLength, Length>(element, text88, in styleLength, in length, in context);
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

		public StyleDiff()
		{
			this.m_MatchedRules = new MatchedRulesExtractor(null);
		}

		public void Refresh(VisualElement element, StyleDiffAdditionalDataFlags flags = StyleDiffAdditionalDataFlags.All)
		{
			bool flag = element == null;
			if (!flag)
			{
				VisualTreeAsset visualTreeAssetSource = element.visualTreeAssetSource;
				StyleSheet styleSheet = (visualTreeAssetSource ? visualTreeAssetSource.inlineSheet : null);
				InlineStyleAccess inlineStyleAccess = element.inlineStyleAccess;
				StyleRule styleRule = ((inlineStyleAccess != null) ? inlineStyleAccess.inlineRule.rule : null);
				this.Refresh(element, styleSheet, styleRule, flags);
			}
		}

		internal void Refresh(VisualElement element, StyleSheet styleSheet, StyleRule styleRule, StyleDiffAdditionalDataFlags flags = StyleDiffAdditionalDataFlags.All)
		{
			this.m_MatchedRules.Clear();
			this.uxmlOverrides.Clear();
			Dictionary<string, UxmlData> dictionary;
			using (CollectionPool<Dictionary<string, UxmlData>, KeyValuePair<string, UxmlData>>.Get(out dictionary))
			{
				bool flag = (flags & StyleDiffAdditionalDataFlags.UxmlInlineProperties) == StyleDiffAdditionalDataFlags.UxmlInlineProperties && styleRule != null;
				if (flag)
				{
					foreach (StyleProperty styleProperty in styleRule.properties)
					{
						string text;
						bool flag2 = StylePropertyUtil.ussNameToCSharpName.TryGetValue(styleProperty.name, out text) && text != styleProperty.name;
						if (flag2)
						{
							this.uxmlOverrides.Add(text);
							UxmlData valueOrDefault = dictionary.GetValueOrDefault(text);
							dictionary[text] = UxmlData.WithProperty(in valueOrDefault, styleProperty);
						}
						this.uxmlOverrides.Add(styleProperty.name);
						UxmlData valueOrDefault2 = dictionary.GetValueOrDefault(text);
						dictionary[styleProperty.name] = UxmlData.WithProperty(in valueOrDefault2, styleProperty);
					}
				}
				bool flag3 = (flags & StyleDiffAdditionalDataFlags.Bindings) == StyleDiffAdditionalDataFlags.Bindings;
				if (flag3)
				{
					List<BindingInfo> list;
					using (CollectionPool<List<BindingInfo>, BindingInfo>.Get(out list))
					{
						element.GetBindingInfos(list);
						foreach (BindingInfo bindingInfo in list)
						{
							BindingId bindingId = bindingInfo.bindingId;
							PropertyPath propertyPath = in bindingId;
							bool flag4 = propertyPath.Length == 2 && propertyPath[0].IsName && string.CompareOrdinal(propertyPath[0].Name, "style") == 0 && propertyPath[1].IsName;
							if (flag4)
							{
								string name = propertyPath[1].Name;
								this.uxmlOverrides.Add(name);
								UxmlData valueOrDefault3 = dictionary.GetValueOrDefault(name);
								dictionary[name] = UxmlData.WithBindingInfo(in valueOrDefault3, bindingInfo);
							}
						}
					}
				}
				bool flag5 = (flags & StyleDiffAdditionalDataFlags.Selectors) == StyleDiffAdditionalDataFlags.Selectors;
				if (flag5)
				{
					Dictionary<string, SelectorMatchRecord> dictionary2;
					using (CollectionPool<Dictionary<string, SelectorMatchRecord>, KeyValuePair<string, SelectorMatchRecord>>.Get(out dictionary2))
					{
						this.FindMatchingRules(element, dictionary2);
						foreach (KeyValuePair<string, SelectorMatchRecord> keyValuePair in dictionary2)
						{
							UxmlData valueOrDefault4 = dictionary.GetValueOrDefault(keyValuePair.Key);
							dictionary[keyValuePair.Key] = UxmlData.WithSelector(in valueOrDefault4, keyValuePair.Value);
						}
					}
				}
				StyleDiff.ResolutionContext resolutionContext = new StyleDiff.ResolutionContext(this, styleSheet, dictionary, this.uxmlOverrides);
				this.Refresh(element, in resolutionContext);
			}
		}

		private void FindMatchingRules(VisualElement element, Dictionary<string, SelectorMatchRecord> propertyToMatchRecord)
		{
			this.m_MatchedRules.FindMatchingRules(element);
			for (int i = 0; i < this.m_MatchedRules.matchRecords.Count; i++)
			{
				SelectorMatchRecord selectorMatchRecord = this.m_MatchedRules.matchRecords[i];
				foreach (StyleProperty styleProperty in selectorMatchRecord.complexSelector.rule.properties)
				{
					string text;
					bool flag = StylePropertyUtil.ussNameToCSharpName.TryGetValue(styleProperty.name, out text) && text != styleProperty.name;
					if (flag)
					{
						propertyToMatchRecord[text] = selectorMatchRecord;
					}
					propertyToMatchRecord[styleProperty.name] = selectorMatchRecord;
				}
			}
		}

		private static StylePropertyData<TInline, TComputed> ComputeStyleProperty<TInline, TComputed>(VisualElement element, string propertyName, in TInline inlineStyle, in TComputed computedStyle, in StyleDiff.ResolutionContext context)
		{
			StylePropertyData<TInline, TComputed> stylePropertyData = new StylePropertyData<TInline, TComputed>
			{
				target = element,
				inlineValue = inlineStyle,
				computedValue = computedStyle
			};
			UxmlData uxmlData;
			bool flag = !context.uxmlData.TryGetValue(propertyName, out uxmlData);
			StylePropertyData<TInline, TComputed> stylePropertyData2;
			if (flag)
			{
				context.ClearOverride(propertyName);
				stylePropertyData2 = stylePropertyData;
			}
			else
			{
				bool flag2 = uxmlData.inlineProperty != null;
				stylePropertyData.uxmlValue = (flag2 ? new UxmlStyleProperty(uxmlData.inlineProperty.values, uxmlData.inlineProperty.ContainsVariable()) : new UxmlStyleProperty(Array.Empty<StyleValueHandle>(), false));
				stylePropertyData.binding = uxmlData.bindingInfo.binding;
				bool flag3 = flag2 || uxmlData.bindingInfo.binding != null;
				if (flag3)
				{
					context.MarkAsOverride(propertyName);
				}
				else
				{
					context.ClearOverride(propertyName);
				}
				stylePropertyData.selector = uxmlData.selector;
				stylePropertyData2 = stylePropertyData;
			}
			return stylePropertyData2;
		}

		public bool HasUxmlOverrides(string stylePropertyName)
		{
			return !string.IsNullOrEmpty(stylePropertyName) && this.uxmlOverrides.Contains(stylePropertyName);
		}

		private void Notify([CallerMemberName] string name = null)
		{
			this.m_Version += 1L;
			EventHandler<BindablePropertyChangedEventArgs> eventHandler = this.propertyChanged;
			if (eventHandler != null)
			{
				BindingId bindingId = name;
				eventHandler(this, new BindablePropertyChangedEventArgs(in bindingId));
			}
		}

		public long GetViewHashCode()
		{
			return this.m_Version;
		}

		public void Dispose()
		{
			this.m_MatchedRules.Clear();
			this.uxmlOverrides.Clear();
			this.DisposeProperties();
		}

		private void DisposeProperties()
		{
			this.m_AlignContent.Dispose();
			this.m_AlignItems.Dispose();
			this.m_AlignSelf.Dispose();
			this.m_AspectRatio.Dispose();
			this.m_BackgroundColor.Dispose();
			this.m_BackgroundImage.Dispose();
			this.m_BackgroundPositionX.Dispose();
			this.m_BackgroundPositionY.Dispose();
			this.m_BackgroundRepeat.Dispose();
			this.m_BackgroundSize.Dispose();
			this.m_BorderBottomColor.Dispose();
			this.m_BorderBottomLeftRadius.Dispose();
			this.m_BorderBottomRightRadius.Dispose();
			this.m_BorderBottomWidth.Dispose();
			this.m_BorderLeftColor.Dispose();
			this.m_BorderLeftWidth.Dispose();
			this.m_BorderRightColor.Dispose();
			this.m_BorderRightWidth.Dispose();
			this.m_BorderTopColor.Dispose();
			this.m_BorderTopLeftRadius.Dispose();
			this.m_BorderTopRightRadius.Dispose();
			this.m_BorderTopWidth.Dispose();
			this.m_Bottom.Dispose();
			this.m_Color.Dispose();
			this.m_Cursor.Dispose();
			this.m_Display.Dispose();
			this.m_Filter.Dispose();
			this.m_FlexBasis.Dispose();
			this.m_FlexDirection.Dispose();
			this.m_FlexGrow.Dispose();
			this.m_FlexShrink.Dispose();
			this.m_FlexWrap.Dispose();
			this.m_FontSize.Dispose();
			this.m_Height.Dispose();
			this.m_JustifyContent.Dispose();
			this.m_Left.Dispose();
			this.m_LetterSpacing.Dispose();
			this.m_MarginBottom.Dispose();
			this.m_MarginLeft.Dispose();
			this.m_MarginRight.Dispose();
			this.m_MarginTop.Dispose();
			this.m_MaxHeight.Dispose();
			this.m_MaxWidth.Dispose();
			this.m_MinHeight.Dispose();
			this.m_MinWidth.Dispose();
			this.m_Opacity.Dispose();
			this.m_Overflow.Dispose();
			this.m_PaddingBottom.Dispose();
			this.m_PaddingLeft.Dispose();
			this.m_PaddingRight.Dispose();
			this.m_PaddingTop.Dispose();
			this.m_Position.Dispose();
			this.m_Right.Dispose();
			this.m_Rotate.Dispose();
			this.m_Scale.Dispose();
			this.m_TextOverflow.Dispose();
			this.m_TextShadow.Dispose();
			this.m_Top.Dispose();
			this.m_TransformOrigin.Dispose();
			this.m_TransitionDelay.Dispose();
			this.m_TransitionDuration.Dispose();
			this.m_TransitionProperty.Dispose();
			this.m_TransitionTimingFunction.Dispose();
			this.m_Translate.Dispose();
			this.m_UnityBackgroundImageTintColor.Dispose();
			this.m_UnityEditorTextRenderingMode.Dispose();
			this.m_UnityFont.Dispose();
			this.m_UnityFontDefinition.Dispose();
			this.m_UnityFontStyleAndWeight.Dispose();
			this.m_UnityMaterial.Dispose();
			this.m_UnityOverflowClipBox.Dispose();
			this.m_UnityParagraphSpacing.Dispose();
			this.m_UnitySliceBottom.Dispose();
			this.m_UnitySliceLeft.Dispose();
			this.m_UnitySliceRight.Dispose();
			this.m_UnitySliceScale.Dispose();
			this.m_UnitySliceTop.Dispose();
			this.m_UnitySliceType.Dispose();
			this.m_UnityTextAlign.Dispose();
			this.m_UnityTextAutoSize.Dispose();
			this.m_UnityTextGenerator.Dispose();
			this.m_UnityTextOutlineColor.Dispose();
			this.m_UnityTextOutlineWidth.Dispose();
			this.m_UnityTextOverflowPosition.Dispose();
			this.m_Visibility.Dispose();
			this.m_WhiteSpace.Dispose();
			this.m_Width.Dispose();
			this.m_WordSpacing.Dispose();
		}

		private StylePropertyData<StyleEnum<Align>, Align> m_AlignContent;

		private StylePropertyData<StyleEnum<Align>, Align> m_AlignItems;

		private StylePropertyData<StyleEnum<Align>, Align> m_AlignSelf;

		private StylePropertyData<StyleRatio, Ratio> m_AspectRatio;

		private StylePropertyData<StyleColor, Color> m_BackgroundColor;

		private StylePropertyData<StyleBackground, Background> m_BackgroundImage;

		private StylePropertyData<StyleBackgroundPosition, BackgroundPosition> m_BackgroundPositionX;

		private StylePropertyData<StyleBackgroundPosition, BackgroundPosition> m_BackgroundPositionY;

		private StylePropertyData<StyleBackgroundRepeat, BackgroundRepeat> m_BackgroundRepeat;

		private StylePropertyData<StyleBackgroundSize, BackgroundSize> m_BackgroundSize;

		private StylePropertyData<StyleColor, Color> m_BorderBottomColor;

		private StylePropertyData<StyleLength, Length> m_BorderBottomLeftRadius;

		private StylePropertyData<StyleLength, Length> m_BorderBottomRightRadius;

		private StylePropertyData<StyleFloat, float> m_BorderBottomWidth;

		private StylePropertyData<StyleColor, Color> m_BorderLeftColor;

		private StylePropertyData<StyleFloat, float> m_BorderLeftWidth;

		private StylePropertyData<StyleColor, Color> m_BorderRightColor;

		private StylePropertyData<StyleFloat, float> m_BorderRightWidth;

		private StylePropertyData<StyleColor, Color> m_BorderTopColor;

		private StylePropertyData<StyleLength, Length> m_BorderTopLeftRadius;

		private StylePropertyData<StyleLength, Length> m_BorderTopRightRadius;

		private StylePropertyData<StyleFloat, float> m_BorderTopWidth;

		private StylePropertyData<StyleLength, Length> m_Bottom;

		private StylePropertyData<StyleColor, Color> m_Color;

		private StylePropertyData<StyleCursor, Cursor> m_Cursor;

		private StylePropertyData<StyleEnum<DisplayStyle>, DisplayStyle> m_Display;

		private StylePropertyData<StyleList<FilterFunction>, List<FilterFunction>> m_Filter;

		private StylePropertyData<StyleLength, Length> m_FlexBasis;

		private StylePropertyData<StyleEnum<FlexDirection>, FlexDirection> m_FlexDirection;

		private StylePropertyData<StyleFloat, float> m_FlexGrow;

		private StylePropertyData<StyleFloat, float> m_FlexShrink;

		private StylePropertyData<StyleEnum<Wrap>, Wrap> m_FlexWrap;

		private StylePropertyData<StyleLength, Length> m_FontSize;

		private StylePropertyData<StyleLength, Length> m_Height;

		private StylePropertyData<StyleEnum<Justify>, Justify> m_JustifyContent;

		private StylePropertyData<StyleLength, Length> m_Left;

		private StylePropertyData<StyleLength, Length> m_LetterSpacing;

		private StylePropertyData<StyleLength, Length> m_MarginBottom;

		private StylePropertyData<StyleLength, Length> m_MarginLeft;

		private StylePropertyData<StyleLength, Length> m_MarginRight;

		private StylePropertyData<StyleLength, Length> m_MarginTop;

		private StylePropertyData<StyleLength, Length> m_MaxHeight;

		private StylePropertyData<StyleLength, Length> m_MaxWidth;

		private StylePropertyData<StyleLength, Length> m_MinHeight;

		private StylePropertyData<StyleLength, Length> m_MinWidth;

		private StylePropertyData<StyleFloat, float> m_Opacity;

		private StylePropertyData<StyleEnum<Overflow>, OverflowInternal> m_Overflow;

		private StylePropertyData<StyleLength, Length> m_PaddingBottom;

		private StylePropertyData<StyleLength, Length> m_PaddingLeft;

		private StylePropertyData<StyleLength, Length> m_PaddingRight;

		private StylePropertyData<StyleLength, Length> m_PaddingTop;

		private StylePropertyData<StyleEnum<Position>, Position> m_Position;

		private StylePropertyData<StyleLength, Length> m_Right;

		private StylePropertyData<StyleRotate, Rotate> m_Rotate;

		private StylePropertyData<StyleScale, Scale> m_Scale;

		private StylePropertyData<StyleEnum<TextOverflow>, TextOverflow> m_TextOverflow;

		private StylePropertyData<StyleTextShadow, TextShadow> m_TextShadow;

		private StylePropertyData<StyleLength, Length> m_Top;

		private StylePropertyData<StyleTransformOrigin, TransformOrigin> m_TransformOrigin;

		private StylePropertyData<StyleList<TimeValue>, List<TimeValue>> m_TransitionDelay;

		private StylePropertyData<StyleList<TimeValue>, List<TimeValue>> m_TransitionDuration;

		private StylePropertyData<StyleList<StylePropertyName>, List<StylePropertyName>> m_TransitionProperty;

		private StylePropertyData<StyleList<EasingFunction>, List<EasingFunction>> m_TransitionTimingFunction;

		private StylePropertyData<StyleTranslate, Translate> m_Translate;

		private StylePropertyData<StyleColor, Color> m_UnityBackgroundImageTintColor;

		private StylePropertyData<StyleEnum<EditorTextRenderingMode>, EditorTextRenderingMode> m_UnityEditorTextRenderingMode;

		private StylePropertyData<StyleFont, Font> m_UnityFont;

		private StylePropertyData<StyleFontDefinition, FontDefinition> m_UnityFontDefinition;

		private StylePropertyData<StyleEnum<FontStyle>, FontStyle> m_UnityFontStyleAndWeight;

		private StylePropertyData<StyleMaterialDefinition, MaterialDefinition> m_UnityMaterial;

		private StylePropertyData<StyleEnum<OverflowClipBox>, OverflowClipBox> m_UnityOverflowClipBox;

		private StylePropertyData<StyleLength, Length> m_UnityParagraphSpacing;

		private StylePropertyData<StyleInt, int> m_UnitySliceBottom;

		private StylePropertyData<StyleInt, int> m_UnitySliceLeft;

		private StylePropertyData<StyleInt, int> m_UnitySliceRight;

		private StylePropertyData<StyleFloat, float> m_UnitySliceScale;

		private StylePropertyData<StyleInt, int> m_UnitySliceTop;

		private StylePropertyData<StyleEnum<SliceType>, SliceType> m_UnitySliceType;

		private StylePropertyData<StyleEnum<TextAnchor>, TextAnchor> m_UnityTextAlign;

		private StylePropertyData<StyleTextAutoSize, TextAutoSize> m_UnityTextAutoSize;

		private StylePropertyData<StyleEnum<TextGeneratorType>, TextGeneratorType> m_UnityTextGenerator;

		private StylePropertyData<StyleColor, Color> m_UnityTextOutlineColor;

		private StylePropertyData<StyleFloat, float> m_UnityTextOutlineWidth;

		private StylePropertyData<StyleEnum<TextOverflowPosition>, TextOverflowPosition> m_UnityTextOverflowPosition;

		private StylePropertyData<StyleEnum<Visibility>, Visibility> m_Visibility;

		private StylePropertyData<StyleEnum<WhiteSpace>, WhiteSpace> m_WhiteSpace;

		private StylePropertyData<StyleLength, Length> m_Width;

		private StylePropertyData<StyleLength, Length> m_WordSpacing;

		internal static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Style.StyleDiff", Allocator.Persistent);

		private long m_Version;

		[CreateProperty]
		private readonly HashSet<string> uxmlOverrides = new HashSet<string>();

		private MatchedRulesExtractor m_MatchedRules;

		internal readonly struct ResolutionContext
		{
			public ResolutionContext(StyleDiff diff, StyleSheet inline, Dictionary<string, UxmlData> uxmlData, HashSet<string> uxmlOverrides)
			{
				this.diff = diff;
				this.styleSheet = inline;
				this.uxmlData = uxmlData;
				this.uxmlOverrides = uxmlOverrides;
			}

			public void MarkAsOverride(string name)
			{
				bool flag = this.uxmlOverrides.Add(name);
				if (flag)
				{
					this.diff.Notify(name);
				}
			}

			public void ClearOverride(string name)
			{
				bool flag = this.uxmlOverrides.Remove(name);
				if (flag)
				{
					this.diff.Notify(name);
				}
			}

			public readonly StyleDiff diff;

			public readonly StyleSheet styleSheet;

			public readonly Dictionary<string, UxmlData> uxmlData;

			public readonly HashSet<string> uxmlOverrides;
		}
	}
}
