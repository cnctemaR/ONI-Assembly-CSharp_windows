using System;
using System.Runtime.CompilerServices;
using ImGuiNET;
using UnityEngine;

public class DevToolEntity_DebugGoTo : DevTool
{
	public DevToolEntity_DebugGoTo()
		: this(Option.None)
	{
	}

	public DevToolEntity_DebugGoTo(Option<DevToolEntityTarget.ForWorldGameObject> target)
	{
		this.targetOpt = target;
	}

	protected override void RenderTo(DevPanel panel)
	{
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.MenuItem("Eyedrop New Target"))
			{
				panel.PushDevTool(new DevToolEntity_EyeDrop(delegate(DevToolEntityTarget target)
				{
					this.targetOpt = (DevToolEntityTarget.ForWorldGameObject)target;
				}, new Func<DevToolEntityTarget, Option<string>>(DevToolEntity_DebugGoTo.GetErrorForCandidateTarget)));
			}
			ImGui.EndMenuBar();
		}
		this.Name = "Debug Go To";
		if (this.targetOpt.IsNone())
		{
			ImGui.TextWrapped("No Target selected");
			return;
		}
		DevToolEntityTarget.ForWorldGameObject forWorldGameObject = this.targetOpt.Unwrap();
		Option<string> errorForCandidateTarget = DevToolEntity_DebugGoTo.GetErrorForCandidateTarget(forWorldGameObject);
		if (errorForCandidateTarget.IsSome())
		{
			ImGui.TextWrapped(errorForCandidateTarget.Unwrap());
			return;
		}
		this.Name = "Debug Go To for: " + DevToolEntity.GetNameFor(forWorldGameObject.gameObject);
		if (forWorldGameObject.gameObject.IsNullOrDestroyed())
		{
			ImGui.TextWrapped("Target GameObject is null");
			return;
		}
		ImGui.Checkbox("Draw Bounding Box", ref this.shouldDrawBoundingBox);
		ImGuiEx.SimpleField("Target GameObject", DevToolEntity.GetNameFor(forWorldGameObject.gameObject));
		ImGuiEx.SimpleField("Destination Cell Index", DevToolEntity_DebugGoTo.<RenderTo>g__GetCellName|6_1(this.destinationSimCellTarget));
		if (ImGui.Button("Select New Destination Cell"))
		{
			panel.PushDevTool(new DevToolEntity_EyeDrop(delegate(DevToolEntityTarget target)
			{
				this.destinationSimCellTarget = (DevToolEntityTarget.ForSimCell)target;
			}, delegate(DevToolEntityTarget uncastTarget)
			{
				if (!(uncastTarget is DevToolEntityTarget.ForSimCell))
				{
					return "Target is not a sim cell";
				}
				return Option.None;
			}));
		}
		ImGui.Separator();
		ImGui.Checkbox("Should Continously Request", ref this.shouldContinouslyRequest);
		string text = (this.shouldContinouslyRequest ? "Disable continous requests" : (this.destinationSimCellTarget.IsNone() ? "No destination target." : null));
		if (ImGuiEx.Button("Request Target go to Destination", text) || (this.shouldContinouslyRequest && this.destinationSimCellTarget.IsSome()))
		{
			DebugGoToMonitor.Instance smi = forWorldGameObject.gameObject.GetSMI<DebugGoToMonitor.Instance>();
			CreatureDebugGoToMonitor.Instance smi2 = forWorldGameObject.gameObject.GetSMI<CreatureDebugGoToMonitor.Instance>();
			if (!smi.IsNullOrDestroyed())
			{
				smi.GoToCell(this.destinationSimCellTarget.Unwrap().cellIndex);
			}
			else if (!smi2.IsNullOrDestroyed())
			{
				smi2.GoToCell(this.destinationSimCellTarget.Unwrap().cellIndex);
			}
			else
			{
				DebugUtil.DevLogError("No debug goto SMI found");
			}
		}
		if (this.shouldDrawBoundingBox)
		{
			Option<ValueTuple<Vector2, Vector2>> screenRect = forWorldGameObject.GetScreenRect();
			if (screenRect.IsSome())
			{
				DevToolEntity.DrawBoundingBox(screenRect.Unwrap(), "[Target]", ImGui.IsWindowFocused());
			}
			if (this.destinationSimCellTarget.IsSome())
			{
				Option<ValueTuple<Vector2, Vector2>> screenRect2 = this.destinationSimCellTarget.Unwrap().GetScreenRect();
				if (screenRect2.IsSome())
				{
					DevToolEntity.DrawBoundingBox(screenRect2.Unwrap(), "[Destination]", ImGui.IsWindowFocused());
				}
			}
		}
	}

	public static Option<string> GetErrorForCandidateTarget(DevToolEntityTarget uncastTarget)
	{
		if (!(uncastTarget is DevToolEntityTarget.ForWorldGameObject))
		{
			return "Target must be a world GameObject";
		}
		DevToolEntityTarget.ForWorldGameObject forWorldGameObject = (DevToolEntityTarget.ForWorldGameObject)uncastTarget;
		if (forWorldGameObject.gameObject.IsNullOrDestroyed())
		{
			return "Target GameObject is null or destroyed";
		}
		if (forWorldGameObject.gameObject.GetSMI<DebugGoToMonitor.Instance>().IsNullOrDestroyed() && forWorldGameObject.gameObject.GetSMI<CreatureDebugGoToMonitor.Instance>().IsNullOrDestroyed())
		{
			return "Target GameObject doesn't have either a DebugGoToMonitor or CreatureDebugGoToMonitor";
		}
		return Option.None;
	}

	[CompilerGenerated]
	internal static string <RenderTo>g__GetCellName|6_1(Option<DevToolEntityTarget.ForSimCell> target)
	{
		if (!target.IsNone())
		{
			return target.Unwrap().cellIndex.ToString();
		}
		return "<None>";
	}

	private Option<DevToolEntityTarget.ForWorldGameObject> targetOpt;

	private Option<DevToolEntityTarget.ForSimCell> destinationSimCellTarget;

	private bool shouldDrawBoundingBox = true;

	private bool shouldContinouslyRequest;
}
