﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EcommerceAspNetMvc.DB;

namespace EcommerceAspNetMvc.Models
{
    public class ProfilViewModel
    {
        public Members Member { get; set; }

        public List<Addresses> Addresses { get; set; }
    }
}