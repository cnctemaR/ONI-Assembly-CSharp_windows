using System;
using System.Collections.Generic;
using Klei;
using KSerialization.Converters;

public class ContentContainer : YamlIO<ContentContainer>
{
	public ContentContainer()
	{
		this.content = new List<CodexWidget>();
	}

	public ContentContainer(List<CodexWidget> content, ContentContainer.ContentLayout contentLayout)
	{
		this.content = content;
		this.contentLayout = contentLayout;
	}

	public List<CodexWidget> content { get; set; }

	public string lockID { get; set; }

	[StringEnumConverter]
	public ContentContainer.ContentLayout contentLayout { get; set; }

	public bool showBeforeGeneratedContent { get; set; }

	public enum ContentLayout
	{
		Vertical,
		Horizontal,
		Grid
	}
}
