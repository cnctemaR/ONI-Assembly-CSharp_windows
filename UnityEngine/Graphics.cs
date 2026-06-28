using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public sealed class Graphics
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Array ExtractArrayFromList(object list);

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
		{
			bool flag = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
		{
			bool flag = true;
			bool flag2 = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, flag2, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, flag3, flag2, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			MaterialPropertyBlock materialPropertyBlock = null;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, materialPropertyBlock, flag3, flag2, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = 0;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, num, materialPropertyBlock, flag3, flag2, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = 0;
			Camera camera = null;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, num, materialPropertyBlock, flag3, flag2, flag);
		}

		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
		{
			bool flag = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			bool flag = true;
			Transform transform = null;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, transform, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			bool flag = true;
			Transform transform = null;
			bool flag2 = true;
			Graphics.DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, flag2, transform, flag);
		}

		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor, [DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMeshImpl(mesh, Matrix4x4.TRS(position, rotation, Vector3.one), material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
		{
			bool flag = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
		{
			bool flag = true;
			bool flag2 = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, flag2, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, flag3, flag2, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			MaterialPropertyBlock materialPropertyBlock = null;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, materialPropertyBlock, flag3, flag2, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = 0;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, num, materialPropertyBlock, flag3, flag2, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer)
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			MaterialPropertyBlock materialPropertyBlock = null;
			int num = 0;
			Camera camera = null;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, num, materialPropertyBlock, flag3, flag2, flag);
		}

		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMeshImpl(mesh, matrix, material, layer, camera, submeshIndex, properties, (!castShadows) ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows, null, useLightProbes);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor)
		{
			bool flag = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			bool flag = true;
			Transform transform = null;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, transform, flag);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			bool flag = true;
			Transform transform = null;
			bool flag2 = true;
			Graphics.DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, flag2, transform, flag);
		}

		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor, [DefaultValue("true")] bool useLightProbes)
		{
			Graphics.DrawMeshImpl(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor, useLightProbes);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshMatrix(ref Internal_DrawMeshMatrixArguments arguments, MaterialPropertyBlock properties, Material material, Mesh mesh, Camera camera);

		private static void Internal_DrawMeshNow1(Mesh mesh, int subsetIndex, Vector3 position, Quaternion rotation)
		{
			Graphics.INTERNAL_CALL_Internal_DrawMeshNow1(mesh, subsetIndex, ref position, ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawMeshNow1(Mesh mesh, int subsetIndex, ref Vector3 position, ref Quaternion rotation);

		private static void Internal_DrawMeshNow2(Mesh mesh, int subsetIndex, Matrix4x4 matrix)
		{
			Graphics.INTERNAL_CALL_Internal_DrawMeshNow2(mesh, subsetIndex, ref matrix);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawMeshNow2(Mesh mesh, int subsetIndex, ref Matrix4x4 matrix);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DrawProcedural(MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount);

		[ExcludeFromDocs]
		public static void DrawProcedural(MeshTopology topology, int vertexCount)
		{
			int num = 1;
			Graphics.DrawProcedural(topology, vertexCount, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset);

		[ExcludeFromDocs]
		public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			int num = 0;
			Graphics.DrawProceduralIndirect(topology, bufferWithArgs, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetMaxDrawMeshInstanceCount();

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
		{
			Camera camera = null;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			Camera camera = null;
			int num = 0;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, flag, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, shadowCastingMode, flag, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
			MaterialPropertyBlock materialPropertyBlock = null;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, materialPropertyBlock, shadowCastingMode, flag, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
			MaterialPropertyBlock materialPropertyBlock = null;
			int num2 = matrices.Length;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, num2, materialPropertyBlock, shadowCastingMode, flag, num, camera);
		}

		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, [DefaultValue("matrices.Length")] int count, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera)
		{
			Graphics.DrawMeshInstancedImpl(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
		{
			Camera camera = null;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			Camera camera = null;
			int num = 0;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, castShadows, flag, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, properties, shadowCastingMode, flag, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
			MaterialPropertyBlock materialPropertyBlock = null;
			Graphics.DrawMeshInstanced(mesh, submeshIndex, material, matrices, materialPropertyBlock, shadowCastingMode, flag, num, camera);
		}

		public static void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera)
		{
			Graphics.DrawMeshInstancedImpl(mesh, submeshIndex, material, matrices, properties, castShadows, receiveShadows, layer, camera);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera);

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer)
		{
			Camera camera = null;
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
		{
			Camera camera = null;
			int num = 0;
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, flag, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, shadowCastingMode, flag, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
			MaterialPropertyBlock materialPropertyBlock = null;
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, materialPropertyBlock, shadowCastingMode, flag, num, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs)
		{
			Camera camera = null;
			int num = 0;
			bool flag = true;
			ShadowCastingMode shadowCastingMode = ShadowCastingMode.On;
			MaterialPropertyBlock materialPropertyBlock = null;
			int num2 = 0;
			Graphics.DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, num2, materialPropertyBlock, shadowCastingMode, flag, num, camera);
		}

		public static void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("ShadowCastingMode.On")] ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("0")] int layer, [DefaultValue("null")] Camera camera)
		{
			Graphics.DrawMeshInstancedIndirectImpl(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
		}

		private static void Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
		{
			Graphics.INTERNAL_CALL_Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, ref bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, ref Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera);

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Material mat)
		{
			int num = -1;
			Graphics.DrawTexture(screenRect, texture, mat, num);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture)
		{
			int num = -1;
			Material material = null;
			Graphics.DrawTexture(screenRect, texture, material, num);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.DrawTexture(screenRect, texture, 0, 0, 0, 0, mat, pass);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
		{
			int num = -1;
			Graphics.DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat, num);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			int num = -1;
			Material material = null;
			Graphics.DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, material, num);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), leftBorder, rightBorder, topBorder, bottomBorder, mat, pass);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat)
		{
			int num = -1;
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat, num);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			int num = -1;
			Material material = null;
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, material, num);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
		{
			Color32 color = new Color32(128, 128, 128, 128);
			Graphics.DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat)
		{
			int num = -1;
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, num);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color)
		{
			int num = -1;
			Material material = null;
			Graphics.DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, material, num);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, [DefaultValue("null")] Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.DrawTextureImpl(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat, pass);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Internal_DrawTexture(ref Internal_DrawTextureArguments args);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ExecuteCommandBuffer(CommandBuffer buffer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Blit(Texture source, RenderTexture dest);

		[ExcludeFromDocs]
		public static void Blit(Texture source, RenderTexture dest, Material mat)
		{
			int num = -1;
			Graphics.Blit(source, dest, mat, num);
		}

		public static void Blit(Texture source, RenderTexture dest, Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.Internal_BlitMaterial(source, dest, mat, pass, true);
		}

		[ExcludeFromDocs]
		public static void Blit(Texture source, Material mat)
		{
			int num = -1;
			Graphics.Blit(source, mat, num);
		}

		public static void Blit(Texture source, Material mat, [DefaultValue("-1")] int pass)
		{
			Graphics.Internal_BlitMaterial(source, null, mat, pass, false);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BlitMaterial(Texture source, RenderTexture dest, Material mat, int pass, bool setRT);

		public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, params Vector2[] offsets)
		{
			Graphics.Internal_BlitMultiTap(source, dest, mat, offsets);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BlitMultiTap(Texture source, RenderTexture dest, Material mat, Vector2[] offsets);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Full(Texture src, Texture dst);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Slice_AllMips(Texture src, int srcElement, Texture dst, int dstElement);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Slice(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Region(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ConvertTexture_Full(Texture src, Texture dst);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ConvertTexture_Slice(Texture src, int srcElement, Texture dst, int dstElement);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetNullRT();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRTSimple(out RenderBuffer color, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetMRTFullSetup(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice, RenderBufferLoadAction[] colorLoadSA, RenderBufferStoreAction[] colorStoreSA, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetMRTSimple(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, int depthSlice);

		public static RenderBuffer activeColorBuffer
		{
			get
			{
				RenderBuffer renderBuffer;
				Graphics.GetActiveColorBuffer(out renderBuffer);
				return renderBuffer;
			}
		}

		public static RenderBuffer activeDepthBuffer
		{
			get
			{
				RenderBuffer renderBuffer;
				Graphics.GetActiveDepthBuffer(out renderBuffer);
				return renderBuffer;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveColorBuffer(out RenderBuffer res);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveDepthBuffer(out RenderBuffer res);

		public static void SetRandomWriteTarget(int index, RenderTexture uav)
		{
			Graphics.Internal_SetRandomWriteTargetRT(index, uav);
		}

		[ExcludeFromDocs]
		public static void SetRandomWriteTarget(int index, ComputeBuffer uav)
		{
			bool flag = false;
			Graphics.SetRandomWriteTarget(index, uav, flag);
		}

		public static void SetRandomWriteTarget(int index, ComputeBuffer uav, [DefaultValue("false")] bool preserveCounterValue)
		{
			if (uav == null)
			{
				throw new ArgumentNullException("uav");
			}
			if (uav.m_Ptr == IntPtr.Zero)
			{
				throw new ObjectDisposedException("uav");
			}
			Graphics.Internal_SetRandomWriteTargetBuffer(index, uav, preserveCounterValue);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearRandomWriteTargets();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav, bool preserveCounterValue);

		public static extern GraphicsTier activeTier
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetupVertexLights(Light[] lights);

		internal static void CheckLoadActionValid(RenderBufferLoadAction load, string bufferType)
		{
			if (load != RenderBufferLoadAction.Load && load != RenderBufferLoadAction.DontCare)
			{
				throw new ArgumentException(UnityString.Format("Bad {0} LoadAction provided.", new object[] { bufferType }));
			}
		}

		internal static void CheckStoreActionValid(RenderBufferStoreAction store, string bufferType)
		{
			if (store != RenderBufferStoreAction.Store && store != RenderBufferStoreAction.DontCare)
			{
				throw new ArgumentException(UnityString.Format("Bad {0} StoreAction provided.", new object[] { bufferType }));
			}
		}

		internal static void SetRenderTargetImpl(RenderTargetSetup setup)
		{
			if (setup.color.Length == 0)
			{
				throw new ArgumentException("Invalid color buffer count for SetRenderTarget");
			}
			if (setup.color.Length != setup.colorLoad.Length)
			{
				throw new ArgumentException("Color LoadAction and Buffer arrays have different sizes");
			}
			if (setup.color.Length != setup.colorStore.Length)
			{
				throw new ArgumentException("Color StoreAction and Buffer arrays have different sizes");
			}
			foreach (RenderBufferLoadAction renderBufferLoadAction in setup.colorLoad)
			{
				Graphics.CheckLoadActionValid(renderBufferLoadAction, "Color");
			}
			foreach (RenderBufferStoreAction renderBufferStoreAction in setup.colorStore)
			{
				Graphics.CheckStoreActionValid(renderBufferStoreAction, "Color");
			}
			Graphics.CheckLoadActionValid(setup.depthLoad, "Depth");
			Graphics.CheckStoreActionValid(setup.depthStore, "Depth");
			if (setup.cubemapFace < CubemapFace.Unknown || setup.cubemapFace > CubemapFace.NegativeZ)
			{
				throw new ArgumentException("Bad CubemapFace provided");
			}
			Graphics.Internal_SetMRTFullSetup(setup.color, out setup.depth, setup.mipLevel, setup.cubemapFace, setup.depthSlice, setup.colorLoad, setup.colorStore, setup.depthLoad, setup.depthStore);
		}

		internal static void SetRenderTargetImpl(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
		{
			RenderBuffer renderBuffer = colorBuffer;
			RenderBuffer renderBuffer2 = depthBuffer;
			Graphics.Internal_SetRTSimple(out renderBuffer, out renderBuffer2, mipLevel, face, depthSlice);
		}

		internal static void SetRenderTargetImpl(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
		{
			if (rt)
			{
				Graphics.SetRenderTargetImpl(rt.colorBuffer, rt.depthBuffer, mipLevel, face, depthSlice);
			}
			else
			{
				Graphics.Internal_SetNullRT();
			}
		}

		internal static void SetRenderTargetImpl(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
		{
			RenderBuffer renderBuffer = depthBuffer;
			Graphics.Internal_SetMRTSimple(colorBuffers, out renderBuffer, mipLevel, face, depthSlice);
		}

		public static void SetRenderTarget(RenderTexture rt)
		{
			Graphics.SetRenderTargetImpl(rt, 0, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderTexture rt, int mipLevel)
		{
			Graphics.SetRenderTargetImpl(rt, mipLevel, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face)
		{
			Graphics.SetRenderTargetImpl(rt, mipLevel, face, 0);
		}

		public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face, int depthSlice)
		{
			Graphics.SetRenderTargetImpl(rt, mipLevel, face, depthSlice);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, 0, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, 0);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face, int depthSlice)
		{
			Graphics.SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face, depthSlice);
		}

		public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer)
		{
			Graphics.SetRenderTargetImpl(colorBuffers, depthBuffer, 0, CubemapFace.Unknown, 0);
		}

		public static void SetRenderTarget(RenderTargetSetup setup)
		{
			Graphics.SetRenderTargetImpl(setup);
		}

		public static void CopyTexture(Texture src, Texture dst)
		{
			Graphics.CopyTexture_Full(src, dst);
		}

		public static void CopyTexture(Texture src, int srcElement, Texture dst, int dstElement)
		{
			Graphics.CopyTexture_Slice_AllMips(src, srcElement, dst, dstElement);
		}

		public static void CopyTexture(Texture src, int srcElement, int srcMip, Texture dst, int dstElement, int dstMip)
		{
			Graphics.CopyTexture_Slice(src, srcElement, srcMip, dst, dstElement, dstMip);
		}

		public static void CopyTexture(Texture src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, Texture dst, int dstElement, int dstMip, int dstX, int dstY)
		{
			Graphics.CopyTexture_Region(src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, dst, dstElement, dstMip, dstX, dstY);
		}

		public static bool ConvertTexture(Texture src, Texture dst)
		{
			return Graphics.ConvertTexture_Full(src, dst);
		}

		public static bool ConvertTexture(Texture src, int srcElement, Texture dst, int dstElement)
		{
			return Graphics.ConvertTexture_Slice(src, srcElement, dst, dstElement);
		}

		private static void DrawMeshImpl(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, Transform probeAnchor, bool useLightProbes)
		{
			Internal_DrawMeshMatrixArguments internal_DrawMeshMatrixArguments = default(Internal_DrawMeshMatrixArguments);
			internal_DrawMeshMatrixArguments.layer = layer;
			internal_DrawMeshMatrixArguments.submeshIndex = submeshIndex;
			internal_DrawMeshMatrixArguments.matrix = matrix;
			internal_DrawMeshMatrixArguments.castShadows = (int)castShadows;
			internal_DrawMeshMatrixArguments.receiveShadows = ((!receiveShadows) ? 0 : 1);
			internal_DrawMeshMatrixArguments.reflectionProbeAnchorInstanceID = ((!(probeAnchor != null)) ? 0 : probeAnchor.GetInstanceID());
			internal_DrawMeshMatrixArguments.useLightProbes = useLightProbes;
			Graphics.Internal_DrawMeshMatrix(ref internal_DrawMeshMatrixArguments, properties, material, mesh, camera);
		}

		private static void DrawTextureImpl(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, Material mat, int pass)
		{
			Internal_DrawTextureArguments internal_DrawTextureArguments = default(Internal_DrawTextureArguments);
			internal_DrawTextureArguments.screenRect = screenRect;
			internal_DrawTextureArguments.sourceRect = sourceRect;
			internal_DrawTextureArguments.leftBorder = leftBorder;
			internal_DrawTextureArguments.rightBorder = rightBorder;
			internal_DrawTextureArguments.topBorder = topBorder;
			internal_DrawTextureArguments.bottomBorder = bottomBorder;
			internal_DrawTextureArguments.color = color;
			internal_DrawTextureArguments.pass = pass;
			internal_DrawTextureArguments.texture = texture;
			internal_DrawTextureArguments.mat = mat;
			Graphics.Internal_DrawTexture(ref internal_DrawTextureArguments);
		}

		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation)
		{
			Graphics.DrawMeshNow(mesh, position, rotation, -1);
		}

		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
		{
			Graphics.Internal_DrawMeshNow1(mesh, materialIndex, position, rotation);
		}

		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix)
		{
			Graphics.DrawMeshNow(mesh, matrix, -1);
		}

		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex)
		{
			Graphics.Internal_DrawMeshNow2(mesh, materialIndex, matrix);
		}

		private static void DrawMeshInstancedImpl(Mesh mesh, int submeshIndex, Material material, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
		{
			if (!SystemInfo.supportsInstancing)
			{
				throw new InvalidOperationException("Instancing is not supported.");
			}
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
			{
				throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
			}
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			if (!material.enableInstancing)
			{
				throw new InvalidOperationException("Material needs to enable instancing for use with DrawMeshInstanced.");
			}
			if (matrices == null)
			{
				throw new ArgumentNullException("matrices");
			}
			if (count < 0 || count > Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length))
			{
				throw new ArgumentOutOfRangeException("count", string.Format("Count must be in the range of 0 to {0}.", Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length)));
			}
			if (count > 0)
			{
				Graphics.Internal_DrawMeshInstanced(mesh, submeshIndex, material, matrices, count, properties, castShadows, receiveShadows, layer, camera);
			}
		}

		private static void DrawMeshInstancedImpl(Mesh mesh, int submeshIndex, Material material, List<Matrix4x4> matrices, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
		{
			if (matrices == null)
			{
				throw new ArgumentNullException("matrices");
			}
			if (matrices.Count > Graphics.kMaxDrawMeshInstanceCount)
			{
				throw new ArgumentOutOfRangeException("matrices", string.Format("Matrix list count must be in the range of 0 to {0}.", Graphics.kMaxDrawMeshInstanceCount));
			}
			Graphics.DrawMeshInstancedImpl(mesh, submeshIndex, material, (Matrix4x4[])Graphics.ExtractArrayFromList(matrices), matrices.Count, properties, castShadows, receiveShadows, layer, camera);
		}

		private static void DrawMeshInstancedIndirectImpl(Mesh mesh, int submeshIndex, Material material, Bounds bounds, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
		{
			if (!SystemInfo.supportsInstancing)
			{
				throw new InvalidOperationException("Instancing is not supported.");
			}
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			if (submeshIndex < 0 || submeshIndex >= mesh.subMeshCount)
			{
				throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
			}
			if (material == null)
			{
				throw new ArgumentNullException("material");
			}
			if (bufferWithArgs == null)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			Graphics.Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, bounds, bufferWithArgs, argsOffset, properties, castShadows, receiveShadows, layer, camera);
		}

		internal static readonly int kMaxDrawMeshInstanceCount = Graphics.Internal_GetMaxDrawMeshInstanceCount();
	}
}
