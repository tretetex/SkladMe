using System;
using ApiResponse = SkServ.Utils.ApiResponse;

namespace SkServ.Model
{
    public class LicenseInfo
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? BannedTo { get; set; }
        public string BannedInfo { get; set; }

        public static LicenseInfo FromJson(ApiResponse response)
        {
            return new LicenseInfo
            {
                Code = response["code"],
                Message = response["message"],
                Expires = response["expires"],
                ActiveFrom = response["active_from"],
                BannedTo = response["banned_to"],
                BannedInfo = response["banned_info"]
            };
        }
    }
}
