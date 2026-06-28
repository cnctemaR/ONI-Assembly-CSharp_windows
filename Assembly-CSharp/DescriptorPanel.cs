using System;
using System.Collections.Generic;
using UnityEngine;

public class DescriptorPanel : KMonoBehaviour
{
	public void SetDescriptors(IList<Descriptor> descriptors)
	{
		int i;
		for (i = 0; i < descriptors.Count; i++)
		{
			GameObject gameObject;
			if (i >= this.labels.Count)
			{
				gameObject = Util.KInstantiate(ScreenPrefabs.Instance.DescriptionLabel, base.gameObject, null);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				this.labels.Add(gameObject);
			}
			else
			{
				gameObject = this.labels[i];
			}
			gameObject.GetComponent<LocText>().text = descriptors[i].IndentedText();
			gameObject.GetComponent<ToolTip>().toolTip = descriptors[i].tooltipText;
			gameObject.SetActive(true);
		}
		while (i < this.labels.Count)
		{
			this.labels[i].SetActive(false);
			i++;
		}
	}

	private List<GameObject> labels = new List<GameObject>();
}
