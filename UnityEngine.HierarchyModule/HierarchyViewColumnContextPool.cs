using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal sealed class HierarchyViewColumnContextPool<TPooledObject> where TPooledObject : class
	{
		public HierarchyViewColumnContextPool(Func<TPooledObject> objectCreator)
		{
			this.m_ObjectCreator = objectCreator;
		}

		public TPooledObject Get(int contextId)
		{
			HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation poolForContext = this.GetPoolForContext(contextId);
			TPooledObject tpooledObject = poolForContext.Pool.Get();
			poolForContext.Active.Add(tpooledObject);
			return tpooledObject;
		}

		public void Release(int contextId, TPooledObject obj)
		{
			HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation poolForContext = this.GetPoolForContext(contextId);
			poolForContext.Pool.Release(obj);
			poolForContext.Active.Remove(obj);
		}

		public IReadOnlyCollection<TPooledObject> GetActiveObjects(int contextId)
		{
			HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation contextPoolImplementation;
			bool flag = this.m_Pools.TryGetValue(contextId, out contextPoolImplementation);
			IReadOnlyCollection<TPooledObject> readOnlyCollection;
			if (flag)
			{
				readOnlyCollection = contextPoolImplementation.Active;
			}
			else
			{
				readOnlyCollection = Array.Empty<TPooledObject>();
			}
			return readOnlyCollection;
		}

		public void Clear(int contextId)
		{
			HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation contextPoolImplementation;
			bool flag = this.m_Pools.TryGetValue(contextId, out contextPoolImplementation);
			if (flag)
			{
				contextPoolImplementation.Pool.Dispose();
				contextPoolImplementation.Active.Clear();
				this.m_Pools.Remove(contextId);
			}
		}

		internal bool Exists(int contextId)
		{
			return this.m_Pools.ContainsKey(contextId);
		}

		private HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation GetPoolForContext(int contextId)
		{
			HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation contextPoolImplementation;
			bool flag = !this.m_Pools.TryGetValue(contextId, out contextPoolImplementation);
			if (flag)
			{
				contextPoolImplementation = new HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation(this.m_ObjectCreator);
				this.m_Pools[contextId] = contextPoolImplementation;
			}
			return contextPoolImplementation;
		}

		private readonly Func<TPooledObject> m_ObjectCreator;

		private readonly Dictionary<int, HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation> m_Pools = new Dictionary<int, HierarchyViewColumnContextPool<TPooledObject>.ContextPoolImplementation>();

		private class ContextPoolImplementation
		{
			public ObjectPool<TPooledObject> Pool { get; private set; }

			public HashSet<TPooledObject> Active { get; private set; } = new HashSet<TPooledObject>();

			public ContextPoolImplementation(Func<TPooledObject> creator)
			{
				this.Pool = new ObjectPool<TPooledObject>(creator, null, null, null, true, 10, 10000);
			}
		}
	}
}
