using System;

namespace YamlDotNet.Serialization
{
	public delegate TComponent WrapperFactory<TComponentBase, TComponent>(TComponentBase wrapped) where TComponent : TComponentBase;
}
