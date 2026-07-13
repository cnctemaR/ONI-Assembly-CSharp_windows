using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.LowLevelPhysics2D
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[StructLayout(LayoutKind.Sequential)]
	internal static class PhysicsWorldRenderer
	{
		[RequiredByNativeCode]
		private static void InitializeRendering()
		{
			bool flag = PhysicsWorldRenderer.s_IsInitialized;
			if (!flag)
			{
				PhysicsWorldRenderer.s_DrawerGroups = new PhysicsWorldRenderer.DrawerGroup[128];
				PhysicsWorldRenderer.s_UsingBIRP = GraphicsSettings.currentRenderPipeline == null;
				bool flag2 = PhysicsWorldRenderer.s_UsingBIRP;
				if (flag2)
				{
					Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(PhysicsWorldRenderer.BIRP_RenderAllWorlds));
				}
				else
				{
					RenderPipelineManager.endCameraRendering += PhysicsWorldRenderer.SRP_RenderAllWorlds;
				}
				PhysicsWorldRenderer.s_IsInitialized = true;
			}
		}

		[RequiredByNativeCode]
		private static void ShutdownRendering()
		{
			bool flag = !PhysicsWorldRenderer.s_IsInitialized;
			if (!flag)
			{
				bool flag2 = PhysicsWorldRenderer.s_UsingBIRP;
				if (flag2)
				{
					Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(PhysicsWorldRenderer.BIRP_RenderAllWorlds));
				}
				else
				{
					RenderPipelineManager.endCameraRendering -= PhysicsWorldRenderer.SRP_RenderAllWorlds;
				}
				bool flag3 = PhysicsWorldRenderer.s_DrawerGroups != null;
				if (flag3)
				{
					foreach (PhysicsWorldRenderer.DrawerGroup drawerGroup in PhysicsWorldRenderer.s_DrawerGroups)
					{
						drawerGroup.Dispose();
					}
					PhysicsWorldRenderer.s_DrawerGroups = null;
				}
				bool flag4 = PhysicsWorldRenderer.s_RendererCommandBuffer != null;
				if (flag4)
				{
					PhysicsWorldRenderer.s_RendererCommandBuffer.Dispose();
					PhysicsWorldRenderer.s_RendererCommandBuffer = null;
				}
				PhysicsWorldRenderer.s_IsInitialized = false;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static PhysicsAABB GetCameraViewAABB(Camera camera)
		{
			bool flag = !camera.orthographic;
			PhysicsAABB physicsAABB;
			if (flag)
			{
				physicsAABB = default(PhysicsAABB);
			}
			else
			{
				Vector2 vector = camera.transform.position;
				float orthographicSize = camera.orthographicSize;
				Vector2 vector2 = new Vector2(orthographicSize * camera.aspect, orthographicSize);
				physicsAABB = new PhysicsAABB
				{
					lowerBound = vector - vector2,
					upperBound = vector + vector2
				};
			}
			return physicsAABB;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsCameraTypeValid(Camera camera)
		{
			CameraType cameraType = camera.cameraType;
			return cameraType == CameraType.Game || cameraType == CameraType.SceneView;
		}

		private static void BIRP_RenderAllWorlds(Camera camera)
		{
			bool flag = !PhysicsWorldRenderer.IsCameraTypeValid(camera);
			if (!flag)
			{
				bool flag2 = PhysicsWorld.bypassLowLevel || !PhysicsWorld.isRenderingAllowed;
				if (!flag2)
				{
					if (PhysicsWorldRenderer.s_RendererCommandBuffer == null)
					{
						PhysicsWorldRenderer.s_RendererCommandBuffer = new CommandBuffer
						{
							name = "LowLevelPhysics2D.WorldRenderer"
						};
					}
					PhysicsWorld.DrawAllWorlds(PhysicsWorldRenderer.GetCameraViewAABB(camera));
					Graphics.ExecuteCommandBuffer(PhysicsWorldRenderer.s_RendererCommandBuffer);
					PhysicsWorldRenderer.s_RendererCommandBuffer.Clear();
				}
			}
		}

		private static void SRP_RenderAllWorlds(ScriptableRenderContext context, Camera camera)
		{
			bool flag = !PhysicsWorldRenderer.IsCameraTypeValid(camera);
			if (!flag)
			{
				if (PhysicsWorldRenderer.s_RendererCommandBuffer == null)
				{
					PhysicsWorldRenderer.s_RendererCommandBuffer = new CommandBuffer
					{
						name = "LowLevelPhysics2D.WorldRenderer"
					};
				}
				PhysicsWorld.DrawAllWorlds(PhysicsWorldRenderer.GetCameraViewAABB(camera));
				context.ExecuteCommandBuffer(PhysicsWorldRenderer.s_RendererCommandBuffer);
				context.Submit();
				PhysicsWorldRenderer.s_RendererCommandBuffer.Clear();
			}
		}

		[RequiredByNativeCode]
		private static void SendDrawResultsToCommandBuffer(PhysicsWorld physicsWorld, PhysicsWorld.DrawResults drawResults, PhysicsWorld.TransformPlane transformPlane, float thickness, float fillAlpha, int drawCapacity)
		{
			bool flag = PhysicsWorldRenderer.s_DrawerGroups == null || PhysicsWorldRenderer.s_RendererCommandBuffer == null;
			if (flag)
			{
				throw new NullReferenceException("PhysicsWorldRenderer is not ready.");
			}
			ref PhysicsWorldRenderer.DrawerGroup ptr = ref PhysicsWorldRenderer.s_DrawerGroups[(int)(physicsWorld.m_Index1 - 1)];
			ptr.Draw(PhysicsWorldRenderer.s_RendererCommandBuffer, ref drawResults, thickness, fillAlpha, transformPlane, drawCapacity);
		}

		private static bool s_IsInitialized;

		private static bool s_UsingBIRP;

		private static CommandBuffer s_RendererCommandBuffer;

		private static PhysicsWorldRenderer.DrawerGroup[] s_DrawerGroups;

		private struct DrawerGroup : IDisposable
		{
			public readonly bool IsValid
			{
				get
				{
					return this.m_Drawers != null;
				}
			}

			public void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
			{
				if (this.m_Drawers == null)
				{
					this.m_Drawers = new PhysicsWorldRenderer.DrawerGroup.BaseDrawer[]
					{
						new PhysicsWorldRenderer.DrawerGroup.PolygonGeometryDrawer(),
						new PhysicsWorldRenderer.DrawerGroup.CircleGeometryDrawer(),
						new PhysicsWorldRenderer.DrawerGroup.CapsuleGeometryDrawer(),
						new PhysicsWorldRenderer.DrawerGroup.LineDrawer(),
						new PhysicsWorldRenderer.DrawerGroup.PointDrawer()
					};
				}
				foreach (PhysicsWorldRenderer.DrawerGroup.BaseDrawer baseDrawer in this.m_Drawers)
				{
					baseDrawer.Draw(rendererCommandBuffer, ref drawResults, thickness, fillAlpha, transformPlane, drawCapacity);
				}
			}

			public void Dispose()
			{
				bool flag = !this.IsValid;
				if (!flag)
				{
					foreach (PhysicsWorldRenderer.DrawerGroup.BaseDrawer baseDrawer in this.m_Drawers)
					{
						baseDrawer.Dispose();
					}
					this.m_Drawers = null;
				}
			}

			private PhysicsWorldRenderer.DrawerGroup.BaseDrawer[] m_Drawers;

			private abstract class BaseDrawer : IDisposable
			{
				protected Mesh GetMesh()
				{
					bool flag = this.m_Mesh == null;
					if (flag)
					{
						this.m_Mesh = new Mesh
						{
							vertices = new Vector3[]
							{
								new Vector3(-1.1f, -1.1f, 0f),
								new Vector3(-1.1f, 1.1f, 0f),
								new Vector3(1.1f, 1.1f, 0f),
								new Vector3(1.1f, -1.1f, 0f)
							},
							normals = new Vector3[]
							{
								-Vector3.forward,
								-Vector3.forward,
								-Vector3.forward,
								-Vector3.forward
							},
							uv = new Vector2[]
							{
								Vector2.zero,
								new Vector2(0f, 1f),
								Vector2.one,
								new Vector2(1f, 0f)
							},
							triangles = new int[] { 0, 1, 2, 2, 3, 0 }
						};
					}
					return this.m_Mesh;
				}

				public void Dispose()
				{
					bool disposed = this.m_Disposed;
					if (!disposed)
					{
						GraphicsBuffer graphicsBuffer = this.m_GraphicsBuffer;
						if (graphicsBuffer != null)
						{
							graphicsBuffer.Dispose();
						}
						this.m_GraphicsBuffer = null;
						this.m_CommandData = null;
						ComputeBuffer elementBuffer = this.m_ElementBuffer;
						if (elementBuffer != null)
						{
							elementBuffer.Dispose();
						}
						this.m_ElementBuffer = null;
						this.m_ShaderMaterialPropertyBlock = null;
						bool flag = this.m_Mesh != null;
						if (flag)
						{
							Object.DestroyImmediate(this.m_Mesh);
							this.m_Mesh = null;
						}
						bool flag2 = this.m_ShaderMaterial != null;
						if (flag2)
						{
							Object.Destroy(this.m_ShaderMaterial);
							this.m_ShaderMaterial = null;
						}
						this.m_Disposed = true;
					}
				}

				public abstract void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity);

				private bool m_Disposed;

				protected Mesh m_Mesh = null;

				protected GraphicsBuffer m_GraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, 20);

				protected GraphicsBuffer.IndirectDrawIndexedArgs[] m_CommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];

				protected ComputeBuffer m_ElementBuffer;

				protected Material m_ShaderMaterial;

				protected MaterialPropertyBlock m_ShaderMaterialPropertyBlock;

				protected readonly Bounds m_CullingBounds = new Bounds(Vector3.zero, 100000f * Vector3.one);

				protected readonly int m_ElementBufferShaderProperty = Shader.PropertyToID("element_buffer");

				protected readonly int m_TransformPlaneShaderProperty = Shader.PropertyToID("transform_plane");

				protected readonly int m_ThicknessShaderProperty = Shader.PropertyToID("thickness");

				protected readonly int m_FillAlphaShaderProperty = Shader.PropertyToID("fillAlpha");
			}

			private sealed class PolygonGeometryDrawer : PhysicsWorldRenderer.DrawerGroup.BaseDrawer
			{
				public PolygonGeometryDrawer()
				{
					this.m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_PolygonGeometry.mat", "Hidden/Physics2D/SDF_PolygonGeometry");
					this.m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
				}

				public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
				{
					NativeArray<PhysicsWorld.DrawResults.PolygonGeometryElement> polygonGeometryArray = drawResults.polygonGeometryArray;
					int length = polygonGeometryArray.Length;
					bool flag = length == 0;
					if (!flag)
					{
						this.m_CommandData[0].indexCountPerInstance = base.GetMesh().GetIndexCount(0);
						this.m_CommandData[0].instanceCount = (uint)length;
						this.m_GraphicsBuffer.SetData(this.m_CommandData);
						bool flag2 = this.m_ElementBuffer == null;
						if (flag2)
						{
							this.m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.PolygonGeometryElement.Size());
						}
						else
						{
							bool flag3 = this.m_ElementBuffer.count < length;
							if (flag3)
							{
								this.m_ElementBuffer.Release();
								this.m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.PolygonGeometryElement.Size());
							}
						}
						this.m_ElementBuffer.SetData<PhysicsWorld.DrawResults.PolygonGeometryElement>(polygonGeometryArray);
						this.m_ShaderMaterialPropertyBlock.SetBuffer(this.m_ElementBufferShaderProperty, this.m_ElementBuffer);
						this.m_ShaderMaterialPropertyBlock.SetInteger(this.m_TransformPlaneShaderProperty, (int)transformPlane);
						this.m_ShaderMaterialPropertyBlock.SetFloat(this.m_ThicknessShaderProperty, thickness);
						this.m_ShaderMaterialPropertyBlock.SetFloat(this.m_FillAlphaShaderProperty, fillAlpha);
						rendererCommandBuffer.DrawMeshInstancedIndirect(base.GetMesh(), 0, this.m_ShaderMaterial, 0, this.m_GraphicsBuffer, 0, this.m_ShaderMaterialPropertyBlock);
					}
				}
			}

			private sealed class CircleGeometryDrawer : PhysicsWorldRenderer.DrawerGroup.BaseDrawer
			{
				public CircleGeometryDrawer()
				{
					this.m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_CircleGeometry.mat", "Hidden/Physics2D/SDF_CircleGeometry");
					this.m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
				}

				public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
				{
					NativeArray<PhysicsWorld.DrawResults.CircleGeometryElement> circleGeometryArray = drawResults.circleGeometryArray;
					int length = circleGeometryArray.Length;
					bool flag = length == 0;
					if (!flag)
					{
						this.m_CommandData[0].indexCountPerInstance = base.GetMesh().GetIndexCount(0);
						this.m_CommandData[0].instanceCount = (uint)length;
						this.m_GraphicsBuffer.SetData(this.m_CommandData);
						bool flag2 = this.m_ElementBuffer == null;
						if (flag2)
						{
							this.m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.CircleGeometryElement.Size());
						}
						else
						{
							bool flag3 = this.m_ElementBuffer.count < length;
							if (flag3)
							{
								this.m_ElementBuffer.Release();
								this.m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.CircleGeometryElement.Size());
							}
						}
						this.m_ElementBuffer.SetData<PhysicsWorld.DrawResults.CircleGeometryElement>(circleGeometryArray);
						this.m_ShaderMaterialPropertyBlock.SetBuffer(this.m_ElementBufferShaderProperty, this.m_ElementBuffer);
						this.m_ShaderMaterialPropertyBlock.SetInteger(this.m_TransformPlaneShaderProperty, (int)transformPlane);
						this.m_ShaderMaterialPropertyBlock.SetFloat(this.m_ThicknessShaderProperty, thickness);
						this.m_ShaderMaterialPropertyBlock.SetFloat(this.m_FillAlphaShaderProperty, fillAlpha);
						rendererCommandBuffer.DrawMeshInstancedIndirect(base.GetMesh(), 0, this.m_ShaderMaterial, 0, this.m_GraphicsBuffer, 0, this.m_ShaderMaterialPropertyBlock);
					}
				}
			}

			private sealed class CapsuleGeometryDrawer : PhysicsWorldRenderer.DrawerGroup.BaseDrawer
			{
				public CapsuleGeometryDrawer()
				{
					this.m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_CapsuleGeometry.mat", "Hidden/Physics2D/SDF_CapsuleGeometry");
					this.m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
				}

				public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
				{
					NativeArray<PhysicsWorld.DrawResults.CapsuleGeometryElement> capsuleGeometryArray = drawResults.capsuleGeometryArray;
					int length = capsuleGeometryArray.Length;
					bool flag = length == 0;
					if (!flag)
					{
						this.m_CommandData[0].indexCountPerInstance = base.GetMesh().GetIndexCount(0);
						this.m_CommandData[0].instanceCount = (uint)length;
						this.m_GraphicsBuffer.SetData(this.m_CommandData);
						bool flag2 = this.m_ElementBuffer == null;
						if (flag2)
						{
							this.m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.CapsuleGeometryElement.Size());
						}
						else
						{
							bool flag3 = this.m_ElementBuffer.count < length;
							if (flag3)
							{
								this.m_ElementBuffer.Release();
								this.m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.CapsuleGeometryElement.Size());
							}
						}
						this.m_ElementBuffer.SetData<PhysicsWorld.DrawResults.CapsuleGeometryElement>(capsuleGeometryArray);
						this.m_ShaderMaterialPropertyBlock.SetBuffer(this.m_ElementBufferShaderProperty, this.m_ElementBuffer);
						this.m_ShaderMaterialPropertyBlock.SetInteger(this.m_TransformPlaneShaderProperty, (int)transformPlane);
						this.m_ShaderMaterialPropertyBlock.SetFloat(this.m_ThicknessShaderProperty, thickness);
						this.m_ShaderMaterialPropertyBlock.SetFloat(this.m_FillAlphaShaderProperty, fillAlpha);
						rendererCommandBuffer.DrawMeshInstancedIndirect(base.GetMesh(), 0, this.m_ShaderMaterial, 0, this.m_GraphicsBuffer, 0, this.m_ShaderMaterialPropertyBlock);
					}
				}
			}

			private sealed class LineDrawer : PhysicsWorldRenderer.DrawerGroup.BaseDrawer
			{
				public LineDrawer()
				{
					this.m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_Line.mat", "Hidden/Physics2D/SDF_Line");
					this.m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
				}

				public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
				{
					NativeArray<PhysicsWorld.DrawResults.LineElement> lineArray = drawResults.lineArray;
					int length = lineArray.Length;
					bool flag = length == 0;
					if (!flag)
					{
						this.m_CommandData[0].indexCountPerInstance = base.GetMesh().GetIndexCount(0);
						this.m_CommandData[0].instanceCount = (uint)length;
						this.m_GraphicsBuffer.SetData(this.m_CommandData);
						bool flag2 = this.m_ElementBuffer == null;
						if (flag2)
						{
							this.m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.LineElement.Size());
						}
						else
						{
							bool flag3 = this.m_ElementBuffer.count < length;
							if (flag3)
							{
								this.m_ElementBuffer.Release();
								this.m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.LineElement.Size());
							}
						}
						this.m_ElementBuffer.SetData<PhysicsWorld.DrawResults.LineElement>(lineArray);
						this.m_ShaderMaterialPropertyBlock.SetBuffer(this.m_ElementBufferShaderProperty, this.m_ElementBuffer);
						this.m_ShaderMaterialPropertyBlock.SetInteger(this.m_TransformPlaneShaderProperty, (int)transformPlane);
						this.m_ShaderMaterialPropertyBlock.SetFloat(this.m_ThicknessShaderProperty, thickness);
						rendererCommandBuffer.DrawMeshInstancedIndirect(base.GetMesh(), 0, this.m_ShaderMaterial, 0, this.m_GraphicsBuffer, 0, this.m_ShaderMaterialPropertyBlock);
					}
				}
			}

			private sealed class PointDrawer : PhysicsWorldRenderer.DrawerGroup.BaseDrawer
			{
				public PointDrawer()
				{
					this.m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_Point.mat", "Hidden/Physics2D/SDF_Point");
					this.m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
				}

				public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
				{
					NativeArray<PhysicsWorld.DrawResults.PointElement> pointArray = drawResults.pointArray;
					int length = pointArray.Length;
					bool flag = length == 0;
					if (!flag)
					{
						this.m_CommandData[0].indexCountPerInstance = base.GetMesh().GetIndexCount(0);
						this.m_CommandData[0].instanceCount = (uint)length;
						this.m_GraphicsBuffer.SetData(this.m_CommandData);
						bool flag2 = this.m_ElementBuffer == null;
						if (flag2)
						{
							this.m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.PointElement.Size());
						}
						else
						{
							bool flag3 = this.m_ElementBuffer.count < length;
							if (flag3)
							{
								this.m_ElementBuffer.Release();
								this.m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.PointElement.Size());
							}
						}
						this.m_ElementBuffer.SetData<PhysicsWorld.DrawResults.PointElement>(pointArray);
						this.m_ShaderMaterialPropertyBlock.SetBuffer(this.m_ElementBufferShaderProperty, this.m_ElementBuffer);
						this.m_ShaderMaterialPropertyBlock.SetInteger(this.m_TransformPlaneShaderProperty, (int)transformPlane);
						this.m_ShaderMaterialPropertyBlock.SetFloat(this.m_ThicknessShaderProperty, thickness);
						rendererCommandBuffer.DrawMeshInstancedIndirect(base.GetMesh(), 0, this.m_ShaderMaterial, 0, this.m_GraphicsBuffer, 0, this.m_ShaderMaterialPropertyBlock);
					}
				}
			}
		}
	}
}
