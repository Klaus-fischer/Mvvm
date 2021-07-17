// <copyright file="IViewModel.cs" company="Klaus-Fischer-Inc">
// Copyright (c) Klaus-Fischer-Inc. All rights reserved.
// </copyright>

namespace Mvvm.Core
{
    using System.ComponentModel;

    /// <summary>
    /// Declaration of an view model.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// To invoke the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that was changed.</param>
        void InvokeOnPropertyChanged(string propertyName);

        /// <summary>
        /// ViewModel will suppress <see cref="INotifyPropertyChanged"/> notifications on the property after calling this method.
        /// </summary>
        /// <param name="propertyName">Name of the property to suppress notifications.</param>
        /// <param name="currentValue">The current value of the property.</param>
        void SuppressNotifications(string propertyName, object currentValue);

        /// <summary>
        /// ViewModel will restore the <see cref="INotifyPropertyChanged"/> notifications on the properties.
        /// </summary>
        /// <param name="propertyName">Name of the property to restore notifications.</param>
        /// <param name="currentValue">The current value, to invoke <see cref="INotifyPropertyChanged"/> if values changed.</param>
        void RestoreNotifications(string propertyName, object currentValue);
    }
}
