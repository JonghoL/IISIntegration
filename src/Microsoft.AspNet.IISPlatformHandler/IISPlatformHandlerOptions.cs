﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.Http.Authentication;

namespace Microsoft.AspNet.IISPlatformHandler
{
    public class IISPlatformHandlerOptions
    {
        /// <summary>
        /// If true the authentication middleware alter the request user coming in and respond to generic challenges.
        /// If false the authentication middleware will only provide identity and respond to challenges when explicitly indicated
        /// by the AuthenticationScheme.
        /// </summary>
        public bool AutomaticAuthentication { get; set; } = true;

        /// <summary>
        /// Additional information about the authentication type which is made available to the application.
        /// </summary>
        public IList<AuthenticationDescription> AuthenticationDescriptions { get; } = new List<AuthenticationDescription>()
        {
            new AuthenticationDescription()
            {
                AuthenticationScheme = IISPlatformHandlerDefaults.Negotiate,
                DisplayName = IISPlatformHandlerDefaults.Negotiate
            },
            new AuthenticationDescription()
            {
                AuthenticationScheme = IISPlatformHandlerDefaults.Ntlm,
                DisplayName = IISPlatformHandlerDefaults.Ntlm
            }
        };
    }
}