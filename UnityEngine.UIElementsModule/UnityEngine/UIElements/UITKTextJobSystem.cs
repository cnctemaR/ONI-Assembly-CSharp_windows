using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.Pool;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class UITKTextJobSystem
	{
		public UITKTextJobSystem()
		{
			this.m_PrepareTextJobifiedCallback = new MeshGenerationCallback(this.PrepareTextJobified);
			this.m_GenerateTextJobifiedCallback = new MeshGenerationCallback(this.GenerateTextJobified);
			this.m_AddDrawEntriesCallback = new MeshGenerationCallback(this.AddDrawEntries);
		}

		private static void OnGetManagedJob(UITKTextJobSystem.ManagedJobData managedJobData)
		{
			managedJobData.vertices = null;
			managedJobData.indices = null;
			managedJobData.materials = null;
			managedJobData.renderModes = null;
			managedJobData.prepareSuccess = false;
		}

		internal void GenerateText(MeshGenerationContext mgc, TextElement textElement)
		{
			MeshGenerationNode meshGenerationNode;
			mgc.InsertMeshGenerationNode(out meshGenerationNode);
			UITKTextJobSystem.ManagedJobData managedJobData = UITKTextJobSystem.s_JobDataPool.Get();
			managedJobData.visualElement = textElement;
			managedJobData.node = meshGenerationNode;
			this.textJobDatas.Add(managedJobData);
			bool flag = this.hasPendingTextWork;
			if (!flag)
			{
				this.hasPendingTextWork = true;
				this.textJobDatasHandle = GCHandle.Alloc(this.textJobDatas);
				mgc.AddMeshGenerationCallback(this.m_PrepareTextJobifiedCallback, null, MeshGenerationCallbackType.WorkThenFork, false);
			}
		}

		internal void PrepareTextJobified(MeshGenerationContext mgc, object _)
		{
			TextHandle.InitThreadArrays();
			PanelTextSettings.InitializeDefaultPanelTextSettingsIfNull();
			TextHandle.UpdateCurrentFrame();
			this.hasPendingTextWork = false;
			UITKTextJobSystem.PrepareTextJobData prepareTextJobData = new UITKTextJobSystem.PrepareTextJobData
			{
				managedJobDataHandle = this.textJobDatasHandle
			};
			TextGenerator.IsExecutingJob = true;
			JobHandle jobHandle = prepareTextJobData.Schedule(this.textJobDatas.Count, 1, default(JobHandle));
			mgc.AddMeshGenerationJob(jobHandle);
			mgc.AddMeshGenerationCallback(this.m_GenerateTextJobifiedCallback, null, MeshGenerationCallbackType.Work, true);
		}

		private void GenerateTextJobified(MeshGenerationContext mgc, object _)
		{
			TextGenerator.IsExecutingJob = false;
			foreach (UITKTextJobSystem.ManagedJobData managedJobData in this.textJobDatas)
			{
				TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(managedJobData.visualElement);
				if (textSettingsFrom != null)
				{
					UnicodeLineBreakingRules lineBreakingRules = textSettingsFrom.lineBreakingRules;
					if (lineBreakingRules != null)
					{
						lineBreakingRules.LoadLineBreakingRules();
					}
				}
				List<FontAsset> list = ((textSettingsFrom != null) ? textSettingsFrom.fallbackOSFontAssets : null);
				bool prepareSuccess = managedJobData.prepareSuccess;
				if (!prepareSuccess)
				{
					managedJobData.visualElement.uitkTextHandle.ConvertUssToTextGenerationSettings(true, null);
					managedJobData.visualElement.uitkTextHandle.PrepareFontAsset();
				}
			}
			FontAsset.UpdateFontAssetsInUpdateQueue();
			TempMeshAllocator tempMeshAllocator;
			mgc.GetTempMeshAllocator(out tempMeshAllocator);
			UITKTextJobSystem.GenerateTextJobData generateTextJobData = new UITKTextJobSystem.GenerateTextJobData
			{
				managedJobDataHandle = this.textJobDatasHandle,
				alloc = tempMeshAllocator
			};
			TextHandle.UpdateCurrentFrame();
			TextGenerator.IsExecutingJob = true;
			JobHandle jobHandle = generateTextJobData.Schedule(this.textJobDatas.Count, 1, default(JobHandle));
			mgc.AddMeshGenerationJob(jobHandle);
			mgc.AddMeshGenerationCallback(this.m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, true);
		}

		private static void ConvertMeshInfoToUIRVertex(MeshInfo[] meshInfos, TempMeshAllocator alloc, TextElement visualElement, ref List<Material> materials, ref List<NativeSlice<Vertex>> verticesArray, ref List<NativeSlice<ushort>> indicesArray, ref List<GlyphRenderMode> renderModes)
		{
			ObjectPool<List<Material>> objectPool = UITKTextJobSystem.s_MaterialsPool;
			lock (objectPool)
			{
				materials = UITKTextJobSystem.s_MaterialsPool.Get();
				verticesArray = UITKTextJobSystem.s_VerticesPool.Get();
				indicesArray = UITKTextJobSystem.s_IndicesPool.Get();
				renderModes = UITKTextJobSystem.s_RenderModesPool.Get();
			}
			Vector2 min = visualElement.contentRect.min;
			float num = 1f / visualElement.scaledPixelsPerPoint;
			bool hasMultipleColors = visualElement.uitkTextHandle.textInfo.hasMultipleColors;
			bool flag2 = hasMultipleColors;
			if (flag2)
			{
				visualElement.renderData.flags |= RenderDataFlags.IsIgnoringDynamicColorHint;
			}
			else
			{
				visualElement.renderData.flags &= ~RenderDataFlags.IsIgnoringDynamicColorHint;
			}
			foreach (MeshInfo meshInfo in meshInfos)
			{
				Debug.Assert((meshInfo.vertexCount & 3) == 0);
				int num2 = (int)((ulong)UIRenderDevice.maxVerticesPerPage & 18446744073709551612UL);
				int num3 = meshInfo.vertexCount;
				int num4 = 0;
				do
				{
					int num5 = Mathf.Min(num3, num2);
					int num6 = num5 >> 2;
					int num7 = num6 * 6;
					materials.Add(meshInfo.material);
					renderModes.Add(meshInfo.glyphRenderMode);
					bool flag3 = meshInfo.glyphRenderMode != GlyphRenderMode.SMOOTH && meshInfo.glyphRenderMode != GlyphRenderMode.COLOR;
					bool flag4 = meshInfo.applySDF && !hasMultipleColors && (RenderEvents.NeedsColorID(visualElement) || (flag3 && RenderEvents.NeedsTextCoreSettings(visualElement)));
					NativeSlice<Vertex> nativeSlice;
					NativeSlice<ushort> nativeSlice2;
					alloc.AllocateTempMesh(num5, num7, out nativeSlice, out nativeSlice2);
					int j = 0;
					int num8 = 0;
					while (j < num5)
					{
						nativeSlice[j] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num4], min, num, flag4, false);
						nativeSlice[j + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num4 + 1], min, num, flag4, false);
						nativeSlice[j + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num4 + 2], min, num, flag4, false);
						nativeSlice[j + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref meshInfo.vertexData[num4 + 3], min, num, flag4, false);
						nativeSlice2[num8] = (ushort)j;
						nativeSlice2[num8 + 1] = (ushort)(j + 1);
						nativeSlice2[num8 + 2] = (ushort)(j + 2);
						nativeSlice2[num8 + 3] = (ushort)(j + 2);
						nativeSlice2[num8 + 4] = (ushort)(j + 3);
						nativeSlice2[num8 + 5] = (ushort)j;
						j += 4;
						num4 += 4;
						num8 += 6;
					}
					verticesArray.Add(nativeSlice);
					indicesArray.Add(nativeSlice2);
					num3 -= num5;
				}
				while (num3 > 0);
				Debug.Assert(num3 == 0);
			}
		}

		private void AddDrawEntries(MeshGenerationContext mgc, object _)
		{
			TextGenerator.IsExecutingJob = false;
			foreach (UITKTextJobSystem.ManagedJobData managedJobData in this.textJobDatas)
			{
				TextElement visualElement = managedJobData.visualElement;
				mgc.Begin(managedJobData.node.GetParentEntry(), visualElement, visualElement.nestedRenderData ?? visualElement.renderData);
				visualElement.uitkTextHandle.HandleLinkAndATagCallbacks();
				Action<TextElement.GlyphsEnumerable> postProcessTextVertices = visualElement.PostProcessTextVertices;
				if (postProcessTextVertices != null)
				{
					postProcessTextVertices(new TextElement.GlyphsEnumerable(visualElement, managedJobData.vertices));
				}
				mgc.meshGenerator.DrawText(managedJobData.vertices, managedJobData.indices, managedJobData.materials, managedJobData.renderModes);
				managedJobData.visualElement.OnGenerateTextOver(mgc);
				mgc.End();
				managedJobData.Release();
			}
			this.textJobDatas.Clear();
			this.textJobDatasHandle.Free();
		}

		private static readonly ProfilerMarker k_ExecuteMarker = new ProfilerMarker("TextJob.GenerateText");

		private static readonly ProfilerMarker k_UpdateMainThreadMarker = new ProfilerMarker("TextJob.UpdateMainThread");

		private static readonly ProfilerMarker k_PrepareMainThreadMarker = new ProfilerMarker("TextJob.PrepareMainThread");

		private static readonly ProfilerMarker k_PrepareJobifiedMarker = new ProfilerMarker("TextJob.PrepareJobified");

		private GCHandle textJobDatasHandle;

		private List<UITKTextJobSystem.ManagedJobData> textJobDatas = new List<UITKTextJobSystem.ManagedJobData>();

		private bool hasPendingTextWork;

		private static ObjectPool<UITKTextJobSystem.ManagedJobData> s_JobDataPool = new ObjectPool<UITKTextJobSystem.ManagedJobData>(() => new UITKTextJobSystem.ManagedJobData(), new Action<UITKTextJobSystem.ManagedJobData>(UITKTextJobSystem.OnGetManagedJob), delegate(UITKTextJobSystem.ManagedJobData inst)
		{
			inst.visualElement = null;
		}, null, false, 10, 10000);

		private static ObjectPool<List<Material>> s_MaterialsPool = new ObjectPool<List<Material>>(() => new List<Material>(), null, delegate(List<Material> list)
		{
			list.Clear();
		}, null, false, 10, 10000);

		private static ObjectPool<List<GlyphRenderMode>> s_RenderModesPool = new ObjectPool<List<GlyphRenderMode>>(() => new List<GlyphRenderMode>(), null, delegate(List<GlyphRenderMode> list)
		{
			list.Clear();
		}, null, false, 10, 10000);

		private static ObjectPool<List<NativeSlice<Vertex>>> s_VerticesPool = new ObjectPool<List<NativeSlice<Vertex>>>(() => new List<NativeSlice<Vertex>>(), null, delegate(List<NativeSlice<Vertex>> list)
		{
			list.Clear();
		}, null, false, 10, 10000);

		private static ObjectPool<List<NativeSlice<ushort>>> s_IndicesPool = new ObjectPool<List<NativeSlice<ushort>>>(() => new List<NativeSlice<ushort>>(), null, delegate(List<NativeSlice<ushort>> list)
		{
			list.Clear();
		}, null, false, 10, 10000);

		internal MeshGenerationCallback m_PrepareTextJobifiedCallback;

		internal MeshGenerationCallback m_GenerateTextJobifiedCallback;

		internal MeshGenerationCallback m_AddDrawEntriesCallback;

		private class ManagedJobData
		{
			public void Release()
			{
				bool flag = this.materials != null;
				if (flag)
				{
					UITKTextJobSystem.s_MaterialsPool.Release(this.materials);
					UITKTextJobSystem.s_VerticesPool.Release(this.vertices);
					UITKTextJobSystem.s_IndicesPool.Release(this.indices);
					UITKTextJobSystem.s_RenderModesPool.Release(this.renderModes);
				}
				UITKTextJobSystem.s_JobDataPool.Release(this);
			}

			public TextElement visualElement;

			public MeshGenerationNode node;

			public List<Material> materials;

			public List<GlyphRenderMode> renderModes;

			public List<NativeSlice<Vertex>> vertices;

			public List<NativeSlice<ushort>> indices;

			public bool prepareSuccess;
		}

		private struct PrepareTextJobData : IJobParallelFor
		{
			public void Execute(int index)
			{
				List<UITKTextJobSystem.ManagedJobData> list = (List<UITKTextJobSystem.ManagedJobData>)this.managedJobDataHandle.Target;
				UITKTextJobSystem.ManagedJobData managedJobData = list[index];
				TextElement visualElement = managedJobData.visualElement;
				managedJobData.prepareSuccess = visualElement.uitkTextHandle.ConvertUssToTextGenerationSettings(true, null);
				bool prepareSuccess = managedJobData.prepareSuccess;
				if (prepareSuccess)
				{
					managedJobData.prepareSuccess = visualElement.uitkTextHandle.PrepareFontAsset();
				}
			}

			public GCHandle managedJobDataHandle;
		}

		private struct GenerateTextJobData : IJobParallelFor
		{
			public void Execute(int index)
			{
				List<UITKTextJobSystem.ManagedJobData> list = (List<UITKTextJobSystem.ManagedJobData>)this.managedJobDataHandle.Target;
				UITKTextJobSystem.ManagedJobData managedJobData = list[index];
				TextElement visualElement = managedJobData.visualElement;
				bool flag = visualElement.PostProcessTextVertices != null;
				if (flag)
				{
					visualElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
				}
				visualElement.uitkTextHandle.UpdateMesh();
				TextInfo textInfo = visualElement.uitkTextHandle.textInfo;
				MeshInfo[] meshInfo = textInfo.meshInfo;
				List<Material> list2 = null;
				List<NativeSlice<Vertex>> list3 = null;
				List<NativeSlice<ushort>> list4 = null;
				List<GlyphRenderMode> list5 = null;
				UITKTextJobSystem.ConvertMeshInfoToUIRVertex(meshInfo, this.alloc, visualElement, ref list2, ref list3, ref list4, ref list5);
				managedJobData.materials = list2;
				managedJobData.vertices = list3;
				managedJobData.indices = list4;
				managedJobData.renderModes = list5;
				visualElement.uitkTextHandle.HandleATag();
				visualElement.uitkTextHandle.HandleLinkTag();
			}

			public GCHandle managedJobDataHandle;

			[ReadOnly]
			public TempMeshAllocator alloc;
		}
	}
}
