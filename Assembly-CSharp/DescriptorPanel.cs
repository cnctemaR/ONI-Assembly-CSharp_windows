using System;
using System.Collections.Generic;
using UnityEngine;

public class DescriptorPanel : KMonoBehaviour
{
	public bool HasDescriptors()
	{
		return this.labels.Count > 0;
	}

	public void SetDescriptors(IList<Descriptor> descriptors)
	{
		int i;
		for (i = 0; i < descriptors.Count; i++)
		{
			GameObject gameObject2;
			if (i >= this.labels.Count)
			{
				GameObject gameObject = ((!(this.customLabelPrefab != null)) ? ScreenPrefabs.Instance.DescriptionLabel : this.customLabelPrefab);
				gameObject2 = Util.KInstantiate(gameObject, base.gameObject, null);
				gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
				this.labels.Add(gameObject2);
			}
			else
			{
				gameObject2 = this.labels[i];
			}
			gameObject2.GetComponent<LocText>().text = descriptors[i].IndentedText();
			gameObject2.GetComponent<ToolTip>().toolTip = descriptors[i].tooltipText;
			gameObject2.SetActive(true);
		}
		while (i < this.labels.Count)
		{
			this.labels[i].SetActive(false);
			i++;
		}
	}

	[SerializeField]
	private GameObject customLabelPrefab;

	private List<GameObject> labels = new List<GameObject>();
}
