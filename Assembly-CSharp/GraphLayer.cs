using System;
using UnityEngine;

[RequireComponent(typeof(GraphBase))]
public class GraphLayer : KMonoBehaviour
{
	public GraphBase graph
	{
		get
		{
			if (this.graph_base == null)
			{
				this.graph_base = base.GetComponent<GraphBase>();
			}
			return this.graph_base;
		}
	}

	[MyCmpReq]
	protected GraphBase graph_base;
}
