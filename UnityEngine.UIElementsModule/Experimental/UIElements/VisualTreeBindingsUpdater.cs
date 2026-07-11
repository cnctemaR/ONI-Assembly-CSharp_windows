using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	internal class VisualTreeBindingsUpdater : BaseVisualTreeHierarchyTrackerUpdater
	{
		public override string description
		{
			get
			{
				return "Update Bindings";
			}
		}

		private IBinding GetUpdaterFromElement(VisualElement ve)
		{
			IBindable bindable = ve as IBindable;
			return (bindable != null) ? bindable.binding : null;
		}

		private void StartTracking(VisualElement ve)
		{
			this.m_ElementsToAdd.Add(ve);
			this.m_ElementsToRemove.Remove(ve);
		}

		private void StopTracking(VisualElement ve)
		{
			this.m_ElementsToRemove.Add(ve);
			this.m_ElementsToAdd.Remove(ve);
		}

		private void StartTrackingRecursive(VisualElement ve)
		{
			IBinding updaterFromElement = this.GetUpdaterFromElement(ve);
			if (updaterFromElement != null)
			{
				this.StartTracking(ve);
			}
			int childCount = ve.shadow.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.shadow[i];
				this.StartTrackingRecursive(visualElement);
			}
		}

		private void StopTrackingRecursive(VisualElement ve)
		{
			this.StopTracking(ve);
			int childCount = ve.shadow.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.shadow[i];
				this.StopTrackingRecursive(visualElement);
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			base.OnVersionChanged(ve, versionChangeType);
			if ((versionChangeType & VersionChangeType.Bindings) == VersionChangeType.Bindings)
			{
				if (this.GetUpdaterFromElement(ve) != null)
				{
					this.StartTracking(ve);
				}
				else
				{
					this.StopTracking(ve);
				}
			}
		}

		protected override void OnHierarchyChange(VisualElement ve, HierarchyChangeType type)
		{
			if (type != HierarchyChangeType.Add)
			{
				if (type == HierarchyChangeType.Remove)
				{
					this.StopTrackingRecursive(ve);
				}
			}
			else
			{
				this.StartTrackingRecursive(ve);
			}
		}

		private static long CurrentTime()
		{
			return Panel.TimeSinceStartup();
		}

		public void PerformTrackingOperations()
		{
			foreach (VisualElement visualElement in this.m_ElementsToAdd)
			{
				IBinding updaterFromElement = this.GetUpdaterFromElement(visualElement);
				if (updaterFromElement != null)
				{
					this.m_ElementsWithBindings.Add(visualElement);
				}
			}
			this.m_ElementsToAdd.Clear();
			foreach (VisualElement visualElement2 in this.m_ElementsToRemove)
			{
				this.m_ElementsWithBindings.Remove(visualElement2);
			}
			this.m_ElementsToRemove.Clear();
		}

		public override void Update()
		{
			base.Update();
			this.PerformTrackingOperations();
			if (this.m_ElementsWithBindings.Count > 0)
			{
				long num = VisualTreeBindingsUpdater.CurrentTime();
				if (this.m_LastUpdateTime + 100L < num)
				{
					this.UpdateBindings();
					this.m_LastUpdateTime = num;
				}
			}
		}

		private void UpdateBindings()
		{
			foreach (VisualElement visualElement in this.m_ElementsWithBindings)
			{
				IBinding updaterFromElement = this.GetUpdaterFromElement(visualElement);
				if (updaterFromElement == null || visualElement.elementPanel != base.panel)
				{
					if (updaterFromElement != null)
					{
						updaterFromElement.Release();
					}
					this.StopTracking(visualElement);
				}
				else
				{
					this.updatedBindings.Add(updaterFromElement);
				}
			}
			foreach (IBinding binding in this.updatedBindings)
			{
				binding.PreUpdate();
			}
			foreach (IBinding binding2 in this.updatedBindings)
			{
				binding2.Update();
			}
			this.updatedBindings.Clear();
		}

		private readonly HashSet<VisualElement> m_ElementsWithBindings = new HashSet<VisualElement>();

		private readonly HashSet<VisualElement> m_ElementsToAdd = new HashSet<VisualElement>();

		private readonly HashSet<VisualElement> m_ElementsToRemove = new HashSet<VisualElement>();

		private const int kMinUpdateDelay = 100;

		private long m_LastUpdateTime = 0L;

		private List<IBinding> updatedBindings = new List<IBinding>();
	}
}
