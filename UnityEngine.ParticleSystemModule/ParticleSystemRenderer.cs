using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/ParticleSystem/ParticleSystemRenderer.h")]
	[NativeHeader("ParticleSystemScriptingClasses.h")]
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemRendererScriptBindings.h")]
	public sealed class ParticleSystemRenderer : Renderer
	{
		[Obsolete("EnableVertexStreams is deprecated. Use SetActiveVertexStreams instead.", false)]
		public void EnableVertexStreams(ParticleSystemVertexStreams streams)
		{
			this.Internal_SetVertexStreams(streams, true);
		}

		[Obsolete("DisableVertexStreams is deprecated. Use SetActiveVertexStreams instead.", false)]
		public void DisableVertexStreams(ParticleSystemVertexStreams streams)
		{
			this.Internal_SetVertexStreams(streams, false);
		}

		[Obsolete("AreVertexStreamsEnabled is deprecated. Use GetActiveVertexStreams instead.", false)]
		public bool AreVertexStreamsEnabled(ParticleSystemVertexStreams streams)
		{
			return this.Internal_GetEnabledVertexStreams(streams) == streams;
		}

		[Obsolete("GetEnabledVertexStreams is deprecated. Use GetActiveVertexStreams instead.", false)]
		public ParticleSystemVertexStreams GetEnabledVertexStreams(ParticleSystemVertexStreams streams)
		{
			return this.Internal_GetEnabledVertexStreams(streams);
		}

		[Obsolete("Internal_SetVertexStreams is deprecated. Use SetActiveVertexStreams instead.", false)]
		internal void Internal_SetVertexStreams(ParticleSystemVertexStreams streams, bool enabled)
		{
			List<ParticleSystemVertexStream> list = new List<ParticleSystemVertexStream>(this.activeVertexStreamsCount);
			this.GetActiveVertexStreams(list);
			if (enabled)
			{
				bool flag = (streams & ParticleSystemVertexStreams.Position) > ParticleSystemVertexStreams.None;
				if (flag)
				{
					bool flag2 = !list.Contains(ParticleSystemVertexStream.Position);
					if (flag2)
					{
						list.Add(ParticleSystemVertexStream.Position);
					}
				}
				bool flag3 = (streams & ParticleSystemVertexStreams.Normal) > ParticleSystemVertexStreams.None;
				if (flag3)
				{
					bool flag4 = !list.Contains(ParticleSystemVertexStream.Normal);
					if (flag4)
					{
						list.Add(ParticleSystemVertexStream.Normal);
					}
				}
				bool flag5 = (streams & ParticleSystemVertexStreams.Tangent) > ParticleSystemVertexStreams.None;
				if (flag5)
				{
					bool flag6 = !list.Contains(ParticleSystemVertexStream.Tangent);
					if (flag6)
					{
						list.Add(ParticleSystemVertexStream.Tangent);
					}
				}
				bool flag7 = (streams & ParticleSystemVertexStreams.Color) > ParticleSystemVertexStreams.None;
				if (flag7)
				{
					bool flag8 = !list.Contains(ParticleSystemVertexStream.Color);
					if (flag8)
					{
						list.Add(ParticleSystemVertexStream.Color);
					}
				}
				bool flag9 = (streams & ParticleSystemVertexStreams.UV) > ParticleSystemVertexStreams.None;
				if (flag9)
				{
					bool flag10 = !list.Contains(ParticleSystemVertexStream.UV);
					if (flag10)
					{
						list.Add(ParticleSystemVertexStream.UV);
					}
				}
				bool flag11 = (streams & ParticleSystemVertexStreams.UV2BlendAndFrame) > ParticleSystemVertexStreams.None;
				if (flag11)
				{
					bool flag12 = !list.Contains(ParticleSystemVertexStream.UV2);
					if (flag12)
					{
						list.Add(ParticleSystemVertexStream.UV2);
						list.Add(ParticleSystemVertexStream.AnimBlend);
						list.Add(ParticleSystemVertexStream.AnimFrame);
					}
				}
				bool flag13 = (streams & ParticleSystemVertexStreams.CenterAndVertexID) > ParticleSystemVertexStreams.None;
				if (flag13)
				{
					bool flag14 = !list.Contains(ParticleSystemVertexStream.Center);
					if (flag14)
					{
						list.Add(ParticleSystemVertexStream.Center);
						list.Add(ParticleSystemVertexStream.VertexID);
					}
				}
				bool flag15 = (streams & ParticleSystemVertexStreams.Size) > ParticleSystemVertexStreams.None;
				if (flag15)
				{
					bool flag16 = !list.Contains(ParticleSystemVertexStream.SizeXYZ);
					if (flag16)
					{
						list.Add(ParticleSystemVertexStream.SizeXYZ);
					}
				}
				bool flag17 = (streams & ParticleSystemVertexStreams.Rotation) > ParticleSystemVertexStreams.None;
				if (flag17)
				{
					bool flag18 = !list.Contains(ParticleSystemVertexStream.Rotation3D);
					if (flag18)
					{
						list.Add(ParticleSystemVertexStream.Rotation3D);
					}
				}
				bool flag19 = (streams & ParticleSystemVertexStreams.Velocity) > ParticleSystemVertexStreams.None;
				if (flag19)
				{
					bool flag20 = !list.Contains(ParticleSystemVertexStream.Velocity);
					if (flag20)
					{
						list.Add(ParticleSystemVertexStream.Velocity);
					}
				}
				bool flag21 = (streams & ParticleSystemVertexStreams.Lifetime) > ParticleSystemVertexStreams.None;
				if (flag21)
				{
					bool flag22 = !list.Contains(ParticleSystemVertexStream.AgePercent);
					if (flag22)
					{
						list.Add(ParticleSystemVertexStream.AgePercent);
						list.Add(ParticleSystemVertexStream.InvStartLifetime);
					}
				}
				bool flag23 = (streams & ParticleSystemVertexStreams.Custom1) > ParticleSystemVertexStreams.None;
				if (flag23)
				{
					bool flag24 = !list.Contains(ParticleSystemVertexStream.Custom1XYZW);
					if (flag24)
					{
						list.Add(ParticleSystemVertexStream.Custom1XYZW);
					}
				}
				bool flag25 = (streams & ParticleSystemVertexStreams.Custom2) > ParticleSystemVertexStreams.None;
				if (flag25)
				{
					bool flag26 = !list.Contains(ParticleSystemVertexStream.Custom2XYZW);
					if (flag26)
					{
						list.Add(ParticleSystemVertexStream.Custom2XYZW);
					}
				}
				bool flag27 = (streams & ParticleSystemVertexStreams.Random) > ParticleSystemVertexStreams.None;
				if (flag27)
				{
					bool flag28 = !list.Contains(ParticleSystemVertexStream.StableRandomXYZ);
					if (flag28)
					{
						list.Add(ParticleSystemVertexStream.StableRandomXYZ);
						list.Add(ParticleSystemVertexStream.VaryingRandomX);
					}
				}
			}
			else
			{
				bool flag29 = (streams & ParticleSystemVertexStreams.Position) > ParticleSystemVertexStreams.None;
				if (flag29)
				{
					list.Remove(ParticleSystemVertexStream.Position);
				}
				bool flag30 = (streams & ParticleSystemVertexStreams.Normal) > ParticleSystemVertexStreams.None;
				if (flag30)
				{
					list.Remove(ParticleSystemVertexStream.Normal);
				}
				bool flag31 = (streams & ParticleSystemVertexStreams.Tangent) > ParticleSystemVertexStreams.None;
				if (flag31)
				{
					list.Remove(ParticleSystemVertexStream.Tangent);
				}
				bool flag32 = (streams & ParticleSystemVertexStreams.Color) > ParticleSystemVertexStreams.None;
				if (flag32)
				{
					list.Remove(ParticleSystemVertexStream.Color);
				}
				bool flag33 = (streams & ParticleSystemVertexStreams.UV) > ParticleSystemVertexStreams.None;
				if (flag33)
				{
					list.Remove(ParticleSystemVertexStream.UV);
				}
				bool flag34 = (streams & ParticleSystemVertexStreams.UV2BlendAndFrame) > ParticleSystemVertexStreams.None;
				if (flag34)
				{
					list.Remove(ParticleSystemVertexStream.UV2);
					list.Remove(ParticleSystemVertexStream.AnimBlend);
					list.Remove(ParticleSystemVertexStream.AnimFrame);
				}
				bool flag35 = (streams & ParticleSystemVertexStreams.CenterAndVertexID) > ParticleSystemVertexStreams.None;
				if (flag35)
				{
					list.Remove(ParticleSystemVertexStream.Center);
					list.Remove(ParticleSystemVertexStream.VertexID);
				}
				bool flag36 = (streams & ParticleSystemVertexStreams.Size) > ParticleSystemVertexStreams.None;
				if (flag36)
				{
					list.Remove(ParticleSystemVertexStream.SizeXYZ);
				}
				bool flag37 = (streams & ParticleSystemVertexStreams.Rotation) > ParticleSystemVertexStreams.None;
				if (flag37)
				{
					list.Remove(ParticleSystemVertexStream.Rotation3D);
				}
				bool flag38 = (streams & ParticleSystemVertexStreams.Velocity) > ParticleSystemVertexStreams.None;
				if (flag38)
				{
					list.Remove(ParticleSystemVertexStream.Velocity);
				}
				bool flag39 = (streams & ParticleSystemVertexStreams.Lifetime) > ParticleSystemVertexStreams.None;
				if (flag39)
				{
					list.Remove(ParticleSystemVertexStream.AgePercent);
					list.Remove(ParticleSystemVertexStream.InvStartLifetime);
				}
				bool flag40 = (streams & ParticleSystemVertexStreams.Custom1) > ParticleSystemVertexStreams.None;
				if (flag40)
				{
					list.Remove(ParticleSystemVertexStream.Custom1XYZW);
				}
				bool flag41 = (streams & ParticleSystemVertexStreams.Custom2) > ParticleSystemVertexStreams.None;
				if (flag41)
				{
					list.Remove(ParticleSystemVertexStream.Custom2XYZW);
				}
				bool flag42 = (streams & ParticleSystemVertexStreams.Random) > ParticleSystemVertexStreams.None;
				if (flag42)
				{
					list.Remove(ParticleSystemVertexStream.StableRandomXYZW);
					list.Remove(ParticleSystemVertexStream.VaryingRandomX);
				}
			}
			this.SetActiveVertexStreams(list);
		}

		[Obsolete("Internal_GetVertexStreams is deprecated. Use GetActiveVertexStreams instead.", false)]
		internal ParticleSystemVertexStreams Internal_GetEnabledVertexStreams(ParticleSystemVertexStreams streams)
		{
			List<ParticleSystemVertexStream> list = new List<ParticleSystemVertexStream>(this.activeVertexStreamsCount);
			this.GetActiveVertexStreams(list);
			ParticleSystemVertexStreams particleSystemVertexStreams = ParticleSystemVertexStreams.None;
			bool flag = list.Contains(ParticleSystemVertexStream.Position);
			if (flag)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Position;
			}
			bool flag2 = list.Contains(ParticleSystemVertexStream.Normal);
			if (flag2)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Normal;
			}
			bool flag3 = list.Contains(ParticleSystemVertexStream.Tangent);
			if (flag3)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Tangent;
			}
			bool flag4 = list.Contains(ParticleSystemVertexStream.Color);
			if (flag4)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Color;
			}
			bool flag5 = list.Contains(ParticleSystemVertexStream.UV);
			if (flag5)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.UV;
			}
			bool flag6 = list.Contains(ParticleSystemVertexStream.UV2);
			if (flag6)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.UV2BlendAndFrame;
			}
			bool flag7 = list.Contains(ParticleSystemVertexStream.Center);
			if (flag7)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.CenterAndVertexID;
			}
			bool flag8 = list.Contains(ParticleSystemVertexStream.SizeXYZ);
			if (flag8)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Size;
			}
			bool flag9 = list.Contains(ParticleSystemVertexStream.Rotation3D);
			if (flag9)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Rotation;
			}
			bool flag10 = list.Contains(ParticleSystemVertexStream.Velocity);
			if (flag10)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Velocity;
			}
			bool flag11 = list.Contains(ParticleSystemVertexStream.AgePercent);
			if (flag11)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Lifetime;
			}
			bool flag12 = list.Contains(ParticleSystemVertexStream.Custom1XYZW);
			if (flag12)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Custom1;
			}
			bool flag13 = list.Contains(ParticleSystemVertexStream.Custom2XYZW);
			if (flag13)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Custom2;
			}
			bool flag14 = list.Contains(ParticleSystemVertexStream.StableRandomXYZ);
			if (flag14)
			{
				particleSystemVertexStreams |= ParticleSystemVertexStreams.Random;
			}
			return particleSystemVertexStreams & streams;
		}

		[Obsolete("BakeMesh with useTransform is deprecated. Use BakeMesh with ParticleSystemBakeMeshOptions instead.", false)]
		public void BakeMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeMesh(mesh, Camera.main, useTransform);
		}

		[Obsolete("BakeMesh with useTransform is deprecated. Use BakeMesh with ParticleSystemBakeMeshOptions instead.", false)]
		public void BakeMesh(Mesh mesh, Camera camera, bool useTransform = false)
		{
			this.BakeMesh(mesh, camera, useTransform ? ParticleSystemBakeMeshOptions.BakeRotationAndScale : ParticleSystemBakeMeshOptions.Default);
		}

		[Obsolete("BakeTrailsMesh with useTransform is deprecated. Use BakeTrailsMesh with ParticleSystemBakeMeshOptions instead.", false)]
		public void BakeTrailsMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeTrailsMesh(mesh, Camera.main, useTransform);
		}

		[Obsolete("BakeTrailsMesh with useTransform is deprecated. Use BakeTrailsMesh with ParticleSystemBakeMeshOptions instead.", false)]
		public void BakeTrailsMesh(Mesh mesh, Camera camera, bool useTransform = false)
		{
			this.BakeTrailsMesh(mesh, camera, useTransform ? ParticleSystemBakeMeshOptions.BakeRotationAndScale : ParticleSystemBakeMeshOptions.Default);
		}

		[NativeName("RenderAlignment")]
		public ParticleSystemRenderSpace alignment
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_alignment_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_alignment_Injected(intPtr, value);
			}
		}

		public ParticleSystemRenderMode renderMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_renderMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_renderMode_Injected(intPtr, value);
			}
		}

		public ParticleSystemMeshDistribution meshDistribution
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_meshDistribution_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_meshDistribution_Injected(intPtr, value);
			}
		}

		public ParticleSystemSortMode sortMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_sortMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_sortMode_Injected(intPtr, value);
			}
		}

		public float lengthScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_lengthScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_lengthScale_Injected(intPtr, value);
			}
		}

		public float velocityScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_velocityScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_velocityScale_Injected(intPtr, value);
			}
		}

		public float cameraVelocityScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_cameraVelocityScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_cameraVelocityScale_Injected(intPtr, value);
			}
		}

		public float normalDirection
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_normalDirection_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_normalDirection_Injected(intPtr, value);
			}
		}

		public float shadowBias
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_shadowBias_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_shadowBias_Injected(intPtr, value);
			}
		}

		public float sortingFudge
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_sortingFudge_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_sortingFudge_Injected(intPtr, value);
			}
		}

		public float minParticleSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_minParticleSize_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_minParticleSize_Injected(intPtr, value);
			}
		}

		public float maxParticleSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_maxParticleSize_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_maxParticleSize_Injected(intPtr, value);
			}
		}

		public Vector3 pivot
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ParticleSystemRenderer.get_pivot_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_pivot_Injected(intPtr, ref value);
			}
		}

		public Vector3 flip
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector3 vector;
				ParticleSystemRenderer.get_flip_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_flip_Injected(intPtr, ref value);
			}
		}

		public SpriteMaskInteraction maskInteraction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_maskInteraction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_maskInteraction_Injected(intPtr, value);
			}
		}

		public Material trailMaterial
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Material>(ParticleSystemRenderer.get_trailMaterial_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_trailMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(value));
			}
		}

		internal Material oldTrailMaterial
		{
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_oldTrailMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(value));
			}
		}

		public bool enableGPUInstancing
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_enableGPUInstancing_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_enableGPUInstancing_Injected(intPtr, value);
			}
		}

		public bool allowRoll
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_allowRoll_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_allowRoll_Injected(intPtr, value);
			}
		}

		public bool freeformStretching
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_freeformStretching_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_freeformStretching_Injected(intPtr, value);
			}
		}

		public bool rotateWithStretchDirection
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_rotateWithStretchDirection_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_rotateWithStretchDirection_Injected(intPtr, value);
			}
		}

		public bool applyActiveColorSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_applyActiveColorSpace_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_applyActiveColorSpace_Injected(intPtr, value);
			}
		}

		public Mesh mesh
		{
			[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetMesh", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Mesh>(ParticleSystemRenderer.get_mesh_Injected(intPtr));
			}
			[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetMesh", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ParticleSystemRenderer.set_mesh_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(value));
			}
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetMeshes", HasExplicitThis = true)]
		[RequiredByNativeCode]
		public int GetMeshes([NotNull] [Out] Mesh[] meshes)
		{
			if (meshes == null)
			{
				ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ParticleSystemRenderer.GetMeshes_Injected(intPtr, meshes);
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetMeshes", HasExplicitThis = true)]
		public void SetMeshes([NotNull] Mesh[] meshes, int size)
		{
			if (meshes == null)
			{
				ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ParticleSystemRenderer.SetMeshes_Injected(intPtr, meshes, size);
		}

		public void SetMeshes(Mesh[] meshes)
		{
			this.SetMeshes(meshes, meshes.Length);
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetMeshWeightings", HasExplicitThis = true)]
		public unsafe int GetMeshWeightings([NotNull] [Out] float[] weightings)
		{
			if (weightings == null)
			{
				ThrowHelper.ThrowArgumentNullException(weightings, "weightings");
			}
			int meshWeightings_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (float[] array = weightings)
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					meshWeightings_Injected = ParticleSystemRenderer.GetMeshWeightings_Injected(intPtr, out blittableArrayWrapper);
				}
			}
			finally
			{
				float[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<float>(ref array);
			}
			return meshWeightings_Injected;
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetMeshWeightings", HasExplicitThis = true)]
		public unsafe void SetMeshWeightings([NotNull] float[] weightings, int size)
		{
			if (weightings == null)
			{
				ThrowHelper.ThrowArgumentNullException(weightings, "weightings");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(weightings);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ParticleSystemRenderer.SetMeshWeightings_Injected(intPtr, ref managedSpanWrapper, size);
			}
		}

		public void SetMeshWeightings(float[] weightings)
		{
			this.SetMeshWeightings(weightings, weightings.Length);
		}

		public int meshCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_meshCount_Injected(intPtr);
			}
		}

		public void BakeMesh(Mesh mesh, ParticleSystemBakeMeshOptions options)
		{
			this.BakeMesh(mesh, Camera.main, options);
		}

		public void BakeMesh([NotNull] Mesh mesh, [NotNull] Camera camera, ParticleSystemBakeMeshOptions options)
		{
			if (mesh == null)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(mesh);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			IntPtr intPtr3 = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr3 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ParticleSystemRenderer.BakeMesh_Injected(intPtr, intPtr2, intPtr3, options);
		}

		public void BakeTrailsMesh(Mesh mesh, ParticleSystemBakeMeshOptions options)
		{
			this.BakeTrailsMesh(mesh, Camera.main, options);
		}

		public void BakeTrailsMesh([NotNull] Mesh mesh, [NotNull] Camera camera, ParticleSystemBakeMeshOptions options)
		{
			if (mesh == null)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(mesh);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			IntPtr intPtr3 = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr3 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ParticleSystemRenderer.BakeTrailsMesh_Injected(intPtr, intPtr2, intPtr3, options);
		}

		public int BakeTexture(ref Texture2D verticesTexture, ParticleSystemBakeTextureOptions options)
		{
			return this.BakeTexture(ref verticesTexture, Camera.main, options);
		}

		public int BakeTexture(ref Texture2D verticesTexture, Camera camera, ParticleSystemBakeTextureOptions options)
		{
			bool flag = this.renderMode == ParticleSystemRenderMode.Mesh;
			if (flag)
			{
				throw new InvalidOperationException("Baking mesh particles to texture requires supplying an indices texture");
			}
			int num;
			verticesTexture = this.BakeTextureNoIndicesInternal(verticesTexture, camera, options, out num);
			return num;
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::BakeTextureNoIndices", HasExplicitThis = true)]
		private Texture2D BakeTextureNoIndicesInternal(Texture2D verticesTexture, [NotNull] Camera camera, ParticleSystemBakeTextureOptions options, out int indexCount)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.Marshal<Texture2D>(verticesTexture);
			IntPtr intPtr3 = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr3 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(ParticleSystemRenderer.BakeTextureNoIndicesInternal_Injected(intPtr, intPtr2, intPtr3, options, out indexCount));
		}

		public int BakeTexture(ref Texture2D verticesTexture, ref Texture2D indicesTexture, ParticleSystemBakeTextureOptions options)
		{
			return this.BakeTexture(ref verticesTexture, ref indicesTexture, Camera.main, options);
		}

		public int BakeTexture(ref Texture2D verticesTexture, ref Texture2D indicesTexture, Camera camera, ParticleSystemBakeTextureOptions options)
		{
			int num;
			ParticleSystemRenderer.BakeTextureOutput bakeTextureOutput = this.BakeTextureInternal(verticesTexture, indicesTexture, camera, options, out num);
			verticesTexture = bakeTextureOutput.vertices;
			indicesTexture = bakeTextureOutput.indices;
			return num;
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::BakeTexture", HasExplicitThis = true)]
		private ParticleSystemRenderer.BakeTextureOutput BakeTextureInternal(Texture2D verticesTexture, Texture2D indicesTexture, [NotNull] Camera camera, ParticleSystemBakeTextureOptions options, out int indexCount)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.Marshal<Texture2D>(verticesTexture);
			IntPtr intPtr3 = Object.MarshalledUnityObject.Marshal<Texture2D>(indicesTexture);
			IntPtr intPtr4 = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr4 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ParticleSystemRenderer.BakeTextureOutput bakeTextureOutput;
			ParticleSystemRenderer.BakeTextureInternal_Injected(intPtr, intPtr2, intPtr3, intPtr4, options, out indexCount, out bakeTextureOutput);
			return bakeTextureOutput;
		}

		public int BakeTrailsTexture(ref Texture2D verticesTexture, ref Texture2D indicesTexture, ParticleSystemBakeTextureOptions options)
		{
			return this.BakeTrailsTexture(ref verticesTexture, ref indicesTexture, Camera.main, options);
		}

		public int BakeTrailsTexture(ref Texture2D verticesTexture, ref Texture2D indicesTexture, Camera camera, ParticleSystemBakeTextureOptions options)
		{
			int num;
			ParticleSystemRenderer.BakeTextureOutput bakeTextureOutput = this.BakeTrailsTextureInternal(verticesTexture, indicesTexture, camera, options, out num);
			verticesTexture = bakeTextureOutput.vertices;
			indicesTexture = bakeTextureOutput.indices;
			return num;
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::BakeTrailsTexture", HasExplicitThis = true)]
		private ParticleSystemRenderer.BakeTextureOutput BakeTrailsTextureInternal(Texture2D verticesTexture, Texture2D indicesTexture, [NotNull] Camera camera, ParticleSystemBakeTextureOptions options, out int indexCount)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.Marshal<Texture2D>(verticesTexture);
			IntPtr intPtr3 = Object.MarshalledUnityObject.Marshal<Texture2D>(indicesTexture);
			IntPtr intPtr4 = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr4 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			ParticleSystemRenderer.BakeTextureOutput bakeTextureOutput;
			ParticleSystemRenderer.BakeTrailsTextureInternal_Injected(intPtr, intPtr2, intPtr3, intPtr4, options, out indexCount, out bakeTextureOutput);
			return bakeTextureOutput;
		}

		public int activeVertexStreamsCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_activeVertexStreamsCount_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetActiveVertexStreams", HasExplicitThis = true)]
		public unsafe void SetActiveVertexStreams([NotNull] List<ParticleSystemVertexStream> streams)
		{
			if (streams == null)
			{
				ThrowHelper.ThrowArgumentNullException(streams, "streams");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (ParticleSystemVertexStream[] array = NoAllocHelpers.ExtractArrayFromList<ParticleSystemVertexStream>(streams))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, streams.Count);
					ParticleSystemRenderer.SetActiveVertexStreams_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ParticleSystemVertexStream>(streams);
			}
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetActiveVertexStreams", HasExplicitThis = true)]
		public unsafe void GetActiveVertexStreams([NotNull] List<ParticleSystemVertexStream> streams)
		{
			if (streams == null)
			{
				ThrowHelper.ThrowArgumentNullException(streams, "streams");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (ParticleSystemVertexStream[] array = NoAllocHelpers.ExtractArrayFromList<ParticleSystemVertexStream>(streams))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, streams.Count);
					ParticleSystemRenderer.GetActiveVertexStreams_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ParticleSystemVertexStream>(streams);
			}
		}

		public int activeTrailVertexStreamsCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ParticleSystemRenderer.get_activeTrailVertexStreamsCount_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetActiveTrailVertexStreams", HasExplicitThis = true)]
		public unsafe void SetActiveTrailVertexStreams([NotNull] List<ParticleSystemVertexStream> streams)
		{
			if (streams == null)
			{
				ThrowHelper.ThrowArgumentNullException(streams, "streams");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (ParticleSystemVertexStream[] array = NoAllocHelpers.ExtractArrayFromList<ParticleSystemVertexStream>(streams))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, streams.Count);
					ParticleSystemRenderer.SetActiveTrailVertexStreams_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ParticleSystemVertexStream>(streams);
			}
		}

		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetActiveTrailVertexStreams", HasExplicitThis = true)]
		public unsafe void GetActiveTrailVertexStreams([NotNull] List<ParticleSystemVertexStream> streams)
		{
			if (streams == null)
			{
				ThrowHelper.ThrowArgumentNullException(streams, "streams");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<ParticleSystemRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (ParticleSystemVertexStream[] array = NoAllocHelpers.ExtractArrayFromList<ParticleSystemVertexStream>(streams))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, streams.Count);
					ParticleSystemRenderer.GetActiveTrailVertexStreams_Injected(intPtr, ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<ParticleSystemVertexStream>(streams);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ParticleSystemRenderSpace get_alignment_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_alignment_Injected(IntPtr _unity_self, ParticleSystemRenderSpace value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ParticleSystemRenderMode get_renderMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderMode_Injected(IntPtr _unity_self, ParticleSystemRenderMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ParticleSystemMeshDistribution get_meshDistribution_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_meshDistribution_Injected(IntPtr _unity_self, ParticleSystemMeshDistribution value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ParticleSystemSortMode get_sortMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortMode_Injected(IntPtr _unity_self, ParticleSystemSortMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_lengthScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lengthScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_velocityScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_velocityScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_cameraVelocityScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cameraVelocityScale_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_normalDirection_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_normalDirection_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_shadowBias_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowBias_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_sortingFudge_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingFudge_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_minParticleSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_minParticleSize_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_maxParticleSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maxParticleSize_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_pivot_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pivot_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_flip_Injected(IntPtr _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_flip_Injected(IntPtr _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteMaskInteraction get_maskInteraction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_trailMaterial_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_trailMaterial_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_oldTrailMaterial_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableGPUInstancing_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableGPUInstancing_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_allowRoll_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_allowRoll_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_freeformStretching_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_freeformStretching_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_rotateWithStretchDirection_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotateWithStretchDirection_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_applyActiveColorSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_applyActiveColorSpace_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_mesh_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mesh_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMeshes_Injected(IntPtr _unity_self, [Out] Mesh[] meshes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMeshes_Injected(IntPtr _unity_self, Mesh[] meshes, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetMeshWeightings_Injected(IntPtr _unity_self, out BlittableArrayWrapper weightings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMeshWeightings_Injected(IntPtr _unity_self, ref ManagedSpanWrapper weightings, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_meshCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BakeMesh_Injected(IntPtr _unity_self, IntPtr mesh, IntPtr camera, ParticleSystemBakeMeshOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BakeTrailsMesh_Injected(IntPtr _unity_self, IntPtr mesh, IntPtr camera, ParticleSystemBakeMeshOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr BakeTextureNoIndicesInternal_Injected(IntPtr _unity_self, IntPtr verticesTexture, IntPtr camera, ParticleSystemBakeTextureOptions options, out int indexCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BakeTextureInternal_Injected(IntPtr _unity_self, IntPtr verticesTexture, IntPtr indicesTexture, IntPtr camera, ParticleSystemBakeTextureOptions options, out int indexCount, out ParticleSystemRenderer.BakeTextureOutput ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BakeTrailsTextureInternal_Injected(IntPtr _unity_self, IntPtr verticesTexture, IntPtr indicesTexture, IntPtr camera, ParticleSystemBakeTextureOptions options, out int indexCount, out ParticleSystemRenderer.BakeTextureOutput ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_activeVertexStreamsCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetActiveVertexStreams_Injected(IntPtr _unity_self, ref BlittableListWrapper streams);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveVertexStreams_Injected(IntPtr _unity_self, ref BlittableListWrapper streams);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_activeTrailVertexStreamsCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetActiveTrailVertexStreams_Injected(IntPtr _unity_self, ref BlittableListWrapper streams);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveTrailVertexStreams_Injected(IntPtr _unity_self, ref BlittableListWrapper streams);

		internal struct BakeTextureOutput
		{
			[NativeName("first")]
			internal Texture2D vertices;

			[NativeName("second")]
			internal Texture2D indices;
		}
	}
}
