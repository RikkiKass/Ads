using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ads.Data;

namespace Ads.Web.Models
{
    public class AdViewModel
    {
        public Ad Ad { get; set; }

        public bool CanDelete { get; set; }
    }
}
