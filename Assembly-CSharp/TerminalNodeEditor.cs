using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using UnityEngine;

[Node(false, "Noise/Terminus", new Type[] { typeof(NoiseNodeCanvas) })]
public class TerminalNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "terminalNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(TerminalNodeEditor);
		}
	}

	public override Node Create(Vector2 pos)
	{
		TerminalNodeEditor terminalNodeEditor = ScriptableObject.CreateInstance<TerminalNodeEditor>();
		terminalNodeEditor.rect = new Rect(pos.x, pos.y, 250f, 125f);
		terminalNodeEditor.name = "Terminus";
		terminalNodeEditor.CreateInput("Final Node", "IModule3D", NodeSide.Top, 10f);
		terminalNodeEditor.CreateOutput("Display", "IModule3D", NodeSide.Right, 30f);
		return terminalNodeEditor;
	}

	public override bool Calculate()
	{
		if (base.allInputsReady())
		{
			this.Outputs[0].SetValue<IModule3D>(this.Inputs[0].GetValue<IModule3D>());
			return true;
		}
		return false;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "terminalNodeEditor";
}
