using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class UIRRepaintUpdater : BaseVisualTreeUpdater
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

		public event Action<UIRenderDevice> BeforeDrawChain
		{
			add
			{
				bool flag = this.renderChain != null;
				if (flag)
				{
					this.renderChain.BeforeDrawChain += value;
				}
			}
			remove
			{
				bool flag = this.renderChain != null;
				if (flag)
				{
					this.renderChain.BeforeDrawChain -= value;
				}
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = this.renderChain == null;
			if (!flag)
			{
				bool flag2 = (versionChangeType & VersionChangeType.Transform) > (VersionChangeType)0;
				bool flag3 = (versionChangeType & VersionChangeType.Size) > (VersionChangeType)0;
				bool flag4 = (versionChangeType & VersionChangeType.Overflow) > (VersionChangeType)0;
				bool flag5 = (versionChangeType & VersionChangeType.BorderRadius) > (VersionChangeType)0;
				bool flag6 = (versionChangeType & VersionChangeType.BorderWidth) > (VersionChangeType)0;
				bool flag7 = flag2 || flag3 || flag6;
				if (flag7)
				{
					this.renderChain.UIEOnTransformOrSizeChanged(ve, flag2, flag3 || flag6);
				}
				bool flag8 = flag4 || flag5;
				if (flag8)
				{
					this.renderChain.UIEOnClippingChanged(ve, false);
				}
				bool flag9 = (versionChangeType & VersionChangeType.Opacity) > (VersionChangeType)0;
				if (flag9)
				{
					this.renderChain.UIEOnOpacityChanged(ve);
				}
				bool flag10 = (versionChangeType & VersionChangeType.Repaint) > (VersionChangeType)0;
				if (flag10)
				{
					this.renderChain.UIEOnVisualsChanged(ve, false);
				}
			}
		}

		public override void Update()
		{
			RenderChain renderChain = this.renderChain;
			bool flag = ((renderChain != null) ? renderChain.device : null) == null;
			if (!flag)
			{
				this.DrawChain(base.panel.GetViewport(), base.panel.GetProjection());
			}
		}

		internal RenderChain DebugGetRenderChain()
		{
			return this.renderChain;
		}

		protected virtual RenderChain CreateRenderChain()
		{
			return new RenderChain(base.panel, base.panel.standardShader);
		}

		protected virtual void DrawChain(Rect viewport, Matrix4x4 projection)
		{
			using (UIRRepaintUpdater.s_MarkerDrawChain.Auto())
			{
				this.renderChain.Render(viewport, projection, base.panel.clearFlags);
			}
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
				KeyValuePair<int, Panel> keyValuePair = panelsIterator.Current;
				UIRRepaintUpdater uirrepaintUpdater = keyValuePair.Value.GetUpdater(VisualTreeUpdatePhase.Repaint) as UIRRepaintUpdater;
				RenderChain renderChain = ((uirrepaintUpdater != null) ? uirrepaintUpdater.renderChain : null);
				if (recreate)
				{
					if (renderChain != null)
					{
						renderChain.AfterRenderDeviceRelease();
					}
				}
				else if (renderChain != null)
				{
					renderChain.BeforeRenderDeviceRelease();
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
			this.DisposeRenderChain();
			bool flag = base.panel != null;
			if (flag)
			{
				this.renderChain = this.CreateRenderChain();
				bool flag2 = base.panel.visualTree != null;
				if (flag2)
				{
					this.renderChain.UIEOnChildAdded(base.panel.visualTree.hierarchy.parent, base.panel.visualTree, (base.panel.visualTree.hierarchy.parent == null) ? 0 : base.panel.visualTree.hierarchy.parent.IndexOf(base.panel.visualTree));
					this.renderChain.UIEOnVisualsChanged(base.panel.visualTree, true);
				}
				base.panel.standardShaderChanged += this.OnPanelStandardShaderChanged;
				base.panel.hierarchyChanged += this.OnPanelHierarchyChanged;
			}
		}

		private void OnPanelHierarchyChanged(VisualElement ve, HierarchyChangeType changeType)
		{
			bool flag = this.renderChain == null || ve.panel == null;
			if (!flag)
			{
				switch (changeType)
				{
				case HierarchyChangeType.Add:
					this.renderChain.UIEOnChildAdded(ve.hierarchy.parent, ve, (ve.hierarchy.parent != null) ? ve.hierarchy.parent.IndexOf(ve) : 0);
					break;
				case HierarchyChangeType.Remove:
					this.renderChain.UIEOnChildRemoving(ve);
					break;
				case HierarchyChangeType.Move:
					this.renderChain.UIEOnChildrenReordered(ve);
					break;
				}
			}
		}

		private void OnPanelStandardShaderChanged()
		{
			bool flag = this.renderChain != null;
			if (flag)
			{
				this.renderChain.UIEOnStandardShaderChanged(base.panel.standardShader);
			}
		}

		private void ResetAllElementsDataRecursive(VisualElement ve)
		{
			ve.renderChainData = default(RenderChainVEData);
			int i = ve.hierarchy.childCount - 1;
			while (i >= 0)
			{
				this.ResetAllElementsDataRecursive(ve.hierarchy[i--]);
			}
		}

		private void DisposeRenderChain()
		{
			bool flag = this.renderChain != null;
			if (flag)
			{
				IPanel panel = this.renderChain.panel;
				this.renderChain.Dispose();
				this.renderChain = null;
				bool flag2 = panel != null;
				if (flag2)
				{
					base.panel.hierarchyChanged -= this.OnPanelHierarchyChanged;
					base.panel.standardShaderChanged -= this.OnPanelStandardShaderChanged;
					this.ResetAllElementsDataRecursive(panel.visualTree);
				}
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
					this.DisposeRenderChain();
				}
				this.disposed = true;
			}
		}

		internal RenderChain renderChain;

		private static ProfilerMarker s_MarkerDrawChain = new ProfilerMarker("DrawChain");

		private static readonly string s_Description = "UIRepaint";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(UIRRepaintUpdater.s_Description);
	}
}
