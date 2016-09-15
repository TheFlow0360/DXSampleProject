using System;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace DevExpressGridInconsistencyDemo
{
    public class TableView : DevExpress.Xpf.Grid.TableView
    {
        static TableView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TableView), new FrameworkPropertyMetadata(typeof(DevExpress.Xpf.Grid.TableView)));
        }

        public TableView()
        {
            AllowGrouping = true;
            ShowGroupPanel = true;
            AlternationCount = 2;
            ShowTotalSummary = true;
            ShowAutoFilterRow = true;
            AutoWidth = false;
            ShowCheckBoxSelectorColumn = false;
            AllowEditing = false;
            ExtendScrollBarToFixedColumns = true;
            AllowFixedColumnMenu = true;
            FixedLineWidth = 3;
            AllowFilterEditor = true;
            AllowColumnFiltering = true;
            AllowSorting = true;

            NavigationStyle = GridViewNavigationStyle.Row;
            
            AllowHorizontalScrollingVirtualization = true;
        }
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            BehaviorCollection behaviors = Interaction.GetBehaviors(this);
            behaviors?.Add(new RowSelectionBehavior());
        }
    }
}