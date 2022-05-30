// <copyright file="ViewModelExtensions.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;

    /// <summary>
    /// Extends <see cref="ViewModel"/> by some Expression calls.
    /// </summary>
    public static class ViewModelExtensions
    {
        /// <summary>
        /// ViewModel will suppress <see cref="INotifyPropertyChanged"/> notifications on the property after calling this method.
        /// </summary>
        /// <example>
        /// ...
        /// public string MyProperty { get; set; }
        ///
        /// public void DoSomoeThing()
        /// {
        ///     this.SuppressNotifications(() => this.MyProperty);
        ///
        ///     // does the same as:
        ///     this.SuppressNotifications(nameof(this.MyProperty), this.MyProperty);
        /// }
        /// ...
        /// </example>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="viewModel">The view model to extend.</param>
        /// <param name="expression">The PropertyExpression to the property to suppress.</param>
        public static void SuppressNotifications<TProperty>(this ViewModel viewModel, Expression<Func<TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                var propertyName = me.Member.Name;
                var value = expression.Compile().Invoke();
                viewModel.SuppressNotifications(propertyName, value);

                return;
            }

            throw new InvalidOperationException($"Expression must point to an Property of the current ViewModel.");
        }

        /// <summary>
        /// ViewModel will restore the <see cref="INotifyPropertyChanged"/> notifications on the properties.
        /// </summary>
        /// <example>
        /// ...
        /// public string MyProperty { get; set; }
        ///
        /// public void DoSomoeThing()
        /// {
        ///     this.RestoreNotifications(() => this.MyProperty);
        ///
        ///     // does the same as:
        ///     this.RestoreNotifications(nameof(this.MyProperty), this.MyProperty);
        /// }
        /// ...
        /// </example>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="viewModel">The view model to extend.</param>
        /// <param name="expression">The PropertyExpression to the property to restore.</param>
        public static void RestoreNotifications<TProperty>(this ViewModel viewModel, Expression<Func<TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.MemberAccess && expression.Body is MemberExpression me)
            {
                var propertyName = me.Member.Name;
                var value = expression.Compile().Invoke();
                viewModel.RestoreNotifications(propertyName, value);

                return;
            }

            throw new InvalidOperationException($"Expression must point to an Property of the current ViewModel.");
        }

        /// <summary>
        /// ViewModel will suppress <see cref="INotifyPropertyChanged"/> notifications on the property inside the action.
        /// The notifications will be restored after that.
        /// </summary>
        /// <example>
        /// ...
        /// public string MyProperty { get; set; }
        ///
        /// public void DoSomoeThing()
        /// {
        ///     this.SuppressNotificationsInside(() => this.MyProperty, () =>
        ///     {
        ///         // do something special in here.
        ///     });
        ///
        ///     // does the same as:
        ///     // this.SuppressNotifications(nameof(this.MyProperty), this.MyProperty);
        ///
        ///     // do something special in here.
        ///
        ///     // this.RestoreNotifications(nameof(this.MyProperty), this.MyProperty);
        /// }
        /// ...
        /// </example>
        /// <typeparam name="TProperty">Type of the property.</typeparam>
        /// <param name="viewModel">The view model to extend.</param>
        /// <param name="expression">The PropertyExpression to the property to suppress.</param>
        /// <param name="action">The action to run while notifications are suppressed.</param>
        public static void SuppressNotificationsInside<TProperty>(this ViewModel viewModel, Expression<Func<TProperty>> expression, Action action)
        {
            viewModel.SuppressNotifications(expression);
            action();
            viewModel.RestoreNotifications(expression);
        }
    }
}
