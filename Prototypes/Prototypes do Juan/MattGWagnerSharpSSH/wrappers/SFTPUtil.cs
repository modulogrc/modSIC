using System;
using System.Collections.Generic;
using Tamir.SharpSsh;
using System.IO;
using Tamir.SharpSsh.java.io;

namespace mattgwagner.SharpSsh.wrappers
{
    public class SFTPUtil
    {
        String host, user, pass;

        Sftp getNewInstance()
        {
            var sftp = new Sftp(host, user, pass);

            sftp.Connect();

            return sftp;
        }

        public SFTPUtil(String host, String user, String pass)
        {
            this.host = host;
            this.user = user;
            this.pass = pass;
        }

        public IEnumerable<String> ListFiles(String remotePath)
        {
            using (var sftp = getNewInstance())
            {
                foreach (String f in sftp.GetFileList(remotePath))
                {
                    if (f != "." && f != "..")
                    {
                        yield return f;
                    }
                }
            }
        }

        public void GetFile(String remotePath, String localPath)
        {
            using (var sftp = getNewInstance())
            {
                sftp.Get(remotePath, localPath);
            }
        }

        public void GetLotsOfFiles(String remotePath, String localPath)
        {
            // only need to do this since we use it to do the full path later
            if (!remotePath.EndsWith("/")) remotePath += "/";

            foreach (var f in ListFiles(remotePath))
            {
                GetFile(remotePath + f, localPath);
            }
        }

        public void PutFile(String localPath, String remotePath)
        {
            using (var sftp = getNewInstance())
            {
                sftp.Put(localPath, remotePath);
            }
        }

        public void DeleteFile(String remotePath)
        {
            using (var sftp = getNewInstance())
            {
                sftp.DeleteFile(remotePath);
            }
        }

        public void ExecuteCommand(String command)
        {
            using (var ssh = new SshExec(host, user, pass))
            {
                ssh.Connect();

                ssh.RunCommand(command);
            }
        }
    }
}
