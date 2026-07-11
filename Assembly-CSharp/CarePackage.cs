using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class CarePackage : StateMachineComponent<CarePackage.SMInstance>
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		if (this.info != null)
		{
			this.SetAnimToInfo();
		}
		this.reactable = this.CreateReactable();
	}

	public Reactable CreateReactable()
	{
		return new EmoteReactable(base.gameObject, "UpgradeFX", Db.Get().ChoreTypes.Emote, "anim_cheer_kanim", 15, 8, 0f, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "cheer_pre"
		}).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "cheer_loop"
		}).AddStep(new EmoteReactable.EmoteStep
		{
			anim = "cheer_pst"
		});
	}

	protected override void OnCleanUp()
	{
		this.reactable.Cleanup();
		base.OnCleanUp();
	}

	public void SetInfo(CarePackageInfo info)
	{
		this.info = info;
		this.SetAnimToInfo();
	}

	private void SetAnimToInfo()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab("Meter".ToTag()), base.gameObject, null);
		GameObject prefab = Assets.GetPrefab(this.info.id);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
		SymbolOverrideController component3 = prefab.GetComponent<SymbolOverrideController>();
		KBatchedAnimController component4 = gameObject.GetComponent<KBatchedAnimController>();
		component4.transform.SetLocalPosition(Vector3.forward);
		component4.AnimFiles = component2.AnimFiles;
		component4.isMovable = true;
		component4.animWidth = component2.animWidth;
		component4.animHeight = component2.animHeight;
		if (component3 != null)
		{
			SymbolOverrideController symbolOverrideController = SymbolOverrideControllerUtil.AddToPrefab(gameObject);
			foreach (SymbolOverrideController.SymbolEntry symbolEntry in component3.GetSymbolOverrides)
			{
				symbolOverrideController.AddSymbolOverride(symbolEntry.targetSymbol, symbolEntry.sourceSymbol, 0);
			}
		}
		component4.initialAnim = component2.initialAnim;
		component4.initialMode = KAnim.PlayMode.Loop;
		KBatchedAnimTracker component5 = gameObject.GetComponent<KBatchedAnimTracker>();
		component5.controller = component;
		component5.symbol = new HashedString("snapTO_object");
		component5.offset = new Vector3(0f, 0.5f, 0f);
		gameObject.SetActive(true);
		component.SetSymbolVisiblity("snapTO_object", false);
		new KAnimLink(component, component4);
	}

	private void SpawnContents()
	{
		if (this.info == null)
		{
			global::Debug.LogWarning("CarePackage has no data to spawn from. Probably a save from before the CarePackage info data was serialized.");
			return;
		}
		GameObject gameObject = null;
		GameObject prefab = Assets.GetPrefab(this.info.id);
		Element element = ElementLoader.GetElement(this.info.id.ToTag());
		Vector3 vector = base.transform.position + Vector3.up / 2f;
		if (element == null && prefab != null)
		{
			int num = 0;
			while ((float)num < this.info.quantity)
			{
				gameObject = Util.KInstantiate(prefab, vector);
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
				num++;
			}
		}
		else if (element != null)
		{
			float quantity = this.info.quantity;
			gameObject = element.substance.SpawnResource(vector, quantity, element.defaultValues.temperature, byte.MaxValue, 0, false, true, false);
		}
		else
		{
			global::Debug.LogWarning("Can't find spawnable thing from tag " + this.info.id);
		}
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
	}

	[Serialize]
	public CarePackageInfo info;

	private Reactable reactable;

	public class SMInstance : GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.GameInstance
	{
		public SMInstance(CarePackage master)
			: base(master)
		{
		}

		public List<Chore> activeUseChores;
	}

	public class States : GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.spawn;
			this.spawn.PlayAnim("portalbirth").OnAnimQueueComplete(this.open);
			this.open.PlayAnim("portalbirth_pst").QueueAnim("object_idle_loop", false, null).Exit(delegate(CarePackage.SMInstance smi)
			{
				smi.master.SpawnContents();
			})
				.ScheduleGoTo(1f, this.pst);
			this.pst.PlayAnim("object_idle_pst").ScheduleGoTo(5f, this.destroy);
			this.destroy.Enter(delegate(CarePackage.SMInstance smi)
			{
				Util.KDestroyGameObject(smi.master.gameObject);
			});
		}

		public GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State spawn;

		public GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State open;

		public GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State pst;

		public GameStateMachine<CarePackage.States, CarePackage.SMInstance, CarePackage, object>.State destroy;
	}
}
