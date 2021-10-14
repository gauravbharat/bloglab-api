using System;
using System.Threading.Tasks;
using BlogLab.Models.Account;

namespace BlogLab.Services
{
    public interface ITokenService
    {
        public string CreateToken(ApplicationUserIdentity user);
    }
}
