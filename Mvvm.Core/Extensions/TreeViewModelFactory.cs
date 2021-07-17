// <copyright file="TreeViewModelFactory.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="TreeViewModel{T}"/> class.
    /// </summary>
    public static class TreeViewModelFactory
    {
        /// <summary>
        /// Delegate to determine if a parent contains a child.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
        /// <param name="parent">The parent node.</param>
        /// <param name="children">The possible child node.</param>
        /// <returns>True if the child belongs to the parent.</returns>
        public delegate bool IsChildren<T>(T parent, T children);

        /// <summary>
        /// Builds the tree out of a flat collection of tree view models.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
        /// <param name="collection">The node collection.</param>
        /// <param name="isRoot">Condition to determine the root nodes.</param>
        /// <param name="isChild">Condition to find the child nodes of an parent node.</param>
        /// <returns>Collection of root node items with children.</returns>
        public static IEnumerable<T> BuildTree<T>(IEnumerable<T> collection, Func<T, bool> isRoot, IsChildren<T> isChild)
            where T : TreeViewModel<T>
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (isRoot is null)
            {
                throw new ArgumentNullException(nameof(isRoot));
            }

            if (isChild is null)
            {
                throw new ArgumentNullException(nameof(isChild));
            }

            foreach (var node in collection)
            {
                if (node.Children.Any())
                {
                    throw new InvalidOperationException("Nodes must not have children.");
                }
            }

            foreach (var root in collection.Where(isRoot))
            {
                BuildTree(root, collection, isChild);

                yield return root;
            }
        }

        private static void BuildTree<T>(T node, IEnumerable<T> collection, IsChildren<T> isChild)
            where T : TreeViewModel<T>
        {
            node.AddRange(collection.Where(o => isChild(node, o)));

            foreach (var child in node.Children)
            {
                BuildTree(child, collection, isChild);
            }
        }
    }
}
