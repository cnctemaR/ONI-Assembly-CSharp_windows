using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

namespace UnityEngine
{
	internal class InternalStaticBatchingUtility
	{
		public static void CombineRoot(GameObject staticBatchRoot, InternalStaticBatchingUtility.StaticBatcherGOSorter sorter)
		{
			InternalStaticBatchingUtility.Combine(staticBatchRoot, false, false, sorter);
		}

		public static void Combine(GameObject staticBatchRoot, bool combineOnlyStatic, bool isEditorPostprocessScene, InternalStaticBatchingUtility.StaticBatcherGOSorter sorter)
		{
			GameObject[] array = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
			List<GameObject> list = new List<GameObject>();
			GameObject[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				GameObject gameObject = array2[i];
				bool flag = staticBatchRoot != null;
				if (!flag)
				{
					goto IL_0053;
				}
				bool flag2 = !gameObject.transform.IsChildOf(staticBatchRoot.transform);
				if (!flag2)
				{
					goto IL_0053;
				}
				IL_0075:
				i++;
				continue;
				IL_0053:
				bool flag3 = combineOnlyStatic && !gameObject.isStaticBatchable;
				if (flag3)
				{
					goto IL_0075;
				}
				list.Add(gameObject);
				goto IL_0075;
			}
			array = list.ToArray();
			InternalStaticBatchingUtility.CombineGameObjects(array, staticBatchRoot, isEditorPostprocessScene, sorter);
		}

		private static uint GetMeshFormatHash(Mesh mesh)
		{
			bool flag = mesh == null;
			uint num;
			if (flag)
			{
				num = 0U;
			}
			else
			{
				uint num2 = 1U;
				int vertexAttributeCount = mesh.vertexAttributeCount;
				for (int i = 0; i < vertexAttributeCount; i++)
				{
					VertexAttributeDescriptor vertexAttribute = mesh.GetVertexAttribute(i);
					uint num3 = (uint)(vertexAttribute.attribute | (VertexAttribute)((int)vertexAttribute.format << 4) | (VertexAttribute)((uint)vertexAttribute.dimension << 8));
					num2 = num2 * 2654435761U + num3;
				}
				num = num2;
			}
			return num;
		}

		private static GameObject[] SortGameObjectsForStaticBatching(GameObject[] gos, InternalStaticBatchingUtility.StaticBatcherGOSorter sorter)
		{
			gos = gos.OrderBy<GameObject, long>(delegate(GameObject g)
			{
				Renderer renderer = InternalStaticBatchingUtility.StaticBatcherGOSorter.GetRenderer(g);
				return sorter.GetMaterialId(renderer);
			}).ThenBy<GameObject, int>(delegate(GameObject g)
			{
				Renderer renderer2 = InternalStaticBatchingUtility.StaticBatcherGOSorter.GetRenderer(g);
				return sorter.GetLightmapIndex(renderer2);
			}).ThenBy<GameObject, uint>(delegate(GameObject g)
			{
				Mesh mesh = InternalStaticBatchingUtility.StaticBatcherGOSorter.GetMesh(g);
				return InternalStaticBatchingUtility.GetMeshFormatHash(mesh);
			})
				.ThenBy<GameObject, long>(delegate(GameObject g)
				{
					Renderer renderer3 = InternalStaticBatchingUtility.StaticBatcherGOSorter.GetRenderer(g);
					return sorter.GetRendererId(renderer3);
				})
				.ToArray<GameObject>();
			return gos;
		}

		public static void CombineGameObjects(GameObject[] gos, GameObject staticBatchRoot, bool isEditorPostprocessScene, InternalStaticBatchingUtility.StaticBatcherGOSorter sorter)
		{
			Matrix4x4 matrix4x = Matrix4x4.identity;
			Transform transform = null;
			bool flag = staticBatchRoot;
			if (flag)
			{
				matrix4x = staticBatchRoot.transform.worldToLocalMatrix;
				transform = staticBatchRoot.transform;
			}
			int num = 0;
			int num2 = 0;
			List<MeshSubsetCombineUtility.MeshContainer> list = new List<MeshSubsetCombineUtility.MeshContainer>();
			using (StaticBatchingUtility.s_SortMarker.Auto())
			{
				gos = InternalStaticBatchingUtility.SortGameObjectsForStaticBatching(gos, sorter ?? new InternalStaticBatchingUtility.StaticBatcherGOSorter());
			}
			uint num3 = 0U;
			foreach (GameObject gameObject in gos)
			{
				MeshFilter meshFilter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
				bool flag2 = meshFilter == null;
				if (!flag2)
				{
					Mesh sharedMesh = meshFilter.sharedMesh;
					bool flag3 = sharedMesh == null || (!isEditorPostprocessScene && !sharedMesh.canAccess);
					if (!flag3)
					{
						bool flag4 = !StaticBatchingHelper.IsMeshBatchable(sharedMesh);
						if (!flag4)
						{
							Renderer component = meshFilter.GetComponent<Renderer>();
							bool flag5 = component == null || !component.enabled;
							if (!flag5)
							{
								bool flag6 = component.staticBatchIndex != 0;
								if (!flag6)
								{
									Material[] array2 = component.sharedMaterials;
									bool flag7 = array2.Any<Material>((Material m) => m != null && m.shader != null && m.shader.disableBatching > DisableBatchingType.False);
									if (!flag7)
									{
										int vertexCount = sharedMesh.vertexCount;
										bool flag8 = vertexCount == 0;
										if (!flag8)
										{
											MeshRenderer meshRenderer = component as MeshRenderer;
											bool flag9 = meshRenderer != null;
											if (flag9)
											{
												bool flag10 = meshRenderer.additionalVertexStreams != null;
												if (flag10)
												{
													bool flag11 = vertexCount != meshRenderer.additionalVertexStreams.vertexCount;
													if (flag11)
													{
														goto IL_046F;
													}
												}
												bool flag12 = meshRenderer.enlightenVertexStream != null;
												if (flag12)
												{
													bool flag13 = vertexCount != meshRenderer.enlightenVertexStream.vertexCount;
													if (flag13)
													{
														goto IL_046F;
													}
												}
											}
											uint meshFormatHash = InternalStaticBatchingUtility.GetMeshFormatHash(sharedMesh);
											bool flag14 = num2 + vertexCount > 64000 || meshFormatHash != num3;
											if (flag14)
											{
												InternalStaticBatchingUtility.MakeBatch(list, transform, num++);
												list.Clear();
												num2 = 0;
											}
											num3 = meshFormatHash;
											MeshSubsetCombineUtility.MeshInstance meshInstance = default(MeshSubsetCombineUtility.MeshInstance);
											meshInstance.meshInstanceID = sharedMesh.GetInstanceID();
											meshInstance.rendererInstanceID = component.GetInstanceID();
											bool flag15 = meshRenderer != null;
											if (flag15)
											{
												bool flag16 = meshRenderer.additionalVertexStreams != null;
												if (flag16)
												{
													meshInstance.additionalVertexStreamsMeshInstanceID = meshRenderer.additionalVertexStreams.GetInstanceID();
												}
												bool flag17 = meshRenderer.enlightenVertexStream != null;
												if (flag17)
												{
													meshInstance.enlightenVertexStreamMeshInstanceID = meshRenderer.enlightenVertexStream.GetInstanceID();
												}
											}
											meshInstance.transform = matrix4x * meshFilter.transform.localToWorldMatrix;
											meshInstance.lightmapScaleOffset = component.lightmapScaleOffset;
											meshInstance.realtimeLightmapScaleOffset = component.realtimeLightmapScaleOffset;
											MeshSubsetCombineUtility.MeshContainer meshContainer = new MeshSubsetCombineUtility.MeshContainer
											{
												gameObject = gameObject,
												instance = meshInstance,
												subMeshInstances = new List<MeshSubsetCombineUtility.SubMeshInstance>()
											};
											list.Add(meshContainer);
											bool flag18 = array2.Length > sharedMesh.subMeshCount;
											if (flag18)
											{
												Debug.LogWarning(string.Concat(new string[]
												{
													"Mesh '",
													sharedMesh.name,
													"' has more materials (",
													array2.Length.ToString(),
													") than subsets (",
													sharedMesh.subMeshCount.ToString(),
													")"
												}), component);
												Material[] array3 = new Material[sharedMesh.subMeshCount];
												for (int j = 0; j < sharedMesh.subMeshCount; j++)
												{
													array3[j] = component.sharedMaterials[j];
												}
												component.sharedMaterials = array3;
												array2 = array3;
											}
											for (int k = 0; k < Math.Min(array2.Length, sharedMesh.subMeshCount); k++)
											{
												MeshSubsetCombineUtility.SubMeshInstance subMeshInstance = default(MeshSubsetCombineUtility.SubMeshInstance);
												subMeshInstance.meshInstanceID = meshFilter.sharedMesh.GetInstanceID();
												subMeshInstance.vertexOffset = num2;
												subMeshInstance.subMeshIndex = k;
												subMeshInstance.gameObjectInstanceID = gameObject.GetInstanceID();
												subMeshInstance.transform = meshInstance.transform;
												meshContainer.subMeshInstances.Add(subMeshInstance);
											}
											num2 += sharedMesh.vertexCount;
										}
									}
								}
							}
						}
					}
				}
				IL_046F:;
			}
			InternalStaticBatchingUtility.MakeBatch(list, transform, num);
		}

		private static void MakeBatch(List<MeshSubsetCombineUtility.MeshContainer> meshes, Transform staticBatchRootTransform, int batchIndex)
		{
			bool flag = meshes.Count < 2;
			if (!flag)
			{
				using (StaticBatchingUtility.s_MakeBatchMarker.Auto())
				{
					List<MeshSubsetCombineUtility.MeshInstance> list = new List<MeshSubsetCombineUtility.MeshInstance>();
					List<MeshSubsetCombineUtility.SubMeshInstance> list2 = new List<MeshSubsetCombineUtility.SubMeshInstance>();
					foreach (MeshSubsetCombineUtility.MeshContainer meshContainer in meshes)
					{
						list.Add(meshContainer.instance);
						list2.AddRange(meshContainer.subMeshInstances);
					}
					string text = "Combined Mesh";
					text = text + " (root: " + ((staticBatchRootTransform != null) ? staticBatchRootTransform.name : "scene") + ")";
					bool flag2 = batchIndex > 0;
					if (flag2)
					{
						text = text + " " + (batchIndex + 1).ToString();
					}
					Mesh mesh = StaticBatchingHelper.InternalCombineVertices(list.ToArray(), text);
					StaticBatchingHelper.InternalCombineIndices(list2.ToArray(), mesh);
					int num = 0;
					foreach (MeshSubsetCombineUtility.MeshContainer meshContainer2 in meshes)
					{
						MeshFilter meshFilter = (MeshFilter)meshContainer2.gameObject.GetComponent(typeof(MeshFilter));
						meshFilter.sharedMesh = mesh;
						int count = meshContainer2.subMeshInstances.Count;
						Renderer component = meshContainer2.gameObject.GetComponent<Renderer>();
						component.SetStaticBatchInfo(num, count);
						component.staticBatchRootTransform = staticBatchRootTransform;
						component.enabled = false;
						component.enabled = true;
						MeshRenderer meshRenderer = component as MeshRenderer;
						bool flag3 = meshRenderer != null;
						if (flag3)
						{
							meshRenderer.additionalVertexStreams = null;
							meshRenderer.enlightenVertexStream = null;
						}
						num += count;
					}
				}
			}
		}

		private const int MaxVerticesInBatch = 64000;

		private const string CombinedMeshPrefix = "Combined Mesh";

		public class StaticBatcherGOSorter
		{
			public virtual long GetMaterialId(Renderer renderer)
			{
				bool flag = renderer == null || renderer.sharedMaterial == null;
				long num;
				if (flag)
				{
					num = 0L;
				}
				else
				{
					num = (long)renderer.sharedMaterial.GetInstanceID();
				}
				return num;
			}

			public int GetLightmapIndex(Renderer renderer)
			{
				bool flag = renderer == null;
				int num;
				if (flag)
				{
					num = -1;
				}
				else
				{
					num = renderer.lightmapIndex;
				}
				return num;
			}

			public static Renderer GetRenderer(GameObject go)
			{
				bool flag = go == null;
				Renderer renderer;
				if (flag)
				{
					renderer = null;
				}
				else
				{
					MeshFilter meshFilter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
					bool flag2 = meshFilter == null;
					if (flag2)
					{
						renderer = null;
					}
					else
					{
						renderer = meshFilter.GetComponent<Renderer>();
					}
				}
				return renderer;
			}

			public static Mesh GetMesh(GameObject go)
			{
				bool flag = go == null;
				Mesh mesh;
				if (flag)
				{
					mesh = null;
				}
				else
				{
					MeshFilter component = go.GetComponent<MeshFilter>();
					bool flag2 = component == null;
					if (flag2)
					{
						mesh = null;
					}
					else
					{
						mesh = component.sharedMesh;
					}
				}
				return mesh;
			}

			public virtual long GetRendererId(Renderer renderer)
			{
				bool flag = renderer == null;
				long num;
				if (flag)
				{
					num = -1L;
				}
				else
				{
					num = (long)renderer.GetInstanceID();
				}
				return num;
			}
		}
	}
}
