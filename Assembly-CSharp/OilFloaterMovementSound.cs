using System;
using System.Runtime.CompilerServices;

internal class OilFloaterMovementSound : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.sound = GlobalAssets.GetSound(this.sound, false);
		base.Subscribe<OilFloaterMovementSound>(1027377649, OilFloaterMovementSound.OnObjectMovementStateChangedDelegate);
		this.cellChangedHandlerID = Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OilFloaterMovementSound.UpdateSoundDispatcher, this, "OilFloaterMovementSound");
	}

	private void OnObjectMovementStateChanged(object data)
	{
		GameHashes gameHashes = Boxed<GameHashes>.Unbox(data);
		this.isMoving = gameHashes == GameHashes.ObjectMovementWakeUp;
		this.UpdateSound();
	}

	private void UpdateSound()
	{
		bool flag = this.isMoving && base.GetComponent<Navigator>().CurrentNavType != NavType.Swim;
		if (flag == this.isPlayingSound)
		{
			return;
		}
		LoopingSounds component = base.GetComponent<LoopingSounds>();
		if (flag)
		{
			component.StartSound(this.sound);
		}
		else
		{
			component.StopSound(this.sound);
		}
		this.isPlayingSound = flag;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(ref this.cellChangedHandlerID);
	}

	public string sound;

	public bool isPlayingSound;

	public bool isMoving;

	private ulong cellChangedHandlerID;

	private static readonly EventSystem.IntraObjectHandler<OilFloaterMovementSound> OnObjectMovementStateChangedDelegate = new EventSystem.IntraObjectHandler<OilFloaterMovementSound>(delegate(OilFloaterMovementSound component, object data)
	{
		component.OnObjectMovementStateChanged(data);
	});

	private static readonly Action<object> UpdateSoundDispatcher = delegate(object obj)
	{
		Unsafe.As<OilFloaterMovementSound>(obj).UpdateSound();
	};
}
