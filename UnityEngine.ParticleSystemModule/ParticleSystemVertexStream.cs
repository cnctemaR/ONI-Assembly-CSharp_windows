using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>All possible particle system vertex shader inputs.</para>
	/// </summary>
	[UsedByNativeCode]
	public enum ParticleSystemVertexStream
	{
		/// <summary>
		///   <para>The position of each particle vertex, in world space.</para>
		/// </summary>
		Position,
		/// <summary>
		///   <para>The vertex normal of each particle.</para>
		/// </summary>
		Normal,
		/// <summary>
		///   <para>The tangent vector for each particle (for normal mapping).</para>
		/// </summary>
		Tangent,
		/// <summary>
		///   <para>The color of each particle.</para>
		/// </summary>
		Color,
		/// <summary>
		///   <para>The first UV stream of each particle.</para>
		/// </summary>
		UV,
		/// <summary>
		///   <para>The second UV stream of each particle.</para>
		/// </summary>
		UV2,
		/// <summary>
		///   <para>The third UV stream of each particle (only for meshes).</para>
		/// </summary>
		UV3,
		/// <summary>
		///   <para>The fourth UV stream of each particle (only for meshes).</para>
		/// </summary>
		UV4,
		/// <summary>
		///   <para>The amount to blend between animated texture frames, from 0 to 1.</para>
		/// </summary>
		AnimBlend,
		/// <summary>
		///   <para>The current animation frame index of each particle.</para>
		/// </summary>
		AnimFrame,
		/// <summary>
		///   <para>The center position of the entire particle, in world space.</para>
		/// </summary>
		Center,
		/// <summary>
		///   <para>The vertex ID of each particle.</para>
		/// </summary>
		VertexID,
		/// <summary>
		///   <para>The X axis size of each particle.</para>
		/// </summary>
		SizeX,
		/// <summary>
		///   <para>The X and Y axis sizes of each particle.</para>
		/// </summary>
		SizeXY,
		/// <summary>
		///   <para>The 3D size of each particle.</para>
		/// </summary>
		SizeXYZ,
		/// <summary>
		///   <para>The Z axis rotation of each particle.</para>
		/// </summary>
		Rotation,
		/// <summary>
		///   <para>The 3D rotation of each particle.</para>
		/// </summary>
		Rotation3D,
		/// <summary>
		///   <para>The Z axis rotational speed of each particle.</para>
		/// </summary>
		RotationSpeed,
		/// <summary>
		///   <para>The 3D rotational speed of each particle.</para>
		/// </summary>
		RotationSpeed3D,
		/// <summary>
		///   <para>The velocity of each particle, in world space.</para>
		/// </summary>
		Velocity,
		/// <summary>
		///   <para>The speed of each particle, calculated by taking the magnitude of the velocity.</para>
		/// </summary>
		Speed,
		/// <summary>
		///   <para>The normalized age of each particle, from 0 to 1.</para>
		/// </summary>
		AgePercent,
		/// <summary>
		///   <para>The reciprocal of the starting lifetime, in seconds (1.0f / startLifetime).</para>
		/// </summary>
		InvStartLifetime,
		/// <summary>
		///   <para>A random number for each particle, which remains constant during their lifetime.</para>
		/// </summary>
		StableRandomX,
		/// <summary>
		///   <para>Two random numbers for each particle, which remain constant during their lifetime.</para>
		/// </summary>
		StableRandomXY,
		/// <summary>
		///   <para>Three random numbers for each particle, which remain constant during their lifetime.</para>
		/// </summary>
		StableRandomXYZ,
		/// <summary>
		///   <para>Four random numbers for each particle, which remain constant during their lifetime.</para>
		/// </summary>
		StableRandomXYZW,
		/// <summary>
		///   <para>A random number for each particle, which changes during their lifetime.</para>
		/// </summary>
		VaryingRandomX,
		/// <summary>
		///   <para>Two random numbers for each particle, which change during their lifetime.</para>
		/// </summary>
		VaryingRandomXY,
		/// <summary>
		///   <para>Three random numbers for each particle, which change during their lifetime.</para>
		/// </summary>
		VaryingRandomXYZ,
		/// <summary>
		///   <para>Four random numbers for each particle, which change during their lifetime.</para>
		/// </summary>
		VaryingRandomXYZW,
		/// <summary>
		///   <para>One custom value for each particle, defined by the Custom Data Module, or ParticleSystem.SetCustomParticleData.</para>
		/// </summary>
		Custom1X,
		/// <summary>
		///   <para>Two custom values for each particle, defined by the Custom Data Module, or ParticleSystem.SetCustomParticleData.</para>
		/// </summary>
		Custom1XY,
		/// <summary>
		///   <para>Three custom values for each particle, defined by the Custom Data Module, or ParticleSystem.SetCustomParticleData.</para>
		/// </summary>
		Custom1XYZ,
		/// <summary>
		///   <para>Four custom values for each particle, defined by the Custom Data Module, or ParticleSystem.SetCustomParticleData.</para>
		/// </summary>
		Custom1XYZW,
		/// <summary>
		///   <para>One custom value for each particle, defined by the Custom Data Module, or ParticleSystem.SetCustomParticleData.</para>
		/// </summary>
		Custom2X,
		/// <summary>
		///   <para>Two custom values for each particle, defined by the Custom Data Module, or ParticleSystem.SetCustomParticleData.</para>
		/// </summary>
		Custom2XY,
		/// <summary>
		///   <para>Three custom values for each particle, defined by the Custom Data Module, or ParticleSystem.SetCustomParticleData.</para>
		/// </summary>
		Custom2XYZ,
		/// <summary>
		///   <para>Four custom values for each particle, defined by the Custom Data Module, or ParticleSystem.SetCustomParticleData.</para>
		/// </summary>
		Custom2XYZW,
		/// <summary>
		///   <para>The accumulated X axis noise, over the lifetime of the particle.</para>
		/// </summary>
		NoiseSumX,
		/// <summary>
		///   <para>The accumulated X and Y axis noise, over the lifetime of the particle.</para>
		/// </summary>
		NoiseSumXY,
		/// <summary>
		///   <para>The accumulated 3D noise, over the lifetime of the particle.</para>
		/// </summary>
		NoiseSumXYZ,
		/// <summary>
		///   <para>The X axis noise on the current frame.</para>
		/// </summary>
		NoiseImpulseX,
		/// <summary>
		///   <para>The X and Y axis noise on the current frame.</para>
		/// </summary>
		NoiseImpulseXY,
		/// <summary>
		///   <para>The 3D noise on the current frame.</para>
		/// </summary>
		NoiseImpulseXYZ
	}
}
