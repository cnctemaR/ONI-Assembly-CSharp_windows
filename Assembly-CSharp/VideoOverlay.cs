using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/VideoOverlay")]
public class VideoOverlay : KMonoBehaviour
{
	public void SetText(List<string> strings)
	{
		if (strings.Count != this.textFields.Count)
		{
			DebugUtil.LogErrorArgs(new object[]
			{
				base.name,
				"expects",
				this.textFields.Count,
				"strings passed to it, got",
				strings.Count
			});
		}
		for (int i = 0; i < this.textFields.Count; i++)
		{
			this.textFields[i].text = strings[i];
		}
	}

	public List<LocText> textFields;
}
