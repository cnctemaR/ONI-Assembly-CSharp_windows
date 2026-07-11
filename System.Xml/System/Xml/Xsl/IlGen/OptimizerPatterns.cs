using System;
using System.Xml.Xsl.Qil;

namespace System.Xml.Xsl.IlGen
{
	internal class OptimizerPatterns : IQilAnnotation
	{
		public static OptimizerPatterns Read(QilNode nd)
		{
			XmlILAnnotation xmlILAnnotation = nd.Annotation as XmlILAnnotation;
			OptimizerPatterns optimizerPatterns = ((xmlILAnnotation != null) ? xmlILAnnotation.Patterns : null);
			if (optimizerPatterns == null)
			{
				if (!nd.XmlType.MaybeMany)
				{
					if (OptimizerPatterns.ZeroOrOneDefault == null)
					{
						optimizerPatterns = new OptimizerPatterns();
						optimizerPatterns.AddPattern(OptimizerPatternName.IsDocOrderDistinct);
						optimizerPatterns.AddPattern(OptimizerPatternName.SameDepth);
						optimizerPatterns.isReadOnly = true;
						OptimizerPatterns.ZeroOrOneDefault = optimizerPatterns;
					}
					else
					{
						optimizerPatterns = OptimizerPatterns.ZeroOrOneDefault;
					}
				}
				else if (nd.XmlType.IsDod)
				{
					if (OptimizerPatterns.DodDefault == null)
					{
						optimizerPatterns = new OptimizerPatterns();
						optimizerPatterns.AddPattern(OptimizerPatternName.IsDocOrderDistinct);
						optimizerPatterns.isReadOnly = true;
						OptimizerPatterns.DodDefault = optimizerPatterns;
					}
					else
					{
						optimizerPatterns = OptimizerPatterns.DodDefault;
					}
				}
				else if (OptimizerPatterns.MaybeManyDefault == null)
				{
					optimizerPatterns = new OptimizerPatterns();
					optimizerPatterns.isReadOnly = true;
					OptimizerPatterns.MaybeManyDefault = optimizerPatterns;
				}
				else
				{
					optimizerPatterns = OptimizerPatterns.MaybeManyDefault;
				}
			}
			return optimizerPatterns;
		}

		public static OptimizerPatterns Write(QilNode nd)
		{
			XmlILAnnotation xmlILAnnotation = XmlILAnnotation.Write(nd);
			OptimizerPatterns optimizerPatterns = xmlILAnnotation.Patterns;
			if (optimizerPatterns == null || optimizerPatterns.isReadOnly)
			{
				optimizerPatterns = new OptimizerPatterns();
				xmlILAnnotation.Patterns = optimizerPatterns;
				if (!nd.XmlType.MaybeMany)
				{
					optimizerPatterns.AddPattern(OptimizerPatternName.IsDocOrderDistinct);
					optimizerPatterns.AddPattern(OptimizerPatternName.SameDepth);
				}
				else if (nd.XmlType.IsDod)
				{
					optimizerPatterns.AddPattern(OptimizerPatternName.IsDocOrderDistinct);
				}
			}
			return optimizerPatterns;
		}

		public static void Inherit(QilNode ndSrc, QilNode ndDst, OptimizerPatternName pattern)
		{
			OptimizerPatterns optimizerPatterns = OptimizerPatterns.Read(ndSrc);
			if (optimizerPatterns.MatchesPattern(pattern))
			{
				OptimizerPatterns optimizerPatterns2 = OptimizerPatterns.Write(ndDst);
				optimizerPatterns2.AddPattern(pattern);
				switch (pattern)
				{
				case OptimizerPatternName.DodReverse:
				case OptimizerPatternName.JoinAndDod:
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.ElementQName, optimizerPatterns.GetArgument(OptimizerPatternArgument.ElementQName));
					return;
				case OptimizerPatternName.EqualityIndex:
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.StepNode, optimizerPatterns.GetArgument(OptimizerPatternArgument.StepNode));
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.StepInput, optimizerPatterns.GetArgument(OptimizerPatternArgument.StepInput));
					return;
				case OptimizerPatternName.FilterAttributeKind:
				case OptimizerPatternName.IsDocOrderDistinct:
				case OptimizerPatternName.IsPositional:
				case OptimizerPatternName.SameDepth:
					break;
				case OptimizerPatternName.FilterContentKind:
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.ElementQName, optimizerPatterns.GetArgument(OptimizerPatternArgument.ElementQName));
					return;
				case OptimizerPatternName.FilterElements:
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.ElementQName, optimizerPatterns.GetArgument(OptimizerPatternArgument.ElementQName));
					return;
				case OptimizerPatternName.MaxPosition:
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.ElementQName, optimizerPatterns.GetArgument(OptimizerPatternArgument.ElementQName));
					return;
				case OptimizerPatternName.Step:
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.StepNode, optimizerPatterns.GetArgument(OptimizerPatternArgument.StepNode));
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.StepInput, optimizerPatterns.GetArgument(OptimizerPatternArgument.StepInput));
					return;
				case OptimizerPatternName.SingleTextRtf:
					optimizerPatterns2.AddArgument(OptimizerPatternArgument.ElementQName, optimizerPatterns.GetArgument(OptimizerPatternArgument.ElementQName));
					break;
				default:
					return;
				}
			}
		}

		public void AddArgument(OptimizerPatternArgument argId, object arg)
		{
			switch (argId)
			{
			case OptimizerPatternArgument.StepNode:
				this.arg0 = arg;
				return;
			case OptimizerPatternArgument.StepInput:
				this.arg1 = arg;
				return;
			case OptimizerPatternArgument.ElementQName:
				this.arg2 = arg;
				return;
			default:
				return;
			}
		}

		public object GetArgument(OptimizerPatternArgument argNum)
		{
			object obj = null;
			switch (argNum)
			{
			case OptimizerPatternArgument.StepNode:
				obj = this.arg0;
				break;
			case OptimizerPatternArgument.StepInput:
				obj = this.arg1;
				break;
			case OptimizerPatternArgument.ElementQName:
				obj = this.arg2;
				break;
			}
			return obj;
		}

		public void AddPattern(OptimizerPatternName pattern)
		{
			this.patterns |= 1 << (int)pattern;
		}

		public bool MatchesPattern(OptimizerPatternName pattern)
		{
			return (this.patterns & (1 << (int)pattern)) != 0;
		}

		public virtual string Name
		{
			get
			{
				return "Patterns";
			}
		}

		public override string ToString()
		{
			string text = "";
			for (int i = 0; i < OptimizerPatterns.PatternCount; i++)
			{
				if (this.MatchesPattern((OptimizerPatternName)i))
				{
					if (text.Length != 0)
					{
						text += ", ";
					}
					string text2 = text;
					OptimizerPatternName optimizerPatternName = (OptimizerPatternName)i;
					text = text2 + optimizerPatternName.ToString();
				}
			}
			return text;
		}

		private static readonly int PatternCount = Enum.GetValues(typeof(OptimizerPatternName)).Length;

		private int patterns;

		private bool isReadOnly;

		private object arg0;

		private object arg1;

		private object arg2;

		private static volatile OptimizerPatterns ZeroOrOneDefault;

		private static volatile OptimizerPatterns MaybeManyDefault;

		private static volatile OptimizerPatterns DodDefault;
	}
}
