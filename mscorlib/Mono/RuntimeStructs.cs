using System;

namespace Mono
{
	internal static class RuntimeStructs
	{
		internal struct RemoteClass
		{
			internal IntPtr default_vtable;

			internal IntPtr xdomain_vtable;

			internal unsafe RuntimeStructs.MonoClass* proxy_class;

			internal IntPtr proxy_class_name;

			internal uint interface_count;
		}

		internal struct MonoClass
		{
		}

		internal struct GenericParamInfo
		{
			internal unsafe RuntimeStructs.MonoClass* pklass;

			internal IntPtr name;

			internal ushort flags;

			internal uint token;

			internal unsafe RuntimeStructs.MonoClass** constraints;
		}

		internal struct GPtrArray
		{
			internal unsafe IntPtr* data;

			internal int len;
		}
	}
}
