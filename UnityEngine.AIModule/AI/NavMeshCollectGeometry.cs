using System;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Used for specifying the type of geometry to collect. Used with NavMeshBuilder.CollectSources.</para>
	/// </summary>
	public enum NavMeshCollectGeometry
	{
		/// <summary>
		///   <para>Collect meshes form the rendered geometry.</para>
		/// </summary>
		RenderMeshes,
		/// <summary>
		///   <para>Collect geometry from the 3D physics collision representation.</para>
		/// </summary>
		PhysicsColliders
	}
}
