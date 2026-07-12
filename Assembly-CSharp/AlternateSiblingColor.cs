using System;
using UnityEngine;
using UnityEngine.UI;

public class AlternateSiblingColor : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		int siblingIndex = base.transform.GetSiblingIndex();
		this.RefreshColor(siblingIndex % 2 == 0);
	}

	private void RefreshColor(bool evenIndex)
	{
		if (this.image == null)
		{
			return;
		}
		this.image.color = (evenIndex ? this.evenColor : this.oddColor);
	}

	private void Update()
	{
		if (this.mySiblingIndex != base.transform.GetSiblingIndex())
		{
			this.mySiblingIndex = base.transform.GetSiblingIndex();
			this.RefreshColor(this.mySiblingIndex % 2 == 0);
		}
	}

	public Color evenColor;

	public Color oddColor;

	public Image image;

	private int mySiblingIndex;
}
