using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine.Bindings;

namespace Unity.Content
{
	[StaticAccessor("GetContentNamespaceManager()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/Misc/ContentNamespace.h")]
	public struct ContentNamespace
	{
		public string GetName()
		{
			this.ThrowIfInvalidNamespace();
			return ContentNamespace.GetNamespaceName(this);
		}

		public bool IsValid
		{
			get
			{
				return ContentNamespace.IsNamespaceHandleValid(this);
			}
		}

		public void Delete()
		{
			bool flag = this.Id == ContentNamespace.s_Default.Id;
			if (flag)
			{
				throw new InvalidOperationException("Cannot delete the default namespace.");
			}
			this.ThrowIfInvalidNamespace();
			ContentNamespace.RemoveNamespace(this);
		}

		private void ThrowIfInvalidNamespace()
		{
			bool flag = !this.IsValid;
			if (flag)
			{
				throw new InvalidOperationException("The provided namespace is invalid. Did you already delete it?");
			}
		}

		public static ContentNamespace Default
		{
			get
			{
				bool flag = !ContentNamespace.s_defaultInitialized;
				if (flag)
				{
					ContentNamespace.s_defaultInitialized = true;
					ContentNamespace.s_Default = ContentNamespace.GetOrCreateNamespace("default");
				}
				return ContentNamespace.s_Default;
			}
		}

		public static ContentNamespace GetOrCreateNamespace(string name)
		{
			bool flag = ContentNamespace.s_ValidName.IsMatch(name);
			if (flag)
			{
				return ContentNamespace.GetOrCreate(name);
			}
			throw new InvalidOperationException("Namespace name can only contain alphanumeric characters and a maximum length of 16 characters.");
		}

		public static ContentNamespace[] GetAll()
		{
			ContentNamespace[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ContentNamespace.GetAll_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ContentNamespace[] array;
				blittableArrayWrapper.Unmarshal<ContentNamespace>(ref array);
				array2 = array;
			}
			return array2;
		}

		internal unsafe static ContentNamespace GetOrCreate(string name)
		{
			ContentNamespace contentNamespace2;
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
				ContentNamespace contentNamespace;
				ContentNamespace.GetOrCreate_Injected(ref managedSpanWrapper, out contentNamespace);
			}
			finally
			{
				char* ptr = null;
				ContentNamespace contentNamespace;
				contentNamespace2 = contentNamespace;
			}
			return contentNamespace2;
		}

		internal static void RemoveNamespace(ContentNamespace ns)
		{
			ContentNamespace.RemoveNamespace_Injected(ref ns);
		}

		internal static string GetNamespaceName(ContentNamespace ns)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				ContentNamespace.GetNamespaceName_Injected(ref ns, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		internal static bool IsNamespaceHandleValid(ContentNamespace ns)
		{
			return ContentNamespace.IsNamespaceHandleValid_Injected(ref ns);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAll_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOrCreate_Injected(ref ManagedSpanWrapper name, out ContentNamespace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveNamespace_Injected([In] ref ContentNamespace ns);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNamespaceName_Injected([In] ref ContentNamespace ns, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsNamespaceHandleValid_Injected([In] ref ContentNamespace ns);

		internal ulong Id;

		private static bool s_defaultInitialized = false;

		private static ContentNamespace s_Default;

		private static Regex s_ValidName = new Regex("^[a-zA-Z0-9]{1,16}$", RegexOptions.Compiled);
	}
}
