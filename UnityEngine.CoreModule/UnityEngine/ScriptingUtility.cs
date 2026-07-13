using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal class ScriptingUtility
	{
		[RequiredByNativeCode]
		private static bool IsManagedCodeWorking()
		{
			ScriptingUtility.TestClass testClass = new ScriptingUtility.TestClass
			{
				value = 42
			};
			return testClass.value == 42;
		}

		[RequiredByNativeCode]
		private static void SetupCallbacks(IntPtr p)
		{
		}

		private struct TestClass
		{
			public int value;
		}
	}
}
