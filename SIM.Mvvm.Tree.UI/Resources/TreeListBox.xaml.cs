// <copyright file="TreeListBox.xaml.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree.UI
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///  Code behind file of TreeListBox.xaml.
    /// </summary>
    public partial class TreeListBox : ResourceDictionary
    {
        /// <summary>
        /// Assign parent items on scroll.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="e">Event arguments.</param>
        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                if (listBox.ItemContainerGenerator.Items == null || listBox.ItemContainerGenerator.Items.Count == 0)
                {
                    return;
                }

                int firstItem = (int)e.VerticalOffset;

                ITreeViewModel item = (ITreeViewModel)listBox.ItemContainerGenerator.Items[firstItem];

                var parentCollection = new List<object>();

                while (item.Parent is not null)
                {
                    item = item.Parent;
                    parentCollection.Insert(0, item);
                }

                if (listBox.Template.FindName("ParentsView", listBox) is ItemsControl parents)
                {
                    parents.ItemsSource = parentCollection;
                }
            }
        }

        private void ParentsView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ItemsControl ic && ic.TemplatedParent is ListBox listBox)
            {
                object frameworkElement = e.OriginalSource;

                while (!(frameworkElement is FrameworkElement))
                {
                    frameworkElement = LogicalTreeHelper.GetParent(frameworkElement as DependencyObject);

                    if (frameworkElement is null)
                    {
                        return;
                    }
                }

                object? clickedItem = (frameworkElement as FrameworkElement)?.DataContext;

                listBox.ScrollIntoView(clickedItem);
            }
        }
    }
}
