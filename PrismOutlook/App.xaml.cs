using System.Windows;
using Fluent;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using PrismOutlook.Core;
using PrismOutlook.Core.Dialogs;
using PrismOutlook.Core.Regions;
using PrismOutlook.Modules.Contacts;
using PrismOutlook.Modules.Mail;
using PrismOutlook.Views;

namespace PrismOutlook;

public partial class App
{
    protected override Window CreateShell() => Container.Resolve<MainWindow>();

    protected override void InitializeShell(Window shell) => base.InitializeShell(shell);

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        _ = containerRegistry.RegisterSingleton<IRegionDialogService, RegionDialogService>();
        _ = containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        _ = moduleCatalog.AddModule<MailModule>();
        _ = moduleCatalog.AddModule<ContactsModule>();
    }

    protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
    {
        base.ConfigureRegionAdapterMappings(regionAdapterMappings);
        //regionAdapterMappings.RegisterMapping(typeof(XamOutlookBar), Container.Resolve<XamOutlookBarRegionAdapter>());
        regionAdapterMappings.RegisterMapping(typeof(Ribbon), Container.Resolve<XamRibbonRegionAdapter>());
    }

    protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
    {
        base.ConfigureDefaultRegionBehaviors(regionBehaviors);
        regionBehaviors.AddIfMissing(DependentViewRegionBehavior.BehaviorKey, typeof(DependentViewRegionBehavior));
    }
}
