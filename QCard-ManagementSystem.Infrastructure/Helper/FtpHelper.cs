using FluentFTP;
using FluentFTP.Helpers;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Net;
using System.Security.Cryptography;
using QCard.ManagementSystem.Domain;

namespace QCard.ManagementSystem.Infrastructure
//namespace TPS.Models.A1300100W
{
    public class FTPHelper
    {
        private string password;
        private string userName;
        private string uri;
        private int bufferSize = 1024;
        public bool Passive = true;
        public bool Binary = true;
        public bool EnableSsl = false;
        public bool Hash = false;

        public FTPHelper()
        {

        }

        private FTPConnection GetFTPConnection(string uri, string username, string password)
        {
            FTPConnection ftpConnection = new FTPConnection();
            ftpConnection.Uri = uri;
            ftpConnection.Username = username;
            ftpConnection.Password = password;
            return ftpConnection;
        }

        public void Upload(FTPConnection ftpConnection, string source, string destination, out bool isValid, out string errorMessage, bool isSecured = true)
        {
            try
            {
                this.uri = ftpConnection.Uri;
                this.userName = ftpConnection.Username;
                this.password = ftpConnection.Password;
                var status = UploadFile(source, destination, isSecured);
                isValid = status.IsSuccess();
                errorMessage = status.IsSuccess() ? string.Empty : "Failed to upload file to FTP Server.";
            }
            catch (Exception ex)
            {
                isValid = false;
                errorMessage = ex.Message;
            }
        }

        public FtpStatus UploadFile(string source, string destination, bool isSecured = true)
        {
            using (FtpClient client = new FtpClient(this.uri, this.userName, this.password))
            {
                if (isSecured)
                {
                    client.AutoConnect();
                }
                else
                {
                    var ftpProfiles = client.AutoDetect();
                    var usedProfile = ftpProfiles.First();
                    usedProfile.Protocols = System.Security.Authentication.SslProtocols.None;
                    usedProfile.Encryption = FtpEncryptionMode.None;
                    client.Connect(usedProfile);
                }

                return client.UploadFile(source, destination, existsMode: FtpRemoteExists.Overwrite);
            }
        }

        public void Download(FTPConnection ftpConnection, string destination, out byte[] bytes)
        {
            this.uri = ftpConnection.Uri;
            this.userName = ftpConnection.Username;
            this.password = ftpConnection.Password;
            bytes = DownloadFile(destination);
        }

        public byte[] DownloadFile(string filePath)
        {
            using (FtpClient client = new FtpClient(this.uri, this.userName, this.password))
            {
                client.AutoConnect();
                byte[] bytes = new byte[] { };
                if (IsFileExist(new FTPConnection() { Uri = this.uri, Username = this.userName, Password = this.password }, filePath))
                {
                    bool result = client.DownloadBytes(out bytes, filePath);
                    return bytes;
                }
                return null;
            }
        }

        public void DownloadStream(FTPConnection ftpConnection, string destination, out Stream stream)
        {
            this.uri = ftpConnection.Uri;
            this.userName = ftpConnection.Username;
            this.password = ftpConnection.Password;
            stream = DownloadFileAsStream(destination);
        }

        public Stream DownloadFileAsStream(string filePath)
        {
            using (FtpClient client = new FtpClient(this.uri, this.userName, this.password))
            {
                client.AutoConnect();
                Stream stream = null;
                if (IsFileExist(new FTPConnection() { Uri = this.uri, Username = this.userName, Password = this.password }, filePath))
                {
                    bool result = client.DownloadStream(stream, filePath);
                    return stream;
                }
                return null;
            }
        }

        public bool IsFileExist(FTPConnection connection, String filePath)
        {
            using (FtpClient client = new FtpClient(connection.Uri, connection.Username, connection.Password))
            {
                client.AutoConnect();
                return client.FileExists(filePath);
            }
        }

        public void DeleteFile(FTPConnection connection, string filePath)
        {
            using (FtpClient client = new FtpClient(connection.Uri, connection.Username, connection.Password))
            {
                client.AutoConnect();
                client.DeleteFile(filePath);
            }
        }
    }
}
