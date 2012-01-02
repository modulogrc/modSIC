using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;

namespace FrameworkNG
{
    public static class AccManUtils
    {
        public static PrincipalContext accManConnect(string machimeName, string user, string pass)
        {
            PrincipalContext retVal = new PrincipalContext(ContextType.Machine, machimeName, user, pass);
            return retVal;
        }

        public static void accManDisconnect(PrincipalContext ctx)
        {
            ctx.Dispose();
        }

        public static Principal getPrincipalBySid(PrincipalContext ctx, string sid)
        {
            return Principal.FindByIdentity(ctx, IdentityType.Sid ,sid);
        }

        public static Principal getPrincipalByName(PrincipalContext ctx, string entityName)
        {
            return Principal.FindByIdentity(ctx, IdentityType.Name, entityName);
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
    }
}
