using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Experimental.U2D
{
	/// <summary>
	///   <para>A list of methods that allow the caller to override what the SpriteRenderer renders.</para>
	/// </summary>
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
	[NativeHeader("Runtime/Graphics/Mesh/SpriteRenderer.h")]
	public static class SpriteRendererDataAccessExtensions
	{
		/// <summary>
		///   <para>Returns an array of vertices to be deformed by the caller.</para>
		/// </summary>
		/// <param name="spriteRenderer"></param>
		public static NativeArray<Vector3> GetDeformableVertices(this SpriteRenderer spriteRenderer)
		{
			SpriteChannelInfo deformableChannelInfo = spriteRenderer.GetDeformableChannelInfo(VertexAttribute.Position);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(deformableChannelInfo.buffer, deformableChannelInfo.count, Allocator.Invalid);
		}

		/// <summary>
		///   <para>Stop using the deformable buffer to render the Sprite and use the original mesh instead.</para>
		/// </summary>
		/// <param name="renderer"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DeactivateDeformableBuffer(this SpriteRenderer renderer);

		/// <summary>
		///   <para>Provides the JobHandle that updates the deform buffer to the SpriteRenderer.</para>
		/// </summary>
		/// <param name="spriteRenderer"></param>
		/// <param name="fence"></param>
		public static void UpdateDeformableBuffer(this SpriteRenderer spriteRenderer, JobHandle fence)
		{
			SpriteRendererDataAccessExtensions.UpdateDeformableBuffer_Injected(spriteRenderer, ref fence);
		}

		private static SpriteChannelInfo GetDeformableChannelInfo(this SpriteRenderer sprite, VertexAttribute channel)
		{
			SpriteChannelInfo spriteChannelInfo;
			SpriteRendererDataAccessExtensions.GetDeformableChannelInfo_Injected(sprite, channel, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		internal static void SetLocalAABB(this SpriteRenderer renderer, Bounds aabb)
		{
			SpriteRendererDataAccessExtensions.SetLocalAABB_Injected(renderer, ref aabb);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateDeformableBuffer_Injected(SpriteRenderer spriteRenderer, ref JobHandle fence);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDeformableChannelInfo_Injected(SpriteRenderer sprite, VertexAttribute channel, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalAABB_Injected(SpriteRenderer renderer, ref Bounds aabb);
	}
}
