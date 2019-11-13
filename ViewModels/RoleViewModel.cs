﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCManukauTech.ViewModels
{
    public class RoleViewModel
    {
        [Key]
        public string RoleId { get; set; }
        
        public string RoleName { get; set; }
           
    }
}
