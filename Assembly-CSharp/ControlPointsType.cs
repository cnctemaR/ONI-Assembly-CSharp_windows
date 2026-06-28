using System;
using NodeEditorFramework;
using ProcGen.Noise;
using UnityEngine;

public class ControlPointsType : IConnectionTypeDeclaration
{
	public string Identifier
	{
		get
		{
			return "ControlPoints";
		}
	}

	public Type Type
	{
		get
		{
			return typeof(ControlPointList);
		}
	}

	public Color Color
	{
		get
		{
			return Color.green;
		}
	}

	public string InKnobTex
	{
		get
		{
			return "Textures/In_Knob.png";
		}
	}

	public string OutKnobTex
	{
		get
		{
			return "Textures/Out_Knob.png";
		}
	}
}
