using System;
using LightInject;

namespace HuntLog.Tests
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            var containerOptions = new ContainerOptions() { EnablePropertyInjection = false };
            var container = new ServiceContainer(containerOptions);
            container.RegisterFrom<CompositionRoot>();
        }
    }
}
