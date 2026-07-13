using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	public class Painter2D : IDisposable
	{
		internal bool isDetached
		{
			get
			{
				return this.m_DetachedAllocator != null;
			}
		}

		internal Painter2D(MeshGenerationContext ctx)
		{
			this.m_Handle = new SafeHandleAccess(UIPainter2D.Create(false));
			this.m_Ctx = ctx;
			this.m_JobSnapshots = new List<Painter2D.Painter2DJobData>(32);
			this.m_VectorImageToRelease = new List<VectorImage>(16);
			this.m_OnMeshGenerationDelegate = new MeshGenerationCallback(this.OnMeshGeneration);
			this.Reset();
		}

		public Painter2D()
		{
			this.m_Handle = new SafeHandleAccess(UIPainter2D.Create(true));
			this.m_DetachedAllocator = new DetachedAllocator();
			Painter2D.isPainterActive = true;
			this.m_OnMeshGenerationDelegate = new MeshGenerationCallback(this.OnMeshGeneration);
			this.Reset();
		}

		internal void Reset()
		{
			UIPainter2D.Reset(this.m_Handle);
		}

		internal MeshWriteData Allocate(int vertexCount, int indexCount)
		{
			bool isDetached = this.isDetached;
			MeshWriteData meshWriteData;
			if (isDetached)
			{
				meshWriteData = this.m_DetachedAllocator.Alloc(vertexCount, indexCount);
			}
			else
			{
				meshWriteData = this.m_Ctx.Allocate(vertexCount, indexCount, null);
			}
			return meshWriteData;
		}

		public void Clear()
		{
			bool flag = !this.isDetached;
			if (flag)
			{
				Debug.LogError("Clear() cannot be called on a Painter2D associated with a MeshGenerationContext. You should create your own instance of Painter2D instead.");
			}
			else
			{
				this.m_DetachedAllocator.Clear();
				this.Reset();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool disposed = this.m_Disposed;
			if (!disposed)
			{
				if (disposing)
				{
					bool flag = !this.m_Handle.IsNull();
					if (flag)
					{
						UIPainter2D.Destroy(this.m_Handle);
						this.m_Handle = new SafeHandleAccess(IntPtr.Zero);
					}
					bool flag2 = this.m_DetachedAllocator != null;
					if (flag2)
					{
						this.m_DetachedAllocator.Dispose();
					}
					this.m_JobParameters.Dispose();
					bool flag3 = this.m_VectorImageToRelease != null;
					if (flag3)
					{
						foreach (VectorImage vectorImage in this.m_VectorImageToRelease)
						{
							bool flag4 = vectorImage != null;
							if (flag4)
							{
								UIRUtility.Destroy(vectorImage.atlas);
								UIRUtility.Destroy(vectorImage);
							}
						}
						this.m_VectorImageToRelease.Clear();
					}
				}
				this.m_Disposed = true;
			}
		}

		public float lineWidth
		{
			get
			{
				return UIPainter2D.GetLineWidth(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetLineWidth(this.m_Handle, value);
			}
		}

		public Color strokeColor
		{
			get
			{
				return UIPainter2D.GetStrokeColor(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetStrokeColor(this.m_Handle, value);
			}
		}

		public Gradient strokeGradient
		{
			get
			{
				return UIPainter2D.GetStrokeGradient(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetStrokeGradient(this.m_Handle, value);
			}
		}

		internal Matrix4x4 fillTransform
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
			set
			{
				UIPainter2D.SetFillTransform(this.m_Handle, value);
			}
		}

		internal float opacity
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
			set
			{
				UIPainter2D.SetOpacity(this.m_Handle, value);
			}
		}

		public FillGradient fillGradient
		{
			set
			{
				this.m_FillGradient = value;
				UIPainter2D.SetFillGradient(this.m_Handle, value);
			}
		}

		public FillGradient strokeFillGradient
		{
			set
			{
				this.m_StrokeFillGradient = value;
				UIPainter2D.SetStrokeFillGradient(this.m_Handle, value);
			}
		}

		private bool hasStrokeFillGradient
		{
			get
			{
				return UIPainter2D.HasStrokeFillGradient(this.m_Handle);
			}
		}

		private bool hasFillGradient
		{
			get
			{
				return UIPainter2D.HasFillGradient(this.m_Handle);
			}
		}

		private bool hasFillTexture
		{
			get
			{
				return UIPainter2D.HasFillTexture(this.m_Handle);
			}
		}

		public Texture2D fillTexture
		{
			set
			{
				this.m_FillTexture = value;
				UIPainter2D.SetHasFillTexture(this.m_Handle, value != null);
			}
		}

		public Color fillColor
		{
			get
			{
				return UIPainter2D.GetFillColor(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetFillColor(this.m_Handle, value);
			}
		}

		public LineJoin lineJoin
		{
			get
			{
				return UIPainter2D.GetLineJoin(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetLineJoin(this.m_Handle, value);
			}
		}

		public LineCap lineCap
		{
			get
			{
				return UIPainter2D.GetLineCap(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetLineCap(this.m_Handle, value);
			}
		}

		public float miterLimit
		{
			get
			{
				return UIPainter2D.GetMiterLimit(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetMiterLimit(this.m_Handle, value);
			}
		}

		public ReadOnlySpan<float> dashPattern
		{
			set
			{
				UIPainter2D.SetDashPattern(this.m_Handle, value);
			}
		}

		public void SetDashPattern(float dash, float gap)
		{
			UIPainter2D.SetDashGapPattern(this.m_Handle, dash, gap);
		}

		public float dashOffset
		{
			get
			{
				return UIPainter2D.GetDashOffset(this.m_Handle);
			}
			set
			{
				UIPainter2D.SetDashOffset(this.m_Handle, value);
			}
		}

		internal static bool isPainterActive { get; set; }

		private bool ValidateState()
		{
			bool flag = this.isDetached || Painter2D.isPainterActive;
			bool flag2 = !flag;
			if (flag2)
			{
				Debug.LogError("Cannot issue vector graphics commands outside of generateVisualContent callback");
			}
			return flag;
		}

		public void BeginPath()
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.BeginPath(this.m_Handle);
			}
		}

		public void ClosePath()
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.ClosePath(this.m_Handle);
			}
		}

		public void MoveTo(Vector2 pos)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.MoveTo(this.m_Handle, pos);
			}
		}

		public void LineTo(Vector2 pos)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.LineTo(this.m_Handle, pos);
			}
		}

		public void ArcTo(Vector2 p1, Vector2 p2, float radius)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.ArcTo(this.m_Handle, p1, p2, radius);
			}
		}

		public void Arc(Vector2 center, float radius, Angle startAngle, Angle endAngle, ArcDirection direction = ArcDirection.Clockwise)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.Arc(this.m_Handle, center, radius, startAngle.ToRadians(), endAngle.ToRadians(), direction);
			}
		}

		public void BezierCurveTo(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.BezierCurveTo(this.m_Handle, p1, p2, p3);
			}
		}

		public void QuadraticCurveTo(Vector2 p1, Vector2 p2)
		{
			bool flag = !this.ValidateState();
			if (!flag)
			{
				UIPainter2D.QuadraticCurveTo(this.m_Handle, p1, p2);
			}
		}

		public unsafe void Stroke()
		{
			using (Painter2D.s_StrokeMarker.Auto())
			{
				bool flag = !this.ValidateState();
				if (!flag)
				{
					bool isDetached = this.isDetached;
					if (isDetached)
					{
						MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.Stroke(this.m_Handle, true);
						bool flag2 = meshWriteDataInterface.vertexCount == 0;
						if (!flag2)
						{
							MeshWriteData meshWriteData = this.Allocate(meshWriteDataInterface.vertexCount, meshWriteDataInterface.indexCount);
							bool hasStrokeFillGradient = this.hasStrokeFillGradient;
							if (hasStrokeFillGradient)
							{
								this.m_DetachedAllocator.AddGradient(this.m_StrokeFillGradient);
							}
							NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
							NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
							meshWriteData.SetAllVertices(nativeSlice);
							meshWriteData.SetAllIndices(nativeSlice2);
						}
					}
					else
					{
						IntPtr intPtr = IntPtr.Zero;
						bool hasStrokeFillGradient2 = this.hasStrokeFillGradient;
						if (hasStrokeFillGradient2)
						{
							VectorImage vectorImage = ScriptableObject.CreateInstance<VectorImage>();
							this.m_VectorImageToRelease.Add(vectorImage);
							Texture2D texture2D;
							GradientSettings gradientSettings;
							Painter2D.CreateTextureAndGradientSettings(ref this.m_StrokeFillGradient, out texture2D, out gradientSettings);
							vectorImage.atlas = texture2D;
							vectorImage.settings = new GradientSettings[] { gradientSettings };
							intPtr = this.m_Ctx.renderData.parent.renderTree.m_GCHandlePool.GetIntPtr(vectorImage);
						}
						UnsafeMeshGenerationNode unsafeMeshGenerationNode;
						this.m_Ctx.InsertUnsafeMeshGenerationNode(out unsafeMeshGenerationNode);
						int num = UIPainter2D.TakeStrokeSnapshot(this.m_Handle);
						this.m_JobSnapshots.Add(new Painter2D.Painter2DJobData
						{
							node = unsafeMeshGenerationNode,
							snapshotIndex = num,
							vectorImagePtr = intPtr,
							texturePtr = IntPtr.Zero
						});
					}
				}
			}
		}

		private static void SetSolidTextureData(Texture2D targetTexture, Color color, int width, int height)
		{
			NativeArray<Color32> rawTextureData = targetTexture.GetRawTextureData<Color32>();
			int width2 = targetTexture.width;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					rawTextureData[j + i * width2] = color;
				}
			}
		}

		private static void SetGradientTextureData(Texture2D texture, int width, int x, int y, Gradient gradient, bool duplicateOnBorder = false)
		{
			NativeArray<Color32> rawTextureData = texture.GetRawTextureData<Color32>();
			float num = 1f / (float)Math.Max(1, width - 1);
			for (int i = 0; i < width; i++)
			{
				float num2 = (float)i * num;
				Color color = gradient.Evaluate(num2);
				rawTextureData[x + i + y * texture.width] = color;
				if (duplicateOnBorder)
				{
					bool flag = i == 0;
					if (flag)
					{
						rawTextureData[x + width + y * texture.width] = color;
					}
					else
					{
						bool flag2 = i == width - 1;
						if (flag2)
						{
							rawTextureData[x - 1 + y * texture.width] = color;
						}
					}
					rawTextureData[x + i + (y + 1) * texture.width] = color;
					rawTextureData[x + i + (y - 1) * texture.width] = color;
				}
			}
		}

		private static void SetupSolidColor(int width, int height, int x, int y, out GradientSettings gradientSettings)
		{
			gradientSettings = default(GradientSettings);
			gradientSettings.gradientType = GradientType.Linear;
			gradientSettings.addressMode = AddressMode.Clamp;
			gradientSettings.location.x = x;
			gradientSettings.location.y = y;
			gradientSettings.location.width = width;
			gradientSettings.location.height = height;
			gradientSettings.radialFocus = Vector2.zero;
		}

		private static void SetupGradient(ref FillGradient fillGradient, int width, int x, int y, out GradientSettings gradientSettings)
		{
			gradientSettings = default(GradientSettings);
			gradientSettings.gradientType = fillGradient.gradientType;
			gradientSettings.addressMode = fillGradient.addressMode;
			gradientSettings.location.x = x;
			gradientSettings.location.y = y;
			gradientSettings.location.width = width;
			gradientSettings.location.height = 1;
			Vector2 vector = ((fillGradient.radius > 1E-30f) ? ((fillGradient.focus - fillGradient.center) / fillGradient.radius) : Vector2.zero);
			gradientSettings.radialFocus = vector;
		}

		private static void SetupGradientForTexture(int width, int height, int x, int y, out GradientSettings gradientSettings)
		{
			gradientSettings = default(GradientSettings);
			gradientSettings.gradientType = GradientType.Linear;
			gradientSettings.addressMode = AddressMode.Clamp;
			gradientSettings.location.x = x;
			gradientSettings.location.y = y;
			gradientSettings.location.width = width;
			gradientSettings.location.height = height;
			gradientSettings.radialFocus = Vector2.zero;
		}

		private static void CreateTextureAndGradientSettings(ref FillGradient fillGradient, out Texture2D texture, out GradientSettings gradientSettings)
		{
			texture = new Texture2D(64, 1, TextureFormat.RGBA32, false);
			Painter2D.SetGradientTextureData(texture, 64, 0, 0, fillGradient.gradient, false);
			texture.Apply(false, true);
			Painter2D.SetupGradient(ref fillGradient, 64, 0, 0, out gradientSettings);
		}

		public unsafe void Fill(FillRule fillRule = FillRule.NonZero)
		{
			using (Painter2D.s_FillMarker.Auto())
			{
				bool flag = !this.ValidateState();
				if (!flag)
				{
					bool isDetached = this.isDetached;
					if (isDetached)
					{
						MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.Fill(this.m_Handle, fillRule);
						bool flag2 = meshWriteDataInterface.vertexCount == 0;
						if (!flag2)
						{
							MeshWriteData meshWriteData = this.Allocate(meshWriteDataInterface.vertexCount, meshWriteDataInterface.indexCount);
							bool hasFillGradient = this.hasFillGradient;
							if (hasFillGradient)
							{
								this.m_DetachedAllocator.AddGradient(this.m_FillGradient);
							}
							bool hasFillTexture = this.hasFillTexture;
							if (hasFillTexture)
							{
								this.m_DetachedAllocator.AddTexture(this.m_FillTexture);
							}
							NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
							NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
							meshWriteData.SetAllVertices(nativeSlice);
							meshWriteData.SetAllIndices(nativeSlice2);
						}
					}
					else
					{
						IntPtr intPtr = IntPtr.Zero;
						IntPtr intPtr2 = IntPtr.Zero;
						bool hasFillGradient2 = this.hasFillGradient;
						if (hasFillGradient2)
						{
							VectorImage vectorImage = ScriptableObject.CreateInstance<VectorImage>();
							this.m_VectorImageToRelease.Add(vectorImage);
							Texture2D texture2D;
							GradientSettings gradientSettings;
							Painter2D.CreateTextureAndGradientSettings(ref this.m_FillGradient, out texture2D, out gradientSettings);
							vectorImage.atlas = texture2D;
							vectorImage.settings = new GradientSettings[] { gradientSettings };
							intPtr = this.m_Ctx.renderData.parent.renderTree.m_GCHandlePool.GetIntPtr(vectorImage);
						}
						bool hasFillTexture2 = this.hasFillTexture;
						if (hasFillTexture2)
						{
							intPtr2 = this.m_Ctx.renderData.parent.renderTree.m_GCHandlePool.GetIntPtr(this.m_FillTexture);
						}
						UnsafeMeshGenerationNode unsafeMeshGenerationNode;
						this.m_Ctx.InsertUnsafeMeshGenerationNode(out unsafeMeshGenerationNode);
						int num = UIPainter2D.TakeFillSnapshot(this.m_Handle, fillRule);
						this.m_JobSnapshots.Add(new Painter2D.Painter2DJobData
						{
							node = unsafeMeshGenerationNode,
							snapshotIndex = num,
							vectorImagePtr = intPtr,
							texturePtr = intPtr2
						});
					}
				}
			}
		}

		internal void ScheduleJobs(MeshGenerationContext mgc)
		{
			int count = this.m_JobSnapshots.Count;
			bool flag = count == 0;
			if (!flag)
			{
				bool flag2 = this.m_JobParameters.Length < count;
				if (flag2)
				{
					this.m_JobParameters.Dispose();
					this.m_JobParameters = new NativeArray<Painter2D.Painter2DJobData>(count, Painter2D.k_MemoryLabel, NativeArrayOptions.UninitializedMemory);
				}
				for (int i = 0; i < count; i++)
				{
					this.m_JobParameters[i] = this.m_JobSnapshots[i];
				}
				this.m_JobSnapshots.Clear();
				Painter2D.Painter2DJob painter2DJob = new Painter2D.Painter2DJob
				{
					painterHandle = this.m_Handle,
					jobParameters = this.m_JobParameters.Slice<Painter2D.Painter2DJobData>(0, count)
				};
				mgc.GetTempMeshAllocator(out painter2DJob.allocator);
				JobHandle jobHandle = painter2DJob.Schedule(count, 1, default(JobHandle));
				mgc.AddMeshGenerationJob(jobHandle);
				mgc.AddMeshGenerationCallback(this.m_OnMeshGenerationDelegate, null, MeshGenerationCallbackType.Work, true);
			}
		}

		private void OnMeshGeneration(MeshGenerationContext ctx, object data)
		{
			UIPainter2D.ClearSnapshots(this.m_Handle);
		}

		public bool SaveToVectorImage(VectorImage vectorImage)
		{
			bool flag = !this.isDetached;
			bool flag2;
			if (flag)
			{
				Debug.LogError("SaveToVectorImage cannot be called on a Painter2D associated with a MeshGenerationContext. You should create your own instance of Painter2D instead.");
				flag2 = false;
			}
			else
			{
				bool flag3 = vectorImage == null;
				if (flag3)
				{
					throw new NullReferenceException("The provided vectorImage is null");
				}
				List<MeshWriteData> meshes = this.m_DetachedAllocator.meshes;
				int num = 0;
				int num2 = 0;
				foreach (MeshWriteData meshWriteData in meshes)
				{
					num += meshWriteData.m_Vertices.Length;
					num2 += meshWriteData.m_Indices.Length;
				}
				Rect bbox = UIPainter2D.GetBBox(this.m_Handle);
				VectorImageVertex[] array = new VectorImageVertex[num];
				ushort[] array2 = new ushort[num2];
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 1;
				UIRAtlasAllocator uiratlasAllocator = new UIRAtlasAllocator(64, 4096, 1);
				List<RectInt> list = new List<RectInt>();
				List<int> list2 = new List<int>();
				bool flag4 = this.m_DetachedAllocator.HasGradientsOrTextures();
				if (flag4)
				{
					RectInt rectInt;
					uiratlasAllocator.TryAllocate(1 + 2 * num6, 1 + 2 * num6, out rectInt);
					list.Add(rectInt);
				}
				for (int i = 0; i < meshes.Count; i++)
				{
					MeshWriteData meshWriteData2 = meshes[i];
					NativeSlice<Vertex> vertices = meshWriteData2.m_Vertices;
					bool flag5 = false;
					bool flag6 = this.m_DetachedAllocator.HasGradientAtMeshIndex(i);
					if (flag6)
					{
						RectInt rectInt2;
						bool flag7 = !uiratlasAllocator.TryAllocate(64 + 2 * num6, 1 + 2 * num6, out rectInt2);
						if (flag7)
						{
							Debug.LogError("SaveToVectorImage cannot save VectorImage since texture atlas has no space left.");
							return false;
						}
						list.Add(rectInt2);
						list2.Add(i);
						flag5 = true;
					}
					bool flag8 = this.m_DetachedAllocator.HasTextureAtMeshIndex(i);
					if (flag8)
					{
						Texture textureFromMeshIndex = this.m_DetachedAllocator.GetTextureFromMeshIndex(i);
						RectInt rectInt3;
						bool flag9 = !uiratlasAllocator.TryAllocate(textureFromMeshIndex.width, textureFromMeshIndex.height, out rectInt3);
						if (flag9)
						{
							Debug.LogError("SaveToVectorImage cannot save VectorImage since texture atlas has no space left.");
							return false;
						}
						list.Add(rectInt3);
						list2.Add(-1);
						flag5 = true;
					}
					for (int j = 0; j < vertices.Length; j++)
					{
						Vertex vertex = vertices[j];
						Vector3 position = vertex.position;
						position.x -= bbox.x;
						position.y -= bbox.y;
						array[num3++] = new VectorImageVertex
						{
							position = new Vector3(position.x, position.y, Vertex.nearZ),
							tint = vertex.tint,
							uv = vertex.uv,
							flags = vertex.flags,
							settingIndex = (uint)(flag5 ? (list.Count - 1) : 0),
							circle = vertex.circle
						};
					}
					NativeSlice<ushort> indices = meshWriteData2.m_Indices;
					for (int k = 0; k < indices.Length; k++)
					{
						array2[num4++] = (ushort)((int)indices[k] + num5);
					}
					num5 += vertices.Length;
				}
				vectorImage.version = 0;
				vectorImage.vertices = array;
				vectorImage.indices = array2;
				vectorImage.size = bbox.size;
				bool flag10 = list.Count > 0;
				if (flag10)
				{
					RenderTexture renderTexture = new RenderTexture(uiratlasAllocator.physicalWidth, uiratlasAllocator.physicalHeight, 0, RenderTextureFormat.ARGB32);
					List<GradientSettings> list3 = new List<GradientSettings>(list.Count);
					for (int l = 0; l < list.Count; l++)
					{
						RectInt rectInt4 = list[l];
						int num7 = ((l > 0) ? list2[l - 1] : (-1));
						Texture2D texture2D = null;
						bool flag11 = l == 0;
						if (flag11)
						{
							texture2D = new Texture2D(rectInt4.width, rectInt4.height, TextureFormat.RGBA32, false);
							Painter2D.SetSolidTextureData(texture2D, Color.white, rectInt4.width, rectInt4.height);
							texture2D.Apply(false, true);
							GradientSettings gradientSettings;
							Painter2D.SetupSolidColor(rectInt4.width, rectInt4.height, rectInt4.x + num6, rectInt4.y + num6, out gradientSettings);
							list3.Add(gradientSettings);
						}
						else
						{
							bool flag12 = num7 != -1;
							if (flag12)
							{
								FillGradient gradientFromMeshIndex = this.m_DetachedAllocator.GetGradientFromMeshIndex(num7);
								texture2D = new Texture2D(rectInt4.width, rectInt4.height, TextureFormat.RGBA32, false);
								Painter2D.SetGradientTextureData(texture2D, rectInt4.width - 2 * num6, num6, num6, gradientFromMeshIndex.gradient, true);
								texture2D.Apply(false, true);
								GradientSettings gradientSettings2;
								Painter2D.SetupGradient(ref gradientFromMeshIndex, rectInt4.width - 2 * num6, rectInt4.x + num6, rectInt4.y + num6, out gradientSettings2);
								list3.Add(gradientSettings2);
							}
							else
							{
								RenderTexture.active = renderTexture;
								Rect rect = new Rect((float)rectInt4.x, (float)(renderTexture.height - rectInt4.height - rectInt4.y), (float)rectInt4.width, (float)rectInt4.height);
								Texture textureFromMeshIndex2 = this.m_DetachedAllocator.GetTextureFromMeshIndex(l - 1);
								GL.PushMatrix();
								GL.LoadPixelMatrix(0f, (float)renderTexture.width, (float)renderTexture.height, 0f);
								Graphics.DrawTexture(rect, textureFromMeshIndex2);
								GL.PopMatrix();
								RenderTexture.active = null;
								GradientSettings gradientSettings3;
								Painter2D.SetupGradientForTexture(rectInt4.width, rectInt4.height, rectInt4.x, rectInt4.y, out gradientSettings3);
								list3.Add(gradientSettings3);
							}
						}
						bool flag13 = texture2D != null;
						if (flag13)
						{
							Graphics.CopyTexture(texture2D, 0, 0, 0, 0, rectInt4.width, rectInt4.height, renderTexture, 0, 0, rectInt4.x, rectInt4.y);
							UIRUtility.Destroy(texture2D);
						}
					}
					RenderTexture.active = renderTexture;
					Texture2D texture2D2 = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
					texture2D2.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
					texture2D2.Apply();
					RenderTexture.active = null;
					vectorImage.atlas = texture2D2;
					vectorImage.settings = list3.ToArray();
					renderTexture.Release();
				}
				flag2 = true;
			}
			return flag2;
		}

		private static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Renderer.Painter2D", Allocator.Persistent);

		private MeshGenerationContext m_Ctx;

		internal DetachedAllocator m_DetachedAllocator;

		internal SafeHandleAccess m_Handle;

		private FillGradient m_FillGradient;

		private Texture2D m_FillTexture;

		private FillGradient m_StrokeFillGradient;

		private List<Painter2D.Painter2DJobData> m_JobSnapshots = null;

		private List<VectorImage> m_VectorImageToRelease = null;

		private NativeArray<Painter2D.Painter2DJobData> m_JobParameters;

		private bool m_Disposed;

		private static readonly ProfilerMarker s_StrokeMarker = new ProfilerMarker("Painter2D.Stroke");

		private static readonly ProfilerMarker s_FillMarker = new ProfilerMarker("Painter2D.Fill");

		private MeshGenerationCallback m_OnMeshGenerationDelegate;

		private struct Painter2DJobData
		{
			public UnsafeMeshGenerationNode node;

			public int snapshotIndex;

			public IntPtr vectorImagePtr;

			public IntPtr texturePtr;
		}

		private struct Painter2DJob : IJobParallelFor
		{
			public unsafe void Execute(int i)
			{
				Painter2D.Painter2DJobData painter2DJobData = this.jobParameters[i];
				MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.ExecuteSnapshotFromJob(this.painterHandle, painter2DJobData.snapshotIndex);
				NativeSlice<Vertex> nativeSlice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
				NativeSlice<ushort> nativeSlice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
				bool flag = nativeSlice.Length == 0 || nativeSlice2.Length == 0;
				if (!flag)
				{
					NativeSlice<Vertex> nativeSlice3;
					NativeSlice<ushort> nativeSlice4;
					this.allocator.AllocateTempMesh(nativeSlice.Length, nativeSlice2.Length, out nativeSlice3, out nativeSlice4);
					Debug.Assert(nativeSlice3.Length == nativeSlice.Length);
					Debug.Assert(nativeSlice4.Length == nativeSlice2.Length);
					nativeSlice3.CopyFrom(nativeSlice);
					nativeSlice4.CopyFrom(nativeSlice2);
					bool flag2 = painter2DJobData.vectorImagePtr != IntPtr.Zero;
					if (flag2)
					{
						VectorImage vectorImage = (VectorImage)GCHandle.FromIntPtr(painter2DJobData.vectorImagePtr).Target;
						painter2DJobData.node.DrawGradientsInternal(nativeSlice3, nativeSlice4, vectorImage);
					}
					else
					{
						bool flag3 = painter2DJobData.texturePtr != IntPtr.Zero;
						if (flag3)
						{
							Texture texture = GCHandle.FromIntPtr(painter2DJobData.texturePtr).Target as Texture;
							painter2DJobData.node.DrawMesh(nativeSlice3, nativeSlice4, texture);
						}
						else
						{
							painter2DJobData.node.DrawMesh(nativeSlice3, nativeSlice4, null);
						}
					}
				}
			}

			[NativeDisableUnsafePtrRestriction]
			public IntPtr painterHandle;

			[ReadOnly]
			public TempMeshAllocator allocator;

			[ReadOnly]
			public NativeSlice<Painter2D.Painter2DJobData> jobParameters;
		}
	}
}
