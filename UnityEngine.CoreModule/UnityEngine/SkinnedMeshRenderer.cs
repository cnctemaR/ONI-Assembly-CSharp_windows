using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>The Skinned Mesh filter.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/Mesh/SkinnedMeshRenderer.h")]
	public class SkinnedMeshRenderer : Renderer
	{
		/// <summary>
		///   <para>The maximum number of bones affecting a single vertex.</para>
		/// </summary>
		public extern SkinQuality quality
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>If enabled, the Skinned Mesh will be updated when offscreen. If disabled, this also disables updating animations.</para>
		/// </summary>
		public extern bool updateWhenOffscreen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Transform rootBone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The bones used to skin the mesh.</para>
		/// </summary>
		public extern Transform[] bones
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The mesh used for skinning.</para>
		/// </summary>
		[NativeProperty("Mesh")]
		public extern Mesh sharedMesh
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Specifies whether skinned motion vectors should be used for this renderer.</para>
		/// </summary>
		[NativeProperty("SkinnedMeshMotionVectors")]
		public extern bool skinnedMotionVectors
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns weight of BlendShape on this renderer.</para>
		/// </summary>
		/// <param name="index"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetBlendShapeWeight(int index);

		/// <summary>
		///   <para>Sets the weight in percent of a BlendShape on this Renderer.</para>
		/// </summary>
		/// <param name="index">The index of the BlendShape to modify.</param>
		/// <param name="value">The weight in percent for this BlendShape.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBlendShapeWeight(int index, float value);

		/// <summary>
		///   <para>Creates a snapshot of SkinnedMeshRenderer and stores it in mesh.</para>
		/// </summary>
		/// <param name="mesh">A static mesh that will receive the snapshot of the skinned mesh.</param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void BakeMesh(Mesh mesh);

		[FreeFunction(Name = "SkinnedMeshRendererScripting::GetLocalAABB", HasExplicitThis = true)]
		private Bounds GetLocalAABB()
		{
			Bounds bounds;
			this.GetLocalAABB_Injected(out bounds);
			return bounds;
		}

		private void SetLocalAABB(Bounds b)
		{
			this.SetLocalAABB_Injected(ref b);
		}

		/// <summary>
		///   <para>AABB of this Skinned Mesh in its local space.</para>
		/// </summary>
		public Bounds localBounds
		{
			get
			{
				return this.GetLocalAABB();
			}
			set
			{
				this.SetLocalAABB(value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetLocalAABB_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetLocalAABB_Injected(ref Bounds b);
	}
}
