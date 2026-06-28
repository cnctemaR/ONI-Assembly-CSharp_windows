using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using ProcGen.Noise;
using UnityEngine;

[Node(false, "Noise/Select", new Type[] { typeof(NoiseNodeCanvas) })]
public class SelectorModuleNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "selectorModuleNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(SelectorModuleNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return this.target;
	}

	public override Node Create(Vector2 pos)
	{
		SelectorModuleNodeEditor selectorModuleNodeEditor = ScriptableObject.CreateInstance<SelectorModuleNodeEditor>();
		selectorModuleNodeEditor.rect = new Rect(pos.x, pos.y, 300f, 100f);
		selectorModuleNodeEditor.name = "Selector";
		selectorModuleNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		selectorModuleNodeEditor.CreateInput("Selector", "IModule3D", NodeSide.Top, 10f);
		selectorModuleNodeEditor.CreateInput("Left", "IModule3D", NodeSide.Left, 10f);
		selectorModuleNodeEditor.CreateInput("Right", "IModule3D", NodeSide.Left, 30f);
		return selectorModuleNodeEditor;
	}

	public override bool Calculate()
	{
		if (!base.allInputsReady())
		{
			return false;
		}
		IModule3D value = this.Inputs[0].GetValue<IModule3D>();
		if (value == null)
		{
			return false;
		}
		IModule3D value2 = this.Inputs[1].GetValue<IModule3D>();
		if (value2 == null)
		{
			return false;
		}
		IModule3D value3 = this.Inputs[2].GetValue<IModule3D>();
		if (value3 == null)
		{
			return false;
		}
		IModule3D module3D = this.target.CreateModule(value, value3, value2);
		if (module3D == null)
		{
			return false;
		}
		this.Outputs[0].SetValue<IModule3D>(module3D);
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "selectorModuleNodeEditor";

	[SerializeField]
	public Selector target = new Selector();
}
