using System;

namespace UnityEngine.UI
{
	[ExecuteAlways]
	public abstract class HorizontalOrVerticalLayoutGroup : LayoutGroup
	{
		public float spacing
		{
			get
			{
				return this.m_Spacing;
			}
			set
			{
				base.SetProperty<float>(ref this.m_Spacing, value);
			}
		}

		public bool childForceExpandWidth
		{
			get
			{
				return this.m_ChildForceExpandWidth;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildForceExpandWidth, value);
			}
		}

		public bool childForceExpandHeight
		{
			get
			{
				return this.m_ChildForceExpandHeight;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildForceExpandHeight, value);
			}
		}

		public bool childControlWidth
		{
			get
			{
				return this.m_ChildControlWidth;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildControlWidth, value);
			}
		}

		public bool childControlHeight
		{
			get
			{
				return this.m_ChildControlHeight;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildControlHeight, value);
			}
		}

		public bool childScaleWidth
		{
			get
			{
				return this.m_ChildScaleWidth;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildScaleWidth, value);
			}
		}

		public bool childScaleHeight
		{
			get
			{
				return this.m_ChildScaleHeight;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ChildScaleHeight, value);
			}
		}

		public bool reverseArrangement
		{
			get
			{
				return this.m_ReverseArrangement;
			}
			set
			{
				base.SetProperty<bool>(ref this.m_ReverseArrangement, value);
			}
		}

		protected void CalcAlongAxis(int axis, bool isVertical)
		{
			float num = (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical);
			bool flag = ((axis == 0) ? this.m_ChildControlWidth : this.m_ChildControlHeight);
			bool flag2 = ((axis == 0) ? this.m_ChildScaleWidth : this.m_ChildScaleHeight);
			bool flag3 = ((axis == 0) ? this.m_ChildForceExpandWidth : this.m_ChildForceExpandHeight);
			float num2 = num;
			float num3 = num;
			float num4 = 0f;
			bool flag4 = isVertical ^ (axis == 1);
			int count = base.rectChildren.Count;
			for (int i = 0; i < count; i++)
			{
				RectTransform rectTransform = base.rectChildren[i];
				float num5;
				float num6;
				float num7;
				this.GetChildSizes(rectTransform, axis, flag, flag3, out num5, out num6, out num7);
				if (flag2)
				{
					float num8 = rectTransform.localScale[axis];
					num5 *= num8;
					num6 *= num8;
					num7 *= num8;
				}
				if (flag4)
				{
					num2 = Mathf.Max(num5 + num, num2);
					num3 = Mathf.Max(num6 + num, num3);
					num4 = Mathf.Max(num7, num4);
				}
				else
				{
					num2 += num5 + this.spacing;
					num3 += num6 + this.spacing;
					num4 += num7;
				}
			}
			if (!flag4 && base.rectChildren.Count > 0)
			{
				num2 -= this.spacing;
				num3 -= this.spacing;
			}
			num3 = Mathf.Max(num2, num3);
			base.SetLayoutInputForAxis(num2, num3, num4, axis);
		}

		protected void SetChildrenAlongAxis(int axis, bool isVertical)
		{
			float num = base.rectTransform.rect.size[axis];
			bool flag = ((axis == 0) ? this.m_ChildControlWidth : this.m_ChildControlHeight);
			bool flag2 = ((axis == 0) ? this.m_ChildScaleWidth : this.m_ChildScaleHeight);
			bool flag3 = ((axis == 0) ? this.m_ChildForceExpandWidth : this.m_ChildForceExpandHeight);
			float alignmentOnAxis = base.GetAlignmentOnAxis(axis);
			bool flag4 = isVertical ^ (axis == 1);
			int num2 = (this.m_ReverseArrangement ? (base.rectChildren.Count - 1) : 0);
			int num3 = (this.m_ReverseArrangement ? 0 : base.rectChildren.Count);
			int num4 = (this.m_ReverseArrangement ? (-1) : 1);
			if (flag4)
			{
				float num5 = num - (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical);
				int num6 = num2;
				while (this.m_ReverseArrangement ? (num6 >= num3) : (num6 < num3))
				{
					RectTransform rectTransform = base.rectChildren[num6];
					float num7;
					float num8;
					float num9;
					this.GetChildSizes(rectTransform, axis, flag, flag3, out num7, out num8, out num9);
					float num10 = (flag2 ? rectTransform.localScale[axis] : 1f);
					float num11 = Mathf.Clamp(num5, num7, (num9 > 0f) ? num : num8);
					float startOffset = base.GetStartOffset(axis, num11 * num10);
					if (flag)
					{
						base.SetChildAlongAxisWithScale(rectTransform, axis, startOffset, num11, num10);
					}
					else
					{
						float num12 = (num11 - rectTransform.sizeDelta[axis]) * alignmentOnAxis;
						base.SetChildAlongAxisWithScale(rectTransform, axis, startOffset + num12, num10);
					}
					num6 += num4;
				}
				return;
			}
			float num13 = (float)((axis == 0) ? base.padding.left : base.padding.top);
			float num14 = 0f;
			float num15 = num - base.GetTotalPreferredSize(axis);
			if (num15 > 0f)
			{
				if (base.GetTotalFlexibleSize(axis) == 0f)
				{
					num13 = base.GetStartOffset(axis, base.GetTotalPreferredSize(axis) - (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical));
				}
				else if (base.GetTotalFlexibleSize(axis) > 0f)
				{
					num14 = num15 / base.GetTotalFlexibleSize(axis);
				}
			}
			float num16 = 0f;
			if (base.GetTotalMinSize(axis) != base.GetTotalPreferredSize(axis))
			{
				num16 = Mathf.Clamp01((num - base.GetTotalMinSize(axis)) / (base.GetTotalPreferredSize(axis) - base.GetTotalMinSize(axis)));
			}
			int num17 = num2;
			while (this.m_ReverseArrangement ? (num17 >= num3) : (num17 < num3))
			{
				RectTransform rectTransform2 = base.rectChildren[num17];
				float num18;
				float num19;
				float num20;
				this.GetChildSizes(rectTransform2, axis, flag, flag3, out num18, out num19, out num20);
				float num21 = (flag2 ? rectTransform2.localScale[axis] : 1f);
				float num22 = Mathf.Lerp(num18, num19, num16);
				num22 += num20 * num14;
				if (flag)
				{
					base.SetChildAlongAxisWithScale(rectTransform2, axis, num13, num22, num21);
				}
				else
				{
					float num23 = (num22 - rectTransform2.sizeDelta[axis]) * alignmentOnAxis;
					base.SetChildAlongAxisWithScale(rectTransform2, axis, num13 + num23, num21);
				}
				num13 += num22 * num21 + this.spacing;
				num17 += num4;
			}
		}

		private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand, out float min, out float preferred, out float flexible)
		{
			if (!controlSize)
			{
				min = child.sizeDelta[axis];
				preferred = min;
				flexible = 0f;
			}
			else
			{
				min = LayoutUtility.GetMinSize(child, axis);
				preferred = LayoutUtility.GetPreferredSize(child, axis);
				flexible = LayoutUtility.GetFlexibleSize(child, axis);
			}
			if (childForceExpand)
			{
				flexible = Mathf.Max(flexible, 1f);
			}
		}

		[SerializeField]
		protected float m_Spacing;

		[SerializeField]
		protected bool m_ChildForceExpandWidth = true;

		[SerializeField]
		protected bool m_ChildForceExpandHeight = true;

		[SerializeField]
		protected bool m_ChildControlWidth = true;

		[SerializeField]
		protected bool m_ChildControlHeight = true;

		[SerializeField]
		protected bool m_ChildScaleWidth;

		[SerializeField]
		protected bool m_ChildScaleHeight;

		[SerializeField]
		protected bool m_ReverseArrangement;
	}
}
