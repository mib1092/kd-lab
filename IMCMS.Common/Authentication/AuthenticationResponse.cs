using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Common.Authentication
{
    public enum AuthenticationResponse
    {
        Authorized,
        Unauthorized,
        AccountLocked,
        IPLocked,
		RemoteIPLocked,
		LockedOut,
		LocalSuccess,
		ImagemakersSuccess
    }
}
