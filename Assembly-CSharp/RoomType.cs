using System;
using Klei.AI;
using STRINGS;

public class RoomType : Resource
{
	public RoomType(string id, string name, string tooltip, string effect, RoomTypeCategory category, RoomConstraints.Constraint primary_constraint, RoomConstraints.Constraint[] additional_constraints, RoomDetails.Detail[] display_details, int priority = 0, RoomType[] upgrade_paths = null, bool single_assignee = false, bool priority_building_use = false, string[] effects = null)
		: base(id, name)
	{
		this.tooltip = tooltip;
		this.effect = effect;
		this.category = category;
		this.primary_constraint = primary_constraint;
		this.additional_constraints = additional_constraints;
		this.display_details = display_details;
		this.priority = priority;
		this.upgrade_paths = upgrade_paths;
		this.single_assignee = single_assignee;
		this.priority_building_use = priority_building_use;
		this.effects = effects;
		if (this.upgrade_paths != null)
		{
			foreach (RoomType roomType in this.upgrade_paths)
			{
				Debug.Assert(roomType != null, name + " has a null upgrade path. Maybe it wasn't initialized yet.");
			}
		}
	}

	public string tooltip { get; private set; }

	public string effect { get; private set; }

	public RoomConstraints.Constraint primary_constraint { get; private set; }

	public RoomConstraints.Constraint[] additional_constraints { get; private set; }

	public int priority { get; private set; }

	public bool single_assignee { get; private set; }

	public RoomDetails.Detail[] display_details { get; private set; }

	public bool priority_building_use { get; private set; }

	public RoomTypeCategory category { get; private set; }

	public RoomType[] upgrade_paths { get; private set; }

	public string[] effects { get; private set; }

	public RoomType.RoomIdentificationResult isSatisfactory(Room candidate_room)
	{
		if (this.primary_constraint != null && !this.primary_constraint.isSatisfied(candidate_room))
		{
			return RoomType.RoomIdentificationResult.primary_unsatisfied;
		}
		if (this.additional_constraints != null)
		{
			foreach (RoomConstraints.Constraint constraint in this.additional_constraints)
			{
				if (!constraint.isSatisfied(candidate_room))
				{
					return RoomType.RoomIdentificationResult.primary_satisfied;
				}
			}
		}
		return RoomType.RoomIdentificationResult.all_satisfied;
	}

	public string GetCriteriaString()
	{
		string text = string.Concat(new string[]
		{
			"<b>",
			this.Name,
			"</b>\n",
			this.tooltip,
			UI.HORIZONTAL_BR_RULE,
			ROOMS.CRITERIA.HEADER
		});
		if (this == Db.Get().RoomTypes.Neutral)
		{
			text = text + "\n    • " + ROOMS.CRITERIA.NEUTRAL_TYPE;
		}
		text += ((this.primary_constraint != null) ? ("\n    • " + this.primary_constraint.name) : string.Empty);
		if (this.additional_constraints != null)
		{
			foreach (RoomConstraints.Constraint constraint in this.additional_constraints)
			{
				text = text + "\n    • " + constraint.name;
			}
		}
		return text;
	}

	public string GetRoomEffectsString()
	{
		if (this.effects != null && this.effects.Length > 0)
		{
			string text = ROOMS.EFFECTS.HEADER;
			foreach (string text2 in this.effects)
			{
				Effect effect = Db.Get().effects.Get(text2);
				text += Effect.CreateTooltip(effect, false, "\n    • ");
			}
			return text;
		}
		return null;
	}

	public void TriggerRoomEffects(KPrefabID triggerer, Effects target)
	{
		if (this.primary_constraint == null)
		{
			return;
		}
		if (triggerer == null)
		{
			return;
		}
		if (this.effects == null)
		{
			return;
		}
		if (this.primary_constraint.building_criteria(triggerer))
		{
			foreach (string text in this.effects)
			{
				target.Add(text, true);
			}
		}
	}

	public enum RoomIdentificationResult
	{
		all_satisfied,
		primary_satisfied,
		primary_unsatisfied
	}
}
