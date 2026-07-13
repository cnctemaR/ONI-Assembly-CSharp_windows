using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class LargeImpactorKeepsake : GameStateMachine<LargeImpactorKeepsake, LargeImpactorKeepsake.Instance, IStateMachineTarget, LargeImpactorKeepsake.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.notification;
		this.notification.ParamTransition<bool>(this.HasNotificationBeenAknowledged, this.idle, GameStateMachine<LargeImpactorKeepsake, LargeImpactorKeepsake.Instance, IStateMachineTarget, LargeImpactorKeepsake.Def>.IsTrue).ToggleNotification(new Func<LargeImpactorKeepsake.Instance, Notification>(LargeImpactorKeepsake.GetNotification));
		this.idle.DoNothing();
	}

	public static Notification GetNotification(LargeImpactorKeepsake.Instance smi)
	{
		return smi.notification;
	}

	private GameStateMachine<LargeImpactorKeepsake, LargeImpactorKeepsake.Instance, IStateMachineTarget, LargeImpactorKeepsake.Def>.State notification;

	private GameStateMachine<LargeImpactorKeepsake, LargeImpactorKeepsake.Instance, IStateMachineTarget, LargeImpactorKeepsake.Def>.State idle;

	private StateMachine<LargeImpactorKeepsake, LargeImpactorKeepsake.Instance, IStateMachineTarget, LargeImpactorKeepsake.Def>.BoolParameter HasNotificationBeenAknowledged;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<LargeImpactorKeepsake, LargeImpactorKeepsake.Instance, IStateMachineTarget, LargeImpactorKeepsake.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, LargeImpactorKeepsake.Def def)
			: base(master, def)
		{
			this.notification = this.CreateDeathNotification();
		}

		private Notification CreateDeathNotification()
		{
			string text = MISC.NOTIFICATIONS.LARGE_IMPACTOR_KEEPSAKE.NAME;
			NotificationType notificationType = NotificationType.Event;
			Func<List<Notification>, object, string> func = (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.LARGE_IMPACTOR_KEEPSAKE.TOOLTIP;
			object obj = null;
			bool flag = false;
			float num = 0f;
			Transform transform = base.gameObject.transform;
			return new Notification(text, notificationType, func, obj, flag, num, new Notification.ClickCallback(this.MarkAsAknowledgedAndFocusCamera), this, transform, true, true, false);
		}

		private void MarkAsAknowledgedAndFocusCamera(object data)
		{
			if (data == null)
			{
				return;
			}
			LargeImpactorKeepsake.Instance instance = (LargeImpactorKeepsake.Instance)data;
			instance.sm.HasNotificationBeenAknowledged.Set(true, instance, false);
			GameUtil.FocusCamera(base.gameObject.transform, true, true);
		}

		public Notification notification;
	}
}
