using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using RMERP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

namespace RMERP.Helpers
{
    public class SessionUtils
    {
        HttpRequest request;
        HttpResponse response;
        public SessionUtils(HttpRequest _request, HttpResponse _response)
        {
            request = _request;
            response = _response;
        }
        public void Login(AdminUsers user)
        {
            var claims = new List<Claim>();
            try
            {
                // Setting  
                claims.Add(new Claim(ClaimTypes.Name, user.ADM_EmailId));
                var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimPrincipal = new ClaimsPrincipal(claimIdenties);
                var authenticationManager =request.HttpContext;

                // Sign In.  
                authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(1);
                response.Cookies.Append("AdminID", Convert.ToString(user.ADM_Id), options);
                response.Cookies.Append("AdminFirstName", Convert.ToString(user.ADM_FirstName), options);
                response.Cookies.Append("FirmID", Convert.ToString(user.FRM_Id), options);
                string fullName = user.ADM_FirstName + " " + user.ADM_MiddleName + " " + user.ADM_LastName;
                response.Cookies.Append("AdminFullName", Convert.ToString(fullName), options);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int GetLoggedAdminID()
        {
            return Convert.ToInt32(request.Cookies["AdminID"]);
        }
        public int? GetLoggedFirmID()
        {
            if (request.Cookies["FirmID"] == "" || request.Cookies["FirmID"] == null)
                return null;
            else
                return Convert.ToInt32(request.Cookies["FirmID"]);
        }
        public string GetLoggedAdminFullName()
        {
            return request.Cookies["AdminFullName"];
        }
    }
}
