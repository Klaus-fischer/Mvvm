// <copyright file="IFilterableTreeViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    /// <summary>
    /// Extends <see cref="ITreeViewModel"/> by filter option.
    /// </summary>
    /// <typeparam name="T">Type of the tree node to filter.</typeparam>
    public interface IFilterableTreeViewModel<T>
        where T : ITreeViewModel
    {
        /// <summary>
        /// Gets or sets the filter for the view model.
        /// </summary>
        ITreeNodeFilter<T>? Filter { get; set; }
    }
}