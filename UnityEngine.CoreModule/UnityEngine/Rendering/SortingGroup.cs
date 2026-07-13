using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeType(Header = "Runtime/2D/Sorting/SortingGroup.h")]
	[RequireComponent(typeof(Transform))]
	public sealed class SortingGroup : Behaviour
	{
		[StaticAccessor("SortingGroup", StaticAccessorType.DoubleColon)]
		internal static extern int invalidSortingGroupID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[StaticAccessor("SortingGroup", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void UpdateAllSortingGroups();

		[StaticAccessor("SortingGroup", StaticAccessorType.DoubleColon)]
		internal static SortingGroup GetSortingGroupByIndex(int index)
		{
			return Unmarshal.UnmarshalUnityObject<SortingGroup>(SortingGroup.GetSortingGroupByIndex_Injected(index));
		}

		public unsafe string sortingLayerName
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					SortingGroup.get_sortingLayerName_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					SortingGroup.set_sortingLayerName_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public int sortingLayerID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SortingGroup.get_sortingLayerID_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SortingGroup.set_sortingLayerID_Injected(intPtr, value);
			}
		}

		public int sortingOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SortingGroup.get_sortingOrder_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SortingGroup.set_sortingOrder_Injected(intPtr, value);
			}
		}

		public bool sortAtRoot
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SortingGroup.get_sortAtRoot_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SortingGroup.set_sortAtRoot_Injected(intPtr, value);
			}
		}

		internal int sortingGroupID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SortingGroup.get_sortingGroupID_Injected(intPtr);
			}
		}

		internal int sortingGroupOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SortingGroup.get_sortingGroupOrder_Injected(intPtr);
			}
		}

		internal int index
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SortingGroup.get_index_Injected(intPtr);
			}
		}

		internal uint sortingKey
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SortingGroup.get_sortingKey_Injected(intPtr);
			}
		}

		internal bool sort3DAs2D
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SortingGroup>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SortingGroup.get_sort3DAs2D_Injected(intPtr);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSortingGroupByIndex_Injected(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sortingLayerName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingLayerName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingLayerID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingLayerID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingOrder_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_sortAtRoot_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortAtRoot_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingGroupID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingGroupOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_index_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint get_sortingKey_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_sort3DAs2D_Injected(IntPtr _unity_self);
	}
}
