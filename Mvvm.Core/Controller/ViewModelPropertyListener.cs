// <copyright file="ViewModelPropertyListener.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Observer class to monitor properties of a view model and
    /// fire <see cref="INotifyPropertyChanged.PropertyChanged"/> if depend property changed.
    /// </summary>
    internal class ViewModelPropertyListener
    {
        private readonly IViewModel viewModel;
        private readonly string propertyName;
        private readonly string[] dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelPropertyListener"/> class.
        /// </summary>
        /// <param name="target">Target view model to observe.</param>
        /// <param name="viewModel">View model to notify.</param>
        /// <param name="propertyName">Property name to call <see cref="IViewModel.InvokeOnPropertyChanged(string)"/> with.</param>
        /// <param name="dependencies">Collection of dependencies to monitor.</param>
        public ViewModelPropertyListener(
            INotifyPropertyChanged target,
            IViewModel viewModel,
            string propertyName,
            params string[] dependencies)
        {
            this.viewModel = viewModel;
            this.propertyName = propertyName;
            this.dependencies = dependencies;
            target.PropertyChanged += this.NotifyViewModelOnTargetPropertyChanged;
        }

        private void NotifyViewModelOnTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.dependencies.Contains(e.PropertyName))
            {
                this.viewModel.InvokeOnPropertyChanged(this.propertyName);
            }
        }
    }
}
