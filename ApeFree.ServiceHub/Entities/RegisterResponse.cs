using System;
using System.Collections.Generic;
using System.Text;

namespace ApeFree.ServiceHub.Entities
{
    public class RegisterResponse:BaseResponse
    {
        public Dictionary<string,string> Signs { get; set; }
    }
}
