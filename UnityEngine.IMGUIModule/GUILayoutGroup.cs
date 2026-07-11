using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal class GUILayoutGroup : GUILayoutEntry
	{
		public GUILayoutGroup()
			: base(0f, 0f, 0f, 0f, GUIStyle.none)
		{
		}

		public GUILayoutGroup(GUIStyle _style, GUILayoutOption[] options)
			: base(0f, 0f, 0f, 0f, _style)
		{
			if (options != null)
			{
				this.ApplyOptions(options);
			}
			this.m_MarginLeft = _style.margin.left;
			this.m_MarginRight = _style.margin.right;
			this.m_MarginTop = _style.margin.top;
			this.m_MarginBottom = _style.margin.bottom;
		}

		public override int marginLeft
		{
			[CompilerGenerated]
			get
			{
				return this.m_MarginLeft;
			}
		}

		public override int marginRight
		{
			[CompilerGenerated]
			get
			{
				return this.m_MarginRight;
			}
		}

		public override int marginTop
		{
			[CompilerGenerated]
			get
			{
				return this.m_MarginTop;
			}
		}

		public override int marginBottom
		{
			[CompilerGenerated]
			get
			{
				return this.m_MarginBottom;
			}
		}

		public override void ApplyOptions(GUILayoutOption[] options)
		{
			if (options != null)
			{
				base.ApplyOptions(options);
				foreach (GUILayoutOption guilayoutOption in options)
				{
					switch (guilayoutOption.type)
					{
					case GUILayoutOption.Type.fixedWidth:
					case GUILayoutOption.Type.minWidth:
					case GUILayoutOption.Type.maxWidth:
						this.m_UserSpecifiedHeight = true;
						break;
					case GUILayoutOption.Type.fixedHeight:
					case GUILayoutOption.Type.minHeight:
					case GUILayoutOption.Type.maxHeight:
						this.m_UserSpecifiedWidth = true;
						break;
					case GUILayoutOption.Type.spacing:
						this.spacing = (float)((int)guilayoutOption.value);
						break;
					}
				}
			}
		}

		protected override void ApplyStyleSettings(GUIStyle style)
		{
			base.ApplyStyleSettings(style);
			RectOffset margin = style.margin;
			this.m_MarginLeft = margin.left;
			this.m_MarginRight = margin.right;
			this.m_MarginTop = margin.top;
			this.m_MarginBottom = margin.bottom;
		}

		public void ResetCursor()
		{
			this.m_Cursor = 0;
		}

		public Rect PeekNext()
		{
			if (this.m_Cursor < this.entries.Count)
			{
				GUILayoutEntry guilayoutEntry = this.entries[this.m_Cursor];
				return guilayoutEntry.rect;
			}
			throw new ArgumentException(string.Concat(new object[]
			{
				"Getting control ",
				this.m_Cursor,
				"'s position in a group with only ",
				this.entries.Count,
				" controls when doing ",
				Event.current.rawType,
				"\nAborting"
			}));
		}

		public GUILayoutEntry GetNext()
		{
			if (this.m_Cursor < this.entries.Count)
			{
				GUILayoutEntry guilayoutEntry = this.entries[this.m_Cursor];
				this.m_Cursor++;
				return guilayoutEntry;
			}
			throw new ArgumentException(string.Concat(new object[]
			{
				"Getting control ",
				this.m_Cursor,
				"'s position in a group with only ",
				this.entries.Count,
				" controls when doing ",
				Event.current.rawType,
				"\nAborting"
			}));
		}

		public Rect GetLast()
		{
			Rect rect;
			if (this.m_Cursor == 0)
			{
				Debug.LogError("You cannot call GetLast immediately after beginning a group.");
				rect = GUILayoutEntry.kDummyRect;
			}
			else if (this.m_Cursor <= this.entries.Count)
			{
				GUILayoutEntry guilayoutEntry = this.entries[this.m_Cursor - 1];
				rect = guilayoutEntry.rect;
			}
			else
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Getting control ",
					this.m_Cursor,
					"'s position in a group with only ",
					this.entries.Count,
					" controls when doing ",
					Event.current.type
				}));
				rect = GUILayoutEntry.kDummyRect;
			}
			return rect;
		}

		public void Add(GUILayoutEntry e)
		{
			this.entries.Add(e);
		}

		public override void CalcWidth()
		{
			if (this.entries.Count == 0)
			{
				this.maxWidth = (this.minWidth = (float)base.style.padding.horizontal);
			}
			else
			{
				int num = 0;
				int num2 = 0;
				this.m_ChildMinWidth = 0f;
				this.m_ChildMaxWidth = 0f;
				this.m_StretchableCountX = 0;
				bool flag = true;
				if (this.isVertical)
				{
					foreach (GUILayoutEntry guilayoutEntry in this.entries)
					{
						guilayoutEntry.CalcWidth();
						if (guilayoutEntry.consideredForMargin)
						{
							if (!flag)
							{
								num = Mathf.Min(guilayoutEntry.marginLeft, num);
								num2 = Mathf.Min(guilayoutEntry.marginRight, num2);
							}
							else
							{
								num = guilayoutEntry.marginLeft;
								num2 = guilayoutEntry.marginRight;
								flag = false;
							}
							this.m_ChildMinWidth = Mathf.Max(guilayoutEntry.minWidth + (float)guilayoutEntry.marginHorizontal, this.m_ChildMinWidth);
							this.m_ChildMaxWidth = Mathf.Max(guilayoutEntry.maxWidth + (float)guilayoutEntry.marginHorizontal, this.m_ChildMaxWidth);
						}
						this.m_StretchableCountX += guilayoutEntry.stretchWidth;
					}
					this.m_ChildMinWidth -= (float)(num + num2);
					this.m_ChildMaxWidth -= (float)(num + num2);
				}
				else
				{
					int num3 = 0;
					foreach (GUILayoutEntry guilayoutEntry2 in this.entries)
					{
						guilayoutEntry2.CalcWidth();
						if (guilayoutEntry2.consideredForMargin)
						{
							int num4;
							if (!flag)
							{
								num4 = ((num3 <= guilayoutEntry2.marginLeft) ? guilayoutEntry2.marginLeft : num3);
							}
							else
							{
								num4 = 0;
								flag = false;
							}
							this.m_ChildMinWidth += guilayoutEntry2.minWidth + this.spacing + (float)num4;
							this.m_ChildMaxWidth += guilayoutEntry2.maxWidth + this.spacing + (float)num4;
							num3 = guilayoutEntry2.marginRight;
							this.m_StretchableCountX += guilayoutEntry2.stretchWidth;
						}
						else
						{
							this.m_ChildMinWidth += guilayoutEntry2.minWidth;
							this.m_ChildMaxWidth += guilayoutEntry2.maxWidth;
							this.m_StretchableCountX += guilayoutEntry2.stretchWidth;
						}
					}
					this.m_ChildMinWidth -= this.spacing;
					this.m_ChildMaxWidth -= this.spacing;
					if (this.entries.Count != 0)
					{
						num = this.entries[0].marginLeft;
						num2 = num3;
					}
					else
					{
						num2 = (num = 0);
					}
				}
				float num5;
				float num6;
				if (base.style != GUIStyle.none || this.m_UserSpecifiedWidth)
				{
					num5 = (float)Mathf.Max(base.style.padding.left, num);
					num6 = (float)Mathf.Max(base.style.padding.right, num2);
				}
				else
				{
					this.m_MarginLeft = num;
					this.m_MarginRight = num2;
					num6 = (num5 = 0f);
				}
				this.minWidth = Mathf.Max(this.minWidth, this.m_ChildMinWidth + num5 + num6);
				if (this.maxWidth == 0f)
				{
					this.stretchWidth += this.m_StretchableCountX + ((!base.style.stretchWidth) ? 0 : 1);
					this.maxWidth = this.m_ChildMaxWidth + num5 + num6;
				}
				else
				{
					this.stretchWidth = 0;
				}
				this.maxWidth = Mathf.Max(this.maxWidth, this.minWidth);
				if (base.style.fixedWidth != 0f)
				{
					this.maxWidth = (this.minWidth = base.style.fixedWidth);
					this.stretchWidth = 0;
				}
			}
		}

		public override void SetHorizontal(float x, float width)
		{
			base.SetHorizontal(x, width);
			if (this.resetCoords)
			{
				x = 0f;
			}
			RectOffset padding = base.style.padding;
			if (this.isVertical)
			{
				if (base.style != GUIStyle.none)
				{
					foreach (GUILayoutEntry guilayoutEntry in this.entries)
					{
						float num = (float)Mathf.Max(guilayoutEntry.marginLeft, padding.left);
						float num2 = x + num;
						float num3 = width - (float)Mathf.Max(guilayoutEntry.marginRight, padding.right) - num;
						if (guilayoutEntry.stretchWidth != 0)
						{
							guilayoutEntry.SetHorizontal(num2, num3);
						}
						else
						{
							guilayoutEntry.SetHorizontal(num2, Mathf.Clamp(num3, guilayoutEntry.minWidth, guilayoutEntry.maxWidth));
						}
					}
				}
				else
				{
					float num4 = x - (float)this.marginLeft;
					float num5 = width + (float)base.marginHorizontal;
					foreach (GUILayoutEntry guilayoutEntry2 in this.entries)
					{
						if (guilayoutEntry2.stretchWidth != 0)
						{
							guilayoutEntry2.SetHorizontal(num4 + (float)guilayoutEntry2.marginLeft, num5 - (float)guilayoutEntry2.marginHorizontal);
						}
						else
						{
							guilayoutEntry2.SetHorizontal(num4 + (float)guilayoutEntry2.marginLeft, Mathf.Clamp(num5 - (float)guilayoutEntry2.marginHorizontal, guilayoutEntry2.minWidth, guilayoutEntry2.maxWidth));
						}
					}
				}
			}
			else
			{
				if (base.style != GUIStyle.none)
				{
					float num6 = (float)padding.left;
					float num7 = (float)padding.right;
					if (this.entries.Count != 0)
					{
						num6 = Mathf.Max(num6, (float)this.entries[0].marginLeft);
						num7 = Mathf.Max(num7, (float)this.entries[this.entries.Count - 1].marginRight);
					}
					x += num6;
					width -= num7 + num6;
				}
				float num8 = width - this.spacing * (float)(this.entries.Count - 1);
				float num9 = 0f;
				if (this.m_ChildMinWidth != this.m_ChildMaxWidth)
				{
					num9 = Mathf.Clamp((num8 - this.m_ChildMinWidth) / (this.m_ChildMaxWidth - this.m_ChildMinWidth), 0f, 1f);
				}
				float num10 = 0f;
				if (num8 > this.m_ChildMaxWidth)
				{
					if (this.m_StretchableCountX > 0)
					{
						num10 = (num8 - this.m_ChildMaxWidth) / (float)this.m_StretchableCountX;
					}
				}
				int num11 = 0;
				bool flag = true;
				foreach (GUILayoutEntry guilayoutEntry3 in this.entries)
				{
					float num12 = Mathf.Lerp(guilayoutEntry3.minWidth, guilayoutEntry3.maxWidth, num9);
					num12 += num10 * (float)guilayoutEntry3.stretchWidth;
					if (guilayoutEntry3.consideredForMargin)
					{
						int num13 = guilayoutEntry3.marginLeft;
						if (flag)
						{
							num13 = 0;
							flag = false;
						}
						int num14 = ((num11 <= num13) ? num13 : num11);
						x += (float)num14;
						num11 = guilayoutEntry3.marginRight;
					}
					guilayoutEntry3.SetHorizontal(Mathf.Round(x), Mathf.Round(num12));
					x += num12 + this.spacing;
				}
			}
		}

		public override void CalcHeight()
		{
			if (this.entries.Count == 0)
			{
				this.maxHeight = (this.minHeight = (float)base.style.padding.vertical);
			}
			else
			{
				int num = 0;
				int num2 = 0;
				this.m_ChildMinHeight = 0f;
				this.m_ChildMaxHeight = 0f;
				this.m_StretchableCountY = 0;
				if (this.isVertical)
				{
					int num3 = 0;
					bool flag = true;
					foreach (GUILayoutEntry guilayoutEntry in this.entries)
					{
						guilayoutEntry.CalcHeight();
						if (guilayoutEntry.consideredForMargin)
						{
							int num4;
							if (!flag)
							{
								num4 = Mathf.Max(num3, guilayoutEntry.marginTop);
							}
							else
							{
								num4 = 0;
								flag = false;
							}
							this.m_ChildMinHeight += guilayoutEntry.minHeight + this.spacing + (float)num4;
							this.m_ChildMaxHeight += guilayoutEntry.maxHeight + this.spacing + (float)num4;
							num3 = guilayoutEntry.marginBottom;
							this.m_StretchableCountY += guilayoutEntry.stretchHeight;
						}
						else
						{
							this.m_ChildMinHeight += guilayoutEntry.minHeight;
							this.m_ChildMaxHeight += guilayoutEntry.maxHeight;
							this.m_StretchableCountY += guilayoutEntry.stretchHeight;
						}
					}
					this.m_ChildMinHeight -= this.spacing;
					this.m_ChildMaxHeight -= this.spacing;
					if (this.entries.Count != 0)
					{
						num = this.entries[0].marginTop;
						num2 = num3;
					}
					else
					{
						num = (num2 = 0);
					}
				}
				else
				{
					bool flag2 = true;
					foreach (GUILayoutEntry guilayoutEntry2 in this.entries)
					{
						guilayoutEntry2.CalcHeight();
						if (guilayoutEntry2.consideredForMargin)
						{
							if (!flag2)
							{
								num = Mathf.Min(guilayoutEntry2.marginTop, num);
								num2 = Mathf.Min(guilayoutEntry2.marginBottom, num2);
							}
							else
							{
								num = guilayoutEntry2.marginTop;
								num2 = guilayoutEntry2.marginBottom;
								flag2 = false;
							}
							this.m_ChildMinHeight = Mathf.Max(guilayoutEntry2.minHeight, this.m_ChildMinHeight);
							this.m_ChildMaxHeight = Mathf.Max(guilayoutEntry2.maxHeight, this.m_ChildMaxHeight);
						}
						this.m_StretchableCountY += guilayoutEntry2.stretchHeight;
					}
				}
				float num5;
				float num6;
				if (base.style != GUIStyle.none || this.m_UserSpecifiedHeight)
				{
					num5 = (float)Mathf.Max(base.style.padding.top, num);
					num6 = (float)Mathf.Max(base.style.padding.bottom, num2);
				}
				else
				{
					this.m_MarginTop = num;
					this.m_MarginBottom = num2;
					num6 = (num5 = 0f);
				}
				this.minHeight = Mathf.Max(this.minHeight, this.m_ChildMinHeight + num5 + num6);
				if (this.maxHeight == 0f)
				{
					this.stretchHeight += this.m_StretchableCountY + ((!base.style.stretchHeight) ? 0 : 1);
					this.maxHeight = this.m_ChildMaxHeight + num5 + num6;
				}
				else
				{
					this.stretchHeight = 0;
				}
				this.maxHeight = Mathf.Max(this.maxHeight, this.minHeight);
				if (base.style.fixedHeight != 0f)
				{
					this.maxHeight = (this.minHeight = base.style.fixedHeight);
					this.stretchHeight = 0;
				}
			}
		}

		public override void SetVertical(float y, float height)
		{
			base.SetVertical(y, height);
			if (this.entries.Count != 0)
			{
				RectOffset padding = base.style.padding;
				if (this.resetCoords)
				{
					y = 0f;
				}
				if (this.isVertical)
				{
					if (base.style != GUIStyle.none)
					{
						float num = (float)padding.top;
						float num2 = (float)padding.bottom;
						if (this.entries.Count != 0)
						{
							num = Mathf.Max(num, (float)this.entries[0].marginTop);
							num2 = Mathf.Max(num2, (float)this.entries[this.entries.Count - 1].marginBottom);
						}
						y += num;
						height -= num2 + num;
					}
					float num3 = height - this.spacing * (float)(this.entries.Count - 1);
					float num4 = 0f;
					if (this.m_ChildMinHeight != this.m_ChildMaxHeight)
					{
						num4 = Mathf.Clamp((num3 - this.m_ChildMinHeight) / (this.m_ChildMaxHeight - this.m_ChildMinHeight), 0f, 1f);
					}
					float num5 = 0f;
					if (num3 > this.m_ChildMaxHeight)
					{
						if (this.m_StretchableCountY > 0)
						{
							num5 = (num3 - this.m_ChildMaxHeight) / (float)this.m_StretchableCountY;
						}
					}
					int num6 = 0;
					bool flag = true;
					foreach (GUILayoutEntry guilayoutEntry in this.entries)
					{
						float num7 = Mathf.Lerp(guilayoutEntry.minHeight, guilayoutEntry.maxHeight, num4);
						num7 += num5 * (float)guilayoutEntry.stretchHeight;
						if (guilayoutEntry.consideredForMargin)
						{
							int num8 = guilayoutEntry.marginTop;
							if (flag)
							{
								num8 = 0;
								flag = false;
							}
							int num9 = ((num6 <= num8) ? num8 : num6);
							y += (float)num9;
							num6 = guilayoutEntry.marginBottom;
						}
						guilayoutEntry.SetVertical(Mathf.Round(y), Mathf.Round(num7));
						y += num7 + this.spacing;
					}
				}
				else if (base.style != GUIStyle.none)
				{
					foreach (GUILayoutEntry guilayoutEntry2 in this.entries)
					{
						float num10 = (float)Mathf.Max(guilayoutEntry2.marginTop, padding.top);
						float num11 = y + num10;
						float num12 = height - (float)Mathf.Max(guilayoutEntry2.marginBottom, padding.bottom) - num10;
						if (guilayoutEntry2.stretchHeight != 0)
						{
							guilayoutEntry2.SetVertical(num11, num12);
						}
						else
						{
							guilayoutEntry2.SetVertical(num11, Mathf.Clamp(num12, guilayoutEntry2.minHeight, guilayoutEntry2.maxHeight));
						}
					}
				}
				else
				{
					float num13 = y - (float)this.marginTop;
					float num14 = height + (float)base.marginVertical;
					foreach (GUILayoutEntry guilayoutEntry3 in this.entries)
					{
						if (guilayoutEntry3.stretchHeight != 0)
						{
							guilayoutEntry3.SetVertical(num13 + (float)guilayoutEntry3.marginTop, num14 - (float)guilayoutEntry3.marginVertical);
						}
						else
						{
							guilayoutEntry3.SetVertical(num13 + (float)guilayoutEntry3.marginTop, Mathf.Clamp(num14 - (float)guilayoutEntry3.marginVertical, guilayoutEntry3.minHeight, guilayoutEntry3.maxHeight));
						}
					}
				}
			}
		}

		public override string ToString()
		{
			string text = "";
			string text2 = "";
			for (int i = 0; i < GUILayoutEntry.indent; i++)
			{
				text2 += " ";
			}
			string text3 = text;
			text = string.Concat(new object[]
			{
				text3,
				base.ToString(),
				" Margins: ",
				this.m_ChildMinHeight,
				" {\n"
			});
			GUILayoutEntry.indent += 4;
			foreach (GUILayoutEntry guilayoutEntry in this.entries)
			{
				text = text + guilayoutEntry.ToString() + "\n";
			}
			text = text + text2 + "}";
			GUILayoutEntry.indent -= 4;
			return text;
		}

		public List<GUILayoutEntry> entries = new List<GUILayoutEntry>();

		public bool isVertical = true;

		public bool resetCoords = false;

		public float spacing = 0f;

		public bool sameSize = true;

		public bool isWindow = false;

		public int windowID = -1;

		private int m_Cursor = 0;

		protected int m_StretchableCountX = 100;

		protected int m_StretchableCountY = 100;

		protected bool m_UserSpecifiedWidth = false;

		protected bool m_UserSpecifiedHeight = false;

		protected float m_ChildMinWidth = 100f;

		protected float m_ChildMaxWidth = 100f;

		protected float m_ChildMinHeight = 100f;

		protected float m_ChildMaxHeight = 100f;

		protected int m_MarginLeft;

		protected int m_MarginRight;

		protected int m_MarginTop;

		protected int m_MarginBottom;
	}
}
