using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.CodeDom.Compiler
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class CodeGeneratorOptions
	{
		public CodeGeneratorOptions()
		{
			this.properties = new global::System.Collections.Specialized.ListDictionary();
		}

		public bool BlankLinesBetweenMembers
		{
			get
			{
				object obj = this.properties["BlankLinesBetweenMembers"];
				return obj == null || (bool)obj;
			}
			set
			{
				this.properties["BlankLinesBetweenMembers"] = value;
			}
		}

		public string BracingStyle
		{
			get
			{
				object obj = this.properties["BracingStyle"];
				return (obj != null) ? ((string)obj) : "Block";
			}
			set
			{
				this.properties["BracingStyle"] = value;
			}
		}

		public bool ElseOnClosing
		{
			get
			{
				object obj = this.properties["ElseOnClosing"];
				return obj != null && (bool)obj;
			}
			set
			{
				this.properties["ElseOnClosing"] = value;
			}
		}

		public string IndentString
		{
			get
			{
				object obj = this.properties["IndentString"];
				return (obj != null) ? ((string)obj) : "    ";
			}
			set
			{
				this.properties["IndentString"] = value;
			}
		}

		public object this[string index]
		{
			get
			{
				return this.properties[index];
			}
			set
			{
				this.properties[index] = value;
			}
		}

		[ComVisible(false)]
		public bool VerbatimOrder
		{
			get
			{
				object obj = this.properties["VerbatimOrder"];
				return obj != null && (bool)obj;
			}
			set
			{
				this.properties["VerbatimOrder"] = value;
			}
		}

		private IDictionary properties;
	}
}
