using System;

namespace FileHelpers
{
	[Serializable]
	public sealed class ConvertException : FileHelpersException
	{
		public Type FieldType { get; private set; }

		public string FieldStringValue { get; private set; }

		public string MessageExtra { get; private set; }

		public string MessageOriginal { get; private set; }

		public string FieldName { get; internal set; }

		public int LineNumber { get; internal set; }

		public int ColumnNumber { get; internal set; }

		public ConvertException(string origValue, Type destType)
			: this(origValue, destType, string.Empty)
		{
		}

		public ConvertException(string origValue, Type destType, string extraInfo)
			: this(origValue, destType, string.Empty, -1, -1, extraInfo, null)
		{
		}

		public ConvertException(string origValue, Type destType, string fieldName, int lineNumber, int columnNumber, string extraInfo, Exception innerEx)
			: base(ConvertException.MessageBuilder(origValue, destType, fieldName, lineNumber, columnNumber, extraInfo), innerEx)
		{
			this.MessageOriginal = string.Empty;
			this.FieldStringValue = origValue;
			this.FieldType = destType;
			this.LineNumber = lineNumber;
			this.ColumnNumber = columnNumber;
			this.FieldName = fieldName;
			this.MessageExtra = extraInfo;
			if (origValue != null && destType != null)
			{
				this.MessageOriginal = string.Concat(new string[] { "Error Converting '", origValue, "' to type: '", destType.Name, "'. " });
			}
		}

		private static string MessageBuilder(string origValue, Type destType, string fieldName, int lineNumber, int columnNumber, string extraInfo)
		{
			string text = string.Empty;
			if (lineNumber >= 0)
			{
				text = text + "Line: " + lineNumber.ToString() + ". ";
			}
			if (columnNumber >= 0)
			{
				text = text + "Column: " + columnNumber.ToString() + ". ";
			}
			if (!string.IsNullOrEmpty(fieldName))
			{
				text = text + "Field: " + fieldName + ". ";
			}
			if (origValue != null && destType != null)
			{
				string text2 = text;
				text = string.Concat(new string[] { text2, "Error Converting '", origValue, "' to type: '", destType.Name, "'. " });
			}
			return text + extraInfo;
		}
	}
}
