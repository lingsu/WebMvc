﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lxs.Core.Domain.Users;

namespace Lxs.Core
{
    public interface IWorkContext
    {
        User CurrentUser { get; set; }
        bool IsAdmin { get; set; }
    }
}
