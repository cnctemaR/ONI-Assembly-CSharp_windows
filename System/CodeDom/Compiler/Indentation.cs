using System;

namespace System.CodeDom.Compiler
{
	internal sealed class Indentation
	{
		internal Indentation(ExposedTabStringIndentedTextWriter writer, int indent)
		{
			this._writer = writer;
			this._indent = indent;
		}

		internal string IndentationString
		{
			get
			{
				if (this._s == null)
				{
					string tabString = this._writer.TabString;
					switch (this._indent)
					{
					case 0:
						this._s = string.Empty;
						break;
					case 1:
						this._s = tabString;
						break;
					case 2:
						this._s = tabString + tabString;
						break;
					case 3:
						this._s = tabString + tabString + tabString;
						break;
					case 4:
						this._s = tabString + tabString + tabString + tabString;
						break;
					default:
					{
						string[] array = new string[this._indent];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = tabString;
						}
						return string.Concat(array);
					}
					}
				}
				return this._s;
			}
		}

		private readonly ExposedTabStringIndentedTextWriter _writer;

		private readonly int _indent;

		private string _s;
	}
}
