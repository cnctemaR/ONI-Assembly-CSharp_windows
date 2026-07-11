using System;
using System.Collections;

namespace System.Xml.Schema
{
	internal class SelectorActiveAxis : ActiveAxis
	{
		public bool EmptyStack
		{
			get
			{
				return this.KSpointer == 0;
			}
		}

		public int lastDepth
		{
			get
			{
				if (this.KSpointer != 0)
				{
					return ((KSStruct)this.KSs[this.KSpointer - 1]).depth;
				}
				return -1;
			}
		}

		public SelectorActiveAxis(Asttree axisTree, ConstraintStruct cs)
			: base(axisTree)
		{
			this.KSs = new ArrayList();
			this.cs = cs;
		}

		public override bool EndElement(string localname, string URN)
		{
			base.EndElement(localname, URN);
			return this.KSpointer > 0 && base.CurrentDepth == this.lastDepth;
		}

		public int PushKS(int errline, int errcol)
		{
			KeySequence keySequence = new KeySequence(this.cs.TableDim, errline, errcol);
			KSStruct ksstruct;
			if (this.KSpointer < this.KSs.Count)
			{
				ksstruct = (KSStruct)this.KSs[this.KSpointer];
				ksstruct.ks = keySequence;
				for (int i = 0; i < this.cs.TableDim; i++)
				{
					ksstruct.fields[i].Reactivate(keySequence);
				}
			}
			else
			{
				ksstruct = new KSStruct(keySequence, this.cs.TableDim);
				for (int j = 0; j < this.cs.TableDim; j++)
				{
					ksstruct.fields[j] = new LocatedActiveAxis(this.cs.constraint.Fields[j], keySequence, j);
					this.cs.axisFields.Add(ksstruct.fields[j]);
				}
				this.KSs.Add(ksstruct);
			}
			ksstruct.depth = base.CurrentDepth - 1;
			int kspointer = this.KSpointer;
			this.KSpointer = kspointer + 1;
			return kspointer;
		}

		public KeySequence PopKS()
		{
			ArrayList kss = this.KSs;
			int num = this.KSpointer - 1;
			this.KSpointer = num;
			return ((KSStruct)kss[num]).ks;
		}

		private ConstraintStruct cs;

		private ArrayList KSs;

		private int KSpointer;
	}
}
