// <copyright file="IProgressReporter.cs" company="SIM Automation">
// Copyright (c) SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Mvvm
{
    /// <summary>
    /// Declaration of an progress reporter.
    /// </summary>
    public interface IProgressReporter
    {
        /// <summary>
        /// Gets the current progress value.
        /// </summary>
        int CurrentProgress { get; }

        /// <summary>
        /// Gets the maximum value of current progress.
        /// </summary>
        int MaxProgress { get; }

        /// <summary>
        /// Will set the progress to its maximum.
        /// </summary>
        void Finish();

        /// <summary>
        /// Will increment the current progress value.
        /// </summary>
        /// <param name="increment">Size of increment.</param>
        void Increment(int increment = 1);

        /// <summary>
        /// Reset the progress to 0.
        /// </summary>
        /// <param name="maxProgress">The new maximum value.</param>
        void Reset(int maxProgress = 100);
    }
}