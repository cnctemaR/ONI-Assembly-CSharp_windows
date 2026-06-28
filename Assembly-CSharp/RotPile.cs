using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class RotPile : StateMachineComponent<RotPile.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	protected void ConvertToElement()
	{
		PrimaryElement component = base.smi.master.GetComponent<PrimaryElement>();
		SimHashes simHashes = SimHashes.ToxicSand;
		Substance substance = ElementLoader.FindElementByHash(simHashes).substance;
		GameObject gameObject = substance.SpawnResource(base.smi.master.transform.position, component.Mass, component.Temperature, byte.MaxValue, 0, false, false);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ElementLoader.FindElementByHash(simHashes).name, gameObject.transform, 1.5f, false);
		Util.KDestroyGameObject(base.smi.gameObject);
	}

	public class StatesInstance : GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.GameInstance
	{
		public StatesInstance(RotPile master)
			: base(master)
		{
			if (WorldInventory.Instance.IsReachable(base.smi.master.gameObject.GetComponent<Pickupable>()))
			{
				Notification notification = new Notification(MISC.NOTIFICATIONS.FOODROT.NAME, NotificationType.Bad, HashedString.Invalid, new Func<List<Notification>, object, string>(RotPile.StatesInstance.OnRottenTooltip), null, true, 0f, null, null, null);
				notification.tooltipData = master.gameObject.GetProperName();
				base.gameObject.AddOrGet<Notifier>().Add(notification, string.Empty);
			}
		}

		private static string OnRottenTooltip(List<Notification> notifications, object data)
		{
			string text = "\n";
			foreach (Notification notification in notifications)
			{
				if (notification.tooltipData != null)
				{
					text = text + "\n" + (string)notification.tooltipData;
				}
			}
			return string.Format(MISC.NOTIFICATIONS.FOODROT.TOOLTIP, text);
		}

		public AttributeModifier baseDecomposeRate;
	}

	public class States : GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.decomposing;
			base.serializable = true;
			this.decomposing.ParamTransition<float>(this.decompositionAmount, this.convertDestroy, (RotPile.StatesInstance smi, float p) => p >= 600f).Update(delegate(RotPile.StatesInstance smi)
			{
				this.decompositionAmount.Delta(smi.dt, smi);
			});
			this.convertDestroy.Enter(delegate(RotPile.StatesInstance smi)
			{
				smi.master.ConvertToElement();
			});
		}

		public GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.State decomposing;

		public GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.State convertDestroy;

		public StateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.FloatParameter decompositionAmount;
	}
}
