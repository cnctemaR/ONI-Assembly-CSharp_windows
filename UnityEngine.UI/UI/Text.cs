using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Text", 10)]
	public class Text : MaskableGraphic, ILayoutElement
	{
		protected Text()
		{
			base.useLegacyMeshGeneration = false;
		}

		public TextGenerator cachedTextGenerator
		{
			get
			{
				TextGenerator textGenerator;
				if ((textGenerator = this.m_TextCache) == null)
				{
					textGenerator = (this.m_TextCache = ((this.m_Text.Length == 0) ? new TextGenerator() : new TextGenerator(this.m_Text.Length)));
				}
				return textGenerator;
			}
		}

		public TextGenerator cachedTextGeneratorForLayout
		{
			get
			{
				TextGenerator textGenerator;
				if ((textGenerator = this.m_TextCacheForLayout) == null)
				{
					textGenerator = (this.m_TextCacheForLayout = new TextGenerator());
				}
				return textGenerator;
			}
		}

		public override Texture mainTexture
		{
			get
			{
				Texture texture;
				if (this.font != null && this.font.material != null && this.font.material.mainTexture != null)
				{
					texture = this.font.material.mainTexture;
				}
				else if (this.m_Material != null)
				{
					texture = this.m_Material.mainTexture;
				}
				else
				{
					texture = base.mainTexture;
				}
				return texture;
			}
		}

		public void FontTextureChanged()
		{
			if (this)
			{
				if (!this.m_DisableFontTextureRebuiltCallback)
				{
					this.cachedTextGenerator.Invalidate();
					if (this.IsActive())
					{
						if (CanvasUpdateRegistry.IsRebuildingGraphics() || CanvasUpdateRegistry.IsRebuildingLayout())
						{
							this.UpdateGeometry();
						}
						else
						{
							this.SetAllDirty();
						}
					}
				}
			}
		}

		public Font font
		{
			get
			{
				return this.m_FontData.font;
			}
			set
			{
				if (!(this.m_FontData.font == value))
				{
					FontUpdateTracker.UntrackText(this);
					this.m_FontData.font = value;
					FontUpdateTracker.TrackText(this);
					this.SetAllDirty();
				}
			}
		}

		public virtual string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					if (!string.IsNullOrEmpty(this.m_Text))
					{
						this.m_Text = "";
						this.SetVerticesDirty();
					}
				}
				else if (this.m_Text != value)
				{
					this.m_Text = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool supportRichText
		{
			get
			{
				return this.m_FontData.richText;
			}
			set
			{
				if (this.m_FontData.richText != value)
				{
					this.m_FontData.richText = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool resizeTextForBestFit
		{
			get
			{
				return this.m_FontData.bestFit;
			}
			set
			{
				if (this.m_FontData.bestFit != value)
				{
					this.m_FontData.bestFit = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public int resizeTextMinSize
		{
			get
			{
				return this.m_FontData.minSize;
			}
			set
			{
				if (this.m_FontData.minSize != value)
				{
					this.m_FontData.minSize = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public int resizeTextMaxSize
		{
			get
			{
				return this.m_FontData.maxSize;
			}
			set
			{
				if (this.m_FontData.maxSize != value)
				{
					this.m_FontData.maxSize = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public TextAnchor alignment
		{
			get
			{
				return this.m_FontData.alignment;
			}
			set
			{
				if (this.m_FontData.alignment != value)
				{
					this.m_FontData.alignment = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool alignByGeometry
		{
			get
			{
				return this.m_FontData.alignByGeometry;
			}
			set
			{
				if (this.m_FontData.alignByGeometry != value)
				{
					this.m_FontData.alignByGeometry = value;
					this.SetVerticesDirty();
				}
			}
		}

		public int fontSize
		{
			get
			{
				return this.m_FontData.fontSize;
			}
			set
			{
				if (this.m_FontData.fontSize != value)
				{
					this.m_FontData.fontSize = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public HorizontalWrapMode horizontalOverflow
		{
			get
			{
				return this.m_FontData.horizontalOverflow;
			}
			set
			{
				if (this.m_FontData.horizontalOverflow != value)
				{
					this.m_FontData.horizontalOverflow = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public VerticalWrapMode verticalOverflow
		{
			get
			{
				return this.m_FontData.verticalOverflow;
			}
			set
			{
				if (this.m_FontData.verticalOverflow != value)
				{
					this.m_FontData.verticalOverflow = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public float lineSpacing
		{
			get
			{
				return this.m_FontData.lineSpacing;
			}
			set
			{
				if (this.m_FontData.lineSpacing != value)
				{
					this.m_FontData.lineSpacing = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public FontStyle fontStyle
		{
			get
			{
				return this.m_FontData.fontStyle;
			}
			set
			{
				if (this.m_FontData.fontStyle != value)
				{
					this.m_FontData.fontStyle = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				Canvas canvas = base.canvas;
				float num;
				if (!canvas)
				{
					num = 1f;
				}
				else if (!this.font || this.font.dynamic)
				{
					num = canvas.scaleFactor;
				}
				else if (this.m_FontData.fontSize <= 0 || this.font.fontSize <= 0)
				{
					num = 1f;
				}
				else
				{
					num = (float)this.font.fontSize / (float)this.m_FontData.fontSize;
				}
				return num;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.cachedTextGenerator.Invalidate();
			FontUpdateTracker.TrackText(this);
		}

		protected override void OnDisable()
		{
			FontUpdateTracker.UntrackText(this);
			base.OnDisable();
		}

		protected override void UpdateGeometry()
		{
			if (this.font != null)
			{
				base.UpdateGeometry();
			}
		}

		internal void AssignDefaultFont()
		{
			this.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		}

		public TextGenerationSettings GetGenerationSettings(Vector2 extents)
		{
			TextGenerationSettings textGenerationSettings = default(TextGenerationSettings);
			textGenerationSettings.generationExtents = extents;
			if (this.font != null && this.font.dynamic)
			{
				textGenerationSettings.fontSize = this.m_FontData.fontSize;
				textGenerationSettings.resizeTextMinSize = this.m_FontData.minSize;
				textGenerationSettings.resizeTextMaxSize = this.m_FontData.maxSize;
			}
			textGenerationSettings.textAnchor = this.m_FontData.alignment;
			textGenerationSettings.alignByGeometry = this.m_FontData.alignByGeometry;
			textGenerationSettings.scaleFactor = this.pixelsPerUnit;
			textGenerationSettings.color = this.color;
			textGenerationSettings.font = this.font;
			textGenerationSettings.pivot = base.rectTransform.pivot;
			textGenerationSettings.richText = this.m_FontData.richText;
			textGenerationSettings.lineSpacing = this.m_FontData.lineSpacing;
			textGenerationSettings.fontStyle = this.m_FontData.fontStyle;
			textGenerationSettings.resizeTextForBestFit = this.m_FontData.bestFit;
			textGenerationSettings.updateBounds = false;
			textGenerationSettings.horizontalOverflow = this.m_FontData.horizontalOverflow;
			textGenerationSettings.verticalOverflow = this.m_FontData.verticalOverflow;
			return textGenerationSettings;
		}

		public static Vector2 GetTextAnchorPivot(TextAnchor anchor)
		{
			Vector2 vector;
			switch (anchor)
			{
			case TextAnchor.UpperLeft:
				vector = new Vector2(0f, 1f);
				break;
			case TextAnchor.UpperCenter:
				vector = new Vector2(0.5f, 1f);
				break;
			case TextAnchor.UpperRight:
				vector = new Vector2(1f, 1f);
				break;
			case TextAnchor.MiddleLeft:
				vector = new Vector2(0f, 0.5f);
				break;
			case TextAnchor.MiddleCenter:
				vector = new Vector2(0.5f, 0.5f);
				break;
			case TextAnchor.MiddleRight:
				vector = new Vector2(1f, 0.5f);
				break;
			case TextAnchor.LowerLeft:
				vector = new Vector2(0f, 0f);
				break;
			case TextAnchor.LowerCenter:
				vector = new Vector2(0.5f, 0f);
				break;
			case TextAnchor.LowerRight:
				vector = new Vector2(1f, 0f);
				break;
			default:
				vector = Vector2.zero;
				break;
			}
			return vector;
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			if (!(this.font == null))
			{
				this.m_DisableFontTextureRebuiltCallback = true;
				Vector2 size = base.rectTransform.rect.size;
				TextGenerationSettings generationSettings = this.GetGenerationSettings(size);
				this.cachedTextGenerator.PopulateWithErrors(this.text, generationSettings, base.gameObject);
				IList<UIVertex> verts = this.cachedTextGenerator.verts;
				float num = 1f / this.pixelsPerUnit;
				int num2 = verts.Count - 4;
				if (num2 <= 0)
				{
					toFill.Clear();
				}
				else
				{
					Vector2 vector = new Vector2(verts[0].position.x, verts[0].position.y) * num;
					vector = base.PixelAdjustPoint(vector) - vector;
					toFill.Clear();
					if (vector != Vector2.zero)
					{
						for (int i = 0; i < num2; i++)
						{
							int num3 = i & 3;
							this.m_TempVerts[num3] = verts[i];
							UIVertex[] tempVerts = this.m_TempVerts;
							int num4 = num3;
							tempVerts[num4].position = tempVerts[num4].position * num;
							UIVertex[] tempVerts2 = this.m_TempVerts;
							int num5 = num3;
							tempVerts2[num5].position.x = tempVerts2[num5].position.x + vector.x;
							UIVertex[] tempVerts3 = this.m_TempVerts;
							int num6 = num3;
							tempVerts3[num6].position.y = tempVerts3[num6].position.y + vector.y;
							if (num3 == 3)
							{
								toFill.AddUIVertexQuad(this.m_TempVerts);
							}
						}
					}
					else
					{
						for (int j = 0; j < num2; j++)
						{
							int num7 = j & 3;
							this.m_TempVerts[num7] = verts[j];
							UIVertex[] tempVerts4 = this.m_TempVerts;
							int num8 = num7;
							tempVerts4[num8].position = tempVerts4[num8].position * num;
							if (num7 == 3)
							{
								toFill.AddUIVertexQuad(this.m_TempVerts);
							}
						}
					}
					this.m_DisableFontTextureRebuiltCallback = false;
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
				TextGenerationSettings generationSettings = this.GetGenerationSettings(Vector2.zero);
				return this.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.pixelsPerUnit;
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
				TextGenerationSettings generationSettings = this.GetGenerationSettings(new Vector2(base.GetPixelAdjustedRect().size.x, 0f));
				return this.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.pixelsPerUnit;
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

		[SerializeField]
		private FontData m_FontData = FontData.defaultFontData;

		[TextArea(3, 10)]
		[SerializeField]
		protected string m_Text = string.Empty;

		private TextGenerator m_TextCache;

		private TextGenerator m_TextCacheForLayout;

		protected static Material s_DefaultText = null;

		[NonSerialized]
		protected bool m_DisableFontTextureRebuiltCallback = false;

		private readonly UIVertex[] m_TempVerts = new UIVertex[4];
	}
}
