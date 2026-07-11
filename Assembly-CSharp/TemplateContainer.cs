using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using TemplateClasses;
using UnityEngine;

[Serializable]
public class TemplateContainer : YamlIO<TemplateContainer>
{
	public TemplateContainer()
	{
		this.Init();
	}

	public string name { get; private set; }

	public int priority { get; set; }

	public TemplateContainer.Info info { get; set; }

	public List<Cell> cells { get; set; }

	public List<Prefab> buildings { get; set; }

	public List<Prefab> pickupables { get; set; }

	public List<Prefab> elementalOres { get; set; }

	public List<Prefab> otherEntities { get; set; }

	public void Init(List<Cell> _cells, List<Prefab> _buildings, List<Prefab> _pickupables, List<Prefab> _elementalOres, List<Prefab> _otherEntities)
	{
		this.cells = _cells;
		this.buildings = _buildings;
		this.pickupables = _pickupables;
		this.elementalOres = _elementalOres;
		this.otherEntities = _otherEntities;
		this.info = new TemplateContainer.Info();
		this.RefreshInfo();
	}

	public void Init()
	{
		this.cells = new List<Cell>();
		this.buildings = new List<Prefab>();
		this.pickupables = new List<Prefab>();
		this.elementalOres = new List<Prefab>();
		this.otherEntities = new List<Prefab>();
		this.info = new TemplateContainer.Info();
	}

	public void Init(TemplateContainer template)
	{
		this.cells = new List<Cell>(template.cells);
		this.buildings = new List<Prefab>(template.buildings);
		this.pickupables = new List<Prefab>(template.pickupables);
		this.elementalOres = new List<Prefab>(template.elementalOres);
		this.otherEntities = new List<Prefab>(template.otherEntities);
		this.info = new TemplateContainer.Info();
		this.info.size = template.info.size;
		this.info.area = template.info.area;
		this.info.tags = (Tag[])template.info.tags.Clone();
	}

	public void RefreshInfo()
	{
		int num = 1;
		int num2 = -1;
		int num3 = 1;
		int num4 = -1;
		foreach (Cell cell in this.cells)
		{
			if (cell.location_x < num)
			{
				num = cell.location_x;
			}
			if (cell.location_x > num2)
			{
				num2 = cell.location_x;
			}
			if (cell.location_y < num3)
			{
				num3 = cell.location_y;
			}
			if (cell.location_y > num4)
			{
				num4 = cell.location_y;
			}
			this.info.size = new Vector2((float)(1 + (num2 - num)), (float)(1 + (num4 - num3)));
			this.info.area = this.cells.Count;
		}
	}

	public void SaveToYaml(string save_name)
	{
		string text = save_name;
		while (text.Contains("/"))
		{
			int num = text.IndexOf('/') + 1;
			if (text.Length > num)
			{
				text = text.Substring(num);
			}
		}
		this.name = text;
		string templatePath = TemplateCache.GetTemplatePath();
		if (!File.Exists(templatePath))
		{
			Directory.CreateDirectory(templatePath);
		}
		base.Save(templatePath + "/" + save_name + ".yaml", null);
	}

	[Serializable]
	public class Info : YamlIO<TemplateContainer.Info>
	{
		public Vector2f size { get; set; }

		public int area { get; set; }

		public Tag[] tags { get; set; }
	}
}
