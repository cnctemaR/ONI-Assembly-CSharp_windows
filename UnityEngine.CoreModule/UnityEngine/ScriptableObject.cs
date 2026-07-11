using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A class you can derive from if you want to create objects that don't need to be attached to game objects.</para>
	/// </summary>
	[RequiredByNativeCode]
	[NativeClass(null)]
	[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class ScriptableObject : Object
	{
		public ScriptableObject()
		{
			ScriptableObject.CreateScriptableObject(this);
		}

		[NativeConditional("ENABLE_MONO")]
		[Obsolete("Use EditorUtility.SetDirty instead")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetDirty();

		/// <summary>
		///   <para>Creates an instance of a scriptable object.</para>
		/// </summary>
		/// <param name="className">The type of the ScriptableObject to create, as the name of the type.</param>
		/// <param name="type">The type of the ScriptableObject to create, as a System.Type instance.</param>
		/// <returns>
		///   <para>The created ScriptableObject.</para>
		/// </returns>
		public static ScriptableObject CreateInstance(string className)
		{
			return ScriptableObject.CreateScriptableObjectInstanceFromName(className);
		}

		/// <summary>
		///   <para>Creates an instance of a scriptable object.</para>
		/// </summary>
		/// <param name="className">The type of the ScriptableObject to create, as the name of the type.</param>
		/// <param name="type">The type of the ScriptableObject to create, as a System.Type instance.</param>
		/// <returns>
		///   <para>The created ScriptableObject.</para>
		/// </returns>
		public static ScriptableObject CreateInstance(Type type)
		{
			return ScriptableObject.CreateScriptableObjectInstanceFromType(type);
		}

		public static T CreateInstance<T>() where T : ScriptableObject
		{
			return (T)((object)ScriptableObject.CreateInstance(typeof(T)));
		}

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateScriptableObject([Writable] ScriptableObject self);

		[FreeFunction("Scripting::CreateScriptableObject")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ScriptableObject CreateScriptableObjectInstanceFromName(string className);

		[FreeFunction("Scripting::CreateScriptableObjectWithType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ScriptableObject CreateScriptableObjectInstanceFromType(Type type);
	}
}
