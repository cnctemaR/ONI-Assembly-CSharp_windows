using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace TMPro
{
	public abstract class TMP_Text : MaskableGraphic
	{
		[Obsolete("This does nothing. It was added to prevent PLib from breaking after upgrading to Unity 6.3")]
		public bool ignoreRectMaskCulling { get; set; }

		public virtual string text
		{
			get
			{
				if (this.m_IsTextBackingStringDirty)
				{
					return this.InternalTextBackingArrayToString();
				}
				return this.m_text;
			}
			set
			{
				if (!this.m_IsTextBackingStringDirty && this.m_text != null && value != null && this.m_text.Length == value.Length && this.m_text == value)
				{
					return;
				}
				this.m_IsTextBackingStringDirty = false;
				this.m_text = value;
				this.m_inputSource = TMP_Text.TextInputSources.TextString;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public ITextPreprocessor textPreprocessor
		{
			get
			{
				return this.m_TextPreprocessor;
			}
			set
			{
				this.m_TextPreprocessor = value;
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
				if (this.m_isRightToLeft == value)
				{
					return;
				}
				this.m_isRightToLeft = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_fontAsset == value)
				{
					return;
				}
				this.m_fontAsset = value;
				this.LoadFontAsset();
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_sharedMaterial == value)
				{
					return;
				}
				this.SetSharedMaterial(value);
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
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
				if (this.m_sharedMaterial != null && this.m_sharedMaterial.GetInstanceID() == value.GetInstanceID())
				{
					return;
				}
				this.m_sharedMaterial = value;
				this.m_padding = this.GetPaddingForMaterial();
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
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
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public override Color color
		{
			get
			{
				return this.m_fontColor;
			}
			set
			{
				if (this.m_fontColor == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_fontColor = value;
				this.SetVerticesDirty();
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
				if (this.m_fontColor.a == value)
				{
					return;
				}
				this.m_fontColor.a = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
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
				if (this.m_enableVertexGradient == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_enableVertexGradient = value;
				this.SetVerticesDirty();
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

		public TMP_ColorGradient colorGradientPreset
		{
			get
			{
				return this.m_fontColorGradientPreset;
			}
			set
			{
				this.m_havePropertiesChanged = true;
				this.m_fontColorGradientPreset = value;
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
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_tintAllSprites == value)
				{
					return;
				}
				this.m_tintAllSprites = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public TMP_StyleSheet styleSheet
		{
			get
			{
				return this.m_StyleSheet;
			}
			set
			{
				this.m_StyleSheet = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public TMP_Style textStyle
		{
			get
			{
				this.m_TextStyle = this.GetStyle(this.m_TextStyleHashCode);
				if (this.m_TextStyle == null)
				{
					this.m_TextStyle = TMP_Style.NormalStyle;
					this.m_TextStyleHashCode = this.m_TextStyle.hashCode;
				}
				return this.m_TextStyle;
			}
			set
			{
				this.m_TextStyle = value;
				this.m_TextStyleHashCode = this.m_TextStyle.hashCode;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_overrideHtmlColors == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_overrideHtmlColors = value;
				this.SetVerticesDirty();
			}
		}

		public Color32 faceColor
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_faceColor;
				}
				this.m_faceColor = this.m_sharedMaterial.GetColor(ShaderUtilities.ID_FaceColor);
				return this.m_faceColor;
			}
			set
			{
				if (this.m_faceColor.Compare(value))
				{
					return;
				}
				this.SetFaceColor(value);
				this.m_havePropertiesChanged = true;
				this.m_faceColor = value;
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public Color32 outlineColor
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_outlineColor;
				}
				this.m_outlineColor = this.m_sharedMaterial.GetColor(ShaderUtilities.ID_OutlineColor);
				return this.m_outlineColor;
			}
			set
			{
				if (this.m_outlineColor.Compare(value))
				{
					return;
				}
				this.SetOutlineColor(value);
				this.m_havePropertiesChanged = true;
				this.m_outlineColor = value;
				this.SetVerticesDirty();
			}
		}

		public float outlineWidth
		{
			get
			{
				if (this.m_sharedMaterial == null)
				{
					return this.m_outlineWidth;
				}
				this.m_outlineWidth = this.m_sharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth);
				return this.m_outlineWidth;
			}
			set
			{
				if (this.m_outlineWidth == value)
				{
					return;
				}
				this.SetOutlineThickness(value);
				this.m_havePropertiesChanged = true;
				this.m_outlineWidth = value;
				this.SetVerticesDirty();
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
				if (this.m_fontSize == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_fontSize = value;
				if (!this.m_enableAutoSizing)
				{
					this.m_fontSizeBase = this.m_fontSize;
				}
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public FontWeight fontWeight
		{
			get
			{
				return this.m_fontWeight;
			}
			set
			{
				if (this.m_fontWeight == value)
				{
					return;
				}
				this.m_fontWeight = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float pixelsPerUnit
		{
			get
			{
				Canvas canvas = base.canvas;
				if (!canvas)
				{
					return 1f;
				}
				if (!this.font)
				{
					return canvas.scaleFactor;
				}
				if (this.m_currentFontAsset == null || this.m_currentFontAsset.faceInfo.pointSize <= 0f || this.m_fontSize <= 0f)
				{
					return 1f;
				}
				return this.m_fontSize / this.m_currentFontAsset.faceInfo.pointSize;
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
				if (this.m_enableAutoSizing == value)
				{
					return;
				}
				this.m_enableAutoSizing = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_fontSizeMin == value)
				{
					return;
				}
				this.m_fontSizeMin = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_fontSizeMax == value)
				{
					return;
				}
				this.m_fontSizeMax = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_fontStyle == value)
				{
					return;
				}
				this.m_fontStyle = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool isUsingBold
		{
			get
			{
				return this.m_isUsingBold;
			}
		}

		public HorizontalAlignmentOptions horizontalAlignment
		{
			get
			{
				return this.m_HorizontalAlignment;
			}
			set
			{
				if (this.m_HorizontalAlignment == value)
				{
					return;
				}
				this.m_HorizontalAlignment = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public VerticalAlignmentOptions verticalAlignment
		{
			get
			{
				return this.m_VerticalAlignment;
			}
			set
			{
				if (this.m_VerticalAlignment == value)
				{
					return;
				}
				this.m_VerticalAlignment = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public TextAlignmentOptions alignment
		{
			get
			{
				return (TextAlignmentOptions)(this.m_HorizontalAlignment | (HorizontalAlignmentOptions)this.m_VerticalAlignment);
			}
			set
			{
				HorizontalAlignmentOptions horizontalAlignmentOptions = (HorizontalAlignmentOptions)(value & (TextAlignmentOptions)255);
				VerticalAlignmentOptions verticalAlignmentOptions = (VerticalAlignmentOptions)(value & (TextAlignmentOptions)65280);
				if (this.m_HorizontalAlignment == horizontalAlignmentOptions && this.m_VerticalAlignment == verticalAlignmentOptions)
				{
					return;
				}
				this.m_HorizontalAlignment = horizontalAlignmentOptions;
				this.m_VerticalAlignment = verticalAlignmentOptions;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
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
				if (this.m_characterSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_characterSpacing = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float characterHorizontalScale
		{
			get
			{
				return this.m_characterHorizontalScale;
			}
			set
			{
				if (this.m_characterHorizontalScale == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_characterHorizontalScale = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float wordSpacing
		{
			get
			{
				return this.m_wordSpacing;
			}
			set
			{
				if (this.m_wordSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_wordSpacing = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_lineSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_lineSpacing = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public float lineSpacingAdjustment
		{
			get
			{
				return this.m_lineSpacingMax;
			}
			set
			{
				if (this.m_lineSpacingMax == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_lineSpacingMax = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_paragraphSpacing == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_paragraphSpacing = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_charWidthMaxAdj == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_charWidthMaxAdj = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public TextWrappingModes textWrappingMode
		{
			get
			{
				return this.m_TextWrappingMode;
			}
			set
			{
				if (this.m_TextWrappingMode == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_TextWrappingMode = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		[Obsolete("The enabledWordWrapping property is now obsolete. Please use the textWrappingMode property instead.")]
		public bool enableWordWrapping
		{
			get
			{
				return this.m_TextWrappingMode == TextWrappingModes.Normal || this.textWrappingMode == TextWrappingModes.PreserveWhitespace;
			}
			set
			{
				TextWrappingModes textWrappingModes = (value ? TextWrappingModes.Normal : TextWrappingModes.NoWrap);
				if (this.m_TextWrappingMode == textWrappingModes)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_TextWrappingMode = textWrappingModes;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_wordWrappingRatios == value)
				{
					return;
				}
				this.m_wordWrappingRatios = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public TextOverflowModes overflowMode
		{
			get
			{
				return this.m_overflowMode;
			}
			set
			{
				if (this.m_overflowMode == value)
				{
					return;
				}
				this.m_overflowMode = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool isTextOverflowing
		{
			get
			{
				return this.m_firstOverflowCharacterIndex != -1;
			}
		}

		public int firstOverflowCharacterIndex
		{
			get
			{
				return this.m_firstOverflowCharacterIndex;
			}
		}

		public TMP_Text linkedTextComponent
		{
			get
			{
				return this.m_linkedTextComponent;
			}
			set
			{
				if (value == null)
				{
					this.ReleaseLinkedTextComponent(this.m_linkedTextComponent);
					this.m_linkedTextComponent = value;
				}
				else
				{
					if (this.IsSelfOrLinkedAncestor(value))
					{
						return;
					}
					this.ReleaseLinkedTextComponent(this.m_linkedTextComponent);
					this.m_linkedTextComponent = value;
					this.m_linkedTextComponent.parentLinkedComponent = this;
				}
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool isTextTruncated
		{
			get
			{
				return this.m_isTextTruncated;
			}
		}

		[Obsolete("The \"enableKerning\" property has been deprecated. Use the \"fontFeatures\" property to control what features are enabled on the text component.")]
		public bool enableKerning
		{
			get
			{
				return this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.kern);
			}
			set
			{
				if (this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.kern))
				{
					if (value)
					{
						return;
					}
					this.m_ActiveFontFeatures.Remove(OTL_FeatureTag.kern);
					this.m_enableKerning = false;
				}
				else
				{
					if (!value)
					{
						return;
					}
					this.m_ActiveFontFeatures.Add(OTL_FeatureTag.kern);
					this.m_enableKerning = true;
				}
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public List<OTL_FeatureTag> fontFeatures
		{
			get
			{
				return this.m_ActiveFontFeatures;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_ActiveFontFeatures = value;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_enableExtraPadding == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_enableExtraPadding = value;
				this.UpdateMeshPadding();
				this.SetVerticesDirty();
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
				if (this.m_isRichText == value)
				{
					return;
				}
				this.m_isRichText = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public bool emojiFallbackSupport
		{
			get
			{
				return this.m_EmojiFallbackSupport;
			}
			set
			{
				if (this.m_EmojiFallbackSupport == value)
				{
					return;
				}
				this.m_EmojiFallbackSupport = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_parseCtrlCharacters == value)
				{
					return;
				}
				this.m_parseCtrlCharacters = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
				this.SetLayoutDirty();
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
				if (this.m_isOverlay == value)
				{
					return;
				}
				this.m_isOverlay = value;
				this.SetShaderDepth();
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
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
				if (this.m_isOrthographic == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_isOrthographic = value;
				this.SetVerticesDirty();
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
				if (this.m_isCullingEnabled == value)
				{
					return;
				}
				this.m_isCullingEnabled = value;
				this.SetCulling();
				this.m_havePropertiesChanged = true;
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
				if (this.m_ignoreCulling == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_ignoreCulling = value;
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
				if (this.m_horizontalMapping == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_horizontalMapping = value;
				this.SetVerticesDirty();
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
				if (this.m_verticalMapping == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_verticalMapping = value;
				this.SetVerticesDirty();
			}
		}

		public float mappingUvLineOffset
		{
			get
			{
				return this.m_uvLineOffset;
			}
			set
			{
				if (this.m_uvLineOffset == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_uvLineOffset = value;
				this.SetVerticesDirty();
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
				if (this.m_renderMode == value)
				{
					return;
				}
				this.m_renderMode = value;
				this.m_havePropertiesChanged = true;
			}
		}

		public VertexSortingOrder geometrySortingOrder
		{
			get
			{
				return this.m_geometrySortingOrder;
			}
			set
			{
				this.m_geometrySortingOrder = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public bool isTextObjectScaleStatic
		{
			get
			{
				return this.m_IsTextObjectScaleStatic;
			}
			set
			{
				this.m_IsTextObjectScaleStatic = value;
				if (!base.isActiveAndEnabled)
				{
					return;
				}
				if (this.m_IsTextObjectScaleStatic)
				{
					TMP_UpdateManager.UnRegisterTextObjectForUpdate(this);
					return;
				}
				TMP_UpdateManager.RegisterTextObjectForUpdate(this);
			}
		}

		public bool vertexBufferAutoSizeReduction
		{
			get
			{
				return this.m_VertexBufferAutoSizeReduction;
			}
			set
			{
				this.m_VertexBufferAutoSizeReduction = value;
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public int firstVisibleCharacter
		{
			get
			{
				return this.m_firstVisibleCharacter;
			}
			set
			{
				if (this.m_firstVisibleCharacter == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_firstVisibleCharacter = value;
				this.SetVerticesDirty();
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
				if (this.m_maxVisibleCharacters == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_maxVisibleCharacters = value;
				this.SetVerticesDirty();
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
				if (this.m_maxVisibleWords == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_maxVisibleWords = value;
				this.SetVerticesDirty();
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
				if (this.m_maxVisibleLines == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_maxVisibleLines = value;
				this.SetVerticesDirty();
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
				if (this.m_useMaxVisibleDescender == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_useMaxVisibleDescender = value;
				this.SetVerticesDirty();
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
				if (this.m_pageToDisplay == value)
				{
					return;
				}
				this.m_havePropertiesChanged = true;
				this.m_pageToDisplay = value;
				this.SetVerticesDirty();
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
				if (this.m_margin == value)
				{
					return;
				}
				this.m_margin = value;
				this.ComputeMarginSize();
				this.m_havePropertiesChanged = true;
				this.SetVerticesDirty();
			}
		}

		public TMP_TextInfo textInfo
		{
			get
			{
				if (this.m_textInfo == null)
				{
					this.m_textInfo = new TMP_TextInfo(this);
				}
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
				if (this.m_havePropertiesChanged == value)
				{
					return;
				}
				this.m_havePropertiesChanged = value;
				this.SetAllDirty();
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

		public bool isVolumetricText
		{
			get
			{
				return this.m_isVolumetricText;
			}
			set
			{
				if (this.m_isVolumetricText == value)
				{
					return;
				}
				this.m_havePropertiesChanged = value;
				this.m_textInfo.ResetVertexLayout(value);
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		public Bounds bounds
		{
			get
			{
				if (this.m_mesh == null)
				{
					return default(Bounds);
				}
				return this.GetCompoundBounds();
			}
		}

		public Bounds textBounds
		{
			get
			{
				if (this.m_textInfo == null)
				{
					return default(Bounds);
				}
				return this.GetTextBounds();
			}
		}

		public static event Func<int, string, TMP_FontAsset> OnFontAssetRequest;

		public static event Func<int, string, TMP_SpriteAsset> OnSpriteAssetRequest;

		public static event TMP_Text.MissingCharacterEventCallback OnMissingCharacter;

		public virtual event Action<TMP_TextInfo> OnPreRenderText = delegate
		{
		};

		protected TMP_SpriteAnimator spriteAnimator
		{
			get
			{
				if (this.m_spriteAnimator == null)
				{
					this.m_spriteAnimator = base.GetComponent<TMP_SpriteAnimator>();
					if (this.m_spriteAnimator == null)
					{
						this.m_spriteAnimator = base.gameObject.AddComponent<TMP_SpriteAnimator>();
					}
				}
				return this.m_spriteAnimator;
			}
		}

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

		public float minWidth
		{
			get
			{
				return this.m_minWidth;
			}
		}

		public float minHeight
		{
			get
			{
				return this.m_minHeight;
			}
		}

		public float maxWidth
		{
			get
			{
				return this.m_maxWidth;
			}
		}

		public float maxHeight
		{
			get
			{
				return this.m_maxHeight;
			}
		}

		protected LayoutElement layoutElement
		{
			get
			{
				if (this.m_LayoutElement == null)
				{
					this.m_LayoutElement = base.GetComponent<LayoutElement>();
				}
				return this.m_LayoutElement;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				this.m_preferredWidth = this.GetPreferredWidth();
				return this.m_preferredWidth;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				this.m_preferredHeight = this.GetPreferredHeight();
				return this.m_preferredHeight;
			}
		}

		public virtual float renderedWidth
		{
			get
			{
				return this.GetRenderedWidth();
			}
		}

		public virtual float renderedHeight
		{
			get
			{
				return this.GetRenderedHeight();
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
			material.name += " (Instance)";
			return material;
		}

		protected void SetVertexColorGradient(TMP_ColorGradient gradient)
		{
			if (gradient == null)
			{
				return;
			}
			this.m_fontColorGradient.bottomLeft = gradient.bottomLeft;
			this.m_fontColorGradient.bottomRight = gradient.bottomRight;
			this.m_fontColorGradient.topLeft = gradient.topLeft;
			this.m_fontColorGradient.topRight = gradient.topRight;
			this.SetVerticesDirty();
		}

		protected void SetTextSortingOrder(VertexSortingOrder order)
		{
		}

		protected void SetTextSortingOrder(int[] order)
		{
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

		internal virtual void UpdateCulling()
		{
		}

		protected virtual float GetPaddingForMaterial()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			if (this.m_sharedMaterial == null)
			{
				return 0f;
			}
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
			this.m_isSDFShader = this.m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal);
			return this.m_padding;
		}

		protected virtual float GetPaddingForMaterial(Material mat)
		{
			if (mat == null)
			{
				return 0f;
			}
			this.m_padding = ShaderUtilities.GetPadding(mat, this.m_enableExtraPadding, this.m_isUsingBold);
			this.m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(this.m_sharedMaterial);
			this.m_isSDFShader = mat.HasProperty(ShaderUtilities.ID_WeightNormal);
			return this.m_padding;
		}

		protected virtual Vector3[] GetTextContainerLocalCorners()
		{
			return null;
		}

		public virtual void ForceMeshUpdate(bool ignoreActiveState = false, bool forceTextReparsing = false)
		{
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

		public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
			base.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
			this.InternalCrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
		}

		public override void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
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
			switch (this.m_inputSource)
			{
			case TMP_Text.TextInputSources.TextInputBox:
			case TMP_Text.TextInputSources.TextString:
				this.PopulateTextBackingArray((this.m_TextPreprocessor == null) ? this.m_text : this.m_TextPreprocessor.PreprocessText(this.m_text));
				this.PopulateTextProcessingArray();
				break;
			}
			this.SetArraySizes(this.m_TextProcessingArray);
		}

		private void PopulateTextBackingArray(string sourceText)
		{
			int num = ((sourceText == null) ? 0 : sourceText.Length);
			this.PopulateTextBackingArray(sourceText, 0, num);
		}

		private void PopulateTextBackingArray(string sourceText, int start, int length)
		{
			int num = 0;
			int i;
			if (sourceText == null)
			{
				i = 0;
				length = 0;
			}
			else
			{
				i = Mathf.Clamp(start, 0, sourceText.Length);
				length = Mathf.Clamp(length, 0, (start + length < sourceText.Length) ? length : (sourceText.Length - start));
			}
			if (length >= this.m_TextBackingArray.Capacity)
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
			int i;
			if (sourceText == null)
			{
				i = 0;
				length = 0;
			}
			else
			{
				i = Mathf.Clamp(start, 0, sourceText.Length);
				length = Mathf.Clamp(length, 0, (start + length < sourceText.Length) ? length : (sourceText.Length - start));
			}
			if (length >= this.m_TextBackingArray.Capacity)
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
			int i;
			if (sourceText == null)
			{
				i = 0;
				length = 0;
			}
			else
			{
				i = Mathf.Clamp(start, 0, sourceText.Length);
				length = Mathf.Clamp(length, 0, (start + length < sourceText.Length) ? length : (sourceText.Length - start));
			}
			if (length >= this.m_TextBackingArray.Capacity)
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

		private void PopulateTextProcessingArray()
		{
			TMP_TextProcessingStack<int>.SetDefault(this.m_TextStyleStacks, 0);
			int count = this.m_TextBackingArray.Count;
			int num = count;
			string styleOpeningDefinition = this.textStyle.styleOpeningDefinition;
			int num2 = num + ((styleOpeningDefinition != null) ? styleOpeningDefinition.Length : 0);
			if (this.m_TextProcessingArray.Length < num2)
			{
				this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray, num2);
			}
			this.m_TextStyleStackDepth = 0;
			int num3 = 0;
			if (this.textStyle.hashCode != -1183493901)
			{
				this.InsertOpeningStyleTag(this.m_TextStyle, ref this.m_TextProcessingArray, ref num3);
			}
			this.tag_NoParsing = false;
			int i = 0;
			while (i < count)
			{
				uint num4 = this.m_TextBackingArray[i];
				if (num4 == 0U)
				{
					break;
				}
				if (num4 != 92U || i >= count - 1)
				{
					goto IL_0329;
				}
				uint num5 = this.m_TextBackingArray[i + 1];
				if (num5 != 85U)
				{
					if (num5 != 92U)
					{
						switch (num5)
						{
						case 110U:
							if (!this.m_parseCtrlCharacters)
							{
								goto IL_0329;
							}
							this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
							{
								elementType = TextProcessingElementType.TextCharacterElement,
								stringIndex = i,
								length = 1,
								unicode = 10U
							};
							i++;
							num3++;
							break;
						case 111U:
						case 112U:
						case 113U:
						case 115U:
							goto IL_0329;
						case 114U:
							if (!this.m_parseCtrlCharacters)
							{
								goto IL_0329;
							}
							this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
							{
								elementType = TextProcessingElementType.TextCharacterElement,
								stringIndex = i,
								length = 1,
								unicode = 13U
							};
							i++;
							num3++;
							break;
						case 116U:
							if (!this.m_parseCtrlCharacters)
							{
								goto IL_0329;
							}
							this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
							{
								elementType = TextProcessingElementType.TextCharacterElement,
								stringIndex = i,
								length = 1,
								unicode = 9U
							};
							i++;
							num3++;
							break;
						case 117U:
							if (count <= i + 5 || !this.IsValidUTF16(this.m_TextBackingArray, i + 2))
							{
								goto IL_0329;
							}
							this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
							{
								elementType = TextProcessingElementType.TextCharacterElement,
								stringIndex = i,
								length = 6,
								unicode = this.GetUTF16(this.m_TextBackingArray, i + 2)
							};
							i += 5;
							num3++;
							break;
						case 118U:
							if (!this.m_parseCtrlCharacters)
							{
								goto IL_0329;
							}
							this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
							{
								elementType = TextProcessingElementType.TextCharacterElement,
								stringIndex = i,
								length = 1,
								unicode = 11U
							};
							i++;
							num3++;
							break;
						default:
							goto IL_0329;
						}
					}
					else
					{
						if (this.m_parseCtrlCharacters)
						{
							i++;
							goto IL_0329;
						}
						goto IL_0329;
					}
				}
				else
				{
					if (count <= i + 9 || !this.IsValidUTF32(this.m_TextBackingArray, i + 2))
					{
						goto IL_0329;
					}
					this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 10,
						unicode = this.GetUTF32(this.m_TextBackingArray, i + 2)
					};
					i += 9;
					num3++;
				}
				IL_08AF:
				i++;
				continue;
				IL_0329:
				if (num4 >= 55296U && num4 <= 56319U && count > i + 1 && this.m_TextBackingArray[i + 1] >= 56320U && this.m_TextBackingArray[i + 1] <= 57343U)
				{
					this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
					{
						elementType = TextProcessingElementType.TextCharacterElement,
						stringIndex = i,
						length = 2,
						unicode = TMP_TextParsingUtilities.ConvertToUTF32(num4, this.m_TextBackingArray[i + 1])
					};
					i++;
					num3++;
					goto IL_08AF;
				}
				if (num4 == 60U && this.m_isRichText)
				{
					MarkupTag markupTagHashCode = (MarkupTag)this.GetMarkupTagHashCode(this.m_TextBackingArray, i + 1);
					if (markupTagHashCode <= MarkupTag.CR)
					{
						if (markupTagHashCode <= MarkupTag.A)
						{
							if (markupTagHashCode != MarkupTag.NO_PARSE)
							{
								if (markupTagHashCode != MarkupTag.SLASH_NO_PARSE)
								{
									if (markupTagHashCode == MarkupTag.A)
									{
										if (this.m_TextBackingArray.Count > i + 4 && this.m_TextBackingArray[i + 3] == 104U && this.m_TextBackingArray[i + 4] == 114U)
										{
											this.InsertOpeningTextStyle(this.GetStyle(65), ref this.m_TextProcessingArray, ref num3);
										}
									}
								}
								else
								{
									this.tag_NoParsing = false;
								}
							}
							else
							{
								this.tag_NoParsing = true;
							}
						}
						else if (markupTagHashCode != MarkupTag.SLASH_A)
						{
							if (markupTagHashCode != MarkupTag.BR)
							{
								if (markupTagHashCode == MarkupTag.CR)
								{
									if (!this.tag_NoParsing)
									{
										if (num3 == this.m_TextProcessingArray.Length)
										{
											this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray);
										}
										this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
										{
											elementType = TextProcessingElementType.TextCharacterElement,
											stringIndex = i,
											length = 4,
											unicode = 13U
										};
										num3++;
										i += 3;
										goto IL_08AF;
									}
								}
							}
							else if (!this.tag_NoParsing)
							{
								if (num3 == this.m_TextProcessingArray.Length)
								{
									this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray);
								}
								this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
								{
									elementType = TextProcessingElementType.TextCharacterElement,
									stringIndex = i,
									length = 4,
									unicode = 10U
								};
								num3++;
								i += 3;
								goto IL_08AF;
							}
						}
						else
						{
							this.InsertClosingTextStyle(this.GetStyle(65), ref this.m_TextProcessingArray, ref num3);
						}
					}
					else if (markupTagHashCode <= MarkupTag.NBSP)
					{
						if (markupTagHashCode != MarkupTag.SHY)
						{
							if (markupTagHashCode != MarkupTag.ZWJ)
							{
								if (markupTagHashCode == MarkupTag.NBSP)
								{
									if (!this.tag_NoParsing)
									{
										if (num3 == this.m_TextProcessingArray.Length)
										{
											this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray);
										}
										this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
										{
											elementType = TextProcessingElementType.TextCharacterElement,
											stringIndex = i,
											length = 6,
											unicode = 160U
										};
										num3++;
										i += 5;
										goto IL_08AF;
									}
								}
							}
							else if (!this.tag_NoParsing)
							{
								if (num3 == this.m_TextProcessingArray.Length)
								{
									this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray);
								}
								this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
								{
									elementType = TextProcessingElementType.TextCharacterElement,
									stringIndex = i,
									length = 5,
									unicode = 8205U
								};
								num3++;
								i += 4;
								goto IL_08AF;
							}
						}
						else if (!this.tag_NoParsing)
						{
							if (num3 == this.m_TextProcessingArray.Length)
							{
								this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray);
							}
							this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
							{
								elementType = TextProcessingElementType.TextCharacterElement,
								stringIndex = i,
								length = 5,
								unicode = 173U
							};
							num3++;
							i += 4;
							goto IL_08AF;
						}
					}
					else if (markupTagHashCode != MarkupTag.ZWSP)
					{
						if (markupTagHashCode != MarkupTag.STYLE)
						{
							if (markupTagHashCode == MarkupTag.SLASH_STYLE)
							{
								if (!this.tag_NoParsing)
								{
									int j = num3;
									this.ReplaceClosingStyleTag(ref this.m_TextProcessingArray, ref num3);
									while (j < num3)
									{
										this.m_TextProcessingArray[j].stringIndex = i;
										this.m_TextProcessingArray[j].length = 8;
										j++;
									}
									i += 7;
									goto IL_08AF;
								}
							}
						}
						else if (!this.tag_NoParsing)
						{
							int k = num3;
							int num6;
							if (this.ReplaceOpeningStyleTag(ref this.m_TextBackingArray, i, out num6, ref this.m_TextProcessingArray, ref num3))
							{
								while (k < num3)
								{
									this.m_TextProcessingArray[k].stringIndex = i;
									this.m_TextProcessingArray[k].length = num6 - i + 1;
									k++;
								}
								i = num6;
								goto IL_08AF;
							}
						}
					}
					else if (!this.tag_NoParsing)
					{
						if (num3 == this.m_TextProcessingArray.Length)
						{
							this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray);
						}
						this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
						{
							elementType = TextProcessingElementType.TextCharacterElement,
							stringIndex = i,
							length = 6,
							unicode = 8203U
						};
						num3++;
						i += 5;
						goto IL_08AF;
					}
				}
				if (num3 == this.m_TextProcessingArray.Length)
				{
					this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray);
				}
				this.m_TextProcessingArray[num3] = new TMP_Text.TextProcessingElement
				{
					elementType = TextProcessingElementType.TextCharacterElement,
					stringIndex = i,
					length = 1,
					unicode = num4
				};
				num3++;
				goto IL_08AF;
			}
			this.m_TextStyleStackDepth = 0;
			if (this.textStyle.hashCode != -1183493901)
			{
				this.InsertClosingStyleTag(ref this.m_TextProcessingArray, ref num3);
			}
			if (num3 == this.m_TextProcessingArray.Length)
			{
				this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref this.m_TextProcessingArray);
			}
			this.m_TextProcessingArray[num3].unicode = 0U;
			this.m_InternalTextProcessingArraySize = num3;
		}

		private void SetTextInternal(string sourceText)
		{
			int num = ((sourceText == null) ? 0 : sourceText.Length);
			this.PopulateTextBackingArray(sourceText, 0, num);
			TMP_Text.TextInputSources inputSource = this.m_inputSource;
			this.m_inputSource = TMP_Text.TextInputSources.TextString;
			this.PopulateTextProcessingArray();
			this.m_inputSource = inputSource;
		}

		public virtual void SetText(string sourceText)
		{
			int num = ((sourceText == null) ? 0 : sourceText.Length);
			this.PopulateTextBackingArray(sourceText, 0, num);
			this.m_text = sourceText;
			this.m_inputSource = TMP_Text.TextInputSources.TextString;
			this.PopulateTextProcessingArray();
			this.m_havePropertiesChanged = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		[Obsolete("Use the SetText(string) function instead.")]
		public void SetText(string sourceText, bool syncTextInputBox = true)
		{
			int num = ((sourceText == null) ? 0 : sourceText.Length);
			this.PopulateTextBackingArray(sourceText, 0, num);
			this.m_text = sourceText;
			this.m_inputSource = TMP_Text.TextInputSources.TextString;
			this.PopulateTextProcessingArray();
			this.m_havePropertiesChanged = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		public void SetText(string sourceText, float arg0)
		{
			this.SetText(sourceText, arg0, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
		}

		public void SetText(string sourceText, float arg0, float arg1)
		{
			this.SetText(sourceText, arg0, arg1, 0f, 0f, 0f, 0f, 0f, 0f);
		}

		public void SetText(string sourceText, float arg0, float arg1, float arg2)
		{
			this.SetText(sourceText, arg0, arg1, arg2, 0f, 0f, 0f, 0f, 0f);
		}

		public void SetText(string sourceText, float arg0, float arg1, float arg2, float arg3)
		{
			this.SetText(sourceText, arg0, arg1, arg2, arg3, 0f, 0f, 0f, 0f);
		}

		public void SetText(string sourceText, float arg0, float arg1, float arg2, float arg3, float arg4)
		{
			this.SetText(sourceText, arg0, arg1, arg2, arg3, arg4, 0f, 0f, 0f);
		}

		public void SetText(string sourceText, float arg0, float arg1, float arg2, float arg3, float arg4, float arg5)
		{
			this.SetText(sourceText, arg0, arg1, arg2, arg3, arg4, arg5, 0f, 0f);
		}

		public void SetText(string sourceText, float arg0, float arg1, float arg2, float arg3, float arg4, float arg5, float arg6)
		{
			this.SetText(sourceText, arg0, arg1, arg2, arg3, arg4, arg5, arg6, 0f);
		}

		public void SetText(string sourceText, float arg0, float arg1, float arg2, float arg3, float arg4, float arg5, float arg6, float arg7)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int i = 0;
			int num5 = 0;
			while (i < sourceText.Length)
			{
				char c = sourceText[i];
				if (c == '{')
				{
					num4 = 1;
				}
				else if (c == '}')
				{
					switch (num)
					{
					case 0:
						this.AddFloatToInternalTextBackingArray(arg0, num2, num3, ref num5);
						break;
					case 1:
						this.AddFloatToInternalTextBackingArray(arg1, num2, num3, ref num5);
						break;
					case 2:
						this.AddFloatToInternalTextBackingArray(arg2, num2, num3, ref num5);
						break;
					case 3:
						this.AddFloatToInternalTextBackingArray(arg3, num2, num3, ref num5);
						break;
					case 4:
						this.AddFloatToInternalTextBackingArray(arg4, num2, num3, ref num5);
						break;
					case 5:
						this.AddFloatToInternalTextBackingArray(arg5, num2, num3, ref num5);
						break;
					case 6:
						this.AddFloatToInternalTextBackingArray(arg6, num2, num3, ref num5);
						break;
					case 7:
						this.AddFloatToInternalTextBackingArray(arg7, num2, num3, ref num5);
						break;
					}
					num = 0;
					num4 = 0;
					num2 = 0;
					num3 = 0;
				}
				else if (num4 == 1 && c >= '0' && c <= '8')
				{
					num = (int)(c - '0');
					num4 = 2;
				}
				else
				{
					if (num4 == 2)
					{
						if (c == ':')
						{
							goto IL_0150;
						}
						if (c == '.')
						{
							num4 = 3;
							goto IL_0150;
						}
						if (c == '#')
						{
							goto IL_0150;
						}
						if (c == '0')
						{
							num2++;
							goto IL_0150;
						}
						if (c == ',')
						{
							goto IL_0150;
						}
						if (c >= '1' && c <= '9')
						{
							num3 = (int)(c - '0');
							goto IL_0150;
						}
					}
					if (num4 == 3 && c == '0')
					{
						num3++;
					}
					else
					{
						this.m_TextBackingArray[num5] = (uint)c;
						num5++;
					}
				}
				IL_0150:
				i++;
			}
			this.m_TextBackingArray[num5] = 0U;
			this.m_TextBackingArray.Count = num5;
			this.m_IsTextBackingStringDirty = true;
			this.m_inputSource = TMP_Text.TextInputSources.SetText;
			this.PopulateTextProcessingArray();
			this.m_havePropertiesChanged = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		public void SetText(StringBuilder sourceText)
		{
			int num = ((sourceText == null) ? 0 : sourceText.Length);
			this.SetText(sourceText, 0, num);
		}

		private void SetText(StringBuilder sourceText, int start, int length)
		{
			this.PopulateTextBackingArray(sourceText, start, length);
			this.m_IsTextBackingStringDirty = true;
			this.m_inputSource = TMP_Text.TextInputSources.SetTextArray;
			this.PopulateTextProcessingArray();
			this.m_havePropertiesChanged = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		public void SetText(char[] sourceText)
		{
			int num = ((sourceText == null) ? 0 : sourceText.Length);
			this.SetCharArray(sourceText, 0, num);
		}

		public void SetText(char[] sourceText, int start, int length)
		{
			this.SetCharArray(sourceText, start, length);
		}

		public void SetCharArray(char[] sourceText)
		{
			int num = ((sourceText == null) ? 0 : sourceText.Length);
			this.SetCharArray(sourceText, 0, num);
		}

		public void SetCharArray(char[] sourceText, int start, int length)
		{
			this.PopulateTextBackingArray(sourceText, start, length);
			this.m_IsTextBackingStringDirty = true;
			this.m_inputSource = TMP_Text.TextInputSources.SetTextArray;
			this.PopulateTextProcessingArray();
			this.m_havePropertiesChanged = true;
			this.SetVerticesDirty();
			this.SetLayoutDirty();
		}

		private TMP_Style GetStyle(int hashCode)
		{
			TMP_Style tmp_Style = null;
			if (this.m_StyleSheet != null)
			{
				tmp_Style = this.m_StyleSheet.GetStyle(hashCode);
				if (tmp_Style != null)
				{
					return tmp_Style;
				}
			}
			if (TMP_Settings.defaultStyleSheet != null)
			{
				tmp_Style = TMP_Settings.defaultStyleSheet.GetStyle(hashCode);
			}
			return tmp_Style;
		}

		private void InsertOpeningTextStyle(TMP_Style style, ref TMP_Text.TextProcessingElement[] charBuffer, ref int writeIndex)
		{
			this.m_TextStyleStackDepth++;
			this.m_TextStyleStacks[this.m_TextStyleStackDepth].Push(style.hashCode);
			uint[] styleOpeningTagArray = style.styleOpeningTagArray;
			this.InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray);
			this.m_TextStyleStackDepth--;
		}

		private void InsertClosingTextStyle(TMP_Style style, ref TMP_Text.TextProcessingElement[] charBuffer, ref int writeIndex)
		{
			this.m_TextStyleStackDepth++;
			this.m_TextStyleStacks[this.m_TextStyleStackDepth].Push(style.hashCode);
			uint[] styleClosingTagArray = style.styleClosingTagArray;
			this.InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleClosingTagArray);
			this.m_TextStyleStackDepth--;
		}

		private void InsertTextStyleInTextProcessingArray(ref TMP_Text.TextProcessingElement[] charBuffer, ref int writeIndex, uint[] styleDefinition)
		{
			int num = styleDefinition.Length;
			if (writeIndex + num >= charBuffer.Length)
			{
				this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref charBuffer, writeIndex + num);
			}
			int i = 0;
			while (i < num)
			{
				uint num2 = styleDefinition[i];
				if (num2 == 92U && i + 1 < num)
				{
					uint num3 = styleDefinition[i + 1];
					if (num3 <= 92U)
					{
						if (num3 != 85U)
						{
							if (num3 == 92U)
							{
								i++;
							}
						}
						else if (i + 9 < num)
						{
							num2 = this.GetUTF32(styleDefinition, i + 2);
							i += 9;
						}
					}
					else if (num3 != 110U)
					{
						switch (num3)
						{
						case 117U:
							if (i + 5 < num)
							{
								num2 = this.GetUTF16(styleDefinition, i + 2);
								i += 5;
							}
							break;
						}
					}
					else
					{
						num2 = 10U;
						i++;
					}
				}
				if (num2 != 60U)
				{
					goto IL_02C7;
				}
				MarkupTag markupTagHashCode = (MarkupTag)this.GetMarkupTagHashCode(styleDefinition, i + 1);
				if (markupTagHashCode <= MarkupTag.SHY)
				{
					if (markupTagHashCode <= MarkupTag.SLASH_NO_PARSE)
					{
						if (markupTagHashCode == MarkupTag.NO_PARSE)
						{
							this.tag_NoParsing = true;
							goto IL_02C7;
						}
						if (markupTagHashCode != MarkupTag.SLASH_NO_PARSE)
						{
							goto IL_02C7;
						}
						this.tag_NoParsing = false;
						goto IL_02C7;
					}
					else if (markupTagHashCode != MarkupTag.BR)
					{
						if (markupTagHashCode != MarkupTag.CR)
						{
							if (markupTagHashCode != MarkupTag.SHY)
							{
								goto IL_02C7;
							}
							if (this.tag_NoParsing)
							{
								goto IL_02C7;
							}
							charBuffer[writeIndex].unicode = 173U;
							writeIndex++;
							i += 4;
						}
						else
						{
							if (this.tag_NoParsing)
							{
								goto IL_02C7;
							}
							charBuffer[writeIndex].unicode = 13U;
							writeIndex++;
							i += 3;
						}
					}
					else
					{
						if (this.tag_NoParsing)
						{
							goto IL_02C7;
						}
						charBuffer[writeIndex].unicode = 10U;
						writeIndex++;
						i += 3;
					}
				}
				else if (markupTagHashCode <= MarkupTag.NBSP)
				{
					if (markupTagHashCode != MarkupTag.ZWJ)
					{
						if (markupTagHashCode != MarkupTag.NBSP)
						{
							goto IL_02C7;
						}
						if (this.tag_NoParsing)
						{
							goto IL_02C7;
						}
						charBuffer[writeIndex].unicode = 160U;
						writeIndex++;
						i += 5;
					}
					else
					{
						if (this.tag_NoParsing)
						{
							goto IL_02C7;
						}
						charBuffer[writeIndex].unicode = 8205U;
						writeIndex++;
						i += 4;
					}
				}
				else if (markupTagHashCode != MarkupTag.ZWSP)
				{
					if (markupTagHashCode != MarkupTag.STYLE)
					{
						if (markupTagHashCode != MarkupTag.SLASH_STYLE)
						{
							goto IL_02C7;
						}
						if (this.tag_NoParsing)
						{
							goto IL_02C7;
						}
						this.ReplaceClosingStyleTag(ref charBuffer, ref writeIndex);
						i += 7;
					}
					else
					{
						int num4;
						if (this.tag_NoParsing || !this.ReplaceOpeningStyleTag(ref styleDefinition, i, out num4, ref charBuffer, ref writeIndex))
						{
							goto IL_02C7;
						}
						int num5 = num - num4;
						i = num4;
						if (writeIndex + num5 >= charBuffer.Length)
						{
							this.ResizeInternalArray<TMP_Text.TextProcessingElement>(ref charBuffer, writeIndex + num5);
						}
					}
				}
				else
				{
					if (this.tag_NoParsing)
					{
						goto IL_02C7;
					}
					charBuffer[writeIndex].unicode = 8203U;
					writeIndex++;
					i += 5;
				}
				IL_02DC:
				i++;
				continue;
				IL_02C7:
				charBuffer[writeIndex].unicode = num2;
				writeIndex++;
				goto IL_02DC;
			}
		}

		private bool ReplaceOpeningStyleTag(ref TMP_Text.TextBackingContainer sourceText, int srcIndex, out int srcOffset, ref TMP_Text.TextProcessingElement[] charBuffer, ref int writeIndex)
		{
			int styleHashCode = this.GetStyleHashCode(ref sourceText, srcIndex + 7, out srcOffset);
			TMP_Style style = this.GetStyle(styleHashCode);
			if (style == null || srcOffset == 0)
			{
				return false;
			}
			this.m_TextStyleStackDepth++;
			this.m_TextStyleStacks[this.m_TextStyleStackDepth].Push(style.hashCode);
			uint[] styleOpeningTagArray = style.styleOpeningTagArray;
			this.InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray);
			this.m_TextStyleStackDepth--;
			return true;
		}

		private bool ReplaceOpeningStyleTag(ref uint[] sourceText, int srcIndex, out int srcOffset, ref TMP_Text.TextProcessingElement[] charBuffer, ref int writeIndex)
		{
			int styleHashCode = this.GetStyleHashCode(ref sourceText, srcIndex + 7, out srcOffset);
			TMP_Style style = this.GetStyle(styleHashCode);
			if (style == null || srcOffset == 0)
			{
				return false;
			}
			this.m_TextStyleStackDepth++;
			this.m_TextStyleStacks[this.m_TextStyleStackDepth].Push(style.hashCode);
			uint[] styleOpeningTagArray = style.styleOpeningTagArray;
			this.InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray);
			this.m_TextStyleStackDepth--;
			return true;
		}

		private void ReplaceClosingStyleTag(ref TMP_Text.TextProcessingElement[] charBuffer, ref int writeIndex)
		{
			int num = this.m_TextStyleStacks[this.m_TextStyleStackDepth + 1].Pop();
			TMP_Style style = this.GetStyle(num);
			if (style == null)
			{
				return;
			}
			this.m_TextStyleStackDepth++;
			uint[] styleClosingTagArray = style.styleClosingTagArray;
			this.InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleClosingTagArray);
			this.m_TextStyleStackDepth--;
		}

		private void InsertOpeningStyleTag(TMP_Style style, ref TMP_Text.TextProcessingElement[] charBuffer, ref int writeIndex)
		{
			if (style == null)
			{
				return;
			}
			this.m_TextStyleStacks[0].Push(style.hashCode);
			uint[] styleOpeningTagArray = style.styleOpeningTagArray;
			this.InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleOpeningTagArray);
			this.m_TextStyleStackDepth = 0;
		}

		private void InsertClosingStyleTag(ref TMP_Text.TextProcessingElement[] charBuffer, ref int writeIndex)
		{
			int num = this.m_TextStyleStacks[0].Pop();
			uint[] styleClosingTagArray = this.GetStyle(num).styleClosingTagArray;
			this.InsertTextStyleInTextProcessingArray(ref charBuffer, ref writeIndex, styleClosingTagArray);
			this.m_TextStyleStackDepth = 0;
		}

		private int GetMarkupTagHashCode(uint[] styleDefinition, int readIndex)
		{
			int num = 0;
			int num2 = readIndex + 16;
			int num3 = styleDefinition.Length;
			while (readIndex < num2 && readIndex < num3)
			{
				uint num4 = styleDefinition[readIndex];
				if (num4 == 62U || num4 == 61U || num4 == 32U)
				{
					return num;
				}
				num = ((num << 5) + num) ^ (int)TMP_TextParsingUtilities.ToUpperASCIIFast(num4);
				readIndex++;
			}
			return num;
		}

		private int GetMarkupTagHashCode(TMP_Text.TextBackingContainer styleDefinition, int readIndex)
		{
			int num = 0;
			int num2 = readIndex + 16;
			int capacity = styleDefinition.Capacity;
			while (readIndex < num2 && readIndex < capacity)
			{
				uint num3 = styleDefinition[readIndex];
				if (num3 == 62U || num3 == 61U || num3 == 32U)
				{
					return num;
				}
				num = ((num << 5) + num) ^ (int)TMP_TextParsingUtilities.ToUpperASCIIFast(num3);
				readIndex++;
			}
			return num;
		}

		private int GetStyleHashCode(ref uint[] text, int index, out int closeIndex)
		{
			int num = 0;
			closeIndex = 0;
			for (int i = index; i < text.Length; i++)
			{
				if (text[i] != 34U)
				{
					if (text[i] == 62U)
					{
						closeIndex = i;
						break;
					}
					num = ((num << 5) + num) ^ (int)TMP_TextParsingUtilities.ToUpperASCIIFast((char)text[i]);
				}
			}
			return num;
		}

		private int GetStyleHashCode(ref TMP_Text.TextBackingContainer text, int index, out int closeIndex)
		{
			int num = 0;
			closeIndex = 0;
			for (int i = index; i < text.Capacity; i++)
			{
				if (text[i] != 34U)
				{
					if (text[i] == 62U)
					{
						closeIndex = i;
						break;
					}
					num = ((num << 5) + num) ^ (int)TMP_TextParsingUtilities.ToUpperASCIIFast((char)text[i]);
				}
			}
			return num;
		}

		private void ResizeInternalArray<T>(ref T[] array)
		{
			int num = Mathf.NextPowerOfTwo(array.Length + 1);
			Array.Resize<T>(ref array, num);
		}

		private void ResizeInternalArray<T>(ref T[] array, int size)
		{
			size = Mathf.NextPowerOfTwo(size + 1);
			Array.Resize<T>(ref array, size);
		}

		private void AddFloatToInternalTextBackingArray(float value, int padding, int precision, ref int writeIndex)
		{
			if (value < 0f)
			{
				this.m_TextBackingArray[writeIndex] = 45U;
				writeIndex++;
				value = -value;
			}
			decimal num = (decimal)value;
			if (padding == 0 && precision == 0)
			{
				precision = 9;
			}
			else
			{
				num += this.k_Power[Mathf.Min(9, precision)];
			}
			long num2 = (long)num;
			this.AddIntegerToInternalTextBackingArray((double)num2, padding, ref writeIndex);
			if (precision > 0)
			{
				num -= num2;
				if (num != 0m)
				{
					int num3 = writeIndex;
					writeIndex = num3 + 1;
					this.m_TextBackingArray[num3] = 46U;
					for (int i = 0; i < precision; i++)
					{
						num *= 10m;
						long num4 = (long)num;
						num3 = writeIndex;
						writeIndex = num3 + 1;
						this.m_TextBackingArray[num3] = (uint)((ushort)(num4 + 48L));
						num -= num4;
						if (num == 0m)
						{
							i = precision;
						}
					}
				}
			}
		}

		private void AddIntegerToInternalTextBackingArray(double number, int padding, ref int writeIndex)
		{
			int num = 0;
			int num2 = writeIndex;
			do
			{
				this.m_TextBackingArray[num2++] = (uint)((ushort)(number % 10.0 + 48.0));
				number /= 10.0;
				num++;
			}
			while (number > 0.999999999999999 || num < padding);
			int num3 = num2;
			while (writeIndex + 1 < num2)
			{
				num2--;
				uint num4 = this.m_TextBackingArray[writeIndex];
				this.m_TextBackingArray[writeIndex] = this.m_TextBackingArray[num2];
				this.m_TextBackingArray[num2] = num4;
				writeIndex++;
			}
			writeIndex = num3;
		}

		private string InternalTextBackingArrayToString()
		{
			char[] array = new char[this.m_TextBackingArray.Count];
			for (int i = 0; i < this.m_TextBackingArray.Capacity; i++)
			{
				char c = (char)this.m_TextBackingArray[i];
				if (c == '\0')
				{
					break;
				}
				array[i] = c;
			}
			this.m_IsTextBackingStringDirty = false;
			return new string(array);
		}

		internal virtual int SetArraySizes(TMP_Text.TextProcessingElement[] unicodeChars)
		{
			return 0;
		}

		public Vector2 GetPreferredValues()
		{
			this.m_isPreferredWidthDirty = true;
			float preferredWidth = this.GetPreferredWidth();
			this.m_isPreferredHeightDirty = true;
			float preferredHeight = this.GetPreferredHeight();
			this.m_isPreferredWidthDirty = true;
			this.m_isPreferredHeightDirty = true;
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(float width, float height)
		{
			this.m_isCalculatingPreferredValues = true;
			this.ParseInputText();
			Vector2 vector = new Vector2(width, height);
			float preferredWidth = this.GetPreferredWidth(vector);
			float preferredHeight = this.GetPreferredHeight(vector);
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(string text)
		{
			this.m_isCalculatingPreferredValues = true;
			this.SetTextInternal(text);
			this.SetArraySizes(this.m_TextProcessingArray);
			Vector2 vector = TMP_Text.k_LargePositiveVector2;
			float preferredWidth = this.GetPreferredWidth(vector);
			float preferredHeight = this.GetPreferredHeight(vector);
			return new Vector2(preferredWidth, preferredHeight);
		}

		public Vector2 GetPreferredValues(string text, float width, float height)
		{
			this.m_isCalculatingPreferredValues = true;
			this.SetTextInternal(text);
			this.SetArraySizes(this.m_TextProcessingArray);
			Vector2 vector = new Vector2(width, height);
			float preferredWidth = this.GetPreferredWidth(vector, this.m_TextWrappingMode);
			float preferredHeight = this.GetPreferredHeight(vector);
			return new Vector2(preferredWidth, preferredHeight);
		}

		protected float GetPreferredWidth()
		{
			if (TMP_Settings.instance == null)
			{
				return 0f;
			}
			if (!this.m_isPreferredWidthDirty)
			{
				return this.m_preferredWidth;
			}
			float num = (this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize);
			this.m_minFontSize = this.m_fontSizeMin;
			this.m_maxFontSize = this.m_fontSizeMax;
			this.m_charWidthAdjDelta = 0f;
			Vector2 vector = TMP_Text.k_LargePositiveVector2;
			this.m_isCalculatingPreferredValues = true;
			this.ParseInputText();
			this.m_AutoSizeIterationCount = 0;
			TextWrappingModes textWrappingModes = ((this.m_TextWrappingMode == TextWrappingModes.Normal || this.m_TextWrappingMode == TextWrappingModes.NoWrap) ? TextWrappingModes.NoWrap : TextWrappingModes.PreserveWhitespaceNoWrap);
			float x = this.CalculatePreferredValues(ref num, vector, false, textWrappingModes).x;
			this.m_isPreferredWidthDirty = false;
			return x;
		}

		private float GetPreferredWidth(Vector2 margin)
		{
			float num = (this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize);
			this.m_minFontSize = this.m_fontSizeMin;
			this.m_maxFontSize = this.m_fontSizeMax;
			this.m_charWidthAdjDelta = 0f;
			this.m_AutoSizeIterationCount = 0;
			TextWrappingModes textWrappingModes = ((this.m_TextWrappingMode == TextWrappingModes.Normal || this.m_TextWrappingMode == TextWrappingModes.NoWrap) ? TextWrappingModes.NoWrap : TextWrappingModes.PreserveWhitespaceNoWrap);
			return this.CalculatePreferredValues(ref num, margin, false, textWrappingModes).x;
		}

		private float GetPreferredWidth(Vector2 margin, TextWrappingModes wrapMode)
		{
			float num = (this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize);
			this.m_minFontSize = this.m_fontSizeMin;
			this.m_maxFontSize = this.m_fontSizeMax;
			this.m_charWidthAdjDelta = 0f;
			this.m_AutoSizeIterationCount = 0;
			return this.CalculatePreferredValues(ref num, margin, false, wrapMode).x;
		}

		protected float GetPreferredHeight()
		{
			if (TMP_Settings.instance == null)
			{
				return 0f;
			}
			if (!this.m_isPreferredHeightDirty)
			{
				return this.m_preferredHeight;
			}
			float num = (this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize);
			this.m_minFontSize = this.m_fontSizeMin;
			this.m_maxFontSize = this.m_fontSizeMax;
			this.m_charWidthAdjDelta = 0f;
			Vector2 vector = new Vector2((this.m_marginWidth != 0f) ? this.m_marginWidth : TMP_Text.k_LargePositiveFloat, TMP_Text.k_LargePositiveFloat);
			this.m_isCalculatingPreferredValues = true;
			this.ParseInputText();
			this.m_IsAutoSizePointSizeSet = false;
			this.m_AutoSizeIterationCount = 0;
			float num2 = 0f;
			while (!this.m_IsAutoSizePointSizeSet)
			{
				num2 = this.CalculatePreferredValues(ref num, vector, this.m_enableAutoSizing, this.m_TextWrappingMode).y;
				this.m_AutoSizeIterationCount++;
			}
			this.m_isPreferredHeightDirty = false;
			return num2;
		}

		private float GetPreferredHeight(Vector2 margin)
		{
			float num = (this.m_enableAutoSizing ? this.m_fontSizeMax : this.m_fontSize);
			this.m_minFontSize = this.m_fontSizeMin;
			this.m_maxFontSize = this.m_fontSizeMax;
			this.m_charWidthAdjDelta = 0f;
			this.m_IsAutoSizePointSizeSet = false;
			this.m_AutoSizeIterationCount = 0;
			float num2 = 0f;
			while (!this.m_IsAutoSizePointSizeSet)
			{
				num2 = this.CalculatePreferredValues(ref num, margin, this.m_enableAutoSizing, this.m_TextWrappingMode).y;
				this.m_AutoSizeIterationCount++;
			}
			return num2;
		}

		public Vector2 GetRenderedValues()
		{
			return this.GetTextBounds().size;
		}

		public Vector2 GetRenderedValues(bool onlyVisibleCharacters)
		{
			return this.GetTextBounds(onlyVisibleCharacters).size;
		}

		private float GetRenderedWidth()
		{
			return this.GetRenderedValues().x;
		}

		protected float GetRenderedWidth(bool onlyVisibleCharacters)
		{
			return this.GetRenderedValues(onlyVisibleCharacters).x;
		}

		private float GetRenderedHeight()
		{
			return this.GetRenderedValues().y;
		}

		protected float GetRenderedHeight(bool onlyVisibleCharacters)
		{
			return this.GetRenderedValues(onlyVisibleCharacters).y;
		}

		protected virtual Vector2 CalculatePreferredValues(ref float fontSize, Vector2 marginSize, bool isTextAutoSizingEnabled, TextWrappingModes textWrapMode)
		{
			if (this.m_fontAsset == null || this.m_fontAsset.characterLookupTable == null)
			{
				global::UnityEngine.Debug.LogWarning("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + base.GetInstanceID().ToString());
				this.m_IsAutoSizePointSizeSet = true;
				return Vector2.zero;
			}
			if (this.m_TextProcessingArray == null || this.m_TextProcessingArray.Length == 0 || this.m_TextProcessingArray[0].unicode == 0U)
			{
				this.m_IsAutoSizePointSizeSet = true;
				return Vector2.zero;
			}
			this.m_currentFontAsset = this.m_fontAsset;
			this.m_currentMaterial = this.m_sharedMaterial;
			this.m_currentMaterialIndex = 0;
			TMP_Text.m_materialReferenceStack.SetDefault(new MaterialReference(0, this.m_currentFontAsset, null, this.m_currentMaterial, this.m_padding));
			int totalCharacterCount = this.m_totalCharacterCount;
			if (this.m_internalCharacterInfo == null || totalCharacterCount > this.m_internalCharacterInfo.Length)
			{
				this.m_internalCharacterInfo = new TMP_CharacterInfo[(totalCharacterCount > 1024) ? (totalCharacterCount + 256) : Mathf.NextPowerOfTwo(totalCharacterCount)];
			}
			float num = (this.m_isOrthographic ? 1f : 0.1f);
			float num2 = fontSize / this.m_fontAsset.faceInfo.pointSize * this.m_fontAsset.faceInfo.scale * num;
			float num3 = num2;
			float num4 = fontSize * 0.01f * num;
			this.m_fontScaleMultiplier = 1f;
			this.m_currentFontSize = fontSize;
			this.m_sizeStack.SetDefault(this.m_currentFontSize);
			this.m_FontStyleInternal = this.m_fontStyle;
			this.m_lineJustification = this.m_HorizontalAlignment;
			this.m_lineJustificationStack.SetDefault(this.m_lineJustification);
			this.m_baselineOffset = 0f;
			this.m_baselineOffsetStack.Clear();
			this.m_FXScale = Vector3.one;
			this.m_lineOffset = 0f;
			this.m_lineHeight = -32767f;
			float num5 = this.m_currentFontAsset.faceInfo.lineHeight - (this.m_currentFontAsset.faceInfo.ascentLine - this.m_currentFontAsset.faceInfo.descentLine);
			this.m_cSpacing = 0f;
			this.m_monoSpacing = 0f;
			this.m_xAdvance = 0f;
			this.tag_LineIndent = 0f;
			this.tag_Indent = 0f;
			this.m_indentStack.SetDefault(0f);
			this.tag_NoParsing = false;
			this.m_characterCount = 0;
			this.m_firstCharacterOfLine = 0;
			this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
			this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
			this.m_lineNumber = 0;
			this.m_startOfLineAscender = 0f;
			this.m_IsDrivenLineSpacing = false;
			this.m_LastBaseGlyphIndex = int.MinValue;
			bool flag = this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.kern);
			bool flag2 = this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.mark);
			bool flag3 = this.m_ActiveFontFeatures.Contains(OTL_FeatureTag.mkmk);
			float x = marginSize.x;
			this.m_marginLeft = 0f;
			this.m_marginRight = 0f;
			this.m_width = -1f;
			float num6 = x + 0.0001f - this.m_marginLeft - this.m_marginRight;
			this.m_RenderedWidth = 0f;
			this.m_RenderedHeight = 0f;
			this.m_isCalculatingPreferredValues = true;
			this.m_maxCapHeight = 0f;
			this.m_maxTextAscender = 0f;
			float num7 = 0f;
			this.m_ElementDescender = 0f;
			bool flag4 = false;
			bool flag5 = true;
			this.m_isNonBreakingSpace = false;
			bool flag6 = false;
			TMP_Text.CharacterSubstitution characterSubstitution = new TMP_Text.CharacterSubstitution(-1, 0U);
			bool flag7 = false;
			WordWrapState wordWrapState = default(WordWrapState);
			WordWrapState wordWrapState2 = default(WordWrapState);
			WordWrapState wordWrapState3 = default(WordWrapState);
			this.m_AutoSizeIterationCount++;
			int num8 = 0;
			while (num8 < this.m_TextProcessingArray.Length && this.m_TextProcessingArray[num8].unicode != 0U)
			{
				uint num9 = this.m_TextProcessingArray[num8].unicode;
				if (num9 != 26U)
				{
					if (this.m_isRichText && num9 == 60U)
					{
						this.m_isTextLayoutPhase = true;
						this.m_textElementType = TMP_TextElementType.Character;
						int num10;
						if (this.ValidateHtmlTag(this.m_TextProcessingArray, num8 + 1, out num10))
						{
							num8 = num10;
							if (this.m_textElementType == TMP_TextElementType.Character)
							{
								goto IL_1DE2;
							}
						}
					}
					else
					{
						this.m_textElementType = this.m_textInfo.characterInfo[this.m_characterCount].elementType;
						this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
						this.m_currentFontAsset = this.m_textInfo.characterInfo[this.m_characterCount].fontAsset;
					}
					int currentMaterialIndex = this.m_currentMaterialIndex;
					bool isUsingAlternateTypeface = this.m_textInfo.characterInfo[this.m_characterCount].isUsingAlternateTypeface;
					this.m_isTextLayoutPhase = false;
					bool flag8 = false;
					if (characterSubstitution.index == this.m_characterCount)
					{
						num9 = characterSubstitution.unicode;
						this.m_textElementType = TMP_TextElementType.Character;
						flag8 = true;
						if (num9 != 3U)
						{
							if (num9 != 45U)
							{
								if (num9 == 8230U)
								{
									this.m_internalCharacterInfo[this.m_characterCount].textElement = this.m_Ellipsis.character;
									this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Character;
									this.m_internalCharacterInfo[this.m_characterCount].fontAsset = this.m_Ellipsis.fontAsset;
									this.m_internalCharacterInfo[this.m_characterCount].material = this.m_Ellipsis.material;
									this.m_internalCharacterInfo[this.m_characterCount].materialReferenceIndex = this.m_Ellipsis.materialIndex;
									this.m_isTextTruncated = true;
									characterSubstitution.index = this.m_characterCount + 1;
									characterSubstitution.unicode = 3U;
								}
							}
						}
						else
						{
							this.m_internalCharacterInfo[this.m_characterCount].textElement = this.m_currentFontAsset.characterLookupTable[3U];
							this.m_isTextTruncated = true;
						}
					}
					if (this.m_characterCount < this.m_firstVisibleCharacter && num9 != 3U)
					{
						this.m_internalCharacterInfo[this.m_characterCount].isVisible = false;
						this.m_internalCharacterInfo[this.m_characterCount].character = '\u200b';
						this.m_internalCharacterInfo[this.m_characterCount].lineNumber = 0;
						this.m_characterCount++;
					}
					else
					{
						float num11 = 1f;
						if (this.m_textElementType == TMP_TextElementType.Character)
						{
							if ((this.m_FontStyleInternal & FontStyles.UpperCase) == FontStyles.UpperCase)
							{
								if (char.IsLower((char)num9))
								{
									num9 = (uint)char.ToUpper((char)num9);
								}
							}
							else if ((this.m_FontStyleInternal & FontStyles.LowerCase) == FontStyles.LowerCase)
							{
								if (char.IsUpper((char)num9))
								{
									num9 = (uint)char.ToLower((char)num9);
								}
							}
							else if ((this.m_FontStyleInternal & FontStyles.SmallCaps) == FontStyles.SmallCaps && char.IsLower((char)num9))
							{
								num11 = 0.8f;
								num9 = (uint)char.ToUpper((char)num9);
							}
						}
						float num12 = 0f;
						float num13 = 0f;
						float num14 = 0f;
						FaceInfo faceInfo = this.m_currentFontAsset.faceInfo;
						if (this.m_textElementType == TMP_TextElementType.Sprite)
						{
							TMP_SpriteCharacter tmp_SpriteCharacter = (TMP_SpriteCharacter)this.m_textInfo.characterInfo[this.m_characterCount].textElement;
							if (tmp_SpriteCharacter == null)
							{
								goto IL_1DE2;
							}
							this.m_currentSpriteAsset = tmp_SpriteCharacter.textAsset as TMP_SpriteAsset;
							this.m_spriteIndex = (int)tmp_SpriteCharacter.glyphIndex;
							if (num9 == 60U)
							{
								num9 = (uint)(57344 + this.m_spriteIndex);
							}
							FaceInfo faceInfo2 = this.m_currentSpriteAsset.faceInfo;
							if (faceInfo2.pointSize > 0f)
							{
								float num15 = this.m_currentFontSize / faceInfo2.pointSize * faceInfo2.scale * num;
								num3 = tmp_SpriteCharacter.scale * tmp_SpriteCharacter.glyph.scale * num15;
								num13 = faceInfo2.ascentLine;
								num14 = faceInfo2.descentLine;
							}
							else
							{
								float num16 = this.m_currentFontSize / faceInfo.pointSize * faceInfo.scale * num;
								num3 = faceInfo.ascentLine / tmp_SpriteCharacter.glyph.metrics.height * tmp_SpriteCharacter.scale * tmp_SpriteCharacter.glyph.scale * num16;
								float num17 = ((num3 != 0f) ? (num16 / num3) : 0f);
								num13 = faceInfo.ascentLine * num17;
								num14 = faceInfo.descentLine * num17;
							}
							this.m_cached_TextElement = tmp_SpriteCharacter;
							this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Sprite;
							this.m_internalCharacterInfo[this.m_characterCount].scale = num3;
							this.m_currentMaterialIndex = currentMaterialIndex;
						}
						else if (this.m_textElementType == TMP_TextElementType.Character)
						{
							this.m_cached_TextElement = this.m_textInfo.characterInfo[this.m_characterCount].textElement;
							if (this.m_cached_TextElement == null)
							{
								goto IL_1DE2;
							}
							this.m_currentMaterialIndex = this.m_textInfo.characterInfo[this.m_characterCount].materialReferenceIndex;
							float num18;
							if (flag8 && this.m_TextProcessingArray[num8].unicode == 10U && this.m_characterCount != this.m_firstCharacterOfLine)
							{
								num18 = this.m_textInfo.characterInfo[this.m_characterCount - 1].pointSize * num11 / faceInfo.pointSize * faceInfo.scale * num;
							}
							else
							{
								num18 = this.m_currentFontSize * num11 / faceInfo.pointSize * faceInfo.scale * num;
							}
							if (flag8 && num9 == 8230U)
							{
								num13 = 0f;
								num14 = 0f;
							}
							else
							{
								num13 = faceInfo.ascentLine;
								num14 = faceInfo.descentLine;
							}
							num3 = num18 * this.m_fontScaleMultiplier * this.m_cached_TextElement.scale * this.m_cached_TextElement.m_Glyph.scale;
							this.m_internalCharacterInfo[this.m_characterCount].elementType = TMP_TextElementType.Character;
						}
						float num19 = num3;
						if (num9 == 173U || num9 == 3U)
						{
							num3 = 0f;
						}
						this.m_internalCharacterInfo[this.m_characterCount].character = (char)num9;
						Glyph alternativeGlyph = this.m_textInfo.characterInfo[this.m_characterCount].alternativeGlyph;
						GlyphMetrics glyphMetrics = ((alternativeGlyph == null) ? this.m_cached_TextElement.m_Glyph.metrics : alternativeGlyph.metrics);
						bool flag9 = num9 <= 65535U && char.IsWhiteSpace((char)num9);
						GlyphValueRecord glyphValueRecord = default(GlyphValueRecord);
						float num20 = this.m_characterSpacing;
						if (flag && this.m_textElementType == TMP_TextElementType.Character)
						{
							uint glyphIndex = this.m_cached_TextElement.m_GlyphIndex;
							if (this.m_characterCount < totalCharacterCount - 1 && this.m_textInfo.characterInfo[this.m_characterCount + 1].elementType == TMP_TextElementType.Character)
							{
								uint num21 = (this.m_textInfo.characterInfo[this.m_characterCount + 1].textElement.m_GlyphIndex << 16) | glyphIndex;
								GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
								if (this.m_currentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num21, out glyphPairAdjustmentRecord))
								{
									glyphValueRecord = glyphPairAdjustmentRecord.firstAdjustmentRecord.glyphValueRecord;
									num20 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num20);
								}
							}
							if (this.m_characterCount >= 1)
							{
								uint glyphIndex2 = this.m_textInfo.characterInfo[this.m_characterCount - 1].textElement.m_GlyphIndex;
								uint num22 = (glyphIndex << 16) | glyphIndex2;
								GlyphPairAdjustmentRecord glyphPairAdjustmentRecord;
								if (this.textInfo.characterInfo[this.m_characterCount - 1].elementType == TMP_TextElementType.Character && this.m_currentFontAsset.m_FontFeatureTable.m_GlyphPairAdjustmentRecordLookup.TryGetValue(num22, out glyphPairAdjustmentRecord))
								{
									glyphValueRecord += glyphPairAdjustmentRecord.secondAdjustmentRecord.glyphValueRecord;
									num20 = (((glyphPairAdjustmentRecord.featureLookupFlags & FontFeatureLookupFlags.IgnoreSpacingAdjustments) == FontFeatureLookupFlags.IgnoreSpacingAdjustments) ? 0f : num20);
								}
							}
							this.m_internalCharacterInfo[this.m_characterCount].adjustedHorizontalAdvance = glyphValueRecord.xAdvance;
						}
						bool flag10 = TMP_TextParsingUtilities.IsBaseGlyph(num9);
						if (flag10)
						{
							this.m_LastBaseGlyphIndex = this.m_characterCount;
						}
						if (this.m_characterCount > 0 && !flag10)
						{
							if (flag2 && this.m_LastBaseGlyphIndex != -2147483648 && this.m_LastBaseGlyphIndex == this.m_characterCount - 1)
							{
								uint index = this.m_textInfo.characterInfo[this.m_LastBaseGlyphIndex].textElement.glyph.index;
								uint num23 = (this.m_cached_TextElement.glyphIndex << 16) | index;
								MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord;
								if (this.m_currentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num23, out markToBaseAdjustmentRecord))
								{
									float num24 = (this.m_internalCharacterInfo[this.m_LastBaseGlyphIndex].origin - this.m_xAdvance) / num3;
									glyphValueRecord.xPlacement = num24 + markToBaseAdjustmentRecord.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.xPositionAdjustment;
									glyphValueRecord.yPlacement = markToBaseAdjustmentRecord.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord.markPositionAdjustment.yPositionAdjustment;
									num20 = 0f;
								}
							}
							else
							{
								bool flag11 = false;
								if (flag3)
								{
									int num25 = this.m_characterCount - 1;
									while (num25 >= 0 && num25 != this.m_LastBaseGlyphIndex)
									{
										uint index2 = this.m_textInfo.characterInfo[num25].textElement.glyph.index;
										uint num26 = (this.m_cached_TextElement.glyphIndex << 16) | index2;
										MarkToMarkAdjustmentRecord markToMarkAdjustmentRecord;
										if (this.m_currentFontAsset.fontFeatureTable.m_MarkToMarkAdjustmentRecordLookup.TryGetValue(num26, out markToMarkAdjustmentRecord))
										{
											float num27 = (this.m_textInfo.characterInfo[num25].origin - this.m_xAdvance) / num3;
											float num28 = num12 - this.m_lineOffset + this.m_baselineOffset;
											float num29 = (this.m_internalCharacterInfo[num25].baseLine - num28) / num3;
											glyphValueRecord.xPlacement = num27 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.xCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.xPositionAdjustment;
											glyphValueRecord.yPlacement = num29 + markToMarkAdjustmentRecord.baseMarkGlyphAnchorPoint.yCoordinate - markToMarkAdjustmentRecord.combiningMarkPositionAdjustment.yPositionAdjustment;
											num20 = 0f;
											flag11 = true;
											break;
										}
										num25--;
									}
								}
								if (flag2 && this.m_LastBaseGlyphIndex != -2147483648 && !flag11)
								{
									uint index3 = this.m_textInfo.characterInfo[this.m_LastBaseGlyphIndex].textElement.glyph.index;
									uint num30 = (this.m_cached_TextElement.glyphIndex << 16) | index3;
									MarkToBaseAdjustmentRecord markToBaseAdjustmentRecord2;
									if (this.m_currentFontAsset.fontFeatureTable.m_MarkToBaseAdjustmentRecordLookup.TryGetValue(num30, out markToBaseAdjustmentRecord2))
									{
										float num31 = (this.m_internalCharacterInfo[this.m_LastBaseGlyphIndex].origin - this.m_xAdvance) / num3;
										glyphValueRecord.xPlacement = num31 + markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.xCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.xPositionAdjustment;
										glyphValueRecord.yPlacement = markToBaseAdjustmentRecord2.baseGlyphAnchorPoint.yCoordinate - markToBaseAdjustmentRecord2.markPositionAdjustment.yPositionAdjustment;
										num20 = 0f;
									}
								}
							}
						}
						num13 += glyphValueRecord.yPlacement;
						num14 += glyphValueRecord.yPlacement;
						float num32 = 0f;
						if (this.m_monoSpacing != 0f)
						{
							num32 = (this.m_monoSpacing / 2f - (this.m_cached_TextElement.glyph.metrics.width / 2f + this.m_cached_TextElement.glyph.metrics.horizontalBearingX) * num3) * (1f - this.m_charWidthAdjDelta) * this.m_characterHorizontalScale;
							this.m_xAdvance += num32;
						}
						float num33 = 0f;
						if (this.m_textElementType == TMP_TextElementType.Character && !isUsingAlternateTypeface && (this.m_FontStyleInternal & FontStyles.Bold) == FontStyles.Bold)
						{
							num33 = this.m_currentFontAsset.boldSpacing;
						}
						this.m_internalCharacterInfo[this.m_characterCount].origin = this.m_xAdvance + glyphValueRecord.xPlacement * num3;
						this.m_internalCharacterInfo[this.m_characterCount].baseLine = num12 - this.m_lineOffset + this.m_baselineOffset + glyphValueRecord.yPlacement * num3;
						float num34 = ((this.m_textElementType == TMP_TextElementType.Character) ? (num13 * num3 / num11 + this.m_baselineOffset) : (num13 * num3 + this.m_baselineOffset));
						float num35 = ((this.m_textElementType == TMP_TextElementType.Character) ? (num14 * num3 / num11 + this.m_baselineOffset) : (num14 * num3 + this.m_baselineOffset));
						float num36 = num34;
						float num37 = num35;
						bool flag12 = this.m_characterCount == this.m_firstCharacterOfLine;
						if (flag12 || !flag9)
						{
							if (this.m_baselineOffset != 0f)
							{
								num36 = Mathf.Max((num34 - this.m_baselineOffset) / this.m_fontScaleMultiplier, num36);
								num37 = Mathf.Min((num35 - this.m_baselineOffset) / this.m_fontScaleMultiplier, num37);
							}
							this.m_maxLineAscender = Mathf.Max(num36, this.m_maxLineAscender);
							this.m_maxLineDescender = Mathf.Min(num37, this.m_maxLineDescender);
						}
						if (flag12 || !flag9)
						{
							this.m_internalCharacterInfo[this.m_characterCount].adjustedAscender = num36;
							this.m_internalCharacterInfo[this.m_characterCount].adjustedDescender = num37;
							this.m_ElementAscender = (this.m_internalCharacterInfo[this.m_characterCount].ascender = num34 - this.m_lineOffset);
							this.m_ElementDescender = (this.m_internalCharacterInfo[this.m_characterCount].descender = num35 - this.m_lineOffset);
						}
						else
						{
							this.m_internalCharacterInfo[this.m_characterCount].adjustedAscender = this.m_maxLineAscender;
							this.m_internalCharacterInfo[this.m_characterCount].adjustedDescender = this.m_maxLineDescender;
							this.m_ElementAscender = (this.m_internalCharacterInfo[this.m_characterCount].ascender = this.m_maxLineAscender - this.m_lineOffset);
							this.m_ElementDescender = (this.m_internalCharacterInfo[this.m_characterCount].descender = this.m_maxLineDescender - this.m_lineOffset);
						}
						if ((this.m_lineNumber == 0 || this.m_isNewPage) && (flag12 || !flag9))
						{
							this.m_maxTextAscender = this.m_maxLineAscender;
							this.m_maxCapHeight = Mathf.Max(this.m_maxCapHeight, this.m_currentFontAsset.m_FaceInfo.capLine * num3 / num11);
						}
						num7 = Mathf.Min(num7, this.m_ElementDescender);
						if (this.m_lineOffset == 0f && (!flag9 || this.m_characterCount == this.m_firstCharacterOfLine))
						{
							this.m_PageAscender = ((this.m_PageAscender > num34) ? this.m_PageAscender : num34);
						}
						bool flag13 = (this.m_lineJustification & HorizontalAlignmentOptions.Flush) == HorizontalAlignmentOptions.Flush || (this.m_lineJustification & HorizontalAlignmentOptions.Justified) == HorizontalAlignmentOptions.Justified;
						if (num9 == 9U || ((textWrapMode == TextWrappingModes.PreserveWhitespace || textWrapMode == TextWrappingModes.PreserveWhitespaceNoWrap) && (flag9 || num9 == 8203U)) || (!flag9 && num9 != 8203U && num9 != 173U && num9 != 3U) || (num9 == 173U && !flag7) || this.m_textElementType == TMP_TextElementType.Sprite)
						{
							num6 = ((this.m_width != -1f) ? Mathf.Min(x + 0.0001f - this.m_marginLeft - this.m_marginRight, this.m_width) : (x + 0.0001f - this.m_marginLeft - this.m_marginRight));
							float num38 = Mathf.Abs(this.m_xAdvance) + glyphMetrics.horizontalAdvance * (1f - this.m_charWidthAdjDelta) * this.m_characterHorizontalScale * ((num9 == 173U) ? num19 : num3);
							int characterCount = this.m_characterCount;
							if (flag10 && num38 > num6 * (flag13 ? 1.05f : 1f) && textWrapMode != TextWrappingModes.NoWrap && textWrapMode != TextWrappingModes.PreserveWhitespaceNoWrap && this.m_characterCount != this.m_firstCharacterOfLine)
							{
								num8 = this.RestoreWordWrappingState(ref wordWrapState);
								if (this.m_internalCharacterInfo[this.m_characterCount - 1].character == '\u00ad' && !flag7 && this.m_overflowMode == TextOverflowModes.Overflow)
								{
									characterSubstitution.index = this.m_characterCount - 1;
									characterSubstitution.unicode = 45U;
									num8--;
									this.m_characterCount--;
									goto IL_1DE2;
								}
								flag7 = false;
								if (this.m_internalCharacterInfo[this.m_characterCount].character == '\u00ad')
								{
									flag7 = true;
									goto IL_1DE2;
								}
								if (isTextAutoSizingEnabled && flag5)
								{
									if (this.m_charWidthAdjDelta < this.m_charWidthMaxAdj / 100f && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
									{
										float num39 = num38;
										if (this.m_charWidthAdjDelta > 0f)
										{
											num39 /= 1f - this.m_charWidthAdjDelta;
										}
										float num40 = num38 - (num6 - 0.0001f) * (flag13 ? 1.05f : 1f);
										this.m_charWidthAdjDelta += num40 / num39;
										this.m_charWidthAdjDelta = Mathf.Min(this.m_charWidthAdjDelta, this.m_charWidthMaxAdj / 100f);
										return Vector2.zero;
									}
									if (fontSize > this.m_fontSizeMin && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
									{
										this.m_maxFontSize = fontSize;
										float num41 = Mathf.Max((fontSize - this.m_minFontSize) / 2f, 0.05f);
										fontSize -= num41;
										fontSize = Mathf.Max((float)((int)(fontSize * 20f + 0.5f)) / 20f, this.m_fontSizeMin);
										return Vector2.zero;
									}
								}
								float num42 = this.m_maxLineAscender - this.m_startOfLineAscender;
								if (this.m_lineOffset > 0f && Math.Abs(num42) > 0.01f && !this.m_IsDrivenLineSpacing && !this.m_isNewPage)
								{
									this.m_ElementDescender -= num42;
									this.m_lineOffset += num42;
								}
								float maxLineAscender = this.m_maxLineAscender;
								float lineOffset = this.m_lineOffset;
								float num43 = this.m_maxLineDescender - this.m_lineOffset;
								this.m_ElementDescender = ((this.m_ElementDescender < num43) ? this.m_ElementDescender : num43);
								if (!flag4)
								{
									float elementDescender = this.m_ElementDescender;
								}
								if (this.m_useMaxVisibleDescender && (this.m_characterCount >= this.m_maxVisibleCharacters || this.m_lineNumber >= this.m_maxVisibleLines))
								{
									flag4 = true;
								}
								this.m_firstCharacterOfLine = this.m_characterCount;
								this.m_lineVisibleCharacterCount = 0;
								this.SaveWordWrappingState(ref wordWrapState2, num8, this.m_characterCount - 1);
								this.m_lineNumber++;
								float adjustedAscender = this.m_internalCharacterInfo[this.m_characterCount].adjustedAscender;
								if (this.m_lineHeight == -32767f)
								{
									this.m_lineOffset += 0f - this.m_maxLineDescender + adjustedAscender + (num5 + this.m_lineSpacingDelta) * num2 + this.m_lineSpacing * num4;
									this.m_IsDrivenLineSpacing = false;
								}
								else
								{
									this.m_lineOffset += this.m_lineHeight + this.m_lineSpacing * num4;
									this.m_IsDrivenLineSpacing = true;
								}
								this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
								this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
								this.m_startOfLineAscender = adjustedAscender;
								this.m_xAdvance = 0f + this.tag_Indent;
								flag5 = true;
								goto IL_1DE2;
							}
							else
							{
								this.m_RenderedWidth = Mathf.Max(this.m_RenderedWidth, num38 + this.m_marginLeft + this.m_marginRight);
								this.m_RenderedHeight = Mathf.Max(this.m_RenderedHeight, this.m_maxTextAscender - num7);
							}
						}
						if (this.m_lineOffset > 0f && !TMP_Math.Approximately(this.m_maxLineAscender, this.m_startOfLineAscender) && !this.m_IsDrivenLineSpacing && !this.m_isNewPage)
						{
							float num44 = this.m_maxLineAscender - this.m_startOfLineAscender;
							this.m_ElementDescender -= num44;
							this.m_lineOffset += num44;
							this.m_RenderedHeight += num44;
							this.m_startOfLineAscender += num44;
							wordWrapState.lineOffset = this.m_lineOffset;
							wordWrapState.startOfLineAscender = this.m_startOfLineAscender;
						}
						if (num9 == 9U)
						{
							float num45 = this.m_currentFontAsset.faceInfo.tabWidth * (float)this.m_currentFontAsset.tabSize * num3;
							float num46 = Mathf.Ceil(this.m_xAdvance / num45) * num45;
							this.m_xAdvance = ((num46 > this.m_xAdvance) ? num46 : (this.m_xAdvance + num45));
						}
						else if (this.m_monoSpacing != 0f)
						{
							this.m_xAdvance += (this.m_monoSpacing - num32 + (this.m_currentFontAsset.normalSpacingOffset + num20) * num4 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta) * this.m_characterHorizontalScale;
							if (flag9 || num9 == 8203U)
							{
								this.m_xAdvance += this.m_wordSpacing * num4;
							}
						}
						else
						{
							this.m_xAdvance += ((glyphMetrics.horizontalAdvance * this.m_FXScale.x + glyphValueRecord.xAdvance) * num3 + (this.m_currentFontAsset.normalSpacingOffset + num20 + num33) * num4 + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta) * this.m_characterHorizontalScale;
							if (flag9 || num9 == 8203U)
							{
								this.m_xAdvance += this.m_wordSpacing * num4;
							}
						}
						if (num9 == 13U)
						{
							this.m_xAdvance = 0f + this.tag_Indent;
						}
						if (num9 == 10U || num9 == 11U || num9 == 3U || num9 == 8232U || num9 == 8233U || this.m_characterCount == totalCharacterCount - 1)
						{
							float num47 = this.m_maxLineAscender - this.m_startOfLineAscender;
							if (this.m_lineOffset > 0f && Math.Abs(num47) > 0.01f && !this.m_IsDrivenLineSpacing && !this.m_isNewPage)
							{
								this.m_ElementDescender -= num47;
								this.m_lineOffset += num47;
							}
							this.m_isNewPage = false;
							float num48 = this.m_maxLineDescender - this.m_lineOffset;
							this.m_ElementDescender = ((this.m_ElementDescender < num48) ? this.m_ElementDescender : num48);
							if (num9 == 10U || num9 == 11U || (num9 == 45U && flag8) || num9 == 8232U || num9 == 8233U)
							{
								this.SaveWordWrappingState(ref wordWrapState2, num8, this.m_characterCount);
								this.SaveWordWrappingState(ref wordWrapState, num8, this.m_characterCount);
								this.m_lineNumber++;
								this.m_firstCharacterOfLine = this.m_characterCount + 1;
								flag5 = true;
								float adjustedAscender2 = this.m_internalCharacterInfo[this.m_characterCount].adjustedAscender;
								if (this.m_lineHeight == -32767f)
								{
									float num49 = 0f - this.m_maxLineDescender + adjustedAscender2 + (num5 + this.m_lineSpacingDelta) * num2 + (this.m_lineSpacing + ((num9 == 10U || num9 == 8233U) ? this.m_paragraphSpacing : 0f)) * num4;
									this.m_lineOffset += num49;
									this.m_IsDrivenLineSpacing = false;
								}
								else
								{
									this.m_lineOffset += this.m_lineHeight + (this.m_lineSpacing + ((num9 == 10U || num9 == 8233U) ? this.m_paragraphSpacing : 0f)) * num4;
									this.m_IsDrivenLineSpacing = true;
								}
								this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
								this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
								this.m_startOfLineAscender = adjustedAscender2;
								this.m_xAdvance = 0f + this.tag_LineIndent + this.tag_Indent;
								this.m_characterCount++;
								goto IL_1DE2;
							}
							if (num9 == 3U)
							{
								num8 = this.m_TextProcessingArray.Length;
							}
						}
						if ((textWrapMode != TextWrappingModes.NoWrap && textWrapMode != TextWrappingModes.PreserveWhitespaceNoWrap) || this.m_overflowMode == TextOverflowModes.Truncate || this.m_overflowMode == TextOverflowModes.Ellipsis)
						{
							bool flag14 = false;
							bool flag15 = false;
							uint num50 = (uint)((this.m_characterCount + 1 < totalCharacterCount) ? this.m_textInfo.characterInfo[this.m_characterCount + 1].character : '\0');
							if ((flag9 || num9 == 8203U || num9 == 45U || num9 == 173U) && (!this.m_isNonBreakingSpace || flag6) && num9 != 160U && num9 != 8199U && num9 != 8209U && num9 != 8239U && num9 != 8288U)
							{
								if (num9 != 45U || this.m_characterCount <= 0 || !char.IsWhiteSpace(this.m_textInfo.characterInfo[this.m_characterCount - 1].character) || this.m_textInfo.characterInfo[this.m_characterCount - 1].lineNumber != this.m_lineNumber)
								{
									flag5 = false;
									flag14 = true;
									wordWrapState3.previous_WordBreak = -1;
								}
							}
							else if (!this.m_isNonBreakingSpace && ((TMP_TextParsingUtilities.IsHangul(num9) && !TMP_Settings.useModernHangulLineBreakingRules) || TMP_TextParsingUtilities.IsCJK(num9)))
							{
								bool flag16 = TMP_Settings.linebreakingRules.leadingCharacters.Contains(num9);
								bool flag17 = this.m_characterCount < totalCharacterCount - 1 && TMP_Settings.linebreakingRules.followingCharacters.Contains(num50);
								if (!flag16)
								{
									if (!flag17)
									{
										flag5 = false;
										flag14 = true;
									}
									if (flag5)
									{
										if (flag9)
										{
											flag15 = true;
										}
										flag14 = true;
									}
								}
								else if (flag5 && flag12)
								{
									if (flag9)
									{
										flag15 = true;
									}
									flag14 = true;
								}
							}
							else if (!this.m_isNonBreakingSpace && TMP_TextParsingUtilities.IsCJK(num50) && !TMP_Settings.linebreakingRules.followingCharacters.Contains(num50))
							{
								flag14 = true;
							}
							else if (flag5)
							{
								if ((flag9 && num9 != 160U) || (num9 == 173U && !flag7))
								{
									flag15 = true;
								}
								flag14 = true;
							}
							if (flag14)
							{
								this.SaveWordWrappingState(ref wordWrapState, num8, this.m_characterCount);
							}
							if (flag15)
							{
								this.SaveWordWrappingState(ref wordWrapState3, num8, this.m_characterCount);
							}
						}
						this.m_characterCount++;
					}
				}
				IL_1DE2:
				num8++;
			}
			float num51 = this.m_maxFontSize - this.m_minFontSize;
			if (isTextAutoSizingEnabled && num51 > 0.051f && fontSize < this.m_fontSizeMax && this.m_AutoSizeIterationCount < this.m_AutoSizeMaxIterationCount)
			{
				if (this.m_charWidthAdjDelta < this.m_charWidthMaxAdj / 100f)
				{
					this.m_charWidthAdjDelta = 0f;
				}
				this.m_minFontSize = fontSize;
				float num52 = Mathf.Max((this.m_maxFontSize - fontSize) / 2f, 0.05f);
				fontSize += num52;
				fontSize = Mathf.Min((float)((int)(fontSize * 20f + 0.5f)) / 20f, this.m_fontSizeMax);
				return Vector2.zero;
			}
			this.m_IsAutoSizePointSizeSet = true;
			this.m_isCalculatingPreferredValues = false;
			this.m_RenderedWidth += ((this.m_margin.x > 0f) ? this.m_margin.x : 0f);
			this.m_RenderedWidth += ((this.m_margin.z > 0f) ? this.m_margin.z : 0f);
			this.m_RenderedHeight += ((this.m_margin.y > 0f) ? this.m_margin.y : 0f);
			this.m_RenderedHeight += ((this.m_margin.w > 0f) ? this.m_margin.w : 0f);
			this.m_RenderedWidth = (float)((int)(this.m_RenderedWidth * 100f + 1f)) / 100f;
			this.m_RenderedHeight = (float)((int)(this.m_RenderedHeight * 100f + 1f)) / 100f;
			return new Vector2(this.m_RenderedWidth, this.m_RenderedHeight);
		}

		protected virtual Bounds GetCompoundBounds()
		{
			return default(Bounds);
		}

		internal virtual Rect GetCanvasSpaceClippingRect()
		{
			return Rect.zero;
		}

		protected Bounds GetTextBounds()
		{
			if (this.m_textInfo == null || this.m_textInfo.characterCount > this.m_textInfo.characterInfo.Length)
			{
				return default(Bounds);
			}
			Extents extents = new Extents(TMP_Text.k_LargePositiveVector2, TMP_Text.k_LargeNegativeVector2);
			int num = 0;
			while (num < this.m_textInfo.characterCount && num < this.m_textInfo.characterInfo.Length)
			{
				if (this.m_textInfo.characterInfo[num].isVisible)
				{
					extents.min.x = Mathf.Min(extents.min.x, this.m_textInfo.characterInfo[num].origin);
					extents.min.y = Mathf.Min(extents.min.y, this.m_textInfo.characterInfo[num].descender);
					extents.max.x = Mathf.Max(extents.max.x, this.m_textInfo.characterInfo[num].xAdvance);
					extents.max.y = Mathf.Max(extents.max.y, this.m_textInfo.characterInfo[num].ascender);
				}
				num++;
			}
			Vector2 vector;
			vector.x = extents.max.x - extents.min.x;
			vector.y = extents.max.y - extents.min.y;
			return new Bounds((extents.min + extents.max) / 2f, vector);
		}

		protected Bounds GetTextBounds(bool onlyVisibleCharacters)
		{
			if (this.m_textInfo == null)
			{
				return default(Bounds);
			}
			Extents extents = new Extents(TMP_Text.k_LargePositiveVector2, TMP_Text.k_LargeNegativeVector2);
			int num = 0;
			while (num < this.m_textInfo.characterCount && ((num <= this.maxVisibleCharacters && this.m_textInfo.characterInfo[num].lineNumber <= this.m_maxVisibleLines) || !onlyVisibleCharacters))
			{
				if (!onlyVisibleCharacters || this.m_textInfo.characterInfo[num].isVisible)
				{
					extents.min.x = Mathf.Min(extents.min.x, this.m_textInfo.characterInfo[num].origin);
					extents.min.y = Mathf.Min(extents.min.y, this.m_textInfo.characterInfo[num].descender);
					extents.max.x = Mathf.Max(extents.max.x, this.m_textInfo.characterInfo[num].xAdvance);
					extents.max.y = Mathf.Max(extents.max.y, this.m_textInfo.characterInfo[num].ascender);
				}
				num++;
			}
			Vector2 vector;
			vector.x = extents.max.x - extents.min.x;
			vector.y = extents.max.y - extents.min.y;
			return new Bounds((extents.min + extents.max) / 2f, vector);
		}

		protected void AdjustLineOffset(int startIndex, int endIndex, float offset)
		{
			Vector3 vector = new Vector3(0f, offset, 0f);
			for (int i = startIndex; i <= endIndex; i++)
			{
				TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
				int num = i;
				characterInfo[num].bottomLeft = characterInfo[num].bottomLeft - vector;
				TMP_CharacterInfo[] characterInfo2 = this.m_textInfo.characterInfo;
				int num2 = i;
				characterInfo2[num2].topLeft = characterInfo2[num2].topLeft - vector;
				TMP_CharacterInfo[] characterInfo3 = this.m_textInfo.characterInfo;
				int num3 = i;
				characterInfo3[num3].topRight = characterInfo3[num3].topRight - vector;
				TMP_CharacterInfo[] characterInfo4 = this.m_textInfo.characterInfo;
				int num4 = i;
				characterInfo4[num4].bottomRight = characterInfo4[num4].bottomRight - vector;
				TMP_CharacterInfo[] characterInfo5 = this.m_textInfo.characterInfo;
				int num5 = i;
				characterInfo5[num5].ascender = characterInfo5[num5].ascender - vector.y;
				TMP_CharacterInfo[] characterInfo6 = this.m_textInfo.characterInfo;
				int num6 = i;
				characterInfo6[num6].baseLine = characterInfo6[num6].baseLine - vector.y;
				TMP_CharacterInfo[] characterInfo7 = this.m_textInfo.characterInfo;
				int num7 = i;
				characterInfo7[num7].descender = characterInfo7[num7].descender - vector.y;
				if (this.m_textInfo.characterInfo[i].isVisible)
				{
					TMP_CharacterInfo[] characterInfo8 = this.m_textInfo.characterInfo;
					int num8 = i;
					characterInfo8[num8].vertex_BL.position = characterInfo8[num8].vertex_BL.position - vector;
					TMP_CharacterInfo[] characterInfo9 = this.m_textInfo.characterInfo;
					int num9 = i;
					characterInfo9[num9].vertex_TL.position = characterInfo9[num9].vertex_TL.position - vector;
					TMP_CharacterInfo[] characterInfo10 = this.m_textInfo.characterInfo;
					int num10 = i;
					characterInfo10[num10].vertex_TR.position = characterInfo10[num10].vertex_TR.position - vector;
					TMP_CharacterInfo[] characterInfo11 = this.m_textInfo.characterInfo;
					int num11 = i;
					characterInfo11[num11].vertex_BR.position = characterInfo11[num11].vertex_BR.position - vector;
				}
			}
		}

		protected void ResizeLineExtents(int size)
		{
			size = ((size > 1024) ? (size + 256) : Mathf.NextPowerOfTwo(size + 1));
			TMP_LineInfo[] array = new TMP_LineInfo[size];
			for (int i = 0; i < size; i++)
			{
				if (i < this.m_textInfo.lineInfo.Length)
				{
					array[i] = this.m_textInfo.lineInfo[i];
				}
				else
				{
					array[i].lineExtents.min = TMP_Text.k_LargePositiveVector2;
					array[i].lineExtents.max = TMP_Text.k_LargeNegativeVector2;
					array[i].ascender = TMP_Text.k_LargeNegativeFloat;
					array[i].descender = TMP_Text.k_LargePositiveFloat;
				}
			}
			this.m_textInfo.lineInfo = array;
		}

		public virtual TMP_TextInfo GetTextInfo(string text)
		{
			return null;
		}

		public virtual void ComputeMarginSize()
		{
		}

		internal void InsertNewLine(int i, float baseScale, float currentElementScale, float currentEmScale, float boldSpacingAdjustment, float characterSpacingAdjustment, float width, float lineGap, ref bool isMaxVisibleDescenderSet, ref float maxVisibleDescender)
		{
			float num = this.m_maxLineAscender - this.m_startOfLineAscender;
			if (this.m_lineOffset > 0f && Math.Abs(num) > 0.01f && !this.m_IsDrivenLineSpacing && !this.m_isNewPage)
			{
				this.AdjustLineOffset(this.m_firstCharacterOfLine, this.m_characterCount, num);
				this.m_ElementDescender -= num;
				this.m_lineOffset += num;
			}
			float num2 = this.m_maxLineAscender - this.m_lineOffset;
			float num3 = this.m_maxLineDescender - this.m_lineOffset;
			this.m_ElementDescender = ((this.m_ElementDescender < num3) ? this.m_ElementDescender : num3);
			if (!isMaxVisibleDescenderSet)
			{
				maxVisibleDescender = this.m_ElementDescender;
			}
			if (this.m_useMaxVisibleDescender && (this.m_characterCount >= this.m_maxVisibleCharacters || this.m_lineNumber >= this.m_maxVisibleLines))
			{
				isMaxVisibleDescenderSet = true;
			}
			this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex = this.m_firstCharacterOfLine;
			this.m_textInfo.lineInfo[this.m_lineNumber].firstVisibleCharacterIndex = (this.m_firstVisibleCharacterOfLine = ((this.m_firstCharacterOfLine > this.m_firstVisibleCharacterOfLine) ? this.m_firstCharacterOfLine : this.m_firstVisibleCharacterOfLine));
			this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex = (this.m_lastCharacterOfLine = ((this.m_characterCount - 1 > 0) ? (this.m_characterCount - 1) : 0));
			this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex = (this.m_lastVisibleCharacterOfLine = ((this.m_lastVisibleCharacterOfLine < this.m_firstVisibleCharacterOfLine) ? this.m_firstVisibleCharacterOfLine : this.m_lastVisibleCharacterOfLine));
			this.m_textInfo.lineInfo[this.m_lineNumber].characterCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastCharacterIndex - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex + 1;
			this.m_textInfo.lineInfo[this.m_lineNumber].visibleCharacterCount = this.m_lineVisibleCharacterCount;
			this.m_textInfo.lineInfo[this.m_lineNumber].visibleSpaceCount = this.m_textInfo.lineInfo[this.m_lineNumber].lastVisibleCharacterIndex + 1 - this.m_textInfo.lineInfo[this.m_lineNumber].firstCharacterIndex - this.m_lineVisibleCharacterCount;
			this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.min = new Vector2(this.m_textInfo.characterInfo[this.m_firstVisibleCharacterOfLine].bottomLeft.x, num3);
			this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max = new Vector2(this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].topRight.x, num2);
			this.m_textInfo.lineInfo[this.m_lineNumber].length = this.m_textInfo.lineInfo[this.m_lineNumber].lineExtents.max.x;
			this.m_textInfo.lineInfo[this.m_lineNumber].width = width;
			float num4 = (this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].adjustedHorizontalAdvance * currentElementScale + (this.m_currentFontAsset.normalSpacingOffset + characterSpacingAdjustment + boldSpacingAdjustment) * currentEmScale + this.m_cSpacing) * (1f - this.m_charWidthAdjDelta) * this.m_characterHorizontalScale;
			float num5 = (this.m_textInfo.lineInfo[this.m_lineNumber].maxAdvance = this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance + (this.m_isRightToLeft ? num4 : (-num4)));
			this.m_textInfo.characterInfo[this.m_lastVisibleCharacterOfLine].xAdvance = num5;
			this.m_textInfo.lineInfo[this.m_lineNumber].baseline = 0f - this.m_lineOffset;
			this.m_textInfo.lineInfo[this.m_lineNumber].ascender = num2;
			this.m_textInfo.lineInfo[this.m_lineNumber].descender = num3;
			this.m_textInfo.lineInfo[this.m_lineNumber].lineHeight = num2 - num3 + lineGap * baseScale;
			this.m_firstCharacterOfLine = this.m_characterCount;
			this.m_lineVisibleCharacterCount = 0;
			this.m_lineVisibleSpaceCount = 0;
			this.SaveWordWrappingState(ref TMP_Text.m_SavedLineState, i, this.m_characterCount - 1);
			this.m_lineNumber++;
			if (this.m_lineNumber >= this.m_textInfo.lineInfo.Length)
			{
				this.ResizeLineExtents(this.m_lineNumber);
			}
			if (this.m_lineHeight == -32767f)
			{
				float adjustedAscender = this.m_textInfo.characterInfo[this.m_characterCount].adjustedAscender;
				float num6 = 0f - this.m_maxLineDescender + adjustedAscender + (lineGap + this.m_lineSpacingDelta) * baseScale + this.m_lineSpacing * currentEmScale;
				this.m_lineOffset += num6;
				this.m_startOfLineAscender = adjustedAscender;
			}
			else
			{
				this.m_lineOffset += this.m_lineHeight + this.m_lineSpacing * currentEmScale;
			}
			this.m_maxLineAscender = TMP_Text.k_LargeNegativeFloat;
			this.m_maxLineDescender = TMP_Text.k_LargePositiveFloat;
			this.m_xAdvance = 0f + this.tag_Indent;
		}

		internal void SaveWordWrappingState(ref WordWrapState state, int index, int count)
		{
			state.currentFontAsset = this.m_currentFontAsset;
			state.currentSpriteAsset = this.m_currentSpriteAsset;
			state.currentMaterial = this.m_currentMaterial;
			state.currentMaterialIndex = this.m_currentMaterialIndex;
			state.previous_WordBreak = index;
			state.total_CharacterCount = count;
			state.visible_CharacterCount = this.m_lineVisibleCharacterCount;
			state.visibleSpaceCount = this.m_lineVisibleSpaceCount;
			state.visible_LinkCount = this.m_textInfo.linkCount;
			state.firstCharacterIndex = this.m_firstCharacterOfLine;
			state.firstVisibleCharacterIndex = this.m_firstVisibleCharacterOfLine;
			state.lastVisibleCharIndex = this.m_lastVisibleCharacterOfLine;
			state.fontStyle = this.m_FontStyleInternal;
			state.italicAngle = this.m_ItalicAngle;
			state.fontScaleMultiplier = this.m_fontScaleMultiplier;
			state.currentFontSize = this.m_currentFontSize;
			state.xAdvance = this.m_xAdvance;
			state.maxCapHeight = this.m_maxCapHeight;
			state.maxAscender = this.m_maxTextAscender;
			state.maxDescender = this.m_ElementDescender;
			state.startOfLineAscender = this.m_startOfLineAscender;
			state.maxLineAscender = this.m_maxLineAscender;
			state.maxLineDescender = this.m_maxLineDescender;
			state.pageAscender = this.m_PageAscender;
			state.preferredWidth = this.m_preferredWidth;
			state.preferredHeight = this.m_preferredHeight;
			state.renderedWidth = this.m_RenderedWidth;
			state.renderedHeight = this.m_RenderedHeight;
			state.meshExtents = this.m_meshExtents;
			state.lineNumber = this.m_lineNumber;
			state.lineOffset = this.m_lineOffset;
			state.baselineOffset = this.m_baselineOffset;
			state.isDrivenLineSpacing = this.m_IsDrivenLineSpacing;
			state.lastBaseGlyphIndex = this.m_LastBaseGlyphIndex;
			state.cSpace = this.m_cSpacing;
			state.mSpace = this.m_monoSpacing;
			state.horizontalAlignment = this.m_lineJustification;
			state.marginLeft = this.m_marginLeft;
			state.marginRight = this.m_marginRight;
			state.vertexColor = this.m_htmlColor;
			state.underlineColor = this.m_underlineColor;
			state.strikethroughColor = this.m_strikethroughColor;
			state.highlightState = this.m_HighlightState;
			state.isNonBreakingSpace = this.m_isNonBreakingSpace;
			state.tagNoParsing = this.tag_NoParsing;
			state.fxRotation = this.m_FXRotation;
			state.fxScale = this.m_FXScale;
			state.basicStyleStack = this.m_fontStyleStack;
			state.italicAngleStack = this.m_ItalicAngleStack;
			state.colorStack = this.m_colorStack;
			state.underlineColorStack = this.m_underlineColorStack;
			state.strikethroughColorStack = this.m_strikethroughColorStack;
			state.highlightStateStack = this.m_HighlightStateStack;
			state.colorGradientStack = this.m_colorGradientStack;
			state.sizeStack = this.m_sizeStack;
			state.indentStack = this.m_indentStack;
			state.fontWeightStack = this.m_FontWeightStack;
			state.baselineStack = this.m_baselineOffsetStack;
			state.actionStack = this.m_actionStack;
			state.materialReferenceStack = TMP_Text.m_materialReferenceStack;
			state.lineJustificationStack = this.m_lineJustificationStack;
			state.spriteAnimationID = this.m_spriteAnimationID;
			if (this.m_lineNumber < this.m_textInfo.lineInfo.Length)
			{
				state.lineInfo = this.m_textInfo.lineInfo[this.m_lineNumber];
			}
		}

		internal int RestoreWordWrappingState(ref WordWrapState state)
		{
			int previous_WordBreak = state.previous_WordBreak;
			this.m_currentFontAsset = state.currentFontAsset;
			this.m_currentSpriteAsset = state.currentSpriteAsset;
			this.m_currentMaterial = state.currentMaterial;
			this.m_currentMaterialIndex = state.currentMaterialIndex;
			this.m_characterCount = state.total_CharacterCount + 1;
			this.m_lineVisibleCharacterCount = state.visible_CharacterCount;
			this.m_lineVisibleSpaceCount = state.visibleSpaceCount;
			this.m_textInfo.linkCount = state.visible_LinkCount;
			this.m_firstCharacterOfLine = state.firstCharacterIndex;
			this.m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
			this.m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
			this.m_FontStyleInternal = state.fontStyle;
			this.m_ItalicAngle = state.italicAngle;
			this.m_fontScaleMultiplier = state.fontScaleMultiplier;
			this.m_currentFontSize = state.currentFontSize;
			this.m_xAdvance = state.xAdvance;
			this.m_maxCapHeight = state.maxCapHeight;
			this.m_maxTextAscender = state.maxAscender;
			this.m_ElementDescender = state.maxDescender;
			this.m_startOfLineAscender = state.startOfLineAscender;
			this.m_maxLineAscender = state.maxLineAscender;
			this.m_maxLineDescender = state.maxLineDescender;
			this.m_PageAscender = state.pageAscender;
			this.m_preferredWidth = state.preferredWidth;
			this.m_preferredHeight = state.preferredHeight;
			this.m_RenderedWidth = state.renderedWidth;
			this.m_RenderedHeight = state.renderedHeight;
			this.m_meshExtents = state.meshExtents;
			this.m_lineNumber = state.lineNumber;
			this.m_lineOffset = state.lineOffset;
			this.m_baselineOffset = state.baselineOffset;
			this.m_IsDrivenLineSpacing = state.isDrivenLineSpacing;
			this.m_LastBaseGlyphIndex = state.lastBaseGlyphIndex;
			this.m_cSpacing = state.cSpace;
			this.m_monoSpacing = state.mSpace;
			this.m_lineJustification = state.horizontalAlignment;
			this.m_marginLeft = state.marginLeft;
			this.m_marginRight = state.marginRight;
			this.m_htmlColor = state.vertexColor;
			this.m_underlineColor = state.underlineColor;
			this.m_strikethroughColor = state.strikethroughColor;
			this.m_HighlightState = state.highlightState;
			this.m_isNonBreakingSpace = state.isNonBreakingSpace;
			this.tag_NoParsing = state.tagNoParsing;
			this.m_FXRotation = state.fxRotation;
			this.m_FXScale = state.fxScale;
			this.m_fontStyleStack = state.basicStyleStack;
			this.m_ItalicAngleStack = state.italicAngleStack;
			this.m_colorStack = state.colorStack;
			this.m_underlineColorStack = state.underlineColorStack;
			this.m_strikethroughColorStack = state.strikethroughColorStack;
			this.m_HighlightStateStack = state.highlightStateStack;
			this.m_colorGradientStack = state.colorGradientStack;
			this.m_sizeStack = state.sizeStack;
			this.m_indentStack = state.indentStack;
			this.m_FontWeightStack = state.fontWeightStack;
			this.m_baselineOffsetStack = state.baselineStack;
			this.m_actionStack = state.actionStack;
			TMP_Text.m_materialReferenceStack = state.materialReferenceStack;
			this.m_lineJustificationStack = state.lineJustificationStack;
			this.m_spriteAnimationID = state.spriteAnimationID;
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
			vertexColor.a = ((this.m_fontColor32.a < vertexColor.a) ? this.m_fontColor32.a : vertexColor.a);
			bool flag = (this.m_currentFontAsset.m_AtlasRenderMode & (GlyphRenderMode)65536) == (GlyphRenderMode)65536;
			if (!this.m_enableVertexGradient || flag)
			{
				vertexColor = (flag ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, vertexColor.a) : vertexColor);
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else if (!this.m_overrideHtmlColors && this.m_colorStack.index > 1)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = vertexColor;
			}
			else if (this.m_fontColorGradientPreset != null)
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradientPreset.bottomLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradientPreset.topLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradientPreset.topRight * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradientPreset.bottomRight * vertexColor;
			}
			else
			{
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_fontColorGradient.bottomLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_fontColorGradient.topLeft * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_fontColorGradient.topRight * vertexColor;
				this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_fontColorGradient.bottomRight * vertexColor;
			}
			if (this.m_colorGradientPreset != null && !flag)
			{
				if (this.m_colorGradientPresetIsTinted)
				{
					TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
					int characterCount = this.m_characterCount;
					characterInfo[characterCount].vertex_BL.color = characterInfo[characterCount].vertex_BL.color * this.m_colorGradientPreset.bottomLeft;
					TMP_CharacterInfo[] characterInfo2 = this.m_textInfo.characterInfo;
					int characterCount2 = this.m_characterCount;
					characterInfo2[characterCount2].vertex_TL.color = characterInfo2[characterCount2].vertex_TL.color * this.m_colorGradientPreset.topLeft;
					TMP_CharacterInfo[] characterInfo3 = this.m_textInfo.characterInfo;
					int characterCount3 = this.m_characterCount;
					characterInfo3[characterCount3].vertex_TR.color = characterInfo3[characterCount3].vertex_TR.color * this.m_colorGradientPreset.topRight;
					TMP_CharacterInfo[] characterInfo4 = this.m_textInfo.characterInfo;
					int characterCount4 = this.m_characterCount;
					characterInfo4[characterCount4].vertex_BR.color = characterInfo4[characterCount4].vertex_BR.color * this.m_colorGradientPreset.bottomRight;
				}
				else
				{
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = this.m_colorGradientPreset.bottomLeft.MinAlpha(vertexColor);
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = this.m_colorGradientPreset.topLeft.MinAlpha(vertexColor);
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = this.m_colorGradientPreset.topRight.MinAlpha(vertexColor);
					this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = this.m_colorGradientPreset.bottomRight.MinAlpha(vertexColor);
				}
			}
			if (!this.m_isSDFShader)
			{
				style_padding = 0f;
			}
			Glyph alternativeGlyph = this.m_textInfo.characterInfo[this.m_characterCount].alternativeGlyph;
			GlyphRect glyphRect = ((alternativeGlyph == null) ? this.m_cached_TextElement.m_Glyph.glyphRect : alternativeGlyph.glyphRect);
			Vector2 vector;
			vector.x = ((float)glyphRect.x - padding - style_padding) / (float)this.m_currentFontAsset.m_AtlasWidth;
			vector.y = ((float)glyphRect.y - padding - style_padding) / (float)this.m_currentFontAsset.m_AtlasHeight;
			Vector2 vector2;
			vector2.x = vector.x;
			vector2.y = ((float)glyphRect.y + padding + style_padding + (float)glyphRect.height) / (float)this.m_currentFontAsset.m_AtlasHeight;
			Vector2 vector3;
			vector3.x = ((float)glyphRect.x + padding + style_padding + (float)glyphRect.width) / (float)this.m_currentFontAsset.m_AtlasWidth;
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
			Color32 color = (this.m_tintSprite ? this.m_spriteColor.Multiply(vertexColor) : this.m_spriteColor);
			color.a = ((color.a < this.m_fontColor32.a) ? ((color.a < vertexColor.a) ? color.a : vertexColor.a) : this.m_fontColor32.a);
			Color32 color2 = color;
			Color32 color3 = color;
			Color32 color4 = color;
			Color32 color5 = color;
			if (this.m_enableVertexGradient)
			{
				if (this.m_fontColorGradientPreset != null)
				{
					color2 = (this.m_tintSprite ? color2.Multiply(this.m_fontColorGradientPreset.bottomLeft) : color2);
					color3 = (this.m_tintSprite ? color3.Multiply(this.m_fontColorGradientPreset.topLeft) : color3);
					color4 = (this.m_tintSprite ? color4.Multiply(this.m_fontColorGradientPreset.topRight) : color4);
					color5 = (this.m_tintSprite ? color5.Multiply(this.m_fontColorGradientPreset.bottomRight) : color5);
				}
				else
				{
					color2 = (this.m_tintSprite ? color2.Multiply(this.m_fontColorGradient.bottomLeft) : color2);
					color3 = (this.m_tintSprite ? color3.Multiply(this.m_fontColorGradient.topLeft) : color3);
					color4 = (this.m_tintSprite ? color4.Multiply(this.m_fontColorGradient.topRight) : color4);
					color5 = (this.m_tintSprite ? color5.Multiply(this.m_fontColorGradient.bottomRight) : color5);
				}
			}
			if (this.m_colorGradientPreset != null)
			{
				color2 = (this.m_tintSprite ? color2.Multiply(this.m_colorGradientPreset.bottomLeft) : color2);
				color3 = (this.m_tintSprite ? color3.Multiply(this.m_colorGradientPreset.topLeft) : color3);
				color4 = (this.m_tintSprite ? color4.Multiply(this.m_colorGradientPreset.topRight) : color4);
				color5 = (this.m_tintSprite ? color5.Multiply(this.m_colorGradientPreset.bottomRight) : color5);
			}
			this.m_tintSprite = false;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.color = color2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.color = color3;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.color = color4;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.color = color5;
			GlyphRect glyphRect = this.m_cached_TextElement.m_Glyph.glyphRect;
			Vector2 vector = new Vector2((float)glyphRect.x / (float)this.m_currentSpriteAsset.spriteSheet.width, (float)glyphRect.y / (float)this.m_currentSpriteAsset.spriteSheet.height);
			Vector2 vector2 = new Vector2(vector.x, (float)(glyphRect.y + glyphRect.height) / (float)this.m_currentSpriteAsset.spriteSheet.height);
			Vector2 vector3 = new Vector2((float)(glyphRect.x + glyphRect.width) / (float)this.m_currentSpriteAsset.spriteSheet.width, vector2.y);
			Vector2 vector4 = new Vector2(vector3.x, vector.y);
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BL.uv = vector;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TL.uv = vector2;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_TR.uv = vector3;
			this.m_textInfo.characterInfo[this.m_characterCount].vertex_BR.uv = vector4;
		}

		protected virtual void FillCharacterVertexBuffers(int i)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			int vertexCount = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			if (vertexCount >= this.m_textInfo.meshInfo[materialReferenceIndex].vertices.Length)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo((vertexCount + 4) / 4));
			}
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = vertexCount;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[vertexCount] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + vertexCount] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + vertexCount] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + vertexCount] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[vertexCount] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + vertexCount] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + vertexCount] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + vertexCount] = characterInfo[i].vertex_BR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexCount] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + vertexCount] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + vertexCount] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + vertexCount] = characterInfo[i].vertex_BR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[vertexCount] = (this.m_ConvertToLinearSpace ? characterInfo[i].vertex_BL.color.GammaToLinear() : characterInfo[i].vertex_BL.color);
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + vertexCount] = (this.m_ConvertToLinearSpace ? characterInfo[i].vertex_TL.color.GammaToLinear() : characterInfo[i].vertex_TL.color);
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + vertexCount] = (this.m_ConvertToLinearSpace ? characterInfo[i].vertex_TR.color.GammaToLinear() : characterInfo[i].vertex_TR.color);
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + vertexCount] = (this.m_ConvertToLinearSpace ? characterInfo[i].vertex_BR.color.GammaToLinear() : characterInfo[i].vertex_BR.color);
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = vertexCount + 4;
		}

		protected virtual void FillCharacterVertexBuffers(int i, bool isVolumetric)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			int vertexCount = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			if (vertexCount >= this.m_textInfo.meshInfo[materialReferenceIndex].vertices.Length)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo((vertexCount + (isVolumetric ? 8 : 4)) / 4));
			}
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = vertexCount;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[vertexCount] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + vertexCount] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + vertexCount] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + vertexCount] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[vertexCount] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + vertexCount] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + vertexCount] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + vertexCount] = characterInfo[i].vertex_BR.uv;
			if (isVolumetric)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[4 + vertexCount] = characterInfo[i].vertex_BL.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[5 + vertexCount] = characterInfo[i].vertex_TL.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[6 + vertexCount] = characterInfo[i].vertex_TR.uv;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[7 + vertexCount] = characterInfo[i].vertex_BR.uv;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexCount] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + vertexCount] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + vertexCount] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + vertexCount] = characterInfo[i].vertex_BR.uv2;
			if (isVolumetric)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[4 + vertexCount] = characterInfo[i].vertex_BL.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[5 + vertexCount] = characterInfo[i].vertex_TL.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[6 + vertexCount] = characterInfo[i].vertex_TR.uv2;
				this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[7 + vertexCount] = characterInfo[i].vertex_BR.uv2;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[vertexCount] = characterInfo[i].vertex_BL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + vertexCount] = characterInfo[i].vertex_TL.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + vertexCount] = characterInfo[i].vertex_TR.color;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + vertexCount] = characterInfo[i].vertex_BR.color;
			if (isVolumetric)
			{
				Color32 color = new Color32(byte.MaxValue, byte.MaxValue, 128, byte.MaxValue);
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[4 + vertexCount] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[5 + vertexCount] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[6 + vertexCount] = color;
				this.m_textInfo.meshInfo[materialReferenceIndex].colors32[7 + vertexCount] = color;
			}
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = vertexCount + ((!isVolumetric) ? 4 : 8);
		}

		protected virtual void FillSpriteVertexBuffers(int i)
		{
			int materialReferenceIndex = this.m_textInfo.characterInfo[i].materialReferenceIndex;
			int vertexCount = this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount;
			if (vertexCount >= this.m_textInfo.meshInfo[materialReferenceIndex].vertices.Length)
			{
				this.m_textInfo.meshInfo[materialReferenceIndex].ResizeMeshInfo(Mathf.NextPowerOfTwo((vertexCount + 4) / 4));
			}
			TMP_CharacterInfo[] characterInfo = this.m_textInfo.characterInfo;
			this.m_textInfo.characterInfo[i].vertexIndex = vertexCount;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[vertexCount] = characterInfo[i].vertex_BL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[1 + vertexCount] = characterInfo[i].vertex_TL.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[2 + vertexCount] = characterInfo[i].vertex_TR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].vertices[3 + vertexCount] = characterInfo[i].vertex_BR.position;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[vertexCount] = characterInfo[i].vertex_BL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[1 + vertexCount] = characterInfo[i].vertex_TL.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[2 + vertexCount] = characterInfo[i].vertex_TR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs0[3 + vertexCount] = characterInfo[i].vertex_BR.uv;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[vertexCount] = characterInfo[i].vertex_BL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[1 + vertexCount] = characterInfo[i].vertex_TL.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[2 + vertexCount] = characterInfo[i].vertex_TR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].uvs2[3 + vertexCount] = characterInfo[i].vertex_BR.uv2;
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[vertexCount] = (this.m_ConvertToLinearSpace ? characterInfo[i].vertex_BL.color.GammaToLinear() : characterInfo[i].vertex_BL.color);
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[1 + vertexCount] = (this.m_ConvertToLinearSpace ? characterInfo[i].vertex_TL.color.GammaToLinear() : characterInfo[i].vertex_TL.color);
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[2 + vertexCount] = (this.m_ConvertToLinearSpace ? characterInfo[i].vertex_TR.color.GammaToLinear() : characterInfo[i].vertex_TR.color);
			this.m_textInfo.meshInfo[materialReferenceIndex].colors32[3 + vertexCount] = (this.m_ConvertToLinearSpace ? characterInfo[i].vertex_BR.color.GammaToLinear() : characterInfo[i].vertex_BR.color);
			this.m_textInfo.meshInfo[materialReferenceIndex].vertexCount = vertexCount + 4;
		}

		protected virtual void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, float startScale, float endScale, float maxScale, float sdfScale, Color32 underlineColor)
		{
			this.GetUnderlineSpecialCharacter(this.m_fontAsset);
			if (this.m_Underline.character == null)
			{
				if (!TMP_Settings.warningsDisabled)
				{
					global::UnityEngine.Debug.LogWarning("Unable to add underline or strikethrough since the character [0x5F] used by these features is not present in the Font Asset assigned to this text object.", this);
				}
				return;
			}
			int materialIndex = this.m_Underline.materialIndex;
			int num = index + 12;
			if (num > this.m_textInfo.meshInfo[materialIndex].vertices.Length)
			{
				this.m_textInfo.meshInfo[materialIndex].ResizeMeshInfo(num / 4);
			}
			start.y = Mathf.Min(start.y, end.y);
			end.y = Mathf.Min(start.y, end.y);
			GlyphMetrics metrics = this.m_Underline.character.glyph.metrics;
			GlyphRect glyphRect = this.m_Underline.character.glyph.glyphRect;
			float num2 = metrics.width / 2f * maxScale;
			if (end.x - start.x < metrics.width * maxScale)
			{
				num2 = (end.x - start.x) / 2f;
			}
			float num3 = this.m_padding * startScale / maxScale;
			float num4 = this.m_padding * endScale / maxScale;
			float underlineThickness = this.m_Underline.fontAsset.faceInfo.underlineThickness;
			Vector3[] vertices = this.m_textInfo.meshInfo[materialIndex].vertices;
			vertices[index] = start + new Vector3(0f, 0f - (underlineThickness + this.m_padding) * maxScale, 0f);
			vertices[index + 1] = start + new Vector3(0f, this.m_padding * maxScale, 0f);
			vertices[index + 2] = vertices[index + 1] + new Vector3(num2, 0f, 0f);
			vertices[index + 3] = vertices[index] + new Vector3(num2, 0f, 0f);
			vertices[index + 4] = vertices[index + 3];
			vertices[index + 5] = vertices[index + 2];
			vertices[index + 6] = end + new Vector3(-num2, this.m_padding * maxScale, 0f);
			vertices[index + 7] = end + new Vector3(-num2, -(underlineThickness + this.m_padding) * maxScale, 0f);
			vertices[index + 8] = vertices[index + 7];
			vertices[index + 9] = vertices[index + 6];
			vertices[index + 10] = end + new Vector3(0f, this.m_padding * maxScale, 0f);
			vertices[index + 11] = end + new Vector3(0f, -(underlineThickness + this.m_padding) * maxScale, 0f);
			Vector4[] uvs = this.m_textInfo.meshInfo[materialIndex].uvs0;
			int atlasWidth = this.m_Underline.fontAsset.atlasWidth;
			int atlasHeight = this.m_Underline.fontAsset.atlasHeight;
			float num5 = Mathf.Abs(sdfScale);
			Vector4 vector = new Vector4(((float)glyphRect.x - num3) / (float)atlasWidth, ((float)glyphRect.y - this.m_padding) / (float)atlasHeight, 0f, num5);
			Vector4 vector2 = new Vector4(vector.x, ((float)(glyphRect.y + glyphRect.height) + this.m_padding) / (float)atlasHeight, 0f, num5);
			Vector4 vector3 = new Vector4(((float)glyphRect.x - num3 + (float)glyphRect.width / 2f) / (float)atlasWidth, vector2.y, 0f, num5);
			Vector4 vector4 = new Vector4(vector3.x, vector.y, 0f, num5);
			Vector4 vector5 = new Vector4(((float)glyphRect.x + num4 + (float)glyphRect.width / 2f) / (float)atlasWidth, vector2.y, 0f, num5);
			Vector4 vector6 = new Vector4(vector5.x, vector.y, 0f, num5);
			Vector4 vector7 = new Vector4(((float)glyphRect.x + num4 + (float)glyphRect.width) / (float)atlasWidth, vector2.y, 0f, num5);
			Vector4 vector8 = new Vector4(vector7.x, vector.y, 0f, num5);
			uvs[index] = vector;
			uvs[1 + index] = vector2;
			uvs[2 + index] = vector3;
			uvs[3 + index] = vector4;
			uvs[4 + index] = new Vector4(vector3.x - vector3.x * 0.001f, vector.y, 0f, num5);
			uvs[5 + index] = new Vector4(vector3.x - vector3.x * 0.001f, vector2.y, 0f, num5);
			uvs[6 + index] = new Vector4(vector3.x + vector3.x * 0.001f, vector2.y, 0f, num5);
			uvs[7 + index] = new Vector4(vector3.x + vector3.x * 0.001f, vector.y, 0f, num5);
			uvs[8 + index] = vector6;
			uvs[9 + index] = vector5;
			uvs[10 + index] = vector7;
			uvs[11 + index] = vector8;
			float num6 = (vertices[index + 2].x - start.x) / (end.x - start.x);
			Vector2[] uvs2 = this.m_textInfo.meshInfo[materialIndex].uvs2;
			uvs2[index] = new Vector2(0f, 0f);
			uvs2[1 + index] = new Vector2(0f, 1f);
			uvs2[2 + index] = new Vector2(num6, 1f);
			uvs2[3 + index] = new Vector2(num6, 0f);
			float num7 = (vertices[index + 4].x - start.x) / (end.x - start.x);
			num6 = (vertices[index + 6].x - start.x) / (end.x - start.x);
			uvs2[4 + index] = new Vector2(num7, 0f);
			uvs2[5 + index] = new Vector2(num7, 1f);
			uvs2[6 + index] = new Vector2(num6, 1f);
			uvs2[7 + index] = new Vector2(num6, 0f);
			num7 = (vertices[index + 8].x - start.x) / (end.x - start.x);
			uvs2[8 + index] = new Vector2(num7, 0f);
			uvs2[9 + index] = new Vector2(num7, 1f);
			uvs2[10 + index] = new Vector2(1f, 1f);
			uvs2[11 + index] = new Vector2(1f, 0f);
			underlineColor.a = ((this.m_fontColor32.a < underlineColor.a) ? this.m_fontColor32.a : underlineColor.a);
			Color32[] colors = this.m_textInfo.meshInfo[materialIndex].colors32;
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

		protected virtual void DrawTextHighlight(Vector3 start, Vector3 end, ref int index, Color32 highlightColor)
		{
			if (this.m_Underline.character == null)
			{
				this.GetUnderlineSpecialCharacter(this.m_fontAsset);
				if (this.m_Underline.character == null)
				{
					if (!TMP_Settings.warningsDisabled)
					{
						global::UnityEngine.Debug.LogWarning("Unable to add highlight since the primary Font Asset doesn't contain the underline character.", this);
					}
					return;
				}
			}
			int materialIndex = this.m_Underline.materialIndex;
			int num = index + 4;
			if (num > this.m_textInfo.meshInfo[materialIndex].vertices.Length)
			{
				this.m_textInfo.meshInfo[materialIndex].ResizeMeshInfo(num / 4);
			}
			Vector3[] vertices = this.m_textInfo.meshInfo[materialIndex].vertices;
			vertices[index] = start;
			vertices[index + 1] = new Vector3(start.x, end.y, 0f);
			vertices[index + 2] = end;
			vertices[index + 3] = new Vector3(end.x, start.y, 0f);
			Vector4[] uvs = this.m_textInfo.meshInfo[materialIndex].uvs0;
			int atlasWidth = this.m_Underline.fontAsset.atlasWidth;
			int atlasHeight = this.m_Underline.fontAsset.atlasHeight;
			GlyphRect glyphRect = this.m_Underline.character.glyph.glyphRect;
			Vector2 vector = new Vector2(((float)glyphRect.x + (float)glyphRect.width / 2f) / (float)atlasWidth, ((float)glyphRect.y + (float)glyphRect.height / 2f) / (float)atlasHeight);
			Vector2 vector2 = new Vector2(1f / (float)atlasWidth, 1f / (float)atlasHeight);
			uvs[index] = vector - vector2;
			uvs[index + 1] = vector + new Vector2(-vector2.x, vector2.y);
			uvs[index + 2] = vector + vector2;
			uvs[index + 3] = vector + new Vector2(vector2.x, -vector2.y);
			Vector2[] uvs2 = this.m_textInfo.meshInfo[materialIndex].uvs2;
			Vector2 vector3 = new Vector2(0f, 1f);
			uvs2[index] = vector3;
			uvs2[index + 1] = vector3;
			uvs2[index + 2] = vector3;
			uvs2[index + 3] = vector3;
			highlightColor.a = ((this.m_fontColor32.a < highlightColor.a) ? this.m_fontColor32.a : highlightColor.a);
			Color32[] colors = this.m_textInfo.meshInfo[materialIndex].colors32;
			colors[index] = highlightColor;
			colors[index + 1] = highlightColor;
			colors[index + 2] = highlightColor;
			colors[index + 3] = highlightColor;
			index += 4;
		}

		protected void LoadDefaultSettings()
		{
			if (this.m_fontSize == -99f || this.m_isWaitingOnResourceLoad)
			{
				this.m_rectTransform = this.rectTransform;
				if (TMP_Settings.autoSizeTextContainer)
				{
					this.autoSizeTextContainer = true;
				}
				else if (base.GetType() == typeof(TextMeshPro))
				{
					if (this.m_rectTransform.sizeDelta == new Vector2(100f, 100f))
					{
						this.m_rectTransform.sizeDelta = TMP_Settings.defaultTextMeshProTextContainerSize;
					}
				}
				else if (this.m_rectTransform.sizeDelta == new Vector2(100f, 100f))
				{
					this.m_rectTransform.sizeDelta = TMP_Settings.defaultTextMeshProUITextContainerSize;
				}
				this.m_TextWrappingMode = TMP_Settings.textWrappingMode;
				this.m_ActiveFontFeatures = new List<OTL_FeatureTag>(TMP_Settings.fontFeatures);
				this.m_enableExtraPadding = TMP_Settings.enableExtraPadding;
				this.m_tintAllSprites = TMP_Settings.enableTintAllSprites;
				this.m_parseCtrlCharacters = TMP_Settings.enableParseEscapeCharacters;
				this.m_fontSize = (this.m_fontSizeBase = TMP_Settings.defaultFontSize);
				this.m_fontSizeMin = this.m_fontSize * TMP_Settings.defaultTextAutoSizingMinRatio;
				this.m_fontSizeMax = this.m_fontSize * TMP_Settings.defaultTextAutoSizingMaxRatio;
				this.m_isWaitingOnResourceLoad = false;
				this.raycastTarget = TMP_Settings.enableRaycastTarget;
				this.m_IsTextObjectScaleStatic = TMP_Settings.isTextObjectScaleStatic;
			}
			else
			{
				if (this.m_textAlignment < (TextAlignmentOptions)255)
				{
					this.m_textAlignment = TMP_Compatibility.ConvertTextAlignmentEnumValues(this.m_textAlignment);
				}
				if (this.m_ActiveFontFeatures.Count == 1 && this.m_ActiveFontFeatures[0] == (OTL_FeatureTag)0U)
				{
					this.m_ActiveFontFeatures.Clear();
					if (this.m_enableKerning)
					{
						this.m_ActiveFontFeatures.Add(OTL_FeatureTag.kern);
					}
				}
			}
			if (this.m_textAlignment != TextAlignmentOptions.Converted)
			{
				this.m_HorizontalAlignment = (HorizontalAlignmentOptions)(this.m_textAlignment & (TextAlignmentOptions)255);
				this.m_VerticalAlignment = (VerticalAlignmentOptions)(this.m_textAlignment & (TextAlignmentOptions)65280);
				this.m_textAlignment = TextAlignmentOptions.Converted;
			}
		}

		protected void GetSpecialCharacters(TMP_FontAsset fontAsset)
		{
			this.GetEllipsisSpecialCharacter(fontAsset);
			this.GetUnderlineSpecialCharacter(fontAsset);
		}

		protected void GetEllipsisSpecialCharacter(TMP_FontAsset fontAsset)
		{
			bool flag;
			TMP_Character tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(8230U, fontAsset, false, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
			if (tmp_Character == null && fontAsset.m_FallbackFontAssetTable != null && fontAsset.m_FallbackFontAssetTable.Count > 0)
			{
				tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAssets(8230U, fontAsset, fontAsset.m_FallbackFontAssetTable, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
			}
			if (tmp_Character == null && TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
			{
				tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAssets(8230U, fontAsset, TMP_Settings.fallbackFontAssets, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
			}
			if (tmp_Character == null && TMP_Settings.defaultFontAsset != null)
			{
				tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(8230U, TMP_Settings.defaultFontAsset, true, this.m_FontStyleInternal, this.m_FontWeightInternal, out flag);
			}
			if (tmp_Character != null)
			{
				this.m_Ellipsis = new TMP_Text.SpecialCharacter(tmp_Character, 0);
			}
		}

		protected void GetUnderlineSpecialCharacter(TMP_FontAsset fontAsset)
		{
			bool flag;
			TMP_Character characterFromFontAsset = TMP_FontAssetUtilities.GetCharacterFromFontAsset(95U, fontAsset, false, FontStyles.Normal, FontWeight.Regular, out flag);
			if (characterFromFontAsset != null)
			{
				this.m_Underline = new TMP_Text.SpecialCharacter(characterFromFontAsset, 0);
			}
		}

		protected void ReplaceTagWithCharacter(int[] chars, int insertionIndex, int tagLength, char c)
		{
			chars[insertionIndex] = (int)c;
			for (int i = insertionIndex + tagLength; i < chars.Length; i++)
			{
				chars[i - 3] = chars[i];
			}
		}

		protected TMP_FontAsset GetFontAssetForWeight(int fontWeight)
		{
			bool flag = (this.m_FontStyleInternal & FontStyles.Italic) == FontStyles.Italic || (this.m_fontStyle & FontStyles.Italic) == FontStyles.Italic;
			int num = fontWeight / 100;
			TMP_FontAsset tmp_FontAsset;
			if (flag)
			{
				tmp_FontAsset = this.m_currentFontAsset.fontWeightTable[num].italicTypeface;
			}
			else
			{
				tmp_FontAsset = this.m_currentFontAsset.fontWeightTable[num].regularTypeface;
			}
			return tmp_FontAsset;
		}

		internal TMP_TextElement GetTextElement(uint unicode, TMP_FontAsset fontAsset, FontStyles fontStyle, FontWeight fontWeight, out bool isUsingAlternativeTypeface)
		{
			TMP_Character tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(unicode, fontAsset, true, fontStyle, fontWeight, out isUsingAlternativeTypeface);
			if (tmp_Character != null)
			{
				fontAsset.AddCharacterToLookupCache(unicode, tmp_Character, fontStyle, fontWeight, isUsingAlternativeTypeface);
				return tmp_Character;
			}
			if (fontAsset.instanceID != this.m_fontAsset.instanceID)
			{
				tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(unicode, this.m_fontAsset, false, fontStyle, fontWeight, out isUsingAlternativeTypeface);
				if (tmp_Character != null)
				{
					fontAsset.AddCharacterToLookupCache(unicode, tmp_Character, fontStyle, fontWeight, isUsingAlternativeTypeface);
					return tmp_Character;
				}
				if (this.m_fontAsset.m_FallbackFontAssetTable != null && this.m_fontAsset.m_FallbackFontAssetTable.Count > 0)
				{
					tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAssets(unicode, fontAsset, this.m_fontAsset.m_FallbackFontAssetTable, true, fontStyle, fontWeight, out isUsingAlternativeTypeface);
				}
				if (tmp_Character != null)
				{
					fontAsset.AddCharacterToLookupCache(unicode, tmp_Character, fontStyle, fontWeight, isUsingAlternativeTypeface);
					return tmp_Character;
				}
			}
			if (fontStyle != FontStyles.Normal || fontWeight != FontWeight.Regular)
			{
				tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(unicode, fontAsset, true, FontStyles.Normal, FontWeight.Regular, out isUsingAlternativeTypeface);
				if (tmp_Character != null)
				{
					fontAsset.AddCharacterToLookupCache(unicode, tmp_Character, FontStyles.Normal, FontWeight.Regular, isUsingAlternativeTypeface);
					return tmp_Character;
				}
				if (TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
				{
					tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAssets(unicode, fontAsset, TMP_Settings.fallbackFontAssets, true, FontStyles.Normal, FontWeight.Regular, out isUsingAlternativeTypeface);
				}
				if (tmp_Character != null)
				{
					fontAsset.AddCharacterToLookupCache(unicode, tmp_Character, FontStyles.Normal, FontWeight.Regular, isUsingAlternativeTypeface);
					return tmp_Character;
				}
				if (TMP_Settings.defaultFontAsset != null)
				{
					tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(unicode, TMP_Settings.defaultFontAsset, true, FontStyles.Normal, FontWeight.Regular, out isUsingAlternativeTypeface);
				}
				if (tmp_Character != null)
				{
					fontAsset.AddCharacterToLookupCache(unicode, tmp_Character, FontStyles.Normal, FontWeight.Regular, isUsingAlternativeTypeface);
					return tmp_Character;
				}
			}
			if (this.m_spriteAsset != null)
			{
				TMP_SpriteCharacter spriteCharacterFromSpriteAsset = TMP_FontAssetUtilities.GetSpriteCharacterFromSpriteAsset(unicode, this.m_spriteAsset, true);
				if (spriteCharacterFromSpriteAsset != null)
				{
					return spriteCharacterFromSpriteAsset;
				}
			}
			if (TMP_Settings.fallbackFontAssets != null && TMP_Settings.fallbackFontAssets.Count > 0)
			{
				tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAssets(unicode, fontAsset, TMP_Settings.fallbackFontAssets, true, fontStyle, fontWeight, out isUsingAlternativeTypeface);
			}
			if (tmp_Character != null)
			{
				fontAsset.AddCharacterToLookupCache(unicode, tmp_Character, fontStyle, fontWeight, isUsingAlternativeTypeface);
				return tmp_Character;
			}
			if (TMP_Settings.defaultFontAsset != null)
			{
				tmp_Character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(unicode, TMP_Settings.defaultFontAsset, true, fontStyle, fontWeight, out isUsingAlternativeTypeface);
			}
			if (tmp_Character != null)
			{
				fontAsset.AddCharacterToLookupCache(unicode, tmp_Character, fontStyle, fontWeight, isUsingAlternativeTypeface);
				return tmp_Character;
			}
			if (TMP_Settings.defaultSpriteAsset != null)
			{
				TMP_SpriteCharacter spriteCharacterFromSpriteAsset2 = TMP_FontAssetUtilities.GetSpriteCharacterFromSpriteAsset(unicode, TMP_Settings.defaultSpriteAsset, true);
				if (spriteCharacterFromSpriteAsset2 != null)
				{
					return spriteCharacterFromSpriteAsset2;
				}
			}
			return null;
		}

		protected virtual void SetActiveSubMeshes(bool state)
		{
		}

		protected virtual void DestroySubMeshObjects()
		{
		}

		public virtual void ClearMesh()
		{
		}

		public virtual void ClearMesh(bool uploadGeometry)
		{
		}

		public virtual string GetParsedText()
		{
			if (this.m_textInfo == null)
			{
				return string.Empty;
			}
			int characterCount = this.m_textInfo.characterCount;
			char[] array = new char[characterCount];
			int num = 0;
			while (num < characterCount && num < this.m_textInfo.characterInfo.Length)
			{
				array[num] = this.m_textInfo.characterInfo[num].character;
				num++;
			}
			return new string(array);
		}

		internal bool IsSelfOrLinkedAncestor(TMP_Text targetTextComponent)
		{
			return targetTextComponent == null || (this.parentLinkedComponent != null && this.parentLinkedComponent.IsSelfOrLinkedAncestor(targetTextComponent)) || base.GetInstanceID() == targetTextComponent.GetInstanceID();
		}

		internal void ReleaseLinkedTextComponent(TMP_Text targetTextComponent)
		{
			if (targetTextComponent == null)
			{
				return;
			}
			TMP_Text linkedTextComponent = targetTextComponent.linkedTextComponent;
			if (linkedTextComponent != null)
			{
				this.ReleaseLinkedTextComponent(linkedTextComponent);
			}
			targetTextComponent.text = string.Empty;
			targetTextComponent.firstVisibleCharacter = 0;
			targetTextComponent.linkedTextComponent = null;
			targetTextComponent.parentLinkedComponent = null;
		}

		protected void DoMissingGlyphCallback(int unicode, int stringIndex, TMP_FontAsset fontAsset)
		{
			TMP_Text.MissingCharacterEventCallback onMissingCharacter = TMP_Text.OnMissingCharacter;
			if (onMissingCharacter == null)
			{
				return;
			}
			onMissingCharacter(unicode, stringIndex, this.m_text, fontAsset, this);
		}

		protected Vector2 PackUV(float x, float y, float scale)
		{
			Vector2 vector;
			vector.x = (float)((int)(x * 511f));
			vector.y = (float)((int)(y * 511f));
			vector.x = vector.x * 4096f + vector.y;
			vector.y = scale;
			return vector;
		}

		protected float PackUV(float x, float y)
		{
			float num = (float)((double)((int)(x * 511f)));
			double num2 = (double)((int)(y * 511f));
			return (float)((double)num * 4096.0 + num2);
		}

		internal virtual void InternalUpdate()
		{
		}

		protected uint HexToInt(char hex)
		{
			switch (hex)
			{
			case '0':
				return 0U;
			case '1':
				return 1U;
			case '2':
				return 2U;
			case '3':
				return 3U;
			case '4':
				return 4U;
			case '5':
				return 5U;
			case '6':
				return 6U;
			case '7':
				return 7U;
			case '8':
				return 8U;
			case '9':
				return 9U;
			case ':':
			case ';':
			case '<':
			case '=':
			case '>':
			case '?':
			case '@':
				break;
			case 'A':
				return 10U;
			case 'B':
				return 11U;
			case 'C':
				return 12U;
			case 'D':
				return 13U;
			case 'E':
				return 14U;
			case 'F':
				return 15U;
			default:
				switch (hex)
				{
				case 'a':
					return 10U;
				case 'b':
					return 11U;
				case 'c':
					return 12U;
				case 'd':
					return 13U;
				case 'e':
					return 14U;
				case 'f':
					return 15U;
				}
				break;
			}
			return 15U;
		}

		private bool IsValidUTF16(TMP_Text.TextBackingContainer text, int index)
		{
			for (int i = 0; i < 4; i++)
			{
				uint num = text[index + i];
				if ((num < 48U || num > 57U) && (num < 97U || num > 102U) && (num < 65U || num > 70U))
				{
					return false;
				}
			}
			return true;
		}

		private uint GetUTF16(uint[] text, int i)
		{
			return 0U + (this.HexToInt((char)text[i]) << 12) + (this.HexToInt((char)text[i + 1]) << 8) + (this.HexToInt((char)text[i + 2]) << 4) + this.HexToInt((char)text[i + 3]);
		}

		private uint GetUTF16(TMP_Text.TextBackingContainer text, int i)
		{
			return 0U + (this.HexToInt((char)text[i]) << 12) + (this.HexToInt((char)text[i + 1]) << 8) + (this.HexToInt((char)text[i + 2]) << 4) + this.HexToInt((char)text[i + 3]);
		}

		private bool IsValidUTF32(TMP_Text.TextBackingContainer text, int index)
		{
			for (int i = 0; i < 8; i++)
			{
				uint num = text[index + i];
				if ((num < 48U || num > 57U) && (num < 97U || num > 102U) && (num < 65U || num > 70U))
				{
					return false;
				}
			}
			return true;
		}

		private uint GetUTF32(uint[] text, int i)
		{
			return 0U + (this.HexToInt((char)text[i]) << 28) + (this.HexToInt((char)text[i + 1]) << 24) + (this.HexToInt((char)text[i + 2]) << 20) + (this.HexToInt((char)text[i + 3]) << 16) + (this.HexToInt((char)text[i + 4]) << 12) + (this.HexToInt((char)text[i + 5]) << 8) + (this.HexToInt((char)text[i + 6]) << 4) + this.HexToInt((char)text[i + 7]);
		}

		private uint GetUTF32(TMP_Text.TextBackingContainer text, int i)
		{
			return 0U + (this.HexToInt((char)text[i]) << 28) + (this.HexToInt((char)text[i + 1]) << 24) + (this.HexToInt((char)text[i + 2]) << 20) + (this.HexToInt((char)text[i + 3]) << 16) + (this.HexToInt((char)text[i + 4]) << 12) + (this.HexToInt((char)text[i + 5]) << 8) + (this.HexToInt((char)text[i + 6]) << 4) + this.HexToInt((char)text[i + 7]);
		}

		protected Color32 HexCharsToColor(char[] hexChars, int tagCount)
		{
			if (tagCount == 4)
			{
				byte b = (byte)(this.HexToInt(hexChars[1]) * 16U + this.HexToInt(hexChars[1]));
				byte b2 = (byte)(this.HexToInt(hexChars[2]) * 16U + this.HexToInt(hexChars[2]));
				byte b3 = (byte)(this.HexToInt(hexChars[3]) * 16U + this.HexToInt(hexChars[3]));
				return new Color32(b, b2, b3, byte.MaxValue);
			}
			if (tagCount == 5)
			{
				byte b4 = (byte)(this.HexToInt(hexChars[1]) * 16U + this.HexToInt(hexChars[1]));
				byte b5 = (byte)(this.HexToInt(hexChars[2]) * 16U + this.HexToInt(hexChars[2]));
				byte b6 = (byte)(this.HexToInt(hexChars[3]) * 16U + this.HexToInt(hexChars[3]));
				byte b7 = (byte)(this.HexToInt(hexChars[4]) * 16U + this.HexToInt(hexChars[4]));
				return new Color32(b4, b5, b6, b7);
			}
			if (tagCount == 7)
			{
				byte b8 = (byte)(this.HexToInt(hexChars[1]) * 16U + this.HexToInt(hexChars[2]));
				byte b9 = (byte)(this.HexToInt(hexChars[3]) * 16U + this.HexToInt(hexChars[4]));
				byte b10 = (byte)(this.HexToInt(hexChars[5]) * 16U + this.HexToInt(hexChars[6]));
				return new Color32(b8, b9, b10, byte.MaxValue);
			}
			if (tagCount == 9)
			{
				byte b11 = (byte)(this.HexToInt(hexChars[1]) * 16U + this.HexToInt(hexChars[2]));
				byte b12 = (byte)(this.HexToInt(hexChars[3]) * 16U + this.HexToInt(hexChars[4]));
				byte b13 = (byte)(this.HexToInt(hexChars[5]) * 16U + this.HexToInt(hexChars[6]));
				byte b14 = (byte)(this.HexToInt(hexChars[7]) * 16U + this.HexToInt(hexChars[8]));
				return new Color32(b11, b12, b13, b14);
			}
			if (tagCount == 10)
			{
				byte b15 = (byte)(this.HexToInt(hexChars[7]) * 16U + this.HexToInt(hexChars[7]));
				byte b16 = (byte)(this.HexToInt(hexChars[8]) * 16U + this.HexToInt(hexChars[8]));
				byte b17 = (byte)(this.HexToInt(hexChars[9]) * 16U + this.HexToInt(hexChars[9]));
				return new Color32(b15, b16, b17, byte.MaxValue);
			}
			if (tagCount == 11)
			{
				byte b18 = (byte)(this.HexToInt(hexChars[7]) * 16U + this.HexToInt(hexChars[7]));
				byte b19 = (byte)(this.HexToInt(hexChars[8]) * 16U + this.HexToInt(hexChars[8]));
				byte b20 = (byte)(this.HexToInt(hexChars[9]) * 16U + this.HexToInt(hexChars[9]));
				byte b21 = (byte)(this.HexToInt(hexChars[10]) * 16U + this.HexToInt(hexChars[10]));
				return new Color32(b18, b19, b20, b21);
			}
			if (tagCount == 13)
			{
				byte b22 = (byte)(this.HexToInt(hexChars[7]) * 16U + this.HexToInt(hexChars[8]));
				byte b23 = (byte)(this.HexToInt(hexChars[9]) * 16U + this.HexToInt(hexChars[10]));
				byte b24 = (byte)(this.HexToInt(hexChars[11]) * 16U + this.HexToInt(hexChars[12]));
				return new Color32(b22, b23, b24, byte.MaxValue);
			}
			if (tagCount == 15)
			{
				byte b25 = (byte)(this.HexToInt(hexChars[7]) * 16U + this.HexToInt(hexChars[8]));
				byte b26 = (byte)(this.HexToInt(hexChars[9]) * 16U + this.HexToInt(hexChars[10]));
				byte b27 = (byte)(this.HexToInt(hexChars[11]) * 16U + this.HexToInt(hexChars[12]));
				byte b28 = (byte)(this.HexToInt(hexChars[13]) * 16U + this.HexToInt(hexChars[14]));
				return new Color32(b25, b26, b27, b28);
			}
			return new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}

		protected Color32 HexCharsToColor(char[] hexChars, int startIndex, int length)
		{
			if (length == 7)
			{
				byte b = (byte)(this.HexToInt(hexChars[startIndex + 1]) * 16U + this.HexToInt(hexChars[startIndex + 2]));
				byte b2 = (byte)(this.HexToInt(hexChars[startIndex + 3]) * 16U + this.HexToInt(hexChars[startIndex + 4]));
				byte b3 = (byte)(this.HexToInt(hexChars[startIndex + 5]) * 16U + this.HexToInt(hexChars[startIndex + 6]));
				return new Color32(b, b2, b3, byte.MaxValue);
			}
			if (length == 9)
			{
				byte b4 = (byte)(this.HexToInt(hexChars[startIndex + 1]) * 16U + this.HexToInt(hexChars[startIndex + 2]));
				byte b5 = (byte)(this.HexToInt(hexChars[startIndex + 3]) * 16U + this.HexToInt(hexChars[startIndex + 4]));
				byte b6 = (byte)(this.HexToInt(hexChars[startIndex + 5]) * 16U + this.HexToInt(hexChars[startIndex + 6]));
				byte b7 = (byte)(this.HexToInt(hexChars[startIndex + 7]) * 16U + this.HexToInt(hexChars[startIndex + 8]));
				return new Color32(b4, b5, b6, b7);
			}
			return TMP_Text.s_colorWhite;
		}

		private int GetAttributeParameters(char[] chars, int startIndex, int length, ref float[] parameters)
		{
			int i = startIndex;
			int num = 0;
			while (i < startIndex + length)
			{
				parameters[num] = this.ConvertToFloat(chars, startIndex, length, out i);
				length -= i - startIndex + 1;
				startIndex = i + 1;
				num++;
			}
			return num;
		}

		protected float ConvertToFloat(char[] chars, int startIndex, int length)
		{
			int num;
			return this.ConvertToFloat(chars, startIndex, length, out num);
		}

		protected float ConvertToFloat(char[] chars, int startIndex, int length, out int lastIndex)
		{
			if (startIndex == 0)
			{
				lastIndex = 0;
				return -32768f;
			}
			int num = startIndex + length;
			bool flag = true;
			float num2 = 0f;
			int num3 = 1;
			if (chars[startIndex] == '+')
			{
				num3 = 1;
				startIndex++;
			}
			else if (chars[startIndex] == '-')
			{
				num3 = -1;
				startIndex++;
			}
			float num4 = 0f;
			for (int i = startIndex; i < num; i++)
			{
				uint num5 = (uint)chars[i];
				if ((num5 >= 48U && num5 <= 57U) || num5 == 46U)
				{
					if (num5 == 46U)
					{
						flag = false;
						num2 = 0.1f;
					}
					else if (flag)
					{
						num4 = num4 * 10f + (float)((ulong)(num5 - 48U) * (ulong)((long)num3));
					}
					else
					{
						num4 += (num5 - 48U) * num2 * (float)num3;
						num2 *= 0.1f;
					}
				}
				else if (num5 == 44U)
				{
					if (i + 1 < num && chars[i + 1] == ' ')
					{
						lastIndex = i + 1;
					}
					else
					{
						lastIndex = i;
					}
					if (num4 > 32767f)
					{
						return -32768f;
					}
					return num4;
				}
			}
			lastIndex = num;
			if (num4 > 32767f)
			{
				return -32768f;
			}
			return num4;
		}

		private void ClearMarkupTagAttributes()
		{
			int num = TMP_Text.m_xmlAttribute.Length;
			for (int i = 0; i < num; i++)
			{
				TMP_Text.m_xmlAttribute[i] = default(RichTextTagAttribute);
			}
		}

		internal bool ValidateHtmlTag(TMP_Text.TextProcessingElement[] chars, int startIndex, out int endIndex)
		{
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
			while (num3 < chars.Length && chars[num3].unicode != 0U && num < TMP_Text.m_htmlTag.Length && chars[num3].unicode != 60U)
			{
				uint unicode = chars[num3].unicode;
				if (unicode == 62U)
				{
					flag2 = true;
					endIndex = num3;
					TMP_Text.m_htmlTag[num] = '\0';
					break;
				}
				TMP_Text.m_htmlTag[num] = (char)unicode;
				num++;
				if (b == 1)
				{
					if (tagValueType == TagValueType.None)
					{
						if (unicode == 43U || unicode == 45U || unicode == 46U || (unicode >= 48U && unicode <= 57U))
						{
							tagUnitType = TagUnitType.Pixels;
							tagValueType = (TMP_Text.m_xmlAttribute[num2].valueType = TagValueType.NumericalValue);
							TMP_Text.m_xmlAttribute[num2].valueStartIndex = num - 1;
							RichTextTagAttribute[] xmlAttribute = TMP_Text.m_xmlAttribute;
							int num4 = num2;
							xmlAttribute[num4].valueLength = xmlAttribute[num4].valueLength + 1;
						}
						else if (unicode == 35U)
						{
							tagUnitType = TagUnitType.Pixels;
							tagValueType = (TMP_Text.m_xmlAttribute[num2].valueType = TagValueType.ColorValue);
							TMP_Text.m_xmlAttribute[num2].valueStartIndex = num - 1;
							RichTextTagAttribute[] xmlAttribute2 = TMP_Text.m_xmlAttribute;
							int num5 = num2;
							xmlAttribute2[num5].valueLength = xmlAttribute2[num5].valueLength + 1;
						}
						else if (unicode == 34U)
						{
							tagUnitType = TagUnitType.Pixels;
							tagValueType = (TMP_Text.m_xmlAttribute[num2].valueType = TagValueType.StringValue);
							TMP_Text.m_xmlAttribute[num2].valueStartIndex = num;
						}
						else
						{
							tagUnitType = TagUnitType.Pixels;
							tagValueType = (TMP_Text.m_xmlAttribute[num2].valueType = TagValueType.StringValue);
							TMP_Text.m_xmlAttribute[num2].valueStartIndex = num - 1;
							TMP_Text.m_xmlAttribute[num2].valueHashCode = ((TMP_Text.m_xmlAttribute[num2].valueHashCode << 5) + TMP_Text.m_xmlAttribute[num2].valueHashCode) ^ (int)TMP_TextUtilities.ToUpperFast((char)unicode);
							RichTextTagAttribute[] xmlAttribute3 = TMP_Text.m_xmlAttribute;
							int num6 = num2;
							xmlAttribute3[num6].valueLength = xmlAttribute3[num6].valueLength + 1;
						}
					}
					else if (tagValueType == TagValueType.NumericalValue)
					{
						if (unicode == 112U || unicode == 101U || unicode == 37U || unicode == 32U)
						{
							b = 2;
							tagValueType = TagValueType.None;
							if (unicode != 37U)
							{
								if (unicode == 101U)
								{
									tagUnitType = (TMP_Text.m_xmlAttribute[num2].unitType = TagUnitType.FontUnits);
								}
								else
								{
									tagUnitType = (TMP_Text.m_xmlAttribute[num2].unitType = TagUnitType.Pixels);
								}
							}
							else
							{
								tagUnitType = (TMP_Text.m_xmlAttribute[num2].unitType = TagUnitType.Percentage);
							}
							num2++;
							TMP_Text.m_xmlAttribute[num2].nameHashCode = 0;
							TMP_Text.m_xmlAttribute[num2].valueHashCode = 0;
							TMP_Text.m_xmlAttribute[num2].valueType = TagValueType.None;
							TMP_Text.m_xmlAttribute[num2].unitType = TagUnitType.Pixels;
							TMP_Text.m_xmlAttribute[num2].valueStartIndex = 0;
							TMP_Text.m_xmlAttribute[num2].valueLength = 0;
						}
						else
						{
							RichTextTagAttribute[] xmlAttribute4 = TMP_Text.m_xmlAttribute;
							int num7 = num2;
							xmlAttribute4[num7].valueLength = xmlAttribute4[num7].valueLength + 1;
						}
					}
					else if (tagValueType == TagValueType.ColorValue)
					{
						if (unicode != 32U)
						{
							RichTextTagAttribute[] xmlAttribute5 = TMP_Text.m_xmlAttribute;
							int num8 = num2;
							xmlAttribute5[num8].valueLength = xmlAttribute5[num8].valueLength + 1;
						}
						else
						{
							b = 2;
							tagValueType = TagValueType.None;
							tagUnitType = TagUnitType.Pixels;
							num2++;
							TMP_Text.m_xmlAttribute[num2].nameHashCode = 0;
							TMP_Text.m_xmlAttribute[num2].valueType = TagValueType.None;
							TMP_Text.m_xmlAttribute[num2].unitType = TagUnitType.Pixels;
							TMP_Text.m_xmlAttribute[num2].valueHashCode = 0;
							TMP_Text.m_xmlAttribute[num2].valueStartIndex = 0;
							TMP_Text.m_xmlAttribute[num2].valueLength = 0;
						}
					}
					else if (tagValueType == TagValueType.StringValue)
					{
						if (unicode != 34U)
						{
							TMP_Text.m_xmlAttribute[num2].valueHashCode = ((TMP_Text.m_xmlAttribute[num2].valueHashCode << 5) + TMP_Text.m_xmlAttribute[num2].valueHashCode) ^ (int)TMP_TextUtilities.ToUpperFast((char)unicode);
							RichTextTagAttribute[] xmlAttribute6 = TMP_Text.m_xmlAttribute;
							int num9 = num2;
							xmlAttribute6[num9].valueLength = xmlAttribute6[num9].valueLength + 1;
						}
						else
						{
							b = 2;
							tagValueType = TagValueType.None;
							tagUnitType = TagUnitType.Pixels;
							num2++;
							TMP_Text.m_xmlAttribute[num2].nameHashCode = 0;
							TMP_Text.m_xmlAttribute[num2].valueType = TagValueType.None;
							TMP_Text.m_xmlAttribute[num2].unitType = TagUnitType.Pixels;
							TMP_Text.m_xmlAttribute[num2].valueHashCode = 0;
							TMP_Text.m_xmlAttribute[num2].valueStartIndex = 0;
							TMP_Text.m_xmlAttribute[num2].valueLength = 0;
						}
					}
				}
				if (unicode == 61U)
				{
					b = 1;
				}
				if (b == 0 && unicode == 32U)
				{
					if (flag)
					{
						return false;
					}
					flag = true;
					b = 2;
					tagValueType = TagValueType.None;
					tagUnitType = TagUnitType.Pixels;
					num2++;
					TMP_Text.m_xmlAttribute[num2].nameHashCode = 0;
					TMP_Text.m_xmlAttribute[num2].valueType = TagValueType.None;
					TMP_Text.m_xmlAttribute[num2].unitType = TagUnitType.Pixels;
					TMP_Text.m_xmlAttribute[num2].valueHashCode = 0;
					TMP_Text.m_xmlAttribute[num2].valueStartIndex = 0;
					TMP_Text.m_xmlAttribute[num2].valueLength = 0;
				}
				if (b == 0)
				{
					TMP_Text.m_xmlAttribute[num2].nameHashCode = ((TMP_Text.m_xmlAttribute[num2].nameHashCode << 5) + TMP_Text.m_xmlAttribute[num2].nameHashCode) ^ (int)TMP_TextUtilities.ToUpperFast((char)unicode);
				}
				if (b == 2 && unicode == 32U)
				{
					b = 0;
				}
				num3++;
			}
			if (!flag2)
			{
				return false;
			}
			if (this.tag_NoParsing && TMP_Text.m_xmlAttribute[0].nameHashCode != -294095813)
			{
				return false;
			}
			if (TMP_Text.m_xmlAttribute[0].nameHashCode == -294095813)
			{
				this.tag_NoParsing = false;
				return true;
			}
			if (TMP_Text.m_htmlTag[0] == '#' && num == 4)
			{
				this.m_htmlColor = this.HexCharsToColor(TMP_Text.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			if (TMP_Text.m_htmlTag[0] == '#' && num == 5)
			{
				this.m_htmlColor = this.HexCharsToColor(TMP_Text.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			if (TMP_Text.m_htmlTag[0] == '#' && num == 7)
			{
				this.m_htmlColor = this.HexCharsToColor(TMP_Text.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			if (TMP_Text.m_htmlTag[0] == '#' && num == 9)
			{
				this.m_htmlColor = this.HexCharsToColor(TMP_Text.m_htmlTag, num);
				this.m_colorStack.Add(this.m_htmlColor);
				return true;
			}
			MarkupTag nameHashCode = (MarkupTag)TMP_Text.m_xmlAttribute[0].nameHashCode;
			if (nameHashCode <= MarkupTag.SLASH_STRIKETHROUGH)
			{
				if (nameHashCode <= MarkupTag.LINE_INDENT)
				{
					if (nameHashCode <= MarkupTag.SLASH_INDENT)
					{
						if (nameHashCode <= MarkupTag.SLASH_MARGIN)
						{
							if (nameHashCode <= MarkupTag.FONT_WEIGHT)
							{
								if (nameHashCode == MarkupTag.GRADIENT)
								{
									int valueHashCode = TMP_Text.m_xmlAttribute[0].valueHashCode;
									TMP_ColorGradient tmp_ColorGradient;
									if (MaterialReferenceManager.TryGetColorGradientPreset(valueHashCode, out tmp_ColorGradient))
									{
										this.m_colorGradientPreset = tmp_ColorGradient;
									}
									else
									{
										if (tmp_ColorGradient == null)
										{
											tmp_ColorGradient = Resources.Load<TMP_ColorGradient>(TMP_Settings.defaultColorGradientPresetsPath + new string(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength));
										}
										if (tmp_ColorGradient == null)
										{
											return false;
										}
										MaterialReferenceManager.AddColorGradientPreset(valueHashCode, tmp_ColorGradient);
										this.m_colorGradientPreset = tmp_ColorGradient;
									}
									this.m_colorGradientPresetIsTinted = false;
									int num10 = 1;
									while (num10 < TMP_Text.m_xmlAttribute.Length && TMP_Text.m_xmlAttribute[num10].nameHashCode != 0)
									{
										if (TMP_Text.m_xmlAttribute[num10].nameHashCode == 2960519)
										{
											this.m_colorGradientPresetIsTinted = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[num10].valueStartIndex, TMP_Text.m_xmlAttribute[num10].valueLength) != 0f;
										}
										num10++;
									}
									this.m_colorGradientStack.Add(this.m_colorGradientPreset);
									return true;
								}
								if (nameHashCode != MarkupTag.FONT_WEIGHT)
								{
									return false;
								}
								float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
								if (num11 == -32768f)
								{
									return false;
								}
								int num12 = (int)num11;
								if (num12 <= 400)
								{
									if (num12 <= 200)
									{
										if (num12 != 100)
										{
											if (num12 == 200)
											{
												this.m_FontWeightInternal = FontWeight.ExtraLight;
											}
										}
										else
										{
											this.m_FontWeightInternal = FontWeight.Thin;
										}
									}
									else if (num12 != 300)
									{
										if (num12 == 400)
										{
											this.m_FontWeightInternal = FontWeight.Regular;
										}
									}
									else
									{
										this.m_FontWeightInternal = FontWeight.Light;
									}
								}
								else if (num12 <= 600)
								{
									if (num12 != 500)
									{
										if (num12 == 600)
										{
											this.m_FontWeightInternal = FontWeight.SemiBold;
										}
									}
									else
									{
										this.m_FontWeightInternal = FontWeight.Medium;
									}
								}
								else if (num12 != 700)
								{
									if (num12 != 800)
									{
										if (num12 == 900)
										{
											this.m_FontWeightInternal = FontWeight.Black;
										}
									}
									else
									{
										this.m_FontWeightInternal = FontWeight.Heavy;
									}
								}
								else
								{
									this.m_FontWeightInternal = FontWeight.Bold;
								}
								this.m_FontWeightStack.Add(this.m_FontWeightInternal);
								return true;
							}
							else
							{
								if (nameHashCode == MarkupTag.SLASH_GRADIENT)
								{
									this.m_colorGradientPreset = this.m_colorGradientStack.Remove();
									return true;
								}
								if (nameHashCode == MarkupTag.ACTION)
								{
									int valueHashCode2 = TMP_Text.m_xmlAttribute[0].valueHashCode;
									if (this.m_isTextLayoutPhase)
									{
										this.m_actionStack.Add(valueHashCode2);
										global::UnityEngine.Debug.Log("Action ID: [" + valueHashCode2.ToString() + "] First character index: " + this.m_characterCount.ToString());
									}
									return true;
								}
								if (nameHashCode != MarkupTag.SLASH_MARGIN)
								{
									return false;
								}
								this.m_marginLeft = 0f;
								this.m_marginRight = 0f;
								return true;
							}
						}
						else if (nameHashCode <= MarkupTag.CHARACTER_SPACE)
						{
							if (nameHashCode == MarkupTag.SLASH_MONOSPACE)
							{
								this.m_monoSpacing = 0f;
								this.m_duoSpace = false;
								return true;
							}
							if (nameHashCode != MarkupTag.CHARACTER_SPACE)
							{
								return false;
							}
							float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
							if (num11 == -32768f)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_cSpacing = num11 * (this.m_isOrthographic ? 1f : 0.1f);
								break;
							case TagUnitType.FontUnits:
								this.m_cSpacing = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
								break;
							case TagUnitType.Percentage:
								return false;
							}
							return true;
						}
						else if (nameHashCode != MarkupTag.INDENT)
						{
							if (nameHashCode == MarkupTag.LOWERCASE)
							{
								this.m_FontStyleInternal |= FontStyles.LowerCase;
								this.m_fontStyleStack.Add(FontStyles.LowerCase);
								return true;
							}
							if (nameHashCode != MarkupTag.SLASH_INDENT)
							{
								return false;
							}
							this.tag_Indent = this.m_indentStack.Remove();
							return true;
						}
						else
						{
							float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
							if (num11 == -32768f)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.tag_Indent = num11 * (this.m_isOrthographic ? 1f : 0.1f);
								break;
							case TagUnitType.FontUnits:
								this.tag_Indent = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
								break;
							case TagUnitType.Percentage:
								this.tag_Indent = this.m_marginWidth * num11 / 100f;
								break;
							}
							this.m_indentStack.Add(this.tag_Indent);
							this.m_xAdvance = this.tag_Indent;
							return true;
						}
					}
					else if (nameHashCode <= MarkupTag.SLASH_ACTION)
					{
						if (nameHashCode <= MarkupTag.SLASH_CHARACTER_SPACE)
						{
							if (nameHashCode == MarkupTag.SLASH_LOWERCASE)
							{
								if ((this.m_fontStyle & FontStyles.LowerCase) != FontStyles.LowerCase && this.m_fontStyleStack.Remove(FontStyles.LowerCase) == 0)
								{
									this.m_FontStyleInternal &= ~FontStyles.LowerCase;
								}
								return true;
							}
							if (nameHashCode != MarkupTag.SLASH_CHARACTER_SPACE)
							{
								return false;
							}
							if (!this.m_isTextLayoutPhase)
							{
								return true;
							}
							if (this.m_characterCount > 0)
							{
								this.m_xAdvance -= this.m_cSpacing;
								this.m_textInfo.characterInfo[this.m_characterCount - 1].xAdvance = this.m_xAdvance;
							}
							this.m_cSpacing = 0f;
							return true;
						}
						else if (nameHashCode != MarkupTag.MARGIN)
						{
							if (nameHashCode != MarkupTag.MONOSPACE)
							{
								if (nameHashCode != MarkupTag.SLASH_ACTION)
								{
									return false;
								}
								if (this.m_isTextLayoutPhase)
								{
									global::UnityEngine.Debug.Log("Action ID: [" + this.m_actionStack.CurrentItem().ToString() + "] Last character index: " + (this.m_characterCount - 1).ToString());
								}
								this.m_actionStack.Remove();
								return true;
							}
							else
							{
								float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
								if (num11 == -32768f)
								{
									return false;
								}
								switch (TMP_Text.m_xmlAttribute[0].unitType)
								{
								case TagUnitType.Pixels:
									this.m_monoSpacing = num11 * (this.m_isOrthographic ? 1f : 0.1f);
									break;
								case TagUnitType.FontUnits:
									this.m_monoSpacing = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
									break;
								case TagUnitType.Percentage:
									return false;
								}
								if (TMP_Text.m_xmlAttribute[1].nameHashCode == 582810522)
								{
									this.m_duoSpace = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[1].valueStartIndex, TMP_Text.m_xmlAttribute[1].valueLength) != 0f;
								}
								return true;
							}
						}
						else
						{
							TagValueType valueType = TMP_Text.m_xmlAttribute[0].valueType;
							float num11;
							if (valueType == TagValueType.None)
							{
								int num13 = 1;
								while (num13 < TMP_Text.m_xmlAttribute.Length && TMP_Text.m_xmlAttribute[num13].nameHashCode != 0)
								{
									MarkupTag markupTag = (MarkupTag)TMP_Text.m_xmlAttribute[num13].nameHashCode;
									if (markupTag != MarkupTag.LEFT)
									{
										if (markupTag == MarkupTag.RIGHT)
										{
											num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[num13].valueStartIndex, TMP_Text.m_xmlAttribute[num13].valueLength);
											if (num11 == -32768f)
											{
												return false;
											}
											switch (TMP_Text.m_xmlAttribute[num13].unitType)
											{
											case TagUnitType.Pixels:
												this.m_marginRight = num11 * (this.m_isOrthographic ? 1f : 0.1f);
												break;
											case TagUnitType.FontUnits:
												this.m_marginRight = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
												break;
											case TagUnitType.Percentage:
												this.m_marginRight = (this.m_marginWidth - ((this.m_width != -1f) ? this.m_width : 0f)) * num11 / 100f;
												break;
											}
											this.m_marginRight = ((this.m_marginRight >= 0f) ? this.m_marginRight : 0f);
										}
									}
									else
									{
										num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[num13].valueStartIndex, TMP_Text.m_xmlAttribute[num13].valueLength);
										if (num11 == -32768f)
										{
											return false;
										}
										switch (TMP_Text.m_xmlAttribute[num13].unitType)
										{
										case TagUnitType.Pixels:
											this.m_marginLeft = num11 * (this.m_isOrthographic ? 1f : 0.1f);
											break;
										case TagUnitType.FontUnits:
											this.m_marginLeft = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
											break;
										case TagUnitType.Percentage:
											this.m_marginLeft = (this.m_marginWidth - ((this.m_width != -1f) ? this.m_width : 0f)) * num11 / 100f;
											break;
										}
										this.m_marginLeft = ((this.m_marginLeft >= 0f) ? this.m_marginLeft : 0f);
									}
									num13++;
								}
								return true;
							}
							if (valueType != TagValueType.NumericalValue)
							{
								return false;
							}
							num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
							if (num11 == -32768f)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_marginLeft = num11 * (this.m_isOrthographic ? 1f : 0.1f);
								break;
							case TagUnitType.FontUnits:
								this.m_marginLeft = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
								break;
							case TagUnitType.Percentage:
								this.m_marginLeft = (this.m_marginWidth - ((this.m_width != -1f) ? this.m_width : 0f)) * num11 / 100f;
								break;
							}
							this.m_marginLeft = ((this.m_marginLeft >= 0f) ? this.m_marginLeft : 0f);
							this.m_marginRight = this.m_marginLeft;
							return true;
						}
					}
					else if (nameHashCode <= MarkupTag.ROTATE)
					{
						if (nameHashCode == MarkupTag.SLASH_MATERIAL)
						{
							MaterialReference materialReference = TMP_Text.m_materialReferenceStack.Remove();
							this.m_currentMaterial = materialReference.material;
							this.m_currentMaterialIndex = materialReference.index;
							return true;
						}
						if (nameHashCode != MarkupTag.ROTATE)
						{
							return false;
						}
						float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
						if (num11 == -32768f)
						{
							return false;
						}
						this.m_FXRotation = Quaternion.Euler(0f, 0f, num11);
						return true;
					}
					else if (nameHashCode != MarkupTag.SPRITE)
					{
						if (nameHashCode == MarkupTag.SLASH_TABLE)
						{
							return false;
						}
						if (nameHashCode != MarkupTag.LINE_INDENT)
						{
							return false;
						}
						float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
						if (num11 == -32768f)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
							this.tag_LineIndent = num11 * (this.m_isOrthographic ? 1f : 0.1f);
							break;
						case TagUnitType.FontUnits:
							this.tag_LineIndent = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
							break;
						case TagUnitType.Percentage:
							this.tag_LineIndent = this.m_marginWidth * num11 / 100f;
							break;
						}
						this.m_xAdvance += this.tag_LineIndent;
						return true;
					}
					else
					{
						int valueHashCode3 = TMP_Text.m_xmlAttribute[0].valueHashCode;
						this.m_spriteIndex = -1;
						TMP_SpriteAsset tmp_SpriteAsset;
						if (TMP_Text.m_xmlAttribute[0].valueType == TagValueType.None || TMP_Text.m_xmlAttribute[0].valueType == TagValueType.NumericalValue)
						{
							if (this.m_spriteAsset != null)
							{
								this.m_currentSpriteAsset = this.m_spriteAsset;
							}
							else if (this.m_defaultSpriteAsset != null)
							{
								this.m_currentSpriteAsset = this.m_defaultSpriteAsset;
							}
							else if (this.m_defaultSpriteAsset == null)
							{
								if (TMP_Settings.defaultSpriteAsset != null)
								{
									this.m_defaultSpriteAsset = TMP_Settings.defaultSpriteAsset;
								}
								else
								{
									this.m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
								}
								this.m_currentSpriteAsset = this.m_defaultSpriteAsset;
							}
							if (this.m_currentSpriteAsset == null)
							{
								return false;
							}
						}
						else if (MaterialReferenceManager.TryGetSpriteAsset(valueHashCode3, out tmp_SpriteAsset))
						{
							this.m_currentSpriteAsset = tmp_SpriteAsset;
						}
						else
						{
							if (tmp_SpriteAsset == null)
							{
								Func<int, string, TMP_SpriteAsset> onSpriteAssetRequest = TMP_Text.OnSpriteAssetRequest;
								tmp_SpriteAsset = ((onSpriteAssetRequest != null) ? onSpriteAssetRequest(valueHashCode3, new string(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength)) : null);
								if (tmp_SpriteAsset == null)
								{
									tmp_SpriteAsset = Resources.Load<TMP_SpriteAsset>(TMP_Settings.defaultSpriteAssetPath + new string(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength));
								}
							}
							if (tmp_SpriteAsset == null)
							{
								return false;
							}
							MaterialReferenceManager.AddSpriteAsset(valueHashCode3, tmp_SpriteAsset);
							this.m_currentSpriteAsset = tmp_SpriteAsset;
						}
						if (TMP_Text.m_xmlAttribute[0].valueType == TagValueType.NumericalValue)
						{
							int num14 = (int)this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
							if (num14 == -32768)
							{
								return false;
							}
							if (num14 > this.m_currentSpriteAsset.spriteCharacterTable.Count - 1)
							{
								return false;
							}
							this.m_spriteIndex = num14;
						}
						this.m_spriteColor = TMP_Text.s_colorWhite;
						this.m_tintSprite = false;
						int num15 = 0;
						while (num15 < TMP_Text.m_xmlAttribute.Length && TMP_Text.m_xmlAttribute[num15].nameHashCode != 0)
						{
							int nameHashCode2 = TMP_Text.m_xmlAttribute[num15].nameHashCode;
							int num16 = 0;
							MarkupTag markupTag = (MarkupTag)nameHashCode2;
							if (markupTag <= MarkupTag.NAME)
							{
								if (markupTag != MarkupTag.ANIM)
								{
									if (markupTag != MarkupTag.NAME)
									{
										goto IL_2EBF;
									}
									this.m_currentSpriteAsset = TMP_SpriteAsset.SearchForSpriteByHashCode(this.m_currentSpriteAsset, TMP_Text.m_xmlAttribute[num15].valueHashCode, true, out num16);
									if (num16 == -1)
									{
										return false;
									}
									this.m_spriteIndex = num16;
								}
								else
								{
									if (this.GetAttributeParameters(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[num15].valueStartIndex, TMP_Text.m_xmlAttribute[num15].valueLength, ref TMP_Text.m_attributeParameterValues) != 3)
									{
										return false;
									}
									this.m_spriteIndex = (int)TMP_Text.m_attributeParameterValues[0];
									if (this.m_isTextLayoutPhase)
									{
										this.spriteAnimator.DoSpriteAnimation(this.m_characterCount, this.m_currentSpriteAsset, this.m_spriteIndex, (int)TMP_Text.m_attributeParameterValues[1], (int)TMP_Text.m_attributeParameterValues[2]);
									}
								}
							}
							else if (markupTag != MarkupTag.TINT)
							{
								if (markupTag != MarkupTag.COLOR)
								{
									if (markupTag != MarkupTag.INDEX)
									{
										goto IL_2EBF;
									}
									num16 = (int)this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[1].valueStartIndex, TMP_Text.m_xmlAttribute[1].valueLength);
									if (num16 == -32768)
									{
										return false;
									}
									if (num16 > this.m_currentSpriteAsset.spriteCharacterTable.Count - 1)
									{
										return false;
									}
									this.m_spriteIndex = num16;
								}
								else
								{
									this.m_spriteColor = this.HexCharsToColor(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[num15].valueStartIndex, TMP_Text.m_xmlAttribute[num15].valueLength);
								}
							}
							else
							{
								this.m_tintSprite = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[num15].valueStartIndex, TMP_Text.m_xmlAttribute[num15].valueLength) != 0f;
							}
							IL_2ECA:
							num15++;
							continue;
							IL_2EBF:
							if (nameHashCode2 != -991527447)
							{
								return false;
							}
							goto IL_2ECA;
						}
						if (this.m_spriteIndex == -1)
						{
							return false;
						}
						this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentSpriteAsset.material, this.m_currentSpriteAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
						this.m_textElementType = TMP_TextElementType.Sprite;
						return true;
					}
				}
				else
				{
					if (nameHashCode <= MarkupTag.MARGIN_LEFT)
					{
						if (nameHashCode <= MarkupTag.SLASH_FONT_WEIGHT)
						{
							if (nameHashCode <= MarkupTag.SLASH_ALLCAPS)
							{
								if (nameHashCode != MarkupTag.LINE_HEIGHT)
								{
									if (nameHashCode != MarkupTag.SLASH_ALLCAPS)
									{
										return false;
									}
								}
								else
								{
									float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
									if (num11 == -32768f)
									{
										return false;
									}
									switch (tagUnitType)
									{
									case TagUnitType.Pixels:
										this.m_lineHeight = num11 * (this.m_isOrthographic ? 1f : 0.1f);
										break;
									case TagUnitType.FontUnits:
										this.m_lineHeight = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
										break;
									case TagUnitType.Percentage:
									{
										float num17 = this.m_currentFontSize / this.m_currentFontAsset.faceInfo.pointSize * this.m_currentFontAsset.faceInfo.scale * (this.m_isOrthographic ? 1f : 0.1f);
										this.m_lineHeight = this.m_fontAsset.faceInfo.lineHeight * num11 / 100f * num17;
										break;
									}
									}
									return true;
								}
							}
							else
							{
								if (nameHashCode == MarkupTag.SMALLCAPS)
								{
									this.m_FontStyleInternal |= FontStyles.SmallCaps;
									this.m_fontStyleStack.Add(FontStyles.SmallCaps);
									return true;
								}
								if (nameHashCode == MarkupTag.SLASH_ROTATE)
								{
									this.m_FXRotation = Quaternion.identity;
									return true;
								}
								if (nameHashCode != MarkupTag.SLASH_FONT_WEIGHT)
								{
									return false;
								}
								this.m_FontWeightStack.Remove();
								if (this.m_FontStyleInternal == FontStyles.Bold)
								{
									this.m_FontWeightInternal = FontWeight.Bold;
								}
								else
								{
									this.m_FontWeightInternal = this.m_FontWeightStack.Peek();
								}
								return true;
							}
						}
						else if (nameHashCode <= MarkupTag.MARGIN_RIGHT)
						{
							if (nameHashCode != MarkupTag.SLASH_UPPERCASE)
							{
								if (nameHashCode != MarkupTag.MARGIN_RIGHT)
								{
									return false;
								}
								float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
								if (num11 == -32768f)
								{
									return false;
								}
								switch (tagUnitType)
								{
								case TagUnitType.Pixels:
									this.m_marginRight = num11 * (this.m_isOrthographic ? 1f : 0.1f);
									break;
								case TagUnitType.FontUnits:
									this.m_marginRight = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
									break;
								case TagUnitType.Percentage:
									this.m_marginRight = (this.m_marginWidth - ((this.m_width != -1f) ? this.m_width : 0f)) * num11 / 100f;
									break;
								}
								this.m_marginRight = ((this.m_marginRight >= 0f) ? this.m_marginRight : 0f);
								return true;
							}
						}
						else
						{
							if (nameHashCode == MarkupTag.NO_PARSE)
							{
								this.tag_NoParsing = true;
								return true;
							}
							if (nameHashCode == MarkupTag.UPPERCASE)
							{
								goto IL_2F72;
							}
							if (nameHashCode != MarkupTag.MARGIN_LEFT)
							{
								return false;
							}
							float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
							if (num11 == -32768f)
							{
								return false;
							}
							switch (tagUnitType)
							{
							case TagUnitType.Pixels:
								this.m_marginLeft = num11 * (this.m_isOrthographic ? 1f : 0.1f);
								break;
							case TagUnitType.FontUnits:
								this.m_marginLeft = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
								break;
							case TagUnitType.Percentage:
								this.m_marginLeft = (this.m_marginWidth - ((this.m_width != -1f) ? this.m_width : 0f)) * num11 / 100f;
								break;
							}
							this.m_marginLeft = ((this.m_marginLeft >= 0f) ? this.m_marginLeft : 0f);
							return true;
						}
						if ((this.m_fontStyle & FontStyles.UpperCase) != FontStyles.UpperCase && this.m_fontStyleStack.Remove(FontStyles.UpperCase) == 0)
						{
							this.m_FontStyleInternal &= ~FontStyles.UpperCase;
						}
						return true;
					}
					if (nameHashCode <= MarkupTag.STRIKETHROUGH)
					{
						if (nameHashCode <= MarkupTag.A)
						{
							if (nameHashCode == MarkupTag.SLASH_VERTICAL_OFFSET)
							{
								this.m_baselineOffset = 0f;
								return true;
							}
							if (nameHashCode != MarkupTag.A)
							{
								return false;
							}
							if (this.m_isTextLayoutPhase && !this.m_isCalculatingPreferredValues && TMP_Text.m_xmlAttribute[1].nameHashCode == 2535353)
							{
								int linkCount = this.m_textInfo.linkCount;
								if (linkCount + 1 > this.m_textInfo.linkInfo.Length)
								{
									TMP_TextInfo.Resize<TMP_LinkInfo>(ref this.m_textInfo.linkInfo, linkCount + 1);
								}
								this.m_textInfo.linkInfo[linkCount].textComponent = this;
								this.m_textInfo.linkInfo[linkCount].hashCode = 2535353;
								this.m_textInfo.linkInfo[linkCount].linkTextfirstCharacterIndex = this.m_characterCount;
								this.m_textInfo.linkInfo[linkCount].SetLinkID(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[1].valueStartIndex, TMP_Text.m_xmlAttribute[1].valueLength);
							}
							return true;
						}
						else
						{
							if (nameHashCode == MarkupTag.BOLD)
							{
								this.m_FontStyleInternal |= FontStyles.Bold;
								this.m_fontStyleStack.Add(FontStyles.Bold);
								this.m_FontWeightInternal = FontWeight.Bold;
								return true;
							}
							if (nameHashCode == MarkupTag.ITALIC)
							{
								this.m_FontStyleInternal |= FontStyles.Italic;
								this.m_fontStyleStack.Add(FontStyles.Italic);
								if (TMP_Text.m_xmlAttribute[1].nameHashCode == 75347905)
								{
									this.m_ItalicAngle = (int)this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[1].valueStartIndex, TMP_Text.m_xmlAttribute[1].valueLength);
									if (this.m_ItalicAngle < -180 || this.m_ItalicAngle > 180)
									{
										return false;
									}
								}
								else
								{
									this.m_ItalicAngle = (int)this.m_currentFontAsset.italicStyle;
								}
								this.m_ItalicAngleStack.Add(this.m_ItalicAngle);
								return true;
							}
							if (nameHashCode != MarkupTag.STRIKETHROUGH)
							{
								return false;
							}
							this.m_FontStyleInternal |= FontStyles.Strikethrough;
							this.m_fontStyleStack.Add(FontStyles.Strikethrough);
							if (TMP_Text.m_xmlAttribute[1].nameHashCode == 81999901)
							{
								this.m_strikethroughColor = this.HexCharsToColor(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[1].valueStartIndex, TMP_Text.m_xmlAttribute[1].valueLength);
								this.m_strikethroughColor.a = ((this.m_htmlColor.a < this.m_strikethroughColor.a) ? this.m_htmlColor.a : this.m_strikethroughColor.a);
							}
							else
							{
								this.m_strikethroughColor = this.m_htmlColor;
							}
							this.m_strikethroughColorStack.Add(this.m_strikethroughColor);
							return true;
						}
					}
					else if (nameHashCode <= MarkupTag.SLASH_BOLD)
					{
						if (nameHashCode == MarkupTag.UNDERLINE)
						{
							this.m_FontStyleInternal |= FontStyles.Underline;
							this.m_fontStyleStack.Add(FontStyles.Underline);
							if (TMP_Text.m_xmlAttribute[1].nameHashCode == 81999901)
							{
								this.m_underlineColor = this.HexCharsToColor(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[1].valueStartIndex, TMP_Text.m_xmlAttribute[1].valueLength);
								this.m_underlineColor.a = ((this.m_htmlColor.a < this.m_underlineColor.a) ? this.m_htmlColor.a : this.m_underlineColor.a);
							}
							else
							{
								this.m_underlineColor = this.m_htmlColor;
							}
							this.m_underlineColorStack.Add(this.m_underlineColor);
							return true;
						}
						if (nameHashCode == MarkupTag.SLASH_ITALIC)
						{
							if ((this.m_fontStyle & FontStyles.Italic) != FontStyles.Italic)
							{
								this.m_ItalicAngle = this.m_ItalicAngleStack.Remove();
								if (this.m_fontStyleStack.Remove(FontStyles.Italic) == 0)
								{
									this.m_FontStyleInternal &= ~FontStyles.Italic;
								}
							}
							return true;
						}
						if (nameHashCode != MarkupTag.SLASH_BOLD)
						{
							return false;
						}
						if ((this.m_fontStyle & FontStyles.Bold) != FontStyles.Bold && this.m_fontStyleStack.Remove(FontStyles.Bold) == 0)
						{
							this.m_FontStyleInternal &= ~FontStyles.Bold;
							this.m_FontWeightInternal = this.m_FontWeightStack.Peek();
						}
						return true;
					}
					else
					{
						if (nameHashCode == MarkupTag.SLASH_A)
						{
							if (this.m_isTextLayoutPhase && !this.m_isCalculatingPreferredValues)
							{
								int linkCount2 = this.m_textInfo.linkCount;
								this.m_textInfo.linkInfo[linkCount2].linkTextLength = this.m_characterCount - this.m_textInfo.linkInfo[linkCount2].linkTextfirstCharacterIndex;
								this.m_textInfo.linkCount++;
							}
							return true;
						}
						if (nameHashCode == MarkupTag.SLASH_UNDERLINE)
						{
							if ((this.m_fontStyle & FontStyles.Underline) != FontStyles.Underline && this.m_fontStyleStack.Remove(FontStyles.Underline) == 0)
							{
								this.m_FontStyleInternal &= ~FontStyles.Underline;
							}
							this.m_underlineColor = this.m_underlineColorStack.Remove();
							return true;
						}
						if (nameHashCode != MarkupTag.SLASH_STRIKETHROUGH)
						{
							return false;
						}
						if ((this.m_fontStyle & FontStyles.Strikethrough) != FontStyles.Strikethrough && this.m_fontStyleStack.Remove(FontStyles.Strikethrough) == 0)
						{
							this.m_FontStyleInternal &= ~FontStyles.Strikethrough;
						}
						this.m_strikethroughColor = this.m_strikethroughColorStack.Remove();
						return true;
					}
				}
			}
			else if (nameHashCode <= MarkupTag.SLASH_SIZE)
			{
				if (nameHashCode <= MarkupTag.PAGE)
				{
					if (nameHashCode <= MarkupTag.SLASH_SUPERSCRIPT)
					{
						if (nameHashCode <= MarkupTag.SUBSCRIPT)
						{
							if (nameHashCode != MarkupTag.POSITION)
							{
								if (nameHashCode != MarkupTag.SUBSCRIPT)
								{
									return false;
								}
								this.m_fontScaleMultiplier *= ((this.m_currentFontAsset.faceInfo.subscriptSize > 0f) ? this.m_currentFontAsset.faceInfo.subscriptSize : 1f);
								this.m_baselineOffsetStack.Push(this.m_baselineOffset);
								TMP_Text.m_materialReferenceStack.Push(TMP_Text.m_materialReferences[this.m_currentMaterialIndex]);
								float num17 = this.m_currentFontSize / this.m_currentFontAsset.faceInfo.pointSize * this.m_currentFontAsset.faceInfo.scale * (this.m_isOrthographic ? 1f : 0.1f);
								this.m_baselineOffset += this.m_currentFontAsset.faceInfo.subscriptOffset * num17 * this.m_fontScaleMultiplier;
								this.m_fontStyleStack.Add(FontStyles.Subscript);
								this.m_FontStyleInternal |= FontStyles.Subscript;
								return true;
							}
							else
							{
								float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
								if (num11 == -32768f)
								{
									return false;
								}
								switch (tagUnitType)
								{
								case TagUnitType.Pixels:
									this.m_xAdvance = num11 * (this.m_isOrthographic ? 1f : 0.1f);
									return true;
								case TagUnitType.FontUnits:
									this.m_xAdvance = num11 * this.m_currentFontSize * (this.m_isOrthographic ? 1f : 0.1f);
									return true;
								case TagUnitType.Percentage:
									this.m_xAdvance = this.m_marginWidth * num11 / 100f;
									return true;
								default:
									return false;
								}
							}
						}
						else
						{
							if (nameHashCode == MarkupTag.SUPERSCRIPT)
							{
								this.m_fontScaleMultiplier *= ((this.m_currentFontAsset.faceInfo.superscriptSize > 0f) ? this.m_currentFontAsset.faceInfo.superscriptSize : 1f);
								this.m_baselineOffsetStack.Push(this.m_baselineOffset);
								TMP_Text.m_materialReferenceStack.Push(TMP_Text.m_materialReferences[this.m_currentMaterialIndex]);
								float num17 = this.m_currentFontSize / this.m_currentFontAsset.faceInfo.pointSize * this.m_currentFontAsset.faceInfo.scale * (this.m_isOrthographic ? 1f : 0.1f);
								this.m_baselineOffset += this.m_currentFontAsset.faceInfo.superscriptOffset * num17 * this.m_fontScaleMultiplier;
								this.m_fontStyleStack.Add(FontStyles.Superscript);
								this.m_FontStyleInternal |= FontStyles.Superscript;
								return true;
							}
							if (nameHashCode == MarkupTag.SLASH_SUBSCRIPT)
							{
								if ((this.m_FontStyleInternal & FontStyles.Subscript) == FontStyles.Subscript)
								{
									TMP_FontAsset fontAsset = TMP_Text.m_materialReferenceStack.Pop().fontAsset;
									if (this.m_fontScaleMultiplier < 1f)
									{
										this.m_baselineOffset = this.m_baselineOffsetStack.Pop();
										this.m_fontScaleMultiplier /= ((fontAsset.faceInfo.subscriptSize > 0f) ? fontAsset.faceInfo.subscriptSize : 1f);
									}
									if (this.m_fontStyleStack.Remove(FontStyles.Subscript) == 0)
									{
										this.m_FontStyleInternal &= ~FontStyles.Subscript;
									}
								}
								return true;
							}
							if (nameHashCode != MarkupTag.SLASH_SUPERSCRIPT)
							{
								return false;
							}
							if ((this.m_FontStyleInternal & FontStyles.Superscript) == FontStyles.Superscript)
							{
								TMP_FontAsset fontAsset2 = TMP_Text.m_materialReferenceStack.Pop().fontAsset;
								if (this.m_fontScaleMultiplier < 1f)
								{
									this.m_baselineOffset = this.m_baselineOffsetStack.Pop();
									this.m_fontScaleMultiplier /= ((fontAsset2.faceInfo.superscriptSize > 0f) ? fontAsset2.faceInfo.superscriptSize : 1f);
								}
								if (this.m_fontStyleStack.Remove(FontStyles.Superscript) == 0)
								{
									this.m_FontStyleInternal &= ~FontStyles.Superscript;
								}
							}
							return true;
						}
					}
					else if (nameHashCode <= MarkupTag.FONT)
					{
						if (nameHashCode == MarkupTag.SLASH_POSITION)
						{
							this.m_isIgnoringAlignment = false;
							return true;
						}
						if (nameHashCode != MarkupTag.FONT)
						{
							return false;
						}
						int valueHashCode4 = TMP_Text.m_xmlAttribute[0].valueHashCode;
						int nameHashCode3 = TMP_Text.m_xmlAttribute[1].nameHashCode;
						int num18 = TMP_Text.m_xmlAttribute[1].valueHashCode;
						if (valueHashCode4 == -620974005)
						{
							this.m_currentFontAsset = TMP_Text.m_materialReferences[0].fontAsset;
							this.m_currentMaterial = TMP_Text.m_materialReferences[0].material;
							this.m_currentMaterialIndex = 0;
							TMP_Text.m_materialReferenceStack.Add(TMP_Text.m_materialReferences[0]);
							return true;
						}
						TMP_FontAsset tmp_FontAsset;
						MaterialReferenceManager.TryGetFontAsset(valueHashCode4, out tmp_FontAsset);
						if (tmp_FontAsset == null)
						{
							Func<int, string, TMP_FontAsset> onFontAssetRequest = TMP_Text.OnFontAssetRequest;
							tmp_FontAsset = ((onFontAssetRequest != null) ? onFontAssetRequest(valueHashCode4, new string(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength)) : null);
							if (tmp_FontAsset == null)
							{
								tmp_FontAsset = Resources.Load<TMP_FontAsset>(TMP_Settings.defaultFontAssetPath + new string(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength));
							}
							if (tmp_FontAsset == null)
							{
								return false;
							}
							MaterialReferenceManager.AddFontAsset(tmp_FontAsset);
						}
						if (nameHashCode3 == 0 && num18 == 0)
						{
							this.m_currentMaterial = tmp_FontAsset.material;
							this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
							TMP_Text.m_materialReferenceStack.Add(TMP_Text.m_materialReferences[this.m_currentMaterialIndex]);
						}
						else
						{
							if (nameHashCode3 != 825491659)
							{
								return false;
							}
							Material material;
							if (MaterialReferenceManager.TryGetMaterial(num18, out material))
							{
								this.m_currentMaterial = material;
								this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
								TMP_Text.m_materialReferenceStack.Add(TMP_Text.m_materialReferences[this.m_currentMaterialIndex]);
							}
							else
							{
								material = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[1].valueStartIndex, TMP_Text.m_xmlAttribute[1].valueLength));
								if (material == null)
								{
									return false;
								}
								MaterialReferenceManager.AddFontMaterial(num18, material);
								this.m_currentMaterial = material;
								this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, tmp_FontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
								TMP_Text.m_materialReferenceStack.Add(TMP_Text.m_materialReferences[this.m_currentMaterialIndex]);
							}
						}
						this.m_currentFontAsset = tmp_FontAsset;
						return true;
					}
					else
					{
						if (nameHashCode == MarkupTag.LINK)
						{
							if (this.m_isTextLayoutPhase && !this.m_isCalculatingPreferredValues)
							{
								int linkCount3 = this.m_textInfo.linkCount;
								if (linkCount3 + 1 > this.m_textInfo.linkInfo.Length)
								{
									TMP_TextInfo.Resize<TMP_LinkInfo>(ref this.m_textInfo.linkInfo, linkCount3 + 1);
								}
								this.m_textInfo.linkInfo[linkCount3].textComponent = this;
								this.m_textInfo.linkInfo[linkCount3].hashCode = TMP_Text.m_xmlAttribute[0].valueHashCode;
								this.m_textInfo.linkInfo[linkCount3].linkTextfirstCharacterIndex = this.m_characterCount;
								this.m_textInfo.linkInfo[linkCount3].linkIdFirstCharacterIndex = startIndex + TMP_Text.m_xmlAttribute[0].valueStartIndex;
								this.m_textInfo.linkInfo[linkCount3].linkIdLength = TMP_Text.m_xmlAttribute[0].valueLength;
								this.m_textInfo.linkInfo[linkCount3].SetLinkID(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
							}
							return true;
						}
						if (nameHashCode == MarkupTag.MARK)
						{
							this.m_FontStyleInternal |= FontStyles.Highlight;
							this.m_fontStyleStack.Add(FontStyles.Highlight);
							Color32 color = new Color32(byte.MaxValue, byte.MaxValue, 0, 64);
							TMP_Offset tmp_Offset = TMP_Offset.zero;
							int num19 = 0;
							while (num19 < TMP_Text.m_xmlAttribute.Length && TMP_Text.m_xmlAttribute[num19].nameHashCode != 0)
							{
								MarkupTag markupTag = (MarkupTag)TMP_Text.m_xmlAttribute[num19].nameHashCode;
								if (markupTag != MarkupTag.PADDING)
								{
									if (markupTag != MarkupTag.MARK)
									{
										if (markupTag == MarkupTag.COLOR)
										{
											color = this.HexCharsToColor(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[num19].valueStartIndex, TMP_Text.m_xmlAttribute[num19].valueLength);
										}
									}
									else if (TMP_Text.m_xmlAttribute[num19].valueType == TagValueType.ColorValue)
									{
										color = this.HexCharsToColor(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
									}
								}
								else
								{
									if (this.GetAttributeParameters(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[num19].valueStartIndex, TMP_Text.m_xmlAttribute[num19].valueLength, ref TMP_Text.m_attributeParameterValues) != 4)
									{
										return false;
									}
									tmp_Offset = new TMP_Offset(TMP_Text.m_attributeParameterValues[0], TMP_Text.m_attributeParameterValues[1], TMP_Text.m_attributeParameterValues[2], TMP_Text.m_attributeParameterValues[3]);
									tmp_Offset *= this.m_fontSize * 0.01f * (this.m_isOrthographic ? 1f : 0.1f);
								}
								num19++;
							}
							color.a = ((this.m_htmlColor.a < color.a) ? this.m_htmlColor.a : color.a);
							this.m_HighlightState = new HighlightState(color, tmp_Offset);
							this.m_HighlightStateStack.Push(this.m_HighlightState);
							return true;
						}
						if (nameHashCode != MarkupTag.PAGE)
						{
							return false;
						}
						if (this.m_overflowMode == TextOverflowModes.Page)
						{
							this.m_xAdvance = 0f + this.tag_LineIndent + this.tag_Indent;
							this.m_lineOffset = 0f;
							this.m_pageNumber++;
							this.m_isNewPage = true;
						}
						return true;
					}
				}
				else if (nameHashCode <= MarkupTag.TH)
				{
					if (nameHashCode <= MarkupTag.SIZE)
					{
						if (nameHashCode == MarkupTag.NO_BREAK)
						{
							this.m_isNonBreakingSpace = true;
							return true;
						}
						if (nameHashCode != MarkupTag.SIZE)
						{
							return false;
						}
						float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
						if (num11 == -32768f)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
							if (TMP_Text.m_htmlTag[5] == '+')
							{
								this.m_currentFontSize = this.m_fontSize + num11;
								this.m_sizeStack.Add(this.m_currentFontSize);
								return true;
							}
							if (TMP_Text.m_htmlTag[5] == '-')
							{
								this.m_currentFontSize = this.m_fontSize + num11;
								this.m_sizeStack.Add(this.m_currentFontSize);
								return true;
							}
							this.m_currentFontSize = num11;
							this.m_sizeStack.Add(this.m_currentFontSize);
							return true;
						case TagUnitType.FontUnits:
							this.m_currentFontSize = this.m_fontSize * num11;
							this.m_sizeStack.Add(this.m_currentFontSize);
							return true;
						case TagUnitType.Percentage:
							this.m_currentFontSize = this.m_fontSize * num11 / 100f;
							this.m_sizeStack.Add(this.m_currentFontSize);
							return true;
						default:
							return false;
						}
					}
					else
					{
						if (nameHashCode == MarkupTag.TR)
						{
							return false;
						}
						if (nameHashCode == MarkupTag.TD)
						{
							return false;
						}
						if (nameHashCode != MarkupTag.TH)
						{
							return false;
						}
						return false;
					}
				}
				else if (nameHashCode <= MarkupTag.SLASH_MARK)
				{
					if (nameHashCode == MarkupTag.SLASH_NO_BREAK)
					{
						this.m_isNonBreakingSpace = false;
						return true;
					}
					if (nameHashCode != MarkupTag.SLASH_MARK)
					{
						return false;
					}
					if ((this.m_fontStyle & FontStyles.Highlight) != FontStyles.Highlight)
					{
						this.m_HighlightStateStack.Remove();
						this.m_HighlightState = this.m_HighlightStateStack.current;
						if (this.m_fontStyleStack.Remove(FontStyles.Highlight) == 0)
						{
							this.m_FontStyleInternal &= ~FontStyles.Highlight;
						}
					}
					return true;
				}
				else
				{
					if (nameHashCode == MarkupTag.SLASH_LINK)
					{
						if (this.m_isTextLayoutPhase && !this.m_isCalculatingPreferredValues && this.m_textInfo.linkCount < this.m_textInfo.linkInfo.Length)
						{
							this.m_textInfo.linkInfo[this.m_textInfo.linkCount].linkTextLength = this.m_characterCount - this.m_textInfo.linkInfo[this.m_textInfo.linkCount].linkTextfirstCharacterIndex;
							this.m_textInfo.linkCount++;
						}
						return true;
					}
					if (nameHashCode == MarkupTag.SLASH_FONT)
					{
						MaterialReference materialReference2 = TMP_Text.m_materialReferenceStack.Remove();
						this.m_currentFontAsset = materialReference2.fontAsset;
						this.m_currentMaterial = materialReference2.material;
						this.m_currentMaterialIndex = materialReference2.index;
						return true;
					}
					if (nameHashCode != MarkupTag.SLASH_SIZE)
					{
						return false;
					}
					this.m_currentFontSize = this.m_sizeStack.Remove();
					return true;
				}
			}
			else if (nameHashCode <= MarkupTag.SLASH_TH)
			{
				if (nameHashCode <= MarkupTag.SLASH_LINE_INDENT)
				{
					if (nameHashCode <= MarkupTag.ALPHA)
					{
						if (nameHashCode == MarkupTag.ALIGN)
						{
							MarkupTag markupTag = (MarkupTag)TMP_Text.m_xmlAttribute[0].valueHashCode;
							if (markupTag <= MarkupTag.LEFT)
							{
								if (markupTag == MarkupTag.CENTER)
								{
									this.m_lineJustification = HorizontalAlignmentOptions.Center;
									this.m_lineJustificationStack.Add(this.m_lineJustification);
									return true;
								}
								if (markupTag == MarkupTag.LEFT)
								{
									this.m_lineJustification = HorizontalAlignmentOptions.Left;
									this.m_lineJustificationStack.Add(this.m_lineJustification);
									return true;
								}
							}
							else
							{
								if (markupTag == MarkupTag.FLUSH)
								{
									this.m_lineJustification = HorizontalAlignmentOptions.Flush;
									this.m_lineJustificationStack.Add(this.m_lineJustification);
									return true;
								}
								if (markupTag == MarkupTag.RIGHT)
								{
									this.m_lineJustification = HorizontalAlignmentOptions.Right;
									this.m_lineJustificationStack.Add(this.m_lineJustification);
									return true;
								}
								if (markupTag == MarkupTag.JUSTIFIED)
								{
									this.m_lineJustification = HorizontalAlignmentOptions.Justified;
									this.m_lineJustificationStack.Add(this.m_lineJustification);
									return true;
								}
							}
							return false;
						}
						if (nameHashCode != MarkupTag.ALPHA)
						{
							return false;
						}
						if (TMP_Text.m_xmlAttribute[0].valueLength != 3)
						{
							return false;
						}
						this.m_htmlColor.a = (byte)(this.HexToInt(TMP_Text.m_htmlTag[7]) * 16U + this.HexToInt(TMP_Text.m_htmlTag[8]));
						return true;
					}
					else if (nameHashCode != MarkupTag.COLOR)
					{
						if (nameHashCode == MarkupTag.CLASS)
						{
							return false;
						}
						if (nameHashCode != MarkupTag.SLASH_LINE_INDENT)
						{
							return false;
						}
						this.tag_LineIndent = 0f;
						return true;
					}
					else
					{
						if (TMP_Text.m_htmlTag[6] == '#' && num == 10)
						{
							this.m_htmlColor = this.HexCharsToColor(TMP_Text.m_htmlTag, num);
							this.m_colorStack.Add(this.m_htmlColor);
							return true;
						}
						if (TMP_Text.m_htmlTag[6] == '#' && num == 11)
						{
							this.m_htmlColor = this.HexCharsToColor(TMP_Text.m_htmlTag, num);
							this.m_colorStack.Add(this.m_htmlColor);
							return true;
						}
						if (TMP_Text.m_htmlTag[6] == '#' && num == 13)
						{
							this.m_htmlColor = this.HexCharsToColor(TMP_Text.m_htmlTag, num);
							this.m_colorStack.Add(this.m_htmlColor);
							return true;
						}
						if (TMP_Text.m_htmlTag[6] == '#' && num == 15)
						{
							this.m_htmlColor = this.HexCharsToColor(TMP_Text.m_htmlTag, num);
							this.m_colorStack.Add(this.m_htmlColor);
							return true;
						}
						int num12 = TMP_Text.m_xmlAttribute[0].valueHashCode;
						if (num12 <= 2457214)
						{
							if (num12 <= -1108587920)
							{
								if (num12 == -1250222130)
								{
									this.m_htmlColor = new Color32(160, 32, 240, byte.MaxValue);
									this.m_colorStack.Add(this.m_htmlColor);
									return true;
								}
								if (num12 == -1108587920)
								{
									this.m_htmlColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
									this.m_colorStack.Add(this.m_htmlColor);
									return true;
								}
							}
							else
							{
								if (num12 == -882444668)
								{
									this.m_htmlColor = Color.yellow;
									this.m_colorStack.Add(this.m_htmlColor);
									return true;
								}
								if (num12 == 91635)
								{
									this.m_htmlColor = Color.red;
									this.m_colorStack.Add(this.m_htmlColor);
									return true;
								}
								if (num12 == 2457214)
								{
									this.m_htmlColor = Color.blue;
									this.m_colorStack.Add(this.m_htmlColor);
									return true;
								}
							}
						}
						else if (num12 <= 81074727)
						{
							if (num12 == 2638345)
							{
								this.m_htmlColor = new Color32(128, 128, 128, byte.MaxValue);
								this.m_colorStack.Add(this.m_htmlColor);
								return true;
							}
							if (num12 == 81074727)
							{
								this.m_htmlColor = Color.black;
								this.m_colorStack.Add(this.m_htmlColor);
								return true;
							}
						}
						else
						{
							if (num12 == 87065851)
							{
								this.m_htmlColor = Color.green;
								this.m_colorStack.Add(this.m_htmlColor);
								return true;
							}
							if (num12 == 105680263)
							{
								this.m_htmlColor = Color.white;
								this.m_colorStack.Add(this.m_htmlColor);
								return true;
							}
							if (num12 == 341063360)
							{
								this.m_htmlColor = new Color32(173, 216, 230, byte.MaxValue);
								this.m_colorStack.Add(this.m_htmlColor);
								return true;
							}
						}
						return false;
					}
				}
				else if (nameHashCode <= MarkupTag.SCALE)
				{
					if (nameHashCode != MarkupTag.SPACE)
					{
						if (nameHashCode != MarkupTag.SCALE)
						{
							return false;
						}
						float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
						if (num11 == -32768f)
						{
							return false;
						}
						this.m_FXScale = new Vector3(num11, 1f, 1f);
						return true;
					}
					else
					{
						float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
						if (num11 == -32768f)
						{
							return false;
						}
						switch (tagUnitType)
						{
						case TagUnitType.Pixels:
							this.m_xAdvance += num11 * (this.m_isOrthographic ? 1f : 0.1f);
							return true;
						case TagUnitType.FontUnits:
							this.m_xAdvance += num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
							return true;
						case TagUnitType.Percentage:
							return false;
						default:
							return false;
						}
					}
				}
				else if (nameHashCode != MarkupTag.WIDTH)
				{
					if (nameHashCode == MarkupTag.SLASH_TR)
					{
						return false;
					}
					if (nameHashCode != MarkupTag.SLASH_TH)
					{
						return false;
					}
					return false;
				}
				else
				{
					float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
					if (num11 == -32768f)
					{
						return false;
					}
					switch (tagUnitType)
					{
					case TagUnitType.Pixels:
						this.m_width = num11 * (this.m_isOrthographic ? 1f : 0.1f);
						break;
					case TagUnitType.FontUnits:
						return false;
					case TagUnitType.Percentage:
						this.m_width = this.m_marginWidth * num11 / 100f;
						break;
					}
					return true;
				}
			}
			else if (nameHashCode <= MarkupTag.TABLE)
			{
				if (nameHashCode <= MarkupTag.SLASH_SMALLCAPS)
				{
					if (nameHashCode == MarkupTag.SLASH_TD)
					{
						return false;
					}
					if (nameHashCode != MarkupTag.SLASH_SMALLCAPS)
					{
						return false;
					}
					if ((this.m_fontStyle & FontStyles.SmallCaps) != FontStyles.SmallCaps && this.m_fontStyleStack.Remove(FontStyles.SmallCaps) == 0)
					{
						this.m_FontStyleInternal &= ~FontStyles.SmallCaps;
					}
					return true;
				}
				else
				{
					if (nameHashCode == MarkupTag.SLASH_LINE_HEIGHT)
					{
						this.m_lineHeight = -32767f;
						return true;
					}
					if (nameHashCode != MarkupTag.ALLCAPS)
					{
						if (nameHashCode != MarkupTag.TABLE)
						{
							return false;
						}
						return false;
					}
				}
			}
			else if (nameHashCode <= MarkupTag.SLASH_ALIGN)
			{
				if (nameHashCode != MarkupTag.MATERIAL)
				{
					if (nameHashCode == MarkupTag.SLASH_COLOR)
					{
						this.m_htmlColor = this.m_colorStack.Remove();
						return true;
					}
					if (nameHashCode != MarkupTag.SLASH_ALIGN)
					{
						return false;
					}
					this.m_lineJustification = this.m_lineJustificationStack.Remove();
					return true;
				}
				else
				{
					int num18 = TMP_Text.m_xmlAttribute[0].valueHashCode;
					if (num18 == -620974005)
					{
						this.m_currentMaterial = TMP_Text.m_materialReferences[0].material;
						this.m_currentMaterialIndex = 0;
						TMP_Text.m_materialReferenceStack.Add(TMP_Text.m_materialReferences[0]);
						return true;
					}
					Material material;
					if (MaterialReferenceManager.TryGetMaterial(num18, out material))
					{
						this.m_currentMaterial = material;
						this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
						TMP_Text.m_materialReferenceStack.Add(TMP_Text.m_materialReferences[this.m_currentMaterialIndex]);
					}
					else
					{
						material = Resources.Load<Material>(TMP_Settings.defaultFontAssetPath + new string(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength));
						if (material == null)
						{
							return false;
						}
						MaterialReferenceManager.AddFontMaterial(num18, material);
						this.m_currentMaterial = material;
						this.m_currentMaterialIndex = MaterialReference.AddMaterialReference(this.m_currentMaterial, this.m_currentFontAsset, ref TMP_Text.m_materialReferences, TMP_Text.m_materialReferenceIndexLookup);
						TMP_Text.m_materialReferenceStack.Add(TMP_Text.m_materialReferences[this.m_currentMaterialIndex]);
					}
					return true;
				}
			}
			else
			{
				if (nameHashCode == MarkupTag.SLASH_WIDTH)
				{
					this.m_width = -1f;
					return true;
				}
				if (nameHashCode == MarkupTag.SLASH_SCALE)
				{
					this.m_FXScale = Vector3.one;
					return true;
				}
				if (nameHashCode != MarkupTag.VERTICAL_OFFSET)
				{
					return false;
				}
				float num11 = this.ConvertToFloat(TMP_Text.m_htmlTag, TMP_Text.m_xmlAttribute[0].valueStartIndex, TMP_Text.m_xmlAttribute[0].valueLength);
				if (num11 == -32768f)
				{
					return false;
				}
				switch (tagUnitType)
				{
				case TagUnitType.Pixels:
					this.m_baselineOffset = num11 * (this.m_isOrthographic ? 1f : 0.1f);
					return true;
				case TagUnitType.FontUnits:
					this.m_baselineOffset = num11 * (this.m_isOrthographic ? 1f : 0.1f) * this.m_currentFontSize;
					return true;
				case TagUnitType.Percentage:
					return false;
				default:
					return false;
				}
			}
			IL_2F72:
			this.m_FontStyleInternal |= FontStyles.UpperCase;
			this.m_fontStyleStack.Add(FontStyles.UpperCase);
			return true;
		}

		[SerializeField]
		[TextArea(5, 10)]
		protected string m_text;

		private bool m_IsTextBackingStringDirty;

		[SerializeField]
		protected ITextPreprocessor m_TextPreprocessor;

		[SerializeField]
		protected bool m_isRightToLeft;

		[SerializeField]
		protected TMP_FontAsset m_fontAsset;

		protected TMP_FontAsset m_currentFontAsset;

		protected bool m_isSDFShader;

		[SerializeField]
		protected Material m_sharedMaterial;

		protected Material m_currentMaterial;

		protected static MaterialReference[] m_materialReferences = new MaterialReference[4];

		protected static Dictionary<int, int> m_materialReferenceIndexLookup = new Dictionary<int, int>();

		protected static TMP_TextProcessingStack<MaterialReference> m_materialReferenceStack = new TMP_TextProcessingStack<MaterialReference>(new MaterialReference[16]);

		protected int m_currentMaterialIndex;

		[SerializeField]
		protected Material[] m_fontSharedMaterials;

		[SerializeField]
		protected Material m_fontMaterial;

		[SerializeField]
		protected Material[] m_fontMaterials;

		protected bool m_isMaterialDirty;

		[SerializeField]
		protected Color32 m_fontColor32 = Color.white;

		[SerializeField]
		protected Color m_fontColor = Color.white;

		protected static Color32 s_colorWhite = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		protected Color32 m_underlineColor = TMP_Text.s_colorWhite;

		protected Color32 m_strikethroughColor = TMP_Text.s_colorWhite;

		internal HighlightState m_HighlightState = new HighlightState(TMP_Text.s_colorWhite, TMP_Offset.zero);

		internal bool m_ConvertToLinearSpace;

		[SerializeField]
		protected bool m_enableVertexGradient;

		[SerializeField]
		protected ColorMode m_colorMode = ColorMode.FourCornersGradient;

		[SerializeField]
		protected VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		[SerializeField]
		protected TMP_ColorGradient m_fontColorGradientPreset;

		[SerializeField]
		protected TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		protected bool m_tintAllSprites;

		protected bool m_tintSprite;

		protected Color32 m_spriteColor;

		[SerializeField]
		protected TMP_StyleSheet m_StyleSheet;

		internal TMP_Style m_TextStyle;

		[SerializeField]
		protected int m_TextStyleHashCode;

		[SerializeField]
		protected bool m_overrideHtmlColors;

		[SerializeField]
		protected Color32 m_faceColor = Color.white;

		[SerializeField]
		protected Color32 m_outlineColor = Color.black;

		protected float m_outlineWidth;

		protected Vector3 m_currentEnvMapRotation;

		protected bool m_hasEnvMapProperty;

		[SerializeField]
		protected float m_fontSize = -99f;

		protected float m_currentFontSize;

		[SerializeField]
		protected float m_fontSizeBase = 36f;

		protected TMP_TextProcessingStack<float> m_sizeStack = new TMP_TextProcessingStack<float>(16);

		[SerializeField]
		protected FontWeight m_fontWeight = FontWeight.Regular;

		protected FontWeight m_FontWeightInternal = FontWeight.Regular;

		protected TMP_TextProcessingStack<FontWeight> m_FontWeightStack = new TMP_TextProcessingStack<FontWeight>(8);

		[SerializeField]
		protected bool m_enableAutoSizing;

		protected float m_maxFontSize;

		protected float m_minFontSize;

		protected int m_AutoSizeIterationCount;

		protected int m_AutoSizeMaxIterationCount = 100;

		protected bool m_IsAutoSizePointSizeSet;

		[SerializeField]
		protected float m_fontSizeMin;

		[SerializeField]
		protected float m_fontSizeMax;

		[SerializeField]
		protected FontStyles m_fontStyle;

		protected FontStyles m_FontStyleInternal;

		protected TMP_FontStyleStack m_fontStyleStack;

		protected bool m_isUsingBold;

		[SerializeField]
		protected HorizontalAlignmentOptions m_HorizontalAlignment = HorizontalAlignmentOptions.Left;

		[SerializeField]
		protected VerticalAlignmentOptions m_VerticalAlignment = VerticalAlignmentOptions.Top;

		[SerializeField]
		[FormerlySerializedAs("m_lineJustification")]
		protected TextAlignmentOptions m_textAlignment = TextAlignmentOptions.Converted;

		protected HorizontalAlignmentOptions m_lineJustification;

		protected TMP_TextProcessingStack<HorizontalAlignmentOptions> m_lineJustificationStack = new TMP_TextProcessingStack<HorizontalAlignmentOptions>(new HorizontalAlignmentOptions[16]);

		protected Vector3[] m_textContainerLocalCorners = new Vector3[4];

		[SerializeField]
		protected float m_characterSpacing;

		protected float m_cSpacing;

		protected float m_monoSpacing;

		protected bool m_duoSpace;

		[SerializeField]
		private protected float m_characterHorizontalScale = 1f;

		[SerializeField]
		protected float m_wordSpacing;

		[SerializeField]
		protected float m_lineSpacing;

		protected float m_lineSpacingDelta;

		protected float m_lineHeight = -32767f;

		protected bool m_IsDrivenLineSpacing;

		[SerializeField]
		protected float m_lineSpacingMax;

		[SerializeField]
		protected float m_paragraphSpacing;

		[SerializeField]
		protected float m_charWidthMaxAdj;

		protected float m_charWidthAdjDelta;

		[SerializeField]
		[FormerlySerializedAs("m_enableWordWrapping")]
		protected TextWrappingModes m_TextWrappingMode;

		protected bool m_isCharacterWrappingEnabled;

		protected bool m_isNonBreakingSpace;

		protected bool m_isIgnoringAlignment;

		[SerializeField]
		protected float m_wordWrappingRatios = 0.4f;

		[SerializeField]
		protected TextOverflowModes m_overflowMode;

		[SerializeField]
		protected int m_firstOverflowCharacterIndex = -1;

		[SerializeField]
		protected TMP_Text m_linkedTextComponent;

		[SerializeField]
		internal TMP_Text parentLinkedComponent;

		protected bool m_isTextTruncated;

		[SerializeField]
		protected bool m_enableKerning;

		protected int m_LastBaseGlyphIndex;

		[SerializeField]
		protected List<OTL_FeatureTag> m_ActiveFontFeatures = new List<OTL_FeatureTag> { (OTL_FeatureTag)0U };

		[SerializeField]
		protected bool m_enableExtraPadding;

		[SerializeField]
		protected bool checkPaddingRequired;

		[SerializeField]
		protected bool m_isRichText = true;

		[SerializeField]
		private bool m_EmojiFallbackSupport = true;

		[SerializeField]
		protected bool m_parseCtrlCharacters = true;

		protected bool m_isOverlay;

		[SerializeField]
		protected bool m_isOrthographic;

		[SerializeField]
		protected bool m_isCullingEnabled;

		protected bool m_isMaskingEnabled;

		protected bool isMaskUpdateRequired;

		[SerializeField]
		protected bool m_ignoreCulling = true;

		[SerializeField]
		protected TextureMappingOptions m_horizontalMapping;

		[SerializeField]
		protected TextureMappingOptions m_verticalMapping;

		[SerializeField]
		protected float m_uvLineOffset;

		protected TextRenderFlags m_renderMode = TextRenderFlags.Render;

		[SerializeField]
		protected VertexSortingOrder m_geometrySortingOrder;

		[SerializeField]
		protected bool m_IsTextObjectScaleStatic;

		[SerializeField]
		protected bool m_VertexBufferAutoSizeReduction;

		[SerializeField]
		protected int m_firstVisibleCharacter;

		protected int m_maxVisibleCharacters = 99999;

		protected int m_maxVisibleWords = 99999;

		protected int m_maxVisibleLines = 99999;

		[SerializeField]
		protected bool m_useMaxVisibleDescender = true;

		[SerializeField]
		protected int m_pageToDisplay = 1;

		protected bool m_isNewPage;

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

		protected Vector2 m_PreviousRectTransformSize;

		protected Vector2 m_PreviousPivotPosition;

		protected bool m_autoSizeTextContainer;

		protected Mesh m_mesh;

		[SerializeField]
		protected bool m_isVolumetricText;

		protected TMP_SpriteAnimator m_spriteAnimator;

		protected float m_flexibleHeight = -1f;

		protected float m_flexibleWidth = -1f;

		protected float m_minWidth;

		protected float m_minHeight;

		protected float m_maxWidth;

		protected float m_maxHeight;

		protected LayoutElement m_LayoutElement;

		protected float m_preferredWidth;

		protected float m_RenderedWidth;

		protected bool m_isPreferredWidthDirty;

		protected float m_preferredHeight;

		protected float m_RenderedHeight;

		protected bool m_isPreferredHeightDirty;

		protected bool m_isCalculatingPreferredValues;

		protected int m_layoutPriority;

		protected bool m_isLayoutDirty;

		protected bool m_isAwake;

		internal bool m_isWaitingOnResourceLoad;

		internal TMP_Text.TextInputSources m_inputSource;

		protected float m_fontScaleMultiplier;

		private static char[] m_htmlTag = new char[128];

		private static RichTextTagAttribute[] m_xmlAttribute = new RichTextTagAttribute[8];

		private static float[] m_attributeParameterValues = new float[16];

		protected float tag_LineIndent;

		protected float tag_Indent;

		protected TMP_TextProcessingStack<float> m_indentStack = new TMP_TextProcessingStack<float>(new float[16]);

		protected bool tag_NoParsing;

		protected bool m_isTextLayoutPhase;

		protected Quaternion m_FXRotation;

		protected Vector3 m_FXScale;

		internal TMP_Text.TextProcessingElement[] m_TextProcessingArray = new TMP_Text.TextProcessingElement[8];

		internal int m_InternalTextProcessingArraySize;

		private TMP_CharacterInfo[] m_internalCharacterInfo;

		protected int m_totalCharacterCount;

		internal static WordWrapState m_SavedWordWrapState = default(WordWrapState);

		internal static WordWrapState m_SavedLineState = default(WordWrapState);

		internal static WordWrapState m_SavedEllipsisState = default(WordWrapState);

		internal static WordWrapState m_SavedLastValidState = default(WordWrapState);

		internal static WordWrapState m_SavedSoftLineBreakState = default(WordWrapState);

		internal static TMP_TextProcessingStack<WordWrapState> m_EllipsisInsertionCandidateStack = new TMP_TextProcessingStack<WordWrapState>(8, 8);

		protected int m_characterCount;

		protected int m_firstCharacterOfLine;

		protected int m_firstVisibleCharacterOfLine;

		protected int m_lastCharacterOfLine;

		protected int m_lastVisibleCharacterOfLine;

		protected int m_lineNumber;

		protected int m_lineVisibleCharacterCount;

		protected int m_lineVisibleSpaceCount;

		protected int m_pageNumber;

		protected float m_PageAscender;

		protected float m_maxTextAscender;

		protected float m_maxCapHeight;

		protected float m_ElementAscender;

		protected float m_ElementDescender;

		protected float m_maxLineAscender;

		protected float m_maxLineDescender;

		protected float m_startOfLineAscender;

		protected float m_startOfLineDescender;

		protected float m_lineOffset;

		protected Extents m_meshExtents;

		protected Color32 m_htmlColor = new Color(255f, 255f, 255f, 128f);

		protected TMP_TextProcessingStack<Color32> m_colorStack = new TMP_TextProcessingStack<Color32>(new Color32[16]);

		protected TMP_TextProcessingStack<Color32> m_underlineColorStack = new TMP_TextProcessingStack<Color32>(new Color32[16]);

		protected TMP_TextProcessingStack<Color32> m_strikethroughColorStack = new TMP_TextProcessingStack<Color32>(new Color32[16]);

		protected TMP_TextProcessingStack<HighlightState> m_HighlightStateStack = new TMP_TextProcessingStack<HighlightState>(new HighlightState[16]);

		protected TMP_ColorGradient m_colorGradientPreset;

		protected TMP_TextProcessingStack<TMP_ColorGradient> m_colorGradientStack = new TMP_TextProcessingStack<TMP_ColorGradient>(new TMP_ColorGradient[16]);

		protected bool m_colorGradientPresetIsTinted;

		protected float m_tabSpacing;

		protected float m_spacing;

		protected TMP_TextProcessingStack<int>[] m_TextStyleStacks = new TMP_TextProcessingStack<int>[8];

		protected int m_TextStyleStackDepth;

		protected TMP_TextProcessingStack<int> m_ItalicAngleStack = new TMP_TextProcessingStack<int>(new int[16]);

		protected int m_ItalicAngle;

		protected TMP_TextProcessingStack<int> m_actionStack = new TMP_TextProcessingStack<int>(new int[16]);

		protected float m_padding;

		protected float m_baselineOffset;

		protected TMP_TextProcessingStack<float> m_baselineOffsetStack = new TMP_TextProcessingStack<float>(new float[16]);

		protected float m_xAdvance;

		protected TMP_TextElementType m_textElementType;

		protected TMP_TextElement m_cached_TextElement;

		protected TMP_Text.SpecialCharacter m_Ellipsis;

		protected TMP_Text.SpecialCharacter m_Underline;

		protected TMP_SpriteAsset m_defaultSpriteAsset;

		protected TMP_SpriteAsset m_currentSpriteAsset;

		protected int m_spriteCount;

		protected int m_spriteIndex;

		protected int m_spriteAnimationID;

		private static ProfilerMarker k_ParseTextMarker = new ProfilerMarker("TMP Parse Text");

		private static ProfilerMarker k_InsertNewLineMarker = new ProfilerMarker("TMP.InsertNewLine");

		protected bool m_ignoreActiveState;

		private TMP_Text.TextBackingContainer m_TextBackingArray = new TMP_Text.TextBackingContainer(4);

		private readonly decimal[] k_Power = new decimal[] { 0.5m, 0.05m, 0.005m, 0.0005m, 0.00005m, 0.000005m, 0.0000005m, 0.00000005m, 0.000000005m, 0.0000000005m };

		protected static Vector2 k_LargePositiveVector2 = new Vector2(2.1474836E+09f, 2.1474836E+09f);

		protected static Vector2 k_LargeNegativeVector2 = new Vector2(-2.1474836E+09f, -2.1474836E+09f);

		protected static float k_LargePositiveFloat = 32767f;

		protected static float k_LargeNegativeFloat = -32767f;

		protected static int k_LargePositiveInt = int.MaxValue;

		protected static int k_LargeNegativeInt = -2147483647;

		public delegate void MissingCharacterEventCallback(int unicode, int stringIndex, string text, TMP_FontAsset fontAsset, TMP_Text textComponent);

		protected struct CharacterSubstitution
		{
			public CharacterSubstitution(int index, uint unicode)
			{
				this.index = index;
				this.unicode = unicode;
			}

			public int index;

			public uint unicode;
		}

		internal enum TextInputSources
		{
			TextInputBox,
			SetText,
			SetTextArray,
			TextString
		}

		[DebuggerDisplay("Unicode ({unicode})  '{(char)unicode}'")]
		internal struct TextProcessingElement
		{
			public TextProcessingElementType elementType;

			public uint unicode;

			public int stringIndex;

			public int length;
		}

		protected struct SpecialCharacter
		{
			public SpecialCharacter(TMP_Character character, int materialIndex)
			{
				this.character = character;
				this.fontAsset = character.textAsset as TMP_FontAsset;
				this.material = ((this.fontAsset != null) ? this.fontAsset.material : null);
				this.materialIndex = materialIndex;
			}

			public TMP_Character character;

			public TMP_FontAsset fontAsset;

			public Material material;

			public int materialIndex;
		}

		private struct TextBackingContainer
		{
			public uint[] Text
			{
				get
				{
					return this.m_Array;
				}
			}

			public int Capacity
			{
				get
				{
					return this.m_Array.Length;
				}
			}

			public int Count
			{
				get
				{
					return this.m_Index;
				}
				set
				{
					this.m_Index = value;
				}
			}

			public uint this[int index]
			{
				get
				{
					return this.m_Array[index];
				}
				set
				{
					if (index >= this.m_Array.Length)
					{
						this.Resize(index);
					}
					this.m_Array[index] = value;
				}
			}

			public TextBackingContainer(int size)
			{
				this.m_Array = new uint[size];
				this.m_Index = 0;
			}

			public void Resize(int size)
			{
				size = Mathf.NextPowerOfTwo(size + 1);
				Array.Resize<uint>(ref this.m_Array, size);
			}

			private uint[] m_Array;

			private int m_Index;
		}
	}
}
