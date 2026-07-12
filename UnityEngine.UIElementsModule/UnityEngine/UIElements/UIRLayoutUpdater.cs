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
					string text = "Layout update is struggling to process current layout (consider simplifying to avoid recursive layout): ";
					VisualElement visualTree = base.visualTree;
					Debug.LogError(text + ((visualTree != null) ? visualTree.ToString() : null));
					break;
				}
			}
		}

		private void UpdateSubTree(VisualElement ve, int currentLayoutPass)
		{
			Rect rect = new Rect(ve.yogaNode.LayoutX, ve.yogaNode.LayoutY, ve.yogaNode.LayoutWidth, ve.yogaNode.LayoutHeight);
			Rect rect2 = new Rect(ve.yogaNode.LayoutPaddingLeft, ve.yogaNode.LayoutPaddingTop, ve.yogaNode.LayoutWidth - (ve.yogaNode.LayoutPaddingLeft + ve.yogaNode.LayoutPaddingRight), ve.yogaNode.LayoutHeight - (ve.yogaNode.LayoutPaddingTop + ve.yogaNode.LayoutPaddingBottom));
			Rect lastLayout = ve.lastLayout;
			Rect lastPadding = ve.lastPadding;
			VersionChangeType versionChangeType = (VersionChangeType)0;
			bool flag = lastLayout.size != rect.size;
			bool flag2 = lastPadding.size != rect2.size;
			bool flag3 = flag || flag2;
			if (flag3)
			{
				versionChangeType |= VersionChangeType.Size | VersionChangeType.Repaint;
			}
			bool flag4 = rect.position != lastLayout.position;
			bool flag5 = rect2.position != lastPadding.position;
			bool flag6 = flag4 || flag5;
			if (flag6)
			{
				versionChangeType |= VersionChangeType.Transform;
			}
			bool flag7 = versionChangeType > (VersionChangeType)0;
			if (flag7)
			{
				ve.IncrementVersion(versionChangeType);
			}
			ve.lastLayout = rect;
			ve.lastPadding = rect2;
			bool hasNewLayout = ve.yogaNode.HasNewLayout;
			bool flag8 = hasNewLayout;
			if (flag8)
			{
				int childCount = ve.hierarchy.childCount;
				for (int i = 0; i < childCount; i++)
				{
					this.UpdateSubTree(ve.hierarchy[i], currentLayoutPass);
				}
			}
			bool flag9 = flag || flag4;
			if (flag9)
			{
				using (GeometryChangedEvent pooled = GeometryChangedEvent.GetPooled(lastLayout, rect))
				{
					pooled.layoutPass = currentLayoutPass;
					pooled.target = ve;
					ve.SendEvent(pooled);
				}
			}
			bool flag10 = hasNewLayout;
			if (flag10)
			{
				ve.yogaNode.MarkLayoutSeen();
			}
		}

		private const int kMaxValidateLayoutCount = 5;

		private static readonly string s_Description = "UIR Update Layout";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(UIRLayoutUpdater.s_Description);
	}
}
