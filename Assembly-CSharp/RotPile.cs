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
		float mass = component.Mass;
		float temperature = component.Temperature;
		if (mass <= 0f)
		{
			Util.KDestroyGameObject(base.gameObject);
			return;
		}
		SimHashes simHashes = SimHashes.ToxicSand;
		GameObject gameObject = ElementLoader.FindElementByHash(simHashes).substance.SpawnResource(base.smi.master.transform.GetPosition(), mass, temperature, byte.MaxValue, 0, false, false, false);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ElementLoader.FindElementByHash(simHashes).name, gameObject.transform, 1.5f, false);
		Util.KDestroyGameObject(base.smi.gameObject);
	}

	private static string OnRottenTooltip(List<Notification> notifications, object data)
	{
		string text = "";
		foreach (Notification notification in notifications)
		{
			if (notification.tooltipData != null)
			{
				text = text + "\n• " + (string)notification.tooltipData + " ";
			}
		}
		return string.Format(MISC.NOTIFICATIONS.FOODROT.TOOLTIP, text);
	}

	public void TryClearNotification()
	{
		if (this.notification != null)
		{
			base.gameObject.AddOrGet<Notifier>().Remove(this.notification);
		}
	}

	public void TryCreateNotification()
	{
		WorldContainer myWorld = base.smi.master.GetMyWorld();
		if (myWorld != null && myWorld.worldInventory.IsReachable(base.smi.master.gameObject.GetComponent<Pickupable>()))
		{
			this.notification = new Notification(MISC.NOTIFICATIONS.FOODROT.NAME, NotificationType.BadMinor, new Func<List<Notification>, object, string>(RotPile.OnRottenTooltip), null, true, 0f, null, null, null, true, false);
			this.notification.tooltipData = base.smi.master.gameObject.GetProperName();
			base.gameObject.AddOrGet<Notifier>().Add(this.notification, "");
		}
	}

	private Notification notification;

	public class StatesInstance : GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile, object>.GameInstance
	{
		public StatesInstance(RotPile master)
			: base(master)
		{
		}

		public AttributeModifier baseDecomposeRate;
	}

	public class States : GameStateMachine<RotPile.States, RotPile.StatesInstance, RotPile>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.decomposing;
			base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
			this.decomposing.Enter(delegate(RotPile.StatesInstance smi)
			{
				smi.master.TryCreateNotification();
			}).Exit(delegate(RotPile.StatesInstance smi)
			{
				smi.master.TryClearNotification();
			}).ParamTransition<float>(this.decompositionAmount, this.convertDestroy, (RotPile.StatesInstance smi, float p) => p >= 600f)
				.Update("Decomposing", delegate(RotPile.StatesInstance smi, float dt)
				{
					this.decompositionAmount.Delta(dt, smi);
				}, UpdateRate.SIM_200ms, false);
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
