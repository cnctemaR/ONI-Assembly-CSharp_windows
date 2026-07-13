using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[ExtensionOfNativeClass]
	[NativeClass(null)]
	[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class ScriptableObject : Object
	{
		public ScriptableObject()
		{
			ScriptableObject.CreateScriptableObject(this);
		}

		[Obsolete("Use EditorUtility.SetDirty instead")]
		[NativeConditional("ENABLE_MONO")]
		public void SetDirty()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ScriptableObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ScriptableObject.SetDirty_Injected(intPtr);
		}

		public static ScriptableObject CreateInstance(string className)
		{
			return ScriptableObject.CreateScriptableObjectInstanceFromName(className);
		}

		public static ScriptableObject CreateInstance(Type type)
		{
			return ScriptableObject.CreateScriptableObjectInstanceFromType(type, true);
		}

		public static T CreateInstance<T>() where T : ScriptableObject
		{
			return (T)((object)ScriptableObject.CreateInstance(typeof(T)));
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static ScriptableObject CreateInstance(Type type, Action<ScriptableObject> initialize)
		{
			bool flag = !typeof(ScriptableObject).IsAssignableFrom(type);
			if (flag)
			{
				throw new ArgumentException("Type must inherit ScriptableObject.", "type");
			}
			ScriptableObject scriptableObject = ScriptableObject.CreateScriptableObjectInstanceFromType(type, false);
			try
			{
				initialize(scriptableObject);
			}
			finally
			{
				ScriptableObject.ResetAndApplyDefaultInstances(scriptableObject);
			}
			return scriptableObject;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateScriptableObject([Writable] ScriptableObject self);

		[FreeFunction("Scripting::CreateScriptableObject")]
		private unsafe static ScriptableObject CreateScriptableObjectInstanceFromName(string className)
		{
			ScriptableObject scriptableObject;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = className.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = ScriptableObject.CreateScriptableObjectInstanceFromName_Injected(ref managedSpanWrapper);
			}
			finally
			{
				IntPtr intPtr;
				scriptableObject = Unmarshal.UnmarshalUnityObject<ScriptableObject>(intPtr);
				char* ptr = null;
			}
			return scriptableObject;
		}

		[NativeMethod(Name = "Scripting::CreateScriptableObjectWithType", IsFreeFunction = true, ThrowsException = true)]
		internal static ScriptableObject CreateScriptableObjectInstanceFromType(Type type, bool applyDefaultsAndReset)
		{
			return Unmarshal.UnmarshalUnityObject<ScriptableObject>(ScriptableObject.CreateScriptableObjectInstanceFromType_Injected(type, applyDefaultsAndReset));
		}

		[FreeFunction("Scripting::ResetAndApplyDefaultInstances")]
		internal static void ResetAndApplyDefaultInstances([NotNull] Object obj)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(obj);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			ScriptableObject.ResetAndApplyDefaultInstances_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDirty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateScriptableObjectInstanceFromName_Injected(ref ManagedSpanWrapper className);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateScriptableObjectInstanceFromType_Injected(Type type, bool applyDefaultsAndReset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetAndApplyDefaultInstances_Injected(IntPtr obj);
	}
}
