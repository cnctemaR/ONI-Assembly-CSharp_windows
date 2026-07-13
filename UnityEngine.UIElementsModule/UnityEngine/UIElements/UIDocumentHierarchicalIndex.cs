using System;
using System.Text;

namespace UnityEngine.UIElements
{
	internal struct UIDocumentHierarchicalIndex : IComparable<UIDocumentHierarchicalIndex>
	{
		public int CompareTo(UIDocumentHierarchicalIndex other)
		{
			bool flag = this.pathToParent == null;
			int num;
			if (flag)
			{
				bool flag2 = other.pathToParent == null;
				if (flag2)
				{
					num = 0;
				}
				else
				{
					num = 1;
				}
			}
			else
			{
				bool flag3 = other.pathToParent == null;
				if (flag3)
				{
					num = -1;
				}
				else
				{
					int num2 = this.pathToParent.Length;
					int num3 = other.pathToParent.Length;
					int num4 = 0;
					while (num4 < num2 && num4 < num3)
					{
						bool flag4 = this.pathToParent[num4] < other.pathToParent[num4];
						if (flag4)
						{
							return -1;
						}
						bool flag5 = this.pathToParent[num4] > other.pathToParent[num4];
						if (flag5)
						{
							return 1;
						}
						num4++;
					}
					bool flag6 = num2 > num3;
					if (flag6)
					{
						num = 1;
					}
					else
					{
						bool flag7 = num2 < num3;
						if (flag7)
						{
							num = -1;
						}
						else
						{
							num = 0;
						}
					}
				}
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("pathToParent = [");
			bool flag = this.pathToParent != null;
			if (flag)
			{
				int num = this.pathToParent.Length;
				for (int i = 0; i < num; i++)
				{
					stringBuilder.Append(this.pathToParent[i]);
					bool flag2 = i < num - 1;
					if (flag2)
					{
						stringBuilder.Append(", ");
					}
				}
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		internal int[] pathToParent;
	}
}
