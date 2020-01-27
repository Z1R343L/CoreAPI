﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreAPI.Models;
using CoreAPI.Helpers;
using System.IO;
using System.ComponentModel.DataAnnotations;
using PKHeX.Core;

namespace CoreAPI.Controllers
{
    [ApiController]
    public class LegalizeController : ControllerBase
    {
        // POST: api/Legalize
        [Route("api/[controller]")]
        [HttpPost]
        public Legalize Legalize([FromForm] [Required] IFormFile pokemon, [FromHeader] string Version)
        {
            using var memoryStream = new MemoryStream();
            pokemon.CopyTo(memoryStream);
            PKM pkm;
            byte[] data = memoryStream.ToArray();
            try
            {
                pkm = PKMConverter.GetPKMfromBytes(data);
                if (pkm == null)
                {
                    throw new System.ArgumentException("Bad data!");
                }
            }
            catch
            {
                Response.StatusCode = 400;
                return null;
            }

            if (Version == "" || Version == null)
            {
                Version = Utils.GetGameVersion(pkm).ToString();
            }
            Legalize L = new Legalize(pkm, Version);
            return L;
        }
        // POST: api/LegalityCheck 
        [Route("api/LegalityCheck")]
        [HttpPost]
        public string CheckLegality([FromForm] [Required] IFormFile pokemon, [FromHeader] string Version)
        {
            using var memoryStream = new MemoryStream();
            pokemon.CopyTo(memoryStream);
            PKM pkm;
            byte[] data = memoryStream.ToArray();
            try
            {
                pkm = PKMConverter.GetPKMfromBytes(data);
                if (pkm == null)
                {
                    throw new System.ArgumentException("Bad data!");
                }
            }
            catch
            {
                Response.StatusCode = 400;
                return null;
            }

            if (Version == "" || Version == null)
            {
                Version = Utils.GetGameVersion(pkm).ToString();
            }
            var la = new LegalityAnalysis(pkm);
            return la.Report();
        }
    }
}