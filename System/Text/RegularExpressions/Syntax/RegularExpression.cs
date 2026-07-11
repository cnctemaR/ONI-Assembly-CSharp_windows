using System;

namespace System.Text.RegularExpressions.Syntax
{
	internal class RegularExpression : Group
	{
		public RegularExpression()
		{
			this.group_count = 0;
		}

		public int GroupCount
		{
			get
			{
				return this.group_count;
			}
			set
			{
				this.group_count = value;
			}
		}

		public override void Compile(ICompiler cmp, bool reverse)
		{
			int num;
			int num2;
			this.GetWidth(out num, out num2);
			cmp.EmitInfo(this.group_count, num, num2);
			AnchorInfo anchorInfo = this.GetAnchorInfo(reverse);
			LinkRef linkRef = cmp.NewLink();
			cmp.EmitAnchor(reverse, anchorInfo.Offset, linkRef);
			if (anchorInfo.IsPosition)
			{
				cmp.EmitPosition(anchorInfo.Position);
			}
			else if (anchorInfo.IsSubstring)
			{
				cmp.EmitString(anchorInfo.Substring, anchorInfo.IgnoreCase, reverse);
			}
			cmp.EmitTrue();
			cmp.ResolveLink(linkRef);
			base.Compile(cmp, reverse);
			cmp.EmitTrue();
		}

		private int group_count;
	}
}
