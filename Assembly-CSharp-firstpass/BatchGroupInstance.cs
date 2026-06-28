using System;
using System.Collections.Generic;
using UnityEngine;

public class BatchGroupInstance
{
	public BatchGroupInstance(KAnimBatchGroup group)
	{
		this.group = group;
		this.requiresRebuild = true;
		this.overriddenSymbols = new Dictionary<KAnimHashedString, List<KAnim.Build.SymbolFrameInstance>>();
		this.overrideSourceFile = new Dictionary<KAnimHashedString, HashedString>();
		this.textures = new List<Texture2D>();
		this.symbolFrameInstances = new List<KAnim.Build.SymbolFrameInstance>(group.data.symbolFrameInstances);
	}

	public KAnimBatchGroup group { get; private set; }

	public Dictionary<KAnimHashedString, List<KAnim.Build.SymbolFrameInstance>> overriddenSymbols { get; private set; }

	public Dictionary<KAnimHashedString, HashedString> overrideSourceFile { get; private set; }

	public bool requiresRebuild { get; private set; }

	public List<KAnim.Build.SymbolFrameInstance> symbolFrameInstances { get; private set; }

	public List<Texture2D> textures { get; private set; }

	public void DestroyTex()
	{
		if (this.buildTex != null)
		{
			global::UnityEngine.Object.Destroy(this.buildTex);
			this.buildTex = null;
		}
	}

	public void Rebuild(MaterialPropertyBlock matProperties)
	{
		this.group.InitBuild(this);
		matProperties.SetTexture("buildTex", this.buildTex);
		this.requiresRebuild = false;
	}

	public void ClearOverrides()
	{
		Dictionary<KAnimHashedString, List<KAnim.Build.SymbolFrameInstance>>.Enumerator enumerator = this.overriddenSymbols.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KBatchGroupData data = this.group.data;
			KeyValuePair<KAnimHashedString, List<KAnim.Build.SymbolFrameInstance>> keyValuePair = enumerator.Current;
			KAnim.Build.Symbol buildSymbol = data.GetBuildSymbol(keyValuePair.Key);
			for (int i = 0; i < buildSymbol.numFrames; i++)
			{
				List<KAnim.Build.SymbolFrameInstance> symbolFrameInstances = this.symbolFrameInstances;
				int num = buildSymbol.firstFrameIdx + i;
				KeyValuePair<KAnimHashedString, List<KAnim.Build.SymbolFrameInstance>> keyValuePair2 = enumerator.Current;
				symbolFrameInstances[num] = keyValuePair2.Value[i];
			}
		}
		foreach (KeyValuePair<KAnimHashedString, HashedString> keyValuePair3 in this.overrideSourceFile)
		{
			this.RemoveOverrideTarget(keyValuePair3.Key);
		}
		this.overrideSourceFile.Clear();
		this.requiresRebuild = true;
	}

	private void SwapSFI(KAnim.Build.Symbol targetSymbol, int target_frame_idx, int override_frame_idx)
	{
		KAnim.Build.SymbolFrameInstance symbolFrameInstance = this.symbolFrameInstances[targetSymbol.firstFrameIdx + target_frame_idx];
		KAnim.Build.SymbolFrameInstance symbolFrameInstance2 = this.symbolFrameInstances[targetSymbol.firstFrameIdx + override_frame_idx];
		KAnim.Build.SymbolFrame symbolFrame = symbolFrameInstance.symbolFrame;
		symbolFrameInstance.symbolFrame = symbolFrameInstance2.symbolFrame;
		symbolFrameInstance2.symbolFrame = symbolFrame;
		int buildImageIdx = symbolFrameInstance.buildImageIdx;
		symbolFrameInstance.buildImageIdx = symbolFrameInstance2.buildImageIdx;
		symbolFrameInstance2.buildImageIdx = buildImageIdx;
		this.symbolFrameInstances[targetSymbol.firstFrameIdx + target_frame_idx] = symbolFrameInstance;
		this.symbolFrameInstances[targetSymbol.firstFrameIdx + override_frame_idx] = symbolFrameInstance2;
		this.requiresRebuild = true;
	}

	public void RemoveOverrideSymbolFrameInstance(KAnimHashedString target)
	{
		KAnim.Build.Symbol buildSymbol = this.group.data.GetBuildSymbol(target);
		int num = this.swaps.FindIndex((BatchGroupInstance.SFSwap sfs) => sfs.target == target);
		if (num != -1)
		{
			this.SwapSFI(buildSymbol, this.swaps[num].override_frame, this.swaps[num].targ_frame);
			this.swaps.RemoveAt(num);
		}
	}

	public void OverrideSymbolFrameInstance(KAnimHashedString target, int target_frame_idx, int override_frame_idx)
	{
		if (target_frame_idx == override_frame_idx)
		{
			return;
		}
		KAnim.Build.Symbol buildSymbol = this.group.data.GetBuildSymbol(target);
		int num = this.swaps.FindIndex((BatchGroupInstance.SFSwap sfs) => sfs.target == target);
		if (num != -1)
		{
			this.SwapSFI(buildSymbol, this.swaps[num].override_frame, this.swaps[num].targ_frame);
			this.swaps.RemoveAt(num);
		}
		this.SwapSFI(buildSymbol, target_frame_idx, override_frame_idx);
		BatchGroupInstance.SFSwap sfswap = default(BatchGroupInstance.SFSwap);
		sfswap.target = target;
		sfswap.targ_frame = target_frame_idx;
		sfswap.override_frame = override_frame_idx;
		this.swaps.Add(sfswap);
	}

	private void RemoveOverrideTarget(KAnimHashedString target)
	{
		if (this.texIndexes.ContainsKey(target))
		{
			List<int> list = this.texIndexes[target];
			for (int i = 0; i < list.Count; i++)
			{
				Debug.Assert(list[i] >= 0);
				this.texUse[list[i]].Remove(target);
				if (this.texUse[list[i]].Count == 0)
				{
					this.textures[list[i]] = null;
				}
			}
			this.texIndexes.Remove(target);
		}
	}

	private void AddOverrideTarget(KAnimHashedString target, HashedString source, List<int> textureIndexList, KAnimHashedString sourceSymbol)
	{
		this.overrideSourceFile[target] = source;
		bool flag = false;
		for (int i = 0; i < textureIndexList.Count; i++)
		{
			int num2;
			int num = (num2 = i);
			num2 = textureIndexList[num2];
			textureIndexList[num] = num2 - this.group.data.textures.Count;
			if (textureIndexList[i] >= 0)
			{
				if (!this.texUse.ContainsKey(textureIndexList[i]))
				{
					this.texUse[textureIndexList[i]] = new List<KAnimHashedString>();
				}
				this.texUse[textureIndexList[i]].Add(target);
				flag = true;
			}
		}
		if (flag)
		{
			this.texIndexes[target] = textureIndexList;
		}
	}

	public void RemoveOverride(KAnimHashedString target)
	{
		if (!this.overriddenSymbols.ContainsKey(target))
		{
			return;
		}
		KAnim.Build.Symbol buildSymbol = this.group.data.GetBuildSymbol(target);
		for (int i = 0; i < this.overriddenSymbols[target].Count; i++)
		{
			this.symbolFrameInstances[buildSymbol.firstFrameIdx + i] = this.overriddenSymbols[target][i];
		}
		this.RemoveOverrideTarget(target);
		this.overrideSourceFile.Remove(target);
		this.requiresRebuild = true;
	}

	public bool AddOverride(KAnimHashedString target, HashedString source, List<KAnim.Build.SymbolFrameInstance> substituteFrames, List<int> textureIndexList, KAnimHashedString srcPath)
	{
		Debug.Assert(source != this.group.batchID, "Cant override if its already in the batch");
		KAnim.Build.Symbol buildSymbol = this.group.data.GetBuildSymbol(target);
		Debug.Assert(buildSymbol != null && buildSymbol.numFrames > 0);
		if (!this.overriddenSymbols.ContainsKey(target))
		{
			List<KAnim.Build.SymbolFrameInstance> list = new List<KAnim.Build.SymbolFrameInstance>();
			for (int i = 0; i < buildSymbol.numFrames; i++)
			{
				list.Add(buildSymbol.GetFrame(i));
			}
			if (list.Count == 0)
			{
				return false;
			}
			this.overriddenSymbols[target] = list;
		}
		else if (this.texIndexes.ContainsKey(target))
		{
			this.RemoveOverrideTarget(target);
			this.overrideSourceFile.Remove(target);
		}
		for (int j = 0; j < buildSymbol.numFrames; j++)
		{
			int num = j;
			if (j >= substituteFrames.Count)
			{
				num = substituteFrames.Count - 1;
			}
			KAnim.Build.SymbolFrameInstance symbolFrameInstance = substituteFrames[num];
			symbolFrameInstance.symbolIdx = this.overriddenSymbols[target][num].symbolIdx;
			substituteFrames[num] = symbolFrameInstance;
			this.symbolFrameInstances[buildSymbol.firstFrameIdx + j] = substituteFrames[num];
		}
		this.AddOverrideTarget(target, source, textureIndexList, srcPath);
		this.requiresRebuild = true;
		return true;
	}

	public bool AddOverrideTexture(Texture2D atlas, ref int index)
	{
		Debug.Assert(atlas != null);
		index = this.group.data.textures.FindIndex((Texture2D a) => a == atlas);
		if (index != -1)
		{
			return false;
		}
		index = this.textures.FindIndex((Texture2D a) => a == atlas);
		if (index == -1)
		{
			index = this.textures.FindIndex((Texture2D a) => a == null);
			if (index != -1)
			{
				this.textures[index] = atlas;
				index += this.group.data.textures.Count;
			}
			else
			{
				index = this.textures.Count + this.group.data.textures.Count;
				this.textures.Add(atlas);
			}
			return true;
		}
		index += this.group.data.textures.Count;
		Debug.Assert(index < this.textures.Count + this.group.data.textures.Count);
		return false;
	}

	public Texture2D buildTex;

	private List<BatchGroupInstance.SFSwap> swaps = new List<BatchGroupInstance.SFSwap>();

	private Dictionary<int, List<KAnimHashedString>> texUse = new Dictionary<int, List<KAnimHashedString>>();

	private Dictionary<KAnimHashedString, List<int>> texIndexes = new Dictionary<KAnimHashedString, List<int>>();

	private struct SFSwap
	{
		public KAnimHashedString target;

		public int targ_frame;

		public int override_frame;
	}
}
