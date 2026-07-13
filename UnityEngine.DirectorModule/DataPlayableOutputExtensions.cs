using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Playables
{
	[NativeHeader("Runtime/Director/Core/HPlayableOutput.h")]
	[NativeHeader("Modules/Director/ScriptBindings/DataPlayableOutputExtensions.bindings.h")]
	[StaticAccessor("DataPlayableOutputExtensionsBindings", StaticAccessorType.DoubleColon)]
	internal static class DataPlayableOutputExtensions
	{
		[NativeThrows]
		internal unsafe static bool InternalCreateDataOutput(ref PlayableGraph graph, string name, Type type, out PlayableOutputHandle handle)
		{
			bool flag;
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
				flag = DataPlayableOutputExtensions.InternalCreateDataOutput_Injected(ref graph, ref managedSpanWrapper, type, out handle);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalCreateDataOutput_Injected(ref PlayableGraph graph, ref ManagedSpanWrapper name, Type type, out PlayableOutputHandle handle);
	}
}
