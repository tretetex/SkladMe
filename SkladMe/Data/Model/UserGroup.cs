﻿using System.Collections.Generic;
using SkDAL.Base;

namespace SkDAL.Model
{
    public class UserGroup : BaseModel
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public string Background { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}