using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

public class KGlobalAnimParser
{
	private static KGlobalAnimParser instance
	{
		get
		{
			return Singleton<KGlobalAnimParser>.Instance;
		}
	}

	public static void CreateInstance()
	{
		Singleton<KGlobalAnimParser>.CreateInstance();
	}

	public static KGlobalAnimParser Get()
	{
		return KGlobalAnimParser.instance;
	}

	public static void DestroyInstance()
	{
		if (KGlobalAnimParser.instance != null)
		{
			KGlobalAnimParser.instance.commandFiles.Clear();
			KGlobalAnimParser.instance.commandFiles = null;
			KGlobalAnimParser.instance.files.Clear();
			KGlobalAnimParser.instance.files = null;
		}
		Singleton<KGlobalAnimParser>.DestroyInstance();
	}

	public KAnimFileData GetFile(KAnimFile anim_file)
	{
		KAnimFileData kanimFileData = null;
		int instanceID = anim_file.GetInstanceID();
		if (!this.files.TryGetValue(instanceID, out kanimFileData))
		{
			kanimFileData = new KAnimFileData(anim_file.name);
			this.files[instanceID] = kanimFileData;
		}
		return kanimFileData;
	}

	public KAnimFileData Load(KAnimFile anim_file)
	{
		KAnimFileData kanimFileData = null;
		int instanceID = anim_file.GetInstanceID();
		if (!this.files.TryGetValue(instanceID, out kanimFileData))
		{
			kanimFileData = this.GetFile(anim_file);
		}
		return kanimFileData;
	}

	public static AnimCommandFile GetParseCommands(string path)
	{
		string fullName = Directory.GetParent(path).FullName;
		HashedString hashedString = new HashedString(fullName);
		if (KGlobalAnimParser.Get().commandFiles.ContainsKey(hashedString))
		{
			return KGlobalAnimParser.instance.commandFiles[hashedString];
		}
		string text = Path.Combine(fullName, KGlobalAnimParser.ANIM_COMMAND_FILE);
		if (File.Exists(text))
		{
			AnimCommandFile animCommandFile = YamlIO.LoadFile<AnimCommandFile>(text, null, null);
			animCommandFile.directory = "Assets/anim/" + Directory.GetParent(path).Name;
			KGlobalAnimParser.instance.commandFiles[hashedString] = animCommandFile;
			return animCommandFile;
		}
		return null;
	}

	public static void ParseAnimData(KBatchGroupData data, HashedString fileNameHash, FastReader reader, KAnimFileData animFile)
	{
		KGlobalAnimParser.CheckHeader("ANIM", reader);
		KGlobalAnimParser.Assert(reader.ReadUInt32() == 5U, "Invalid anim.bytes version");
		reader.ReadInt32();
		reader.ReadInt32();
		int num = reader.ReadInt32();
		animFile.maxVisSymbolFrames = 0;
		animFile.animCount = 0;
		animFile.frameCount = 0;
		animFile.elementCount = 0;
		animFile.firstAnimIndex = data.anims.Count;
		animFile.animBatchTag = data.groupID;
		data.animIndex.Add(fileNameHash, data.anims.Count);
		animFile.firstElementIndex = data.frameElements.Count;
		for (int i = 0; i < num; i++)
		{
			KAnim.Anim anim = new KAnim.Anim(animFile, data.anims.Count);
			anim.name = reader.ReadKleiString();
			string text = animFile.name + "." + anim.name;
			anim.id = text;
			HashCache.Get().Add(anim.name);
			HashCache.Get().Add(text);
			anim.hash = anim.name;
			anim.rootSymbol.HashValue = reader.ReadInt32();
			anim.frameRate = reader.ReadSingle();
			anim.firstFrameIdx = data.animFrames.Count;
			anim.numFrames = reader.ReadInt32();
			anim.totalTime = (float)anim.numFrames / anim.frameRate;
			anim.scaledBoundingRadius = 0f;
			for (int j = 0; j < anim.numFrames; j++)
			{
				KAnim.Anim.Frame frame = default(KAnim.Anim.Frame);
				float num2 = reader.ReadSingle();
				float num3 = reader.ReadSingle();
				float num4 = reader.ReadSingle();
				float num5 = reader.ReadSingle();
				frame.bbox = new AABB3(new Vector3(num2 - num4 * 0.5f, -(num3 + num5 * 0.5f), 0f) * 0.005f, new Vector3(num2 + num4 * 0.5f, -(num3 - num5 * 0.5f), 0f) * 0.005f);
				float num6 = Math.Max(Math.Abs(frame.bbox.max.x), Math.Abs(frame.bbox.min.x));
				float num7 = Math.Max(Math.Abs(frame.bbox.max.y), Math.Abs(frame.bbox.min.y));
				float num8 = Math.Max(num6, num7);
				anim.unScaledSize.x = Math.Max(anim.unScaledSize.x, num6 / 0.005f);
				anim.unScaledSize.y = Math.Max(anim.unScaledSize.y, num7 / 0.005f);
				anim.scaledBoundingRadius = Math.Max(anim.scaledBoundingRadius, Mathf.Sqrt(num8 * num8 + num8 * num8));
				frame.idx = data.animFrames.Count;
				frame.firstElementIdx = data.frameElements.Count;
				frame.numElements = reader.ReadInt32();
				int num9 = 0;
				for (int k = 0; k < frame.numElements; k++)
				{
					KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
					frameElement.fileHash = fileNameHash;
					frameElement.symbol = new KAnimHashedString(reader.ReadInt32());
					frameElement.frame = reader.ReadInt32();
					frameElement.folder = new KAnimHashedString(reader.ReadInt32());
					reader.ReadInt32();
					float num10 = reader.ReadSingle();
					float num11 = reader.ReadSingle();
					float num12 = reader.ReadSingle();
					float num13 = reader.ReadSingle();
					frameElement.multColour = new Color(num13, num12, num11, num10);
					float num14 = reader.ReadSingle();
					float num15 = reader.ReadSingle();
					float num16 = reader.ReadSingle();
					float num17 = reader.ReadSingle();
					float num18 = reader.ReadSingle();
					float num19 = reader.ReadSingle();
					reader.ReadSingle();
					frameElement.transform.m00 = num14;
					frameElement.transform.m01 = num16;
					frameElement.transform.m02 = num18;
					frameElement.transform.m10 = num15;
					frameElement.transform.m11 = num17;
					frameElement.transform.m12 = num19;
					int symbolIndex = data.GetSymbolIndex(frameElement.symbol);
					if (symbolIndex == -1)
					{
						num9++;
						frameElement.symbol = KGlobalAnimParser.MISSING_SYMBOL;
					}
					else
					{
						frameElement.symbolIdx = symbolIndex;
						data.frameElements.Add(frameElement);
						animFile.elementCount++;
					}
				}
				frame.numElements -= num9;
				data.animFrames.Add(frame);
				animFile.frameCount++;
			}
			data.AddAnim(anim);
			animFile.animCount++;
		}
		global::Debug.Assert(num == animFile.animCount);
		data.animCount[fileNameHash] = animFile.animCount;
		animFile.maxVisSymbolFrames = Math.Max(animFile.maxVisSymbolFrames, reader.ReadInt32());
		data.UpdateMaxVisibleSymbols(animFile.maxVisSymbolFrames);
		KGlobalAnimParser.ParseHashTable(reader);
	}

	private static void ParseHashTable(FastReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int num2 = reader.ReadInt32();
			string text = reader.ReadKleiString();
			HashCache.Get().Add(num2, text);
		}
	}

	public static int ParseBuildData(KBatchGroupData data, KAnimHashedString fileNameHash, FastReader reader, List<Texture2D> textures)
	{
		KGlobalAnimParser.CheckHeader("BILD", reader);
		int num = reader.ReadInt32();
		if (num != 10 && num != 9)
		{
			KAnimHashedString kanimHashedString = fileNameHash;
			global::Debug.LogError(kanimHashedString.ToString() + " has invalid build.bytes version [" + num.ToString() + "]");
			return -1;
		}
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(data.groupID);
		if (group == null)
		{
			global::Debug.LogErrorFormat("[{1}] Failed to get group [{0}]", new object[] { data.groupID, fileNameHash.DebuggerDisplay });
		}
		int num2 = reader.ReadInt32();
		int num3 = reader.ReadInt32();
		KAnim.Build build = data.AddNewBuildFile(fileNameHash);
		build.textureCount = textures.Count;
		if (textures.Count > 0)
		{
			data.AddTextures(textures);
		}
		build.symbols = new KAnim.Build.Symbol[num2];
		build.frames = new KAnim.Build.SymbolFrame[num3];
		build.name = reader.ReadKleiString();
		build.batchTag = (group.swapTarget.IsValid ? group.target : data.groupID);
		build.fileHash = fileNameHash;
		int num4 = 0;
		for (int i = 0; i < build.symbols.Length; i++)
		{
			KAnimHashedString kanimHashedString2 = new KAnimHashedString(reader.ReadInt32());
			KAnim.Build.Symbol symbol = new KAnim.Build.Symbol();
			symbol.build = build;
			symbol.hash = kanimHashedString2;
			if (num > 9)
			{
				symbol.path = new KAnimHashedString(reader.ReadInt32());
			}
			symbol.colourChannel = new KAnimHashedString(reader.ReadInt32());
			symbol.flags = reader.ReadInt32();
			symbol.firstFrameIdx = data.symbolFrameInstances.Count;
			symbol.numFrames = reader.ReadInt32();
			symbol.symbolIndexInSourceBuild = i;
			int num5 = 0;
			for (int j = 0; j < symbol.numFrames; j++)
			{
				KAnim.Build.SymbolFrame symbolFrame = new KAnim.Build.SymbolFrame();
				KAnim.Build.SymbolFrameInstance symbolFrameInstance = default(KAnim.Build.SymbolFrameInstance);
				symbolFrameInstance.symbolFrame = symbolFrame;
				symbolFrame.fileNameHash = fileNameHash;
				symbolFrame.sourceFrameNum = reader.ReadInt32();
				symbolFrame.duration = reader.ReadInt32();
				symbolFrameInstance.buildImageIdx = data.textureStartIndex[fileNameHash] + reader.ReadInt32();
				if (symbolFrameInstance.buildImageIdx >= textures.Count + data.textureStartIndex[fileNameHash])
				{
					global::Debug.LogErrorFormat("{0} Symbol: [{1}] tex count: [{2}] buildImageIdx: [{3}] group total [{4}]", new object[]
					{
						fileNameHash.ToString(),
						symbol.hash,
						textures.Count,
						symbolFrameInstance.buildImageIdx,
						data.textureStartIndex[fileNameHash]
					});
				}
				symbolFrameInstance.symbolIdx = data.GetSymbolCount();
				num5 = Math.Max(symbolFrame.sourceFrameNum + symbolFrame.duration, num5);
				float num6 = reader.ReadSingle();
				float num7 = reader.ReadSingle();
				float num8 = reader.ReadSingle();
				float num9 = reader.ReadSingle();
				symbolFrame.bboxMin = new Vector2(num6 - num8 * 0.5f, num7 - num9 * 0.5f);
				symbolFrame.bboxMax = new Vector2(num6 + num8 * 0.5f, num7 + num9 * 0.5f);
				float num10 = reader.ReadSingle();
				float num11 = reader.ReadSingle();
				float num12 = reader.ReadSingle();
				float num13 = reader.ReadSingle();
				symbolFrame.uvMin = new Vector2(num10, 1f - num11);
				symbolFrame.uvMax = new Vector2(num12, 1f - num13);
				build.frames[num4] = symbolFrame;
				data.symbolFrameInstances.Add(symbolFrameInstance);
				num4++;
			}
			symbol.numLookupFrames = num5;
			data.AddBuildSymbol(symbol);
			build.symbols[i] = symbol;
		}
		KGlobalAnimParser.ParseHashTable(reader);
		return build.index;
	}

	public static void PostParse(KBatchGroupData data)
	{
		for (int i = 0; i < data.GetSymbolCount(); i++)
		{
			KAnim.Build.Symbol symbol = data.GetSymbol(i);
			if (symbol == null)
			{
				global::Debug.LogWarning(string.Concat(new string[]
				{
					"Symbol null for [",
					data.groupID.ToString(),
					"] idx: [",
					i.ToString(),
					"]"
				}));
			}
			else
			{
				if (symbol.numLookupFrames <= 0)
				{
					int num = symbol.numFrames;
					for (int j = symbol.firstFrameIdx; j < symbol.firstFrameIdx + symbol.numFrames; j++)
					{
						KAnim.Build.SymbolFrameInstance symbolFrameInstance = data.GetSymbolFrameInstance(j);
						num = Mathf.Max(num, symbolFrameInstance.symbolFrame.sourceFrameNum + symbolFrameInstance.symbolFrame.duration);
					}
					symbol.numLookupFrames = num;
				}
				symbol.frameLookup = new int[symbol.numLookupFrames];
				if (symbol.numLookupFrames <= 0)
				{
					string[] array = new string[9];
					array[0] = "No lookup frames for  [";
					array[1] = data.groupID.ToString();
					array[2] = "] build: [";
					array[3] = symbol.build.name;
					array[4] = "] idx: [";
					array[5] = i.ToString();
					array[6] = "] id: [";
					int num2 = 7;
					KAnimHashedString kanimHashedString = symbol.hash;
					array[num2] = kanimHashedString.ToString();
					array[8] = "]";
					global::Debug.LogWarning(string.Concat(array));
				}
				else
				{
					for (int k = 0; k < symbol.numLookupFrames; k++)
					{
						symbol.frameLookup[k] = -1;
					}
					for (int l = symbol.firstFrameIdx; l < symbol.firstFrameIdx + symbol.numFrames; l++)
					{
						KAnim.Build.SymbolFrameInstance symbolFrameInstance2 = data.GetSymbolFrameInstance(l);
						if (symbolFrameInstance2.symbolFrame == null)
						{
							string[] array2 = new string[7];
							array2[0] = "No symbol frame  [";
							array2[1] = data.groupID.ToString();
							array2[2] = "] symFrameIdx: [";
							array2[3] = l.ToString();
							array2[4] = "] id: [";
							int num3 = 5;
							KAnimHashedString kanimHashedString = symbol.hash;
							array2[num3] = kanimHashedString.ToString();
							array2[6] = "]";
							global::Debug.LogWarning(string.Concat(array2));
						}
						else
						{
							for (int m = symbolFrameInstance2.symbolFrame.sourceFrameNum; m < symbolFrameInstance2.symbolFrame.sourceFrameNum + symbolFrameInstance2.symbolFrame.duration; m++)
							{
								if (m >= symbol.frameLookup.Length)
								{
									string[] array3 = new string[11];
									array3[0] = "Too many lookup frames [";
									array3[1] = m.ToString();
									array3[2] = ">=";
									array3[3] = symbol.frameLookup.Length.ToString();
									array3[4] = "] for  [";
									array3[5] = data.groupID.ToString();
									array3[6] = "] idx: [";
									array3[7] = i.ToString();
									array3[8] = "] id: [";
									int num4 = 9;
									KAnimHashedString kanimHashedString = symbol.hash;
									array3[num4] = kanimHashedString.ToString();
									array3[10] = "]";
									global::Debug.LogWarning(string.Concat(array3));
								}
								else
								{
									symbol.frameLookup[m] = l;
								}
							}
						}
					}
					string text = HashCache.Get().Get(symbol.path);
					if (!string.IsNullOrEmpty(text))
					{
						int num5 = text.IndexOf("/");
						if (num5 != -1)
						{
							string text2 = text.Substring(0, num5);
							symbol.folder = new KAnimHashedString(text2);
							HashCache.Get().Add(symbol.folder.HashValue, text2);
						}
					}
				}
			}
		}
	}

	private static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new Exception(message);
		}
	}

	private static void CheckHeader(string header, FastReader reader)
	{
		char[] array = reader.ReadChars(header.Length);
		for (int i = 0; i < header.Length; i++)
		{
			if (array[i] != header[i])
			{
				throw new Exception("Expected " + header);
			}
		}
	}

	public static KAnimHashedString MISSING_SYMBOL = new KAnimHashedString("MISSING_SYMBOL");

	public static string ANIM_COMMAND_FILE = "batchgroup.yaml";

	public const float ANIM_SCALE = 0.005f;

	private Dictionary<HashedString, AnimCommandFile> commandFiles = new Dictionary<HashedString, AnimCommandFile>();

	private Dictionary<int, KAnimFileData> files = new Dictionary<int, KAnimFileData>();
}
