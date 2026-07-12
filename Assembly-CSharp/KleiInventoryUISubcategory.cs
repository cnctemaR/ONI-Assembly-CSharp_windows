using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KleiInventoryUISubcategory : KMonoBehaviour
{
	public bool IsOpen
	{
		get
		{
			return this.stateExpanded;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.expandButton.onClick = delegate
		{
			this.ToggleOpen(!this.stateExpanded);
		};
	}

	public void SetIdentity(string label, Sprite icon)
	{
		this.label.SetText(label);
		this.icon.sprite = icon;
	}

	public void RefreshDisplay()
	{
		foreach (GameObject gameObject in this.dummyItems)
		{
			gameObject.SetActive(false);
		}
		int num = 0;
		for (int i = 0; i < this.gridLayout.transform.childCount; i++)
		{
			if (this.gridLayout.transform.GetChild(i).gameObject.activeSelf)
			{
				num++;
			}
		}
		base.gameObject.SetActive(num != 0);
		int j = 0;
		int num2 = num % this.gridLayout.constraintCount;
		if (num2 > 0)
		{
			j = this.gridLayout.constraintCount - num2;
		}
		while (j > this.dummyItems.Count)
		{
			this.dummyItems.Add(Util.KInstantiateUI(this.dummyPrefab, this.gridLayout.gameObject, false));
		}
		for (int k = 0; k < j; k++)
		{
			this.dummyItems[k].SetActive(true);
			this.dummyItems[k].transform.SetAsLastSibling();
		}
		this.headerLayout.minWidth = base.transform.parent.rectTransform().rect.width - 8f;
	}

	public void ToggleOpen(bool open)
	{
		this.gridLayout.gameObject.SetActive(open);
		this.stateExpanded = open;
		this.expandButton.ChangeState(this.stateExpanded ? 1 : 0);
	}

	[SerializeField]
	private GameObject dummyPrefab;

	public string subcategoryID;

	public GridLayoutGroup gridLayout;

	public List<GameObject> dummyItems;

	[SerializeField]
	private LayoutElement headerLayout;

	[SerializeField]
	private Image icon;

	[SerializeField]
	private LocText label;

	[SerializeField]
	private MultiToggle expandButton;

	private bool stateExpanded = true;
}
