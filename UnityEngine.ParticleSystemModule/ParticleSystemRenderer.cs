using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/ParticleSystem/ParticleSystemRenderer.h")]
	[NativeHeader("ParticleSystemScriptingClasses.h")]
	[RequireComponent(typeof(Transform))]
	[RequireComponent(typeof(Transform))]
	public sealed class ParticleSystemRenderer : Renderer
	{
		[Obsolete("EnableVertexStreams is deprecated.Use SetActiveVertexStreams instead.", false)]
		public void EnableVertexStreams(ParticleSystemVertexStreams streams)
		{
			this.Internal_SetVertexStreams(streams, true);
		}

		[Obsolete("DisableVertexStreams is deprecated.Use SetActiveVertexStreams instead.", false)]
		public void DisableVertexStreams(ParticleSystemVertexStreams streams)
		{
			this.Internal_SetVertexStreams(streams, false);
		}

		[Obsolete("AreVertexStreamsEnabled is deprecated.Use GetActiveVertexStreams instead.", false)]
		public bool AreVertexStreamsEnabled(ParticleSystemVertexStreams streams)
		{
			return this.Internal_GetEnabledVertexStreams(streams) == streams;
		}

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

		[NativeName("RenderAlignment")]
		public extern ParticleSystemRenderSpace alignment
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ParticleSystemRenderMode renderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern ParticleSystemSortMode sortMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float lengthScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float velocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float cameraVelocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float normalDirection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float shadowBias
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float sortingFudge
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float minParticleSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float maxParticleSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public Vector3 flip
		{
			get
			{
				Vector3 vector;
				this.get_flip_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_flip_Injected(ref value);
			}
		}

		public extern SpriteMaskInteraction maskInteraction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Material trailMaterial
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enableGPUInstancing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool allowRoll
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public void BakeMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeMesh(mesh, Camera.main, useTransform);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BakeMesh([NotNull] Mesh mesh, [NotNull] Camera camera, bool useTransform = false);

		public void BakeTrailsMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeTrailsMesh(mesh, Camera.main, useTransform);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BakeTrailsMesh([NotNull] Mesh mesh, [NotNull] Camera camera, bool useTransform = false);

		public extern Mesh mesh
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public int meshCount
		{
			get
			{
				return this.Internal_GetMeshCount();
			}
		}

		[GeneratedByOldBindingsGenerator]
		[RequiredByNativeCode]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int Internal_GetMeshCount();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetMeshes(Mesh[] meshes);

		public void SetMeshes(Mesh[] meshes)
		{
			this.SetMeshes(meshes, meshes.Length);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetMeshes(Mesh[] meshes, int size);

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_pivot_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_pivot_Injected(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_flip_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_flip_Injected(ref Vector3 value);
	}
}
