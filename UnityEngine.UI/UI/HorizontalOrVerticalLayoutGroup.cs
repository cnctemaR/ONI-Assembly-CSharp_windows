using System;

namespace UnityEngine.UI
{
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

		protected void CalcAlongAxis(int axis, bool isVertical)
		{
			float num = (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal);
			bool flag = ((axis != 0) ? this.m_ChildControlHeight : this.m_ChildControlWidth);
			bool flag2 = ((axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth);
			float num2 = num;
			float num3 = num;
			float num4 = 0f;
			bool flag3 = isVertical ^ (axis == 1);
			for (int i = 0; i < base.rectChildren.Count; i++)
			{
				RectTransform rectTransform = base.rectChildren[i];
				float num5;
				float num6;
				float num7;
				this.GetChildSizes(rectTransform, axis, flag, flag2, out num5, out num6, out num7);
				if (flag3)
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
			if (!flag3 && base.rectChildren.Count > 0)
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
			bool flag = ((axis != 0) ? this.m_ChildControlHeight : this.m_ChildControlWidth);
			bool flag2 = ((axis != 0) ? this.childForceExpandHeight : this.childForceExpandWidth);
			float alignmentOnAxis = base.GetAlignmentOnAxis(axis);
			bool flag3 = isVertical ^ (axis == 1);
			if (flag3)
			{
				float num2 = num - (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal);
				for (int i = 0; i < base.rectChildren.Count; i++)
				{
					RectTransform rectTransform = base.rectChildren[i];
					float num3;
					float num4;
					float num5;
					this.GetChildSizes(rectTransform, axis, flag, flag2, out num3, out num4, out num5);
					float num6 = Mathf.Clamp(num2, num3, (num5 <= 0f) ? num4 : num);
					float startOffset = base.GetStartOffset(axis, num6);
					if (flag)
					{
						base.SetChildAlongAxis(rectTransform, axis, startOffset, num6);
					}
					else
					{
						float num7 = (num6 - rectTransform.sizeDelta[axis]) * alignmentOnAxis;
						base.SetChildAlongAxis(rectTransform, axis, startOffset + num7);
					}
				}
			}
			else
			{
				float num8 = (float)((axis != 0) ? base.padding.top : base.padding.left);
				if (base.GetTotalFlexibleSize(axis) == 0f && base.GetTotalPreferredSize(axis) < num)
				{
					num8 = base.GetStartOffset(axis, base.GetTotalPreferredSize(axis) - (float)((axis != 0) ? base.padding.vertical : base.padding.horizontal));
				}
				float num9 = 0f;
				if (base.GetTotalMinSize(axis) != base.GetTotalPreferredSize(axis))
				{
					num9 = Mathf.Clamp01((num - base.GetTotalMinSize(axis)) / (base.GetTotalPreferredSize(axis) - base.GetTotalMinSize(axis)));
				}
				float num10 = 0f;
				if (num > base.GetTotalPreferredSize(axis))
				{
					if (base.GetTotalFlexibleSize(axis) > 0f)
					{
						num10 = (num - base.GetTotalPreferredSize(axis)) / base.GetTotalFlexibleSize(axis);
					}
				}
				for (int j = 0; j < base.rectChildren.Count; j++)
				{
					RectTransform rectTransform2 = base.rectChildren[j];
					float num11;
					float num12;
					float num13;
					this.GetChildSizes(rectTransform2, axis, flag, flag2, out num11, out num12, out num13);
					float num14 = Mathf.Lerp(num11, num12, num9);
					num14 += num13 * num10;
					if (flag)
					{
						base.SetChildAlongAxis(rectTransform2, axis, num8, num14);
					}
					else
					{
						float num15 = (num14 - rectTransform2.sizeDelta[axis]) * alignmentOnAxis;
						base.SetChildAlongAxis(rectTransform2, axis, num8 + num15);
					}
					num8 += num14 + this.spacing;
				}
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
		protected float m_Spacing = 0f;

		[SerializeField]
		protected bool m_ChildForceExpandWidth = true;

		[SerializeField]
		protected bool m_ChildForceExpandHeight = true;

		[SerializeField]
		protected bool m_ChildControlWidth = true;

		[SerializeField]
		protected bool m_ChildControlHeight = true;
	}
}
