using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModsenLibGateWay.Helpers;
using ModsenLibGateWay.Models;
using ModsenLibGateWayAbstractions.DataTransferObjects;
using ModsenLibGateWayCQS.Roles.Queries;
using ModsenLibGateWayCQS.Users.Commands;
using ModsenLibGateWayCQS.Users.Queries;
using Serilog;
using System;
using System.Security.Claims;

namespace ModsenLibGateWay.Controllers
{
    /// <summary>
    /// Account Controller
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly JWTSha256 _jwtUtil;
        private readonly MD5 _md5;
        private readonly DataChecker _datachecker;
        private readonly IMediator _mediator;

        /// <summary>
        /// Account Controller Ctor
        /// </summary>
        public AccountController(IMapper mapper, JWTSha256 jwtUtil, MD5 md5, IMediator mediator, DataChecker datachecker)
        {
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _md5 = md5;
            _mediator = mediator;
            _datachecker = datachecker;
        }

        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="model CustomerRequestModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CustomerRegRequestModel model)
        {
            try
            {
                var customerDto = _mapper.Map<UserDto>(model);
                if (customerDto == null)
                    return BadRequest(new { model, Message = "RegData is null" });
                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                    return BadRequest(new { model, Message = "Email or Password is null or empty" });
                var resMail = await _datachecker.CheckEmail(customerDto.Email, HttpContext);
                if (resMail == null)
                    return BadRequest(new { model, Message = "Email check error" });
                if (resMail.isMatched == false)
                    return BadRequest(new { model, resMail });
                var resAge = _datachecker.BirthDateCheck(customerDto.BirthDate);
                if (resAge == null)
                    return BadRequest(new { model, Message = "Birth date check error" });
                if (resAge.isMatched == false)
                    return BadRequest(new { model, resAge });

                customerDto.PasswordHash = _md5.CreateMd5(model.Password);
                var res = await _mediator.Send(new CreateUserWithRoleCommand()
                { UserDto = customerDto, RoleName = "User" });
                if (res > 0)
                {
                    var role = await _mediator.Send(new GetRoleByNameQuery() { RoleName = "User" });

                    if (role == null)
                        return BadRequest(new { model, Message = "User's roles are not found" });
                    var response = await _jwtUtil.GenerateTokenAsync(customerDto, new List<RoleDto> { role });
                    return Ok(response);
                }
                else
                    return BadRequest(new ErrorModel() { Message = "User is not created" });

            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                return BadRequest();
            }
        }


        /// <summary>
        /// UserLoginPreview
        /// </summary>
        /// <returns> UserDataModelResponse</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserLoginPreview()
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {

                var sUserId = User.FindFirst("UId");

                if (sUserId == null)
                {
                    return BadRequest();
                }

                try
                {

                    if (sUserId != null)
                    {
                            if (Guid.TryParse(sUserId!.Value, out Guid userId))
                            {
                                var user = await _mediator.Send(new GetUserByIdQuery() { UserId = userId });
                                var roles = User.FindAll(ClaimsIdentity.DefaultRoleClaimType).Select(c => c.Value).ToList();
                                if (user != null && roles != null)
                                {
                                    string fName = user.Name + " " + user.Surname;
                                    UserDataModelResponse model = new()
                                    {
                                        ActiveEmail = user.Email,
                                        ActiveFullName = fName,
                                        RoleNames = roles
                                    };
                                    return Ok(model);
                                }
                            }

                    }
                }

                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                }
            }

            return BadRequest(new ErrorModel() { Message = "User data is not found. Something went wrong" });
        }

        /// <summary>
        /// RemoveAccount
        /// </summary>
        /// <param name="model RemoveAccountRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveAccount(RemoveAccountRequest model)
        {
            if (User.Identities.Any(identity => identity.IsAuthenticated) &&
                 model != null  && model.Pwd != null && model.Keyword != null)
            {
                try
                {
                    var sMainUserId = User.FindFirst("UId");
                    if (sMainUserId == null)
                    {
                        return BadRequest("User is not found. Something went wrong");
                    }
                    var mRes = Guid.TryParse(sMainUserId.Value, out Guid UId);
                    if (!mRes)
                    {
                        return BadRequest("User is not found. Something went wrong");
                    }
                    var pwdHash = _md5.CreateMd5(model?.Pwd);
                    var res = await _mediator.Send(new CheckUserByUsernamePasswordQuery()
                    { Id = UId, PwdHash = pwdHash, Keyword = model?.Keyword });
                    if (res == 0)
                    {
                        model!.SystemInfo = "User is not found. Something went wrong";
                        return BadRequest(model);
                    }
                    if (res == 1)
                    {
                        model!.SystemInfo = "You have entered wrong password or code name";
                        return BadRequest(model);
                    }
                    else
                    {
                        var token = await _jwtUtil.GetRefreshToken(UId);
                        var result = await _mediator.Send(new RemoveUserByIdCommand() { Id = UId});
                        if (result > 0)
                        {                        
                               // await _jwtUtil.RemoveRefreshTokenAsync(UId, token);
                                return Ok("Your account has been deleted");
                        }
                        else
                        {
                            model!.SystemInfo = "Your account has not been removed<br/>Something went wrong";
                            return BadRequest(model);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"{e.Message}. {Environment.NewLine} {e.StackTrace}");
                    return BadRequest();
                }
            }
            RemoveAccountRequest model1 = new() { SystemInfo = "Something went wrong" };
            return BadRequest(model1);
        }
    }
}
