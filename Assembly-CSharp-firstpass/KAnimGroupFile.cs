using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

public class KAnimGroupFile : ScriptableObject
{
	public static void Destroy()
	{
		KAnimGroupFile.groupfile = null;
	}

	public static string GetFilePath()
	{
		return "Assets/anim/resources/animgrouptags.asset";
	}

	public static KAnimGroupFile GetGroupFile()
	{
		if (KAnimGroupFile.groupfile == null)
		{
			KAnimGroupFile.groupfile = (KAnimGroupFile)Resources.Load("animgrouptags", typeof(KAnimGroupFile));
		}
		return KAnimGroupFile.groupfile;
	}

	public static void SetGroupFile(KAnimGroupFile file)
	{
		KAnimGroupFile.groupfile = file;
		KAnimGroupFile.groupfile.Sort();
	}

	public static KAnimGroupFile.Group AddDynamicGroup(HashedString tag)
	{
		KAnimGroupFile.GetGroupFile();
		List<KAnimGroupFile.Group> data = KAnimGroupFile.groupfile.GetData();
		KAnimGroupFile.Group group = new KAnimGroupFile.Group(tag);
		data.Add(group);
		return group;
	}

	public static KAnimGroupFile.Group GetGroup(HashedString tag)
	{
		KAnimGroupFile.Group group = null;
		KAnimGroupFile.GetGroupFile();
		List<KAnimGroupFile.Group> data = KAnimGroupFile.groupfile.GetData();
		for (int i = 0; i < data.Count; i++)
		{
			KAnimGroupFile.Group group2 = data[i];
			if (group2.id == tag || group2.target == tag)
			{
				group = group2;
				break;
			}
		}
		return group;
	}

	public static KAnimGroupFile.GroupFile SaveMyGroupFile(KAnimFile kaf, AnimCommandFile akf)
	{
		KAnimGroupFile.GroupFile groupFile = new KAnimGroupFile.GroupFile();
		groupFile.groupID = akf.GetGroupName(kaf);
		groupFile.commandDirectory = Path.GetDirectoryName(kaf.homedirectory);
		groupFile.Save(kaf.homedirectory + "/mygroup.yaml");
		return groupFile;
	}

	public HashedString GetGroupForHomeDirectory(HashedString homedirectory)
	{
		for (int i = 0; i < this.currentGroup.Count; i++)
		{
			if (this.currentGroup[i].first == homedirectory)
			{
				return this.currentGroup[i].second;
			}
		}
		return default(HashedString);
	}

	public List<KAnimGroupFile.Group> GetData()
	{
		return new List<KAnimGroupFile.Group>(this.groups);
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
			group.lookupUnderGroupName = akf.LookupSymbolUnderGroupName;
			group.isMultiInstance = akf.MultiInstance;
			group.maxGroupSize = akf.MaxGroupSize;
			group.materialType = akf.MaterialType;
			group.renderType = akf.RendererType;
			int num2 = this.groups.FindIndex((KAnimGroupFile.Group t) => t.commandDirectory == group.commandDirectory);
			if (num2 == -1)
			{
				if (flag)
				{
					if (akf.TargetBuild != null && akf.TargetBuild != string.Empty)
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
			}
			this.groups.Add(group);
		}
		return num;
	}

	public bool AddAnimFile(KAnimGroupFile.GroupFile gf, AnimCommandFile akf, KAnimFile file)
	{
		int num = this.AddGroup(akf, gf, file);
		return this.AddFile(num, file);
	}

	private bool AddFile(int groupIndex, KAnimFile file)
	{
		if (!this.groups[groupIndex].files.Contains(file))
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
			this.groups[groupIndex].files.Add(file);
			return true;
		}
		return false;
	}

	public void LoadAll()
	{
		this.fileData.Clear();
		KGlobalAnimParser.Destroy();
		int i = 0;
		while (i < this.groups.Count)
		{
			KBatchGroupData kbatchGroupData;
			if (this.groups[i].target.isValid)
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].target, false);
			}
			else
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].id, false);
			}
			kbatchGroupData.lookupUnderGroupName = this.groups[i].lookupUnderGroupName;
			HashedString hashedString = this.groups[i].id;
			if (this.groups[i].renderType != KAnimBatchGroup.RendererType.AnimOnly)
			{
				goto IL_0107;
			}
			if (this.groups[i].swapTarget.isValid)
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].swapTarget, false);
				hashedString = this.groups[i].swapTarget;
				goto IL_0107;
			}
			IL_024B:
			i++;
			continue;
			IL_0107:
			for (int j = 0; j < this.groups[i].files.Count; j++)
			{
				KAnimFile kanimFile = this.groups[i].files[j];
				if (kanimFile != null && kanimFile.buildFile != null && !this.fileData.ContainsKey(kanimFile.GetInstanceID()))
				{
					if (kanimFile.buildFile.bytes == null || kanimFile.buildFile.bytes.Length == 0)
					{
						global::Debug.LogWarning("Build File [" + kanimFile.buildFile.name + "] has 0 bytes", null);
					}
					else
					{
						HashedString hashedString2 = new HashedString(kanimFile.name);
						HashCache.Get().Add(hashedString2.HashValue, kanimFile.name);
						KAnimFileData file = KGlobalAnimParser.Get().GetFile(kanimFile);
						file.maxVisSymbolFrames = 0;
						file.batchTag = hashedString;
						file.buildIndex = KGlobalAnimParser.ParseBuildData(kbatchGroupData, hashedString2, new FastReader(kanimFile.buildFile.bytes), kanimFile.textures);
						this.fileData.Add(kanimFile.GetInstanceID(), file);
					}
				}
			}
			goto IL_024B;
		}
		for (int k = 0; k < this.groups.Count; k++)
		{
			if (this.groups[k].renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[k].swapTarget, false);
				KBatchGroupData batchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[k].animTarget, false);
				for (int l = 0; l < batchGroupData.builds.Count; l++)
				{
					KAnim.Build build = batchGroupData.builds[l];
					if (build != null && build.symbols != null)
					{
						for (int m = 0; m < build.symbols.Length; m++)
						{
							KAnim.Build.Symbol symbol = build.symbols[m];
							if (symbol != null)
							{
								if (symbol.hash.IsValid() && batchGroupData2.GetFirstIndex(symbol.hash) == -1)
								{
									KAnim.Build.Symbol symbol2 = new KAnim.Build.Symbol();
									symbol2.build = build;
									symbol2.hash = symbol.hash;
									symbol2.path = symbol.path;
									symbol2.colourChannel = symbol.colourChannel;
									symbol2.flags = symbol.flags;
									symbol2.firstFrameIdx = batchGroupData2.symbolFrameInstances.Count;
									symbol2.numFrames = symbol.numFrames;
									for (int n = 0; n < symbol2.numFrames; n++)
									{
										KAnim.Build.SymbolFrameInstance symbolFrameInstance = batchGroupData.GetSymbolFrameInstance(n + symbol.firstFrameIdx);
										KAnim.Build.SymbolFrameInstance symbolFrameInstance2 = default(KAnim.Build.SymbolFrameInstance);
										symbolFrameInstance2.symbolFrame = symbolFrameInstance.symbolFrame;
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
		}
		for (int num = 0; num < this.groups.Count; num++)
		{
			if (this.groups[num].renderType != KAnimBatchGroup.RendererType.DontRender)
			{
				KBatchGroupData kbatchGroupData2;
				if (this.groups[num].animTarget.isValid)
				{
					kbatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num].animTarget, false);
				}
				else
				{
					kbatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num].id, false);
				}
				for (int num2 = 0; num2 < this.groups[num].files.Count; num2++)
				{
					KAnimFile kanimFile2 = this.groups[num].files[num2];
					if (kanimFile2 != null && kanimFile2.animFile != null)
					{
						if (kanimFile2.animFile.bytes == null || kanimFile2.animFile.bytes.Length == 0)
						{
							global::Debug.LogWarning("Anim File [" + kanimFile2.animFile.name + "] has 0 bytes", null);
						}
						else
						{
							if (!this.fileData.ContainsKey(kanimFile2.GetInstanceID()))
							{
								KAnimFileData file2 = KGlobalAnimParser.Get().GetFile(kanimFile2);
								file2.maxVisSymbolFrames = 0;
								file2.batchTag = this.groups[num].id;
								this.fileData.Add(kanimFile2.GetInstanceID(), file2);
							}
							HashedString hashedString3 = new HashedString(kanimFile2.name);
							FastReader fastReader = new FastReader(kanimFile2.animFile.bytes);
							KAnimFileData kanimFileData = this.fileData[kanimFile2.GetInstanceID()];
							KGlobalAnimParser.ParseAnimData(kbatchGroupData2, hashedString3, fastReader, kanimFileData);
						}
					}
				}
			}
		}
		for (int num3 = 0; num3 < this.groups.Count; num3++)
		{
			KBatchGroupData kbatchGroupData3;
			if (this.groups[num3].target.isValid)
			{
				kbatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num3].target, false);
			}
			else
			{
				kbatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num3].id, false);
			}
			KGlobalAnimParser.PostParse(kbatchGroupData3);
		}
	}

	private void Sort()
	{
		for (int i = 0; i < this.groups.Count; i++)
		{
			this.groups[i].files.RemoveAll((KAnimFile f) => f == null || f.name == null);
		}
		this.groups.RemoveAll((KAnimGroupFile.Group f) => f == null || f.files.Count == 0);
		this.groups.Sort((KAnimGroupFile.Group file0, KAnimGroupFile.Group file1) => file0.id.HashValue.CompareTo(file1.id.HashValue));
		for (int j = 0; j < this.groups.Count; j++)
		{
			if (this.groups[j].files.Count != 1)
			{
				List<KAnimFile> list = this.groups[j].files.FindAll((KAnimFile f) => f.buildFile != null);
				this.groups[j].files.RemoveAll((KAnimFile f) => f.buildFile != null);
				list.Sort((KAnimFile file0, KAnimFile file1) => (file0.homedirectory + file0.name).CompareTo(file1.homedirectory + file1.name));
				this.groups[j].files.Sort((KAnimFile file0, KAnimFile file1) => (file0.homedirectory + file0.name).CompareTo(file1.homedirectory + file1.name));
				this.groups[j].files.InsertRange(0, list);
			}
		}
	}

	public const string ANIM_GROUP_FILE = "mygroup.yaml";

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
		public string commandDirectory = string.Empty;

		[SerializeField]
		public List<KAnimFile> files = new List<KAnimFile>();

		[SerializeField]
		public KAnimBatchGroup.RendererType renderType;

		[SerializeField]
		public KAnimBatchGroup.MaterialType materialType;

		[SerializeField]
		public int maxVisibleSymbols;

		[SerializeField]
		public int maxGroupSize;

		[SerializeField]
		public bool lookupUnderGroupName = true;

		[SerializeField]
		public HashedString target;

		[SerializeField]
		public HashedString swapTarget;

		[SerializeField]
		public HashedString animTarget;

		[SerializeField]
		public bool isMultiInstance;
	}

	public class GroupFile : YamlIO<KAnimGroupFile.GroupFile>
	{
		public string groupID { get; set; }

		public string commandDirectory { get; set; }
	}
}
