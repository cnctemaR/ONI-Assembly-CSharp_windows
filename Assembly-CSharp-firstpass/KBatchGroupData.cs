using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class KBatchGroupData
{
	public HashedString groupID { get; private set; }

	public bool isSwap { get; private set; }

	public int maxVisibleSymbols { get; private set; }

	public int maxSymbolsPerBuild
	{
		get
		{
			return this.frameElementSymbols.Count;
		}
	}

	public int maxSymbolFrameInstancesPerbuild
	{
		get
		{
			return this.symbolFrameInstances.Count;
		}
	}

	public int animDataStartOffset
	{
		get
		{
			return this.symbolFrameInstances.Count * 12;
		}
	}

	public List<KAnim.Anim> anims { get; private set; }

	public Dictionary<KAnimHashedString, int> animIndex { get; private set; }

	public List<KAnim.Anim.Frame> animFrames { get; private set; }

	public List<KAnim.Anim.FrameElement> frameElements { get; private set; }

	public List<KAnim.Build> builds { get; private set; }

	public List<KAnim.Build.Symbol> frameElementSymbols { get; private set; }

	public Dictionary<KAnimHashedString, int> frameElementSymbolIndices { get; private set; }

	public List<KAnim.Build.SymbolFrameInstance> symbolFrameInstances { get; private set; }

	public Dictionary<KAnimHashedString, int> textureStartIndex { get; private set; }

	public List<Texture2D> textures { get; private set; }

	public KBatchGroupData(HashedString id)
	{
		this.groupID = id;
		this.maxVisibleSymbols = 1;
		this.Init();
	}

	private void Init()
	{
		this.anims = new List<KAnim.Anim>();
		this.animIndex = new Dictionary<KAnimHashedString, int>();
		this.animFrames = new List<KAnim.Anim.Frame>();
		this.frameElements = new List<KAnim.Anim.FrameElement>();
		this.builds = new List<KAnim.Build>();
		this.frameElementSymbols = new List<KAnim.Build.Symbol>();
		this.frameElementSymbolIndices = new Dictionary<KAnimHashedString, int>();
		this.symbolFrameInstances = new List<KAnim.Build.SymbolFrameInstance>();
		this.textures = new List<Texture2D>();
		this.textureStartIndex = new Dictionary<KAnimHashedString, int>();
	}

	public void FreeResources()
	{
		if (this.anims != null)
		{
			this.anims.Clear();
			this.anims = null;
		}
		if (this.animIndex != null)
		{
			this.animIndex.Clear();
			this.animIndex = null;
		}
		if (this.animFrames != null)
		{
			this.animFrames.Clear();
			this.animFrames = null;
		}
		if (this.frameElements != null)
		{
			this.frameElements.Clear();
			this.frameElements = null;
		}
		if (this.builds != null)
		{
			this.builds.Clear();
			this.builds = null;
		}
		if (this.frameElementSymbols != null)
		{
			this.frameElementSymbols.Clear();
			this.frameElementSymbols = null;
		}
		if (this.symbolFrameInstances != null)
		{
			this.symbolFrameInstances.Clear();
			this.symbolFrameInstances = null;
		}
		if (this.textures != null)
		{
			this.textures.Clear();
			this.textures = null;
		}
		if (this.textureStartIndex != null)
		{
			this.textureStartIndex.Clear();
			this.textureStartIndex = null;
		}
	}

	public KAnim.Build AddNewBuildFile(KAnimHashedString fileHash)
	{
		this.textureStartIndex.Add(fileHash, this.textures.Count);
		KAnim.Build build = new KAnim.Build();
		build.textureStartIdx = this.textures.Count;
		build.fileHash = fileHash;
		build.index = this.builds.Count;
		this.builds.Add(build);
		return build;
	}

	public void AddTextures(List<Texture2D> buildtextures)
	{
		this.textures.AddRange(buildtextures);
	}

	public void AddAnim(KAnim.Anim anim)
	{
		global::Debug.Assert(anim.index == this.anims.Count);
		this.anims.Add(anim);
	}

	public KAnim.Anim GetAnim(int anim)
	{
		if (anim < 0 || anim >= this.anims.Count)
		{
			global::Debug.LogError(string.Format("Anim [{0}] out of range [{1}] in batch [{2}]", anim, this.anims.Count, this.groupID));
		}
		return this.anims[anim];
	}

	public KAnim.Build GetBuild(int index)
	{
		return this.builds[index];
	}

	public void UpdateMaxVisibleSymbols(int newCount)
	{
		this.maxVisibleSymbols = Mathf.Min(120, Mathf.Max(this.maxVisibleSymbols, newCount));
	}

	public KAnim.Build.Symbol GetSymbol(KAnimHashedString symbol_name)
	{
		int num = 0;
		if (!this.frameElementSymbolIndices.TryGetValue(symbol_name, out num))
		{
			return null;
		}
		return this.frameElementSymbols[num];
	}

	public KAnim.Build.Symbol GetSymbol(int index)
	{
		if (index >= 0 && index < this.frameElementSymbols.Count)
		{
			return this.frameElementSymbols[index];
		}
		return null;
	}

	public void AddBuildSymbol(KAnim.Build.Symbol symbol)
	{
		if (!this.frameElementSymbolIndices.ContainsKey(symbol.hash))
		{
			this.frameElementSymbolIndices.Add(symbol.hash, this.frameElementSymbols.Count);
		}
		this.frameElementSymbols.Add(symbol);
	}

	public int GetSymbolCount()
	{
		return this.frameElementSymbols.Count;
	}

	public KAnim.Build.SymbolFrameInstance GetSymbolFrameInstance(int index)
	{
		if (index >= 0 && index < this.symbolFrameInstances.Count)
		{
			return this.symbolFrameInstances[index];
		}
		return new KAnim.Build.SymbolFrameInstance
		{
			symbolIdx = -1
		};
	}

	public Texture2D GetTexure(int index)
	{
		if (index < 0 || this.textures == null || index >= this.textures.Count)
		{
			return null;
		}
		return this.textures[index];
	}

	public KAnim.Build.Symbol GetBuildSymbol(int idx)
	{
		if (this.frameElementSymbols == null || idx < 0 || idx >= this.frameElementSymbols.Count)
		{
			return null;
		}
		return this.frameElementSymbols[idx];
	}

	public KAnim.Anim.Frame GetFrame(int index)
	{
		if (index < 0 || index >= this.animFrames.Count)
		{
			return KAnim.Anim.Frame.InvalidFrame;
		}
		return this.animFrames[index];
	}

	public KAnim.Anim.FrameElement GetFrameElement(int index)
	{
		return this.frameElements[index];
	}

	public List<KAnim.Anim.Frame> GetAnimFrames()
	{
		return this.animFrames;
	}

	public List<KAnim.Anim.FrameElement> GetAnimFrameElements()
	{
		return this.frameElements;
	}

	public int GetBuildSymbolFrameCount()
	{
		return this.symbolFrameInstances.Count;
	}

	public void WriteAnimData(int start_index, NativeArray<float> data)
	{
		List<KAnim.Anim.Frame> animFrames = this.GetAnimFrames();
		List<KAnim.Anim.FrameElement> animFrameElements = this.GetAnimFrameElements();
		int num = 1 + ((animFrames.Count == 0) ? this.symbolFrameInstances.Count : animFrames.Count);
		if (animFrames.Count == 0 && this.symbolFrameInstances.Count == 0 && animFrameElements.Count == 0)
		{
			global::Debug.LogError(string.Concat(new string[]
			{
				"Eh, no data ",
				animFrames.Count.ToString(),
				" ",
				this.symbolFrameInstances.Count.ToString(),
				" ",
				animFrameElements.Count.ToString()
			}));
		}
		data[start_index++] = (float)num;
		data[start_index++] = (float)animFrames.Count;
		data[start_index++] = (float)animFrameElements.Count;
		data[start_index++] = (float)this.symbolFrameInstances.Count;
		if (animFrames.Count == 0)
		{
			for (int i = 0; i < this.symbolFrameInstances.Count; i++)
			{
				this.WriteAnimFrame(data, start_index, i, 1);
				start_index += 4;
			}
			for (int j = 0; j < this.symbolFrameInstances.Count; j++)
			{
				this.WriteAnimFrameElement(data, start_index, j, Matrix2x3.identity, 1f);
				start_index += 8;
			}
			return;
		}
		for (int k = 0; k < animFrames.Count; k++)
		{
			this.WriteAnimFrame(data, start_index, animFrames[k].firstElementIdx, animFrames[k].numElements);
			start_index += 4;
		}
		for (int l = 0; l < animFrameElements.Count; l++)
		{
			KAnim.Anim.FrameElement frameElement = animFrameElements[l];
			if (frameElement.symbol == KGlobalAnimParser.MISSING_SYMBOL)
			{
				this.WriteAnimFrameElement(data, start_index, -1, Matrix2x3.identity, 1f);
			}
			else
			{
				KAnim.Build.Symbol buildSymbol = this.GetBuildSymbol(this.GetSymbolIndex(frameElement.symbol));
				if (buildSymbol == null)
				{
					string[] array = new string[5];
					array[0] = "Missing symbol for Anim Frame Element: [";
					array[1] = HashCache.Get().Get(frameElement.symbol);
					array[2] = ": ";
					int num2 = 3;
					KAnimHashedString symbol = frameElement.symbol;
					array[num2] = symbol.ToString();
					array[4] = "]";
					global::Debug.LogError(string.Concat(array));
				}
				int frameIdx = buildSymbol.GetFrameIdx(frameElement.frame);
				this.WriteAnimFrameElement(data, start_index, frameIdx, frameElement.transform, frameElement.multAlpha);
			}
			start_index += 8;
		}
	}

	public int GetFirstIndex(KAnimHashedString symbol)
	{
		return this.frameElementSymbols.FindIndex((KAnim.Build.Symbol fes) => fes.hash == symbol);
	}

	public int GetSymbolIndex(KAnimHashedString symbol)
	{
		int num = 0;
		if (!this.frameElementSymbolIndices.TryGetValue(symbol, out num))
		{
			return -1;
		}
		return num;
	}

	public int WriteBuildData(List<KAnim.Build.SymbolFrameInstance> symbol_frame_instances, NativeArray<float> data)
	{
		for (int i = 0; i < symbol_frame_instances.Count; i++)
		{
			this.Write(data, i * 12, i, this.symbolFrameInstances[i].buildImageIdx, symbol_frame_instances[i]);
		}
		return symbol_frame_instances.Count * 12;
	}

	private void Write(NativeArray<float> data, int startIndex, int thisFrameIndex, int atlasIndex, KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		data[startIndex] = (float)atlasIndex;
		KAnim.Build.SymbolFrame symbolFrame = symbol_frame_instance.symbolFrame;
		KAnim.Build.Symbol buildSymbol = this.GetBuildSymbol(symbol_frame_instance.symbolIdx);
		if (buildSymbol == null || symbolFrame == null)
		{
			data[startIndex + 1] = 0f;
			data[startIndex + 2] = 0f;
		}
		else
		{
			data[startIndex + 1] = (float)buildSymbol.flags;
			data[startIndex + 2] = (float)buildSymbol.symbolIndexInSourceBuild;
		}
		data[startIndex + 3] = 3.452817E+09f;
		if (symbolFrame == null)
		{
			return;
		}
		data[startIndex + 4] = symbolFrame.bboxMin.x;
		data[startIndex + 5] = symbolFrame.bboxMin.y;
		data[startIndex + 6] = symbolFrame.bboxMax.x;
		data[startIndex + 7] = symbolFrame.bboxMax.y;
		data[startIndex + 8] = symbolFrame.uvMin.x;
		data[startIndex + 9] = symbolFrame.uvMin.y;
		data[startIndex + 10] = symbolFrame.uvMax.x;
		data[startIndex + 11] = symbolFrame.uvMax.y;
	}

	private void WriteAnimFrame(NativeArray<float> data, int startIndex, int firstElementIdx, int numElements)
	{
		data[startIndex] = (float)firstElementIdx;
		data[startIndex + 1] = (float)numElements;
	}

	private void WriteAnimFrameElement(NativeArray<float> data, int startIndex, int symbolFrameIdx, Matrix2x3 transform, float multAlpha)
	{
		if (symbolFrameIdx != -1010)
		{
			data[startIndex] = (float)symbolFrameIdx;
			data[startIndex + 1] = multAlpha;
			data[startIndex + 2] = transform.m00;
			data[startIndex + 3] = transform.m01;
			data[startIndex + 4] = transform.m02;
			data[startIndex + 5] = transform.m10;
			data[startIndex + 6] = transform.m11;
			data[startIndex + 7] = transform.m12;
			return;
		}
		data[startIndex] = (float)symbolFrameIdx;
		data[startIndex + 1] = multAlpha;
		data[startIndex + 2] = 0f;
		data[startIndex + 3] = 0f;
		data[startIndex + 4] = 0f;
		data[startIndex + 5] = 0f;
		data[startIndex + 6] = 0f;
		data[startIndex + 7] = 0f;
	}

	public const int SIZE_OF_SYMBOL_FRAME_ELEMENT = 12;

	public const int SIZE_OF_ANIM_FRAME = 4;

	public const int SIZE_OF_ANIM_FRAME_ELEMENT = 8;

	private const int MAX_VISIBLE_SYMBOLS = 120;

	public const int MAX_GROUP_SIZE = 30;

	private const int NULL_DATA_FRAME_ID = -1010;
}
