using System;
using System.Collections.Generic;

public class SceneCuller : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		SceneCuller.Instance = this;
	}

	public void AddObject(VisibilityMonitor.Instance obj)
	{
		this.objects.Add(obj);
	}

	public void RemoveObject(VisibilityMonitor.Instance obj)
	{
		this.objects.Remove(obj);
	}

	public void AddCullable(VisibilityMonitor.Instance obj)
	{
		this.cullables.Add(obj);
	}

	public void RemoveCullable(VisibilityMonitor.Instance obj)
	{
		this.cullables.Remove(obj);
	}

	public void AddHidden(VisibilityMonitor.Instance obj)
	{
		this.hidden.Add(obj);
	}

	public void RemoveHidden(VisibilityMonitor.Instance obj)
	{
		this.hidden.Remove(obj);
	}

	public static SceneCuller Instance;

	private SceneCuller.ObjectList objects = new SceneCuller.ObjectList("Objects");

	private SceneCuller.ObjectList cullables = new SceneCuller.ObjectList("Cullables");

	private SceneCuller.ObjectList hidden = new SceneCuller.ObjectList("Hidden");

	private class ObjectList
	{
		public ObjectList(string name)
		{
			this.name = name;
		}

		public void Add(VisibilityMonitor.Instance obj)
		{
			if (!this.objects.Contains(obj))
			{
				this.objects.Add(obj);
			}
		}

		public void Remove(VisibilityMonitor.Instance obj)
		{
			this.objects.Remove(obj);
		}

		public List<VisibilityMonitor.Instance> objects = new List<VisibilityMonitor.Instance>();

		private string name;
	}
}
