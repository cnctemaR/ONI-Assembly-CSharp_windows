using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UnityEngine;

public class DevToolStateMachineDebug : DevTool
{
	private void Update()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.selectedGameObject == null)
		{
			this.lockSelection = false;
		}
		SelectTool instance = SelectTool.Instance;
		GameObject gameObject;
		if (instance == null)
		{
			gameObject = null;
		}
		else
		{
			KSelectable selected = instance.selected;
			gameObject = ((selected != null) ? selected.gameObject : null);
		}
		GameObject gameObject2 = gameObject;
		if (!this.lockSelection && this.selectedGameObject != gameObject2 && gameObject2 != null && gameObject2.GetComponentsInChildren<StateMachineController>().Length != 0)
		{
			this.selectedGameObject = gameObject2;
			this.selectedStateMachine = 0;
		}
	}

	public void ShowEditor(StateMachineDebuggerSettings.Entry entry)
	{
		ImGui.Text(entry.typeName);
		ImGui.SameLine();
		ImGui.PushID(entry.typeName);
		ImGui.PushID(1);
		ImGui.Checkbox("", ref entry.enableConsoleLogging);
		ImGui.PopID();
		ImGui.SameLine();
		ImGui.PushID(2);
		ImGui.Checkbox("", ref entry.breakOnGoTo);
		ImGui.PopID();
		ImGui.SameLine();
		ImGui.PushID(3);
		ImGui.Checkbox("", ref entry.saveHistory);
		ImGui.PopID();
		ImGui.PopID();
	}

	protected override void RenderTo(DevPanel panel)
	{
		this.Update();
		ImGui.InputText("Filter:", ref this.stateMachineFilter, 256U);
		if (this.showSettings = ImGui.CollapsingHeader("Debug Settings:"))
		{
			if (ImGui.Button("Reset"))
			{
				StateMachineDebuggerSettings.Get().Clear();
			}
			ImGui.Text("EnableConsoleLogging / BreakOnGoTo / SaveHistory");
			int num = 0;
			foreach (StateMachineDebuggerSettings.Entry entry in StateMachineDebuggerSettings.Get())
			{
				if (string.IsNullOrEmpty(this.stateMachineFilter) || entry.typeName.ToLower().IndexOf(this.stateMachineFilter) >= 0)
				{
					this.ShowEditor(entry);
					num++;
				}
			}
		}
		if (Application.isPlaying && this.selectedGameObject != null)
		{
			StateMachineController[] componentsInChildren = this.selectedGameObject.GetComponentsInChildren<StateMachineController>();
			if (componentsInChildren.Length == 0)
			{
				return;
			}
			List<string> list = new List<string>();
			List<StateMachine.Instance> list2 = new List<StateMachine.Instance>();
			List<StateMachine.BaseDef> list3 = new List<StateMachine.BaseDef>();
			foreach (StateMachineController stateMachineController in componentsInChildren)
			{
				foreach (StateMachine.Instance instance in stateMachineController)
				{
					string text = stateMachineController.name + "." + instance.ToString();
					if (instance.isCrashed)
					{
						text = "(ERROR)" + text;
					}
					list.Add(text);
				}
			}
			List<string> list4;
			if (this.stateMachineFilter == null || this.stateMachineFilter == "")
			{
				list4 = list.Select<string, string>((string name) => name.ToLower()).ToList<string>();
			}
			else
			{
				list4 = (from name in list
					where name.ToLower().Contains(this.stateMachineFilter)
					select name.ToLower()).ToList<string>();
			}
			foreach (StateMachineController stateMachineController2 in componentsInChildren)
			{
				foreach (StateMachine.Instance instance2 in stateMachineController2)
				{
					string text2 = stateMachineController2.name + "." + instance2.ToString();
					if (instance2.isCrashed)
					{
						text2 = "(ERROR)" + text2;
					}
					if (list4.Contains(text2.ToLower()))
					{
						list2.Add(instance2);
					}
				}
				foreach (StateMachine.BaseDef baseDef in stateMachineController2.GetDefs<StateMachine.BaseDef>())
				{
					list3.Add(baseDef);
				}
			}
			if (list4.Count == 0)
			{
				string text3 = "Defs";
				string text4;
				if (list3.Count != 0)
				{
					text4 = string.Join(", ", list3.Select<StateMachine.BaseDef, string>((StateMachine.BaseDef d) => d.GetType().ToString()));
				}
				else
				{
					text4 = "(none)";
				}
				ImGui.LabelText(text3, text4);
				foreach (StateMachineController stateMachineController3 in componentsInChildren)
				{
					this.ShowControllerLog(stateMachineController3);
				}
				return;
			}
			this.selectedStateMachine = Math.Min(this.selectedStateMachine, list4.Count - 1);
			string text5 = "Defs";
			string text6;
			if (list3.Count != 0)
			{
				text6 = string.Join(", ", list3.Select<StateMachine.BaseDef, string>((StateMachine.BaseDef d) => d.GetType().ToString()));
			}
			else
			{
				text6 = "(none)";
			}
			ImGui.LabelText(text5, text6);
			ImGui.Checkbox("Lock selection", ref this.lockSelection);
			ImGui.Indent();
			ImGui.Combo("Select state machine", ref this.selectedStateMachine, list4.ToArray(), list4.Count);
			ImGui.Unindent();
			StateMachine.Instance instance3 = list2[this.selectedStateMachine];
			this.ShowStates(instance3);
			this.ShowTags(instance3);
			this.ShowDetails(instance3);
			this.ShowLog(instance3);
			this.ShowControllerLog(instance3);
			this.ShowHistory(instance3.GetMaster().GetComponent<StateMachineController>());
			this.ShowKAnimControllerLog();
		}
	}

	private void ShowStates(StateMachine.Instance state_machine_instance)
	{
		StateMachine stateMachine = state_machine_instance.GetStateMachine();
		ImGui.Text(stateMachine.ToString() + ": ");
		ImGui.Checkbox("Break On GoTo: ", ref state_machine_instance.breakOnGoTo);
		ImGui.Checkbox("Console Logging: ", ref state_machine_instance.enableConsoleLogging);
		string text = "None";
		StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
		if (currentState != null)
		{
			text = currentState.name;
		}
		string[] array = stateMachine.GetStateNames().Append("None");
		array[0] = array[0];
		int num = Array.IndexOf<string>(array, text);
		int num2 = num;
		for (int i = 0; i < array.Length; i++)
		{
			ImGui.RadioButton(array[i], ref num2, i);
		}
		if (num2 != num)
		{
			if (array[num2] == "None")
			{
				state_machine_instance.StopSM("StateMachineEditor.StopSM");
				return;
			}
			state_machine_instance.GoTo(array[num2]);
		}
	}

	public void ShowTags(StateMachine.Instance state_machine_instance)
	{
		ImGui.Text("Tags:");
		ImGui.Indent();
		KPrefabID component = state_machine_instance.GetComponent<KPrefabID>();
		if (component != null)
		{
			foreach (Tag tag in component.Tags)
			{
				ImGui.Text(tag.Name);
			}
		}
		ImGui.Unindent();
	}

	private void ShowDetails(StateMachine.Instance state_machine_instance)
	{
		state_machine_instance.GetStateMachine();
		string text = "None";
		StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
		if (currentState != null)
		{
			text = currentState.name;
		}
		ImGui.Text(text + ": ");
		ImGui.Indent();
		this.ShowParameters(state_machine_instance);
		this.ShowEvents(state_machine_instance);
		this.ShowTransitions(state_machine_instance);
		this.ShowEnterActions(state_machine_instance);
		this.ShowExitActions(state_machine_instance);
		ImGui.Unindent();
	}

	private void ShowParameters(StateMachine.Instance state_machine_instance)
	{
		ImGui.Text("Parameters:");
		ImGui.Indent();
		StateMachine.Parameter.Context[] parameterContexts = state_machine_instance.GetParameterContexts();
		for (int i = 0; i < parameterContexts.Length; i++)
		{
			parameterContexts[i].ShowDevTool(state_machine_instance);
		}
		ImGui.Unindent();
	}

	private void ShowEvents(StateMachine.Instance state_machine_instance)
	{
		StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
		ImGui.Text("Events: ");
		if (currentState == null)
		{
			return;
		}
		ImGui.Indent();
		for (int i = 0; i < currentState.GetStateCount(); i++)
		{
			StateMachine.BaseState state = currentState.GetState(i);
			if (state.events != null)
			{
				foreach (StateEvent stateEvent in state.events)
				{
					ImGui.Text(stateEvent.GetName());
				}
			}
		}
		ImGui.Unindent();
	}

	private void ShowTransitions(StateMachine.Instance state_machine_instance)
	{
		StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
		ImGui.Text("Transitions:");
		if (currentState == null)
		{
			return;
		}
		ImGui.Indent();
		for (int i = 0; i < currentState.GetStateCount(); i++)
		{
			StateMachine.BaseState state = currentState.GetState(i);
			if (state.transitions != null)
			{
				for (int j = 0; j < state.transitions.Count; j++)
				{
					ImGui.Text(state.transitions[j].ToString());
				}
			}
		}
		ImGui.Unindent();
	}

	private void ShowExitActions(StateMachine.Instance state_machine_instance)
	{
		StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
		ImGui.Text("Exit Actions: ");
		if (currentState == null)
		{
			return;
		}
		ImGui.Indent();
		for (int i = 0; i < currentState.GetStateCount(); i++)
		{
			StateMachine.BaseState state = currentState.GetState(i);
			if (state.exitActions != null)
			{
				foreach (StateMachine.Action action in state.exitActions)
				{
					ImGui.Text(action.name);
				}
			}
		}
		ImGui.Unindent();
	}

	private void ShowEnterActions(StateMachine.Instance state_machine_instance)
	{
		StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
		ImGui.Text("Enter Actions: ");
		if (currentState == null)
		{
			return;
		}
		ImGui.Indent();
		for (int i = 0; i < currentState.GetStateCount(); i++)
		{
			StateMachine.BaseState state = currentState.GetState(i);
			if (state.enterActions != null)
			{
				foreach (StateMachine.Action action in state.enterActions)
				{
					ImGui.Text(action.name);
				}
			}
		}
		ImGui.Unindent();
	}

	private void ShowLog(StateMachine.Instance state_machine_instance)
	{
		ImGui.Text("Machine Log:");
	}

	private void ShowKAnimControllerLog()
	{
		this.selectedGameObject.GetComponentInChildren<KAnimControllerBase>() == null;
	}

	private void ShowHistory(StateMachineController controller)
	{
		ImGui.Text("Logger disabled");
	}

	private void ShowControllerLog(StateMachineController controller)
	{
		ImGui.Text("Object Log:");
	}

	private void ShowControllerLog(StateMachine.Instance state_machine)
	{
		if (!state_machine.GetMaster().isNull)
		{
			this.ShowControllerLog(state_machine.GetMaster().GetComponent<StateMachineController>());
		}
	}

	private int selectedStateMachine;

	private int selectedLog;

	private GameObject selectedGameObject;

	private Vector2 scrollPos;

	private bool lockSelection;

	private bool showSettings;

	private string stateMachineFilter = "";
}
