using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectorCore.Models
{
    public class TokenInfo
    {
        public TokenInfo() 
        {
            Token = string.Empty;
        }
        public TokenInfo(string token)
        {
            Token = token;
        }
        public string Token {get;set;}
        public string? UserName { get; set; }        
    }
}
