using System;
using UnityEngine;
using UnityEngine.UI;

public class DetailScreenTabHeader : KTabMenuHeader
{
	public override void ActivateTabArtwork(int tabIdx)
	{
		base.ActivateTabArtwork(tabIdx);
		if (tabIdx >= this.transform.childCount)
		{
			return;
		}
		for (int i = 0; i < this.transform.childCount; i++)
		{
			LayoutElement component = this.transform.GetChild(i).GetComponent<LayoutElement>();
			if (component != null)
			{
				if (i == tabIdx)
				{
					component.preferredHeight = this.SelectedHeight;
					component.transform.FindChild("Icon").GetComponent<Image>().color = new Color(0.14509805f, 0.16470589f, 0.23137255f);
				}
				else
				{
					component.preferredHeight = this.UnselectedHeight;
					component.transform.FindChild("Icon").GetComponent<Image>().color = new Color(0.35686275f, 0.37254903f, 0.4509804f);
				}
			}
		}
	}

	public float SelectedHeight = 36f;

	public float UnselectedHeight = 30f;
}
