// <copyright file="TreeViewModelExtensions.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Extension methods for <see cref="TreeViewModel{T}"/> class.
    /// </summary>
    public static class TreeViewModelExtensions
    {
        /// <summary>
        /// Adds single items to the <see cref="TreeViewModel{T}.Children"/> collection.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
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
        /// <typeparam name="T">Type of the tree view model.</typeparam>
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
        /// Checks if this <see cref="TreeViewModel{T}"/> matches the predicate or one of the parents.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
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
    }
}
