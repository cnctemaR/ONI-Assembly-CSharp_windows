using System;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicHammer : Switch
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.animController = base.GetComponent<KBatchedAnimController>();
		this.switchedOn = false;
		this.UpdateVisualState(false, false);
		this.rotatable = base.GetComponent<Rotatable>();
		CellOffset rotatedCellOffset = this.rotatable.GetRotatedCellOffset(this.target_offset);
		int num = Grid.PosToCell(base.transform.GetPosition());
		this.resonator_cell = Grid.OffsetCell(num, rotatedCellOffset);
		base.Subscribe<LogicHammer>(-801688580, LogicHammer.OnLogicValueChangedDelegate);
		base.Subscribe<LogicHammer>(-592767678, LogicHammer.OnOperationalChangedDelegate);
		base.OnToggle += this.OnSwitchToggled;
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		bool flag = false;
		if (this.operational.IsOperational && toggled_on)
		{
			flag = this.TriggerAudio();
			this.operational.SetActive(true, false);
		}
		else
		{
			this.operational.SetActive(false, false);
		}
		this.UpdateVisualState(flag, false);
	}

	private void OnOperationalChanged(object data)
	{
		if (this.operational.IsOperational)
		{
			this.SetState(LogicCircuitNetwork.IsBitActive(0, this.logic_value));
			return;
		}
		this.UpdateVisualState(false, false);
	}

	private bool TriggerAudio()
	{
		if (this.wasOn || !this.switchedOn)
		{
			return false;
		}
		string text = null;
		if (!Grid.IsValidCell(this.resonator_cell))
		{
			return false;
		}
		float num = float.NaN;
		GameObject gameObject = Grid.Objects[this.resonator_cell, 1];
		if (gameObject == null)
		{
			gameObject = Grid.Objects[this.resonator_cell, 30];
			if (gameObject == null)
			{
				gameObject = Grid.Objects[this.resonator_cell, 26];
				if (gameObject != null)
				{
					Wire component = gameObject.GetComponent<Wire>();
					if (component != null)
					{
						ElectricalUtilityNetwork electricalUtilityNetwork = (ElectricalUtilityNetwork)Game.Instance.electricalConduitSystem.GetNetworkForCell(component.GetNetworkCell());
						if (electricalUtilityNetwork != null)
						{
							num = (float)electricalUtilityNetwork.allWires.Count;
						}
					}
				}
				else
				{
					gameObject = Grid.Objects[this.resonator_cell, 31];
					if (gameObject != null)
					{
						if (gameObject.GetComponent<LogicWire>() != null)
						{
							LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(this.resonator_cell);
							if (networkForCell != null)
							{
								num = (float)networkForCell.WireCount;
							}
						}
					}
					else
					{
						gameObject = Grid.Objects[this.resonator_cell, 12];
						if (gameObject != null)
						{
							Conduit component2 = gameObject.GetComponent<Conduit>();
							FlowUtilityNetwork flowUtilityNetwork = (FlowUtilityNetwork)Conduit.GetNetworkManager(ConduitType.Gas).GetNetworkForCell(component2.GetNetworkCell());
							if (flowUtilityNetwork != null)
							{
								num = (float)flowUtilityNetwork.conduitCount;
							}
						}
						else
						{
							gameObject = Grid.Objects[this.resonator_cell, 16];
							if (gameObject != null)
							{
								Conduit component3 = gameObject.GetComponent<Conduit>();
								FlowUtilityNetwork flowUtilityNetwork2 = (FlowUtilityNetwork)Conduit.GetNetworkManager(ConduitType.Liquid).GetNetworkForCell(component3.GetNetworkCell());
								if (flowUtilityNetwork2 != null)
								{
									num = (float)flowUtilityNetwork2.conduitCount;
								}
							}
							else
							{
								gameObject = Grid.Objects[this.resonator_cell, 20];
								gameObject != null;
							}
						}
					}
				}
			}
		}
		if (gameObject != null)
		{
			Building component4 = gameObject.GetComponent<BuildingComplete>();
			if (component4 != null)
			{
				text = component4.Def.PrefabID;
			}
		}
		if (text != null)
		{
			string text2 = StringFormatter.Combine(LogicHammer.SOUND_EVENT_PREFIX, text);
			text2 = GlobalAssets.GetSound(text2, true);
			if (text2 == null)
			{
				text2 = GlobalAssets.GetSound(LogicHammer.DEFAULT_NO_SOUND_EVENT, false);
			}
			Vector3 position = base.transform.position;
			position.z = 0f;
			EventInstance eventInstance = KFMOD.BeginOneShot(text2, position, 1f);
			if (!float.IsNaN(num))
			{
				eventInstance.setParameterValue(LogicHammer.PARAMETER_NAME, num);
			}
			KFMOD.EndOneShot(eventInstance);
			return true;
		}
		return false;
	}

	private void UpdateVisualState(bool connected, bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			if (this.switchedOn)
			{
				if (connected)
				{
					this.animController.Play(LogicHammer.ON_HIT_ANIMS, KAnim.PlayMode.Once);
					return;
				}
				this.animController.Play(LogicHammer.ON_MISS_ANIMS, KAnim.PlayMode.Once);
				return;
			}
			else
			{
				this.animController.Play(LogicHammer.OFF_ANIMS, KAnim.PlayMode.Once);
			}
		}
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == LogicHammer.PORT_ID)
		{
			this.SetState(LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue));
			this.logic_value = logicValueChanged.newValue;
		}
	}

	protected KBatchedAnimController animController;

	private static readonly EventSystem.IntraObjectHandler<LogicHammer> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicHammer>(delegate(LogicHammer component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicHammer> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<LogicHammer>(delegate(LogicHammer component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public static readonly HashedString PORT_ID = new HashedString("LogicHammerInput");

	private static string PARAMETER_NAME = "hammerObjectCount";

	private static string SOUND_EVENT_PREFIX = "Hammer_strike_";

	private static string DEFAULT_NO_SOUND_EVENT = "Hammer_strike_default";

	[MyCmpGet]
	private Operational operational;

	private int resonator_cell;

	private CellOffset target_offset = new CellOffset(-1, 0);

	private Rotatable rotatable;

	private int logic_value;

	private bool wasOn;

	protected static readonly HashedString[] ON_HIT_ANIMS = new HashedString[] { "on_hit" };

	protected static readonly HashedString[] ON_MISS_ANIMS = new HashedString[] { "on_miss" };

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[] { "off_pre", "off" };
}
