using System;

namespace UnityEngine.UIElements.UIR
{
	internal class DefaultElementBuilder : BaseElementBuilder
	{
		public DefaultElementBuilder(RenderTreeManager renderTreeManager)
		{
			this.m_RenderTreeManager = renderTreeManager;
		}

		public override bool RequiresStencilMask(VisualElement ve)
		{
			return UIRUtility.IsRoundRect(ve) || UIRUtility.IsVectorImageBackground(ve);
		}

		protected unsafe override void DrawVisualElementBackground(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			RenderData renderData = mgc.renderData;
			bool flag = visualElement.layout.width <= 1E-30f || visualElement.layout.height <= 1E-30f;
			if (!flag)
			{
				ComputedStyle computedStyle = *visualElement.computedStyle;
				Color backgroundColor = computedStyle.backgroundColor;
				renderData.backgroundAlpha = backgroundColor.a;
				bool flag2 = backgroundColor.a > 1E-30f;
				if (flag2)
				{
					MeshGenerator.RectangleParams rectangleParams = new MeshGenerator.RectangleParams
					{
						rect = visualElement.rect,
						uv = new Rect(0f, 0f, 1f, 1f),
						color = backgroundColor,
						colorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.backgroundColorID),
						playmodeTintColor = visualElement.playModeTintColor
					};
					MeshGenerator.GetVisualElementRadii(visualElement, out rectangleParams.topLeftRadius, out rectangleParams.bottomLeftRadius, out rectangleParams.topRightRadius, out rectangleParams.bottomRightRadius);
					MeshGenerator.AdjustBackgroundSizeForBorders(visualElement, ref rectangleParams);
					mgc.meshGenerator.DrawRectangle(rectangleParams);
				}
				Vector4 vector = new Vector4((float)computedStyle.unitySliceLeft, (float)computedStyle.unitySliceTop, (float)computedStyle.unitySliceRight, (float)computedStyle.unitySliceBottom);
				MeshGenerator.RectangleParams rectangleParams2 = default(MeshGenerator.RectangleParams);
				MeshGenerator.GetVisualElementRadii(visualElement, out rectangleParams2.topLeftRadius, out rectangleParams2.bottomLeftRadius, out rectangleParams2.topRightRadius, out rectangleParams2.bottomRightRadius);
				Background backgroundImage = computedStyle.backgroundImage;
				bool flag3 = backgroundImage.texture != null || backgroundImage.sprite != null || backgroundImage.vectorImage != null || backgroundImage.renderTexture != null;
				if (flag3)
				{
					MeshGenerator.RectangleParams rectangleParams3 = default(MeshGenerator.RectangleParams);
					float num = visualElement.resolvedStyle.unitySliceScale;
					Color playModeTintColor = visualElement.playModeTintColor;
					bool flag4;
					ScaleMode scaleMode = BackgroundPropertyHelper.ResolveUnityBackgroundScaleMode(computedStyle.backgroundPositionX, computedStyle.backgroundPositionY, computedStyle.backgroundRepeat, computedStyle.backgroundSize, out flag4);
					bool flag5 = backgroundImage.texture != null;
					if (flag5)
					{
						bool flag6 = Mathf.RoundToInt(vector.x) != 0 || Mathf.RoundToInt(vector.y) != 0 || Mathf.RoundToInt(vector.z) != 0 || Mathf.RoundToInt(vector.w) != 0;
						rectangleParams3 = MeshGenerator.RectangleParams.MakeTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.texture, flag6 ? (flag4 ? scaleMode : ScaleMode.StretchToFill) : ScaleMode.ScaleToFit, playModeTintColor);
						rectangleParams3.rect = new Rect(0f, 0f, (float)rectangleParams3.texture.width, (float)rectangleParams3.texture.height);
					}
					else
					{
						bool flag7 = backgroundImage.sprite != null;
						if (flag7)
						{
							bool flag8 = !flag4 || scaleMode == ScaleMode.ScaleAndCrop;
							rectangleParams3 = MeshGenerator.RectangleParams.MakeSprite(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.sprite, flag8 ? ScaleMode.StretchToFill : scaleMode, playModeTintColor, rectangleParams2.HasRadius(0.001f), ref vector, flag8);
							bool flag9 = rectangleParams3.texture != null;
							if (flag9)
							{
								rectangleParams3.rect = new Rect(0f, 0f, backgroundImage.sprite.rect.width, backgroundImage.sprite.rect.height);
							}
							num *= UIElementsUtility.PixelsPerUnitScaleForElement(visualElement, backgroundImage.sprite);
						}
						else
						{
							bool flag10 = backgroundImage.renderTexture != null;
							if (flag10)
							{
								rectangleParams3 = MeshGenerator.RectangleParams.MakeTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.renderTexture, ScaleMode.ScaleToFit, playModeTintColor);
								rectangleParams3.rect = new Rect(0f, 0f, (float)rectangleParams3.texture.width, (float)rectangleParams3.texture.height);
							}
							else
							{
								bool flag11 = backgroundImage.vectorImage != null;
								if (flag11)
								{
									bool flag12 = !flag4 || scaleMode == ScaleMode.ScaleAndCrop;
									rectangleParams3 = MeshGenerator.RectangleParams.MakeVectorTextured(visualElement.rect, new Rect(0f, 0f, 1f, 1f), backgroundImage.vectorImage, flag12 ? ScaleMode.StretchToFill : scaleMode, playModeTintColor);
									rectangleParams3.rect = new Rect(0f, 0f, rectangleParams3.vectorImage.size.x, rectangleParams3.vectorImage.size.y);
								}
							}
						}
					}
					rectangleParams3.topLeftRadius = rectangleParams2.topLeftRadius;
					rectangleParams3.topRightRadius = rectangleParams2.topRightRadius;
					rectangleParams3.bottomRightRadius = rectangleParams2.bottomRightRadius;
					rectangleParams3.bottomLeftRadius = rectangleParams2.bottomLeftRadius;
					bool flag13 = vector != Vector4.zero;
					if (flag13)
					{
						rectangleParams3.leftSlice = Mathf.RoundToInt(vector.x);
						rectangleParams3.topSlice = Mathf.RoundToInt(vector.y);
						rectangleParams3.rightSlice = Mathf.RoundToInt(vector.z);
						rectangleParams3.bottomSlice = Mathf.RoundToInt(vector.w);
						rectangleParams3.sliceScale = num;
						bool flag14 = computedStyle.unitySliceType == SliceType.Tiled;
						if (flag14)
						{
							rectangleParams3.meshFlags |= MeshGenerationContext.MeshFlags.SliceTiled;
						}
						bool flag15 = !flag4;
						if (flag15)
						{
							rectangleParams3.backgroundPositionX = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill);
							rectangleParams3.backgroundPositionY = BackgroundPropertyHelper.ConvertScaleModeToBackgroundPosition(ScaleMode.StretchToFill);
							rectangleParams3.backgroundRepeat = BackgroundPropertyHelper.ConvertScaleModeToBackgroundRepeat(ScaleMode.StretchToFill);
							rectangleParams3.backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(ScaleMode.StretchToFill);
						}
						else
						{
							rectangleParams3.backgroundPositionX = computedStyle.backgroundPositionX;
							rectangleParams3.backgroundPositionY = computedStyle.backgroundPositionY;
							rectangleParams3.backgroundRepeat = computedStyle.backgroundRepeat;
							rectangleParams3.backgroundSize = computedStyle.backgroundSize;
						}
					}
					else
					{
						rectangleParams3.backgroundPositionX = computedStyle.backgroundPositionX;
						rectangleParams3.backgroundPositionY = computedStyle.backgroundPositionY;
						rectangleParams3.backgroundRepeat = computedStyle.backgroundRepeat;
						rectangleParams3.backgroundSize = computedStyle.backgroundSize;
					}
					rectangleParams3.color = computedStyle.unityBackgroundImageTintColor;
					rectangleParams3.colorPage = ColorPage.Init(this.m_RenderTreeManager, visualElement.renderData.tintColorID);
					MeshGenerator.AdjustBackgroundSizeForBorders(visualElement, ref rectangleParams3);
					bool flag16 = rectangleParams3.texture != null || rectangleParams3.vectorImage != null;
					if (flag16)
					{
						mgc.meshGenerator.DrawRectangleRepeat(rectangleParams3, visualElement.rect, visualElement.scaledPixelsPerPoint);
					}
					else
					{
						mgc.meshGenerator.DrawRectangle(rectangleParams3);
					}
				}
			}
		}

		protected override void DrawVisualElementBorder(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			RenderData renderData = mgc.renderData;
			bool flag = visualElement.layout.width >= 1E-30f && visualElement.layout.height >= 1E-30f;
			if (flag)
			{
				IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
				bool flag2 = (resolvedStyle.borderLeftColor != Color.clear && resolvedStyle.borderLeftWidth > 0f) || (resolvedStyle.borderTopColor != Color.clear && resolvedStyle.borderTopWidth > 0f) || (resolvedStyle.borderRightColor != Color.clear && resolvedStyle.borderRightWidth > 0f) || (resolvedStyle.borderBottomColor != Color.clear && resolvedStyle.borderBottomWidth > 0f);
				if (flag2)
				{
					MeshGenerator.BorderParams borderParams = new MeshGenerator.BorderParams
					{
						rect = visualElement.rect,
						leftColor = resolvedStyle.borderLeftColor,
						topColor = resolvedStyle.borderTopColor,
						rightColor = resolvedStyle.borderRightColor,
						bottomColor = resolvedStyle.borderBottomColor,
						leftWidth = resolvedStyle.borderLeftWidth,
						topWidth = resolvedStyle.borderTopWidth,
						rightWidth = resolvedStyle.borderRightWidth,
						bottomWidth = resolvedStyle.borderBottomWidth,
						leftColorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.borderLeftColorID),
						topColorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.borderTopColorID),
						rightColorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.borderRightColorID),
						bottomColorPage = ColorPage.Init(this.m_RenderTreeManager, renderData.borderBottomColorID),
						playmodeTintColor = visualElement.playModeTintColor
					};
					MeshGenerator.GetVisualElementRadii(visualElement, out borderParams.topLeftRadius, out borderParams.bottomLeftRadius, out borderParams.topRightRadius, out borderParams.bottomRightRadius);
					mgc.meshGenerator.DrawBorder(borderParams);
				}
			}
		}

		protected override void DrawVisualElementStencilMask(MeshGenerationContext mgc)
		{
			bool flag = UIRUtility.IsVectorImageBackground(mgc.visualElement);
			if (flag)
			{
				this.DrawVisualElementBackground(mgc);
			}
			else
			{
				DefaultElementBuilder.GenerateStencilClipEntryForRoundedRectBackground(mgc);
			}
		}

		private static void GenerateStencilClipEntryForRoundedRectBackground(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			bool flag = visualElement.layout.width <= 1E-30f || visualElement.layout.height <= 1E-30f;
			if (!flag)
			{
				IResolvedStyle resolvedStyle = visualElement.resolvedStyle;
				Vector2 vector;
				Vector2 vector2;
				Vector2 vector3;
				Vector2 vector4;
				MeshGenerator.GetVisualElementRadii(visualElement, out vector, out vector2, out vector3, out vector4);
				float borderTopWidth = resolvedStyle.borderTopWidth;
				float borderLeftWidth = resolvedStyle.borderLeftWidth;
				float borderBottomWidth = resolvedStyle.borderBottomWidth;
				float borderRightWidth = resolvedStyle.borderRightWidth;
				MeshGenerator.RectangleParams rectangleParams = new MeshGenerator.RectangleParams
				{
					rect = visualElement.rect,
					color = Color.white,
					topLeftRadius = Vector2.Max(Vector2.zero, vector - new Vector2(borderLeftWidth, borderTopWidth)),
					topRightRadius = Vector2.Max(Vector2.zero, vector3 - new Vector2(borderRightWidth, borderTopWidth)),
					bottomLeftRadius = Vector2.Max(Vector2.zero, vector2 - new Vector2(borderLeftWidth, borderBottomWidth)),
					bottomRightRadius = Vector2.Max(Vector2.zero, vector4 - new Vector2(borderRightWidth, borderBottomWidth)),
					playmodeTintColor = visualElement.playModeTintColor
				};
				rectangleParams.rect.x = rectangleParams.rect.x + borderLeftWidth;
				rectangleParams.rect.y = rectangleParams.rect.y + borderTopWidth;
				rectangleParams.rect.width = rectangleParams.rect.width - (borderLeftWidth + borderRightWidth);
				rectangleParams.rect.height = rectangleParams.rect.height - (borderTopWidth + borderBottomWidth);
				bool flag2 = visualElement.computedStyle.unityOverflowClipBox == OverflowClipBox.ContentBox;
				if (flag2)
				{
					rectangleParams.rect.x = rectangleParams.rect.x + resolvedStyle.paddingLeft;
					rectangleParams.rect.y = rectangleParams.rect.y + resolvedStyle.paddingTop;
					rectangleParams.rect.width = rectangleParams.rect.width - (resolvedStyle.paddingLeft + resolvedStyle.paddingRight);
					rectangleParams.rect.height = rectangleParams.rect.height - (resolvedStyle.paddingTop + resolvedStyle.paddingBottom);
				}
				mgc.meshGenerator.DrawRectangle(rectangleParams);
			}
		}

		public override void ScheduleMeshGenerationJobs(MeshGenerationContext mgc)
		{
			mgc.meshGenerator.ScheduleJobs(mgc);
			bool hasPainter2D = mgc.hasPainter2D;
			if (hasPainter2D)
			{
				mgc.painter2D.ScheduleJobs(mgc);
			}
		}

		private RenderTreeManager m_RenderTreeManager;
	}
}
