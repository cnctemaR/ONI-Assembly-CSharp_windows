using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.Pool;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal class ATGTextJobSystem
	{
		public ATGTextJobSystem()
		{
			this.m_GenerateTextJobifiedCallback = new MeshGenerationCallback(this.GenerateTextJobified);
			this.m_PopulateGlyphsCallback = new MeshGenerationCallback(this.PopulateGlyphs);
			this.m_AddDrawEntriesCallback = new MeshGenerationCallback(this.AddDrawEntries);
		}

		private static bool PrepareTextElementForJobsOnMainThread(TextElement textElement)
		{
			textElement.uitkTextHandle.EnsureIsReadyForJobs();
			bool flag = textElement.computedStyle.unityFontDefinition.fontAsset == null;
			if (flag)
			{
				bool flag2 = !textElement.uitkTextHandle.ConvertUssToNativeTextGenerationSettings(null, null);
				if (flag2)
				{
					return false;
				}
			}
			bool enableRichText = textElement.enableRichText;
			if (enableRichText)
			{
				TextSettings textSettingsFrom = TextUtilities.GetTextSettingsFrom(textElement);
				RichTextTagParser.PreloadFontAssetsFromTags(textElement.renderedTextString, textSettingsFrom);
				RichTextTagParser.PreloadSpriteAssetsFromTags(textElement.renderedTextString, textSettingsFrom);
			}
			return true;
		}

		internal unsafe void PrepareShapingBeforeLayout(BaseVisualElementPanel panel)
		{
			bool flag = !panel.visualTree.layoutNode.IsDirty;
			if (!flag)
			{
				bool flag2 = !panel.textElementRegistry.IsValueCreated;
				if (!flag2)
				{
					using (ATGTextJobSystem.k_PrepareShapingMarker.Auto())
					{
						foreach (TextElement textElement in panel.textElementRegistry.Value)
						{
							bool isDirty = textElement.layoutNode.IsDirty;
							if (isDirty)
							{
								bool flag3 = TextUtilities.IsAdvancedTextEnabledForElement(textElement) && TextElement.AnySizeAutoOrNone(*textElement.computedStyle);
								if (flag3)
								{
									bool flag4 = ATGTextJobSystem.PrepareTextElementForJobsOnMainThread(textElement);
									if (flag4)
									{
										this.m_PrepareShapingDataList.Add(textElement);
									}
								}
							}
						}
						bool flag5 = this.m_PrepareShapingDataList.Count > 0;
						if (flag5)
						{
							FontAsset.CreateHbFaceIfNeeded();
							GCHandle gchandle = GCHandle.Alloc(this.m_PrepareShapingDataList);
							ATGTextJobSystem.PrepareShapingJob prepareShapingJob = new ATGTextJobSystem.PrepareShapingJob
							{
								managedJobDataHandle = gchandle
							};
							(ref prepareShapingJob).ScheduleParallelByRef<ATGTextJobSystem.PrepareShapingJob>(this.m_PrepareShapingDataList.Count, 1, default(JobHandle)).Complete();
							gchandle.Free();
							this.m_PrepareShapingDataList.Clear();
						}
					}
				}
			}
		}

		public void GenerateText(MeshGenerationContext mgc, TextElement textElement)
		{
			MeshGenerationNode meshGenerationNode;
			mgc.InsertMeshGenerationNode(out meshGenerationNode);
			ATGTextJobSystem.ManagedJobData managedJobData = ATGTextJobSystem.s_JobDataPool.Get();
			managedJobData.textElement = textElement;
			managedJobData.node = meshGenerationNode;
			this.textJobDatas.Add(managedJobData);
			bool flag = this.hasPendingTextWork;
			if (!flag)
			{
				this.hasPendingTextWork = true;
				this.textJobDatasHandle = GCHandle.Alloc(this.textJobDatas);
				MeshGenerationCallbackType meshGenerationCallbackType = (ATGTextJobSystem.k_IsMultiThreaded ? MeshGenerationCallbackType.Fork : MeshGenerationCallbackType.Work);
				mgc.AddMeshGenerationCallback(this.m_GenerateTextJobifiedCallback, null, meshGenerationCallbackType, false);
			}
		}

		private void GenerateTextJobified(MeshGenerationContext mgc, object _)
		{
			TempMeshAllocator tempMeshAllocator;
			mgc.GetTempMeshAllocator(out tempMeshAllocator);
			ATGTextJobSystem.GenerateTextJobData generateTextJobData = new ATGTextJobSystem.GenerateTextJobData
			{
				managedJobDataHandle = this.textJobDatasHandle,
				alloc = tempMeshAllocator
			};
			for (int i = this.textJobDatas.Count - 1; i >= 0; i--)
			{
				ATGTextJobSystem.ManagedJobData managedJobData = this.textJobDatas[i];
				TextElement textElement = managedJobData.textElement;
				bool flag = ATGTextJobSystem.PrepareTextElementForJobsOnMainThread(textElement);
				bool flag2 = !flag;
				if (flag2)
				{
					this.textJobDatas.RemoveAt(i);
				}
			}
			FontAsset.CreateHbFaceIfNeeded();
			bool flag3 = ATGTextJobSystem.k_IsMultiThreaded;
			if (flag3)
			{
				JobHandle jobHandle = (ref generateTextJobData).ScheduleParallelByRef<ATGTextJobSystem.GenerateTextJobData>(this.textJobDatas.Count, 1, default(JobHandle));
				mgc.AddMeshGenerationJob(jobHandle);
				mgc.AddMeshGenerationCallback(this.m_PopulateGlyphsCallback, null, MeshGenerationCallbackType.Work, true);
			}
			else
			{
				for (int j = 0; j < this.textJobDatas.Count; j++)
				{
					generateTextJobData.Execute(j);
				}
				mgc.AddMeshGenerationCallback(this.m_PopulateGlyphsCallback, null, MeshGenerationCallbackType.Work, false);
			}
		}

		private void PopulateGlyphs(MeshGenerationContext mgc, object _)
		{
			Dictionary<int, HashSet<uint>> dictionary = ATGTextJobSystem.s_AggregatedMissingGlyphsPool.Get();
			bool flag = false;
			foreach (ATGTextJobSystem.ManagedJobData managedJobData in this.textJobDatas)
			{
				bool flag2 = !managedJobData.hasMissingGlyphs;
				if (!flag2)
				{
					foreach (KeyValuePair<int, HashSet<uint>> keyValuePair in managedJobData.missingGlyphsPerFontAsset)
					{
						bool flag3 = keyValuePair.Value.Count == 0;
						if (!flag3)
						{
							flag = true;
							HashSet<uint> hashSet;
							bool flag4 = !dictionary.TryGetValue(keyValuePair.Key, out hashSet);
							if (flag4)
							{
								hashSet = new HashSet<uint>();
								dictionary[keyValuePair.Key] = hashSet;
							}
							hashSet.UnionWith(keyValuePair.Value);
						}
					}
				}
			}
			bool flag5 = !flag;
			if (flag5)
			{
				ATGTextJobSystem.s_AggregatedMissingGlyphsPool.Release(dictionary);
				this.AddDrawEntries(mgc, _);
			}
			else
			{
				foreach (KeyValuePair<int, HashSet<uint>> keyValuePair2 in dictionary)
				{
					TextAsset textAssetByID = TextAsset.GetTextAssetByID(keyValuePair2.Key);
					if (textAssetByID == null)
					{
						goto IL_015D;
					}
					FontAsset fontAsset = textAssetByID as FontAsset;
					if (fontAsset == null)
					{
						goto IL_015D;
					}
					bool flag6 = keyValuePair2.Value.Count == 0;
					IL_015E:
					bool flag7 = flag6;
					if (flag7)
					{
						continue;
					}
					ATGTextJobSystem.s_GlyphsToAddBuffer.Clear();
					ATGTextJobSystem.s_GlyphsToAddBuffer.AddRange(keyValuePair2.Value);
					fontAsset.TryAddGlyphs(ATGTextJobSystem.s_GlyphsToAddBuffer);
					continue;
					IL_015D:
					flag6 = true;
					goto IL_015E;
				}
				ATGTextJobSystem.s_AggregatedMissingGlyphsPool.Release(dictionary);
				FontAsset.UpdateFontAssetsInUpdateQueue();
				TempMeshAllocator tempMeshAllocator;
				mgc.GetTempMeshAllocator(out tempMeshAllocator);
				ATGTextJobSystem.ConvertToUIRVertexJobData convertToUIRVertexJobData = new ATGTextJobSystem.ConvertToUIRVertexJobData
				{
					managedJobDataHandle = this.textJobDatasHandle,
					alloc = tempMeshAllocator
				};
				JobHandle jobHandle = (ref convertToUIRVertexJobData).ScheduleParallelByRef<ATGTextJobSystem.ConvertToUIRVertexJobData>(this.textJobDatas.Count, 1, default(JobHandle));
				mgc.AddMeshGenerationJob(jobHandle);
				mgc.AddMeshGenerationCallback(this.m_AddDrawEntriesCallback, null, MeshGenerationCallbackType.Work, true);
			}
		}

		private void AddDrawEntries(MeshGenerationContext mgc, object _)
		{
			foreach (ATGTextJobSystem.ManagedJobData managedJobData in this.textJobDatas)
			{
				bool success = managedJobData.success;
				if (success)
				{
					NativeTextInfo textInfo = managedJobData.textInfo;
					mgc.Begin(managedJobData.node.GetParentEntry(), managedJobData.textElement, managedJobData.textElement.renderData);
					Action<TextElement.GlyphsEnumerable> postProcessTextVertices = managedJobData.textElement.PostProcessTextVertices;
					if (postProcessTextVertices != null)
					{
						postProcessTextVertices(new TextElement.GlyphsEnumerable(managedJobData.textElement, managedJobData.vertices, textInfo.meshInfos));
					}
					mgc.meshGenerator.DrawText(managedJobData.vertices, managedJobData.indices, managedJobData.atlases, managedJobData.renderModes, managedJobData.sdfScales);
					managedJobData.textElement.OnGenerateTextOverNative(mgc);
					managedJobData.textElement.uitkTextHandle.UpdateATGTextEventHandler();
					mgc.End();
				}
				ATGTextJobSystem.s_JobDataPool.Release(managedJobData);
			}
			this.textJobDatas.Clear();
			this.textJobDatasHandle.Free();
			this.hasPendingTextWork = false;
		}

		private unsafe static void ConvertMeshInfoToUIRVertex(Span<ATGMeshInfo> meshInfos, TempMeshAllocator alloc, TextElement visualElement, List<List<List<int>>> textElementIndicesByMesh, List<bool> hasMultipleColorsByMesh, ref List<Texture2D> atlases, ref List<NativeSlice<Vertex>> verticesArray, ref List<NativeSlice<ushort>> indicesArray, ref List<GlyphRenderMode> renderModes, ref List<float> sdfScales)
		{
			float num = 1f / visualElement.scaledPixelsPerPoint;
			for (int i = 0; i < meshInfos.Length; i++)
			{
				ATGMeshInfo atgmeshInfo = *meshInfos[i];
				FontAsset fontAsset = null;
				SpriteAsset spriteAsset = null;
				TextAsset textAssetByID = TextAsset.GetTextAssetByID(atgmeshInfo.textAssetId);
				bool flag = textAssetByID == null;
				if (!flag)
				{
					bool flag2 = false;
					bool flag3 = textAssetByID is FontAsset;
					int num2;
					if (flag3)
					{
						fontAsset = textAssetByID as FontAsset;
						num2 = fontAsset.atlasTextures.Length;
					}
					else
					{
						flag2 = true;
						spriteAsset = textAssetByID as SpriteAsset;
						num2 = 1;
					}
					int num3 = (int)((ulong)UIRenderDevice.maxVerticesPerPage & 18446744073709551612UL);
					bool flag4 = hasMultipleColorsByMesh[i];
					bool flag5 = flag4;
					if (flag5)
					{
						visualElement.renderData.flags |= RenderDataFlags.IsIgnoringDynamicColorHint;
					}
					else
					{
						visualElement.renderData.flags &= ~RenderDataFlags.IsIgnoringDynamicColorHint;
					}
					for (int j = 0; j < num2; j++)
					{
						List<int> list = textElementIndicesByMesh[i][j];
						int k;
						int num4;
						for (k = list.Count * 4; k > 0; k -= num4)
						{
							num4 = Mathf.Min(k, num3);
							int num5 = num4 >> 2;
							int num6 = num5 * 6;
							bool flag6 = flag2;
							if (flag6)
							{
								atlases.Add((Texture2D)spriteAsset.spriteSheet);
								renderModes.Add(GlyphRenderMode.COLOR);
							}
							else
							{
								atlases.Add(fontAsset.atlasTextures[j]);
								renderModes.Add(fontAsset.atlasRenderMode);
							}
							float num7 = 0f;
							bool flag7;
							if (!flag2)
							{
								List<GlyphRenderMode> list2 = renderModes;
								flag7 = !TextGeneratorUtilities.IsBitmapRendering(list2[list2.Count - 1]);
							}
							else
							{
								flag7 = false;
							}
							bool flag8 = flag7;
							if (flag8)
							{
								num7 = (float)(fontAsset.atlasPadding + 1);
							}
							sdfScales.Add(num7);
							bool flag9 = !flag2 && fontAsset.atlasRenderMode != GlyphRenderMode.SMOOTH && fontAsset.atlasRenderMode != GlyphRenderMode.COLOR;
							bool flag10 = visualElement.PostProcessTextVertices == null && !flag4 && (RenderEvents.NeedsColorID(visualElement) || (flag9 && RenderEvents.NeedsTextCoreSettings(visualElement)));
							NativeSlice<Vertex> nativeSlice;
							NativeSlice<ushort> nativeSlice2;
							alloc.AllocateTempMesh(num4, num6, out nativeSlice, out nativeSlice2);
							Vector2 min = visualElement.contentRect.min;
							int l = 0;
							int num8 = 0;
							int num9 = 0;
							while (l < num4)
							{
								bool flag11 = !flag2 && (fontAsset.atlasRenderMode == GlyphRenderMode.COLOR || fontAsset.atlasRenderMode == GlyphRenderMode.COLOR_HINTED);
								NativeTextElementInfo nativeTextElementInfo = *atgmeshInfo.textElementInfos[list[num8]];
								nativeSlice[l] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.bottomLeft, min, num, flag10, flag11);
								nativeSlice[l + 1] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.topLeft, min, num, flag10, flag11);
								nativeSlice[l + 2] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.topRight, min, num, flag10, flag11);
								nativeSlice[l + 3] = MeshGenerator.ConvertTextVertexToUIRVertex(ref nativeTextElementInfo.bottomRight, min, num, flag10, flag11);
								nativeSlice2[num9] = (ushort)l;
								nativeSlice2[num9 + 1] = (ushort)(l + 1);
								nativeSlice2[num9 + 2] = (ushort)(l + 2);
								nativeSlice2[num9 + 3] = (ushort)(l + 2);
								nativeSlice2[num9 + 4] = (ushort)(l + 3);
								nativeSlice2[num9 + 5] = (ushort)l;
								l += 4;
								num8++;
								num9 += 6;
							}
							verticesArray.Add(nativeSlice);
							indicesArray.Add(nativeSlice2);
						}
						Debug.Assert(k == 0);
					}
				}
			}
		}

		private GCHandle textJobDatasHandle;

		private List<ATGTextJobSystem.ManagedJobData> textJobDatas = new List<ATGTextJobSystem.ManagedJobData>();

		private bool hasPendingTextWork;

		private static readonly ObjectPool<ATGTextJobSystem.ManagedJobData> s_JobDataPool = new ObjectPool<ATGTextJobSystem.ManagedJobData>(() => new ATGTextJobSystem.ManagedJobData(), null, delegate(ATGTextJobSystem.ManagedJobData inst)
		{
			inst.Clear();
		}, null, false, 10, 10000);

		private static ObjectPool<Dictionary<int, HashSet<uint>>> s_AggregatedMissingGlyphsPool = new ObjectPool<Dictionary<int, HashSet<uint>>>(() => new Dictionary<int, HashSet<uint>>(), null, delegate(Dictionary<int, HashSet<uint>> dict)
		{
			foreach (HashSet<uint> hashSet in dict.Values)
			{
				hashSet.Clear();
			}
		}, null, false, 10, 10000);

		internal MeshGenerationCallback m_GenerateTextJobifiedCallback;

		internal MeshGenerationCallback m_PopulateGlyphsCallback;

		internal MeshGenerationCallback m_AddDrawEntriesCallback;

		private static readonly ProfilerMarker k_GenerateTextMarker = new ProfilerMarker("ATGTextJob.GenerateText");

		private static readonly ProfilerMarker k_ATGTextJobMarker = new ProfilerMarker("ATGTextJob");

		private static readonly ProfilerMarker k_PrepareShapingMarker = new ProfilerMarker("LayoutUpdater.PrepareShaping");

		private static readonly bool k_IsMultiThreaded = true;

		private List<TextElement> m_PrepareShapingDataList = new List<TextElement>();

		private static List<uint> s_GlyphsToAddBuffer = new List<uint>();

		private class ManagedJobData
		{
			public void Clear()
			{
				this.textElement = null;
				this.node = default(MeshGenerationNode);
				this.textInfo = default(NativeTextInfo);
				this.success = false;
				this.hasMissingGlyphs = false;
				this.atlases.Clear();
				this.sdfScales.Clear();
				this.vertices.Clear();
				this.indices.Clear();
				this.renderModes.Clear();
				this.hasMultipleColorsByMesh.Clear();
				foreach (List<List<int>> list in this.textElementIndicesByMesh)
				{
					foreach (List<int> list2 in list)
					{
						list2.Clear();
					}
				}
				foreach (HashSet<uint> hashSet in this.missingGlyphsPerFontAsset.Values)
				{
					hashSet.Clear();
				}
			}

			public TextElement textElement;

			public MeshGenerationNode node;

			public NativeTextInfo textInfo;

			public bool success;

			public List<Texture2D> atlases = new List<Texture2D>();

			public List<float> sdfScales = new List<float>();

			public List<NativeSlice<Vertex>> vertices = new List<NativeSlice<Vertex>>();

			public List<NativeSlice<ushort>> indices = new List<NativeSlice<ushort>>();

			public List<GlyphRenderMode> renderModes = new List<GlyphRenderMode>();

			public List<List<List<int>>> textElementIndicesByMesh = new List<List<List<int>>>();

			public List<bool> hasMultipleColorsByMesh = new List<bool>();

			public Dictionary<int, HashSet<uint>> missingGlyphsPerFontAsset = new Dictionary<int, HashSet<uint>>();

			public bool hasMissingGlyphs;
		}

		private struct PrepareShapingJob : IJobFor
		{
			public void Execute(int index)
			{
				List<TextElement> list = (List<TextElement>)this.managedJobDataHandle.Target;
				TextElement textElement = list[index];
				textElement.uitkTextHandle.ShapeText();
			}

			public GCHandle managedJobDataHandle;
		}

		private struct GenerateTextJobData : IJobFor
		{
			public void Execute(int index)
			{
				List<ATGTextJobSystem.ManagedJobData> list = (List<ATGTextJobSystem.ManagedJobData>)this.managedJobDataHandle.Target;
				ATGTextJobSystem.ManagedJobData managedJobData = list[index];
				TextElement textElement = managedJobData.textElement;
				bool flag = textElement.computedStyle.unityFontDefinition.fontAsset != null;
				bool flag2 = textElement.PostProcessTextVertices != null;
				if (flag2)
				{
					textElement.uitkTextHandle.CacheTextGenerationInfo();
				}
				ATGTextJobSystem.ManagedJobData managedJobData2 = managedJobData;
				ATGTextJobSystem.ManagedJobData managedJobData3 = managedJobData;
				ValueTuple<NativeTextInfo, bool> valueTuple = textElement.uitkTextHandle.UpdateNative(flag);
				managedJobData2.textInfo = valueTuple.Item1;
				managedJobData3.success = valueTuple.Item2;
				managedJobData.hasMissingGlyphs = managedJobData.textElement.uitkTextHandle.HasMissingGlyphs(managedJobData.textInfo, ref managedJobData.missingGlyphsPerFontAsset);
				bool flag3 = !managedJobData.hasMissingGlyphs;
				if (flag3)
				{
					managedJobData.textElement.uitkTextHandle.ProcessMeshInfos(managedJobData.textInfo, ref managedJobData.textElementIndicesByMesh, ref managedJobData.hasMultipleColorsByMesh);
					ATGTextJobSystem.ConvertMeshInfoToUIRVertex(managedJobData.textInfo.meshInfos, this.alloc, managedJobData.textElement, managedJobData.textElementIndicesByMesh, managedJobData.hasMultipleColorsByMesh, ref managedJobData.atlases, ref managedJobData.vertices, ref managedJobData.indices, ref managedJobData.renderModes, ref managedJobData.sdfScales);
				}
			}

			public GCHandle managedJobDataHandle;

			[ReadOnly]
			public TempMeshAllocator alloc;
		}

		private struct ConvertToUIRVertexJobData : IJobFor
		{
			public void Execute(int index)
			{
				List<ATGTextJobSystem.ManagedJobData> list = (List<ATGTextJobSystem.ManagedJobData>)this.managedJobDataHandle.Target;
				ATGTextJobSystem.ManagedJobData managedJobData = list[index];
				TextElement textElement = managedJobData.textElement;
				bool hasMissingGlyphs = managedJobData.hasMissingGlyphs;
				if (hasMissingGlyphs)
				{
					managedJobData.textElement.uitkTextHandle.ProcessMeshInfos(managedJobData.textInfo, ref managedJobData.textElementIndicesByMesh, ref managedJobData.hasMultipleColorsByMesh);
					ATGTextJobSystem.ConvertMeshInfoToUIRVertex(managedJobData.textInfo.meshInfos, this.alloc, managedJobData.textElement, managedJobData.textElementIndicesByMesh, managedJobData.hasMultipleColorsByMesh, ref managedJobData.atlases, ref managedJobData.vertices, ref managedJobData.indices, ref managedJobData.renderModes, ref managedJobData.sdfScales);
				}
			}

			public GCHandle managedJobDataHandle;

			[ReadOnly]
			public TempMeshAllocator alloc;
		}
	}
}
