using System;
using System.Collections.Generic;
using Klei;
using KSerialization.Converters;

public class CodexWidget : YamlIO<CodexWidget>
{
	public CodexWidget()
	{
		this.properties = new Dictionary<string, string>();
		this.objectProperties = new Dictionary<string, object>();
		this.properties["preferredWidth"] = "-1";
		this.properties["preferredHeight"] = "-1";
	}

	public CodexWidget(CodexWidget.ContentType type)
		: this(type, new Dictionary<string, string>())
	{
	}

	public CodexWidget(CodexWidget.ContentType type, Dictionary<string, string> properties, Dictionary<string, object> objectProperties)
	{
		this.type = type;
		this.properties = properties;
		this.objectProperties = objectProperties;
		if (!properties.ContainsKey("preferredWidth"))
		{
			properties["preferredWidth"] = "-1";
		}
		if (!properties.ContainsKey("preferredHeight"))
		{
			properties["preferredHeight"] = "-1";
		}
	}

	public CodexWidget(CodexWidget.ContentType type, Dictionary<string, string> properties)
	{
		this.type = type;
		this.properties = properties;
		this.objectProperties = new Dictionary<string, object>();
		if (!properties.ContainsKey("preferredWidth"))
		{
			properties["preferredWidth"] = "-1";
		}
		if (!properties.ContainsKey("preferredHeight"))
		{
			properties["preferredHeight"] = "-1";
		}
	}

	[StringEnumConverter]
	public CodexWidget.ContentType type { get; set; }

	public Dictionary<string, string> properties { get; set; }

	public Dictionary<string, object> objectProperties { get; set; }

	public enum ContentType
	{
		Text,
		Image,
		DividerLine,
		Spacer,
		LabelWithIcon,
		LENGTH
	}
}
