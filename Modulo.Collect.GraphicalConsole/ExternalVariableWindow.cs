#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011-2014, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 * - Redistributions of source code must retain the above copyright notice,
 *   this list of conditions and the following disclaimer.
 *   
 * - Redistributions in binary form must reproduce the above copyright 
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 *   
 * - Neither the name of Modulo Security, LLC nor the names of its
 *   contributors may be used to endorse or promote products derived from
 *   this software without specific  prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 * */
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Modulo.Collect.GraphicalConsole
{
    public partial class ExternalVariableWindow : Form, IExternalVariableView
    {
        #region Private Members
        const int StartX = 10;
        const int IncY = 35;
        int ControlY = 0;
        IExternalVariable mainWindow;
        #endregion

        #region Constructor
        public ExternalVariableWindow(IExternalVariable _mainWindow)
        {
            mainWindow = _mainWindow;
            new ExternalVariableController(this);

            InitializeComponent();
        }
        #endregion

        #region IExternalVariableView Members
        public event EventHandler<CreateControlsEventArgs> OnCreateControls;
        public event EventHandler<ExternalVariableEventArgs> OnGetExternalVariables;
        public event EventHandler<ValidateEventArgs> OnValidate;

        public void AddControl(Control control)
        {
            control.Location = new Point(StartX, ControlY);
            control.Width = panel.Width - 40;

            ControlY += control.Height > IncY ? control.Height + 10 : IncY;
            panel.Controls.Add(control);
        }

        public void AddControlWithLabel(Label label, Control control)
        {
            label.Location = new Point(StartX, ControlY);
            ControlY += 20;
            panel.Controls.Add(label);

            control.Location = new Point(StartX, ControlY);
            control.Width = panel.Width - 40;

            ControlY += control.Height > IncY ? control.Height + 10 : IncY;
            panel.Controls.Add(control);

            if (!(control is ComboBox))
            {
                control.CausesValidation = true;
                control.Validated += new EventHandler(control_Validated);
                control.Validating += new CancelEventHandler(control_Validating);
            }
        }

        void control_Validating(object sender, CancelEventArgs e)
        {
            var control = sender as Control;

            var args = new ValidateEventArgs();
            args.Variable = mainWindow.ExternalVariables.Where(n => n.id == control.Name).FirstOrDefault();

            if (control is ComboBox)
            {
                args.Value = new ExternalVariableHelper().GetSelectItemValue(control as ComboBox);
            }
            else
            {
                args.Value = control.Text;
            }

            OnValidate(this, args);
            if (!args.Result)
            {
                e.Cancel = true;
                errorProvider.SetError(control, args.ErrorMessage);
            }
        }

        void control_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(sender as Control, "");
        }
        #endregion

        #region Form Events
        private void ExternalVariableWindow_Shown(object sender, EventArgs e)
        {
            var args = new CreateControlsEventArgs();
            args.Variables = mainWindow.ExternalVariables;
            args.Values = mainWindow.ExternalVariablesValues ?? new Dictionary<string, string>();

            this.OnCreateControls(this, args);
            if (args.Result)
            {
                Dialog.Error(args.ErrorMessage);
                this.Close();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var args = new ExternalVariableEventArgs();
            args.ExternalVariables = mainWindow.ExternalVariables;
            this.OnGetExternalVariables(this, args);

            mainWindow.ExternalVariablesXml = args.Xml;
            mainWindow.ExternalVariablesValues = args.Values;
        }

        private void ExternalVariableWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
        #endregion
    }
}
