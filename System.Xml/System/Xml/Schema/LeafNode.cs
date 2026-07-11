using System;

namespace System.Xml.Schema
{
	internal class LeafNode : SyntaxTreeNode
	{
		public LeafNode(int pos)
		{
			this.pos = pos;
		}

		public int Pos
		{
			get
			{
				return this.pos;
			}
			set
			{
				this.pos = value;
			}
		}

		public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
		{
		}

		public override SyntaxTreeNode Clone(Positions positions)
		{
			return new LeafNode(positions.Add(positions[this.pos].symbol, positions[this.pos].particle));
		}

		public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
		{
			firstpos.Set(this.pos);
			lastpos.Set(this.pos);
		}

		public override bool IsNullable
		{
			get
			{
				return false;
			}
		}

		private int pos;
	}
}
