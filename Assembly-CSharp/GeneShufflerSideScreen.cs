using System;
using STRINGS;
using UnityEngine;

public class GeneShufflerSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.button.onClick += delegate
		{
			this.target.SetWorkTime(0f);
		};
		this.Refresh();
	}

	public override void SetTarget(GameObject target)
	{
		GeneShuffler component = target.GetComponent<GeneShuffler>();
		if (component == null)
		{
			global::Debug.LogError("Target doesn't have a GeneShuffler associated with it.", null);
		}
		else
		{
			this.target = component;
			this.Refresh();
		}
	}

	private void Refresh()
	{
		if (this.target != null)
		{
			if (this.target.WorkComplete)
			{
				this.contents.SetActive(true);
				this.label.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.COMPLETE;
				this.button.gameObject.SetActive(true);
			}
			else if (this.target.IsConsumed)
			{
				this.contents.SetActive(true);
				this.label.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.CONSUMED;
				this.button.gameObject.SetActive(false);
			}
			else if (this.target.IsWorking)
			{
				this.contents.SetActive(true);
				this.label.text = UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.UNDERWAY;
				this.button.gameObject.SetActive(false);
			}
			else
			{
				this.contents.SetActive(false);
			}
		}
		else
		{
			this.contents.SetActive(false);
		}
	}

	[SerializeField]
	private LocText label;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private GeneShuffler target;

	[SerializeField]
	private GameObject contents;
}
