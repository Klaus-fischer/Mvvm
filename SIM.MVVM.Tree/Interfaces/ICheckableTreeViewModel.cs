// <copyright file="ICheckableTreeViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    /// <summary>
    /// Extends <see cref="ITreeViewModel"/> by checkable property.
    /// </summary>
    public interface ICheckableTreeViewModel : ITreeViewModel
    {
        /// <summary>
        ///  Gets or sets a value indicating whether item is checked.
        /// </summary>
        bool? IsChecked { get; set; }
    }
}
