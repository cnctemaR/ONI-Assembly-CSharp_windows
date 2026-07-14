using System;
using UnityEngine;

public class UnderwaterSoundEvent : SoundEvent
{
	public UnderwaterSoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, sound_name, frame, do_load, is_looping, min_interval, is_dynamic)
	{
		this.underwaterSound = StringFormatter.Combine(base.sound, "_uw");
	}

	public static bool IsVisiblyInLiquid(Vector3 position)
	{
		int num = Grid.PosToCell(new Vector2(position.x, position.y - 0.05f));
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (!Grid.IsLiquid(num))
		{
			return false;
		}
		int num2 = Grid.CellAbove(num);
		if (Grid.IsValidCell(num2) && Grid.IsLiquid(num2))
		{
			return true;
		}
		float num3 = Grid.Mass[num];
		float num4 = position.y - (float)((int)position.y);
		return num3 / 1000f >= num4;
	}

	private bool TryResolveMultitool(AnimEventManager.EventPlayerData behaviour, out string result)
	{
		KBatchedAnimEventToggler componentInParent = behaviour.controller.GetComponentInParent<KBatchedAnimEventToggler>();
		if (componentInParent != null && componentInParent.gameObject.name == "LaserEffect")
		{
			result = (UnderwaterSoundEvent.IsVisiblyInLiquid(behaviour.controller.transform.GetPosition()) ? this.underwaterSound : base.sound);
			return true;
		}
		result = null;
		return false;
	}

	private string Resolve(AnimEventManager.EventPlayerData behaviour)
	{
		string text;
		if (this.TryResolveMultitool(behaviour, out text))
		{
			return text;
		}
		if (Grid.IsNavigatableLiquid(Grid.PosToCell(behaviour.position)))
		{
			return this.underwaterSound;
		}
		Navigator navigator;
		if (!behaviour.controller.transform.root.TryGetComponent<Navigator>(out navigator))
		{
			return base.sound;
		}
		if (navigator.CurrentNavType != NavType.Swim)
		{
			return base.sound;
		}
		return this.underwaterSound;
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		base.PlaySound(behaviour, this.Resolve(behaviour));
	}

	public override void Stop(AnimEventManager.EventPlayerData behaviour)
	{
		if (!base.looping)
		{
			return;
		}
		LoopingSounds loopingSounds;
		if (!behaviour.controller.TryGetComponent<LoopingSounds>(out loopingSounds))
		{
			return;
		}
		loopingSounds.StopSound(base.sound);
		loopingSounds.StopSound(this.underwaterSound);
	}

	public const string POSTFIX = "_uw";

	private readonly string underwaterSound;
}
