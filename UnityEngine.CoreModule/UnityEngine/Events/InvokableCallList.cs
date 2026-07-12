using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Events
{
	internal class InvokableCallList
	{
		public int Count
		{
			get
			{
				return this.m_PersistentCalls.Count + this.m_RuntimeCalls.Count;
			}
		}

		public void AddPersistentInvokableCall(BaseInvokableCall call)
		{
			this.m_PersistentCalls.Add(call);
			this.m_NeedsUpdate = true;
		}

		public void AddListener(BaseInvokableCall call)
		{
			this.m_RuntimeCalls.Add(call);
			this.m_NeedsUpdate = true;
		}

		public void RemoveListener(object targetObj, MethodInfo method)
		{
			List<BaseInvokableCall> list = new List<BaseInvokableCall>();
			for (int i = 0; i < this.m_RuntimeCalls.Count; i++)
			{
				bool flag = this.m_RuntimeCalls[i].Find(targetObj, method);
				if (flag)
				{
					list.Add(this.m_RuntimeCalls[i]);
				}
			}
			this.m_RuntimeCalls.RemoveAll(new Predicate<BaseInvokableCall>(list.Contains));
			List<BaseInvokableCall> list2 = new List<BaseInvokableCall>(this.m_PersistentCalls.Count + this.m_RuntimeCalls.Count);
			list2.AddRange(this.m_PersistentCalls);
			list2.AddRange(this.m_RuntimeCalls);
			this.m_ExecutingCalls = list2;
			this.m_NeedsUpdate = false;
		}

		public void Clear()
		{
			this.m_RuntimeCalls.Clear();
			List<BaseInvokableCall> list = new List<BaseInvokableCall>(this.m_PersistentCalls);
			this.m_ExecutingCalls = list;
			this.m_NeedsUpdate = false;
		}

		public void ClearPersistent()
		{
			this.m_PersistentCalls.Clear();
			List<BaseInvokableCall> list = new List<BaseInvokableCall>(this.m_RuntimeCalls);
			this.m_ExecutingCalls = list;
			this.m_NeedsUpdate = false;
		}

		public List<BaseInvokableCall> PrepareInvoke()
		{
			bool needsUpdate = this.m_NeedsUpdate;
			if (needsUpdate)
			{
				this.m_ExecutingCalls.Clear();
				this.m_ExecutingCalls.AddRange(this.m_PersistentCalls);
				this.m_ExecutingCalls.AddRange(this.m_RuntimeCalls);
				this.m_NeedsUpdate = false;
			}
			return this.m_ExecutingCalls;
		}

		private readonly List<BaseInvokableCall> m_PersistentCalls = new List<BaseInvokableCall>();

		private readonly List<BaseInvokableCall> m_RuntimeCalls = new List<BaseInvokableCall>();

		private List<BaseInvokableCall> m_ExecutingCalls = new List<BaseInvokableCall>();

		private bool m_NeedsUpdate = true;
	}
}
