using System;
using System.Diagnostics;
using ImGuiNET;
using UnityEngine;

public class DevToolStatusItems : DevTool
{
	public DevToolStatusItems()
		: this(Option.None)
	{
	}

	public DevToolStatusItems(Option<DevToolEntityTarget.ForWorldGameObject> target)
	{
		this.targetOpt = target;
		this.tableDrawer = ImGuiObjectTableDrawer<StatusItemGroup.Entry>.New().RemoveFlags(ImGuiTableFlags.SizingFixedFit).AddFlags(ImGuiTableFlags.Resizable)
			.Column("Text", (StatusItemGroup.Entry entry) => entry.GetName())
			.Column("Id Name", (StatusItemGroup.Entry entry) => entry.item.Id)
			.Column("Notification Type", (StatusItemGroup.Entry entry) => entry.item.notificationType)
			.Column("Category", delegate(StatusItemGroup.Entry entry)
			{
				StatusItemCategory category = entry.category;
				return ((category != null) ? category.Name : null) ?? "<no category>";
			})
			.Column("OnAdded Callstack", delegate(StatusItemGroup.Entry entry)
			{
				StackTrace stackTrace;
				if (this.statusItemStackTraceWatcher.GetStackTraceForEntry(entry, out stackTrace))
				{
					if (ImGui.Selectable("copy callstack"))
					{
						ImGui.SetClipboardText(stackTrace.ToString());
					}
					ImGuiEx.TooltipForPrevious(stackTrace.ToString());
					return;
				}
				ImGui.Text("<None>");
			})
			.Build();
		base.OnUninit += delegate
		{
			this.statusItemStackTraceWatcher.Dispose();
		};
	}

	protected override void RenderTo(DevPanel panel)
	{
		this.statusItemStackTraceWatcher.SetTarget(this.targetOpt.AndThen<GameObject>((DevToolEntityTarget.ForWorldGameObject t) => t.gameObject).AndThen<KSelectable>((GameObject go) => go.GetComponent<KSelectable>()).AndThen<StatusItemGroup>((KSelectable s) => s.GetStatusItemGroup()));
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.MenuItem("Eyedrop New Target"))
			{
				panel.PushDevTool(new DevToolEntity_EyeDrop(delegate(DevToolEntityTarget target)
				{
					this.targetOpt = (DevToolEntityTarget.ForWorldGameObject)target;
				}, new Func<DevToolEntityTarget, Option<string>>(DevToolStatusItems.GetErrorForCandidateTarget)));
			}
			string text = null;
			if (this.targetOpt.IsNone())
			{
				text = "No target selected.";
			}
			else
			{
				Option<string> errorForCandidateTarget = DevToolStatusItems.GetErrorForCandidateTarget(this.targetOpt.Unwrap());
				if (errorForCandidateTarget.IsSome())
				{
					text = errorForCandidateTarget.Unwrap();
				}
			}
			if (ImGuiEx.MenuItem("Debug Target", text))
			{
				panel.PushValue<DevToolEntityTarget.ForWorldGameObject>(this.targetOpt.Unwrap());
			}
			ImGui.EndMenuBar();
		}
		this.Name = "Status Items";
		if (this.targetOpt.IsNone())
		{
			ImGui.TextWrapped("No Target selected");
			return;
		}
		DevToolEntityTarget.ForWorldGameObject forWorldGameObject = this.targetOpt.Unwrap();
		Option<string> errorForCandidateTarget2 = DevToolStatusItems.GetErrorForCandidateTarget(forWorldGameObject);
		if (errorForCandidateTarget2.IsSome())
		{
			ImGui.TextWrapped(errorForCandidateTarget2.Unwrap());
			return;
		}
		this.Name = "Status Items for: " + DevToolEntity.GetNameFor(forWorldGameObject.gameObject);
		bool shouldWatch = this.statusItemStackTraceWatcher.GetShouldWatch();
		if (ImGui.Checkbox("Should Track OnAdded Callstacks", ref shouldWatch))
		{
			this.statusItemStackTraceWatcher.SetShouldWatch(shouldWatch);
		}
		ImGui.Checkbox("Draw Bounding Box", ref this.shouldDrawBoundingBox);
		this.tableDrawer.Draw(forWorldGameObject.gameObject.GetComponent<KSelectable>().GetStatusItemGroup().GetEnumerator());
		if (this.shouldDrawBoundingBox)
		{
			Option<ValueTuple<Vector2, Vector2>> screenRect = forWorldGameObject.GetScreenRect();
			if (screenRect.IsSome())
			{
				DevToolEntity.DrawBoundingBox(screenRect.Unwrap(), forWorldGameObject.GetDebugName(), ImGui.IsWindowFocused());
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
		KSelectable component = forWorldGameObject.gameObject.GetComponent<KSelectable>();
		if (component.IsNullOrDestroyed())
		{
			return "Target GameObject doesn't have a KSelectable";
		}
		if (component.GetStatusItemGroup().IsNullOrDestroyed())
		{
			return "Target GameObject doesn't have a StatusItemGroup";
		}
		return Option.None;
	}

	private Option<DevToolEntityTarget.ForWorldGameObject> targetOpt;

	private ImGuiObjectTableDrawer<StatusItemGroup.Entry> tableDrawer;

	private StatusItemStackTraceWatcher statusItemStackTraceWatcher = new StatusItemStackTraceWatcher();

	private bool shouldDrawBoundingBox = true;
}
