using System;

namespace UnityEngine.Experimental.UIElements
{
	internal abstract class BaseVisualTreeHierarchyTrackerUpdater : BaseVisualTreeUpdater
	{
		protected abstract void OnHierarchyChange(VisualElement ve, HierarchyChangeType type);

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			if ((versionChangeType & VersionChangeType.Hierarchy) == VersionChangeType.Hierarchy)
			{
				BaseVisualTreeHierarchyTrackerUpdater.State state = this.m_State;
				if (state != BaseVisualTreeHierarchyTrackerUpdater.State.Waiting)
				{
					if (state != BaseVisualTreeHierarchyTrackerUpdater.State.TrackingRemove)
					{
						if (state == BaseVisualTreeHierarchyTrackerUpdater.State.TrackingAddOrMove)
						{
							this.ProcessAddOrMove(ve);
						}
					}
					else
					{
						this.ProcessRemove(ve);
					}
				}
				else
				{
					this.ProcessNewChange(ve);
				}
			}
		}

		public override void Update()
		{
			Debug.Assert(this.m_State == BaseVisualTreeHierarchyTrackerUpdater.State.TrackingAddOrMove || this.m_State == BaseVisualTreeHierarchyTrackerUpdater.State.Waiting);
			if (this.m_State == BaseVisualTreeHierarchyTrackerUpdater.State.TrackingAddOrMove)
			{
				this.OnHierarchyChange(this.m_CurrentChangeElement, HierarchyChangeType.Move);
				this.m_State = BaseVisualTreeHierarchyTrackerUpdater.State.Waiting;
			}
			this.m_CurrentChangeElement = null;
			this.m_CurrentChangeParent = null;
		}

		private void ProcessNewChange(VisualElement ve)
		{
			this.m_CurrentChangeElement = ve;
			this.m_CurrentChangeParent = ve.parent;
			if (this.m_CurrentChangeParent == null && ve.panel != null)
			{
				this.OnHierarchyChange(this.m_CurrentChangeElement, HierarchyChangeType.Move);
				this.m_State = BaseVisualTreeHierarchyTrackerUpdater.State.Waiting;
			}
			else
			{
				this.m_State = ((this.m_CurrentChangeParent != null) ? BaseVisualTreeHierarchyTrackerUpdater.State.TrackingAddOrMove : BaseVisualTreeHierarchyTrackerUpdater.State.TrackingRemove);
			}
		}

		private void ProcessAddOrMove(VisualElement ve)
		{
			Debug.Assert(this.m_CurrentChangeParent != null);
			if (this.m_CurrentChangeParent == ve)
			{
				this.OnHierarchyChange(this.m_CurrentChangeElement, HierarchyChangeType.Add);
				this.m_State = BaseVisualTreeHierarchyTrackerUpdater.State.Waiting;
			}
			else
			{
				this.OnHierarchyChange(this.m_CurrentChangeElement, HierarchyChangeType.Move);
				this.ProcessNewChange(ve);
			}
		}

		private void ProcessRemove(VisualElement ve)
		{
			this.OnHierarchyChange(this.m_CurrentChangeElement, HierarchyChangeType.Remove);
			if (ve.panel != null)
			{
				this.m_CurrentChangeParent = null;
				this.m_CurrentChangeElement = null;
				this.m_State = BaseVisualTreeHierarchyTrackerUpdater.State.Waiting;
			}
			else
			{
				this.m_CurrentChangeElement = ve;
			}
		}

		private BaseVisualTreeHierarchyTrackerUpdater.State m_State = BaseVisualTreeHierarchyTrackerUpdater.State.Waiting;

		private VisualElement m_CurrentChangeElement;

		private VisualElement m_CurrentChangeParent;

		private enum State
		{
			Waiting,
			TrackingAddOrMove,
			TrackingRemove
		}
	}
}
