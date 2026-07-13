using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class VisualTreeLayoutUpdater : BaseVisualTreeUpdater
	{
		public override ProfilerMarker profilerMarker
		{
			get
			{
				return VisualTreeLayoutUpdater.s_ProfilerMarker;
			}
		}

		public unsafe override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = (versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Layout)) == (VersionChangeType)0;
			if (!flag)
			{
				LayoutNode layoutNode = *ve.layoutNode;
				bool flag2 = layoutNode != LayoutNode.Undefined && layoutNode.UsesMeasure;
				if (flag2)
				{
					layoutNode.MarkDirty();
				}
			}
		}

		public override void Update()
		{
			this.m_TextJobSystem.PrepareShapingBeforeLayout(base.panel);
			int num = 0;
			bool isDirty = base.visualTree.layoutNode.IsDirty;
			if (isDirty)
			{
				this.missedHierarchyChangeEventsList.Clear();
				while (base.visualTree.layoutNode.IsDirty)
				{
					this.changeEventsList.Clear();
					bool flag = num > 0;
					if (flag)
					{
						base.panel.ApplyStyles();
						this.m_TextJobSystem.PrepareShapingBeforeLayout(base.panel);
					}
					try
					{
						base.panel.duringLayoutPhase = true;
						base.visualTree.layoutNode.CalculateLayout(float.NaN, float.NaN);
					}
					finally
					{
						base.panel.duringLayoutPhase = false;
					}
					this.UpdateSubTree(base.visualTree, this.changeEventsList);
					this.DispatchChangeEvents(this.changeEventsList, num);
					bool flag2 = !base.visualTree.layoutNode.IsDirty;
					if (flag2)
					{
						this.DispatchMissedHierarchyChangeEvents(this.missedHierarchyChangeEventsList, num);
						this.missedHierarchyChangeEventsList.Clear();
					}
					bool flag3 = num++ >= 10;
					if (flag3)
					{
						string text = "Layout update is struggling to process current layout (consider simplifying to avoid recursive layout): ";
						VisualElement visualTree = base.visualTree;
						Debug.LogError(text + ((visualTree != null) ? visualTree.ToString() : null));
						break;
					}
				}
			}
			base.visualTree.focusController.ReevaluateFocus();
		}

		private static bool UpdateHierarchyDisplayed(VisualElement ve, List<ValueTuple<Rect, Rect, VisualElement>> changeEvents, bool inheritedDisplayed = true)
		{
			bool flag = inheritedDisplayed & (ve.resolvedStyle.display != DisplayStyle.None);
			bool flag2 = inheritedDisplayed && !flag;
			if (flag2)
			{
				ve.disableRendering = true;
			}
			else
			{
				bool flag3 = flag;
				if (flag3)
				{
					ve.disableRendering = false;
				}
			}
			bool flag4 = ve.areAncestorsAndSelfDisplayed == flag;
			bool flag5;
			if (flag4)
			{
				flag5 = false;
			}
			else
			{
				ve.areAncestorsAndSelfDisplayed = flag;
				bool flag6 = !flag;
				if (flag6)
				{
					if (inheritedDisplayed)
					{
						ve.IncrementVersion(VersionChangeType.Size);
					}
					bool flag7 = ve.HasSelfEventInterests(EventBase<GeometryChangedEvent>.EventCategory);
					if (flag7)
					{
						changeEvents.Add(new ValueTuple<Rect, Rect, VisualElement>(ve.lastLayout, Rect.zero, ve));
					}
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						VisualTreeLayoutUpdater.UpdateHierarchyDisplayed(ve.hierarchy[i], changeEvents, flag);
					}
				}
				flag5 = true;
			}
			return flag5;
		}

		private void UpdateSubTree(VisualElement ve, List<ValueTuple<Rect, Rect, VisualElement>> changeEvents)
		{
			bool flag = VisualTreeLayoutUpdater.UpdateHierarchyDisplayed(ve, changeEvents, true);
			bool flag2 = !ve.areAncestorsAndSelfDisplayed;
			if (!flag2)
			{
				Rect rect = new Rect(ve.layoutNode.LayoutX, ve.layoutNode.LayoutY, ve.layoutNode.LayoutWidth, ve.layoutNode.LayoutHeight);
				Rect rect2 = new Rect(ve.layoutNode.LayoutPaddingLeft, ve.layoutNode.LayoutPaddingLeft, ve.layoutNode.LayoutPaddingRight, ve.layoutNode.LayoutPaddingBottom);
				Rect rect3 = new Rect(rect2.x, rect2.y, rect.width - (rect2.x + rect2.width), rect.height - (rect2.y + rect2.height));
				Rect lastLayout = ve.lastLayout;
				Rect lastPseudoPadding = ve.lastPseudoPadding;
				VersionChangeType versionChangeType = (VersionChangeType)0;
				bool flag3 = lastLayout.size != rect.size;
				bool flag4 = lastPseudoPadding.size != rect3.size;
				bool flag5 = flag3 || flag4;
				if (flag5)
				{
					versionChangeType |= VersionChangeType.Size | VersionChangeType.Repaint;
				}
				bool flag6 = rect.position != lastLayout.position;
				bool flag7 = rect3.position != lastPseudoPadding.position;
				bool flag8 = flag6 || flag7 || flag;
				if (flag8)
				{
					versionChangeType |= VersionChangeType.Transform;
				}
				bool flag9 = flag;
				if (flag9)
				{
					versionChangeType |= VersionChangeType.Size;
				}
				bool flag10 = (versionChangeType & (VersionChangeType.Transform | VersionChangeType.Size)) == VersionChangeType.Size;
				if (flag10)
				{
					bool flag11 = !ve.hasDefaultRotationAndScale;
					if (flag11)
					{
						bool flag12 = !Mathf.Approximately(ve.resolvedStyle.transformOrigin.x, 0f) || !Mathf.Approximately(ve.resolvedStyle.transformOrigin.y, 0f);
						if (flag12)
						{
							versionChangeType |= VersionChangeType.Transform;
						}
					}
				}
				bool flag13 = versionChangeType > (VersionChangeType)0;
				if (flag13)
				{
					ve.IncrementVersion(versionChangeType);
				}
				ve.lastLayout = rect;
				ve.lastPseudoPadding = rect3;
				bool hasNewLayout = ve.layoutNode.HasNewLayout;
				bool flag14 = hasNewLayout;
				if (flag14)
				{
					int childCount = ve.hierarchy.childCount;
					for (int i = 0; i < childCount; i++)
					{
						VisualElement visualElement = ve.hierarchy[i];
						bool hasNewLayout2 = visualElement.layoutNode.HasNewLayout;
						if (hasNewLayout2)
						{
							this.UpdateSubTree(visualElement, changeEvents);
						}
					}
				}
				bool flag15 = ve.HasSelfEventInterests(EventBase<GeometryChangedEvent>.EventCategory);
				if (flag15)
				{
					bool flag16 = flag3 || flag6 || flag;
					if (flag16)
					{
						changeEvents.Add(new ValueTuple<Rect, Rect, VisualElement>(flag ? Rect.zero : lastLayout, rect, ve));
						bool receivesHierarchyGeometryChangedEvents = ve.receivesHierarchyGeometryChangedEvents;
						if (receivesHierarchyGeometryChangedEvents)
						{
							this.missedHierarchyChangeEventsList.Remove(ve);
						}
					}
					else
					{
						bool flag17 = ve.receivesHierarchyGeometryChangedEvents && ve.boundingBoxDirtiedSinceLastLayoutPass;
						if (flag17)
						{
							this.missedHierarchyChangeEventsList.Add(ve);
						}
					}
				}
				ve.boundingBoxDirtiedSinceLastLayoutPass = false;
				bool flag18 = hasNewLayout;
				if (flag18)
				{
					ve.layoutNode.MarkLayoutSeen();
				}
			}
		}

		private void DispatchChangeEvents(List<ValueTuple<Rect, Rect, VisualElement>> changeEvents, int currentLayoutPass)
		{
			foreach (ValueTuple<Rect, Rect, VisualElement> valueTuple in changeEvents)
			{
				Rect item = valueTuple.Item1;
				Rect item2 = valueTuple.Item2;
				VisualElement item3 = valueTuple.Item3;
				using (GeometryChangedEvent pooled = GeometryChangedEvent.GetPooled(item, item2))
				{
					pooled.layoutPass = currentLayoutPass;
					EventDispatchUtilities.SendEventDirectlyToTarget(pooled, base.panel, item3);
				}
			}
		}

		private void DispatchMissedHierarchyChangeEvents(List<VisualElement> missedHierarchyChangeEvents, int currentLayoutPass)
		{
			foreach (VisualElement visualElement in missedHierarchyChangeEvents)
			{
				using (GeometryChangedEvent pooled = GeometryChangedEvent.GetPooled(Rect.zero, visualElement.layout))
				{
					pooled.layoutPass = currentLayoutPass;
					EventDispatchUtilities.SendEventDirectlyToTarget(pooled, base.panel, visualElement);
				}
			}
		}

		public const int kMaxValidateLayoutCount = 10;

		private static readonly string s_Description = "UIElements.UpdateLayout";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(VisualTreeLayoutUpdater.s_Description);

		private static readonly ProfilerMarker k_ComputeLayoutMarker = new ProfilerMarker("LayoutUpdater.ComputeLayout");

		private static readonly ProfilerMarker k_UpdateSubTreeMarker = new ProfilerMarker("LayoutUpdater.UpdateSubTree");

		private static readonly ProfilerMarker k_DispatchChangeEventsMarker = new ProfilerMarker("LayoutUpdater.DispatchChangeEvents");

		private List<ValueTuple<Rect, Rect, VisualElement>> changeEventsList = new List<ValueTuple<Rect, Rect, VisualElement>>();

		private List<VisualElement> missedHierarchyChangeEventsList = new List<VisualElement>();

		private TextJobSystem m_TextJobSystem = new TextJobSystem();
	}
}
