using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ReflectionPartCreationInfo : IReflectionPartCreationInfo, ICompositionElement
	{
		public ReflectionPartCreationInfo(Lazy<Type> partType, bool isDisposalRequired, Lazy<IEnumerable<ImportDefinition>> imports, Lazy<IEnumerable<ExportDefinition>> exports, Lazy<IDictionary<string, object>> metadata, ICompositionElement origin)
		{
			Assumes.NotNull<Lazy<Type>>(partType);
			this._partType = partType;
			this._isDisposalRequired = isDisposalRequired;
			this._imports = imports;
			this._exports = exports;
			this._metadata = metadata;
			this._origin = origin;
		}

		public Type GetPartType()
		{
			return this._partType.GetNotNullValue<Type>("type");
		}

		public Lazy<Type> GetLazyPartType()
		{
			return this._partType;
		}

		public ConstructorInfo GetConstructor()
		{
			if (this._constructor == null)
			{
				ConstructorInfo[] array = (from parameterImport in this.GetImports().OfType<ReflectionParameterImportDefinition>()
					select parameterImport.ImportingLazyParameter.Value.Member).OfType<ConstructorInfo>().Distinct<ConstructorInfo>().ToArray<ConstructorInfo>();
				if (array.Length == 1)
				{
					this._constructor = array[0];
				}
				else if (array.Length == 0)
				{
					this._constructor = this.GetPartType().GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
				}
			}
			return this._constructor;
		}

		public bool IsDisposalRequired
		{
			get
			{
				return this._isDisposalRequired;
			}
		}

		public IDictionary<string, object> GetMetadata()
		{
			if (this._metadata == null)
			{
				return null;
			}
			return this._metadata.Value;
		}

		public IEnumerable<ExportDefinition> GetExports()
		{
			if (this._exports == null)
			{
				yield break;
			}
			IEnumerable<ExportDefinition> value = this._exports.Value;
			if (value == null)
			{
				yield break;
			}
			foreach (ExportDefinition exportDefinition in value)
			{
				ReflectionMemberExportDefinition reflectionMemberExportDefinition = exportDefinition as ReflectionMemberExportDefinition;
				if (reflectionMemberExportDefinition == null)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_InvalidExportDefinition, exportDefinition.GetType()));
				}
				yield return reflectionMemberExportDefinition;
			}
			IEnumerator<ExportDefinition> enumerator = null;
			yield break;
			yield break;
		}

		public IEnumerable<ImportDefinition> GetImports()
		{
			if (this._imports == null)
			{
				yield break;
			}
			IEnumerable<ImportDefinition> value = this._imports.Value;
			if (value == null)
			{
				yield break;
			}
			foreach (ImportDefinition importDefinition in value)
			{
				ReflectionImportDefinition reflectionImportDefinition = importDefinition as ReflectionImportDefinition;
				if (reflectionImportDefinition == null)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_InvalidMemberImportDefinition, importDefinition.GetType()));
				}
				yield return reflectionImportDefinition;
			}
			IEnumerator<ImportDefinition> enumerator = null;
			yield break;
			yield break;
		}

		public string DisplayName
		{
			get
			{
				return this.GetPartType().GetDisplayName();
			}
		}

		public ICompositionElement Origin
		{
			get
			{
				return this._origin;
			}
		}

		private readonly Lazy<Type> _partType;

		private readonly Lazy<IEnumerable<ImportDefinition>> _imports;

		private readonly Lazy<IEnumerable<ExportDefinition>> _exports;

		private readonly Lazy<IDictionary<string, object>> _metadata;

		private readonly ICompositionElement _origin;

		private ConstructorInfo _constructor;

		private bool _isDisposalRequired;
	}
}
