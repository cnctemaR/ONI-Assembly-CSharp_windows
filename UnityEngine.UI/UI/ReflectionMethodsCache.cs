using System;
using System.Reflection;

namespace UnityEngine.UI
{
	internal class ReflectionMethodsCache
	{
		public ReflectionMethodsCache()
		{
			MethodInfo method = typeof(Physics).GetMethod("Raycast", new Type[]
			{
				typeof(Ray),
				typeof(RaycastHit).MakeByRefType(),
				typeof(float),
				typeof(int)
			});
			if (method != null)
			{
				this.raycast3D = (ReflectionMethodsCache.Raycast3DCallback)Delegate.CreateDelegate(typeof(ReflectionMethodsCache.Raycast3DCallback), method);
			}
			MethodInfo method2 = typeof(Physics).GetMethod("RaycastAll", new Type[]
			{
				typeof(Ray),
				typeof(float),
				typeof(int)
			});
			if (method2 != null)
			{
				this.raycast3DAll = (ReflectionMethodsCache.RaycastAllCallback)Delegate.CreateDelegate(typeof(ReflectionMethodsCache.RaycastAllCallback), method2);
			}
			MethodInfo method3 = typeof(Physics).GetMethod("RaycastNonAlloc", new Type[]
			{
				typeof(Ray),
				typeof(RaycastHit[]),
				typeof(float),
				typeof(int)
			});
			if (method3 != null)
			{
				this.getRaycastNonAlloc = (ReflectionMethodsCache.GetRaycastNonAllocCallback)Delegate.CreateDelegate(typeof(ReflectionMethodsCache.GetRaycastNonAllocCallback), method3);
			}
			MethodInfo method4 = typeof(Physics2D).GetMethod("Raycast", new Type[]
			{
				typeof(Vector2),
				typeof(Vector2),
				typeof(float),
				typeof(int)
			});
			if (method4 != null)
			{
				this.raycast2D = (ReflectionMethodsCache.Raycast2DCallback)Delegate.CreateDelegate(typeof(ReflectionMethodsCache.Raycast2DCallback), method4);
			}
			MethodInfo method5 = typeof(Physics2D).GetMethod("GetRayIntersectionAll", new Type[]
			{
				typeof(Ray),
				typeof(float),
				typeof(int)
			});
			if (method5 != null)
			{
				this.getRayIntersectionAll = (ReflectionMethodsCache.GetRayIntersectionAllCallback)Delegate.CreateDelegate(typeof(ReflectionMethodsCache.GetRayIntersectionAllCallback), method5);
			}
			MethodInfo method6 = typeof(Physics2D).GetMethod("GetRayIntersectionNonAlloc", new Type[]
			{
				typeof(Ray),
				typeof(RaycastHit2D[]),
				typeof(float),
				typeof(int)
			});
			if (method6 != null)
			{
				this.getRayIntersectionAllNonAlloc = (ReflectionMethodsCache.GetRayIntersectionAllNonAllocCallback)Delegate.CreateDelegate(typeof(ReflectionMethodsCache.GetRayIntersectionAllNonAllocCallback), method6);
			}
		}

		public static ReflectionMethodsCache Singleton
		{
			get
			{
				if (ReflectionMethodsCache.s_ReflectionMethodsCache == null)
				{
					ReflectionMethodsCache.s_ReflectionMethodsCache = new ReflectionMethodsCache();
				}
				return ReflectionMethodsCache.s_ReflectionMethodsCache;
			}
		}

		public ReflectionMethodsCache.Raycast3DCallback raycast3D;

		public ReflectionMethodsCache.RaycastAllCallback raycast3DAll;

		public ReflectionMethodsCache.GetRaycastNonAllocCallback getRaycastNonAlloc;

		public ReflectionMethodsCache.Raycast2DCallback raycast2D;

		public ReflectionMethodsCache.GetRayIntersectionAllCallback getRayIntersectionAll;

		public ReflectionMethodsCache.GetRayIntersectionAllNonAllocCallback getRayIntersectionAllNonAlloc;

		private static ReflectionMethodsCache s_ReflectionMethodsCache;

		public delegate bool Raycast3DCallback(Ray r, out RaycastHit hit, float f, int i);

		public delegate RaycastHit[] RaycastAllCallback(Ray r, float f, int i);

		public delegate int GetRaycastNonAllocCallback(Ray r, RaycastHit[] results, float f, int i);

		public delegate RaycastHit2D Raycast2DCallback(Vector2 p1, Vector2 p2, float f, int i);

		public delegate RaycastHit2D[] GetRayIntersectionAllCallback(Ray r, float f, int i);

		public delegate int GetRayIntersectionAllNonAllocCallback(Ray r, RaycastHit2D[] results, float f, int i);
	}
}
