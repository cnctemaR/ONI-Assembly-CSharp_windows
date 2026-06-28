using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class MinionVitalsPanel : KMonoBehaviour
{
	public void Init()
	{
		this.AddLine(Db.Get().Amounts.HitPoints, this.icon_hitpoints, null);
		this.AddLine(Db.Get().Amounts.Stress, this.icon_stress, null);
		this.AddLine(Db.Get().Amounts.Bladder, this.icon_bladder, null);
		this.AddLine(Db.Get().Amounts.Breath, this.icon_breath, null);
		this.AddLine(Db.Get().Amounts.Stamina, this.icon_stamina, null);
		this.AddLine(Db.Get().Amounts.Calories, this.icon_calories, null);
		this.AddLine(Db.Get().Amounts.Temperature, this.icon_temperature, null);
		this.AddLine(Db.Get().Amounts.ExternalTemperature, this.icon_temperature, null);
		this.AddLine(Db.Get().Amounts.Decor, this.icon_decor, (AmountInstance ainstance) => this.GetDecorTooltip(ainstance));
		this.AddLine(Db.Get().Amounts.Maturity, this.icon_maturity, null);
		this.AddLine(Db.Get().Amounts.Fertilization, this.icon_calories, null);
		this.AddLine(Db.Get().Amounts.Irrigation, this.icon_calories, null);
		this.AddLine(Db.Get().Amounts.YieldBonus, this.icon_hitpoints, null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.schedulerHandle = GameScheduler.Instance.SchedulePeriodic("Refresh Vitals Screen", 0.25f, new Action<object>(this.Refresh), null, null, 0f, null);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		this.schedulerHandle.Clear();
	}

	private string GetDecorTooltip(AmountInstance amount_instance)
	{
		string tooltip = amount_instance.amount.GetTooltip(amount_instance);
		AttributeInstance attributeInstance = Db.Get().Attributes.DecorExpectation.Lookup(amount_instance.gameObject);
		string text = tooltip;
		return string.Concat(new object[]
		{
			text,
			"\n\n",
			attributeInstance.Name,
			": ",
			attributeInstance.GetTotalValue()
		});
	}

	private void AddLine(Amount amount, Sprite icon, Func<AmountInstance, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(this.LineItemPrefab, base.gameObject, false);
		gameObject.GetComponentInChildren<Image>().sprite = icon;
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(true);
		MinionVitalsPanel.VitalLine vitalLine = default(MinionVitalsPanel.VitalLine);
		vitalLine.amount = amount;
		vitalLine.go = gameObject;
		vitalLine.locText = gameObject.GetComponentInChildren<LocText>();
		vitalLine.imageToggle = gameObject.GetComponentInChildren<ValueTrendImageToggle>();
		if (tooltip_func != null)
		{
			vitalLine.tooltip = tooltip_func;
		}
		else
		{
			vitalLine.tooltip = new Func<AmountInstance, string>(amount.GetTooltip);
		}
		this.vitalsLines.Add(vitalLine);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.schedulerHandle.Clear();
	}

	public void Refresh(object data)
	{
		if (this.selectedEntity == null)
		{
			return;
		}
		Amounts amounts = this.selectedEntity.GetAmounts();
		for (int i = 0; i < this.vitalsLines.Count; i++)
		{
			MinionVitalsPanel.VitalLine vitalLine = this.vitalsLines[i];
			bool flag = false;
			for (int j = 0; j < amounts.Count; j++)
			{
				AmountInstance amountInstance = amounts[j];
				if (vitalLine.amount == amountInstance.amount)
				{
					vitalLine.locText.SetText(vitalLine.amount.GetDescription(amountInstance));
					vitalLine.imageToggle.SetValue(amountInstance, vitalLine.tooltip);
					flag = true;
					if (!vitalLine.go.activeSelf)
					{
						vitalLine.go.SetActive(true);
					}
					break;
				}
			}
			if (!flag && vitalLine.go.activeSelf)
			{
				vitalLine.go.SetActive(false);
			}
		}
	}

	public Sprite icon_stress;

	public Sprite icon_breath;

	public Sprite icon_stamina;

	public Sprite icon_calories;

	public Sprite icon_temperature;

	public Sprite icon_bladder;

	public Sprite icon_decor;

	public Sprite icon_hitpoints;

	public Sprite icon_maturity;

	public GameObject LineItemPrefab;

	public GameObject selectedEntity;

	private SchedulerHandle schedulerHandle;

	public List<MinionVitalsPanel.VitalLine> vitalsLines = new List<MinionVitalsPanel.VitalLine>();

	public struct VitalLine
	{
		public Amount amount;

		public GameObject go;

		public ValueTrendImageToggle imageToggle;

		public LocText locText;

		public Func<AmountInstance, string> tooltip;
	}
}
