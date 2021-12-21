// <copyright file="CheckableTreeViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
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

                var allChecked = ((ITreeViewModel)this).Children.OfType<T>().All(o => o.IsChecked == true);

                if (allChecked)
                {
                    return true;
                }

                var allUnChecked = ((ITreeViewModel)this).Children.OfType<T>().All(o => o.IsChecked == false);

                if (allUnChecked)
                {
                    return false;
                }

                return null;
            }

            set
            {
                var oldValue = this.IsChecked;
                var newValue = value ?? this.IsChecked != true;

                if (!this.HasChildren)
                {
                    this.isChecked = newValue;
                }
                else
                {
                    foreach (var child in ((ITreeViewModel)this).Children.OfType<T>())
                    {
                        child.IsChecked = newValue;
                    }
                }

                this.OnPropertyChanged(nameof(this.IsChecked), oldValue, newValue);
            }
        }
    }
}
