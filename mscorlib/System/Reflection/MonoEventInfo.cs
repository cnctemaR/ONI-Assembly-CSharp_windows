using System;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
	internal struct MonoEventInfo
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_event_info(MonoEvent ev, out MonoEventInfo info);

		internal static MonoEventInfo GetEventInfo(MonoEvent ev)
		{
			MonoEventInfo monoEventInfo;
			MonoEventInfo.get_event_info(ev, out monoEventInfo);
			return monoEventInfo;
		}

		public Type declaring_type;

		public Type reflected_type;

		public string name;

		public MethodInfo add_method;

		public MethodInfo remove_method;

		public MethodInfo raise_method;

		public EventAttributes attrs;

		public MethodInfo[] other_methods;
	}
}
