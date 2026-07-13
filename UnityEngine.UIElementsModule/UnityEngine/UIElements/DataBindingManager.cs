using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	internal sealed class DataBindingManager : IDisposable
	{
		internal BindingLogLevel logLevel
		{
			get
			{
				return this.m_LogLevel ?? DataBindingManager.globalLogLevel;
			}
			set
			{
				this.m_LogLevel = new BindingLogLevel?(value);
			}
		}

		internal void ResetLogLevel()
		{
			this.m_LogLevel = null;
		}

		internal DataBindingManager(BaseVisualElementPanel panel)
		{
			this.m_Panel = panel;
			this.m_DataSourceTracker = new DataBindingManager.HierarchyDataSourceTracker(this);
			this.m_BindingsTracker = new DataBindingManager.HierarchyBindingTracker(panel);
			this.m_DetectedChangesFromUI = new List<DataBindingManager.ChangesFromUI>();
		}

		internal int GetTrackedDataSourcesCount()
		{
			return this.m_DataSourceTracker.GetTrackedDataSourcesCount();
		}

		internal bool IsTrackingDataSource(VisualElement element)
		{
			return this.m_DataSourceTracker.IsTrackingDataSource(element);
		}

		internal bool TryGetLastVersion(object source, out long version)
		{
			return this.m_DataSourceTracker.TryGetLastVersion(source, out version);
		}

		internal void UpdateVersion(object source, long version)
		{
			this.m_DataSourceTracker.UpdateVersion(source, version);
		}

		internal void CacheUIBindingResult(DataBindingManager.BindingData bindingData, BindingResult result)
		{
			bindingData.m_SourceToUILastUpdate = new BindingResult?(result);
		}

		internal bool TryGetLastUIBindingResult(DataBindingManager.BindingData bindingData, out BindingResult result)
		{
			bool flag = bindingData.m_SourceToUILastUpdate != null;
			bool flag2;
			if (flag)
			{
				result = bindingData.m_SourceToUILastUpdate.Value;
				flag2 = true;
			}
			else
			{
				result = default(BindingResult);
				flag2 = false;
			}
			return flag2;
		}

		internal void CacheSourceBindingResult(DataBindingManager.BindingData bindingData, BindingResult result)
		{
			bindingData.m_UIToSourceLastUpdate = new BindingResult?(result);
		}

		internal bool TryGetLastSourceBindingResult(DataBindingManager.BindingData bindingData, out BindingResult result)
		{
			bool flag = bindingData.m_UIToSourceLastUpdate != null;
			bool flag2;
			if (flag)
			{
				result = bindingData.m_UIToSourceLastUpdate.Value;
				flag2 = true;
			}
			else
			{
				result = default(BindingResult);
				flag2 = false;
			}
			return flag2;
		}

		internal DataSourceContext GetResolvedDataSourceContext(VisualElement element, DataBindingManager.BindingData bindingData)
		{
			return (element.panel == this.m_Panel) ? this.m_DataSourceTracker.GetResolvedDataSourceContext(element, bindingData) : default(DataSourceContext);
		}

		internal bool TryGetSource(VisualElement element, out object dataSource)
		{
			bool flag = element.panel == this.m_Panel;
			bool flag2;
			if (flag)
			{
				dataSource = this.m_DataSourceTracker.GetHierarchyDataSource(element);
				flag2 = true;
			}
			else
			{
				dataSource = null;
				flag2 = false;
			}
			return flag2;
		}

		internal object TrackHierarchyDataSource(VisualElement element)
		{
			return (element.panel == this.m_Panel) ? this.m_DataSourceTracker.GetHierarchicalDataSourceContext(element).dataSource : null;
		}

		internal int GetRefCount(object dataSource)
		{
			return this.m_DataSourceTracker.GetRefCount(dataSource);
		}

		internal int GetBoundElementsCount()
		{
			return this.m_BindingsTracker.GetTrackedElementsCount();
		}

		internal IEnumerable<VisualElement> GetBoundElements()
		{
			return this.m_BindingsTracker.GetBoundElements();
		}

		internal IEnumerable<VisualElement> GetUnorderedBoundElements()
		{
			return this.m_BindingsTracker.GetUnorderedBoundElements();
		}

		public DataBindingManager.IgnoreUIChangesScope IgnoreChangesScope(VisualElement target, BindingId bindingId, Binding binding)
		{
			return new DataBindingManager.IgnoreUIChangesScope(this, target, bindingId, binding);
		}

		internal List<DataBindingManager.ChangesFromUI> GetChangedDetectedFromUI()
		{
			return this.m_DetectedChangesFromUI;
		}

		internal HashSet<PropertyPath> GetChangedDetectedFromSource(object dataSource)
		{
			return this.m_DataSourceTracker.GetChangesFromSource(dataSource);
		}

		internal void ClearChangesFromSource(object dataSource)
		{
			this.m_DataSourceTracker.ClearChangesFromSource(dataSource);
		}

		internal List<DataBindingManager.BindingData> GetBindingData(VisualElement element)
		{
			DataBindingManager.BindingDataCollection bindingDataCollection;
			return (element.panel == this.m_Panel) ? (this.m_BindingsTracker.TryGetBindingCollection(element, out bindingDataCollection) ? bindingDataCollection.GetBindings() : DataBindingManager.s_Empty) : DataBindingManager.s_Empty;
		}

		internal bool TryGetBindingData(VisualElement element, in BindingId bindingId, out DataBindingManager.BindingData bindingData)
		{
			bindingData = null;
			DataBindingManager.BindingDataCollection bindingDataCollection;
			bool flag = element.panel == this.m_Panel && this.m_BindingsTracker.TryGetBindingCollection(element, out bindingDataCollection);
			bool flag2;
			if (flag)
			{
				flag2 = bindingDataCollection.TryGetBindingData(in bindingId, out bindingData);
			}
			else
			{
				bindingData = null;
				flag2 = false;
			}
			return flag2;
		}

		internal void RegisterBinding(VisualElement element, in BindingId bindingId, Binding binding)
		{
			Assert.IsFalse(binding == null);
			Assert.IsFalse((in bindingId).IsEmpty, "[UI Toolkit] Could not register binding on element of type '" + element.GetType().Name + "': target property path is empty.");
			DataBindingManager.BindingDataCollection bindingDataCollection;
			DataBindingManager.BindingData bindingData;
			bool flag = this.m_BindingsTracker.TryGetBindingCollection(element, out bindingDataCollection) && bindingDataCollection.TryGetBindingData(in bindingId, out bindingData);
			BindingActivationContext bindingActivationContext;
			if (flag)
			{
				Binding binding2 = bindingData.binding;
				bindingActivationContext = new BindingActivationContext(element, in bindingId);
				binding2.OnDeactivated(in bindingActivationContext);
				DataSourceContext resolvedDataSourceContext = this.m_DataSourceTracker.GetResolvedDataSourceContext(element, bindingData);
				IDataSourceProvider dataSourceProvider = bindingData.binding as IDataSourceProvider;
				object obj = ((dataSourceProvider != null) ? dataSourceProvider.dataSource : null);
				PropertyPath propertyPath = ((dataSourceProvider != null) ? dataSourceProvider.dataSourcePath : default(PropertyPath));
				bool flag2 = resolvedDataSourceContext.dataSource != obj || resolvedDataSourceContext.dataSourcePath != propertyPath;
				if (flag2)
				{
					Binding binding3 = bindingData.binding;
					DataSourceContext dataSourceContext = new DataSourceContext(obj, in propertyPath);
					DataSourceContextChanged dataSourceContextChanged = new DataSourceContextChanged(element, in bindingId, in resolvedDataSourceContext, in dataSourceContext);
					binding3.OnDataSourceChanged(in dataSourceContextChanged);
				}
				this.m_DataSourceTracker.DecreaseBindingRefCount(ref bindingData);
			}
			DataBindingManager.BindingData pooledBindingData = this.GetPooledBindingData(new BindingTarget(element, in bindingId), binding);
			this.m_DataSourceTracker.IncreaseBindingRefCount(ref pooledBindingData);
			this.m_BindingsTracker.StartTrackingBinding(element, pooledBindingData);
			bindingActivationContext = new BindingActivationContext(element, in bindingId);
			binding.OnActivated(in bindingActivationContext);
		}

		internal void UnregisterBinding(VisualElement element, in BindingId bindingId)
		{
			DataBindingManager.BindingDataCollection bindingDataCollection;
			bool flag = !this.m_BindingsTracker.TryGetBindingCollection(element, out bindingDataCollection);
			if (!flag)
			{
				DataBindingManager.BindingData bindingData;
				bool flag2 = bindingDataCollection.TryGetBindingData(in bindingId, out bindingData);
				if (flag2)
				{
					DataSourceContext resolvedDataSourceContext = this.m_DataSourceTracker.GetResolvedDataSourceContext(element, bindingData);
					IDataSourceProvider dataSourceProvider = bindingData.binding as IDataSourceProvider;
					object obj = ((dataSourceProvider != null) ? dataSourceProvider.dataSource : null);
					PropertyPath propertyPath = ((dataSourceProvider != null) ? dataSourceProvider.dataSourcePath : default(PropertyPath));
					bool flag3 = resolvedDataSourceContext.dataSource != obj || resolvedDataSourceContext.dataSourcePath != propertyPath;
					if (flag3)
					{
						Binding binding = bindingData.binding;
						DataSourceContext dataSourceContext = new DataSourceContext(obj, in propertyPath);
						DataSourceContextChanged dataSourceContextChanged = new DataSourceContextChanged(element, in bindingId, in resolvedDataSourceContext, in dataSourceContext);
						binding.OnDataSourceChanged(in dataSourceContextChanged);
					}
					Binding binding2 = bindingData.binding;
					BindingActivationContext bindingActivationContext = new BindingActivationContext(element, in bindingId);
					binding2.OnDeactivated(in bindingActivationContext);
					this.m_DataSourceTracker.DecreaseBindingRefCount(ref bindingData);
					this.m_BindingsTracker.StopTrackingBinding(element, bindingData);
					this.ReleasePoolBindingData(bindingData);
				}
			}
		}

		internal void TransferBindingRequests(VisualElement element)
		{
			bool flag = !this.m_BindingsTracker.IsTrackingElement(element);
			if (!flag)
			{
				DataBindingManager.BindingDataCollection bindingDataCollection;
				bool flag2 = this.m_BindingsTracker.TryGetBindingCollection(element, out bindingDataCollection);
				if (flag2)
				{
					List<DataBindingManager.BindingData> bindings = bindingDataCollection.GetBindings();
					while (bindings.Count > 0)
					{
						List<DataBindingManager.BindingData> list = bindings;
						DataBindingManager.BindingData bindingData = list[list.Count - 1];
						DataBindingManager.CreateBindingRequest(element, in bindingData.target.bindingId, bindingData.binding, true);
						this.UnregisterBinding(element, in bindingData.target.bindingId);
					}
				}
				this.m_BindingsTracker.StopTrackingElement(element);
			}
		}

		public void InvalidateCachedDataSource(HashSet<VisualElement> addedOrMovedElements, HashSet<VisualElement> removedElements)
		{
			this.m_DataSourceTracker.InvalidateCachedDataSource(addedOrMovedElements, removedElements);
		}

		public void Dispose()
		{
			this.m_BindingsTracker.Dispose();
			this.m_DataSourceTracker.Dispose();
			this.m_DetectedChangesFromUI.Clear();
		}

		public static void CreateBindingRequest(VisualElement target, in BindingId bindingId, Binding binding)
		{
			DataBindingManager.CreateBindingRequest(target, in bindingId, binding, false);
		}

		private static void CreateBindingRequest(VisualElement target, in BindingId bindingId, Binding binding, bool isTransferring)
		{
			List<DataBindingManager.BindingRequest> list = (List<DataBindingManager.BindingRequest>)target.GetProperty(DataBindingManager.k_RequestBindingPropertyName);
			bool flag = list == null;
			if (flag)
			{
				list = new List<DataBindingManager.BindingRequest>();
				target.SetProperty(DataBindingManager.k_RequestBindingPropertyName, list);
			}
			bool flag2 = true;
			for (int i = 0; i < list.Count; i++)
			{
				DataBindingManager.BindingRequest bindingRequest = list[i];
				bool flag3 = (in bindingRequest.bindingId) == (in bindingId);
				if (flag3)
				{
					if (isTransferring)
					{
						flag2 = false;
					}
					else
					{
						list[i] = bindingRequest.CancelRequest();
					}
				}
			}
			list.Add(new DataBindingManager.BindingRequest(in bindingId, binding, flag2));
		}

		public static void CreateClearAllBindingsRequest(VisualElement target)
		{
			DataBindingManager.CreateBindingRequest(target, in DataBindingManager.k_ClearBindingsToken, null);
		}

		public void ProcessBindingRequests(VisualElement element)
		{
			List<DataBindingManager.BindingRequest> list = (List<DataBindingManager.BindingRequest>)element.GetProperty(DataBindingManager.k_RequestBindingPropertyName);
			bool flag = list == null;
			if (!flag)
			{
				for (int i = 0; i < list.Count; i++)
				{
					DataBindingManager.BindingRequest bindingRequest = list[i];
					bool flag2 = !bindingRequest.shouldProcess;
					if (!flag2)
					{
						bool flag3 = (in bindingRequest.bindingId) == (in DataBindingManager.k_ClearBindingsToken);
						if (flag3)
						{
							this.ClearAllBindings(element);
						}
						else
						{
							bool flag4 = (in bindingRequest.bindingId) == (in BindingId.Invalid);
							if (flag4)
							{
								IPanel panel = element.panel;
								Panel panel2 = panel as Panel;
								string text = ((panel2 != null) ? panel2.name : null) ?? panel.visualTree.name;
								Debug.LogError(string.Concat(new string[]
								{
									"[UI Toolkit] Trying to set a binding on `",
									string.IsNullOrWhiteSpace(element.name) ? "<no name>" : element.name,
									" (",
									TypeUtility.GetTypeDisplayName(element.GetType()),
									")` without setting the \"property\" attribute is not supported (",
									text,
									")."
								}));
							}
							else
							{
								bool flag5 = bindingRequest.binding != null;
								if (flag5)
								{
									this.RegisterBinding(element, in bindingRequest.bindingId, bindingRequest.binding);
								}
								else
								{
									this.UnregisterBinding(element, in bindingRequest.bindingId);
								}
							}
						}
					}
				}
				list.Clear();
			}
		}

		private void ClearAllBindings(VisualElement element)
		{
			List<DataBindingManager.BindingData> list = CollectionPool<List<DataBindingManager.BindingData>, DataBindingManager.BindingData>.Get();
			try
			{
				list.AddRange(this.GetBindingData(element));
				foreach (DataBindingManager.BindingData bindingData in list)
				{
					this.UnregisterBinding(element, in bindingData.target.bindingId);
				}
			}
			finally
			{
				CollectionPool<List<DataBindingManager.BindingData>, DataBindingManager.BindingData>.Release(list);
			}
		}

		internal static bool AnyPendingBindingRequests(VisualElement element)
		{
			List<DataBindingManager.BindingRequest> list = (List<DataBindingManager.BindingRequest>)element.GetProperty(DataBindingManager.k_RequestBindingPropertyName);
			bool flag = list == null;
			return !flag && list.Count > 0;
		}

		internal static void GetBindingRequests(VisualElement element, [TupleElementNames(new string[] { "binding", "bindingId" })] List<ValueTuple<Binding, BindingId>> bindingRequests)
		{
			List<DataBindingManager.BindingRequest> list = (List<DataBindingManager.BindingRequest>)element.GetProperty(DataBindingManager.k_RequestBindingPropertyName);
			bool flag = list == null;
			if (!flag)
			{
				HashSet<BindingId> hashSet = CollectionPool<HashSet<BindingId>, BindingId>.Get();
				try
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						DataBindingManager.BindingRequest bindingRequest = list[i];
						bool flag2 = hashSet.Add(bindingRequest.bindingId);
						if (flag2)
						{
							bindingRequests.Add(new ValueTuple<Binding, BindingId>(bindingRequest.binding, bindingRequest.bindingId));
						}
					}
				}
				finally
				{
					CollectionPool<HashSet<BindingId>, BindingId>.Release(hashSet);
				}
			}
		}

		internal static bool TryGetBindingRequest(VisualElement element, in BindingId bindingId, out Binding binding)
		{
			List<DataBindingManager.BindingRequest> list = (List<DataBindingManager.BindingRequest>)element.GetProperty(DataBindingManager.k_RequestBindingPropertyName);
			bool flag = list == null;
			bool flag2;
			if (flag)
			{
				binding = null;
				flag2 = false;
			}
			else
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					DataBindingManager.BindingRequest bindingRequest = list[i];
					bool flag3 = (in bindingId) != (in bindingRequest.bindingId);
					if (!flag3)
					{
						binding = bindingRequest.binding;
						return true;
					}
				}
				binding = null;
				flag2 = false;
			}
			return flag2;
		}

		public void DirtyBindingOrder()
		{
			this.m_BindingsTracker.SetDirty();
		}

		public void TrackDataSource(object previous, object current)
		{
			this.m_DataSourceTracker.DecreaseRefCount(previous);
			this.m_DataSourceTracker.IncreaseRefCount(current);
		}

		[return: TupleElementNames(new string[] { "boundElementsCount", "trackedDataSourcesCount" })]
		internal ValueTuple<int, int> GetTrackedInfo()
		{
			int trackedElementsCount = this.m_BindingsTracker.GetTrackedElementsCount();
			int trackedDataSourcesCount = this.m_DataSourceTracker.GetTrackedDataSourcesCount();
			return new ValueTuple<int, int>(trackedElementsCount, trackedDataSourcesCount);
		}

		public void ClearSourceCache()
		{
			this.m_DataSourceTracker.ClearSourceCache();
		}

		public DataBindingManager.BindingData GetPooledBindingData(BindingTarget target, Binding binding)
		{
			bool flag = this.m_BindingDataLocalPool.Count > 0;
			DataBindingManager.BindingData bindingData;
			if (flag)
			{
				List<DataBindingManager.BindingData> bindingDataLocalPool = this.m_BindingDataLocalPool;
				bindingData = bindingDataLocalPool[bindingDataLocalPool.Count - 1];
				this.m_BindingDataLocalPool.RemoveAt(this.m_BindingDataLocalPool.Count - 1);
			}
			else
			{
				bindingData = new DataBindingManager.BindingData();
			}
			bindingData.target = target;
			bindingData.binding = binding;
			return bindingData;
		}

		public void ReleasePoolBindingData(DataBindingManager.BindingData data)
		{
			data.Reset();
			this.m_BindingDataLocalPool.Add(data);
		}

		private readonly List<DataBindingManager.BindingData> m_BindingDataLocalPool = new List<DataBindingManager.BindingData>(64);

		private static readonly PropertyName k_RequestBindingPropertyName = "__unity-binding-request";

		private static readonly BindingId k_ClearBindingsToken = "$__BindingManager--ClearAllBindings";

		internal static BindingLogLevel globalLogLevel = BindingLogLevel.All;

		private BindingLogLevel? m_LogLevel;

		private static readonly List<DataBindingManager.BindingData> s_Empty = new List<DataBindingManager.BindingData>();

		private readonly BaseVisualElementPanel m_Panel;

		private readonly DataBindingManager.HierarchyDataSourceTracker m_DataSourceTracker;

		private readonly DataBindingManager.HierarchyBindingTracker m_BindingsTracker;

		private readonly List<DataBindingManager.ChangesFromUI> m_DetectedChangesFromUI;

		private DataBindingManager.IgnoreUIChangesData m_IgnoreUIChangesData;

		private readonly struct BindingRequest
		{
			public BindingRequest(in BindingId bindingId, Binding binding, bool shouldProcess = true)
			{
				this.bindingId = bindingId;
				this.binding = binding;
				this.shouldProcess = shouldProcess;
			}

			public DataBindingManager.BindingRequest CancelRequest()
			{
				return new DataBindingManager.BindingRequest(in this.bindingId, this.binding, false);
			}

			public readonly BindingId bindingId;

			public readonly Binding binding;

			public readonly bool shouldProcess;
		}

		private struct BindingDataCollection : IDisposable
		{
			public static DataBindingManager.BindingDataCollection Create()
			{
				return new DataBindingManager.BindingDataCollection
				{
					m_BindingPerId = CollectionPool<Dictionary<BindingId, DataBindingManager.BindingData>, KeyValuePair<BindingId, DataBindingManager.BindingData>>.Get(),
					m_Bindings = CollectionPool<List<DataBindingManager.BindingData>, DataBindingManager.BindingData>.Get()
				};
			}

			public void AddBindingData(DataBindingManager.BindingData bindingData)
			{
				DataBindingManager.BindingData bindingData2;
				bool flag = this.m_BindingPerId.TryGetValue(bindingData.target.bindingId, out bindingData2);
				if (flag)
				{
					this.m_Bindings.Remove(bindingData2);
				}
				this.m_BindingPerId[bindingData.target.bindingId] = bindingData;
				this.m_Bindings.Add(bindingData);
			}

			public bool TryGetBindingData(in BindingId bindingId, out DataBindingManager.BindingData data)
			{
				return this.m_BindingPerId.TryGetValue(bindingId, out data);
			}

			public bool RemoveBindingData(DataBindingManager.BindingData bindingData)
			{
				DataBindingManager.BindingData bindingData2;
				bool flag = !this.m_BindingPerId.TryGetValue(bindingData.target.bindingId, out bindingData2);
				return !flag && this.m_Bindings.Remove(bindingData2) && this.m_BindingPerId.Remove(bindingData2.target.bindingId);
			}

			public List<DataBindingManager.BindingData> GetBindings()
			{
				return this.m_Bindings;
			}

			public int GetBindingCount()
			{
				return this.m_Bindings.Count;
			}

			public void Dispose()
			{
				bool flag = this.m_BindingPerId != null;
				if (flag)
				{
					CollectionPool<Dictionary<BindingId, DataBindingManager.BindingData>, KeyValuePair<BindingId, DataBindingManager.BindingData>>.Release(this.m_BindingPerId);
				}
				this.m_BindingPerId = null;
				bool flag2 = this.m_Bindings != null;
				if (flag2)
				{
					CollectionPool<List<DataBindingManager.BindingData>, DataBindingManager.BindingData>.Release(this.m_Bindings);
				}
				this.m_Bindings = null;
			}

			private Dictionary<BindingId, DataBindingManager.BindingData> m_BindingPerId;

			private List<DataBindingManager.BindingData> m_Bindings;
		}

		internal class BindingData
		{
			public object localDataSource { get; set; }

			public void Reset()
			{
				this.version += 1L;
				this.target = default(BindingTarget);
				this.binding = null;
				this.localDataSource = null;
				this.m_LastContext = default(DataSourceContext);
				this.m_SourceToUILastUpdate = null;
				this.m_UIToSourceLastUpdate = null;
			}

			public DataSourceContext context
			{
				get
				{
					return this.m_LastContext;
				}
				set
				{
					bool flag = this.m_LastContext.dataSource == value.dataSource && this.m_LastContext.dataSourcePath == value.dataSourcePath;
					if (!flag)
					{
						DataSourceContext lastContext = this.m_LastContext;
						this.m_LastContext = value;
						Binding binding = this.binding;
						DataSourceContextChanged dataSourceContextChanged = new DataSourceContextChanged(this.target.element, in this.target.bindingId, in lastContext, in value);
						binding.OnDataSourceChanged(in dataSourceContextChanged);
						this.binding.MarkDirty();
					}
				}
			}

			public long version;

			public BindingTarget target;

			public Binding binding;

			private DataSourceContext m_LastContext;

			public BindingResult? m_SourceToUILastUpdate;

			public BindingResult? m_UIToSourceLastUpdate;
		}

		internal readonly struct ChangesFromUI
		{
			public ChangesFromUI(DataBindingManager.BindingData bindingData)
			{
				this.bindingData = bindingData;
				this.version = bindingData.version;
				this.binding = bindingData.binding;
			}

			public bool IsValid
			{
				get
				{
					return this.version == this.bindingData.version && this.binding == this.bindingData.binding;
				}
			}

			public readonly long version;

			public readonly Binding binding;

			public readonly DataBindingManager.BindingData bindingData;
		}

		private class HierarchyBindingTracker : IDisposable
		{
			public int GetTrackedElementsCount()
			{
				return this.m_BoundElements.Count;
			}

			public List<VisualElement> GetBoundElements()
			{
				bool isDirty = this.m_IsDirty;
				if (isDirty)
				{
					this.OrderBindings(this.m_Panel.visualTree);
				}
				return this.m_OrderedBindings;
			}

			public IEnumerable<VisualElement> GetUnorderedBoundElements()
			{
				return this.m_BoundElements;
			}

			public HierarchyBindingTracker(BaseVisualElementPanel panel)
			{
				this.m_Panel = panel;
				this.m_BindingSorter = new DataBindingManager.HierarchyBindingTracker.HierarchicalBindingsSorter();
				this.m_BindingDataPerElement = new Dictionary<VisualElement, DataBindingManager.BindingDataCollection>();
				this.m_BoundElements = new HashSet<VisualElement>();
				this.m_OrderedBindings = new List<VisualElement>();
				this.m_IsDirty = true;
				this.m_OnPropertyChanged = new EventCallback<PropertyChangedEvent, Dictionary<VisualElement, DataBindingManager.BindingDataCollection>>(this.OnPropertyChanged);
			}

			public void SetDirty()
			{
				this.m_IsDirty = true;
			}

			public bool TryGetBindingCollection(VisualElement element, out DataBindingManager.BindingDataCollection collection)
			{
				return this.m_BindingDataPerElement.TryGetValue(element, out collection);
			}

			public bool IsTrackingElement(VisualElement element)
			{
				return this.m_BoundElements.Contains(element);
			}

			public void StartTrackingBinding(VisualElement element, DataBindingManager.BindingData binding)
			{
				bool flag = this.m_BoundElements.Add(element);
				DataBindingManager.BindingDataCollection bindingDataCollection;
				if (flag)
				{
					bindingDataCollection = DataBindingManager.BindingDataCollection.Create();
					this.m_BindingDataPerElement.Add(element, bindingDataCollection);
					element.RegisterCallback<PropertyChangedEvent, Dictionary<VisualElement, DataBindingManager.BindingDataCollection>>(this.m_OnPropertyChanged, this.m_BindingDataPerElement, TrickleDown.NoTrickleDown);
				}
				else
				{
					bool flag2 = !this.m_BindingDataPerElement.TryGetValue(element, out bindingDataCollection);
					if (flag2)
					{
						throw new InvalidOperationException("Trying to add a binding to an element which doesn't have a binding collection. This is an internal bug. Please report using `Help > Report a Bug...`");
					}
				}
				binding.binding.MarkDirty();
				bindingDataCollection.AddBindingData(binding);
				this.m_BindingDataPerElement[element] = bindingDataCollection;
				this.SetDirty();
			}

			private void OnPropertyChanged(PropertyChangedEvent evt, Dictionary<VisualElement, DataBindingManager.BindingDataCollection> bindingCollection)
			{
				VisualElement visualElement = evt.target as VisualElement;
				bool flag = visualElement == null;
				if (flag)
				{
					throw new InvalidOperationException("Trying to track property changes on a non 'VisualElement'. This is an internal bug. Please report using `Help > Report a Bug...`");
				}
				DataBindingManager.BindingDataCollection bindingDataCollection;
				bool flag2 = !bindingCollection.TryGetValue(visualElement, out bindingDataCollection);
				if (flag2)
				{
					throw new InvalidOperationException("Trying to track property changes on a 'VisualElement' that is not being tracked. This is an internal bug. Please report using `Help > Report a Bug...`");
				}
				BindingId property = evt.property;
				DataBindingManager.BindingData bindingData;
				Binding binding;
				bool flag3 = bindingDataCollection.TryGetBindingData(in property, out bindingData) && visualElement.TryGetBinding(evt.property, out binding) && bindingData.binding == binding;
				if (flag3)
				{
					bool flag4 = !this.m_Panel.dataBindingManager.m_IgnoreUIChangesData.ShouldIgnoreChange(visualElement, binding, evt.property);
					if (flag4)
					{
						this.m_Panel.dataBindingManager.m_DetectedChangesFromUI.Add(new DataBindingManager.ChangesFromUI(bindingData));
					}
				}
			}

			public void StopTrackingBinding(VisualElement element, DataBindingManager.BindingData binding)
			{
				DataBindingManager.BindingDataCollection bindingDataCollection;
				bool flag = this.m_BoundElements.Contains(element) && this.m_BindingDataPerElement.TryGetValue(element, out bindingDataCollection);
				if (flag)
				{
					bindingDataCollection.RemoveBindingData(binding);
					bool flag2 = bindingDataCollection.GetBindingCount() == 0;
					if (flag2)
					{
						this.StopTrackingElement(element);
						element.UnregisterCallback<PropertyChangedEvent, Dictionary<VisualElement, DataBindingManager.BindingDataCollection>>(this.m_OnPropertyChanged, TrickleDown.NoTrickleDown);
					}
					else
					{
						this.m_BindingDataPerElement[element] = bindingDataCollection;
					}
					this.SetDirty();
					return;
				}
				throw new InvalidOperationException("Trying to remove a binding to an element which doesn't have a binding collection. This is an internal bug. Please report using `Help > Report a Bug...`");
			}

			public void StopTrackingElement(VisualElement element)
			{
				DataBindingManager.BindingDataCollection bindingDataCollection;
				bool flag = this.m_BindingDataPerElement.TryGetValue(element, out bindingDataCollection);
				if (flag)
				{
					bindingDataCollection.Dispose();
				}
				this.m_BindingDataPerElement.Remove(element);
				this.m_BoundElements.Remove(element);
				this.SetDirty();
			}

			public void Dispose()
			{
				foreach (KeyValuePair<VisualElement, DataBindingManager.BindingDataCollection> keyValuePair in this.m_BindingDataPerElement)
				{
					keyValuePair.Value.Dispose();
				}
				this.m_BindingDataPerElement.Clear();
				this.m_BoundElements.Clear();
				this.m_OrderedBindings.Clear();
			}

			private void OrderBindings(VisualElement root)
			{
				this.m_OrderedBindings.Clear();
				this.m_BindingSorter.boundElements = this.m_BoundElements;
				this.m_BindingSorter.results = this.m_OrderedBindings;
				this.m_BindingSorter.Traverse(root);
				this.m_IsDirty = false;
			}

			private readonly BaseVisualElementPanel m_Panel;

			private readonly DataBindingManager.HierarchyBindingTracker.HierarchicalBindingsSorter m_BindingSorter;

			private readonly Dictionary<VisualElement, DataBindingManager.BindingDataCollection> m_BindingDataPerElement;

			private readonly HashSet<VisualElement> m_BoundElements;

			private readonly List<VisualElement> m_OrderedBindings;

			private bool m_IsDirty;

			private EventCallback<PropertyChangedEvent, Dictionary<VisualElement, DataBindingManager.BindingDataCollection>> m_OnPropertyChanged;

			private class HierarchicalBindingsSorter : HierarchyTraversal
			{
				public HashSet<VisualElement> boundElements { get; set; }

				public List<VisualElement> results { get; set; }

				public override void TraverseRecursive(VisualElement element, int depth)
				{
					bool flag = this.boundElements.Count == this.results.Count;
					if (!flag)
					{
						bool flag2 = this.boundElements.Contains(element);
						if (flag2)
						{
							this.results.Add(element);
						}
						base.Recurse(element, depth);
					}
				}
			}
		}

		private class HierarchyDataSourceTracker : IDisposable
		{
			private DataBindingManager.HierarchyDataSourceTracker.SourceInfo GetPooledSourceInfo()
			{
				bool flag = this.m_SourceInfosPool.Count > 0;
				DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
				if (flag)
				{
					List<DataBindingManager.HierarchyDataSourceTracker.SourceInfo> sourceInfosPool = this.m_SourceInfosPool;
					sourceInfo = sourceInfosPool[sourceInfosPool.Count - 1];
					this.m_SourceInfosPool.RemoveAt(this.m_SourceInfosPool.Count - 1);
				}
				else
				{
					sourceInfo = new DataBindingManager.HierarchyDataSourceTracker.SourceInfo();
				}
				return sourceInfo;
			}

			private void ReleasePooledSourceInfo(DataBindingManager.HierarchyDataSourceTracker.SourceInfo info)
			{
				info.lastVersion = long.MinValue;
				info.refCount = 0;
				HashSet<PropertyPath> detectedChangesNoAlloc = info.detectedChangesNoAlloc;
				if (detectedChangesNoAlloc != null)
				{
					detectedChangesNoAlloc.Clear();
				}
				this.m_SourceInfosPool.Add(info);
			}

			public HierarchyDataSourceTracker(DataBindingManager manager)
			{
				this.m_DataBindingManager = manager;
				this.m_ResolvedHierarchicalDataSourceContext = new Dictionary<VisualElement, DataSourceContext>();
				this.m_BindingRefCount = new Dictionary<Binding, int>();
				DataBindingManager.HierarchyDataSourceTracker.ObjectComparer objectComparer = new DataBindingManager.HierarchyDataSourceTracker.ObjectComparer();
				this.m_SourceInfos = new Dictionary<object, DataBindingManager.HierarchyDataSourceTracker.SourceInfo>(objectComparer);
				this.m_SourcesToRemove = new HashSet<object>(objectComparer);
				this.m_InvalidateResolvedDataSources = new DataBindingManager.HierarchyDataSourceTracker.InvalidateDataSourcesTraversal(this);
				this.m_Handler = new EventHandler<BindablePropertyChangedEventArgs>(this.TrackPropertyChanges);
				this.m_VisualElementHandler = new EventCallback<PropertyChangedEvent, VisualElement>(this.OnVisualElementPropertyChanged);
			}

			internal void IncreaseBindingRefCount(ref DataBindingManager.BindingData bindingData)
			{
				Binding binding = bindingData.binding;
				bool flag = binding == null;
				if (!flag)
				{
					int num;
					bool flag2 = !this.m_BindingRefCount.TryGetValue(binding, out num);
					if (flag2)
					{
						num = 0;
					}
					IDataSourceProvider dataSourceProvider = binding as IDataSourceProvider;
					bool flag3 = dataSourceProvider != null;
					if (flag3)
					{
						this.IncreaseRefCount(dataSourceProvider.dataSource);
						bindingData.localDataSource = dataSourceProvider.dataSource;
					}
					this.m_BindingRefCount[binding] = num + 1;
				}
			}

			internal void DecreaseBindingRefCount(ref DataBindingManager.BindingData bindingData)
			{
				Binding binding = bindingData.binding;
				bool flag = binding == null;
				if (!flag)
				{
					int num;
					bool flag2 = !this.m_BindingRefCount.TryGetValue(binding, out num);
					if (flag2)
					{
						throw new InvalidOperationException("Trying to release a binding that isn't tracked. This is an internal bug. Please report using `Help > Report a Bug...`");
					}
					bool flag3 = num == 1;
					if (flag3)
					{
						this.m_BindingRefCount.Remove(binding);
					}
					else
					{
						this.m_BindingRefCount[binding] = num - 1;
					}
					IDataSourceProvider dataSourceProvider = binding as IDataSourceProvider;
					bool flag4 = dataSourceProvider != null;
					if (flag4)
					{
						this.DecreaseRefCount(dataSourceProvider.dataSource);
					}
				}
			}

			internal void IncreaseRefCount(object dataSource)
			{
				bool flag = dataSource == null;
				if (!flag)
				{
					bool flag2 = this.m_SourcesToRemove.Remove(dataSource);
					DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
					bool flag3 = !this.m_SourceInfos.TryGetValue(dataSource, out sourceInfo);
					if (flag3)
					{
						sourceInfo = (this.m_SourceInfos[dataSource] = this.GetPooledSourceInfo());
						flag2 = true;
					}
					bool flag4 = flag2;
					if (flag4)
					{
						INotifyBindablePropertyChanged notifyBindablePropertyChanged = dataSource as INotifyBindablePropertyChanged;
						bool flag5 = notifyBindablePropertyChanged != null;
						if (flag5)
						{
							notifyBindablePropertyChanged.propertyChanged += this.m_Handler;
						}
						VisualElement visualElement = dataSource as VisualElement;
						bool flag6 = visualElement != null;
						if (flag6)
						{
							visualElement.RegisterCallback<PropertyChangedEvent, VisualElement>(this.m_VisualElementHandler, visualElement, TrickleDown.NoTrickleDown);
						}
					}
					DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo2 = sourceInfo;
					int num = sourceInfo2.refCount + 1;
					sourceInfo2.refCount = num;
				}
			}

			private void OnVisualElementPropertyChanged(PropertyChangedEvent evt, VisualElement element)
			{
				BindingId property = evt.property;
				this.TrackPropertyChanges(element, in property);
			}

			internal void DecreaseRefCount(object dataSource)
			{
				bool flag = dataSource == null;
				if (!flag)
				{
					DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
					bool flag2 = !this.m_SourceInfos.TryGetValue(dataSource, out sourceInfo) || sourceInfo.refCount == 0;
					if (flag2)
					{
						throw new InvalidOperationException("Trying to release a data source that isn't tracked. This is an internal bug. Please report using `Help > Report a Bug...`");
					}
					bool flag3 = sourceInfo.refCount == 1;
					if (flag3)
					{
						sourceInfo.refCount = 0;
						this.m_SourcesToRemove.Add(dataSource);
						INotifyBindablePropertyChanged notifyBindablePropertyChanged = dataSource as INotifyBindablePropertyChanged;
						bool flag4 = notifyBindablePropertyChanged != null;
						if (flag4)
						{
							notifyBindablePropertyChanged.propertyChanged -= this.m_Handler;
						}
						VisualElement visualElement = dataSource as VisualElement;
						bool flag5 = visualElement != null;
						if (flag5)
						{
							visualElement.UnregisterCallback<PropertyChangedEvent, VisualElement>(this.m_VisualElementHandler, TrickleDown.NoTrickleDown);
						}
					}
					else
					{
						DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo2 = sourceInfo;
						int num = sourceInfo2.refCount - 1;
						sourceInfo2.refCount = num;
					}
				}
			}

			public int GetRefCount(object dataSource)
			{
				DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
				return this.m_SourceInfos.TryGetValue(dataSource, out sourceInfo) ? sourceInfo.refCount : 0;
			}

			public int GetTrackedDataSourcesCount()
			{
				return this.m_ResolvedHierarchicalDataSourceContext.Count;
			}

			public bool IsTrackingDataSource(VisualElement element)
			{
				return this.m_ResolvedHierarchicalDataSourceContext.ContainsKey(element);
			}

			public HashSet<PropertyPath> GetChangesFromSource(object dataSource)
			{
				DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
				return this.m_SourceInfos.TryGetValue(dataSource, out sourceInfo) ? sourceInfo.detectedChangesNoAlloc : null;
			}

			public void ClearChangesFromSource(object dataSource)
			{
				DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
				bool flag = !this.m_SourceInfos.TryGetValue(dataSource, out sourceInfo);
				if (!flag)
				{
					HashSet<PropertyPath> detectedChangesNoAlloc = sourceInfo.detectedChangesNoAlloc;
					if (detectedChangesNoAlloc != null)
					{
						detectedChangesNoAlloc.Clear();
					}
				}
			}

			public void InvalidateCachedDataSource(HashSet<VisualElement> elements, HashSet<VisualElement> removedElements)
			{
				List<VisualElement> list = CollectionPool<List<VisualElement>, VisualElement>.Get();
				try
				{
					foreach (VisualElement visualElement in elements)
					{
						list.Add(visualElement);
					}
					this.m_InvalidateResolvedDataSources.Invalidate(list, removedElements);
				}
				finally
				{
					CollectionPool<List<VisualElement>, VisualElement>.Release(list);
				}
			}

			public DataSourceContext GetResolvedDataSourceContext(VisualElement element, DataBindingManager.BindingData bindingData)
			{
				object obj = null;
				PropertyPath propertyPath = default(PropertyPath);
				IDataSourceProvider dataSourceProvider = bindingData.binding as IDataSourceProvider;
				bool flag = dataSourceProvider != null;
				if (flag)
				{
					obj = dataSourceProvider.dataSource;
					propertyPath = dataSourceProvider.dataSourcePath;
				}
				object localDataSource = bindingData.localDataSource;
				object obj2 = obj;
				PropertyPath propertyPath2 = propertyPath;
				try
				{
					bool flag2 = obj == null;
					if (flag2)
					{
						this.DecreaseRefCount(localDataSource);
						DataSourceContext hierarchicalDataSourceContext = this.GetHierarchicalDataSourceContext(element);
						obj2 = hierarchicalDataSourceContext.dataSource;
						PropertyPath propertyPath3;
						if (propertyPath.IsEmpty)
						{
							propertyPath3 = hierarchicalDataSourceContext.dataSourcePath;
						}
						else
						{
							PropertyPath dataSourcePath = hierarchicalDataSourceContext.dataSourcePath;
							propertyPath3 = PropertyPath.Combine(in dataSourcePath, in propertyPath);
						}
						propertyPath2 = propertyPath3;
						return new DataSourceContext(obj2, in propertyPath2);
					}
					bool flag3 = obj != localDataSource;
					if (flag3)
					{
						this.DecreaseRefCount(localDataSource);
						this.IncreaseRefCount(obj);
					}
				}
				finally
				{
					bindingData.localDataSource = obj;
					DataSourceContext dataSourceContext = new DataSourceContext(obj2, in propertyPath2);
					bindingData.context = dataSourceContext;
				}
				return new DataSourceContext(obj2, in propertyPath2);
			}

			private void TrackPropertyChanges(object sender, BindablePropertyChangedEventArgs args)
			{
				BindingId propertyName = args.propertyName;
				this.TrackPropertyChanges(sender, in propertyName);
			}

			private void TrackPropertyChanges(object sender, PropertyPath propertyPath)
			{
				DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
				bool flag = !this.m_SourceInfos.TryGetValue(sender, out sourceInfo);
				if (!flag)
				{
					HashSet<PropertyPath> detectedChanges = sourceInfo.detectedChanges;
					detectedChanges.Add(propertyPath);
				}
			}

			public bool TryGetLastVersion(object source, out long version)
			{
				DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
				bool flag = source != null && this.m_SourceInfos.TryGetValue(source, out sourceInfo);
				bool flag2;
				if (flag)
				{
					version = sourceInfo.lastVersion;
					flag2 = true;
				}
				else
				{
					version = -1L;
					flag2 = false;
				}
				return flag2;
			}

			public void UpdateVersion(object source, long version)
			{
				DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo = this.m_SourceInfos[source];
				sourceInfo.lastVersion = version;
				this.m_SourceInfos[source] = sourceInfo;
			}

			internal object GetHierarchyDataSource(VisualElement element)
			{
				return this.GetHierarchicalDataSourceContext(element).dataSource;
			}

			internal DataSourceContext GetHierarchicalDataSourceContext(VisualElement element)
			{
				DataSourceContext dataSourceContext;
				bool flag = this.m_ResolvedHierarchicalDataSourceContext.TryGetValue(element, out dataSourceContext);
				DataSourceContext dataSourceContext2;
				if (flag)
				{
					dataSourceContext2 = dataSourceContext;
				}
				else
				{
					VisualElement visualElement = element;
					PropertyPath propertyPath = default(PropertyPath);
					while (visualElement != null)
					{
						bool flag2 = !visualElement.isDataSourcePathEmpty;
						if (flag2)
						{
							PropertyPath dataSourcePath = visualElement.dataSourcePath;
							propertyPath = PropertyPath.Combine(in dataSourcePath, in propertyPath);
						}
						bool flag3 = visualElement.dataSource != null;
						if (flag3)
						{
							object dataSource = visualElement.dataSource;
							return this.m_ResolvedHierarchicalDataSourceContext[element] = new DataSourceContext(dataSource, in propertyPath);
						}
						visualElement = visualElement.hierarchy.parent;
					}
					dataSourceContext2 = (this.m_ResolvedHierarchicalDataSourceContext[element] = new DataSourceContext(null, in propertyPath));
				}
				return dataSourceContext2;
			}

			internal void RemoveHierarchyDataSourceContextFromElement(VisualElement element)
			{
				this.m_ResolvedHierarchicalDataSourceContext.Remove(element);
			}

			public void Dispose()
			{
				this.m_ResolvedHierarchicalDataSourceContext.Clear();
				this.m_BindingRefCount.Clear();
				this.m_SourcesToRemove.Clear();
				this.m_SourceInfosPool.Clear();
				this.m_SourceInfos.Clear();
			}

			public void ClearSourceCache()
			{
				foreach (object obj in this.m_SourcesToRemove)
				{
					DataBindingManager.HierarchyDataSourceTracker.SourceInfo sourceInfo;
					bool flag = this.m_SourceInfos.TryGetValue(obj, out sourceInfo);
					if (!flag)
					{
						throw new InvalidOperationException("Trying to release a data source that isn't tracked. This is an internal bug. Please report using `Help > Report a Bug...`");
					}
					bool flag2 = sourceInfo.refCount == 0;
					if (!flag2)
					{
						throw new InvalidOperationException("Trying to release a data source that is still being referenced. This is an internal bug. Please report using `Help > Report a Bug...`");
					}
					this.m_SourceInfos.Remove(obj);
					this.ReleasePooledSourceInfo(sourceInfo);
				}
				this.m_SourcesToRemove.Clear();
			}

			private readonly List<DataBindingManager.HierarchyDataSourceTracker.SourceInfo> m_SourceInfosPool = new List<DataBindingManager.HierarchyDataSourceTracker.SourceInfo>();

			private readonly DataBindingManager m_DataBindingManager;

			private readonly Dictionary<VisualElement, DataSourceContext> m_ResolvedHierarchicalDataSourceContext;

			private readonly Dictionary<Binding, int> m_BindingRefCount;

			private readonly Dictionary<object, DataBindingManager.HierarchyDataSourceTracker.SourceInfo> m_SourceInfos;

			private readonly HashSet<object> m_SourcesToRemove;

			private readonly DataBindingManager.HierarchyDataSourceTracker.InvalidateDataSourcesTraversal m_InvalidateResolvedDataSources;

			private readonly EventHandler<BindablePropertyChangedEventArgs> m_Handler;

			private readonly EventCallback<PropertyChangedEvent, VisualElement> m_VisualElementHandler;

			private class SourceInfo
			{
				public long lastVersion { get; set; }

				public int refCount { get; set; }

				public HashSet<PropertyPath> detectedChanges
				{
					get
					{
						HashSet<PropertyPath> hashSet;
						if ((hashSet = this.m_DetectedChanges) == null)
						{
							hashSet = (this.m_DetectedChanges = new HashSet<PropertyPath>());
						}
						return hashSet;
					}
				}

				public HashSet<PropertyPath> detectedChangesNoAlloc
				{
					get
					{
						return this.m_DetectedChanges;
					}
				}

				private HashSet<PropertyPath> m_DetectedChanges;
			}

			private class InvalidateDataSourcesTraversal : HierarchyTraversal
			{
				public InvalidateDataSourcesTraversal(DataBindingManager.HierarchyDataSourceTracker dataSourceTracker)
				{
					this.m_DataSourceTracker = dataSourceTracker;
					this.m_VisitedElements = new HashSet<VisualElement>();
				}

				public void Invalidate(List<VisualElement> addedOrMovedElements, HashSet<VisualElement> removedElements)
				{
					this.m_VisitedElements.Clear();
					for (int i = 0; i < addedOrMovedElements.Count; i++)
					{
						VisualElement visualElement = addedOrMovedElements[i];
						this.Traverse(visualElement);
					}
					foreach (VisualElement visualElement2 in removedElements)
					{
						bool flag = this.m_VisitedElements.Contains(visualElement2);
						if (!flag)
						{
							this.Traverse(visualElement2);
						}
					}
				}

				public override void TraverseRecursive(VisualElement element, int depth)
				{
					bool flag = this.m_VisitedElements.Contains(element);
					if (!flag)
					{
						bool flag2 = depth > 0 && element.dataSource != null;
						if (!flag2)
						{
							this.m_VisitedElements.Add(element);
							this.m_DataSourceTracker.RemoveHierarchyDataSourceContextFromElement(element);
							base.Recurse(element, depth);
						}
					}
				}

				private readonly DataBindingManager.HierarchyDataSourceTracker m_DataSourceTracker;

				private readonly HashSet<VisualElement> m_VisitedElements;
			}

			private class ObjectComparer : IEqualityComparer<object>
			{
				bool IEqualityComparer<object>.Equals(object x, object y)
				{
					return x == y || EqualityComparer<object>.Default.Equals(x, y);
				}

				int IEqualityComparer<object>.GetHashCode(object obj)
				{
					return RuntimeHelpers.GetHashCode(obj);
				}
			}
		}

		private struct IgnoreUIChangesData
		{
			public bool ShouldIgnoreChange(VisualElement ve, Binding b, BindingId id)
			{
				return this.element == ve && this.binding == b && (in this.bindingId) == (in id);
			}

			public VisualElement element;

			public Binding binding;

			public BindingId bindingId;
		}

		public struct IgnoreUIChangesScope : IDisposable
		{
			internal IgnoreUIChangesScope(DataBindingManager manager, VisualElement target, BindingId bindingId, Binding binding)
			{
				this.manager = manager;
				this.m_ScopeData = this.manager.m_IgnoreUIChangesData;
				this.manager.m_IgnoreUIChangesData = new DataBindingManager.IgnoreUIChangesData
				{
					element = target,
					binding = binding,
					bindingId = bindingId
				};
			}

			public void Dispose()
			{
				this.manager.m_IgnoreUIChangesData = this.m_ScopeData;
			}

			private DataBindingManager.IgnoreUIChangesData m_ScopeData;

			private DataBindingManager manager;
		}
	}
}
