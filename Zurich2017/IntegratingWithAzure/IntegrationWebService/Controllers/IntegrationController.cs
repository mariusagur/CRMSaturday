﻿using IntegrationWebService.Factories;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.ServiceModel.Description;
using System.Web;
using System.Web.Mvc;

namespace IntegrationWebService.Controllers
{
    [Authorize]
    public class IntegrationController : Controller
    {
        private string OrganizationUri = ConfigurationManager.AppSettings["ida:OrganizationUri"];

        // GET: Integration
        public string Index()
        {
            var authResult = AuthFactory.GetAuthContext().AcquireToken(OrganizationUri, AuthFactory.GetCredentials());
            var crmUri = new Uri(OrganizationUri + "/XRMServices/2011/Organization.svc/web?SdkClientVersion=8.2");
            using (var service = new OrganizationWebProxyClient(crmUri, false))
            {
                service.HeaderToken = authResult.AccessToken;
                var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
                var accounts = service.RetrieveMultiple(qe);

                return JsonConvert.SerializeObject(accounts.Entities);
            }
        }
    }
}