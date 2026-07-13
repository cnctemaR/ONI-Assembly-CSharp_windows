using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.U2D
{
	[NativeHeader("Runtime/Graphics/Mesh/SpriteRenderer.h")]
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
	public static class SpriteRendererDataAccessExtensions
	{
		internal static void SetDeformableBuffer(this SpriteRenderer spriteRenderer, NativeArray<byte> src)
		{
			bool flag = spriteRenderer.sprite == null;
			if (flag)
			{
				throw new ArgumentException(string.Format("spriteRenderer does not have a valid sprite set.", Array.Empty<object>()));
			}
			bool flag2 = src.Length != SpriteDataAccessExtensions.GetPrimaryVertexStreamSize(spriteRenderer.sprite);
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("custom sprite vertex data size must match sprite asset's vertex data size {0} {1}", src.Length, SpriteDataAccessExtensions.GetPrimaryVertexStreamSize(spriteRenderer.sprite)));
			}
			SpriteRendererDataAccessExtensions.SetDeformableBuffer(spriteRenderer, src.GetUnsafeReadOnlyPtr<byte>(), src.Length);
		}

		internal static void SetDeformableBuffer(this SpriteRenderer spriteRenderer, NativeArray<Vector3> src)
		{
			bool flag = spriteRenderer.sprite == null;
			if (flag)
			{
				throw new InvalidOperationException("spriteRenderer does not have a valid sprite set.");
			}
			bool flag2 = src.Length != spriteRenderer.sprite.GetVertexCount();
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("The src length {0} must match the vertex count of source Sprite {1}.", src.Length, spriteRenderer.sprite.GetVertexCount()));
			}
			SpriteRendererDataAccessExtensions.SetDeformableBuffer(spriteRenderer, src.GetUnsafeReadOnlyPtr<Vector3>(), src.Length);
		}

		internal static void SetBatchDeformableBufferAndLocalAABBArray(SpriteRenderer[] spriteRenderers, NativeArray<IntPtr> buffers, NativeArray<int> bufferSizes, NativeArray<Bounds> bounds)
		{
			int num = spriteRenderers.Length;
			bool flag = num != buffers.Length || num != bufferSizes.Length || num != bounds.Length;
			if (flag)
			{
				throw new ArgumentException("Input array sizes are not the same.");
			}
			SpriteRendererDataAccessExtensions.SetBatchDeformableBufferAndLocalAABBArray(spriteRenderers, buffers.GetUnsafeReadOnlyPtr<IntPtr>(), bufferSizes.GetUnsafeReadOnlyPtr<int>(), bounds.GetUnsafeReadOnlyPtr<Bounds>(), num);
		}

		internal static void SetBoneTransformsArray(SpriteRenderer[] spriteRenderers, NativeArray<IntPtr> buffers, NativeArray<int> bufferSizes, NativeArray<Bounds> bounds)
		{
			int num = spriteRenderers.Length;
			bool flag = num != buffers.Length || num != bufferSizes.Length || num != bounds.Length;
			if (flag)
			{
				throw new ArgumentException("Input array sizes are not the same.");
			}
			SpriteRendererDataAccessExtensions.SetBoneTransformsArray(spriteRenderers, buffers.GetUnsafeReadOnlyPtr<IntPtr>(), bufferSizes.GetUnsafeReadOnlyPtr<int>(), bounds.GetUnsafeReadOnlyPtr<Bounds>(), num);
		}

		internal unsafe static bool IsUsingDeformableBuffer(this SpriteRenderer spriteRenderer, IntPtr buffer)
		{
			return SpriteRendererDataAccessExtensions.IsUsingDeformableBuffer(spriteRenderer, (void*)buffer);
		}

		internal static void SetBoneTransforms(this SpriteRenderer spriteRenderer, NativeArray<Matrix4x4> src)
		{
			SpriteRendererDataAccessExtensions.SetBoneTransforms(spriteRenderer, src.GetUnsafeReadOnlyPtr<Matrix4x4>(), src.Length);
		}

		public static void DeactivateDeformableBuffer([NotNull] this SpriteRenderer renderer)
		{
			if (renderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(renderer);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			SpriteRendererDataAccessExtensions.DeactivateDeformableBuffer_Injected(intPtr);
		}

		internal static void SetLocalAABB([NotNull] this SpriteRenderer renderer, Bounds aabb)
		{
			if (renderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(renderer);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			SpriteRendererDataAccessExtensions.SetLocalAABB_Injected(intPtr, ref aabb);
		}

		private unsafe static void SetDeformableBuffer([NotNull] SpriteRenderer spriteRenderer, void* src, int count)
		{
			if (spriteRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(spriteRenderer);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			SpriteRendererDataAccessExtensions.SetDeformableBuffer_Injected(intPtr, src, count);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetBatchDeformableBufferAndLocalAABBArray(SpriteRenderer[] spriteRenderers, void* buffers, void* bufferSizes, void* bounds, int count);

		private unsafe static bool IsUsingDeformableBuffer([NotNull] SpriteRenderer spriteRenderer, void* buffer)
		{
			if (spriteRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(spriteRenderer);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			return SpriteRendererDataAccessExtensions.IsUsingDeformableBuffer_Injected(intPtr, buffer);
		}

		private unsafe static void SetBoneTransforms([NotNull] SpriteRenderer spriteRenderer, void* src, int count)
		{
			if (spriteRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(spriteRenderer);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			SpriteRendererDataAccessExtensions.SetBoneTransforms_Injected(intPtr, src, count);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetBoneTransformsArray(SpriteRenderer[] spriteRenderers, void* buffers, void* bufferSizes, void* bounds, int count);

		internal static void SetupMaterialProperties([NotNull] SpriteRenderer spriteRenderer)
		{
			if (spriteRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(spriteRenderer);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			SpriteRendererDataAccessExtensions.SetupMaterialProperties_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsGPUSkinningEnabled();

		internal static bool IsSRPBatchingEnabled([NotNull] this SpriteRenderer spriteRenderer)
		{
			if (spriteRenderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteRenderer>(spriteRenderer);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(spriteRenderer, "spriteRenderer");
			}
			return SpriteRendererDataAccessExtensions.IsSRPBatchingEnabled_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DeactivateDeformableBuffer_Injected(IntPtr renderer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalAABB_Injected(IntPtr renderer, [In] ref Bounds aabb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetDeformableBuffer_Injected(IntPtr spriteRenderer, void* src, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool IsUsingDeformableBuffer_Injected(IntPtr spriteRenderer, void* buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetBoneTransforms_Injected(IntPtr spriteRenderer, void* src, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetupMaterialProperties_Injected(IntPtr spriteRenderer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsSRPBatchingEnabled_Injected(IntPtr spriteRenderer);
	}
}
