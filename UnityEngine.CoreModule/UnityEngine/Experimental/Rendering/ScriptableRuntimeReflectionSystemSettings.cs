using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	[NativeHeader("Runtime/Camera/ScriptableRuntimeReflectionSystem.h")]
	[RequiredByNativeCode]
	public static class ScriptableRuntimeReflectionSystemSettings
	{
		public static IScriptableRuntimeReflectionSystem system
		{
			get
			{
				return ScriptableRuntimeReflectionSystemSettings.Internal_ScriptableRuntimeReflectionSystemSettings_system;
			}
			set
			{
				if (value == null || value.Equals(null))
				{
					Debug.LogError("'null' cannot be assigned to ScriptableRuntimeReflectionSystemSettings.system");
				}
				else
				{
					if (!(ScriptableRuntimeReflectionSystemSettings.system is BuiltinRuntimeReflectionSystem) && !(value is BuiltinRuntimeReflectionSystem) && ScriptableRuntimeReflectionSystemSettings.system != value)
					{
						Debug.LogWarningFormat("ScriptableRuntimeReflectionSystemSettings.system is assigned more than once. Only a the last instance will be used. (Last instance {0}, New instance {1})", new object[]
						{
							ScriptableRuntimeReflectionSystemSettings.system,
							value
						});
					}
					ScriptableRuntimeReflectionSystemSettings.Internal_ScriptableRuntimeReflectionSystemSettings_system = value;
				}
			}
		}

		private static IScriptableRuntimeReflectionSystem Internal_ScriptableRuntimeReflectionSystemSettings_system
		{
			get
			{
				return ScriptableRuntimeReflectionSystemSettings.s_Instance.implementation;
			}
			[RequiredByNativeCode]
			set
			{
				if (ScriptableRuntimeReflectionSystemSettings.s_Instance.implementation != value)
				{
					if (ScriptableRuntimeReflectionSystemSettings.s_Instance.implementation != null)
					{
						ScriptableRuntimeReflectionSystemSettings.s_Instance.implementation.Dispose();
					}
				}
				ScriptableRuntimeReflectionSystemSettings.s_Instance.implementation = value;
			}
		}

		private static ScriptableRuntimeReflectionSystemWrapper Internal_ScriptableRuntimeReflectionSystemSettings_instance
		{
			[RequiredByNativeCode]
			get
			{
				return ScriptableRuntimeReflectionSystemSettings.s_Instance;
			}
		}

		[StaticAccessor("ScriptableRuntimeReflectionSystem", StaticAccessorType.DoubleColon)]
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ScriptingDirtyReflectionSystemInstance();

		private static ScriptableRuntimeReflectionSystemWrapper s_Instance = new ScriptableRuntimeReflectionSystemWrapper();
	}
}
