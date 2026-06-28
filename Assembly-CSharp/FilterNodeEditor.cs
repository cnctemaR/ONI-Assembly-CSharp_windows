using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using ProcGen.Noise;
using UnityEngine;

[Node(false, "Noise/Filter", new Type[] { typeof(NoiseNodeCanvas) })]
public class FilterNodeEditor : BaseNodeEditor
{
	public override string GetID
	{
		get
		{
			return "filterNodeEditor";
		}
	}

	public override Type GetObjectType
	{
		get
		{
			return typeof(FilterNodeEditor);
		}
	}

	public override NoiseBase GetTarget()
	{
		return this.target;
	}

	public override Node Create(Vector2 pos)
	{
		FilterNodeEditor filterNodeEditor = ScriptableObject.CreateInstance<FilterNodeEditor>();
		filterNodeEditor.rect = new Rect(pos.x, pos.y, 300f, 200f);
		filterNodeEditor.name = "Filter";
		filterNodeEditor.CreateInput("Source Node", "IModule3D", NodeSide.Left, 30f);
		filterNodeEditor.CreateOutput("Next Node", "IModule3D", NodeSide.Right, 30f);
		return filterNodeEditor;
	}

	public override bool Calculate()
	{
		bool flag;
		if (!base.allInputsReady())
		{
			flag = false;
		}
		else
		{
			IModule3D value = this.Inputs[0].GetValue<IModule3D>();
			if (value == null)
			{
				flag = false;
			}
			else
			{
				IModule3D module3D = this.target.CreateModule();
				if (module3D == null)
				{
					flag = false;
				}
				else
				{
					((FilterModule)module3D).Primitive3D = value;
					this.Outputs[0].SetValue<IModule3D>(module3D);
					flag = true;
				}
			}
		}
		return flag;
	}

	protected override void NodeGUI()
	{
		base.NodeGUI();
	}

	private const string Id = "filterNodeEditor";

	[SerializeField]
	public Filter target = new Filter();
}
