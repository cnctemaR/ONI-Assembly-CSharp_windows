using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D
{
	[MovedFrom("UnityEngine.Experimental.U2D")]
	[NativeType(Header = "Modules/SpriteShape/Public/SpriteShapeRenderer.h")]
	public class SpriteShapeRenderer : Renderer
	{
		public Color color
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				SpriteShapeRenderer.get_color_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteShapeRenderer.set_color_Injected(intPtr, ref value);
			}
		}

		public SpriteMaskInteraction maskInteraction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return SpriteShapeRenderer.get_maskInteraction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				SpriteShapeRenderer.set_maskInteraction_Injected(intPtr, value);
			}
		}

		public void Prepare(JobHandle handle, SpriteShapeParameters shapeParams, Sprite[] sprites)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SpriteShapeRenderer.Prepare_Injected(intPtr, ref handle, ref shapeParams, sprites);
		}

		private NativeArray<T> GetNativeDataArray<T>(SpriteShapeDataType dataType) where T : struct
		{
			SpriteChannelInfo dataInfo = this.GetDataInfo(dataType);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(dataInfo.buffer, dataInfo.count, Allocator.Invalid);
		}

		private unsafe NativeSlice<T> GetChannelDataArray<T>(SpriteShapeDataType dataType, VertexAttribute channel) where T : struct
		{
			SpriteChannelInfo channelInfo = this.GetChannelInfo(channel);
			byte* ptr = (byte*)channelInfo.buffer + channelInfo.offset;
			return NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<T>((void*)ptr, channelInfo.stride, channelInfo.count);
		}

		private void SetSegmentCount(int geomCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SpriteShapeRenderer.SetSegmentCount_Injected(intPtr, geomCount);
		}

		private void SetMeshDataCount(int vertexCount, int indexCount)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SpriteShapeRenderer.SetMeshDataCount_Injected(intPtr, vertexCount, indexCount);
		}

		private void SetMeshChannelInfo(int vertexCount, int indexCount, int hotChannelMask)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SpriteShapeRenderer.SetMeshChannelInfo_Injected(intPtr, vertexCount, indexCount, hotChannelMask);
		}

		private SpriteChannelInfo GetDataInfo(SpriteShapeDataType arrayType)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SpriteChannelInfo spriteChannelInfo;
			SpriteShapeRenderer.GetDataInfo_Injected(intPtr, arrayType, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		private SpriteChannelInfo GetChannelInfo(VertexAttribute channel)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SpriteChannelInfo spriteChannelInfo;
			SpriteShapeRenderer.GetChannelInfo_Injected(intPtr, channel, out spriteChannelInfo);
			return spriteChannelInfo;
		}

		public void SetLocalAABB(Bounds bounds)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			SpriteShapeRenderer.SetLocalAABB_Injected(intPtr, ref bounds);
		}

		public int GetSplineMeshCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<SpriteShapeRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return SpriteShapeRenderer.GetSplineMeshCount_Injected(intPtr);
		}

		public NativeArray<Bounds> GetBounds()
		{
			return this.GetNativeDataArray<Bounds>(SpriteShapeDataType.BoundingBox);
		}

		public NativeArray<SpriteShapeSegment> GetSegments(int dataSize)
		{
			this.SetSegmentCount(dataSize);
			return this.GetNativeDataArray<SpriteShapeSegment>(SpriteShapeDataType.Segment);
		}

		public void GetChannels(int dataSize, out NativeArray<ushort> indices, out NativeSlice<Vector3> vertices, out NativeSlice<Vector2> texcoords)
		{
			this.SetMeshDataCount(dataSize, dataSize);
			indices = this.GetNativeDataArray<ushort>(SpriteShapeDataType.Index);
			vertices = this.GetChannelDataArray<Vector3>(SpriteShapeDataType.ChannelVertex, VertexAttribute.Position);
			texcoords = this.GetChannelDataArray<Vector2>(SpriteShapeDataType.ChannelTexCoord0, VertexAttribute.TexCoord0);
		}

		public void GetChannels(int dataSize, out NativeArray<ushort> indices, out NativeSlice<Vector3> vertices, out NativeSlice<Vector2> texcoords, out NativeSlice<Color32> colors)
		{
			this.SetMeshChannelInfo(dataSize, dataSize, 8);
			indices = this.GetNativeDataArray<ushort>(SpriteShapeDataType.Index);
			vertices = this.GetChannelDataArray<Vector3>(SpriteShapeDataType.ChannelVertex, VertexAttribute.Position);
			texcoords = this.GetChannelDataArray<Vector2>(SpriteShapeDataType.ChannelTexCoord0, VertexAttribute.TexCoord0);
			colors = this.GetChannelDataArray<Color32>(SpriteShapeDataType.ChannelColor, VertexAttribute.Color);
		}

		public void GetChannels(int dataSize, out NativeArray<ushort> indices, out NativeSlice<Vector3> vertices, out NativeSlice<Vector2> texcoords, out NativeSlice<Vector4> tangents)
		{
			this.SetMeshChannelInfo(dataSize, dataSize, 4);
			indices = this.GetNativeDataArray<ushort>(SpriteShapeDataType.Index);
			vertices = this.GetChannelDataArray<Vector3>(SpriteShapeDataType.ChannelVertex, VertexAttribute.Position);
			texcoords = this.GetChannelDataArray<Vector2>(SpriteShapeDataType.ChannelTexCoord0, VertexAttribute.TexCoord0);
			tangents = this.GetChannelDataArray<Vector4>(SpriteShapeDataType.ChannelTangent, VertexAttribute.Tangent);
		}

		public void GetChannels(int dataSize, out NativeArray<ushort> indices, out NativeSlice<Vector3> vertices, out NativeSlice<Vector2> texcoords, out NativeSlice<Color32> colors, out NativeSlice<Vector4> tangents)
		{
			this.SetMeshChannelInfo(dataSize, dataSize, 12);
			indices = this.GetNativeDataArray<ushort>(SpriteShapeDataType.Index);
			vertices = this.GetChannelDataArray<Vector3>(SpriteShapeDataType.ChannelVertex, VertexAttribute.Position);
			texcoords = this.GetChannelDataArray<Vector2>(SpriteShapeDataType.ChannelTexCoord0, VertexAttribute.TexCoord0);
			colors = this.GetChannelDataArray<Color32>(SpriteShapeDataType.ChannelColor, VertexAttribute.Color);
			tangents = this.GetChannelDataArray<Vector4>(SpriteShapeDataType.ChannelTangent, VertexAttribute.Tangent);
		}

		public void GetChannels(int dataSize, out NativeArray<ushort> indices, out NativeSlice<Vector3> vertices, out NativeSlice<Vector2> texcoords, out NativeSlice<Vector4> tangents, out NativeSlice<Vector3> normals)
		{
			this.SetMeshChannelInfo(dataSize, dataSize, 6);
			indices = this.GetNativeDataArray<ushort>(SpriteShapeDataType.Index);
			vertices = this.GetChannelDataArray<Vector3>(SpriteShapeDataType.ChannelVertex, VertexAttribute.Position);
			texcoords = this.GetChannelDataArray<Vector2>(SpriteShapeDataType.ChannelTexCoord0, VertexAttribute.TexCoord0);
			tangents = this.GetChannelDataArray<Vector4>(SpriteShapeDataType.ChannelTangent, VertexAttribute.Tangent);
			normals = this.GetChannelDataArray<Vector3>(SpriteShapeDataType.ChannelNormal, VertexAttribute.Normal);
		}

		public void GetChannels(int dataSize, out NativeArray<ushort> indices, out NativeSlice<Vector3> vertices, out NativeSlice<Vector2> texcoords, out NativeSlice<Color32> colors, out NativeSlice<Vector4> tangents, out NativeSlice<Vector3> normals)
		{
			this.SetMeshChannelInfo(dataSize, dataSize, 14);
			indices = this.GetNativeDataArray<ushort>(SpriteShapeDataType.Index);
			vertices = this.GetChannelDataArray<Vector3>(SpriteShapeDataType.ChannelVertex, VertexAttribute.Position);
			texcoords = this.GetChannelDataArray<Vector2>(SpriteShapeDataType.ChannelTexCoord0, VertexAttribute.TexCoord0);
			colors = this.GetChannelDataArray<Color32>(SpriteShapeDataType.ChannelColor, VertexAttribute.Color);
			tangents = this.GetChannelDataArray<Vector4>(SpriteShapeDataType.ChannelTangent, VertexAttribute.Tangent);
			normals = this.GetChannelDataArray<Vector3>(SpriteShapeDataType.ChannelNormal, VertexAttribute.Normal);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_color_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_color_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteMaskInteraction get_maskInteraction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Prepare_Injected(IntPtr _unity_self, [In] ref JobHandle handle, [In] ref SpriteShapeParameters shapeParams, Sprite[] sprites);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSegmentCount_Injected(IntPtr _unity_self, int geomCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMeshDataCount_Injected(IntPtr _unity_self, int vertexCount, int indexCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMeshChannelInfo_Injected(IntPtr _unity_self, int vertexCount, int indexCount, int hotChannelMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDataInfo_Injected(IntPtr _unity_self, SpriteShapeDataType arrayType, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetChannelInfo_Injected(IntPtr _unity_self, VertexAttribute channel, out SpriteChannelInfo ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLocalAABB_Injected(IntPtr _unity_self, [In] ref Bounds bounds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSplineMeshCount_Injected(IntPtr _unity_self);
	}
}
