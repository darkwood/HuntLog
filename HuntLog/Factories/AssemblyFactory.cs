using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HuntLog.AppModule;
using Xamarin.Forms;

namespace HuntLog.Factories
{
    public static class AssemblyFactory
    {
        private static Type _vmInterface = typeof(IViewModel);
        private static Type _viewInterface = typeof(Page);

        public static List<Type> GetViewModels()
        {
            Type[] q = GetAllTypes();
            var vms = q.Where(x => x.Name.EndsWith("ViewModel", StringComparison.Ordinal)
                                    && _vmInterface.IsAssignableFrom(x));
            return vms.ToList();
        }

        public static List<Type> GetViews()
        {
            Type[] q = GetAllTypes();
            var views = q.Where(x => x.Name.EndsWith("View", StringComparison.Ordinal)
                                    && _viewInterface.IsAssignableFrom(x));
            return views.ToList();
        }

        private static Type[] GetAllTypes()
        {
            return (from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass
                    && (t.Namespace != null
                    && t.Namespace.StartsWith("HuntLog", StringComparison.Ordinal))
                    select t).ToArray();
        }
    }
}
