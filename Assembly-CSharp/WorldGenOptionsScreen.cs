using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class WorldGenOptionsScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.title.SetText(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.TITLE);
		this.dismissButton.onClick += delegate
		{
			this.Deactivate();
		};
		LocText component = this.dismissButton.transform.GetChild(0).GetComponent<LocText>();
		component.SetText(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.DONE_BUTTON);
		this.closeButton.onClick += delegate
		{
			this.Deactivate();
		};
		this.useSeedLable.SetText(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.USE_SEED);
		this.useSeedInput.onValueChanged.AddListener(delegate
		{
			this.OnSeedChange();
		});
		this.useSeedToggle.onValueChanged.AddListener(delegate(bool b)
		{
			KPlayerPrefs.SetInt(OfflineWorldGen.USE_WORLD_SEED_KEY, (!b) ? 0 : 1);
		});
		this.useSeedToggle.isOn = KPlayerPrefs.GetInt(OfflineWorldGen.USE_WORLD_SEED_KEY, 0) == 1;
		int @int = KPlayerPrefs.GetInt(OfflineWorldGen.WORLD_SEED_KEY, -1);
		this.useSeedInput.text = ((@int != -1) ? @int.ToString() : string.Empty);
		this.randomiseButton.onClick += delegate
		{
			this.GetNewRandom();
		};
		LocText component2 = this.randomiseButton.transform.GetChild(0).GetComponent<LocText>();
		component2.SetText(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.RANDOM_BUTTON);
		GameObject gameObject = this.randomiseButton.transform.GetChild(0).gameObject;
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.WORLD_GEN_OPTIONS_SCREEN.RANDOM_BUTTON_TOOLTIP);
	}

	public void OnSeedChange()
	{
		int num;
		try
		{
			num = Convert.ToInt32(this.useSeedInput.text);
		}
		catch
		{
			num = 0;
		}
		OfflineWorldGen.SetSeed(num);
	}

	private void GetNewRandom()
	{
		int num = global::UnityEngine.Random.Range(0, int.MaxValue);
		this.useSeedInput.text = num.ToString();
		OfflineWorldGen.SetSeed(num);
	}

	public LocText title;

	public KButton dismissButton;

	public KButton closeButton;

	public LocText useSeedLable;

	public KButton randomiseButton;

	public InputField useSeedInput;

	public Toggle useSeedToggle;
}
