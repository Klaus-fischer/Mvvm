// <copyright file="TreeViewModelExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="TreeViewModel{T}"/> class.
    /// </summary>
    public static class TreeViewModelExtensions
    {
        /// <summary>
        /// Adds single items to the <see cref="TreeViewModel{T}.Children"/> collection.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="model">Root model to add items.</param>
        /// <param name="item">Item to add.</param>
        /// <param name="items">Optional additional items.</param>
        public static void Add<T>(this T model, T item, params T[] items)
            where T : TreeViewModel<T>
        {
            IEnumerable<T> itemsToAdd = new List<T>() { item };

            if (items?.Any() == true)
            {
                itemsToAdd = itemsToAdd.Concat(items);
            }

            model.AddRange(itemsToAdd);
        }

        /// <summary>
        /// Adds single items to the <see cref="TreeViewModel{T}.Children"/> collection.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="model">Root model to add items.</param>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="item">Item to add.</param>
        /// <param name="items">Optional additional items.</param>
        public static void Insert<T>(this T model, int index, T item, params T[] items)
            where T : TreeViewModel<T>
        {
            IEnumerable<T> itemsToAdd = new List<T>() { item };

            if (items?.Any() == true)
            {
                itemsToAdd = itemsToAdd.Concat(items);
            }

            model.InsertRange(index, itemsToAdd);
        }

        /// <summary>
        /// Concatenation of all items.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="models">Models to show.</param>
        /// <param name="predicate">Predicate to filter items.</param>
        /// <returns>IEnumeration of items.</returns>
        public static IEnumerable<T> Get<T>(this IEnumerable<T> models, Predicate<T>? predicate = null)
            where T : ITreeViewModel
        {
            if (predicate == null)
            {
                predicate = t => true;
            }

            return models.SelectMany(x => x.Get(predicate));
        }

        /// <summary>
        /// Concatenation of child items.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="model">Model to show.</param>
        /// <param name="predicate">Predicate to filter items.</param>
        /// <returns>IEnumeration of items.</returns>
        public static IEnumerable<T> Get<T>(this T model, Predicate<T>? predicate = null)
            where T : ITreeViewModel
        {
            if (predicate == null)
            {
                predicate = t => true;
            }

            if (predicate(model))
            {
                yield return model;
            }

            if (model.Children is not null)
            {
                foreach (var child in model.Children.OfType<T>().Get(predicate))
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Checks if this <see cref="TreeViewModel{T}"/> matches the predicate or one of the parents.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="model">Model to show.</param>
        /// <param name="test">Condition to check.</param>
        /// <param name="parentOnly">Flag that indicates if only the parents of the current item will be checked.</param>
        /// <returns>True if condition matches.</returns>
        public static bool ContainsRootNode<T>(this T model, Predicate<T> test, bool parentOnly = false)
            where T : TreeViewModel<T>
        {
            if (!parentOnly && test(model))
            {
                return true;
            }

            if (model.Parent == null)
            {
                return false;
            }
            else
            {
                return model.Parent.ContainsRootNode(test, false);
            }
        }

        /// <summary>
        /// Checks if this <see cref="TreeViewModel{T}"/> matches the predicate or one of the parents.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="model">Model to show.</param>
        /// <param name="test">Condition to check.</param>
        /// <param name="parentOnly">Flag that indicates if only the parents of the current item will be checked.</param>
        /// <returns>True if condition matches.</returns>
        public static TreeViewModel<T>? FindRoot<T>(
            this T model,
            Predicate<T> test,
            bool parentOnly = false)
            where T : TreeViewModel<T>
        {
            if (!parentOnly && test(model))
            {
                return model;
            }

            if (model.Parent == null)
            {
                return null;
            }
            else
            {
                return model.Parent.FindRoot(test, false);
            }
        }

        /// <summary>
        /// Expands all nodes in the tree.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="model">Model to expand recursive.</param>
        public static void ExpandAll<T>(this T model)
            where T : TreeViewModel<T>
        {
            model.IsExpanded = true;

            foreach (var child in model[..])
            {
                child.ExpandAll();
            }
        }

        /// <summary>
        /// Expands all child nodes of the item.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="model">Model to expand.</param>
        public static void ExpandFirstLevel<T>(this T model)
            where T : TreeViewModel<T>
        {
            model.IsExpanded = true;
            foreach (var child in model[..])
            {
                child.IsExpanded = true;
            }
        }

        /// <summary>
        /// Returns a string that describes the path of the node through the tree.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="item">Item to show.</param>
        /// <param name="select">Pointer to the string value to connect.</param>
        /// <param name="join">The join value.</param>
        /// <returns>The concatenated string.</returns>
        public static string RootedJoinStrings<T>(this T item, Func<T, string> select, string join = " / ")
            where T : TreeViewModel<T>
        {
            var stack = new Stack<T>();

            stack.Push(item);

            var parent = item.Parent;

            while (parent != null)
            {
                stack.Push(parent);

                parent = parent.Parent;
            }

            return string.Join(join, stack.Select(select));
        }

        /// <summary>
        /// Returns all parents the tree up to the root node.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="item">Start node (will not be in result).</param>
        /// <returns>Collection of tree nodes.</returns>
        public static IEnumerable<T> GetParents<T>(this T item)
            where T : ITreeViewModel
        {
            while (item.Parent is T parent)
            {
                yield return parent;

                item = parent;
            }
        }
    }
}
