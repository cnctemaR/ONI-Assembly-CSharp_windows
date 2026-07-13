using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	public class MeshGenerationContext
	{
		public VisualElement visualElement { get; private set; }

		internal RenderData renderData { get; private set; }

		public Painter2D painter2D
		{
			get
			{
				bool disposed = this.disposed;
				Painter2D painter2D;
				if (disposed)
				{
					Debug.LogError("Accessing painter2D on disposed MeshGenerationContext");
					painter2D = null;
				}
				else
				{
					bool flag = this.m_Painter2D == null;
					if (flag)
					{
						this.m_Painter2D = new Painter2D(this);
					}
					painter2D = this.m_Painter2D;
				}
				return painter2D;
			}
		}

		internal bool hasPainter2D
		{
			get
			{
				return this.m_Painter2D != null;
			}
		}

		internal IMeshGenerator meshGenerator { get; set; }

		internal EntryRecorder entryRecorder { get; private set; }

		internal Entry parentEntry { get; private set; }

		internal MeshGenerationContext(MeshWriteDataPool meshWriteDataPool, EntryRecorder entryRecorder, TempMeshAllocatorImpl allocator, MeshGenerationDeferrer meshGenerationDeferrer, MeshGenerationNodeManager meshGenerationNodeManager)
		{
			this.m_MeshWriteDataPool = meshWriteDataPool;
			this.m_Allocator = allocator;
			this.m_MeshGenerationDeferrer = meshGenerationDeferrer;
			this.m_MeshGenerationNodeManager = meshGenerationNodeManager;
			this.entryRecorder = entryRecorder;
			this.meshGenerator = new MeshGenerator(this);
		}

		public void AllocateTempMesh(int vertexCount, int indexCount, out NativeSlice<Vertex> vertices, out NativeSlice<ushort> indices)
		{
			this.m_Allocator.AllocateTempMesh(vertexCount, indexCount, out vertices, out indices);
		}

		public MeshWriteData Allocate(int vertexCount, int indexCount, Texture texture = null)
		{
			MeshWriteData meshWriteData2;
			using (MeshGenerationContext.k_AllocateMarker.Auto())
			{
				MeshWriteData meshWriteData = this.m_MeshWriteDataPool.Get();
				bool flag = vertexCount == 0 || indexCount == 0;
				if (flag)
				{
					meshWriteData.Reset(default(NativeSlice<Vertex>), default(NativeSlice<ushort>));
					meshWriteData2 = meshWriteData;
				}
				else
				{
					bool flag2 = (long)vertexCount > (long)((ulong)UIRenderDevice.maxVerticesPerPage);
					if (flag2)
					{
						throw new ArgumentOutOfRangeException("vertexCount", string.Format("Attempting to allocate {0} vertices which exceeds the limit of {1}.", vertexCount, UIRenderDevice.maxVerticesPerPage));
					}
					NativeSlice<Vertex> nativeSlice;
					NativeSlice<ushort> nativeSlice2;
					this.m_Allocator.AllocateTempMesh(vertexCount, indexCount, out nativeSlice, out nativeSlice2);
					Debug.Assert(nativeSlice.Length == vertexCount);
					Debug.Assert(nativeSlice2.Length == indexCount);
					meshWriteData.Reset(nativeSlice, nativeSlice2);
					this.entryRecorder.DrawMesh(this.parentEntry, meshWriteData.m_Vertices, meshWriteData.m_Indices, texture, TextureOptions.None);
					meshWriteData2 = meshWriteData;
				}
			}
			return meshWriteData2;
		}

		public void DrawMesh(NativeSlice<Vertex> vertices, NativeSlice<ushort> indices, Texture texture = null)
		{
			bool flag = vertices.Length == 0 || indices.Length == 0;
			if (!flag)
			{
				this.entryRecorder.DrawMesh(this.parentEntry, vertices, indices, texture, TextureOptions.None);
			}
		}

		public void DrawVectorImage(VectorImage vectorImage, Vector2 offset, Angle rotationAngle, Vector2 scale)
		{
			using (MeshGenerationContext.k_DrawVectorImageMarker.Auto())
			{
				this.meshGenerator.DrawVectorImage(vectorImage, offset, rotationAngle, scale);
			}
		}

		public void DrawText(string text, Vector2 pos, float fontSize, Color color, FontAsset font = null)
		{
			bool flag = font == null;
			if (flag)
			{
				font = TextUtilities.GetFontAsset(this.visualElement);
			}
			this.meshGenerator.DrawText(text, pos, fontSize, color, font);
		}

		public void GetTempMeshAllocator(out TempMeshAllocator allocator)
		{
			this.m_Allocator.CreateNativeHandle(out allocator);
		}

		public void InsertMeshGenerationNode(out MeshGenerationNode node)
		{
			Entry entry = this.entryRecorder.InsertPlaceholder(this.parentEntry);
			this.m_MeshGenerationNodeManager.CreateNode(entry, out node);
		}

		internal void InsertUnsafeMeshGenerationNode(out UnsafeMeshGenerationNode node)
		{
			Entry entry = this.entryRecorder.InsertPlaceholder(this.parentEntry);
			this.m_MeshGenerationNodeManager.CreateUnsafeNode(entry, out node);
		}

		public void AddMeshGenerationJob(JobHandle jobHandle)
		{
			this.m_MeshGenerationDeferrer.AddMeshGenerationJob(jobHandle);
		}

		internal void AddMeshGenerationCallback(MeshGenerationCallback callback, object userData, MeshGenerationCallbackType callbackType, bool isJobDependent)
		{
			this.m_MeshGenerationDeferrer.AddMeshGenerationCallback(callback, userData, callbackType, isJobDependent);
		}

		internal void Begin(Entry parentEntry, VisualElement ve, RenderData renderData)
		{
			bool flag = this.visualElement != null;
			if (flag)
			{
				throw new InvalidOperationException("Begin can only be called when there is no target set. Did you forget to call End?");
			}
			bool flag2 = parentEntry == null;
			if (flag2)
			{
				throw new ArgumentException("The state of the provided MeshGenerationNode is invalid (entry is null).");
			}
			bool flag3 = ve == null;
			if (flag3)
			{
				throw new ArgumentException("ve");
			}
			this.parentEntry = parentEntry;
			this.visualElement = ve;
			this.renderData = renderData;
			this.meshGenerator.currentElement = ve;
		}

		internal void End()
		{
			bool flag = this.visualElement == null;
			if (flag)
			{
				throw new InvalidOperationException("End can only be called after a successful call to Begin.");
			}
			this.meshGenerator.currentElement = null;
			this.visualElement = null;
			this.parentEntry = null;
			Painter2D painter2D = this.m_Painter2D;
			if (painter2D != null)
			{
				painter2D.Reset();
			}
		}

		internal bool disposed { get; private set; }

		internal void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool disposed = this.disposed;
			if (!disposed)
			{
				if (disposing)
				{
					Painter2D painter2D = this.m_Painter2D;
					if (painter2D != null)
					{
						painter2D.Dispose();
					}
					this.m_Painter2D = null;
					this.m_MeshWriteDataPool = null;
					this.entryRecorder = null;
					MeshGenerator meshGenerator = this.meshGenerator as MeshGenerator;
					if (meshGenerator != null)
					{
						meshGenerator.Dispose();
					}
					this.meshGenerator = null;
					this.m_Allocator = null;
					this.m_MeshGenerationDeferrer = null;
					this.m_MeshGenerationNodeManager = null;
				}
				this.disposed = true;
			}
		}

		private Painter2D m_Painter2D;

		private MeshWriteDataPool m_MeshWriteDataPool;

		private TempMeshAllocatorImpl m_Allocator;

		private MeshGenerationDeferrer m_MeshGenerationDeferrer;

		private MeshGenerationNodeManager m_MeshGenerationNodeManager;

		private static readonly ProfilerMarker k_AllocateMarker = new ProfilerMarker("UIR.MeshGenerationContext.Allocate");

		private static readonly ProfilerMarker k_DrawVectorImageMarker = new ProfilerMarker("UIR.MeshGenerationContext.DrawVectorImage");

		[Flags]
		internal enum MeshFlags
		{
			None = 0,
			SkipDynamicAtlas = 2,
			IsUsingVectorImageGradients = 4,
			SliceTiled = 8
		}
	}
}
