using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Data;
using Npgsql;
using PrototypeWebBlockchain.Models;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Web;
using System.Text;
using PrototypeWebBlockchain.Functions.Default;

namespace PrototypeWebBlockchain.Repository
{
    public class MemberFunction : DefaultWeb3
    {
        public Member CreateAccount(Member member)
        {
            var web3 = InitializeWeb3();

            var taskCreateAccount = web3.Personal.NewAccount.SendRequestAsync(member.password);
            taskCreateAccount.Wait();

            member.w_address = taskCreateAccount.Result;

            return member;
        }
    }
}