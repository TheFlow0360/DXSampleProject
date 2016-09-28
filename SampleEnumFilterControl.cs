using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;

namespace DevExpressGridInconsistencyDemo
{
    public class SampleEnumFilterControl : ComboBoxEdit
    {
        public SampleEnumFilterControl()
        {
            PropertyCollection = new HashSet<SampleEnumWrapper>();
            PropertyCollection.Add(new SampleEnumWrapper());
            foreach (var enumValue in Enum.GetValues(typeof(SampleEnum)).Cast<SampleEnum>())
            {
                PropertyCollection.Add(new SampleEnumWrapper() { Enum = enumValue });
            }
            this.SelectedIndex = 0;
        }

        public static readonly DependencyProperty SelectedEnumProperty = DependencyProperty.Register(
            "SelectedEnum",
            typeof(SampleEnum?),
            typeof(SampleEnumFilterControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var This = (SampleEnumFilterControl) d;
        }

        public SampleEnum? SelectedEnum
        {
            get { return (SampleEnum?)GetValue(SelectedEnumProperty); }
            set { SetValue(SelectedEnumProperty, value); }
        }

        public ICollection<SampleEnumWrapper> PropertyCollection { get; set; }

        protected override void ItemsSourceChanged(Object itemsSource)
        {
            base.ItemsSourceChanged(PropertyCollection);
        }

        protected override BaseEditSettings CreateEditorSettings()
        {
            return new SampleEnumFilterEditSettings();
        }
    }

    public class SampleEnumFilterEditSettings : ComboBoxEditSettings
    {
        static SampleEnumFilterEditSettings()
        {
            EditorSettingsProvider.Default.RegisterUserEditor2(typeof(SampleEnumFilterControl), typeof(SampleEnumFilterEditSettings), optimized => optimized ? new InplaceBaseEdit() : (IBaseEdit)new SampleEnumFilterControl(), () => new SampleEnumFilterEditSettings());
        }

        public SampleEnumFilterEditSettings()
        {
            PropertyCollection = new HashSet<SampleEnumWrapper>();
            PropertyCollection.Add(new SampleEnumWrapper());
            foreach (var enumValue in Enum.GetValues(typeof(SampleEnum)).Cast<SampleEnum>())
            {
                PropertyCollection.Add(new SampleEnumWrapper() { Enum = enumValue });
            }
            this.ItemsSource = PropertyCollection;
        }

        public static readonly DependencyProperty SelectedEnumProperty = DependencyProperty.Register(
            "SelectedEnum",
            typeof(SampleEnum?),
            typeof(SampleEnumFilterEditSettings),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var This = (SampleEnumFilterEditSettings) d;
        }

        public SampleEnum? SelectedEnum
        {
            get { return (SampleEnum?)GetValue(SelectedEnumProperty); }
            set { SetValue(SelectedEnumProperty, value); }
        }

        public ICollection<SampleEnumWrapper> PropertyCollection { get; set; }

        protected override void AssignToEditCore(IBaseEdit edit)
        {
            base.AssignToEditCore(edit);
            if (edit is SampleEnumFilterControl)
            {
                var editor = (SampleEnumFilterControl)edit;
                editor.SelectedEnum = SelectedEnum;
                editor.PropertyCollection = PropertyCollection;
            }
        }
    }
}