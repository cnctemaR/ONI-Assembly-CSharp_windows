using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI.CoroutineTween;

namespace UnityEngine.UI
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[ExecuteAlways]
	public abstract class Graphic : UIBehaviour, ICanvasElement
	{
		public static Material defaultGraphicMaterial
		{
			get
			{
				if (Graphic.s_DefaultUI == null)
				{
					Graphic.s_DefaultUI = Canvas.GetDefaultCanvasMaterial();
				}
				return Graphic.s_DefaultUI;
			}
		}

		public virtual Color color
		{
			get
			{
				return this.m_Color;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_Color, value))
				{
					this.SetVerticesDirty();
				}
			}
		}

		public virtual bool raycastTarget
		{
			get
			{
				return this.m_RaycastTarget;
			}
			set
			{
				if (value != this.m_RaycastTarget)
				{
					if (this.m_RaycastTarget)
					{
						GraphicRegistry.UnregisterRaycastGraphicForCanvas(this.canvas, this);
					}
					this.m_RaycastTarget = value;
					if (this.m_RaycastTarget && base.isActiveAndEnabled)
					{
						GraphicRegistry.RegisterRaycastGraphicForCanvas(this.canvas, this);
					}
				}
				this.m_RaycastTargetCache = value;
			}
		}

		public Vector4 raycastPadding
		{
			get
			{
				return this.m_RaycastPadding;
			}
			set
			{
				this.m_RaycastPadding = value;
			}
		}

		protected bool useLegacyMeshGeneration { get; set; }

		protected Graphic()
		{
			if (this.m_ColorTweenRunner == null)
			{
				this.m_ColorTweenRunner = new TweenRunner<ColorTween>();
			}
			this.m_ColorTweenRunner.Init(this);
			this.useLegacyMeshGeneration = true;
		}

		public virtual void SetAllDirty()
		{
			if (this.m_SkipLayoutUpdate)
			{
				this.m_SkipLayoutUpdate = false;
			}
			else
			{
				this.SetLayoutDirty();
			}
			if (this.m_SkipMaterialUpdate)
			{
				this.m_SkipMaterialUpdate = false;
			}
			else
			{
				this.SetMaterialDirty();
			}
			this.SetVerticesDirty();
			this.SetRaycastDirty();
		}

		public virtual void SetLayoutDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			if (this.m_OnDirtyLayoutCallback != null)
			{
				this.m_OnDirtyLayoutCallback();
			}
		}

		public virtual void SetVerticesDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			this.m_VertsDirty = true;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			if (this.m_OnDirtyVertsCallback != null)
			{
				this.m_OnDirtyVertsCallback();
			}
		}

		public virtual void SetMaterialDirty()
		{
			if (!this.IsActive())
			{
				return;
			}
			this.m_MaterialDirty = true;
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			if (this.m_OnDirtyMaterialCallback != null)
			{
				this.m_OnDirtyMaterialCallback();
			}
		}

		public void SetRaycastDirty()
		{
			if (this.m_RaycastTargetCache != this.m_RaycastTarget)
			{
				if (this.m_RaycastTarget && base.isActiveAndEnabled)
				{
					GraphicRegistry.RegisterRaycastGraphicForCanvas(this.canvas, this);
				}
				else if (!this.m_RaycastTarget)
				{
					GraphicRegistry.UnregisterRaycastGraphicForCanvas(this.canvas, this);
				}
			}
			this.m_RaycastTargetCache = this.m_RaycastTarget;
		}

		protected override void OnRectTransformDimensionsChange()
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (CanvasUpdateRegistry.IsRebuildingLayout())
				{
					this.SetVerticesDirty();
					return;
				}
				this.SetVerticesDirty();
				this.SetLayoutDirty();
			}
		}

		protected override void OnBeforeTransformParentChanged()
		{
			GraphicRegistry.UnregisterGraphicForCanvas(this.canvas, this);
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.m_Canvas = null;
			if (!this.IsActive())
			{
				return;
			}
			this.CacheCanvas();
			GraphicRegistry.RegisterGraphicForCanvas(this.canvas, this);
			this.SetAllDirty();
		}

		public int depth
		{
			get
			{
				return this.canvasRenderer.absoluteDepth;
			}
		}

		public RectTransform rectTransform
		{
			get
			{
				if (this.m_RectTransform == null)
				{
					this.m_RectTransform = base.GetComponent<RectTransform>();
				}
				return this.m_RectTransform;
			}
		}

		public Canvas canvas
		{
			get
			{
				if (this.m_Canvas == null)
				{
					this.CacheCanvas();
				}
				return this.m_Canvas;
			}
		}

		private void CacheCanvas()
		{
			List<Canvas> list = CollectionPool<List<Canvas>, Canvas>.Get();
			base.gameObject.GetComponentsInParent<Canvas>(false, list);
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].isActiveAndEnabled)
					{
						this.m_Canvas = list[i];
						break;
					}
					if (i == list.Count - 1)
					{
						this.m_Canvas = null;
					}
				}
			}
			else
			{
				this.m_Canvas = null;
			}
			CollectionPool<List<Canvas>, Canvas>.Release(list);
		}

		public CanvasRenderer canvasRenderer
		{
			get
			{
				if (this.m_CanvasRenderer == null)
				{
					this.m_CanvasRenderer = base.GetComponent<CanvasRenderer>();
					if (this.m_CanvasRenderer == null)
					{
						this.m_CanvasRenderer = base.gameObject.AddComponent<CanvasRenderer>();
					}
				}
				return this.m_CanvasRenderer;
			}
		}

		public virtual Material defaultMaterial
		{
			get
			{
				return Graphic.defaultGraphicMaterial;
			}
		}

		public virtual Material material
		{
			get
			{
				if (!(this.m_Material != null))
				{
					return this.defaultMaterial;
				}
				return this.m_Material;
			}
			set
			{
				if (this.m_Material == value)
				{
					return;
				}
				this.m_Material = value;
				this.SetMaterialDirty();
			}
		}

		public virtual Material materialForRendering
		{
			get
			{
				List<IMaterialModifier> list = CollectionPool<List<IMaterialModifier>, IMaterialModifier>.Get();
				base.GetComponents<IMaterialModifier>(list);
				Material material = this.material;
				for (int i = 0; i < list.Count; i++)
				{
					material = list[i].GetModifiedMaterial(material);
				}
				CollectionPool<List<IMaterialModifier>, IMaterialModifier>.Release(list);
				return material;
			}
		}

		public virtual Texture mainTexture
		{
			get
			{
				return Graphic.s_WhiteTexture;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.CacheCanvas();
			GraphicRegistry.RegisterGraphicForCanvas(this.canvas, this);
			if (Graphic.s_WhiteTexture == null)
			{
				Graphic.s_WhiteTexture = Texture2D.whiteTexture;
			}
			this.SetAllDirty();
		}

		protected override void OnDisable()
		{
			GraphicRegistry.DisableGraphicForCanvas(this.canvas, this);
			CanvasUpdateRegistry.DisableCanvasElementForRebuild(this);
			if (this.canvasRenderer != null)
			{
				this.canvasRenderer.Clear();
			}
			LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
			base.OnDisable();
		}

		protected override void OnDestroy()
		{
			GraphicRegistry.UnregisterGraphicForCanvas(this.canvas, this);
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_CachedMesh)
			{
				Object.Destroy(this.m_CachedMesh);
			}
			this.m_CachedMesh = null;
			base.OnDestroy();
		}

		protected override void OnCanvasHierarchyChanged()
		{
			Canvas canvas = this.m_Canvas;
			this.m_Canvas = null;
			if (!this.IsActive())
			{
				GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
				return;
			}
			this.CacheCanvas();
			if (canvas != this.m_Canvas)
			{
				GraphicRegistry.UnregisterGraphicForCanvas(canvas, this);
				if (this.IsActive())
				{
					GraphicRegistry.RegisterGraphicForCanvas(this.canvas, this);
				}
			}
		}

		public virtual void OnCullingChanged()
		{
			if (!this.canvasRenderer.cull && (this.m_VertsDirty || this.m_MaterialDirty))
			{
				CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
			}
		}

		public virtual void Rebuild(CanvasUpdate update)
		{
			if (this.canvasRenderer == null || this.canvasRenderer.cull)
			{
				return;
			}
			if (update == CanvasUpdate.PreRender)
			{
				if (this.m_VertsDirty)
				{
					this.UpdateGeometry();
					this.m_VertsDirty = false;
				}
				if (this.m_MaterialDirty)
				{
					this.UpdateMaterial();
					this.m_MaterialDirty = false;
				}
			}
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		protected virtual void UpdateMaterial()
		{
			if (!this.IsActive())
			{
				return;
			}
			this.canvasRenderer.materialCount = 1;
			this.canvasRenderer.SetMaterial(this.materialForRendering, 0);
			this.canvasRenderer.SetTexture(this.mainTexture);
		}

		protected virtual void UpdateGeometry()
		{
			if (this.useLegacyMeshGeneration)
			{
				this.DoLegacyMeshGeneration();
				return;
			}
			this.DoMeshGeneration();
		}

		private void DoMeshGeneration()
		{
			if (this.rectTransform != null && this.rectTransform.rect.width >= 0f && this.rectTransform.rect.height >= 0f)
			{
				this.OnPopulateMesh(Graphic.s_VertexHelper);
			}
			else
			{
				Graphic.s_VertexHelper.Clear();
			}
			List<Component> list = CollectionPool<List<Component>, Component>.Get();
			base.GetComponents(typeof(IMeshModifier), list);
			for (int i = 0; i < list.Count; i++)
			{
				((IMeshModifier)list[i]).ModifyMesh(Graphic.s_VertexHelper);
			}
			CollectionPool<List<Component>, Component>.Release(list);
			Graphic.s_VertexHelper.FillMesh(Graphic.workerMesh);
			this.canvasRenderer.SetMesh(Graphic.workerMesh);
		}

		private void DoLegacyMeshGeneration()
		{
			if (this.rectTransform != null && this.rectTransform.rect.width >= 0f && this.rectTransform.rect.height >= 0f)
			{
				this.OnPopulateMesh(Graphic.workerMesh);
			}
			else
			{
				Graphic.workerMesh.Clear();
			}
			List<Component> list = CollectionPool<List<Component>, Component>.Get();
			base.GetComponents(typeof(IMeshModifier), list);
			for (int i = 0; i < list.Count; i++)
			{
				((IMeshModifier)list[i]).ModifyMesh(Graphic.workerMesh);
			}
			CollectionPool<List<Component>, Component>.Release(list);
			this.canvasRenderer.SetMesh(Graphic.workerMesh);
		}

		protected static Mesh workerMesh
		{
			get
			{
				if (Graphic.s_Mesh == null)
				{
					Graphic.s_Mesh = new Mesh();
					Graphic.s_Mesh.name = "Shared UI Mesh";
				}
				return Graphic.s_Mesh;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use OnPopulateMesh instead.", true)]
		protected virtual void OnFillVBO(List<UIVertex> vbo)
		{
		}

		[Obsolete("Use OnPopulateMesh(VertexHelper vh) instead.", false)]
		protected virtual void OnPopulateMesh(Mesh m)
		{
			this.OnPopulateMesh(Graphic.s_VertexHelper);
			Graphic.s_VertexHelper.FillMesh(m);
		}

		protected virtual void OnPopulateMesh(VertexHelper vh)
		{
			Rect pixelAdjustedRect = this.GetPixelAdjustedRect();
			Vector4 vector = new Vector4(pixelAdjustedRect.x, pixelAdjustedRect.y, pixelAdjustedRect.x + pixelAdjustedRect.width, pixelAdjustedRect.y + pixelAdjustedRect.height);
			Color32 color = this.color;
			vh.Clear();
			vh.AddVert(new Vector3(vector.x, vector.y), color, new Vector2(0f, 0f));
			vh.AddVert(new Vector3(vector.x, vector.w), color, new Vector2(0f, 1f));
			vh.AddVert(new Vector3(vector.z, vector.w), color, new Vector2(1f, 1f));
			vh.AddVert(new Vector3(vector.z, vector.y), color, new Vector2(1f, 0f));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		protected override void OnDidApplyAnimationProperties()
		{
			this.SetAllDirty();
		}

		public virtual void SetNativeSize()
		{
		}

		public virtual bool Raycast(Vector2 sp, Camera eventCamera)
		{
			return this.Raycast(sp, eventCamera, false);
		}

		protected bool Raycast(Vector2 sp, Camera eventCamera, bool ignoreMasks)
		{
			if (!base.isActiveAndEnabled)
			{
				return false;
			}
			Transform transform = base.transform;
			List<Component> list = CollectionPool<List<Component>, Component>.Get();
			bool flag = false;
			bool flag2 = true;
			bool flag3 = false;
			while (transform != null)
			{
				bool flag4 = true;
				bool flag5 = false;
				bool flag6 = true;
				transform.GetComponents<Component>(list);
				for (int i = 0; i < list.Count; i++)
				{
					Component component = list[i];
					Canvas canvas = component as Canvas;
					if (canvas != null && canvas.overrideSorting)
					{
						flag2 = false;
					}
					ICanvasRaycastFilter canvasRaycastFilter = component as ICanvasRaycastFilter;
					if (canvasRaycastFilter != null && (!ignoreMasks || (!(component is Mask) && !(component is RectMask2D))))
					{
						CanvasGroup canvasGroup = component as CanvasGroup;
						if (canvasGroup != null)
						{
							if (canvasGroup.enabled && !flag)
							{
								if (canvasGroup.ignoreParentGroups)
								{
									flag = true;
								}
								flag4 = canvasRaycastFilter.IsRaycastLocationValid(sp, eventCamera);
								if (!flag4)
								{
									break;
								}
							}
						}
						else
						{
							if (flag3)
							{
								Graphic graphic = component as Graphic;
								if (graphic != null && !graphic.raycastTarget)
								{
									goto IL_0116;
								}
							}
							flag5 |= component is Mask;
							flag4 = canvasRaycastFilter.IsRaycastLocationValid(sp, eventCamera);
							if (!flag4)
							{
								if (!flag3 || !(component is MaskableGraphic))
								{
									break;
								}
								flag6 = flag4;
								if (!ignoreMasks && flag5)
								{
									break;
								}
								flag4 = true;
							}
						}
					}
					IL_0116:;
				}
				if (!flag4 || (flag5 && !flag6))
				{
					CollectionPool<List<Component>, Component>.Release(list);
					return false;
				}
				transform = (flag2 ? transform.parent : null);
				flag3 = true;
			}
			CollectionPool<List<Component>, Component>.Release(list);
			return true;
		}

		public Vector2 PixelAdjustPoint(Vector2 point)
		{
			if (!this.canvas || this.canvas.renderMode == RenderMode.WorldSpace || this.canvas.scaleFactor == 0f || !this.canvas.pixelPerfect)
			{
				return point;
			}
			return RectTransformUtility.PixelAdjustPoint(point, base.transform, this.canvas);
		}

		public Rect GetPixelAdjustedRect()
		{
			if (!this.canvas || this.canvas.renderMode == RenderMode.WorldSpace || this.canvas.scaleFactor == 0f || !this.canvas.pixelPerfect)
			{
				return this.rectTransform.rect;
			}
			return RectTransformUtility.PixelAdjustRect(this.rectTransform, this.canvas);
		}

		public virtual void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
		{
			this.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, true);
		}

		public virtual void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB)
		{
			if (this.canvasRenderer == null || (!useRGB && !useAlpha))
			{
				return;
			}
			if (this.canvasRenderer.GetColor().Equals(targetColor))
			{
				this.m_ColorTweenRunner.StopTween();
				return;
			}
			ColorTween.ColorTweenMode colorTweenMode = ((useRGB && useAlpha) ? ColorTween.ColorTweenMode.All : (useRGB ? ColorTween.ColorTweenMode.RGB : ColorTween.ColorTweenMode.Alpha));
			ColorTween colorTween = new ColorTween
			{
				duration = duration,
				startColor = this.canvasRenderer.GetColor(),
				targetColor = targetColor
			};
			colorTween.AddOnChangedCallback(new UnityAction<Color>(this.canvasRenderer.SetColor));
			colorTween.ignoreTimeScale = ignoreTimeScale;
			colorTween.tweenMode = colorTweenMode;
			this.m_ColorTweenRunner.StartTween(colorTween);
		}

		private static Color CreateColorFromAlpha(float alpha)
		{
			Color black = Color.black;
			black.a = alpha;
			return black;
		}

		public virtual void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
		{
			this.CrossFadeColor(Graphic.CreateColorFromAlpha(alpha), duration, ignoreTimeScale, true, false);
		}

		public void RegisterDirtyLayoutCallback(UnityAction action)
		{
			this.m_OnDirtyLayoutCallback = (UnityAction)Delegate.Combine(this.m_OnDirtyLayoutCallback, action);
		}

		public void UnregisterDirtyLayoutCallback(UnityAction action)
		{
			this.m_OnDirtyLayoutCallback = (UnityAction)Delegate.Remove(this.m_OnDirtyLayoutCallback, action);
		}

		public void RegisterDirtyVerticesCallback(UnityAction action)
		{
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Combine(this.m_OnDirtyVertsCallback, action);
		}

		public void UnregisterDirtyVerticesCallback(UnityAction action)
		{
			this.m_OnDirtyVertsCallback = (UnityAction)Delegate.Remove(this.m_OnDirtyVertsCallback, action);
		}

		public void RegisterDirtyMaterialCallback(UnityAction action)
		{
			this.m_OnDirtyMaterialCallback = (UnityAction)Delegate.Combine(this.m_OnDirtyMaterialCallback, action);
		}

		public void UnregisterDirtyMaterialCallback(UnityAction action)
		{
			this.m_OnDirtyMaterialCallback = (UnityAction)Delegate.Remove(this.m_OnDirtyMaterialCallback, action);
		}

		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		protected static Material s_DefaultUI = null;

		protected static Texture2D s_WhiteTexture = null;

		[FormerlySerializedAs("m_Mat")]
		[SerializeField]
		protected Material m_Material;

		[SerializeField]
		private Color m_Color = Color.white;

		[NonSerialized]
		protected bool m_SkipLayoutUpdate;

		[NonSerialized]
		protected bool m_SkipMaterialUpdate;

		[SerializeField]
		private bool m_RaycastTarget = true;

		private bool m_RaycastTargetCache = true;

		[SerializeField]
		private Vector4 m_RaycastPadding;

		[NonSerialized]
		private RectTransform m_RectTransform;

		[NonSerialized]
		private CanvasRenderer m_CanvasRenderer;

		[NonSerialized]
		private Canvas m_Canvas;

		[NonSerialized]
		private bool m_VertsDirty;

		[NonSerialized]
		private bool m_MaterialDirty;

		[NonSerialized]
		protected UnityAction m_OnDirtyLayoutCallback;

		[NonSerialized]
		protected UnityAction m_OnDirtyVertsCallback;

		[NonSerialized]
		protected UnityAction m_OnDirtyMaterialCallback;

		[NonSerialized]
		protected static Mesh s_Mesh;

		[NonSerialized]
		private static readonly VertexHelper s_VertexHelper = new VertexHelper();

		[NonSerialized]
		protected Mesh m_CachedMesh;

		[NonSerialized]
		protected Vector2[] m_CachedUvs;

		[NonSerialized]
		private readonly TweenRunner<ColorTween> m_ColorTweenRunner;
	}
}
