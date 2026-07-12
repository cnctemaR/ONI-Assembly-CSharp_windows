using System;
using System.Collections.Generic;
using Database;
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
		this.debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && this.target.part == MonumentPartResource.Part.Top);
		this.flipButton.onClick += delegate
		{
			this.target.GetComponent<Rotatable>().Rotate();
		};
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.target = target.GetComponent<MonumentPart>();
		this.debugVictoryButton.gameObject.SetActive(DebugHandler.InstantBuildMode && this.target.part == MonumentPartResource.Part.Top);
		this.GenerateStateButtons();
	}

	public void GenerateStateButtons()
	{
		for (int i = this.buttons.Count - 1; i >= 0; i--)
		{
			Util.KDestroyGameObject(this.buttons[i]);
		}
		this.buttons.Clear();
		using (List<MonumentPartResource>.Enumerator enumerator = Db.GetMonumentParts().GetParts(this.target.part).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MonumentPartResource state = enumerator.Current;
				GameObject gameObject = Util.KInstantiateUI(this.stateButtonPrefab, this.buttonContainer.gameObject, true);
				string state2 = state.State;
				string symbolName = state.SymbolName;
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					this.target.SetState(state.Id);
				};
				this.buttons.Add(gameObject);
				gameObject.GetComponent<KButton>().fgImage.sprite = Def.GetUISpriteFromMultiObjectAnim(state.AnimFile, state2, false, symbolName);
			}
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
