using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine
{
	internal class InternalStaticBatchingUtility
	{
		public static void CombineRoot(GameObject staticBatchRoot)
		{
			InternalStaticBatchingUtility.Combine(staticBatchRoot, false, false);
		}

		public static void Combine(GameObject staticBatchRoot, bool combineOnlyStatic, bool isEditorPostprocessScene)
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
			InternalStaticBatchingUtility.CombineGameObjects(array, staticBatchRoot, isEditorPostprocessScene);
		}

		public static void CombineGameObjects(GameObject[] gos, GameObject staticBatchRoot, bool isEditorPostprocessScene)
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
			Array.Sort(gos, new InternalStaticBatchingUtility.SortGO());
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
								Material[] array = component.sharedMaterials;
								if (!array.Any<Material>((Material m) => m != null && m.shader != null && m.shader.disableBatching != DisableBatchingType.False))
								{
									int vertexCount = sharedMesh.vertexCount;
									if (vertexCount != 0)
									{
										MeshRenderer meshRenderer = component as MeshRenderer;
										if (meshRenderer != null && meshRenderer.additionalVertexStreams != null)
										{
											if (vertexCount != meshRenderer.additionalVertexStreams.vertexCount)
											{
												goto IL_0387;
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
										if (array.Length > sharedMesh.subMeshCount)
										{
											Debug.LogWarning(string.Concat(new object[] { "Mesh '", sharedMesh.name, "' has more materials (", array.Length, ") than subsets (", sharedMesh.subMeshCount, ")" }), component);
											Material[] array2 = new Material[sharedMesh.subMeshCount];
											for (int j = 0; j < sharedMesh.subMeshCount; j++)
											{
												array2[j] = component.sharedMaterials[j];
											}
											component.sharedMaterials = array2;
											array = array2;
										}
										for (int k = 0; k < Math.Min(array.Length, sharedMesh.subMeshCount); k++)
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
				IL_0387:;
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

		internal class SortGO : IComparer
		{
			int IComparer.Compare(object a, object b)
			{
				int num;
				if (a == b)
				{
					num = 0;
				}
				else
				{
					Renderer renderer = InternalStaticBatchingUtility.SortGO.GetRenderer(a as GameObject);
					Renderer renderer2 = InternalStaticBatchingUtility.SortGO.GetRenderer(b as GameObject);
					int num2 = InternalStaticBatchingUtility.SortGO.GetMaterialId(renderer).CompareTo(InternalStaticBatchingUtility.SortGO.GetMaterialId(renderer2));
					if (num2 == 0)
					{
						num2 = InternalStaticBatchingUtility.SortGO.GetLightmapIndex(renderer).CompareTo(InternalStaticBatchingUtility.SortGO.GetLightmapIndex(renderer2));
					}
					num = num2;
				}
				return num;
			}

			private static int GetMaterialId(Renderer renderer)
			{
				int num;
				if (renderer == null || renderer.sharedMaterial == null)
				{
					num = 0;
				}
				else
				{
					num = renderer.sharedMaterial.GetInstanceID();
				}
				return num;
			}

			private static int GetLightmapIndex(Renderer renderer)
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

			private static Renderer GetRenderer(GameObject go)
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
		}
	}
}
