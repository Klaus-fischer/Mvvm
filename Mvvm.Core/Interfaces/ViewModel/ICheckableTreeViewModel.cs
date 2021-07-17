// <copyright file="ICheckableTreeViewModel.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
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
