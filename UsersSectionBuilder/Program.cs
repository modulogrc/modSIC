/*
 * Modulo Open Distributed SCAP Infrastructure Collector (modSIC)
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace UserSectionBuilder
{
    class Program
    {
        static string GetAuthorizationHash(string username, string password)
        {
            HashAlgorithm myHash = new SHA1Managed();
            byte[] clientBytes = Encoding.UTF8.GetBytes(username + "/MODULO/" + password);
            byte[] clientHash = myHash.ComputeHash(clientBytes);
            return BitConverter.ToString(clientHash).Replace("-", "");
        }

        static void Main(string[] args)
        {
            string msgSuccess = "";
            bool userFound = false;

            if (args.Length < 2)
            {
                Console.Write(
                    "\nUSAGE:\n" +
                    "  {0} username password\n" +
                    "    Shows XML element to add by hand to App.config\n" +
                    "  {0} username password <app-config-file-path>\n" +
                    "    Adds a username/password to App.config\n" +
                    "    (Or changes password if username exists)\n" +
                    "  {0} username /DEL <app-config-file-path>\n" +
                    "    Deletes username from App.config\n",
                    Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]));
            }
            else
            {
                string username = args[0];
                string password = args[1];
                var token = GetAuthorizationHash(username, password);

                if (args.Length >= 3)
                {
                    XmlDocument doc = new XmlDocument();
                    XmlNode usersList;
                    try
                    {
                        doc.Load(args[2]);
                        usersList = doc["configuration"];
                        usersList = usersList["UsersSection"];
                        usersList = usersList["users"];
                    }
                    catch
                    {
                        Console.Write("\nCan't read XML file.\n(Does it exist? Is it a XML with a 'configuration/UsersSection/users' path?)\n");
                        return;
                    }

                    foreach (XmlNode thisUser in usersList)
                    {
                        if (thisUser.Name == "add" && thisUser.Attributes["name"].Value == args[0])
                        {
                            userFound = true;
                            if (args[1].ToLower() == "/del")
                            {
                                if (args[0] == "admin")
                                {
                                    Console.Write("\nUser {0} must not be deleted.\n", args[0]);
                                    return;
                                }
                                usersList.RemoveChild(thisUser);
                                msgSuccess = String.Format("\nUser {0} deleted.\n", args[0]);
                                break;
                            }
                            else
                            {
                                thisUser.Attributes["hash"].Value = token;
                                msgSuccess = String.Format("\nPassword changed for user {0}.\n", args[0]);
                                break;
                            }
                        }
                    }

                    if (!userFound)
                    {
                        if (args[1].ToLower() == "/del")
                        {
                            Console.Write("\nUser {0} not found.\n", args[0]);
                            return;
                        }
                        else
                        {
                            XmlElement newUser = doc.CreateElement("add");
                            newUser.Attributes.Append(doc.CreateAttribute("name"));
                            newUser.Attributes.Append(doc.CreateAttribute("hash"));
                            newUser.Attributes["name"].Value = args[0];
                            newUser.Attributes["hash"].Value = token;
                            usersList.AppendChild(newUser);
                            msgSuccess = String.Format("\nUser {0} added to XML file.\n", args[0]);
                        }
                    }

                    FileInfo curFile = new FileInfo(args[2]);
                    bool wasRO = curFile.IsReadOnly;
                    if (wasRO)
                        curFile.IsReadOnly = false;
                    new FileInfo(args[2]).CopyTo(args[2] + ".bak", true);
                    new FileInfo(args[2] + ".bak").IsReadOnly = false;
                    doc.Save(args[2]);
                    if (wasRO)
                        curFile.IsReadOnly = true;

                    if (!String.IsNullOrEmpty(msgSuccess))
                        Console.Write("{0}", msgSuccess);
                }
                else
                {
                    Console.Write(
                        "\nCopy the following text and replace the default UsersSection section at the\n" +
                        "modSIC configuration file.\n\n" +
                        "<UsersSection>\n" +
                        "    <users>\n" +
                        "\t<add name=\"{0}\" hash=\"{1}\"/>\n" +
                        "    </users>\n" +
                        "</UsersSection>\n\n" +
                        "Or, alternatively, if you want to preserve the existing login, copy just the\n" +
                        "line beginning with '<add name...' and add it under the existing one.\n",
                        username, token);
                }
            }
        }
    }
}
