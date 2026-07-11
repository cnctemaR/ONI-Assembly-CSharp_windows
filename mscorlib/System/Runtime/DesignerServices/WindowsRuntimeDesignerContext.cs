using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using Unity;

namespace System.Runtime.DesignerServices
{
	public sealed class WindowsRuntimeDesignerContext
	{
		[SecurityCritical]
		public WindowsRuntimeDesignerContext(IEnumerable<string> paths, string name)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public string Name
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		[SecurityCritical]
		public Assembly GetAssembly(string assemblyName)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecurityCritical]
		public Type GetType(string typeName)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecurityCritical]
		public static void InitializeSharedContext(IEnumerable<string> paths)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		[SecurityCritical]
		public static void SetIterationContext(WindowsRuntimeDesignerContext context)
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
