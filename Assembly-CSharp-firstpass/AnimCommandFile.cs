using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using KSerialization.Converters;

public class AnimCommandFile : YamlIO<AnimCommandFile>
{
	public AnimCommandFile()
	{
		this.MaxGroupSize = 60;
		this.DefaultBuilds = new Dictionary<string, List<string>>();
		this.TagGroup = AnimCommandFile.GroupBy.DontGroup;
	}

	[StringEnumConverter]
	public AnimCommandFile.ConfigType Type { get; private set; }

	[StringEnumConverter]
	public AnimCommandFile.GroupBy TagGroup { get; private set; }

	[StringEnumConverter]
	public KAnimBatchGroup.RendererType RendererType { get; private set; }

	public string TargetBuild { get; private set; }

	public string AnimTargetBuild { get; private set; }

	public string SwapTargetBuild { get; private set; }

	public Dictionary<string, List<string>> DefaultBuilds { get; private set; }

	public bool LookupSymbolUnderGroupName { get; private set; }

	public bool MultiInstance { get; private set; }

	public int MaxGroupSize { get; private set; }

	public bool IsSwap(KAnimFile file)
	{
		bool flag;
		if (this.TagGroup != AnimCommandFile.GroupBy.NamedGroup)
		{
			flag = false;
		}
		else
		{
			string fileName = Path.GetFileName(file.homedirectory);
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.DefaultBuilds)
			{
				if (keyValuePair.Value.Contains(fileName))
				{
					return false;
				}
			}
			flag = true;
		}
		return flag;
	}

	public void AddGroupFile(KAnimGroupFile.GroupFile gf)
	{
		if (!this.groupFiles.Contains(gf))
		{
			this.groupFiles.Add(gf);
		}
	}

	public string GetGroupName(KAnimFile kaf)
	{
		string text;
		switch (this.TagGroup)
		{
		case AnimCommandFile.GroupBy.__IGNORE__:
			text = null;
			break;
		case AnimCommandFile.GroupBy.DontGroup:
			text = kaf.name;
			break;
		case AnimCommandFile.GroupBy.Folder:
			text = Path.GetFileName(this.directory) + (this.groupFiles.Count / 10).ToString();
			break;
		case AnimCommandFile.GroupBy.NamedGroup:
		{
			string fileName = Path.GetFileName(kaf.homedirectory);
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.DefaultBuilds)
			{
				if (keyValuePair.Value.Contains(fileName))
				{
					return keyValuePair.Key;
				}
			}
			text = this.TargetBuild;
			break;
		}
		case AnimCommandFile.GroupBy.NamedGroupNoSplit:
			text = this.TargetBuild;
			break;
		default:
			text = null;
			break;
		}
		return text;
	}

	[NonSerialized]
	public string directory = "";

	[NonSerialized]
	private List<KAnimGroupFile.GroupFile> groupFiles = new List<KAnimGroupFile.GroupFile>();

	public enum ConfigType
	{
		Default,
		AnimOnly
	}

	public enum GroupBy
	{
		__IGNORE__,
		DontGroup,
		Folder,
		NamedGroup,
		NamedGroupNoSplit
	}
}
