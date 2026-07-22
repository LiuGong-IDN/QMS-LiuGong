using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QCard.ManagementSystem.Domain.Modules.Core.Model;

namespace QCard.ManagementSystem.Domain.Modules.Core.Repository
{
    public partial interface IMessageRepository : IDapperRepository<Message>
    {
    }

    public partial class MessageRepository : DapperRepository<Message>, IMessageRepository
    {
        public MessageRepository(IDbConnection connection, IDbTransaction transaction) : base(connection, transaction)
        {
        }
    }
}
