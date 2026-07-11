using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Enum mask of possible shader channel properties that can also be included when the Canvas mesh is created.</para>
	/// </summary>
	[Flags]
	public enum AdditionalCanvasShaderChannels
	{
		/// <summary>
		///   <para>No additional shader parameters are needed.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Include UV1 on the mesh vertices.</para>
		/// </summary>
		TexCoord1 = 1,
		/// <summary>
		///   <para>Include UV2 on the mesh vertices.</para>
		/// </summary>
		TexCoord2 = 2,
		/// <summary>
		///   <para>Include UV3 on the mesh vertices.</para>
		/// </summary>
		TexCoord3 = 4,
		/// <summary>
		///   <para>Include the normals on the mesh vertices.</para>
		/// </summary>
		Normal = 8,
		/// <summary>
		///   <para>Include the Tangent on the mesh vertices.</para>
		/// </summary>
		Tangent = 16
	}
}
