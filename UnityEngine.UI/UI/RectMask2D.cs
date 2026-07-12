using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Rect Mask 2D", 14)]
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class RectMask2D : UIBehaviour, IClipper, ICanvasRaycastFilter
	{
		public Vector4 padding
		{
			get
			{
				return this.m_Padding;
			}
			set
			{
				this.m_Padding = value;
				MaskUtilities.Notify2DMaskStateChanged(this);
			}
		}

		public Vector2Int softness
		{
			get
			{
				return this.m_Softness;
			}
			set
			{
				this.m_Softness.x = Mathf.Max(0, value.x);
				this.m_Softness.y = Mathf.Max(0, value.y);
				MaskUtilities.Notify2DMaskStateChanged(this);
			}
		}

		internal Canvas Canvas
		{
			get
			{
				if (this.m_Canvas == null)
				{
					List<Canvas> list = CollectionPool<List<Canvas>, Canvas>.Get();
					base.gameObject.GetComponentsInParent<Canvas>(false, list);
					if (list.Count > 0)
					{
						this.m_Canvas = list[list.Count - 1];
					}
					else
					{
						this.m_Canvas = null;
					}
					CollectionPool<List<Canvas>, Canvas>.Release(list);
				}
				return this.m_Canvas;
			}
		}

		public Rect canvasRect
		{
			get
			{
				return this.m_VertexClipper.GetCanvasRect(this.rectTransform, this.Canvas);
			}
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

		protected RectMask2D()
		{
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_ShouldRecalculateClipRects = true;
			ClipperRegistry.Register(this);
			MaskUtilities.Notify2DMaskStateChanged(this);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.m_ClipTargets.Clear();
			this.m_MaskableTargets.Clear();
			this.m_Clippers.Clear();
			ClipperRegistry.Disable(this);
			MaskUtilities.Notify2DMaskStateChanged(this);
		}

		protected override void OnDestroy()
		{
			ClipperRegistry.Unregister(this);
			base.OnDestroy();
		}

		public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return !base.isActiveAndEnabled || RectTransformUtility.RectangleContainsScreenPoint(this.rectTransform, sp, eventCamera, this.m_Padding);
		}

		private Rect rootCanvasRect
		{
			get
			{
				this.rectTransform.GetWorldCorners(this.m_Corners);
				if (this.Canvas != null)
				{
					Canvas rootCanvas = this.Canvas.rootCanvas;
					for (int i = 0; i < 4; i++)
					{
						this.m_Corners[i] = rootCanvas.transform.InverseTransformPoint(this.m_Corners[i]);
					}
				}
				return new Rect(this.m_Corners[0].x, this.m_Corners[0].y, this.m_Corners[2].x - this.m_Corners[0].x, this.m_Corners[2].y - this.m_Corners[0].y);
			}
		}

		public virtual void PerformClipping()
		{
			if (this.Canvas == null)
			{
				return;
			}
			if (this.m_ShouldRecalculateClipRects)
			{
				MaskUtilities.GetRectMasksForClip(this, this.m_Clippers);
				this.m_ShouldRecalculateClipRects = false;
			}
			bool flag = true;
			Rect rect = Clipping.FindCullAndClipWorldRect(this.m_Clippers, out flag);
			RenderMode renderMode = this.Canvas.rootCanvas.renderMode;
			if ((renderMode == RenderMode.ScreenSpaceCamera || renderMode == RenderMode.ScreenSpaceOverlay) && !rect.Overlaps(this.rootCanvasRect, true))
			{
				rect = Rect.zero;
				flag = false;
			}
			if (rect != this.m_LastClipRectCanvasSpace)
			{
				foreach (IClippable clippable in this.m_ClipTargets)
				{
					clippable.SetClipRect(rect, flag);
				}
				using (HashSet<MaskableGraphic>.Enumerator enumerator2 = this.m_MaskableTargets.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						MaskableGraphic maskableGraphic = enumerator2.Current;
						maskableGraphic.SetClipRect(rect, flag);
						maskableGraphic.Cull(rect, flag);
					}
					goto IL_01B5;
				}
			}
			if (this.m_ForceClip)
			{
				foreach (IClippable clippable2 in this.m_ClipTargets)
				{
					clippable2.SetClipRect(rect, flag);
				}
				using (HashSet<MaskableGraphic>.Enumerator enumerator2 = this.m_MaskableTargets.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						MaskableGraphic maskableGraphic2 = enumerator2.Current;
						maskableGraphic2.SetClipRect(rect, flag);
						if (maskableGraphic2.canvasRenderer.hasMoved)
						{
							maskableGraphic2.Cull(rect, flag);
						}
					}
					goto IL_01B5;
				}
			}
			foreach (MaskableGraphic maskableGraphic3 in this.m_MaskableTargets)
			{
				maskableGraphic3.Cull(rect, flag);
			}
			IL_01B5:
			this.m_LastClipRectCanvasSpace = rect;
			this.m_ForceClip = false;
			this.UpdateClipSoftness();
		}

		public virtual void UpdateClipSoftness()
		{
			if (this.Canvas == null)
			{
				return;
			}
			foreach (IClippable clippable in this.m_ClipTargets)
			{
				clippable.SetClipSoftness(this.m_Softness);
			}
			foreach (MaskableGraphic maskableGraphic in this.m_MaskableTargets)
			{
				maskableGraphic.SetClipSoftness(this.m_Softness);
			}
		}

		public void AddClippable(IClippable clippable)
		{
			if (clippable == null)
			{
				return;
			}
			this.m_ShouldRecalculateClipRects = true;
			MaskableGraphic maskableGraphic = clippable as MaskableGraphic;
			if (maskableGraphic == null)
			{
				this.m_ClipTargets.Add(clippable);
			}
			else
			{
				this.m_MaskableTargets.Add(maskableGraphic);
			}
			this.m_ForceClip = true;
		}

		public void RemoveClippable(IClippable clippable)
		{
			if (clippable == null)
			{
				return;
			}
			this.m_ShouldRecalculateClipRects = true;
			clippable.SetClipRect(default(Rect), false);
			MaskableGraphic maskableGraphic = clippable as MaskableGraphic;
			if (maskableGraphic == null)
			{
				this.m_ClipTargets.Remove(clippable);
			}
			else
			{
				this.m_MaskableTargets.Remove(maskableGraphic);
			}
			this.m_ForceClip = true;
		}

		protected override void OnTransformParentChanged()
		{
			this.m_Canvas = null;
			base.OnTransformParentChanged();
			this.m_ShouldRecalculateClipRects = true;
		}

		protected override void OnCanvasHierarchyChanged()
		{
			this.m_Canvas = null;
			base.OnCanvasHierarchyChanged();
			this.m_ShouldRecalculateClipRects = true;
		}

		[NonSerialized]
		private readonly RectangularVertexClipper m_VertexClipper = new RectangularVertexClipper();

		[NonSerialized]
		private RectTransform m_RectTransform;

		[NonSerialized]
		private HashSet<MaskableGraphic> m_MaskableTargets = new HashSet<MaskableGraphic>();

		[NonSerialized]
		private HashSet<IClippable> m_ClipTargets = new HashSet<IClippable>();

		[NonSerialized]
		private bool m_ShouldRecalculateClipRects;

		[NonSerialized]
		private List<RectMask2D> m_Clippers = new List<RectMask2D>();

		[NonSerialized]
		private Rect m_LastClipRectCanvasSpace;

		[NonSerialized]
		private bool m_ForceClip;

		[SerializeField]
		private Vector4 m_Padding;

		[SerializeField]
		private Vector2Int m_Softness;

		[NonSerialized]
		private Canvas m_Canvas;

		private Vector3[] m_Corners = new Vector3[4];
	}
}
