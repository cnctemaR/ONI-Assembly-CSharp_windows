using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.U2D
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
				flag = typeof(T) == typeof(Vector2);
				break;
			default:
				throw new InvalidOperationException(string.Format("The requested channel '{0}' is unknown.", channel));
			}
			if (!flag)
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
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(bindPoseInfo.buffer, bindPoseInfo.count, Allocator.Invalid);
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

		public static NativeArray<BoneWeight> GetBoneWeights(this Sprite sprite)
		{
			SpriteChannelInfo boneWeightsInfo = SpriteDataAccessExtensions.GetBoneWeightsInfo(sprite);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BoneWeight>(boneWeightsInfo.buffer, boneWeightsInfo.count, Allocator.Invalid);
		}

		public static void SetBoneWeights(this Sprite sprite, NativeArray<BoneWeight> src)
		{
			SpriteDataAccessExtensions.SetBoneWeightsData(sprite, src.GetUnsafeReadOnlyPtr<BoneWeight>(), src.Length);
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasVertexAttribute(this Sprite sprite, VertexAttribute channel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetVertexCount(this Sprite sprite, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetVertexCount(this Sprite sprite);

		private static SpriteChannelInfo GetBindPoseInfo(Sprite sprite)
		{
			SpriteChannelInfo spriteChannelInfo;
			SpriteDataAccessExtensions.GetBindPoseInfo_Injected(sprite, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetBindPoseData(Sprite sprite, void* src, int count);

		private static SpriteChannelInfo GetIndicesInfo(Sprite sprite)
		{
			SpriteChannelInfo spriteChannelInfo;
			SpriteDataAccessExtensions.GetIndicesInfo_Injected(sprite, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetIndicesData(Sprite sprite, void* src, int count);

		private static SpriteChannelInfo GetChannelInfo(Sprite sprite, VertexAttribute channel)
		{
			SpriteChannelInfo spriteChannelInfo;
			SpriteDataAccessExtensions.GetChannelInfo_Injected(sprite, channel, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetChannelData(Sprite sprite, VertexAttribute channel, void* src);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteBone[] GetBoneInfo(Sprite sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBoneData(Sprite sprite, SpriteBone[] src);

		private static SpriteChannelInfo GetBoneWeightsInfo(Sprite sprite)
		{
			SpriteChannelInfo spriteChannelInfo;
			SpriteDataAccessExtensions.GetBoneWeightsInfo_Injected(sprite, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetBoneWeightsData(Sprite sprite, void* src, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBindPoseInfo_Injected(Sprite sprite, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetIndicesInfo_Injected(Sprite sprite, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetChannelInfo_Injected(Sprite sprite, VertexAttribute channel, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBoneWeightsInfo_Injected(Sprite sprite, out SpriteChannelInfo ret);
	}
}
