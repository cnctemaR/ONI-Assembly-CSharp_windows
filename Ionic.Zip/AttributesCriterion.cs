using System;
using System.IO;
using System.Text;
using Ionic.Zip;

namespace Ionic
{
	internal class AttributesCriterion : SelectionCriterion
	{
		internal string AttributeString
		{
			get
			{
				string text = "";
				if ((this._Attributes & FileAttributes.Hidden) != (FileAttributes)0)
				{
					text += "H";
				}
				if ((this._Attributes & FileAttributes.System) != (FileAttributes)0)
				{
					text += "S";
				}
				if ((this._Attributes & FileAttributes.ReadOnly) != (FileAttributes)0)
				{
					text += "R";
				}
				if ((this._Attributes & FileAttributes.Archive) != (FileAttributes)0)
				{
					text += "A";
				}
				if ((this._Attributes & FileAttributes.ReparsePoint) != (FileAttributes)0)
				{
					text += "L";
				}
				if ((this._Attributes & FileAttributes.NotContentIndexed) != (FileAttributes)0)
				{
					text += "I";
				}
				return text;
			}
			set
			{
				this._Attributes = FileAttributes.Normal;
				string text = value.ToUpper();
				int i = 0;
				while (i < text.Length)
				{
					char c = text[i];
					if (c <= 'L')
					{
						if (c != 'A')
						{
							switch (c)
							{
							case 'H':
								if ((this._Attributes & FileAttributes.Hidden) != (FileAttributes)0)
								{
									throw new ArgumentException(string.Format("Repeated flag. ({0})", c), "value");
								}
								this._Attributes |= FileAttributes.Hidden;
								break;
							case 'I':
								if ((this._Attributes & FileAttributes.NotContentIndexed) != (FileAttributes)0)
								{
									throw new ArgumentException(string.Format("Repeated flag. ({0})", c), "value");
								}
								this._Attributes |= FileAttributes.NotContentIndexed;
								break;
							case 'J':
							case 'K':
								goto IL_01BB;
							case 'L':
								if ((this._Attributes & FileAttributes.ReparsePoint) != (FileAttributes)0)
								{
									throw new ArgumentException(string.Format("Repeated flag. ({0})", c), "value");
								}
								this._Attributes |= FileAttributes.ReparsePoint;
								break;
							default:
								goto IL_01BB;
							}
						}
						else
						{
							if ((this._Attributes & FileAttributes.Archive) != (FileAttributes)0)
							{
								throw new ArgumentException(string.Format("Repeated flag. ({0})", c), "value");
							}
							this._Attributes |= FileAttributes.Archive;
						}
					}
					else if (c != 'R')
					{
						if (c != 'S')
						{
							goto IL_01BB;
						}
						if ((this._Attributes & FileAttributes.System) != (FileAttributes)0)
						{
							throw new ArgumentException(string.Format("Repeated flag. ({0})", c), "value");
						}
						this._Attributes |= FileAttributes.System;
					}
					else
					{
						if ((this._Attributes & FileAttributes.ReadOnly) != (FileAttributes)0)
						{
							throw new ArgumentException(string.Format("Repeated flag. ({0})", c), "value");
						}
						this._Attributes |= FileAttributes.ReadOnly;
					}
					i++;
					continue;
					IL_01BB:
					throw new ArgumentException(value);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("attributes ").Append(EnumUtil.GetDescription(this.Operator)).Append(" ")
				.Append(this.AttributeString);
			return stringBuilder.ToString();
		}

		private bool _EvaluateOne(FileAttributes fileAttrs, FileAttributes criterionAttrs)
		{
			return (this._Attributes & criterionAttrs) != criterionAttrs || (fileAttrs & criterionAttrs) == criterionAttrs;
		}

		internal override bool Evaluate(string filename)
		{
			if (Directory.Exists(filename))
			{
				return this.Operator != ComparisonOperator.EqualTo;
			}
			FileAttributes attributes = File.GetAttributes(filename);
			return this._Evaluate(attributes);
		}

		private bool _Evaluate(FileAttributes fileAttrs)
		{
			bool flag = this._EvaluateOne(fileAttrs, FileAttributes.Hidden);
			if (flag)
			{
				flag = this._EvaluateOne(fileAttrs, FileAttributes.System);
			}
			if (flag)
			{
				flag = this._EvaluateOne(fileAttrs, FileAttributes.ReadOnly);
			}
			if (flag)
			{
				flag = this._EvaluateOne(fileAttrs, FileAttributes.Archive);
			}
			if (flag)
			{
				flag = this._EvaluateOne(fileAttrs, FileAttributes.NotContentIndexed);
			}
			if (flag)
			{
				flag = this._EvaluateOne(fileAttrs, FileAttributes.ReparsePoint);
			}
			if (this.Operator != ComparisonOperator.EqualTo)
			{
				flag = !flag;
			}
			return flag;
		}

		internal override bool Evaluate(ZipEntry entry)
		{
			FileAttributes attributes = entry.Attributes;
			return this._Evaluate(attributes);
		}

		private FileAttributes _Attributes;

		internal ComparisonOperator Operator;
	}
}
