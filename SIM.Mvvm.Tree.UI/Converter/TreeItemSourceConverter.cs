// <copyright file="TreeItemSourceConverter.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree.UI
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    public class TreeItemSourceConverter : MarkupExtension, IValueConverter
    {
        private ICollectionView? view;

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = TreeItemCollectionSource.Create(value);

            if (items is not null && CollectionViewSource.GetDefaultView(items) is ICollectionView view)
            {
                view.Filter = this.Filter;

                items.CollectionChanged += (s, a) => view.Refresh();

                this.view = view;
                return this.view;
            }

            return value;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool Filter(object obj)
        {
            if (obj is ITreeViewModel tvm)
            {
                return tvm.IsVisible;
            }

            return false;
        }
    }
}
