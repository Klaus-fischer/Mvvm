// <copyright file="ITreeViewModelExtensions.cs" company="Klaus-Fischer-Inc">
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
    public static class ITreeViewModelExtensions
    {
        /// <summary>
        /// Concatenation of all items.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
        /// <param name="models">Models to show.</param>
        /// <param name="predicate">Predicate to filter items.</param>
        /// <returns>IEnumeration of items.</returns>
        public static IEnumerable<T> GetAll<T>(this IEnumerable<T> models, Predicate<T>? predicate = null)
            where T : ITreeViewModel
        {
            if (predicate == null)
            {
                predicate = t => true;
            }

            return models.SelectMany(x => x.GetAll(predicate));
        }

        /// <summary>
        /// Concatenation of child items.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
        /// <param name="model">Model to show.</param>
        /// <param name="predicate">Predicate to filter items.</param>
        /// <returns>IEnumeration of items.</returns>
        public static IEnumerable<T> GetAll<T>(this T model, Predicate<T>? predicate = null)
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
                foreach (var child in model.Children.OfType<T>().GetAll())
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Recursive call to expand all nodes.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
        /// <param name="node">The current node.</param>
        public static void ExpandAll<T>(this T node)
            where T : ITreeViewModel
        {
            node.IsExpanded = true;

            foreach (var child in node.Children)
            {
                child.ExpandAll();
            }
        }

        /// <summary>
        /// Checks if this <see cref="ITreeViewModel"/> matches the predicate or one of the parents.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
        /// <param name="node">The current node.</param>
        /// <param name="test">Condition to check.</param>
        /// <param name="parentOnly">Flag that indicates if only the parents of the current item will be checked.</param>
        /// <returns>True if condition matches.</returns>
        public static bool ContainsRootNode<T>(this T node, Predicate<T> test, bool parentOnly = false)
            where T : ITreeViewModel
        {
            if (!parentOnly && test(node))
            {
                return true;
            }

            if (node.Parent is T p)
            {
                return p.ContainsRootNode(test, false);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a string based of the parent nodes.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
        /// <param name="node">The current node.</param>
        /// <param name="select">The convert routine.</param>
        /// <param name="join">The string to join the items.</param>
        /// <returns></returns>
        public static string RootedJoinStrings<T>(this T node, Func<T, string> select, string join = " / ")
            where T : class, ITreeViewModel
        {
            var stack = new Stack<T>();

            stack.Push(node);

            var parent = node.Parent;

            while (parent is T p)
            {
                stack.Push(p);

                parent = parent.Parent;
            }

            var sb = new StringBuilder();

            while (stack.Any())
            {
                sb.Append(select(stack.Pop()));
                if (stack.Any())
                {
                    sb.Append(join);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns the parent with the expected rank.
        /// </summary>
        /// <typeparam name="T">Type of the tree view model.</typeparam>
        /// <param name="node">The node to start searching.</param>
        /// <param name="rank">The expected rank.</param>
        /// <returns>The parent node with the expected rank.</returns>
        public static T GetParentWithRank<T>(this T node, int rank)
            where T : ITreeViewModel
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (rank > node.Rank)
            {
                throw new ArgumentException($"Rank ({rank}) can not be greater than the rank ({node.Rank}) of the current node.");
            }

            if (node.Rank == rank)
            {
                return node;
            }
            else
            {
                if (node.Parent is T parent)
                {
                    return parent.GetParentWithRank(rank);
                }

                throw new InvalidOperationException($"Parent with rank {rank} could not be found.");
            }
        }

        /// <summary>
        /// Returns a rank indicator string.
        /// </summary>
        /// <param name="item">Item to indicate.</param>
        /// <param name="indention">The indention size.</param>
        /// <returns>Rank indicator string.</returns>
        public static string GetRankIndicator(this ITreeViewModel item, int indention = 1)
        {
            var dashes = string.Empty.PadLeft(indention - 1, '─');
            var spaces = string.Empty.PadLeft(indention - 1, ' ');

            var output = !item.IsRoot ? spaces : dashes + (item.HasChildren ? "┬" : "─") + dashes;

            if (item.IsRoot)
            {
                return output;
            }

            for (var i = 1; i < item.Rank; i++)
            {
                var p = item.GetParentWithRank(i);

                output += (p.IsLastItem ? " " : "│") + spaces;
            }

            output += (item.IsLastItem ? "└" : "├") + dashes;

            output += (item.HasChildren ? "┬" : "─") + dashes;

            return output;
        }
    }
}
