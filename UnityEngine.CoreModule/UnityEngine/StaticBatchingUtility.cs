using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>StaticBatchingUtility can prepare your objects to take advantage of Unity's static batching.</para>
	/// </summary>
	public sealed class StaticBatchingUtility
	{
		/// <summary>
		///   <para>StaticBatchingUtility.Combine prepares all children of the staticBatchRoot for static batching.</para>
		/// </summary>
		/// <param name="staticBatchRoot"></param>
		public static void Combine(GameObject staticBatchRoot)
		{
			InternalStaticBatchingUtility.CombineRoot(staticBatchRoot);
		}

		/// <summary>
		///   <para>StaticBatchingUtility.Combine prepares all gos for static batching. staticBatchRoot is treated as their parent.</para>
		/// </summary>
		/// <param name="gos"></param>
		/// <param name="staticBatchRoot"></param>
		public static void Combine(GameObject[] gos, GameObject staticBatchRoot)
		{
			InternalStaticBatchingUtility.CombineGameObjects(gos, staticBatchRoot, false);
		}
	}
}
