﻿using Common.DTO;
using DTO.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface IGoogleAuthService
    {
        Task<ResponseDTO> GoogleSignIn(GoogleAuthTokenDTO googleAuthToken);
    }
}
