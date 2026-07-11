using System;
using Unity.Profiling;
using UnityEngine.Yoga;

namespace UnityEngine.UIElements
{
	internal class UIRLayoutUpdater : BaseVisualTreeUpdater
	{
		public override ProfilerMarker profilerMarker
		{
			get
			{
				return UIRLayoutUpdater.s_ProfilerMarker;
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Layout)) == (VersionChangeType)0;
			if (!flag)
			{
				bool flag2 = (versionChangeType & VersionChangeType.Hierarchy) != (VersionChangeType)0 && base.panel.duringLayoutPhase;
				if (flag2)
				{
					throw new InvalidOperationException("Hierarchy change detected while computing layout, this is not supported.");
				}
				YogaNode yogaNode = ve.yogaNode;
				bool flag3 = yogaNode != null && yogaNode.IsMeasureDefined;
				if (flag3)
				{
					yogaNode.MarkDirty();
				}
			}
		}

		public override void Update()
		{
			int num = 0;
			while (base.visualTree.yogaNode.IsDirty)
			{
				bool flag = num > 0;
				if (flag)
				{
					base.panel.ApplyStyles();
				}
				base.panel.duringLayoutPhase = true;
				base.visualTree.yogaNode.CalculateLayout(float.NaN, float.NaN);
				base.panel.duringLayoutPhase = false;
				using (new EventDispatcherGate(base.visualTree.panel.dispatcher))
				{
					this.UpdateSubTree(base.visualTree, num);
				}
				bool flag2 = num++ >= 5;
				if (flag2)
				{
					Debug.LogError("Layout update is struggling to process current layout (consider simplifying to avoid recursive layout): " + base.visualTree);
					break;
				}
			}
		}

		private void UpdateSubTree(VisualElement ve, int currentLayoutPass)
		{
			Rect rect = new Rect(ve.yogaNode.LayoutX, ve.yogaNode.LayoutY, ve.yogaNode.LayoutWidth, ve.yogaNode.LayoutHeight);
			Rect lastLayout = ve.lastLayout;
			bool flag = false;
			VersionChangeType versionChangeType = (VersionChangeType)0;
			bool flag2 = lastLayout.width != rect.width || lastLayout.height != rect.height;
			if (flag2)
			{
				versionChangeType |= VersionChangeType.Size | VersionChangeType.Repaint;
				flag = true;
			}
			bool flag3 = rect.position != lastLayout.position;
			if (flag3)
			{
				versionChangeType |= VersionChangeType.Transform;
				flag = true;
			}
			bool flag4 = versionChangeType > (VersionChangeType)0;
			if (flag4)
			{
				ve.IncrementVersion(versionChangeType);
			}
			ve.lastLayout = rect;
			bool hasNewLayout = ve.yogaNode.HasNewLayout;
			bool flag5 = hasNewLayout;
			if (flag5)
			{
				int childCount = ve.hierarchy.childCount;
				for (int i = 0; i < childCount; i++)
				{
					this.UpdateSubTree(ve.hierarchy[i], currentLayoutPass);
				}
			}
			bool flag6 = flag;
			if (flag6)
			{
				using (GeometryChangedEvent pooled = GeometryChangedEvent.GetPooled(lastLayout, rect))
				{
					pooled.layoutPass = currentLayoutPass;
					pooled.target = ve;
					ve.SendEvent(pooled);
				}
			}
			bool flag7 = hasNewLayout;
			if (flag7)
			{
				ve.yogaNode.MarkLayoutSeen();
			}
		}

		private const int kMaxValidateLayoutCount = 5;

		private static readonly string s_Description = "UIR Update Layout";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(UIRLayoutUpdater.s_Description);
	}
}
