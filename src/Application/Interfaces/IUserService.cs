﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArquivoMate.Application.Interfaces
{
    public interface IUserService
    {
        Task<Guid?> GetUserId();
        string? GetUserName();
    }
}
