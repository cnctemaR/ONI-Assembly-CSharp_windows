using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[ComVisible(true)]
	[Serializable]
	public class TextElementEnumerator : IEnumerator
	{
		internal TextElementEnumerator(string str, int startpos)
		{
			this.index = -1;
			this.startpos = startpos;
			this.str = str.Substring(startpos);
			this.element = null;
		}

		public object Current
		{
			get
			{
				if (this.element == null)
				{
					throw new InvalidOperationException();
				}
				return this.element;
			}
		}

		public int ElementIndex
		{
			get
			{
				if (this.element == null)
				{
					throw new InvalidOperationException();
				}
				return this.elementindex + this.startpos;
			}
		}

		public string GetTextElement()
		{
			if (this.element == null)
			{
				throw new InvalidOperationException();
			}
			return this.element;
		}

		public bool MoveNext()
		{
			this.elementindex = this.index + 1;
			if (this.elementindex < this.str.Length)
			{
				this.element = StringInfo.GetNextTextElement(this.str, this.elementindex);
				this.index += this.element.Length;
				return true;
			}
			this.element = null;
			return false;
		}

		public void Reset()
		{
			this.element = null;
			this.index = -1;
		}

		private int index;

		private int elementindex;

		private int startpos;

		private string str;

		private string element;
	}
}
