using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

public class KGlobalAnimParser
{
	public static KGlobalAnimParser Get()
	{
		if (KGlobalAnimParser.instance == null)
		{
			KGlobalAnimParser.instance = new KGlobalAnimParser();
		}
		return KGlobalAnimParser.instance;
	}

	public static void Destroy()
	{
		if (KGlobalAnimParser.instance != null)
		{
			KGlobalAnimParser.instance = null;
		}
	}

	public static AnimCommandFile GetParseCommands(string path)
	{
		string fullName = Directory.GetParent(path).FullName;
		string text = Path.Combine(fullName, KGlobalAnimParser.ANIM_COMMAND_FILE);
		if (File.Exists(text))
		{
			AnimCommandFile animCommandFile = YamlIO<AnimCommandFile>.LoadFile(text);
			animCommandFile.directory = path;
			return animCommandFile;
		}
		return null;
	}

	public static string GetTagGroup(string path)
	{
		return KGlobalAnimParser.GetParseCommands(path).GetGroupName();
	}

	public KAnimFileData Parse(KAnimFile file)
	{
		KAnimFileData kanimFileData = null;
		HashedString ignore = KAnimBatchManager.IGNORE;
		if (file.animFile != null || file.buildFile != null)
		{
			if (file.homedirectory != null && file.homedirectory != string.Empty)
			{
				AnimCommandFile parseCommands = KGlobalAnimParser.GetParseCommands(file.homedirectory);
				if (parseCommands != null)
				{
					string groupName = parseCommands.GetGroupName();
					if (groupName != null && groupName != string.Empty)
					{
						ignore = new HashedString(groupName);
					}
					else
					{
						Debug.LogWarning(string.Concat(new string[] { "No tag_group_name for [", file.name, "] [", file.homedirectory, "]" }));
					}
				}
				else
				{
					Debug.LogWarning(string.Concat(new string[] { "No parseCommands for [", file.name, "] [", file.homedirectory, "]" }));
				}
			}
			else
			{
				Debug.LogWarning("No file.path for [" + file.name + "]");
			}
			if (ignore == KAnimBatchManager.IGNORE)
			{
				kanimFileData = new KAnimFileData();
				kanimFileData.batchTag = ignore;
				kanimFileData.name = file.name;
			}
			else
			{
				try
				{
					kanimFileData = KGlobalAnimParser.Parse(ignore, file.animFile, file.buildFile, file.textures, file.name);
					kanimFileData.name = file.name;
					KGlobalAnimParser.PostParse(KAnimBatchManager.Instance().GetBatchGroupData(ignore));
				}
				catch (Exception ex)
				{
					string message = ex.Message;
					string stackTrace = ex.StackTrace;
					Output.LogError(new object[] { "Error importing", file.name, message, stackTrace });
					throw ex;
				}
			}
		}
		return kanimFileData;
	}

	private static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new InvalidDataException(message);
		}
	}

	private static KAnimFileData Parse(HashedString batchTag, TextAsset animData, TextAsset buildData, List<Texture2D> textures, string filename)
	{
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(batchTag);
		KBatchGroupData kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(group.id);
		KAnimFileData kanimFileData = new KAnimFileData();
		kanimFileData.anims = new KAnim.Anim[0];
		kanimFileData.animFrames = new KAnim.Anim.Frame[0];
		kanimFileData.animFrameElements = new KAnim.Anim.FrameElement[0];
		kanimFileData.maxVisSymbolFrames = 0;
		kanimFileData.build = null;
		kanimFileData.name = filename;
		kanimFileData.batchTag = kbatchGroupData.groupID;
		HashedString hashedString = new HashedString(filename);
		HashCache.Get().Add(hashedString.HashValue, filename);
		try
		{
			if (buildData != null && buildData.bytes != null && buildData.bytes.Length > 0)
			{
				List<KAnim.AnimHash> list = new List<KAnim.AnimHash>();
				if (group.renderType == KAnimBatchGroup.RendererType.AnimOnly && group.swapTarget.isValid)
				{
					kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(group.swapTarget);
				}
				kanimFileData.build = KGlobalAnimParser.ParseBuildData(kbatchGroupData, hashedString, new FastReader(buildData.bytes), list, textures);
				kanimFileData.hashTable.hashes = list.ToArray();
			}
			if (animData != null && animData.bytes != null && animData.bytes.Length > 0)
			{
				if (group.renderType == KAnimBatchGroup.RendererType.AnimOnly && group.animTarget.isValid)
				{
					kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(group.animTarget);
				}
				KGlobalAnimParser.ParseAnimData(kbatchGroupData, hashedString, new FastReader(animData.bytes), kanimFileData);
			}
		}
		catch (Exception ex)
		{
			string stackTrace = ex.StackTrace;
			string message = ex.Message;
			Output.LogError(new object[] { string.Concat(new string[] { "Exception while parsing kanim file [", filename, "]:\n\t", message, "\n", stackTrace }) });
		}
		return kanimFileData;
	}

	public static void ParseAnimData(KBatchGroupData data, HashedString fileNameHash, FastReader reader, KAnimFileData animFile)
	{
		KAnimParser.CheckHeader("ANIM", reader);
		uint num = reader.ReadUInt32();
		KGlobalAnimParser.Assert(num == 5U, "Invalid anim.bytes version");
		reader.ReadInt32();
		reader.ReadInt32();
		int num2 = reader.ReadInt32();
		List<KAnim.Anim> list = new List<KAnim.Anim>();
		List<KAnim.Anim.Frame> list2 = new List<KAnim.Anim.Frame>();
		List<KAnim.Anim.FrameElement> list3 = new List<KAnim.Anim.FrameElement>();
		animFile.maxVisSymbolFrames = 0;
		data.animIndex.Add(fileNameHash, data.anims.Count);
		data.animFrameIndex.Add(fileNameHash, data.animFrames.Count);
		HashSet<int> hashSet = new HashSet<int>();
		for (int i = 0; i < num2; i++)
		{
			KAnim.Anim anim = new KAnim.Anim();
			anim.name = reader.ReadKleiString();
			anim.id = animFile.name + "." + anim.name;
			anim.hash = new HashedString(anim.name);
			anim.rootSymbol.HashValue = reader.ReadInt32();
			anim.frameRate = reader.ReadSingle();
			anim.firstFrameIdx = data.animFrames.Count;
			anim.numFrames = reader.ReadInt32();
			anim.totalTime = (float)anim.numFrames / anim.frameRate;
			anim.scaledBoundingRadius = 0f;
			for (int j = 0; j < anim.numFrames; j++)
			{
				KAnim.Anim.Frame frame = default(KAnim.Anim.Frame);
				float num3 = reader.ReadSingle();
				float num4 = reader.ReadSingle();
				float num5 = reader.ReadSingle();
				float num6 = reader.ReadSingle();
				frame.bbox = new AABB3(new Vector3(num3 - num5 * 0.5f, -(num4 + num6 * 0.5f), 0f) * 0.005f, new Vector3(num3 + num5 * 0.5f, -(num4 - num6 * 0.5f), 0f) * 0.005f);
				float num7 = Math.Max(Math.Abs(frame.bbox.max.x), Math.Abs(frame.bbox.min.x));
				float num8 = Math.Max(Math.Abs(frame.bbox.max.y), Math.Abs(frame.bbox.min.y));
				float num9 = Math.Max(num7, num8);
				anim.unScaledSize.x = Math.Max(anim.unScaledSize.x, num7 / 0.005f);
				anim.unScaledSize.y = Math.Max(anim.unScaledSize.y, num8 / 0.005f);
				anim.scaledBoundingRadius = Math.Max(anim.scaledBoundingRadius, Mathf.Sqrt(num9 * num9 + num9 * num9));
				frame.idx = data.animFrames.Count;
				frame.firstElementIdx = data.frameElements.Count;
				frame.numElements = reader.ReadInt32();
				int num10 = 0;
				for (int k = 0; k < frame.numElements; k++)
				{
					KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
					frameElement.fileHash = fileNameHash;
					frameElement.symbol = new KAnimHashedString(reader.ReadInt32());
					frameElement.frame = reader.ReadInt32();
					frameElement.folder = new KAnimHashedString(reader.ReadInt32());
					frameElement.flags = reader.ReadInt32();
					float num11 = reader.ReadSingle();
					float num12 = reader.ReadSingle();
					float num13 = reader.ReadSingle();
					float num14 = reader.ReadSingle();
					frameElement.multColour = new Color(num14, num13, num12, num11);
					float num15 = reader.ReadSingle();
					float num16 = reader.ReadSingle();
					float num17 = reader.ReadSingle();
					float num18 = reader.ReadSingle();
					float num19 = reader.ReadSingle();
					float num20 = reader.ReadSingle();
					reader.ReadSingle();
					frameElement.transform.m00 = num15;
					frameElement.transform.m01 = num17;
					frameElement.transform.m02 = num19;
					frameElement.transform.m10 = num16;
					frameElement.transform.m11 = num18;
					frameElement.transform.m12 = num20;
					int symbolIndex = data.GetSymbolIndex(frameElement.symbol, fileNameHash);
					if (symbolIndex == -1)
					{
						hashSet.Add(frameElement.symbol.HashValue);
						num10++;
						frameElement.symbol = KGlobalAnimParser.MISSING_SYMBOL;
					}
					else
					{
						data.frameElements.Add(frameElement);
						list3.Add(frameElement);
					}
				}
				frame.numElements -= num10;
				data.animFrames.Add(frame);
				list2.Add(frame);
			}
			data.anims.Add(anim);
			list.Add(anim);
		}
		animFile.anims = list.ToArray();
		animFile.animFrames = list2.ToArray();
		animFile.animFrameElements = list3.ToArray();
		animFile.maxVisSymbolFrames = Math.Max(animFile.maxVisSymbolFrames, reader.ReadInt32());
		data.UpdateMaxVisibleSymbols(animFile.maxVisSymbolFrames);
		List<KAnim.AnimHash> list4;
		if (animFile.hashTable.hashes != null)
		{
			list4 = new List<KAnim.AnimHash>(animFile.hashTable.hashes);
		}
		else
		{
			list4 = new List<KAnim.AnimHash>();
		}
		KGlobalAnimParser.ParseHashTable(reader, list4);
		animFile.hashTable.hashes = list4.ToArray();
		if (hashSet.Count > 0)
		{
			KGlobalAnimParser.groupSYmbol[fileNameHash] = data.groupID;
			KGlobalAnimParser.allMissingSymbols[fileNameHash] = new List<int>(hashSet);
		}
		else
		{
			KGlobalAnimParser.groupSYmbol.Remove(fileNameHash);
			KGlobalAnimParser.allMissingSymbols.Remove(fileNameHash);
		}
	}

	public static void ClearMissingSymbols()
	{
		KGlobalAnimParser.allMissingSymbols = new Dictionary<HashedString, List<int>>();
		KGlobalAnimParser.groupSYmbol = new Dictionary<HashedString, HashedString>();
	}

	public static void DumpMissingSymbols()
	{
		if (KGlobalAnimParser.allMissingSymbols.Count == 0)
		{
			return;
		}
		string text = "The following animations are missing symbols:\n";
		HashedString hashedString = default(HashedString);
		foreach (KeyValuePair<HashedString, List<int>> keyValuePair in KGlobalAnimParser.allMissingSymbols)
		{
			if (hashedString != KGlobalAnimParser.groupSYmbol[keyValuePair.Key])
			{
				if (hashedString.HashValue != 0)
				{
					Debug.LogWarning(string.Concat(new object[] { "[", hashedString, "] ", text }));
					text = string.Empty;
				}
				hashedString = KGlobalAnimParser.groupSYmbol[keyValuePair.Key];
			}
			string text2 = string.Empty;
			foreach (int num in keyValuePair.Value)
			{
				text2 = text2 + " " + HashCache.Get().Get(num);
			}
			string text3 = text;
			text = string.Concat(new object[]
			{
				text3,
				"\t",
				HashCache.Get().Get(keyValuePair.Key),
				" (",
				keyValuePair.Value.Count,
				"): ",
				text2,
				"\n"
			});
		}
		Debug.LogWarning(string.Concat(new object[] { "[", hashedString, "] ", text }));
	}

	private static void ParseHashTable(FastReader reader, List<KAnim.AnimHash> hashes)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			KAnim.AnimHash hash = default(KAnim.AnimHash);
			hash.Hash = reader.ReadInt32();
			hash.String = reader.ReadKleiString();
			KAnimHashedString kanimHashedString = new KAnimHashedString(hash.String);
			if (kanimHashedString.HashValue != hash.Hash)
			{
				Debug.LogError("Hash function broken");
				return;
			}
			if (hashes.FindIndex((KAnim.AnimHash h) => h.Hash == hash.Hash) == -1)
			{
				hashes.Add(hash);
				HashCache.Get().Add(hash.Hash, hash.String);
			}
		}
	}

	public static KAnim.Build ParseBuildData(KBatchGroupData data, KAnimHashedString fileNameHash, FastReader reader, List<KAnim.AnimHash> hashes, List<Texture2D> textures)
	{
		KAnimParser.CheckHeader("BILD", reader);
		int num = reader.ReadInt32();
		if (num != 9 && num != 10)
		{
			Debug.LogError(fileNameHash + " has invalid build.bytes version");
		}
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(data.groupID);
		Debug.Assert(group != null);
		KAnim.Build build = new KAnim.Build();
		build.textures = ((textures.Count <= 0) ? null : textures.ToArray());
		data.AddNewBuildFile(fileNameHash);
		data.textures.AddRange(build.textures);
		int num2 = reader.ReadInt32();
		int num3 = reader.ReadInt32();
		build.symbols = new KAnim.Build.Symbol[num2];
		build.frames = new KAnim.Build.SymbolFrame[num3];
		build.name = reader.ReadKleiString();
		if (group == null || data == null)
		{
			Debug.Log("eh?");
		}
		build.batchTag = ((!group.swapTarget.isValid) ? data.groupID : group.target);
		build.fileHash = fileNameHash;
		int num4 = 0;
		for (int i = 0; i < build.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = new KAnim.Build.Symbol();
			symbol.build = build;
			symbol.hash = new KAnimHashedString(reader.ReadInt32());
			if (num > 9)
			{
				symbol.path = new KAnimHashedString(reader.ReadInt32());
			}
			symbol.colourChannel = new KAnimHashedString(reader.ReadInt32());
			symbol.flags = reader.ReadInt32();
			symbol.firstFrameIdx = data.symbolFrameInstances.Count;
			symbol.numFrames = reader.ReadInt32();
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
				Debug.AssertFormat(symbolFrameInstance.buildImageIdx < textures.Count + data.textureStartIndex[fileNameHash], "{0} Symbol: [{1}] tex count: [{2}] buildImageIdx: [{3}] group total [{4}]", new object[]
				{
					fileNameHash.ToString(),
					symbol.hash,
					textures.Count,
					symbolFrameInstance.buildImageIdx,
					data.textureStartIndex[fileNameHash]
				});
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
				Vector2 vector = new Vector2(num10, num11);
				Vector2 vector2 = new Vector2(num12, num13);
				symbolFrame.v0 = new Vector3(symbolFrame.bboxMin.x, symbolFrame.bboxMin.y, 0f);
				symbolFrame.v1 = new Vector3(symbolFrame.bboxMax.x, symbolFrame.bboxMin.y, 0f);
				symbolFrame.v2 = new Vector3(symbolFrame.bboxMin.x, symbolFrame.bboxMax.y, 0f);
				symbolFrame.v3 = new Vector3(symbolFrame.bboxMax.x, symbolFrame.bboxMax.y, 0f);
				symbolFrame.uv0 = new Vector2(vector.x, 1f - vector.y);
				symbolFrame.uv1 = new Vector2(vector2.x, 1f - vector.y);
				symbolFrame.uv2 = new Vector2(vector.x, 1f - vector2.y);
				symbolFrame.uv3 = new Vector2(vector2.x, 1f - vector2.y);
				build.frames[num4] = symbolFrame;
				data.symbolFrameInstances.Add(symbolFrameInstance);
				num4++;
			}
			symbol.numLookupFrames = num5;
			data.AddBuildSymbol(symbol);
			build.symbols[i] = symbol;
		}
		data.AddBuild(build);
		KGlobalAnimParser.ParseHashTable(reader, hashes);
		return build;
	}

	public static void PostParse(KBatchGroupData data)
	{
		for (int i = 0; i < data.GetSymbolCount(); i++)
		{
			KAnim.Build.Symbol symbol = data.GetSymbol(i);
			if (symbol == null)
			{
				Debug.LogWarning(string.Concat(new object[] { "Symbol null for [", data.groupID, "] idx: [", i, "]" }));
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
					Debug.LogWarning(string.Concat(new object[]
					{
						"No lookup frames for  [",
						data.groupID,
						"] build: [",
						symbol.build.name,
						"] idx: [",
						i,
						"] id: [",
						symbol.hash,
						"]"
					}));
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
							Debug.LogWarning(string.Concat(new object[] { "No symbol frame  [", data.groupID, "] symFrameIdx: [", l, "] id: [", symbol.hash, "]" }));
						}
						else
						{
							for (int m = symbolFrameInstance2.symbolFrame.sourceFrameNum; m < symbolFrameInstance2.symbolFrame.sourceFrameNum + symbolFrameInstance2.symbolFrame.duration; m++)
							{
								if (m >= symbol.frameLookup.Length)
								{
									Debug.LogWarning(string.Concat(new object[]
									{
										"Too many lookup frames [",
										m,
										">=",
										symbol.frameLookup.Length,
										"] for  [",
										data.groupID,
										"] idx: [",
										i,
										"] id: [",
										symbol.hash,
										"]"
									}));
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
						int num2 = text.IndexOf("/");
						if (num2 != -1)
						{
							string text2 = text.Substring(0, num2);
							symbol.folder = new KAnimHashedString(text2);
							HashCache.Get().Add(symbol.folder.HashValue, text2);
						}
					}
				}
			}
		}
	}

	public const float ANIM_SCALE = 0.005f;

	public static KAnimHashedString MISSING_SYMBOL = new KAnimHashedString("MISSING_SYMBOL");

	public static string ANIM_COMMAND_FILE = "batchgroup.yaml";

	private static KGlobalAnimParser instance = null;

	private static Dictionary<HashedString, List<int>> allMissingSymbols = new Dictionary<HashedString, List<int>>();

	private static Dictionary<HashedString, HashedString> groupSYmbol = new Dictionary<HashedString, HashedString>();
}
