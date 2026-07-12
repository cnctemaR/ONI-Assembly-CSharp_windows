using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SymbolOverrideController")]
public class SymbolOverrideController : KMonoBehaviour
{
	public SymbolOverrideController.SymbolEntry[] GetSymbolOverrides
	{
		get
		{
			return this.symbolOverrides.ToArray();
		}
	}

	public int version { get; private set; }

	protected override void OnPrefabInit()
	{
		this.animController = base.GetComponent<KBatchedAnimController>();
		DebugUtil.Assert(base.GetComponent<KBatchedAnimController>() != null, "SymbolOverrideController requires KBatchedAnimController");
		DebugUtil.Assert(base.GetComponent<KBatchedAnimController>().usingNewSymbolOverrideSystem, "SymbolOverrideController requires usingNewSymbolOverrideSystem to be set to true. Try adding the component by calling: SymbolOverrideControllerUtil.AddToPrefab");
		for (int i = 0; i < this.symbolOverrides.Count; i++)
		{
			SymbolOverrideController.SymbolEntry symbolEntry = this.symbolOverrides[i];
			symbolEntry.sourceSymbol = KAnimBatchManager.Instance().GetBatchGroupData(symbolEntry.sourceSymbolBatchTag).GetSymbol(symbolEntry.sourceSymbolId);
			this.symbolOverrides[i] = symbolEntry;
		}
		this.atlases = new KAnimBatch.AtlasList(0);
		this.faceGraph = base.GetComponent<FaceGraph>();
	}

	public int AddSymbolOverride(HashedString target_symbol, KAnim.Build.Symbol source_symbol, int priority = 0)
	{
		if (source_symbol == null)
		{
			throw new Exception("NULL source symbol when overriding: " + target_symbol.ToString());
		}
		SymbolOverrideController.SymbolEntry symbolEntry = new SymbolOverrideController.SymbolEntry
		{
			targetSymbol = target_symbol,
			sourceSymbol = source_symbol,
			sourceSymbolId = new HashedString(source_symbol.hash.HashValue),
			sourceSymbolBatchTag = source_symbol.build.batchTag,
			priority = priority
		};
		int num = this.GetSymbolOverrideIdx(target_symbol, priority);
		if (num >= 0)
		{
			this.symbolOverrides[num] = symbolEntry;
		}
		else
		{
			num = this.symbolOverrides.Count;
			this.symbolOverrides.Add(symbolEntry);
		}
		this.MarkDirty();
		return num;
	}

	public void RemoveSymbolOverride(HashedString target_symbol, int priority = 0)
	{
		for (int i = 0; i < this.symbolOverrides.Count; i++)
		{
			SymbolOverrideController.SymbolEntry symbolEntry = this.symbolOverrides[i];
			if (symbolEntry.targetSymbol == target_symbol && symbolEntry.priority == priority)
			{
				this.symbolOverrides.RemoveAt(i);
				break;
			}
		}
		this.MarkDirty();
	}

	public void RemoveAllSymbolOverrides(int priority = 0)
	{
		this.symbolOverrides.RemoveAll((SymbolOverrideController.SymbolEntry x) => x.priority >= priority);
		this.MarkDirty();
	}

	public int GetSymbolOverrideIdx(HashedString target_symbol, int priority = 0)
	{
		for (int i = 0; i < this.symbolOverrides.Count; i++)
		{
			SymbolOverrideController.SymbolEntry symbolEntry = this.symbolOverrides[i];
			if (symbolEntry.targetSymbol == target_symbol && symbolEntry.priority == priority)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetAtlasIdx(Texture2D atlas)
	{
		return this.atlases.GetAtlasIdx(atlas);
	}

	public void ApplyOverrides()
	{
		if (this.requiresSorting)
		{
			this.symbolOverrides.Sort((SymbolOverrideController.SymbolEntry x, SymbolOverrideController.SymbolEntry y) => x.priority - y.priority);
			this.requiresSorting = false;
		}
		KAnimBatch batch = this.animController.GetBatch();
		DebugUtil.Assert(batch != null);
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.animController.batchGroupID);
		int count = batch.atlases.Count;
		this.atlases.Clear(count);
		DictionaryPool<HashedString, Pair<int, int>, SymbolOverrideController>.PooledDictionary pooledDictionary = DictionaryPool<HashedString, Pair<int, int>, SymbolOverrideController>.Allocate();
		ListPool<SymbolOverrideController.SymbolEntry, SymbolOverrideController>.PooledList pooledList = ListPool<SymbolOverrideController.SymbolEntry, SymbolOverrideController>.Allocate();
		for (int i = 0; i < this.symbolOverrides.Count; i++)
		{
			SymbolOverrideController.SymbolEntry symbolEntry = this.symbolOverrides[i];
			Pair<int, int> pair;
			if (pooledDictionary.TryGetValue(symbolEntry.targetSymbol, out pair))
			{
				int first = pair.first;
				if (symbolEntry.priority > first)
				{
					int second = pair.second;
					pooledDictionary[symbolEntry.targetSymbol] = new Pair<int, int>(symbolEntry.priority, second);
					pooledList[second] = symbolEntry;
				}
			}
			else
			{
				pooledDictionary[symbolEntry.targetSymbol] = new Pair<int, int>(symbolEntry.priority, pooledList.Count);
				pooledList.Add(symbolEntry);
			}
		}
		DictionaryPool<KAnim.Build, SymbolOverrideController.BatchGroupInfo, SymbolOverrideController>.PooledDictionary pooledDictionary2 = DictionaryPool<KAnim.Build, SymbolOverrideController.BatchGroupInfo, SymbolOverrideController>.Allocate();
		for (int j = 0; j < pooledList.Count; j++)
		{
			SymbolOverrideController.SymbolEntry symbolEntry2 = pooledList[j];
			SymbolOverrideController.BatchGroupInfo batchGroupInfo;
			if (!pooledDictionary2.TryGetValue(symbolEntry2.sourceSymbol.build, out batchGroupInfo))
			{
				batchGroupInfo = new SymbolOverrideController.BatchGroupInfo
				{
					build = symbolEntry2.sourceSymbol.build,
					data = KAnimBatchManager.Instance().GetBatchGroupData(symbolEntry2.sourceSymbol.build.batchTag)
				};
				Texture2D texture = symbolEntry2.sourceSymbol.build.GetTexture(0);
				int num = batch.atlases.GetAtlasIdx(texture);
				if (num < 0)
				{
					num = this.atlases.Add(texture);
				}
				batchGroupInfo.atlasIdx = num;
				pooledDictionary2[batchGroupInfo.build] = batchGroupInfo;
			}
			KAnim.Build.Symbol symbol = batchGroupData.GetSymbol(symbolEntry2.targetSymbol);
			if (symbol != null)
			{
				this.animController.SetSymbolOverrides(symbol.firstFrameIdx, symbol.numFrames, batchGroupInfo.atlasIdx, batchGroupInfo.data, symbolEntry2.sourceSymbol.firstFrameIdx, symbolEntry2.sourceSymbol.numFrames);
			}
		}
		pooledDictionary2.Recycle();
		pooledList.Recycle();
		pooledDictionary.Recycle();
		if (this.faceGraph != null)
		{
			this.faceGraph.ApplyShape();
		}
	}

	public void ApplyAtlases()
	{
		KAnimBatch batch = this.animController.GetBatch();
		this.atlases.Apply(batch.matProperties);
	}

	public KAnimBatch.AtlasList GetAtlasList()
	{
		return this.atlases;
	}

	public void MarkDirty()
	{
		if (this.animController != null)
		{
			this.animController.SetDirty();
		}
		int num = this.version + 1;
		this.version = num;
		this.requiresSorting = true;
	}

	public bool applySymbolOverridesEveryFrame;

	[SerializeField]
	private List<SymbolOverrideController.SymbolEntry> symbolOverrides = new List<SymbolOverrideController.SymbolEntry>();

	private KAnimBatch.AtlasList atlases;

	private KBatchedAnimController animController;

	private FaceGraph faceGraph;

	private bool requiresSorting;

	[Serializable]
	public struct SymbolEntry
	{
		public HashedString targetSymbol;

		[NonSerialized]
		public KAnim.Build.Symbol sourceSymbol;

		public HashedString sourceSymbolId;

		public HashedString sourceSymbolBatchTag;

		public int priority;
	}

	private struct SymbolToOverride
	{
		public KAnim.Build.Symbol sourceSymbol;

		public HashedString targetSymbol;

		public KBatchGroupData data;

		public int atlasIdx;
	}

	private class BatchGroupInfo
	{
		public KAnim.Build build;

		public int atlasIdx;

		public KBatchGroupData data;
	}
}
