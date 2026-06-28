using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{text}")]
public struct Descriptor
{
	public Descriptor(string txt, string tooltip, Descriptor.DescriptorType descriptorType = Descriptor.DescriptorType.Effect, bool only_for_simple_info_screen = false)
	{
		this.indent = 0;
		this.text = txt;
		this.tooltipText = Util.StripTextFormatting(tooltip);
		this.type = descriptorType;
		this.onlyForSimpleInfoScreen = only_for_simple_info_screen;
	}

	public void SetupDescriptor(string txt, string tooltip, Descriptor.DescriptorType descriptorType = Descriptor.DescriptorType.Effect)
	{
		this.text = txt;
		this.tooltipText = Util.StripTextFormatting(tooltip);
		this.type = descriptorType;
	}

	public void IncreaseIndent()
	{
		this.indent++;
	}

	public void DecreaseIndent()
	{
		this.indent = Mathf.Max(this.indent - 1, 0);
	}

	public string IndentedText()
	{
		string text = this.text;
		for (int i = 0; i < this.indent; i++)
		{
			text = "    " + text;
		}
		return text;
	}

	public string text;

	public string tooltipText;

	public int indent;

	public Descriptor.DescriptorType type;

	public bool onlyForSimpleInfoScreen;

	public enum DescriptorType
	{
		Requirement,
		Effect,
		Lifecycle,
		Information,
		DiseaseSource,
		Detail,
		Symptom,
		SymptomAidable
	}
}
