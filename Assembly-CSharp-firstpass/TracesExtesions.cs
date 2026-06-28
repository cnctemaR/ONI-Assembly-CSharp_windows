using System;
using System.Diagnostics;
using UnityEngine;

public static class TracesExtesions
{
	public static void DeleteObject(this GameObject go)
	{
		Traces.Instance.TraceDestroy(go, new StackTrace(true));
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Trigger(1502190696, go);
		}
		global::UnityEngine.Object.Destroy(go);
	}

	public static void DeleteObject(this Component cmp)
	{
		cmp.gameObject.DeleteObject();
	}
}
