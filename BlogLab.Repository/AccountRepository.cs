using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using BlogLab.Models.Account;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BlogLab.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AccountRepository(IConfiguration config) {
            _config = config;
            _connectionString = _config.GetConnectionString(GlobalVariables.connectionString);
        }


        public async Task<IdentityResult> CreateAsync(ApplicationUserIdentity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // virtual data table
            var dataTable = new DataTable();
            dataTable.Columns.Add("username", typeof(string));
            dataTable.Columns.Add("normalizedUserName", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("normalizedEmail", typeof(string));
            dataTable.Columns.Add("fullname", typeof(string));
            dataTable.Columns.Add("passwordHash", typeof(string));

            dataTable.Rows.Add(
                user.Username,
                user.NormalizedUsername,
                user.Email,
                user.NormalizedEmail,
                user.Fullname,
                user.PasswordHash
            );

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                await connection.ExecuteAsync("Account_Insert",
                    new { Account = dataTable.AsTableValuedParameter("dbo.AccountType") }
                    ,
                    commandType: CommandType.StoredProcedure
                    );
            }

            return IdentityResult.Success;


        }

        public async Task<ApplicationUserIdentity> GetByUsernameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ApplicationUserIdentity applicationUser;

            using (var connection = new SqlConnection(_connectionString)) {
                await connection.OpenAsync(cancellationToken);

                applicationUser = await connection.QuerySingleOrDefaultAsync<ApplicationUserIdentity>(
                    "Account_GetByUsername", new { NormalizedUsername = normalizedUserName }, commandType: CommandType.StoredProcedure


                    );

            }

            return applicationUser;


        }
    }
}
