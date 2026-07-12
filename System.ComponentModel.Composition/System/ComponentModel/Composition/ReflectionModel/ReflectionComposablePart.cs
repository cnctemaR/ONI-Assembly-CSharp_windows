using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ReflectionComposablePart : ComposablePart, ICompositionElement
	{
		public ReflectionComposablePart(ReflectionComposablePartDefinition definition)
		{
			Requires.NotNull<ReflectionComposablePartDefinition>(definition, "definition");
			this._definition = definition;
		}

		public ReflectionComposablePart(ReflectionComposablePartDefinition definition, object attributedPart)
		{
			Requires.NotNull<ReflectionComposablePartDefinition>(definition, "definition");
			Requires.NotNull<object>(attributedPart, "attributedPart");
			this._definition = definition;
			if (attributedPart is ValueType)
			{
				throw new ArgumentException(Strings.ArgumentValueType, "attributedPart");
			}
			this._cachedInstance = attributedPart;
		}

		protected virtual void EnsureRunning()
		{
		}

		protected void RequiresRunning()
		{
			this.EnsureRunning();
		}

		protected virtual void ReleaseInstanceIfNecessary(object instance)
		{
		}

		protected object CachedInstance
		{
			get
			{
				object @lock = this._lock;
				object cachedInstance;
				lock (@lock)
				{
					cachedInstance = this._cachedInstance;
				}
				return cachedInstance;
			}
		}

		public ReflectionComposablePartDefinition Definition
		{
			get
			{
				this.RequiresRunning();
				return this._definition;
			}
		}

		public override IDictionary<string, object> Metadata
		{
			get
			{
				this.RequiresRunning();
				return this.Definition.Metadata;
			}
		}

		public sealed override IEnumerable<ImportDefinition> ImportDefinitions
		{
			get
			{
				this.RequiresRunning();
				return this.Definition.ImportDefinitions;
			}
		}

		public sealed override IEnumerable<ExportDefinition> ExportDefinitions
		{
			get
			{
				this.RequiresRunning();
				return this.Definition.ExportDefinitions;
			}
		}

		string ICompositionElement.DisplayName
		{
			get
			{
				return this.GetDisplayName();
			}
		}

		ICompositionElement ICompositionElement.Origin
		{
			get
			{
				return this.Definition;
			}
		}

		public override object GetExportedValue(ExportDefinition definition)
		{
			this.RequiresRunning();
			Requires.NotNull<ExportDefinition>(definition, "definition");
			ExportingMember exportingMember = null;
			object @lock = this._lock;
			lock (@lock)
			{
				exportingMember = this.GetExportingMemberFromDefinition(definition);
				if (exportingMember == null)
				{
					throw ExceptionBuilder.CreateExportDefinitionNotOnThisComposablePart("definition");
				}
				this.EnsureGettable();
			}
			return this.GetExportedValue(exportingMember);
		}

		public override void SetImport(ImportDefinition definition, IEnumerable<Export> exports)
		{
			this.RequiresRunning();
			Requires.NotNull<ImportDefinition>(definition, "definition");
			Requires.NotNull<IEnumerable<Export>>(exports, "exports");
			ImportingItem importingItemFromDefinition = this.GetImportingItemFromDefinition(definition);
			if (importingItemFromDefinition == null)
			{
				throw ExceptionBuilder.CreateImportDefinitionNotOnThisComposablePart("definition");
			}
			this.EnsureSettable(definition);
			Export[] array = exports.AsArray<Export>();
			ReflectionComposablePart.EnsureCardinality(definition, array);
			this.SetImport(importingItemFromDefinition, array);
		}

		public override void Activate()
		{
			this.RequiresRunning();
			this.SetNonPrerequisiteImports();
			this.NotifyImportSatisfied();
			object @lock = this._lock;
			lock (@lock)
			{
				this._initialCompositionComplete = true;
			}
		}

		public override string ToString()
		{
			return this.GetDisplayName();
		}

		private object GetExportedValue(ExportingMember member)
		{
			object obj = null;
			if (member.RequiresInstance)
			{
				obj = this.GetInstanceActivatingIfNeeded();
			}
			return member.GetExportedValue(obj, this._lock);
		}

		private void SetImport(ImportingItem item, Export[] exports)
		{
			object obj = item.CastExportsToImportType(exports);
			object @lock = this._lock;
			lock (@lock)
			{
				this._invokeImportsSatisfied = true;
				this._importValues[item.Definition] = obj;
			}
		}

		private object GetInstanceActivatingIfNeeded()
		{
			if (this._cachedInstance != null)
			{
				return this._cachedInstance;
			}
			ConstructorInfo constructorInfo = null;
			object[] array = null;
			object obj = this._lock;
			lock (obj)
			{
				if (!this.RequiresActivation())
				{
					return null;
				}
				constructorInfo = this.Definition.GetConstructor();
				if (constructorInfo == null)
				{
					throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_PartConstructorMissing, this.Definition.GetPartType().FullName), this.Definition.ToElement());
				}
				array = this.GetConstructorArguments();
			}
			object obj2 = this.CreateInstance(constructorInfo, array);
			this.SetPrerequisiteImports();
			obj = this._lock;
			lock (obj)
			{
				if (this._cachedInstance == null)
				{
					this._cachedInstance = obj2;
					obj2 = null;
				}
			}
			if (obj2 == null)
			{
				this.ReleaseInstanceIfNecessary(obj2);
			}
			return this._cachedInstance;
		}

		private object[] GetConstructorArguments()
		{
			ReflectionParameterImportDefinition[] array = this.ImportDefinitions.OfType<ReflectionParameterImportDefinition>().ToArray<ReflectionParameterImportDefinition>();
			object[] arguments = new object[array.Length];
			this.UseImportedValues<ReflectionParameterImportDefinition>(array, delegate(ImportingItem import, ReflectionParameterImportDefinition definition, object value)
			{
				if (definition.Cardinality == ImportCardinality.ZeroOrMore && !import.ImportType.IsAssignableCollectionType)
				{
					throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportManyOnParameterCanOnlyBeAssigned, this.Definition.GetPartType().FullName, definition.ImportingLazyParameter.Value.Name), this.Definition.ToElement());
				}
				arguments[definition.ImportingLazyParameter.Value.Position] = value;
			}, true);
			return arguments;
		}

		private bool RequiresActivation()
		{
			return this.ImportDefinitions.Any<ImportDefinition>() || this.ExportDefinitions.Any<ExportDefinition>((ExportDefinition definition) => this.GetExportingMemberFromDefinition(definition).RequiresInstance);
		}

		private void EnsureGettable()
		{
			if (this._initialCompositionComplete)
			{
				return;
			}
			foreach (ImportDefinition importDefinition in this.ImportDefinitions.Where<ImportDefinition>((ImportDefinition definition) => definition.IsPrerequisite))
			{
				if (!this._importValues.ContainsKey(importDefinition))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.InvalidOperation_GetExportedValueBeforePrereqImportSet, importDefinition.ToElement().DisplayName));
				}
			}
		}

		private void EnsureSettable(ImportDefinition definition)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._initialCompositionComplete && !definition.IsRecomposable)
				{
					throw new InvalidOperationException(Strings.InvalidOperation_DefinitionCannotBeRecomposed);
				}
			}
		}

		private static void EnsureCardinality(ImportDefinition definition, Export[] exports)
		{
			Requires.NullOrNotNullElements<Export>(exports, "exports");
			ExportCardinalityCheckResult exportCardinalityCheckResult = ExportServices.CheckCardinality<Export>(definition, exports);
			if (exportCardinalityCheckResult == ExportCardinalityCheckResult.NoExports)
			{
				throw new ArgumentException(Strings.Argument_ExportsEmpty, "exports");
			}
			if (exportCardinalityCheckResult != ExportCardinalityCheckResult.TooManyExports)
			{
				Assumes.IsTrue(exportCardinalityCheckResult == ExportCardinalityCheckResult.Match);
				return;
			}
			throw new ArgumentException(Strings.Argument_ExportsTooMany, "exports");
		}

		private object CreateInstance(ConstructorInfo constructor, object[] arguments)
		{
			Exception ex = null;
			object obj = null;
			try
			{
				obj = constructor.SafeInvoke(arguments);
			}
			catch (TypeInitializationException ex)
			{
			}
			catch (TargetInvocationException ex2)
			{
				ex = ex2.InnerException;
			}
			if (ex != null)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_PartConstructorThrewException, this.Definition.GetPartType().FullName), this.Definition.ToElement(), ex);
			}
			return obj;
		}

		private void SetNonPrerequisiteImports()
		{
			IEnumerable<ImportDefinition> enumerable = this.ImportDefinitions.Where<ImportDefinition>((ImportDefinition import) => !import.IsPrerequisite);
			this.UseImportedValues<ImportDefinition>(enumerable, new Action<ImportingItem, ImportDefinition, object>(this.SetExportedValueForImport), false);
		}

		private void SetPrerequisiteImports()
		{
			IEnumerable<ImportDefinition> enumerable = this.ImportDefinitions.Where<ImportDefinition>((ImportDefinition import) => import.IsPrerequisite);
			this.UseImportedValues<ImportDefinition>(enumerable, new Action<ImportingItem, ImportDefinition, object>(this.SetExportedValueForImport), false);
		}

		private void SetExportedValueForImport(ImportingItem import, ImportDefinition definition, object value)
		{
			ImportingMember importingMember = (ImportingMember)import;
			object instanceActivatingIfNeeded = this.GetInstanceActivatingIfNeeded();
			importingMember.SetExportedValue(instanceActivatingIfNeeded, value);
		}

		private void UseImportedValues<TImportDefinition>(IEnumerable<TImportDefinition> definitions, Action<ImportingItem, TImportDefinition, object> useImportValue, bool errorIfMissing) where TImportDefinition : ImportDefinition
		{
			CompositionResult compositionResult = CompositionResult.SucceededResult;
			foreach (TImportDefinition timportDefinition in definitions)
			{
				ImportingItem importingItemFromDefinition = this.GetImportingItemFromDefinition(timportDefinition);
				object obj;
				if (!this.TryGetImportValue(timportDefinition, out obj))
				{
					if (!errorIfMissing)
					{
						continue;
					}
					if (timportDefinition.Cardinality == ImportCardinality.ExactlyOne)
					{
						CompositionError compositionError = CompositionError.Create(CompositionErrorId.ImportNotSetOnPart, Strings.ImportNotSetOnPart, new object[]
						{
							this.Definition.GetPartType().FullName,
							timportDefinition.ToString()
						});
						compositionResult = compositionResult.MergeError(compositionError);
						continue;
					}
					obj = importingItemFromDefinition.CastExportsToImportType(new Export[0]);
				}
				useImportValue(importingItemFromDefinition, timportDefinition, obj);
			}
			compositionResult.ThrowOnErrors();
		}

		private bool TryGetImportValue(ImportDefinition definition, out object value)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				if (this._importValues.TryGetValue(definition, out value))
				{
					this._importValues.Remove(definition);
					return true;
				}
			}
			value = null;
			return false;
		}

		private void NotifyImportSatisfied()
		{
			if (this._invokeImportsSatisfied && !this._invokingImportsSatisfied)
			{
				IPartImportsSatisfiedNotification partImportsSatisfiedNotification = this.GetInstanceActivatingIfNeeded() as IPartImportsSatisfiedNotification;
				if (partImportsSatisfiedNotification != null)
				{
					try
					{
						this._invokingImportsSatisfied = true;
						partImportsSatisfiedNotification.OnImportsSatisfied();
					}
					catch (Exception ex)
					{
						throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_PartOnImportsSatisfiedThrewException, this.Definition.GetPartType().FullName), this.Definition.ToElement(), ex);
					}
					finally
					{
						this._invokingImportsSatisfied = false;
					}
					this._invokeImportsSatisfied = false;
				}
			}
		}

		private ExportingMember GetExportingMemberFromDefinition(ExportDefinition definition)
		{
			ReflectionMemberExportDefinition reflectionMemberExportDefinition = definition as ReflectionMemberExportDefinition;
			if (reflectionMemberExportDefinition == null)
			{
				return null;
			}
			int index = reflectionMemberExportDefinition.GetIndex();
			ExportingMember exportingMember;
			if (!this._exportsCache.TryGetValue(index, out exportingMember))
			{
				exportingMember = ReflectionComposablePart.GetExportingMember(definition);
				if (exportingMember != null)
				{
					this._exportsCache[index] = exportingMember;
				}
			}
			return exportingMember;
		}

		private ImportingItem GetImportingItemFromDefinition(ImportDefinition definition)
		{
			ImportingItem importingItem;
			if (!this._importsCache.TryGetValue(definition, out importingItem))
			{
				importingItem = ReflectionComposablePart.GetImportingItem(definition);
				if (importingItem != null)
				{
					this._importsCache[definition] = importingItem;
				}
			}
			return importingItem;
		}

		private static ImportingItem GetImportingItem(ImportDefinition definition)
		{
			ReflectionImportDefinition reflectionImportDefinition = definition as ReflectionImportDefinition;
			if (reflectionImportDefinition != null)
			{
				return reflectionImportDefinition.ToImportingItem();
			}
			return null;
		}

		private static ExportingMember GetExportingMember(ExportDefinition definition)
		{
			ReflectionMemberExportDefinition reflectionMemberExportDefinition = definition as ReflectionMemberExportDefinition;
			if (reflectionMemberExportDefinition != null)
			{
				return reflectionMemberExportDefinition.ToExportingMember();
			}
			return null;
		}

		private string GetDisplayName()
		{
			return this._definition.GetPartType().GetDisplayName();
		}

		private readonly ReflectionComposablePartDefinition _definition;

		private readonly Dictionary<ImportDefinition, object> _importValues = new Dictionary<ImportDefinition, object>();

		private readonly Dictionary<ImportDefinition, ImportingItem> _importsCache = new Dictionary<ImportDefinition, ImportingItem>();

		private readonly Dictionary<int, ExportingMember> _exportsCache = new Dictionary<int, ExportingMember>();

		private bool _invokeImportsSatisfied = true;

		private bool _invokingImportsSatisfied;

		private bool _initialCompositionComplete;

		private volatile object _cachedInstance;

		private object _lock = new object();
	}
}
