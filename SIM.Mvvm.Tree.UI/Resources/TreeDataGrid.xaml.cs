// <copyright file="TreeListBox.xaml.cs" company="SIM Automation">
// Copyright (c) 2020 SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Core.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    ///  Code behind file of TreeListBox.xaml.
    /// </summary>
    public partial class TreeDataGrid : ResourceDictionary
    {
        /// <summary>
        /// Assign parent items on scroll.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event arguments.</param>
        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                if (dg.ItemContainerGenerator.Items == null || dg.ItemContainerGenerator.Items.Count == 0)
                {
                    return;
                }

                int firstItem = (int)e.VerticalOffset;

                dynamic item = dg.ItemContainerGenerator.Items[firstItem];

                var parentCollection = new List<object>();

                while (item.Parent != null)
                {
                    item = item.Parent;
                    parentCollection.Insert(0, item);
                }

                if (dg.Template.FindName("DG_ScrollViewer", dg) is ScrollViewer sv)
                {
                    if (sv.Template.FindName("PART_ParentsContentPresenter", sv) is ItemsControl parentsView)
                    {
                        parentsView.ItemsSource = parentCollection;
                    }
                }
            }
        }

        private void OnPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
