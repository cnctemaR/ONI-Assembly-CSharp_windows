using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using ProcGen.Noise;
using UnityEngine;

[Node(false, "Noise/Combine", new Type[] { typeof(NoiseNodeCanvas) })]
public class CombinerModuleNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "combinerModuleNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(CombinerModuleNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return this.target;
	}

	public override Node Create(Vector2 pos)
	{
		CombinerModuleNodeEditor combinerModuleNodeEditor = ScriptableObject.CreateInstance<CombinerModuleNodeEditor>();
		combinerModuleNodeEditor.rect = new Rect(pos.x, pos.y, 300f, 200f);
		combinerModuleNodeEditor.name = "Combine";
		combinerModuleNodeEditor.CreateInput("Source A", "IModule3D", NodeSide.Left, 10f);
		combinerModuleNodeEditor.CreateInput("Source B", "IModule3D", NodeSide.Left, 30f);
		combinerModuleNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return combinerModuleNodeEditor;
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
		IModule3D module3D = this.target.CreateModule(value, value2);
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

	private const string Id = "combinerModuleNodeEditor";

	[SerializeField]
	public Combiner target = new Combiner();
}
