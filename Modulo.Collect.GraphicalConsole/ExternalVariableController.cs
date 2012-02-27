#region License
/* * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
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
using System.Linq;
using System.Text;
using Modulo.Collect.OVAL.Definitions;
using Modulo.Collect.OVAL.Common;
using System.Windows.Forms;
using System.Globalization;
using Modulo.Collect.OVAL.Definitions.validators;

namespace Modulo.Collect.GraphicalConsole
{
    public class ExternalVariableController
    {
        #region Private Members
        IExternalVariableView view;
        #endregion

        #region Public Members
        public List<Control> Controls { get; set; }
        #endregion

        #region Constructor
        public ExternalVariableController(IExternalVariableView _view)
        {
            view = _view;
            Controls = new List<Control>();

            view.OnCreateControls += new EventHandler<CreateControlsEventArgs>(view_OnCreateControls);
            view.OnGetExternalVariables += new EventHandler<ExternalVariableEventArgs>(view_OnGetExternalVariables);
            view.OnValidate += new EventHandler<ValidateEventArgs>(view_OnValidate);
        }
        #endregion

        #region View Events
        public void view_OnCreateControls(object sender, CreateControlsEventArgs e)
        {
            var factory = new ExternalVariableEditorFactory();
            foreach (var variable in e.Variables)
            {
                var value = e.Values.Where(n => n.Key == variable.id).Select(m => m.Value).FirstOrDefault();
                var control = factory.CreateExternalVariableEditor(variable).GetEditor(value);

                if (variable.datatype == SimpleDatatypeEnumeration.boolean)
                {
                    view.AddControl(control);
                }
                else
                {
                    var label = new Label() { Text = String.Format("{0}", variable.comment) };
                    label.AutoSize = true;
                    Controls.Add(label);

                    view.AddControlWithLabel(label, control);
                }

                Controls.Add(control);
            }
        }

        public void view_OnGetExternalVariables(object sender, ExternalVariableEventArgs e)
        {
            var helper = new ExternalVariableHelper();

            e.Values = new Dictionary<string, string>();
            foreach (var control in Controls)
            {
                if (control is Panel)
                {
                    var panel = (Panel)control;
                    string value = String.Empty;
                    foreach (var item in panel.Controls)
                    {
                        if (item is RadioButton)
                        {
                            var radio = (RadioButton)item;
                            if (radio.Checked)
                            {
                                value = String.Format("{0}", radio.Tag);
                                break;
                            }
                        }
                    }
                    e.Values.Add(panel.Name, value);
                }
                else if (control is CheckBox)
                {
                    var check = (CheckBox)control;
                    e.Values.Add(check.Name, check.Checked.ToString().ToLower());
                }
                else if (control is ComboBox)
                {
                    var value = helper.GetSelectItemValue((ComboBox)control);
                    e.Values.Add(control.Name, value);
                }
                else if (!(control is Label))
                {
                    e.Values.Add(control.Name, control.Text);
                }
            }

            e.Xml = helper.GetEvaluatedExternalVariables(e.Values, e.ExternalVariables);
        }

        public void view_OnValidate(object sender, ValidateEventArgs e)
        {
            ExternalVariableValidator externalVariableValidator = new ExternalVariableValidator();
            IEnumerable<string> messages = externalVariableValidator.ValidateValue(e.Variable, e.Value);
            e.ErrorMessage = this.GetMessageText(messages);
            e.Result = messages.Count() == 0;
        }

        private string GetMessageText(IEnumerable<string> messages)
        {
            var builder = new StringBuilder();
            foreach (var message in messages)
            {
                builder.AppendLine(message);
            }
            return builder.ToString();
        }
        #endregion
    }
}
