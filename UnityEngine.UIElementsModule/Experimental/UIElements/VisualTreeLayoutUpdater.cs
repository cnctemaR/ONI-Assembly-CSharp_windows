using System;
using UnityEngine.Yoga;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualTreeLayoutUpdater : BaseVisualTreeUpdater
	{
		public override string description
		{
			get
			{
				return "Update Layout";
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Layout)) != (VersionChangeType)0)
			{
				YogaNode yogaNode = ve.yogaNode;
				if (yogaNode != null && yogaNode.IsMeasureDefined)
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
				if (num > 0)
				{
					base.panel.ApplyStyles();
				}
				base.visualTree.yogaNode.CalculateLayout(float.NaN, float.NaN);
				using (new EventDispatcher.Gate(base.visualTree.panel.dispatcher))
				{
					this.UpdateSubTree(base.visualTree);
				}
				if (num++ >= 5)
				{
					Debug.LogError("Layout update is struggling to process current layout (consider simplifying to avoid recursive layout): " + base.visualTree);
					break;
				}
			}
		}

		private void UpdateSubTree(VisualElement root)
		{
			Rect rect = new Rect(root.yogaNode.LayoutX, root.yogaNode.LayoutY, root.yogaNode.LayoutWidth, root.yogaNode.LayoutHeight);
			Rect lastLayout = root.renderData.lastLayout;
			bool flag = lastLayout != rect;
			if (flag)
			{
				if (rect.position != lastLayout.position)
				{
					root.IncrementVersion(VersionChangeType.Transform);
				}
				root.IncrementVersion(VersionChangeType.Clip);
				root.renderData.lastLayout = rect;
			}
			bool hasNewLayout = root.yogaNode.HasNewLayout;
			if (hasNewLayout)
			{
				for (int i = 0; i < root.shadow.childCount; i++)
				{
					this.UpdateSubTree(root.shadow[i]);
				}
			}
			if (flag)
			{
				using (GeometryChangedEvent pooled = GeometryChangedEvent.GetPooled(lastLayout, rect))
				{
					pooled.target = root;
					root.SendEvent(pooled);
				}
			}
			if (hasNewLayout)
			{
				root.yogaNode.MarkLayoutSeen();
				root.IncrementVersion(VersionChangeType.Repaint);
			}
		}

		private const int kMaxValidateLayoutCount = 5;
	}
}
