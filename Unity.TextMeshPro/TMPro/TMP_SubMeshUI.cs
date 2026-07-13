using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace TMPro
{
	[ExecuteAlways]
	[RequireComponent(typeof(CanvasRenderer))]
	public class TMP_SubMeshUI : MaskableGraphic
	{
		public TMP_FontAsset fontAsset
		{
			get
			{
				return this.m_fontAsset;
			}
			set
			{
				this.m_fontAsset = value;
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

		public override Texture mainTexture
		{
			get
			{
				if (this.sharedMaterial != null)
				{
					return this.sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex);
				}
				return null;
			}
		}

		public override Material material
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
				this.m_material = value;
				this.m_sharedMaterial = value;
				this.m_padding = this.GetPaddingForMaterial();
				this.SetVerticesDirty();
				this.SetMaterialDirty();
			}
		}

		public Material sharedMaterial
		{
			get
			{
				return this.m_sharedMaterial;
			}
			set
			{
				this.SetSharedMaterial(value);
			}
		}

		public Material fallbackMaterial
		{
			get
			{
				return this.m_fallbackMaterial;
			}
			set
			{
				if (this.m_fallbackMaterial == value)
				{
					return;
				}
				if (this.m_fallbackMaterial != null && this.m_fallbackMaterial != value)
				{
					TMP_MaterialManager.ReleaseFallbackMaterial(this.m_fallbackMaterial);
				}
				this.m_fallbackMaterial = value;
				TMP_MaterialManager.AddFallbackMaterialReference(this.m_fallbackMaterial);
				this.SetSharedMaterial(this.m_fallbackMaterial);
			}
		}

		public Material fallbackSourceMaterial
		{
			get
			{
				return this.m_fallbackSourceMaterial;
			}
			set
			{
				this.m_fallbackSourceMaterial = value;
			}
		}

		public override Material materialForRendering
		{
			get
			{
				return TMP_MaterialManager.GetMaterialForRendering(this, this.m_sharedMaterial);
			}
		}

		public bool isDefaultMaterial
		{
			get
			{
				return this.m_isDefaultMaterial;
			}
			set
			{
				this.m_isDefaultMaterial = value;
			}
		}

		public float padding
		{
			get
			{
				return this.m_padding;
			}
			set
			{
				this.m_padding = value;
			}
		}

		public Mesh mesh
		{
			get
			{
				if (this.m_mesh == null)
				{
					this.m_mesh = new Mesh();
					this.m_mesh.hideFlags = HideFlags.HideAndDontSave;
				}
				return this.m_mesh;
			}
			set
			{
				this.m_mesh = value;
			}
		}

		public TMP_Text textComponent
		{
			get
			{
				if (this.m_TextComponent == null)
				{
					this.m_TextComponent = base.GetComponentInParent<TextMeshProUGUI>();
				}
				return this.m_TextComponent;
			}
		}

		public static TMP_SubMeshUI AddSubTextObject(TextMeshProUGUI textComponent, MaterialReference materialReference)
		{
			GameObject gameObject = new GameObject();
			gameObject.hideFlags = (TMP_Settings.hideSubTextObjects ? HideFlags.HideAndDontSave : HideFlags.DontSave);
			gameObject.transform.SetParent(textComponent.transform, false);
			gameObject.transform.SetAsFirstSibling();
			gameObject.layer = textComponent.gameObject.layer;
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.sizeDelta = Vector2.zero;
			rectTransform.pivot = textComponent.rectTransform.pivot;
			gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
			TMP_SubMeshUI tmp_SubMeshUI = gameObject.AddComponent<TMP_SubMeshUI>();
			tmp_SubMeshUI.m_TextComponent = textComponent;
			tmp_SubMeshUI.m_materialReferenceIndex = materialReference.index;
			tmp_SubMeshUI.m_fontAsset = materialReference.fontAsset;
			tmp_SubMeshUI.m_spriteAsset = materialReference.spriteAsset;
			tmp_SubMeshUI.m_isDefaultMaterial = materialReference.isDefaultMaterial;
			tmp_SubMeshUI.SetSharedMaterial(materialReference.material);
			if (!textComponent.maskable)
			{
				tmp_SubMeshUI.maskable = false;
				tmp_SubMeshUI.RecalculateClipping();
			}
			return tmp_SubMeshUI;
		}

		protected override void OnEnable()
		{
			if (!this.m_isRegisteredForEvents)
			{
				this.m_isRegisteredForEvents = true;
			}
			if (base.hideFlags != HideFlags.DontSave)
			{
				base.hideFlags = HideFlags.DontSave;
			}
			this.m_ShouldRecalculateStencil = true;
			this.RecalculateClipping();
			this.RecalculateMasking();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.m_fallbackMaterial != null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(this.m_fallbackMaterial);
				this.m_fallbackMaterial = null;
			}
		}

		protected override void OnDestroy()
		{
			if (this.m_mesh != null)
			{
				global::UnityEngine.Object.DestroyImmediate(this.m_mesh);
			}
			if (this.m_MaskMaterial != null)
			{
				TMP_MaterialManager.ReleaseStencilMaterial(this.m_MaskMaterial);
			}
			if (this.m_fallbackMaterial != null)
			{
				TMP_MaterialManager.ReleaseFallbackMaterial(this.m_fallbackMaterial);
				this.m_fallbackMaterial = null;
			}
			this.m_isRegisteredForEvents = false;
			this.RecalculateClipping();
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.havePropertiesChanged = true;
				this.m_TextComponent.SetAllDirty();
			}
		}

		protected override void OnTransformParentChanged()
		{
			if (!this.IsActive())
			{
				return;
			}
			this.m_ShouldRecalculateStencil = true;
			this.RecalculateClipping();
			this.RecalculateMasking();
		}

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			Material material = baseMaterial;
			if (this.m_ShouldRecalculateStencil)
			{
				Transform transform = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
				this.m_StencilValue = (base.maskable ? MaskUtilities.GetStencilDepth(base.transform, transform) : 0);
				this.m_ShouldRecalculateStencil = false;
			}
			if (this.m_StencilValue > 0)
			{
				Material material2 = StencilMaterial.Add(material, (1 << this.m_StencilValue) - 1, StencilOp.Keep, CompareFunction.Equal, ColorWriteMask.All, (1 << this.m_StencilValue) - 1, 0);
				StencilMaterial.Remove(this.m_MaskMaterial);
				this.m_MaskMaterial = material2;
				material = this.m_MaskMaterial;
			}
			return material;
		}

		public float GetPaddingForMaterial()
		{
			return ShaderUtilities.GetPadding(this.m_sharedMaterial, this.m_TextComponent.extraPadding, this.m_TextComponent.isUsingBold);
		}

		public float GetPaddingForMaterial(Material mat)
		{
			return ShaderUtilities.GetPadding(mat, this.m_TextComponent.extraPadding, this.m_TextComponent.isUsingBold);
		}

		public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
		{
			this.m_padding = ShaderUtilities.GetPadding(this.m_sharedMaterial, isExtraPadding, isUsingBold);
		}

		public override void SetAllDirty()
		{
		}

		public override void SetVerticesDirty()
		{
		}

		public override void SetLayoutDirty()
		{
		}

		public override void SetMaterialDirty()
		{
			this.m_materialDirty = true;
			this.UpdateMaterial();
			if (this.m_OnDirtyMaterialCallback != null)
			{
				this.m_OnDirtyMaterialCallback();
			}
		}

		public void SetPivotDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			base.rectTransform.pivot = this.m_TextComponent.rectTransform.pivot;
		}

		private Transform GetRootCanvasTransform()
		{
			if (this.m_RootCanvasTransform == null)
			{
				this.m_RootCanvasTransform = this.m_TextComponent.canvas.rootCanvas.transform;
			}
			return this.m_RootCanvasTransform;
		}

		public override void Cull(Rect clipRect, bool validRect)
		{
		}

		protected override void UpdateGeometry()
		{
		}

		public override void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.PreRender)
			{
				if (!this.m_materialDirty)
				{
					return;
				}
				this.UpdateMaterial();
				this.m_materialDirty = false;
			}
		}

		public void RefreshMaterial()
		{
			this.UpdateMaterial();
		}

		protected override void UpdateMaterial()
		{
			if (this.m_sharedMaterial == null)
			{
				return;
			}
			if (this.m_sharedMaterial.HasProperty(ShaderUtilities.ShaderTag_CullMode) && this.textComponent.fontSharedMaterial != null)
			{
				float @float = this.textComponent.fontSharedMaterial.GetFloat(ShaderUtilities.ShaderTag_CullMode);
				this.m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_CullMode, @float);
			}
			base.canvasRenderer.materialCount = 1;
			base.canvasRenderer.SetMaterial(this.materialForRendering, 0);
		}

		public override void RecalculateClipping()
		{
			base.RecalculateClipping();
		}

		private Material GetMaterial()
		{
			return this.m_sharedMaterial;
		}

		private Material GetMaterial(Material mat)
		{
			if (this.m_material == null || this.m_material.GetInstanceID() != mat.GetInstanceID())
			{
				this.m_material = this.CreateMaterialInstance(mat);
			}
			this.m_sharedMaterial = this.m_material;
			this.m_padding = this.GetPaddingForMaterial();
			this.SetVerticesDirty();
			this.SetMaterialDirty();
			return this.m_sharedMaterial;
		}

		private Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			material.name += " (Instance)";
			return material;
		}

		private Material GetSharedMaterial()
		{
			return base.canvasRenderer.GetMaterial();
		}

		private void SetSharedMaterial(Material mat)
		{
			this.m_sharedMaterial = mat;
			this.m_Material = this.m_sharedMaterial;
			this.m_padding = this.GetPaddingForMaterial();
			this.SetMaterialDirty();
		}

		[SerializeField]
		private TMP_FontAsset m_fontAsset;

		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		private Material m_material;

		[SerializeField]
		private Material m_sharedMaterial;

		private Material m_fallbackMaterial;

		private Material m_fallbackSourceMaterial;

		[SerializeField]
		private bool m_isDefaultMaterial;

		[SerializeField]
		private float m_padding;

		private Mesh m_mesh;

		[SerializeField]
		private TextMeshProUGUI m_TextComponent;

		[NonSerialized]
		private bool m_isRegisteredForEvents;

		private bool m_materialDirty;

		[SerializeField]
		private int m_materialReferenceIndex;

		private Transform m_RootCanvasTransform;
	}
}
