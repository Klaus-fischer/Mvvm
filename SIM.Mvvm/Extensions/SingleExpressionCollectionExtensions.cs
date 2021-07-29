// <copyright file="SingleExpressionCollectionExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Expressions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows.Input;
    using SIM.Mvvm;

    /// <summary>
    /// Extends <see cref="ExpressionExtensions"/> by collection calls.
    /// </summary>
    public static class SingleExpressionCollectionExtensions
    {
        /// <summary>
        /// Adds an parameterless callback for a property changed event.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        /// public string MySecondProperty { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty) // single property to listen to
        ///         .Call(MyPropertyChanged, SomethingChanged); // all callbacks to register for each property changed event.
        /// }
        ///
        /// private void MyPropertyChanged()
        /// {
        ///     ...
        /// }
        ///
        /// private void SomethingChanged()
        /// {
        ///     ...
        /// }
        /// ...
        /// </example>
        /// <param name="monitor">The collection of property monitors to register the callbacks to.</param>
        /// <param name="action">The callback action.</param>
        /// <param name="additionalActions">Some additional actions to register.</param>
        /// <returns>The property monitor for chaining.</returns>
        public static IPropertyMonitor Call(
            this IPropertyMonitor monitor,
            Action action,
            params Action[] additionalActions)
        {
            monitor.Call(action);

            foreach (var optional in additionalActions)
            {
                monitor.Call(optional);
            }

            return monitor;
        }

        /// <summary>
        /// Adds an <see cref="EventHandler{TEventArgs}"/> callback for a property changed event.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        /// public string MySecondProperty { get; set; }
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty) // single property to listen to
        ///         .Call(this.MyPropertyChanged, this.SomePropertyChanged);    // all callbacks to register for each property changed event.
        /// }
        ///
        /// private void MyPropertyChanged(object sender, <see cref="AdvancedPropertyChangedEventArgs"/> arguments)
        /// {
        ///     ...
        /// }
        ///
        /// private void SomePropertyChanged(object sender, <see cref="AdvancedPropertyChangedEventArgs"/> arguments)
        /// {
        ///     ...
        /// }
        /// ...
        /// </example>
        /// <param name="monitor">The collection of property monitors to register the callbacks to.</param>
        /// <param name="eventHandler">The callback event handler.</param>
        /// <param name="additionalEventHandler">Some additional event handler.</param>
        /// <returns>The property monitor for chaining.</returns>
        public static IPropertyMonitor Call(
            this IPropertyMonitor monitor,
            EventHandler<AdvancedPropertyChangedEventArgs> eventHandler,
            params EventHandler<AdvancedPropertyChangedEventArgs>[] additionalEventHandler)
        {
            monitor.Call(eventHandler);

            foreach (var optional in additionalEventHandler)
            {
                monitor.Call(optional);
            }

            return monitor;
        }

        /// <summary>
        /// Adds a property notification to a different view model.
        /// </summary>
        /// <example>
        /// public string MyProperty { get; set; }
        /// public string MySecondProperty { get; set; }
        ///
        /// public string SomeOtherProperty => this.MyProperty + "Something" + this.MySecondProperty;
        ///
        /// public MyViewModel()
        /// {
        ///     this.Listen(() => this.MyProperty) // single property to listen to
        ///         .Notify(() => this.SomeOtherProperty);
        /// }
        /// ...
        /// </example>
        /// <param name="monitor">The collection of property monitors to register the callbacks to.</param>
        /// <param name="expressions">The collection of expression to the property to notify.
        /// Must be a MemberExpression to a <see cref="IViewModel"/> object.</param>
        /// <returns>The collection of property monitors.</returns>
        public static IPropertyMonitor Notify(
            this IPropertyMonitor monitor,
            params Expression<Func<object>>[] expressions)
        {
            foreach (var expression in expressions)
            {
                var exp = ConvertExpression(expression, out var propertyType);

                if (GetGenericMethodInfo(nameof(ExpressionExtensions.Notify), new Type[] { propertyType }) is MethodInfo methodInfo)
                {
                    methodInfo.Invoke(null, new object[] { monitor, exp });
                }
            }

            return monitor;
        }

        internal static dynamic ConvertExpression(Expression<Func<object>> lambda, out Type propertyType)
        {
            var expression = lambda.Body;

            if (expression.NodeType == ExpressionType.Convert && expression is UnaryExpression ue)
            {
                expression = ue.Operand;
            }

            if (expression.NodeType == ExpressionType.MemberAccess && expression is MemberExpression me)
            {
                propertyType = me.Member is PropertyInfo pi ? pi.PropertyType
                             : throw new InvalidOperationException("Expression must show a Property");

                return Expression.Lambda(me);
            }

            throw new InvalidOperationException();
        }

        internal static MethodInfo? GetGenericMethodInfo(string methodName, Type[] genericTypes)
        {
            var methodInfo = typeof(ExpressionExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(o => o.IsGenericMethod && o.Name == methodName)
                .Where(o => o.GetGenericArguments().Length == genericTypes.Length)
                .FirstOrDefault();

            methodInfo = methodInfo?.MakeGenericMethod(genericTypes);

            return methodInfo;
        }
    }
}
