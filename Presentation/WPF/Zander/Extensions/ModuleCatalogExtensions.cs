using System;
using Microsoft.Practices.Prism.Modularity;

namespace Zander.Presentation.WPF.Zander.Extensions {
	public static class ModuleCatalogExtensions {
		public static void RegisterModule<T>(this IModuleCatalog catalog) where T : IModule {
			RegisterModule<T>(catalog, InitializationMode.WhenAvailable);
		}

		public static void RegisterModule<T>(this IModuleCatalog catalog, InitializationMode mode) where T : IModule {
			var type = typeof(T);

			var module = new ModuleInfo {
				ModuleName = type.Name,
				ModuleType = type.AssemblyQualifiedName,
				InitializationMode = mode
			};

			catalog.AddModule(module);

		}
	}
}
