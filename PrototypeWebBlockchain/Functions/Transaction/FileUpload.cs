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
using System.Configuration;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts.Managed;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.ABI.Decoders;
using PrototypeWebBlockchain.Functions.Default;

namespace PrototypeWebBlockchain.Repository
{
    public class FileUpload : DefaultWeb3
    {

        private Contract InitContract()
        {

            var web3 = InitializeWeb3(HttpContext.Current.Session["Address"].ToString(), HttpContext.Current.Session["Password"].ToString());

            string abi = ConfigurationManager.AppSettings["ContractABI"];

            var FileContract = web3.Eth.GetContract(abi, ConfigurationManager.AppSettings["ContractAddress"]);

            web3.Eth.TransactionManager.DefaultGas = 999999;

            return FileContract;

        }

        private void FiletoHash(string savepath, string savehash)
        {
            File.Move(savepath, savehash);
        }

        public string ConvertSavedFileToSha(string fileSavePath)
        {

            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream input = File.Open(fileSavePath, FileMode.Open))
                {
                    var shaValue = BitConverter.ToString(sha256.ComputeHash(input)).Replace("-", "");

                    return shaValue;
                }
            }
        }

        public void UploadFile(FileClass _file, TransactionRepository _transactionRepository, string _memberID)
        {
            var FileContract = InitContract();

            // Validate the uploaded image(optional)
            int imageId = _transactionRepository.nextid().FirstOrDefault();
            string imagepath = ConfigurationManager.AppSettings["FileImagePath"];
            string imageName = imageId.ToString() + '_' + _memberID + '_' + _file.image.FileName.ToString();

            // Get the complete file path
            string fileSavePath = Path.Combine(imagepath + imageName);

            // Save the uploaded file to "UploadedFiles" folder-
            _file.image.SaveAs(fileSavePath);
            _file.hash = ConvertSavedFileToSha(fileSavePath);

            var TaskAddFile = FileContract.GetFunction("AddFiles").SendTransactionAsync(HttpContext.Current.Session["Address"].ToString(), imageId, HttpContext.Current.Session["Address"].ToString(), _file.hash.ToString(), DateTime.Now.ToString());
            TaskAddFile.Wait();


            var transaction = new TransactionData()
            {
                id = imageId,
                member_id = Int32.Parse(_memberID.ToString()),
                filename = imageName,
                filepath = imagepath,
                date = DateTime.Now.ToString()
            };


            _transactionRepository.Add(transaction);
        }

    }
}