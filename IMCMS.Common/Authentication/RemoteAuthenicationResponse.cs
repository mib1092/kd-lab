using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMCMS.Common.Authentication
{
    public enum RemoteAuthenicationResponse
    {
        Valid,
        Invalid,
        IPLocked,
        UnableToComplete,
        NotConfigured
    }
}
