using System;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Xml.Xsl
{
	[Serializable]
	internal class XslLoadException : XslTransformException
	{
		protected XslLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if ((bool)info.GetValue("hasLineInfo", typeof(bool)))
			{
				string text = (string)info.GetValue("Uri", typeof(string));
				int num = (int)info.GetValue("StartLine", typeof(int));
				int num2 = (int)info.GetValue("StartPos", typeof(int));
				int num3 = (int)info.GetValue("EndLine", typeof(int));
				int num4 = (int)info.GetValue("EndPos", typeof(int));
				this.lineInfo = new SourceLineInfo(text, num, num2, num3, num4);
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hasLineInfo", this.lineInfo != null);
			if (this.lineInfo != null)
			{
				info.AddValue("Uri", this.lineInfo.Uri);
				info.AddValue("StartLine", this.lineInfo.Start.Line);
				info.AddValue("StartPos", this.lineInfo.Start.Pos);
				info.AddValue("EndLine", this.lineInfo.End.Line);
				info.AddValue("EndPos", this.lineInfo.End.Pos);
			}
		}

		internal XslLoadException(string res, params string[] args)
			: base(null, res, args)
		{
		}

		internal XslLoadException(Exception inner, ISourceLineInfo lineInfo)
			: base(inner, "XSLT compile error.", null)
		{
			this.SetSourceLineInfo(lineInfo);
		}

		internal XslLoadException(CompilerError error)
			: base("{0}", new string[] { error.ErrorText })
		{
			int line = error.Line;
			int num = error.Column;
			if (line == 0)
			{
				num = 0;
			}
			else if (num == 0)
			{
				num = 1;
			}
			this.SetSourceLineInfo(new SourceLineInfo(error.FileName, line, num, line, num));
		}

		internal void SetSourceLineInfo(ISourceLineInfo lineInfo)
		{
			this.lineInfo = lineInfo;
		}

		public override string SourceUri
		{
			get
			{
				if (this.lineInfo == null)
				{
					return null;
				}
				return this.lineInfo.Uri;
			}
		}

		public override int LineNumber
		{
			get
			{
				if (this.lineInfo == null)
				{
					return 0;
				}
				return this.lineInfo.Start.Line;
			}
		}

		public override int LinePosition
		{
			get
			{
				if (this.lineInfo == null)
				{
					return 0;
				}
				return this.lineInfo.Start.Pos;
			}
		}

		private static string AppendLineInfoMessage(string message, ISourceLineInfo lineInfo)
		{
			if (lineInfo != null)
			{
				string fileName = SourceLineInfo.GetFileName(lineInfo.Uri);
				string text = XslTransformException.CreateMessage("An error occurred at {0}({1},{2}).", new string[]
				{
					fileName,
					lineInfo.Start.Line.ToString(CultureInfo.InvariantCulture),
					lineInfo.Start.Pos.ToString(CultureInfo.InvariantCulture)
				});
				if (text != null && text.Length > 0)
				{
					if (message.Length > 0 && !XmlCharType.Instance.IsWhiteSpace(message[message.Length - 1]))
					{
						message += " ";
					}
					message += text;
				}
			}
			return message;
		}

		internal static string CreateMessage(ISourceLineInfo lineInfo, string res, params string[] args)
		{
			return XslLoadException.AppendLineInfoMessage(XslTransformException.CreateMessage(res, args), lineInfo);
		}

		internal override string FormatDetailedMessage()
		{
			return XslLoadException.AppendLineInfoMessage(this.Message, this.lineInfo);
		}

		private ISourceLineInfo lineInfo;
	}
}
