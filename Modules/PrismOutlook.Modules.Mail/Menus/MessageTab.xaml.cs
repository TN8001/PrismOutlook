using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using PrismOutlook.Core;

namespace PrismOutlook.Modules.Mail.Menus;

public partial class MessageTab : Fluent.RibbonTabItem, ISupportDataContext, ISupportRichText
{
    private RichTextBox _richTextEditor;
    private bool _updatingState;

    public static double[] FontSizes => new double[] {
                3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5,
                10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 15.0,
                16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0,
                32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
                80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
                };

    public RichTextBox RichTextEditor
    {
        get => _richTextEditor;
        set
        {
            if (_richTextEditor != null)
            {
                _richTextEditor.Loaded -= RichTextEditor_Loaded;
                _richTextEditor.SelectionChanged -= RichTextEditor_SelectionChanged;
            }

            _richTextEditor = value;

            if (_richTextEditor != null)
            {
                _richTextEditor.Loaded += RichTextEditor_Loaded;
                _richTextEditor.SelectionChanged += RichTextEditor_SelectionChanged;
            }
        }
    }

    public MessageTab()
    {
        InitializeComponent();

        _fontSizes.ItemsSource = FontSizes;
        _fontNames.ItemsSource = Fonts.SystemFontFamilies.ToList().Select(x => x.Source);
    }

    private void FontSizes_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_updatingState) return;
        if (sender is ComboBox comboBox)
        {
            if (comboBox.SelectedItem == null) return;

            var points = (double)comboBox.SelectedItem;
            var pixels = Points2Pixels(points);
            RichTextEditor.Selection.ApplyFontSize(pixels);
        }
    }

    private void FontNames_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_updatingState) return;
        if (sender is ComboBox comboBox)
        {
            if (comboBox.SelectedItem == null) return;

            var s = (string)comboBox.SelectedItem;
            RichTextEditor.Selection.ApplyFont(s);
        }
    }

    private void RichTextEditor_SelectionChanged(object sender, RoutedEventArgs e)
        => UpdateVisualState();

    private void RichTextEditor_Loaded(object sender, RoutedEventArgs e)
        => UpdateVisualState();

    private void UpdateVisualState()
    {
        _updatingState = true;

        TextRange range = RichTextEditor.Selection;

        UpdateFontSizes(range);
        UpdateFontFamily(range);

        if (range.GetPropertyValue(TextElement.FontWeightProperty) is FontWeight weight)
            UpdateToggleButton(_boldButton, weight == FontWeights.Bold);

        if (range.GetPropertyValue(TextElement.FontStyleProperty) is FontStyle style)
            UpdateToggleButton(_italicButton, style == FontStyles.Italic);

        UpdateUnderlineState(range);
        UpdateAlignment(range);
        UpdateParagraphListStyleState(range);

        _updatingState = false;
    }

    private void UpdateAlignment(TextRange range)
    {
        if (range.GetPropertyValue(FlowDocument.TextAlignmentProperty) is TextAlignment alignment)
        {
            var t = alignment switch
            {
                TextAlignment.Left => _alignLeft,
                TextAlignment.Center => _alignCenter,
                TextAlignment.Right => _alignRight,
                TextAlignment.Justify => _alignJustify,
                _ => _alignLeft,
            };
            UpdateToggleButton(t, true);
        }
    }

    void UpdateUnderlineState(TextRange range)
    {
        var p = range.GetPropertyValue(TextBlock.TextDecorationsProperty);
        if (p is TextDecorationCollection c)
        {
            var b = c.Any() && c.All(x => x.Location == TextDecorationLocation.Underline);
            UpdateToggleButton(_underlineButton, b);
            return;
        }

        UpdateToggleButton(_underlineButton, false);
    }

    private void UpdateToggleButton(ToggleButton button, bool? value)
        => button.IsChecked = value.HasValue && value.Value;

    private void UpdateFontSizes(TextRange range)
    {
        if (range.GetPropertyValue(TextElement.FontSizeProperty) is double pixels)
        {
            var points = Pixels2Points(pixels);
            _fontSizes.SelectedItem = points;
        }
    }
    private static double Points2Pixels(double points) => points * (96.0 / 72.0);
    private static double Pixels2Points(double pixels) => Math.Round(pixels / (96.0 / 72.0), 1);

    void UpdateFontFamily(TextRange range)
    {
        if (range.GetPropertyValue(TextElement.FontFamilyProperty) is FontFamily fontFamily)
            _fontNames.SelectedItem = fontFamily.Source;
    }

    void UpdateParagraphListStyleState(TextRange range)
    {
        var list = range.Start.Parent.FindAncestor<List>();
        if (list != null)
        {
            UpdateToggleButton(_bulletsButton, list.MarkerStyle == TextMarkerStyle.Disc);
            UpdateToggleButton(_numbersButton, list.MarkerStyle == TextMarkerStyle.Decimal);
        }
        else
        {
            UpdateToggleButton(_bulletsButton, false);
            UpdateToggleButton(_numbersButton, false);
        }
    }
}


internal static class Extensions
{
    public static TextRange GetSelection(this RichTextBox rtb) 
        => new(rtb.Selection.Start, rtb.Selection.End);
    public static void ApplyFontSize(this TextRange range, double size) 
        => range.ApplyPropertyValue(TextElement.FontSizeProperty, size);
    public static void ApplyFont(this TextRange range, string name) 
        => range.ApplyPropertyValue(TextElement.FontFamilyProperty, name);

    public static T FindAncestor<T>(this DependencyObject depObj) where T : DependencyObject
    {
        while (depObj != null)
        {
            if (depObj is T target) return target;
            depObj = LogicalTreeHelper.GetParent(depObj);
        }
        return null;
    }
}