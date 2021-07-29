// <copyright file="IViewModelExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows.Input;
    using SIM.Mvvm.Expressions;

    internal static class IViewModelExtensions
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

                var pmFactory = PropertyMonitorFactory.Current;

                foreach (var propertyName in dependsOn.PropertyNames)
                {
                    var propertyMonitor = pmFactory.GetPropertyMonitor(viewModel, propertyName);
                    propertyMonitor.RegisterViewModelProperty(viewModel, property.Name);

                    if (typeof(ICommand).IsAssignableFrom(property.PropertyType))
                    {
                        var factory = CommandNotifierFactory.Current;
                        var cmdNotifier = factory.GetCommandNotifier(viewModel, property.Name);
                        propertyMonitor.Call(cmdNotifier.NotifyCommandChanged);
                    }
                }
            }
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

            var factory = PropertyMonitorFactory.Current;

            foreach (var propertyName in callOn.PropertyNames)
            {
                if (parameters.Length == 0)
                {
                    factory.GetPropertyMonitor(viewModel, propertyName).Call(
                        (Action)method.CreateDelegate(typeof(Action), viewModel));
                }
                else
                {
                    factory.GetPropertyMonitor(viewModel, propertyName).Call(
                        (EventHandler<AdvancedPropertyChangedEventArgs>)method.CreateDelegate(typeof(EventHandler<AdvancedPropertyChangedEventArgs>), viewModel));
                }
            }

            return true;
        }
    }
}
