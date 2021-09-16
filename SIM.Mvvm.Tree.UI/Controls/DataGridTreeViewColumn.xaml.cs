// <copyright file="DataGridTreeViewColumn.xaml.cs" company="SIM Automation">
// Copyright (c) 2020 SIM Automation. All rights reserved.
// </copyright>

namespace SIM.Core.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Code behind for DataGridTreeViewColumnTemplate.xaml.
    /// </summary>
    public partial class DataGridTreeViewColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridTreeViewColumn"/> class.
        /// </summary>
        public DataGridTreeViewColumn()
        {
            this.InitializeComponent();
            this.IsReadOnly = true;
            this.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
            this.CanUserResize = false;
            this.CanUserReorder = false;
            this.CanUserSort = false;
        }
    }
}
