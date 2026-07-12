using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.AttributedModel
{
	internal class AttributedExportDefinition : ExportDefinition
	{
		public AttributedExportDefinition(AttributedPartCreationInfo partCreationInfo, MemberInfo member, ExportAttribute exportAttribute, Type typeIdentityType, string contractName)
			: base(contractName, null)
		{
			Assumes.NotNull<AttributedPartCreationInfo>(partCreationInfo);
			Assumes.NotNull<MemberInfo>(member);
			Assumes.NotNull<ExportAttribute>(exportAttribute);
			this._partCreationInfo = partCreationInfo;
			this._member = member;
			this._exportAttribute = exportAttribute;
			this._typeIdentityType = typeIdentityType;
		}

		public override IDictionary<string, object> Metadata
		{
			get
			{
				if (this._metadata == null)
				{
					IDictionary<string, object> dictionary;
					this._member.TryExportMetadataForMember(out dictionary);
					string text = (this._exportAttribute.IsContractNameSameAsTypeIdentity() ? this.ContractName : this._member.GetTypeIdentityFromExport(this._typeIdentityType));
					dictionary.Add("ExportTypeIdentity", text);
					IDictionary<string, object> metadata = this._partCreationInfo.GetMetadata();
					if (metadata != null && metadata.ContainsKey("System.ComponentModel.Composition.CreationPolicy"))
					{
						dictionary.Add("System.ComponentModel.Composition.CreationPolicy", metadata["System.ComponentModel.Composition.CreationPolicy"]);
					}
					if (this._typeIdentityType != null && this._member.MemberType != MemberTypes.Method && this._typeIdentityType.ContainsGenericParameters)
					{
						dictionary.Add("System.ComponentModel.Composition.GenericExportParametersOrderMetadataName", GenericServices.GetGenericParametersOrder(this._typeIdentityType));
					}
					this._metadata = dictionary;
				}
				return this._metadata;
			}
		}

		private readonly AttributedPartCreationInfo _partCreationInfo;

		private readonly MemberInfo _member;

		private readonly ExportAttribute _exportAttribute;

		private readonly Type _typeIdentityType;

		private IDictionary<string, object> _metadata;
	}
}
