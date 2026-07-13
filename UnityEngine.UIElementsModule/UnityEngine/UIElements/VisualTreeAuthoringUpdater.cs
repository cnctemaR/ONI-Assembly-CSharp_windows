using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace UnityEngine.UIElements
{
	internal sealed class VisualTreeAuthoringUpdater : BaseVisualTreeUpdater
	{
		internal VisualTreeAuthoringUpdater.StateSnapshot GetState()
		{
			return new VisualTreeAuthoringUpdater.StateSnapshot
			{
				processorsCount = this.m_RegisteredProcessors.Count,
				containsAccumulatedChanges = this.m_Accumulator.ContainsChanges(),
				isProcessingChanges = this.m_AccumulatingChanges
			};
		}

		public override ProfilerMarker profilerMarker
		{
			get
			{
				return VisualTreeAuthoringUpdater.s_UpdateProfilerMarker;
			}
		}

		private bool shouldUpdate
		{
			get
			{
				return this.m_AccumulatingChanges || this.m_ProcessorRegistrationList.Count > 0;
			}
		}

		public VisualTreeAuthoringUpdater()
		{
			base.panelChanged += this.OnPanelChanged;
			this.m_Changes1 = new AuthoringChanges();
			this.m_Changes2 = new AuthoringChanges();
			this.m_Accumulator = this.m_Changes1;
			this.m_Notifier = this.m_Changes2;
		}

		public void RegisterProcessor(IVisualElementChangeProcessor processor)
		{
			bool flag = this.m_RegisteredProcessors.Contains(processor) || this.m_ProcessorRegistrationList.Contains(processor);
			if (!flag)
			{
				this.m_ProcessorRegistrationList.Add(processor);
				this.m_ProcessorUnregistrationList.Remove(processor);
			}
		}

		public void UnregisterProcessor(IVisualElementChangeProcessor processor)
		{
			bool flag = !this.m_RegisteredProcessors.Contains(processor) || this.m_ProcessorUnregistrationList.Contains(processor);
			if (!flag)
			{
				this.m_ProcessorUnregistrationList.Add(processor);
				this.m_ProcessorRegistrationList.Remove(processor);
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = !this.m_AccumulatingChanges;
			if (!flag)
			{
				using (VisualTreeAuthoringUpdater.s_UpdateChangeProfilerMarker.Auto())
				{
					bool flag2 = (versionChangeType & (VersionChangeType.Layout | VersionChangeType.Styles)) > (VersionChangeType)0;
					if (flag2)
					{
						this.m_Accumulator.styleChanged.Add(ve);
					}
					bool flag3 = (versionChangeType & VersionChangeType.StyleSheet) > (VersionChangeType)0;
					if (flag3)
					{
						this.m_Accumulator.stylingContextChanged.Add(ve);
					}
					bool flag4 = (versionChangeType & (VersionChangeType.Bindings | VersionChangeType.BindingRegistration | VersionChangeType.DataSource)) > (VersionChangeType)0;
					if (flag4)
					{
						this.m_Accumulator.bindingContextChanged.Add(ve);
					}
				}
			}
		}

		public override void Update()
		{
			bool flag = !this.shouldUpdate;
			if (!flag)
			{
				bool? flag2 = null;
				this.SwapBuffers();
				AuthoringChanges notifier = this.m_Notifier;
				bool flag3 = notifier.ContainsChanges();
				if (flag3)
				{
					for (int i = 0; i < this.m_RegisteredProcessors.Count; i++)
					{
						IVisualElementChangeProcessor visualElementChangeProcessor = this.m_RegisteredProcessors[i];
						visualElementChangeProcessor.ProcessChanges(base.panel, notifier);
					}
				}
				for (int j = 0; j < this.m_ProcessorRegistrationList.Count; j++)
				{
					IVisualElementChangeProcessor visualElementChangeProcessor2 = this.m_ProcessorRegistrationList[j];
					this.m_RegisteredProcessors.Add(visualElementChangeProcessor2);
					visualElementChangeProcessor2.BeginProcessing(base.panel);
					flag2 = new bool?(true);
				}
				this.m_ProcessorRegistrationList.Clear();
				for (int k = 0; k < this.m_ProcessorUnregistrationList.Count; k++)
				{
					IVisualElementChangeProcessor visualElementChangeProcessor3 = this.m_ProcessorUnregistrationList[k];
					this.m_RegisteredProcessors.Remove(visualElementChangeProcessor3);
					visualElementChangeProcessor3.EndProcessing(base.panel);
				}
				this.m_ProcessorUnregistrationList.Clear();
				bool flag4 = this.m_RegisteredProcessors.Count == 0;
				if (flag4)
				{
					flag2 = new bool?(false);
				}
				bool flag5 = flag2 != null;
				if (flag5)
				{
					this.m_AccumulatingChanges = flag2.Value;
				}
				notifier.Clear();
			}
		}

		private void OnPanelChanged(BaseVisualElementPanel p)
		{
			bool flag = this.m_AttachedPanel == p;
			if (!flag)
			{
				bool flag2 = this.m_AttachedPanel != null;
				if (flag2)
				{
					this.m_AttachedPanel.hierarchyChanged -= this.OnHierarchyChange;
				}
				this.m_AttachedPanel = p;
				bool flag3 = this.m_AttachedPanel != null;
				if (flag3)
				{
					this.m_AttachedPanel.hierarchyChanged += this.OnHierarchyChange;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.SwapBuffers();
			AuthoringChanges notifier = this.m_Notifier;
			for (int i = 0; i < this.m_RegisteredProcessors.Count; i++)
			{
				IVisualElementChangeProcessor visualElementChangeProcessor = this.m_RegisteredProcessors[i];
				visualElementChangeProcessor.ProcessChanges(base.panel, notifier);
				visualElementChangeProcessor.EndProcessing(base.panel);
			}
			this.m_ProcessorRegistrationList.Clear();
			this.m_ProcessorUnregistrationList.Clear();
			this.m_RegisteredProcessors.Clear();
			base.panelChanged -= this.OnPanelChanged;
			bool flag = this.m_AttachedPanel != null;
			if (flag)
			{
				this.m_AttachedPanel.hierarchyChanged -= this.OnHierarchyChange;
			}
			notifier.Clear();
		}

		private void OnHierarchyChange(VisualElement ve, HierarchyChangeType type, IReadOnlyList<VisualElement> additionalContext = null)
		{
			bool flag = !this.m_AccumulatingChanges;
			if (!flag)
			{
				using (VisualTreeAuthoringUpdater.s_UpdateChangeProfilerMarker.Auto())
				{
					switch (type)
					{
					case HierarchyChangeType.AddedToParent:
					case HierarchyChangeType.ChildrenReordered:
						this.m_Accumulator.addedOrMovedElements.Add(ve);
						break;
					case HierarchyChangeType.RemovedFromParent:
						this.m_Accumulator.addedOrMovedElements.Remove(ve);
						break;
					case HierarchyChangeType.AttachedToPanel:
					{
						for (int i = 0; i < additionalContext.Count; i++)
						{
							VisualElement visualElement = additionalContext[i];
							this.m_Accumulator.addedOrMovedElements.Add(visualElement);
							this.m_Accumulator.removedFromPanel.Remove(visualElement);
						}
						break;
					}
					case HierarchyChangeType.DetachedFromPanel:
					{
						for (int j = 0; j < additionalContext.Count; j++)
						{
							VisualElement visualElement2 = additionalContext[j];
							this.m_Accumulator.addedOrMovedElements.Remove(visualElement2);
							this.m_Accumulator.removedFromPanel.Add(visualElement2);
						}
						break;
					}
					}
				}
			}
		}

		private void SwapBuffers()
		{
			bool flag = this.m_Accumulator == this.m_Changes1;
			if (flag)
			{
				this.m_Accumulator = this.m_Changes2;
				this.m_Notifier = this.m_Changes1;
			}
			else
			{
				this.m_Accumulator = this.m_Changes1;
				this.m_Notifier = this.m_Changes2;
			}
		}

		private const VersionChangeType k_StyleChangedFlags = VersionChangeType.Layout | VersionChangeType.Styles;

		private const VersionChangeType k_StylingContextChangedFlags = VersionChangeType.StyleSheet;

		private const VersionChangeType k_BindingsChangedFlags = VersionChangeType.Bindings | VersionChangeType.BindingRegistration | VersionChangeType.DataSource;

		private static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("Update Authoring");

		private static readonly ProfilerMarker s_UpdateChangeProfilerMarker = new ProfilerMarker("Update Authoring - Change");

		private readonly List<IVisualElementChangeProcessor> m_RegisteredProcessors = new List<IVisualElementChangeProcessor>();

		private readonly List<IVisualElementChangeProcessor> m_ProcessorRegistrationList = new List<IVisualElementChangeProcessor>();

		private readonly List<IVisualElementChangeProcessor> m_ProcessorUnregistrationList = new List<IVisualElementChangeProcessor>();

		private BaseVisualElementPanel m_AttachedPanel;

		private readonly AuthoringChanges m_Changes1;

		private readonly AuthoringChanges m_Changes2;

		private AuthoringChanges m_Accumulator;

		private AuthoringChanges m_Notifier;

		private bool m_AccumulatingChanges;

		internal struct StateSnapshot
		{
			public int processorsCount;

			public bool containsAccumulatedChanges;

			public bool isProcessingChanges;
		}
	}
}
