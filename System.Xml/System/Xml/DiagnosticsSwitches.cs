using System;
using System.Diagnostics;

namespace System.Xml
{
	internal static class DiagnosticsSwitches
	{
		public static BooleanSwitch XmlSchemaContentModel
		{
			get
			{
				if (DiagnosticsSwitches.xmlSchemaContentModel == null)
				{
					DiagnosticsSwitches.xmlSchemaContentModel = new BooleanSwitch("XmlSchemaContentModel", "Enable tracing for the XmlSchema content model.");
				}
				return DiagnosticsSwitches.xmlSchemaContentModel;
			}
		}

		public static TraceSwitch XmlSchema
		{
			get
			{
				if (DiagnosticsSwitches.xmlSchema == null)
				{
					DiagnosticsSwitches.xmlSchema = new TraceSwitch("XmlSchema", "Enable tracing for the XmlSchema class.");
				}
				return DiagnosticsSwitches.xmlSchema;
			}
		}

		public static BooleanSwitch KeepTempFiles
		{
			get
			{
				if (DiagnosticsSwitches.keepTempFiles == null)
				{
					DiagnosticsSwitches.keepTempFiles = new BooleanSwitch("XmlSerialization.Compilation", "Keep XmlSerialization generated (temp) files.");
				}
				return DiagnosticsSwitches.keepTempFiles;
			}
		}

		public static BooleanSwitch PregenEventLog
		{
			get
			{
				if (DiagnosticsSwitches.pregenEventLog == null)
				{
					DiagnosticsSwitches.pregenEventLog = new BooleanSwitch("XmlSerialization.PregenEventLog", "Log failures while loading pre-generated XmlSerialization assembly.");
				}
				return DiagnosticsSwitches.pregenEventLog;
			}
		}

		public static TraceSwitch XmlSerialization
		{
			get
			{
				if (DiagnosticsSwitches.xmlSerialization == null)
				{
					DiagnosticsSwitches.xmlSerialization = new TraceSwitch("XmlSerialization", "Enable tracing for the System.Xml.Serialization component.");
				}
				return DiagnosticsSwitches.xmlSerialization;
			}
		}

		public static TraceSwitch XslTypeInference
		{
			get
			{
				if (DiagnosticsSwitches.xslTypeInference == null)
				{
					DiagnosticsSwitches.xslTypeInference = new TraceSwitch("XslTypeInference", "Enable tracing for the XSLT type inference algorithm.");
				}
				return DiagnosticsSwitches.xslTypeInference;
			}
		}

		public static BooleanSwitch NonRecursiveTypeLoading
		{
			get
			{
				if (DiagnosticsSwitches.nonRecursiveTypeLoading == null)
				{
					DiagnosticsSwitches.nonRecursiveTypeLoading = new BooleanSwitch("XmlSerialization.NonRecursiveTypeLoading", "Turn on non-recursive algorithm generating XmlMappings for CLR types.");
				}
				return DiagnosticsSwitches.nonRecursiveTypeLoading;
			}
		}

		private static volatile BooleanSwitch xmlSchemaContentModel;

		private static volatile TraceSwitch xmlSchema;

		private static volatile BooleanSwitch keepTempFiles;

		private static volatile BooleanSwitch pregenEventLog;

		private static volatile TraceSwitch xmlSerialization;

		private static volatile TraceSwitch xslTypeInference;

		private static volatile BooleanSwitch nonRecursiveTypeLoading;
	}
}
