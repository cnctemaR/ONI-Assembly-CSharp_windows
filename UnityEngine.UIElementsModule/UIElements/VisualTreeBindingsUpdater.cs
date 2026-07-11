using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace UnityEngine.UIElements
{
	internal class VisualTreeBindingsUpdater : BaseVisualTreeHierarchyTrackerUpdater
	{
		public override ProfilerMarker profilerMarker
		{
			get
			{
				return VisualTreeBindingsUpdater.s_ProfilerMarker;
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
			bool flag = updaterFromElement != null;
			if (flag)
			{
				this.StartTracking(ve);
			}
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.hierarchy[i];
				this.StartTrackingRecursive(visualElement);
			}
		}

		private void StopTrackingRecursive(VisualElement ve)
		{
			this.StopTracking(ve);
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement visualElement = ve.hierarchy[i];
				this.StopTrackingRecursive(visualElement);
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			base.OnVersionChanged(ve, versionChangeType);
			bool flag = (versionChangeType & VersionChangeType.Bindings) == VersionChangeType.Bindings;
			if (flag)
			{
				bool flag2 = this.GetUpdaterFromElement(ve) != null;
				if (flag2)
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
				bool flag = updaterFromElement != null;
				if (flag)
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
			bool flag = this.m_ElementsWithBindings.Count > 0;
			if (flag)
			{
				long num = VisualTreeBindingsUpdater.CurrentTime();
				bool flag2 = this.m_LastUpdateTime + 100L < num;
				if (flag2)
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
				bool flag = updaterFromElement == null || visualElement.elementPanel != base.panel;
				if (flag)
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

		internal void PollElementsWithBindings(Action<VisualElement, IBinding> callback)
		{
			this.PerformTrackingOperations();
			bool flag = this.m_ElementsWithBindings.Count > 0;
			if (flag)
			{
				foreach (VisualElement visualElement in this.m_ElementsWithBindings)
				{
					IBinding updaterFromElement = this.GetUpdaterFromElement(visualElement);
					bool flag2 = updaterFromElement == null || visualElement.elementPanel != base.panel;
					if (flag2)
					{
						if (updaterFromElement != null)
						{
							updaterFromElement.Release();
						}
						this.StopTracking(visualElement);
					}
					else
					{
						callback(visualElement, updaterFromElement);
					}
				}
			}
		}

		private static readonly string s_Description = "Update Bindings";

		private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(VisualTreeBindingsUpdater.s_Description);

		private readonly HashSet<VisualElement> m_ElementsWithBindings = new HashSet<VisualElement>();

		private readonly HashSet<VisualElement> m_ElementsToAdd = new HashSet<VisualElement>();

		private readonly HashSet<VisualElement> m_ElementsToRemove = new HashSet<VisualElement>();

		private const int kMinUpdateDelay = 100;

		private long m_LastUpdateTime = 0L;

		private static ProfilerMarker s_MarkerUpdate = new ProfilerMarker("Bindings.Update");

		private static ProfilerMarker s_MarkerPoll = new ProfilerMarker("Bindings.PollElementsWithBindings");

		private List<IBinding> updatedBindings = new List<IBinding>();
	}
}
