using System;
using NodeEditorFramework;
using ProcGen.Noise;
using UnityEngine;

[Node(false, "Noise/Curve Control", new Type[] { typeof(NoiseNodeCanvas) })]
public class ControlPointsNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "controlPointsNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(ControlPointsNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return this.target;
	}

	public override Node Create(Vector2 pos)
	{
		ControlPointsNodeEditor controlPointsNodeEditor = ScriptableObject.CreateInstance<ControlPointsNodeEditor>();
		controlPointsNodeEditor.rect = new Rect(pos.x, pos.y, 300f, ControlPointsNodeEditor.height);
		controlPointsNodeEditor.name = "Curve Control";
		controlPointsNodeEditor.CreateOutput("Curve", "ControlPoints", NodeSide.Right, 30f);
		return controlPointsNodeEditor;
	}

	public override bool Calculate()
	{
		this.Outputs[0].SetValue<ControlPointList>(this.target);
		return true;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "controlPointsNodeEditor";

	[SerializeField]
	public ControlPointList target = new ControlPointList();

	private static float height = 100f;
}
