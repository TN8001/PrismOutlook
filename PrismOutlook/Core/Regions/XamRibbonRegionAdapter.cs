using System;
using System.Collections.Specialized;
using Fluent;
using Prism.Regions;

namespace PrismOutlook.Core.Regions;

public class XamRibbonRegionAdapter : RegionAdapterBase<Ribbon>
{
    public XamRibbonRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory) { }

    protected override void Adapt(IRegion region, Ribbon regionTarget)
    {
        ArgumentNullException.ThrowIfNull(region);
        ArgumentNullException.ThrowIfNull(regionTarget);

        region.Views.CollectionChanged += (s, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var view in e.NewItems) AddViewToRegion(view, regionTarget);

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var view in e.OldItems) RemoveViewFromRegion(view, regionTarget);
            }
        };
    }

    protected override IRegion CreateRegion() => new SingleActiveRegion();

    private static void AddViewToRegion(object view, Ribbon xamRibbon)
    {
        if (view is RibbonTabItem ribbonTabItem) xamRibbon.Tabs.Add(ribbonTabItem);
    }

    private static void RemoveViewFromRegion(object view, Ribbon xamRibbon)
    {
        if (view is RibbonTabItem ribbonTabItem) xamRibbon.Tabs.Remove(ribbonTabItem);
    }
}
