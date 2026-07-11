using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Renders particles on to the screen (Shuriken).</para>
	/// </summary>
	[RequireComponent(typeof(Transform))]
	[NativeHeader("ParticleSystemScriptingClasses.h")]
	[NativeHeader("Runtime/ParticleSystem/ParticleSystemRenderer.h")]
	[RequireComponent(typeof(Transform))]
	public sealed class ParticleSystemRenderer : Renderer
	{
		/// <summary>
		///   <para>Mesh used as particle instead of billboarded texture.</para>
		/// </summary>
		public extern Mesh mesh
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The number of meshes being used for particle rendering.</para>
		/// </summary>
		public int meshCount
		{
			get
			{
				return this.Internal_GetMeshCount();
			}
		}

		[RequiredByNativeCode]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetMeshCount();

		/// <summary>
		///   <para>Get the array of meshes to be used as particles.</para>
		/// </summary>
		/// <param name="meshes">This array will be populated with the list of meshes being used for particle rendering.</param>
		/// <returns>
		///   <para>The number of meshes actually written to the destination array.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetMeshes(Mesh[] meshes);

		/// <summary>
		///   <para>Set an array of meshes to be used as particles when the ParticleSystemRenderer.renderMode is set to ParticleSystemRenderMode.Mesh.</para>
		/// </summary>
		/// <param name="meshes">Array of meshes to be used.</param>
		/// <param name="size">Number of elements from the mesh array to be applied.</param>
		public void SetMeshes(Mesh[] meshes)
		{
			this.SetMeshes(meshes, meshes.Length);
		}

		/// <summary>
		///   <para>Set an array of meshes to be used as particles when the ParticleSystemRenderer.renderMode is set to ParticleSystemRenderMode.Mesh.</para>
		/// </summary>
		/// <param name="meshes">Array of meshes to be used.</param>
		/// <param name="size">Number of elements from the mesh array to be applied.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetMeshes(Mesh[] meshes, int size);

		/// <summary>
		///   <para>The number of currently active custom vertex streams.</para>
		/// </summary>
		public extern int activeVertexStreamsCount
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public void SetActiveVertexStreams(List<ParticleSystemVertexStream> streams)
		{
			if (streams == null)
			{
				throw new ArgumentNullException("streams");
			}
			this.SetActiveVertexStreamsInternal(streams);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetActiveVertexStreamsInternal(object streams);

		public void GetActiveVertexStreams(List<ParticleSystemVertexStream> streams)
		{
			if (streams == null)
			{
				throw new ArgumentNullException("streams");
			}
			this.GetActiveVertexStreamsInternal(streams);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void GetActiveVertexStreamsInternal(object streams);

		/// <summary>
		///   <para>Enable a set of vertex shader streams on the particle system renderer.</para>
		/// </summary>
		/// <param name="streams">Streams to enable.</param>
		[Obsolete("EnableVertexStreams is deprecated.Use SetActiveVertexStreams instead.", false)]
		public void EnableVertexStreams(ParticleSystemVertexStreams streams)
		{
			this.Internal_SetVertexStreams(streams, true);
		}

		/// <summary>
		///   <para>Disable a set of vertex shader streams on the particle system renderer.
		/// The position stream is always enabled, and any attempts to remove it will be ignored.</para>
		/// </summary>
		/// <param name="streams">Streams to disable.</param>
		[Obsolete("DisableVertexStreams is deprecated.Use SetActiveVertexStreams instead.", false)]
		public void DisableVertexStreams(ParticleSystemVertexStreams streams)
		{
			this.Internal_SetVertexStreams(streams, false);
		}

		/// <summary>
		///   <para>Query whether the particle system renderer uses a particular set of vertex streams.</para>
		/// </summary>
		/// <param name="streams">Streams to query.</param>
		/// <returns>
		///   <para>Whether all the queried streams are enabled or not.</para>
		/// </returns>
		[Obsolete("AreVertexStreamsEnabled is deprecated.Use GetActiveVertexStreams instead.", false)]
		public bool AreVertexStreamsEnabled(ParticleSystemVertexStreams streams)
		{
			return this.Internal_GetEnabledVertexStreams(streams) == streams;
		}

		/// <summary>
		///   <para>Query whether the particle system renderer uses a particular set of vertex streams.</para>
		/// </summary>
		/// <param name="streams">Streams to query.</param>
		/// <returns>
		///   <para>Returns the subset of the queried streams that are actually enabled.</para>
		/// </returns>
		[Obsolete("GetEnabledVertexStreams is deprecated.Use GetActiveVertexStreams instead.", false)]
		public ParticleSystemVertexStreams GetEnabledVertexStreams(ParticleSystemVertexStreams streams)
		{
			return this.Internal_GetEnabledVertexStreams(streams);
		}

		[Obsolete("Internal_SetVertexStreams is deprecated.Use SetActiveVertexStreams instead.", false)]
		internal void Internal_SetVertexStreams(ParticleSystemVertexStreams streams, bool enabled)
		{
			List<ParticleSystemVertexStream> list = new List<ParticleSystemVertexStream>(this.activeVertexStreamsCount);
			this.GetActiveVertexStreams(list);
			if (enabled)
			{
				if ((streams & ParticleSystemVertexStreams.Position) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Position))
					{
						list.Add(ParticleSystemVertexStream.Position);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Normal) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Normal))
					{
						list.Add(ParticleSystemVertexStream.Normal);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Tangent) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Tangent))
					{
						list.Add(ParticleSystemVertexStream.Tangent);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Color) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Color))
					{
						list.Add(ParticleSystemVertexStream.Color);
					}
				}
				if ((streams & ParticleSystemVertexStreams.UV) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.UV))
					{
						list.Add(ParticleSystemVertexStream.UV);
					}
				}
				if ((streams & ParticleSystemVertexStreams.UV2BlendAndFrame) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.UV2))
					{
						list.Add(ParticleSystemVertexStream.UV2);
						list.Add(ParticleSystemVertexStream.AnimBlend);
						list.Add(ParticleSystemVertexStream.AnimFrame);
					}
				}
				if ((streams & ParticleSystemVertexStreams.CenterAndVertexID) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Center))
					{
						list.Add(ParticleSystemVertexStream.Center);
						list.Add(ParticleSystemVertexStream.VertexID);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Size) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.SizeXYZ))
					{
						list.Add(ParticleSystemVertexStream.SizeXYZ);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Rotation) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Rotation3D))
					{
						list.Add(ParticleSystemVertexStream.Rotation3D);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Velocity) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Velocity))
					{
						list.Add(ParticleSystemVertexStream.Velocity);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Lifetime) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.AgePercent))
					{
						list.Add(ParticleSystemVertexStream.AgePercent);
						list.Add(ParticleSystemVertexStream.InvStartLifetime);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Custom1) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Custom1XYZW))
					{
						list.Add(ParticleSystemVertexStream.Custom1XYZW);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Custom2) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.Custom2XYZW))
					{
						list.Add(ParticleSystemVertexStream.Custom2XYZW);
					}
				}
				if ((streams & ParticleSystemVertexStreams.Random) != ParticleSystemVertexStreams.None)
				{
					if (!list.Contains(ParticleSystemVertexStream.StableRandomXYZ))
					{
						list.Add(ParticleSystemVertexStream.StableRandomXYZ);
						list.Add(ParticleSystemVertexStream.VaryingRandomX);
					}
				}
			}
			else
			{
				if ((streams & ParticleSystemVertexStreams.Position) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Position);
				}
				if ((streams & ParticleSystemVertexStreams.Normal) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Normal);
				}
				if ((streams & ParticleSystemVertexStreams.Tangent) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Tangent);
				}
				if ((streams & ParticleSystemVertexStreams.Color) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Color);
				}
				if ((streams & ParticleSystemVertexStreams.UV) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.UV);
				}
				if ((streams & ParticleSystemVertexStreams.UV2BlendAndFrame) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.UV2);
					list.Remove(ParticleSystemVertexStream.AnimBlend);
					list.Remove(ParticleSystemVertexStream.AnimFrame);
				}
				if ((streams & ParticleSystemVertexStreams.CenterAndVertexID) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Center);
					list.Remove(ParticleSystemVertexStream.VertexID);
				}
				if ((streams & ParticleSystemVertexStreams.Size) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.SizeXYZ);
				}
				if ((streams & ParticleSystemVertexStreams.Rotation) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Rotation3D);
				}
				if ((streams & ParticleSystemVertexStreams.Velocity) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Velocity);
				}
				if ((streams & ParticleSystemVertexStreams.Lifetime) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.AgePercent);
					list.Remove(ParticleSystemVertexStream.InvStartLifetime);
				}
				if ((streams & ParticleSystemVertexStreams.Custom1) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Custom1XYZW);
				}
				if ((streams & ParticleSystemVertexStreams.Custom2) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.Custom2XYZW);
				}
				if ((streams & ParticleSystemVertexStreams.Random) != ParticleSystemVertexStreams.None)
				{
					list.Remove(ParticleSystemVertexStream.StableRandomXYZW);
					list.Remove(ParticleSystemVertexStream.VaryingRandomX);
				}
			}
			this.SetActiveVertexStreams(list);
		}

		[Obsolete("Internal_GetVertexStreams is deprecated.Use GetActiveVertexStreams instead.", false)]
		internal ParticleSystemVertexStreams Internal_GetEnabledVertexStreams(ParticleSystemVertexStreams streams)
		{
			List<ParticleSystemVertexStream> list = new List<ParticleSystemVertexStream>(this.activeVertexStreamsCount);
			this.GetActiveVertexStreams(list);
			ParticleSystemVertexStreams particleSystemVertexStreams = ParticleSystemVertexStreams.None;
			if (list.Contains(ParticleSystemVertexStream.Position))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Position;
			}
			if (list.Contains(ParticleSystemVertexStream.Normal))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Normal;
			}
			if (list.Contains(ParticleSystemVertexStream.Tangent))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Tangent;
			}
			if (list.Contains(ParticleSystemVertexStream.Color))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Color;
			}
			if (list.Contains(ParticleSystemVertexStream.UV))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.UV;
			}
			if (list.Contains(ParticleSystemVertexStream.UV2))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.UV2BlendAndFrame;
			}
			if (list.Contains(ParticleSystemVertexStream.Center))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.CenterAndVertexID;
			}
			if (list.Contains(ParticleSystemVertexStream.SizeXYZ))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Size;
			}
			if (list.Contains(ParticleSystemVertexStream.Rotation3D))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Rotation;
			}
			if (list.Contains(ParticleSystemVertexStream.Velocity))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Velocity;
			}
			if (list.Contains(ParticleSystemVertexStream.AgePercent))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Lifetime;
			}
			if (list.Contains(ParticleSystemVertexStream.Custom1XYZW))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Custom1;
			}
			if (list.Contains(ParticleSystemVertexStream.Custom2XYZW))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Custom2;
			}
			if (list.Contains(ParticleSystemVertexStream.StableRandomXYZ))
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Random;
			}
			return particleSystemVertexStreams & streams;
		}

		/// <summary>
		///   <para>Control the direction that particles face.</para>
		/// </summary>
		[NativeName("RenderAlignment")]
		public extern ParticleSystemRenderSpace alignment
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How particles are drawn.</para>
		/// </summary>
		public extern ParticleSystemRenderMode renderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Sort particles within a system.</para>
		/// </summary>
		public extern ParticleSystemSortMode sortMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much are the particles stretched in their direction of motion.</para>
		/// </summary>
		public extern float lengthScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much are the particles stretched depending on "how fast they move".</para>
		/// </summary>
		public extern float velocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much are the particles stretched depending on the Camera's speed.</para>
		/// </summary>
		public extern float cameraVelocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>How much are billboard particle normals oriented towards the camera.</para>
		/// </summary>
		public extern float normalDirection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Biases particle system sorting amongst other transparencies.</para>
		/// </summary>
		public extern float sortingFudge
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Clamp the minimum particle size.</para>
		/// </summary>
		public extern float minParticleSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Clamp the maximum particle size.</para>
		/// </summary>
		public extern float maxParticleSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Modify the pivot point used for rotating particles.</para>
		/// </summary>
		public Vector3 pivot
		{
			get
			{
				Vector3 vector;
				this.get_pivot_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_pivot_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Specifies how the Particle System Renderer interacts with SpriteMask.</para>
		/// </summary>
		public extern SpriteMaskInteraction maskInteraction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Set the material used by the Trail module for attaching trails to particles.</para>
		/// </summary>
		public extern Material trailMaterial
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Enables GPU Instancing on platforms where it is supported.</para>
		/// </summary>
		public extern bool enableGPUInstancing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Creates a snapshot of ParticleSystemRenderer and stores it in mesh.</para>
		/// </summary>
		/// <param name="mesh">A static mesh that will receive the snapshot of the particles.</param>
		/// <param name="camera">The camera used for determining which way view space particles will face.</param>
		/// <param name="useTransform">Include the rotation and scale of the Transform in the baked mesh.</param>
		public void BakeMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeMesh(mesh, Camera.main, useTransform);
		}

		/// <summary>
		///   <para>Creates a snapshot of ParticleSystemRenderer and stores it in mesh.</para>
		/// </summary>
		/// <param name="mesh">A static mesh that will receive the snapshot of the particles.</param>
		/// <param name="camera">The camera used for determining which way view space particles will face.</param>
		/// <param name="useTransform">Include the rotation and scale of the Transform in the baked mesh.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BakeMesh([NotNull] Mesh mesh, [NotNull] Camera camera, bool useTransform = false);

		/// <summary>
		///   <para>Creates a snapshot of ParticleSystem Trails and stores them in mesh.</para>
		/// </summary>
		/// <param name="mesh">A static mesh that will receive the snapshot of the particle trails.</param>
		/// <param name="camera">The camera used for determining which way view space trails will face.</param>
		/// <param name="useTransform">Include the rotation and scale of the Transform in the baked mesh.</param>
		public void BakeTrailsMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeTrailsMesh(mesh, Camera.main, useTransform);
		}

		/// <summary>
		///   <para>Creates a snapshot of ParticleSystem Trails and stores them in mesh.</para>
		/// </summary>
		/// <param name="mesh">A static mesh that will receive the snapshot of the particle trails.</param>
		/// <param name="camera">The camera used for determining which way view space trails will face.</param>
		/// <param name="useTransform">Include the rotation and scale of the Transform in the baked mesh.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BakeTrailsMesh([NotNull] Mesh mesh, [NotNull] Camera camera, bool useTransform = false);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_pivot_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_pivot_Injected(ref Vector3 value);
	}
}
