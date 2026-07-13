using System;
using System.Runtime.CompilerServices;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern ContentNamespace[] GetAll();

		internal static ContentNamespace GetOrCreate(string name)
		{
			ContentNamespace contentNamespace;
			ContentNamespace.GetOrCreate_Injected(name, out contentNamespace);
			return contentNamespace;
		}

		internal static void RemoveNamespace(ContentNamespace ns)
		{
			ContentNamespace.RemoveNamespace_Injected(ref ns);
		}

		internal static string GetNamespaceName(ContentNamespace ns)
		{
			return ContentNamespace.GetNamespaceName_Injected(ref ns);
		}

		internal static bool IsNamespaceHandleValid(ContentNamespace ns)
		{
			return ContentNamespace.IsNamespaceHandleValid_Injected(ref ns);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOrCreate_Injected(string name, out ContentNamespace ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveNamespace_Injected(ref ContentNamespace ns);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetNamespaceName_Injected(ref ContentNamespace ns);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsNamespaceHandleValid_Injected(ref ContentNamespace ns);

		internal ulong Id;

		private static bool s_defaultInitialized = false;

		private static ContentNamespace s_Default;

		private static Regex s_ValidName = new Regex("^[a-zA-Z0-9]{1,16}$", RegexOptions.Compiled);
	}
}
