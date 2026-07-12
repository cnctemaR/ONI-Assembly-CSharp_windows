using System;

namespace UnityEngine.UIElements.UIR
{
	internal class BasicNodePool<T> : LinkedPool<BasicNode<T>>
	{
		private static void Reset(BasicNode<T> node)
		{
			node.next = null;
			node.data = default(T);
		}

		private static BasicNode<T> Create()
		{
			return new BasicNode<T>();
		}

		public BasicNodePool()
			: base(new Func<BasicNode<T>>(BasicNodePool<T>.Create), new Action<BasicNode<T>>(BasicNodePool<T>.Reset), 10000)
		{
		}
	}
}
