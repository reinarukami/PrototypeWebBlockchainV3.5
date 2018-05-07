using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;

namespace PrototypeWebBlockchain.Models
{
    [FunctionOutput]
    public class FileDTO
    {
        [Parameter("uint256", 1)]
        public int id { get; set; }

        [Parameter("string", 2)]
        public string filehash { get; set; }

        [Parameter("string", 3)]
        public string date { get; set; }

    }
}