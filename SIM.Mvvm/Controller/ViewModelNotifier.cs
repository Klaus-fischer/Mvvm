// <copyright file="ViewModelNotifier.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System.Collections.ObjectModel;

    internal class ViewModelNotifier
    {
        readonly IViewModel viewModel;

        readonly Collection<string> properties = new();

        public ViewModelNotifier(IViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CheckViewModel(IViewModel target)
            => ReferenceEquals(this.viewModel, target);

        public void AddProperty(string property)
        {
            if (!this.properties.Contains(property))
            {
                this.properties.Add(property);
            }
        }

        public void RemoveProperty(string property)
        {
            this.properties.Remove(property);
        }

        public void InvokePropertyChanged()
        {
            foreach (var property in this.properties)
            {
                this.viewModel.OnPropertyChanged(property);
            }
        }
    }
}
