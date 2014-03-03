using System;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Modularity;
using System.Linq;

namespace Zander.Presentation.WPF.Zander.Extensions {
	public static class ModuleCatalogExtensions {
		public static void RegisterModule<T>(this IModuleCatalog catalog, params Type[] dependsOn) where T : IModule {
			RegisterModule<T>(catalog, InitializationMode.WhenAvailable, dependsOn);
		}

        public static void RegisterModule<T>(this IModuleCatalog catalog, InitializationMode mode, params Type[] dependsOn) where T : IModule {
			var type = typeof(T);

			var module = new ModuleInfo {
				ModuleName = type.Name,
				ModuleType = type.AssemblyQualifiedName,
				InitializationMode = mode,
                DependsOn = new Collection<string>(dependsOn.Select(x => x.Name).ToList())
			};

			catalog.AddModule(module);
        }
	}
}
