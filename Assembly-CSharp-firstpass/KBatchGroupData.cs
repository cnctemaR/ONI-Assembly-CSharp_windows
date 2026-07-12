using System;
using System.Collections.Generic;
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
			return this.symbolFrameInstances.Count * 16;
		}
	}

	public List<KAnim.Anim> anims { get; private set; }

	public Dictionary<KAnimHashedString, int> animIndex { get; private set; }

	public Dictionary<KAnimHashedString, int> animCount { get; private set; }

	public List<KAnim.Anim.Frame> animFrames { get; private set; }

	public List<KAnim.Anim.FrameElement> frameElements { get; private set; }

	public List<KAnim.Build> builds { get; private set; }

	public List<KAnim.Build.Symbol> frameElementSymbols { get; private set; }

	public Dictionary<KAnimHashedString, int> frameElementSymbolIndices { get; private set; }

	public List<KAnim.Build.SymbolFrameInstance> symbolFrameInstances { get; private set; }

	public Dictionary<KAnimHashedString, int> textureStartIndex { get; private set; }

	public Dictionary<KAnimHashedString, int> firstSymbolIndex { get; private set; }

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
		this.animCount = new Dictionary<KAnimHashedString, int>();
		this.animFrames = new List<KAnim.Anim.Frame>();
		this.frameElements = new List<KAnim.Anim.FrameElement>();
		this.builds = new List<KAnim.Build>();
		this.frameElementSymbols = new List<KAnim.Build.Symbol>();
		this.frameElementSymbolIndices = new Dictionary<KAnimHashedString, int>();
		this.symbolFrameInstances = new List<KAnim.Build.SymbolFrameInstance>();
		this.textures = new List<Texture2D>();
		this.textureStartIndex = new Dictionary<KAnimHashedString, int>();
		this.firstSymbolIndex = new Dictionary<KAnimHashedString, int>();
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
		if (this.animCount != null)
		{
			this.animCount.Clear();
			this.animCount = null;
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
		if (this.firstSymbolIndex != null)
		{
			this.firstSymbolIndex.Clear();
			this.firstSymbolIndex = null;
		}
	}

	public KAnim.Build AddNewBuildFile(KAnimHashedString fileHash)
	{
		this.textureStartIndex.Add(fileHash, this.textures.Count);
		this.firstSymbolIndex.Add(fileHash, this.GetSymbolCount());
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

	public void WriteAnimData(int start_index, float[] data)
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
				this.WriteAnimFrame(data, start_index, i, i, 1, i);
				start_index += 4;
			}
			for (int j = 0; j < this.symbolFrameInstances.Count; j++)
			{
				this.WriteAnimFrameElement(data, start_index, j, j, Matrix2x3.identity, Color.white, 0);
				start_index += 16;
			}
			return;
		}
		for (int k = 0; k < animFrames.Count; k++)
		{
			this.Write(data, start_index, k, animFrames[k]);
			start_index += 4;
		}
		for (int l = 0; l < animFrameElements.Count; l++)
		{
			KAnim.Anim.FrameElement frameElement = animFrameElements[l];
			if (frameElement.symbol == KGlobalAnimParser.MISSING_SYMBOL)
			{
				this.WriteAnimFrameElement(data, start_index, -1, l, Matrix2x3.identity, Color.white, 0);
			}
			else
			{
				KAnim.Build.Symbol buildSymbol = this.GetBuildSymbol(frameElement.symbolIdx);
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
				this.Write(data, start_index, frameIdx, l, frameElement);
			}
			start_index += 16;
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

	public int WriteBuildData(List<KAnim.Build.SymbolFrameInstance> symbol_frame_instances, float[] data)
	{
		int i;
		for (i = 0; i < symbol_frame_instances.Count; i++)
		{
			this.Write(data, i * 16, i, this.symbolFrameInstances[i].buildImageIdx, symbol_frame_instances[i]);
		}
		return i * 16;
	}

	private void Write(float[] data, int startIndex, int thisFrameIndex, int atlasIndex, KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		data[startIndex++] = (float)atlasIndex;
		data[startIndex++] = (float)thisFrameIndex;
		data[startIndex++] = (float)symbol_frame_instance.symbolIdx;
		KAnim.Build.SymbolFrame symbolFrame = symbol_frame_instance.symbolFrame;
		KAnim.Build.Symbol buildSymbol = this.GetBuildSymbol(symbol_frame_instance.symbolIdx);
		if (buildSymbol == null || symbolFrame == null)
		{
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
			data[startIndex++] = 0f;
		}
		else
		{
			data[startIndex++] = (float)buildSymbol.numFrames;
			data[startIndex++] = (float)buildSymbol.flags;
			if (this.firstSymbolIndex.ContainsKey(buildSymbol.build.fileHash))
			{
				data[startIndex++] = (float)this.firstSymbolIndex[buildSymbol.build.fileHash];
			}
			else
			{
				data[startIndex++] = 0f;
			}
			data[startIndex++] = (float)buildSymbol.symbolIndexInSourceBuild;
		}
		data[startIndex++] = 3.452817E+09f;
		if (symbolFrame == null)
		{
			return;
		}
		data[startIndex++] = symbolFrame.bboxMin.x;
		data[startIndex++] = symbolFrame.bboxMin.y;
		data[startIndex++] = symbolFrame.bboxMax.x;
		data[startIndex++] = symbolFrame.bboxMax.y;
		data[startIndex++] = symbolFrame.uvMin.x;
		data[startIndex++] = symbolFrame.uvMin.y;
		data[startIndex++] = symbolFrame.uvMax.x;
		data[startIndex++] = symbolFrame.uvMax.y;
	}

	private void WriteAnimFrame(float[] data, int startIndex, int firstElementIdx, int idx, int numElements, int thisFrameIndex)
	{
		data[startIndex++] = (float)firstElementIdx;
		data[startIndex++] = (float)numElements;
		data[startIndex++] = (float)thisFrameIndex;
		data[startIndex++] = (float)idx;
	}

	private void Write(float[] data, int startIndex, int thisFrameIndex, KAnim.Anim.Frame frame)
	{
		this.WriteAnimFrame(data, startIndex, frame.firstElementIdx, frame.idx, frame.numElements, thisFrameIndex);
	}

	private void WriteAnimFrameElement(float[] data, int startIndex, int symbolFrameIdx, int thisFrameIndex, Matrix2x3 transform, Color colour, int flags)
	{
		if (symbolFrameIdx != -1010)
		{
			data[startIndex++] = (float)symbolFrameIdx;
			data[startIndex++] = (float)thisFrameIndex;
			data[startIndex++] = (float)flags;
			data[startIndex++] = 0f;
			data[startIndex++] = colour.r;
			data[startIndex++] = colour.g;
			data[startIndex++] = colour.b;
			data[startIndex++] = colour.a;
			data[startIndex++] = transform.m00;
			data[startIndex++] = transform.m01;
			data[startIndex++] = transform.m02;
			data[startIndex++] = 2.8801546E+09f;
			data[startIndex++] = transform.m10;
			data[startIndex++] = transform.m11;
			data[startIndex++] = transform.m12;
			data[startIndex++] = 3.1664858E+09f;
			return;
		}
		data[startIndex++] = (float)symbolFrameIdx;
		data[startIndex++] = (float)thisFrameIndex;
		data[startIndex++] = (float)flags;
		data[startIndex++] = -1f;
		data[startIndex++] = colour.r;
		data[startIndex++] = colour.g;
		data[startIndex++] = colour.b;
		data[startIndex++] = colour.a;
		data[startIndex++] = 0f;
		data[startIndex++] = 0f;
		data[startIndex++] = 0f;
		data[startIndex++] = 2.8801546E+09f;
		data[startIndex++] = 0f;
		data[startIndex++] = 0f;
		data[startIndex++] = 0f;
		data[startIndex++] = 3.1664858E+09f;
	}

	private void WriteNullFrameElement(float[] data, int startIndex, int thisFrameIndex)
	{
		this.WriteAnimFrameElement(data, startIndex, -1010, thisFrameIndex, Matrix2x3.identity, Color.black, 0);
	}

	private void Write(float[] data, int startIndex, int symbolFrameIdx, int thisFrameIndex, KAnim.Anim.FrameElement element)
	{
		this.WriteAnimFrameElement(data, startIndex, symbolFrameIdx, thisFrameIndex, element.transform, element.multColour, element.flags);
	}

	public const int SIZE_OF_SYMBOL_FRAME_ELEMENT = 16;

	public const int SIZE_OF_ANIM_FRAME = 4;

	public const int SIZE_OF_ANIM_FRAME_ELEMENT = 16;

	private const int MAX_VISIBLE_SYMBOLS = 120;

	public const int MAX_GROUP_SIZE = 30;

	private const int NULL_DATA_FRAME_ID = -1010;
}
