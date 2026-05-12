namespace Winterhaven.Gateway.Presentation.Targets.Users;

using StreamJsonRpc;
using System;

internal sealed class UserRpcTarget : RpcTargetBase
{
    [JsonRpcMethod("user.login")]
    public static bool Login(string username)
    {
        return username == "mat" ? throw new InvalidOperationException("Testing") : true;
    }
}