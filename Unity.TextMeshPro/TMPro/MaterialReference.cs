using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMPro
{
	public struct MaterialReference
	{
		public MaterialReference(int index, TMP_FontAsset fontAsset, TMP_SpriteAsset spriteAsset, Material material, float padding)
		{
			this.index = index;
			this.fontAsset = fontAsset;
			this.spriteAsset = spriteAsset;
			this.material = material;
			this.isDefaultMaterial = material.GetInstanceID() == fontAsset.material.GetInstanceID();
			this.isFallbackMaterial = false;
			this.fallbackMaterial = null;
			this.padding = padding;
			this.referenceCount = 0;
		}

		public static bool Contains(MaterialReference[] materialReferences, TMP_FontAsset fontAsset)
		{
			int instanceID = fontAsset.GetInstanceID();
			int num = 0;
			while (num < materialReferences.Length && materialReferences[num].fontAsset != null)
			{
				if (materialReferences[num].fontAsset.GetInstanceID() == instanceID)
				{
					return true;
				}
				num++;
			}
			return false;
		}

		public static int AddMaterialReference(Material material, TMP_FontAsset fontAsset, ref MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
		{
			int instanceID = material.GetInstanceID();
			int count;
			if (materialReferenceIndexLookup.TryGetValue(instanceID, out count))
			{
				return count;
			}
			count = materialReferenceIndexLookup.Count;
			materialReferenceIndexLookup[instanceID] = count;
			if (count >= materialReferences.Length)
			{
				Array.Resize<MaterialReference>(ref materialReferences, Mathf.NextPowerOfTwo(count + 1));
			}
			materialReferences[count].index = count;
			materialReferences[count].fontAsset = fontAsset;
			materialReferences[count].spriteAsset = null;
			materialReferences[count].material = material;
			materialReferences[count].isDefaultMaterial = instanceID == fontAsset.material.GetInstanceID();
			materialReferences[count].referenceCount = 0;
			return count;
		}

		public static int AddMaterialReference(Material material, TMP_SpriteAsset spriteAsset, ref MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
		{
			int instanceID = material.GetInstanceID();
			int count;
			if (materialReferenceIndexLookup.TryGetValue(instanceID, out count))
			{
				return count;
			}
			count = materialReferenceIndexLookup.Count;
			materialReferenceIndexLookup[instanceID] = count;
			if (count >= materialReferences.Length)
			{
				Array.Resize<MaterialReference>(ref materialReferences, Mathf.NextPowerOfTwo(count + 1));
			}
			materialReferences[count].index = count;
			materialReferences[count].fontAsset = materialReferences[0].fontAsset;
			materialReferences[count].spriteAsset = spriteAsset;
			materialReferences[count].material = material;
			materialReferences[count].isDefaultMaterial = true;
			materialReferences[count].referenceCount = 0;
			return count;
		}

		public int index;

		public TMP_FontAsset fontAsset;

		public TMP_SpriteAsset spriteAsset;

		public Material material;

		public bool isDefaultMaterial;

		public bool isFallbackMaterial;

		public Material fallbackMaterial;

		public float padding;

		public int referenceCount;
	}
}
