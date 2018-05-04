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

namespace PrototypeWebBlockchain.Repository
{
    public class FileUpload 
    {
        private void UnlockAccount()
        {
            string senderAddress = "0xA009AB033bdBbF097970bA862d392e08Cc47C532";

            string password = "password";

            var account = new ManagedAccount(senderAddress, password);

            var web3 = new Web3(account);
        }

        private Contract initContract()
        {
            string senderAddress = "0xA009AB033bdBbF097970bA862d392e08Cc47C532";

            string password = "password";

            var account = new ManagedAccount(senderAddress, password);

            var web3 = new Web3(account);

            string abi = "[{'constant':false,'inputs':[{'name':'_id','type':'uint256'},{'name':'_fileHash','type':'string'},{'name':'_date','type':'string'}],'name':'AddFiles','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'anonymous':false,'inputs':[{'indexed':false,'name':'ID','type':'uint256'},{'indexed':false,'name':'FILEHASH','type':'string'},{'indexed':false,'name':'DATE','type':'string'}],'name':'FileUploadEvent','type':'event'}]";

            var FileContract = web3.Eth.GetContract(abi, "0x230146F7d273d9bD3E539A490D8b1BD49c35F04A");

            web3.Eth.TransactionManager.DefaultGas = 999999;

            return FileContract;
        }

        private TransactionReceipt GetReciept(string transactionHash)
        {
            string senderAddress = "0xA009AB033bdBbF097970bA862d392e08Cc47C532";

            string password = "password";

            var account = new ManagedAccount(senderAddress, password);

            var web3 = new Web3(account);

            string abi = "[{'constant':false,'inputs':[{'name':'_id','type':'uint256'},{'name':'_fileHash','type':'string'},{'name':'_date','type':'string'}],'name':'AddFiles','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'anonymous':false,'inputs':[{'indexed':false,'name':'ID','type':'uint256'},{'indexed':false,'name':'FILEHASH','type':'string'},{'indexed':false,'name':'DATE','type':'string'}],'name':'FileUploadEvent','type':'event'}]";

            var FileContract = web3.Eth.GetContract(abi, "0x230146F7d273d9bD3E539A490D8b1BD49c35F04A");

            web3.Eth.TransactionManager.DefaultGas = 90000;

            var task = web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            task.Wait();

            var task2 = web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(task.Result.TransactionHash);
            task2.Wait();


            return task2.Result;
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

        private void FiletoHash(string savepath , string savehash)
        {
            File.Move(savepath, savehash);
        }

        public void UploadFile(Image image, TransactionRepository transactionRepository, string memberID)
        {
            var FileContract = initContract();

            // Validate the uploaded image(optional)
            int imageId = transactionRepository.nextid().FirstOrDefault();
            string imagepath = ConfigurationManager.AppSettings["FileImagePath"];
            string imageName = imageId.ToString() + '_' + memberID + '_' + image.image.FileName.ToString();

            // Get the complete file path
            string fileSavePath = Path.Combine(imagepath + imageName);

            // Save the uploaded file to "UploadedFiles" folder-
            image.image.SaveAs(fileSavePath);
            image.hash = ConvertSavedFileToSha(fileSavePath);

            var TaskCheckFile =FileContract.GetFunction("AddFiles").CallAsync<bool>(imageId, "test", "test");
            TaskCheckFile.Wait();

            var TaskCheckGas = FileContract.GetFunction("AddFiles").EstimateGasAsync("0xA009AB033bdBbF097970bA862d392e08Cc47C532", null, null, imageId, image.hash.ToString(), DateTime.Now.ToString());
            TaskCheckGas.Wait();

            var TaskAddFile = FileContract.GetFunction("AddFiles").SendTransactionAsync("0xA009AB033bdBbF097970bA862d392e08Cc47C532", imageId, image.hash.ToString(), DateTime.Now.ToString());
            TaskAddFile.Wait();


            UnlockAccount();

            //var TaskGetFile = FileContract.GetFunction("getFiles").CallAsync<List<string>>();
            //TaskGetFile.Wait();

            var transaction = new TransactionData()
            {
                id = imageId,
                member_id = Int32.Parse(memberID.ToString()),
                transaction_hash = TaskAddFile.Result,
                filename = imageName,
                filepath = imagepath,
                date = DateTime.Now.ToString()
            };


            transactionRepository.Add(transaction);
        }

        public void ValidateFile(TransactionData transaction)
         {
             var FileContract = initContract();

             var TaskGetFiles = GetReciept(transaction.transaction_hash);

            // Validate the uploaded image(optional)
            string imagepath = ConfigurationManager.AppSettings["FileImagePath"];
            string imageName = transaction.filename;

            // Get the complete file path
            string fileSavePath = Path.Combine(imagepath + imageName);

            // Save the uploaded file to "UploadedFiles" folder


        }



    }
}