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
			for (int i = 0; i < base.rectChildren.Count; i++)
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
			if (isVertical ^ (axis == 1))
			{
				float num2 = num - (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical);
				for (int i = 0; i < base.rectChildren.Count; i++)
				{
					RectTransform rectTransform = base.rectChildren[i];
					float num3;
					float num4;
					float num5;
					this.GetChildSizes(rectTransform, axis, flag, flag3, out num3, out num4, out num5);
					float num6 = (flag2 ? rectTransform.localScale[axis] : 1f);
					float num7 = Mathf.Clamp(num2, num3, (num5 > 0f) ? num : num4);
					float startOffset = base.GetStartOffset(axis, num7 * num6);
					if (flag)
					{
						base.SetChildAlongAxisWithScale(rectTransform, axis, startOffset, num7, num6);
					}
					else
					{
						float num8 = (num7 - rectTransform.sizeDelta[axis]) * alignmentOnAxis;
						base.SetChildAlongAxisWithScale(rectTransform, axis, startOffset + num8, num6);
					}
				}
				return;
			}
			float num9 = (float)((axis == 0) ? base.padding.left : base.padding.top);
			float num10 = 0f;
			float num11 = num - base.GetTotalPreferredSize(axis);
			if (num11 > 0f)
			{
				if (base.GetTotalFlexibleSize(axis) == 0f)
				{
					num9 = base.GetStartOffset(axis, base.GetTotalPreferredSize(axis) - (float)((axis == 0) ? base.padding.horizontal : base.padding.vertical));
				}
				else if (base.GetTotalFlexibleSize(axis) > 0f)
				{
					num10 = num11 / base.GetTotalFlexibleSize(axis);
				}
			}
			float num12 = 0f;
			if (base.GetTotalMinSize(axis) != base.GetTotalPreferredSize(axis))
			{
				num12 = Mathf.Clamp01((num - base.GetTotalMinSize(axis)) / (base.GetTotalPreferredSize(axis) - base.GetTotalMinSize(axis)));
			}
			for (int j = 0; j < base.rectChildren.Count; j++)
			{
				RectTransform rectTransform2 = base.rectChildren[j];
				float num13;
				float num14;
				float num15;
				this.GetChildSizes(rectTransform2, axis, flag, flag3, out num13, out num14, out num15);
				float num16 = (flag2 ? rectTransform2.localScale[axis] : 1f);
				float num17 = Mathf.Lerp(num13, num14, num12);
				num17 += num15 * num10;
				if (flag)
				{
					base.SetChildAlongAxisWithScale(rectTransform2, axis, num9, num17, num16);
				}
				else
				{
					float num18 = (num17 - rectTransform2.sizeDelta[axis]) * alignmentOnAxis;
					base.SetChildAlongAxisWithScale(rectTransform2, axis, num9 + num18, num16);
				}
				num9 += num17 * num16 + this.spacing;
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
	}
}
