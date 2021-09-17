// <copyright file="TreeItemCollectionSource.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using SIM.Mvvm.Tree;

    public abstract class TreeItemCollectionSource : INotifyCollectionChanged, IEnumerable<ITreeViewModel>
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public abstract IEnumerator<ITreeViewModel> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        protected void InvokeCollectionChanged()
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public static TreeItemCollectionSource? Create(object model)
        {
            if (model is ITreeViewModel tvm)
            {
                return new ItemCollectionSource(tvm);
            }

            if (model is IEnumerable ie)
            {
                return new ItemsCollectionSource(ie.OfType<ITreeViewModel>());
            }

            return null;
        }

        private class ItemCollectionSource : TreeItemCollectionSource
        {
            public ItemCollectionSource(ITreeViewModel treeViewModel)
            {
                this.TreeViewModel = treeViewModel ?? throw new ArgumentNullException(nameof(treeViewModel));
                this.TreeViewModel.CollectionChanged += (s, a) => this.InvokeCollectionChanged();
            }

            public ITreeViewModel TreeViewModel { get; }

            public override IEnumerator<ITreeViewModel> GetEnumerator()
            {
                return this.TreeViewModel.Get().GetEnumerator();
            }
        }

        private class ItemsCollectionSource : TreeItemCollectionSource
        {
            public ItemsCollectionSource(IEnumerable<ITreeViewModel> treeViewModel)
            {
                this.Collection = treeViewModel ?? throw new ArgumentNullException(nameof(treeViewModel));

                foreach (var item in treeViewModel)
                {
                    item.CollectionChanged += this.OnCollectionChanged;
                }
            }

            private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                foreach (var item in this.Collection.OfType<ITreeViewModel>())
                {
                    item.CollectionChanged -= this.OnCollectionChanged;
                    item.CollectionChanged += this.OnCollectionChanged;
                }

                this.InvokeCollectionChanged();
            }

            public IEnumerable<ITreeViewModel> Collection { get; }

            public override IEnumerator<ITreeViewModel> GetEnumerator()
            {
                return this.Collection.SelectMany(o => o.Get()).GetEnumerator();
            }
        }
    }
}
