// <copyright file="ViewModelNotifier.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Handler to notify a view model.
    /// </summary>
    internal class ViewModelNotifier
    {
        private readonly WeakReference<IViewModel> viewModelReference;

        private readonly Collection<string> properties = new Collection<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelNotifier"/> class.
        /// </summary>
        /// <param name="viewModel">View model to notify.</param>
        public ViewModelNotifier(IViewModel viewModel)
        {
            this.viewModelReference = new WeakReference<IViewModel>(viewModel);
        }

        /// <summary>
        /// Checks if view model is alive.
        /// </summary>
        /// <returns>True if view model is alive.</returns>
        public bool IsAlive()
            => this.viewModelReference.TryGetTarget(out var _);

        /// <summary>
        /// Handler to validate if the current notifier belongs to the view model.
        /// </summary>
        /// <param name="target">View model to check.</param>
        /// <returns>True if view model is alive and the reference equals.</returns>
        public bool CheckViewModel(IViewModel target)
            => this.viewModelReference.TryGetTarget(out var vm) && ReferenceEquals(vm, target);

        /// <summary>
        /// Adds a property to notification list.
        /// </summary>
        /// <param name="property">Name of the property.</param>
        public void AddProperty(string property)
        {
            if (!this.properties.Contains(property))
            {
                this.properties.Add(property);
            }
        }

        /// <summary>
        /// Removes a property from notification list.
        /// </summary>
        /// <param name="property">Name of the property.</param>
        public void RemoveProperty(string property)
        {
            this.properties.Remove(property);
        }

        /// <summary>
        /// Raises <see cref="IViewModel.OnPropertyChanged(string)"/> for every registered property.
        /// </summary>
        public void InvokePropertyChanged()
        {
            if (this.viewModelReference.TryGetTarget(out var viewModel))
            {
                foreach (var property in this.properties)
                {
                    viewModel.OnPropertyChanged(property);
                }
            }
        }
    }
}
