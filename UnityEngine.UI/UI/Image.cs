using System;
using UnityEngine.Serialization;
using UnityEngine.Sprites;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Image", 11)]
	public class Image : MaskableGraphic, ISerializationCallbackReceiver, ILayoutElement, ICanvasRaycastFilter
	{
		protected Image()
		{
			base.useLegacyMeshGeneration = false;
		}

		public Sprite sprite
		{
			get
			{
				return this.m_Sprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
				{
					this.SetAllDirty();
				}
			}
		}

		public Sprite overrideSprite
		{
			get
			{
				return this.activeSprite;
			}
			set
			{
				if (SetPropertyUtility.SetClass<Sprite>(ref this.m_OverrideSprite, value))
				{
					this.SetAllDirty();
				}
			}
		}

		private Sprite activeSprite
		{
			get
			{
				return (!(this.m_OverrideSprite != null)) ? this.sprite : this.m_OverrideSprite;
			}
		}

		public Image.Type type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<Image.Type>(ref this.m_Type, value))
				{
					this.SetVerticesDirty();
				}
			}
		}

		public bool preserveAspect
		{
			get
			{
				return this.m_PreserveAspect;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<bool>(ref this.m_PreserveAspect, value))
				{
					this.SetVerticesDirty();
				}
			}
		}

		public bool fillCenter
		{
			get
			{
				return this.m_FillCenter;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<bool>(ref this.m_FillCenter, value))
				{
					this.SetVerticesDirty();
				}
			}
		}

		public Image.FillMethod fillMethod
		{
			get
			{
				return this.m_FillMethod;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<Image.FillMethod>(ref this.m_FillMethod, value))
				{
					this.SetVerticesDirty();
					this.m_FillOrigin = 0;
				}
			}
		}

		public float fillAmount
		{
			get
			{
				return this.m_FillAmount;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_FillAmount, Mathf.Clamp01(value)))
				{
					this.SetVerticesDirty();
				}
			}
		}

		public bool fillClockwise
		{
			get
			{
				return this.m_FillClockwise;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<bool>(ref this.m_FillClockwise, value))
				{
					this.SetVerticesDirty();
				}
			}
		}

		public int fillOrigin
		{
			get
			{
				return this.m_FillOrigin;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_FillOrigin, value))
				{
					this.SetVerticesDirty();
				}
			}
		}

		[Obsolete("eventAlphaThreshold has been deprecated. Use eventMinimumAlphaThreshold instead (UnityUpgradable) -> alphaHitTestMinimumThreshold")]
		public float eventAlphaThreshold
		{
			get
			{
				return 1f - this.alphaHitTestMinimumThreshold;
			}
			set
			{
				this.alphaHitTestMinimumThreshold = 1f - value;
			}
		}

		public float alphaHitTestMinimumThreshold
		{
			get
			{
				return this.m_AlphaHitTestMinimumThreshold;
			}
			set
			{
				this.m_AlphaHitTestMinimumThreshold = value;
			}
		}

		public static Material defaultETC1GraphicMaterial
		{
			get
			{
				if (Image.s_ETC1DefaultUI == null)
				{
					Image.s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
				}
				return Image.s_ETC1DefaultUI;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				Texture texture;
				if (this.activeSprite == null)
				{
					if (this.material != null && this.material.mainTexture != null)
					{
						texture = this.material.mainTexture;
					}
					else
					{
						texture = Graphic.s_WhiteTexture;
					}
				}
				else
				{
					texture = this.activeSprite.texture;
				}
				return texture;
			}
		}

		public bool hasBorder
		{
			get
			{
				return this.activeSprite != null && this.activeSprite.border.sqrMagnitude > 0f;
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				float num = 100f;
				if (this.activeSprite)
				{
					num = this.activeSprite.pixelsPerUnit;
				}
				float num2 = 100f;
				if (base.canvas)
				{
					num2 = base.canvas.referencePixelsPerUnit;
				}
				return num / num2;
			}
		}

		public override Material material
		{
			get
			{
				Material material;
				if (this.m_Material != null)
				{
					material = this.m_Material;
				}
				else if (this.activeSprite && this.activeSprite.associatedAlphaSplitTexture != null)
				{
					material = Image.defaultETC1GraphicMaterial;
				}
				else
				{
					material = this.defaultMaterial;
				}
				return material;
			}
			set
			{
				base.material = value;
			}
		}

		public virtual void OnBeforeSerialize()
		{
		}

		public virtual void OnAfterDeserialize()
		{
			if (this.m_FillOrigin < 0)
			{
				this.m_FillOrigin = 0;
			}
			else if (this.m_FillMethod == Image.FillMethod.Horizontal && this.m_FillOrigin > 1)
			{
				this.m_FillOrigin = 0;
			}
			else if (this.m_FillMethod == Image.FillMethod.Vertical && this.m_FillOrigin > 1)
			{
				this.m_FillOrigin = 0;
			}
			else if (this.m_FillOrigin > 3)
			{
				this.m_FillOrigin = 0;
			}
			this.m_FillAmount = Mathf.Clamp(this.m_FillAmount, 0f, 1f);
		}

		private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
		{
			Vector4 vector = ((!(this.activeSprite == null)) ? DataUtility.GetPadding(this.activeSprite) : Vector4.zero);
			Vector2 vector2 = ((!(this.activeSprite == null)) ? new Vector2(this.activeSprite.rect.width, this.activeSprite.rect.height) : Vector2.zero);
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			int num = Mathf.RoundToInt(vector2.x);
			int num2 = Mathf.RoundToInt(vector2.y);
			Vector4 vector3 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
			if (shouldPreserveAspect && vector2.sqrMagnitude > 0f)
			{
				float num3 = vector2.x / vector2.y;
				float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
				if (num3 > num4)
				{
					float height = pixelAdjustedRect.height;
					pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
					pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
				}
				else
				{
					float width = pixelAdjustedRect.width;
					pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
					pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
				}
			}
			vector3 = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.w);
			return vector3;
		}

		public override void SetNativeSize()
		{
			if (this.activeSprite != null)
			{
				float num = this.activeSprite.rect.width / this.pixelsPerUnit;
				float num2 = this.activeSprite.rect.height / this.pixelsPerUnit;
				base.rectTransform.anchorMax = base.rectTransform.anchorMin;
				base.rectTransform.sizeDelta = new Vector2(num, num2);
				this.SetAllDirty();
			}
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (this.activeSprite == null)
			{
				base.OnPopulateMesh(toFill);
			}
			else
			{
				switch (this.type)
				{
				case Image.Type.Simple:
					this.GenerateSimpleSprite(toFill, this.m_PreserveAspect);
					break;
				case Image.Type.Sliced:
					this.GenerateSlicedSprite(toFill);
					break;
				case Image.Type.Tiled:
					this.GenerateTiledSprite(toFill);
					break;
				case Image.Type.Filled:
					this.GenerateFilledSprite(toFill, this.m_PreserveAspect);
					break;
				}
			}
		}

		protected override void UpdateMaterial()
		{
			base.UpdateMaterial();
			if (this.activeSprite == null)
			{
				base.canvasRenderer.SetAlphaTexture(null);
			}
			else
			{
				Texture2D associatedAlphaSplitTexture = this.activeSprite.associatedAlphaSplitTexture;
				if (associatedAlphaSplitTexture != null)
				{
					base.canvasRenderer.SetAlphaTexture(associatedAlphaSplitTexture);
				}
			}
		}

		private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
		{
			Vector4 drawingDimensions = this.GetDrawingDimensions(lPreserveAspect);
			Vector4 vector = ((!(this.activeSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(this.activeSprite));
			Color color = this.color;
			vh.Clear();
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector.x, vector.y));
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector.x, vector.w));
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector.z, vector.w));
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector.z, vector.y));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		private void GenerateSlicedSprite(VertexHelper toFill)
		{
			if (!this.hasBorder)
			{
				this.GenerateSimpleSprite(toFill, false);
			}
			else
			{
				Vector4 vector;
				Vector4 vector2;
				Vector4 vector3;
				Vector4 vector4;
				if (this.activeSprite != null)
				{
					vector = DataUtility.GetOuterUV(this.activeSprite);
					vector2 = DataUtility.GetInnerUV(this.activeSprite);
					vector3 = DataUtility.GetPadding(this.activeSprite);
					vector4 = this.activeSprite.border;
				}
				else
				{
					vector = Vector4.zero;
					vector2 = Vector4.zero;
					vector3 = Vector4.zero;
					vector4 = Vector4.zero;
				}
				Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
				Vector4 adjustedBorders = this.GetAdjustedBorders(vector4 / this.pixelsPerUnit, pixelAdjustedRect);
				vector3 /= this.pixelsPerUnit;
				Image.s_VertScratch[0] = new Vector2(vector3.x, vector3.y);
				Image.s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - vector3.z, pixelAdjustedRect.height - vector3.w);
				Image.s_VertScratch[1].x = adjustedBorders.x;
				Image.s_VertScratch[1].y = adjustedBorders.y;
				Image.s_VertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
				Image.s_VertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
				for (int i = 0; i < 4; i++)
				{
					Vector2[] array = Image.s_VertScratch;
					int num = i;
					array[num].x = array[num].x + pixelAdjustedRect.x;
					Vector2[] array2 = Image.s_VertScratch;
					int num2 = i;
					array2[num2].y = array2[num2].y + pixelAdjustedRect.y;
				}
				Image.s_UVScratch[0] = new Vector2(vector.x, vector.y);
				Image.s_UVScratch[1] = new Vector2(vector2.x, vector2.y);
				Image.s_UVScratch[2] = new Vector2(vector2.z, vector2.w);
				Image.s_UVScratch[3] = new Vector2(vector.z, vector.w);
				toFill.Clear();
				for (int j = 0; j < 3; j++)
				{
					int num3 = j + 1;
					for (int k = 0; k < 3; k++)
					{
						if (this.m_FillCenter || j != 1 || k != 1)
						{
							int num4 = k + 1;
							Image.AddQuad(toFill, new Vector2(Image.s_VertScratch[j].x, Image.s_VertScratch[k].y), new Vector2(Image.s_VertScratch[num3].x, Image.s_VertScratch[num4].y), this.color, new Vector2(Image.s_UVScratch[j].x, Image.s_UVScratch[k].y), new Vector2(Image.s_UVScratch[num3].x, Image.s_UVScratch[num4].y));
						}
					}
				}
			}
		}

		private void GenerateTiledSprite(VertexHelper toFill)
		{
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector2 vector4;
			if (this.activeSprite != null)
			{
				vector = DataUtility.GetOuterUV(this.activeSprite);
				vector2 = DataUtility.GetInnerUV(this.activeSprite);
				vector3 = this.activeSprite.border;
				vector4 = this.activeSprite.rect.size;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector2.one * 100f;
			}
			Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
			float num = (vector4.x - vector3.x - vector3.z) / this.pixelsPerUnit;
			float num2 = (vector4.y - vector3.y - vector3.w) / this.pixelsPerUnit;
			vector3 = this.GetAdjustedBorders(vector3 / this.pixelsPerUnit, pixelAdjustedRect);
			Vector2 vector5 = new Vector2(vector2.x, vector2.y);
			Vector2 vector6 = new Vector2(vector2.z, vector2.w);
			float x = vector3.x;
			float num3 = pixelAdjustedRect.width - vector3.z;
			float y = vector3.y;
			float num4 = pixelAdjustedRect.height - vector3.w;
			toFill.Clear();
			Vector2 vector7 = vector6;
			if (num <= 0f)
			{
				num = num3 - x;
			}
			if (num2 <= 0f)
			{
				num2 = num4 - y;
			}
			if (this.activeSprite != null && (this.hasBorder || this.activeSprite.packed || this.activeSprite.texture.wrapMode != TextureWrapMode.Repeat))
			{
				long num5;
				long num6;
				if (this.m_FillCenter)
				{
					num5 = (long)Math.Ceiling((double)((num3 - x) / num));
					num6 = (long)Math.Ceiling((double)((num4 - y) / num2));
					double num7;
					if (this.hasBorder)
					{
						num7 = ((double)num5 + 2.0) * ((double)num6 + 2.0) * 4.0;
					}
					else
					{
						num7 = (double)(num5 * num6) * 4.0;
					}
					if (num7 > 65000.0)
					{
						Debug.LogError("Too many sprite tiles on Image \"" + base.name + "\". The tile size will be increased. To remove the limit on the number of tiles, convert the Sprite to an Advanced texture, remove the borders, clear the Packing tag and set the Wrap mode to Repeat.", this);
						double num8 = 16250.0;
						double num9;
						if (this.hasBorder)
						{
							num9 = ((double)num5 + 2.0) / ((double)num6 + 2.0);
						}
						else
						{
							num9 = (double)num5 / (double)num6;
						}
						double num10 = Math.Sqrt(num8 / num9);
						double num11 = num10 * num9;
						if (this.hasBorder)
						{
							num10 -= 2.0;
							num11 -= 2.0;
						}
						num5 = (long)Math.Floor(num10);
						num6 = (long)Math.Floor(num11);
						num = (num3 - x) / (float)num5;
						num2 = (num4 - y) / (float)num6;
					}
				}
				else if (this.hasBorder)
				{
					num5 = (long)Math.Ceiling((double)((num3 - x) / num));
					num6 = (long)Math.Ceiling((double)((num4 - y) / num2));
					double num12 = ((double)(num6 + num5) + 2.0) * 2.0 * 4.0;
					if (num12 > 65000.0)
					{
						Debug.LogError("Too many sprite tiles on Image \"" + base.name + "\". The tile size will be increased. To remove the limit on the number of tiles, convert the Sprite to an Advanced texture, remove the borders, clear the Packing tag and set the Wrap mode to Repeat.", this);
						double num13 = 16250.0;
						double num14 = (double)num5 / (double)num6;
						double num15 = (num13 - 4.0) / (2.0 * (1.0 + num14));
						double num16 = num15 * num14;
						num5 = (long)Math.Floor(num15);
						num6 = (long)Math.Floor(num16);
						num = (num3 - x) / (float)num5;
						num2 = (num4 - y) / (float)num6;
					}
				}
				else
				{
					num5 = (num6 = 0L);
				}
				if (this.m_FillCenter)
				{
					for (long num17 = 0L; num17 < num6; num17 += 1L)
					{
						float num18 = y + (float)num17 * num2;
						float num19 = y + (float)(num17 + 1L) * num2;
						if (num19 > num4)
						{
							vector7.y = vector5.y + (vector6.y - vector5.y) * (num4 - num18) / (num19 - num18);
							num19 = num4;
						}
						vector7.x = vector6.x;
						for (long num20 = 0L; num20 < num5; num20 += 1L)
						{
							float num21 = x + (float)num20 * num;
							float num22 = x + (float)(num20 + 1L) * num;
							if (num22 > num3)
							{
								vector7.x = vector5.x + (vector6.x - vector5.x) * (num3 - num21) / (num22 - num21);
								num22 = num3;
							}
							Image.AddQuad(toFill, new Vector2(num21, num18) + pixelAdjustedRect.position, new Vector2(num22, num19) + pixelAdjustedRect.position, this.color, vector5, vector7);
						}
					}
				}
				if (this.hasBorder)
				{
					vector7 = vector6;
					for (long num23 = 0L; num23 < num6; num23 += 1L)
					{
						float num24 = y + (float)num23 * num2;
						float num25 = y + (float)(num23 + 1L) * num2;
						if (num25 > num4)
						{
							vector7.y = vector5.y + (vector6.y - vector5.y) * (num4 - num24) / (num25 - num24);
							num25 = num4;
						}
						Image.AddQuad(toFill, new Vector2(0f, num24) + pixelAdjustedRect.position, new Vector2(x, num25) + pixelAdjustedRect.position, this.color, new Vector2(vector.x, vector5.y), new Vector2(vector5.x, vector7.y));
						Image.AddQuad(toFill, new Vector2(num3, num24) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num25) + pixelAdjustedRect.position, this.color, new Vector2(vector6.x, vector5.y), new Vector2(vector.z, vector7.y));
					}
					vector7 = vector6;
					for (long num26 = 0L; num26 < num5; num26 += 1L)
					{
						float num27 = x + (float)num26 * num;
						float num28 = x + (float)(num26 + 1L) * num;
						if (num28 > num3)
						{
							vector7.x = vector5.x + (vector6.x - vector5.x) * (num3 - num27) / (num28 - num27);
							num28 = num3;
						}
						Image.AddQuad(toFill, new Vector2(num27, 0f) + pixelAdjustedRect.position, new Vector2(num28, y) + pixelAdjustedRect.position, this.color, new Vector2(vector5.x, vector.y), new Vector2(vector7.x, vector5.y));
						Image.AddQuad(toFill, new Vector2(num27, num4) + pixelAdjustedRect.position, new Vector2(num28, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(vector5.x, vector6.y), new Vector2(vector7.x, vector.w));
					}
					Image.AddQuad(toFill, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, this.color, new Vector2(vector.x, vector.y), new Vector2(vector5.x, vector5.y));
					Image.AddQuad(toFill, new Vector2(num3, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, this.color, new Vector2(vector6.x, vector.y), new Vector2(vector.z, vector5.y));
					Image.AddQuad(toFill, new Vector2(0f, num4) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(vector.x, vector6.y), new Vector2(vector5.x, vector.w));
					Image.AddQuad(toFill, new Vector2(num3, num4) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, this.color, new Vector2(vector6.x, vector6.y), new Vector2(vector.z, vector.w));
				}
			}
			else
			{
				Vector2 vector8 = new Vector2((num3 - x) / num, (num4 - y) / num2);
				if (this.m_FillCenter)
				{
					Image.AddQuad(toFill, new Vector2(x, y) + pixelAdjustedRect.position, new Vector2(num3, num4) + pixelAdjustedRect.position, this.color, Vector2.Scale(vector5, vector8), Vector2.Scale(vector6, vector8));
				}
			}
		}

		private static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			for (int i = 0; i < 4; i++)
			{
				vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);
			}
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		private static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0f), color, new Vector2(uvMin.x, uvMin.y));
			vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0f), color, new Vector2(uvMin.x, uvMax.y));
			vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0f), color, new Vector2(uvMax.x, uvMax.y));
			vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0f), color, new Vector2(uvMax.x, uvMin.y));
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
		{
			Rect rect = base.rectTransform.rect;
			for (int i = 0; i <= 1; i++)
			{
				if (rect.size[i] != 0f)
				{
					float num = adjustedRect.size[i] / rect.size[i];
					ref Vector4 ptr = ref border;
					int num2;
					border[num2 = i] = ptr[num2] * num;
					ptr = ref border;
					int num3;
					border[num3 = i + 2] = ptr[num3] * num;
				}
				float num4 = border[i] + border[i + 2];
				if (adjustedRect.size[i] < num4 && num4 != 0f)
				{
					float num = adjustedRect.size[i] / num4;
					ref Vector4 ptr = ref border;
					int num5;
					border[num5 = i] = ptr[num5] * num;
					ptr = ref border;
					int num6;
					border[num6 = i + 2] = ptr[num6] * num;
				}
			}
			return border;
		}

		private void GenerateFilledSprite(VertexHelper toFill, bool preserveAspect)
		{
			toFill.Clear();
			if (this.m_FillAmount >= 0.001f)
			{
				Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect);
				Vector4 vector = ((!(this.activeSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(this.activeSprite));
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = this.color;
				float num = vector.x;
				float num2 = vector.y;
				float num3 = vector.z;
				float num4 = vector.w;
				if (this.m_FillMethod == Image.FillMethod.Horizontal || this.m_FillMethod == Image.FillMethod.Vertical)
				{
					if (this.fillMethod == Image.FillMethod.Horizontal)
					{
						float num5 = (num3 - num) * this.m_FillAmount;
						if (this.m_FillOrigin == 1)
						{
							drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * this.m_FillAmount;
							num = num3 - num5;
						}
						else
						{
							drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * this.m_FillAmount;
							num3 = num + num5;
						}
					}
					else if (this.fillMethod == Image.FillMethod.Vertical)
					{
						float num6 = (num4 - num2) * this.m_FillAmount;
						if (this.m_FillOrigin == 1)
						{
							drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * this.m_FillAmount;
							num2 = num4 - num6;
						}
						else
						{
							drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * this.m_FillAmount;
							num4 = num2 + num6;
						}
					}
				}
				Image.s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
				Image.s_Xy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
				Image.s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
				Image.s_Xy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
				Image.s_Uv[0] = new Vector2(num, num2);
				Image.s_Uv[1] = new Vector2(num, num4);
				Image.s_Uv[2] = new Vector2(num3, num4);
				Image.s_Uv[3] = new Vector2(num3, num2);
				if (this.m_FillAmount < 1f && this.m_FillMethod != Image.FillMethod.Horizontal && this.m_FillMethod != Image.FillMethod.Vertical)
				{
					if (this.fillMethod == Image.FillMethod.Radial90)
					{
						if (Image.RadialCut(Image.s_Xy, Image.s_Uv, this.m_FillAmount, this.m_FillClockwise, this.m_FillOrigin))
						{
							Image.AddQuad(toFill, Image.s_Xy, this.color, Image.s_Uv);
						}
					}
					else if (this.fillMethod == Image.FillMethod.Radial180)
					{
						for (int i = 0; i < 2; i++)
						{
							int num7 = ((this.m_FillOrigin <= 1) ? 0 : 1);
							float num8;
							float num9;
							float num10;
							float num11;
							if (this.m_FillOrigin == 0 || this.m_FillOrigin == 2)
							{
								num8 = 0f;
								num9 = 1f;
								if (i == num7)
								{
									num10 = 0f;
									num11 = 0.5f;
								}
								else
								{
									num10 = 0.5f;
									num11 = 1f;
								}
							}
							else
							{
								num10 = 0f;
								num11 = 1f;
								if (i == num7)
								{
									num8 = 0.5f;
									num9 = 1f;
								}
								else
								{
									num8 = 0f;
									num9 = 0.5f;
								}
							}
							Image.s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num10);
							Image.s_Xy[1].x = Image.s_Xy[0].x;
							Image.s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num11);
							Image.s_Xy[3].x = Image.s_Xy[2].x;
							Image.s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num8);
							Image.s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num9);
							Image.s_Xy[2].y = Image.s_Xy[1].y;
							Image.s_Xy[3].y = Image.s_Xy[0].y;
							Image.s_Uv[0].x = Mathf.Lerp(num, num3, num10);
							Image.s_Uv[1].x = Image.s_Uv[0].x;
							Image.s_Uv[2].x = Mathf.Lerp(num, num3, num11);
							Image.s_Uv[3].x = Image.s_Uv[2].x;
							Image.s_Uv[0].y = Mathf.Lerp(num2, num4, num8);
							Image.s_Uv[1].y = Mathf.Lerp(num2, num4, num9);
							Image.s_Uv[2].y = Image.s_Uv[1].y;
							Image.s_Uv[3].y = Image.s_Uv[0].y;
							float num12 = ((!this.m_FillClockwise) ? (this.m_FillAmount * 2f - (float)(1 - i)) : (this.fillAmount * 2f - (float)i));
							if (Image.RadialCut(Image.s_Xy, Image.s_Uv, Mathf.Clamp01(num12), this.m_FillClockwise, (i + this.m_FillOrigin + 3) % 4))
							{
								Image.AddQuad(toFill, Image.s_Xy, this.color, Image.s_Uv);
							}
						}
					}
					else if (this.fillMethod == Image.FillMethod.Radial360)
					{
						for (int j = 0; j < 4; j++)
						{
							float num13;
							float num14;
							if (j < 2)
							{
								num13 = 0f;
								num14 = 0.5f;
							}
							else
							{
								num13 = 0.5f;
								num14 = 1f;
							}
							float num15;
							float num16;
							if (j == 0 || j == 3)
							{
								num15 = 0f;
								num16 = 0.5f;
							}
							else
							{
								num15 = 0.5f;
								num16 = 1f;
							}
							Image.s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num13);
							Image.s_Xy[1].x = Image.s_Xy[0].x;
							Image.s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num14);
							Image.s_Xy[3].x = Image.s_Xy[2].x;
							Image.s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num15);
							Image.s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num16);
							Image.s_Xy[2].y = Image.s_Xy[1].y;
							Image.s_Xy[3].y = Image.s_Xy[0].y;
							Image.s_Uv[0].x = Mathf.Lerp(num, num3, num13);
							Image.s_Uv[1].x = Image.s_Uv[0].x;
							Image.s_Uv[2].x = Mathf.Lerp(num, num3, num14);
							Image.s_Uv[3].x = Image.s_Uv[2].x;
							Image.s_Uv[0].y = Mathf.Lerp(num2, num4, num15);
							Image.s_Uv[1].y = Mathf.Lerp(num2, num4, num16);
							Image.s_Uv[2].y = Image.s_Uv[1].y;
							Image.s_Uv[3].y = Image.s_Uv[0].y;
							float num17 = ((!this.m_FillClockwise) ? (this.m_FillAmount * 4f - (float)(3 - (j + this.m_FillOrigin) % 4)) : (this.m_FillAmount * 4f - (float)((j + this.m_FillOrigin) % 4)));
							if (Image.RadialCut(Image.s_Xy, Image.s_Uv, Mathf.Clamp01(num17), this.m_FillClockwise, (j + 2) % 4))
							{
								Image.AddQuad(toFill, Image.s_Xy, this.color, Image.s_Uv);
							}
						}
					}
				}
				else
				{
					Image.AddQuad(toFill, Image.s_Xy, this.color, Image.s_Uv);
				}
			}
		}

		private static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
		{
			bool flag;
			if (fill < 0.001f)
			{
				flag = false;
			}
			else
			{
				if ((corner & 1) == 1)
				{
					invert = !invert;
				}
				if (!invert && fill > 0.999f)
				{
					flag = true;
				}
				else
				{
					float num = Mathf.Clamp01(fill);
					if (invert)
					{
						num = 1f - num;
					}
					num *= 1.5707964f;
					float num2 = Mathf.Cos(num);
					float num3 = Mathf.Sin(num);
					Image.RadialCut(xy, num2, num3, invert, corner);
					Image.RadialCut(uv, num2, num3, invert, corner);
					flag = true;
				}
			}
			return flag;
		}

		private static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
		{
			int num = (corner + 1) % 4;
			int num2 = (corner + 2) % 4;
			int num3 = (corner + 3) % 4;
			if ((corner & 1) == 1)
			{
				if (sin > cos)
				{
					cos /= sin;
					sin = 1f;
					if (invert)
					{
						xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
						xy[num2].x = xy[num].x;
					}
				}
				else if (cos > sin)
				{
					sin /= cos;
					cos = 1f;
					if (!invert)
					{
						xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
						xy[num3].y = xy[num2].y;
					}
				}
				else
				{
					cos = 1f;
					sin = 1f;
				}
				if (!invert)
				{
					xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
				}
				else
				{
					xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
				}
			}
			else
			{
				if (cos > sin)
				{
					sin /= cos;
					cos = 1f;
					if (!invert)
					{
						xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
						xy[num2].y = xy[num].y;
					}
				}
				else if (sin > cos)
				{
					cos /= sin;
					sin = 1f;
					if (invert)
					{
						xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
						xy[num3].x = xy[num2].x;
					}
				}
				else
				{
					cos = 1f;
					sin = 1f;
				}
				if (invert)
				{
					xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
				}
				else
				{
					xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
				}
			}
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				float num;
				if (this.activeSprite == null)
				{
					num = 0f;
				}
				else if (this.type == Image.Type.Sliced || this.type == Image.Type.Tiled)
				{
					num = DataUtility.GetMinSize(this.activeSprite).x / this.pixelsPerUnit;
				}
				else
				{
					num = this.activeSprite.rect.size.x / this.pixelsPerUnit;
				}
				return num;
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				float num;
				if (this.activeSprite == null)
				{
					num = 0f;
				}
				else if (this.type == Image.Type.Sliced || this.type == Image.Type.Tiled)
				{
					num = DataUtility.GetMinSize(this.activeSprite).y / this.pixelsPerUnit;
				}
				else
				{
					num = this.activeSprite.rect.size.y / this.pixelsPerUnit;
				}
				return num;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return 0;
			}
		}

		public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			bool flag;
			Vector2 vector;
			if (this.alphaHitTestMinimumThreshold <= 0f)
			{
				flag = true;
			}
			else if (this.alphaHitTestMinimumThreshold > 1f)
			{
				flag = false;
			}
			else if (this.activeSprite == null)
			{
				flag = true;
			}
			else if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector))
			{
				flag = false;
			}
			else
			{
				Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
				vector.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
				vector.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
				vector = this.MapCoordinate(vector, pixelAdjustedRect);
				Rect textureRect = this.activeSprite.textureRect;
				Vector2 vector2 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
				float num = Mathf.Lerp(textureRect.x, textureRect.xMax, vector2.x) / (float)this.activeSprite.texture.width;
				float num2 = Mathf.Lerp(textureRect.y, textureRect.yMax, vector2.y) / (float)this.activeSprite.texture.height;
				try
				{
					flag = this.activeSprite.texture.GetPixelBilinear(num, num2).a >= this.alphaHitTestMinimumThreshold;
				}
				catch (UnityException ex)
				{
					Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
					flag = true;
				}
			}
			return flag;
		}

		private Vector2 MapCoordinate(Vector2 local, Rect rect)
		{
			Rect rect2 = this.activeSprite.rect;
			Vector2 vector;
			if (this.type == Image.Type.Simple || this.type == Image.Type.Filled)
			{
				vector = new Vector2(local.x * rect2.width / rect.width, local.y * rect2.height / rect.height);
			}
			else
			{
				Vector4 border = this.activeSprite.border;
				Vector4 adjustedBorders = this.GetAdjustedBorders(border / this.pixelsPerUnit, rect);
				for (int i = 0; i < 2; i++)
				{
					if (local[i] > adjustedBorders[i])
					{
						if (rect.size[i] - local[i] <= adjustedBorders[i + 2])
						{
							ref Vector2 ptr = ref local;
							int num;
							local[num = i] = ptr[num] - (rect.size[i] - rect2.size[i]);
						}
						else if (this.type == Image.Type.Sliced)
						{
							float num2 = Mathf.InverseLerp(adjustedBorders[i], rect.size[i] - adjustedBorders[i + 2], local[i]);
							local[i] = Mathf.Lerp(border[i], rect2.size[i] - border[i + 2], num2);
						}
						else
						{
							ref Vector2 ptr = ref local;
							int num3;
							local[num3 = i] = ptr[num3] - adjustedBorders[i];
							local[i] = Mathf.Repeat(local[i], rect2.size[i] - border[i] - border[i + 2]);
							ptr = ref local;
							int num4;
							local[num4 = i] = ptr[num4] + border[i];
						}
					}
				}
				vector = local;
			}
			return vector;
		}

		protected static Material s_ETC1DefaultUI = null;

		[FormerlySerializedAs("m_Frame")]
		[SerializeField]
		private Sprite m_Sprite;

		[NonSerialized]
		private Sprite m_OverrideSprite;

		[SerializeField]
		private Image.Type m_Type = Image.Type.Simple;

		[SerializeField]
		private bool m_PreserveAspect = false;

		[SerializeField]
		private bool m_FillCenter = true;

		[SerializeField]
		private Image.FillMethod m_FillMethod = Image.FillMethod.Radial360;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_FillAmount = 1f;

		[SerializeField]
		private bool m_FillClockwise = true;

		[SerializeField]
		private int m_FillOrigin;

		private float m_AlphaHitTestMinimumThreshold = 0f;

		private static readonly Vector2[] s_VertScratch = new Vector2[4];

		private static readonly Vector2[] s_UVScratch = new Vector2[4];

		private static readonly Vector3[] s_Xy = new Vector3[4];

		private static readonly Vector3[] s_Uv = new Vector3[4];

		public enum Type
		{
			Simple,
			Sliced,
			Tiled,
			Filled
		}

		public enum FillMethod
		{
			Horizontal,
			Vertical,
			Radial90,
			Radial180,
			Radial360
		}

		public enum OriginHorizontal
		{
			Left,
			Right
		}

		public enum OriginVertical
		{
			Bottom,
			Top
		}

		public enum Origin90
		{
			BottomLeft,
			TopLeft,
			TopRight,
			BottomRight
		}

		public enum Origin180
		{
			Bottom,
			Left,
			Top,
			Right
		}

		public enum Origin360
		{
			Bottom,
			Right,
			Top,
			Left
		}
	}
}
