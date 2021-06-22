using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api_asp_net_4.Data;

namespace api_asp_net_4
{
    public interface IJwtAuthenticationManager
    {
        public Task<string> AuthenticateAsync(AppDbContext context, string username, string password);
    }
}
