using System;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class GameFlowManager : StateMachineComponent<GameFlowManager.StatesInstance>, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		GameFlowManager.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public bool IsGameOver()
	{
		return base.smi.IsInsideState(base.smi.sm.gameover);
	}

	[MyCmpAdd]
	private Notifier notifier;

	public static GameFlowManager Instance;

	public class StatesInstance : GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager>.GameInstance
	{
		public StatesInstance(GameFlowManager smi)
			: base(smi)
		{
		}

		public bool IsIncapacitated(GameObject go)
		{
			return go.GetComponent<Effects>().GetMentalBreakEffect() != null;
		}

		public void CheckForGameOver()
		{
			if (!Game.Instance.GameStarted())
			{
				return;
			}
			bool flag = false;
			if (Components.LiveMinionIdentities.Count == 0)
			{
				flag = true;
			}
			else
			{
				flag = true;
				foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
				{
					if (!this.IsIncapacitated(minionIdentity.gameObject))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				this.GoTo(base.sm.gameover.pending);
			}
		}

		public Notification colonyLostNotification = new Notification(MISC.NOTIFICATIONS.COLONYLOST.NAME, NotificationType.Bad, null, null, null, false, 0f, null, null, null);
	}

	public class States : GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.loading;
			this.loading.ScheduleGoTo(4f, this.running);
			this.running.Update("CheckForGameOver", delegate(GameFlowManager.StatesInstance smi)
			{
				smi.CheckForGameOver();
			});
			this.gameover.TriggerOnEnter(GameHashes.GameOver, null).ToggleNotification((GameFlowManager.StatesInstance smi) => smi.colonyLostNotification);
			this.gameover.pending.Enter("Goto(gameover.active)", delegate(GameFlowManager.StatesInstance smi)
			{
				UIScheduler.Instance.Schedule("Goto(gameover.active)", 4f, delegate(object d)
				{
					smi.GoTo(this.gameover.active);
				}, null, null);
			});
			this.gameover.active.Enter("StartGameOverScreen", delegate(GameFlowManager.StatesInstance smi)
			{
				GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.GameOverScreen, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay).GetComponent<KScreen>().Show(true);
			});
		}

		public GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager>.State loading;

		public GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager>.State running;

		public GameFlowManager.States.GameOverState gameover;

		public class GameOverState : GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager>.State
		{
			public GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager>.State pending;

			public GameStateMachine<GameFlowManager.States, GameFlowManager.StatesInstance, GameFlowManager>.State active;
		}
	}
}
