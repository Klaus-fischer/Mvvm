﻿// <copyright file="TreeViewModel{T}.cs" company="SIM Automation">
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
        /// <summary>
        /// Gets or sets a list of sub nodes.
        /// </summary>
        private readonly List<T> children = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewModel{T}"/> class.
        /// </summary>
        protected TreeViewModel()
        {
            _ = this.Listen(() => this.IsExpanded).Call(this.HandleIsExpandedChanged);
            _ = this.Listen(() => this.IsVisible).Call(this.IsVisiblePropertyChanged);
            this.PropertyChanged += this.NotifyParentOnPropertyChanged;
        }

        /// <inheritdoc/>
        public event ChildPropertyChangedEventHandler? ChildPropertyChanged;

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
        /// Gets or sets a value indicating whether children should be collapsed or not.
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Gets a value indicating whether item is visible or not.
        /// </summary>
        public virtual bool IsVisible => this.Parent?.IsExpanded ?? true;

        /// <summary>
        /// Gets a value indicating whether child is last of parents child list.
        /// </summary>
        public virtual bool IsLastItem => this.Parent?.children.LastOrDefault() == this;

        /// <summary>
        /// Gets a value indicating whether this item has child items.
        /// </summary>
        [DependsOn(nameof(children))]
        public virtual bool HasChildren => this.children.Any();

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
        /// Gets the child with the requested index.
        /// </summary>
        /// <param name="index">Index of the child.</param>
        /// <returns>The child with the index.</returns>
        public T? this[Index index]
        {
            get
            {
                var i = index.GetOffset(this.children.Count);
                if (i >= 0 && i < this.children.Count)
                {
                    return this.children[i];
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
                var (offset, length) = range.GetOffsetAndLength(this.children.Count);

                for (int i = 0; i < length; i++)
                {
                    var index = offset + i;

                    if (index < this.children.Count)
                    {
                        yield return this.children[index];
                    }
                }
            }
        }

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
            this.Parent?.ChildPropertyChanged?.Invoke(
                this.Parent,
                new ChildPropertyChangedEventArgs(this, args.PropertyName));
        }

        private void HandleIsExpandedChanged(object? sender, AdvancedPropertyChangedEventArgs e)
        {
            if (this.IsExpanded && !this.HasChildren)
            {
                this.SuppressNotificationsInside(() => this.IsExpanded, () =>
                {
                    this.IsExpanded = false;
                });

                return;
            }

            this.children.ForEach(o => o.IsVisiblePropertyChanged());

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