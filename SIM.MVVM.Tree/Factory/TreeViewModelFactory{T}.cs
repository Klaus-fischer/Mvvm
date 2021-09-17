// <copyright file="TreeViewModelFactory{T}.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Declaration of a TreeViewModel factory.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target <see cref="TreeViewModel{T}"/>.</typeparam>
    /// <typeparam name="TSource">The type of the source collection.</typeparam>
    public interface ITreeViewModelFactory<TTarget, TSource>
        where TTarget : TreeViewModel<TTarget>
    {
        /// <summary>
        /// Builds the collection of tree view models.
        /// </summary>
        /// <param name="collection">The collection to inspect.</param>
        /// <returns>The converted collection.</returns>
        IEnumerable<TTarget> BuildTree(IEnumerable<TSource> collection);
    }

    /// <summary>
    /// Generic implementation of a <see cref="ITreeViewModelFactory{TTarget, TSource}"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target <see cref="TreeViewModel{T}"/>.</typeparam>
    /// <typeparam name="TSource">The type of the source collection.</typeparam>
    public abstract class TreeViewModelFactory<TTarget, TSource> : ITreeViewModelFactory<TTarget, TSource>
        where TTarget : TreeViewModel<TTarget>
    {
        /// <summary>
        /// Gets or sets the maximum recursion depth.
        /// </summary>
        public int MaxDepth { get; set; } = 10;

        /// <summary>
        /// Builds the collection of tree view models.
        /// </summary>
        /// <param name="collection">The collection to inspect.</param>
        /// <returns>The converted collection.</returns>
        public IEnumerable<TTarget> BuildTree(IEnumerable<TSource> collection)
        {
            foreach (var item in this.GetRootItems(collection))
            {
                if (this.Convert(item) is TTarget rootNode)
                {
                    this.BuildTree(rootNode, item, collection, 0);

                    yield return rootNode;
                }
            }
        }

        /// <summary>
        /// Converts a source item into target type..
        /// </summary>
        /// <param name="item">Item to convert.</param>
        /// <returns>The converted item.</returns>
        protected abstract TTarget? Convert(TSource item);

        /// <summary>
        /// Gets the root items of the collection.
        /// </summary>
        /// <param name="collection">Collection of items.</param>
        /// <returns>Only the root items.</returns>
        protected abstract IEnumerable<TSource> GetRootItems(IEnumerable<TSource> collection);

        /// <summary>
        /// Returns true if child relates to parent.
        /// </summary>
        /// <param name="parent">The parent to analyze.</param>
        /// <param name="child">The child to analyze.</param>
        /// <returns>True if child belongs to the parent item.</returns>
        protected abstract bool IsChild(TSource parent, TSource child);

        private void BuildTree(TTarget node, TSource source, IEnumerable<TSource> collection, int depth)
        {
            if (depth >= this.MaxDepth)
            {
                return;
            }

            foreach (var item in collection.Where(o => this.IsChild(source, o)))
            {
                if (this.Convert(item) is TTarget childNode)
                {
                    this.BuildTree(childNode, item, collection, depth + 1);

                    node.Add(childNode);
                }
            }
        }
    }
}
