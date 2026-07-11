using System;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Mask", 13)]
	[ExecuteAlways]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class Mask : UIBehaviour, ICanvasRaycastFilter, IMaterialModifier
	{
		protected Mask()
		{
		}

		public RectTransform rectTransform
		{
			get
			{
				RectTransform rectTransform;
				if ((rectTransform = this.m_RectTransform) == null)
				{
					rectTransform = (this.m_RectTransform = base.GetComponent<RectTransform>());
				}
				return rectTransform;
			}
		}

		public bool showMaskGraphic
		{
			get
			{
				return this.m_ShowMaskGraphic;
			}
			set
			{
				if (this.m_ShowMaskGraphic != value)
				{
					this.m_ShowMaskGraphic = value;
					if (this.graphic != null)
					{
						this.graphic.SetMaterialDirty();
					}
				}
			}
		}

		public Graphic graphic
		{
			get
			{
				Graphic graphic;
				if ((graphic = this.m_Graphic) == null)
				{
					graphic = (this.m_Graphic = base.GetComponent<Graphic>());
				}
				return graphic;
			}
		}

		public virtual bool MaskEnabled()
		{
			return this.IsActive() && this.graphic != null;
		}

		[Obsolete("Not used anymore.")]
		public virtual void OnSiblingGraphicEnabledDisabled()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.graphic != null)
			{
				this.graphic.canvasRenderer.hasPopInstruction = true;
				this.graphic.SetMaterialDirty();
			}
			MaskUtilities.NotifyStencilStateChanged(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.graphic != null)
			{
				this.graphic.SetMaterialDirty();
				this.graphic.canvasRenderer.hasPopInstruction = false;
				this.graphic.canvasRenderer.popMaterialCount = 0;
			}
			StencilMaterial.Remove(this.m_MaskMaterial);
			this.m_MaskMaterial = null;
			StencilMaterial.Remove(this.m_UnmaskMaterial);
			this.m_UnmaskMaterial = null;
			MaskUtilities.NotifyStencilStateChanged(this);
		}

		public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return !base.isActiveAndEnabled || RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, sp, eventCamera);
		}

		public virtual Material GetModifiedMaterial(Material baseMaterial)
		{
			Material material;
			if (!this.MaskEnabled())
			{
				material = baseMaterial;
			}
			else
			{
				Transform transform = MaskUtilities.FindRootSortOverrideCanvas(base.transform);
				int stencilDepth = MaskUtilities.GetStencilDepth(base.transform, transform);
				if (stencilDepth >= 8)
				{
					Debug.LogError("Attempting to use a stencil mask with depth > 8", base.gameObject);
					material = baseMaterial;
				}
				else
				{
					int num = 1 << stencilDepth;
					if (num == 1)
					{
						Material material2 = StencilMaterial.Add(baseMaterial, 1, StencilOp.Replace, CompareFunction.Always, (!this.m_ShowMaskGraphic) ? ((ColorWriteMask)0) : ColorWriteMask.All);
						StencilMaterial.Remove(this.m_MaskMaterial);
						this.m_MaskMaterial = material2;
						Material material3 = StencilMaterial.Add(baseMaterial, 1, StencilOp.Zero, CompareFunction.Always, (ColorWriteMask)0);
						StencilMaterial.Remove(this.m_UnmaskMaterial);
						this.m_UnmaskMaterial = material3;
						this.graphic.canvasRenderer.popMaterialCount = 1;
						this.graphic.canvasRenderer.SetPopMaterial(this.m_UnmaskMaterial, 0);
						material = this.m_MaskMaterial;
					}
					else
					{
						Material material4 = StencilMaterial.Add(baseMaterial, num | (num - 1), StencilOp.Replace, CompareFunction.Equal, (!this.m_ShowMaskGraphic) ? ((ColorWriteMask)0) : ColorWriteMask.All, num - 1, num | (num - 1));
						StencilMaterial.Remove(this.m_MaskMaterial);
						this.m_MaskMaterial = material4;
						this.graphic.canvasRenderer.hasPopInstruction = true;
						Material material5 = StencilMaterial.Add(baseMaterial, num - 1, StencilOp.Replace, CompareFunction.Equal, (ColorWriteMask)0, num - 1, num | (num - 1));
						StencilMaterial.Remove(this.m_UnmaskMaterial);
						this.m_UnmaskMaterial = material5;
						this.graphic.canvasRenderer.popMaterialCount = 1;
						this.graphic.canvasRenderer.SetPopMaterial(this.m_UnmaskMaterial, 0);
						material = this.m_MaskMaterial;
					}
				}
			}
			return material;
		}

		[NonSerialized]
		private RectTransform m_RectTransform;

		[SerializeField]
		private bool m_ShowMaskGraphic = true;

		[NonSerialized]
		private Graphic m_Graphic;

		[NonSerialized]
		private Material m_MaskMaterial;

		[NonSerialized]
		private Material m_UnmaskMaterial;
	}
}
