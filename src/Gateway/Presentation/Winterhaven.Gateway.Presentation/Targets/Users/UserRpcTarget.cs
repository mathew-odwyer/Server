namespace Winterhaven.Gateway.Presentation.Targets.Users;

using AutoMapper;
using MediatR;
using StreamJsonRpc;
using System;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Application.Requests.Users.UserLogin;
using Winterhaven.Gateway.Core.Application.Requests.Users.UserRegister;
using Winterhaven.Gateway.Presentation.Attributes;
using Winterhaven.Gateway.Presentation.DTOs.User;

internal sealed class UserRpcTarget : RpcTargetBase
{
    public UserRpcTarget(IMediator mediator, IMapper mapper)
        : base(mediator, mapper)
    {
    }

    [JsonRpcAuthorize]
    [JsonRpcMethod("user.refresh", UseSingleObjectParameterDeserialization = true)]
    public void Refresh()
    {
    }

    [JsonRpcMethod("user.login", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserLoginResponseDto> Login(UserLoginRequestDto dto)
    {
        var request = this.Mapper.Map<UserLoginRequest>(dto);
        var response = await this.Mediator.Send(request).ConfigureAwait(false);

        return this.Mapper.Map<UserLoginResponseDto>(response);
    }

    [JsonRpcMethod("user.register", UseSingleObjectParameterDeserialization = true)]
    public async Task<UserRegisterResponseDto> Register(UserRegisterRequestDto dto)
    {
        var request = this.Mapper.Map<UserRegisterRequest>(dto);
        var response = await this.Mediator.Send(request).ConfigureAwait(false);

        return this.Mapper.Map<UserRegisterResponseDto>(response);
    }
}