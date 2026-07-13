using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.SceneManagement;

namespace UnityEngine
{
	public static class PhysicsSceneExtensions
	{
		public static PhysicsScene GetPhysicsScene(this Scene scene)
		{
			bool flag = !scene.IsValid();
			if (flag)
			{
				throw new ArgumentException("Cannot get physics scene; Unity scene is invalid.", "scene");
			}
			PhysicsScene physicsScene_Internal = PhysicsSceneExtensions.GetPhysicsScene_Internal(scene);
			bool flag2 = physicsScene_Internal.IsValid();
			if (flag2)
			{
				return physicsScene_Internal;
			}
			throw new Exception("The physics scene associated with the Unity scene is invalid.");
		}

		[StaticAccessor("GetPhysicsManager()", StaticAccessorType.Dot)]
		[NativeMethod("GetPhysicsSceneFromUnityScene")]
		private static PhysicsScene GetPhysicsScene_Internal(Scene scene)
		{
			PhysicsScene physicsScene;
			PhysicsSceneExtensions.GetPhysicsScene_Internal_Injected(ref scene, out physicsScene);
			return physicsScene;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPhysicsScene_Internal_Injected([In] ref Scene scene, out PhysicsScene ret);
	}
}
