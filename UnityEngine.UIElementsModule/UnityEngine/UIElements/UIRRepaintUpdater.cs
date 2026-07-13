using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class UIRRepaintUpdater : BaseVisualTreeUpdater, IPanelRenderer
	{
		public UIRRepaintUpdater()
		{
			base.panelChanged += this.OnPanelChanged;
		}

		public override ProfilerMarker profilerMarker
		{
			get
			{
				return UIRRepaintUpdater.s_ProfilerMarker;
			}
		}

		public bool forceGammaRendering
		{
			get
			{
				return this.m_ForceGammaRendering;
			}
			set
			{
				bool flag = this.m_ForceGammaRendering == value;
				if (!flag)
				{
					this.m_ForceGammaRendering = value;
					this.DestroyRenderChain();
				}
			}
		}

		public uint vertexBudget
		{
			get
			{
				return this.m_VertexBudget;
			}
			set
			{
				bool flag = this.m_VertexBudget == value;
				if (!flag)
				{
					this.m_VertexBudget = value;
					this.DestroyRenderChain();
				}
			}
		}

		public TextureSlotCount textureSlotCount
		{
			get
			{
				return this.m_TextureSlotCount;
			}
			set
			{
				this.m_TextureSlotCount = value;
			}
		}

		public bool drawStats { get; set; }

		public bool breakBatches { get; set; }

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = this.renderTreeManager == null;
			if (!flag)
			{
				bool flag2 = (versionChangeType & VersionChangeType.Transform) > (VersionChangeType)0;
				bool flag3 = (versionChangeType & VersionChangeType.Size) > (VersionChangeType)0;
				bool flag4 = (versionChangeType & VersionChangeType.Overflow) > (VersionChangeType)0;
				bool flag5 = (versionChangeType & VersionChangeType.BorderRadius) > (VersionChangeType)0;
				bool flag6 = (versionChangeType & VersionChangeType.BorderWidth) > (VersionChangeType)0;
				bool flag7 = (versionChangeType & VersionChangeType.RenderHints) > (VersionChangeType)0;
				bool flag8 = (versionChangeType & VersionChangeType.DisableRendering) > (VersionChangeType)0;
				bool flag9 = (versionChangeType & VersionChangeType.Repaint) > (VersionChangeType)0;
				bool flag10 = false;
				bool flag11 = ve.renderData != null;
				if (flag11)
				{
					flag10 = (ve.useRenderTexture && (ve.renderData.flags & RenderDataFlags.IsSubTreeQuad) == (RenderDataFlags)0) || (!ve.useRenderTexture && (ve.renderData.flags & RenderDataFlags.IsSubTreeQuad) > (RenderDataFlags)0);
				}
				bool flag12 = flag7 || flag10;
				if (flag12)
				{
					this.renderTreeManager.UIEOnRenderHintsChanged(ve);
				}
				bool flag13 = flag2 || flag3 || flag6;
				if (flag13)
				{
					this.renderTreeManager.UIEOnTransformOrSizeChanged(ve, flag2, flag3 || flag6);
				}
				bool flag14 = flag4 || flag5;
				if (flag14)
				{
					this.renderTreeManager.UIEOnClippingChanged(ve, false);
				}
				bool flag15 = (versionChangeType & VersionChangeType.Opacity) > (VersionChangeType)0;
				if (flag15)
				{
					this.renderTreeManager.UIEOnOpacityChanged(ve, false);
				}
				bool flag16 = (versionChangeType & VersionChangeType.Color) > (VersionChangeType)0;
				if (flag16)
				{
					this.renderTreeManager.UIEOnColorChanged(ve);
				}
				bool flag17 = flag9;
				if (flag17)
				{
					this.renderTreeManager.UIEOnVisualsChanged(ve, false);
				}
				bool flag18 = flag8 && !flag9;
				if (flag18)
				{
					this.renderTreeManager.UIEOnDisableRenderingChanged(ve);
				}
			}
		}

		public override void Update()
		{
			bool flag = this.renderTreeManager == null;
			if (flag)
			{
				this.InitRenderChain();
			}
			bool flag2 = this.renderTreeManager == null || this.renderTreeManager.device == null;
			if (!flag2)
			{
				this.renderTreeManager.ProcessChanges();
				this.renderTreeManager.drawStats = this.drawStats;
				this.renderTreeManager.device.breakBatches = this.breakBatches;
				this.renderTreeManager.textureSlotCount = this.textureSlotCount;
			}
		}

		public void Render()
		{
			bool flag = this.renderTreeManager == null;
			if (!flag)
			{
				Debug.Assert(!this.renderTreeManager.drawInCameras);
				this.renderTreeManager.RenderRootTree();
			}
		}

		protected virtual RenderTreeManager CreateRenderChain()
		{
			return new RenderTreeManager(base.panel);
		}

		static UIRRepaintUpdater()
		{
			Utility.GraphicsResourcesRecreate += UIRRepaintUpdater.OnGraphicsResourcesRecreate;
		}

		private static void OnGraphicsResourcesRecreate(bool recreate)
		{
			bool flag = !recreate;
			if (flag)
			{
				UIRenderDevice.PrepareForGfxDeviceRecreate();
			}
			Dictionary<int, Panel>.Enumerator panelsIterator = UIElementsUtility.GetPanelsIterator();
			while (panelsIterator.MoveNext())
			{
				if (recreate)
				{
					KeyValuePair<int, Panel> keyValuePair = panelsIterator.Current;
					AtlasBase atlas = keyValuePair.Value.atlas;
					if (atlas != null)
					{
						atlas.Reset();
					}
				}
				else
				{
					KeyValuePair<int, Panel> keyValuePair = panelsIterator.Current;
					keyValuePair.Value.panelRenderer.Reset();
				}
			}
			bool flag2 = !recreate;
			if (flag2)
			{
				UIRenderDevice.FlushAllPendingDeviceDisposes();
			}
			else
			{
				UIRenderDevice.WrapUpGfxDeviceRecreate();
			}
		}

		private void OnPanelChanged(BaseVisualElementPanel obj)
		{
			this.DetachFromPanel();
			this.AttachToPanel();
		}

		private void AttachToPanel()
		{
			Debug.Assert(this.attachedPanel == null);
			bool flag = base.panel == null;
			if (!flag)
			{
				this.attachedPanel = base.panel;
				this.attachedPanel.isFlatChanged += this.OnPanelIsFlatChanged;
				this.attachedPanel.atlasChanged += this.OnPanelAtlasChanged;
				this.attachedPanel.hierarchyChanged += this.OnPanelHierarchyChanged;
				Debug.Assert(this.attachedPanel.panelRenderer == null);
				this.attachedPanel.panelRenderer = this;
				BaseRuntimePanel baseRuntimePanel = base.panel as BaseRuntimePanel;
				bool flag2 = baseRuntimePanel != null;
				if (flag2)
				{
					baseRuntimePanel.drawsInCamerasChanged += this.OnPanelDrawsInCamerasChanged;
				}
			}
		}

		private void DetachFromPanel()
		{
			bool flag = this.attachedPanel == null;
			if (!flag)
			{
				this.DestroyRenderChain();
				BaseRuntimePanel baseRuntimePanel = base.panel as BaseRuntimePanel;
				bool flag2 = baseRuntimePanel != null;
				if (flag2)
				{
					baseRuntimePanel.drawsInCamerasChanged -= this.OnPanelDrawsInCamerasChanged;
				}
				this.attachedPanel.isFlatChanged -= this.OnPanelIsFlatChanged;
				this.attachedPanel.atlasChanged -= this.OnPanelAtlasChanged;
				this.attachedPanel.hierarchyChanged -= this.OnPanelHierarchyChanged;
				Debug.Assert(this.attachedPanel.panelRenderer == this);
				this.attachedPanel.panelRenderer = null;
				this.attachedPanel = null;
			}
		}

		private void InitRenderChain()
		{
			Debug.Assert(this.attachedPanel != null);
			this.renderTreeManager = this.CreateRenderChain();
			this.renderTreeManager.UIEOnChildAdded(this.attachedPanel.visualTree);
		}

		public void Reset()
		{
			this.DestroyRenderChain();
		}

		private void DestroyRenderChain()
		{
			bool flag = this.renderTreeManager == null;
			if (!flag)
			{
				this.renderTreeManager.Dispose();
				this.renderTreeManager = null;
				this.ResetAllElementsDataRecursive(this.attachedPanel.visualTree);
			}
		}

		private void OnPanelIsFlatChanged()
		{
			this.DestroyRenderChain();
		}

		private void OnPanelAtlasChanged()
		{
			this.DestroyRenderChain();
		}

		private void OnPanelDrawsInCamerasChanged()
		{
			this.DestroyRenderChain();
		}

		private void OnPanelHierarchyChanged(VisualElement ve, HierarchyChangeType changeType, IReadOnlyList<VisualElement> additionalContext = null)
		{
			bool flag = this.renderTreeManager == null;
			if (!flag)
			{
				switch (changeType)
				{
				case HierarchyChangeType.AddedToParent:
					this.renderTreeManager.UIEOnChildAdded(ve);
					break;
				case HierarchyChangeType.RemovedFromParent:
					this.renderTreeManager.UIEOnChildRemoving(ve);
					break;
				case HierarchyChangeType.ChildrenReordered:
					this.renderTreeManager.UIEOnChildrenReordered(ve);
					break;
				}
			}
		}

		private void ResetAllElementsDataRecursive(VisualElement ve)
		{
			ve.renderData = null;
			ve.nestedRenderData = null;
			int i = ve.hierarchy.childCount - 1;
			while (i >= 0)
			{
				this.ResetAllElementsDataRecursive(ve.hierarchy[i--]);
			}
		}

		private protected bool disposed { protected get; private set; }

		protected override void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					this.DetachFromPanel();
				}
				this.disposed = true;
			}
		}

		private BaseVisualElementPanel attachedPanel;

		internal RenderTreeManager renderTreeManager;

		private static readonly string s_Description = "UIElements.UpdateRenderData";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(UIRRepaintUpdater.s_Description);

		private bool m_ForceGammaRendering;

		private uint m_VertexBudget;

		private TextureSlotCount m_TextureSlotCount = TextureSlotCount.Eight;
	}
}
