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
	}

	[StringEnumConverter]
	public AnimCommandFile.ConfigType Type { get; private set; }

	[StringEnumConverter]
	public AnimCommandFile.GroupBy TagGroup { get; private set; }

	[StringEnumConverter]
	public AnimCommandFile.ParseOrder Order { get; private set; }

	[StringEnumConverter]
	public KAnimBatchGroup.RendererType RendererType { get; private set; }

	[StringEnumConverter]
	public KAnimBatchGroup.MaterialType MaterialType { get; private set; }

	public string TargetBuild { get; private set; }

	public string AnimTargetBuild { get; private set; }

	public string SwapTargetBuild { get; private set; }

	public Dictionary<string, List<string>> DefaultBuilds { get; private set; }

	public bool LookupSymbolUnderGroupName { get; private set; }

	public bool MultiInstance { get; private set; }

	public int MaxGroupSize { get; private set; }

	public bool IsSwap()
	{
		if (this.TagGroup != AnimCommandFile.GroupBy.NamedGroup)
		{
			return false;
		}
		string fileName = Path.GetFileName(this.directory);
		foreach (KeyValuePair<string, List<string>> keyValuePair in this.DefaultBuilds)
		{
			if (keyValuePair.Value.Contains(fileName))
			{
				return false;
			}
		}
		return true;
	}

	public string GetGroupName()
	{
		string text = Path.GetFileName(this.directory);
		string fullName = Directory.GetParent(this.directory).FullName;
		string siblingIndex = this.GetSiblingIndex(fullName, text);
		switch (this.TagGroup)
		{
		case AnimCommandFile.GroupBy.__IGNORE__:
			return null;
		case AnimCommandFile.GroupBy.Folder:
			text = Directory.GetParent(this.directory).Name;
			text += siblingIndex;
			break;
		case AnimCommandFile.GroupBy.NamedGroup:
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.DefaultBuilds)
			{
				if (keyValuePair.Value.Contains(text))
				{
					return keyValuePair.Key;
				}
			}
			text = this.TargetBuild;
			break;
		case AnimCommandFile.GroupBy.NamedGroupNoSplit:
			text = this.TargetBuild;
			break;
		}
		return text;
	}

	private string GetSiblingIndex(string parentDirectory, string target)
	{
		string[] directories = Directory.GetDirectories(parentDirectory);
		string text = string.Empty;
		if (directories.Length > 10)
		{
			for (int i = 0; i < directories.Length; i++)
			{
				string fileName = Path.GetFileName(directories[i]);
				if (fileName == target)
				{
					text = (i / 10).ToString();
					break;
				}
			}
		}
		return text;
	}

	[NonSerialized]
	public string directory = string.Empty;

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
		ParentFolder,
		ParentConfig,
		NamedGroup,
		NamedGroupNoSplit
	}

	public enum ParseOrder
	{
		Default,
		ParseDefaultBuildThenAnimsThenSwaps
	}
}
