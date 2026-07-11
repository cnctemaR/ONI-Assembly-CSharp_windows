using System;
using UnityEngine;

public class TechTreeTitle : Resource
{
	public Vector2 center
	{
		get
		{
			return this.node.center;
		}
	}

	public float width
	{
		get
		{
			return this.node.width;
		}
	}

	public float height
	{
		get
		{
			return this.node.height;
		}
	}

	public TechTreeTitle(string id, ResourceSet parent, string name, ResourceTreeNode node)
		: base(id, parent, name)
	{
		this.node = node;
	}

	public string desc;

	private ResourceTreeNode node;
}
