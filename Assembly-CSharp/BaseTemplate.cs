using System;
using System.Collections.Generic;
using BaseTemplateClasses;
using UnityEngine;

[Serializable]
public class BaseTemplate : ScriptableObject
{
	public BaseTemplateConduitConnection[] getUtilityConnections
	{
		get
		{
			return this.utilityConnections;
		}
	}

	public void Init(List<BaseTemplateCellInfo> _cells, List<BaseTemplatePrefabInfo> _buildings, List<BaseTemplatePrefabInfo> _pickupables, List<BaseTemplatePrefabInfo> _elementalOres, BaseTemplateConduitConnection[] _utilityConnections = null)
	{
		this.cells = _cells;
		this.buildings = _buildings;
		this.pickupables = _pickupables;
		this.elementalOres = _elementalOres;
		this.utilityConnections = _utilityConnections;
	}

	public List<BaseTemplateCellInfo> cells;

	public List<BaseTemplatePrefabInfo> buildings;

	public List<BaseTemplatePrefabInfo> pickupables;

	public List<BaseTemplatePrefabInfo> elementalOres;

	[HideInInspector]
	public BaseTemplateConduitConnection[] utilityConnections;
}
