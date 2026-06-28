using System;
using Klei.Noise;
using NodeEditorFramework;
using UnityEngine;

public class FloatListType : IConnectionTypeDeclaration
{
	public string Identifier
	{
		get
		{
			return "FloatList";
		}
	}

	public Type Type
	{
		get
		{
			return typeof(FloatList);
		}
	}

	public Color Color
	{
		get
		{
			return Color.blue;
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
