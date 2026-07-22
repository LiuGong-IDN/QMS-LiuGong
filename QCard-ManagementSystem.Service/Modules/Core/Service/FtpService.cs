using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Monitoring.Model;
using FluentFTP;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class FtpService : DbService
    {
        public FtpService(IDbHelper db) : base(db)
        {
        }

        public static void SendFtp(ControlBatchProcess controlBatchProcess, string ftpfileName, string filename)
        {
            FtpClient ftp = new FtpClient();
            System.Net.NetworkCredential credential = new System.Net.NetworkCredential();

            credential.UserName = controlBatchProcess.USER_ID;
            credential.Password = controlBatchProcess.PASSWORD;
            ftp.Host = controlBatchProcess.IP_ADDRESS;
            ftp.Credentials = credential;

            ftp.Connect();

            using (var fileStream = File.OpenRead(filename))
            {
                using (var ftpStream = ftp.OpenWrite(string.Format("{0}/{1}/{2}", "", controlBatchProcess.DEST_FOLDER, ftpfileName), FtpDataType.ASCII))
                {
                    var buffer = new byte[8 * 1024];
                    int count;
                    while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ftpStream.Write(buffer, 0, count);
                    }
                }
            }

            ftp.Disconnect();
        }
    }
}
