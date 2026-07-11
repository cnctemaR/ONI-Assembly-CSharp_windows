using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>All possible particle system vertex shader inputs.</para>
	/// </summary>
	[Flags]
	[Obsolete("ParticleSystemVertexStreams is deprecated. Please use ParticleSystemVertexStream instead.", false)]
	public enum ParticleSystemVertexStreams
	{
		/// <summary>
		///   <para>The world space position of each particle.</para>
		/// </summary>
		Position = 1,
		/// <summary>
		///   <para>The normal of each particle.</para>
		/// </summary>
		Normal = 2,
		/// <summary>
		///   <para>Tangent vectors for normal mapping.</para>
		/// </summary>
		Tangent = 4,
		/// <summary>
		///   <para>The color of each particle.</para>
		/// </summary>
		Color = 8,
		/// <summary>
		///   <para>The texture coordinates of each particle.</para>
		/// </summary>
		UV = 16,
		/// <summary>
		///   <para>With the Texture Sheet Animation module enabled, this contains the UVs for the second texture frame, the blend factor for each particle, and the raw frame, allowing for blending of frames.</para>
		/// </summary>
		UV2BlendAndFrame = 32,
		/// <summary>
		///   <para>The center position of each particle, with the vertex ID of each particle, from 0-3, stored in the w component.</para>
		/// </summary>
		CenterAndVertexID = 64,
		/// <summary>
		///   <para>The size of each particle.</para>
		/// </summary>
		Size = 128,
		/// <summary>
		///   <para>The rotation of each particle.</para>
		/// </summary>
		Rotation = 256,
		/// <summary>
		///   <para>The 3D velocity of each particle.</para>
		/// </summary>
		Velocity = 512,
		/// <summary>
		///   <para>Alive time as a 0-1 value in the X component, and Total Lifetime in the Y component.
		/// To get the current particle age, simply multiply X by Y.</para>
		/// </summary>
		Lifetime = 1024,
		/// <summary>
		///   <para>The first stream of custom data, supplied from script.</para>
		/// </summary>
		Custom1 = 2048,
		/// <summary>
		///   <para>The second stream of custom data, supplied from script.</para>
		/// </summary>
		Custom2 = 4096,
		/// <summary>
		///   <para>4 random numbers. The first 3 are deterministic and assigned once when each particle is born, but the 4th value will change during the lifetime of the particle.</para>
		/// </summary>
		Random = 8192,
		/// <summary>
		///   <para>A mask with no vertex streams enabled.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>A mask with all vertex streams enabled.</para>
		/// </summary>
		All = 2147483647
	}
}
