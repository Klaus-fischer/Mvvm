// <copyright file="DesignTimeFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree.UI
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Input;

#if DEBUG

    public class DesignTimeFactory
    {
        private static ITreeViewModel? designTree;

        public static ITreeViewModel DesignTree => designTree ?? CreateDesignTree();

        private static ITreeViewModel CreateDesignTree()
        {
            var root = new TreeViewItem()
            {
                IsExpanded = true,
                IsVisible = true,
                IsLastItem = true,
                HasChildren = true,
                IsRoot = true,
                Rank = 0,
                Level = 1,
            };

            var child = new TreeViewItem()
            {
                IsExpanded = true,
                IsVisible = true,
                IsLastItem = true,
                HasChildren = true,
                IsRoot = false,
                Rank = 0,
                Level = 1,
                Parent = root,
            };

            root.Children = new List<TreeViewItem>() { child };

            var grandChild = new TreeViewItem()
            {
                IsExpanded = true,
                IsVisible = true,
                IsLastItem = false,
                HasChildren = true,
                IsRoot = false,
                Rank = 1,
                Level = 2,
                Parent = child,
            };

            child.Children = new List<TreeViewItem>() { grandChild };

            var grandgrandChild = new TreeViewItem()
            {
                IsExpanded = true,
                IsVisible = true,
                IsLastItem = false,
                HasChildren = true,
                IsRoot = false,
                Rank = 2,
                Level = 3,
                Parent = grandChild,
            };

            grandChild.Children = new List<TreeViewItem>() { grandgrandChild };

            return designTree = grandgrandChild;
        }

        internal class TreeViewItem : ITreeViewModel
        {
            public event ChildPropertyChangedEventHandler? ChildPropertyChanged;

            public event PropertyChangedEventHandler? PropertyChanged;

            public event NotifyCollectionChangedEventHandler? CollectionChanged;

            public bool IsExpanded { get; set; }

            public bool IsVisible { get; set; }

            public bool IsLastItem { get; set; }

            public bool HasChildren { get; set; }

            public bool IsRoot { get; set; }

            public int Rank { get; set; }

            public int Level { get; set; }

            public ICommand? Expand { get; set; }

            public ITreeViewModel? Parent { get; set; }

            public IEnumerable<ITreeViewModel> Children { get; set; }
        }
    }

#endif

}