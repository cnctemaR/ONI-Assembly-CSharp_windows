using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TMPro
{
	public class TMP_Text : MaskableGraphic
	{
		public string text
		{
			get
			{
				return this.m_text;
			}
			set
			{
				if (!(this.m_text == value))
				{
					this.m_text = value;
					this.m_inputSource = TMP_Text.TextInputSources.String;
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.m_isInputParsingRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool isRightToLeftText
		{
			get
			{
				return this.m_isRightToLeft;
			}
			set
			{
				if (this.m_isRightToLeft != value)
				{
					this.m_isRightToLeft = value;
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.m_isInputParsingRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public TMP_FontAsset font
		{
			get
			{
				return this.m_fontAsset;
			}
			set
			{
				if (!(this.m_fontAsset == value))
				{
					this.m_fontAsset = value;
					this.LoadFontAsset();
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.m_isInputParsingRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public virtual Material fontSharedMaterial
		{
			get
			{
				return this.m_sharedMaterial;
			}
			set
			{
				if (!(this.m_sharedMaterial == value))
				{
					this.SetSharedMaterial(value);
					this.m_havePropertiesChanged = true;
					this.m_isInputParsingRequired = true;
					this.SetVerticesDirty();
					this.SetMaterialDirty();
				}
			}
		}

		public virtual Material[] fontSharedMaterials
		{
			get
			{
				return this.GetSharedMaterials();
			}
			set
			{
				this.SetSharedMaterials(value);
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public Material fontMaterial
		{
			get
			{
				return this.GetMaterial(this.m_sharedMaterial);
			}
			set
			{
				if (!(this.m_sharedMaterial != null) || this.m_sharedMaterial.GetInstanceID() != value.GetInstanceID())
				{
					this.m_sharedMaterial = value;
					this.m_padding = this.GetPaddingForMaterial();
					this.m_havePropertiesChanged = true;
					this.m_isInputParsingRequired = true;
					this.SetVerticesDirty();
					this.SetMaterialDirty();
				}
			}
		}

		public virtual Material[] fontMaterials
		{
			get
			{
				return this.GetMaterials(this.m_fontSharedMaterials);
			}
			set
			{
				this.SetSharedMaterials(value);
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public new Color color
		{
			get
			{
				return this.m_fontColor;
			}
			set
			{
				if (!(this.m_fontColor == value))
				{
					this.m_havePropertiesChanged = true;
					this.m_fontColor = value;
					this.SetVerticesDirty();
				}
			}
		}

		public float alpha
		{
			get
			{
				return this.m_fontColor.a;
			}
			set
			{
				if (this.m_fontColor.a != value)
				{
					this.m_fontColor.a = value;
					this.m_havePropertiesChanged = true;
					this.SetVerticesDirty();
				}
			}
		}

		public bool enableVertexGradient
		{
			get
			{
				return this.m_enableVertexGradient;
			}
			set
			{
				if (this.m_enableVertexGradient != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_enableVertexGradient = value;
					this.SetVerticesDirty();
				}
			}
		}

		public VertexGradient colorGradient
		{
			get
			{
				return this.m_fontColorGradient;
			}
			set
			{
				this.m_havePropertiesChanged = true;
				this.m_fontColorGradient = value;
				this.SetVerticesDirty();
			}
		}

		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return this.m_spriteAsset;
			}
			set
			{
				this.m_spriteAsset = value;
			}
		}

		public bool tintAllSprites
		{
			get
			{
				return this.m_tintAllSprites;
			}
			set
			{
				if (this.m_tintAllSprites != value)
				{
					this.m_tintAllSprites = value;
					this.m_havePropertiesChanged = true;
					this.SetVerticesDirty();
				}
			}
		}

		public bool overrideColorTags
		{
			get
			{
				return this.m_overrideHtmlColors;
			}
			set
			{
				if (this.m_overrideHtmlColors != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_overrideHtmlColors = value;
					this.SetVerticesDirty();
				}
			}
		}

		public Color32 faceColor
		{
			get
			{
				Color32 color;
				if (this.m_sharedMaterial == null)
				{
					color = this.m_faceColor;
				}
				else
				{
					this.m_faceColor = this.m_sharedMaterial.GetColor(ShaderUtilities.ID_FaceColor);
					color = this.m_faceColor;
				}
				return color;
			}
			set
			{
				if (!this.m_faceColor.Compare(value))
				{
					this.SetFaceColor(value);
					this.m_havePropertiesChanged = true;
					this.m_faceColor = value;
					this.SetVerticesDirty();
					this.SetMaterialDirty();
				}
			}
		}

		public Color32 outlineColor
		{
			get
			{
				Color32 color;
				if (this.m_sharedMaterial == null)
				{
					color = this.m_outlineColor;
				}
				else
				{
					this.m_outlineColor = this.m_sharedMaterial.GetColor(ShaderUtilities.ID_OutlineColor);
					color = this.m_outlineColor;
				}
				return color;
			}
			set
			{
				if (!this.m_outlineColor.Compare(value))
				{
					this.SetOutlineColor(value);
					this.m_havePropertiesChanged = true;
					this.m_outlineColor = value;
					this.SetVerticesDirty();
				}
			}
		}

		public float outlineWidth
		{
			get
			{
				float num;
				if (this.m_sharedMaterial == null)
				{
					num = this.m_outlineWidth;
				}
				else
				{
					this.m_outlineWidth = this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth);
					num = this.m_outlineWidth;
				}
				return num;
			}
			set
			{
				if (this.m_outlineWidth != value)
				{
					this.SetOutlineThickness(value);
					this.m_havePropertiesChanged = true;
					this.m_outlineWidth = value;
					this.SetVerticesDirty();
				}
			}
		}

		public float fontSize
		{
			get
			{
				return this.m_fontSize;
			}
			set
			{
				if (this.m_fontSize != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
					this.m_fontSize = value;
					if (!this.m_enableAutoSizing)
					{
						this.m_fontSizeBase = this.m_fontSize;
					}
				}
			}
		}

		public float fontScale
		{
			get
			{
				return this.m_fontScale;
			}
		}

		public int fontWeight
		{
			get
			{
				return this.m_fontWeight;
			}
			set
			{
				if (this.m_fontWeight != value)
				{
					this.m_fontWeight = value;
					this.m_isCalculateSizeRequired = true;
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
				else if (!this.font)
				{
					num = canvas.scaleFactor;
				}
				else if (this.m_currentFontAsset == null || this.m_currentFontAsset.fontInfo.PointSize <= 0f || this.m_fontSize <= 0f)
				{
					num = 1f;
				}
				else
				{
					num = this.m_fontSize / this.m_currentFontAsset.fontInfo.PointSize;
				}
				return num;
			}
		}

		public bool enableAutoSizing
		{
			get
			{
				return this.m_enableAutoSizing;
			}
			set
			{
				if (this.m_enableAutoSizing != value)
				{
					this.m_enableAutoSizing = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public float fontSizeMin
		{
			get
			{
				return this.m_fontSizeMin;
			}
			set
			{
				if (this.m_fontSizeMin != value)
				{
					this.m_fontSizeMin = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public float fontSizeMax
		{
			get
			{
				return this.m_fontSizeMax;
			}
			set
			{
				if (this.m_fontSizeMax != value)
				{
					this.m_fontSizeMax = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public FontStyles fontStyle
		{
			get
			{
				return this.m_fontStyle;
			}
			set
			{
				if (this.m_fontStyle != value)
				{
					this.m_fontStyle = value;
					this.m_havePropertiesChanged = true;
					this.checkPaddingRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool isUsingBold
		{
			get
			{
				return this.m_isUsingBold;
			}
		}

		public TextAlignmentOptions alignment
		{
			get
			{
				return this.m_textAlignment;
			}
			set
			{
				if (this.m_textAlignment != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_textAlignment = value;
					this.SetVerticesDirty();
				}
			}
		}

		public float characterSpacing
		{
			get
			{
				return this.m_characterSpacing;
			}
			set
			{
				if (this.m_characterSpacing != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
					this.m_characterSpacing = value;
				}
			}
		}

		public float lineSpacing
		{
			get
			{
				return this.m_lineSpacing;
			}
			set
			{
				if (this.m_lineSpacing != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
					this.m_lineSpacing = value;
				}
			}
		}

		public float paragraphSpacing
		{
			get
			{
				return this.m_paragraphSpacing;
			}
			set
			{
				if (this.m_paragraphSpacing != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
					this.m_paragraphSpacing = value;
				}
			}
		}

		public float characterWidthAdjustment
		{
			get
			{
				return this.m_charWidthMaxAdj;
			}
			set
			{
				if (this.m_charWidthMaxAdj != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
					this.m_charWidthMaxAdj = value;
				}
			}
		}

		public bool enableWordWrapping
		{
			get
			{
				return this.m_enableWordWrapping;
			}
			set
			{
				if (this.m_enableWordWrapping != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isInputParsingRequired = true;
					this.m_isCalculateSizeRequired = true;
					this.m_enableWordWrapping = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public float wordWrappingRatios
		{
			get
			{
				return this.m_wordWrappingRatios;
			}
			set
			{
				if (this.m_wordWrappingRatios != value)
				{
					this.m_wordWrappingRatios = value;
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public TextOverflowModes OverflowMode
		{
			get
			{
				return this.m_overflowMode;
			}
			set
			{
				if (this.m_overflowMode != value)
				{
					this.m_overflowMode = value;
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool enableKerning
		{
			get
			{
				return this.m_enableKerning;
			}
			set
			{
				if (this.m_enableKerning != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
					this.m_enableKerning = value;
				}
			}
		}

		public bool extraPadding
		{
			get
			{
				return this.m_enableExtraPadding;
			}
			set
			{
				if (this.m_enableExtraPadding != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_enableExtraPadding = value;
					this.UpdateMeshPadding();
					this.SetVerticesDirty();
				}
			}
		}

		public bool richText
		{
			get
			{
				return this.m_isRichText;
			}
			set
			{
				if (this.m_isRichText != value)
				{
					this.m_isRichText = value;
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
					this.m_isInputParsingRequired = true;
				}
			}
		}

		public bool parseCtrlCharacters
		{
			get
			{
				return this.m_parseCtrlCharacters;
			}
			set
			{
				if (this.m_parseCtrlCharacters != value)
				{
					this.m_parseCtrlCharacters = value;
					this.m_havePropertiesChanged = true;
					this.m_isCalculateSizeRequired = true;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
					this.m_isInputParsingRequired = true;
				}
			}
		}

		public bool isOverlay
		{
			get
			{
				return this.m_isOverlay;
			}
			set
			{
				if (this.m_isOverlay != value)
				{
					this.m_isOverlay = value;
					this.SetShaderDepth();
					this.m_havePropertiesChanged = true;
					this.SetVerticesDirty();
				}
			}
		}

		public bool isOrthographic
		{
			get
			{
				return this.m_isOrthographic;
			}
			set
			{
				if (this.m_isOrthographic != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isOrthographic = value;
					this.SetVerticesDirty();
				}
			}
		}

		public bool enableCulling
		{
			get
			{
				return this.m_isCullingEnabled;
			}
			set
			{
				if (this.m_isCullingEnabled != value)
				{
					this.m_isCullingEnabled = value;
					this.SetCulling();
					this.m_havePropertiesChanged = true;
				}
			}
		}

		public bool ignoreVisibility
		{
			get
			{
				return this.m_ignoreCulling;
			}
			set
			{
				if (this.m_ignoreCulling != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_ignoreCulling = value;
				}
			}
		}

		public TextureMappingOptions horizontalMapping
		{
			get
			{
				return this.m_horizontalMapping;
			}
			set
			{
				if (this.m_horizontalMapping != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_horizontalMapping = value;
					this.SetVerticesDirty();
				}
			}
		}

		public TextureMappingOptions verticalMapping
		{
			get
			{
				return this.m_verticalMapping;
			}
			set
			{
				if (this.m_verticalMapping != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_verticalMapping = value;
					this.SetVerticesDirty();
				}
			}
		}

		public TextRenderFlags renderMode
		{
			get
			{
				return this.m_renderMode;
			}
			set
			{
				if (this.m_renderMode != value)
				{
					this.m_renderMode = value;
					this.m_havePropertiesChanged = true;
				}
			}
		}

		public int maxVisibleCharacters
		{
			get
			{
				return this.m_maxVisibleCharacters;
			}
			set
			{
				if (this.m_maxVisibleCharacters != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_maxVisibleCharacters = value;
					this.SetVerticesDirty();
				}
			}
		}

		public int maxVisibleWords
		{
			get
			{
				return this.m_maxVisibleWords;
			}
			set
			{
				if (this.m_maxVisibleWords != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_maxVisibleWords = value;
					this.SetVerticesDirty();
				}
			}
		}

		public int maxVisibleLines
		{
			get
			{
				return this.m_maxVisibleLines;
			}
			set
			{
				if (this.m_maxVisibleLines != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isInputParsingRequired = true;
					this.m_maxVisibleLines = value;
					this.SetVerticesDirty();
				}
			}
		}

		public bool useMaxVisibleDescender
		{
			get
			{
				return this.m_useMaxVisibleDescender;
			}
			set
			{
				if (this.m_useMaxVisibleDescender != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_isInputParsingRequired = true;
					this.SetVerticesDirty();
				}
			}
		}

		public int pageToDisplay
		{
			get
			{
				return this.m_pageToDisplay;
			}
			set
			{
				if (this.m_pageToDisplay != value)
				{
					this.m_havePropertiesChanged = true;
					this.m_pageToDisplay = value;
					this.SetVerticesDirty();
				}
			}
		}

		public virtual Vector4 margin
		{
			get
			{
				return this.m_margin;
			}
			set
			{
				if (!(this.m_margin == value))
				{
					this.m_margin = value;
					this.ComputeMarginSize();
					this.m_havePropertiesChanged = true;
					this.SetVerticesDirty();
				}
			}
		}

		public TMP_TextInfo textInfo
		{
			get
			{
				return this.m_textInfo;
			}
		}

		public bool havePropertiesChanged
		{
			get
			{
				return this.m_havePropertiesChanged;
			}
			set
			{
				if (this.m_havePropertiesChanged != value)
				{
					this.m_havePropertiesChanged = value;
					this.SetVerticesDirty();
					this.SetLayoutDirty();
				}
			}
		}

		public bool isUsingLegacyAnimationComponent
		{
			get
			{
				return this.m_isUsingLegacyAnimationComponent;
			}
			set
			{
				this.m_isUsingLegacyAnimationComponent = value;
			}
		}

		public new Transform transform
		{
			get
			{
				if (this.m_transform == null)
				{
					this.m_transform = base.GetComponent<Transform>();
				}
				return this.m_transform;
			}
		}

		public new RectTransform rectTransform
		{
			get
			{
				if (this.m_rectTransform == null)
				{
					this.m_rectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_rectTransform;
			}
		}

		public virtual bool autoSizeTextContainer { get; set; }

		public virtual Mesh mesh
		{
			get
			{
				return this.m_mesh;
			}
		}

		public virtual Bounds bounds { get; set; }

		public float flexibleHeight
		{
			get
			{
				return this.m_flexibleHeight;
			}
		}

		public float flexibleWidth
		{
			get
			{
				return this.m_flexibleWidth;
			}
		}

		public float minHeight
		{
			get
			{
				return this.m_minHeight;
			}
		}

		public float minWidth
		{
			get
			{
				return this.m_minWidth;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				return (this.m_preferredWidth != 9999f) ? this.m_preferredWidth : this.GetPreferredWidth();
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				return (this.m_preferredHeight != 9999f) ? this.m_preferredHeight : this.GetPreferredHeight();
			}
		}

		public int layoutPriority
		{
			get
			{
				return this.m_layoutPriority;
			}
		}

		protected virtual void LoadFontAsset()
		{
		}

		protected virtual void SetSharedMaterial(Material mat)
		{
		}

		protected virtual Material GetMaterial(Material mat)
		{
			return null;
		}

		protected virtual void SetFontBaseMaterial(Material mat)
		{
		}

		protected virtual Material[] GetSharedMaterials()
		{
			return null;
		}

		protected virtual void SetSharedMaterials(Material[] materials)
		{
		}

		protected virtual Material[] GetMaterials(Material[] mats)
		{
			return null;
		}

		protected virtual Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			Material material2 = material;
			material2.name += " (Instance)";
			return material;
		}

		protected virtual void SetFaceColor(Color32 color)
		{
		}

		protected virtual void SetOutlineColor(Color32 color)
		{
		}

		protected virtual void SetOutlineThickness(float thickness)
		{
		}

		protected virtual void SetShaderDepth()
		{
		}

		protected virtual void SetCulling()
		{
		}

		protected virtual float GetPaddingForMaterial()
		{
			return 0f;
		}

		protected virtual float GetPaddingForMaterial(Material mat)
		{
			return 0f;
		}

		protected virtual Vector3[] GetTextContainerLocalCorners()
		{
			return null;
		}

		public virtual void ForceMeshUpdate()
		{
		}

		public virtual void ForceMeshUpdate(bool ignoreActiveState)
		{
		}

		internal void SetTextInternal(string text)
		{
			this.m_text = text;
			this.m_renderMode = TextRenderFlags.DontRender;
			this.m_isInputParsingRequired = true;
			this.ForceMeshUpdate();
			this.m_renderMode = TextRenderFlags.Render;
		}

		public virtual void UpdateGeometry(Mesh mesh, int index)
		{
		}

		public virtual void UpdateVertexData(TMP_VertexDataUpdateFlags flags)
		{
		}

		public virtual void UpdateVertexData()
		{
		}

		public virtual void SetVertices(Vector3[] vertices)
		{
		}

		public virtual void UpdateMeshPadding()
		{
		}

		public new void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
			base.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
			this.InternalCrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
		}

		public new void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
			base.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
			this.InternalCrossFadeAlpha(alpha, duration, ignoreTimeScale);
		}

		protected virtual void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
		}

		protected virtual void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
		}

		protected void ParseInputText()
		{
			this.m_isInputParsingRequired = false;
			switch (this.m_inputSource)
			{
			case TMP_Text.TextInputSources.Text:
			case TMP_Text.TextInputSources.String:
				this.StringToCharArray(this.m_text, ref this.m_char_buffer);
				break;
			case TMP_Text.TextInputSources.SetText:
				this.SetTextArrayToCharArray(this.m_input_CharArray, ref this.m_char_buffer);
				break;
			}
			this.SetArraySizes(this.m_char_buffer);
		}

		public void SetText(string text)
		{
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		public void SetText(string text, float arg0)
		{
			this.SetText(text, arg0, 255f, 255f);
		}

		public void SetText(string text, float arg0, float arg1)
		{
			this.SetText(text, arg0, arg1, 255f);
		}

		public void SetText(string text, float arg0, float arg1, float arg2)
		{
			if (!(text == this.old_text) || arg0 != this.old_arg0 || arg1 != this.old_arg1 || arg2 != this.old_arg2)
			{
				this.old_text = text;
				this.old_arg1 = 255f;
				this.old_arg2 = 255f;
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < text.Length; i++)
				{
					char c = text[i];
					if (c == '{')
					{
						if (text[i + 2] == ':')
						{
							num = (int)(text[i + 3] - '0');
						}
						int num3 = (int)(text[i + 1] - '0');
						if (num3 != 0)
						{
							if (num3 != 1)
							{
								if (num3 == 2)
								{
									this.old_arg2 = arg2;
									this.AddFloatToCharArray(arg2, ref num2, num);
								}
							}
							else
							{
								this.old_arg1 = arg1;
								this.AddFloatToCharArray(arg1, ref num2, num);
							}
						}
						else
						{
							this.old_arg0 = arg0;
							this.AddFloatToCharArray(arg0, ref num2, num);
						}
						if (text[i + 2] == ':')
						{
							i += 4;
						}
						else
						{
							i += 2;
						}
					}
					else
					{
						this.m_input_CharArray[num2] = c;
						num2++;
					}
				}
				this.m_input_CharArray[num2] = '\0';
				this.m_charArray_Length = num2;
				this.m_inputSource = TMP_Text.TextInputSources.SetText;
				this.m_isInputParsingRequired = true;
				this.m_havePropertiesChanged = true;
				this.m_isCalculateSizeRequired = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public void SetText(StringBuilder text)
		{
			this.StringBuilderToIntArray(text, ref this.m_char_buffer);
			this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
			this.m_isInputParsingRequired = true;
			this.m_havePropertiesChanged = true;
			this.m_isCalculateSizeRequired = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		public void SetCharArray(char[] charArray)
		{
			if (charArray != null && charArray.Length != 0)
			{
				if (this.m_char_buffer.Length <= charArray.Length)
				{
					int num = Mathf.NextPowerOfTwo(charArray.Length + 1);
					this.m_char_buffer = new int[num];
				}
				int num2 = 0;
				int i = 0;
				while (i < charArray.Length)
				{
					if (charArray[i] != '\\' || i >= charArray.Length - 1)
					{
						goto IL_00C6;
					}
					int num3 = (int)charArray[i + 1];
					if (num3 != 110)
					{
						if (num3 != 114)
						{
							if (num3 != 116)
							{
								goto IL_00C6;
							}
							this.m_char_buffer[num2] = 9;
							i++;
							num2++;
						}
						else
						{
							this.m_char_buffer[num2] = 13;
							i++;
							num2++;
						}
					}
					else
					{
						this.m_char_buffer[num2] = 10;
						i++;
						num2++;
					}
					IL_00D6:
					i++;
					continue;
					IL_00C6:
					this.m_char_buffer[num2] = (int)charArray[i];
					num2++;
					goto IL_00D6;
				}
				this.m_char_buffer[num2] = 0;
				this.m_inputSource = TMP_Text.TextInputSources.SetCharArray;
				this.m_havePropertiesChanged = true;
				this.m_isInputParsingRequired = true;
			}
		}

		protected void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
		{
			if (charArray != null && this.m_charArray_Length != 0)
			{
				if (charBuffer.Length <= this.m_charArray_Length)
				{
					int num = ((this.m_charArray_Length <= 1024) ? Mathf.NextPowerOfTwo(this.m_charArray_Length + 1) : (this.m_charArray_Length + 256));
					charBuffer = new int[num];
				}
				int num2 = 0;
				for (int i = 0; i < this.m_charArray_Length; i++)
				{
					if (char.IsHighSurrogate(charArray[i]) && char.IsLowSurrogate(charArray[i + 1]))
					{
						charBuffer[num2] = char.ConvertToUtf32(charArray[i], charArray[i + 1]);
						i++;
						num2++;
					}
					else
					{
						charBuffer[num2] = (int)charArray[i];
						num2++;
					}
				}
				charBuffer[num2] = 0;
			}
		}

		protected void StringToCharArray(string text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
			}
			else
			{
				if (chars == null || chars.Length <= text.Length)
				{
					int num = ((text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256));
					chars = new int[num];
				}
				int num2 = 0;
				int i = 0;
				while (i < text.Length)
				{
					if (this.m_inputSource == TMP_Text.TextInputSources.Text && text[i] == '\\' && text.Length > i + 1)
					{
						int num3 = (int)text[i + 1];
						switch (num3)
						{
						case 114:
							if (this.m_parseCtrlCharacters)
							{
								chars[num2] = 13;
								i++;
								num2++;
								goto IL_0233;
							}
							break;
						default:
							if (num3 != 85)
							{
								if (num3 != 92)
								{
									if (num3 == 110)
									{
										if (this.m_parseCtrlCharacters)
										{
											chars[num2] = 10;
											i++;
											num2++;
											goto IL_0233;
										}
									}
								}
								else if (text.Length > i + 2)
								{
									chars[num2] = (int)text[i + 1];
									chars[num2 + 1] = (int)text[i + 2];
									i += 2;
									num2 += 2;
									goto IL_0233;
								}
							}
							else if (text.Length > i + 9)
							{
								chars[num2] = this.GetUTF32(i + 2);
								i += 9;
								num2++;
								goto IL_0233;
							}
							break;
						case 116:
							if (this.m_parseCtrlCharacters)
							{
								chars[num2] = 9;
								i++;
								num2++;
								goto IL_0233;
							}
							break;
						case 117:
							if (text.Length > i + 5)
							{
								chars[num2] = (int)((ushort)this.GetUTF16(i + 2));
								i += 5;
								num2++;
								goto IL_0233;
							}
							break;
						}
						goto IL_01D8;
					}
					goto IL_01D8;
					IL_0233:
					i++;
					continue;
					IL_01D8:
					if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
					{
						chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
						i++;
						num2++;
						goto IL_0233;
					}
					chars[num2] = (int)text[i];
					num2++;
					goto IL_0233;
				}
				chars[num2] = 0;
			}
		}

		protected void StringBuilderToIntArray(StringBuilder text, ref int[] chars)
		{
			if (text == null)
			{
				chars[0] = 0;
			}
			else
			{
				if (chars == null || chars.Length <= text.Length)
				{
					int num = ((text.Length <= 1024) ? Mathf.NextPowerOfTwo(text.Length + 1) : (text.Length + 256));
					chars = new int[num];
				}
				int num2 = 0;
				int i = 0;
				while (i < text.Length)
				{
					if (this.m_parseCtrlCharacters && text[i] == '\\' && text.Length > i + 1)
					{
						int num3 = (int)text[i + 1];
						switch (num3)
						{
						case 114:
							chars[num2] = 13;
							i++;
							num2++;
							goto IL_0203;
						default:
							if (num3 != 85)
							{
								if (num3 != 92)
								{
									if (num3 == 110)
									{
										chars[num2] = 10;
										i++;
										num2++;
										goto IL_0203;
									}
								}
								else if (text.Length > i + 2)
								{
									chars[num2] = (int)text[i + 1];
									chars[num2 + 1] = (int)text[i + 2];
									i += 2;
									num2 += 2;
									goto IL_0203;
								}
							}
							else if (text.Length > i + 9)
							{
								chars[num2] = this.GetUTF32(i + 2);
								i += 9;
								num2++;
								goto IL_0203;
							}
							break;
						case 116:
							chars[num2] = 9;
							i++;
							num2++;
							goto IL_0203;
						case 117:
							if (text.Length > i + 5)
							{
								chars[num2] = (int)((ushort)this.GetUTF16(i + 2));
								i += 5;
								num2++;
								goto IL_0203;
							}
							break;
						}
						goto IL_01A8;
					}
					goto IL_01A8;
					IL_0203:
					i++;
					continue;
					IL_01A8:
					if (char.IsHighSurrogate(text[i]) && char.IsLowSurrogate(text[i + 1]))
					{
						chars[num2] = char.ConvertToUtf32(text[i], text[i + 1]);
						i++;
						num2++;
						goto IL_0203;
					}
					chars[num2] = (int)text[i];
					num2++;
					goto IL_0203;
				}
				chars[num2] = 0;
			}
		}

		protected void AddFloatToCharArray(float number, ref int index, int precision)
		{
			if (number < 0f)
			{
				this.m_input_CharArray[index++] = '-';
				number = -number;
			}
			number += this.k_Power[Mathf.Min(9, precision)];
			int num = (int)number;
			this.AddIntToCharArray(num, ref index, precision);
			if (precision > 0)
			{
				this.m_input_CharArray[index++] = '.';
				number -= (float)num;
				for (int i = 0; i < precision; i++)
				{
					number *= 10f;
					int num2 = (int)number;
					this.m_input_CharArray[index++] = (char)(num2 + 48);
					number -= (float)num2;
				}
			}
		}

		protected void AddIntToCharArray(int number, ref int index, int precision)
		{
			if (number < 0)
			{
				this.m_input_CharArray[index++] = '-';
				number = -number;
			}
			int num = index;
			do
			{
				this.m_input_CharArray[num++] = (char)(number % 10 + 48);
				number /= 10;
			}
			while (number > 0);
			int num2 = num;
			while (index + 1 < num)
			{
				num--;
				char c = this.m_input_CharArray[index];
				this.m_input_CharArray[index] = this.m_input_CharArray[num];
				this.m_input_CharArray[num] = c;
				index++;
			}
			index = num2;
		}

		protected virtual int SetArraySizes(int[] chars)
		{
			return 0;
		}

		protected virtual void GenerateTextMesh()
		{
		}

		public Vector2 GetPreferredValues()
		{
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.ParseInputText();
			}
			float preferredWidth = this.GetPreferredWidth();
			float preferredHeight = this.GetPreferredHeight();
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(float width, float height)
		{
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.ParseInputText();
			}
			Vector2 vector = new Vector2(width, height);
			float preferredWidth = this.GetPreferredWidth(vector);
			float preferredHeight = this.GetPreferredHeight(vector);
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(string text)
		{
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.SetArraySizes(this.m_char_buffer);
			Vector2 vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			float preferredWidth = this.GetPreferredWidth(vector);
			float preferredHeight = this.GetPreferredHeight(vector);
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(string text, float width, float height)
		{
			this.StringToCharArray(text, ref this.m_char_buffer);
			this.SetArraySizes(this.m_char_buffer);
			Vector2 vector = new Vector2(width, height);
			float preferredWidth = this.GetPreferredWidth(vector);
			float preferredHeight = this.GetPreferredHeight(vector);
			return new Vector2(preferredWidth, preferredHeight);
		}

		protected float GetPreferredWidth()
		{
			float num = ((!this.m_enableAutoSizing) ? this.m_fontSize : this.m_fontSizeMax);
			Vector2 vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.ParseInputText();
			}
			return this.CalculatePreferredValues(num, vector).x;
		}

		protected float GetPreferredWidth(Vector2 margin)
		{
			float num = ((!this.m_enableAutoSizing) ? this.m_fontSize : this.m_fontSizeMax);
			return this.CalculatePreferredValues(num, margin).x;
		}

		protected float GetPreferredHeight()
		{
			float num = ((!this.m_enableAutoSizing) ? this.m_fontSize : this.m_fontSizeMax);
			Vector2 vector = new Vector2((this.m_marginWidth == 0f) ? float.PositiveInfinity : this.m_marginWidth, float.PositiveInfinity);
			if (this.m_isInputParsingRequired || this.m_isTextTruncated)
			{
				this.ParseInputText();
			}
			return this.CalculatePreferredValues(num, vector).y;
		}

		protected float GetPreferredHeight(Vector2 margin)
		{
			float num = ((!this.m_enableAutoSizing) ? this.m_fontSize : this.m_fontSizeMax);
			return this.CalculatePreferredValues(num, margin).y;
		}

		protected virtual Vector2 CalculatePreferredValues(float defaultFontSize, Vector2 marginSize)
		{
			Vector2 vector;
			if (this.m_fontAsset == null || this.m_fontAsset.characterDictionary == null)
			{
				global::Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + base.GetInstanceID(), null);
				vector = Vector2.zero;
			}
			else if (this.m_char_buffer == null || this.m_char_buffer.Length == 0 || this.m_char_buffer[0] == 0)
			{
				vector = Vector2.zero;
			}
			else
			{
				this.m_currentFontAsset = this.m_fontAsset;
				this.m_currentMaterial = this.m_sharedMaterial;
				this.m_currentMaterialIndex = 0;
				this.m_materialReferenceStack.SetDefault(new MaterialReference(0, this.m_currentFontAsset, null, this.m_currentMaterial, this.m_padding));
				int totalCharacterCount = this.m_totalCharacterCount;
				if (this.m_internalCharacterInfo == null || totalCharacterCount > this.m_internalCharacterInfo.Length)
				{
					this.m_internalCharacterInfo = new TMP_CharacterInfo[(totalCharacterCount <= 1024) ? Mathf.NextPowerOfTwo(totalCharacterCount) : (totalCharacterCount + 256)];
				}
				this.m_fontScale = defaultFontSize / this.m_currentFontAsset.fontInfo.PointSize * ((!this.m_isOrthographic) ? 0.1f : 1f);
				this.m_fontScaleMultiplier = 1f;
				float num = defaultFontSize / this.m_fontAsset.fontInfo.PointSize * this.m_fontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
				float num2 = this.m_fontScale;
				this.m_currentFontSize = defaultFontSize;
				this.m_sizeStack.SetDefault(this.m_currentFontSize);
				this.m_style = this.m_fontStyle;
				this.m_baselineOffset = 0f;
				this.m_styleStack.Clear();
				this.m_lineOffset = 0f;
				this.m_lineHeight = 0f;
				float num3 = this.m_currentFontAsset.fontInfo.LineHeight - (this.m_currentFontAsset.fontInfo.Ascender - this.m_currentFontAsset.fontInfo.Descender);
				this.m_cSpacing = 0f;
				this.m_monoSpacing = 0f;
				this.m_xAdvance = 0f;
				float num4 = 0f;
				this.tag_LineIndent = 0f;
				this.tag_Indent = 0f;
				this.m_indentStack.SetDefault(0f);
				this.tag_NoParsing = false;
				this.m_characterCount = 0;
				this.m_firstCharacterOfLine = 0;
				this.m_maxLineAscender = float.NegativeInfinity;
				this.m_maxLineDescender = float.PositiveInfinity;
				this.m_lineNumber = 0;
				float x = marginSize.x;
				this.m_marginLeft = 0f;
				this.m_marginRight = 0f;
				this.m_width = -1f;
				float num5 = 0f;
				float num6 = 0f;
				this.m_maxAscender = 0f;
				this.m_maxDescender = 0f;
				bool flag = true;
				bool flag2 = false;
				WordWrapState wordWrapState = default(WordWrapState);
				this.SaveWordWrappingState(ref wordWrapState, 0, 0);
				WordWrapState wordWrapState2 = default(WordWrapState);
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				while (this.m_char_buffer[num9] != 0)
				{
					int num10 = this.m_char_buffer[num9];
					this.m_textElementType = TMP_TextElementType.Character;
					this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
					this.m_currentFontAsset = this.m_materialReferences[this.m_currentMaterialIndex].fontAsset;
					int currentMaterialIndex = this.m_currentMaterialIndex;
					if (this.m_isRichText && num10 == 60)
					{
						this.m_isParsingText = true;
						if (this.ValidateHtmlTag(this.m_char_buffer, num9 + 1, out num8))
						{
							num9 = num8;
							if (this.m_textElementType == TMP_TextElementType.Character)
							{
								goto IL_11AD;
							}
						}
						goto IL_03BE;
					}
					goto IL_03BE;
					IL_11AD:
					num9++;
					continue;
					IL_03BE:
					this.m_isParsingText = false;
					bool isUsingAlternateTypeface = this.m_internalCharacterInfo[this.m_characterCount].isUsingAlternateTypeface;
					float num11 = 1f;
					if (this.m_textElementType == TMP_TextElementType.Character)
					{
						if ((this.m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
						{
							if (char.IsLower((char)num10))
							{
								num10 = (int)char.ToUpper((char)num10);
							}
						}
						else if ((this.m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
						{
							if (char.IsUpper((char)num10))
							{
								num10 = (int)char.ToLower((char)num10);
							}
						}
						else if ((this.m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (this.m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps)
						{
							if (char.IsLower((char)num10))
							{
								num11 = 0.8f;
								num10 = (int)char.ToUpper((char)num10);
							}
						}
					}
					if (this.m_textElementType == TMP_TextElementType.Sprite)
					{
						TMP_Sprite tmp_Sprite = this.m_currentSpriteAsset.spriteInfoList[this.m_spriteIndex];
						if (tmp_Sprite == null)
						{
							goto IL_11AD;
						}
						num10 = 57344 + this.m_spriteIndex;
						this.m_currentFontAsset = this.m_fontAsset;
						float num12 = this.m_currentFontSize / this.m_fontAsset.fontInfo.PointSize * this.m_fontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
						num2 = this.m_fontAsset.fontInfo.Ascender / tmp_Sprite.height * tmp_Sprite.scale * num12;
						this.m_cached_TextElement = tmp_Sprite;
						this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Sprite;
						this.m_currentMaterialIndex = currentMaterialIndex;
					}
					else if (this.m_textElementType == TMP_TextElementType.Character)
					{
						this.m_cached_TextElement = this.m_textInfo.characterInfo[this.m_characterCount].textElement;
						if (this.m_cached_TextElement == null)
						{
							goto IL_11AD;
						}
						this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
						this.m_fontScale = this.m_currentFontSize * num11 / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
						num2 = this.m_fontScale * this.m_fontScaleMultiplier;
						this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Character;
					}
					float num13 = num2;
					if (num10 == 173)
					{
						num2 = 0f;
					}
					this.m_internalCharacterInfo[this.m_characterCount].character = (char)num10;
					if (this.m_enableKerning && this.m_characterCount >= 1)
					{
						int character = (int)this.m_internalCharacterInfo[this.m_characterCount - 1].character;
						KerningPairKey kerningPairKey = new KerningPairKey(character, num10);
						KerningPair kerningPair;
						this.m_currentFontAsset.kerningDictionary.TryGetValue(kerningPairKey.key, out kerningPair);
						if (kerningPair != null)
						{
							this.m_xAdvance += kerningPair.XadvanceOffset * num2;
						}
					}
					float num14 = 0f;
					if (this.m_monoSpacing != 0f)
					{
						num14 = this.m_monoSpacing / 2f - (this.m_cached_TextElement.width / 2f + this.m_cached_TextElement.xOffset) * num2;
						this.m_xAdvance += num14;
					}
					float num15;
					if (this.m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && ((this.m_style & FontStyles.Bold) == FontStyles.Bold || (this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold))
					{
						num15 = 1f + this.m_currentFontAsset.boldSpacing * 0.01f;
					}
					else
					{
						num15 = 1f;
					}
					this.m_internalCharacterInfo[this.m_characterCount].baseLine = 0f - this.m_lineOffset + this.m_baselineOffset;
					float num16 = this.m_currentFontAsset.fontInfo.Ascender * ((this.m_textElementType != TMP_TextElementType.Character) ? this.m_internalCharacterInfo[this.m_characterCount].scale : num2) + this.m_baselineOffset;
					this.m_internalCharacterInfo[this.m_characterCount].ascender = num16 - this.m_lineOffset;
					this.m_maxLineAscender = ((num16 <= this.m_maxLineAscender) ? this.m_maxLineAscender : num16);
					float num17 = this.m_currentFontAsset.fontInfo.Descender * ((this.m_textElementType != TMP_TextElementType.Character) ? this.m_internalCharacterInfo[this.m_characterCount].scale : num2) + this.m_baselineOffset;
					float num18 = (this.m_internalCharacterInfo[this.m_characterCount].descender = num17 - this.m_lineOffset);
					this.m_maxLineDescender = ((num17 >= this.m_maxLineDescender) ? this.m_maxLineDescender : num17);
					if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript || (this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
					{
						float num19 = (num16 - this.m_baselineOffset) / this.m_currentFontAsset.fontInfo.SubSize;
						num16 = this.m_maxLineAscender;
						this.m_maxLineAscender = ((num19 <= this.m_maxLineAscender) ? this.m_maxLineAscender : num19);
						float num20 = (num17 - this.m_baselineOffset) / this.m_currentFontAsset.fontInfo.SubSize;
						num17 = this.m_maxLineDescender;
						this.m_maxLineDescender = ((num20 >= this.m_maxLineDescender) ? this.m_maxLineDescender : num20);
					}
					if (this.m_lineNumber == 0)
					{
						this.m_maxAscender = ((this.m_maxAscender <= num16) ? num16 : this.m_maxAscender);
					}
					if (num10 == 9 || !char.IsWhiteSpace((char)num10) || this.m_textElementType == TMP_TextElementType.Sprite)
					{
						float num21 = ((this.m_width == -1f) ? (x + 0.0001f - this.m_marginLeft - this.m_marginRight) : Mathf.Min(x + 0.0001f - this.m_marginLeft - this.m_marginRight, this.m_width));
						if (this.m_xAdvance + this.m_cached_TextElement.xAdvance * ((num10 == 173) ? num13 : num2) > num21)
						{
							if (this.enableWordWrapping && this.m_characterCount != this.m_firstCharacterOfLine)
							{
								if (num7 == wordWrapState2.previous_WordBreak || flag)
								{
									if (!this.m_isCharacterWrappingEnabled)
									{
										this.m_isCharacterWrappingEnabled = true;
									}
									else
									{
										flag2 = true;
									}
								}
								num9 = this.RestoreWordWrappingState(ref wordWrapState2);
								num7 = num9;
								if (this.m_char_buffer[num9] == 173)
								{
									this.m_isTextTruncated = true;
									this.m_char_buffer[num9] = 45;
									this.CalculatePreferredValues(defaultFontSize, marginSize);
									return Vector2.zero;
								}
								if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f)
								{
									float num22 = this.m_maxLineAscender - this.m_startOfLineAscender;
									this.AdjustLineOffset(this.m_firstCharacterOfLine, this.m_characterCount, num22);
									this.m_lineOffset += num22;
									wordWrapState2.lineOffset = this.m_lineOffset;
									wordWrapState2.previousLineAscender = this.m_maxLineAscender;
								}
								float num23 = this.m_maxLineAscender - this.m_lineOffset;
								float num24 = this.m_maxLineDescender - this.m_lineOffset;
								this.m_maxDescender = ((this.m_maxDescender >= num24) ? num24 : this.m_maxDescender);
								this.m_firstCharacterOfLine = this.m_characterCount;
								num5 += this.m_xAdvance;
								if (this.m_enableWordWrapping)
								{
									num6 = this.m_maxAscender - this.m_maxDescender;
								}
								else
								{
									num6 = Mathf.Max(num6, num23 - num24);
								}
								this.SaveWordWrappingState(ref wordWrapState, num9, this.m_characterCount - 1);
								this.m_lineNumber++;
								if (this.m_lineHeight == 0f)
								{
									float num25 = this.m_internalCharacterInfo[this.m_characterCount].ascender - this.m_internalCharacterInfo[this.m_characterCount].baseLine;
									float num26 = 0f - this.m_maxLineDescender + num25 + (num3 + this.m_lineSpacing + this.m_lineSpacingDelta) * num;
									this.m_lineOffset += num26;
									this.m_startOfLineAscender = num25;
								}
								else
								{
									this.m_lineOffset += this.m_lineHeight + this.m_lineSpacing * num;
								}
								this.m_maxLineAscender = float.NegativeInfinity;
								this.m_maxLineDescender = float.PositiveInfinity;
								this.m_xAdvance = this.tag_Indent;
								goto IL_11AD;
							}
						}
					}
					if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f && !this.m_isNewPage)
					{
						float num27 = this.m_maxLineAscender - this.m_startOfLineAscender;
						this.AdjustLineOffset(this.m_firstCharacterOfLine, this.m_characterCount, num27);
						num18 -= num27;
						this.m_lineOffset += num27;
						this.m_startOfLineAscender += num27;
						wordWrapState2.lineOffset = this.m_lineOffset;
						wordWrapState2.previousLineAscender = this.m_startOfLineAscender;
					}
					if (num10 == 9)
					{
						this.m_xAdvance += this.m_currentFontAsset.fontInfo.TabWidth * num2;
					}
					else if (this.m_monoSpacing != 0f)
					{
						this.m_xAdvance += this.m_monoSpacing - num14 + (this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing;
					}
					else
					{
						this.m_xAdvance += (this.m_cached_TextElement.xAdvance * num15 + this.m_characterSpacing + this.m_currentFontAsset.normalSpacingOffset) * num2 + this.m_cSpacing;
					}
					if (num10 == 13)
					{
						num4 = Mathf.Max(num4, num5 + this.m_xAdvance);
						num5 = 0f;
						this.m_xAdvance = this.tag_Indent;
					}
					if (num10 == 10 || this.m_characterCount == totalCharacterCount - 1)
					{
						if (this.m_lineNumber > 0 && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && this.m_lineHeight == 0f)
						{
							float num28 = this.m_maxLineAscender - this.m_startOfLineAscender;
							this.AdjustLineOffset(this.m_firstCharacterOfLine, this.m_characterCount, num28);
							num18 -= num28;
							this.m_lineOffset += num28;
						}
						float num29 = this.m_maxLineDescender - this.m_lineOffset;
						this.m_maxDescender = ((this.m_maxDescender >= num29) ? num29 : this.m_maxDescender);
						this.m_firstCharacterOfLine = this.m_characterCount + 1;
						if (num10 == 10 && this.m_characterCount != totalCharacterCount - 1)
						{
							num4 = Mathf.Max(num4, num5 + this.m_xAdvance);
							num5 = 0f;
						}
						else
						{
							num5 = Mathf.Max(num4, num5 + this.m_xAdvance);
						}
						num6 = this.m_maxAscender - this.m_maxDescender;
						if (num10 == 10)
						{
							this.SaveWordWrappingState(ref wordWrapState, num9, this.m_characterCount);
							this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
							this.m_lineNumber++;
							if (this.m_lineHeight == 0f)
							{
								float num26 = 0f - this.m_maxLineDescender + num16 + (num3 + this.m_lineSpacing + this.m_paragraphSpacing + this.m_lineSpacingDelta) * num;
								this.m_lineOffset += num26;
							}
							else
							{
								this.m_lineOffset += this.m_lineHeight + (this.m_lineSpacing + this.m_paragraphSpacing) * num;
							}
							this.m_maxLineAscender = float.NegativeInfinity;
							this.m_maxLineDescender = float.PositiveInfinity;
							this.m_startOfLineAscender = num16;
							this.m_xAdvance = this.tag_LineIndent + this.tag_Indent;
						}
					}
					if (this.m_enableWordWrapping || this.m_overflowMode == TextOverflowModes.Truncate || this.m_overflowMode == TextOverflowModes.Ellipsis)
					{
						if ((char.IsWhiteSpace((char)num10) || num10 == 45 || num10 == 173) && !this.m_isNonBreakingSpace && num10 != 160 && num10 != 8209 && num10 != 8239 && num10 != 8288)
						{
							this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
							this.m_isCharacterWrappingEnabled = false;
							flag = false;
						}
						else if (num10 > 11904 && num10 < 40959 && !this.m_isNonBreakingSpace)
						{
							if (!TMP_Settings.linebreakingRules.leadingCharacters.ContainsKey(num10) && this.m_characterCount < totalCharacterCount - 1 && !TMP_Settings.linebreakingRules.followingCharacters.ContainsKey((int)this.m_internalCharacterInfo[this.m_characterCount + 1].character))
							{
								this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
								this.m_isCharacterWrappingEnabled = false;
								flag = false;
							}
						}
						else if (flag || this.m_isCharacterWrappingEnabled || flag2)
						{
							this.SaveWordWrappingState(ref wordWrapState2, num9, this.m_characterCount);
						}
					}
					this.m_characterCount++;
					goto IL_11AD;
				}
				this.m_isCharacterWrappingEnabled = false;
				num5 += ((this.m_margin.x <= 0f) ? 0f : this.m_margin.x);
				num5 += ((this.m_margin.z <= 0f) ? 0f : this.m_margin.z);
				num6 += ((this.m_margin.y <= 0f) ? 0f : this.m_margin.y);
				num6 += ((this.m_margin.w <= 0f) ? 0f : this.m_margin.w);
				vector = new Vector2(num5, num6);
			}
			return vector;
		}

		protected virtual void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
		}

		protected void ResizeLineExtents(int size)
		{
			size = ((size <= 1024) ? Mathf.NextPowerOfTwo(size + 1) : (size + 256));
			TMP_LineInfo[] array = new TMP_LineInfo[size];
			for (int i = 0; i < size; i++)
			{
				if (i < this.m_textInfo.lineInfo.Length)
				{
					array[i] = this.m_textInfo.lineInfo[i];
				}
				else
				{
					array[i].lineExtents.min = TMP_Text.k_InfinityVectorPositive;
					array[i].lineExtents.max = TMP_Text.k_InfinityVectorNegative;
					array[i].ascender = TMP_Text.k_InfinityVectorNegative.x;
					array[i].descender = TMP_Text.k_InfinityVectorPositive.x;
				}
			}
			this.m_textInfo.lineInfo = array;
		}

		public virtual TMP_TextInfo GetTextInfo(string text)
		{
			return null;
		}

		protected virtual void ComputeMarginSize()
		{
		}

		protected int GetArraySizes(int[] chars)
		{
			int num = 0;
			this.m_totalCharacterCount = 0;
			this.m_isUsingBold = false;
			this.m_isParsingText = false;
			int num2 = 0;
			while (chars[num2] != 0)
			{
				int num3 = chars[num2];
				if (!this.m_isRichText || num3 != 60)
				{
					goto IL_0067;
				}
				if (!this.ValidateHtmlTag(chars, num2 + 1, out num))
				{
					goto IL_0067;
				}
				num2 = num;
				if ((this.m_style & FontStyles.Bold) == FontStyles.Bold)
				{
					this.m_isUsingBold = true;
				}
				IL_0084:
				num2++;
				continue;
				IL_0067:
				if (!char.IsWhiteSpace((char)num3))
				{
				}
				this.m_totalCharacterCount++;
				goto IL_0084;
			}
			return this.m_totalCharacterCount;
		}

		protected void SaveWordWrappingState(ref WordWrapState state, int index, int count)
		{
			state.currentFontAsset = this.m_currentFontAsset;
			state.currentSpriteAsset = this.m_currentSpriteAsset;
			state.currentMaterial = this.m_currentMaterial;
			state.currentMaterialIndex = this.m_currentMaterialIndex;
			state.previous_WordBreak = index;
			state.total_CharacterCount = count;
			state.visible_CharacterCount = this.m_lineVisibleCharacterCount;
			state.visible_LinkCount = this.m_textInfo.linkCount;
			state.firstCharacterIndex = this.m_firstCharacterOfLine;
			state.firstVisibleCharacterIndex = this.m_firstVisibleCharacterOfLine;
			state.lastVisibleCharIndex = this.m_lastVisibleCharacterOfLine;
			state.fontStyle = this.m_style;
			state.fontScale = this.m_fontScale;
			state.fontScaleMultiplier = this.m_fontScaleMultiplier;
			state.currentFontSize = this.m_currentFontSize;
			state.xAdvance = this.m_xAdvance;
			state.maxAscender = this.m_maxAscender;
			state.maxDescender = this.m_maxDescender;
			state.maxLineAscender = this.m_maxLineAscender;
			state.maxLineDescender = this.m_maxLineDescender;
			state.previousLineAscender = this.m_startOfLineAscender;
			state.preferredWidth = this.m_preferredWidth;
			state.preferredHeight = this.m_preferredHeight;
			state.meshExtents = this.m_meshExtents;
			state.lineNumber = this.m_lineNumber;
			state.lineOffset = this.m_lineOffset;
			state.baselineOffset = this.m_baselineOffset;
			state.vertexColor = this.m_htmlColor;
			state.tagNoParsing = this.tag_NoParsing;
			state.colorStack = this.m_colorStack;
			state.sizeStack = this.m_sizeStack;
			state.fontWeightStack = this.m_fontWeightStack;
			state.styleStack = this.m_styleStack;
			state.actionStack = this.m_actionStack;
			state.materialReferenceStack = this.m_materialReferenceStack;
			if (this.m_lineNumber < this.m_textInfo.lineInfo.Length)
			{
				state.lineInfo = this.m_textInfo.lineInfo[this.m_lineNumber];
			}
		}

		protected int RestoreWordWrappingState(ref WordWrapState state)
		{
			int previous_WordBreak = state.previous_WordBreak;
			this.m_currentFontAsset = state.currentFontAsset;
			this.m_currentSpriteAsset = state.currentSpriteAsset;
			this.m_currentMaterial = state.currentMaterial;
			this.m_currentMaterialIndex = state.currentMaterialIndex;
			this.m_characterCount = state.total_CharacterCount + 1;
			this.m_lineVisibleCharacterCount = state.visible_CharacterCount;
			this.m_textInfo.linkCount = state.visible_LinkCount;
			this.m_firstCharacterOfLine = state.firstCharacterIndex;
			this.m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			this.m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			this.m_style = state.fontStyle;
			this.m_fontScale = state.fontScale;
			this.m_fontScaleMultiplier = state.fontScaleMultiplier;
			this.m_currentFontSize = state.currentFontSize;
			this.m_xAdvance = state.xAdvance;
			this.m_maxAscender = state.maxAscender;
			this.m_maxDescender = state.maxDescender;
			this.m_maxLineAscender = state.maxLineAscender;
			this.m_maxLineDescender = state.maxLineDescender;
			this.m_startOfLineAscender = state.previousLineAscender;
			this.m_preferredWidth = state.preferredWidth;
			this.m_preferredHeight = state.preferredHeight;
			this.m_meshExtents = state.meshExtents;
			this.m_lineNumber = state.lineNumber;
			this.m_lineOffset = state.lineOffset;
			this.m_baselineOffset = state.baselineOffset;
			this.m_htmlColor = state.vertexColor;
			this.tag_NoParsing = state.tagNoParsing;
			this.m_colorStack = state.colorStack;
			this.m_sizeStack = state.sizeStack;
			this.m_fontWeightStack = state.fontWeightStack;
			this.m_styleStack = state.styleStack;
			this.m_actionStack = state.actionStack;
			this.m_materialReferenceStack = state.materialReferenceStack;
			if (this.m_lineNumber < this.m_textInfo.lineInfo.Length)
			{
				this.m_textInfo.lineInfo[this.m_lineNumber] = state.lineInfo;
			}
			return previous_WordBreak;
		}

		protected virtual void SaveGlyphVertexInfo(float padding, float style_padding, Color32 vertexColor)
		{
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
			vertexColor.a = ((this.m_fontColor32.a >= vertexColor.a) ? vertexColor.a : this.m_fontColor32.a);
			if (!this.m_enableVertexGradient)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradient.bottomLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradient.topLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradient.topRight * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradient.bottomRight * vertexColor;
			}
			if (!this.m_isSDFShader)
			{
				style_padding = 0f;
			}
			FaceInfo fontInfo = this.m_currentFontAsset.fontInfo;
			Vector2 vector;
			vector.x = (this.m_cached_TextElement.x - padding - style_padding) / fontInfo.AtlasWidth;
			vector.y = 1f - (this.m_cached_TextElement.y + padding + style_padding + this.m_cached_TextElement.height) / fontInfo.AtlasHeight;
			Vector2 vector2;
			vector2.x = vector.x;
			vector2.y = 1f - (this.m_cached_TextElement.y - padding - style_padding) / fontInfo.AtlasHeight;
			Vector2 vector3;
			vector3.x = (this.m_cached_TextElement.x + padding + style_padding + this.m_cached_TextElement.width) / fontInfo.AtlasWidth;
			vector3.y = vector2.y;
			Vector2 vector4;
			vector4.x = vector3.x;
			vector4.y = vector.y;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = vector;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = vector2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = vector3;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = vector4;
		}

		protected virtual void SaveSpriteVertexInfo(Color32 vertexColor)
		{
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.position = this.m_textInfo.characterInfo[this.m_characterCount].topLeft;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.position = this.m_textInfo.characterInfo[this.m_characterCount].topRight;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.position = this.m_textInfo.characterInfo[this.m_characterCount].bottomRight;
			if (this.m_tintAllSprites)
			{
				this.m_tintSprite = true;
			}
			Color32 color = ((!this.m_tintSprite) ? this.m_spriteColor : this.m_spriteColor.Multiply(vertexColor));
			color.a = ((color.a >= this.m_fontColor32.a) ? this.m_fontColor32.a : (color.a = ((color.a >= vertexColor.a) ? vertexColor.a : color.a)));
			if (!this.m_enableVertexGradient)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = color;
			}
			else if (!this.m_overrideHtmlColors && !this.m_htmlColor.CompareRGB(this.m_fontColor32))
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = color;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = color;
			}
			else
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradient.bottomLeft));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradient.topLeft));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradient.topRight));
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = ((!this.m_tintSprite) ? color : color.Multiply(this.m_fontColorGradient.bottomRight));
			}
			Vector2 vector = new Vector2(this.m_cached_TextElement.x / (float)this.m_currentSpriteAsset.spriteSheet.width, this.m_cached_TextElement.y / (float)this.m_currentSpriteAsset.spriteSheet.height);
			Vector2 vector2 = new Vector2(vector.x, (this.m_cached_TextElement.y + this.m_cached_TextElement.height) / (float)this.m_currentSpriteAsset.spriteSheet.height);
			Vector2 vector3 = new Vector2((this.m_cached_TextElement.x + this.m_cached_TextElement.width) / (float)this.m_currentSpriteAsset.spriteSheet.width, vector2.y);
			Vector2 vector4 = new Vector2(vector3.x, vector.y);
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = vector;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = vector2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = vector3;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = vector4;
		}

		protected virtual void FillCharacterVertexBuffers(int i, int index_X4)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = index_X4;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
		}

		protected virtual void FillSpriteVertexBuffers(int i, int index_X4)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			index_X4 = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = index_X4;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[index_X4] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + index_X4] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + index_X4] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + index_X4] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[index_X4] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + index_X4] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + index_X4] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + index_X4] = characterInfo[i].vertex_BR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[index_X4] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + index_X4] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + index_X4] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + index_X4] = characterInfo[i].vertex_BR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[index_X4] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + index_X4] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + index_X4] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + index_X4] = characterInfo[i].vertex_BR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = index_X4 + 4;
		}

		protected virtual void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, Color32 underlineColor)
		{
			if (this.m_cached_Underline_GlyphInfo == null)
			{
				if (!TMP_Settings.warningsDisabled)
				{
					global::Debug.LogWarning("Unable to add underline since the Font Asset doesn't contain the underline character.", this);
				}
			}
			else
			{
				int num = index + 12;
				if (num > this.m_textInfo.meshInfo[0].vertices.Length)
				{
					this.m_textInfo.meshInfo[0].ResizeMeshInfo(num / 4);
				}
				start.y = Mathf.Min(start.y, end.y);
				end.y = Mathf.Min(start.y, end.y);
				float num2 = this.m_cached_Underline_GlyphInfo.width / 2f * maxScale;
				if (end.x - start.x < this.m_cached_Underline_GlyphInfo.width * maxScale)
				{
					num2 = (end.x - start.x) / 2f;
				}
				float num3 = this.m_padding * startScale / maxScale;
				float num4 = this.m_padding * endScale / maxScale;
				float height = this.m_cached_Underline_GlyphInfo.height;
				Vector3[] vertices = this.m_textInfo.meshInfo[0].vertices;
				vertices[index] = start + new Vector3(0f, 0f - (height + this.m_padding) * maxScale, 0f);
				vertices[index + 1] = start + new Vector3(0f, this.m_padding * maxScale, 0f);
				vertices[index + 2] = vertices[index + 1] + new Vector3(num2, 0f, 0f);
				vertices[index + 3] = vertices[index] + new Vector3(num2, 0f, 0f);
				vertices[index + 4] = vertices[index + 3];
				vertices[index + 5] = vertices[index + 2];
				vertices[index + 6] = end + new Vector3(-num2, this.m_padding * maxScale, 0f);
				vertices[index + 7] = end + new Vector3(-num2, -(height + this.m_padding) * maxScale, 0f);
				vertices[index + 8] = vertices[index + 7];
				vertices[index + 9] = vertices[index + 6];
				vertices[index + 10] = end + new Vector3(0f, this.m_padding * maxScale, 0f);
				vertices[index + 11] = end + new Vector3(0f, -(height + this.m_padding) * maxScale, 0f);
				Vector2[] uvs = this.m_textInfo.meshInfo[0].uvs0;
				Vector2 vector = new Vector2((this.m_cached_Underline_GlyphInfo.x - num3) / this.m_fontAsset.fontInfo.AtlasWidth, 1f - (this.m_cached_Underline_GlyphInfo.y + this.m_padding + this.m_cached_Underline_GlyphInfo.height) / this.m_fontAsset.fontInfo.AtlasHeight);
				Vector2 vector2 = new Vector2(vector.x, 1f - (this.m_cached_Underline_GlyphInfo.y - this.m_padding) / this.m_fontAsset.fontInfo.AtlasHeight);
				Vector2 vector3 = new Vector2((this.m_cached_Underline_GlyphInfo.x - num3 + this.m_cached_Underline_GlyphInfo.width / 2f) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
				Vector2 vector4 = new Vector2(vector3.x, vector.y);
				Vector2 vector5 = new Vector2((this.m_cached_Underline_GlyphInfo.x + num4 + this.m_cached_Underline_GlyphInfo.width / 2f) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
				Vector2 vector6 = new Vector2(vector5.x, vector.y);
				Vector2 vector7 = new Vector2((this.m_cached_Underline_GlyphInfo.x + num4 + this.m_cached_Underline_GlyphInfo.width) / this.m_fontAsset.fontInfo.AtlasWidth, vector2.y);
				Vector2 vector8 = new Vector2(vector7.x, vector.y);
				uvs[index] = vector;
				uvs[1 + index] = vector2;
				uvs[2 + index] = vector3;
				uvs[3 + index] = vector4;
				uvs[4 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector.y);
				uvs[5 + index] = new Vector2(vector3.x - vector3.x * 0.001f, vector2.y);
				uvs[6 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector2.y);
				uvs[7 + index] = new Vector2(vector3.x + vector3.x * 0.001f, vector.y);
				uvs[8 + index] = vector6;
				uvs[9 + index] = vector5;
				uvs[10 + index] = vector7;
				uvs[11 + index] = vector8;
				float num5 = (vertices[index + 2].x - start.x) / (end.x - start.x);
				float num6 = ((maxScale * this.m_rectTransform.lossyScale.y != 0f) ? this.m_rectTransform.lossyScale.y : 1f);
				float num7 = num6;
				Vector2[] uvs2 = this.m_textInfo.meshInfo[0].uvs2;
				uvs2[index] = this.PackUV(0f, 0f, num6);
				uvs2[1 + index] = this.PackUV(0f, 1f, num6);
				uvs2[2 + index] = this.PackUV(num5, 1f, num6);
				uvs2[3 + index] = this.PackUV(num5, 0f, num6);
				float num8 = (vertices[index + 4].x - start.x) / (end.x - start.x);
				num5 = (vertices[index + 6].x - start.x) / (end.x - start.x);
				uvs2[4 + index] = this.PackUV(num8, 0f, num7);
				uvs2[5 + index] = this.PackUV(num8, 1f, num7);
				uvs2[6 + index] = this.PackUV(num5, 1f, num7);
				uvs2[7 + index] = this.PackUV(num5, 0f, num7);
				num8 = (vertices[index + 8].x - start.x) / (end.x - start.x);
				num5 = (vertices[index + 6].x - start.x) / (end.x - start.x);
				uvs2[8 + index] = this.PackUV(num8, 0f, num6);
				uvs2[9 + index] = this.PackUV(num8, 1f, num6);
				uvs2[10 + index] = this.PackUV(1f, 1f, num6);
				uvs2[11 + index] = this.PackUV(1f, 0f, num6);
				Color32[] colors = this.m_textInfo.meshInfo[0].colors32;
				colors[index] = underlineColor;
				colors[1 + index] = underlineColor;
				colors[2 + index] = underlineColor;
				colors[3 + index] = underlineColor;
				colors[4 + index] = underlineColor;
				colors[5 + index] = underlineColor;
				colors[6 + index] = underlineColor;
				colors[7 + index] = underlineColor;
				colors[8 + index] = underlineColor;
				colors[9 + index] = underlineColor;
				colors[10 + index] = underlineColor;
				colors[11 + index] = underlineColor;
				index += 12;
			}
		}

		protected void GetSpecialCharacters(TMP_FontAsset fontAsset)
		{
			if (!fontAsset.characterDictionary.TryGetValue(95, out this.m_cached_Underline_GlyphInfo))
			{
			}
			if (!fontAsset.characterDictionary.TryGetValue(8230, out this.m_cached_Ellipsis_GlyphInfo))
			{
			}
		}

		protected TMP_FontAsset GetFontAssetForWeight(int fontWeight)
		{
			bool flag = (this.m_style & FontStyles.Italic) == FontStyles.Italic || (this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic;
			int num = fontWeight / 100;
			TMP_FontAsset tmp_FontAsset;
			if (flag)
			{
				tmp_FontAsset = this.m_currentFontAsset.fontWeights[num].italicTypeface;
			}
			else
			{
				tmp_FontAsset = this.m_currentFontAsset.fontWeights[num].regularTypeface;
			}
			return tmp_FontAsset;
		}

		protected virtual void SetActiveSubMeshes(bool state)
		{
		}

		protected Vector2 PackUV(float x, float y, float scale)
		{
			Vector2 vector;
			vector.x = Mathf.Floor(x * 511f);
			vector.y = Mathf.Floor(y * 511f);
			vector.x = vector.x * 4096f + vector.y;
			vector.y = scale;
			return vector;
		}

		protected float PackUV(float x, float y)
		{
			double num = Math.Floor((double)(x * 511f));
			double num2 = Math.Floor((double)(y * 511f));
			return (float)(num * 4096.0 + num2);
		}

		protected int HexToInt(char hex)
		{
			int num;
			switch (hex)
			{
			case '0':
				num = 0;
				break;
			case '1':
				num = 1;
				break;
			case '2':
				num = 2;
				break;
			case '3':
				num = 3;
				break;
			case '4':
				num = 4;
				break;
			case '5':
				num = 5;
				break;
			case '6':
				num = 6;
				break;
			case '7':
				num = 7;
				break;
			case '8':
				num = 8;
				break;
			case '9':
				num = 9;
				break;
			default:
				switch (hex)
				{
				case 'a':
					num = 10;
					break;
				case 'b':
					num = 11;
					break;
				case 'c':
					num = 12;
					break;
				case 'd':
					num = 13;
					break;
				case 'e':
					num = 14;
					break;
				case 'f':
					num = 15;
					break;
				default:
					num = 15;
					break;
				}
				break;
			case 'A':
				num = 10;
				break;
			case 'B':
				num = 11;
				break;
			case 'C':
				num = 12;
				break;
			case 'D':
				num = 13;
				break;
			case 'E':
				num = 14;
				break;
			case 'F':
				num = 15;
				break;
			}
			return num;
		}

		protected int GetUTF16(int i)
		{
			int num = this.HexToInt(this.m_text[i]) * 4096;
			num += this.HexToInt(this.m_text[i + 1]) * 256;
			num += this.HexToInt(this.m_text[i + 2]) * 16;
			return num + this.HexToInt(this.m_text[i + 3]);
		}

		protected int GetUTF32(int i)
		{
			int num = 0;
			num += this.HexToInt(this.m_text[i]) * 268435456;
			num += this.HexToInt(this.m_text[i + 1]) * 16777216;
			num += this.HexToInt(this.m_text[i + 2]) * 1048576;
			num += this.HexToInt(this.m_text[i + 3]) * 65536;
			num += this.HexToInt(this.m_text[i + 4]) * 4096;
			num += this.HexToInt(this.m_text[i + 5]) * 256;
			num += this.HexToInt(this.m_text[i + 6]) * 16;
			return num + this.HexToInt(this.m_text[i + 7]);
		}

		protected Color32 HexCharsToColor(char[] hexChars, int tagCount)
		{
			Color32 color;
			if (tagCount == 7)
			{
				byte b = (byte)(this.HexToInt(hexChars[1]) * 16 + this.HexToInt(hexChars[2]));
				byte b2 = (byte)(this.HexToInt(hexChars[3]) * 16 + this.HexToInt(hexChars[4]));
				byte b3 = (byte)(this.HexToInt(hexChars[5]) * 16 + this.HexToInt(hexChars[6]));
				color = new Color32(b, b2, b3, byte.MaxValue);
			}
			else if (tagCount == 9)
			{
				byte b4 = (byte)(this.HexToInt(hexChars[1]) * 16 + this.HexToInt(hexChars[2]));
				byte b5 = (byte)(this.HexToInt(hexChars[3]) * 16 + this.HexToInt(hexChars[4]));
				byte b6 = (byte)(this.HexToInt(hexChars[5]) * 16 + this.HexToInt(hexChars[6]));
				byte b7 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				color = new Color32(b4, b5, b6, b7);
			}
			else if (tagCount == 13)
			{
				byte b8 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				byte b9 = (byte)(this.HexToInt(hexChars[9]) * 16 + this.HexToInt(hexChars[10]));
				byte b10 = (byte)(this.HexToInt(hexChars[11]) * 16 + this.HexToInt(hexChars[12]));
				color = new Color32(b8, b9, b10, byte.MaxValue);
			}
			else if (tagCount == 15)
			{
				byte b11 = (byte)(this.HexToInt(hexChars[7]) * 16 + this.HexToInt(hexChars[8]));
				byte b12 = (byte)(this.HexToInt(hexChars[9]) * 16 + this.HexToInt(hexChars[10]));
				byte b13 = (byte)(this.HexToInt(hexChars[11]) * 16 + this.HexToInt(hexChars[12]));
				byte b14 = (byte)(this.HexToInt(hexChars[13]) * 16 + this.HexToInt(hexChars[14]));
				color = new Color32(b11, b12, b13, b14);
			}
			else
			{
				color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
			return color;
		}

		protected Color32 HexCharsToColor(char[] hexChars, int startIndex, int length)
		{
			Color32 color;
			if (length == 7)
			{
				byte b = (byte)(this.HexToInt(hexChars[startIndex + 1]) * 16 + this.HexToInt(hexChars[startIndex + 2]));
				byte b2 = (byte)(this.HexToInt(hexChars[startIndex + 3]) * 16 + this.HexToInt(hexChars[startIndex + 4]));
				byte b3 = (byte)(this.HexToInt(hexChars[startIndex + 5]) * 16 + this.HexToInt(hexChars[startIndex + 6]));
				color = new Color32(b, b2, b3, byte.MaxValue);
			}
			else if (length == 9)
			{
				byte b4 = (byte)(this.HexToInt(hexChars[startIndex + 1]) * 16 + this.HexToInt(hexChars[startIndex + 2]));
				byte b5 = (byte)(this.HexToInt(hexChars[startIndex + 3]) * 16 + this.HexToInt(hexChars[startIndex + 4]));
				byte b6 = (byte)(this.HexToInt(hexChars[startIndex + 5]) * 16 + this.HexToInt(hexChars[startIndex + 6]));
				byte b7 = (byte)(this.HexToInt(hexChars[startIndex + 7]) * 16 + this.HexToInt(hexChars[startIndex + 8]));
				color = new Color32(b4, b5, b6, b7);
			}
			else
			{
				color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
			return color;
		}

		protected float ConvertToFloat(char[] chars, int startIndex, int length, int decimalPointIndex)
		{
			float num;
			if (startIndex == 0)
			{
				num = -9999f;
			}
			else
			{
				int num2 = startIndex + length - 1;
				float num3 = 0f;
				float num4 = 1f;
				decimalPointIndex = ((decimalPointIndex <= 0) ? (num2 + 1) : decimalPointIndex);
				if (chars[startIndex] == '-')
				{
					startIndex++;
					num4 = -1f;
				}
				if (chars[startIndex] == '+' || chars[startIndex] == '%')
				{
					startIndex++;
				}
				for (int i = startIndex; i < num2 + 1; i++)
				{
					if (!char.IsDigit(chars[i]) && chars[i] != '.')
					{
						return -9999f;
					}
					int num5 = decimalPointIndex - i;
					switch (num5 + 3)
					{
					case 0:
						num3 += (float)(chars[i] - '0') * 0.001f;
						break;
					case 1:
						num3 += (float)(chars[i] - '0') * 0.01f;
						break;
					case 2:
						num3 += (float)(chars[i] - '0') * 0.1f;
						break;
					case 4:
						num3 += (float)(chars[i] - '0');
						break;
					case 5:
						num3 += (float)((chars[i] - '0') * '\n');
						break;
					case 6:
						num3 += (float)((chars[i] - '0') * 'd');
						break;
					case 7:
						num3 += (float)((chars[i] - '0') * 'Ϩ');
						break;
					}
				}
				num = num3 * num4;
			}
			return num;
		}

		protected bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
		{
			int num = 0;
			byte b = 0;
			TagUnits tagUnits = TagUnits.Pixels;
			TagType tagType = TagType.None;
			int num2 = 0;
			this.m_xmlAttribute[num2].nameHashCode = 0;
			this.m_xmlAttribute[num2].valueType = TagType.None;
			this.m_xmlAttribute[num2].valueHashCode = 0;
			this.m_xmlAttribute[num2].valueStartIndex = 0;
			this.m_xmlAttribute[num2].valueLength = 0;
			this.m_xmlAttribute[num2].valueDecimalIndex = 0;
			endIndex = startIndex;
			bool flag = false;
			bool flag2 = false;
			int num3 = startIndex;
			while (num3 < chars.Length && chars[num3] != 0 && num < this.m_htmlTag.Length && chars[num3] != 60)
			{
				if (chars[num3] == 62)
				{
					flag2 = true;
					endIndex = num3;
					this.m_htmlTag[num] = '\0';
					break;
				}
				this.m_htmlTag[num] = (char)chars[num3];
				num++;
				if (b == 1)
				{
					if (this.m_xmlAttribute[num2].valueStartIndex == 0)
					{
						if (chars[num3] == 43 || chars[num3] == 45 || char.IsDigit((char)chars[num3]))
						{
							tagType = TagType.NumericalValue;
							this.m_xmlAttribute[num2].valueType = TagType.NumericalValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							XML_TagAttribute[] xmlAttribute = this.m_xmlAttribute;
							int num4 = num2;
							xmlAttribute[num4].valueLength = xmlAttribute[num4].valueLength + 1;
						}
						else if (chars[num3] == 35)
						{
							tagType = TagType.ColorValue;
							this.m_xmlAttribute[num2].valueType = TagType.ColorValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							XML_TagAttribute[] xmlAttribute2 = this.m_xmlAttribute;
							int num5 = num2;
							xmlAttribute2[num5].valueLength = xmlAttribute2[num5].valueLength + 1;
						}
						else if (chars[num3] != 34)
						{
							tagType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueType = TagType.StringValue;
							this.m_xmlAttribute[num2].valueStartIndex = num - 1;
							this.m_xmlAttribute[num2].valueHashCode = ((this.m_xmlAttribute[num2].valueHashCode << 5) + this.m_xmlAttribute[num2].valueHashCode) ^ chars[num3];
							XML_TagAttribute[] xmlAttribute3 = this.m_xmlAttribute;
							int num6 = num2;
							xmlAttribute3[num6].valueLength = xmlAttribute3[num6].valueLength + 1;
						}
					}
					else if (tagType == TagType.NumericalValue)
					{
						if (chars[num3] == 46)
						{
							this.m_xmlAttribute[num2].valueDecimalIndex = num - 1;
						}
						if (chars[num3] == 112 || chars[num3] == 101 || chars[num3] == 37 || chars[num3] == 32)
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
							if (chars[num3] == 101)
							{
								tagUnits = TagUnits.FontUnits;
							}
							else if (chars[num3] == 37)
							{
								tagUnits = TagUnits.Percentage;
							}
						}
						else if (b != 2)
						{
							XML_TagAttribute[] xmlAttribute4 = this.m_xmlAttribute;
							int num7 = num2;
							xmlAttribute4[num7].valueLength = xmlAttribute4[num7].valueLength + 1;
						}
					}
					else if (tagType == TagType.ColorValue)
					{
						if (chars[num3] != 32)
						{
							XML_TagAttribute[] xmlAttribute5 = this.m_xmlAttribute;
							int num8 = num2;
							xmlAttribute5[num8].valueLength = xmlAttribute5[num8].valueLength + 1;
						}
						else
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
						}
					}
					else if (tagType == TagType.StringValue)
					{
						if (chars[num3] != 34)
						{
							this.m_xmlAttribute[num2].valueHashCode = ((this.m_xmlAttribute[num2].valueHashCode << 5) + this.m_xmlAttribute[num2].valueHashCode) ^ chars[num3];
							XML_TagAttribute[] xmlAttribute6 = this.m_xmlAttribute;
							int num9 = num2;
							xmlAttribute6[num9].valueLength = xmlAttribute6[num9].valueLength + 1;
						}
						else
						{
							b = 2;
							tagType = TagType.None;
							num2++;
							this.m_xmlAttribute[num2].nameHashCode = 0;
							this.m_xmlAttribute[num2].valueType = TagType.None;
							this.m_xmlAttribute[num2].valueHashCode = 0;
							this.m_xmlAttribute[num2].valueStartIndex = 0;
							this.m_xmlAttribute[num2].valueLength = 0;
							this.m_xmlAttribute[num2].valueDecimalIndex = 0;
						}
					}
				}
				if (chars[num3] == 61)
				{
					b = 1;
				}
				if (b == 0 && chars[num3] == 32)
				{
					if (flag)
					{
						return false;
					}
					flag = true;
					b = 2;
					tagType = TagType.None;
					num2++;
					this.m_xmlAttribute[num2].nameHashCode = 0;
					this.m_xmlAttribute[num2].valueType = TagType.None;
					this.m_xmlAttribute[num2].valueHashCode = 0;
					this.m_xmlAttribute[num2].valueStartIndex = 0;
					this.m_xmlAttribute[num2].valueLength = 0;
					this.m_xmlAttribute[num2].valueDecimalIndex = 0;
				}
				if (b == 0)
				{
					this.m_xmlAttribute[num2].nameHashCode = (this.m_xmlAttribute[num2].nameHashCode << 3) - this.m_xmlAttribute[num2].nameHashCode + chars[num3];
				}
				if (b == 2 && chars[num3] == 32)
				{
					b = 0;
				}
				num3++;
			}
			if (!flag2)
			{
				return false;
			}
			if (this.tag_NoParsing && this.m_xmlAttribute[0].nameHashCode != 53822163)
			{
				return false;
			}
			if (this.m_xmlAttribute[0].nameHashCode == 53822163)
			{
				this.tag_NoParsing = false;
				return true;
			}
			if (this.m_htmlTag[0] == '#' && num == 7)
			{
				this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			if (this.m_htmlTag[0] == '#' && num == 9)
			{
				this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			int nameHashCode = this.m_xmlAttribute[0].nameHashCode;
			switch (nameHashCode)
			{
			case 115:
				this.m_style |= FontStyles.Strikethrough;
				return true;
			default:
				if (nameHashCode == 426)
				{
					return true;
				}
				if (nameHashCode == 427)
				{
					if ((this.m_fontStyle & FontStyles.Bold) != FontStyles.Bold)
					{
						this.m_style &= (FontStyles)(-2);
						this.m_fontWeightInternal = this.m_fontWeightStack.Remove();
					}
					return true;
				}
				switch (nameHashCode)
				{
				case 444:
					if ((this.m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough)
					{
						this.m_style &= (FontStyles)(-65);
					}
					return true;
				default:
					if (nameHashCode != 13526026)
					{
						if (nameHashCode == 730022849)
						{
							this.m_style |= FontStyles.LowerCase;
							return true;
						}
						if (nameHashCode == 766244328)
						{
							this.m_style |= FontStyles.SmallCaps;
							return true;
						}
						if (nameHashCode != 781906058)
						{
							if (nameHashCode != 1100728678)
							{
								if (nameHashCode != 1109349752)
								{
									if (nameHashCode != 1109386397)
									{
										if (nameHashCode == -1885698441)
										{
											this.m_fontWeightInternal = this.m_fontWeightStack.Remove();
											if (this.m_fontWeightInternal == 400)
											{
												this.m_style &= (FontStyles)(-2);
											}
											return true;
										}
										if (nameHashCode == -1668324918)
										{
											this.m_style &= (FontStyles)(-9);
											return true;
										}
										if (nameHashCode != -1632103439)
										{
											if (nameHashCode != -1616441709)
											{
												if (nameHashCode != -884817987)
												{
													if (nameHashCode == -445573839)
													{
														this.m_lineHeight = 0f;
														return true;
													}
													if (nameHashCode == -445537194)
													{
														this.tag_LineIndent = 0f;
														return true;
													}
													if (nameHashCode != -330774850)
													{
														if (nameHashCode == 98)
														{
															this.m_style |= FontStyles.Bold;
															this.m_fontWeightInternal = 700;
															this.m_fontWeightStack.Add(700);
															return true;
														}
														if (nameHashCode == 105)
														{
															this.m_style |= FontStyles.Italic;
															return true;
														}
														if (nameHashCode == 434)
														{
															this.m_style &= (FontStyles)(-3);
															return true;
														}
														if (nameHashCode != 6380)
														{
															if (nameHashCode == 6552)
															{
																this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_currentFontAsset.fontInfo.SubSize);
																this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SubscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
																this.m_style |= FontStyles.Subscript;
																return true;
															}
															if (nameHashCode == 6566)
															{
																this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_currentFontAsset.fontInfo.SubSize);
																this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SuperscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
																this.m_style |= FontStyles.Superscript;
																return true;
															}
															if (nameHashCode == 22501)
															{
																this.m_isIgnoringAlignment = false;
																return true;
															}
															if (nameHashCode == 22673)
															{
																if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript)
																{
																	if ((this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
																	{
																		this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_currentFontAsset.fontInfo.SubSize);
																		this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SuperscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
																	}
																	else
																	{
																		this.m_baselineOffset = 0f;
																		this.m_fontScaleMultiplier = 1f;
																	}
																	this.m_style &= (FontStyles)(-257);
																}
																return true;
															}
															if (nameHashCode == 22687)
															{
																if ((this.m_style & FontStyles.Superscript) == FontStyles.Superscript)
																{
																	if ((this.m_style & FontStyles.Subscript) == FontStyles.Subscript)
																	{
																		this.m_fontScaleMultiplier = ((this.m_currentFontAsset.fontInfo.SubSize <= 0f) ? 1f : this.m_currentFontAsset.fontInfo.SubSize);
																		this.m_baselineOffset = this.m_currentFontAsset.fontInfo.SubscriptOffset * this.m_fontScale * this.m_fontScaleMultiplier;
																	}
																	else
																	{
																		this.m_baselineOffset = 0f;
																		this.m_fontScaleMultiplier = 1f;
																	}
																	this.m_style &= (FontStyles)(-129);
																}
																return true;
															}
															if (nameHashCode != 41311)
															{
																if (nameHashCode == 43066)
																{
																	if (this.m_isParsingText)
																	{
																		int num10 = this.m_textInfo.linkInfo.Length;
																		if (this.m_textInfo.linkCount + 1 > num10)
																		{
																			TMP_TextInfo.Resize<TMP_LinkInfo>(ref this.m_textInfo.linkInfo, num10 + 1);
																		}
																		int linkCount = this.m_textInfo.linkCount;
																		this.m_textInfo.linkInfo[linkCount].textComponent = this;
																		this.m_textInfo.linkInfo[linkCount].hashCode = this.m_xmlAttribute[0].valueHashCode;
																		this.m_textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = this.m_characterCount;
																		this.m_textInfo.linkInfo[linkCount].linkIdFirstCharacterIndex = startIndex + this.m_xmlAttribute[0].valueStartIndex;
																		this.m_textInfo.linkInfo[linkCount].linkIdLength = this.m_xmlAttribute[0].valueLength;
																		this.m_textInfo.linkInfo[linkCount].SetLinkID(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength);
																	}
																	return true;
																}
																if (nameHashCode == 43969)
																{
																	this.m_isNonBreakingSpace = true;
																	return true;
																}
																if (nameHashCode == 43991)
																{
																	if (this.m_overflowMode == TextOverflowModes.Page)
																	{
																		this.m_xAdvance = this.tag_LineIndent + this.tag_Indent;
																		this.m_lineOffset = 0f;
																		this.m_pageNumber++;
																		this.m_isNewPage = true;
																	}
																	return true;
																}
																if (nameHashCode != 45545)
																{
																	if (nameHashCode == 154158)
																	{
																		MaterialReference materialReference = this.m_materialReferenceStack.Remove();
																		this.m_currentFontAsset = materialReference.fontAsset;
																		this.m_currentMaterial = materialReference.material;
																		this.m_currentMaterialIndex = materialReference.index;
																		this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																		return true;
																	}
																	if (nameHashCode == 155913)
																	{
																		if (this.m_isParsingText)
																		{
																			this.m_textInfo.linkInfo[this.m_textInfo.linkCount].linkTextLength = this.m_characterCount - this.m_textInfo.linkInfo[this.m_textInfo.linkCount].linkTextfirstCharacterIndex;
																			this.m_textInfo.linkCount++;
																		}
																		return true;
																	}
																	if (nameHashCode == 156816)
																	{
																		this.m_isNonBreakingSpace = false;
																		return true;
																	}
																	if (nameHashCode == 158392)
																	{
																		this.m_currentFontSize = this.m_sizeStack.Remove();
																		this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																		return true;
																	}
																	if (nameHashCode != 275917)
																	{
																		if (nameHashCode != 276254)
																		{
																			if (nameHashCode == 280416)
																			{
																				return false;
																			}
																			if (nameHashCode != 281955)
																			{
																				if (nameHashCode != 320078)
																				{
																					if (nameHashCode != 322689)
																					{
																						if (nameHashCode != 327550)
																						{
																							if (nameHashCode == 1065846)
																							{
																								this.m_lineJustification = this.m_textAlignment;
																								return true;
																							}
																							if (nameHashCode == 1071884)
																							{
																								this.m_htmlColor = this.m_colorStack.Remove();
																								return true;
																							}
																							if (nameHashCode != 1112618)
																							{
																								if (nameHashCode == 1117479)
																								{
																									this.m_width = -1f;
																									return true;
																								}
																								if (nameHashCode == 1750458)
																								{
																									return false;
																								}
																								if (nameHashCode == 1913798)
																								{
																									int valueHashCode = this.m_xmlAttribute[0].valueHashCode;
																									if (this.m_isParsingText)
																									{
																										this.m_actionStack.Add(valueHashCode);
																										global::Debug.Log(string.Concat(new object[] { "Action ID: [", valueHashCode, "] First character index: ", this.m_characterCount }), null);
																									}
																									return true;
																								}
																								if (nameHashCode != 1983971)
																								{
																									if (nameHashCode != 2068980)
																									{
																										if (nameHashCode != 2109854)
																										{
																											if (nameHashCode != 2152041)
																											{
																												if (nameHashCode == 2246877)
																												{
																													int valueHashCode2 = this.m_xmlAttribute[0].valueHashCode;
																													TMP_SpriteAsset tmp_SpriteAsset;
																													if (this.m_xmlAttribute[0].valueType == TagType.None || this.m_xmlAttribute[0].valueType == TagType.NumericalValue)
																													{
																														if (this.m_defaultSpriteAsset == null)
																														{
																															if (TMP_Settings.defaultSpriteAsset != null)
																															{
																																this.m_defaultSpriteAsset = TMP_Settings.defaultSpriteAsset;
																															}
																															else
																															{
																																this.m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
																															}
																														}
																														this.m_currentSpriteAsset = this.m_defaultSpriteAsset;
																														if (this.m_currentSpriteAsset == null)
																														{
																															return false;
																														}
																													}
																													else if (MaterialReferenceManager.TryGetSpriteAsset(valueHashCode2, out tmp_SpriteAsset))
																													{
																														this.m_currentSpriteAsset = tmp_SpriteAsset;
																													}
																													else
																													{
																														if (tmp_SpriteAsset == null)
																														{
																															tmp_SpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprites/" + new string(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength));
																														}
																														if (tmp_SpriteAsset == null)
																														{
																															return false;
																														}
																														MaterialReferenceManager.AddSpriteAsset(valueHashCode2, tmp_SpriteAsset);
																														this.m_currentSpriteAsset = tmp_SpriteAsset;
																													}
																													if (this.m_xmlAttribute[0].valueType == TagType.NumericalValue)
																													{
																														int num11 = (int)this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																														if (num11 == -9999)
																														{
																															return false;
																														}
																														if (num11 > this.m_currentSpriteAsset.spriteInfoList.Count - 1)
																														{
																															return false;
																														}
																														this.m_spriteIndex = num11;
																													}
																													else if (this.m_xmlAttribute[1].nameHashCode == 43347)
																													{
																														int spriteIndex = this.m_currentSpriteAsset.GetSpriteIndex(this.m_xmlAttribute[1].valueHashCode);
																														if (spriteIndex == -1)
																														{
																															return false;
																														}
																														this.m_spriteIndex = spriteIndex;
																													}
																													else
																													{
																														if (this.m_xmlAttribute[1].nameHashCode != 295562)
																														{
																															return false;
																														}
																														int num12 = (int)this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength, this.m_xmlAttribute[1].valueDecimalIndex);
																														if (num12 == -9999)
																														{
																															return false;
																														}
																														if (num12 > this.m_currentSpriteAsset.spriteInfoList.Count - 1)
																														{
																															return false;
																														}
																														this.m_spriteIndex = num12;
																													}
																													this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentSpriteAsset.material, this.m_currentSpriteAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																													this.m_spriteColor = TMP_Text.s_colorWhite;
																													this.m_tintSprite = false;
																													if (this.m_xmlAttribute[1].nameHashCode == 45819)
																													{
																														this.m_tintSprite = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength, this.m_xmlAttribute[1].valueDecimalIndex) != 0f;
																													}
																													else if (this.m_xmlAttribute[2].nameHashCode == 45819)
																													{
																														this.m_tintSprite = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[2].valueStartIndex, this.m_xmlAttribute[2].valueLength, this.m_xmlAttribute[2].valueDecimalIndex) != 0f;
																													}
																													if (this.m_xmlAttribute[1].nameHashCode == 281955)
																													{
																														this.m_spriteColor = this.HexCharsToColor(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength);
																													}
																													else if (this.m_xmlAttribute[2].nameHashCode == 281955)
																													{
																														this.m_spriteColor = this.HexCharsToColor(this.m_htmlTag, this.m_xmlAttribute[2].valueStartIndex, this.m_xmlAttribute[2].valueLength);
																													}
																													this.m_xmlAttribute[1].nameHashCode = 0;
																													this.m_xmlAttribute[2].nameHashCode = 0;
																													this.m_textElementType = TMP_TextElementType.Sprite;
																													return true;
																												}
																												if (nameHashCode == 7443301)
																												{
																													if (this.m_isParsingText)
																													{
																														global::Debug.Log(string.Concat(new object[]
																														{
																															"Action ID: [",
																															this.m_actionStack.CurrentItem(),
																															"] Last character index: ",
																															this.m_characterCount - 1
																														}), null);
																													}
																													this.m_actionStack.Remove();
																													return true;
																												}
																												if (nameHashCode == 7513474)
																												{
																													this.m_cSpacing = 0f;
																													return true;
																												}
																												if (nameHashCode == 7598483)
																												{
																													this.tag_Indent = this.m_indentStack.Remove();
																													return true;
																												}
																												if (nameHashCode == 7639357)
																												{
																													this.m_marginLeft = 0f;
																													this.m_marginRight = 0f;
																													return true;
																												}
																												if (nameHashCode == 7681544)
																												{
																													this.m_monoSpacing = 0f;
																													return true;
																												}
																												if (nameHashCode == 15115642)
																												{
																													this.tag_NoParsing = true;
																													return true;
																												}
																												if (nameHashCode != 16034505)
																												{
																													if (nameHashCode != 52232547)
																													{
																														if (nameHashCode != 54741026)
																														{
																															return false;
																														}
																														this.m_baselineOffset = 0f;
																														return true;
																													}
																												}
																												else
																												{
																													float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																													if (num13 == -9999f || num13 == 0f)
																													{
																														return false;
																													}
																													if (tagUnits == TagUnits.Pixels)
																													{
																														this.m_baselineOffset = num13;
																														return true;
																													}
																													if (tagUnits != TagUnits.FontUnits)
																													{
																														return tagUnits != TagUnits.Percentage && false;
																													}
																													this.m_baselineOffset = num13 * this.m_fontScale * this.m_fontAsset.fontInfo.Ascender;
																													return true;
																												}
																											}
																											else
																											{
																												float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																												if (num13 == -9999f || num13 == 0f)
																												{
																													return false;
																												}
																												if (tagUnits != TagUnits.Pixels)
																												{
																													if (tagUnits != TagUnits.FontUnits)
																													{
																														if (tagUnits == TagUnits.Percentage)
																														{
																															return false;
																														}
																													}
																													else
																													{
																														this.m_monoSpacing = num13;
																														this.m_monoSpacing *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																													}
																												}
																												else
																												{
																													this.m_monoSpacing = num13;
																												}
																												return true;
																											}
																										}
																										else
																										{
																											float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																											if (num13 == -9999f || num13 == 0f)
																											{
																												return false;
																											}
																											this.m_marginLeft = num13;
																											if (tagUnits != TagUnits.Pixels)
																											{
																												if (tagUnits != TagUnits.FontUnits)
																												{
																													if (tagUnits == TagUnits.Percentage)
																													{
																														this.m_marginLeft = (this.m_marginWidth - ((this.m_width == -1f) ? 0f : this.m_width)) * this.m_marginLeft / 100f;
																													}
																												}
																												else
																												{
																													this.m_marginLeft *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																												}
																											}
																											this.m_marginLeft = ((this.m_marginLeft < 0f) ? 0f : this.m_marginLeft);
																											this.m_marginRight = this.m_marginLeft;
																											return true;
																										}
																									}
																									else
																									{
																										float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																										if (num13 == -9999f || num13 == 0f)
																										{
																											return false;
																										}
																										if (tagUnits != TagUnits.Pixels)
																										{
																											if (tagUnits != TagUnits.FontUnits)
																											{
																												if (tagUnits == TagUnits.Percentage)
																												{
																													this.tag_Indent = this.m_marginWidth * num13 / 100f;
																												}
																											}
																											else
																											{
																												this.tag_Indent = num13;
																												this.tag_Indent *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																											}
																										}
																										else
																										{
																											this.tag_Indent = num13;
																										}
																										this.m_indentStack.Add(this.tag_Indent);
																										this.m_xAdvance = this.tag_Indent;
																										return true;
																									}
																								}
																								else
																								{
																									float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																									if (num13 == -9999f || num13 == 0f)
																									{
																										return false;
																									}
																									if (tagUnits != TagUnits.Pixels)
																									{
																										if (tagUnits != TagUnits.FontUnits)
																										{
																											if (tagUnits == TagUnits.Percentage)
																											{
																												return false;
																											}
																										}
																										else
																										{
																											this.m_cSpacing = num13;
																											this.m_cSpacing *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																										}
																									}
																									else
																									{
																										this.m_cSpacing = num13;
																									}
																									return true;
																								}
																							}
																							else
																							{
																								TMP_Style tmp_Style = TMP_StyleSheet.GetStyle(this.m_xmlAttribute[0].valueHashCode);
																								if (tmp_Style == null)
																								{
																									int num14 = this.m_styleStack.Remove();
																									tmp_Style = TMP_StyleSheet.GetStyle(num14);
																								}
																								if (tmp_Style == null)
																								{
																									return false;
																								}
																								for (int i = 0; i < tmp_Style.styleClosingTagArray.Length; i++)
																								{
																									if (tmp_Style.styleClosingTagArray[i] == 60)
																									{
																										this.ValidateHtmlTag(tmp_Style.styleClosingTagArray, i + 1, out i);
																									}
																								}
																								return true;
																							}
																						}
																						else
																						{
																							float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																							if (num13 == -9999f || num13 == 0f)
																							{
																								return false;
																							}
																							if (tagUnits != TagUnits.Pixels)
																							{
																								if (tagUnits == TagUnits.FontUnits)
																								{
																									return false;
																								}
																								if (tagUnits == TagUnits.Percentage)
																								{
																									this.m_width = this.m_marginWidth * num13 / 100f;
																								}
																							}
																							else
																							{
																								this.m_width = num13;
																							}
																							return true;
																						}
																					}
																					else
																					{
																						TMP_Style tmp_Style = TMP_StyleSheet.GetStyle(this.m_xmlAttribute[0].valueHashCode);
																						if (tmp_Style == null)
																						{
																							return false;
																						}
																						this.m_styleStack.Add(tmp_Style.hashCode);
																						for (int j = 0; j < tmp_Style.styleOpeningTagArray.Length; j++)
																						{
																							if (tmp_Style.styleOpeningTagArray[j] == 60)
																							{
																								this.ValidateHtmlTag(tmp_Style.styleOpeningTagArray, j + 1, out j);
																							}
																						}
																						return true;
																					}
																				}
																				else
																				{
																					float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																					if (num13 == -9999f || num13 == 0f)
																					{
																						return false;
																					}
																					if (tagUnits == TagUnits.Pixels)
																					{
																						this.m_xAdvance += num13;
																						return true;
																					}
																					if (tagUnits != TagUnits.FontUnits)
																					{
																						return tagUnits != TagUnits.Percentage && false;
																					}
																					this.m_xAdvance += num13 * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																					return true;
																				}
																			}
																			else
																			{
																				if (this.m_htmlTag[6] == '#' && num == 13)
																				{
																					this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				if (this.m_htmlTag[6] == '#' && num == 15)
																				{
																					this.m_htmlColor = this.HexCharsToColor(this.m_htmlTag, num);
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				int valueHashCode3 = this.m_xmlAttribute[0].valueHashCode;
																				if (valueHashCode3 == -36881330)
																				{
																					this.m_htmlColor = new Color32(160, 32, 240, byte.MaxValue);
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				if (valueHashCode3 == 125395)
																				{
																					this.m_htmlColor = Color.red;
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				if (valueHashCode3 == 3573310)
																				{
																					this.m_htmlColor = Color.blue;
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				if (valueHashCode3 == 26556144)
																				{
																					this.m_htmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				if (valueHashCode3 == 117905991)
																				{
																					this.m_htmlColor = Color.black;
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				if (valueHashCode3 == 121463835)
																				{
																					this.m_htmlColor = Color.green;
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				if (valueHashCode3 == 140357351)
																				{
																					this.m_htmlColor = Color.white;
																					this.m_colorStack.Add(this.m_htmlColor);
																					return true;
																				}
																				if (valueHashCode3 != 554054276)
																				{
																					return false;
																				}
																				this.m_htmlColor = Color.yellow;
																				this.m_colorStack.Add(this.m_htmlColor);
																				return true;
																			}
																		}
																		else
																		{
																			if (this.m_xmlAttribute[0].valueLength != 3)
																			{
																				return false;
																			}
																			this.m_htmlColor.a = (byte)(this.HexToInt(this.m_htmlTag[7]) * 16 + this.HexToInt(this.m_htmlTag[8]));
																			return true;
																		}
																	}
																	else
																	{
																		int valueHashCode4 = this.m_xmlAttribute[0].valueHashCode;
																		if (valueHashCode4 == -523808257)
																		{
																			this.m_lineJustification = TextAlignmentOptions.Justified;
																			return true;
																		}
																		if (valueHashCode4 == -458210101)
																		{
																			this.m_lineJustification = TextAlignmentOptions.Center;
																			return true;
																		}
																		if (valueHashCode4 == 3774683)
																		{
																			this.m_lineJustification = TextAlignmentOptions.Left;
																			return true;
																		}
																		if (valueHashCode4 != 136703040)
																		{
																			return false;
																		}
																		this.m_lineJustification = TextAlignmentOptions.Right;
																		return true;
																	}
																}
																else
																{
																	float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
																	if (num13 == -9999f || num13 == 0f)
																	{
																		return false;
																	}
																	if (tagUnits != TagUnits.Pixels)
																	{
																		if (tagUnits == TagUnits.FontUnits)
																		{
																			this.m_currentFontSize = this.m_fontSize * num13;
																			this.m_sizeStack.Add(this.m_currentFontSize);
																			this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																			return true;
																		}
																		if (tagUnits != TagUnits.Percentage)
																		{
																			return false;
																		}
																		this.m_currentFontSize = this.m_fontSize * num13 / 100f;
																		this.m_sizeStack.Add(this.m_currentFontSize);
																		this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																		return true;
																	}
																	else
																	{
																		if (this.m_htmlTag[5] == '+')
																		{
																			this.m_currentFontSize = this.m_fontSize + num13;
																			this.m_sizeStack.Add(this.m_currentFontSize);
																			this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																			return true;
																		}
																		if (this.m_htmlTag[5] == '-')
																		{
																			this.m_currentFontSize = this.m_fontSize + num13;
																			this.m_sizeStack.Add(this.m_currentFontSize);
																			this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																			return true;
																		}
																		this.m_currentFontSize = num13;
																		this.m_sizeStack.Add(this.m_currentFontSize);
																		this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																		return true;
																	}
																}
															}
															else
															{
																int valueHashCode5 = this.m_xmlAttribute[0].valueHashCode;
																int nameHashCode2 = this.m_xmlAttribute[1].nameHashCode;
																int valueHashCode6 = this.m_xmlAttribute[1].valueHashCode;
																if (valueHashCode5 == 764638571 || valueHashCode5 == 523367755)
																{
																	this.m_currentFontAsset = this.m_materialReferences[0].fontAsset;
																	this.m_currentMaterial = this.m_materialReferences[0].material;
																	this.m_currentMaterialIndex = 0;
																	this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																	this.m_materialReferenceStack.Add(this.m_materialReferences[0]);
																	return true;
																}
																TMP_FontAsset tmp_FontAsset;
																if (!MaterialReferenceManager.TryGetFontAsset(valueHashCode5, out tmp_FontAsset))
																{
																	tmp_FontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/" + new string(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength));
																	if (tmp_FontAsset == null)
																	{
																		return false;
																	}
																	MaterialReferenceManager.AddFontAsset(tmp_FontAsset);
																}
																if (nameHashCode2 == 0 && valueHashCode6 == 0)
																{
																	this.m_currentMaterial = tmp_FontAsset.material;
																	this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																	this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
																}
																else
																{
																	if (nameHashCode2 != 103415287)
																	{
																		return false;
																	}
																	Material material;
																	if (MaterialReferenceManager.TryGetMaterial(valueHashCode6, out material))
																	{
																		this.m_currentMaterial = material;
																		this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																		this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
																	}
																	else
																	{
																		material = Resources.Load<Material>("Fonts & Materials/" + new string(this.m_htmlTag, this.m_xmlAttribute[1].valueStartIndex, this.m_xmlAttribute[1].valueLength));
																		if (material == null)
																		{
																			return false;
																		}
																		MaterialReferenceManager.AddFontMaterial(valueHashCode6, material);
																		this.m_currentMaterial = material;
																		this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, this.m_materialReferences, this.m_materialReferenceIndexLookup);
																		this.m_materialReferenceStack.Add(this.m_materialReferences[this.m_currentMaterialIndex]);
																	}
																}
																this.m_currentFontAsset = tmp_FontAsset;
																this.m_fontScale = this.m_currentFontSize / this.m_currentFontAsset.fontInfo.PointSize * this.m_currentFontAsset.fontInfo.Scale * ((!this.m_isOrthographic) ? 0.1f : 1f);
																return true;
															}
														}
														else
														{
															float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
															if (num13 == -9999f)
															{
																return false;
															}
															if (tagUnits == TagUnits.Pixels)
															{
																this.m_xAdvance = num13;
																return true;
															}
															if (tagUnits == TagUnits.FontUnits)
															{
																this.m_xAdvance = num13 * this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
																return true;
															}
															if (tagUnits != TagUnits.Percentage)
															{
																return false;
															}
															this.m_xAdvance = this.m_marginWidth * num13 / 100f;
															return true;
														}
													}
													else
													{
														float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
														if (num13 == -9999f || num13 == 0f)
														{
															return false;
														}
														if ((this.m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
														{
															return true;
														}
														this.m_style &= (FontStyles)(-2);
														int num15 = (int)num13;
														if (num15 != 100)
														{
															if (num15 != 200)
															{
																if (num15 != 300)
																{
																	if (num15 != 400)
																	{
																		if (num15 != 500)
																		{
																			if (num15 != 600)
																			{
																				if (num15 != 700)
																				{
																					if (num15 != 800)
																					{
																						if (num15 == 900)
																						{
																							this.m_fontWeightInternal = 900;
																						}
																					}
																					else
																					{
																						this.m_fontWeightInternal = 800;
																					}
																				}
																				else
																				{
																					this.m_fontWeightInternal = 700;
																					this.m_style |= FontStyles.Bold;
																				}
																			}
																			else
																			{
																				this.m_fontWeightInternal = 600;
																			}
																		}
																		else
																		{
																			this.m_fontWeightInternal = 500;
																		}
																	}
																	else
																	{
																		this.m_fontWeightInternal = 400;
																	}
																}
																else
																{
																	this.m_fontWeightInternal = 300;
																}
															}
															else
															{
																this.m_fontWeightInternal = 200;
															}
														}
														else
														{
															this.m_fontWeightInternal = 100;
														}
														this.m_fontWeightStack.Add(this.m_fontWeightInternal);
														return true;
													}
												}
												else
												{
													float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
													if (num13 == -9999f || num13 == 0f)
													{
														return false;
													}
													this.m_marginRight = num13;
													if (tagUnits != TagUnits.Pixels)
													{
														if (tagUnits != TagUnits.FontUnits)
														{
															if (tagUnits == TagUnits.Percentage)
															{
																this.m_marginRight = (this.m_marginWidth - ((this.m_width == -1f) ? 0f : this.m_width)) * this.m_marginRight / 100f;
															}
														}
														else
														{
															this.m_marginRight *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
														}
													}
													this.m_marginRight = ((this.m_marginRight < 0f) ? 0f : this.m_marginRight);
													return true;
												}
											}
											this.m_style &= (FontStyles)(-17);
											return true;
										}
										this.m_style &= (FontStyles)(-33);
										return true;
									}
									else
									{
										float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
										if (num13 == -9999f || num13 == 0f)
										{
											return false;
										}
										if (tagUnits != TagUnits.Pixels)
										{
											if (tagUnits != TagUnits.FontUnits)
											{
												if (tagUnits == TagUnits.Percentage)
												{
													this.tag_LineIndent = this.m_marginWidth * num13 / 100f;
												}
											}
											else
											{
												this.tag_LineIndent = num13;
												this.tag_LineIndent *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
											}
										}
										else
										{
											this.tag_LineIndent = num13;
										}
										this.m_xAdvance += this.tag_LineIndent;
										return true;
									}
								}
								else
								{
									float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
									if (num13 == -9999f || num13 == 0f)
									{
										return false;
									}
									this.m_lineHeight = num13;
									if (tagUnits != TagUnits.Pixels)
									{
										if (tagUnits != TagUnits.FontUnits)
										{
											if (tagUnits == TagUnits.Percentage)
											{
												this.m_lineHeight = this.m_fontAsset.fontInfo.LineHeight * this.m_lineHeight / 100f * this.m_fontScale;
											}
										}
										else
										{
											this.m_lineHeight *= this.m_fontAsset.fontInfo.LineHeight * this.m_fontScale;
										}
									}
									return true;
								}
							}
							else
							{
								float num13 = this.ConvertToFloat(this.m_htmlTag, this.m_xmlAttribute[0].valueStartIndex, this.m_xmlAttribute[0].valueLength, this.m_xmlAttribute[0].valueDecimalIndex);
								if (num13 == -9999f || num13 == 0f)
								{
									return false;
								}
								this.m_marginLeft = num13;
								if (tagUnits != TagUnits.Pixels)
								{
									if (tagUnits != TagUnits.FontUnits)
									{
										if (tagUnits == TagUnits.Percentage)
										{
											this.m_marginLeft = (this.m_marginWidth - ((this.m_width == -1f) ? 0f : this.m_width)) * this.m_marginLeft / 100f;
										}
									}
									else
									{
										this.m_marginLeft *= this.m_fontScale * this.m_fontAsset.fontInfo.TabWidth / (float)this.m_fontAsset.tabSize;
									}
								}
								this.m_marginLeft = ((this.m_marginLeft < 0f) ? 0f : this.m_marginLeft);
								return true;
							}
						}
					}
					this.m_style |= FontStyles.UpperCase;
					return true;
				case 446:
					if ((this.m_fontStyle & FontStyles.Underline) != FontStyles.Underline)
					{
						this.m_style &= (FontStyles)(-5);
					}
					return true;
				}
				break;
			case 117:
				this.m_style |= FontStyles.Underline;
				return true;
			}
		}

		[SerializeField]
		protected string m_text;

		[SerializeField]
		protected bool m_isRightToLeft = false;

		[SerializeField]
		protected TMP_FontAsset m_fontAsset;

		protected TMP_FontAsset m_currentFontAsset;

		protected bool m_isSDFShader;

		[SerializeField]
		protected Material m_sharedMaterial;

		protected Material m_currentMaterial;

		protected MaterialReference[] m_materialReferences = new MaterialReference[32];

		protected Dictionary<int, int> m_materialReferenceIndexLookup = new Dictionary<int, int>();

		protected TMP_XmlTagStack<MaterialReference> m_materialReferenceStack = new TMP_XmlTagStack<MaterialReference>(new MaterialReference[16]);

		protected int m_currentMaterialIndex;

		protected int m_sharedMaterialHashCode;

		[SerializeField]
		protected Material[] m_fontSharedMaterials;

		[SerializeField]
		protected Material m_fontMaterial;

		[SerializeField]
		protected Material[] m_fontMaterials;

		protected bool m_isMaterialDirty;

		[FormerlySerializedAs("m_fontColor")]
		[SerializeField]
		protected Color32 m_fontColor32 = Color.white;

		[SerializeField]
		protected Color m_fontColor = Color.white;

		protected static Color32 s_colorWhite = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		[SerializeField]
		protected bool m_enableVertexGradient;

		[SerializeField]
		protected VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		protected TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		protected bool m_tintAllSprites;

		protected bool m_tintSprite;

		protected Color32 m_spriteColor;

		[SerializeField]
		protected bool m_overrideHtmlColors = false;

		[SerializeField]
		protected Color32 m_faceColor = Color.white;

		[SerializeField]
		protected Color32 m_outlineColor = Color.black;

		protected float m_outlineWidth = 0f;

		[SerializeField]
		protected float m_fontSize = 36f;

		protected float m_currentFontSize;

		[SerializeField]
		protected float m_fontSizeBase = 36f;

		protected TMP_XmlTagStack<float> m_sizeStack = new TMP_XmlTagStack<float>(new float[16]);

		[SerializeField]
		protected int m_fontWeight = 400;

		protected int m_fontWeightInternal;

		protected TMP_XmlTagStack<int> m_fontWeightStack = new TMP_XmlTagStack<int>(new int[16]);

		[SerializeField]
		protected bool m_enableAutoSizing;

		protected float m_maxFontSize;

		protected float m_minFontSize;

		[SerializeField]
		protected float m_fontSizeMin = 0f;

		[SerializeField]
		protected float m_fontSizeMax = 0f;

		[SerializeField]
		protected FontStyles m_fontStyle = FontStyles.Normal;

		protected FontStyles m_style = FontStyles.Normal;

		protected bool m_isUsingBold = false;

		[SerializeField]
		[FormerlySerializedAs("m_lineJustification")]
		protected TextAlignmentOptions m_textAlignment = TextAlignmentOptions.TopLeft;

		protected TextAlignmentOptions m_lineJustification;

		protected Vector3[] m_textContainerLocalCorners = new Vector3[4];

		[SerializeField]
		protected float m_characterSpacing = 0f;

		protected float m_cSpacing = 0f;

		protected float m_monoSpacing = 0f;

		[SerializeField]
		protected float m_lineSpacing = 0f;

		protected float m_lineSpacingDelta = 0f;

		protected float m_lineHeight = 0f;

		[SerializeField]
		protected float m_lineSpacingMax = 0f;

		[SerializeField]
		protected float m_paragraphSpacing = 0f;

		[SerializeField]
		protected float m_charWidthMaxAdj = 0f;

		protected float m_charWidthAdjDelta = 0f;

		[SerializeField]
		protected bool m_enableWordWrapping = false;

		protected bool m_isCharacterWrappingEnabled = false;

		protected bool m_isNonBreakingSpace = false;

		protected bool m_isIgnoringAlignment;

		[SerializeField]
		protected float m_wordWrappingRatios = 0.4f;

		[SerializeField]
		protected TextOverflowModes m_overflowMode = TextOverflowModes.Overflow;

		protected bool m_isTextTruncated;

		[SerializeField]
		protected bool m_enableKerning;

		[SerializeField]
		protected bool m_enableExtraPadding = false;

		[SerializeField]
		protected bool checkPaddingRequired;

		[SerializeField]
		protected bool m_isRichText = true;

		[SerializeField]
		protected bool m_parseCtrlCharacters = true;

		protected bool m_isOverlay = false;

		[SerializeField]
		protected bool m_isOrthographic = false;

		[SerializeField]
		protected bool m_isCullingEnabled = false;

		[SerializeField]
		protected bool m_ignoreCulling = true;

		[SerializeField]
		protected TextureMappingOptions m_horizontalMapping = TextureMappingOptions.Character;

		[SerializeField]
		protected TextureMappingOptions m_verticalMapping = TextureMappingOptions.Character;

		protected TextRenderFlags m_renderMode = TextRenderFlags.Render;

		protected int m_maxVisibleCharacters = 99999;

		protected int m_maxVisibleWords = 99999;

		protected int m_maxVisibleLines = 99999;

		[SerializeField]
		protected bool m_useMaxVisibleDescender = true;

		[SerializeField]
		protected int m_pageToDisplay = 1;

		protected bool m_isNewPage = false;

		[SerializeField]
		protected Vector4 m_margin = new Vector4(0f, 0f, 0f, 0f);

		protected float m_marginLeft;

		protected float m_marginRight;

		protected float m_marginWidth;

		protected float m_marginHeight;

		protected float m_width = -1f;

		[SerializeField]
		protected TMP_TextInfo m_textInfo;

		[SerializeField]
		protected bool m_havePropertiesChanged;

		[SerializeField]
		protected bool m_isUsingLegacyAnimationComponent;

		protected Transform m_transform;

		protected RectTransform m_rectTransform;

		protected Mesh m_mesh;

		protected float m_flexibleHeight = -1f;

		protected float m_flexibleWidth = -1f;

		protected float m_minHeight;

		protected float m_minWidth;

		protected float m_preferredWidth = 9999f;

		protected float m_renderedWidth;

		protected float m_preferredHeight = 9999f;

		protected float m_renderedHeight;

		protected int m_layoutPriority = 0;

		protected bool m_isCalculateSizeRequired = false;

		protected bool m_isLayoutDirty;

		protected bool m_verticesAlreadyDirty;

		protected bool m_layoutAlreadyDirty;

		protected bool m_isAwake;

		[SerializeField]
		protected bool m_isInputParsingRequired = false;

		[SerializeField]
		protected TMP_Text.TextInputSources m_inputSource;

		protected string old_text;

		protected float old_arg0;

		protected float old_arg1;

		protected float old_arg2;

		protected float m_fontScale;

		protected float m_fontScaleMultiplier;

		protected char[] m_htmlTag = new char[128];

		protected XML_TagAttribute[] m_xmlAttribute = new XML_TagAttribute[8];

		protected float tag_LineIndent = 0f;

		protected float tag_Indent = 0f;

		protected TMP_XmlTagStack<float> m_indentStack = new TMP_XmlTagStack<float>(new float[16]);

		protected bool tag_NoParsing;

		protected bool m_isParsingText;

		protected int[] m_char_buffer;

		private TMP_CharacterInfo[] m_internalCharacterInfo;

		protected char[] m_input_CharArray = new char[256];

		private int m_charArray_Length = 0;

		protected int m_totalCharacterCount;

		protected int m_characterCount;

		protected int m_firstCharacterOfLine;

		protected int m_firstVisibleCharacterOfLine;

		protected int m_lastCharacterOfLine;

		protected int m_lastVisibleCharacterOfLine;

		protected int m_lineNumber;

		protected int m_lineVisibleCharacterCount;

		protected int m_pageNumber;

		protected float m_maxAscender;

		protected float m_maxDescender;

		protected float m_maxLineAscender;

		protected float m_maxLineDescender;

		protected float m_startOfLineAscender;

		protected float m_lineOffset;

		protected Extents m_meshExtents;

		protected Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);

		protected TMP_XmlTagStack<Color32> m_colorStack = new TMP_XmlTagStack<Color32>(new Color32[16]);

		protected float m_tabSpacing = 0f;

		protected float m_spacing = 0f;

		protected TMP_XmlTagStack<int> m_styleStack = new TMP_XmlTagStack<int>(new int[16]);

		protected TMP_XmlTagStack<int> m_actionStack = new TMP_XmlTagStack<int>(new int[16]);

		protected float m_padding = 0f;

		protected float m_baselineOffset;

		protected float m_xAdvance;

		protected TMP_TextElementType m_textElementType;

		protected TMP_TextElement m_cached_TextElement;

		protected TMP_Glyph m_cached_Underline_GlyphInfo;

		protected TMP_Glyph m_cached_Ellipsis_GlyphInfo;

		protected TMP_SpriteAsset m_defaultSpriteAsset;

		protected TMP_SpriteAsset m_currentSpriteAsset;

		protected int m_spriteCount = 0;

		protected int m_spriteIndex;

		protected InlineGraphicManager m_inlineGraphics;

		protected bool m_ignoreActiveState;

		private readonly float[] k_Power = new float[] { 0.5f, 0.05f, 0.005f, 0.0005f, 5E-05f, 5E-06f, 5E-07f, 5E-08f, 5E-09f, 5E-10f };

		protected static Vector2 k_InfinityVectorPositive = new Vector2(1000000f, 1000000f);

		protected static Vector2 k_InfinityVectorNegative = new Vector2(-1000000f, -1000000f);

		protected enum TextInputSources
		{
			Text,
			SetText,
			SetCharArray,
			String
		}
	}
}
