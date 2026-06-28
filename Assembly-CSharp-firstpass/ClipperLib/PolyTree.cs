using System;
using System.Collections.Generic;

namespace ClipperLib
{
	public class PolyTree : PolyNode
	{
		~PolyTree()
		{
			this.Clear();
		}

		public void Clear()
		{
			for (int i = 0; i < this.m_AllPolys.Count; i++)
			{
				this.m_AllPolys[i] = null;
			}
			this.m_AllPolys.Clear();
			this.m_Childs.Clear();
		}

		public PolyNode GetFirst()
		{
			if (this.m_Childs.Count > 0)
			{
				return this.m_Childs[0];
			}
			return null;
		}

		public int Total
		{
			get
			{
				int num = this.m_AllPolys.Count;
				if (num > 0 && this.m_Childs[0] != this.m_AllPolys[0])
				{
					num--;
				}
				return num;
			}
		}

		internal List<PolyNode> m_AllPolys = new List<PolyNode>();
	}
}
