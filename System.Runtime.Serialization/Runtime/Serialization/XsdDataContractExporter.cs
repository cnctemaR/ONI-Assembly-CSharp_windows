using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization.Diagnostics;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	public class XsdDataContractExporter
	{
		public XsdDataContractExporter()
		{
		}

		public XsdDataContractExporter(XmlSchemaSet schemas)
		{
			this.schemas = schemas;
		}

		public ExportOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public XmlSchemaSet Schemas
		{
			get
			{
				XmlSchemaSet schemaSet = this.GetSchemaSet();
				SchemaImporter.CompileSchemaSet(schemaSet);
				return schemaSet;
			}
		}

		private XmlSchemaSet GetSchemaSet()
		{
			if (this.schemas == null)
			{
				this.schemas = new XmlSchemaSet();
				this.schemas.XmlResolver = null;
			}
			return this.schemas;
		}

		private DataContractSet DataContractSet
		{
			get
			{
				if (this.dataContractSet == null)
				{
					this.dataContractSet = new DataContractSet((this.Options == null) ? null : this.Options.GetSurrogate());
				}
				return this.dataContractSet;
			}
		}

		private void TraceExportBegin()
		{
			if (DiagnosticUtility.ShouldTraceInformation)
			{
				TraceUtility.Trace(TraceEventType.Information, 196616, global::System.Runtime.Serialization.SR.GetString("XSD export begins"));
			}
		}

		private void TraceExportEnd()
		{
			if (DiagnosticUtility.ShouldTraceInformation)
			{
				TraceUtility.Trace(TraceEventType.Information, 196617, global::System.Runtime.Serialization.SR.GetString("XSD export ends"));
			}
		}

		private void TraceExportError(Exception exception)
		{
			if (DiagnosticUtility.ShouldTraceError)
			{
				TraceUtility.Trace(TraceEventType.Error, 196620, global::System.Runtime.Serialization.SR.GetString("XSD export error"), null, exception);
			}
		}

		public void Export(ICollection<Assembly> assemblies)
		{
			if (assemblies == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
			}
			this.TraceExportBegin();
			DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
			try
			{
				foreach (Assembly assembly in assemblies)
				{
					if (assembly == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Cannot export null assembly.", new object[] { "assemblies" })));
					}
					Type[] types = assembly.GetTypes();
					for (int i = 0; i < types.Length; i++)
					{
						this.CheckAndAddType(types[i]);
					}
				}
				this.Export();
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(ex);
				throw;
			}
			this.TraceExportEnd();
		}

		public void Export(ICollection<Type> types)
		{
			if (types == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));
			}
			this.TraceExportBegin();
			DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
			try
			{
				foreach (Type type in types)
				{
					if (type == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Cannot export null type.", new object[] { "types" })));
					}
					this.AddType(type);
				}
				this.Export();
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(ex);
				throw;
			}
			this.TraceExportEnd();
		}

		public void Export(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			this.TraceExportBegin();
			DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
			try
			{
				this.AddType(type);
				this.Export();
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(ex);
				throw;
			}
			this.TraceExportEnd();
		}

		public XmlQualifiedName GetSchemaTypeName(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			type = this.GetSurrogatedType(type);
			DataContract dataContract = DataContract.GetDataContract(type);
			DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
			XmlDataContract xmlDataContract = dataContract as XmlDataContract;
			if (xmlDataContract != null && xmlDataContract.IsAnonymous)
			{
				return XmlQualifiedName.Empty;
			}
			return dataContract.StableName;
		}

		public XmlSchemaType GetSchemaType(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			type = this.GetSurrogatedType(type);
			DataContract dataContract = DataContract.GetDataContract(type);
			DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
			XmlDataContract xmlDataContract = dataContract as XmlDataContract;
			if (xmlDataContract != null && xmlDataContract.IsAnonymous)
			{
				return xmlDataContract.XsdType;
			}
			return null;
		}

		public XmlQualifiedName GetRootElementName(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			type = this.GetSurrogatedType(type);
			DataContract dataContract = DataContract.GetDataContract(type);
			DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
			if (dataContract.HasRoot)
			{
				return new XmlQualifiedName(dataContract.TopLevelElementName.Value, dataContract.TopLevelElementNamespace.Value);
			}
			return null;
		}

		private Type GetSurrogatedType(Type type)
		{
			IDataContractSurrogate surrogate;
			if (this.options != null && (surrogate = this.Options.GetSurrogate()) != null)
			{
				type = DataContractSurrogateCaller.GetDataContractType(surrogate, type);
			}
			return type;
		}

		private void CheckAndAddType(Type type)
		{
			type = this.GetSurrogatedType(type);
			if (!type.ContainsGenericParameters && DataContract.IsTypeSerializable(type))
			{
				this.AddType(type);
			}
		}

		private void AddType(Type type)
		{
			this.DataContractSet.Add(type);
		}

		private void Export()
		{
			this.AddKnownTypes();
			new SchemaExporter(this.GetSchemaSet(), this.DataContractSet).Export();
		}

		private void AddKnownTypes()
		{
			if (this.Options != null)
			{
				Collection<Type> knownTypes = this.Options.KnownTypes;
				if (knownTypes != null)
				{
					for (int i = 0; i < knownTypes.Count; i++)
					{
						Type type = knownTypes[i];
						if (type == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Cannot export null known type.")));
						}
						this.AddType(type);
					}
				}
			}
		}

		public bool CanExport(ICollection<Assembly> assemblies)
		{
			if (assemblies == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
			}
			DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
			bool flag;
			try
			{
				foreach (Assembly assembly in assemblies)
				{
					if (assembly == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Cannot export null assembly.", new object[] { "assemblies" })));
					}
					Type[] types = assembly.GetTypes();
					for (int i = 0; i < types.Length; i++)
					{
						this.CheckAndAddType(types[i]);
					}
				}
				this.AddKnownTypes();
				flag = true;
			}
			catch (InvalidDataContractException)
			{
				this.dataContractSet = dataContractSet;
				flag = false;
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(ex);
				throw;
			}
			return flag;
		}

		public bool CanExport(ICollection<Type> types)
		{
			if (types == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));
			}
			DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
			bool flag;
			try
			{
				foreach (Type type in types)
				{
					if (type == null)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Cannot export null type.", new object[] { "types" })));
					}
					this.AddType(type);
				}
				this.AddKnownTypes();
				flag = true;
			}
			catch (InvalidDataContractException)
			{
				this.dataContractSet = dataContractSet;
				flag = false;
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(ex);
				throw;
			}
			return flag;
		}

		public bool CanExport(Type type)
		{
			if (type == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
			}
			DataContractSet dataContractSet = ((this.dataContractSet == null) ? null : new DataContractSet(this.dataContractSet));
			bool flag;
			try
			{
				this.AddType(type);
				this.AddKnownTypes();
				flag = true;
			}
			catch (InvalidDataContractException)
			{
				this.dataContractSet = dataContractSet;
				flag = false;
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				this.dataContractSet = dataContractSet;
				this.TraceExportError(ex);
				throw;
			}
			return flag;
		}

		private ExportOptions options;

		private XmlSchemaSet schemas;

		private DataContractSet dataContractSet;
	}
}
