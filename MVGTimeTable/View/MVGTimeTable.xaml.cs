﻿// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGTimeTable.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MVGTimeTable
{
    public partial class MVGTimeTable : UserControl
    {
        private readonly Dictionary<string, double> savedWidths = new Dictionary<string, double>
        {
            { Common.ColumnName[Column.Line], 0 },
            { Common.ColumnName[Column.Destination], 0 },
            { Common.ColumnName[Column.TimeToDeparture], 0 },
            { Common.ColumnName[Column.DepartureTime], 0 },
            { Common.ColumnName[Column.Platform], 0 }
        };

        private readonly Dictionary<string, double> minWidths = new Dictionary<string, double>
        {
            { Common.ColumnName[Column.Line], 0 },
            { Common.ColumnName[Column.Destination], 0 },
            { Common.ColumnName[Column.TimeToDeparture], 0 },
            { Common.ColumnName[Column.DepartureTime], 0 },
            { Common.ColumnName[Column.Platform], 0 }
        };

        public MVGTimeTable()
        {
            InitializeComponent();
            SetEventsHandlers();

        }

        /// ************************************************************************************************
        /// <summary>
        /// Initializes array with minimum widths of the columns to set column width explicitely in case they are too narrow.
        /// </summary>
        private void SetMinWidth()
        {
            GridView gridView = listViewTimeTable.View as GridView;

            foreach (GridViewColumn gridViewColumn in gridView.Columns)
            {
                string headerName = gridViewColumn.Header.ToString();
                Thickness th = Common.GetMargin(headerName, (DataContext as DeparturesViewModel).TableFontSize);
                minWidths[headerName] = (DataContext as DeparturesViewModel).MinColumnsSize[headerName] + th.Left + th.Right;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Items CurrentChanged handler. This event is invoked when all items in table are updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Items_CurrentChanged(object sender, EventArgs e)
        {
            AutoSizeColumns();
        }

        /// ************************************************************************************************
        /// <summary>
        /// Add handlers to the Items.CurrentChanged and Column Property Changed events for all columns except Destination
        /// </summary>
        private void SetEventsHandlers()
        {
            this.DataContextChanged += MVGTable_DataContextChanged;
            listViewTimeTable.Items.CurrentChanged += Items_CurrentChanged;
            GridView gridView = listViewTimeTable.View as GridView;
            for (int i = 0; i < gridView.Columns.Count; ++i)
            {
                if (i != Common.UtmostColumn)
                {
                    ((INotifyPropertyChanged)gridView.Columns[i]).PropertyChanged += ColumnWidthChanged;
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Column Width Changed Event Handler
        /// </summary>
        private void ColumnWidthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is GridViewColumn && e.PropertyName == "ActualWidth")
            {
                GridViewColumn gridViewColumn = (GridViewColumn)sender;
                if (gridViewColumn.ActualWidth > 0 && gridViewColumn.ActualWidth != savedWidths[gridViewColumn.Header.ToString()])
                {
                    if (gridViewColumn.ActualWidth < minWidths[gridViewColumn.Header.ToString()])
                    {
                        savedWidths[gridViewColumn.Header.ToString()] = minWidths[gridViewColumn.Header.ToString()];
                        gridViewColumn.Width = minWidths[gridViewColumn.Header.ToString()];
                    }
                    else
                    {
                        savedWidths[gridViewColumn.Header.ToString()] = gridViewColumn.ActualWidth;
                        SetUtmostColumnWidth();
                    }
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Autosizes Columns of ListView. It's needed after update of the binded data.
        /// </summary>
        private void AutoSizeColumns()
        {
            if (listViewTimeTable.View is GridView gridView)
            {
                for (int i = 0; i < gridView.Columns.Count; ++i)
                {
                    if (gridView.Columns[i].Header.ToString() == Common.ColumnName[Column.Destination]) continue;
                    if (double.IsNaN(gridView.Columns[i].Width))
                    {
                        gridView.Columns[i].Width = gridView.Columns[i].ActualWidth;
                    }
                    gridView.Columns[i].Width = double.NaN;
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Calculates width of the utmost column after an autosizing
        /// </summary>
        private void SetUtmostColumnWidth()
        {
            listViewTimeTable.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            listViewTimeTable.Arrange(new Rect(0, 0, listViewTimeTable.DesiredSize.Width, listViewTimeTable.DesiredSize.Height));

            GridView gridView = listViewTimeTable.View as GridView;

            if (Common.UtmostColumn >= gridView.Columns.Count) return;

            double fullColumnsWidth = 0;

            foreach (string key in savedWidths.Keys)
            {
                if (key == Common.ColumnName[Column.Destination]) continue;
                if (savedWidths[key] == 0) // There is at least one unmeasured column. I don't know why it happens but it's true.
                {
                    return;
                }
                fullColumnsWidth += savedWidths[key];
            }
            double utmostWidth = this.ActualWidth - fullColumnsWidth;
            if (utmostWidth > 0)
            {
                gridView.Columns[Common.UtmostColumn].Width = utmostWidth;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Event handler for Data Context Changed Event. Starts initializing MinWidth array if View is binded with view model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MVGTable_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is DeparturesViewModel)
            {
                SetMinWidth();
            }
        }
    }
}