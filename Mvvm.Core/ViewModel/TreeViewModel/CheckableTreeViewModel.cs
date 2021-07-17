// <copyright file="CheckableTreeViewModel.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.Linq;

    /// <summary>
    /// Extends <see cref="TreeViewModel{T}"/> by checkable property.
    /// </summary>
    /// <typeparam name="T">Type of concrete Tree view model.</typeparam>
    public abstract class CheckableTreeViewModel<T> : TreeViewModel<T>, ICheckableTreeViewModel
        where T : CheckableTreeViewModel<T>
    {
        private bool isChecked;

        /// <summary>
        ///  Gets or sets a value indicating whether item is checked.
        /// </summary>
        public bool? IsChecked
        {
            get
            {
                if (!this.HasChildren)
                {
                    return this.isChecked;
                }

                var allChecked = this.Children.OfType<CheckableTreeViewModel<T>>()
                    .All(o => o.IsChecked == true);

                if (allChecked)
                {
                    return true;
                }

                var allUnChecked = this.Children.OfType<CheckableTreeViewModel<T>>()
                    .All(o => o.IsChecked == false);

                if (allUnChecked)
                {
                    return false;
                }

                return null;
            }

            set
            {
                var newValue = value ?? this.IsChecked != true;

                if (!this.HasChildren)
                {
                    this.isChecked = newValue;
                }
                else
                {
                    foreach (var child in this.Children.OfType<CheckableTreeViewModel<T>>())
                    {
                        child.IsChecked = newValue;
                    }
                }

                ((IViewModel)this).InvokeOnPropertyChanged(nameof(this.IsChecked));
            }
        }

        /// <inheritdoc/>
        protected override void OnChildPropertyChanged(object sender, string propertyName)
        {
            if (propertyName == nameof(this.IsChecked))
            {
                ((IViewModel)this).InvokeOnPropertyChanged(nameof(this.IsChecked));
            }
        }
    }
}
