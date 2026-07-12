using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition
{
	internal static class ExportServices
	{
		internal static bool IsDefaultMetadataViewType(Type metadataViewType)
		{
			Assumes.NotNull<Type>(metadataViewType);
			return metadataViewType.IsAssignableFrom(ExportServices.DefaultMetadataViewType);
		}

		internal static bool IsDictionaryConstructorViewType(Type metadataViewType)
		{
			Assumes.NotNull<Type>(metadataViewType);
			return metadataViewType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.DefaultBinder, new Type[] { typeof(IDictionary<string, object>) }, new ParameterModifier[0]) != null;
		}

		internal static Func<Export, object> CreateStronglyTypedLazyFactory(Type exportType, Type metadataViewType)
		{
			MethodInfo methodInfo;
			if (metadataViewType != null)
			{
				methodInfo = ExportServices._createStronglyTypedLazyOfTM.MakeGenericMethod(new Type[]
				{
					exportType ?? ExportServices.DefaultExportedValueType,
					metadataViewType
				});
			}
			else
			{
				methodInfo = ExportServices._createStronglyTypedLazyOfT.MakeGenericMethod(new Type[] { exportType ?? ExportServices.DefaultExportedValueType });
			}
			Assumes.NotNull<MethodInfo>(methodInfo);
			return (Func<Export, object>)Delegate.CreateDelegate(typeof(Func<Export, object>), methodInfo);
		}

		internal static Func<Export, Lazy<object, object>> CreateSemiStronglyTypedLazyFactory(Type exportType, Type metadataViewType)
		{
			MethodInfo methodInfo = ExportServices._createSemiStronglyTypedLazy.MakeGenericMethod(new Type[]
			{
				exportType ?? ExportServices.DefaultExportedValueType,
				metadataViewType ?? ExportServices.DefaultMetadataViewType
			});
			Assumes.NotNull<MethodInfo>(methodInfo);
			return (Func<Export, Lazy<object, object>>)Delegate.CreateDelegate(typeof(Func<Export, Lazy<object, object>>), methodInfo);
		}

		internal static Lazy<T, M> CreateStronglyTypedLazyOfTM<T, M>(Export export)
		{
			IDisposable disposable = export as IDisposable;
			if (disposable != null)
			{
				return new ExportServices.DisposableLazy<T, M>(() => ExportServices.GetCastedExportedValue<T>(export), AttributedModelServices.GetMetadataView<M>(export.Metadata), disposable, LazyThreadSafetyMode.PublicationOnly);
			}
			return new Lazy<T, M>(() => ExportServices.GetCastedExportedValue<T>(export), AttributedModelServices.GetMetadataView<M>(export.Metadata), LazyThreadSafetyMode.PublicationOnly);
		}

		internal static Lazy<T> CreateStronglyTypedLazyOfT<T>(Export export)
		{
			IDisposable disposable = export as IDisposable;
			if (disposable != null)
			{
				return new ExportServices.DisposableLazy<T>(() => ExportServices.GetCastedExportedValue<T>(export), disposable, LazyThreadSafetyMode.PublicationOnly);
			}
			return new Lazy<T>(() => ExportServices.GetCastedExportedValue<T>(export), LazyThreadSafetyMode.PublicationOnly);
		}

		internal static Lazy<object, object> CreateSemiStronglyTypedLazy<T, M>(Export export)
		{
			IDisposable disposable = export as IDisposable;
			if (disposable != null)
			{
				return new ExportServices.DisposableLazy<object, object>(() => ExportServices.GetCastedExportedValue<T>(export), AttributedModelServices.GetMetadataView<M>(export.Metadata), disposable, LazyThreadSafetyMode.PublicationOnly);
			}
			return new Lazy<object, object>(() => ExportServices.GetCastedExportedValue<T>(export), AttributedModelServices.GetMetadataView<M>(export.Metadata), LazyThreadSafetyMode.PublicationOnly);
		}

		internal static T GetCastedExportedValue<T>(Export export)
		{
			return ExportServices.CastExportedValue<T>(export.ToElement(), export.Value);
		}

		internal static T CastExportedValue<T>(ICompositionElement element, object exportedValue)
		{
			object obj = null;
			if (!ContractServices.TryCast(typeof(T), exportedValue, out obj))
			{
				throw new CompositionContractMismatchException(string.Format(CultureInfo.CurrentCulture, Strings.ContractMismatch_ExportedValueCannotBeCastToT, element.DisplayName, typeof(T)));
			}
			return (T)((object)obj);
		}

		internal static ExportCardinalityCheckResult CheckCardinality<T>(ImportDefinition definition, IEnumerable<T> enumerable)
		{
			return ExportServices.MatchCardinality((enumerable != null) ? enumerable.GetCardinality<T>() : EnumerableCardinality.Zero, definition.Cardinality);
		}

		private static ExportCardinalityCheckResult MatchCardinality(EnumerableCardinality actualCardinality, ImportCardinality importCardinality)
		{
			if (actualCardinality != EnumerableCardinality.Zero)
			{
				if (actualCardinality != EnumerableCardinality.TwoOrMore)
				{
					Assumes.IsTrue(actualCardinality == EnumerableCardinality.One);
				}
				else if (importCardinality.IsAtMostOne())
				{
					return ExportCardinalityCheckResult.TooManyExports;
				}
			}
			else if (importCardinality == ImportCardinality.ExactlyOne)
			{
				return ExportCardinalityCheckResult.NoExports;
			}
			return ExportCardinalityCheckResult.Match;
		}

		private static readonly MethodInfo _createStronglyTypedLazyOfTM = typeof(ExportServices).GetMethod("CreateStronglyTypedLazyOfTM", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly MethodInfo _createStronglyTypedLazyOfT = typeof(ExportServices).GetMethod("CreateStronglyTypedLazyOfT", BindingFlags.Static | BindingFlags.NonPublic);

		private static readonly MethodInfo _createSemiStronglyTypedLazy = typeof(ExportServices).GetMethod("CreateSemiStronglyTypedLazy", BindingFlags.Static | BindingFlags.NonPublic);

		internal static readonly Type DefaultMetadataViewType = typeof(IDictionary<string, object>);

		internal static readonly Type DefaultExportedValueType = typeof(object);

		private sealed class DisposableLazy<T, TMetadataView> : Lazy<T, TMetadataView>, IDisposable
		{
			public DisposableLazy(Func<T> valueFactory, TMetadataView metadataView, IDisposable disposable, LazyThreadSafetyMode mode)
				: base(valueFactory, metadataView, mode)
			{
				Assumes.NotNull<IDisposable>(disposable);
				this._disposable = disposable;
			}

			void IDisposable.Dispose()
			{
				this._disposable.Dispose();
			}

			private IDisposable _disposable;
		}

		private sealed class DisposableLazy<T> : Lazy<T>, IDisposable
		{
			public DisposableLazy(Func<T> valueFactory, IDisposable disposable, LazyThreadSafetyMode mode)
				: base(valueFactory, mode)
			{
				Assumes.NotNull<IDisposable>(disposable);
				this._disposable = disposable;
			}

			void IDisposable.Dispose()
			{
				this._disposable.Dispose();
			}

			private IDisposable _disposable;
		}
	}
}
