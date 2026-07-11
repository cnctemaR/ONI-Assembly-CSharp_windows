using System;
using System.Collections.Generic;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;
using NodeEditorFramework;
using ProcGen.Noise;
using UnityEngine;

[Node(false, "Noise/Modify", new Type[] { typeof(NoiseNodeCanvas) })]
public class ModifierModuleNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "modifierModuleNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(ModifierModuleNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return this.target;
	}

	public override Node Create(Vector2 pos)
	{
		ModifierModuleNodeEditor modifierModuleNodeEditor = ScriptableObject.CreateInstance<ModifierModuleNodeEditor>();
		modifierModuleNodeEditor.rect = new Rect(pos.x, pos.y, 300f, 100f);
		modifierModuleNodeEditor.name = "Modify";
		modifierModuleNodeEditor.CreateInput("Source", "IModule3D", NodeSide.Left, 10f);
		modifierModuleNodeEditor.CreateInput("Curve Control", "ControlPoints", NodeSide.Left, 50f);
		modifierModuleNodeEditor.CreateInput("Terrace Control", "FloatList", NodeSide.Left, 60f);
		modifierModuleNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return modifierModuleNodeEditor;
	}

	public override bool Calculate()
	{
		IModule3D value = this.Inputs[0].GetValue<IModule3D>();
		if (value == null)
		{
			return false;
		}
		ControlPointList value2 = this.Inputs[1].GetValue<ControlPointList>();
		if (this.target.modifyType == ProcGen.Noise.Modifier.ModifyType.Curve && (value2 == null || value2.points.Count == 0))
		{
			return false;
		}
		FloatList value3 = this.Inputs[2].GetValue<FloatList>();
		if (this.target.modifyType == ProcGen.Noise.Modifier.ModifyType.Terrace && (value3 == null || value3.points.Count == 0))
		{
			return false;
		}
		IModule3D module3D = this.target.CreateModule(value);
		if (module3D == null)
		{
			return false;
		}
		if (this.target.modifyType == ProcGen.Noise.Modifier.ModifyType.Curve)
		{
			Curve curve = module3D as Curve;
			curve.ClearControlPoints();
			using (List<ControlPoint>.Enumerator enumerator = value2.GetControls().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ControlPoint controlPoint = enumerator.Current;
					curve.AddControlPoint(controlPoint);
				}
				goto IL_013B;
			}
		}
		if (this.target.modifyType == ProcGen.Noise.Modifier.ModifyType.Terrace)
		{
			Terrace terrace = module3D as Terrace;
			terrace.ClearControlPoints();
			foreach (float num in value3.points)
			{
				terrace.AddControlPoint(num);
			}
		}
		IL_013B:
		this.Outputs[0].SetValue<IModule3D>(module3D);
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "modifierModuleNodeEditor";

	[SerializeField]
	public ProcGen.Noise.Modifier target = new ProcGen.Noise.Modifier();
}
