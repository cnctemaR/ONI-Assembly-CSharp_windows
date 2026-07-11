using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.XPath;

namespace System.Xml.Xsl.Runtime
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct DodSequenceMerge
	{
		public void Create(XmlQueryRuntime runtime)
		{
			this.firstSequence = null;
			this.sequencesToMerge = null;
			this.nodeCount = 0;
			this.runtime = runtime;
		}

		public void AddSequence(IList<XPathNavigator> sequence)
		{
			if (sequence.Count == 0)
			{
				return;
			}
			if (this.firstSequence == null)
			{
				this.firstSequence = sequence;
				return;
			}
			if (this.sequencesToMerge == null)
			{
				this.sequencesToMerge = new List<IEnumerator<XPathNavigator>>();
				this.MoveAndInsertSequence(this.firstSequence.GetEnumerator());
				this.nodeCount = this.firstSequence.Count;
			}
			this.MoveAndInsertSequence(sequence.GetEnumerator());
			this.nodeCount += sequence.Count;
		}

		public IList<XPathNavigator> MergeSequences()
		{
			if (this.firstSequence == null)
			{
				return XmlQueryNodeSequence.Empty;
			}
			if (this.sequencesToMerge == null || this.sequencesToMerge.Count <= 1)
			{
				return this.firstSequence;
			}
			XmlQueryNodeSequence xmlQueryNodeSequence = new XmlQueryNodeSequence(this.nodeCount);
			while (this.sequencesToMerge.Count != 1)
			{
				IEnumerator<XPathNavigator> enumerator = this.sequencesToMerge[this.sequencesToMerge.Count - 1];
				this.sequencesToMerge.RemoveAt(this.sequencesToMerge.Count - 1);
				xmlQueryNodeSequence.Add(enumerator.Current);
				this.MoveAndInsertSequence(enumerator);
			}
			do
			{
				xmlQueryNodeSequence.Add(this.sequencesToMerge[0].Current);
			}
			while (this.sequencesToMerge[0].MoveNext());
			return xmlQueryNodeSequence;
		}

		private void MoveAndInsertSequence(IEnumerator<XPathNavigator> sequence)
		{
			if (sequence.MoveNext())
			{
				this.InsertSequence(sequence);
			}
		}

		private void InsertSequence(IEnumerator<XPathNavigator> sequence)
		{
			for (int i = this.sequencesToMerge.Count - 1; i >= 0; i--)
			{
				int num = this.runtime.ComparePosition(sequence.Current, this.sequencesToMerge[i].Current);
				if (num == -1)
				{
					this.sequencesToMerge.Insert(i + 1, sequence);
					return;
				}
				if (num == 0 && !sequence.MoveNext())
				{
					return;
				}
			}
			this.sequencesToMerge.Insert(0, sequence);
		}

		private IList<XPathNavigator> firstSequence;

		private List<IEnumerator<XPathNavigator>> sequencesToMerge;

		private int nodeCount;

		private XmlQueryRuntime runtime;
	}
}
