﻿using Markify.Core.IDE;
using Markify.Core.Rendering;

using Markify.Services.IO;
using Markify.Services.Settings;
using Markify.Services.Processing;

using Ninject.Modules;

namespace Markify.Services
{
    internal sealed class ServicesModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            Bind<IPageWriter>().To<FileWriter>();
            Bind<IProjectProcessor>().To<ProjectProcessor>();
            Bind<IRendererService>().To<RendererService>();
            Bind<IDocumentationGenerator>().To<DocumentationGenerator>();
            Bind<ISettingsProvider>().To<SettingsProvider>();
            Bind<ISolutionExplorerFilterProvider>().To<SolutionExplorerFilterProvider>();
        }

        #endregion
    }
}