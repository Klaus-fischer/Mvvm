// <copyright file="ITreeNodeFilter.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    /// <summary>
    /// Declaration of an tree node filter.
    /// </summary>
    /// <typeparam name="T">Type of the tree node to evaluate.</typeparam>
    public interface ITreeNodeFilter<T>
        where T : ITreeViewModel
    {
        /// <summary>
        /// Filter method for <see cref="IFilterableTreeViewModel{T}"/>.
        /// </summary>
        /// <param name="node">Tree node to inspect.</param>
        /// <returns>True if item should be visible, otherwise false.</returns>
        public bool Filter(T node);
    }
}
