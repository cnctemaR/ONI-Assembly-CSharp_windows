using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

public static class TemplateCache
{
	public static void Init()
	{
		TemplateCache.templates = new Dictionary<string, TemplateContainer>();
		TemplateCache.baseTemplatePath = FileSystem.Normalize(Path.Combine(Application.streamingAssetsPath, "templates"));
	}

	public static void Clear()
	{
		TemplateCache.templates = null;
		TemplateCache.baseTemplatePath = null;
	}

	public static string GetTemplatePath()
	{
		return TemplateCache.baseTemplatePath;
	}

	public static TemplateContainer GetStartingBaseTemplate(string startingTemplateName)
	{
		DebugUtil.Assert(startingTemplateName != null, "Tried loading a starting template named ", startingTemplateName);
		if (TemplateCache.baseTemplatePath == null)
		{
			TemplateCache.Init();
		}
		return TemplateCache.GetTemplate(Path.Combine("bases", startingTemplateName));
	}

	public static TemplateContainer GetTemplate(string templatePath)
	{
		if (!TemplateCache.templates.ContainsKey(templatePath))
		{
			TemplateCache.templates.Add(templatePath, null);
		}
		if (TemplateCache.templates[templatePath] == null)
		{
			string text = FileSystem.Normalize(Path.Combine(TemplateCache.baseTemplatePath, templatePath));
			TemplateContainer templateContainer = YamlIO.LoadFile<TemplateContainer>(text + ".yaml", null, null);
			if (templateContainer == null)
			{
				global::Debug.LogWarning("Missing template [" + text + ".yaml]");
			}
			TemplateCache.templates[templatePath] = templateContainer;
		}
		return TemplateCache.templates[templatePath];
	}

	private static void GetAssetPaths(string folder, List<string> paths)
	{
		FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(TemplateCache.baseTemplatePath, folder)), "*.yaml", paths);
	}

	public static List<string> CollectBaseTemplateNames(string folder = "bases")
	{
		List<string> list = new List<string>();
		ListPool<string, TemplateContainer>.PooledList pooledList = ListPool<string, TemplateContainer>.Allocate();
		TemplateCache.GetAssetPaths(folder, pooledList);
		foreach (string text in pooledList)
		{
			string text2 = FileSystem.Normalize(Path.Combine(folder, Path.GetFileNameWithoutExtension(text)));
			list.Add(text2);
			if (!TemplateCache.templates.ContainsKey(text2))
			{
				TemplateCache.templates.Add(text2, null);
			}
		}
		pooledList.Recycle();
		list.Sort((string x, string y) => x.CompareTo(y));
		return list;
	}

	public static List<TemplateContainer> CollectBaseTemplateAssets(string folder = "bases")
	{
		List<TemplateContainer> list = new List<TemplateContainer>();
		ListPool<string, TemplateContainer>.PooledList pooledList = ListPool<string, TemplateContainer>.Allocate();
		TemplateCache.GetAssetPaths(folder, pooledList);
		foreach (string text in pooledList)
		{
			list.Add(YamlIO.LoadFile<TemplateContainer>(text, null, null));
		}
		pooledList.Recycle();
		list.Sort(delegate(TemplateContainer x, TemplateContainer y)
		{
			if (y.priority - x.priority == 0)
			{
				return x.name.CompareTo(y.name);
			}
			return y.priority - x.priority;
		});
		return list;
	}

	private static string baseTemplatePath;

	private static Dictionary<string, TemplateContainer> templates;

	private const string defaultAssetFolder = "bases";
}
