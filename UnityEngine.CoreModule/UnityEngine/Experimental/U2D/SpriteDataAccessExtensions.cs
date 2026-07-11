using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Experimental.U2D
{
	/// <summary>
	///   <para>A list of methods designed for reading and writing to the rich internal data of a Sprite.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
	[NativeHeader("Runtime/2D/Common/SpriteDataAccess.h")]
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

		/// <summary>
		///   <para>Returns an array of BindPoses.</para>
		/// </summary>
		/// <param name="sprite">The sprite to retrieve the bind pose from.</param>
		/// <returns>
		///   <para>A list of bind poses for this sprite. There is no need to dispose the returned NativeArray.</para>
		/// </returns>
		public static NativeArray<Matrix4x4> GetBindPoses(this Sprite sprite)
		{
			SpriteChannelInfo bindPoseInfo = SpriteDataAccessExtensions.GetBindPoseInfo(sprite);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(bindPoseInfo.buffer, bindPoseInfo.count, Allocator.Invalid);
		}

		public static void SetBindPoses(this Sprite sprite, NativeArray<Matrix4x4> src)
		{
			SpriteDataAccessExtensions.SetBindPoseData(sprite, src.GetUnsafeReadOnlyPtr<Matrix4x4>(), src.Length);
		}

		/// <summary>
		///   <para>Returns a list of indices. This is the same as Sprite.triangle.</para>
		/// </summary>
		/// <param name="sprite"></param>
		/// <returns>
		///   <para>A read-only list of indices indicating how the triangles are formed between the vertices. The array is marked as undisposable.</para>
		/// </returns>
		public static NativeArray<ushort> GetIndices(this Sprite sprite)
		{
			SpriteChannelInfo indicesInfo = SpriteDataAccessExtensions.GetIndicesInfo(sprite);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<ushort>(indicesInfo.buffer, indicesInfo.count, Allocator.Invalid);
		}

		public static void SetIndices(this Sprite sprite, NativeArray<ushort> src)
		{
			SpriteDataAccessExtensions.SetIndicesData(sprite, src.GetUnsafeReadOnlyPtr<ushort>(), src.Length);
		}

		/// <summary>
		///   <para>Returns a list of BoneWeight that corresponds to each and every vertice in this Sprite.</para>
		/// </summary>
		/// <param name="sprite">The Sprite to get the BoneWeights from.</param>
		/// <returns>
		///   <para>The list of BoneWeight. The length should equal the number of vertices. There is no need to call dispose on this NativeArray.</para>
		/// </returns>
		public static NativeArray<BoneWeight> GetBoneWeights(this Sprite sprite)
		{
			SpriteChannelInfo boneWeightsInfo = SpriteDataAccessExtensions.GetBoneWeightsInfo(sprite);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BoneWeight>(boneWeightsInfo.buffer, boneWeightsInfo.count, Allocator.Invalid);
		}

		public static void SetBoneWeights(this Sprite sprite, NativeArray<BoneWeight> src)
		{
			SpriteDataAccessExtensions.SetBoneWeightsData(sprite, src.GetUnsafeReadOnlyPtr<BoneWeight>(), src.Length);
		}

		/// <summary>
		///   <para>Returns a list of SpriteBone in this Sprite.</para>
		/// </summary>
		/// <param name="sprite">The sprite to get the list of SpriteBone from.</param>
		/// <returns>
		///   <para>An array of SpriteBone that belongs to this Sprite.</para>
		/// </returns>
		public static SpriteBone[] GetBones(this Sprite sprite)
		{
			return SpriteDataAccessExtensions.GetBoneInfo(sprite);
		}

		/// <summary>
		///   <para>Sets the SpriteBones for this Sprite.</para>
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="src"></param>
		public static void SetBones(this Sprite sprite, SpriteBone[] src)
		{
			SpriteDataAccessExtensions.SetBoneData(sprite, src);
		}

		/// <summary>
		///   <para>Checks if a specific channel exists for this Sprite.</para>
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="channel"></param>
		/// <returns>
		///   <para>True if the channel exists.</para>
		/// </returns>
		[NativeName("HasChannel")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasVertexAttribute(this Sprite sprite, VertexAttribute channel);

		/// <summary>
		///   <para>Sets the vertex count. This resizes the internal buffer. It also preserves any configurations of VertexAttributes.</para>
		/// </summary>
		/// <param name="sprite"></param>
		/// <param name="count"></param>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetVertexCount(this Sprite sprite, int count);

		/// <summary>
		///   <para>Returns the number of vertices in this Sprite.</para>
		/// </summary>
		/// <param name="sprite"></param>
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
