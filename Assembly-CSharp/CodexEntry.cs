using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CodexEntry
{
	public CodexEntry()
	{
	}

	public CodexEntry(string category, List<ContentContainer> contentContainers, string name)
	{
		this.category = category;
		this.name = name;
		this.contentContainers = contentContainers;
		if (string.IsNullOrEmpty(this.sortString))
		{
			this.sortString = UI.StripLinkFormatting(name);
		}
	}

	public CodexEntry(string category, string titleKey, List<ContentContainer> contentContainers)
	{
		this.category = category;
		this.title = titleKey;
		this.contentContainers = contentContainers;
		if (string.IsNullOrEmpty(this.sortString))
		{
			this.sortString = UI.StripLinkFormatting(this.title);
		}
	}

	public List<ContentContainer> contentContainers
	{
		get
		{
			return this._contentContainers;
		}
		private set
		{
			this._contentContainers = value;
		}
	}

	public static List<string> ContentContainerDebug(List<ContentContainer> _contentContainers)
	{
		List<string> list = new List<string>();
		foreach (ContentContainer contentContainer in _contentContainers)
		{
			if (contentContainer != null)
			{
				string text = string.Concat(new object[]
				{
					"<b>",
					contentContainer.contentLayout.ToString(),
					" container: ",
					(contentContainer.content != null) ? contentContainer.content.Count : 0,
					" items</b>"
				});
				if (contentContainer.content != null)
				{
					text += "\n";
					for (int i = 0; i < contentContainer.content.Count; i++)
					{
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							"    • ",
							contentContainer.content[i].ToString(),
							": ",
							CodexEntry.GetContentWidgetDebugString(contentContainer.content[i]),
							"\n"
						});
					}
				}
				list.Add(text);
			}
			else
			{
				list.Add("null container");
			}
		}
		return list;
	}

	private static string GetContentWidgetDebugString(ICodexWidget widget)
	{
		CodexText codexText = widget as CodexText;
		if (codexText != null)
		{
			return codexText.text;
		}
		CodexLabelWithIcon codexLabelWithIcon = widget as CodexLabelWithIcon;
		if (codexLabelWithIcon != null)
		{
			return codexLabelWithIcon.label.text + " / " + codexLabelWithIcon.icon.spriteName;
		}
		CodexImage codexImage = widget as CodexImage;
		if (codexImage != null)
		{
			return codexImage.spriteName;
		}
		CodexVideo codexVideo = widget as CodexVideo;
		if (codexVideo != null)
		{
			return codexVideo.name;
		}
		CodexIndentedLabelWithIcon codexIndentedLabelWithIcon = widget as CodexIndentedLabelWithIcon;
		if (codexIndentedLabelWithIcon != null)
		{
			return codexIndentedLabelWithIcon.label.text + " / " + codexIndentedLabelWithIcon.icon.spriteName;
		}
		return string.Empty;
	}

	public void CreateContentContainerCollection()
	{
		this.contentContainers = new List<ContentContainer>();
	}

	public void InsertContentContainer(int index, ContentContainer container)
	{
		this.contentContainers.Insert(index, container);
	}

	public void RemoveContentContainerAt(int index)
	{
		this.contentContainers.RemoveAt(index);
	}

	public void AddContentContainer(ContentContainer container)
	{
		this.contentContainers.Add(container);
	}

	public void AddContentContainerRange(IEnumerable<ContentContainer> containers)
	{
		this.contentContainers.AddRange(containers);
	}

	public void RemoveContentContainer(ContentContainer container)
	{
		this.contentContainers.Remove(container);
	}

	public ICodexWidget GetFirstWidget()
	{
		for (int i = 0; i < this.contentContainers.Count; i++)
		{
			if (this.contentContainers[i].content != null)
			{
				for (int j = 0; j < this.contentContainers[i].content.Count; j++)
				{
					if (this.contentContainers[i].content[j] != null)
					{
						return this.contentContainers[i].content[j];
					}
				}
			}
		}
		return null;
	}

	public string id
	{
		get
		{
			return this._id;
		}
		set
		{
			this._id = value;
		}
	}

	public string parentId
	{
		get
		{
			return this._parentId;
		}
		set
		{
			this._parentId = value;
		}
	}

	public string category
	{
		get
		{
			return this._category;
		}
		set
		{
			this._category = value;
		}
	}

	public string title
	{
		get
		{
			return this._title;
		}
		set
		{
			this._title = value;
		}
	}

	public string name
	{
		get
		{
			return this._name;
		}
		set
		{
			this._name = value;
		}
	}

	public string subtitle
	{
		get
		{
			return this._subtitle;
		}
		set
		{
			this._subtitle = value;
		}
	}

	public List<SubEntry> subEntries
	{
		get
		{
			return this._subEntries;
		}
		set
		{
			this._subEntries = value;
		}
	}

	public Sprite icon
	{
		get
		{
			return this._icon;
		}
		set
		{
			this._icon = value;
		}
	}

	public Color iconColor
	{
		get
		{
			return this._iconColor;
		}
		set
		{
			this._iconColor = value;
		}
	}

	public string iconPrefabID
	{
		get
		{
			return this._iconPrefabID;
		}
		set
		{
			this._iconPrefabID = value;
		}
	}

	public bool disabled
	{
		get
		{
			return this._disabled;
		}
		set
		{
			this._disabled = value;
		}
	}

	public bool searchOnly
	{
		get
		{
			return this._searchOnly;
		}
		set
		{
			this._searchOnly = value;
		}
	}

	public int customContentLength
	{
		get
		{
			return this._customContentLength;
		}
		set
		{
			this._customContentLength = value;
		}
	}

	public string sortString
	{
		get
		{
			return this._sortString;
		}
		set
		{
			this._sortString = value;
		}
	}

	public bool showBeforeGeneratedCategoryLinks
	{
		get
		{
			return this._showBeforeGeneratedCategoryLinks;
		}
		set
		{
			this._showBeforeGeneratedCategoryLinks = value;
		}
	}

	public EntryDevLog log = new EntryDevLog();

	private List<ContentContainer> _contentContainers = new List<ContentContainer>();

	private string _id;

	private string _parentId;

	private string _category;

	private string _title;

	private string _name;

	private string _subtitle;

	private List<SubEntry> _subEntries = new List<SubEntry>();

	private Sprite _icon;

	private Color _iconColor = Color.white;

	private string _iconPrefabID;

	private bool _disabled;

	private bool _searchOnly;

	private int _customContentLength;

	private string _sortString;

	private bool _showBeforeGeneratedCategoryLinks;
}
