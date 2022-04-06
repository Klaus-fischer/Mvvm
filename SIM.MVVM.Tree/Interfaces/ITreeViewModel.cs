// <copyright file="ITreeViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Interface that describes a tree structure with view model behavior.
    /// </summary>
    public interface ITreeViewModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        /// Event to notify parent if a child's property was changed.
        /// </summary>
        event ChildPropertyChangedEventHandler? ChildPropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether children should be collapsed or not.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets a value indicating whether item is visible or not.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Gets a value indicating whether child is last of parents child list.
        /// </summary>
        bool IsLastItem { get; }

        /// <summary>
        /// Gets a value indicating whether this item has child items.
        /// </summary>
        bool HasChildren { get; }

        /// <summary>
        /// Gets a value indicating whether this item is a root item.
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Gets the rank of the item.
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// Gets the level of the item (adding 1 to rank).
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Gets command to search for sub items.
        /// </summary>
        ICommand? Expand { get; }

        /// <summary>
        /// Gets the parent of the node, or null if root node.
        /// </summary>
        ITreeViewModel? Parent { get; }

        /// <summary>
        /// Gets all children of the current tree view item.
        /// </summary>
        IReadOnlyCollection<ITreeViewModel> Children { get; }
    }
}
