using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamInputInterpreter
{
	public int NumOfISteamInputs
	{
		get
		{
			return this.m_nInputs;
		}
	}

	public bool Initialized
	{
		get
		{
			return this.m_InputInitialized;
		}
	}

	public void OnEnable()
	{
		this.m_InputInitialized = DistributionPlatform.Initialized && SteamInput.Init(true);
		if (this.m_InputInitialized)
		{
			this.m_InputHandles = new InputHandle_t[16];
			this.Precache();
		}
	}

	private void OnDisable()
	{
		if (this.m_InputInitialized)
		{
			SteamInput.Shutdown();
		}
		this.m_InputInitialized = false;
	}

	public void Precache()
	{
		this.m_ActionSetNames = Enum.GetNames(typeof(SteamInputInterpreter.EActionSets));
		this.m_numActionSets = this.m_ActionSetNames.Length;
		this.m_ActionSetHandles = new InputActionSetHandle_t[this.m_numActionSets];
		for (int i = 0; i < this.m_numActionSets; i++)
		{
			this.m_ActionSetHandles[i] = SteamInput.GetActionSetHandle(this.m_ActionSetNames[i]);
		}
		this.m_MainGameActionSetAnalogActionNames = Enum.GetNames(typeof(SteamInputInterpreter.EAnalogActions_MainGameActionSet));
		this.m_numMainGameActionSetAnalogActions = this.m_MainGameActionSetAnalogActionNames.Length;
		this.m_MainGameActionSetAnalogActionHandles = new InputAnalogActionHandle_t[this.m_numMainGameActionSetAnalogActions];
		for (int j = 0; j < this.m_numMainGameActionSetAnalogActions; j++)
		{
			this.m_MainGameActionSetAnalogActionHandles[j] = SteamInput.GetAnalogActionHandle(this.m_MainGameActionSetAnalogActionNames[j]);
		}
		this.m_MainGameActionSetDigitalActionNames = Enum.GetNames(typeof(SteamInputInterpreter.EDigitalActions_MainGameActionSet));
		this.m_numMainGameActionSetDigitalActions = this.m_MainGameActionSetDigitalActionNames.Length;
		this.m_MainGameActionSetDigitalActionHandles = new InputDigitalActionHandle_t[this.m_numMainGameActionSetDigitalActions];
		for (int k = 0; k < this.m_numMainGameActionSetDigitalActions; k++)
		{
			this.m_MainGameActionSetDigitalActionHandles[k] = SteamInput.GetDigitalActionHandle(this.m_MainGameActionSetDigitalActionNames[k]);
		}
		if (this.kleiActionToSteamActionLookup.Count < 1)
		{
			this.kleiActionToSteamActionLookup.Add(global::Action.MouseLeft, SteamInputInterpreter.EDigitalActions_MainGameActionSet.affirmative_click);
			this.kleiActionToSteamActionLookup.Add(global::Action.MouseRight, SteamInputInterpreter.EDigitalActions_MainGameActionSet.negative_click);
			this.kleiActionToSteamActionLookup.Add(global::Action.CameraHome, SteamInputInterpreter.EDigitalActions_MainGameActionSet.camera_home);
			this.kleiActionToSteamActionLookup.Add(global::Action.ZoomIn, SteamInputInterpreter.EDigitalActions_MainGameActionSet.camera_zoom_in_scroll_down);
			this.kleiActionToSteamActionLookup.Add(global::Action.ZoomOut, SteamInputInterpreter.EDigitalActions_MainGameActionSet.camera_zoom_out_scroll_up);
			this.kleiActionToSteamActionLookup.Add(global::Action.TogglePause, SteamInputInterpreter.EDigitalActions_MainGameActionSet.sim_pause);
			this.kleiActionToSteamActionLookup.Add(global::Action.CycleSpeed, SteamInputInterpreter.EDigitalActions_MainGameActionSet.sim_cycle_speed);
			this.kleiActionToSteamActionLookup.Add(global::Action.RotateBuilding, SteamInputInterpreter.EDigitalActions_MainGameActionSet.rotate_building);
			this.kleiActionToSteamActionLookup.Add(global::Action.CopyBuilding, SteamInputInterpreter.EDigitalActions_MainGameActionSet.copy_building);
			this.kleiActionToSteamActionLookup.Add(global::Action.Dig, SteamInputInterpreter.EDigitalActions_MainGameActionSet.dig_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.BuildingCancel, SteamInputInterpreter.EDigitalActions_MainGameActionSet.cancel_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.BuildingDeconstruct, SteamInputInterpreter.EDigitalActions_MainGameActionSet.deconstruct_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.Prioritize, SteamInputInterpreter.EDigitalActions_MainGameActionSet.priority_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.Disinfect, SteamInputInterpreter.EDigitalActions_MainGameActionSet.disinfect_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.Clear, SteamInputInterpreter.EDigitalActions_MainGameActionSet.sweep_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.Mop, SteamInputInterpreter.EDigitalActions_MainGameActionSet.mop_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.Attack, SteamInputInterpreter.EDigitalActions_MainGameActionSet.attack_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.Capture, SteamInputInterpreter.EDigitalActions_MainGameActionSet.wrangle_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.Harvest, SteamInputInterpreter.EDigitalActions_MainGameActionSet.harvest_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.EmptyPipe, SteamInputInterpreter.EDigitalActions_MainGameActionSet.empty_tool);
			this.kleiActionToSteamActionLookup.Add(global::Action.Escape, SteamInputInterpreter.EDigitalActions_MainGameActionSet.pause_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManageVitals, SteamInputInterpreter.EDigitalActions_MainGameActionSet.vitals_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManageConsumables, SteamInputInterpreter.EDigitalActions_MainGameActionSet.consumables_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManageSchedule, SteamInputInterpreter.EDigitalActions_MainGameActionSet.schedule_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManagePriorities, SteamInputInterpreter.EDigitalActions_MainGameActionSet.priorities_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManageSkills, SteamInputInterpreter.EDigitalActions_MainGameActionSet.skills_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManageResearch, SteamInputInterpreter.EDigitalActions_MainGameActionSet.research_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManageStarmap, SteamInputInterpreter.EDigitalActions_MainGameActionSet.starmap_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManageReport, SteamInputInterpreter.EDigitalActions_MainGameActionSet.colony_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.ManageDatabase, SteamInputInterpreter.EDigitalActions_MainGameActionSet.codex_menu);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay1, SteamInputInterpreter.EDigitalActions_MainGameActionSet.oxygen_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay2, SteamInputInterpreter.EDigitalActions_MainGameActionSet.power_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay3, SteamInputInterpreter.EDigitalActions_MainGameActionSet.temperature_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay4, SteamInputInterpreter.EDigitalActions_MainGameActionSet.materials_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay5, SteamInputInterpreter.EDigitalActions_MainGameActionSet.light_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay6, SteamInputInterpreter.EDigitalActions_MainGameActionSet.plumbing_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay7, SteamInputInterpreter.EDigitalActions_MainGameActionSet.ventilation_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay8, SteamInputInterpreter.EDigitalActions_MainGameActionSet.decor_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay9, SteamInputInterpreter.EDigitalActions_MainGameActionSet.germs_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay10, SteamInputInterpreter.EDigitalActions_MainGameActionSet.farming_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay11, SteamInputInterpreter.EDigitalActions_MainGameActionSet.rooms_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay12, SteamInputInterpreter.EDigitalActions_MainGameActionSet.exosuits_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay13, SteamInputInterpreter.EDigitalActions_MainGameActionSet.automation_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay14, SteamInputInterpreter.EDigitalActions_MainGameActionSet.shipping_overlay);
			this.kleiActionToSteamActionLookup.Add(global::Action.Overlay15, SteamInputInterpreter.EDigitalActions_MainGameActionSet.radiation_overlay);
		}
		SteamInput.RunFrame(true);
		this.m_nInputs = SteamInput.GetConnectedControllers(this.m_InputHandles);
		for (int l = 0; l < this.m_InputHandles.Length; l++)
		{
			SteamInput.ActivateActionSet(this.m_InputHandles[l], this.m_ActionSetHandles[0]);
		}
	}

	public void Reset()
	{
		SteamInput.RunFrame(true);
		this.m_nInputs = SteamInput.GetConnectedControllers(this.m_InputHandles);
		for (int i = 0; i < this.m_InputHandles.Length; i++)
		{
			SteamInput.ActivateActionSet(this.m_InputHandles[i], this.m_ActionSetHandles[0]);
		}
	}

	public bool GetSteamInputActionIsDown(global::Action action)
	{
		bool flag = false;
		for (int i = 0; i < this.m_nInputs; i++)
		{
			SteamInputInterpreter.EDigitalActions_MainGameActionSet edigitalActions_MainGameActionSet;
			if (this.kleiActionToSteamActionLookup.TryGetValue(action, out edigitalActions_MainGameActionSet))
			{
				flag |= this.controllerDatas[i].digitalDatas[(int)edigitalActions_MainGameActionSet].down;
			}
		}
		return flag;
	}

	public Vector2 GetSteamCursorMovement()
	{
		Vector2 vector = Vector2.zero;
		for (int i = 0; i < this.m_nInputs; i++)
		{
			float x = this.controllerDatas[i].analogDatas[1].x;
			float y = this.controllerDatas[i].analogDatas[1].y;
			if (Mathf.Abs(x) > Mathf.Epsilon || Mathf.Abs(y) > Mathf.Epsilon)
			{
				vector += new Vector2(x, -y);
			}
		}
		return vector;
	}

	public Vector2 GetSteamCameraMovement()
	{
		Vector2 vector = Vector2.zero;
		for (int i = 0; i < this.m_nInputs; i++)
		{
			float x = this.controllerDatas[i].analogDatas[0].x;
			float y = this.controllerDatas[i].analogDatas[0].y;
			if (Mathf.Abs(x) > Mathf.Epsilon || Mathf.Abs(y) > Mathf.Epsilon)
			{
				vector += new Vector2(x, y);
			}
		}
		return vector;
	}

	public string GetActionGlyph(global::Action action)
	{
		int num = -1;
		string text = string.Empty;
		SteamInputInterpreter.EDigitalActions_MainGameActionSet edigitalActions_MainGameActionSet;
		if (this.kleiActionToSteamActionLookup.TryGetValue(action, out edigitalActions_MainGameActionSet))
		{
			InputDigitalActionHandle_t inputDigitalActionHandle_t = this.m_MainGameActionSetDigitalActionHandles[(int)edigitalActions_MainGameActionSet];
			EInputActionOrigin[] array = new EInputActionOrigin[8];
			SteamInput.GetDigitalActionOrigins(this.m_InputHandles[this.activeControllerIndex], this.m_ActionSetHandles[0], inputDigitalActionHandle_t, array);
			int num2 = 0;
			int num3 = (int)array[0];
			switch (SteamInput.GetInputTypeForHandle(this.m_InputHandles[this.activeControllerIndex]))
			{
			case ESteamInputType.k_ESteamInputType_Unknown:
				num2 = 0;
				break;
			case ESteamInputType.k_ESteamInputType_SteamController:
				text = "SteamControllerSheet";
				num2 = 1;
				break;
			case ESteamInputType.k_ESteamInputType_XBox360Controller:
				text = "XB360Sheet";
				num2 = 153;
				break;
			case ESteamInputType.k_ESteamInputType_XBoxOneController:
				text = "XboxOneSheet";
				num2 = 114;
				break;
			case ESteamInputType.k_ESteamInputType_GenericGamepad:
				num2 = 0;
				break;
			case ESteamInputType.k_ESteamInputType_PS4Controller:
				text = "PS4Sheet";
				num2 = 50;
				break;
			case ESteamInputType.k_ESteamInputType_SwitchJoyConPair:
				num2 = 192;
				break;
			case ESteamInputType.k_ESteamInputType_SwitchProController:
				num2 = 192;
				break;
			case ESteamInputType.k_ESteamInputType_PS5Controller:
				num2 = 258;
				break;
			case ESteamInputType.k_ESteamInputType_SteamDeckController:
				text = "SteamDeckSheet";
				num2 = 333;
				break;
			}
			num = num3 - num2;
		}
		if (num >= 0)
		{
			return string.Concat(new string[]
			{
				"<sprite=\"",
				text,
				"\" index=",
				num.ToString(),
				">"
			});
		}
		return "";
	}

	public void CheckForControllerChange()
	{
		SteamInput.RunFrame(true);
		int nInputs = this.m_nInputs;
		this.m_nInputs = SteamInput.GetConnectedControllers(this.m_InputHandles);
		for (int i = 0; i < this.m_nInputs; i++)
		{
			SteamInput.ActivateActionSet(this.m_InputHandles[i], this.m_ActionSetHandles[0]);
		}
		if (nInputs == 0 && this.m_nInputs != 0)
		{
			this.Precache();
		}
	}

	public void Update()
	{
		if (!this.m_InputInitialized)
		{
			return;
		}
		this.CheckForControllerChange();
		if (this.controllerDatas.Length != this.m_nInputs)
		{
			this.controllerDatas = new SteamInputInterpreter.ControllerData[this.m_nInputs];
			for (int i = 0; i < this.m_nInputs; i++)
			{
				this.controllerDatas[i].analogDatas = new SteamInputInterpreter.AnalogData[this.m_numMainGameActionSetAnalogActions];
				this.controllerDatas[i].digitalDatas = new SteamInputInterpreter.DigitalData[this.m_numMainGameActionSetDigitalActions];
			}
		}
		for (int j = 0; j < this.m_nInputs; j++)
		{
			for (int k = 0; k < this.m_numMainGameActionSetDigitalActions; k++)
			{
				bool flag = SteamInput.GetDigitalActionData(this.m_InputHandles[j], this.m_MainGameActionSetDigitalActionHandles[k]).bState > 0;
				this.controllerDatas[j].digitalDatas[k].down = flag;
				if (flag && (!KInputManager.currentControllerIsGamepad || this.activeControllerIndex != j))
				{
					this.activeControllerIndex = j;
					KInputManager.currentControllerIsGamepad = true;
					KInputManager.InputChange.Invoke();
				}
			}
			for (int l = 0; l < this.m_numMainGameActionSetAnalogActions; l++)
			{
				InputAnalogActionData_t analogActionData = SteamInput.GetAnalogActionData(this.m_InputHandles[j], this.m_MainGameActionSetAnalogActionHandles[l]);
				this.controllerDatas[j].analogDatas[l].x = analogActionData.x;
				this.controllerDatas[j].analogDatas[l].y = analogActionData.y;
				if ((Mathf.Abs(analogActionData.x) > Mathf.Epsilon || Mathf.Abs(analogActionData.y) > Mathf.Epsilon) && (!KInputManager.currentControllerIsGamepad || this.activeControllerIndex != j))
				{
					this.activeControllerIndex = j;
					KInputManager.currentControllerIsGamepad = true;
					KInputManager.InputChange.Invoke();
				}
			}
		}
	}

	private Vector2 m_ScrollPos;

	private bool m_InputInitialized;

	private int m_nInputs;

	private int activeControllerIndex;

	private Dictionary<global::Action, SteamInputInterpreter.EDigitalActions_MainGameActionSet> kleiActionToSteamActionLookup = new Dictionary<global::Action, SteamInputInterpreter.EDigitalActions_MainGameActionSet>();

	private int m_numActionSets;

	private int m_numMainGameActionSetAnalogActions;

	private int m_numMainGameActionSetDigitalActions;

	private string[] m_ActionSetNames;

	private string[] m_MainGameActionSetAnalogActionNames;

	private string[] m_MainGameActionSetDigitalActionNames;

	private InputActionSetHandle_t[] m_ActionSetHandles;

	private InputAnalogActionHandle_t[] m_MainGameActionSetAnalogActionHandles;

	private InputDigitalActionHandle_t[] m_MainGameActionSetDigitalActionHandles;

	private InputHandle_t[] m_InputHandles;

	private SteamInputInterpreter.ControllerData[] controllerDatas = new SteamInputInterpreter.ControllerData[0];

	private enum EActionSets
	{
		MainGameActionSet
	}

	private enum EAnalogActions_MainGameActionSet
	{
		Camera,
		Cursor
	}

	private enum EDigitalActions_MainGameActionSet
	{
		affirmative_click,
		negative_click,
		camera_home,
		camera_zoom_in_scroll_down,
		camera_zoom_out_scroll_up,
		sim_pause,
		sim_cycle_speed,
		rotate_building,
		copy_building,
		dig_tool,
		cancel_tool,
		deconstruct_tool,
		priority_tool,
		disinfect_tool,
		sweep_tool,
		mop_tool,
		attack_tool,
		wrangle_tool,
		harvest_tool,
		empty_tool,
		build_menu_up,
		build_menu_down,
		build_menu_left,
		build_menu_right,
		pause_menu,
		vitals_menu,
		consumables_menu,
		schedule_menu,
		priorities_menu,
		skills_menu,
		research_menu,
		starmap_menu,
		colony_menu,
		codex_menu,
		oxygen_overlay,
		power_overlay,
		temperature_overlay,
		materials_overlay,
		light_overlay,
		plumbing_overlay,
		ventilation_overlay,
		decor_overlay,
		germs_overlay,
		farming_overlay,
		rooms_overlay,
		exosuits_overlay,
		automation_overlay,
		shipping_overlay,
		radiation_overlay,
		toggle_resources,
		toggle_diagnostics
	}

	private struct AnalogData
	{
		public float x;

		public float y;
	}

	private struct DigitalData
	{
		public bool down;
	}

	private struct ControllerData
	{
		public SteamInputInterpreter.AnalogData[] analogDatas;

		public SteamInputInterpreter.DigitalData[] digitalDatas;
	}
}
