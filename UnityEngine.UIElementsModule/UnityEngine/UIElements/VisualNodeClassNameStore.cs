using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualNodeClassNameStore.h")]
	internal class VisualNodeClassNameStore : IDisposable
	{
		public VisualNodeClassNameStore()
			: this(VisualNodeClassNameStore.Internal_Create(), false)
		{
		}

		private VisualNodeClassNameStore(IntPtr ptr, bool isWrapper)
		{
			this.m_Ptr = ptr;
			this.m_IsWrapper = isWrapper;
		}

		~VisualNodeClassNameStore()
		{
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
					VisualNodeClassNameStore.Internal_Destroy(this.m_Ptr);
				}
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public string GetClassNameManaged(int id)
		{
			int i = this.m_ClassNames.Length;
			bool flag = (ulong)id < (ulong)((long)i);
			if (flag)
			{
				bool flag2 = !string.IsNullOrEmpty(this.m_ClassNames[id]);
				if (flag2)
				{
					return this.m_ClassNames[id];
				}
			}
			else
			{
				while (i <= id)
				{
					i *= 2;
				}
				Array.Resize<string>(ref this.m_ClassNames, i);
			}
			string className = this.GetClassName(id);
			this.m_ClassNames[id] = className;
			return className;
		}

		public int GetClassNameIdManaged(string className)
		{
			int classNameId;
			bool flag = this.m_Map.TryGetValue(className, out classNameId);
			int num;
			if (flag)
			{
				num = classNameId;
			}
			else
			{
				classNameId = this.GetClassNameId(className);
				this.m_Map.Add(className, classNameId);
				num = classNameId;
			}
			return num;
		}

		[FreeFunction("VisualNodeClassNameStore::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create();

		[FreeFunction("VisualNodeClassNameStore::Destroy")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[NativeThrows]
		internal unsafe int Insert(string className)
		{
			int num;
			try
			{
				IntPtr intPtr = VisualNodeClassNameStore.BindingsMarshaller.ConvertToNative(this);
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
				num = VisualNodeClassNameStore.Insert_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		[NativeThrows]
		internal unsafe int GetClassNameId(string className)
		{
			int classNameId_Injected;
			try
			{
				IntPtr intPtr = VisualNodeClassNameStore.BindingsMarshaller.ConvertToNative(this);
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
				classNameId_Injected = VisualNodeClassNameStore.GetClassNameId_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return classNameId_Injected;
		}

		[NativeThrows]
		internal string GetClassName(int id)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = VisualNodeClassNameStore.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				VisualNodeClassNameStore.GetClassName_Injected(intPtr, id, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Insert_Injected(IntPtr _unity_self, ref ManagedSpanWrapper className);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetClassNameId_Injected(IntPtr _unity_self, ref ManagedSpanWrapper className);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetClassName_Injected(IntPtr _unity_self, int id, out ManagedSpanWrapper ret);

		[RequiredByNativeCode]
		private IntPtr m_Ptr;

		[RequiredByNativeCode]
		private bool m_IsWrapper;

		private string[] m_ClassNames = new string[512];

		private Dictionary<string, int> m_Map = new Dictionary<string, int>();

		[UsedImplicitly]
		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(VisualNodeClassNameStore store)
			{
				return store.m_Ptr;
			}

			public static VisualNodeClassNameStore ConvertToManaged(IntPtr ptr)
			{
				return new VisualNodeClassNameStore(ptr, true);
			}
		}
	}
}
