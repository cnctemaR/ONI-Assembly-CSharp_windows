using System;
using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.UIR
{
	internal class RenderData
	{
		public RenderChainCommand lastTailOrHeadCommand
		{
			get
			{
				return this.lastTailCommand ?? this.lastHeadCommand;
			}
		}

		public static bool AllocatesID(BMPAlloc alloc)
		{
			return alloc.ownedState == OwnedState.Owned && alloc.IsValid();
		}

		public static bool InheritsID(BMPAlloc alloc)
		{
			return alloc.ownedState == OwnedState.Inherited && alloc.IsValid();
		}

		public void Init()
		{
			this.owner = null;
			this.renderTree = null;
			this.parent = null;
			this.nextSibling = null;
			this.prevSibling = null;
			this.firstChild = null;
			this.lastChild = null;
			this.groupTransformAncestor = null;
			this.boneTransformAncestor = null;
			this.prevDirty = null;
			this.nextDirty = null;
			this.flags = RenderDataFlags.IsClippingRectDirty;
			this.depthInRenderTree = 0;
			this.dirtiedValues = RenderDataDirtyTypes.None;
			this.dirtyID = 0U;
			this.firstHeadCommand = null;
			this.lastHeadCommand = null;
			this.firstTailCommand = null;
			this.lastTailCommand = null;
			this.localFlipsWinding = false;
			this.worldFlipsWinding = false;
			this.worldTransformScaleZero = false;
			this.clipMethod = ClipMethod.Undetermined;
			this.childrenStencilRef = 0;
			this.childrenMaskDepth = 0;
			this.headMesh = null;
			this.tailMesh = null;
			this.verticesSpace = Matrix4x4.identity;
			this.transformID = UIRVEShaderInfoAllocator.identityTransform;
			this.clipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
			this.opacityID = UIRVEShaderInfoAllocator.fullOpacity;
			this.colorID = BMPAlloc.Invalid;
			this.backgroundColorID = BMPAlloc.Invalid;
			this.borderLeftColorID = BMPAlloc.Invalid;
			this.borderTopColorID = BMPAlloc.Invalid;
			this.borderRightColorID = BMPAlloc.Invalid;
			this.borderBottomColorID = BMPAlloc.Invalid;
			this.tintColorID = BMPAlloc.Invalid;
			this.textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
			this.compositeOpacity = float.MaxValue;
			this.backgroundAlpha = 0f;
			this.graphicEntries = null;
			this.pendingRepaint = false;
			this.pendingHierarchicalRepaint = false;
			this.clippingRect = Rect.zero;
			this.clippingRectMinusGroup = Rect.zero;
			this.clippingRectIsInfinite = false;
		}

		public void Reset()
		{
			this.owner = null;
			this.renderTree = null;
			this.parent = null;
			this.nextSibling = null;
			this.prevSibling = null;
			this.firstChild = null;
			this.lastChild = null;
			this.groupTransformAncestor = null;
			this.boneTransformAncestor = null;
			this.prevDirty = null;
			this.nextDirty = null;
			this.firstHeadCommand = null;
			this.lastHeadCommand = null;
			this.firstTailCommand = null;
			this.lastTailCommand = null;
			this.headMesh = null;
			this.tailMesh = null;
			this.graphicEntries = null;
		}

		public bool isGroupTransform
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags & RenderDataFlags.IsGroupTransform) == RenderDataFlags.IsGroupTransform;
			}
		}

		public bool isIgnoringDynamicColorHint
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags & RenderDataFlags.IsIgnoringDynamicColorHint) == RenderDataFlags.IsIgnoringDynamicColorHint;
			}
		}

		public bool hasExtraData
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags & RenderDataFlags.HasExtraData) == RenderDataFlags.HasExtraData;
			}
		}

		public bool hasExtraMeshes
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags & RenderDataFlags.HasExtraMeshes) == RenderDataFlags.HasExtraMeshes;
			}
		}

		public bool isSubTreeQuad
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags & RenderDataFlags.IsSubTreeQuad) == RenderDataFlags.IsSubTreeQuad;
			}
		}

		public bool isNestedRenderTreeRoot
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags & RenderDataFlags.IsNestedRenderTreeRoot) == RenderDataFlags.IsNestedRenderTreeRoot;
			}
		}

		public bool isClippingRectDirty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (this.flags & RenderDataFlags.IsClippingRectDirty) == RenderDataFlags.IsClippingRectDirty;
			}
		}

		public Rect clippingRect
		{
			get
			{
				bool isClippingRectDirty = this.isClippingRectDirty;
				if (isClippingRectDirty)
				{
					this.UpdateClippingRect();
					this.flags &= ~RenderDataFlags.IsClippingRectDirty;
				}
				return this.m_ClippingRect;
			}
			set
			{
				this.m_ClippingRect = value;
			}
		}

		public Rect clippingRectMinusGroup
		{
			get
			{
				bool isClippingRectDirty = this.isClippingRectDirty;
				if (isClippingRectDirty)
				{
					this.UpdateClippingRect();
					this.flags &= ~RenderDataFlags.IsClippingRectDirty;
				}
				return this.m_ClippingRectMinusGroup;
			}
			set
			{
				this.m_ClippingRectMinusGroup = value;
			}
		}

		internal bool clippingRectIsInfinite
		{
			get
			{
				bool isClippingRectDirty = this.isClippingRectDirty;
				if (isClippingRectDirty)
				{
					this.UpdateClippingRect();
					this.flags &= ~RenderDataFlags.IsClippingRectDirty;
				}
				return this.m_ClippingRectIsInfinite;
			}
			set
			{
				this.m_ClippingRectIsInfinite = value;
			}
		}

		internal void UpdateClippingRect()
		{
			bool flag = this.parent == null || this.parent.clippingRectIsInfinite;
			bool flag2 = this.parent != null;
			Rect rect;
			Rect rect2;
			if (flag2)
			{
				rect = this.parent.clippingRect;
				bool isGroupTransform = this.parent.isGroupTransform;
				if (isGroupTransform)
				{
					rect2 = DrawParams.k_UnlimitedRect;
					flag = true;
				}
				else
				{
					rect2 = this.parent.clippingRectMinusGroup;
				}
			}
			else
			{
				VisualElement visualElement = this.owner;
				Rect rect3 = ((((visualElement != null) ? visualElement.panel : null) != null) ? this.owner.panel.visualTree.rect : DrawParams.k_UnlimitedRect);
				bool drawInCameras = this.renderTree.renderTreeManager.drawInCameras;
				if (drawInCameras)
				{
					rect3 = DrawParams.k_UnlimitedRect;
				}
				rect2 = rect3;
				rect = rect3;
			}
			bool flag3 = this.owner.ShouldClip();
			if (flag3)
			{
				Rect rect4;
				RenderData.GetLocalClippingRect(this.owner, out rect4);
				bool isGroupTransform2 = this.isGroupTransform;
				if (isGroupTransform2)
				{
					this.m_ClippingRectMinusGroup = Rect.zero;
				}
				else
				{
					bool isNestedRenderTreeRoot = this.isNestedRenderTreeRoot;
					if (isNestedRenderTreeRoot)
					{
						this.m_ClippingRectMinusGroup = rect4;
					}
					else
					{
						Rect rect5 = rect4;
						VisualElement.TransformAlignedRect(this.owner.worldTransformRef, ref rect5);
						bool flag4 = this.groupTransformAncestor != null;
						if (flag4)
						{
							VisualElement.TransformAlignedRect(this.groupTransformAncestor.owner.worldTransformInverse, ref rect5);
						}
						else
						{
							VisualElement.TransformAlignedRect(this.renderTree.rootRenderData.owner.worldTransformInverse, ref rect5);
						}
						this.m_ClippingRectMinusGroup = (flag ? rect5 : RenderData.IntersectClipRects(rect5, rect2));
					}
				}
				VisualElement.TransformAlignedRect(this.owner.worldTransformRef, ref rect4);
				RenderTree renderTree = this.renderTree;
				RenderData rootRenderData = renderTree.rootRenderData;
				bool flag5 = !renderTree.isRootRenderTree;
				if (flag5)
				{
					VisualElement.TransformAlignedRect(rootRenderData.owner.worldTransformInverse, ref rect4);
				}
				this.m_ClippingRect = RenderData.IntersectClipRects(rect4, rect);
			}
			else
			{
				this.m_ClippingRect = rect;
				this.m_ClippingRectMinusGroup = rect2;
				this.m_ClippingRectIsInfinite = flag;
			}
		}

		private static Rect IntersectClipRects(Rect rect, Rect parentRect)
		{
			float num = Mathf.Max(rect.xMin, parentRect.xMin);
			float num2 = Mathf.Min(rect.xMax, parentRect.xMax);
			float num3 = Mathf.Max(rect.yMin, parentRect.yMin);
			float num4 = Mathf.Min(rect.yMax, parentRect.yMax);
			float num5 = Mathf.Max(num2 - num, 0f);
			float num6 = Mathf.Max(num4 - num3, 0f);
			return new Rect(num, num3, num5, num6);
		}

		private static void GetLocalClippingRect(VisualElement owner, out Rect localRect)
		{
			IResolvedStyle resolvedStyle = owner.resolvedStyle;
			localRect = owner.rect;
			localRect.x += resolvedStyle.borderLeftWidth;
			localRect.y += resolvedStyle.borderTopWidth;
			localRect.width -= resolvedStyle.borderLeftWidth + resolvedStyle.borderRightWidth;
			localRect.height -= resolvedStyle.borderTopWidth + resolvedStyle.borderBottomWidth;
			bool flag = owner.computedStyle.unityOverflowClipBox == OverflowClipBox.ContentBox;
			if (flag)
			{
				localRect.x += resolvedStyle.paddingLeft;
				localRect.y += resolvedStyle.paddingTop;
				localRect.width -= resolvedStyle.paddingLeft + resolvedStyle.paddingRight;
				localRect.height -= resolvedStyle.paddingTop + resolvedStyle.paddingBottom;
			}
		}

		public VisualElement owner;

		public RenderTree renderTree;

		public RenderData parent;

		public RenderData prevSibling;

		public RenderData nextSibling;

		public RenderData firstChild;

		public RenderData lastChild;

		public RenderData groupTransformAncestor;

		public RenderData boneTransformAncestor;

		public RenderData prevDirty;

		public RenderData nextDirty;

		public RenderDataFlags flags;

		public int depthInRenderTree;

		public RenderDataDirtyTypes dirtiedValues;

		public uint dirtyID;

		public RenderChainCommand firstHeadCommand;

		public RenderChainCommand lastHeadCommand;

		public RenderChainCommand firstTailCommand;

		public RenderChainCommand lastTailCommand;

		public bool localFlipsWinding;

		public bool worldFlipsWinding;

		public bool worldTransformScaleZero;

		public ClipMethod clipMethod;

		public int childrenStencilRef;

		public int childrenMaskDepth;

		public MeshHandle headMesh;

		public MeshHandle tailMesh;

		public Matrix4x4 verticesSpace;

		public BMPAlloc transformID;

		public BMPAlloc clipRectID;

		public BMPAlloc opacityID;

		public BMPAlloc textCoreSettingsID;

		public BMPAlloc colorID;

		public BMPAlloc backgroundColorID;

		public BMPAlloc borderLeftColorID;

		public BMPAlloc borderTopColorID;

		public BMPAlloc borderRightColorID;

		public BMPAlloc borderBottomColorID;

		public BMPAlloc tintColorID;

		public float compositeOpacity;

		public float backgroundAlpha;

		public BasicNode<GraphicEntry> graphicEntries;

		public bool pendingRepaint;

		public bool pendingHierarchicalRepaint;

		private Rect m_ClippingRect;

		private Rect m_ClippingRectMinusGroup;

		private bool m_ClippingRectIsInfinite;
	}
}
