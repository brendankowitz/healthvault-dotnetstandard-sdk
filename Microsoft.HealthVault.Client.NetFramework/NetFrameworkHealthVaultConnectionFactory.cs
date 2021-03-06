﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client.NetFramework
{
    public static class HealthVaultConnectionFactory
    {
        private static readonly object s_instanceLock = new object();
        private static IHealthVaultConnectionFactory s_current;

        /// <summary>
        /// Gets or sets the invoke context used for WinForms projects. 
        /// </summary>
        /// <remarks>In Windows Forms apps, you need to assign your Form object to this property.
        /// For WPF apps, it is ignored.</remarks>
        public static ISynchronizeInvoke WinFormsInvoke { get; set; }

        /// <summary>
        /// Gets the current IHealthVaultConnectionFactory instance.
        /// </summary>
        public static IHealthVaultConnectionFactory Current
        {
            get
            {
                lock (s_instanceLock)
                {
                    if (s_current == null)
                    {
                        ClientIoc.EnsureTypesRegistered();
                        s_current = new HealthVaultConnectionFactoryInternal();
                    }

                    return s_current;
                }
            }
        }
    }
}
