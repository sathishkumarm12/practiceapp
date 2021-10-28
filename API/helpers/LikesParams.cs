using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.helpers
{
    public class LikesParams : PaginationParams
    {
        public int userId { get; set; }
        public string predicate { get; set; }
    }
}