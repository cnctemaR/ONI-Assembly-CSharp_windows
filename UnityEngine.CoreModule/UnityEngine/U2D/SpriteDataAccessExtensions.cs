using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine.U2D
{
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
	[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
	public static class SpriteDataAccessExtensions
	{
		private static void CheckAttributeTypeMatchesAndThrow<T>(VertexAttribute channel)
		{
			bool flag;
			switch (channel)
			{
			case VertexAttribute.Position:
			case VertexAttribute.Normal:
				flag = typeof(T) == typeof(Vector3);
				break;
			case VertexAttribute.Tangent:
				flag = typeof(T) == typeof(Vector4);
				break;
			case VertexAttribute.Color:
				flag = typeof(T) == typeof(Color32);
				break;
			case VertexAttribute.TexCoord0:
			case VertexAttribute.TexCoord1:
			case VertexAttribute.TexCoord2:
			case VertexAttribute.TexCoord3:
			case VertexAttribute.TexCoord4:
			case VertexAttribute.TexCoord5:
			case VertexAttribute.TexCoord6:
			case VertexAttribute.TexCoord7:
				flag = typeof(T) == typeof(Vector2);
				break;
			case VertexAttribute.BlendWeight:
				flag = typeof(T) == typeof(BoneWeight);
				break;
			default:
				throw new InvalidOperationException(string.Format("The requested channel '{0}' is unknown.", channel));
			}
			bool flag2 = !flag;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("The requested channel '{0}' does not match the return type {1}.", channel, typeof(T).Name));
			}
		}

		public unsafe static NativeSlice<T> GetVertexAttribute<T>(this Sprite sprite, VertexAttribute channel) where T : struct
		{
			SpriteDataAccessExtensions.CheckAttributeTypeMatchesAndThrow<T>(channel);
			SpriteChannelInfo channelInfo = SpriteDataAccessExtensions.GetChannelInfo(sprite, channel);
			byte* ptr = (byte*)channelInfo.buffer + channelInfo.offset;
			return NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<T>((void*)ptr, channelInfo.stride, channelInfo.count);
		}

		public static void SetVertexAttribute<T>(this Sprite sprite, VertexAttribute channel, NativeArray<T> src) where T : struct
		{
			SpriteDataAccessExtensions.CheckAttributeTypeMatchesAndThrow<T>(channel);
			SpriteDataAccessExtensions.SetChannelData(sprite, channel, src.GetUnsafeReadOnlyPtr<T>());
		}

		public static NativeArray<Matrix4x4> GetBindPoses(this Sprite sprite)
		{
			SpriteChannelInfo bindPoseInfo = SpriteDataAccessExtensions.GetBindPoseInfo(sprite);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(bindPoseInfo.buffer, bindPoseInfo.count, Allocator.None);
		}

		public static void SetBindPoses(this Sprite sprite, NativeArray<Matrix4x4> src)
		{
			SpriteDataAccessExtensions.SetBindPoseData(sprite, src.GetUnsafeReadOnlyPtr<Matrix4x4>(), src.Length);
		}

		public static NativeArray<ushort> GetIndices(this Sprite sprite)
		{
			SpriteChannelInfo indicesInfo = SpriteDataAccessExtensions.GetIndicesInfo(sprite);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ushort>(indicesInfo.buffer, indicesInfo.count, Allocator.Invalid);
		}

		public static void SetIndices(this Sprite sprite, NativeArray<ushort> src)
		{
			SpriteDataAccessExtensions.SetIndicesData(sprite, src.GetUnsafeReadOnlyPtr<ushort>(), src.Length);
		}

		public static SpriteBone[] GetBones(this Sprite sprite)
		{
			return SpriteDataAccessExtensions.GetBoneInfo(sprite);
		}

		public static void SetBones(this Sprite sprite, SpriteBone[] src)
		{
			SpriteDataAccessExtensions.SetBoneData(sprite, src);
		}

		[NativeName("HasChannel")]
		public static bool HasVertexAttribute([NotNull] this Sprite sprite, VertexAttribute channel)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			return SpriteDataAccessExtensions.HasVertexAttribute_Injected(intPtr, channel);
		}

		public static void SetVertexCount([NotNull] this Sprite sprite, int count)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			SpriteDataAccessExtensions.SetVertexCount_Injected(intPtr, count);
		}

		public static int GetVertexCount([NotNull] this Sprite sprite)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			return SpriteDataAccessExtensions.GetVertexCount_Injected(intPtr);
		}

		private static SpriteChannelInfo GetBindPoseInfo([NotNull] Sprite sprite)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			SpriteChannelInfo spriteChannelInfo;
			SpriteDataAccessExtensions.GetBindPoseInfo_Injected(intPtr, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		private unsafe static void SetBindPoseData([NotNull] Sprite sprite, void* src, int count)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			SpriteDataAccessExtensions.SetBindPoseData_Injected(intPtr, src, count);
		}

		private static SpriteChannelInfo GetIndicesInfo([NotNull] Sprite sprite)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			SpriteChannelInfo spriteChannelInfo;
			SpriteDataAccessExtensions.GetIndicesInfo_Injected(intPtr, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		private unsafe static void SetIndicesData([NotNull] Sprite sprite, void* src, int count)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			SpriteDataAccessExtensions.SetIndicesData_Injected(intPtr, src, count);
		}

		private static SpriteChannelInfo GetChannelInfo([NotNull] Sprite sprite, VertexAttribute channel)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			SpriteChannelInfo spriteChannelInfo;
			SpriteDataAccessExtensions.GetChannelInfo_Injected(intPtr, channel, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		private unsafe static void SetChannelData([NotNull] Sprite sprite, VertexAttribute channel, void* src)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			SpriteDataAccessExtensions.SetChannelData_Injected(intPtr, channel, src);
		}

		private static SpriteBone[] GetBoneInfo([NotNull] Sprite sprite)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			return SpriteDataAccessExtensions.GetBoneInfo_Injected(intPtr);
		}

		private static void SetBoneData([NotNull] Sprite sprite, SpriteBone[] src)
		{
			if (sprite == null)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Sprite>(sprite);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sprite, "sprite");
			}
			SpriteDataAccessExtensions.SetBoneData_Injected(intPtr, src);
		}

		internal static int GetPrimaryVertexStreamSize(Sprite sprite)
		{
			return SpriteDataAccessExtensions.GetPrimaryVertexStreamSize_Injected(Object.MarshalledUnityObject.Marshal<Sprite>(sprite));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasVertexAttribute_Injected(IntPtr sprite, VertexAttribute channel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVertexCount_Injected(IntPtr sprite, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVertexCount_Injected(IntPtr sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBindPoseInfo_Injected(IntPtr sprite, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetBindPoseData_Injected(IntPtr sprite, void* src, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetIndicesInfo_Injected(IntPtr sprite, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetIndicesData_Injected(IntPtr sprite, void* src, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetChannelInfo_Injected(IntPtr sprite, VertexAttribute channel, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetChannelData_Injected(IntPtr sprite, VertexAttribute channel, void* src);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteBone[] GetBoneInfo_Injected(IntPtr sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoneData_Injected(IntPtr sprite, SpriteBone[] src);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPrimaryVertexStreamSize_Injected(IntPtr sprite);
	}
}
