using System;
using System.Text;

namespace System.Xml.Schema
{
	internal class KeySequence
	{
		internal KeySequence(int dim, int line, int col)
		{
			this.dim = dim;
			this.ks = new TypedObject[dim];
			this.posline = line;
			this.poscol = col;
		}

		public int PosLine
		{
			get
			{
				return this.posline;
			}
		}

		public int PosCol
		{
			get
			{
				return this.poscol;
			}
		}

		public KeySequence(TypedObject[] ks)
		{
			this.ks = ks;
			this.dim = ks.Length;
			this.posline = (this.poscol = 0);
		}

		public object this[int index]
		{
			get
			{
				return this.ks[index];
			}
			set
			{
				this.ks[index] = (TypedObject)value;
			}
		}

		internal bool IsQualified()
		{
			for (int i = 0; i < this.ks.Length; i++)
			{
				if (this.ks[i] == null || this.ks[i].Value == null)
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (this.hashcode != -1)
			{
				return this.hashcode;
			}
			this.hashcode = 0;
			for (int i = 0; i < this.ks.Length; i++)
			{
				this.ks[i].SetDecimal();
				if (this.ks[i].IsDecimal)
				{
					for (int j = 0; j < this.ks[i].Dim; j++)
					{
						this.hashcode += this.ks[i].Dvalue[j].GetHashCode();
					}
				}
				else
				{
					Array array = this.ks[i].Value as Array;
					if (array != null)
					{
						XmlAtomicValue[] array2 = array as XmlAtomicValue[];
						if (array2 != null)
						{
							for (int k = 0; k < array2.Length; k++)
							{
								this.hashcode += ((XmlAtomicValue)array2.GetValue(k)).TypedValue.GetHashCode();
							}
						}
						else
						{
							for (int l = 0; l < ((Array)this.ks[i].Value).Length; l++)
							{
								this.hashcode += ((Array)this.ks[i].Value).GetValue(l).GetHashCode();
							}
						}
					}
					else
					{
						this.hashcode += this.ks[i].Value.GetHashCode();
					}
				}
			}
			return this.hashcode;
		}

		public override bool Equals(object other)
		{
			KeySequence keySequence = (KeySequence)other;
			for (int i = 0; i < this.ks.Length; i++)
			{
				if (!this.ks[i].Equals(keySequence.ks[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.ks[0].ToString());
			for (int i = 1; i < this.ks.Length; i++)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(this.ks[i].ToString());
			}
			return stringBuilder.ToString();
		}

		private TypedObject[] ks;

		private int dim;

		private int hashcode = -1;

		private int posline;

		private int poscol;
	}
}
