using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using Klei.CustomSettings;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SaveUpgradeWarning")]
public class SaveUpgradeWarning : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Game game = this.game;
		game.OnLoad = (Action<Game.GameSaveData>)Delegate.Combine(game.OnLoad, new Action<Game.GameSaveData>(this.OnLoad));
	}

	protected override void OnCleanUp()
	{
		Game game = this.game;
		game.OnLoad = (Action<Game.GameSaveData>)Delegate.Remove(game.OnLoad, new Action<Game.GameSaveData>(this.OnLoad));
		base.OnCleanUp();
	}

	private void OnLoad(Game.GameSaveData data)
	{
		foreach (SaveUpgradeWarning.Upgrade upgrade in new List<SaveUpgradeWarning.Upgrade>
		{
			new SaveUpgradeWarning.Upgrade(7, 5, new global::System.Action(this.SuddenMoraleHelper)),
			new SaveUpgradeWarning.Upgrade(7, 13, new global::System.Action(this.BedAndBathHelper)),
			new SaveUpgradeWarning.Upgrade(7, 16, new global::System.Action(this.NewAutomationWarning))
		})
		{
			if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(upgrade.major, upgrade.minor))
			{
				upgrade.action();
			}
		}
	}

	private void SuddenMoraleHelper()
	{
		Effect morale_effect = Db.Get().effects.Get("SuddenMoraleHelper");
		CustomizableDialogScreen screen = Util.KInstantiateUI<CustomizableDialogScreen>(ScreenPrefabs.Instance.CustomizableDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, true);
		screen.AddOption(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_BUFF, delegate
		{
			foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
			{
				minionIdentity.GetComponent<Effects>().Add(morale_effect, true);
			}
			screen.Deactivate();
		});
		screen.AddOption(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_DISABLE, delegate
		{
			SettingConfig morale = CustomGameSettingConfigs.Morale;
			CustomGameSettings.Instance.customGameMode = CustomGameSettings.CustomGameMode.Custom;
			CustomGameSettings.Instance.SetQualitySetting(morale, morale.GetLevel("Disabled").id);
			screen.Deactivate();
		});
		screen.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER, Mathf.RoundToInt(morale_effect.duration / 600f)), UI.FRONTEND.SAVEUPGRADEWARNINGS.SUDDENMORALEHELPER_TITLE, null);
	}

	private void BedAndBathHelper()
	{
		if (SaveGame.Instance == null)
		{
			return;
		}
		ColonyAchievementTracker component = SaveGame.Instance.GetComponent<ColonyAchievementTracker>();
		if (component == null)
		{
			return;
		}
		ColonyAchievement basicComforts = Db.Get().ColonyAchievements.BasicComforts;
		ColonyAchievementStatus colonyAchievementStatus = null;
		if (component.achievements.TryGetValue(basicComforts.Id, out colonyAchievementStatus))
		{
			colonyAchievementStatus.failed = false;
		}
	}

	private void NewAutomationWarning()
	{
		SpriteListDialogScreen screen = Util.KInstantiateUI<SpriteListDialogScreen>(ScreenPrefabs.Instance.SpriteListDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, true);
		screen.AddOption(UI.CONFIRMDIALOG.OK, delegate
		{
			screen.Deactivate();
		});
		string[] array = new string[] { "LiquidVent", "GasVent", "GasVentHighPressure", "SolidVent", "LiquidReservoir", "GasReservoir" };
		for (int i = 0; i < array.Length; i++)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(array[i]);
			screen.AddSprite(buildingDef.GetUISprite("ui", false), buildingDef.Name);
		}
		screen.PopupConfirmDialog(UI.FRONTEND.SAVEUPGRADEWARNINGS.NEWAUTOMATIONWARNING, UI.FRONTEND.SAVEUPGRADEWARNINGS.NEWAUTOMATIONWARNING_TITLE);
	}

	[MyCmpReq]
	private Game game;

	private struct Upgrade
	{
		public Upgrade(int major, int minor, global::System.Action action)
		{
			this.major = major;
			this.minor = minor;
			this.action = action;
		}

		public int major;

		public int minor;

		public global::System.Action action;
	}
}
