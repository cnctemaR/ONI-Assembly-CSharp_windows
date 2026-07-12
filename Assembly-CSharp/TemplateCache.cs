using System;
using System.Collections.Generic;
using Klei;
using ProcGen;

public static class TemplateCache
{
	public static bool Initted { get; private set; }

	public static void Init()
	{
		if (TemplateCache.Initted)
		{
			return;
		}
		TemplateCache.templates = new Dictionary<string, TemplateContainer>();
		TemplateCache.Initted = true;
	}

	public static void Clear()
	{
		TemplateCache.templates = null;
		TemplateCache.Initted = false;
	}

	public static string RewriteTemplatePath(string scopePath)
	{
		string text;
		string text2;
		SettingsCache.GetDlcIdAndPath(scopePath, out text, out text2);
		return SettingsCache.GetAbsoluteContentPath(text, "templates/" + text2);
	}

	public static string RewriteTemplateYaml(string scopePath)
	{
		return TemplateCache.RewriteTemplatePath(scopePath) + ".yaml";
	}

	public static TemplateContainer GetTemplate(string templatePath)
	{
		if (!TemplateCache.templates.ContainsKey(templatePath))
		{
			TemplateCache.templates.Add(templatePath, null);
		}
		if (TemplateCache.templates[templatePath] == null)
		{
			string text = TemplateCache.RewriteTemplateYaml(templatePath);
			TemplateContainer templateContainer = YamlIO.LoadFile<TemplateContainer>(text, null, null);
			if (templateContainer == null)
			{
				Debug.LogWarning("Missing template [" + text + "]");
			}
			templateContainer.name = templatePath;
			TemplateCache.templates[templatePath] = templateContainer;
		}
		return TemplateCache.templates[templatePath];
	}

	public static bool TemplateExists(string templatePath)
	{
		return FileSystem.FileExists(TemplateCache.RewriteTemplateYaml(templatePath));
	}

	private const string defaultAssetFolder = "bases";

	private static Dictionary<string, TemplateContainer> templates;
}
