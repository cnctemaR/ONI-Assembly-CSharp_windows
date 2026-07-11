using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Traces : KMonoBehaviour
{
	public static Traces Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		Traces.Instance = this;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Traces.Instance = null;
	}

	public void TraceDestroy(GameObject go, StackTrace stack_trace)
	{
		if (this.DestroyTraces.Count > 99)
		{
			this.DestroyTraces.RemoveAt(0);
		}
		Traces.Entry entry = new Traces.Entry
		{
			Name = string.Concat(new object[]
			{
				Time.frameCount,
				" ",
				go.name,
				" [",
				go.GetInstanceID(),
				"]"
			}),
			StackTrace = stack_trace
		};
		this.DestroyTraces.Add(entry);
	}

	public List<Traces.Entry> DestroyTraces = new List<Traces.Entry>();

	[Serializable]
	public class Entry
	{
		public string Name;

		public StackTrace StackTrace;

		public bool Foldout;
	}
}
