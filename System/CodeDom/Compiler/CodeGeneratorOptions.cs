using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.CodeDom.Compiler
{
	public class CodeGeneratorOptions
	{
		public object this[string index]
		{
			get
			{
				return this._options[index];
			}
			set
			{
				this._options[index] = value;
			}
		}

		public string IndentString
		{
			get
			{
				object obj = this._options["IndentString"];
				if (obj == null)
				{
					return "    ";
				}
				return (string)obj;
			}
			set
			{
				this._options["IndentString"] = value;
			}
		}

		public string BracingStyle
		{
			get
			{
				object obj = this._options["BracingStyle"];
				if (obj == null)
				{
					return "Block";
				}
				return (string)obj;
			}
			set
			{
				this._options["BracingStyle"] = value;
			}
		}

		public bool ElseOnClosing
		{
			get
			{
				object obj = this._options["ElseOnClosing"];
				return obj != null && (bool)obj;
			}
			set
			{
				this._options["ElseOnClosing"] = value;
			}
		}

		public bool BlankLinesBetweenMembers
		{
			get
			{
				object obj = this._options["BlankLinesBetweenMembers"];
				return obj == null || (bool)obj;
			}
			set
			{
				this._options["BlankLinesBetweenMembers"] = value;
			}
		}

		public bool VerbatimOrder
		{
			get
			{
				object obj = this._options["VerbatimOrder"];
				return obj != null && (bool)obj;
			}
			set
			{
				this._options["VerbatimOrder"] = value;
			}
		}

		private readonly IDictionary _options = new ListDictionary();
	}
}
