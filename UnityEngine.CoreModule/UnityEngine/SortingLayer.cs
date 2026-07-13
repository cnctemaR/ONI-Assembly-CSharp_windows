using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/BaseClasses/TagManager.h")]
	public struct SortingLayer
	{
		public int id
		{
			get
			{
				return this.m_Id;
			}
		}

		public string name
		{
			get
			{
				return SortingLayer.IDToName(this.m_Id);
			}
		}

		public int value
		{
			get
			{
				return SortingLayer.GetLayerValueFromID(this.m_Id);
			}
		}

		public static SortingLayer[] layers
		{
			get
			{
				int[] sortingLayerIDsInternal = SortingLayer.GetSortingLayerIDsInternal();
				SortingLayer[] array = new SortingLayer[sortingLayerIDsInternal.Length];
				for (int i = 0; i < sortingLayerIDsInternal.Length; i++)
				{
					array[i].m_Id = sortingLayerIDsInternal[i];
				}
				return array;
			}
		}

		[FreeFunction("GetTagManager().GetSortingLayerIDs")]
		private static int[] GetSortingLayerIDsInternal()
		{
			int[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				SortingLayer.GetSortingLayerIDsInternal_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("GetTagManager().GetSortingLayerValueFromUniqueID")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLayerValueFromID(int id);

		[FreeFunction("GetTagManager().GetSortingLayerValueFromName")]
		public unsafe static int GetLayerValueFromName(string name)
		{
			int layerValueFromName_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				layerValueFromName_Injected = SortingLayer.GetLayerValueFromName_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return layerValueFromName_Injected;
		}

		[FreeFunction("GetTagManager().GetSortingLayerUniqueIDFromName")]
		public unsafe static int NameToID(string name)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = SortingLayer.NameToID_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		[FreeFunction("GetTagManager().GetSortingLayerNameFromUniqueID")]
		public static string IDToName(int id)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SortingLayer.IDToName_Injected(id, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("GetTagManager().IsSortingLayerUniqueIDValid")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsValid(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSortingLayerIDsInternal_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerValueFromName_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int NameToID_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IDToName_Injected(int id, out ManagedSpanWrapper ret);

		private int m_Id;

		public static SortingLayer.LayerCallback onLayerAdded;

		public static SortingLayer.LayerCallback onLayerRemoved;

		internal static SortingLayer.LayerChangedCallback onLayerChanged;

		public delegate void LayerCallback(SortingLayer layer);

		internal delegate void LayerChangedCallback();
	}
}
