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
		TemplateCache.baseTemplatePath = Application.streamingAssetsPath + "/templates";
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

	public static TemplateContainer GetBaseStartingTemplate()
	{
		if (TemplateCache.baseTemplatePath == null)
		{
			TemplateCache.Init();
		}
		string text = Path.Combine(TemplateCache.baseTemplatePath, "bases/startingBase.yaml");
		return YamlIO<TemplateContainer>.LoadFile(text, null);
	}

	public static TemplateContainer GetTemplate(string templatePath)
	{
		if (!TemplateCache.templates.ContainsKey(templatePath))
		{
			TemplateCache.templates.Add(templatePath, null);
		}
		if (TemplateCache.templates[templatePath] == null)
		{
			string text = Path.Combine(TemplateCache.baseTemplatePath, templatePath);
			TemplateContainer templateContainer = YamlIO<TemplateContainer>.LoadFile(text + ".yaml", null);
			if (templateContainer == null)
			{
				global::Debug.LogWarning("Missing template [" + text + ".yaml]", null);
			}
			TemplateCache.templates[templatePath] = templateContainer;
		}
		return TemplateCache.templates[templatePath];
	}

	public static List<string> CollectBaseTemplateNames(string folder = "bases/")
	{
		List<string> list = new List<string>();
		string text = Path.Combine(TemplateCache.baseTemplatePath, folder);
		string[] files = Directory.GetFiles(text, "*.yaml");
		foreach (string text2 in files)
		{
			string text3 = folder + Path.GetFileNameWithoutExtension(text2);
			list.Add(text3);
			if (!TemplateCache.templates.ContainsKey(text3))
			{
				TemplateCache.templates.Add(text3, null);
			}
		}
		list.Sort((string x, string y) => x.CompareTo(y));
		return list;
	}

	public static List<TemplateContainer> CollectBaseTemplateAssets(string folder = "bases/")
	{
		List<TemplateContainer> list = new List<TemplateContainer>();
		string text = Path.Combine(TemplateCache.baseTemplatePath, folder);
		string[] files = Directory.GetFiles(text, "*.yaml");
		WorkItemCollection<TemplateCache.ParseTemplateWorkItem, object> workItemCollection = new WorkItemCollection<TemplateCache.ParseTemplateWorkItem, object>();
		workItemCollection.Reset(null);
		foreach (string text2 in files)
		{
			workItemCollection.Add(new TemplateCache.ParseTemplateWorkItem
			{
				path = text2
			});
		}
		GlobalJobManager.Run(workItemCollection);
		for (int j = 0; j < workItemCollection.Count; j++)
		{
			if (workItemCollection.GetWorkItem(j).template != null)
			{
				list.Add(workItemCollection.GetWorkItem(j).template);
			}
		}
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

	private struct ParseTemplateWorkItem : IWorkItem<object>
	{
		public void Run(object shared_data)
		{
			this.template = YamlIO<TemplateContainer>.LoadFile(this.path, null);
		}

		public string path;

		public TemplateContainer template;
	}
}
