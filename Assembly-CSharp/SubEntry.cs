using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SubEntry : IHasDlcRestrictions
{
	public SubEntry()
	{
	}

	public SubEntry(string id, string parentEntryID, List<ContentContainer> contentContainers, string name)
	{
		this.id = id;
		this.parentEntryID = parentEntryID;
		this.name = name;
		this.contentContainers = contentContainers;
		if (!string.IsNullOrEmpty(this.lockID))
		{
			foreach (ContentContainer contentContainer in contentContainers)
			{
				contentContainer.lockID = this.lockID;
			}
		}
		if (string.IsNullOrEmpty(this.sortString))
		{
			if (!string.IsNullOrEmpty(this.title))
			{
				this.sortString = UI.StripLinkFormatting(this.title);
				return;
			}
			this.sortString = UI.StripLinkFormatting(name);
		}
	}

	public List<ContentContainer> contentContainers { get; set; }

	public string parentEntryID { get; set; }

	public string id { get; set; }

	public string name { get; set; }

	public string title { get; set; }

	public string subtitle { get; set; }

	public Sprite icon { get; set; }

	public int layoutPriority { get; set; }

	public bool disabled { get; set; }

	public string lockID { get; set; }

	public string[] requiredDlcIds { get; set; }

	public string[] forbiddenDlcIds { get; set; }

	public string[] GetRequiredDlcIds()
	{
		return this.requiredDlcIds;
	}

	public string[] GetForbiddenDlcIds()
	{
		return this.forbiddenDlcIds;
	}

	public string sortString { get; set; }

	public ContentContainer lockedContentContainer;

	public Color iconColor = Color.white;
}
