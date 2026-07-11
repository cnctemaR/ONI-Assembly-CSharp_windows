using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A class that allows creating or modifying meshes from scripts.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
	[RequiredByNativeCode]
	public sealed class Mesh : Object
	{
		/// <summary>
		///   <para>Creates an empty Mesh.</para>
		/// </summary>
		[RequiredByNativeCode]
		public Mesh()
		{
			Mesh.Internal_Create(this);
		}

		[FreeFunction("MeshScripting::CreateMesh")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] Mesh mono);

		[FreeFunction("MeshScripting::MeshFromInstanceId")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Mesh FromInstanceID(int id);

		/// <summary>
		///   <para>Format of the mesh index buffer data.</para>
		/// </summary>
		public extern IndexFormat indexFormat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction(Name = "MeshScripting::GetIndexStart", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern uint GetIndexStartImpl(int submesh);

		[FreeFunction(Name = "MeshScripting::GetIndexCount", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern uint GetIndexCountImpl(int submesh);

		[FreeFunction(Name = "MeshScripting::GetBaseVertex", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern uint GetBaseVertexImpl(int submesh);

		[FreeFunction(Name = "MeshScripting::GetTriangles", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int[] GetTrianglesImpl(int submesh, bool applyBaseVertex);

		[FreeFunction(Name = "MeshScripting::GetIndices", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int[] GetIndicesImpl(int submesh, bool applyBaseVertex);

		[FreeFunction(Name = "SetMeshIndicesFromScript", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetIndicesImpl(int submesh, MeshTopology topology, Array indices, int arraySize, bool calculateBounds, int baseVertex);

		[FreeFunction(Name = "MeshScripting::ExtractTrianglesToArray", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetTrianglesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex);

		[FreeFunction(Name = "MeshScripting::ExtractIndicesToArray", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetIndicesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex);

		[FreeFunction(Name = "MeshScripting::PrintErrorCantAccessChannel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void PrintErrorCantAccessChannel(Mesh.InternalShaderChannel ch);

		[FreeFunction(Name = "MeshScripting::HasChannel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool HasChannel(Mesh.InternalShaderChannel ch);

		[FreeFunction(Name = "SetMeshComponentFromArrayFromScript", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetArrayForChannelImpl(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, Array values, int arraySize);

		[FreeFunction(Name = "AllocExtractMeshComponentFromScript", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Array GetAllocArrayFromChannelImpl(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim);

		[FreeFunction(Name = "ExtractMeshComponentFromScript", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetArrayFromChannelImpl(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, Array values);

		/// <summary>
		///   <para>Gets the number of vertex buffers present in the Mesh. (Read Only)</para>
		/// </summary>
		public extern int vertexBufferCount
		{
			[FreeFunction(Name = "MeshScripting::GetVertexBufferCount", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Retrieves a native (underlying graphics API) pointer to the vertex buffer.</para>
		/// </summary>
		/// <param name="bufferIndex">Which vertex buffer to get (some Meshes might have more than one). See vertexBufferCount.</param>
		/// <param name="index"></param>
		/// <returns>
		///   <para>Pointer to the underlying graphics API vertex buffer.</para>
		/// </returns>
		[NativeThrows]
		[FreeFunction(Name = "MeshScripting::GetNativeVertexBufferPtr", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeVertexBufferPtr(int index);

		/// <summary>
		///   <para>Retrieves a native (underlying graphics API) pointer to the index buffer.</para>
		/// </summary>
		/// <returns>
		///   <para>Pointer to the underlying graphics API index buffer.</para>
		/// </returns>
		[FreeFunction(Name = "MeshScripting::GetNativeIndexBufferPtr", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeIndexBufferPtr();

		/// <summary>
		///   <para>Returns BlendShape count on this mesh.</para>
		/// </summary>
		public extern int blendShapeCount
		{
			[NativeMethod(Name = "GetBlendShapeChannelCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Clears all blend shapes from Mesh.</para>
		/// </summary>
		[FreeFunction(Name = "MeshScripting::ClearBlendShapes", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearBlendShapes();

		/// <summary>
		///   <para>Returns name of BlendShape by given index.</para>
		/// </summary>
		/// <param name="shapeIndex"></param>
		[FreeFunction(Name = "MeshScripting::GetBlendShapeName", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern string GetBlendShapeName(int shapeIndex);

		/// <summary>
		///   <para>Returns index of BlendShape by given name.</para>
		/// </summary>
		/// <param name="blendShapeName"></param>
		[FreeFunction(Name = "MeshScripting::GetBlendShapeIndex", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetBlendShapeIndex(string blendShapeName);

		/// <summary>
		///   <para>Returns the frame count for a blend shape.</para>
		/// </summary>
		/// <param name="shapeIndex">The shape index to get frame count from.</param>
		[FreeFunction(Name = "MeshScripting::GetBlendShapeFrameCount", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetBlendShapeFrameCount(int shapeIndex);

		/// <summary>
		///   <para>Returns the weight of a blend shape frame.</para>
		/// </summary>
		/// <param name="shapeIndex">The shape index of the frame.</param>
		/// <param name="frameIndex">The frame index to get the weight from.</param>
		[FreeFunction(Name = "MeshScripting::GetBlendShapeFrameWeight", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex);

		/// <summary>
		///   <para>Retreives deltaVertices, deltaNormals and deltaTangents of a blend shape frame.</para>
		/// </summary>
		/// <param name="shapeIndex">The shape index of the frame.</param>
		/// <param name="frameIndex">The frame index to get the weight from.</param>
		/// <param name="deltaVertices">Delta vertices output array for the frame being retreived.</param>
		/// <param name="deltaNormals">Delta normals output array for the frame being retreived.</param>
		/// <param name="deltaTangents">Delta tangents output array for the frame being retreived.</param>
		[FreeFunction(Name = "GetBlendShapeFrameVerticesFromScript", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);

		/// <summary>
		///   <para>Adds a new blend shape frame.</para>
		/// </summary>
		/// <param name="shapeName">Name of the blend shape to add a frame to.</param>
		/// <param name="frameWeight">Weight for the frame being added.</param>
		/// <param name="deltaVertices">Delta vertices for the frame being added.</param>
		/// <param name="deltaNormals">Delta normals for the frame being added.</param>
		/// <param name="deltaTangents">Delta tangents for the frame being added.</param>
		[FreeFunction(Name = "AddBlendShapeFrameFromScript", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetBoneWeightCount();

		/// <summary>
		///   <para>The bone weights of each vertex.</para>
		/// </summary>
		[NativeName("BoneWeightsFromScript")]
		public extern BoneWeight[] boneWeights
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetBindposeCount();

		/// <summary>
		///   <para>The bind poses. The bind pose at each index refers to the bone with the same index.</para>
		/// </summary>
		[NativeName("BindPosesFromScript")]
		public extern Matrix4x4[] bindposes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction(Name = "MeshScripting::ExtractBoneWeightsIntoArray", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetBoneWeightsNonAllocImpl([Out] BoneWeight[] values);

		[FreeFunction(Name = "MeshScripting::ExtractBindPosesIntoArray", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetBindposesNonAllocImpl([Out] Matrix4x4[] values);

		/// <summary>
		///   <para>Returns state of the Read/Write Enabled checkbox when model was imported.</para>
		/// </summary>
		public extern bool isReadable
		{
			[NativeMethod("GetIsReadable")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool canAccess
		{
			[NativeMethod("CanAccessFromScript")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the number of vertices in the Mesh (Read Only).</para>
		/// </summary>
		public extern int vertexCount
		{
			[NativeMethod("GetVertexCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>The number of sub-meshes inside the Mesh object.</para>
		/// </summary>
		public extern int subMeshCount
		{
			[NativeMethod(Name = "GetSubMeshCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(Name = "MeshScripting::SetSubMeshCount", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The bounding volume of the mesh.</para>
		/// </summary>
		public Bounds bounds
		{
			get
			{
				Bounds bounds;
				this.get_bounds_Injected(out bounds);
				return bounds;
			}
			set
			{
				this.set_bounds_Injected(ref value);
			}
		}

		[NativeMethod("Clear")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ClearImpl(bool keepVertexLayout);

		[NativeMethod("RecalculateBounds")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RecalculateBoundsImpl();

		[NativeMethod("RecalculateNormals")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RecalculateNormalsImpl();

		[NativeMethod("RecalculateTangents")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RecalculateTangentsImpl();

		[NativeMethod("MarkDynamic")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void MarkDynamicImpl();

		[NativeMethod("UploadMeshData")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void UploadMeshDataImpl(bool markNoLongerReadable);

		[FreeFunction(Name = "MeshScripting::GetPrimitiveType", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern MeshTopology GetTopologyImpl(int submesh);

		/// <summary>
		///   <para>The UV distribution metric can be used to calculate the desired mipmap level based on the position of the camera.</para>
		/// </summary>
		/// <param name="uvSetIndex">UV set index to return the UV distibution metric for. 0 for first.</param>
		/// <returns>
		///   <para>Average of triangle area / uv area.</para>
		/// </returns>
		[NativeMethod("GetMeshMetric")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float GetUVDistributionMetric(int uvSetIndex);

		[FreeFunction(Name = "MeshScripting::CombineMeshes", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void CombineMeshesImpl(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices, bool hasLightmapData);

		internal Mesh.InternalShaderChannel GetUVChannel(int uvIndex)
		{
			if (uvIndex < 0 || uvIndex > 7)
			{
				throw new ArgumentException("GetUVChannel called for bad uvIndex", "uvIndex");
			}
			return Mesh.InternalShaderChannel.TexCoord0 + uvIndex;
		}

		internal static int DefaultDimensionForChannel(Mesh.InternalShaderChannel channel)
		{
			int num;
			if (channel == Mesh.InternalShaderChannel.Vertex || channel == Mesh.InternalShaderChannel.Normal)
			{
				num = 3;
			}
			else if (channel >= Mesh.InternalShaderChannel.TexCoord0 && channel <= Mesh.InternalShaderChannel.TexCoord7)
			{
				num = 2;
			}
			else
			{
				if (channel != Mesh.InternalShaderChannel.Tangent && channel != Mesh.InternalShaderChannel.Color)
				{
					throw new ArgumentException("DefaultDimensionForChannel called for bad channel", "channel");
				}
				num = 4;
			}
			return num;
		}

		private T[] GetAllocArrayFromChannel<T>(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim)
		{
			if (this.canAccess)
			{
				if (this.HasChannel(channel))
				{
					return (T[])this.GetAllocArrayFromChannelImpl(channel, format, dim);
				}
			}
			else
			{
				this.PrintErrorCantAccessChannel(channel);
			}
			return new T[0];
		}

		private T[] GetAllocArrayFromChannel<T>(Mesh.InternalShaderChannel channel)
		{
			return this.GetAllocArrayFromChannel<T>(channel, Mesh.InternalVertexChannelType.Float, Mesh.DefaultDimensionForChannel(channel));
		}

		private void SetSizedArrayForChannel(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, Array values, int valuesCount)
		{
			if (this.canAccess)
			{
				this.SetArrayForChannelImpl(channel, format, dim, values, valuesCount);
			}
			else
			{
				this.PrintErrorCantAccessChannel(channel);
			}
		}

		private void SetArrayForChannel<T>(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, T[] values)
		{
			this.SetSizedArrayForChannel(channel, format, dim, values, NoAllocHelpers.SafeLength(values));
		}

		private void SetArrayForChannel<T>(Mesh.InternalShaderChannel channel, T[] values)
		{
			this.SetSizedArrayForChannel(channel, Mesh.InternalVertexChannelType.Float, Mesh.DefaultDimensionForChannel(channel), values, NoAllocHelpers.SafeLength(values));
		}

		private void SetListForChannel<T>(Mesh.InternalShaderChannel channel, Mesh.InternalVertexChannelType format, int dim, List<T> values)
		{
			this.SetSizedArrayForChannel(channel, format, dim, NoAllocHelpers.ExtractArrayFromList(values), NoAllocHelpers.SafeLength<T>(values));
		}

		private void SetListForChannel<T>(Mesh.InternalShaderChannel channel, List<T> values)
		{
			this.SetSizedArrayForChannel(channel, Mesh.InternalVertexChannelType.Float, Mesh.DefaultDimensionForChannel(channel), NoAllocHelpers.ExtractArrayFromList(values), NoAllocHelpers.SafeLength<T>(values));
		}

		private void GetListForChannel<T>(List<T> buffer, int capacity, Mesh.InternalShaderChannel channel, int dim)
		{
			this.GetListForChannel<T>(buffer, capacity, channel, dim, Mesh.InternalVertexChannelType.Float);
		}

		private void GetListForChannel<T>(List<T> buffer, int capacity, Mesh.InternalShaderChannel channel, int dim, Mesh.InternalVertexChannelType channelType)
		{
			buffer.Clear();
			if (!this.canAccess)
			{
				this.PrintErrorCantAccessChannel(channel);
			}
			else if (this.HasChannel(channel))
			{
				NoAllocHelpers.EnsureListElemCount<T>(buffer, capacity);
				this.GetArrayFromChannelImpl(channel, channelType, dim, NoAllocHelpers.ExtractArrayFromList(buffer));
			}
		}

		/// <summary>
		///   <para>Returns a copy of the vertex positions or assigns a new vertex positions array.</para>
		/// </summary>
		public Vector3[] vertices
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector3>(Mesh.InternalShaderChannel.Vertex);
			}
			set
			{
				this.SetArrayForChannel<Vector3>(Mesh.InternalShaderChannel.Vertex, value);
			}
		}

		/// <summary>
		///   <para>The normals of the Mesh.</para>
		/// </summary>
		public Vector3[] normals
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector3>(Mesh.InternalShaderChannel.Normal);
			}
			set
			{
				this.SetArrayForChannel<Vector3>(Mesh.InternalShaderChannel.Normal, value);
			}
		}

		/// <summary>
		///   <para>The tangents of the Mesh.</para>
		/// </summary>
		public Vector4[] tangents
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector4>(Mesh.InternalShaderChannel.Tangent);
			}
			set
			{
				this.SetArrayForChannel<Vector4>(Mesh.InternalShaderChannel.Tangent, value);
			}
		}

		/// <summary>
		///   <para>The base texture coordinates of the Mesh.</para>
		/// </summary>
		public Vector2[] uv
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord0);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord0, value);
			}
		}

		/// <summary>
		///   <para>The second texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public Vector2[] uv2
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord1);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord1, value);
			}
		}

		/// <summary>
		///   <para>The third texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public Vector2[] uv3
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord2);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord2, value);
			}
		}

		/// <summary>
		///   <para>The fourth texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public Vector2[] uv4
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord3);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord3, value);
			}
		}

		/// <summary>
		///   <para>The fifth texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public Vector2[] uv5
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord4);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord4, value);
			}
		}

		/// <summary>
		///   <para>The sixth texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public Vector2[] uv6
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord5);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord5, value);
			}
		}

		/// <summary>
		///   <para>The seventh texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public Vector2[] uv7
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord6);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord6, value);
			}
		}

		/// <summary>
		///   <para>The eighth texture coordinate set of the mesh, if present.</para>
		/// </summary>
		public Vector2[] uv8
		{
			get
			{
				return this.GetAllocArrayFromChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord7);
			}
			set
			{
				this.SetArrayForChannel<Vector2>(Mesh.InternalShaderChannel.TexCoord7, value);
			}
		}

		/// <summary>
		///   <para>Vertex colors of the Mesh.</para>
		/// </summary>
		public Color[] colors
		{
			get
			{
				return this.GetAllocArrayFromChannel<Color>(Mesh.InternalShaderChannel.Color);
			}
			set
			{
				this.SetArrayForChannel<Color>(Mesh.InternalShaderChannel.Color, value);
			}
		}

		/// <summary>
		///   <para>Vertex colors of the Mesh.</para>
		/// </summary>
		public Color32[] colors32
		{
			get
			{
				return this.GetAllocArrayFromChannel<Color32>(Mesh.InternalShaderChannel.Color, Mesh.InternalVertexChannelType.Color, 1);
			}
			set
			{
				this.SetArrayForChannel<Color32>(Mesh.InternalShaderChannel.Color, Mesh.InternalVertexChannelType.Color, 1, value);
			}
		}

		public void GetVertices(List<Vector3> vertices)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("The result vertices list cannot be null.", "vertices");
			}
			this.GetListForChannel<Vector3>(vertices, this.vertexCount, Mesh.InternalShaderChannel.Vertex, Mesh.DefaultDimensionForChannel(Mesh.InternalShaderChannel.Vertex));
		}

		public void SetVertices(List<Vector3> inVertices)
		{
			this.SetListForChannel<Vector3>(Mesh.InternalShaderChannel.Vertex, inVertices);
		}

		public void GetNormals(List<Vector3> normals)
		{
			if (normals == null)
			{
				throw new ArgumentNullException("The result normals list cannot be null.", "normals");
			}
			this.GetListForChannel<Vector3>(normals, this.vertexCount, Mesh.InternalShaderChannel.Normal, Mesh.DefaultDimensionForChannel(Mesh.InternalShaderChannel.Normal));
		}

		public void SetNormals(List<Vector3> inNormals)
		{
			this.SetListForChannel<Vector3>(Mesh.InternalShaderChannel.Normal, inNormals);
		}

		public void GetTangents(List<Vector4> tangents)
		{
			if (tangents == null)
			{
				throw new ArgumentNullException("The result tangents list cannot be null.", "tangents");
			}
			this.GetListForChannel<Vector4>(tangents, this.vertexCount, Mesh.InternalShaderChannel.Tangent, Mesh.DefaultDimensionForChannel(Mesh.InternalShaderChannel.Tangent));
		}

		public void SetTangents(List<Vector4> inTangents)
		{
			this.SetListForChannel<Vector4>(Mesh.InternalShaderChannel.Tangent, inTangents);
		}

		public void GetColors(List<Color> colors)
		{
			if (colors == null)
			{
				throw new ArgumentNullException("The result colors list cannot be null.", "colors");
			}
			this.GetListForChannel<Color>(colors, this.vertexCount, Mesh.InternalShaderChannel.Color, Mesh.DefaultDimensionForChannel(Mesh.InternalShaderChannel.Color));
		}

		public void SetColors(List<Color> inColors)
		{
			this.SetListForChannel<Color>(Mesh.InternalShaderChannel.Color, inColors);
		}

		public void GetColors(List<Color32> colors)
		{
			if (colors == null)
			{
				throw new ArgumentNullException("The result colors list cannot be null.", "colors");
			}
			this.GetListForChannel<Color32>(colors, this.vertexCount, Mesh.InternalShaderChannel.Color, 1, Mesh.InternalVertexChannelType.Color);
		}

		public void SetColors(List<Color32> inColors)
		{
			this.SetListForChannel<Color32>(Mesh.InternalShaderChannel.Color, Mesh.InternalVertexChannelType.Color, 1, inColors);
		}

		private void SetUvsImpl<T>(int uvIndex, int dim, List<T> uvs)
		{
			if (uvIndex < 0 || uvIndex > 7)
			{
				Debug.LogError("The uv index is invalid. Must be in the range 0 to 7.");
			}
			else
			{
				this.SetListForChannel<T>(this.GetUVChannel(uvIndex), Mesh.InternalVertexChannelType.Float, dim, uvs);
			}
		}

		public void SetUVs(int channel, List<Vector2> uvs)
		{
			this.SetUvsImpl<Vector2>(channel, 2, uvs);
		}

		public void SetUVs(int channel, List<Vector3> uvs)
		{
			this.SetUvsImpl<Vector3>(channel, 3, uvs);
		}

		public void SetUVs(int channel, List<Vector4> uvs)
		{
			this.SetUvsImpl<Vector4>(channel, 4, uvs);
		}

		private void GetUVsImpl<T>(int uvIndex, List<T> uvs, int dim)
		{
			if (uvs == null)
			{
				throw new ArgumentNullException("The result uvs list cannot be null.", "uvs");
			}
			if (uvIndex < 0 || uvIndex > 7)
			{
				throw new IndexOutOfRangeException("The uv index is invalid. Must be in the range 0 to 7.");
			}
			this.GetListForChannel<T>(uvs, this.vertexCount, this.GetUVChannel(uvIndex), dim);
		}

		public void GetUVs(int channel, List<Vector2> uvs)
		{
			this.GetUVsImpl<Vector2>(channel, uvs, 2);
		}

		public void GetUVs(int channel, List<Vector3> uvs)
		{
			this.GetUVsImpl<Vector3>(channel, uvs, 3);
		}

		public void GetUVs(int channel, List<Vector4> uvs)
		{
			this.GetUVsImpl<Vector4>(channel, uvs, 4);
		}

		private void PrintErrorCantAccessIndices()
		{
			Debug.LogError(string.Format("Not allowed to access triangles/indices on mesh '{0}' (isReadable is false; Read/Write must be enabled in import settings)", base.name));
		}

		private bool CheckCanAccessSubmesh(int submesh, bool errorAboutTriangles)
		{
			bool flag;
			if (!this.canAccess)
			{
				this.PrintErrorCantAccessIndices();
				flag = false;
			}
			else if (submesh < 0 || submesh >= this.subMeshCount)
			{
				Debug.LogError(string.Format("Failed getting {0}. Submesh index is out of bounds.", (!errorAboutTriangles) ? "indices" : "triangles"), this);
				flag = false;
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private bool CheckCanAccessSubmeshTriangles(int submesh)
		{
			return this.CheckCanAccessSubmesh(submesh, true);
		}

		private bool CheckCanAccessSubmeshIndices(int submesh)
		{
			return this.CheckCanAccessSubmesh(submesh, false);
		}

		/// <summary>
		///   <para>An array containing all triangles in the Mesh.</para>
		/// </summary>
		public int[] triangles
		{
			get
			{
				int[] array;
				if (this.canAccess)
				{
					array = this.GetTrianglesImpl(-1, true);
				}
				else
				{
					this.PrintErrorCantAccessIndices();
					array = new int[0];
				}
				return array;
			}
			set
			{
				if (this.canAccess)
				{
					this.SetTrianglesImpl(-1, value, NoAllocHelpers.SafeLength(value), true, 0);
				}
				else
				{
					this.PrintErrorCantAccessIndices();
				}
			}
		}

		/// <summary>
		///   <para>Fetches the triangle list for the specified sub-mesh on this object.</para>
		/// </summary>
		/// <param name="triangles">A list of vertex indices to populate.</param>
		/// <param name="submesh">The sub-mesh index. See subMeshCount.</param>
		/// <param name="applyBaseVertex">True (default value) will apply base vertex offset to returned indices.</param>
		public int[] GetTriangles(int submesh)
		{
			return this.GetTriangles(submesh, true);
		}

		/// <summary>
		///   <para>Fetches the triangle list for the specified sub-mesh on this object.</para>
		/// </summary>
		/// <param name="triangles">A list of vertex indices to populate.</param>
		/// <param name="submesh">The sub-mesh index. See subMeshCount.</param>
		/// <param name="applyBaseVertex">True (default value) will apply base vertex offset to returned indices.</param>
		public int[] GetTriangles(int submesh, [UnityEngine.Internal.DefaultValue("true")] bool applyBaseVertex)
		{
			return (!this.CheckCanAccessSubmeshTriangles(submesh)) ? new int[0] : this.GetTrianglesImpl(submesh, applyBaseVertex);
		}

		public void GetTriangles(List<int> triangles, int submesh)
		{
			this.GetTriangles(triangles, submesh, true);
		}

		public void GetTriangles(List<int> triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool applyBaseVertex)
		{
			if (triangles == null)
			{
				throw new ArgumentNullException("The result triangles list cannot be null.", "triangles");
			}
			if (submesh < 0 || submesh >= this.subMeshCount)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			NoAllocHelpers.EnsureListElemCount<int>(triangles, (int)this.GetIndexCount(submesh));
			this.GetTrianglesNonAllocImpl(NoAllocHelpers.ExtractArrayFromListT<int>(triangles), submesh, applyBaseVertex);
		}

		/// <summary>
		///   <para>Fetches the index list for the specified sub-mesh.</para>
		/// </summary>
		/// <param name="indices">A list of indices to populate.</param>
		/// <param name="submesh">The sub-mesh index. See subMeshCount.</param>
		/// <param name="applyBaseVertex">True (default value) will apply base vertex offset to returned indices.</param>
		public int[] GetIndices(int submesh)
		{
			return this.GetIndices(submesh, true);
		}

		public int[] GetIndices(int submesh, [UnityEngine.Internal.DefaultValue("true")] bool applyBaseVertex)
		{
			return (!this.CheckCanAccessSubmeshIndices(submesh)) ? new int[0] : this.GetIndicesImpl(submesh, applyBaseVertex);
		}

		public void GetIndices(List<int> indices, int submesh)
		{
			this.GetIndices(indices, submesh, true);
		}

		public void GetIndices(List<int> indices, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool applyBaseVertex)
		{
			if (indices == null)
			{
				throw new ArgumentNullException("The result indices list cannot be null.", "indices");
			}
			if (submesh < 0 || submesh >= this.subMeshCount)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			NoAllocHelpers.EnsureListElemCount<int>(indices, (int)this.GetIndexCount(submesh));
			this.GetIndicesNonAllocImpl(NoAllocHelpers.ExtractArrayFromListT<int>(indices), submesh, applyBaseVertex);
		}

		/// <summary>
		///   <para>Gets the starting index location within the Mesh's index buffer, for the given sub-mesh.</para>
		/// </summary>
		/// <param name="submesh"></param>
		public uint GetIndexStart(int submesh)
		{
			if (submesh < 0 || submesh >= this.subMeshCount)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			return this.GetIndexStartImpl(submesh);
		}

		/// <summary>
		///   <para>Gets the index count of the given sub-mesh.</para>
		/// </summary>
		/// <param name="submesh"></param>
		public uint GetIndexCount(int submesh)
		{
			if (submesh < 0 || submesh >= this.subMeshCount)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			return this.GetIndexCountImpl(submesh);
		}

		/// <summary>
		///   <para>Gets the base vertex index of the given sub-mesh.</para>
		/// </summary>
		/// <param name="submesh">The sub-mesh index. See subMeshCount.</param>
		/// <returns>
		///   <para>The offset applied to all vertex indices of this sub-mesh.</para>
		/// </returns>
		public uint GetBaseVertex(int submesh)
		{
			if (submesh < 0 || submesh >= this.subMeshCount)
			{
				throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
			}
			return this.GetBaseVertexImpl(submesh);
		}

		private void SetTrianglesImpl(int submesh, Array triangles, int arraySize, bool calculateBounds, int baseVertex)
		{
			this.SetIndicesImpl(submesh, MeshTopology.Triangles, triangles, arraySize, calculateBounds, baseVertex);
		}

		/// <summary>
		///   <para>Sets the triangle list for the sub-mesh.</para>
		/// </summary>
		/// <param name="triangles">The list of indices that define the triangles.</param>
		/// <param name="submesh">The sub-mesh to modify.</param>
		/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the triangles. This is done by default.
		/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the triangles.</param>
		/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
		public void SetTriangles(int[] triangles, int submesh)
		{
			this.SetTriangles(triangles, submesh, true, 0);
		}

		public void SetTriangles(int[] triangles, int submesh, bool calculateBounds)
		{
			this.SetTriangles(triangles, submesh, calculateBounds, 0);
		}

		/// <summary>
		///   <para>Sets the triangle list for the sub-mesh.</para>
		/// </summary>
		/// <param name="triangles">The list of indices that define the triangles.</param>
		/// <param name="submesh">The sub-mesh to modify.</param>
		/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the triangles. This is done by default.
		/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the triangles.</param>
		/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
		public void SetTriangles(int[] triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds, [UnityEngine.Internal.DefaultValue("0")] int baseVertex)
		{
			if (this.CheckCanAccessSubmeshTriangles(submesh))
			{
				this.SetTrianglesImpl(submesh, triangles, NoAllocHelpers.SafeLength(triangles), calculateBounds, baseVertex);
			}
		}

		public void SetTriangles(List<int> triangles, int submesh)
		{
			this.SetTriangles(triangles, submesh, true, 0);
		}

		public void SetTriangles(List<int> triangles, int submesh, bool calculateBounds)
		{
			this.SetTriangles(triangles, submesh, calculateBounds, 0);
		}

		public void SetTriangles(List<int> triangles, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds, [UnityEngine.Internal.DefaultValue("0")] int baseVertex)
		{
			if (this.CheckCanAccessSubmeshTriangles(submesh))
			{
				this.SetTrianglesImpl(submesh, NoAllocHelpers.ExtractArrayFromList(triangles), NoAllocHelpers.SafeLength<int>(triangles), calculateBounds, baseVertex);
			}
		}

		/// <summary>
		///   <para>Sets the index buffer for the sub-mesh.</para>
		/// </summary>
		/// <param name="indices">The array of indices that define the Mesh.</param>
		/// <param name="topology">The topology of the Mesh, e.g: Triangles, Lines, Quads, Points, etc. See MeshTopology.</param>
		/// <param name="submesh">The sub-mesh to modify.</param>
		/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the indices. This is done by default.
		/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the indices.</param>
		/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
		public void SetIndices(int[] indices, MeshTopology topology, int submesh)
		{
			this.SetIndices(indices, topology, submesh, true, 0);
		}

		/// <summary>
		///   <para>Sets the index buffer for the sub-mesh.</para>
		/// </summary>
		/// <param name="indices">The array of indices that define the Mesh.</param>
		/// <param name="topology">The topology of the Mesh, e.g: Triangles, Lines, Quads, Points, etc. See MeshTopology.</param>
		/// <param name="submesh">The sub-mesh to modify.</param>
		/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the indices. This is done by default.
		/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the indices.</param>
		/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
		public void SetIndices(int[] indices, MeshTopology topology, int submesh, bool calculateBounds)
		{
			this.SetIndices(indices, topology, submesh, calculateBounds, 0);
		}

		/// <summary>
		///   <para>Sets the index buffer for the sub-mesh.</para>
		/// </summary>
		/// <param name="indices">The array of indices that define the Mesh.</param>
		/// <param name="topology">The topology of the Mesh, e.g: Triangles, Lines, Quads, Points, etc. See MeshTopology.</param>
		/// <param name="submesh">The sub-mesh to modify.</param>
		/// <param name="calculateBounds">Calculate the bounding box of the Mesh after setting the indices. This is done by default.
		/// Use false when you want to use the existing bounding box and reduce the CPU cost of setting the indices.</param>
		/// <param name="baseVertex">Optional vertex offset that is added to all triangle vertex indices.</param>
		public void SetIndices(int[] indices, MeshTopology topology, int submesh, [UnityEngine.Internal.DefaultValue("true")] bool calculateBounds, [UnityEngine.Internal.DefaultValue("0")] int baseVertex)
		{
			if (this.CheckCanAccessSubmeshIndices(submesh))
			{
				this.SetIndicesImpl(submesh, topology, indices, NoAllocHelpers.SafeLength(indices), calculateBounds, baseVertex);
			}
		}

		public void GetBindposes(List<Matrix4x4> bindposes)
		{
			if (bindposes == null)
			{
				throw new ArgumentNullException("The result bindposes list cannot be null.", "bindposes");
			}
			NoAllocHelpers.EnsureListElemCount<Matrix4x4>(bindposes, this.GetBindposeCount());
			this.GetBindposesNonAllocImpl(NoAllocHelpers.ExtractArrayFromListT<Matrix4x4>(bindposes));
		}

		public void GetBoneWeights(List<BoneWeight> boneWeights)
		{
			if (boneWeights == null)
			{
				throw new ArgumentNullException("The result boneWeights list cannot be null.", "boneWeights");
			}
			NoAllocHelpers.EnsureListElemCount<BoneWeight>(boneWeights, this.GetBoneWeightCount());
			this.GetBoneWeightsNonAllocImpl(NoAllocHelpers.ExtractArrayFromListT<BoneWeight>(boneWeights));
		}

		/// <summary>
		///   <para>Clears all vertex data and all triangle indices.</para>
		/// </summary>
		/// <param name="keepVertexLayout"></param>
		public void Clear(bool keepVertexLayout)
		{
			this.ClearImpl(keepVertexLayout);
		}

		public void Clear()
		{
			this.ClearImpl(true);
		}

		/// <summary>
		///   <para>Recalculate the bounding volume of the Mesh from the vertices.</para>
		/// </summary>
		public void RecalculateBounds()
		{
			if (this.canAccess)
			{
				this.RecalculateBoundsImpl();
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call RecalculateBounds() on mesh '{0}'", base.name));
			}
		}

		/// <summary>
		///   <para>Recalculates the normals of the Mesh from the triangles and vertices.</para>
		/// </summary>
		public void RecalculateNormals()
		{
			if (this.canAccess)
			{
				this.RecalculateNormalsImpl();
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call RecalculateNormals() on mesh '{0}'", base.name));
			}
		}

		/// <summary>
		///   <para>Recalculates the tangents of the Mesh from the normals and texture coordinates.</para>
		/// </summary>
		public void RecalculateTangents()
		{
			if (this.canAccess)
			{
				this.RecalculateTangentsImpl();
			}
			else
			{
				Debug.LogError(string.Format("Not allowed to call RecalculateTangents() on mesh '{0}'", base.name));
			}
		}

		/// <summary>
		///   <para>Optimize mesh for frequent updates.</para>
		/// </summary>
		public void MarkDynamic()
		{
			if (this.canAccess)
			{
				this.MarkDynamicImpl();
			}
		}

		/// <summary>
		///   <para>Upload previously done Mesh modifications to the graphics API.</para>
		/// </summary>
		/// <param name="markNoLongerReadable">Frees up system memory copy of mesh data when set to true.</param>
		public void UploadMeshData(bool markNoLongerReadable)
		{
			if (this.canAccess)
			{
				this.UploadMeshDataImpl(markNoLongerReadable);
			}
		}

		/// <summary>
		///   <para>Gets the topology of a sub-mesh.</para>
		/// </summary>
		/// <param name="submesh"></param>
		public MeshTopology GetTopology(int submesh)
		{
			MeshTopology meshTopology;
			if (submesh < 0 || submesh >= this.subMeshCount)
			{
				Debug.LogError(string.Format("Failed getting topology. Submesh index is out of bounds.", new object[0]), this);
				meshTopology = MeshTopology.Triangles;
			}
			else
			{
				meshTopology = this.GetTopologyImpl(submesh);
			}
			return meshTopology;
		}

		/// <summary>
		///   <para>Combines several Meshes into this Mesh.</para>
		/// </summary>
		/// <param name="combine">Descriptions of the Meshes to combine.</param>
		/// <param name="mergeSubMeshes">Defines whether Meshes should be combined into a single sub-mesh.</param>
		/// <param name="useMatrices">Defines whether the transforms supplied in the CombineInstance array should be used or ignored.</param>
		/// <param name="hasLightmapData"></param>
		public void CombineMeshes(CombineInstance[] combine, [UnityEngine.Internal.DefaultValue("true")] bool mergeSubMeshes, [UnityEngine.Internal.DefaultValue("true")] bool useMatrices, [UnityEngine.Internal.DefaultValue("false")] bool hasLightmapData)
		{
			this.CombineMeshesImpl(combine, mergeSubMeshes, useMatrices, hasLightmapData);
		}

		public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices)
		{
			this.CombineMeshesImpl(combine, mergeSubMeshes, useMatrices, false);
		}

		public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
		{
			this.CombineMeshesImpl(combine, mergeSubMeshes, true, false);
		}

		public void CombineMeshes(CombineInstance[] combine)
		{
			this.CombineMeshesImpl(combine, true, true, false);
		}

		/// <summary>
		///   <para>Optimizes the Mesh for display.</para>
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is no longer supported (UnityUpgradable)", true)]
		public void Optimize()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_bounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_bounds_Injected(ref Bounds value);

		internal enum InternalShaderChannel
		{
			Vertex,
			Normal,
			Tangent,
			Color,
			TexCoord0,
			TexCoord1,
			TexCoord2,
			TexCoord3,
			TexCoord4,
			TexCoord5,
			TexCoord6,
			TexCoord7
		}

		internal enum InternalVertexChannelType
		{
			Float,
			Color = 2
		}
	}
}
