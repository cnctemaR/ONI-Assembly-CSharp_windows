using System;
using System.Collections.Generic;
using System.Linq;

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
			foreach (GameObject gameObject in array)
			{
				if (!(staticBatchRoot != null) || gameObject.transform.IsChildOf(staticBatchRoot.transform))
				{
					if (!combineOnlyStatic || gameObject.isStaticBatchable)
					{
						list.Add(gameObject);
					}
				}
			}
			array = list.ToArray();
			InternalStaticBatchingUtility.CombineGameObjects(array, staticBatchRoot, isEditorPostprocessScene, sorter);
		}

		public static GameObject[] SortGameObjectsForStaticbatching(GameObject[] gos, InternalStaticBatchingUtility.StaticBatcherGOSorter sorter)
		{
			gos = gos.OrderBy<GameObject, long>(delegate(GameObject x)
			{
				Renderer renderer = InternalStaticBatchingUtility.StaticBatcherGOSorter.GetRenderer(x);
				return sorter.GetMaterialId(renderer);
			}).ThenBy<GameObject, int>(delegate(GameObject y)
			{
				Renderer renderer2 = InternalStaticBatchingUtility.StaticBatcherGOSorter.GetRenderer(y);
				return sorter.GetLightmapIndex(renderer2);
			}).ThenBy<GameObject, long>(delegate(GameObject z)
			{
				Renderer renderer3 = InternalStaticBatchingUtility.StaticBatcherGOSorter.GetRenderer(z);
				return sorter.GetRendererId(renderer3);
			})
				.ToArray<GameObject>();
			return gos;
		}

		public static void CombineGameObjects(GameObject[] gos, GameObject staticBatchRoot, bool isEditorPostprocessScene, InternalStaticBatchingUtility.StaticBatcherGOSorter sorter)
		{
			Matrix4x4 matrix4x = Matrix4x4.identity;
			Transform transform = null;
			if (staticBatchRoot)
			{
				matrix4x = staticBatchRoot.transform.worldToLocalMatrix;
				transform = staticBatchRoot.transform;
			}
			int num = 0;
			int num2 = 0;
			List<MeshSubsetCombineUtility.MeshContainer> list = new List<MeshSubsetCombineUtility.MeshContainer>();
			gos = InternalStaticBatchingUtility.SortGameObjectsForStaticbatching(gos, sorter ?? new InternalStaticBatchingUtility.StaticBatcherGOSorter());
			foreach (GameObject gameObject in gos)
			{
				MeshFilter meshFilter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
				if (!(meshFilter == null))
				{
					Mesh sharedMesh = meshFilter.sharedMesh;
					if (!(sharedMesh == null) && (isEditorPostprocessScene || sharedMesh.canAccess))
					{
						Renderer component = meshFilter.GetComponent<Renderer>();
						if (!(component == null) && component.enabled)
						{
							if (component.staticBatchIndex == 0)
							{
								Material[] array2 = component.sharedMaterials;
								if (!array2.Any<Material>((Material m) => m != null && m.shader != null && m.shader.disableBatching != DisableBatchingType.False))
								{
									int vertexCount = sharedMesh.vertexCount;
									if (vertexCount != 0)
									{
										MeshRenderer meshRenderer = component as MeshRenderer;
										if (meshRenderer != null && meshRenderer.additionalVertexStreams != null)
										{
											if (vertexCount != meshRenderer.additionalVertexStreams.vertexCount)
											{
												goto IL_0391;
											}
										}
										if (num2 + vertexCount > 64000)
										{
											InternalStaticBatchingUtility.MakeBatch(list, transform, num++);
											list.Clear();
											num2 = 0;
										}
										MeshSubsetCombineUtility.MeshInstance meshInstance = default(MeshSubsetCombineUtility.MeshInstance);
										meshInstance.meshInstanceID = sharedMesh.GetInstanceID();
										meshInstance.rendererInstanceID = component.GetInstanceID();
										if (meshRenderer != null && meshRenderer.additionalVertexStreams != null)
										{
											meshInstance.additionalVertexStreamsMeshInstanceID = meshRenderer.additionalVertexStreams.GetInstanceID();
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
										if (array2.Length > sharedMesh.subMeshCount)
										{
											Debug.LogWarning(string.Concat(new object[] { "Mesh '", sharedMesh.name, "' has more materials (", array2.Length, ") than subsets (", sharedMesh.subMeshCount, ")" }), component);
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
				IL_0391:;
			}
			InternalStaticBatchingUtility.MakeBatch(list, transform, num);
		}

		private static void MakeBatch(List<MeshSubsetCombineUtility.MeshContainer> meshes, Transform staticBatchRootTransform, int batchIndex)
		{
			if (meshes.Count >= 2)
			{
				List<MeshSubsetCombineUtility.MeshInstance> list = new List<MeshSubsetCombineUtility.MeshInstance>();
				List<MeshSubsetCombineUtility.SubMeshInstance> list2 = new List<MeshSubsetCombineUtility.SubMeshInstance>();
				foreach (MeshSubsetCombineUtility.MeshContainer meshContainer in meshes)
				{
					list.Add(meshContainer.instance);
					list2.AddRange(meshContainer.subMeshInstances);
				}
				string text = "Combined Mesh";
				text = text + " (root: " + ((!(staticBatchRootTransform != null)) ? "scene" : staticBatchRootTransform.name) + ")";
				if (batchIndex > 0)
				{
					text = text + " " + (batchIndex + 1);
				}
				Mesh mesh = StaticBatchingHelper.InternalCombineVertices(list.ToArray(), text);
				StaticBatchingHelper.InternalCombineIndices(list2.ToArray(), mesh);
				int num = 0;
				foreach (MeshSubsetCombineUtility.MeshContainer meshContainer2 in meshes)
				{
					MeshFilter meshFilter = (MeshFilter)meshContainer2.gameObject.GetComponent(typeof(MeshFilter));
					meshFilter.sharedMesh = mesh;
					int num2 = meshContainer2.subMeshInstances.Count<MeshSubsetCombineUtility.SubMeshInstance>();
					Renderer component = meshContainer2.gameObject.GetComponent<Renderer>();
					component.SetStaticBatchInfo(num, num2);
					component.staticBatchRootTransform = staticBatchRootTransform;
					component.enabled = false;
					component.enabled = true;
					MeshRenderer meshRenderer = component as MeshRenderer;
					if (meshRenderer != null)
					{
						meshRenderer.additionalVertexStreams = null;
					}
					num += num2;
				}
			}
		}

		private const int MaxVerticesInBatch = 64000;

		private const string CombinedMeshPrefix = "Combined Mesh";

		public class StaticBatcherGOSorter
		{
			public virtual long GetMaterialId(Renderer renderer)
			{
				long num;
				if (renderer == null || renderer.sharedMaterial == null)
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
				int num;
				if (renderer == null)
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
				Renderer renderer;
				if (go == null)
				{
					renderer = null;
				}
				else
				{
					MeshFilter meshFilter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
					if (meshFilter == null)
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

			public virtual long GetRendererId(Renderer renderer)
			{
				long num;
				if (renderer == null)
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
