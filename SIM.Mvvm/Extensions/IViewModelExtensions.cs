﻿// <copyright file="IViewModelExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

#pragma warning disable SA1004 // Documentation lines should begin with single space

namespace SIM.Mvvm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Extension class to extend <see cref="IViewModel"/> (<see cref="INotifyPropertyChanged"/>).
    /// </summary>
    public static class IViewModelExtensions
    {
        /// <summary>
        /// Raises <see cref="IViewModel.InvokeOnPropertyChanged(string)"/> if a dependent property was changed.
        /// </summary>
        /// <typeparam name="T">Type of the Property.</typeparam>
        /// <param name="viewModel">The view model the monitored property belongs to.</param>
        /// <param name="mainViewModel">The view model to notify.</param>
        /// <param name="mainPropertyName">The name that get fired, if a dependency was changed.</param>
        /// <param name="dependencies">The dependencies are properties of the view model.</param>
        /// public class MyViewModel : IViewModel
        /// {
        ///     public int Property => MySubViewModel.Property1 + MySubViewModel.Property2;
        ///     public IViewModel MySubViewModel { get; }
        ///
        ///     MyViewModel(IViewModel subViewModel)
        ///     {
        ///         MySubViewModel = subViewModel.RegisterDependencies(this, nameof(Property), "Property1", "Property2"));
        ///     }
        /// }
        public static void RegisterDependencies<T>(
            this T viewModel,
            IViewModel mainViewModel,
            string mainPropertyName,
            params string[] dependencies)
            where T : IViewModel
        {
            if (dependencies is null || !dependencies.Any())
            {
                throw new ArgumentException("dependencies must contain at least one PropertyName");
            }

            _ = new ViewModelPropertyListener(
                viewModel ?? throw new ArgumentNullException(nameof(viewModel)),
                mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel)),
                mainPropertyName ?? throw new ArgumentNullException(nameof(mainPropertyName)),
                dependencies);
        }
    }
}
