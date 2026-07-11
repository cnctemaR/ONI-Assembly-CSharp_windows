using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class CreatureBait : StateMachineComponent<CreatureBait.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Tag[] constructionElements = base.GetComponent<Deconstructable>().constructionElements;
		this.baitElement = constructionElements[1];
		base.gameObject.GetSMI<Lure.Instance>().SetActiveLures(new Tag[] { this.baitElement });
		base.smi.StartSM();
	}

	[Serialize]
	public Tag baitElement;

	public class StatesInstance : GameStateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait, object>.GameInstance
	{
		public StatesInstance(CreatureBait master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Baited, null).Enter(delegate(CreatureBait.StatesInstance smi)
			{
				KAnim.Build build = ElementLoader.FindElementByName(smi.master.baitElement.ToString()).substance.anim.GetData().build;
				KAnim.Build.Symbol symbol = build.GetSymbol(new KAnimHashedString(build.name));
				HashedString hashedString = "snapTo_bait";
				smi.GetComponent<SymbolOverrideController>().AddSymbolOverride(hashedString, symbol, 0);
			}).TagTransition(GameTags.LureUsed, this.destroy, false);
			this.destroy.PlayAnim("use").EventHandler(GameHashes.AnimQueueComplete, delegate(CreatureBait.StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.master.gameObject);
			});
		}

		public GameStateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait, object>.State idle;

		public GameStateMachine<CreatureBait.States, CreatureBait.StatesInstance, CreatureBait, object>.State destroy;
	}
}
