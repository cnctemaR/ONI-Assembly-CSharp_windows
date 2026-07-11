using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>The mesh emission type.</para>
	/// </summary>
	public enum ParticleSystemMeshShapeType
	{
		/// <summary>
		///   <para>Emit from the vertices of the mesh.</para>
		/// </summary>
		Vertex,
		/// <summary>
		///   <para>Emit from the edges of the mesh.</para>
		/// </summary>
		Edge,
		/// <summary>
		///   <para>Emit from the surface of the mesh.</para>
		/// </summary>
		Triangle
	}
}
