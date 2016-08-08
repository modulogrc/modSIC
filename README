[![Build status](https://ci.appveyor.com/api/projects/status/m3n8xrq3msijimxn/branch/master?svg=true)](https://ci.appveyor.com/project/xunilrj/modsic/branch/master)

Run:

Clients:
..\modSIC Client\modsicgui.exe

..\modSIC Client\modsic.exe

Service:
..\modSIC Service\modsicsrv.exe

About:

Modulo Open Distributed SCAP Infrastructure Collector
The initiative aims at providing a common platform for collecting security data, making it easier for solutions to automate policy compliance, audits, risk assessments, and more, using the industry-standard “Security Content Automation Protocol” (SCAP).

Developed by the National Institute of Standards and Technology (NIST), SCAP is a common format for exchanging IT security data and maintaining a standards-based security posture across enterprise systems. By using SCAP-validated products, organizations automate critical tasks such as verifying the presence of patches, checking system security configurations, and examining systems for signs of compromise. SCAP also helps satisfy federal regulation mandates, such as FDCC and FISMA.

Until now, however, the limited availability of SCAP tools has limited its adoption in the broader security and GRC community. Without these tools, organizations have to write their own APIs or export their data to Excel spreadsheets. By opening Modulo Risk Manager’s proven intelligent SCAP collection technology to the broader security and GRC domain, the open source SCAP initiative will provide a common platform that makes it easier for vendors and end-user organizations to create, improve, and test their own automated data collectors, leveraging:

-Standardized formats
-Distributed architectures
-Transparent code
-Crowd Testing

Modulo Open Distributed SCAP Infrastructure Collector, or modSIC, uses OVAL (Open Vulnerability and Assessment Language) to lead an environment assessment. The use of SCAP standards allows for interoperability between many solutions and for the use of thousands of existing OVAL Definitions from the existing public repositories.
ModSIC service works by receiving an OVAL Definitions XML file and some required information to perform data collection on a given target machine. It scans the machine through the network and retrieves he OVAL System Characteristics and the OVAL Results XML files.

ModSIC can collect data from many infrastructures isolated by different networks in a distributed way. It is scalable and capable of performing multiple jobs simultaneously through its scheduler and a secure persistent database. Moreover, the service is adaptable and can integrate with third-party software using simple communications protocol.

As its features are easily extensible, this project proposes to offer a finished tool for the community. All the probes (currently Windows and Unix) are written as plugins, and new probes can be developed using C#. All the information is stored using RavenDB, an open source document database.

ModSIC can be used in a production environment. In fact, it is already used by Modulo Risk Manager 7 clients.

The modSIC Project is released under a BSD 3-clause license:

************************************************************

Copyright (c) 2012, Modulo Security Solutions
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

•	Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

•	Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

•	Neither the name of Modulo Security Solutions nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

************************************************************
