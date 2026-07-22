using Dapper;
using Modules.MyQCard.ViewModel;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.Model;
using Dapper.Contrib.Extensions;
using System.Data;
using CommandType = System.Data.CommandType;

namespace Modules.MyQCard.Repository
{
    public partial interface IMyQCardRepository : IDapperRepository<QCardView>
    {
        List<QCardView> GetViewQCardByEmail(string email);
    }

    public partial class MyQCardRepository(IDbConnection connection, IDbTransaction transaction) : DapperRepository<QCardView>(connection, transaction), IMyQCardRepository
    {
        public List<QCardView> GetViewQCardByEmail(string email)
        {
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(email))
            {
                conditions.Add("EMAIL LIKE @Email");
                parameters.Add("Email", "%" + email + "%");
            }

            // Kalau ada kondisi
            if (conditions.Count != 0)
            {
                var whereClause = string.Join(" AND ", conditions);
                return [.. Find(whereClause, parameters)];
            }
            else
            {
                // Kalau tidak ada filter, ambil semua
                return [.. FindAll()];
            }
        }
    }
}
