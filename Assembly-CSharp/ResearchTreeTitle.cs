using System;
using UnityEngine;
using UnityEngine.UI;

public class ResearchTreeTitle : MonoBehaviour
{
	public void SetLabel(string txt)
	{
		this.treeLabel.text = txt;
	}

	public void SetColor(int id)
	{
		this.BG.enabled = id % 2 != 0;
	}

	[Header("References")]
	[SerializeField]
	private LocText treeLabel;

	[SerializeField]
	private Image BG;
}
