using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HTCPlatform.ServiceModel.Models.User
{
    public class UserResponse
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsEnable { get; set; }
    }
}

