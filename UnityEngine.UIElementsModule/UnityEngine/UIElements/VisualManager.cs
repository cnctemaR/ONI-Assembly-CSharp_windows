using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using AOT;
using JetBrains.Annotations;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualManager.h")]
	internal sealed class VisualManager : IDisposable
	{
		public static VisualManager SharedManager { get; private set; }

		static VisualManager()
		{
			VisualManager.Initialize();
		}

		private static void Initialize()
		{
			bool flag = VisualManager.s_Initialized;
			if (!flag)
			{
				VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeData>();
				VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodePseudoStateData>();
				VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeClassData>();
				VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeRenderData>();
				VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeTextData>();
				VisualNodePropertyRegistry.RegisterInternalProperty<VisualNodeImguiData>();
				VisualManager.s_Initialized = true;
				bool flag2 = !VisualManager.s_AppDomainUnloadRegistered;
				if (flag2)
				{
					AppDomain.CurrentDomain.DomainUnload += delegate(object _, EventArgs __)
					{
						bool flag3 = VisualManager.s_Initialized;
						if (flag3)
						{
							VisualManager.Shutdown();
						}
					};
					VisualManager.s_AppDomainUnloadRegistered = true;
				}
				VisualManager.SharedManager = new VisualManager();
			}
		}

		private static void Shutdown()
		{
			bool flag = !VisualManager.s_Initialized;
			if (!flag)
			{
				VisualManager.s_Initialized = false;
				VisualManager.SharedManager.Dispose();
			}
		}

		private int RegisterCallbackInstance(VisualManager instance)
		{
			for (int i = 0; i < VisualManager.s_CallbackInstances.Count; i++)
			{
				VisualManager visualManager;
				bool flag = !VisualManager.s_CallbackInstances[i].TryGetTarget(out visualManager);
				if (flag)
				{
					VisualManager.s_CallbackInstances[i] = new WeakReference<VisualManager>(instance);
					return i + 1;
				}
			}
			VisualManager.s_CallbackInstances.Add(new WeakReference<VisualManager>(this));
			return VisualManager.s_CallbackInstances.Count;
		}

		private void UnregisterCallbackInstance(int id)
		{
			VisualManager.s_CallbackInstances[id - 1] = null;
		}

		[MonoPInvokeCallback(typeof(VisualManager.NativeHierarchyChangedDelegate))]
		private static void InvokeHierarchyChanged(IntPtr instance, in VisualNodeHandle handle, HierarchyChangeType type)
		{
			for (int i = 0; i < VisualManager.s_CallbackInstances.Count; i++)
			{
				VisualManager visualManager;
				bool flag = VisualManager.s_CallbackInstances[i].TryGetTarget(out visualManager) && visualManager.m_Ptr == instance;
				if (flag)
				{
					visualManager.OnHierarchyChanged(visualManager, in handle, type);
				}
			}
		}

		[MonoPInvokeCallback(typeof(VisualManager.NativeVersionChangedDelegate))]
		private static void InvokeVersionChanged(IntPtr instance, in VisualNodeHandle handle, VersionChangeType type)
		{
			for (int i = 0; i < VisualManager.s_CallbackInstances.Count; i++)
			{
				VisualManager visualManager;
				bool flag = VisualManager.s_CallbackInstances[i].TryGetTarget(out visualManager) && visualManager.m_Ptr == instance;
				if (flag)
				{
					VersionChangedDelegate onVersionChanged = visualManager.OnVersionChanged;
					if (onVersionChanged != null)
					{
						onVersionChanged(visualManager, in handle, type);
					}
				}
			}
		}

		[MonoPInvokeCallback(typeof(VisualManager.NativeVisualNodeDelegate))]
		private static void InvokeBlur(IntPtr instance, in VisualNodeHandle handle)
		{
			for (int i = 0; i < VisualManager.s_CallbackInstances.Count; i++)
			{
				VisualManager visualManager;
				bool flag = VisualManager.s_CallbackInstances[i].TryGetTarget(out visualManager) && visualManager.m_Ptr == instance;
				if (flag)
				{
					VisualNodeDelegate onBlur = visualManager.OnBlur;
					if (onBlur != null)
					{
						onBlur(visualManager, in handle);
					}
				}
			}
		}

		[MonoPInvokeCallback(typeof(VisualManager.NativeVisualNodeChildDelegate))]
		private static void InvokeChildAdded(IntPtr instance, in VisualNodeHandle handle, in VisualNodeHandle child)
		{
			for (int i = 0; i < VisualManager.s_CallbackInstances.Count; i++)
			{
				VisualManager visualManager;
				bool flag = VisualManager.s_CallbackInstances[i].TryGetTarget(out visualManager) && visualManager.m_Ptr == instance;
				if (flag)
				{
					VisualNodeChildDelegate onChildAdded = visualManager.OnChildAdded;
					if (onChildAdded != null)
					{
						onChildAdded(visualManager, in handle, in child);
					}
				}
			}
		}

		[MonoPInvokeCallback(typeof(VisualManager.NativeVisualNodeChildDelegate))]
		private static void InvokeChildRemoved(IntPtr instance, in VisualNodeHandle handle, in VisualNodeHandle child)
		{
			for (int i = 0; i < VisualManager.s_CallbackInstances.Count; i++)
			{
				VisualManager visualManager;
				bool flag = VisualManager.s_CallbackInstances[i].TryGetTarget(out visualManager) && visualManager.m_Ptr == instance;
				if (flag)
				{
					VisualNodeChildDelegate onChildRemoved = visualManager.OnChildRemoved;
					if (onChildRemoved != null)
					{
						onChildRemoved(visualManager, in handle, in child);
					}
				}
			}
		}

		public bool IsCreated
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		internal VisualNodeClassNameStore ClassNameStore { get; }

		[NativeProperty("Root", TargetType.Field)]
		public VisualNodeHandle Root
		{
			get
			{
				IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				VisualNodeHandle visualNodeHandle;
				VisualManager.get_Root_Injected(intPtr, out visualNodeHandle);
				return visualNodeHandle;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event HierarchyChangedDelegate OnHierarchyChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VersionChangedDelegate OnVersionChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VisualNodeDelegate OnBlur;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VisualNodeChildDelegate OnChildAdded;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event VisualNodeChildDelegate OnChildRemoved;

		public VisualManager()
			: this(VisualManager.Internal_Create(), false)
		{
		}

		private VisualManager(IntPtr ptr, bool isWrapper)
		{
			this.m_InstanceId = this.RegisterCallbackInstance(this);
			this.m_Ptr = ptr;
			this.m_IsWrapper = isWrapper;
			this.ClassNameStore = this.GetClassNameStore();
			this.SetHierarchyChangedCallback(VisualManager.s_HierarchyChangedPtr);
			this.SetVersionChangedCallback(VisualManager.s_VersionChangedPtr);
			this.SetBlurCallback(VisualManager.s_BlurPtr);
			this.SetChildAddedCallback(VisualManager.s_ChildAddedPtr);
			this.SetChildRemovedCallback(VisualManager.s_ChildRemovedPtr);
			this.m_Registry = new VisualNodePropertyRegistry(this);
		}

		~VisualManager()
		{
			this.UnregisterCallbackInstance(this.m_InstanceId);
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				bool flag2 = !this.m_IsWrapper;
				if (flag2)
				{
					VisualManager.Internal_Destroy(this.m_Ptr);
				}
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public VisualPanel CreatePanel()
		{
			this.TryFreePanels();
			VisualPanelHandle visualPanelHandle = this.AddPanel();
			return new VisualPanel(this, visualPanelHandle);
		}

		public void DestroyPanelThreaded(ref VisualPanel panel)
		{
			VisualPanelHandle handle = panel.Handle;
			bool flag = (in handle) == (in VisualPanelHandle.Null);
			if (!flag)
			{
				object panelLock = this.m_PanelLock;
				lock (panelLock)
				{
					this.m_PanelsToRemove.Push(panel.Handle);
				}
				panel = VisualPanel.Null;
			}
		}

		public VisualNode CreateNode()
		{
			this.TryFreeNodes();
			VisualNodeHandle visualNodeHandle = this.AddNode();
			return new VisualNode(this, visualNodeHandle);
		}

		public void DestroyNodeThreaded(ref VisualNode node)
		{
			VisualNodeHandle handle = node.Handle;
			bool flag = (in handle) == (in VisualNodeHandle.Null);
			if (!flag)
			{
				object nodeLock = this.m_NodeLock;
				lock (nodeLock)
				{
					this.m_NodesToRemove.Push(node.Handle);
				}
				node = VisualNode.Null;
			}
		}

		private void TryFreePanels()
		{
			bool flag = false;
			try
			{
				Monitor.TryEnter(this.m_PanelLock, ref flag);
				bool flag2 = flag;
				if (flag2)
				{
					while (this.m_PanelsToRemove.Count > 0)
					{
						VisualPanelHandle visualPanelHandle = this.m_PanelsToRemove.Pop();
						this.RemovePanel(in visualPanelHandle);
					}
				}
			}
			finally
			{
				bool flag3 = flag;
				if (flag3)
				{
					Monitor.Exit(this.m_PanelLock);
				}
			}
		}

		private void TryFreeNodes()
		{
			bool flag = false;
			try
			{
				Monitor.TryEnter(this.m_NodeLock, ref flag);
				bool flag2 = flag;
				if (flag2)
				{
					while (this.m_NodesToRemove.Count > 0)
					{
						VisualNodeHandle visualNodeHandle = this.m_NodesToRemove.Pop();
						this.RemoveNode(in visualNodeHandle);
					}
				}
			}
			finally
			{
				bool flag3 = flag;
				if (flag3)
				{
					Monitor.Exit(this.m_NodeLock);
				}
			}
		}

		public void SetOwner(in VisualNodeHandle handle, VisualElement element)
		{
			this.m_Elements[handle.Id] = ((element != null) ? new WeakReference<VisualElement>(element) : null);
		}

		public VisualElement GetOwner(in VisualNodeHandle handle)
		{
			VisualElement visualElement;
			this.m_Elements[handle.Id].TryGetTarget(out visualElement);
			return visualElement;
		}

		public void SetOwner(in VisualPanelHandle handle, BaseVisualElementPanel panel)
		{
			this.m_Panels[handle.Id] = ((panel != null) ? new WeakReference<BaseVisualElementPanel>(panel) : null);
		}

		public BaseVisualElementPanel GetOwner(in VisualPanelHandle handle)
		{
			BaseVisualElementPanel baseVisualElementPanel;
			this.m_Panels[handle.Id].TryGetTarget(out baseVisualElementPanel);
			return baseVisualElementPanel;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ref T GetProperty<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(VisualNodeHandle handle) where T : struct, ValueType
		{
			return this.m_Registry.GetPropertyRef<T>(handle);
		}

		[FreeFunction("VisualManager::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[FreeFunction("VisualManager::Destroy")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[NativeThrows]
		private void SetHierarchyChangedCallback(IntPtr callback)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetHierarchyChangedCallback_Injected(intPtr, callback);
		}

		[NativeThrows]
		private void SetVersionChangedCallback(IntPtr callback)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetVersionChangedCallback_Injected(intPtr, callback);
		}

		[NativeThrows]
		private void SetBlurCallback(IntPtr callback)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetBlurCallback_Injected(intPtr, callback);
		}

		[NativeThrows]
		private void SetChildAddedCallback(IntPtr callback)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetChildAddedCallback_Injected(intPtr, callback);
		}

		[NativeThrows]
		private void SetChildRemovedCallback(IntPtr callback)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetChildRemovedCallback_Injected(intPtr, callback);
		}

		[NativeThrows]
		internal IntPtr GetPropertyPtr(int index)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.GetPropertyPtr_Injected(intPtr, index);
		}

		internal int PanelCount()
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.PanelCount_Injected(intPtr);
		}

		[NativeThrows]
		internal VisualPanelHandle AddPanel()
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualPanelHandle visualPanelHandle;
			VisualManager.AddPanel_Injected(intPtr, out visualPanelHandle);
			return visualPanelHandle;
		}

		[NativeThrows]
		internal bool RemovePanel(in VisualPanelHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.RemovePanel_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal bool ContainsPanel(in VisualPanelHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.ContainsPanel_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal void ClearPanels()
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.ClearPanels_Injected(intPtr);
		}

		[NativeThrows]
		internal unsafe void* GetPanelDataPtr(in VisualPanelHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.GetPanelDataPtr_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal VisualNodeHandle GetRootContainer(in VisualPanelHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualNodeHandle visualNodeHandle;
			VisualManager.GetRootContainer_Injected(intPtr, in handle, out visualNodeHandle);
			return visualNodeHandle;
		}

		[NativeThrows]
		internal bool SetRootContainer(in VisualPanelHandle handle, in VisualNodeHandle container)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.SetRootContainer_Injected(intPtr, in handle, in container);
		}

		internal int NodeCount()
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.NodeCount_Injected(intPtr);
		}

		[NativeThrows]
		internal VisualNodeHandle AddNode()
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualNodeHandle visualNodeHandle;
			VisualManager.AddNode_Injected(intPtr, out visualNodeHandle);
			return visualNodeHandle;
		}

		[NativeThrows]
		internal bool RemoveNode(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.RemoveNode_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal bool ContainsNode(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.ContainsNode_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal void ClearNodes()
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.ClearNodes_Injected(intPtr);
		}

		[NativeThrows]
		internal unsafe void SetName(in VisualNodeHandle handle, string name)
		{
			try
			{
				IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				VisualManager.SetName_Injected(intPtr, in handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeThrows]
		internal string GetName(in VisualNodeHandle handle)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				VisualManager.GetName_Injected(intPtr, in handle, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeThrows]
		internal int GetChildrenCount(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.GetChildrenCount_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal IntPtr GetChildrenPtr(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.GetChildrenPtr_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal int IndexOfChild(in VisualNodeHandle handle, in VisualNodeHandle child)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.IndexOfChild_Injected(intPtr, in handle, in child);
		}

		[NativeThrows]
		internal bool AddChild(in VisualNodeHandle handle, in VisualNodeHandle child)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.AddChild_Injected(intPtr, in handle, in child);
		}

		[NativeThrows]
		internal bool RemoveChild(in VisualNodeHandle handle, in VisualNodeHandle child)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.RemoveChild_Injected(intPtr, in handle, in child);
		}

		[NativeThrows]
		internal bool InsertChildAtIndex(in VisualNodeHandle handle, int index, in VisualNodeHandle child)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.InsertChildAtIndex_Injected(intPtr, in handle, index, in child);
		}

		[NativeThrows]
		internal bool RemoveChildAtIndex(in VisualNodeHandle handle, int index)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.RemoveChildAtIndex_Injected(intPtr, in handle, index);
		}

		[NativeThrows]
		internal bool ClearChildren(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.ClearChildren_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal VisualNodeHandle GetParent(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualNodeHandle visualNodeHandle;
			VisualManager.GetParent_Injected(intPtr, in handle, out visualNodeHandle);
			return visualNodeHandle;
		}

		[NativeThrows]
		internal bool RemoveFromParent(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.RemoveFromParent_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal unsafe bool AddToClassList(in VisualNodeHandle handle, string className)
		{
			bool flag;
			try
			{
				IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = className.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = VisualManager.AddToClassList_Injected(intPtr, in handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeThrows]
		internal unsafe bool RemoveFromClassList(in VisualNodeHandle handle, string className)
		{
			bool flag;
			try
			{
				IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = className.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = VisualManager.RemoveFromClassList_Injected(intPtr, in handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeThrows]
		internal unsafe bool ClassListContains(in VisualNodeHandle handle, string className)
		{
			bool flag;
			try
			{
				IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = className.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = VisualManager.ClassListContains_Injected(intPtr, in handle, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeThrows]
		internal bool ClearClassList(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.ClearClassList_Injected(intPtr, in handle);
		}

		[NativeThrows]
		private VisualNodeClassNameStore GetClassNameStore()
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr classNameStore_Injected = VisualManager.GetClassNameStore_Injected(intPtr);
			return (classNameStore_Injected == 0) ? null : VisualNodeClassNameStore.BindingsMarshaller.ConvertToManaged(classNameStore_Injected);
		}

		[NativeThrows]
		internal bool IsEnabled(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.IsEnabled_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal void SetEnabled(in VisualNodeHandle handle, bool enabled)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetEnabled_Injected(intPtr, in handle, enabled);
		}

		[NativeThrows]
		internal bool IsEnabledInHierarchy(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.IsEnabledInHierarchy_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal VisualPanelHandle GetPanel(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualPanelHandle visualPanelHandle;
			VisualManager.GetPanel_Injected(intPtr, in handle, out visualPanelHandle);
			return visualPanelHandle;
		}

		[NativeThrows]
		internal void SetPanel(in VisualNodeHandle handle, in VisualPanelHandle panel)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetPanel_Injected(intPtr, in handle, in panel);
		}

		[NativeThrows]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal PseudoStates GetPseudoStates(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.GetPseudoStates_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal void SetPseudoStates(in VisualNodeHandle handle, PseudoStates states)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetPseudoStates_Injected(intPtr, in handle, states);
		}

		[NativeThrows]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal RenderHints GetRenderHints(in VisualNodeHandle handles)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.GetRenderHints_Injected(intPtr, in handles);
		}

		[NativeThrows]
		internal void SetRenderHints(in VisualNodeHandle handle, RenderHints hints)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetRenderHints_Injected(intPtr, in handle, hints);
		}

		[NativeThrows]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal LanguageDirection GetLanguageDirection(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.GetLanguageDirection_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal void SetLanguageDirection(in VisualNodeHandle handle, LanguageDirection direction)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetLanguageDirection_Injected(intPtr, in handle, direction);
		}

		[NativeThrows]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		internal LanguageDirection GetLocalLanguageDirection(in VisualNodeHandle handle)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return VisualManager.GetLocalLanguageDirection_Injected(intPtr, in handle);
		}

		[NativeThrows]
		internal void SetLocalLanguageDirection(in VisualNodeHandle handle, LanguageDirection direction)
		{
			IntPtr intPtr = VisualManager.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			VisualManager.SetLocalLanguageDirection_Injected(intPtr, in handle, direction);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_Root_Injected(IntPtr _unity_self, out VisualNodeHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHierarchyChangedCallback_Injected(IntPtr _unity_self, IntPtr callback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVersionChangedCallback_Injected(IntPtr _unity_self, IntPtr callback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBlurCallback_Injected(IntPtr _unity_self, IntPtr callback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetChildAddedCallback_Injected(IntPtr _unity_self, IntPtr callback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetChildRemovedCallback_Injected(IntPtr _unity_self, IntPtr callback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetPropertyPtr_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PanelCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddPanel_Injected(IntPtr _unity_self, out VisualPanelHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemovePanel_Injected(IntPtr _unity_self, in VisualPanelHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContainsPanel_Injected(IntPtr _unity_self, in VisualPanelHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearPanels_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* GetPanelDataPtr_Injected(IntPtr _unity_self, in VisualPanelHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRootContainer_Injected(IntPtr _unity_self, in VisualPanelHandle handle, out VisualNodeHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetRootContainer_Injected(IntPtr _unity_self, in VisualPanelHandle handle, in VisualNodeHandle container);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int NodeCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddNode_Injected(IntPtr _unity_self, out VisualNodeHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveNode_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContainsNode_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearNodes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetName_Injected(IntPtr _unity_self, in VisualNodeHandle handle, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetName_Injected(IntPtr _unity_self, in VisualNodeHandle handle, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCount_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetChildrenPtr_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int IndexOfChild_Injected(IntPtr _unity_self, in VisualNodeHandle handle, in VisualNodeHandle child);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddChild_Injected(IntPtr _unity_self, in VisualNodeHandle handle, in VisualNodeHandle child);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveChild_Injected(IntPtr _unity_self, in VisualNodeHandle handle, in VisualNodeHandle child);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InsertChildAtIndex_Injected(IntPtr _unity_self, in VisualNodeHandle handle, int index, in VisualNodeHandle child);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveChildAtIndex_Injected(IntPtr _unity_self, in VisualNodeHandle handle, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClearChildren_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetParent_Injected(IntPtr _unity_self, in VisualNodeHandle handle, out VisualNodeHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveFromParent_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddToClassList_Injected(IntPtr _unity_self, in VisualNodeHandle handle, ref ManagedSpanWrapper className);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveFromClassList_Injected(IntPtr _unity_self, in VisualNodeHandle handle, ref ManagedSpanWrapper className);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClassListContains_Injected(IntPtr _unity_self, in VisualNodeHandle handle, ref ManagedSpanWrapper className);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClearClassList_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetClassNameStore_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsEnabled_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetEnabled_Injected(IntPtr _unity_self, in VisualNodeHandle handle, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsEnabledInHierarchy_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPanel_Injected(IntPtr _unity_self, in VisualNodeHandle handle, out VisualPanelHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPanel_Injected(IntPtr _unity_self, in VisualNodeHandle handle, in VisualPanelHandle panel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PseudoStates GetPseudoStates_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPseudoStates_Injected(IntPtr _unity_self, in VisualNodeHandle handle, PseudoStates states);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderHints GetRenderHints_Injected(IntPtr _unity_self, in VisualNodeHandle handles);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderHints_Injected(IntPtr _unity_self, in VisualNodeHandle handle, RenderHints hints);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LanguageDirection GetLanguageDirection_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLanguageDirection_Injected(IntPtr _unity_self, in VisualNodeHandle handle, LanguageDirection direction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LanguageDirection GetLocalLanguageDirection_Injected(IntPtr _unity_self, in VisualNodeHandle handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalLanguageDirection_Injected(IntPtr _unity_self, in VisualNodeHandle handle, LanguageDirection direction);

		private static bool s_Initialized;

		private static bool s_AppDomainUnloadRegistered;

		private static readonly List<WeakReference<VisualManager>> s_CallbackInstances = new List<WeakReference<VisualManager>>();

		private static readonly VisualManager.NativeHierarchyChangedDelegate s_HierarchyChanged = new VisualManager.NativeHierarchyChangedDelegate(VisualManager.InvokeHierarchyChanged);

		private static readonly VisualManager.NativeVersionChangedDelegate s_VersionChanged = new VisualManager.NativeVersionChangedDelegate(VisualManager.InvokeVersionChanged);

		private static readonly VisualManager.NativeVisualNodeDelegate s_Blur = new VisualManager.NativeVisualNodeDelegate(VisualManager.InvokeBlur);

		private static readonly VisualManager.NativeVisualNodeChildDelegate s_ChildAdded = new VisualManager.NativeVisualNodeChildDelegate(VisualManager.InvokeChildAdded);

		private static readonly VisualManager.NativeVisualNodeChildDelegate s_ChildRemoved = new VisualManager.NativeVisualNodeChildDelegate(VisualManager.InvokeChildRemoved);

		private static readonly IntPtr s_HierarchyChangedPtr = Marshal.GetFunctionPointerForDelegate<VisualManager.NativeHierarchyChangedDelegate>(VisualManager.s_HierarchyChanged);

		private static readonly IntPtr s_VersionChangedPtr = Marshal.GetFunctionPointerForDelegate<VisualManager.NativeVersionChangedDelegate>(VisualManager.s_VersionChanged);

		private static readonly IntPtr s_BlurPtr = Marshal.GetFunctionPointerForDelegate<VisualManager.NativeVisualNodeDelegate>(VisualManager.s_Blur);

		private static readonly IntPtr s_ChildAddedPtr = Marshal.GetFunctionPointerForDelegate<VisualManager.NativeVisualNodeChildDelegate>(VisualManager.s_ChildAdded);

		private static readonly IntPtr s_ChildRemovedPtr = Marshal.GetFunctionPointerForDelegate<VisualManager.NativeVisualNodeChildDelegate>(VisualManager.s_ChildRemoved);

		[RequiredByNativeCode]
		private IntPtr m_Ptr;

		[RequiredByNativeCode]
		private bool m_IsWrapper;

		private readonly int m_InstanceId;

		private readonly VisualNodePropertyRegistry m_Registry;

		private readonly ChunkAllocatingArray<WeakReference<VisualElement>> m_Elements = new ChunkAllocatingArray<WeakReference<VisualElement>>();

		private readonly ChunkAllocatingArray<WeakReference<BaseVisualElementPanel>> m_Panels = new ChunkAllocatingArray<WeakReference<BaseVisualElementPanel>>();

		private readonly object m_NodeLock = new object();

		private readonly Stack<VisualNodeHandle> m_NodesToRemove = new Stack<VisualNodeHandle>();

		private readonly object m_PanelLock = new object();

		private readonly Stack<VisualPanelHandle> m_PanelsToRemove = new Stack<VisualPanelHandle>();

		[UsedImplicitly]
		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(VisualManager store)
			{
				return store.m_Ptr;
			}

			public static VisualManager ConvertToManaged(IntPtr ptr)
			{
				return new VisualManager(ptr, true);
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void NativeHierarchyChangedDelegate(IntPtr instance, in VisualNodeHandle handle, HierarchyChangeType type);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void NativeVersionChangedDelegate(IntPtr instance, in VisualNodeHandle handle, VersionChangeType type);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void NativeVisualNodeDelegate(IntPtr instance, in VisualNodeHandle handle);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void NativeVisualNodeChildDelegate(IntPtr instance, in VisualNodeHandle handle, in VisualNodeHandle child);
	}
}
