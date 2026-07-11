using System;
using System.Reflection.Emit;

namespace System.Xml.Serialization
{
	internal class IfState
	{
		internal Label EndIf
		{
			get
			{
				return this.endIf;
			}
			set
			{
				this.endIf = value;
			}
		}

		internal Label ElseBegin
		{
			get
			{
				return this.elseBegin;
			}
			set
			{
				this.elseBegin = value;
			}
		}

		private Label elseBegin;

		private Label endIf;
	}
}
