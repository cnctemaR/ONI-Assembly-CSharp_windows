using System;
using System.Collections.Generic;

namespace ClipperLib
{
	internal class PolyTree : PolyNode
	{
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
			bool flag = this.m_Childs.Count > 0;
			PolyNode polyNode;
			if (flag)
			{
				polyNode = this.m_Childs[0];
			}
			else
			{
				polyNode = null;
			}
			return polyNode;
		}

		public int Total
		{
			get
			{
				int num = this.m_AllPolys.Count;
				bool flag = num > 0 && this.m_Childs[0] != this.m_AllPolys[0];
				if (flag)
				{
					num--;
				}
				return num;
			}
		}

		internal List<PolyNode> m_AllPolys = new List<PolyNode>();
	}
}
