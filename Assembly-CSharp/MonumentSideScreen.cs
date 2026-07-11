using System;
using System.Collections.Generic;
using UnityEngine;

public class MonumentSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<MonumentPart>() != null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.debugVictoryButton.onClick += delegate
		{
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Thriving.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Clothe8Dupes.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.Build4NatureReserves.Id);
			SaveGame.Instance.GetComponent<ColonyAchievementTracker>().DebugTriggerAchievement(Db.Get().ColonyAchievements.ReachedSpace.Id);
			GameScheduler.Instance.Schedule("ForceCheckAchievements", 0.1f, delegate(object data)
			{
				Game.Instance.Trigger(395452326, null);
			}, null, null);
		};
		this.debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && this.target.part == MonumentPart.Part.Top);
		this.flipButton.onClick += delegate
		{
			this.target.GetComponent<Rotatable>().Rotate();
		};
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.target = target.GetComponent<MonumentPart>();
		this.debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && this.target.part == MonumentPart.Part.Top);
		this.GenerateStateButtons();
	}

	public void GenerateStateButtons()
	{
		for (int i = this.buttons.Count - 1; i >= 0; i--)
		{
			Util.KDestroyGameObject(this.buttons[i]);
		}
		this.buttons.Clear();
		foreach (Tuple<string, string> tuple in this.target.selectableStatesAndSymbols)
		{
			GameObject gameObject = Util.KInstantiateUI(this.stateButtonPrefab, this.buttonContainer.gameObject, true);
			string targetState = tuple.first;
			string second = tuple.second;
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				this.target.SetState(targetState);
			};
			this.buttons.Add(gameObject);
			KAnimFile kanimFile = this.target.GetComponent<KBatchedAnimController>().AnimFiles[0];
			gameObject.GetComponent<KButton>().fgImage.sprite = Def.GetUISpriteFromMultiObjectAnim(kanimFile, targetState, false, second);
		}
	}

	private MonumentPart target;

	public KButton debugVictoryButton;

	public KButton flipButton;

	public GameObject stateButtonPrefab;

	private List<GameObject> buttons = new List<GameObject>();

	[SerializeField]
	private RectTransform buttonContainer;
}
