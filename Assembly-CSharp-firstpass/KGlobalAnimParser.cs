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
			global::Debug.Log("Destroying KGlobalAnimParser", null);
			KGlobalAnimParser.instance.commandFiles.Clear();
			KGlobalAnimParser.instance.commandFiles = null;
			KGlobalAnimParser.instance.files.Clear();
			KGlobalAnimParser.instance.files = null;
			KGlobalAnimParser.instance.dynamicFiles.Clear();
			KGlobalAnimParser.instance.dynamicFiles = null;
			KGlobalAnimParser.instance = null;
		}
	}

	public void ClearDynamic()
	{
		this.dynamicFiles.Clear();
	}

	public KAnimFileData GetDynamicFile(HashedString batchTag, string name)
	{
		KAnimFileData kanimFileData = null;
		if (!this.dynamicFiles.TryGetValue(batchTag, out kanimFileData))
		{
			kanimFileData = new KAnimFileData(name);
			kanimFileData.batchTag = batchTag;
			kanimFileData.animBatchTag = batchTag;
			this.dynamicFiles[batchTag] = kanimFileData;
		}
		return kanimFileData;
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
			this.Parse(anim_file, kanimFileData);
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
			AnimCommandFile animCommandFile = YamlIO<AnimCommandFile>.LoadFile(text);
			animCommandFile.directory = "Assets/anim/" + Directory.GetParent(path).Name;
			KGlobalAnimParser.instance.commandFiles[hashedString] = animCommandFile;
			return animCommandFile;
		}
		return null;
	}

	public static string GetTagGroup(string path)
	{
		string text = path + "/mygroup.yaml";
		KAnimGroupFile.GroupFile groupFile = YamlIO<KAnimGroupFile.GroupFile>.LoadFile(text);
		return groupFile.groupID;
	}

	private void Parse(KAnimFile file, KAnimFileData data)
	{
		HashedString ignore = KAnimBatchManager.IGNORE;
		if (file.animFile != null || file.buildFile != null)
		{
			if (file.homedirectory != null && file.homedirectory != string.Empty)
			{
				ignore = new HashedString(KGlobalAnimParser.GetTagGroup(file.homedirectory));
			}
			else
			{
				global::Debug.LogWarning("No file.path for [" + file.name + "]", null);
			}
			if (ignore == KAnimBatchManager.IGNORE)
			{
				data.batchTag = ignore;
			}
			else
			{
				try
				{
					this.Parse(ignore, file, data);
					KGlobalAnimParser.PostParse(KAnimBatchManager.Instance().GetBatchGroupData(ignore, false));
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
	}

	private KAnimFileData Parse(HashedString batchTag, KAnimFile file, KAnimFileData animFile)
	{
		TextAsset animFile2 = file.animFile;
		TextAsset buildFile = file.buildFile;
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(batchTag);
		KBatchGroupData kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(group.id, false);
		animFile.batchTag = kbatchGroupData.groupID;
		HashedString hashedString = new HashedString(file.name);
		HashCache.Get().Add(hashedString.HashValue, file.name);
		try
		{
			if (buildFile != null && buildFile.bytes != null && buildFile.bytes.Length > 0)
			{
				if (group.renderType == KAnimBatchGroup.RendererType.AnimOnly && group.swapTarget.isValid)
				{
					global::Debug.Log(string.Concat(new string[]
					{
						"BUILD Anim only [",
						group.id.ToString(),
						"] -> swapTarget [",
						group.swapTarget.ToString(),
						"]"
					}), null);
					kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(group.swapTarget, false);
				}
				animFile.batchTag = kbatchGroupData.groupID;
				animFile.buildIndex = KGlobalAnimParser.ParseBuildData(kbatchGroupData, hashedString, new FastReader(buildFile.bytes), file.textures);
			}
			if (animFile2 != null && animFile2.bytes != null && animFile2.bytes.Length > 0)
			{
				if (group.renderType == KAnimBatchGroup.RendererType.AnimOnly && group.animTarget.isValid)
				{
					global::Debug.Log(string.Concat(new string[]
					{
						"ANIM Anim only [",
						group.id.ToString(),
						"] -> animTarget [",
						group.animTarget.ToString(),
						"]"
					}), null);
					kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(group.animTarget, false);
				}
				KGlobalAnimParser.ParseAnimData(kbatchGroupData, hashedString, new FastReader(animFile2.bytes), animFile);
			}
		}
		catch (Exception ex)
		{
			string stackTrace = ex.StackTrace;
			string message = ex.Message;
			Output.LogError(new object[] { string.Concat(new string[] { "Exception while parsing kanim file [", file.name, "]:\n\t", message, "\n", stackTrace }) });
		}
		return animFile;
	}

	public static void ParseAnimData(KBatchGroupData data, HashedString fileNameHash, FastReader reader, KAnimFileData animFile)
	{
		KGlobalAnimParser.CheckHeader("ANIM", reader);
		uint num = reader.ReadUInt32();
		KGlobalAnimParser.Assert(num == 5U, "Invalid anim.bytes version");
		reader.ReadInt32();
		reader.ReadInt32();
		int num2 = reader.ReadInt32();
		animFile.maxVisSymbolFrames = 0;
		animFile.animCount = 0;
		animFile.frameCount = 0;
		animFile.elementCount = 0;
		animFile.firstAnimIndex = data.anims.Count;
		animFile.animBatchTag = data.groupID;
		data.animIndex.Add(fileNameHash, data.anims.Count);
		data.animFrameIndex.Add(fileNameHash, data.animFrames.Count);
		animFile.firstElementIndex = data.frameElements.Count;
		for (int i = 0; i < num2; i++)
		{
			KAnim.Anim anim = new KAnim.Anim(animFile, data.anims.Count);
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
						num10++;
						frameElement.symbol = KGlobalAnimParser.MISSING_SYMBOL;
					}
					else
					{
						data.frameElements.Add(frameElement);
						animFile.elementCount++;
					}
				}
				frame.numElements -= num10;
				data.animFrames.Add(frame);
				animFile.frameCount++;
			}
			data.AddAnim(anim);
			animFile.animCount++;
		}
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
		if (num != 10)
		{
			if (num != 9)
			{
				global::Debug.LogError(string.Concat(new object[] { fileNameHash, " has invalid build.bytes version [", num, "]" }), null);
				return -1;
			}
		}
		KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(data.groupID);
		KAnim.Build build = data.AddNewBuildFile(fileNameHash);
		build.textureCount = textures.Count;
		if (textures.Count > 0)
		{
			data.AddTextures(textures);
		}
		int num2 = reader.ReadInt32();
		int num3 = reader.ReadInt32();
		build.symbols = new KAnim.Build.Symbol[num2];
		build.frames = new KAnim.Build.SymbolFrame[num3];
		build.name = reader.ReadKleiString();
		build.batchTag = ((!group.swapTarget.isValid) ? data.groupID : group.target);
		build.fileHash = fileNameHash;
		int num4 = 0;
		for (int i = 0; i < build.symbols.Length; i++)
		{
			KAnimHashedString kanimHashedString = new KAnimHashedString(reader.ReadInt32());
			KAnim.Build.Symbol symbol = new KAnim.Build.Symbol();
			symbol.build = build;
			symbol.hash = kanimHashedString;
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
				global::Debug.LogWarning(string.Concat(new object[] { "Symbol null for [", data.groupID, "] idx: [", i, "]" }), null);
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
					global::Debug.LogWarning(string.Concat(new object[]
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
					}), null);
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
							global::Debug.LogWarning(string.Concat(new object[] { "No symbol frame  [", data.groupID, "] symFrameIdx: [", l, "] id: [", symbol.hash, "]" }), null);
						}
						else
						{
							for (int m = symbolFrameInstance2.symbolFrame.sourceFrameNum; m < symbolFrameInstance2.symbolFrame.sourceFrameNum + symbolFrameInstance2.symbolFrame.duration; m++)
							{
								if (m >= symbol.frameLookup.Length)
								{
									global::Debug.LogWarning(string.Concat(new object[]
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
									}), null);
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

	private static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new InvalidDataException(message);
		}
	}

	private static void CheckHeader(string header, FastReader reader)
	{
		char[] array = reader.ReadChars(header.Length);
		for (int i = 0; i < header.Length; i++)
		{
			if (array[i] != header[i])
			{
				throw new InvalidDataException("Expected " + header);
			}
		}
	}

	public const float ANIM_SCALE = 0.005f;

	public static KAnimHashedString MISSING_SYMBOL = new KAnimHashedString("MISSING_SYMBOL");

	public static string ANIM_COMMAND_FILE = "batchgroup.yaml";

	private Dictionary<HashedString, AnimCommandFile> commandFiles = new Dictionary<HashedString, AnimCommandFile>();

	private Dictionary<HashedString, KAnimFileData> dynamicFiles = new Dictionary<HashedString, KAnimFileData>();

	private Dictionary<int, KAnimFileData> files = new Dictionary<int, KAnimFileData>();

	private static KGlobalAnimParser instance = null;
}
