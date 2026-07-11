using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/PixelPack")]
public class PixelPack : KMonoBehaviour, ISaveLoadable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<PixelPack>(-905833192, PixelPack.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		PixelPack component = ((GameObject)data).GetComponent<PixelPack>();
		if (component != null)
		{
			for (int i = 0; i < component.colorSettings.Count; i++)
			{
				this.colorSettings[i] = component.colorSettings[i];
			}
		}
		this.UpdateColors();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.animController = base.GetComponent<KBatchedAnimController>();
		base.Subscribe<PixelPack>(-801688580, PixelPack.OnLogicValueChangedDelegate);
		base.Subscribe<PixelPack>(-592767678, PixelPack.OnOperationalChangedDelegate);
		if (this.colorSettings == null)
		{
			PixelPack.ColorPair colorPair = new PixelPack.ColorPair
			{
				activeColor = this.defaultActive,
				standbyColor = this.defaultStandby
			};
			PixelPack.ColorPair colorPair2 = new PixelPack.ColorPair
			{
				activeColor = this.defaultActive,
				standbyColor = this.defaultStandby
			};
			PixelPack.ColorPair colorPair3 = new PixelPack.ColorPair
			{
				activeColor = this.defaultActive,
				standbyColor = this.defaultStandby
			};
			PixelPack.ColorPair colorPair4 = new PixelPack.ColorPair
			{
				activeColor = this.defaultActive,
				standbyColor = this.defaultStandby
			};
			this.colorSettings = new List<PixelPack.ColorPair>();
			this.colorSettings.Add(colorPair);
			this.colorSettings.Add(colorPair2);
			this.colorSettings.Add(colorPair3);
			this.colorSettings.Add(colorPair4);
		}
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PixelPack.PORT_ID)
		{
			this.logicValue = logicValueChanged.newValue;
			this.UpdateColors();
		}
	}

	private void OnOperationalChanged(object data)
	{
		if (this.operational.IsOperational)
		{
			this.UpdateColors();
			this.animController.Play(PixelPack.ON_ANIMS, KAnim.PlayMode.Once);
		}
		else
		{
			this.animController.Play(PixelPack.OFF_ANIMS, KAnim.PlayMode.Once);
		}
		this.operational.SetActive(this.operational.IsOperational, false);
	}

	public void UpdateColors()
	{
		if (this.operational.IsOperational)
		{
			LogicPorts component = base.GetComponent<LogicPorts>();
			if (component != null)
			{
				LogicWire.BitDepth connectedWireBitDepth = component.GetConnectedWireBitDepth(PixelPack.PORT_ID);
				if (connectedWireBitDepth == LogicWire.BitDepth.FourBit)
				{
					this.animController.SetSymbolTint(PixelPack.SYMBOL_ONE_NAME, LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[0].activeColor : this.colorSettings[0].standbyColor);
					this.animController.SetSymbolTint(PixelPack.SYMBOL_TWO_NAME, LogicCircuitNetwork.IsBitActive(1, this.logicValue) ? this.colorSettings[1].activeColor : this.colorSettings[1].standbyColor);
					this.animController.SetSymbolTint(PixelPack.SYMBOL_THREE_NAME, LogicCircuitNetwork.IsBitActive(2, this.logicValue) ? this.colorSettings[2].activeColor : this.colorSettings[2].standbyColor);
					this.animController.SetSymbolTint(PixelPack.SYMBOL_FOUR_NAME, LogicCircuitNetwork.IsBitActive(3, this.logicValue) ? this.colorSettings[3].activeColor : this.colorSettings[3].standbyColor);
					return;
				}
				if (connectedWireBitDepth == LogicWire.BitDepth.OneBit)
				{
					this.animController.SetSymbolTint(PixelPack.SYMBOL_ONE_NAME, LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[0].activeColor : this.colorSettings[0].standbyColor);
					this.animController.SetSymbolTint(PixelPack.SYMBOL_TWO_NAME, LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[1].activeColor : this.colorSettings[1].standbyColor);
					this.animController.SetSymbolTint(PixelPack.SYMBOL_THREE_NAME, LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[2].activeColor : this.colorSettings[2].standbyColor);
					this.animController.SetSymbolTint(PixelPack.SYMBOL_FOUR_NAME, LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[3].activeColor : this.colorSettings[3].standbyColor);
				}
			}
		}
	}

	protected KBatchedAnimController animController;

	private static readonly EventSystem.IntraObjectHandler<PixelPack> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<PixelPack>(delegate(PixelPack component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<PixelPack> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PixelPack>(delegate(PixelPack component, object data)
	{
		component.OnOperationalChanged(data);
	});

	public static readonly HashedString PORT_ID = new HashedString("PixelPackInput");

	public static readonly HashedString SYMBOL_ONE_NAME = "screen1";

	public static readonly HashedString SYMBOL_TWO_NAME = "screen2";

	public static readonly HashedString SYMBOL_THREE_NAME = "screen3";

	public static readonly HashedString SYMBOL_FOUR_NAME = "screen4";

	[MyCmpGet]
	private Operational operational;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<PixelPack> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<PixelPack>(delegate(PixelPack component, object data)
	{
		component.OnCopySettings(data);
	});

	public int logicValue;

	[Serialize]
	public List<PixelPack.ColorPair> colorSettings;

	private Color defaultActive = new Color(0.34509805f, 0.84705883f, 0.32941177f);

	private Color defaultStandby = new Color(0.972549f, 0.47058824f, 0.34509805f);

	protected static readonly HashedString[] ON_ANIMS = new HashedString[] { "on_pre", "on" };

	protected static readonly HashedString[] OFF_ANIMS = new HashedString[] { "off_pre", "off" };

	public struct ColorPair
	{
		public Color activeColor;

		public Color standbyColor;
	}
}
