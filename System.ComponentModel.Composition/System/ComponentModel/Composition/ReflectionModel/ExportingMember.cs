using System;
using System.ComponentModel.Composition.Primitives;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	internal class ExportingMember
	{
		public ExportingMember(ExportDefinition definition, ReflectionMember member)
		{
			Assumes.NotNull<ExportDefinition, ReflectionMember>(definition, member);
			this._definition = definition;
			this._member = member;
		}

		public bool RequiresInstance
		{
			get
			{
				return this._member.RequiresInstance;
			}
		}

		public ExportDefinition Definition
		{
			get
			{
				return this._definition;
			}
		}

		public object GetExportedValue(object instance, object @lock)
		{
			this.EnsureReadable();
			if (!this._isValueCached)
			{
				object value;
				try
				{
					value = this._member.GetValue(instance);
				}
				catch (TargetInvocationException ex)
				{
					throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ExportThrewException, this._member.GetDisplayName()), this.Definition.ToElement(), ex.InnerException);
				}
				catch (TargetParameterCountException ex2)
				{
					throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ExportNotValidOnIndexers, this._member.GetDisplayName()), this.Definition.ToElement(), ex2.InnerException);
				}
				lock (@lock)
				{
					if (!this._isValueCached)
					{
						this._cachedValue = value;
						Thread.MemoryBarrier();
						this._isValueCached = true;
					}
				}
			}
			return this._cachedValue;
		}

		private void EnsureReadable()
		{
			if (!this._member.CanRead)
			{
				throw new ComposablePartException(string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_ExportNotReadable, this._member.GetDisplayName()), this.Definition.ToElement());
			}
		}

		private readonly ExportDefinition _definition;

		private readonly ReflectionMember _member;

		private object _cachedValue;

		private volatile bool _isValueCached;
	}
}
