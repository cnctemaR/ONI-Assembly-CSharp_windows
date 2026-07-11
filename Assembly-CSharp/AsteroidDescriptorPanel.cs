using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/AsteroidDescriptorPanel")]
public class AsteroidDescriptorPanel : KMonoBehaviour
{
	public bool HasDescriptors()
	{
		return this.labels.Count > 0;
	}

	public void SetDescriptors(IList<AsteroidDescriptor> descriptors)
	{
		int i;
		for (i = 0; i < descriptors.Count; i++)
		{
			GameObject gameObject;
			if (i >= this.labels.Count)
			{
				gameObject = Util.KInstantiate((this.customLabelPrefab != null) ? this.customLabelPrefab : ScreenPrefabs.Instance.DescriptionLabel, base.gameObject, null);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				this.labels.Add(gameObject);
			}
			else
			{
				gameObject = this.labels[i];
			}
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("Label").text = descriptors[i].text;
			component.GetReference<ToolTip>("ToolTip").toolTip = descriptors[i].tooltip;
			if (descriptors[i].bands != null)
			{
				Transform reference = component.GetReference<Transform>("BandContainer");
				Transform reference2 = component.GetReference<Transform>("BarBitPrefab");
				int j;
				for (j = 0; j < descriptors[i].bands.Count; j++)
				{
					Transform transform;
					if (j >= reference.childCount)
					{
						transform = Util.KInstantiateUI<Transform>(reference2.gameObject, reference.gameObject, false);
					}
					else
					{
						transform = reference.GetChild(j);
					}
					Image component2 = transform.GetComponent<Image>();
					LayoutElement component3 = transform.GetComponent<LayoutElement>();
					component2.color = descriptors[i].bands[j].second;
					component3.flexibleWidth = descriptors[i].bands[j].third;
					transform.GetComponent<ToolTip>().toolTip = descriptors[i].bands[j].first;
					transform.gameObject.SetActive(true);
				}
				while (j < reference.childCount)
				{
					reference.GetChild(j).gameObject.SetActive(false);
					j++;
				}
			}
			gameObject.SetActive(true);
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
