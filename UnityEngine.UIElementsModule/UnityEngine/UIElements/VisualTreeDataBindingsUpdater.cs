using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Profiling;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	internal class VisualTreeDataBindingsUpdater : BaseVisualTreeUpdater
	{
		private DataBindingManager bindingManager
		{
			get
			{
				return base.panel.dataBindingManager;
			}
		}

		public override ProfilerMarker profilerMarker
		{
			get
			{
				return VisualTreeDataBindingsUpdater.s_UpdateProfilerMarker;
			}
		}

		public VisualTreeDataBindingsUpdater()
		{
			base.panelChanged += this.OnPanelChanged;
		}

		protected void OnHierarchyChange(VisualElement ve, HierarchyChangeType type, IReadOnlyList<VisualElement> additionalContext = null)
		{
			bool flag = this.bindingManager.GetBoundElementsCount() == 0 && this.bindingManager.GetTrackedDataSourcesCount() == 0;
			if (!flag)
			{
				switch (type)
				{
				case HierarchyChangeType.AddedToParent:
				case HierarchyChangeType.ChildrenReordered:
					this.m_RemovedElements.Remove(ve);
					this.m_DataSourceChangedRequests.Add(ve);
					break;
				case HierarchyChangeType.RemovedFromParent:
					this.m_DataSourceChangedRequests.Remove(ve);
					this.m_RemovedElements.Add(ve);
					break;
				case HierarchyChangeType.AttachedToPanel:
				{
					for (int i = 0; i < additionalContext.Count; i++)
					{
						VisualElement visualElement = additionalContext[i];
						this.m_RemovedElements.Remove(visualElement);
						this.m_DataSourceChangedRequests.Add(ve);
					}
					break;
				}
				case HierarchyChangeType.DetachedFromPanel:
				{
					for (int j = 0; j < additionalContext.Count; j++)
					{
						VisualElement visualElement2 = additionalContext[j];
						this.m_DataSourceChangedRequests.Remove(ve);
						this.m_RemovedElements.Add(visualElement2);
					}
					break;
				}
				}
				this.bindingManager.DirtyBindingOrder();
			}
		}

		public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
		{
			bool flag = (versionChangeType & VersionChangeType.BindingRegistration) == VersionChangeType.BindingRegistration;
			if (flag)
			{
				this.m_BindingRegistrationRequests.Add(ve);
			}
			bool flag2 = (versionChangeType & VersionChangeType.DataSource) == VersionChangeType.DataSource;
			if (flag2)
			{
				this.m_DataSourceChangedRequests.Add(ve);
			}
		}

		private void CacheAndLogBindingResult(bool appliedOnUiCache, in DataBindingManager.BindingData bindingData, in BindingResult result)
		{
			BindingLogLevel logLevel = this.bindingManager.logLevel;
			bool flag = logLevel == BindingLogLevel.None;
			if (!flag)
			{
				bool flag2 = logLevel == BindingLogLevel.Once;
				if (flag2)
				{
					BindingResult bindingResult;
					if (appliedOnUiCache)
					{
						this.bindingManager.TryGetLastUIBindingResult(bindingData, out bindingResult);
					}
					else
					{
						this.bindingManager.TryGetLastSourceBindingResult(bindingData, out bindingResult);
					}
					bool flag3 = bindingResult.status != result.status || bindingResult.message != result.message;
					if (flag3)
					{
						this.LogResult(in result);
					}
				}
				else
				{
					this.LogResult(in result);
				}
			}
			if (appliedOnUiCache)
			{
				this.bindingManager.CacheUIBindingResult(bindingData, result);
			}
			else
			{
				this.bindingManager.CacheSourceBindingResult(bindingData, result);
			}
		}

		private void LogResult(in BindingResult result)
		{
			bool flag = string.IsNullOrWhiteSpace(result.message);
			if (!flag)
			{
				Panel panel = base.panel as Panel;
				string text = ((panel != null) ? panel.name : null) ?? base.panel.visualTree.name;
				Debug.LogWarning(result.message + " (" + text + ")");
			}
		}

		public override void Update()
		{
			this.ProcessAllBindingRequests();
			this.ProcessDataSourceChangedRequests();
			this.ProcessPropertyChangedEvents(this.m_RanUpdate);
			this.m_BoundsElement.AddRange(this.bindingManager.GetBoundElements());
			foreach (VisualElement visualElement in this.m_BoundsElement)
			{
				List<DataBindingManager.BindingData> bindingData = this.bindingManager.GetBindingData(visualElement);
				int i = 0;
				while (i < bindingData.Count)
				{
					DataBindingManager.BindingData bindingData2 = bindingData[i];
					object dataSource;
					PropertyPath dataSourcePath;
					using (VisualTreeDataBindingsUpdater.s_ShouldUpdateBindingProfilerMarker.Auto())
					{
						DataSourceContext resolvedDataSourceContext = this.bindingManager.GetResolvedDataSourceContext(visualElement, bindingData2);
						dataSource = resolvedDataSourceContext.dataSource;
						dataSourcePath = resolvedDataSourceContext.dataSourcePath;
						ValueTuple<bool, long> dataSourceVersion = this.GetDataSourceVersion(dataSource);
						bool item = dataSourceVersion.Item1;
						long item2 = dataSourceVersion.Item2;
						bool flag = bindingData2.binding == null;
						if (flag)
						{
							goto IL_0325;
						}
						bool flag2 = dataSource != null && this.m_TrackedObjects.Add(dataSource);
						if (flag2)
						{
							this.m_VersionChanges.Add(new VisualTreeDataBindingsUpdater.VersionInfo(dataSource, item2));
						}
						bool isDirty = bindingData2.binding.isDirty;
						if (isDirty)
						{
							this.m_DirtyBindings.Add(bindingData2.binding);
						}
						bool flag3 = !this.m_Updater.ShouldProcessBindingAtStage(bindingData2.binding, BindingUpdateStage.UpdateUI, item, this.m_DirtyBindings.Contains(bindingData2.binding));
						if (flag3)
						{
							goto IL_0325;
						}
						bool flag4 = dataSource != null && item;
						if (flag4)
						{
							this.m_KnownSources.Add(dataSource);
						}
						bool flag5 = bindingData2.binding.updateTrigger == BindingUpdateTrigger.OnSourceChanged && dataSource is INotifyBindablePropertyChanged && !bindingData2.binding.isDirty;
						if (flag5)
						{
							HashSet<PropertyPath> changedDetectedFromSource = this.bindingManager.GetChangedDetectedFromSource(dataSource);
							bool flag6 = changedDetectedFromSource == null || changedDetectedFromSource.Count == 0;
							if (flag6)
							{
								goto IL_0325;
							}
							bool flag7 = dataSourcePath.IsEmpty;
							foreach (PropertyPath propertyPath in changedDetectedFromSource)
							{
								bool flag8 = this.IsPrefix(in propertyPath, in dataSourcePath);
								if (flag8)
								{
									flag7 = true;
									break;
								}
							}
							bool flag9 = !flag7;
							if (flag9)
							{
								goto IL_0325;
							}
						}
					}
					goto IL_0242;
					IL_0325:
					i++;
					continue;
					IL_0242:
					bool isDirty2 = bindingData2.binding.isDirty;
					bindingData2.binding.ClearDirty();
					BindingContext bindingContext = new BindingContext(visualElement, in bindingData2.target.bindingId, in dataSourcePath, dataSource);
					BindingResult bindingResult = default(BindingResult);
					long version = bindingData2.version;
					using (VisualTreeDataBindingsUpdater.s_UpdateBindingProfilerMarker.Auto())
					{
						bindingResult = this.m_Updater.UpdateUI(in bindingContext, bindingData2.binding);
					}
					this.CacheAndLogBindingResult(true, in bindingData2, in bindingResult);
					bool flag10 = bindingData2.version == version;
					if (flag10)
					{
						BindingStatus status = bindingResult.status;
						BindingStatus bindingStatus = status;
						if (bindingStatus != BindingStatus.Success)
						{
							if (bindingStatus == BindingStatus.Pending)
							{
								if (isDirty2)
								{
									bindingData2.binding.MarkDirty();
								}
							}
						}
						else
						{
							this.m_RanUpdate.Add(bindingData2.binding);
						}
					}
					goto IL_0325;
				}
			}
			foreach (VisualTreeDataBindingsUpdater.VersionInfo versionInfo in this.m_VersionChanges)
			{
				this.bindingManager.UpdateVersion(versionInfo.source, versionInfo.version);
			}
			this.ProcessPropertyChangedEvents(this.m_RanUpdate);
			foreach (object obj in this.m_KnownSources)
			{
				this.bindingManager.ClearChangesFromSource(obj);
			}
			this.m_BoundsElement.Clear();
			this.m_VersionChanges.Clear();
			this.m_TrackedObjects.Clear();
			this.m_RanUpdate.Clear();
			this.m_KnownSources.Clear();
			this.m_DirtyBindings.Clear();
			this.bindingManager.ClearSourceCache();
		}

		[return: TupleElementNames(new string[] { "changed", "version" })]
		private ValueTuple<bool, long> GetDataSourceVersion(object source)
		{
			long num;
			bool flag = this.bindingManager.TryGetLastVersion(source, out num);
			ValueTuple<bool, long> valueTuple;
			if (flag)
			{
				IDataSourceViewHashProvider dataSourceViewHashProvider = source as IDataSourceViewHashProvider;
				bool flag2 = dataSourceViewHashProvider == null;
				if (flag2)
				{
					valueTuple = new ValueTuple<bool, long>(source != null, num + 1L);
				}
				else
				{
					long viewHashCode = dataSourceViewHashProvider.GetViewHashCode();
					valueTuple = ((viewHashCode == num) ? new ValueTuple<bool, long>(false, num) : new ValueTuple<bool, long>(true, viewHashCode));
				}
			}
			else
			{
				IDataSourceViewHashProvider dataSourceViewHashProvider2 = source as IDataSourceViewHashProvider;
				bool flag3 = dataSourceViewHashProvider2 != null;
				if (flag3)
				{
					valueTuple = new ValueTuple<bool, long>(true, dataSourceViewHashProvider2.GetViewHashCode());
				}
				else
				{
					valueTuple = new ValueTuple<bool, long>(source != null, 0L);
				}
			}
			return valueTuple;
		}

		private bool IsPrefix(in PropertyPath prefix, in PropertyPath path)
		{
			bool flag = path.Length < prefix.Length;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < prefix.Length; i++)
				{
					PropertyPathPart propertyPathPart = prefix[i];
					PropertyPathPart propertyPathPart2 = path[i];
					bool flag3 = propertyPathPart.Kind != propertyPathPart2.Kind;
					if (flag3)
					{
						return false;
					}
					switch (propertyPathPart.Kind)
					{
					case PropertyPathPartKind.Name:
					{
						bool flag4 = propertyPathPart.Name != propertyPathPart2.Name;
						if (flag4)
						{
							return false;
						}
						break;
					}
					case PropertyPathPartKind.Index:
					{
						bool flag5 = propertyPathPart.Index != propertyPathPart2.Index;
						if (flag5)
						{
							return false;
						}
						break;
					}
					case PropertyPathPartKind.Key:
					{
						bool flag6 = propertyPathPart.Key != propertyPathPart2.Key;
						if (flag6)
						{
							return false;
						}
						break;
					}
					default:
						throw new ArgumentOutOfRangeException();
					}
				}
				flag2 = true;
			}
			return flag2;
		}

		private void ProcessDataSourceChangedRequests()
		{
			using (VisualTreeDataBindingsUpdater.s_ProcessDataSourcesProfilerMarker.Auto())
			{
				bool flag = this.m_DataSourceChangedRequests.Count == 0 && this.m_RemovedElements.Count == 0;
				if (!flag)
				{
					this.m_DataSourceChangedRequests.RemoveWhere((VisualElement e) => e.panel == null);
					this.bindingManager.InvalidateCachedDataSource(this.m_DataSourceChangedRequests, this.m_RemovedElements);
					this.m_DataSourceChangedRequests.Clear();
					this.m_RemovedElements.Clear();
				}
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
			this.ProcessAllBindingRequests();
			this.ProcessDataSourceChangedRequests();
			base.Dispose(disposing);
			this.bindingManager.Dispose();
		}

		private void ProcessAllBindingRequests()
		{
			using (VisualTreeDataBindingsUpdater.s_ProcessBindingRequestsProfilerMarker.Auto())
			{
				for (int i = 0; i < this.m_BindingRegistrationRequests.Count; i++)
				{
					VisualElement visualElement = this.m_BindingRegistrationRequests[i];
					bool flag = visualElement.panel != base.panel;
					if (!flag)
					{
						this.ProcessBindingRequests(visualElement);
					}
				}
				this.m_BindingRegistrationRequests.Clear();
			}
		}

		private void ProcessBindingRequests(VisualElement element)
		{
			this.bindingManager.ProcessBindingRequests(element);
		}

		private void ProcessPropertyChangedEvents(HashSet<Binding> ranUpdate)
		{
			List<DataBindingManager.ChangesFromUI> changedDetectedFromUI = this.bindingManager.GetChangedDetectedFromUI();
			for (int i = 0; i < changedDetectedFromUI.Count; i++)
			{
				DataBindingManager.ChangesFromUI changesFromUI = changedDetectedFromUI[i];
				bool flag = !changesFromUI.IsValid;
				if (!flag)
				{
					DataBindingManager.BindingData bindingData = changesFromUI.bindingData;
					Binding binding = bindingData.binding;
					VisualElement element = bindingData.target.element;
					bool flag2 = !this.m_Updater.ShouldProcessBindingAtStage(binding, BindingUpdateStage.UpdateSource, true, false);
					if (!flag2)
					{
						bool flag3 = ranUpdate.Contains(binding);
						if (!flag3)
						{
							DataSourceContext resolvedDataSourceContext = this.bindingManager.GetResolvedDataSourceContext(bindingData.target.element, bindingData);
							object dataSource = resolvedDataSourceContext.dataSource;
							PropertyPath dataSourcePath = resolvedDataSourceContext.dataSourcePath;
							BindingContext bindingContext = new BindingContext(element, in bindingData.target.bindingId, in dataSourcePath, dataSource);
							BindingResult bindingResult = this.m_Updater.UpdateSource(in bindingContext, binding);
							this.CacheAndLogBindingResult(false, in bindingData, in bindingResult);
							bool flag4 = bindingResult.status == BindingStatus.Success;
							if (flag4)
							{
								bool flag5 = !changesFromUI.IsValid;
								if (!flag5)
								{
									bool isDirty = bindingData.binding.isDirty;
									bindingData.binding.ClearDirty();
									BindingContext bindingContext2 = new BindingContext(element, in bindingData.target.bindingId, in dataSourcePath, dataSource);
									using (this.bindingManager.IgnoreChangesScope(element, bindingContext2.bindingId, binding))
									{
										bindingResult = this.m_Updater.UpdateUI(in bindingContext2, binding);
										this.CacheAndLogBindingResult(true, in bindingData, in bindingResult);
									}
									bool flag6 = bindingResult.status == BindingStatus.Pending;
									if (flag6)
									{
										bool flag7 = isDirty;
										if (flag7)
										{
											bindingData.binding.MarkDirty();
										}
										else
										{
											bindingData.binding.ClearDirty();
										}
									}
								}
							}
						}
					}
				}
			}
			changedDetectedFromUI.Clear();
		}

		internal void PollElementsWithBindings(Action<VisualElement, IBinding> callback)
		{
			bool flag = this.bindingManager.GetBoundElementsCount() > 0;
			if (flag)
			{
				foreach (VisualElement visualElement in this.bindingManager.GetUnorderedBoundElements())
				{
					bool flag2 = visualElement.elementPanel == base.panel;
					if (flag2)
					{
						callback(visualElement, null);
					}
				}
			}
		}

		private static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("UIElements.UpdateRuntimeBindings");

		private static readonly ProfilerMarker s_ProcessBindingRequestsProfilerMarker = new ProfilerMarker("Process Binding Requests");

		private static readonly ProfilerMarker s_ProcessDataSourcesProfilerMarker = new ProfilerMarker("Process Data Sources");

		private static readonly ProfilerMarker s_ShouldUpdateBindingProfilerMarker = new ProfilerMarker("Should Update Binding");

		private static readonly ProfilerMarker s_UpdateBindingProfilerMarker = new ProfilerMarker("Update Binding");

		private readonly BindingUpdater m_Updater = new BindingUpdater();

		private readonly List<VisualElement> m_BindingRegistrationRequests = new List<VisualElement>();

		private readonly HashSet<VisualElement> m_DataSourceChangedRequests = new HashSet<VisualElement>();

		private readonly HashSet<VisualElement> m_RemovedElements = new HashSet<VisualElement>();

		private readonly List<VisualElement> m_BoundsElement = new List<VisualElement>();

		private readonly List<VisualTreeDataBindingsUpdater.VersionInfo> m_VersionChanges = new List<VisualTreeDataBindingsUpdater.VersionInfo>();

		private readonly HashSet<object> m_TrackedObjects = new HashSet<object>();

		private readonly HashSet<Binding> m_RanUpdate = new HashSet<Binding>();

		private readonly HashSet<object> m_KnownSources = new HashSet<object>();

		private readonly HashSet<Binding> m_DirtyBindings = new HashSet<Binding>();

		private BaseVisualElementPanel m_AttachedPanel;

		private readonly struct VersionInfo
		{
			public VersionInfo(object source, long version)
			{
				this.source = source;
				this.version = version;
			}

			public readonly object source;

			public readonly long version;
		}
	}
}
