using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using NodeEditorFramework;
using UnityEngine;

public class IModule3DNodeType : IConnectionTypeDeclaration
{
	public string Identifier
	{
		get
		{
			return "IModule3D";
		}
	}

	public Type Type
	{
		get
		{
			return typeof(IModule3D);
		}
	}

	public Color Color
	{
		get
		{
			return Color.magenta;
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
