// <copyright file="ViewModelExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows.Input;

    internal static class ViewModelExtensions
    {
        /// <summary>
        /// Register all property dependencies marked with the <see cref="DependsOnAttribute"/>.
        /// Registers all callback methods marked with <see cref="CallOnPropertyChangedAttribute"/>.
        /// </summary>
        /// <param name="viewModel">The view model to register.</param>
        public static void RegisterDependencies(this IViewModel viewModel)
        {
            RegisterDependentProperties(viewModel);
            RegisterDependentMethods(viewModel);
        }

        private static void RegisterDependentProperties(IViewModel viewModel)
        {
            foreach (var property in viewModel.GetType().GetProperties())
            {
                if (property.GetCustomAttribute<DependsOnAttribute>() is not DependsOnAttribute dependsOn)
                {
                    continue;
                }

                if (typeof(ICommand).IsAssignableFrom(property.PropertyType))
                {
                    var propertyMonitor = viewModel.GetPropertyMonitor(property.Name);
                    propertyMonitor.OnPropertyChanged += UpdateCommandPropertyChanged;
                }

                foreach (var propertyName in dependsOn.PropertyNames)
                {
                    var propertyMonitor = viewModel.GetPropertyMonitor(propertyName);
                    propertyMonitor.RegisterViewModelProperty(viewModel, property.Name);
                }
            }
        }

        private static void UpdateCommandPropertyChanged(object sender, AdvancedPropertyChangedEventArgs e)
        {
            if (sender is IViewModel viewModel)
            {
                foreach (var monitor in viewModel.PropertyMonitors)
                {
                    if (e.Before is INotifyCommand oldCommand)
                    {
                        monitor.UnregisterCommand(oldCommand);
                    }

                    if (e.After is INotifyCommand newCommand)
                    {
                        monitor.RegisterCommand(newCommand);
                    }
                }
            }
        }

        private static IPropertyMonitor GetPropertyMonitor(this IViewModel viewModel, string propertyName)
        {
            if (viewModel.PropertyMonitors.FirstOrDefault(o => o.PropertyName == propertyName) is IPropertyMonitor monitor)
            {
                return monitor;
            }

            monitor = CreatePropertyMonitor(viewModel, propertyName);
            viewModel.PropertyMonitors.Add(monitor);
            return monitor;
        }

        private static IPropertyMonitor CreatePropertyMonitor(IViewModel viewModel, string propertyName)
        {
            var property = viewModel.GetType().GetProperty(propertyName);

            var propertyType = property.PropertyType;

            var value = Expression.Property(Expression.Constant(viewModel), propertyName);
            var lambda = Expression.Lambda(value);
            var getter = lambda.Compile();

            var propertyMonitorType_T = Type.GetType("SIM.Mvvm.PropertyMonitor`1")
                                            .MakeGenericType(propertyType);

            return (IPropertyMonitor)Activator.CreateInstance(
                propertyMonitorType_T,
                viewModel,
                propertyName,
                getter,
                null);
        }

        private static void RegisterDependentMethods(IViewModel viewModel)
        {
            foreach (var method in viewModel.GetType().GetMethods())
            {
                if (method.GetCustomAttribute<CallOnPropertyChangedAttribute>() is not CallOnPropertyChangedAttribute callOn)
                {
                    continue;
                }

                if (!viewModel.TryAssignCallback(method, callOn, out var message))
                {
                    throw new InvalidOperationException($"Could not assign call back {message}");
                }
            }
        }

        private static bool TryAssignCallback(this IViewModel viewModel, MethodInfo method, CallOnPropertyChangedAttribute callOn, out string? errorMessage)
        {
            var parameters = method.GetParameters();

            errorMessage = null;
            string prefix = $"{method.Name}({string.Join(", ", parameters.Select(o => o.ParameterType.Name))})\n";

            if (parameters.Length > 0)
            {
                if (parameters.Length != 2)
                {
                    errorMessage = prefix + "Only zero or two parameters expected.";
                }
                else if (parameters[0].ParameterType != typeof(object))
                {
                    errorMessage = prefix + "First parameter must have type 'object'";
                }
                else if (!typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
                {
                    errorMessage = $"{prefix}Second parameter must be assignable to type '{nameof(AdvancedPropertyChangedEventArgs)}'";
                }
            }

            if (method.ReturnType != typeof(void))
            {
                errorMessage = $"Return type must be void";
            }

            if (errorMessage is not null)
            {
                return false;
            }

            foreach (var propertyName in callOn.PropertyNames)
            {
                if (parameters.Length == 0)
                {
                    viewModel.GetPropertyMonitor(propertyName).OnPropertyChangedCallback
                        += (Action)method.CreateDelegate(typeof(Action), viewModel);
                }
                else
                {
                    viewModel.GetPropertyMonitor(propertyName).OnPropertyChanged
                        += (EventHandler<AdvancedPropertyChangedEventArgs>)method.CreateDelegate(
                            typeof(EventHandler<AdvancedPropertyChangedEventArgs>),
                            viewModel);
                }
            }

            return true;
        }
    }
}
