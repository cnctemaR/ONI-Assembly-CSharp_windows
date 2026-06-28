using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimGroupFile : ScriptableObject
{
	public static void Destroy()
	{
		KAnimGroupFile.groupfile = null;
	}

	public static string GetFilePath()
	{
		return "Assets/anim/resources/" + KAnimGroupFile.GROUP_FILE + ".asset";
	}

	public static KAnimGroupFile GetGroupFile()
	{
		if (KAnimGroupFile.groupfile == null)
		{
			KAnimGroupFile.groupfile = (KAnimGroupFile)Resources.Load(KAnimGroupFile.GROUP_FILE, typeof(KAnimGroupFile));
		}
		Debug.Assert(KAnimGroupFile.groupfile != null, "Couldn't load group file from resources directory");
		return KAnimGroupFile.groupfile;
	}

	public static void SetGroupFile(KAnimGroupFile file)
	{
		KAnimGroupFile.groupfile = file;
	}

	public static void AddDynamicGroup(HashedString tag)
	{
		KAnimGroupFile.GetGroupFile();
		List<KAnimGroupFile.Group> data = KAnimGroupFile.groupfile.GetData();
		data.Add(new KAnimGroupFile.Group(tag));
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
		return this.groups;
	}

	public List<Pair<HashedString, HashedString>> GetFiles()
	{
		return this.currentGroup;
	}

	public void Reset()
	{
		this.groups = new List<KAnimGroupFile.Group>();
		this.currentGroup = new List<Pair<HashedString, HashedString>>();
	}

	public void Sort()
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
				list.Sort((KAnimFile file0, KAnimFile file1) => file0.name.CompareTo(file1.name));
				this.groups[j].files.Sort((KAnimFile file0, KAnimFile file1) => file0.name.CompareTo(file1.name));
				this.groups[j].files.InsertRange(0, list);
			}
		}
	}

	public bool Add(KAnimFile file, AnimCommandFile akf, HashedString homedirectory)
	{
		string groupName = akf.GetGroupName();
		if (groupName == null)
		{
			Debug.LogWarning("Missing groupname for [" + file.name + "]");
			return false;
		}
		HashedString tag = new HashedString(groupName);
		HashedString pathVal = this.GetGroupForHomeDirectory(homedirectory);
		if (pathVal.isValid)
		{
			if (tag == pathVal)
			{
				return false;
			}
			int num = this.groups.FindIndex((KAnimGroupFile.Group t) => t.id == pathVal);
			Debug.Assert(num != -1);
			this.groups[num].files.Remove(file);
		}
		int num2 = this.groups.FindIndex((KAnimGroupFile.Group t) => t.id == tag);
		if (num2 == -1)
		{
			num2 = this.groups.Count;
			KAnimGroupFile.Group group = new KAnimGroupFile.Group(tag);
			group.lookupUnderGroupName = akf.LookupSymbolUnderGroupName;
			group.isMultiInstance = akf.MultiInstance;
			group.maxGroupSize = akf.MaxGroupSize;
			group.materialType = akf.MaterialType;
			group.renderType = akf.RendererType;
			if (akf.IsSwap() && akf.TargetBuild != null && akf.TargetBuild != string.Empty)
			{
				group.target = new HashedString(akf.TargetBuild);
			}
			if (group.renderType != KAnimBatchGroup.RendererType.DontRender && akf.IsSwap())
			{
				group.renderType = KAnimBatchGroup.RendererType.DontRender;
				group.swapTarget = new HashedString(akf.SwapTargetBuild);
			}
			if (akf.Type == AnimCommandFile.ConfigType.AnimOnly)
			{
				group.target = new HashedString(akf.TargetBuild);
				group.renderType = KAnimBatchGroup.RendererType.AnimOnly;
				group.animTarget = new HashedString(akf.AnimTargetBuild);
				group.swapTarget = new HashedString(akf.SwapTargetBuild);
			}
			this.groups.Add(group);
		}
		if (!this.groups[num2].files.Contains(file))
		{
			Pair<HashedString, HashedString> pair = new Pair<HashedString, HashedString>(homedirectory, tag);
			bool flag = false;
			for (int i = 0; i < this.currentGroup.Count; i++)
			{
				if (this.currentGroup[i].first == homedirectory)
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
			this.groups[num2].files.Add(file);
			return true;
		}
		return false;
	}

	public void LoadAll()
	{
		this.fileData.Clear();
		int i = 0;
		while (i < this.groups.Count)
		{
			Debug.AssertFormat(this.groups[i].id.isValid, "Group invalid groupIndex [{0}]", new object[] { i });
			KBatchGroupData kbatchGroupData;
			if (this.groups[i].target.isValid)
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].target);
			}
			else
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].id);
			}
			kbatchGroupData.lookupUnderGroupName = this.groups[i].lookupUnderGroupName;
			HashedString hashedString = this.groups[i].id;
			if (this.groups[i].renderType != KAnimBatchGroup.RendererType.AnimOnly)
			{
				goto IL_012E;
			}
			if (this.groups[i].swapTarget.isValid)
			{
				kbatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[i].swapTarget);
				hashedString = this.groups[i].swapTarget;
				goto IL_012E;
			}
			IL_02B2:
			i++;
			continue;
			IL_012E:
			for (int j = 0; j < this.groups[i].files.Count; j++)
			{
				KAnimFile kanimFile = this.groups[i].files[j];
				if (kanimFile != null && kanimFile.buildFile != null)
				{
					KAnimFileData kanimFileData = new KAnimFileData();
					kanimFileData.anims = new KAnim.Anim[0];
					kanimFileData.animFrames = new KAnim.Anim.Frame[0];
					kanimFileData.animFrameElements = new KAnim.Anim.FrameElement[0];
					kanimFileData.maxVisSymbolFrames = 0;
					kanimFileData.name = kanimFile.name;
					kanimFileData.batchTag = hashedString;
					List<KAnim.AnimHash> list = new List<KAnim.AnimHash>();
					if (kanimFile.buildFile.bytes == null || kanimFile.buildFile.bytes.Length == 0)
					{
						Debug.LogWarning("Build File [" + kanimFile.buildFile.name + "] has 0 bytes");
					}
					else
					{
						HashedString hashedString2 = new HashedString(kanimFile.name);
						HashCache.Get().Add(hashedString2.HashValue, kanimFile.name);
						kanimFileData.build = KGlobalAnimParser.ParseBuildData(kbatchGroupData, hashedString2, new FastReader(kanimFile.buildFile.bytes), list, kanimFile.textures);
						kanimFileData.hashTable.hashes = list.ToArray();
						KAnimFileManager.Get().Put(kanimFile, kanimFileData);
						this.fileData.Add(kanimFile.GetInstanceID(), kanimFileData);
					}
				}
			}
			goto IL_02B2;
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
			Debug.AssertFormat(this.groups[num].id.isValid, "Group invalid groupIndex [{0}]", new object[] { num });
			if (this.groups[num].renderType != KAnimBatchGroup.RendererType.DontRender)
			{
				KBatchGroupData kbatchGroupData2;
				if (this.groups[num].animTarget.isValid)
				{
					kbatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num].animTarget);
					Debug.AssertFormat(kbatchGroupData2 != null, "Anim group is null for [{0}] -> [{1}]", new object[]
					{
						this.groups[num].id,
						this.groups[num].animTarget
					});
				}
				else
				{
					kbatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num].id);
					Debug.AssertFormat(kbatchGroupData2 != null, "Anim group is null for [{0}]", new object[] { this.groups[num].id });
				}
				for (int num2 = 0; num2 < this.groups[num].files.Count; num2++)
				{
					KAnimFile kanimFile2 = this.groups[num].files[num2];
					if (kanimFile2 != null && kanimFile2.animFile != null)
					{
						if (kanimFile2.animFile.bytes == null || kanimFile2.animFile.bytes.Length == 0)
						{
							Debug.LogWarning("Anim File [" + kanimFile2.animFile.name + "] has 0 bytes");
						}
						else
						{
							if (!this.fileData.ContainsKey(kanimFile2.GetInstanceID()))
							{
								KAnimFileData kanimFileData2 = new KAnimFileData();
								kanimFileData2.anims = new KAnim.Anim[0];
								kanimFileData2.animFrames = new KAnim.Anim.Frame[0];
								kanimFileData2.animFrameElements = new KAnim.Anim.FrameElement[0];
								kanimFileData2.maxVisSymbolFrames = 0;
								kanimFileData2.name = kanimFile2.name;
								kanimFileData2.batchTag = this.groups[num].id;
								kanimFileData2.hashTable.hashes = new KAnim.AnimHash[0];
								KAnimFileManager.Get().Put(kanimFile2, kanimFileData2);
								this.fileData.Add(kanimFile2.GetInstanceID(), kanimFileData2);
							}
							HashedString hashedString3 = new HashedString(kanimFile2.name);
							FastReader fastReader = new FastReader(kanimFile2.animFile.bytes);
							KGlobalAnimParser.ParseAnimData(kbatchGroupData2, hashedString3, fastReader, this.fileData[kanimFile2.GetInstanceID()]);
						}
					}
				}
			}
		}
		for (int num3 = 0; num3 < this.groups.Count; num3++)
		{
			Debug.AssertFormat(this.groups[num3].id.isValid, "Group invalid groupIndex [{0}]", new object[] { num3 });
			KBatchGroupData kbatchGroupData3;
			if (this.groups[num3].target.isValid)
			{
				kbatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num3].target);
				Debug.AssertFormat(kbatchGroupData3 != null, "Group is null for  [{0}] target [{1}]", new object[]
				{
					this.groups[num3].id,
					this.groups[num3].target
				});
			}
			else
			{
				kbatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(this.groups[num3].id);
				Debug.AssertFormat(kbatchGroupData3 != null, "Group is null for [{0}]", new object[] { this.groups[num3].id });
			}
			KGlobalAnimParser.PostParse(kbatchGroupData3);
		}
	}

	private static string GROUP_FILE = "animgrouptags";

	private static KAnimGroupFile groupfile;

	[SerializeField]
	private List<KAnimGroupFile.Group> groups = new List<KAnimGroupFile.Group>();

	[SerializeField]
	private List<Pair<HashedString, HashedString>> currentGroup = new List<Pair<HashedString, HashedString>>();

	private Dictionary<int, KAnimFileData> fileData = new Dictionary<int, KAnimFileData>();

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
}
