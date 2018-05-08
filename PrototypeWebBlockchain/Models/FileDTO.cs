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
        [Parameter("uint256","id", 1)]
        public int id { get; set; }

        [Parameter("uint256","sender", 2)]
        public int sender { get; set; }

        [Parameter("string", "fileHash", 3)]
        public string filehash { get; set; }

        [Parameter("string", "date", 4)]
        public string date { get; set; }

    }
}