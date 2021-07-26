// <copyright file="PropertyMonitorExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class PropertyMonitorExtensions
    {
        public static T RegisterCallback<T>(this T monitor, Action callback)
            where T : IPropertyMonitor
        {
            monitor.OnPropertyChangedCallback += callback;
            return monitor;
        }

        public static T UnregisterCallback<T>(this T monitor, Action callback)
           where T : IPropertyMonitor
        {
            monitor.OnPropertyChangedCallback -= callback;
            return monitor;
        }

        public static T RegisterCallback<T>(this T monitor, EventHandler<AdvancedPropertyChangedEventArgs> callback)
            where T : IPropertyMonitor
        {
            monitor.OnPropertyChanged += callback;
            return monitor;
        }

        public static T UnregisterCallback<T>(this T monitor, EventHandler<AdvancedPropertyChangedEventArgs> callback)
            where T : IPropertyMonitor
        {
            monitor.OnPropertyChanged -= callback;
            return monitor;
        }

        public static T RegisterCommands<T>(
            this T monitor,
            params ICommandInvokeCanExecuteChangedEvent[] commands)
            where T : IPropertyMonitor
        {
            foreach (var command in commands)
            {
                monitor.RegisterCommand(command);
            }

            return monitor;
        }

        public static T UnregisterCommands<T>(
            this T monitor,
            params ICommandInvokeCanExecuteChangedEvent[] commands)
            where T : IPropertyMonitor
        {
            foreach (var command in commands)
            {
                monitor.UnregisterCommand(command);
            }

            return monitor;
        }

        public static T NotifyAlso<T, TProperty>(
            this T monitor,
            Expression<Func<TProperty>> expression)
            where T : IPropertyMonitor
        {
            if (expression.Body is MemberExpression me)
            {
                if (me.Expression is ConstantExpression ce && ce.Value is IViewModel viewModel)
                {
                    monitor.RegisterViewModelProperty(viewModel, me.Member.Name);
                }
            }

            return monitor;
        }

        public static T DependsOn<T, TProperty>(
            this T monitor,
            Expression<Func<TProperty>> expression)
            where T : IPropertyMonitor
        {
            if (expression.Body is MemberExpression me &&
                monitor.Target is IViewModel target)
            {
                if (me.Expression is MemberExpression me1)
                {
                    var member = me1.Member is PropertyInfo pi ? pi.GetValue(target) :
                        me1.Member is FieldInfo fi ? fi.GetValue(target) : null;

                    if (member is IViewModel viewModel)
                    {
                        viewModel[me.Member.Name].RegisterViewModelProperty(target, monitor.PropertyName);
                    }
                    else if (member is INotifyPropertyChanged vm)
                    {
                        PropertyMonitorFactory.Create(vm, me.Member.Name).RegisterViewModelProperty(target, monitor.PropertyName);
                    }
                }
            }

            return monitor;
        }


        public static T RegisterViewModelProperties<T>(
            this T monitor,
            IViewModel target,
            params string[] propertyNames)
            where T : IPropertyMonitor
        {
            foreach (var name in propertyNames)
            {
                monitor.RegisterViewModelProperty(target, name);
            }

            return monitor;
        }

        public static T UnregisterViewModelProperties<T>(
            this T monitor,
            IViewModel target,
            params string[] propertyNames)
            where T : IPropertyMonitor
        {
            foreach (var name in propertyNames)
            {
                monitor.UnregisterViewModelProperty(target, name);
            }

            return monitor;
        }
    }
}
