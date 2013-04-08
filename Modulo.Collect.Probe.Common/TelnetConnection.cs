/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
 * 
 * Copyright (c) 2011, Modulo Solutions for GRC.
 * All rights reserved.
 * 
 * Based on:
 * minimalistic telnet implementation
 * conceived by Tom Janssens on 2007/06/06  for codeproject
 *
 * http://www.corebvba.be
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright 
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of Modulo Security, LLC nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific  prior written permission.
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
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Modulo.Collect.Probe.Common
{
    public enum Verbs
    {
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    public enum Options
    {
        SGA = 3
    }

    public class TelnetConnection
    {
        private TcpClient tcpSocket = null;
        private int timeOutReadMs = 100;
        private int timeOutFragMs = 10;
        private int timeOutLoginMs = 500;

        public int TimeOutLoginMs
        {
            get
            {
                return timeOutLoginMs;
            }

            set
            {
                if ((value > 0) && (value <= 60000))
                    timeOutLoginMs = value;
                else
                    throw new ArgumentException("Login timeout must be positive and at most 60 seconds");
            }
        }

        public int TimeOutFragMs
        {
            get
            {
                return timeOutFragMs;
            }

            set
            {
                if ((value > 0) && (value <= timeOutReadMs / 2))
                    timeOutFragMs = value;
                else
                    throw new ArgumentException("Fragment timeout must be positive and at most half of read timeout");
            }
        }

        public int TimeOutReadMs
        {
            get
            {
                return timeOutReadMs;
            }

            set
            {
                if ((value > 0) && (value <= 60000))
                {
                    timeOutReadMs = value;
                    if (timeOutFragMs > value / 2)
                        timeOutFragMs = value / 10;
                }
                else
                    throw new ArgumentException("Timeout must be positive and at most 60 seconds");
            }
        }

        public TelnetConnection()
        {
        }

        public TelnetConnection(string Hostname, int Port)
        {
            tcpSocket = new TcpClient();
            //tcpSocket.SendTimeout = 100000;
            //tcpSocket.ReceiveTimeout = 100000;
            tcpSocket.Connect(Hostname, Port);
        }

        ~TelnetConnection()
        {
            this.Close();
        }

        public void Close()
        {
            if (tcpSocket != null)
            {
                if (tcpSocket.Connected)
                    tcpSocket.Close();
                tcpSocket = null;
            }
        }

        public string Login(string userOrPass)
        {
            return Login(userOrPass, timeOutLoginMs);
        }

        public string Login(string userOrPass, int loginTimeOutMs)
        {
            return Login(userOrPass, null, loginTimeOutMs);
        }

        public string Login(string userName, string passWord)
        {
            return Login(userName, passWord, timeOutLoginMs);
        }

        public string Login(string userName, string passWord, int loginTimeOutMs)
        {
            string s = "";
            if (!string.IsNullOrEmpty(userName))
            {
                s += Read(loginTimeOutMs, ".*: *");
                if (!s.TrimEnd().EndsWith(":"))
                    throw new ApplicationException("Failed to connect: no login prompt");
                WriteLine(userName);
            }

            if (!string.IsNullOrEmpty(passWord))
            {
                s += Read(loginTimeOutMs, ".*: *");
                if (!s.TrimEnd().EndsWith(":"))
                    throw new ApplicationException("Failed to connect: no password prompt");
                WriteLine(passWord);
            }

            s += Read(loginTimeOutMs, ".*[>#$] *");
            return s;
        }

        public void WriteLine(string cmd)
        {
            Write(cmd + "\n");
        }

        public void Write(string cmd)
        {
            if (!tcpSocket.Connected) return;
            byte[] buf = System.Text.ASCIIEncoding.ASCII.GetBytes(cmd.Replace("\0xFF", "\0xFF\0xFF"));
            tcpSocket.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            return Read(timeOutReadMs);
        }

        public string Read(int readTimeoutMs)
        {
            return Read(readTimeoutMs, null);
        }

        public string Read(int readTimeoutMs, string expectPrompt)
        {
            string retVal = "";

            if (!tcpSocket.Connected) return null;
            StringBuilder sb = new StringBuilder();
            DateTime startRead = DateTime.UtcNow;
            DateTime current;
            do
            {
                do
                {
                    ParseTelnet(sb);
                    System.Threading.Thread.Sleep(timeOutFragMs);
                }
                while (tcpSocket.Available > 0);
                current = DateTime.UtcNow;

                // Test prompt
                retVal = sb.ToString();
                if (!String.IsNullOrEmpty(expectPrompt))
                {
                    string maybePrompt = "";
                    for (int charPos = retVal.Length - 1; charPos >= 0; charPos--)
                    {
                        char c = retVal[charPos];
                        if ((c == '\r') || (c == '\n'))
                        {
                            maybePrompt = retVal.Substring(charPos + 1);
                            break;
                        }
                    }
                    if (!String.IsNullOrEmpty(maybePrompt))
                    {
                        if (Regex.IsMatch(maybePrompt, expectPrompt))
                            break;
                    }
                }
            }
            while ((current - startRead).TotalMilliseconds < readTimeoutMs);
            return retVal;
        }

        public bool IsConnected
        {
            get { return tcpSocket.Connected; }
        }

        private void ParseTelnet(StringBuilder sb)
        {
            while (tcpSocket.Available > 0)
            {
                int input = tcpSocket.GetStream().ReadByte();
                switch (input)
                {
                    case -1:
                        break;
                    case (int)Verbs.IAC:
                        // interpret as command
                        int inputverb = tcpSocket.GetStream().ReadByte();
                        if (inputverb == -1) break;
                        switch (inputverb)
                        {
                            case (int)Verbs.IAC:
                                //literal IAC = 255 escaped, so append char 255 to string
                                sb.Append(inputverb);
                                break;
                            case (int)Verbs.DO:
                            case (int)Verbs.DONT:
                            case (int)Verbs.WILL:
                            case (int)Verbs.WONT:
                                // reply to all commands with "WONT", unless it is SGA (suppres go ahead)
                                int inputoption = tcpSocket.GetStream().ReadByte();
                                if (inputoption == -1) break;
                                tcpSocket.GetStream().WriteByte((byte)Verbs.IAC);
                                if (inputoption == (int)Options.SGA)
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WILL : (byte)Verbs.DO);
                                else
                                    tcpSocket.GetStream().WriteByte(inputverb == (int)Verbs.DO ? (byte)Verbs.WONT : (byte)Verbs.DONT);
                                tcpSocket.GetStream().WriteByte((byte)inputoption);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        sb.Append((char)input);
                        break;
                }
            }
        }

        public void CiscoLogin(string userName, string passWord)
        {
            string retString = this.Login(userName, passWord);
            if (!(retString.EndsWith(">") || retString.EndsWith("#")))
                throw new ApplicationException("Failed to login: no '>' or '#' prompt received after login");

            this.CiscoCommand("terminal length 0", true);
        }

        public void CiscoEnable(string enaPassword)
        {
            string output;

            this.WriteLine("enable");
            output = this.Read(this.TimeOutLoginMs, ".*: *");
            if (!output.TrimEnd().EndsWith(":"))
                throw new ApplicationException("Failed to enable: unexpected output");

            this.WriteLine(enaPassword);
            output = this.Read(this.TimeOutLoginMs, ".*[:#] *");
            if (!output.TrimEnd().EndsWith("#"))
                throw new ApplicationException("Failed to enable: authentication failure");
        }

        private void CheckCommandOutput(string commandOutput)
        {
            if (string.IsNullOrEmpty(commandOutput))
                throw new NoCommandOutputException();
        }

        public virtual string CiscoCommand(string commandValue, bool isLoginAttempt = false)
        {
            var promptChar = '\0';
            this.WriteLine(commandValue);
            
            var output = this.Read(this.TimeOutReadMs, ".*[#>] *");

            CheckCommandOutput(output);

            // Parse away command echo
            if (output.StartsWith(commandValue))
            {
                int startpos = commandValue.Length;
                while ((startpos < output.Length) && ((output[startpos] == '\r') || (output[startpos] == '\n')))
                    startpos++;
                output = output.Substring(startpos);
            }

            // Parse away ending prompt
            int endpos = output.Length - 1;
            while (endpos >= 0)
            {
                if ((output[endpos] == ' ') || (output[endpos] == '\t'))
                    endpos--;
                else
                    break;
            }
            if (endpos >= 0)
            {
                promptChar = output[endpos];
                while ((endpos >= 0) && (output[endpos] != '\r') && (output[endpos] != '\n'))
                    endpos--;
                output = output.Substring(0, endpos + 1);
            }

            if (isLoginAttempt)
            {
                var loginSuccessfully = (promptChar == '>' || promptChar == '#');
                if (!loginSuccessfully)
                    throw new ApplicationException("Failed to login: no '>' prompt received after terminal setup");
            }

            return output;
        }
    }

    public class NoCommandOutputException : Exception { }
}
