// <copyright file="NodeControl.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm.Tree.UI
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using SIM.Mvvm.Tree;

    /// <summary>
    /// Code behind for NodeControl.
    /// </summary>
    public class NodeControl : FrameworkElement
    {
        /// <summary>
        /// DependencyProperty for Rank property.
        /// </summary>
        public static readonly DependencyProperty RankProperty =
            DependencyProperty.Register(
                "Rank",
                typeof(int),
                typeof(NodeControl),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// DependencyProperty for Indent property.
        /// </summary>
        public static readonly DependencyProperty IndentProperty =
            DependencyProperty.Register(
                "Indent",
                typeof(double),
                typeof(NodeControl),
                new FrameworkPropertyMetadata(
                    20.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Foreground.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register(
                "Foreground",
                typeof(Brush),
                typeof(NodeControl),
                new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets the value of the <see cref="ForegroundProperty"/>.
        /// </summary>
        public Brush Foreground
        {
            get { return (Brush)this.GetValue(ForegroundProperty); }
            set { this.SetValue(ForegroundProperty, value); }
        }

        public ITreeViewModel? Model => this.DataContext as ITreeViewModel;

        /// <summary>
        /// Gets or sets the rank of the item.
        /// </summary>
        public int Rank
        {
            get { return (int)this.GetValue(RankProperty); }
            set { this.SetValue(RankProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the indent (per rank).
        /// </summary>
        public double Indent
        {
            get { return (double)this.GetValue(IndentProperty); }
            set { this.SetValue(IndentProperty, value); }
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == IsVisibleProperty)
            {
                if (this.IsVisible)
                {
                    this.InvalidateVisual();
                }
            }

            if (e.Property == DataContextProperty)
            {
                if (e.OldValue is ITreeViewModel old)
                {
                    old.PropertyChanged -= this.DataContextPropertyChanged;
                }

                if (e.NewValue is ITreeViewModel ti)
                {
                    ti.PropertyChanged += this.DataContextPropertyChanged;
                    this.Rank = ti.Rank;
                }

                this.InvalidateVisual();
            }

            base.OnPropertyChanged(e);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                return new Size(0, 0);
            }

            return new Size((this.Rank + 1) * this.Indent, 20);
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                return new Size(0, 0);
            }

            return new Size((this.Rank + 1) * this.Indent, arrangeBounds.Height);
        }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect r;

            if (this.Model?.IsVisible != true)
            {
                return;
            }

            // |  |  |
            // |  |  |
            for (var i = this.Rank - 1; i > 0; i--)
            {
                var p = this.GetParentWithRank(this.Model, i);

                if (!p.IsLastItem)
                {
                    r = new Rect(
                        this.Indent * (i - 0.5) - 1,
                        0,
                        2,
                        this.ActualHeight);

                    drawingContext.DrawRectangle(
                        this.Foreground,
                        null,
                        r);
                }
                else
                {
                    if (i == 1)
                    {
                        break;
                    }
                }
            }

            // |  |  |  |
            // |  |  |
            if (!this.Model.IsRoot)
            {
                r = new Rect(
                          this.Indent * (this.Rank - 0.5) - 1,
                          0,
                          2,
                          this.ActualHeight / 2);
                drawingContext.DrawRectangle(this.Foreground, null, r);
            }

            // |  |  |  |
            // |  |  |  |
            if (!this.Model.IsRoot)
            {
                if (!this.Model.IsLastItem)
                {
                    r = new Rect(
                         this.Indent * (this.Rank - 0.5) - 1,
                         this.ActualHeight / 2,
                         2,
                         this.ActualHeight / 2);
                    drawingContext.DrawRectangle(this.Foreground, null, r);
                }
                else
                {
                    drawingContext.DrawEllipse(
                       this.Foreground,
                       null,
                       new Point(this.Indent * (this.Rank - 0.5), this.ActualHeight / 2),
                       1,
                       1);
                }
            }

            // |  |  |  |__
            // |  |  |  |
            if (!this.Model.IsRoot)
            {
                r = new Rect(
                    this.Indent * (this.Rank - 0.5),
                    this.ActualHeight / 2 - 1,
                    this.Indent,
                    2);
                drawingContext.DrawRectangle(this.Foreground, null, r);
            }

            // |  |  |  |__
            // |  |  |  |  |
            if (this.Model.HasChildren && this.Model.IsExpanded)
            {
                if (this.Model.IsRoot)
                {
                    r = new Rect(
                        this.Indent * 0.5 - 1,
                        this.ActualHeight / 2,
                        2,
                        this.ActualHeight / 2);
                }
                else
                {
                    r = new Rect(
                        this.Indent * (this.Rank + 0.5) - 1,
                        this.ActualHeight / 2,
                        2,
                        this.ActualHeight / 2);
                }

                drawingContext.DrawRectangle(this.Foreground, null, r);
            }

            base.OnRender(drawingContext);
        }

        private void DataContextPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ITreeViewModel.IsVisible) ||
                e.PropertyName == nameof(ITreeViewModel.HasChildren) ||
                e.PropertyName == nameof(ITreeViewModel.IsExpanded) ||
                e.PropertyName == nameof(ITreeViewModel.IsLastItem))
            {
                this.Dispatcher.InvokeAsync(this.InvalidateVisual, DispatcherPriority.Background);
            }

            if (e.PropertyName == nameof(ITreeViewModel.IsExpanded))
            {
                this.Dispatcher.InvokeAsync(this.InvalidateMeasure, DispatcherPriority.Background);
            }
        }

        private ITreeViewModel GetParentWithRank(ITreeViewModel i, int rank)
        {
            if (i.Rank == rank)
            {
                return i;
            }
            else
            {
                return this.GetParentWithRank(i.Parent ?? throw new ArgumentNullException(), rank);
            }
        }
    }
}
