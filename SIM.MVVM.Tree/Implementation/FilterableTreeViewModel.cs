// <copyright file="FilterableTreeViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    using System.Linq;

    /// <summary>
    /// Extends <see cref="TreeViewModel{T}"/> by filtering implementation.
    /// </summary>
    /// <typeparam name="T">Concrete type of the node tree.</typeparam>
    public class FilterableTreeViewModel<T> : TreeViewModel<T>, IFilterableTreeViewModel<T>
        where T : FilterableTreeViewModel<T>
    {
        private bool wasExpanded;
        private ITreeNodeFilter<T>? filter;
        private bool? isVisible;
        private bool? hasChildren;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterableTreeViewModel{T}"/> class.
        /// </summary>
        public FilterableTreeViewModel()
        {
        }

        /// <inheritdoc/>
        public ITreeNodeFilter<T>? Filter
        {
            get => this.filter;
            set => this.ApplyFilter(value);
        }

        /// <inheritdoc/>
        public override bool HasChildren => this.hasChildren ?? base.HasChildren;

        /// <inheritdoc/>
        public override bool IsVisible => base.IsVisible && (this.isVisible ?? true);

        /// <inheritdoc/>
        public override bool IsLastItem => this.Parent?[..].LastOrDefault(o => o.IsVisible) == this;

        private void ApplyFilter(ITreeNodeFilter<T>? value)
        {
            this.Backup(value);

            this.AssignFilterRecursive(value);

            if (value is null)
            {
                this.IsExpanded = this.wasExpanded;
                this.isVisible = null;
                this.hasChildren = null;
            }
            else
            {
                this.hasChildren = this[..].Any(o => o.IsVisible);
                this.isVisible = (this.hasChildren == true) || value.Filter((T)this);
                this.IsExpanded = this.hasChildren == true;
            }

            this.RaisePropertyChangedNotifications();
        }

        private void Backup(ITreeNodeFilter<T>? value)
        {
            this.SuppressNotifications(nameof(this.IsExpanded), this.IsExpanded);
            this.SuppressNotifications(nameof(this.IsVisible), this.IsVisible);

            if (value is not null)
            {
                // backup IsExpanded property.
                if (this.filter is null)
                {
                    this.wasExpanded = this.IsExpanded;
                }

                // expand all if filter selected.
                this.IsExpanded = true;
            }
        }

        private void AssignFilterRecursive(ITreeNodeFilter<T>? value)
        {
            // apply filter recursive.
            foreach (var item in this[..])
            {
                item.Filter = value;
            }

            this.filter = value;
        }

        private void RaisePropertyChangedNotifications()
        {
            this.RestoreNotifications(nameof(this.IsExpanded), this.IsExpanded);
            this.RestoreNotifications(nameof(this.IsVisible), this.IsVisible);

            if (this.IsRoot)
            {
                this.InvokeCollectionChanged();
                this.IsExpanded = true;
            }
            else
            {
                this.OnPropertyChanged(nameof(this.IsExpanded), null, null);
            }
        }
    }
}
