using System;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace System.CodeDom.Compiler
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public class IndentedTextWriter : TextWriter
	{
		public IndentedTextWriter(TextWriter writer)
		{
			this.writer = writer;
			this.tabString = "    ";
			this.newline = true;
		}

		public IndentedTextWriter(TextWriter writer, string tabString)
		{
			this.writer = writer;
			this.tabString = tabString;
			this.newline = true;
		}

		public override Encoding Encoding
		{
			get
			{
				return this.writer.Encoding;
			}
		}

		public int Indent
		{
			get
			{
				return this.indent;
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				this.indent = value;
			}
		}

		public TextWriter InnerWriter
		{
			get
			{
				return this.writer;
			}
		}

		public override string NewLine
		{
			get
			{
				return this.writer.NewLine;
			}
			set
			{
				this.writer.NewLine = value;
			}
		}

		public override void Close()
		{
			this.writer.Close();
		}

		public override void Flush()
		{
			this.writer.Flush();
		}

		public override void Write(bool value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(char value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(char[] value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(double value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(int value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(long value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(object value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(float value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(string value)
		{
			this.OutputTabs();
			this.writer.Write(value);
		}

		public override void Write(string format, object arg)
		{
			this.OutputTabs();
			this.writer.Write(format, arg);
		}

		public override void Write(string format, params object[] args)
		{
			this.OutputTabs();
			this.writer.Write(format, args);
		}

		public override void Write(char[] buffer, int index, int count)
		{
			this.OutputTabs();
			this.writer.Write(buffer, index, count);
		}

		public override void Write(string format, object arg0, object arg1)
		{
			this.OutputTabs();
			this.writer.Write(format, arg0, arg1);
		}

		public override void WriteLine()
		{
			this.OutputTabs();
			this.writer.WriteLine();
			this.newline = true;
		}

		public override void WriteLine(bool value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(char value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(char[] value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(double value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(int value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(long value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(object value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(float value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(string value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		[CLSCompliant(false)]
		public override void WriteLine(uint value)
		{
			this.OutputTabs();
			this.writer.WriteLine(value);
			this.newline = true;
		}

		public override void WriteLine(string format, object arg)
		{
			this.OutputTabs();
			this.writer.WriteLine(format, arg);
			this.newline = true;
		}

		public override void WriteLine(string format, params object[] args)
		{
			this.OutputTabs();
			this.writer.WriteLine(format, args);
			this.newline = true;
		}

		public override void WriteLine(char[] buffer, int index, int count)
		{
			this.OutputTabs();
			this.writer.WriteLine(buffer, index, count);
			this.newline = true;
		}

		public override void WriteLine(string format, object arg0, object arg1)
		{
			this.OutputTabs();
			this.writer.WriteLine(format, arg0, arg1);
			this.newline = true;
		}

		public void WriteLineNoTabs(string value)
		{
			this.writer.WriteLine(value);
			this.newline = true;
		}

		protected virtual void OutputTabs()
		{
			if (this.newline)
			{
				for (int i = 0; i < this.indent; i++)
				{
					this.writer.Write(this.tabString);
				}
				this.newline = false;
			}
		}

		public const string DefaultTabString = "    ";

		private TextWriter writer;

		private string tabString;

		private int indent;

		private bool newline;
	}
}
