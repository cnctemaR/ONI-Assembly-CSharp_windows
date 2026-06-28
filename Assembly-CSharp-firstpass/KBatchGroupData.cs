using System;
using System.Collections.Generic;
using UnityEngine;

public class KBatchGroupData
{
	public KBatchGroupData(HashedString id, bool dynamic)
	{
		this.groupID = id;
		this.maxVisibleSymbols = 1;
		this.isDynamic = dynamic;
		this.Init();
	}

	public bool isDynamic { get; private set; }

	public HashedString groupID { get; private set; }

	public bool isSwap { get; private set; }

	public int maxVisibleSymbols { get; private set; }

	public List<KAnim.Anim> anims { get; private set; }

	public Dictionary<KAnimHashedString, int> animIndex { get; private set; }

	public Dictionary<KAnimHashedString, int> animCount { get; private set; }

	public Dictionary<KAnimHashedString, int> animFrameIndex { get; private set; }

	public List<KAnim.Anim.Frame> animFrames { get; private set; }

	public List<KAnim.Anim.FrameElement> frameElements { get; private set; }

	public List<KAnim.Build> builds { get; private set; }

	public Dictionary<KAnimHashedString, int> buildIndex { get; private set; }

	public List<KAnim.Build.Symbol> frameElementSymbols { get; private set; }

	public List<KAnim.Build.SymbolFrameInstance> symbolFrameInstances { get; private set; }

	public Dictionary<KAnimHashedString, int> textureStartIndex { get; private set; }

	public Dictionary<KAnimHashedString, int> firstSymbolIndex { get; private set; }

	public List<Color> symbolColourOveride { get; private set; }

	public List<Texture2D> textures { get; private set; }

	private void Init()
	{
		this.anims = new List<KAnim.Anim>();
		this.animIndex = new Dictionary<KAnimHashedString, int>();
		this.animCount = new Dictionary<KAnimHashedString, int>();
		this.animFrameIndex = new Dictionary<KAnimHashedString, int>();
		this.animFrames = new List<KAnim.Anim.Frame>();
		this.frameElements = new List<KAnim.Anim.FrameElement>();
		this.builds = new List<KAnim.Build>();
		this.buildIndex = new Dictionary<KAnimHashedString, int>();
		this.frameElementSymbols = new List<KAnim.Build.Symbol>();
		this.symbolFrameInstances = new List<KAnim.Build.SymbolFrameInstance>();
		this.symbolColourOveride = new List<Color>();
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
		if (this.animFrameIndex != null)
		{
			this.animFrameIndex.Clear();
			this.animFrameIndex = null;
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
		if (this.buildIndex != null)
		{
			this.buildIndex.Clear();
			this.buildIndex = null;
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
		if (this.symbolColourOveride != null)
		{
			this.symbolColourOveride.Clear();
			this.symbolColourOveride = null;
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
		this.buildIndex.Add(fileHash, this.builds.Count);
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
		this.anims.Add(anim);
	}

	public KAnim.Anim GetAnim(int anim)
	{
		if (anim < 0 || anim >= this.anims.Count)
		{
			global::Debug.LogError(string.Format("Anim [{0}] out of range [{1}] in batch [{2}]", anim, this.anims.Count, this.groupID), null);
		}
		return this.anims[anim];
	}

	public KAnim.Build GetBuild(KAnimHashedString fileHash)
	{
		if (this.buildIndex.ContainsKey(fileHash))
		{
			return this.builds[this.buildIndex[fileHash]];
		}
		return null;
	}

	public KAnim.Build GetBuild(int index)
	{
		return this.builds[index];
	}

	public void UpdateMaxVisibleSymbols(int newCount)
	{
		this.maxVisibleSymbols = Mathf.Min(120, Mathf.Max(this.maxVisibleSymbols, newCount));
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
		this.frameElementSymbols.Add(symbol);
		this.symbolColourOveride.Add(Color.white);
	}

	public int GetSymbolCount()
	{
		return this.frameElementSymbols.Count;
	}

	public bool HasSymbolFrame(int index)
	{
		return index >= 0 && index < this.symbolFrameInstances.Count;
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

	public void SetColourOveride(int index, Color colour)
	{
		this.symbolColourOveride[index] = colour;
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

	public KAnim.Build.Symbol GetBuildSymbol(KAnimHashedString symbol)
	{
		for (int i = 0; i < this.frameElementSymbols.Count; i++)
		{
			if (this.frameElementSymbols[i].hash == symbol)
			{
				return this.frameElementSymbols[i];
			}
		}
		return null;
	}

	public int GetBuildSymbolIndex(KAnimHashedString symbol)
	{
		for (int i = 0; i < this.frameElementSymbols.Count; i++)
		{
			if (this.frameElementSymbols[i].hash == symbol)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetAnimFileOffset(KAnimHashedString fileNameHash)
	{
		return this.animIndex[fileNameHash];
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

	public int GetFrame(string name)
	{
		return this.animFrameIndex[new KAnimHashedString(name)];
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

	public void WriteAnimData(float[] data)
	{
		List<KAnim.Anim.Frame> animFrames = this.GetAnimFrames();
		List<KAnim.Anim.FrameElement> animFrameElements = this.GetAnimFrameElements();
		int num = 0;
		int num2 = 1 + ((animFrames.Count != 0) ? animFrames.Count : this.symbolFrameInstances.Count);
		if (animFrames.Count == 0 && this.symbolFrameInstances.Count == 0 && animFrameElements.Count == 0)
		{
			global::Debug.LogError(string.Concat(new object[]
			{
				"Eh, no data ",
				animFrames.Count,
				" ",
				this.symbolFrameInstances.Count,
				" ",
				animFrameElements.Count
			}), null);
		}
		data[num++] = (float)num2;
		data[num++] = (float)animFrames.Count;
		data[num++] = (float)animFrameElements.Count;
		data[num++] = (float)this.symbolFrameInstances.Count;
		if (animFrames.Count == 0)
		{
			for (int i = 0; i < this.symbolFrameInstances.Count; i++)
			{
				this.WriteAnimFrame(data, num, i, i, 1, i);
				num += 4;
			}
			for (int j = 0; j < this.symbolFrameInstances.Count; j++)
			{
				this.WriteAnimFrameElement(data, num, j, j, Matrix2x3.identity, Color.white, 0);
				num += 16;
			}
		}
		else
		{
			for (int k = 0; k < animFrames.Count; k++)
			{
				this.Write(data, num, k, animFrames[k]);
				num += 4;
			}
			for (int l = 0; l < animFrameElements.Count; l++)
			{
				KAnim.Anim.FrameElement frameElement = animFrameElements[l];
				if (frameElement.symbol == KGlobalAnimParser.MISSING_SYMBOL)
				{
					this.WriteAnimFrameElement(data, num, -1, l, Matrix2x3.identity, Color.white, 0);
				}
				else
				{
					int symbolIndex = this.GetSymbolIndex(frameElement.symbol, frameElement.fileHash);
					KAnim.Build.Symbol buildSymbol = this.GetBuildSymbol(symbolIndex);
					if (buildSymbol == null)
					{
						global::Debug.LogError(string.Concat(new object[]
						{
							"Missing symbol for Anim Frame Element: [",
							HashCache.Get().Get(frameElement.symbol),
							": ",
							frameElement.symbol,
							"]"
						}), null);
					}
					int frameIdx = buildSymbol.GetFrameIdx(frameElement.frame);
					this.Write(data, num, frameIdx, l, frameElement);
				}
				num += 16;
			}
		}
	}

	public int AddSymbol(KAnimHashedString symbol, KAnim.Build targetBuild)
	{
		KAnim.Build.Symbol symbol2 = new KAnim.Build.Symbol();
		symbol2.hash = symbol;
		symbol2.build = targetBuild;
		symbol2.index = this.frameElementSymbols.Count;
		this.frameElementSymbols.Add(symbol2);
		this.symbolColourOveride.Add(Color.white);
		return symbol2.index;
	}

	public int GetFirstIndex(KAnimHashedString symbol)
	{
		return this.frameElementSymbols.FindIndex((KAnim.Build.Symbol fes) => fes.hash == symbol);
	}

	public int GetSymbolIndex(KAnimHashedString symbol, KAnimHashedString fileNameHash)
	{
		KBatchGroupData.getSymbolIndexSymbolSymbol = symbol;
		KBatchGroupData.getSymbolIndexFileNameHash = fileNameHash;
		if (!this.lookupUnderGroupName)
		{
			return this.frameElementSymbols.FindIndex(KBatchGroupData.getSymbolIndexPredicateSymbolAndFile);
		}
		return this.frameElementSymbols.FindIndex(KBatchGroupData.getSymbolIndexPredicateSymbol);
	}

	public void WriteBuildData(BatchGroupInstance instance, float[] data)
	{
		for (int i = 0; i < instance.symbolFrameInstances.Count; i++)
		{
			this.Write(data, i * 32, i, instance.symbolFrameInstances[i].buildImageIdx, instance.symbolFrameInstances[i]);
		}
	}

	private void Write(float[] data, int startIndex, int thisFrameIndex, int atlasIndex, KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		data[startIndex++] = (float)atlasIndex;
		data[startIndex++] = (float)thisFrameIndex;
		data[startIndex++] = (float)symbol_frame_instance.symbolIdx;
		KAnim.Build.SymbolFrame symbolFrame = symbol_frame_instance.symbolFrame;
		KAnim.Build.Symbol buildSymbol = this.GetBuildSymbol(symbol_frame_instance.symbolIdx);
		if (buildSymbol == null)
		{
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
		}
		data[startIndex++] = 3.1664858E+09f;
		data[startIndex++] = 3.452817E+09f;
		data[startIndex++] = symbolFrame.v0[0];
		data[startIndex++] = symbolFrame.v0[1];
		data[startIndex++] = symbolFrame.v0[2];
		data[startIndex++] = symbolFrame.v1[0];
		data[startIndex++] = symbolFrame.v1[1];
		data[startIndex++] = symbolFrame.v1[2];
		data[startIndex++] = symbolFrame.v2[0];
		data[startIndex++] = symbolFrame.v2[1];
		data[startIndex++] = symbolFrame.v2[2];
		data[startIndex++] = symbolFrame.v3[0];
		data[startIndex++] = symbolFrame.v3[1];
		data[startIndex++] = symbolFrame.v3[2];
		data[startIndex++] = symbolFrame.uv0[0];
		data[startIndex++] = symbolFrame.uv0[1];
		data[startIndex++] = symbolFrame.uv1[0];
		data[startIndex++] = symbolFrame.uv1[1];
		data[startIndex++] = symbolFrame.uv2[0];
		data[startIndex++] = symbolFrame.uv2[1];
		data[startIndex++] = symbolFrame.uv3[0];
		data[startIndex++] = symbolFrame.uv3[1];
		if (this.symbolColourOveride != null && symbol_frame_instance.symbolIdx >= 0 && symbol_frame_instance.symbolIdx < this.symbolColourOveride.Count)
		{
			data[startIndex++] = this.symbolColourOveride[symbol_frame_instance.symbolIdx][0];
			data[startIndex++] = this.symbolColourOveride[symbol_frame_instance.symbolIdx][1];
			data[startIndex++] = this.symbolColourOveride[symbol_frame_instance.symbolIdx][2];
			data[startIndex++] = this.symbolColourOveride[symbol_frame_instance.symbolIdx][3];
		}
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
		data[startIndex++] = (float)symbolFrameIdx;
		data[startIndex++] = (float)thisFrameIndex;
		data[startIndex++] = (float)flags;
		data[startIndex++] = (float)(((float)symbolFrameIdx != -1010f) ? 0 : (-1));
		data[startIndex++] = colour[0];
		data[startIndex++] = colour[1];
		data[startIndex++] = colour[2];
		data[startIndex++] = colour[3];
		data[startIndex++] = (((float)symbolFrameIdx != -1010f) ? transform.m00 : 0f);
		data[startIndex++] = (((float)symbolFrameIdx != -1010f) ? transform.m01 : 0f);
		data[startIndex++] = (((float)symbolFrameIdx != -1010f) ? transform.m02 : 0f);
		data[startIndex++] = 2.8801546E+09f;
		data[startIndex++] = (((float)symbolFrameIdx != -1010f) ? transform.m10 : 0f);
		data[startIndex++] = (((float)symbolFrameIdx != -1010f) ? transform.m11 : 0f);
		data[startIndex++] = (((float)symbolFrameIdx != -1010f) ? transform.m12 : 0f);
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

	public const int SIZE_OF_SYMBOL_FRAME_ELEMENT = 32;

	public const int SIZE_OF_ANIM_FRAME = 4;

	public const int SIZE_OF_ANIM_FRAME_ELEMENT = 16;

	private const int MAX_VISIBLE_SYMBOLS = 120;

	public const int MAX_GROUP_SIZE = 60;

	private const int NULL_DATA_FRAME_ID = -1010;

	public bool lookupUnderGroupName = true;

	private static KAnimHashedString getSymbolIndexSymbolSymbol;

	private static KAnimHashedString getSymbolIndexFileNameHash;

	private static Predicate<KAnim.Build.Symbol> getSymbolIndexPredicateSymbolAndFile = (KAnim.Build.Symbol fes) => fes.hash == KBatchGroupData.getSymbolIndexSymbolSymbol && fes.build.fileHash == KBatchGroupData.getSymbolIndexFileNameHash;

	private static Predicate<KAnim.Build.Symbol> getSymbolIndexPredicateSymbol = (KAnim.Build.Symbol fes) => fes.hash == KBatchGroupData.getSymbolIndexSymbolSymbol;
}
