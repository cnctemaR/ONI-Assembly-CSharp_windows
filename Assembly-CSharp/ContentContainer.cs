using System;
using System.Collections.Generic;
using KSerialization.Converters;
using UnityEngine;

public class ContentContainer
{
	public ContentContainer()
	{
		this.content = new List<ICodexWidget>();
	}

	public ContentContainer(List<ICodexWidget> content, ContentContainer.ContentLayout contentLayout)
	{
		this.content = content;
		this.contentLayout = contentLayout;
	}

	public List<ICodexWidget> content { get; set; }

	public string lockID { get; set; }

	[StringEnumConverter]
	public ContentContainer.ContentLayout contentLayout { get; set; }

	public bool showBeforeGeneratedContent { get; set; }

	public GameObject go;

	public enum ContentLayout
	{
		Vertical,
		Horizontal,
		Grid,
		GridTwoColumn,
		GridTwoColumnTall
	}
}
