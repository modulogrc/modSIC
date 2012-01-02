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
using System.DirectoryServices.AccountManagement;

namespace Modulo.Collect.Probe.Windows.Helpers
{
    public static class AccManUtils
    {
        public static PrincipalContext accManConnect(string machimeName, string user, string pass)
        {
            PrincipalContext retVal = new PrincipalContext(ContextType.Machine, machimeName, user, pass);
            return retVal;
        }

        public static PrincipalContext accManConnect(ContextType contextType, string machimeName, string user, string pass)
        {
            PrincipalContext retVal = new PrincipalContext(contextType, machimeName, user, pass);
            return retVal;
        }

        public static void accManDisconnect(PrincipalContext ctx)
        {
            try
            {
                ctx.Dispose();
            }
            catch (Exception)
            {

            }
        }

        public static Principal getPrincipalBySid(PrincipalContext ctx, string sid)
        {
            return Principal.FindByIdentity(ctx, IdentityType.Sid, sid);
        }

        public static List<Principal> getGroupsByUser(PrincipalContext ctx, string accName)
        {
            List<Principal> retList = new List<Principal>();

            using (var user = UserPrincipal.FindByIdentity(ctx, accName))
            {
                if (user != null)
                {
                    PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();
                    foreach (Principal group in groups)
                    {
                        retList.Add(group);
                    }
                }
            }

            return retList;
        }

        public static List<Principal> getGroupsByUserSid(PrincipalContext ctx, string accSid)
        {
            List<Principal> retList = new List<Principal>();

            using (var user = UserPrincipal.FindByIdentity(ctx, IdentityType.Sid, accSid))
            {
                if (user != null)
                {
                    PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();
                    foreach (Principal group in groups)
                    {
                        retList.Add(group);
                    }
                }
            }

            return retList;
        }

        public static List<Principal> getMembersOfGroup(PrincipalContext ctx, string grpName)
        {
            List<Principal> retList = new List<Principal>();

            using (var group = GroupPrincipal.FindByIdentity(ctx, grpName))
            {
                if (group != null)
                {
                    PrincipalSearchResult<Principal> members = group.GetMembers();
                    foreach (Principal member in members)
                    {
                        retList.Add(member);
                    }
                }
            }

            return retList;
        }

        public static List<Principal> getMembersOfGroupSid(PrincipalContext ctx, string grpSid)
        {
            List<Principal> retList = new List<Principal>();

            using (var group = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, grpSid))
            {
                if (group != null)
                {
                    PrincipalSearchResult<Principal> members = group.GetMembers();
                    foreach (Principal member in members)
                    {
                        retList.Add(member);
                    }
                }
            }

            return retList;
        }

        public static List<String> getMembersOfGroupSidSafely(PrincipalContext ctx, string grpSid)
        {
            List<String> localUsersOfGroup = new List<String>();

            var group = GroupPrincipal.FindByIdentity(ctx, IdentityType.Sid, grpSid);
            try
            {
                PrincipalSearchResult<Principal> members = group.GetMembers();
                foreach (var member in members)
                    localUsersOfGroup.Add(member.Sid.ToString());
            }
            catch (Exception)
            {

            }

            return localUsersOfGroup;
        }
    }
}
