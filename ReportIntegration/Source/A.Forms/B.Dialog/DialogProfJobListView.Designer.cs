namespace Sgs.ReportIntegration
{
    partial class DialogProfJobListView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogProfJobListView));
            this.toDateEdit = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dateCheck = new System.Windows.Forms.CheckBox();
            this.fromDateEdit = new System.Windows.Forms.DateTimePicker();
            this.resetButton = new System.Windows.Forms.Button();
            this.findButton = new System.Windows.Forms.Button();
            this.itemNoEdit = new DevExpress.XtraEditors.TextEdit();
            this.reportGrid = new DevExpress.XtraGrid.GridControl();
            this.reportGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.reportRegTimeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.reportAreaColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.reportJobNoColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.reportItemNoColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.reportProductColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.reportClientColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.itemNoLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.areaCombo = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.jobNoEdit = new DevExpress.XtraEditors.TextEdit();
            this.JobNoLabel = new System.Windows.Forms.Label();
            this.OmNoLabel = new System.Windows.Forms.Label();
            this.OmNoEdit = new DevExpress.XtraEditors.TextEdit();
            this.reportFileNoColumn = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bgPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemNoEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobNoEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OmNoEdit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // bgPanel
            // 
            this.bgPanel.Controls.Add(this.OmNoLabel);
            this.bgPanel.Controls.Add(this.OmNoEdit);
            this.bgPanel.Controls.Add(this.areaCombo);
            this.bgPanel.Controls.Add(this.label19);
            this.bgPanel.Controls.Add(this.okButton);
            this.bgPanel.Controls.Add(this.JobNoLabel);
            this.bgPanel.Controls.Add(this.itemNoLabel);
            this.bgPanel.Controls.Add(this.cancelButton);
            this.bgPanel.Controls.Add(this.reportGrid);
            this.bgPanel.Controls.Add(this.toDateEdit);
            this.bgPanel.Controls.Add(this.label3);
            this.bgPanel.Controls.Add(this.dateCheck);
            this.bgPanel.Controls.Add(this.fromDateEdit);
            this.bgPanel.Controls.Add(this.resetButton);
            this.bgPanel.Controls.Add(this.jobNoEdit);
            this.bgPanel.Controls.Add(this.findButton);
            this.bgPanel.Controls.Add(this.itemNoEdit);
            this.bgPanel.Size = new System.Drawing.Size(1018, 550);
            // 
            // toDateEdit
            // 
            this.toDateEdit.CustomFormat = "yyyy-MM-dd";
            this.toDateEdit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toDateEdit.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.toDateEdit.Location = new System.Drawing.Point(186, 10);
            this.toDateEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.toDateEdit.Name = "toDateEdit";
            this.toDateEdit.Size = new System.Drawing.Size(102, 21);
            this.toDateEdit.TabIndex = 2;
            this.toDateEdit.ValueChanged += new System.EventHandler(this.toDateEdit_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(167, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 15);
            this.label3.TabIndex = 107;
            this.label3.Text = "~";
            // 
            // dateCheck
            // 
            this.dateCheck.Checked = true;
            this.dateCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dateCheck.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateCheck.Location = new System.Drawing.Point(9, 12);
            this.dateCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateCheck.Name = "dateCheck";
            this.dateCheck.Size = new System.Drawing.Size(52, 19);
            this.dateCheck.TabIndex = 0;
            this.dateCheck.TabStop = false;
            this.dateCheck.Tag = "";
            this.dateCheck.Text = "Date";
            this.dateCheck.UseVisualStyleBackColor = true;
            // 
            // fromDateEdit
            // 
            this.fromDateEdit.CustomFormat = "yyyy-MM-dd";
            this.fromDateEdit.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fromDateEdit.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fromDateEdit.Location = new System.Drawing.Point(61, 10);
            this.fromDateEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fromDateEdit.Name = "fromDateEdit";
            this.fromDateEdit.Size = new System.Drawing.Size(102, 21);
            this.fromDateEdit.TabIndex = 1;
            this.fromDateEdit.ValueChanged += new System.EventHandler(this.fromDateEdit_ValueChanged);
            // 
            // resetButton
            // 
            this.resetButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resetButton.Image = ((System.Drawing.Image)(resources.GetObject("resetButton.Image")));
            this.resetButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.resetButton.Location = new System.Drawing.Point(938, 10);
            this.resetButton.Name = "resetButton";
            this.resetButton.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.resetButton.Size = new System.Drawing.Size(68, 24);
            this.resetButton.TabIndex = 6;
            this.resetButton.Text = "     Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // findButton
            // 
            this.findButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.findButton.Image = ((System.Drawing.Image)(resources.GetObject("findButton.Image")));
            this.findButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.findButton.Location = new System.Drawing.Point(868, 10);
            this.findButton.Name = "findButton";
            this.findButton.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.findButton.Size = new System.Drawing.Size(68, 24);
            this.findButton.TabIndex = 5;
            this.findButton.Text = "    Find";
            this.findButton.UseVisualStyleBackColor = true;
            this.findButton.Click += new System.EventHandler(this.findButton_Click);
            // 
            // itemNoEdit
            // 
            this.itemNoEdit.EditValue = "";
            this.itemNoEdit.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.itemNoEdit.Location = new System.Drawing.Point(604, 10);
            this.itemNoEdit.Name = "itemNoEdit";
            this.itemNoEdit.Properties.Appearance.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemNoEdit.Properties.Appearance.Options.UseFont = true;
            this.itemNoEdit.Properties.MaxLength = 20;
            this.itemNoEdit.Size = new System.Drawing.Size(92, 22);
            this.itemNoEdit.TabIndex = 4;
            // 
            // reportGrid
            // 
            this.reportGrid.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.reportGrid.Location = new System.Drawing.Point(8, 38);
            this.reportGrid.LookAndFeel.SkinName = "DevExpress Style";
            this.reportGrid.LookAndFeel.UseDefaultLookAndFeel = false;
            this.reportGrid.MainView = this.reportGridView;
            this.reportGrid.Name = "reportGrid";
            this.reportGrid.Size = new System.Drawing.Size(998, 462);
            this.reportGrid.TabIndex = 7;
            this.reportGrid.TabStop = false;
            this.reportGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.reportGridView});
            // 
            // reportGridView
            // 
            this.reportGridView.Appearance.EvenRow.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.EvenRow.Options.UseFont = true;
            this.reportGridView.Appearance.FixedLine.Options.UseFont = true;
            this.reportGridView.Appearance.FocusedCell.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.FocusedCell.Options.UseFont = true;
            this.reportGridView.Appearance.FocusedRow.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.FocusedRow.Options.UseFont = true;
            this.reportGridView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.HeaderPanel.Options.UseFont = true;
            this.reportGridView.Appearance.HideSelectionRow.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.HideSelectionRow.Options.UseFont = true;
            this.reportGridView.Appearance.OddRow.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.OddRow.Options.UseFont = true;
            this.reportGridView.Appearance.Preview.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.Preview.Options.UseFont = true;
            this.reportGridView.Appearance.Row.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.Row.Options.UseFont = true;
            this.reportGridView.Appearance.SelectedRow.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.SelectedRow.Options.UseFont = true;
            this.reportGridView.Appearance.ViewCaption.Font = new System.Drawing.Font("Arial", 9F);
            this.reportGridView.Appearance.ViewCaption.Options.UseFont = true;
            this.reportGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.reportRegTimeColumn,
            this.reportAreaColumn,
            this.reportJobNoColumn,
            this.reportItemNoColumn,
            this.reportFileNoColumn,
            this.reportProductColumn,
            this.reportClientColumn});
            this.reportGridView.CustomizationFormBounds = new System.Drawing.Rectangle(1710, 580, 210, 186);
            this.reportGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.reportGridView.GridControl = this.reportGrid;
            this.reportGridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.reportGridView.Name = "reportGridView";
            this.reportGridView.OptionsBehavior.Editable = false;
            this.reportGridView.OptionsBehavior.ReadOnly = true;
            this.reportGridView.OptionsHint.ShowColumnHeaderHints = false;
            this.reportGridView.OptionsHint.ShowFooterHints = false;
            this.reportGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.reportGridView.OptionsView.ColumnAutoWidth = false;
            this.reportGridView.OptionsView.ShowGroupPanel = false;
            this.reportGridView.OptionsView.ShowIndicator = false;
            this.reportGridView.Tag = 1;
            this.reportGridView.DoubleClick += new System.EventHandler(this.reportGridView_DoubleClick);
            // 
            // reportRegTimeColumn
            // 
            this.reportRegTimeColumn.AppearanceCell.Options.UseTextOptions = true;
            this.reportRegTimeColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.reportRegTimeColumn.Caption = "DateTime";
            this.reportRegTimeColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            this.reportRegTimeColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.reportRegTimeColumn.FieldName = "registered";
            this.reportRegTimeColumn.MaxWidth = 124;
            this.reportRegTimeColumn.MinWidth = 124;
            this.reportRegTimeColumn.Name = "reportRegTimeColumn";
            this.reportRegTimeColumn.OptionsColumn.AllowEdit = false;
            this.reportRegTimeColumn.OptionsColumn.AllowFocus = false;
            this.reportRegTimeColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.reportRegTimeColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.reportRegTimeColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.reportRegTimeColumn.OptionsColumn.AllowMove = false;
            this.reportRegTimeColumn.OptionsColumn.AllowShowHide = false;
            this.reportRegTimeColumn.OptionsColumn.AllowSize = false;
            this.reportRegTimeColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.reportRegTimeColumn.OptionsColumn.FixedWidth = true;
            this.reportRegTimeColumn.OptionsColumn.ImmediateUpdateRowPosition = DevExpress.Utils.DefaultBoolean.False;
            this.reportRegTimeColumn.OptionsColumn.Printable = DevExpress.Utils.DefaultBoolean.False;
            this.reportRegTimeColumn.OptionsColumn.ReadOnly = true;
            this.reportRegTimeColumn.OptionsFilter.AllowAutoFilter = false;
            this.reportRegTimeColumn.OptionsFilter.AllowFilter = false;
            this.reportRegTimeColumn.Visible = true;
            this.reportRegTimeColumn.VisibleIndex = 0;
            this.reportRegTimeColumn.Width = 124;
            // 
            // reportAreaColumn
            // 
            this.reportAreaColumn.AppearanceCell.Font = new System.Drawing.Font("Arial", 9F);
            this.reportAreaColumn.AppearanceCell.Options.UseFont = true;
            this.reportAreaColumn.AppearanceCell.Options.UseTextOptions = true;
            this.reportAreaColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.reportAreaColumn.AppearanceHeader.Font = new System.Drawing.Font("Arial", 9F);
            this.reportAreaColumn.AppearanceHeader.Options.UseFont = true;
            this.reportAreaColumn.AppearanceHeader.Options.UseTextOptions = true;
            this.reportAreaColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.reportAreaColumn.Caption = "Area";
            this.reportAreaColumn.FieldName = "AreaNo";
            this.reportAreaColumn.MaxWidth = 52;
            this.reportAreaColumn.MinWidth = 52;
            this.reportAreaColumn.Name = "reportAreaColumn";
            this.reportAreaColumn.OptionsColumn.AllowEdit = false;
            this.reportAreaColumn.OptionsColumn.AllowFocus = false;
            this.reportAreaColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.reportAreaColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.reportAreaColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.reportAreaColumn.OptionsColumn.AllowMove = false;
            this.reportAreaColumn.OptionsColumn.AllowShowHide = false;
            this.reportAreaColumn.OptionsColumn.AllowSize = false;
            this.reportAreaColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.reportAreaColumn.OptionsColumn.FixedWidth = true;
            this.reportAreaColumn.OptionsColumn.ImmediateUpdateRowPosition = DevExpress.Utils.DefaultBoolean.False;
            this.reportAreaColumn.OptionsColumn.Printable = DevExpress.Utils.DefaultBoolean.False;
            this.reportAreaColumn.OptionsColumn.ReadOnly = true;
            this.reportAreaColumn.Width = 52;
            // 
            // reportJobNoColumn
            // 
            this.reportJobNoColumn.Caption = "Job No.";
            this.reportJobNoColumn.FieldName = "pro_job";
            this.reportJobNoColumn.Name = "reportJobNoColumn";
            this.reportJobNoColumn.OptionsColumn.AllowEdit = false;
            this.reportJobNoColumn.OptionsColumn.AllowFocus = false;
            this.reportJobNoColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.reportJobNoColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.reportJobNoColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.reportJobNoColumn.OptionsColumn.AllowMove = false;
            this.reportJobNoColumn.OptionsColumn.AllowShowHide = false;
            this.reportJobNoColumn.OptionsColumn.AllowSize = false;
            this.reportJobNoColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.reportJobNoColumn.OptionsColumn.FixedWidth = true;
            this.reportJobNoColumn.OptionsColumn.ReadOnly = true;
            this.reportJobNoColumn.Visible = true;
            this.reportJobNoColumn.VisibleIndex = 1;
            this.reportJobNoColumn.Width = 170;
            // 
            // reportItemNoColumn
            // 
            this.reportItemNoColumn.Caption = "Item No.";
            this.reportItemNoColumn.FieldName = "orderno";
            this.reportItemNoColumn.Name = "reportItemNoColumn";
            this.reportItemNoColumn.OptionsColumn.AllowEdit = false;
            this.reportItemNoColumn.OptionsColumn.AllowFocus = false;
            this.reportItemNoColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.reportItemNoColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.reportItemNoColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.reportItemNoColumn.OptionsColumn.AllowMove = false;
            this.reportItemNoColumn.OptionsColumn.AllowShowHide = false;
            this.reportItemNoColumn.OptionsColumn.AllowSize = false;
            this.reportItemNoColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.reportItemNoColumn.OptionsColumn.FixedWidth = true;
            this.reportItemNoColumn.OptionsColumn.ReadOnly = true;
            this.reportItemNoColumn.OptionsFilter.AllowAutoFilter = false;
            this.reportItemNoColumn.OptionsFilter.AllowFilter = false;
            this.reportItemNoColumn.Visible = true;
            this.reportItemNoColumn.VisibleIndex = 3;
            this.reportItemNoColumn.Width = 170;
            // 
            // reportProductColumn
            // 
            this.reportProductColumn.Caption = "Product Desc";
            this.reportProductColumn.FieldName = "sam_remarks";
            this.reportProductColumn.Name = "reportProductColumn";
            this.reportProductColumn.OptionsColumn.AllowEdit = false;
            this.reportProductColumn.OptionsColumn.AllowFocus = false;
            this.reportProductColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.reportProductColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.reportProductColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.reportProductColumn.OptionsColumn.AllowMove = false;
            this.reportProductColumn.OptionsColumn.AllowShowHide = false;
            this.reportProductColumn.OptionsColumn.AllowSize = false;
            this.reportProductColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.reportProductColumn.OptionsColumn.FixedWidth = true;
            this.reportProductColumn.OptionsColumn.ReadOnly = true;
            this.reportProductColumn.OptionsFilter.AllowAutoFilter = false;
            this.reportProductColumn.OptionsFilter.AllowFilter = false;
            this.reportProductColumn.Visible = true;
            this.reportProductColumn.VisibleIndex = 4;
            this.reportProductColumn.Width = 162;
            // 
            // reportClientColumn
            // 
            this.reportClientColumn.Caption = "Client";
            this.reportClientColumn.FieldName = "cli_name";
            this.reportClientColumn.Name = "reportClientColumn";
            this.reportClientColumn.OptionsColumn.AllowEdit = false;
            this.reportClientColumn.OptionsColumn.AllowFocus = false;
            this.reportClientColumn.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
            this.reportClientColumn.OptionsColumn.AllowIncrementalSearch = false;
            this.reportClientColumn.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.reportClientColumn.OptionsColumn.AllowMove = false;
            this.reportClientColumn.OptionsColumn.AllowShowHide = false;
            this.reportClientColumn.OptionsColumn.AllowSize = false;
            this.reportClientColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.reportClientColumn.OptionsColumn.FixedWidth = true;
            this.reportClientColumn.OptionsColumn.ReadOnly = true;
            this.reportClientColumn.OptionsFilter.AllowAutoFilter = false;
            this.reportClientColumn.OptionsFilter.AllowFilter = false;
            this.reportClientColumn.Visible = true;
            this.reportClientColumn.VisibleIndex = 5;
            this.reportClientColumn.Width = 190;
            // 
            // itemNoLabel
            // 
            this.itemNoLabel.AutoSize = true;
            this.itemNoLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.itemNoLabel.Location = new System.Drawing.Point(548, 13);
            this.itemNoLabel.Name = "itemNoLabel";
            this.itemNoLabel.Size = new System.Drawing.Size(53, 15);
            this.itemNoLabel.TabIndex = 110;
            this.itemNoLabel.Text = "Item No.";
            this.itemNoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Image = ((System.Drawing.Image)(resources.GetObject("okButton.Image")));
            this.okButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.okButton.Location = new System.Drawing.Point(802, 506);
            this.okButton.Name = "okButton";
            this.okButton.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.okButton.Size = new System.Drawing.Size(100, 32);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Image = ((System.Drawing.Image)(resources.GetObject("cancelButton.Image")));
            this.cancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cancelButton.Location = new System.Drawing.Point(906, 506);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.cancelButton.Size = new System.Drawing.Size(100, 32);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // areaCombo
            // 
            this.areaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.areaCombo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.areaCombo.FormattingEnabled = true;
            this.areaCombo.Location = new System.Drawing.Point(334, 8);
            this.areaCombo.Name = "areaCombo";
            this.areaCombo.Size = new System.Drawing.Size(54, 23);
            this.areaCombo.TabIndex = 3;
            // 
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(300, 9);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(32, 22);
            this.label19.TabIndex = 112;
            this.label19.Text = "Area";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // jobNoEdit
            // 
            this.jobNoEdit.EditValue = "";
            this.jobNoEdit.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.jobNoEdit.Location = new System.Drawing.Point(450, 9);
            this.jobNoEdit.Name = "jobNoEdit";
            this.jobNoEdit.Properties.Appearance.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jobNoEdit.Properties.Appearance.Options.UseFont = true;
            this.jobNoEdit.Properties.MaxLength = 20;
            this.jobNoEdit.Size = new System.Drawing.Size(92, 22);
            this.jobNoEdit.TabIndex = 4;
            // 
            // JobNoLabel
            // 
            this.JobNoLabel.AutoSize = true;
            this.JobNoLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JobNoLabel.Location = new System.Drawing.Point(394, 12);
            this.JobNoLabel.Name = "JobNoLabel";
            this.JobNoLabel.Size = new System.Drawing.Size(49, 15);
            this.JobNoLabel.TabIndex = 110;
            this.JobNoLabel.Text = "Job No.";
            this.JobNoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OmNoLabel
            // 
            this.OmNoLabel.AutoSize = true;
            this.OmNoLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OmNoLabel.Location = new System.Drawing.Point(702, 13);
            this.OmNoLabel.Name = "OmNoLabel";
            this.OmNoLabel.Size = new System.Drawing.Size(47, 15);
            this.OmNoLabel.TabIndex = 114;
            this.OmNoLabel.Text = "OM No.";
            this.OmNoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OmNoEdit
            // 
            this.OmNoEdit.EditValue = "";
            this.OmNoEdit.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.OmNoEdit.Location = new System.Drawing.Point(758, 10);
            this.OmNoEdit.Name = "OmNoEdit";
            this.OmNoEdit.Properties.Appearance.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OmNoEdit.Properties.Appearance.Options.UseFont = true;
            this.OmNoEdit.Properties.MaxLength = 20;
            this.OmNoEdit.Size = new System.Drawing.Size(92, 22);
            this.OmNoEdit.TabIndex = 113;
            // 
            // reportFileNoColumn
            // 
            this.reportFileNoColumn.Caption = "OM No.";
            this.reportFileNoColumn.FieldName = "pro_proj";
            this.reportFileNoColumn.Name = "reportFileNoColumn";
            this.reportFileNoColumn.Visible = true;
            this.reportFileNoColumn.VisibleIndex = 2;
            // 
            // DialogProfJobListView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(1018, 550);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DialogProfJobListView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import";
            this.Load += new System.EventHandler(this.DialogProfJobListView_Load);
            this.Shown += new System.EventHandler(this.DialogProfJobListView_Shown);
            this.bgPanel.ResumeLayout(false);
            this.bgPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemNoEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobNoEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OmNoEdit.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker toDateEdit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox dateCheck;
        private System.Windows.Forms.DateTimePicker fromDateEdit;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Button findButton;
        public DevExpress.XtraEditors.TextEdit itemNoEdit;
        private DevExpress.XtraGrid.GridControl reportGrid;
        private DevExpress.XtraGrid.Views.Grid.GridView reportGridView;
        private DevExpress.XtraGrid.Columns.GridColumn reportRegTimeColumn;
        private DevExpress.XtraGrid.Columns.GridColumn reportItemNoColumn;
        private DevExpress.XtraGrid.Columns.GridColumn reportProductColumn;
        private DevExpress.XtraGrid.Columns.GridColumn reportClientColumn;
        private System.Windows.Forms.Label itemNoLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox areaCombo;
        private System.Windows.Forms.Label label19;
        private DevExpress.XtraGrid.Columns.GridColumn reportAreaColumn;
        private System.Windows.Forms.Label JobNoLabel;
        public DevExpress.XtraEditors.TextEdit jobNoEdit;
        private DevExpress.XtraGrid.Columns.GridColumn reportJobNoColumn;
        private System.Windows.Forms.Label OmNoLabel;
        public DevExpress.XtraEditors.TextEdit OmNoEdit;
        private DevExpress.XtraGrid.Columns.GridColumn reportFileNoColumn;
    }
}
