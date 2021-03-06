#pragma checksum "F:\Work\CoreStudyIdpClient\Study.CoreWeb\Views\Home\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "06ee43e33bd1903470b13f607e8c6cce59327f7c"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Index), @"mvc.1.0.view", @"/Views/Home/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Home/Index.cshtml", typeof(AspNetCore.Views_Home_Index))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "F:\Work\CoreStudyIdpClient\Study.CoreWeb\Views\_ViewImports.cshtml"
using Study.CoreWeb;

#line default
#line hidden
#line 2 "F:\Work\CoreStudyIdpClient\Study.CoreWeb\Views\_ViewImports.cshtml"
using Study.CoreWeb.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"06ee43e33bd1903470b13f607e8c6cce59327f7c", @"/Views/Home/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"7b7afd3ec8eafc6badb73435c935ef99f3c585e5", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/oidc/oidc-client.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(0, 913, true);
            WriteLiteral(@"<style>
    .box {
        height: 200px;
        overflow: auto;
        border: 1px solid #ccc
    }

    .btn-box {
        margin-top: 10px;
    }

        .btn-box button {
            margin-right: 10px;
        }
</style>
<div class=""row btn-box"">
    <button class=""btn btn-primary"" onclick=""login()"">登陆 Implicit</button>
    <button class=""btn btn-primary"" onclick=""getuser()"">获取 User Implicit</button>
    <button class=""btn btn-primary"" onclick=""getapi()"">测试 API Implicit</button>
    <button class=""btn btn-primary"" onclick=""removeUser()"">清除 User Implicit</button>
    <button class=""btn btn-primary"" onclick=""iframeSignin()"">刷新 User Implicit</button>
</div>
<hr />
<div class=""row"">
    <h3>User:</h3>
    <div id=""userinfo"" class=""col-md-12 box"">
    </div>
</div>
<div class=""row"">
    <h3>API:</h3>
    <div id=""apiresult"" class=""col-md-12 box"">
    </div>
</div>
");
            EndContext();
            DefineSection("Scripts", async() => {
                BeginContext(930, 6, true);
                WriteLiteral("\r\n    ");
                EndContext();
                BeginContext(936, 53, false);
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "06ee43e33bd1903470b13f607e8c6cce59327f7c4650", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                EndContext();
                BeginContext(989, 3369, true);
                WriteLiteral(@"
    <script type=""text/javascript"">
        Oidc.Log.logger = window.console;
        Oidc.Log.level = Oidc.Log.DEBUG;
        var log = function (msg) { console.log(msg); }
        var testconfig = {
            authority: ""http://localhost:5002"",
            client_id: ""mvc client implicit"",
            redirect_uri: ""http://localhost:5003/callback.html"",
            response_type: ""id_token token"",
            scope: ""api1 openid email phone address profile"",
            clockSkew: 0,
            //启用静默刷新token
            silent_redirect_uri: ""http://localhost:5003/silentref.html"",
            automaticSilentRenew: true,
        };
        var mgr = new Oidc.UserManager(testconfig);
        mgr.events.addUserLoaded(function (user) {
            console.log(""user loaded"", user);
            mgr.getUser().then(function () {
                console.log(""getUser loaded user after userLoaded event fired"");
            });
        });
        mgr.events.addUserUnloaded(function () {
    ");
                WriteLiteral(@"        console.log(""user unloaded"");
        });
        mgr.events.addAccessTokenExpiring(function () {
            log(""Access token expiring..."" + new Date());
        });
        mgr.events.addSilentRenewError(function (err) {
            log(""Silent renew error: "" + err.message);
        });
        mgr.events.addUserSignedOut(function () {
            log(""User signed out of OP"");
            mgr.removeUser();
        });
        var login = function () {
            mgr.signinRedirect();
        };
        var getuser = function () {
            mgr.getUser().then(function (user) {
                log(""got user"");
                $('#userinfo').html(JSON.stringify(user));
            }).catch(function (err) {
                log(err);
            });
        };
        var removeUser = function () {
            mgr.removeUser().then(function () {
                log(""user removed"");
            }).catch(function (err) {
                log(err);
            });
        }
");
                WriteLiteral(@"        var iframeSignin = function () {
            mgr.signinSilent().then(function (user) {
                log(""signed in"", user);
            }).catch(function (err) {
                log(err);
            });
        }
        var getapi = function (token) {
            mgr.getUser().then(function (user) {
                log(""get user success"");
                document.getElementById('userinfo').innerHTML = JSON.stringify(user);
                var settings = {
                    url: 'http://localhost:5001/api/suibian',
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + user.access_token)
                        console.log(""beforeSend"", xhr)
                    },
                    success: function (res) {
                        console.log(""api result success:"", res);
                        $('#apiresult').html(JSON.stringify(res));
                    }, error: function (res) {
                      ");
                WriteLiteral(@"  console.log(""api result error:"", res);
                        $('#apiresult').html(res.responseText);
                    }
                }
                $.ajax(settings);

            }).catch(function (err) {
                log(err);
            });
        }; 
    </script>
");
                EndContext();
            }
            );
            BeginContext(4361, 1121, true);
            WriteLiteral(@"<script>
    //参数参考
    var config = {
        // if we choose to use popup window instead for logins
        //popup_redirect_uri: window.location.origin + ""/popup.html"",
        //popupWindowFeatures: ""menubar=yes,location=yes,toolbar=yes,width=1200,height=800,left=100,top=100;resizable=yes"",

        // these two will be done dynamically from the buttons clicked, but are
        // needed if you want to use the silent_renew

        // this will toggle if profile endpoint is used
        loadUserInfo: false,

        // silent renew will get a new access_token via an iframe
        // just prior to the old access_token expiring (60 seconds prior)
        silent_redirect_uri: window.location.origin + ""/silent.html"",
        automaticSilentRenew: true,

        // will revoke (reference) access tokens at logout time
        revokeAccessTokenOnSignout: true,

        // this will allow all the OIDC protocol claims to be visible in the window. normally a client app
        // wouldn't car");
            WriteLiteral("e about them or want them taking up space\r\n        filterProtocolClaims: false\r\n    };\r\n</script>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
