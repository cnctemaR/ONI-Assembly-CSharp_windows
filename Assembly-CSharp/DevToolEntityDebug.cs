using System;
using System.Collections.Generic;
using Database;
using ImGuiNET;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class DevToolEntityDebug : DevTool
{
	public DevToolEntityDebug()
	{
		this.RequiresGameRunning = true;
	}

	protected override void RenderTo(DevPanel panel)
	{
		if (SelectTool.Instance == null)
		{
			ImGui.Text("SelectTool not available.");
			return;
		}
		ImGui.Checkbox("Lock Selection", ref this.lockSelection);
		GameObject gameObject = null;
		if (this.lockSelection && this.lockedObject != null)
		{
			gameObject = this.lockedObject;
		}
		else
		{
			KSelectable selected = SelectTool.Instance.selected;
			if (!selected.IsNullOrDestroyed())
			{
				gameObject = selected.gameObject;
				if (this.lockSelection)
				{
					this.lockedObject = gameObject;
				}
			}
		}
		if (gameObject == null)
		{
			ImGui.Text("Nothing selected.");
			return;
		}
		Modifiers component = gameObject.GetComponent<Modifiers>();
		if (component == null)
		{
			ImGui.Text("Selected object has no Modifiers component.");
			return;
		}
		this.Name = "Entity Debug: " + gameObject.name;
		if (GameClock.Instance != null)
		{
			float num = GameClock.Instance.GetTime() / 600f;
			ImGui.Text(string.Format("GameTime: {0:F2} cycles", num));
		}
		ImGui.Separator();
		this.DrawAmounts(component);
		this.DrawEffects(component);
		this.DrawAttributes(component);
		this.DrawAttributeLevels(component);
		this.DrawTraits(component);
		this.DrawDeaths(component);
		this.DrawUrges(component);
		this.DrawDiseases(component);
		this.DrawSicknesses(component);
		this.DrawResume(component);
		this.DrawStomach(component);
	}

	private void DrawAmounts(Modifiers entity)
	{
		if (entity.GetAmounts() == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Amounts (Min/Max/Delta)"))
		{
			List<AmountInstance> list = new List<AmountInstance>(entity.GetAmounts().ModifierList);
			list.Sort((AmountInstance x, AmountInstance y) => x.amount.Id.CompareTo(y.amount.Id));
			foreach (AmountInstance amountInstance in list)
			{
				string text = string.Format("{0} ({1}/{2}/{3:F2})", new object[]
				{
					amountInstance.amount.Id,
					amountInstance.GetMin(),
					amountInstance.GetMax(),
					amountInstance.GetDelta()
				});
				float value = amountInstance.value;
				if (ImGui.DragFloat(text, ref value, 0.1f, amountInstance.GetMin(), amountInstance.GetMax()))
				{
					amountInstance.amount.DebugSetValue(amountInstance, value);
				}
			}
		}
	}

	private void DrawEffects(Modifiers entity)
	{
		Effects component = entity.GetComponent<Effects>();
		if (component == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Effects"))
		{
			List<Effect> list = new List<Effect>(Db.Get().effects.resources);
			list.Sort((Effect x, Effect y) => x.Name.CompareTo(y.Name));
			foreach (Effect effect in list)
			{
				if (effect != null)
				{
					bool flag = component.HasEffect(effect);
					if (ImGui.Checkbox(effect.Name, ref flag))
					{
						if (flag)
						{
							component.Add(effect, false);
						}
						else
						{
							component.Remove(effect);
						}
					}
					if (flag)
					{
						ImGui.SameLine();
						EffectInstance effectInstance = component.Get(effect);
						float timeRemaining = effectInstance.timeRemaining;
						ImGui.SetNextItemWidth(100f);
						if (ImGui.DragFloat("##time_" + effect.Id, ref timeRemaining, 0.1f, 0f, 3.4028235E+38f, "%.1f"))
						{
							effectInstance.timeRemaining = timeRemaining;
						}
					}
				}
			}
		}
	}

	private void DrawAttributes(Modifiers entity)
	{
		if (entity.GetAttributes() == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Attributes"))
		{
			foreach (AttributeInstance attributeInstance in entity.GetAttributes())
			{
				float totalValue = attributeInstance.GetTotalValue();
				string text = string.Format("{0}: {1} ({2}/cycle)", attributeInstance.Attribute.Id, totalValue, totalValue * 600f);
				if (attributeInstance.Modifiers.Count > 0)
				{
					bool flag = false;
					this.expandedAttributes.TryGetValue(attributeInstance.Attribute.Id, out flag);
					if (ImGui.TreeNode(attributeInstance.Attribute.Id, text))
					{
						this.expandedAttributes[attributeInstance.Attribute.Id] = true;
						for (int i = 0; i < attributeInstance.Modifiers.Count; i++)
						{
							AttributeModifier attributeModifier = attributeInstance.Modifiers[i];
							string text2 = (attributeModifier.IsMultiplier ? " x " : "");
							ImGui.Text(string.Format("  {0}: {1}{2} ({3}/cycle)", new object[]
							{
								attributeModifier.GetDescription(),
								text2,
								attributeModifier.Value,
								attributeModifier.Value * 600f
							}));
						}
						ImGui.TreePop();
					}
					else
					{
						this.expandedAttributes[attributeInstance.Attribute.Id] = false;
					}
				}
				else
				{
					ImGui.Text(text);
				}
			}
		}
	}

	private void DrawAttributeLevels(Modifiers entity)
	{
		AttributeLevels component = entity.GetComponent<AttributeLevels>();
		if (component == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Attribute Levels"))
		{
			foreach (AttributeLevel attributeLevel in component)
			{
				string text = string.Format("{0} Lv{1}", attributeLevel.attribute.Attribute.Id, attributeLevel.GetLevel());
				string text2 = string.Format("{0:F0}/{1:F0} ({2:F3})", attributeLevel.experience, attributeLevel.GetExperienceForNextLevel(), attributeLevel.GetPercentComplete());
				ImGui.Text(text + ": " + text2);
				ImGui.SameLine();
				if (ImGui.SmallButton("+##" + attributeLevel.attribute.Attribute.Id))
				{
					attributeLevel.LevelUp(component);
				}
			}
		}
	}

	private void DrawTraits(Modifiers entity)
	{
		Traits component = entity.GetComponent<Traits>();
		if (component == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Traits"))
		{
			List<Trait> list = new List<Trait>(Db.Get().traits.resources);
			list.Sort((Trait a, Trait b) => UI.StripLinkFormatting(a.Name).CompareTo(UI.StripLinkFormatting(b.Name)));
			foreach (Trait trait in list)
			{
				bool flag = component.HasTrait(trait);
				if (ImGui.Checkbox(UI.StripLinkFormatting(trait.Name), ref flag))
				{
					if (flag)
					{
						component.Add(trait);
					}
					else
					{
						component.Remove(trait);
					}
				}
			}
		}
	}

	private void DrawDeaths(Modifiers entity)
	{
		DeathMonitor.Instance smi = entity.GetSMI<DeathMonitor.Instance>();
		if (smi == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Deaths"))
		{
			foreach (Death death in Db.Get().Deaths.resources)
			{
				if (ImGui.Button(death.Id))
				{
					smi.Kill(death);
				}
			}
		}
	}

	private void DrawUrges(Modifiers entity)
	{
		ChoreConsumer component = entity.GetComponent<ChoreConsumer>();
		if (component == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Urges"))
		{
			foreach (Urge urge in Db.Get().Urges.resources)
			{
				bool flag = component.HasUrge(urge);
				if (ImGui.Checkbox(urge.Name, ref flag))
				{
					if (flag)
					{
						component.AddUrge(urge);
					}
					else
					{
						component.RemoveUrge(urge);
					}
				}
			}
		}
	}

	private void DrawDiseases(Modifiers entity)
	{
		if (ImGui.CollapsingHeader("Diseases"))
		{
			Diseases diseases = Db.Get().Diseases;
			PrimaryElement component = entity.gameObject.GetComponent<PrimaryElement>();
			for (int i = 0; i < diseases.Count; i++)
			{
				Disease disease = diseases[i];
				int num = (((int)component.DiseaseIdx == i) ? component.DiseaseCount : 0);
				string text = Util.StripTextFormatting(disease.Name);
				ImGui.Text(string.Format("{0}: {1}", text, num));
				ImGui.SameLine();
				if (ImGui.SmallButton("Add 100##" + disease.Id))
				{
					component.AddDisease((byte)i, 100, "debug");
				}
			}
		}
	}

	private void DrawSicknesses(Modifiers entity)
	{
		MinionModifiers component = entity.GetComponent<MinionModifiers>();
		if (component == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Sicknesses"))
		{
			global::Database.Sicknesses sicknesses = Db.Get().Sicknesses;
			Klei.AI.Sicknesses sicknesses2 = component.sicknesses;
			for (int i = 0; i < sicknesses.Count; i++)
			{
				Sickness sickness = sicknesses[i];
				string text = Util.StripTextFormatting(sickness.Name);
				SicknessInstance sicknessInstance = sicknesses2.Get(sickness);
				bool flag = sicknessInstance != null;
				if (ImGui.Checkbox(text + "##sick", ref flag))
				{
					if (flag)
					{
						sicknesses2.Infect(new SicknessExposureInfo(sickness.Id, "debug menu"));
					}
					else
					{
						sicknessInstance.Cure();
					}
				}
				if (flag && sicknessInstance != null)
				{
					ImGui.SameLine();
					float percentCured = sicknessInstance.GetPercentCured();
					ImGui.SetNextItemWidth(100f);
					if (ImGui.DragFloat("##cure_" + sickness.Id, ref percentCured, 0.01f, 0f, 1f, "%.2f"))
					{
						sicknessInstance.SetPercentCured(percentCured);
					}
				}
			}
		}
	}

	private void DrawResume(Modifiers entity)
	{
		MinionResume component = entity.GetComponent<MinionResume>();
		if (component == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Resume"))
		{
			float totalExperienceGained = component.TotalExperienceGained;
			if (ImGui.DragFloat("Total Experience", ref totalExperienceGained, 1f))
			{
				component.AddExperience(totalExperienceGained - component.TotalExperienceGained);
			}
			ImGui.Text(string.Format("Next Level: {0}", MinionResume.CalculateNextExperienceBar(component.TotalSkillPointsGained)));
			ImGui.Text(string.Format("Total Skill Points: {0}", component.TotalSkillPointsGained));
			ImGui.SameLine();
			if (ImGui.SmallButton("+##skillpoint"))
			{
				component.ForceAddSkillPoint();
			}
			ImGui.Separator();
			foreach (Skill skill in new List<Skill>(Db.Get().Skills.resources))
			{
				bool flag = component.MasteryBySkillID.ContainsKey(skill.Id) && component.MasteryBySkillID[skill.Id];
				if (ImGui.Checkbox(UI.StripLinkFormatting(skill.Name) + "##skill_" + skill.Id, ref flag))
				{
					if (flag)
					{
						component.MasterSkill(skill.Id);
					}
					else
					{
						component.UnmasterSkill(skill.Id);
					}
				}
			}
		}
	}

	private void DrawStomach(Modifiers entity)
	{
		CreatureCalorieMonitor.Instance smi = entity.GetSMI<CreatureCalorieMonitor.Instance>();
		if (smi == null)
		{
			return;
		}
		if (ImGui.CollapsingHeader("Stomach"))
		{
			CreatureCalorieMonitor.Stomach stomach = smi.stomach;
			ImGui.Text(string.Format("Fullness: {0}", stomach.GetFullness()));
			ImGui.Text(string.Format("Hunger {0}: {1}", smi.calories.GetMax() * smi.HungryRatio, (smi.calories.GetMax() - smi.calories.value) / (smi.calories.GetMax() * (1f - smi.HungryRatio))));
			List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry> calorieEntries = stomach.GetCalorieEntries();
			for (int i = 0; i < calorieEntries.Count; i++)
			{
				CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry = calorieEntries[i];
				float calories = caloriesConsumedEntry.calories;
				if (ImGui.DragFloat(caloriesConsumedEntry.tag.Name, ref calories, 0.1f))
				{
					caloriesConsumedEntry.calories = calories;
					calorieEntries[i] = caloriesConsumedEntry;
				}
			}
		}
	}

	private bool lockSelection;

	private GameObject lockedObject;

	private Dictionary<string, bool> expandedAttributes = new Dictionary<string, bool>();
}
