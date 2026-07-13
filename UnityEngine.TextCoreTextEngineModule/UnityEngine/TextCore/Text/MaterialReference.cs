using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text
{
	internal struct MaterialReference
	{
		public MaterialReference(int index, FontAsset fontAsset, SpriteAsset spriteAsset, Material material, float padding)
		{
			this.index = index;
			this.fontAsset = fontAsset;
			this.spriteAsset = spriteAsset;
			this.material = material;
			this.isFallbackMaterial = false;
			this.fallbackMaterial = null;
			this.padding = padding;
			this.referenceCount = 0;
		}

		public static bool Contains(MaterialReference[] materialReferences, FontAsset fontAsset)
		{
			int hashCode = fontAsset.GetHashCode();
			int num = 0;
			while (num < materialReferences.Length && materialReferences[num].fontAsset != null)
			{
				bool flag = materialReferences[num].fontAsset.GetHashCode() == hashCode;
				if (flag)
				{
					return true;
				}
				num++;
			}
			return false;
		}

		public static int AddMaterialReference(Material material, FontAsset fontAsset, ref MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
		{
			int hashCode = material.GetHashCode();
			int count;
			bool flag = materialReferenceIndexLookup.TryGetValue(hashCode, out count);
			int num;
			if (flag)
			{
				num = count;
			}
			else
			{
				count = materialReferenceIndexLookup.Count;
				materialReferenceIndexLookup[hashCode] = count;
				bool flag2 = count >= materialReferences.Length;
				if (flag2)
				{
					Array.Resize<MaterialReference>(ref materialReferences, Mathf.NextPowerOfTwo(count + 1));
				}
				materialReferences[count].index = count;
				materialReferences[count].fontAsset = fontAsset;
				materialReferences[count].spriteAsset = null;
				materialReferences[count].material = material;
				materialReferences[count].referenceCount = 0;
				num = count;
			}
			return num;
		}

		public static int AddMaterialReference(Material material, SpriteAsset spriteAsset, ref MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
		{
			int hashCode = material.GetHashCode();
			int count;
			bool flag = materialReferenceIndexLookup.TryGetValue(hashCode, out count);
			int num;
			if (flag)
			{
				num = count;
			}
			else
			{
				count = materialReferenceIndexLookup.Count;
				materialReferenceIndexLookup[hashCode] = count;
				bool flag2 = count >= materialReferences.Length;
				if (flag2)
				{
					Array.Resize<MaterialReference>(ref materialReferences, Mathf.NextPowerOfTwo(count + 1));
				}
				materialReferences[count].index = count;
				materialReferences[count].fontAsset = materialReferences[0].fontAsset;
				materialReferences[count].spriteAsset = spriteAsset;
				materialReferences[count].material = material;
				materialReferences[count].referenceCount = 0;
				num = count;
			}
			return num;
		}

		public int index;

		public FontAsset fontAsset;

		public SpriteAsset spriteAsset;

		public Material material;

		public bool isFallbackMaterial;

		public Material fallbackMaterial;

		public float padding;

		public int referenceCount;
	}
}
