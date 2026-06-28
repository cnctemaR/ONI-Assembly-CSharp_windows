using System;
using System.Collections;

namespace System.Security.Cryptography.Xml
{
	public class TransformChain
	{
		public TransformChain()
		{
			this.chain = new ArrayList();
		}

		public int Count
		{
			get
			{
				return this.chain.Count;
			}
		}

		public Transform this[int index]
		{
			get
			{
				return (Transform)this.chain[index];
			}
		}

		public void Add(Transform transform)
		{
			this.chain.Add(transform);
		}

		public IEnumerator GetEnumerator()
		{
			return this.chain.GetEnumerator();
		}

		private ArrayList chain;
	}
}
