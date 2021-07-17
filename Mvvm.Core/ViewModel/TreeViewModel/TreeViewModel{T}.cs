// <copyright file="TreeViewModel{T}.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    public abstract class TreeViewModel<T> : ViewModel, ITreeViewModel
        where T : TreeViewModel<T>
    {
        /// <summary>
        /// Gets or sets a list of sub nodes.
        /// </summary>
        private readonly List<T> children = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewModel{T}"/> class.
        /// </summary>
        protected TreeViewModel()
        {
            this.AdvancedPropertyChanged += this.HandleIsExpandedChanged;
            this.PropertyChanged += this.HandleIsVisibleChanged;
            this.PropertyChanged += this.InvokeParentOnChildPropertyChanged;
        }

        ///// <summary>
        ///// Gets the child with the requested index.
        ///// </summary>
        ///// <param name="index">Index of the child.</param>
        ///// <returns>The child with the index.</returns>
        //public T? this[Index index]
        //{
        //    get
        //    {
        //        var i = index.GetOffset(this.Children.Count);
        //        if (i >= 0 && i < this.Children.Count)
        //        {
        //            return this.Children[i];
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Enumerates the children inside the range.
        ///// </summary>
        ///// <param name="range">Range of children.</param>
        ///// <returns>Enumeration of children in range.</returns>
        //public IEnumerable<T> this[Range range]
        //{
        //    get
        //    {
        //        var pos = range.GetOffsetAndLength(this.Children.Count);

        //        for (var i = 0; i < pos.Length; i++)
        //        {
        //            var index = pos.Offset + i;

        //            if (index < this.Children.Count)
        //            {
        //                yield return this.Children[index];
        //            }
        //        }
        //    }
        //}

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        public T? Parent { get; protected set; }

        /// <inheritdoc/>
        ITreeViewModel? ITreeViewModel.Parent => this.Parent;

        /// <inheritdoc/>
        IEnumerable<ITreeViewModel> ITreeViewModel.Children => this.children;

        /// <summary>
        /// Gets the collection of children.
        /// </summary>
        public ICollection<T> Children => this.children;

        /// <summary>
        /// Gets or sets a value indicating whether children should be collapsed or not.
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Gets a value indicating whether item is visible or not.
        /// </summary>
        public bool IsVisible => this.Parent?.IsExpanded ?? true;

        /// <summary>
        /// Gets a value indicating whether child is last of parents child list.
        /// </summary>
        public bool IsLastItem => this.Parent?.children.LastOrDefault() == this;

        /// <summary>
        /// Gets a value indicating whether this item has child items.
        /// </summary>
        [DependsOn(nameof(children))]
        public bool HasChildren => this.children.Any();

        /// <summary>
        /// Gets a value indicating whether this item is a root item.
        /// </summary>
        public bool IsRoot => this.Parent == null;

        /// <summary>
        /// Gets the rank of the item.
        /// </summary>
        public int Rank => this.Parent?.Rank + 1 ?? 0;

        /// <summary>
        /// Gets the level of the item (adding 1 to rank).
        /// </summary>
        public int Level => this.Rank + 1;

        /// <summary>
        /// Gets or sets command to search for sub items.
        /// </summary>
        public ICommand? Expand { get; protected set; }

        /// <summary>
        /// Adds a range of items to the children collection.
        /// Will set the <see cref="Parent"/> property of the child items.
        /// </summary>
        /// <param name="collection">Items to add.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                item.Parent = (T)this;
                this.children.Add(item);
            }

            this.BubbleInvokeCollectionChanged();
        }

        /// <summary>
        /// Inserts items to the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="items">The collection whose elements should be inserted.
        /// The collection itself cannot be null, but it can contain elements that are null,
        /// if type T is a reference type.</param>
        public void InsertRange(int index, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.Parent = (T)this;
            }

            this.children.InsertRange(index, items);

            this.BubbleInvokeCollectionChanged();
        }

        /// <summary>
        /// To bubble a collection changed event from child to parent.
        /// </summary>
        /// <param name="sender">Sender of the event (the Child).</param>
        /// <param name="propertyName">Name of the property that got changed on the child.</param>
        protected virtual void OnChildPropertyChanged(object sender, string propertyName)
        {
        }

        /// <summary>
        /// Bubbles the collection changed event.
        /// </summary>
        protected void BubbleInvokeCollectionChanged()
        {
            this.InvokeCollectionChanged();

            if (this.Parent != null)
            {
                this.Parent.BubbleInvokeCollectionChanged();
            }
        }

        /// <summary>
        /// Invocation call of <see cref="CollectionChanged"/> event.
        /// </summary>
        protected void InvokeCollectionChanged()
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void HandleIsExpandedChanged(object? sender, AdvancedPropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.IsExpanded))
            {
                if (this.IsExpanded && !this.HasChildren)
                {
                    this.SuppressNotifications(nameof(this.IsExpanded), 0);

                    this.IsExpanded = false;

                    this.RestoreNotifications(nameof(this.IsExpanded), 0);

                    return;
                }

                this.children.ForEach(o => o.IsVisiblePropertyChanged());

                this.BubbleInvokeCollectionChanged();
            }
        }

        private void HandleIsVisibleChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.IsVisible))
            {
                this.IsVisiblePropertyChanged();
            }
        }

        private void IsVisiblePropertyChanged()
        {
            if (!this.IsVisible)
            {
                this.IsExpanded = false;
            }
        }

        private void InvokeParentOnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Parent?.OnChildPropertyChanged(this, e.PropertyName);
        }
    }
}
