using System;
using System.ComponentModel;

namespace BicycleAPI.Domain.Variables.Permissions;

public static partial class Permissions
{
    public static class User
    {
        [Description("使用者檢視權限")]
        public const string View = "USER.VIEW";
    }
}
