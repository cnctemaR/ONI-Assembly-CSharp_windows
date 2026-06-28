using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class TemplateSelectionInfoPanel : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < this.details.Length; i++)
		{
			Util.KInstantiateUI(this.prefab_detail_label, this.current_detail_container, true);
		}
		this.RefreshDetails(null);
		UIScheduler.Instance.SchedulePeriodic("RefreshTemplateSelectionInfo", 1f, new Action<object>(this.RefreshDetails), null, null);
		this.save_button.onClick += this.SaveCurrentDetails;
	}

	public void SaveCurrentDetails()
	{
		string text = string.Empty;
		for (int i = 0; i < this.details.Length; i++)
		{
			text = text + this.details[i](DebugBaseTemplateButton.Instance.SelectedCells) + "\n";
		}
		text += "\n-------------------\n";
		text += this.saved_detail_label.text;
		this.saved_detail_label.text = text;
	}

	public void RefreshDetails(object data = null)
	{
		for (int i = 0; i < this.details.Length; i++)
		{
			this.current_detail_container.transform.GetChild(i).GetComponent<LocText>().text = this.details[i](DebugBaseTemplateButton.Instance.SelectedCells);
		}
	}

	private static string TotalMass(List<int> cells)
	{
		float num = 0f;
		foreach (int num2 in cells)
		{
			num += Grid.Cell[num2].mass;
		}
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.TOTAL_MASS, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private static string AverageMass(List<int> cells)
	{
		float num = 0f;
		foreach (int num2 in cells)
		{
			num += Grid.Cell[num2].mass;
		}
		num /= (float)cells.Count;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.AVERAGE_MASS, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private static string AverageTemperature(List<int> cells)
	{
		float num = 0f;
		foreach (int num2 in cells)
		{
			num += Grid.Cell[num2].temperature;
		}
		num /= (float)cells.Count;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.AVERAGE_TEMPERATURE, GameUtil.GetFormattedTemperature(num, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
	}

	private static string TotalJoules(List<int> cells)
	{
		float num = 0f;
		foreach (int num2 in cells)
		{
			num += Grid.Element[num2].specificHeatCapacity * Grid.Cell[num2].temperature * (Grid.Cell[num2].mass * 1000f);
		}
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.TOTAL_JOULES, GameUtil.GetFormattedJoules(num, "F1"));
	}

	private static string JoulesPerKilogram(List<int> cells)
	{
		float num = 0f;
		float num2 = 0f;
		foreach (int num3 in cells)
		{
			num += Grid.Element[num3].specificHeatCapacity * Grid.Cell[num3].temperature * (Grid.Cell[num3].mass * 1000f);
			num2 += Grid.Cell[num3].mass;
		}
		num /= num2;
		return string.Format(UI.DEBUG_TOOLS.SAVE_BASE_TEMPLATE.SELECTION_INFO_PANEL.JOULES_PER_KILOGRAM, GameUtil.GetFormattedJoules(num, "F1"));
	}

	private static string MassPerElement(List<int> cells)
	{
		TemplateSelectionInfoPanel.mass_per_element.Clear();
		foreach (int num in cells)
		{
			bool flag = false;
			for (int i = 0; i < TemplateSelectionInfoPanel.mass_per_element.Count; i++)
			{
				if (TemplateSelectionInfoPanel.mass_per_element[i].first == Grid.Element[num])
				{
					TemplateSelectionInfoPanel.mass_per_element[i].second += Grid.Cell[num].mass;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				TemplateSelectionInfoPanel.mass_per_element.Add(new Tuple<Element, float>(Grid.Element[num], Grid.Cell[num].mass));
			}
		}
		TemplateSelectionInfoPanel.mass_per_element.Sort(delegate(Tuple<Element, float> a, Tuple<Element, float> b)
		{
			if (a.second > b.second)
			{
				return -1;
			}
			if (b.second > a.second)
			{
				return 1;
			}
			return 0;
		});
		string text = string.Empty;
		foreach (Tuple<Element, float> tuple in TemplateSelectionInfoPanel.mass_per_element)
		{
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				tuple.first.name,
				": ",
				GameUtil.GetFormattedMass(tuple.second, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"),
				"\n"
			});
		}
		return text;
	}

	[SerializeField]
	private GameObject prefab_detail_label;

	[SerializeField]
	private GameObject current_detail_container;

	[SerializeField]
	private LocText saved_detail_label;

	[SerializeField]
	private KButton save_button;

	private Func<List<int>, string>[] details = new Func<List<int>, string>[]
	{
		new Func<List<int>, string>(TemplateSelectionInfoPanel.TotalMass),
		new Func<List<int>, string>(TemplateSelectionInfoPanel.AverageMass),
		new Func<List<int>, string>(TemplateSelectionInfoPanel.AverageTemperature),
		new Func<List<int>, string>(TemplateSelectionInfoPanel.TotalJoules),
		new Func<List<int>, string>(TemplateSelectionInfoPanel.JoulesPerKilogram),
		new Func<List<int>, string>(TemplateSelectionInfoPanel.MassPerElement)
	};

	private static List<Tuple<Element, float>> mass_per_element = new List<Tuple<Element, float>>();
}
