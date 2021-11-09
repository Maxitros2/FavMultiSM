using FavMultiSM.Registration;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Api.Instagram
{
    public class InstagramApi : ISingletoneService
    {
        public InstagramApi(IConfiguration configuration, ILogger<InstagramApi> logger)
        {
            instLogger = new InstagramAspLogger(logger);
            Configuration = configuration;
            IsBusy = false;
        }
        InstagramAspLogger instLogger;
        IInstaApi instaApi;
        const string StateFile = "state.bin";
        IConfiguration Configuration { get; }
        public bool HasCode { get; set; }
        public bool IsBusy { get; set; }

        public string Code { get; set; }
        public long CurrentUserId { get; set; }
        //!!!Bullshit!!!!
        public async Task<IInstaApi> GetInstaApi()
        {
            if (instaApi != null)
                return instaApi;
            IsBusy = true;
            var userSession = new UserSessionData() { UserName = Configuration["instagam:user"], Password = Configuration["instagam:pass"] };
            instaApi = InstaApiBuilder.CreateBuilder()
                .SetUser(userSession)
                .UseLogger(instLogger)
                .Build();
            LoadSession();
            if (!instaApi.IsUserAuthenticated)
            {
                var logInResult = await instaApi.LoginAsync();
                Debug.WriteLine(logInResult.Value);
                HasCode = false;
                if (logInResult.Succeeded)
                {
                    SaveSession();
                    HasCode = true;
                }
                else
                {
                    // two factor is required
                    if (logInResult.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        HasCode = false;
                        while (String.IsNullOrEmpty(Code))
                        {                           
                            await Task.Delay(1000);
                        }
                        var twoFactorLogin = await instaApi.TwoFactorLoginAsync(Code);
                        if (twoFactorLogin.Succeeded)
                        {
                            SaveSession();
                            HasCode = true;
                        }
                        else
                        {
                            throw new Exception("Instagram login error");
                        }
                    }
                    if (logInResult.Value == InstaLoginResult.ChallengeRequired)
                    {
                        var challenge = await instaApi.GetChallengeRequireVerifyMethodAsync();
                        if (challenge.Succeeded)
                        {

                            var email = await instaApi.RequestVerifyCodeToEmailForChallengeRequireAsync();
                            if (email.Succeeded)
                            {
                                while (String.IsNullOrEmpty(Code))
                                {
                                    await Task.Delay(1000);
                                }
                                var verifyLogin = await instaApi.VerifyCodeForChallengeRequireAsync(Code);
                                if (verifyLogin.Succeeded)
                                {
                                    SaveSession();
                                    HasCode = true;                                    
                                }
                                else
                                {
                                    Code = "";
                                    while (String.IsNullOrEmpty(Code))
                                    {
                                        await Task.Delay(1000);
                                    }
                                    var twoFactorLogin = await instaApi.TwoFactorLoginAsync(Code);
                                    if (twoFactorLogin.Succeeded)
                                    {
                                        SaveSession();
                                        HasCode = true;
                                    }
                                    else
                                    {
                                        throw new Exception("Instagram login error");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                HasCode = true;
                IsBusy = false;
                return instaApi;
            }
            return instaApi;
        }
        void LoadSession()
        {
            try
            {
                // load session file if exists
                if (File.Exists(StateFile))
                {
                    Console.WriteLine("Loading state from file");
                    using (var fs = File.OpenRead(StateFile))
                    {
                        instaApi.LoadStateDataFromString(new StreamReader(fs).ReadToEnd());
                        // in .net core or uwp apps don't use LoadStateDataFromStream
                        // use this one:
                        // _instaApi.LoadStateDataFromString(new StreamReader(fs).ReadToEnd());
                        // you should pass json string as parameter to this function.
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        async void SaveSession()
        {
            IsBusy = false;
            if (instaApi == null)
                return;
            var state = instaApi.GetStateDataAsString();
            // in .net core or uwp apps don't use GetStateDataAsStream.
            // use this one:
            // var state = _instaApi.GetStateDataAsString();
            // this returns you session as json string.
            File.WriteAllText(StateFile, state);
            var CurrentUserIdWait = await instaApi.GetCurrentUserAsync();
            while(!CurrentUserIdWait.Succeeded)
            {
                await Task.Delay(100);
            }
            CurrentUserId = CurrentUserIdWait.Value.Pk;
        }
    }

}
