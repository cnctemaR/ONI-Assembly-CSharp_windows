using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal sealed class ExportFactoryCreator
	{
		public ExportFactoryCreator(Type exportFactoryType)
		{
			Assumes.NotNull<Type>(exportFactoryType);
			this._exportFactoryType = exportFactoryType;
		}

		public Func<Export, object> CreateStronglyTypedExportFactoryFactory(Type exportType, Type metadataViewType)
		{
			MethodInfo methodInfo;
			if (metadataViewType == null)
			{
				methodInfo = ExportFactoryCreator._createStronglyTypedExportFactoryOfT.MakeGenericMethod(new Type[] { exportType });
			}
			else
			{
				methodInfo = ExportFactoryCreator._createStronglyTypedExportFactoryOfTM.MakeGenericMethod(new Type[] { exportType, metadataViewType });
			}
			Assumes.NotNull<MethodInfo>(methodInfo);
			Func<Export, object> exportFactoryFactory = (Func<Export, object>)Delegate.CreateDelegate(typeof(Func<Export, object>), this, methodInfo);
			return (Export e) => exportFactoryFactory(e);
		}

		private object CreateStronglyTypedExportFactoryOfT<T>(Export export)
		{
			Type[] array = new Type[] { typeof(T) };
			Type type = this._exportFactoryType.MakeGenericType(array);
			ExportFactoryCreator.LifetimeContext lifetimeContext = new ExportFactoryCreator.LifetimeContext();
			Func<Tuple<T, Action>> func = () => lifetimeContext.GetExportLifetimeContextFromExport<T>(export);
			object[] array2 = new object[] { func };
			object obj = Activator.CreateInstance(type, array2);
			lifetimeContext.SetInstance(obj);
			return obj;
		}

		private object CreateStronglyTypedExportFactoryOfTM<T, M>(Export export)
		{
			Type[] array = new Type[]
			{
				typeof(T),
				typeof(M)
			};
			Type type = this._exportFactoryType.MakeGenericType(array);
			ExportFactoryCreator.LifetimeContext lifetimeContext = new ExportFactoryCreator.LifetimeContext();
			Func<Tuple<T, Action>> func = () => lifetimeContext.GetExportLifetimeContextFromExport<T>(export);
			M metadataView = AttributedModelServices.GetMetadataView<M>(export.Metadata);
			object[] array2 = new object[] { func, metadataView };
			object obj = Activator.CreateInstance(type, array2);
			lifetimeContext.SetInstance(obj);
			return obj;
		}

		private static readonly MethodInfo _createStronglyTypedExportFactoryOfT = typeof(ExportFactoryCreator).GetMethod("CreateStronglyTypedExportFactoryOfT", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private static readonly MethodInfo _createStronglyTypedExportFactoryOfTM = typeof(ExportFactoryCreator).GetMethod("CreateStronglyTypedExportFactoryOfTM", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		private Type _exportFactoryType;

		private class LifetimeContext
		{
			public Func<ComposablePartDefinition, bool> CatalogFilter { get; private set; }

			public void SetInstance(object instance)
			{
				Assumes.NotNull<object>(instance);
				MethodInfo method = instance.GetType().GetMethod("IncludeInScopedCatalog", BindingFlags.Instance | BindingFlags.NonPublic, null, ExportFactoryCreator.LifetimeContext.types, null);
				this.CatalogFilter = (Func<ComposablePartDefinition, bool>)Delegate.CreateDelegate(typeof(Func<ComposablePartDefinition, bool>), instance, method);
			}

			public Tuple<T, Action> GetExportLifetimeContextFromExport<T>(Export export)
			{
				IDisposable disposable = null;
				CatalogExportProvider.ScopeFactoryExport scopeFactoryExport = export as CatalogExportProvider.ScopeFactoryExport;
				T t;
				if (scopeFactoryExport != null)
				{
					Export export2 = scopeFactoryExport.CreateExportProduct(this.CatalogFilter);
					t = ExportServices.GetCastedExportedValue<T>(export2);
					disposable = export2 as IDisposable;
				}
				else
				{
					CatalogExportProvider.FactoryExport factoryExport = export as CatalogExportProvider.FactoryExport;
					if (factoryExport != null)
					{
						Export export3 = factoryExport.CreateExportProduct();
						t = ExportServices.GetCastedExportedValue<T>(export3);
						disposable = export3 as IDisposable;
					}
					else
					{
						ComposablePartDefinition castedExportedValue = ExportServices.GetCastedExportedValue<ComposablePartDefinition>(export);
						ComposablePart composablePart = castedExportedValue.CreatePart();
						ExportDefinition exportDefinition = castedExportedValue.ExportDefinitions.Single<ExportDefinition>();
						t = ExportServices.CastExportedValue<T>(composablePart.ToElement(), composablePart.GetExportedValue(exportDefinition));
						disposable = composablePart as IDisposable;
					}
				}
				Action action;
				if (disposable != null)
				{
					action = delegate
					{
						disposable.Dispose();
					};
				}
				else
				{
					action = delegate
					{
					};
				}
				return new Tuple<T, Action>(t, action);
			}

			private static Type[] types = new Type[] { typeof(ComposablePartDefinition) };
		}
	}
}
