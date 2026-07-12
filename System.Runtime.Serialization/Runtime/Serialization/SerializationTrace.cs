using System;
using System.Diagnostics;
using System.Security;

namespace System.Runtime.Serialization
{
	internal static class SerializationTrace
	{
		internal static SourceSwitch CodeGenerationSwitch
		{
			get
			{
				return SerializationTrace.CodeGenerationTraceSource.Switch;
			}
		}

		internal static void WriteInstruction(int lineNumber, string instruction)
		{
		}

		internal static void TraceInstruction(string instruction)
		{
		}

		private static TraceSource CodeGenerationTraceSource
		{
			[SecuritySafeCritical]
			get
			{
				if (SerializationTrace.codeGen == null)
				{
					SerializationTrace.codeGen = new TraceSource("System.Runtime.Serialization.CodeGeneration");
				}
				return SerializationTrace.codeGen;
			}
		}

		[SecurityCritical]
		private static TraceSource codeGen;
	}
}
