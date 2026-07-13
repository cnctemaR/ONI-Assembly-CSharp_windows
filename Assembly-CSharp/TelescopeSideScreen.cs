using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class TelescopeSideScreen : SideScreenContent
{
	public TelescopeSideScreen()
	{
		this.refreshDisplayStateDelegate = new Action<object>(this.RefreshDisplayState);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.selectStarmapScreen.onClick += delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
		SpacecraftManager.instance.Subscribe(532901469, this.refreshDisplayStateDelegate);
		this.RefreshDisplayState(null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.RefreshDisplayState(null);
		this.target = SelectTool.Instance.selected.GetComponent<KMonoBehaviour>().gameObject;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.target)
		{
			this.target = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.target)
		{
			this.target = null;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Telescope>() != null;
	}

	private void RefreshDisplayState(object _ = null)
	{
		if (SelectTool.Instance.selected == null)
		{
			return;
		}
		if (SelectTool.Instance.selected.GetComponent<Telescope>() == null)
		{
			return;
		}
		if (!SpacecraftManager.instance.HasAnalysisTarget())
		{
			this.DescriptionText.text = "<b><color=#FF0000>" + UI.UISIDESCREENS.TELESCOPESIDESCREEN.NO_SELECTED_ANALYSIS_TARGET + "</color></b>";
			return;
		}
		string text = UI.UISIDESCREENS.TELESCOPESIDESCREEN.ANALYSIS_TARGET_SELECTED;
		this.DescriptionText.text = text;
	}

	public KButton selectStarmapScreen;

	public Image researchButtonIcon;

	public GameObject content;

	private GameObject target;

	private Action<object> refreshDisplayStateDelegate;

	public LocText DescriptionText;
}
