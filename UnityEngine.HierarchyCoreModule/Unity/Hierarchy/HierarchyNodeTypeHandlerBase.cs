using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyNodeTypeHandlerBase.h")]
	[RequiredByNativeCode]
	[NativeHeader("Modules/HierarchyCore/HierarchyNodeTypeHandlerBaseBindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class HierarchyNodeTypeHandlerBase : IDisposable
	{
		public Hierarchy Hierarchy
		{
			get
			{
				return this.m_Hierarchy;
			}
		}

		protected HierarchyCommandList CommandList
		{
			get
			{
				return this.m_CommandList;
			}
		}

		protected HierarchyNodeTypeHandlerBase()
		{
			this.m_Ptr = HierarchyNodeTypeHandlerBase.ConstructorScope.Ptr;
			this.m_Hierarchy = HierarchyNodeTypeHandlerBase.ConstructorScope.Hierarchy;
			this.m_CommandList = HierarchyNodeTypeHandlerBase.ConstructorScope.CommandList;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		internal HierarchyNodeTypeHandlerBase(IntPtr nativePtr, Hierarchy hierarchy, HierarchyCommandList cmdList)
		{
			this.m_Ptr = nativePtr;
			this.m_Hierarchy = hierarchy;
			this.m_CommandList = cmdList;
		}

		~HierarchyNodeTypeHandlerBase()
		{
			this.Dispose(false);
		}

		protected virtual void Initialize()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		public HierarchyNodeType GetNodeType()
		{
			return new HierarchyNodeType(HierarchyNodeTypeHandlerBase.GetNodeTypeFromType(base.GetType()));
		}

		[NativeMethod(IsThreadSafe = true)]
		public virtual string GetNodeTypeName()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				HierarchyNodeTypeHandlerBase.GetNodeTypeName_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public virtual int GetNodeHashCode(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyNodeTypeHandlerBase.GetNodeHashCode_Injected(intPtr, in node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public virtual HierarchyNodeFlags GetDefaultNodeFlags(in HierarchyNode node, HierarchyNodeFlags defaultFlags = HierarchyNodeFlags.None)
		{
			IntPtr intPtr = HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyNodeTypeHandlerBase.GetDefaultNodeFlags_Injected(intPtr, in node, defaultFlags);
		}

		protected virtual void SearchBegin(HierarchySearchQueryDescriptor query)
		{
		}

		protected virtual bool SearchMatch(in HierarchyNode node)
		{
			return false;
		}

		protected virtual void SearchEnd()
		{
		}

		protected virtual void ViewModelPostUpdate(HierarchyViewModel viewModel)
		{
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule" })]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static HierarchyNodeTypeHandlerBase FromIntPtr(IntPtr handlePtr)
		{
			return (handlePtr != IntPtr.Zero) ? ((HierarchyNodeTypeHandlerBase)GCHandle.FromIntPtr(handlePtr).Target) : null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Internal_SearchBegin(HierarchySearchQueryDescriptor query)
		{
			this.SearchBegin(query);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal bool Internal_SearchMatch(in HierarchyNode node)
		{
			return this.SearchMatch(in node);
		}

		[FreeFunction("HierarchyNodeTypeHandlerManager::Get().GetNodeType", IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodeTypeFromType(Type type);

		[RequiredByNativeCode]
		private static IntPtr CreateNodeTypeHandlerFromType(IntPtr nativePtr, Type handlerType, IntPtr hierarchyPtr, IntPtr cmdListPtr)
		{
			bool flag = nativePtr == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentNullException("nativePtr");
			}
			bool flag2 = hierarchyPtr == IntPtr.Zero;
			if (flag2)
			{
				throw new ArgumentNullException("hierarchyPtr");
			}
			bool flag3 = cmdListPtr == IntPtr.Zero;
			if (flag3)
			{
				throw new ArgumentNullException("cmdListPtr");
			}
			Hierarchy hierarchy = Hierarchy.FromIntPtr(hierarchyPtr);
			HierarchyCommandList hierarchyCommandList = HierarchyCommandList.FromIntPtr(cmdListPtr);
			IntPtr intPtr;
			using (new HierarchyNodeTypeHandlerBase.ConstructorScope(nativePtr, hierarchy, hierarchyCommandList))
			{
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				HierarchyNodeTypeHandlerBase hierarchyNodeTypeHandlerBase = (HierarchyNodeTypeHandlerBase)Activator.CreateInstance(handlerType, bindingFlags, null, null, null);
				bool flag4 = hierarchyNodeTypeHandlerBase == null;
				if (flag4)
				{
					intPtr = IntPtr.Zero;
				}
				else
				{
					intPtr = GCHandle.ToIntPtr(GCHandle.Alloc(hierarchyNodeTypeHandlerBase));
				}
			}
			return intPtr;
		}

		[RequiredByNativeCode]
		private static bool TryGetStaticNodeType(Type handlerType, out int nodeType)
		{
			bool flag = HierarchyNodeTypeHandlerBase.s_NodeTypes.TryGetValue(handlerType, out nodeType);
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				MethodInfo method = handlerType.GetMethod("GetStaticNodeType", BindingFlags.Static | BindingFlags.NonPublic);
				bool flag3 = method != null;
				if (flag3)
				{
					nodeType = (int)method.Invoke(null, null);
					HierarchyNodeTypeHandlerBase.s_NodeTypes.Add(handlerType, nodeType);
					flag2 = true;
				}
				else
				{
					nodeType = 0;
					flag2 = false;
				}
			}
			return flag2;
		}

		[RequiredByNativeCode]
		private static void InvokeInitialize(IntPtr handlePtr)
		{
			HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).Initialize();
		}

		[RequiredByNativeCode]
		private static void InvokeDispose(IntPtr handlePtr)
		{
			HierarchyNodeTypeHandlerBase hierarchyNodeTypeHandlerBase = HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr);
			hierarchyNodeTypeHandlerBase.Dispose(true);
			GC.SuppressFinalize(hierarchyNodeTypeHandlerBase);
		}

		[RequiredByNativeCode]
		private static string InvokeGetNodeTypeName(IntPtr handlePtr)
		{
			return HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).GetNodeTypeName();
		}

		[RequiredByNativeCode]
		private static int InvokeGetNodeHashCode(IntPtr handlePtr, in HierarchyNode node)
		{
			return HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).GetNodeHashCode(in node);
		}

		[RequiredByNativeCode]
		private static HierarchyNodeFlags InvokeGetDefaultNodeFlags(IntPtr handlePtr, in HierarchyNode node, HierarchyNodeFlags defaultFlags)
		{
			return HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).GetDefaultNodeFlags(in node, defaultFlags);
		}

		[RequiredByNativeCode]
		private static bool InvokeChangesPending(IntPtr handlePtr)
		{
			return HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).ChangesPending();
		}

		[RequiredByNativeCode]
		private static bool InvokeIntegrateChanges(IntPtr handlePtr, IntPtr cmdListPtr)
		{
			return HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).IntegrateChanges(HierarchyCommandList.FromIntPtr(cmdListPtr));
		}

		[RequiredByNativeCode]
		private static bool InvokeSearchMatch(IntPtr handlePtr, in HierarchyNode node)
		{
			return HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).SearchMatch(in node);
		}

		[RequiredByNativeCode]
		private static void InvokeSearchEnd(IntPtr handlePtr)
		{
			HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).SearchEnd();
		}

		[RequiredByNativeCode]
		private static void InvokeViewModelPostUpdate(IntPtr handlePtr, IntPtr viewModelPtr)
		{
			HierarchyNodeTypeHandlerBase.FromIntPtr(handlePtr).ViewModelPostUpdate(HierarchyViewModel.FromIntPtr(viewModelPtr));
		}

		[Obsolete("The constructor with a hierarchy parameter is obsolete and is no longer used. Remove the hierarchy parameter from your constructor.")]
		protected HierarchyNodeTypeHandlerBase(Hierarchy hierarchy)
			: this()
		{
		}

		[Obsolete("The IDisposable interface is obsolete and no longer has any effect. Instances of handlers are owned and disposed by the hierarchy so they do not need to be disposed by user code.")]
		public void Dispose()
		{
		}

		[FreeFunction("HierarchyNodeTypeHandlerBaseBindings::ChangesPending", HasExplicitThis = true, IsThreadSafe = true)]
		[Obsolete("ChangesPending is obsolete, it is replaced by adding commands into the hierarchy node type handler's CommandList.", false)]
		protected virtual bool ChangesPending()
		{
			IntPtr intPtr = HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyNodeTypeHandlerBase.ChangesPending_Injected(intPtr);
		}

		[Obsolete("IntegrateChanges is obsolete, it is replaced by adding commands into the hierarchy node type handler's CommandList.", false)]
		[FreeFunction("HierarchyNodeTypeHandlerBaseBindings::IntegrateChanges", HasExplicitThis = true, IsThreadSafe = true)]
		protected virtual bool IntegrateChanges(HierarchyCommandList cmdList)
		{
			IntPtr intPtr = HierarchyNodeTypeHandlerBase.BindingsMarshaller.ConvertToUnmanaged(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyNodeTypeHandlerBase.IntegrateChanges_Injected(intPtr, (cmdList == null) ? ((IntPtr)0) : HierarchyCommandList.BindingsMarshaller.ConvertToUnmanaged(cmdList));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNodeTypeName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodeHashCode_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern HierarchyNodeFlags GetDefaultNodeFlags_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags defaultFlags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ChangesPending_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IntegrateChanges_Injected(IntPtr _unity_self, IntPtr cmdList);

		internal readonly IntPtr m_Ptr;

		private readonly Hierarchy m_Hierarchy;

		private readonly HierarchyCommandList m_CommandList;

		[AutoStaticsCleanupOnCodeReload]
		private static readonly Dictionary<Type, int> s_NodeTypes = new Dictionary<Type, int>();

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToUnmanaged(HierarchyNodeTypeHandlerBase handler)
			{
				return handler.m_Ptr;
			}
		}

		private struct ConstructorScope : IDisposable
		{
			public static IntPtr Ptr
			{
				get
				{
					return HierarchyNodeTypeHandlerBase.ConstructorScope.m_Ptr;
				}
				private set
				{
					HierarchyNodeTypeHandlerBase.ConstructorScope.m_Ptr = value;
				}
			}

			public static Hierarchy Hierarchy
			{
				get
				{
					return HierarchyNodeTypeHandlerBase.ConstructorScope.m_Hierarchy;
				}
				private set
				{
					HierarchyNodeTypeHandlerBase.ConstructorScope.m_Hierarchy = value;
				}
			}

			public static HierarchyCommandList CommandList
			{
				get
				{
					return HierarchyNodeTypeHandlerBase.ConstructorScope.m_CommandList;
				}
				private set
				{
					HierarchyNodeTypeHandlerBase.ConstructorScope.m_CommandList = value;
				}
			}

			public ConstructorScope(IntPtr nativePtr, Hierarchy hierarchy, HierarchyCommandList cmdList)
			{
				HierarchyNodeTypeHandlerBase.ConstructorScope.Ptr = nativePtr;
				HierarchyNodeTypeHandlerBase.ConstructorScope.Hierarchy = hierarchy;
				HierarchyNodeTypeHandlerBase.ConstructorScope.CommandList = cmdList;
			}

			public void Dispose()
			{
				HierarchyNodeTypeHandlerBase.ConstructorScope.Ptr = IntPtr.Zero;
				HierarchyNodeTypeHandlerBase.ConstructorScope.Hierarchy = null;
				HierarchyNodeTypeHandlerBase.ConstructorScope.CommandList = null;
			}

			[NoAutoStaticsCleanup]
			[ThreadStatic]
			private static IntPtr m_Ptr;

			[NoAutoStaticsCleanup]
			[ThreadStatic]
			private static Hierarchy m_Hierarchy;

			[NoAutoStaticsCleanup]
			[ThreadStatic]
			private static HierarchyCommandList m_CommandList;
		}
	}
}
