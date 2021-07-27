// <copyright file="IViewModel.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Declaration of an view model.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the collection of property monitors that listen to this view model properties.
        /// </summary>
        internal Collection<IPropertyMonitor> PropertyMonitors { get; }

        /// <summary>
        /// To invoke the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that was changed.</param>
        void OnPropertyChanged([CallerMemberName] string propertyName = "");
    }
}
