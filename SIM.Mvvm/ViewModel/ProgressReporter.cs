// <copyright file="ProgressReporter.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    using System;
    using System.Threading;

    /// <summary>
    /// Class to handle progress reporting.
    /// </summary>
    public sealed class ProgressReporter : ViewModel, IProgressReporter
    {
        private readonly IProgress<int>? progressReporter;
        private readonly object interlock = new object();

        private int lastReportedProgress;
        private int maxProgress = 100;
        private int currentProgress = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressReporter"/> class.
        /// </summary>
        /// <param name="progress">Progress to report to.</param>
        public ProgressReporter(IProgress<int>? progress)
        {
            this.progressReporter = progress;
        }

        /// <summary>
        /// Gets the maximum value of current progress.
        /// </summary>
        public int MaxProgress
        {
            get => this.maxProgress;
            private set => this.SetPropertyValue(ref this.maxProgress, value);
        }

        /// <summary>
        /// Gets the current progress value.
        /// </summary>
        public int CurrentProgress
        {
            get => this.currentProgress;
            private set => this.SetPropertyValue(ref this.currentProgress, value);
        }

        /// <summary>
        /// Reset the progress to 0.
        /// </summary>
        /// <param name="maxProgress">The new maximum value.</param>
        public void Reset(int maxProgress = 100)
        {
            this.MaxProgress = maxProgress;
            this.CurrentProgress = 0;
            this.Increment(0);
        }

        /// <summary>
        /// Will set the progress to its maximum.
        /// </summary>
        public void Finish()
        {
            this.CurrentProgress = this.MaxProgress;
            this.Increment(0);
        }

        /// <summary>
        /// Will increment the current progress value.
        /// </summary>
        /// <param name="increment">Size of increment.</param>
        public void Increment(int increment = 1)
        {
            lock (this.interlock)
            {
                this.CurrentProgress += increment;

                int progressToReport = (this.CurrentProgress * 100) / this.MaxProgress;

                if (progressToReport != this.lastReportedProgress)
                {
                    this.progressReporter?.Report(progressToReport);
                    this.lastReportedProgress = progressToReport;
                }

                Monitor.PulseAll(this.interlock);
            }
        }
    }
}
