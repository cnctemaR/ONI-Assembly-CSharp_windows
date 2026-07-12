using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ImportingMember : ImportingItem
	{
		public ImportingMember(ContractBasedImportDefinition definition, ReflectionWritableMember member, ImportType importType)
			: base(definition, importType)
		{
			Assumes.NotNull<ContractBasedImportDefinition, ReflectionWritableMember>(definition, member);
			this._member = member;
		}

		public void SetExportedValue(object instance, object value)
		{
			if (this.RequiresCollectionNormalization())
			{
				this.SetCollectionMemberValue(instance, (IEnumerable)value);
				return;
			}
			this.SetSingleMemberValue(instance, value);
		}

		private bool RequiresCollectionNormalization()
		{
			return base.Definition.Cardinality == ImportCardinality.ZeroOrMore && (!this._member.CanWrite || !base.ImportType.IsAssignableCollectionType);
		}

		private void SetSingleMemberValue(object instance, object value)
		{
			this.EnsureWritable();
			try
			{
				this._member.SetValue(instance, value);
			}
			catch (TargetInvocationException ex)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportThrewException, this._member.GetDisplayName()), base.Definition.ToElement(), ex.InnerException);
			}
			catch (TargetParameterCountException ex2)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ImportNotValidOnIndexers, this._member.GetDisplayName()), base.Definition.ToElement(), ex2.InnerException);
			}
		}

		private void EnsureWritable()
		{
			if (!this._member.CanWrite)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportNotWritable, this._member.GetDisplayName()), base.Definition.ToElement());
			}
		}

		private void SetCollectionMemberValue(object instance, IEnumerable values)
		{
			Assumes.NotNull<IEnumerable>(values);
			ICollection<object> collection = null;
			Type collectionElementType = CollectionServices.GetCollectionElementType(base.ImportType.ActualType);
			if (collectionElementType != null)
			{
				collection = this.GetNormalizedCollection(collectionElementType, instance);
			}
			this.EnsureCollectionIsWritable(collection);
			this.PopulateCollection(collection, values);
		}

		private ICollection<object> GetNormalizedCollection(Type itemType, object instance)
		{
			Assumes.NotNull<Type>(itemType);
			object obj = null;
			if (this._member.CanRead)
			{
				try
				{
					obj = this._member.GetValue(instance);
				}
				catch (TargetInvocationException ex)
				{
					throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportCollectionGetThrewException, this._member.GetDisplayName()), base.Definition.ToElement(), ex.InnerException);
				}
			}
			if (obj == null)
			{
				ConstructorInfo constructor = base.ImportType.ActualType.GetConstructor(Type.EmptyTypes);
				if (constructor != null)
				{
					try
					{
						obj = constructor.SafeInvoke(Array.Empty<object>());
					}
					catch (TargetInvocationException ex2)
					{
						throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportCollectionConstructionThrewException, this._member.GetDisplayName(), base.ImportType.ActualType.FullName), base.Definition.ToElement(), ex2.InnerException);
					}
					this.SetSingleMemberValue(instance, obj);
				}
			}
			if (obj == null)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportCollectionNull, this._member.GetDisplayName()), base.Definition.ToElement());
			}
			return CollectionServices.GetCollectionWrapper(itemType, obj);
		}

		private void EnsureCollectionIsWritable(ICollection<object> collection)
		{
			bool flag = true;
			try
			{
				if (collection != null)
				{
					flag = collection.IsReadOnly;
				}
			}
			catch (Exception ex)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportCollectionIsReadOnlyThrewException, this._member.GetDisplayName(), collection.GetType().FullName), base.Definition.ToElement(), ex);
			}
			if (flag)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportCollectionNotWritable, this._member.GetDisplayName()), base.Definition.ToElement());
			}
		}

		private void PopulateCollection(ICollection<object> collection, IEnumerable values)
		{
			Assumes.NotNull<ICollection<object>, IEnumerable>(collection, values);
			try
			{
				collection.Clear();
			}
			catch (Exception ex)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportCollectionClearThrewException, this._member.GetDisplayName(), collection.GetType().FullName), base.Definition.ToElement(), ex);
			}
			foreach (object obj in values)
			{
				try
				{
					collection.Add(obj);
				}
				catch (Exception ex2)
				{
					throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ImportCollectionAddThrewException, this._member.GetDisplayName(), collection.GetType().FullName), base.Definition.ToElement(), ex2);
				}
			}
		}

		private readonly ReflectionWritableMember _member;
	}
}
