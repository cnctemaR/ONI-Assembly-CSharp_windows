using System;
using UnityEngine;

public class GarbageCollectionProfiler : MonoBehaviour
{
	private void Update()
	{
		if (this._Items == null || this._Items.Length != this._ObjectCount)
		{
			this._Items = new GarbageCollectionProfiler.Test[this._ObjectCount];
			for (int i = 0; i < this._ObjectCount; i++)
			{
				this._Items[i] = new GarbageCollectionProfiler.DelegateWithSingleHandler();
			}
		}
		GC.Collect();
	}

	public int _ObjectCount = 100000;

	private GarbageCollectionProfiler.Test[] _Items;

	private class Test
	{
	}

	private class StringTest : GarbageCollectionProfiler.Test
	{
		private string _String;
	}

	private class ObjectTest : GarbageCollectionProfiler.Test
	{
		private object _Object;
	}

	private class DelegateTest : GarbageCollectionProfiler.Test
	{
		private global::System.Action _Delegate;
	}

	private class DelegateWithSingleHandler : GarbageCollectionProfiler.Test
	{
		public DelegateWithSingleHandler()
		{
			this._Delegate = (global::System.Action)Delegate.Combine(this._Delegate, new global::System.Action(this.DoNothing));
		}

		private void DoNothing()
		{
		}

		private global::System.Action _Delegate;
	}
}
