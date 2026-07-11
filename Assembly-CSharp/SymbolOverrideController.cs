using System;
using System.Collections.Generic;
using UnityEngine;

public class SymbolOverrideController : KMonoBehaviour
{
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

	public void AddSymbolOverride(HashedString target_symbol, KAnim.Build.Symbol source_symbol, int priority = 0)
	{
		SymbolOverrideController.SymbolEntry symbolEntry = new SymbolOverrideController.SymbolEntry
		{
			targetSymbol = target_symbol,
			sourceSymbol = source_symbol,
			sourceSymbolId = new HashedString(source_symbol.hash.HashValue),
			sourceSymbolBatchTag = source_symbol.build.batchTag,
			priority = priority
		};
		int symbolOverrideIdx = this.GetSymbolOverrideIdx(target_symbol, priority);
		if (symbolOverrideIdx >= 0)
		{
			this.symbolOverrides[symbolOverrideIdx] = symbolEntry;
		}
		else
		{
			this.symbolOverrides.Add(symbolEntry);
		}
		this.MarkDirty();
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
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		KAnimBatch batch = component.GetBatch();
		DebugUtil.Assert(batch != null, "Assert!");
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(component.batchGroupID);
		int count = batch.atlases.Count;
		this.atlases.Clear(count);
		ListPool<SymbolOverrideController.SymbolToOverride, SymbolOverrideController>.PooledList pooledList = ListPool<SymbolOverrideController.SymbolToOverride, SymbolOverrideController>.Allocate();
		ListPool<SymbolOverrideController.BatchGroupInfo, SymbolOverrideController>.PooledList pooledList2 = ListPool<SymbolOverrideController.BatchGroupInfo, SymbolOverrideController>.Allocate();
		foreach (SymbolOverrideController.SymbolEntry symbolEntry in this.symbolOverrides)
		{
			SymbolOverrideController.BatchGroupInfo batchGroupInfo = default(SymbolOverrideController.BatchGroupInfo);
			foreach (SymbolOverrideController.BatchGroupInfo batchGroupInfo2 in pooledList2)
			{
				if (symbolEntry.sourceSymbol.build == batchGroupInfo2.build)
				{
					batchGroupInfo = batchGroupInfo2;
				}
			}
			if (batchGroupInfo.build == null)
			{
				batchGroupInfo = new SymbolOverrideController.BatchGroupInfo
				{
					build = symbolEntry.sourceSymbol.build,
					data = KAnimBatchManager.Instance().GetBatchGroupData(symbolEntry.sourceSymbol.build.batchTag)
				};
				Texture2D texture = symbolEntry.sourceSymbol.build.GetTexture(0);
				int num = this.atlases.Add(texture);
				batchGroupInfo.atlasIdx = num;
				pooledList2.Add(batchGroupInfo);
			}
			pooledList.Add(new SymbolOverrideController.SymbolToOverride
			{
				sourceSymbol = symbolEntry.sourceSymbol,
				targetSymbol = symbolEntry.targetSymbol,
				data = batchGroupInfo.data,
				atlasIdx = batchGroupInfo.atlasIdx
			});
		}
		pooledList2.Recycle();
		foreach (SymbolOverrideController.SymbolToOverride symbolToOverride in pooledList)
		{
			KAnim.Build.Symbol symbol = batchGroupData.GetSymbol(symbolToOverride.targetSymbol);
			if (symbol != null)
			{
				KAnim.Build.Symbol sourceSymbol = symbolToOverride.sourceSymbol;
				for (int i = 0; i < symbol.numFrames; i++)
				{
					int num2 = Math.Min(sourceSymbol.numFrames - 1, i);
					KAnim.Build.SymbolFrameInstance symbolFrameInstance = symbolToOverride.data.symbolFrameInstances[sourceSymbol.firstFrameIdx + num2];
					symbolFrameInstance.buildImageIdx = symbolToOverride.atlasIdx;
					component.SetSymbolOverride(symbol.firstFrameIdx + i, symbolFrameInstance);
				}
			}
		}
		pooledList.Recycle();
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

	public void MarkDirty()
	{
		if (this.animController != null)
		{
			this.animController.SetDirty();
		}
		this.version++;
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
	private struct SymbolEntry
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

	private struct BatchGroupInfo
	{
		public KAnim.Build build;

		public int atlasIdx;

		public KBatchGroupData data;
	}
}
