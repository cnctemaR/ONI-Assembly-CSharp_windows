using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class AncestorFilter
	{
		private void AddHash(int hash)
		{
			this.m_HashStack.Push(hash);
			this.m_CountingBloomFilter.InsertHash((uint)hash);
		}

		public unsafe bool IsCandidate(StyleComplexSelector complexSel)
		{
			int i = 0;
			while (i < 4)
			{
				bool flag = *((ref complexSel.ancestorHashes.hashes.FixedElementField) + (IntPtr)i * 4) == 0;
				bool flag2;
				if (flag)
				{
					flag2 = true;
				}
				else
				{
					bool flag3 = !this.m_CountingBloomFilter.ContainsHash((uint)(*((ref complexSel.ancestorHashes.hashes.FixedElementField) + (IntPtr)i * 4)));
					if (!flag3)
					{
						i++;
						continue;
					}
					flag2 = false;
				}
				return flag2;
			}
			return true;
		}

		public void PushElement(VisualElement element)
		{
			int count = this.m_HashStack.Count;
			this.AddHash(element.typeName.GetHashCode() * 13);
			bool flag = !string.IsNullOrEmpty(element.name);
			if (flag)
			{
				this.AddHash(element.name.GetHashCode() * 17);
			}
			List<string> classesForIteration = element.GetClassesForIteration();
			for (int i = 0; i < classesForIteration.Count; i++)
			{
				this.AddHash(classesForIteration[i].GetHashCode() * 19);
			}
			this.m_HashStack.Push(this.m_HashStack.Count - count);
		}

		public void PopElement()
		{
			int i = this.m_HashStack.Peek();
			this.m_HashStack.Pop();
			while (i > 0)
			{
				int num = this.m_HashStack.Peek();
				this.m_CountingBloomFilter.RemoveHash((uint)num);
				this.m_HashStack.Pop();
				i--;
			}
		}

		private CountingBloomFilter m_CountingBloomFilter;

		private Stack<int> m_HashStack = new Stack<int>(100);
	}
}
