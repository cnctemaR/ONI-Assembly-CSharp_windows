using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimGroupFile : ScriptableObject
{
	public static void DestroyInstance()
	{
		KAnimGroupFile.groupfile = null;
	}

	public static string GetFilePath(string contentDir = "")
	{
		if (string.IsNullOrEmpty(contentDir))
		{
			return "Assets/anim/base/resources/animgrouptags.asset";
		}
		return "Assets/anim" + string.Format("/{0}/", contentDir) + "animgrouptags.asset";
	}

	public static KAnimGroupFile GetGroupFile()
	{
		global::Debug.Assert(KAnimGroupFile.groupfile != null, "Cannot GetGroupFile before it is loaded.");
		return KAnimGroupFile.groupfile;
	}

	public static KAnimGroupFile.Group GetGroup(HashedString tag)
	{
		global::Debug.Assert(KAnimGroupFile.groupfile != null, "GetGroup called before LoadAll called");
		List<KAnimGroupFile.Group> list = KAnimGroupFile.groupfile.groups;
		global::Debug.Assert(list != null);
		for (int i = 0; i < list.Count; i++)
		{
			KAnimGroupFile.Group group = list[i];
			if (group.id == tag || group.target == tag)
			{
				return group;
			}
		}
		return null;
	}

	public static HashedString GetGroupForHomeDirectory(HashedString homedirectory)
	{
		List<Pair<HashedString, HashedString>> list = KAnimGroupFile.groupfile.currentGroup;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].first == homedirectory)
			{
				return list[i].second;
			}
		}
		return default(HashedString);
	}

	public List<KAnimGroupFile.Group> GetData()
	{
		return this.groups;
	}

	public void Reset()
	{
		this.groups = new List<KAnimGroupFile.Group>();
		this.currentGroup = new List<Pair<HashedString, HashedString>>();
	}

	private int AddGroup(AnimCommandFile akf, KAnimGroupFile.GroupFile gf, KAnimFile file)
	{
		bool flag = akf.IsSwap(file);
		HashedString groupId = new HashedString(gf.groupID);
		int num = this.groups.FindIndex((KAnimGroupFile.Group t) => t.id == groupId);
		if (num == -1)
		{
			num = this.groups.Count;
			KAnimGroupFile.Group group = new KAnimGroupFile.Group(groupId);
			group.commandDirectory = akf.directory;
			group.maxGroupSize = akf.MaxGroupSize;
			group.renderType = akf.RendererType;
			if (this.groups.FindIndex((KAnimGroupFile.Group t) => t.commandDirectory == group.commandDirectory) == -1)
			{
				if (flag)
				{
					if (!string.IsNullOrEmpty(akf.TargetBuild))
					{
						group.target = new HashedString(akf.TargetBuild);
					}
					if (group.renderType != KAnimBatchGroup.RendererType.DontRender)
					{
						group.renderType = KAnimBatchGroup.RendererType.DontRender;
						group.swapTarget = new HashedString(akf.SwapTargetBuild);
					}
				}
				if (akf.Type == AnimCommandFile.ConfigType.AnimOnly)
				{
					group.target = new HashedString(akf.TargetBuild);
					group.renderType = KAnimBatchGroup.RendererType.AnimOnly;
					group.animTarget = new HashedString(akf.AnimTargetBuild);
					group.swapTarget = new HashedString(akf.SwapTargetBuild);
				}
				if (akf.Type == AnimCommandFile.ConfigType.BuildAndAnim)
				{
					group.renderType = KAnimBatchGroup.RendererType.BuildAndAnims;
				}
			}
			this.groups.Add(group);
		}
		return num;
	}

	public bool AddAnimFile(KAnimGroupFile.GroupFile gf, AnimCommandFile akf, KAnimFile file)
	{
		global::Debug.Assert(gf != null);
		global::Debug.Assert(file != null, gf.groupID);
		global::Debug.Assert(akf != null, gf.groupID);
		int num = this.AddGroup(akf, gf, file);
		return this.AddFile(num, file);
	}

	private bool AddFile(int groupIndex, KAnimFile file)
	{
		if (!this.groups[groupIndex].animNames.Contains(file.name))
		{
			Pair<HashedString, HashedString> pair = new Pair<HashedString, HashedString>(file.homedirectory, this.groups[groupIndex].id);
			bool flag = false;
			for (int i = 0; i < this.currentGroup.Count; i++)
			{
				if (this.currentGroup[i].first == file.homedirectory)
				{
					this.currentGroup[i] = pair;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.currentGroup.Add(pair);
			}
			this.groups[groupIndex].animFiles.Add(file);
			this.groups[groupIndex].animNames.Add(file.name);
			return true;
		}
		return false;
	}

	public KAnimGroupFile.AddModResult AddAnimMod(KAnimGroupFile.GroupFile gf, AnimCommandFile akf, KAnimFile file)
	{
		global::Debug.Assert(gf != null);
		global::Debug.Assert(file != null, gf.groupID);
		global::Debug.Assert(akf != null, gf.groupID);
		int num = this.AddGroup(akf, gf, file);
		string name = file.GetData().name;
		int num2 = this.groups[num].animFiles.FindIndex((KAnimFile candidate) => candidate != null && candidate.GetData().name == name);
		if (num2 == -1)
		{
			this.groups[num].animFiles.Add(file);
			this.groups[num].animNames.Add(file.GetData().name);
			return KAnimGroupFile.AddModResult.Added;
		}
		this.groups[num].animFiles[num2].mod = file.mod;
		return KAnimGroupFile.AddModResult.Replaced;
	}

	public static void LoadGroupResourceFile()
	{
		KAnimGroupFile.groupfile = (KAnimGroupFile)Resources.Load("animgrouptags", typeof(KAnimGroupFile));
	}

	public static void LoadAll()
	{
		KAnimGroupFile.groupfile.Load();
	}

	public static void MapNamesToAnimFiles(Dictionary<HashedString, KAnimFile> animTable)
	{
		KAnimGroupFile.groupfile.DoMapNamesToAnimFiles(animTable);
	}

	private void DoMapNamesToAnimFiles(Dictionary<HashedString, KAnimFile> animTable)
	{
		for (int i = 0; i < this.groups.Count; i++)
		{
			this.groups[i].animFiles = new List<KAnimFile>();
			for (int j = 0; j < this.groups[i].animNames.Count; j++)
			{
				HashedString hashedString = this.groups[i].animNames[j];
				KAnimFile kanimFile = null;
				animTable.TryGetValue(hashedString, out kanimFile);
				if (kanimFile != null)
				{
					this.groups[i].animFiles.Add(kanimFile);
				}
			}
		}
	}

	private void Load()
	{
		this.fileData.Clear();
		int i = 0;
		while (i < this.groups.Count)
		{
			if (!this.groups[i].id.IsValid)
			{
				global::Debug.LogErrorFormat("Group invalid groupIndex [{0}]", new object[] { i });
			}
			KBatchGroupData kbatchGroupData;
			if (this.groups[i].target.IsValid)
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].target);
			}
			else
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].id);
			}
			HashedString hashedString = this.groups[i].id;
			if (this.groups[i].renderType != KAnimBatchGroup.RendererType.AnimOnly)
			{
				goto IL_0106;
			}
			if (this.groups[i].swapTarget.IsValid)
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].swapTarget);
				hashedString = this.groups[i].swapTarget;
				goto IL_0106;
			}
			IL_021B:
			i++;
			continue;
			IL_0106:
			for (int j = 0; j < this.groups[i].animFiles.Count; j++)
			{
				KAnimFile kanimFile = this.groups[i].animFiles[j];
				if (kanimFile != null)
				{
					byte[] buildBytes = kanimFile.buildBytes;
					if (buildBytes != null && !this.fileData.ContainsKey(kanimFile.GetInstanceID()))
					{
						if (buildBytes.Length == 0)
						{
							global::Debug.LogWarning("Build File [" + kanimFile.GetData().name + "] has 0 bytes");
						}
						else
						{
							HashedString hashedString2 = new HashedString(kanimFile.name);
							HashCache.Get().Add(hashedString2.HashValue, kanimFile.name);
							KAnimFileData file = KGlobalAnimParser.Get().GetFile(kanimFile);
							file.maxVisSymbolFrames = 0;
							file.batchTag = hashedString;
							file.buildIndex = KGlobalAnimParser.ParseBuildData(kbatchGroupData, hashedString2, new FastReader(buildBytes), kanimFile.textureList);
							this.fileData.Add(kanimFile.GetInstanceID(), file);
						}
					}
				}
			}
			goto IL_021B;
		}
		for (int k = 0; k < this.groups.Count; k++)
		{
			if (this.groups[k].renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[k].swapTarget);
				KBatchGroupData batchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[k].animTarget);
				for (int l = 0; l < batchGroupData.builds.Count; l++)
				{
					KAnim.Build build = batchGroupData.builds[l];
					if (build != null && build.symbols != null)
					{
						for (int m = 0; m < build.symbols.Length; m++)
						{
							KAnim.Build.Symbol symbol = build.symbols[m];
							if (symbol != null && symbol.hash.IsValid() && batchGroupData2.GetFirstIndex(symbol.hash) == -1)
							{
								KAnim.Build.Symbol symbol2 = new KAnim.Build.Symbol();
								symbol2.build = build;
								symbol2.hash = symbol.hash;
								symbol2.path = symbol.path;
								symbol2.colourChannel = symbol.colourChannel;
								symbol2.flags = symbol.flags;
								symbol2.firstFrameIdx = batchGroupData2.symbolFrameInstances.Count;
								symbol2.numFrames = symbol.numFrames;
								symbol2.symbolIndexInSourceBuild = batchGroupData2.frameElementSymbols.Count;
								for (int n = 0; n < symbol2.numFrames; n++)
								{
									KAnim.Build.SymbolFrameInstance symbolFrameInstance = batchGroupData.GetSymbolFrameInstance(n + symbol.firstFrameIdx);
									KAnim.Build.SymbolFrameInstance symbolFrameInstance2 = default(KAnim.Build.SymbolFrameInstance);
									symbolFrameInstance2 = symbolFrameInstance;
									symbolFrameInstance2.buildImageIdx = -1;
									symbolFrameInstance2.symbolIdx = batchGroupData2.GetSymbolCount();
									batchGroupData2.symbolFrameInstances.Add(symbolFrameInstance2);
								}
								batchGroupData2.AddBuildSymbol(symbol2);
							}
						}
					}
				}
			}
		}
		for (int num = 0; num < this.groups.Count; num++)
		{
			if (this.groups[num].renderType == KAnimBatchGroup.RendererType.BuildAndAnims)
			{
				KBatchGroupData batchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num].id);
				int num2 = 0;
				Dictionary<HashedString, int> dictionary = new Dictionary<HashedString, int>();
				for (int num3 = 0; num3 < batchGroupData3.builds.Count; num3++)
				{
					KAnim.Build build2 = batchGroupData3.builds[num3];
					if (build2 != null && build2.symbols != null)
					{
						for (int num4 = 0; num4 < build2.symbols.Length; num4++)
						{
							KAnim.Build.Symbol symbol3 = build2.symbols[num4];
							if (symbol3 != null && symbol3.hash.IsValid())
							{
								global::Debug.Assert(num2 < batchGroupData3.maxSymbolsPerBuild, "Symbol count is larger than symbols in source builds");
								if (!dictionary.ContainsKey(symbol3.hash))
								{
									dictionary[symbol3.hash] = num2;
									num2++;
								}
								symbol3.symbolIndexInSourceBuild = dictionary[symbol3.hash];
							}
						}
					}
				}
			}
		}
		for (int num5 = 0; num5 < this.groups.Count; num5++)
		{
			if (!this.groups[num5].id.IsValid)
			{
				global::Debug.LogErrorFormat("Group invalid groupIndex [{0}]", new object[] { num5 });
			}
			if (this.groups[num5].renderType != KAnimBatchGroup.RendererType.DontRender)
			{
				KBatchGroupData kbatchGroupData2;
				if (this.groups[num5].animTarget.IsValid)
				{
					kbatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num5].animTarget);
					if (kbatchGroupData2 == null)
					{
						global::Debug.LogErrorFormat("Anim group is null for [{0}] -> [{1}]", new object[]
						{
							this.groups[num5].id,
							this.groups[num5].animTarget
						});
					}
				}
				else
				{
					kbatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num5].id);
					if (kbatchGroupData2 == null)
					{
						global::Debug.LogErrorFormat("Anim group is null for [{0}]", new object[] { this.groups[num5].id });
					}
				}
				for (int num6 = 0; num6 < this.groups[num5].animFiles.Count; num6++)
				{
					KAnimFile kanimFile2 = this.groups[num5].animFiles[num6];
					if (kanimFile2 != null)
					{
						byte[] animBytes = kanimFile2.animBytes;
						if (animBytes != null)
						{
							if (animBytes.Length == 0)
							{
								global::Debug.LogWarning("Anim File [" + kanimFile2.GetData().name + "] has 0 bytes");
							}
							else
							{
								if (!this.fileData.ContainsKey(kanimFile2.GetInstanceID()))
								{
									KAnimFileData file2 = KGlobalAnimParser.Get().GetFile(kanimFile2);
									file2.maxVisSymbolFrames = 0;
									file2.batchTag = this.groups[num5].id;
									this.fileData.Add(kanimFile2.GetInstanceID(), file2);
								}
								HashedString hashedString3 = new HashedString(kanimFile2.name);
								FastReader fastReader = new FastReader(animBytes);
								KAnimFileData kanimFileData = this.fileData[kanimFile2.GetInstanceID()];
								KGlobalAnimParser.ParseAnimData(kbatchGroupData2, hashedString3, fastReader, kanimFileData);
							}
						}
					}
				}
			}
		}
		for (int num7 = 0; num7 < this.groups.Count; num7++)
		{
			if (!this.groups[num7].id.IsValid)
			{
				global::Debug.LogErrorFormat("Group invalid groupIndex [{0}]", new object[] { num7 });
			}
			KBatchGroupData kbatchGroupData3;
			if (this.groups[num7].target.IsValid)
			{
				kbatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num7].target);
				if (kbatchGroupData3 == null)
				{
					global::Debug.LogErrorFormat("Group is null for  [{0}] target [{1}]", new object[]
					{
						this.groups[num7].id,
						this.groups[num7].target
					});
				}
			}
			else
			{
				kbatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num7].id);
				if (kbatchGroupData3 == null)
				{
					global::Debug.LogErrorFormat("Group is null for [{0}]", new object[] { this.groups[num7].id });
				}
			}
			KGlobalAnimParser.PostParse(kbatchGroupData3);
		}
	}

	private const string MASTER_GROUP_FILE = "animgrouptags";

	public const int MAX_ANIMS_PER_GROUP = 10;

	private static KAnimGroupFile groupfile;

	private Dictionary<int, KAnimFileData> fileData = new Dictionary<int, KAnimFileData>();

	[SerializeField]
	private List<KAnimGroupFile.Group> groups = new List<KAnimGroupFile.Group>();

	[SerializeField]
	private List<Pair<HashedString, HashedString>> currentGroup = new List<Pair<HashedString, HashedString>>();

	[Serializable]
	public class Group
	{
		public Group(HashedString tag)
		{
			this.id = tag;
		}

		[SerializeField]
		public HashedString id;

		[SerializeField]
		public string commandDirectory = "";

		[SerializeField]
		public List<HashedString> animNames = new List<HashedString>();

		[SerializeField]
		public KAnimBatchGroup.RendererType renderType;

		[SerializeField]
		public int maxVisibleSymbols;

		[SerializeField]
		public int maxGroupSize;

		[SerializeField]
		public HashedString target;

		[SerializeField]
		public HashedString swapTarget;

		[SerializeField]
		public HashedString animTarget;

		[NonSerialized]
		public List<KAnimFile> animFiles = new List<KAnimFile>();
	}

	public class GroupFile
	{
		public string groupID { get; set; }

		public string commandDirectory { get; set; }
	}

	public enum AddModResult
	{
		Added,
		Replaced
	}
}
