using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.SceneManagement;

namespace UnityEngine
{
	public static class PhysicsSceneExtensions
	{
		public static PhysicsScene GetPhysicsScene(this Scene scene)
		{
			if (!scene.IsValid())
			{
				throw new ArgumentException("Cannot get physics scene; Unity scene is invalid.", "scene");
			}
			PhysicsScene physicsScene_Internal = PhysicsSceneExtensions.GetPhysicsScene_Internal(scene);
			if (physicsScene_Internal.IsValid())
			{
				return physicsScene_Internal;
			}
			throw new Exception("The physics scene associated with the Unity scene is invalid.");
		}

		[NativeMethod("GetPhysicsSceneFromUnityScene")]
		[StaticAccessor("GetPhysicsManager()", StaticAccessorType.Dot)]
		private static PhysicsScene GetPhysicsScene_Internal(Scene scene)
		{
			PhysicsScene physicsScene;
			PhysicsSceneExtensions.GetPhysicsScene_Internal_Injected(ref scene, out physicsScene);
			return physicsScene;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPhysicsScene_Internal_Injected(ref Scene scene, out PhysicsScene ret);
	}
}
