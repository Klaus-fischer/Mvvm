// <copyright file="TreeViewModel{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using SIM.Mvvm;
    using SIM.Mvvm.Expressions;

    /// <summary>
    /// Extends view model by tree structure.
    /// </summary>
    /// <typeparam name="T">Concrete type of the tree node.</typeparam>
    public abstract class TreeViewModel<T> : ViewModel, ITreeViewModel
        where T : TreeViewModel<T>
    {
        private bool isExpanded;
        private T? parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewModel{T}"/> class.
        /// </summary>
        protected TreeViewModel()
        {
            this.PropertyChanged += this.NotifyParentOnPropertyChanged;
        }

        /// <inheritdoc/>
        public event ChildPropertyChangedEventHandler? ChildPropertyChanged;

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        public T? Parent
        {
            get => this.parent;
            protected set => this.SetPropertyValue(() => this.parent, value);
        }

        /// <inheritdoc/>
        ITreeViewModel? ITreeViewModel.Parent => this.Parent;

        /// <inheritdoc/>
        public IReadOnlyCollection<ITreeViewModel> Children => this.ChildrenList;

        /// <summary>
        /// Gets or sets a value indicating whether children should be collapsed or not.
        /// </summary>
        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.SetPropertyValue(() => this.isExpanded, value);
        }

        /// <summary>
        /// Gets a value indicating whether item is visible or not.
        /// </summary>
        [DependsOn(nameof(Parent))]
        public virtual bool IsVisible => this.Parent?.IsExpanded ?? true;

        /// <summary>
        /// Gets a value indicating whether child is last of parents child list.
        /// </summary>
        [DependsOn(nameof(Parent))]
        public virtual bool IsLastItem => this.Parent?.ChildrenList.LastOrDefault() == this;

        /// <summary>
        /// Gets a value indicating whether this item has child items.
        /// </summary>
        [DependsOn(nameof(Children))]
        public virtual bool HasChildren => this.ChildrenList.Any();

        /// <summary>
        /// Gets a value indicating whether this item is a root item.
        /// </summary>
        [DependsOn(nameof(Parent))]
        public bool IsRoot => this.Parent == null;

        /// <summary>
        /// Gets the rank of the item.
        /// </summary>
        [DependsOn(nameof(Parent))]
        public int Rank => this.Parent?.Rank + 1 ?? 0;

        /// <summary>
        /// Gets or sets command to search for sub items.
        /// </summary>
        public ICommand? Expand { get; protected set; }

        /// <summary>
        /// Gets a list of sub nodes.
        /// </summary>
        protected List<T> ChildrenList { get; } = new List<T>();

#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Gets the child with the requested index.
        /// </summary>
        /// <param name="index">Index of the child.</param>
        /// <returns>The child with the index.</returns>
        public T? this[Index index]
        {
            get
            {
                var i = index.GetOffset(this.Children.Count);
                if (i >= 0 && i < this.Children.Count)
                {
                    return this.ChildrenList[i];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Enumerates the children inside the range.
        /// </summary>
        /// <param name="range">Range of children.</param>
        /// <returns>Enumeration of children in range.</returns>
        public IEnumerable<T> this[Range range]
        {
            get
            {
                var (offset, length) = range.GetOffsetAndLength(this.Children.Count);

                for (int i = 0; i < length; i++)
                {
                    var index = offset + i;

                    if (index < this.Children.Count)
                    {
                        yield return this.ChildrenList[index];
                    }
                }
            }
        }
#endif

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
                this.ChildrenList.Add(item);
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

            this.ChildrenList.InsertRange(index, items);

            this.BubbleInvokeCollectionChanged();
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

        private void NotifyParentOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(this.isExpanded))
            {
                this.HandleIsExpandedChanged();
            }

            if (args.PropertyName == nameof(this.IsVisible))
            {
                this.IsVisiblePropertyChanged();
            }

            this.Parent?.ChildPropertyChanged?.Invoke(
                this.Parent,
                new ChildPropertyChangedEventArgs(this, args.PropertyName));
        }

        private void HandleIsExpandedChanged()
        {
            if (this.IsExpanded && !this.HasChildren)
            {
                this.isExpanded = false;

                return;
            }

            this.ChildrenList.ForEach(o => o.IsVisiblePropertyChanged());

            this.BubbleInvokeCollectionChanged();
        }

        private void IsVisiblePropertyChanged()
        {
            if (!this.IsVisible)
            {
                this.IsExpanded = false;
            }
        }
    }
}
