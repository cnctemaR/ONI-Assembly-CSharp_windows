using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeClass("UI::CanvasRenderer")]
	[NativeHeader("Modules/UI/CanvasRenderer.h")]
	public sealed class CanvasRenderer : Component
	{
		public bool hasPopInstruction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_hasPopInstruction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasRenderer.set_hasPopInstruction_Injected(intPtr, value);
			}
		}

		public int materialCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_materialCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasRenderer.set_materialCount_Injected(intPtr, value);
			}
		}

		public int popMaterialCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_popMaterialCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasRenderer.set_popMaterialCount_Injected(intPtr, value);
			}
		}

		public int absoluteDepth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_absoluteDepth_Injected(intPtr);
			}
		}

		public bool hasMoved
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_hasMoved_Injected(intPtr);
			}
		}

		public bool cullTransparentMesh
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_cullTransparentMesh_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasRenderer.set_cullTransparentMesh_Injected(intPtr, value);
			}
		}

		[NativeProperty("RectClipping", false, TargetType.Function)]
		public bool hasRectClipping
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_hasRectClipping_Injected(intPtr);
			}
		}

		[NativeProperty("Depth", false, TargetType.Function)]
		public int relativeDepth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_relativeDepth_Injected(intPtr);
			}
		}

		[NativeProperty("ShouldCull", false, TargetType.Function)]
		public bool cull
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CanvasRenderer.get_cull_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasRenderer.set_cull_Injected(intPtr, value);
			}
		}

		[Obsolete("isMask is no longer supported.See EnableClipping for vertex clipping configuration", false)]
		public bool isMask { get; set; }

		public void SetColor(Color color)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.SetColor_Injected(intPtr, ref color);
		}

		public Color GetColor()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			CanvasRenderer.GetColor_Injected(intPtr, out color);
			return color;
		}

		public void EnableRectClipping(Rect rect)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.EnableRectClipping_Injected(intPtr, ref rect);
		}

		public Vector2 clippingSoftness
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				CanvasRenderer.get_clippingSoftness_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				CanvasRenderer.set_clippingSoftness_Injected(intPtr, ref value);
			}
		}

		public void DisableRectClipping()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.DisableRectClipping_Injected(intPtr);
		}

		public void SetMaterial(Material material, int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.SetMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material), index);
		}

		public Material GetMaterial(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(CanvasRenderer.GetMaterial_Injected(intPtr, index));
		}

		public void SetPopMaterial(Material material, int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.SetPopMaterial_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material), index);
		}

		public Material GetPopMaterial(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(CanvasRenderer.GetPopMaterial_Injected(intPtr, index));
		}

		public void SetTexture(Texture texture)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.SetTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(texture));
		}

		public int GetSecondaryTextureCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CanvasRenderer.GetSecondaryTextureCount_Injected(intPtr);
		}

		public void SetSecondaryTextureCount(int size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.SetSecondaryTextureCount_Injected(intPtr, size);
		}

		public string GetSecondaryTextureName(int index)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				CanvasRenderer.GetSecondaryTextureName_Injected(intPtr, index, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public Texture2D GetSecondaryTexture(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Texture2D>(CanvasRenderer.GetSecondaryTexture_Injected(intPtr, index));
		}

		public unsafe void SetSecondaryTexture(int index, string name, Texture2D texture)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				CanvasRenderer.SetSecondaryTexture_Injected(intPtr, index, ref managedSpanWrapper, Object.MarshalledUnityObject.Marshal<Texture2D>(texture));
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void SetAlphaTexture(Texture texture)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.SetAlphaTexture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(texture));
		}

		public void SetMesh(Mesh mesh)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.SetMesh_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(mesh));
		}

		public Mesh GetMesh()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(CanvasRenderer.GetMesh_Injected(intPtr));
		}

		public void Clear()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CanvasRenderer.Clear_Injected(intPtr);
		}

		public float GetAlpha()
		{
			return this.GetColor().a;
		}

		public void SetAlpha(float alpha)
		{
			Color color = this.GetColor();
			color.a = alpha;
			this.SetColor(color);
		}

		public float GetInheritedAlpha()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<CanvasRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CanvasRenderer.GetInheritedAlpha_Injected(intPtr);
		}

		public void SetMaterial(Material material, Texture texture)
		{
			this.materialCount = Math.Max(1, this.materialCount);
			this.SetMaterial(material, 0);
			this.SetTexture(texture);
		}

		public Material GetMaterial()
		{
			return this.GetMaterial(0);
		}

		public static void SplitUIVertexStreams(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector4> uv0S, List<Vector4> uv1S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
		{
			CanvasRenderer.SplitUIVertexStreams(verts, positions, colors, uv0S, uv1S, new List<Vector4>(), new List<Vector4>(), normals, tangents, indices);
		}

		public static void SplitUIVertexStreams(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector4> uv0S, List<Vector4> uv1S, List<Vector4> uv2S, List<Vector4> uv3S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
		{
			CanvasRenderer.SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, uv2S, uv3S, normals, tangents);
			CanvasRenderer.SplitIndicesStreamsInternal(verts, indices);
		}

		public static void CreateUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector4> uv0S, List<Vector4> uv1S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
		{
			CanvasRenderer.CreateUIVertexStream(verts, positions, colors, uv0S, uv1S, new List<Vector4>(), new List<Vector4>(), normals, tangents, indices);
		}

		public static void CreateUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector4> uv0S, List<Vector4> uv1S, List<Vector4> uv2S, List<Vector4> uv3S, List<Vector3> normals, List<Vector4> tangents, List<int> indices)
		{
			CanvasRenderer.CreateUIVertexStreamInternal(verts, positions, colors, uv0S, uv1S, uv2S, uv3S, normals, tangents, indices);
		}

		public static void AddUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector4> uv0S, List<Vector4> uv1S, List<Vector3> normals, List<Vector4> tangents)
		{
			CanvasRenderer.AddUIVertexStream(verts, positions, colors, uv0S, uv1S, new List<Vector4>(), new List<Vector4>(), normals, tangents);
		}

		public static void AddUIVertexStream(List<UIVertex> verts, List<Vector3> positions, List<Color32> colors, List<Vector4> uv0S, List<Vector4> uv1S, List<Vector4> uv2S, List<Vector4> uv3S, List<Vector3> normals, List<Vector4> tangents)
		{
			CanvasRenderer.SplitUIVertexStreamsInternal(verts, positions, colors, uv0S, uv1S, uv2S, uv3S, normals, tangents);
		}

		[Obsolete("UI System now uses meshes.Generate a mesh and use 'SetMesh' instead", false)]
		public void SetVertices(List<UIVertex> vertices)
		{
			this.SetVertices(vertices.ToArray(), vertices.Count);
		}

		[Obsolete("UI System now uses meshes.Generate a mesh and use 'SetMesh' instead", false)]
		public void SetVertices(UIVertex[] vertices, int size)
		{
			Mesh mesh = new Mesh();
			List<Vector3> list = new List<Vector3>();
			List<Color32> list2 = new List<Color32>();
			List<Vector4> list3 = new List<Vector4>();
			List<Vector4> list4 = new List<Vector4>();
			List<Vector4> list5 = new List<Vector4>();
			List<Vector4> list6 = new List<Vector4>();
			List<Vector3> list7 = new List<Vector3>();
			List<Vector4> list8 = new List<Vector4>();
			List<int> list9 = new List<int>();
			for (int i = 0; i < size; i += 4)
			{
				for (int j = 0; j < 4; j++)
				{
					list.Add(vertices[i + j].position);
					list2.Add(vertices[i + j].color);
					list3.Add(vertices[i + j].uv0);
					list4.Add(vertices[i + j].uv1);
					list5.Add(vertices[i + j].uv2);
					list6.Add(vertices[i + j].uv3);
					list7.Add(vertices[i + j].normal);
					list8.Add(vertices[i + j].tangent);
				}
				list9.Add(i);
				list9.Add(i + 1);
				list9.Add(i + 2);
				list9.Add(i + 2);
				list9.Add(i + 3);
				list9.Add(i);
			}
			mesh.SetVertices(list);
			mesh.SetColors(list2);
			mesh.SetNormals(list7);
			mesh.SetTangents(list8);
			mesh.SetUVs(0, list3);
			mesh.SetUVs(1, list4);
			mesh.SetUVs(2, list5);
			mesh.SetUVs(3, list6);
			mesh.SetIndices(list9.ToArray(), MeshTopology.Triangles, 0);
			this.SetMesh(mesh);
			Object.DestroyImmediate(mesh);
		}

		[StaticAccessor("UI", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SplitIndicesStreamsInternal(object verts, object indices);

		[StaticAccessor("UI", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SplitUIVertexStreamsInternal(object verts, object positions, object colors, object uv0S, object uv1S, object uv2S, object uv3S, object normals, object tangents);

		[StaticAccessor("UI", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateUIVertexStreamInternal(object verts, object positions, object colors, object uv0S, object uv1S, object uv2S, object uv3S, object normals, object tangents, object indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasPopInstruction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_hasPopInstruction_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_materialCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_materialCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_popMaterialCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_popMaterialCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_absoluteDepth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasMoved_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_cullTransparentMesh_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cullTransparentMesh_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasRectClipping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_relativeDepth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_cull_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_cull_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColor_Injected(IntPtr _unity_self, [In] ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetColor_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableRectClipping_Injected(IntPtr _unity_self, [In] ref Rect rect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_clippingSoftness_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_clippingSoftness_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableRectClipping_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMaterial_Injected(IntPtr _unity_self, IntPtr material, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetMaterial_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPopMaterial_Injected(IntPtr _unity_self, IntPtr material, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetPopMaterial_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetTexture_Injected(IntPtr _unity_self, IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSecondaryTextureCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSecondaryTextureCount_Injected(IntPtr _unity_self, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSecondaryTextureName_Injected(IntPtr _unity_self, int index, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSecondaryTexture_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSecondaryTexture_Injected(IntPtr _unity_self, int index, ref ManagedSpanWrapper name, IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAlphaTexture_Injected(IntPtr _unity_self, IntPtr texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMesh_Injected(IntPtr _unity_self, IntPtr mesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetMesh_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetInheritedAlpha_Injected(IntPtr _unity_self);
	}
}
