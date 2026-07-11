using System;

namespace YamlDotNet.Serialization.ObjectFactories
{
	public sealed class LambdaObjectFactory : IObjectFactory
	{
		public LambdaObjectFactory(Func<Type, object> factory)
		{
			if (factory == null)
			{
				throw new ArgumentNullException("factory");
			}
			this._factory = factory;
		}

		public object Create(Type type)
		{
			return this._factory(type);
		}

		private readonly Func<Type, object> _factory;
	}
}
